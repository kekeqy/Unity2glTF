using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Uinty2glTF
{
    internal static class StreamExtensions
    {
        public static void Align(this Stream stream, byte pad = 0)
        {
            var count = 3 - ((stream.Position - 1) & 3);
            while (count != 0)
            {
                stream.WriteByte(pad);
                count--;
            }
        }
    }
}