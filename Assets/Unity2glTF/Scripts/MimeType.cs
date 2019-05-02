using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Uinty2glTF
{
    internal class MimeType
    {
        public static string ToFileExtension(string mimeType)
        {
            switch (mimeType)
            {
                case "image/png":
                    return ".png";
                case "image/jpeg":
                    return ".jpg";
                case "image/vnd-ms.dds":
                    return ".dds";
            }

            throw new Exception(string.Format("不支持的mime类型： {0}", mimeType));
        }

        public static string FromFileExtension(string fileExtension)
        {
            switch (fileExtension.ToLower())
            {
                case ".png":
                    return "image/png";
                case ".jpg":
                case ".jpeg":
                    return "image/jpeg";
                case ".dds":
                    return "image/vnd-ms.dds";
            }

            throw new Exception(string.Format("不支持的文件扩展名：{0}", fileExtension));
        }
    }
}