﻿#if UNITY_EDITOR
using UnityEngine;
using System.Collections;

namespace Uinty2glTF
{
    public class GlTF_ColorOrTexture : GlTF_Writer
    {
        public GlTF_ColorOrTexture() { }
        public GlTF_ColorOrTexture(string n) { name = n; }
    }
}
#endif