using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace GOEngine.Implement
{
    static class Vector3Extension
    {
        public static bool EqualsVector3(this Vector3 vec1, ref Vector3 vec2)
        {
            return vec1.x == vec2.x && vec1.z == vec2.z;
        }
    }
}
