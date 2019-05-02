﻿#if UNITY_EDITOR
using UnityEngine;
using System.Collections;

namespace Uinty2glTF
{
    public class GlTF_MaterialColor : GlTF_ColorOrTexture
    {
        public GlTF_MaterialColor(string n, Color c) { name = n; color = new GlTF_ColorRGBA(name, c); }
        public GlTF_ColorRGBA color = new GlTF_ColorRGBA("diffuse");
        public override void Write()
        {
            //		Indent();		jsonWriter.Write ("\"" + name + "\": ");
            color.Write();
        }
    }
}
#endif