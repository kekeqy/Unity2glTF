#if UNITY_EDITOR
using UnityEngine;
using System.Collections;

namespace Uinty2glTF
{
    public class GlTF_DirectionalLight : GlTF_Light
    {
        public override void Write()
        {
            color.Write();
        }
    }
}
#endif