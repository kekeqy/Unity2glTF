#if UNITY_EDITOR
using UnityEngine;
using System.Collections;

namespace Uinty2glTF
{
    public class GlTF_Camera : GlTF_Writer
    {
        public string type;// should be enum ": "perspective"
    }
}
#endif