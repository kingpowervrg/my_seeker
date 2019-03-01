using UnityEngine;
using System;
using System.Text;
using System.Security.Cryptography;
using System.IO;
using GOEngine;

namespace EngineCore
{
    static class SysUtil
    {
        public static bool isDebug = true;

        public static string CreateGUID()
        {
            return Guid.NewGuid().ToString("N");
        }

        public static void DebugBreak()
        {
            if (isDebug)
                DebugUtil.Break();
        }

        public static void Assert(bool val, string msg)
        {
            if (isDebug && !val)
            {
                DebugUtil.Log("assert failed:\n" + msg);
                DebugUtil.Break();
            }
        }

        public static int MilliSecToFps(long time)
        {
            return 1000 / (int)time;
        }

        public static long FpsToMilliSec(int fps)
        {
            return 1000 / fps;
        }

        public static long TickToMilliSec(long tick)
        {
            return tick / (10 * 1000);
        }

        public static long MilliSecToTick(long time)
        {
            return time * 10 * 1000;
        }

        public static float MilliSecToSec(long ms)
        {
            return ((float)ms) / 1000;
        }

        public static long SecToMilliSec(float s)
        {
            return (long)(s * 1000);
        }

        public static string GetAddr(string ip, int port)
        {
            return ip + ":" + port;
        }

        public static string GetMD5Str(string str)
        {
            MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
            byte[] encryptedBytes = md5.ComputeHash(Encoding.ASCII.GetBytes(str));
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < encryptedBytes.Length; i++)
            {
                sb.AppendFormat("{0:x2}", encryptedBytes[i]);
            }
            return sb.ToString();
        }

        public static string GetMD5Str(Stream stream)
        {
            MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
            byte[] encryptedBytes = md5.ComputeHash(stream);
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < encryptedBytes.Length; i++)
            {
                sb.AppendFormat("{0:x2}", encryptedBytes[i]);
            }
            return sb.ToString();
        }

        public static void DrawDebugDimension(Vector3 dim, MathUtil.BoundOriginType origin,
                                            Vector3 pos, Color col)
        {
            if (isDebug)
            {
                Bounds bd = MathUtil.DimensionToBound(dim, origin);
                Matrix4x4 mtx = Matrix4x4.identity;
                mtx.SetTRS(pos, Quaternion.identity, Vector3.one);
                DrawDebugBounds(bd, mtx, col);
            }
        }

        public static void DrawDebugBounds(Bounds bound, Matrix4x4 mtx, Color col)
        {
            if (isDebug)
            {
                Vector3 src = Vector3.zero;
                Vector3 dst = Vector3.zero;

                //bottom
                src.x = bound.min.x;
                src.y = bound.min.y;
                src.z = bound.min.z;
                dst.x = bound.max.x;
                dst.y = bound.min.y;
                dst.z = bound.min.z;
                src = mtx.MultiplyPoint(src);
                dst = mtx.MultiplyPoint(dst);
                DebugUtil.DrawLine(src, dst, col);

                src.x = bound.min.x;
                src.y = bound.min.y;
                src.z = bound.min.z;
                dst.x = bound.min.x;
                dst.y = bound.min.y;
                dst.z = bound.max.z;
                src = mtx.MultiplyPoint(src);
                dst = mtx.MultiplyPoint(dst);
                DebugUtil.DrawLine(src, dst, col);

                src.x = bound.max.x;
                src.y = bound.min.y;
                src.z = bound.max.z;
                dst.x = bound.max.x;
                dst.y = bound.min.y;
                dst.z = bound.min.z;
                src = mtx.MultiplyPoint(src);
                dst = mtx.MultiplyPoint(dst);
                DebugUtil.DrawLine(src, dst, col);

                src.x = bound.max.x;
                src.y = bound.min.y;
                src.z = bound.max.z;
                dst.x = bound.min.x;
                dst.y = bound.min.y;
                dst.z = bound.max.z;
                src = mtx.MultiplyPoint(src);
                dst = mtx.MultiplyPoint(dst);
                DebugUtil.DrawLine(src, dst, col);

                //top
                src.x = bound.max.x;
                src.y = bound.max.y;
                src.z = bound.max.z;
                dst.x = bound.max.x;
                dst.y = bound.max.y;
                dst.z = bound.min.z;
                src = mtx.MultiplyPoint(src);
                dst = mtx.MultiplyPoint(dst);
                DebugUtil.DrawLine(src, dst, col);

                src.x = bound.max.x;
                src.y = bound.max.y;
                src.z = bound.max.z;
                dst.x = bound.min.x;
                dst.y = bound.max.y;
                dst.z = bound.max.z;
                src = mtx.MultiplyPoint(src);
                dst = mtx.MultiplyPoint(dst);
                DebugUtil.DrawLine(src, dst, col);

                src.x = bound.min.x;
                src.y = bound.max.y;
                src.z = bound.min.z;
                dst.x = bound.max.x;
                dst.y = bound.max.y;
                dst.z = bound.min.z;
                src = mtx.MultiplyPoint(src);
                dst = mtx.MultiplyPoint(dst);
                DebugUtil.DrawLine(src, dst, col);

                src.x = bound.min.x;
                src.y = bound.max.y;
                src.z = bound.min.z;
                dst.x = bound.min.x;
                dst.y = bound.max.y;
                dst.z = bound.max.z;
                src = mtx.MultiplyPoint(src);
                dst = mtx.MultiplyPoint(dst);
                DebugUtil.DrawLine(src, dst, col);

                //side
                src.x = bound.min.x;
                src.y = bound.min.y;
                src.z = bound.min.z;
                dst.x = bound.min.x;
                dst.y = bound.max.y;
                dst.z = bound.min.z;
                src = mtx.MultiplyPoint(src);
                dst = mtx.MultiplyPoint(dst);
                DebugUtil.DrawLine(src, dst, col);

                src.x = bound.max.x;
                src.y = bound.min.y;
                src.z = bound.min.z;
                dst.x = bound.max.x;
                dst.y = bound.max.y;
                dst.z = bound.min.z;
                src = mtx.MultiplyPoint(src);
                dst = mtx.MultiplyPoint(dst);
                DebugUtil.DrawLine(src, dst, col);

                src.x = bound.max.x;
                src.y = bound.max.y;
                src.z = bound.max.z;
                dst.x = bound.max.x;
                dst.y = bound.min.y;
                dst.z = bound.max.z;
                src = mtx.MultiplyPoint(src);
                dst = mtx.MultiplyPoint(dst);
                DebugUtil.DrawLine(src, dst, col);

                src.x = bound.min.x;
                src.y = bound.min.y;
                src.z = bound.max.z;
                dst.x = bound.min.x;
                dst.y = bound.max.y;
                dst.z = bound.max.z;
                src = mtx.MultiplyPoint(src);
                dst = mtx.MultiplyPoint(dst);
                DebugUtil.DrawLine(src, dst, col);
            }
        }

        public static KeyCode TransKeyToKeyCode(int key)
        {
            return (KeyCode)key;
        }

        public static int TransKeyCodeToKey(KeyCode code)
        {
            return (int)code;
        }

        //lower str, then rm all space
        public static string GetPureStr(string txt)
        {
            string pt = txt.ToLower();
            pt = pt.Trim();
            pt = pt.Replace('\t', '_');
            pt = pt.Replace(' ', '_');
            pt = pt.Replace('-', '_');
            return pt;
        }

        public static bool StrPureEqual(string txt1, string txt2)
        {
            string pt1 = GetPureStr(txt1);
            string pt2 = GetPureStr(txt2);
            return pt1 == pt2;
        }

        public static bool DetectEnterKeyInStr(string str)
        {
            if (str == null || str == string.Empty) return false;

            return str[str.Length - 1] == '\n';
        }

        public static string RmEnterKeyInStr(string str)
        {
            if (str == null || str == string.Empty) return str;

            string nStr = str;
            while (nStr.Length > 0)
            {
                if (nStr[nStr.Length - 1] == '\n' || nStr[nStr.Length - 1] == '\r')
                    nStr = nStr.Remove(nStr.Length - 1);
                else
                    break;
            }

            return nStr;
        }

        /*
    public static void ClearGuiContent(GUIContent content)
    {
        content.image = null;
        content.text = string.Empty;
        content.tooltip = string.Empty;
    }
    */

        public static bool IsEqual(float f1, float f2)
        {
            return System.Math.Abs(f1 - f2) < 0.00001f;
        }

        public static bool IsEqualXZ(Vector3 v1, Vector3 v2)
        {
            return IsEqual(v1.x, v2.x) && IsEqual(v1.z, v2.z);
        }

        public static bool IsEqual(Vector3 v1, Vector3 v2)
        {
            return IsEqual(v1.x, v2.x) && IsEqual(v1.y, v2.y) && IsEqual(v1.z, v2.z);
        }

        public static GameObject CreateEmptyGameObj(string name)
        {
            GameObject go = new GameObject();
            go.name = name;
            UnityEngine.Object.DontDestroyOnLoad(go);
            return go;
        }

        public static bool RaycastPlane(Vector3 from, Vector3 dir, Plane plane, out Vector3 pos)
        {
            Ray r = new Ray(from, dir);
            return RaycastPlane(r, plane, out pos);
        }

        public static bool RaycastPlane(Ray ray, Plane plane, out Vector3 pos)
        {
            float dist;
            pos = Vector3.zero;
            bool result = plane.Raycast(ray, out dist);
            if (result)
            {
                pos = ray.origin + ray.direction * dist;
            }
            return result;
        }

        public static Vector3 MakeOrientation(Vector3 target, Vector3 from)
        {
            Vector3 diff = target - from;
            diff.y = 0;
            return diff.normalized;
        }
    }
}
