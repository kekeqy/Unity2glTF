using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Uinty2glTF
{
    internal static class Tools
    {
        public static int GetFileLength(string path)
        {
            return checked((int)new FileInfo(path).Length);
        }

        public static int Align(int value)
        {
            return value + 3 - ((value - 1) & 3);
        }

        public static bool IsBase64(string uri)
        {
            return uri.StartsWith("data:");
        }
    }
}