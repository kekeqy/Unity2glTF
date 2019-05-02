﻿#if UNITY_EDITOR
using UnityEngine;
using System.Collections;

namespace Uinty2glTF
{
    public class GlTF_Image : GlTF_Writer
    {
        public string uri { get; set; }

        public static string GetNameFromObject(Object o)
        {
            return "image_" + GlTF_Writer.GetNameFromObject(o, true);
        }

        public override void Write()
        {
            Indent(); jsonWriter.Write("{\n");
            IndentIn();
            Indent(); jsonWriter.Write("\"uri\": \"" + uri + "\"\n");
            IndentOut();
            Indent(); jsonWriter.Write("}");
        }
    }
}
#endif