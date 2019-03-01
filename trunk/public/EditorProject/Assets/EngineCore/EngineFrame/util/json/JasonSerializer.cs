using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace EngineCore
{
	public class Vector3Jason
	{
        public static string SerilizeVector3(object obj)
        {
            Vector3 vec = (Vector3)obj;
            return vec.x + "|" + vec.y + "|" + vec.z;
        }

        public static object DserializeVector3(string str)
        {
            string[] vs = JsonUtil.StringToStringArray(str);
            Vector3 vc = Vector3.zero;
            if(vs.Length == 3)
            {
                vc.x = float.Parse(vs[0]);
                vc.y = float.Parse(vs[1]);
                vc.z = float.Parse(vs[2]);
            }
            return vc;
        }
	}

    public class QuaternionJason
    {
        public static string SerilizeQuaternion(object obj)
        {
            Quaternion vec = (Quaternion)obj;
            return vec.w + "|" + vec.x + "|" + vec.y + "|" + vec.z;
        }

        public static object DserializeQuaternion(string str)
        {
            string[] vs = JsonUtil.StringToStringArray(str);
            Quaternion vc = Quaternion.identity;
            if (vs.Length == 4)
            {
                vc.w = float.Parse(vs[0]);
                vc.x = float.Parse(vs[1]);
                vc.y = float.Parse(vs[2]);
                vc.z = float.Parse(vs[3]);
            }
            return vc;
        }
    }
}
