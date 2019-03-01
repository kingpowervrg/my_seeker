using UnityEngine;
using System;
using GOEngine.Implement;

namespace EngineCore
{
    public static class MathUtil
    {
        public enum BoundOriginType
        {
            BOT_CENTER = 0,
            BOT_BOTTOM_CENTER = 1,
            BOT_LEFT_BOTTOM = 2,
        }

        public const float FloatEpsilon = 0.0001f;

        public const int MASK_PIXEL_ARR_LEN_DEFAULT = 32 * 32;
        public static Color[] maskPixels = new Color[MASK_PIXEL_ARR_LEN_DEFAULT];

        /// <summary>
        /// 计算两个向量的距离
        /// </summary>
        /// <param name="a"></param>
        /// <param name="to">目标向量</param>
        /// <remarks>如果仅需要比较距离，请使用SqrDistanceTo</remarks>
        /// <returns>距离</returns>
        public static float DistanceTo(Vector3 a, Vector3 to)
        {
            return (to - a).magnitude;
        }

        /// <summary>
        /// 计算两个向量距离的平方，如果仅需要比较距离，请使用这个，性能较高
        /// </summary>
        /// <param name="a"></param>
        /// <param name="to">目标向量</param>
        /// <returns>距离</returns>
        public static float SqrDistanceTo(Vector3 a, Vector3 to)
        {
            return (to - a).sqrMagnitude;
        }

        //separating character is default to '\''
        public static Vector3 ToVector3(string str)
        {
            return ToVector3(str, ',');
        }
        public static Vector3 ToVector3(string str, char separator)
        {
            SysUtil.Assert(str != null, "null str to vector3");

            string[] vals = str.Split(separator);
            SysUtil.Assert(vals.Length == 3, "bad format str \"" + str + "\" to vector3");

            Vector3 vec;
            vec.x = Convert.ToSingle(vals[0]);
            vec.y = Convert.ToSingle(vals[1]);
            vec.z = Convert.ToSingle(vals[2]);

            return vec;
        }
        public static string Vector3ToString(Vector3 vec)
        {
            return string.Format("{0},{1},{2}", vec.x, vec.y, vec.z);
        }
        public static Quaternion ToQuaternion(string str)
        {
            SysUtil.Assert(str != null, "null str to quaternion");

            string[] vals = str.Split(',');
            SysUtil.Assert(vals.Length == 4, "bad format str \"" + str + "\" to quaternion");

            Quaternion quat = new Quaternion();
            quat.x = Convert.ToSingle(vals[0]);
            quat.y = Convert.ToSingle(vals[1]);
            quat.z = Convert.ToSingle(vals[2]);
            quat.z = Convert.ToSingle(vals[3]);
            return quat;
        }
        public static string QuatToString(Quaternion quat)
        {
            return string.Format("{0},{1},{2},{3}", quat.x, quat.y, quat.z, quat.w); ;
        }

        public static bool FloatEqual(float f0, float f1)
        {
            //return Mathf.Abs(f0 - f1) <= FloatEpsilon;
            return Mathf.Approximately(f0, f1);
        }

        public static float DegreeToRadian(float degree)
        {
            return degree * Mathf.Deg2Rad;
        }

        public static float RadianToDegree(float radian)
        {
            return radian * Mathf.Rad2Deg;
        }

        public static float GetFBXFaceDir(bool faceLeft)
        {
            if (faceLeft)
                return 0;
            else
                return 180;
        }

        public static float GetColFaceDir(bool faceLeft)
        {
            if (faceLeft)
                return 180;
            else
                return 0;
        }

        //for mouse pos, the left-bottom is (0, 0),
        //for gui pos, the left-top is (0, 0)
        public static Vector3 MousePosToGUIPos(Vector3 pos)
        {
            Vector3 newPos = pos;
            newPos.y = Screen.height - pos.y;
            return newPos;
        }

        public static Bounds DimensionToBound(Vector3 dim, BoundOriginType type)
        {
            Vector3 center = Vector3.zero;
            switch (type)
            {
                case BoundOriginType.BOT_CENTER:
                    center = Vector3.zero;
                    break;
                case BoundOriginType.BOT_BOTTOM_CENTER:
                    center = Vector3.zero;
                    center.y = dim.y / 2;
                    break;
                case BoundOriginType.BOT_LEFT_BOTTOM:
                    center = dim / 2;
                    break;
                default:
                    //Logger.GetFile( GOEngine.LogFile.Global ).LogWarning("invalid bound origin type");
                    break;
            }

            return new Bounds(center, dim);
        }

        public static bool RaycastBound(Ray ray, Bounds bound)
        {
            return bound.IntersectRay(ray);
        }
        public static bool RaycastBound(Ray ray, Bounds bound, out float distance)
        {
            return bound.IntersectRay(ray, out distance);
        }

        public static bool BoundIntersect(Bounds bound0, Bounds bound1)
        {
            if (bound0.min.x > bound1.max.x) return false;
            if (bound0.min.y > bound1.max.y) return false;
            if (bound0.min.z > bound1.max.z) return false;

            if (bound0.max.x < bound1.min.x) return false;
            if (bound0.max.y < bound1.min.y) return false;
            if (bound0.max.z < bound1.min.z) return false;

            return true;
        }

        public static bool BoundIntersect(Bounds bound0, Bounds bound1, ref Bounds intersectBounds)
        {
            Vector3 intersectMin = Vector3.zero;
            intersectMin.x = Math.Max(bound0.min.x, bound1.min.x);
            intersectMin.y = Math.Max(bound0.min.y, bound1.min.y);
            intersectMin.z = Math.Max(bound0.min.z, bound1.min.z);

            Vector3 intersectMax = Vector3.zero;
            intersectMax.x = Math.Min(bound0.max.x, bound1.max.x);
            intersectMax.y = Math.Min(bound0.max.y, bound1.max.y);
            intersectMax.z = Math.Min(bound0.max.z, bound1.max.z);

            if (intersectMax.x <= intersectMin.x) return false;
            if (intersectMax.y <= intersectMin.y) return false;
            if (intersectMax.z <= intersectMin.z) return false;

            Vector3 center = Vector3.zero;
            center.x = (intersectMin.x + intersectMax.x) / 2;
            center.y = (intersectMin.y + intersectMax.y) / 2;
            center.z = (intersectMin.z + intersectMax.z) / 2;

            Vector3 size = Vector3.zero;
            size.x = intersectMax.x - intersectMin.x;
            size.y = intersectMax.y - intersectMin.y;
            size.z = intersectMax.z - intersectMin.z;

            intersectBounds = new Bounds(center, size);
            return true;
        }

        public static bool BoundContainsPoint(Bounds bound, Vector3 pos)
        {
            return bound.Contains(pos);
        }

        public static bool Bound0ContainsBound1(Bounds bound0, Bounds bound1)
        {
            if (bound1.min.x >= bound0.min.x && bound1.max.x <= bound0.max.x
                && bound1.min.y >= bound0.min.y && bound1.max.y <= bound0.max.y
                && bound1.min.z >= bound0.min.z && bound1.max.z <= bound0.max.z)
                return true;

            return false;
        }

        public static Bounds TransformBounds(Transform tm, Bounds bd)
        {
            if (tm == null) return bd;

            Vector3[] posList = new Vector3[8];
            Vector3 pos = bd.min;
            posList[0] = tm.TransformPoint(pos);

            pos.z = bd.max.z;
            posList[1] = tm.TransformPoint(pos);

            pos.y = bd.max.y;
            posList[2] = tm.TransformPoint(pos);

            pos.z = bd.min.z;
            posList[3] = tm.TransformPoint(pos);

            pos.x = bd.max.x;
            posList[4] = tm.TransformPoint(pos);

            pos.z = bd.max.z;
            posList[5] = tm.TransformPoint(pos);

            pos.y = bd.min.y;
            posList[6] = tm.TransformPoint(pos);

            pos.z = bd.min.z;
            posList[7] = tm.TransformPoint(pos);

            Vector3 min = posList[0];
            Vector3 max = posList[0];
            for (int i = 1; i < 8; i++)
            {
                Vector3 v = posList[i];
                if (min.x > v.x) min.x = v.x;
                if (min.y > v.y) min.y = v.y;
                if (min.z > v.z) min.z = v.z;

                if (max.x < v.x) max.x = v.x;
                if (max.y < v.y) max.y = v.y;
                if (max.z < v.z) max.z = v.z;
            }
            Bounds bound = new Bounds(Vector3.zero, Vector3.zero);
            bound.min = min;
            bound.max = max;
            return bound;
        }

        public static Bounds InverseTransformBounds(Transform tm, Bounds bd)
        {
            if (tm == null) return bd;

            Vector3[] posList = new Vector3[8];
            Vector3 pos = bd.min;
            posList[0] = tm.InverseTransformPoint(pos);

            pos.z = bd.max.z;
            posList[1] = tm.InverseTransformPoint(pos);

            pos.y = bd.max.y;
            posList[2] = tm.InverseTransformPoint(pos);

            pos.z = bd.min.z;
            posList[3] = tm.InverseTransformPoint(pos);

            pos.x = bd.max.x;
            posList[4] = tm.InverseTransformPoint(pos);

            pos.z = bd.max.z;
            posList[5] = tm.InverseTransformPoint(pos);

            pos.y = bd.min.y;
            posList[6] = tm.InverseTransformPoint(pos);

            pos.z = bd.min.z;
            posList[7] = tm.InverseTransformPoint(pos);

            Vector3 min = posList[0];
            Vector3 max = posList[0];
            for (int i = 1; i < 8; i++)
            {
                Vector3 v = posList[i];
                if (min.x > v.x) min.x = v.x;
                if (min.y > v.y) min.y = v.y;
                if (min.z > v.z) min.z = v.z;

                if (max.x < v.x) max.x = v.x;
                if (max.y < v.y) max.y = v.y;
                if (max.z < v.z) max.z = v.z;
            }
            Bounds bound = new Bounds(Vector3.zero, Vector3.zero);
            bound.min = min;
            bound.max = max;
            return bound;
        }

        public static Bounds TransformBounds(Matrix4x4 tm, Bounds bd)
        {
            Vector3[] posList = new Vector3[8];
            Vector3 pos = bd.min;
            posList[0] = tm.MultiplyPoint3x4(pos);

            pos.z = bd.max.z;
            posList[1] = tm.MultiplyPoint3x4(pos);

            pos.y = bd.max.y;
            posList[2] = tm.MultiplyPoint3x4(pos);

            pos.z = bd.min.z;
            posList[3] = tm.MultiplyPoint3x4(pos);

            pos.x = bd.max.x;
            posList[4] = tm.MultiplyPoint3x4(pos);

            pos.z = bd.max.z;
            posList[5] = tm.MultiplyPoint3x4(pos);

            pos.y = bd.min.y;
            posList[6] = tm.MultiplyPoint3x4(pos);

            pos.z = bd.min.z;
            posList[7] = tm.MultiplyPoint3x4(pos);

            Vector3 min = posList[0];
            Vector3 max = posList[0];
            for (int i = 1; i < 8; i++)
            {
                Vector3 v = posList[i];
                if (min.x > v.x) min.x = v.x;
                if (min.y > v.y) min.y = v.y;
                if (min.z > v.z) min.z = v.z;

                if (max.x < v.x) max.x = v.x;
                if (max.y < v.y) max.y = v.y;
                if (max.z < v.z) max.z = v.z;
            }
            Bounds bound = new Bounds(Vector3.zero, Vector3.zero);
            bound.min = min;
            bound.max = max;
            return bound;
        }

        public static bool IsBoundsInvalide(Bounds bd)
        {
            return FloatEqual(bd.size.x, 0.0f) || FloatEqual(bd.size.y, 0.0f) || FloatEqual(bd.size.z, 0.0f);
        }

        public static void CalMaskTex(Texture2D tex, Color colMask, Color colNoMask, float angle)
        {
            if (tex.width * tex.height > maskPixels.Length)
                maskPixels = new Color[tex.width * tex.height];

            Array.Clear(maskPixels, 0, maskPixels.Length);

            int w = tex.width;
            int h = tex.height;
            int w_2 = tex.width / 2;
            int h_2 = tex.height / 2;
            angle = Mathf.Clamp(angle, 0, 360);
            if (FloatEqual(angle, 0))
            {
                for (int i = 0; i < w * h; ++i)
                    maskPixels[i] = colNoMask;
            }
            else if (FloatEqual(angle, 360))
            {
                for (int i = 0; i < w * h; ++i)
                    maskPixels[i] = colMask;
            }
            else
            {
                int s_x = 0;
                int s_y = 0;
                int e_x = 0;
                int e_y = 0;
                Color col_1 = Color.black; //right-top
                Color col_2 = Color.black; //right-bottom
                Color col_3 = Color.black; //left-bottom
                Color col_4 = Color.black; //left-top

                float cos = 0.0f;
                Vector3 s_v = Vector3.zero;
                Vector3 e_v = Vector3.zero;

                if (angle < 90)
                {
                    s_x = w_2;
                    s_y = 0;
                    e_x = w;
                    e_y = h_2;

                    cos = Mathf.Cos(DegreeToRadian(angle));
                    s_v.y = 0 - h_2;
                    s_v.Normalize();

                    col_2 = colNoMask;
                    col_3 = colNoMask;
                    col_4 = colNoMask;
                }
                else if (angle < 180)
                {
                    s_x = w_2;
                    s_y = h_2;
                    e_x = w;
                    e_y = h;

                    cos = Mathf.Cos(DegreeToRadian(angle - 90));
                    s_v.x = w - w_2;
                    s_v.Normalize();

                    col_1 = colMask;
                    col_3 = colNoMask;
                    col_4 = colNoMask;
                }
                else if (angle < 270)
                {
                    s_x = 0;
                    s_y = h_2;
                    e_x = w_2;
                    e_y = h;

                    cos = Mathf.Cos(DegreeToRadian(angle - 180));
                    s_v.y = h - h_2;
                    s_v.Normalize();

                    col_1 = colMask;
                    col_2 = colMask;
                    col_4 = colNoMask;
                }
                else if (angle < 360)
                {
                    s_x = 0;
                    s_y = 0;
                    e_x = w_2;
                    e_y = h_2;

                    cos = Mathf.Cos(DegreeToRadian(angle - 270));
                    s_v.x = 0 - w_2;
                    s_v.Normalize();

                    col_1 = colMask;
                    col_2 = colMask;
                    col_3 = colMask;
                }

                for (int i = 0; i < h_2; ++i)
                {
                    for (int j = w_2; j < w; ++j)
                        maskPixels[i * w + j] = col_1;

                    for (int j = 0; j < w_2; ++j)
                        maskPixels[i * w + j] = col_4;
                }
                for (int i = h_2; i < h; ++i)
                {
                    for (int j = w_2; j < w; ++j)
                        maskPixels[i * w + j] = col_2;

                    for (int j = 0; j < w_2; ++j)
                        maskPixels[i * w + j] = col_3;
                }

                for (int i = s_y; i < e_y; ++i)
                {
                    for (int j = s_x; j < e_x; ++j)
                    {
                        e_v.y = i - h_2;
                        e_v.x = j - w_2;
                        e_v.Normalize();

                        if (Vector3.Dot(e_v, s_v) > cos)
                            maskPixels[i * w + j] = colMask;
                        else
                            maskPixels[i * w + j] = colNoMask;
                    }
                }

                for (int i = 0; i < h_2; ++i)
                {
                    for (int j = 0; j < w; ++j)
                    {
                        Color col = maskPixels[i * w + j];
                        maskPixels[i * w + j] = maskPixels[(h - 1 - i) * w + j];
                        maskPixels[(h - 1 - i) * w + j] = col;
                    }
                }
            }

            tex.SetPixels(maskPixels);
            tex.Apply();
        }

        public static int HexChar2Int(char c)
        {
            if (c >= '0' && c <= '9')
                return c - '0';

            if (c >= 'a' && c <= 'f')
                return c - 'a' + 10;

            if (c >= 'A' && c <= 'F')
                return c - 'A' + 10;

            //Logger.GetFile( LogFile.Global ).LogWarning("invalid char format: " + c);
            return 0;
        }

        public static Vector2 ScreenPosToGUI(Vector2 pos)
        {
            Vector2 p = pos;
            p.y = Screen.height - pos.y;
            return p;
        }

        public static Vector2 GUIPosToScreen(Vector2 pos)
        {
            Vector2 p = pos;
            p.y = Screen.height - pos.y;
            return p;
        }

        public static Color StringToColor(string value)
        {
            string tmp = value.Substring(1, value.Length - 2);
            string[] s = tmp.Split(new char[] { ',' });

            float r, g, b, a;
            r = float.Parse(s[0]);
            g = float.Parse(s[1]);
            b = float.Parse(s[2]);
            a = float.Parse(s[3]);

            return new Color(r, g, b, a);
        }

        public static string ColorToString(Color col)
        {
            string s = "(" + col.r.ToString() + "," + col.g.ToString()
                + "," + col.b.ToString() + "," + col.a.ToString() + ")";

            return s;
        }

        public static long DateToMillisecond(int date)
        {
            long millisecond = date * 86400000;
            return millisecond;
        }

        public static int MillisecondToHour(long millisecond)
        {
            int hour = (int)(millisecond / 3600000);
            return hour;
        }

        public static string LimitStringByByte(string src, int MaxBytes)
        {
            if (src == " " || src == ""
                || src == string.Empty || src == null)
                return string.Empty;

            string temp = string.Empty;

            int t = 0;
            char[] q = src.ToCharArray();
            for (int i = 0; i < q.Length; ++i)
            {
                if ((int)q[i] >= 0x4E00 && (int)q[i] <= 0x9FA5)//是否汉字
                {
                    temp += q[i];
                    t += 2;
                }
                else
                {
                    temp += q[i];
                    t += 1;
                }
                if (t >= MaxBytes)
                {
                    //FireEngine.Logger.GetFile( FireEngine.LogFile.Global ).LogWarning("����̫����\n");
                    break;
                }
            }
            return temp;
        }

        public static string LimitNumberString(string src, int MinNum, int MaxNum)
        {
            if (src == " " || src == ""
                || src == string.Empty || src == null)
                return string.Empty;

            string temp = string.Empty;
            char[] q = src.ToCharArray();
            for (int i = 0; i < q.Length; ++i)
            {
                if ((int)q[i] >= 48 && (int)q[i] <= 57)
                {
                    temp += q[i];
                }
                else
                {
                    break;
                }
            }
            if (int.Parse(temp) < MinNum)
                temp = MinNum.ToString();

            if (int.Parse(temp) > MaxNum)
                temp = MaxNum.ToString();

            return temp;
        }

        public static bool GetNearestCrossPoint(Vector2 src, Vector2 dst,
                                                Vector2 min, Vector2 max,
                                                out Vector2 cross)
        {
            cross = Vector2.zero;

            if (src.x < min.x && dst.x < min.x)
                return false;
            if (src.x > max.x && dst.x > max.x)
                return false;
            if (src.y < min.y && dst.y < min.y)
                return false;
            if (src.y > max.y && dst.y > max.y)
                return false;

            if (src.x > min.x && src.x < max.x && src.y > min.y && src.y < max.y
                && dst.x > min.x && dst.x < max.x && dst.y > min.y && dst.y < max.y)
            {
                //Logger.GetFile( LogFile.Global ).LogWarning("segment in rect: " + src + "" + dst + ""
                //                  + min + "" + max);
                return false;
            }

            Vector2 r0;
            r0.x = min.x;
            r0.y = min.y;

            Vector2 r1;
            r1.x = max.x;
            r1.y = min.y;

            Vector2 r2;
            r2.x = max.x;
            r2.y = max.y;

            Vector2 r3;
            r3.x = min.x;
            r3.y = max.y;

            Vector2 p0;
            p0.x = src.x;
            p0.y = src.y;

            Vector2 p1;
            p1.x = dst.x;
            p1.y = dst.y;

            Vector2 c = Vector2.zero;
            bool ok = GetNearestCrossPoint(p0, p1, r0, r1, r2, r3, out c);

            cross = c;
            return ok;
        }

        public static bool GetNearestCrossPoint(Vector2 p0, Vector2 p1,
                                                Vector2 r0, Vector2 r1,
                                                Vector2 r2, Vector2 r3,
                                                out Vector2 pos)
        {
            pos = Vector2.zero;

            Vector2 c0 = Vector2.zero;
            bool i0 = CrossPoint(p0, p1, r0, r1, out c0);

            Vector2 c1 = Vector2.zero;
            bool i1 = CrossPoint(p0, p1, r1, r2, out c1);

            Vector2 c2 = Vector2.zero;
            bool i2 = CrossPoint(p0, p1, r2, r3, out c2);

            Vector2 c3 = Vector2.zero;
            bool i3 = CrossPoint(p0, p1, r3, r0, out c3);

            if (!i0 && !i1 && !i2 && !i3)
                return false;

            float d0 = (c0 - p0).sqrMagnitude;
            float d1 = (c1 - p0).sqrMagnitude;
            float d2 = (c2 - p0).sqrMagnitude;
            float d3 = (c3 - p0).sqrMagnitude;

            Vector2 c = Vector3.zero;
            float d = 0.0f;
            bool find = false;
            if (i0)
            {
                find = true;
                d = d0;
                c = c0;
            }
            if (i1)
            {
                if (!find)
                {
                    find = true;
                    d = d1;
                    c = c1;
                }
                else if (d1 < d)
                {
                    d = d1;
                    c = c1;
                }
            }
            if (i2)
            {
                if (!find)
                {
                    find = true;
                    d = d2;
                    c = c2;
                }
                else if (d2 < d)
                {
                    d = d2;
                    c = c2;
                }
            }
            if (i3)
            {
                if (!find)
                {
                    find = true;
                    d = d3;
                    c = c3;
                }
                else if (d3 < d)
                {
                    d = d3;
                    c = c3;
                }
            }

            if (!find)
            {
                //Logger.GetFile( LogFile.Global ).LogWarning("error");
                return false;
            }

            pos = c;
            return true;
        }

        public static bool CrossPoint(Vector2 p0, Vector2 p1, Vector2 q0,
                                      Vector2 q1, out Vector2 c)
        {
            Vector2 p = Vector2.zero;
            c = p;

            Vector2 dp = p1 - p0;
            Vector2 dq = q1 - q0;

            if (FloatEqual(dp.sqrMagnitude, 0) || FloatEqual(dq.sqrMagnitude, 0))
                return false;

            if (FloatEqual(dp.x, 0) && FloatEqual(dq.x, 0))
                return false;

            if (FloatEqual(dp.y, 0) && FloatEqual(dq.y, 0))
                return false;

            if (!FloatEqual(dp.x, 0) && !FloatEqual(dq.x, 0))
            {
                if (FloatEqual(dp.y / dp.x, dq.y / dq.x)
                   || FloatEqual(dp.y / dp.x, -dq.y / dq.x))
                    return false;
            }

            if (!FloatEqual(dp.y, 0) && !FloatEqual(dq.y, 0))
            {
                if (FloatEqual(dp.x / dp.y, dq.x / dq.y)
                   || FloatEqual(dp.x / dp.y, -dq.x / dq.y))
                    return false;
            }

            float a0 = p0.y * dq.x;
            float a1 = dp.y * dq.x;
            float a2 = q0.y * dq.x;
            float b0 = dq.y * p0.x;
            float b1 = dq.y * q0.x;
            float b2 = dq.y * dp.x;

            float a = a0 - a2 - b0 + b1;
            float b = b2 - a1;

            if (FloatEqual(b, 0.0f))
                return false;

            float vp = a / b;
            float vq = -1;
            if (!FloatEqual(dq.x, 0))
                vq = (p0.x + dp.x * vp - q0.x) / dq.x;
            else if (!FloatEqual(dq.y, 0))
                vq = (p0.y + dp.y * vp - q0.y) / dq.y;
            else
            {
                //Logger.GetFile(LogFile.Global).LogWarning("cross cal error");
            }

            if (vp > 0 - FloatEpsilon && vp < 1 + FloatEpsilon
                && vq > 0 - FloatEpsilon && vq < 1 + FloatEpsilon)
            {
                p.x = p0.x + vp * dp.x;
                p.y = p0.y + vp * dp.y;
                c = p;
                return true;
            }
            else
                return false;
        }


        /// <summary>
        /// Remap 
        /// </summary>
        /// <param name="currentValue"></param>
        /// <param name="iMin"></param>
        /// <param name="iMax"></param>
        /// <param name="oMin"></param>
        /// <param name="oMax"></param>
        /// <returns></returns>
        public static float Remap(float currentValue, float iMin, float iMax, float oMin, float oMax)
        {
            float oSize = oMax - oMin;
            float iSize = iMax - iMin;
            float factor = oSize / iSize;

            return oMin + (currentValue - iMin) * factor;
        }

        /// <summary>
        /// Remap to 0 1
        /// </summary>
        /// <param name="currentValue"></param>
        /// <param name="iMin"></param>
        /// <param name="iMax"></param>
        /// <returns></returns>
        public static float Remap01(float currentValue, float iMin, float iMax)
        {
            return Remap(currentValue, iMin, iMax, 0, 1);
        }
    }
}
