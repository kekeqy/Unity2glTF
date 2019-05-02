﻿#if UNITY_EDITOR
using UnityEngine;
using System.Collections;

namespace Uinty2glTF
{
    public class GlTF_Translation : GlTF_Vector3
    {
        public GlTF_Translation(Vector3 v)
        {
            if (convertRightHanded)
                convertVector3LeftToRightHandedness(ref v);

            items = new float[] { v.x, v.y, v.z };
        }
        public override void Write()
        {
            Indent(); jsonWriter.Write("\"translation\": [ ");
            WriteVals();
            jsonWriter.Write("]");
        }
    }
}
#endif