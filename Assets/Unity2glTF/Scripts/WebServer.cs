using System;
using System.IO;
using System.Net;
using System.Text;
using System.Web;
using System.Collections.Generic;

namespace Uinty2glTF
{
    public class WebServer
    {
        private static Dictionary<string, string> mimeTypes = new Dictionary<string, string>();
        static WebServer()
        {
            mimeTypes.Add(".html", "text/html");
            mimeTypes.Add(".htm", "text/html");
            mimeTypes.Add(".ico", "image/vnd.microsoft.icon");
            mimeTypes.Add(".js", "text/javascript");
            mimeTypes.Add(".jsx", "text/javascript");
            mimeTypes.Add(".png", "image/png");
            mimeTypes.Add(".svg", "image/svg+xml");
            mimeTypes.Add(".woff", "font/woff");
            mimeTypes.Add(".css", "text/css");
        }
        private static string GetMime(string ext)
        {
            if (mimeTypes.ContainsKey(ext)) return mimeTypes[ext];
            return "application/octet-stream";
        }

        private HttpListener listener;
        public int Port { get; set; }
        public string Folder { get; set; }
        public string Prefix
        {
            get { return string.Format("http://localhost:{0}/", Port); }
        }
        public WebServer(int port, string folder)
        {
            Port = port;
            Folder = folder;
            try
            {
                listener = new HttpListener();
                listener.Prefixes.Add(Prefix);
                listener.Start();
                listener.BeginGetContext(onRequest, null);
            }
            catch { }
        }
        private void onRequest(IAsyncResult ar)
        {
            //继续监听
            listener.BeginGetContext(onRequest, null);

            HttpListenerContext context = listener.EndGetContext(ar);
            context.Response.AddHeader("Cache-Control", "no-cache");
            context.Response.AppendHeader("Access-Control-Allow-Origin", "*");//支持跨域访问
            try
            {
                var path = Path.Combine(Folder, UrlDecode(context.Request.Url.PathAndQuery.Substring(1)));
                var questionMarkIndex = path.IndexOf("?");
                if (questionMarkIndex != -1)
                {
                    path = path.Substring(0, questionMarkIndex);
                }
                //var hashIndex = path.IndexOf("#");
                //if (hashIndex != -1)
                //{
                //    path = path.Substring(0, hashIndex);
                //}
                string ext = Path.GetExtension(path).ToLower();
                if (string.IsNullOrEmpty(ext))
                {
                    ext = ".html";
                    path += "index.html";
                }
                context.Response.ContentType = GetMime(ext);
                byte[] buffer = File.ReadAllBytes(path);
                context.Response.ContentLength64 = buffer.Length;
                context.Response.OutputStream.Write(buffer, 0, buffer.Length);
                context.Response.OutputStream.Close();
            }
            catch
            {
                context.Response.StatusCode = 404;
                context.Response.Close();
            }
        }
        private string UrlDecode(string value)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(value);
            int count = bytes.Length;
            int decodedBytesCount = 0;
            byte[] decodedBytes = new byte[count];

            for (int i = 0; i < count; i++)
            {
                int pos = i;
                byte b = bytes[pos];

                if (b == '+')
                {
                    b = (byte)' ';
                }
                else if (b == '%' && i < count - 2)
                {
                    int h1 = HexToInt((char)bytes[pos + 1]);
                    int h2 = HexToInt((char)bytes[pos + 2]);

                    if (h1 >= 0 && h2 >= 0)
                    {     // valid 2 hex chars
                        b = (byte)((h1 << 4) | h2);
                        i += 2;
                    }
                }

                decodedBytes[decodedBytesCount++] = b;
            }

            if (decodedBytesCount < decodedBytes.Length)
            {
                byte[] newDecodedBytes = new byte[decodedBytesCount];
                Array.Copy(decodedBytes, newDecodedBytes, decodedBytesCount);
                decodedBytes = newDecodedBytes;
            }

            return Encoding.UTF8.GetString(decodedBytes);
        }
        private int HexToInt(char h)
        {
            return (h >= '0' && h <= '9') ? h - '0' :
            (h >= 'a' && h <= 'f') ? h - 'a' + 10 :
            (h >= 'A' && h <= 'F') ? h - 'A' + 10 :
            -1;
        }
    }
}
