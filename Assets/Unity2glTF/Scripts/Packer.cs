using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Uinty2glTF
{
    internal class Packer
    {
        public static void Pack(string inputFilePath, string outputFilePath)
        {
            var inputDirectoryPath = Path.GetDirectoryName(inputFilePath);//文件根目录

            JObject json;
            using (var jsonStream = File.OpenRead(inputFilePath))
            using (var jsonStreamReader = new StreamReader(jsonStream))
            using (var jsonTextReader = new JsonTextReader(jsonStreamReader))
            {
                json = (JObject)JToken.ReadFrom(jsonTextReader);
            }

            var position = 0;

            List<string> views = new List<string>();

            var buffers = (JArray)json["buffers"];
            var bufferViews = (JArray)json["bufferViews"];
            var images = (JArray)json["images"];

            if (buffers != null)
            {
                for (var index = buffers.Count - 1; index >= 0; index--)
                {
                    var buffer = (JObject)buffers[index];
                    var uri = (string)buffer["uri"];
                    if (uri != null && !Tools.IsBase64(uri))
                    {
                        foreach (JObject bufferView in bufferViews)
                        {
                            var bufferIndex = (int)bufferView["buffer"];
                            if (bufferIndex == index)
                            {
                                bufferView["buffer"] = -1;

                                var byteOffset = (int?)bufferView["byteOffset"] ?? 0;
                                bufferView.SetValue("byteOffset", position + byteOffset, 0);
                            }
                        }

                        var filePath = Path.Combine(inputDirectoryPath, uri);

                        views.Add(filePath);
                        var fileLength = Tools.GetFileLength(filePath);

                        position += fileLength;
                        position = Tools.Align(position);

                        buffers.RemoveAt(index);
                    }
                }
            }

            if (images != null)
            {
                foreach (JObject image in images)
                {
                    var uri = (string)image["uri"];
                    if (uri != null && !Tools.IsBase64(uri))
                    {
                        var filePath = Path.Combine(inputDirectoryPath, uri);
                        views.Add(filePath);
                        var fileLength = Tools.GetFileLength(filePath);

                        image.Remove("uri");
                        image["bufferView"] = bufferViews.Count;
                        image["mimeType"] = MimeType.FromFileExtension(Path.GetExtension(uri));

                        position = Tools.Align(position);

                        JObject bufferView=new JObject();
                        bufferView["buffer"]=-1;
                        bufferView["byteLength"] = fileLength;
                        bufferView.SetValue("byteOffset", position, 0);
                        bufferViews.Add(bufferView);

                        position += fileLength;
                    }
                }
            }

            if (views.Count != 0)
            {
                if (buffers == null)
                {
                    json["buffers"] = new JArray();
                }

                JObject item = new JObject();
                item["byteLength"] = position;
                buffers.Insert(0, item);

                foreach (var bufferView in bufferViews)
                {
                    var bufferIndex = (int)bufferView["buffer"];
                    bufferView["buffer"] = bufferIndex + 1;
                }
            }

            using (var fileStream = File.Create(outputFilePath))
            using (var binaryWriter = new BinaryWriter(fileStream))
            {
                binaryWriter.Write(Binary.Magic);
                binaryWriter.Write(Binary.Version);

                var chunksPosition = binaryWriter.BaseStream.Position;

                binaryWriter.Write(0U); // length

                var jsonChunkPosition = binaryWriter.BaseStream.Position;

                binaryWriter.Write(0U); // json chunk length
                binaryWriter.Write(Binary.ChunkFormatJson);

                var streamWriter = new StreamWriter(binaryWriter.BaseStream, new UTF8Encoding(false, true), 1024);
                var jsonTextWriter = new JsonTextWriter(streamWriter);
                json.WriteTo(jsonTextWriter);
                jsonTextWriter.Flush();

                binaryWriter.BaseStream.Align(0x20);
                var jsonChunkLength = checked((uint)(binaryWriter.BaseStream.Length - jsonChunkPosition)) - Binary.ChunkHeaderLength;

                binaryWriter.BaseStream.Seek(jsonChunkPosition, SeekOrigin.Begin);
                binaryWriter.Write(jsonChunkLength);

                if (views.Count != 0)
                {
                    binaryWriter.BaseStream.Seek(0, SeekOrigin.End);
                    var binChunkPosition = binaryWriter.BaseStream.Position;

                    binaryWriter.Write(0); // bin chunk length
                    binaryWriter.Write(Binary.ChunkFormatBin);

                    foreach (var fileName in views)
                    {
                        var viewBytes = File.ReadAllBytes(fileName);
                        binaryWriter.BaseStream.Align();
                        binaryWriter.Write(viewBytes);
                    }

                    binaryWriter.BaseStream.Align(0x20);
                    var binChunkLength = checked((uint)(binaryWriter.BaseStream.Length - binChunkPosition)) - Binary.ChunkHeaderLength;

                    binaryWriter.BaseStream.Seek(binChunkPosition, SeekOrigin.Begin);
                    binaryWriter.Write(binChunkLength);
                }

                var length = checked((uint)binaryWriter.BaseStream.Length);

                binaryWriter.BaseStream.Seek(chunksPosition, SeekOrigin.Begin);
                binaryWriter.Write(length);

                jsonTextWriter.Close();
                streamWriter.Dispose();
            }
        }
    }
}