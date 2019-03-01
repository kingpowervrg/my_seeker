using System;
using System.Collections.Generic;
using UnityEngine;

namespace EngineCore
{
    /// <summary>
    /// 2D几何体相交性检测
    /// </summary>
    public static partial class GeometryUtility
    {
        /// <summary>
        /// 相交结果
        /// </summary>
        public class IntersectionResult
        {
            private List<Vector2> m_intersectionPoints = new List<Vector2>();

            public void AddIntersectionPoint(Vector2 intersectionPoint)
            {
                this.m_intersectionPoints.Add(intersectionPoint);
            }

            public List<Vector2> IntersectionPointList
            {
                get { return this.m_intersectionPoints; }
            }

            public int IntersectionPointCount
            {
                get { return m_intersectionPoints.Count; }
            }
        }



        /// <summary>
        /// 两射线求交点
        /// </summary>
        /// <param name="rayOrigin1"></param>
        /// <param name="rayDirection1"></param>
        /// <param name="rayOrigin2"></param>
        /// <param name="rayDirection2"></param>
        /// <returns></returns>
        public static IntersectionResult RayToRayIntersection(Vector2 rayOrigin1, Vector2 rayDirection1, Vector2 rayOrigin2, Vector2 rayDirection2)
        {
            IntersectionResult result = new IntersectionResult();

            rayDirection1 = rayDirection1.normalized;
            rayDirection2 = rayDirection2.normalized;

            float r, s, d;

            if (rayDirection1 != rayDirection2)
            {
                d = Cross(rayDirection1, rayDirection2);
                if (d != 0)
                {
                    Vector2 rayOrigin2ToRayOrigin1 = rayOrigin1 - rayOrigin2;
                    r = Cross(rayDirection2, rayOrigin2ToRayOrigin1) / d;
                    s = Cross(rayDirection1, rayOrigin2ToRayOrigin1) / d;
                    if (r >= 0)
                    {
                        if (s >= 0)
                        {
                            Vector2 intersectionPoint = rayOrigin1 + rayDirection1 * r;
                            result.AddIntersectionPoint(intersectionPoint);
                        }
                    }
                }
            }
            return result;
        }

        /// <summary>
        ///  射线与线段的交点
        /// </summary>
        /// <param name="rayOrigin"></param>
        /// <param name="rayDirection"></param>
        /// <param name="lineStart"></param>
        /// <param name="lineEnd"></param>
        /// <returns></returns>
        public static IntersectionResult RayToLineSegmentIntersection(Vector2 rayOrigin, Vector2 rayDirection, Vector2 lineStart, Vector2 lineEnd)
        {
            IntersectionResult result = new IntersectionResult();
            Vector2 v1 = rayOrigin - lineStart;
            Vector2 v2 = lineEnd - lineStart;
            Vector2 v3 = new Vector2(-rayDirection.y, rayDirection.x);

            var dot = Vector2.Dot(v2, v3);
            if (Math.Abs(dot) < float.Epsilon)
                return result;

            var t1 = Cross(v2, v1) / dot;
            var t2 = Vector2.Dot(v1, v3) / dot;

            if (t1 >= 0.0 && (t2 >= 0.0 && t2 <= 1.0))
            {
                result.AddIntersectionPoint(rayOrigin + t1 * rayDirection);
            }

            return result;
        }

        /// <summary>
        /// 两直线求交点
        /// </summary>
        /// <param name="line1Start"></param>
        /// <param name="line1End"></param>
        /// <param name="isSegmentLine1"></param>
        /// <param name="line2Start"></param>
        /// <param name="line2End"></param>
        /// <param name="isSegmentLine2"></param>
        /// <returns></returns>
        public static IntersectionResult LineToLineIntersection(Vector2 line1Start, Vector2 line1End, bool isSegmentLine1, Vector2 line2Start, Vector2 line2End, bool isSegmentLine2)
        {
            IntersectionResult result = new IntersectionResult();

            Vector2 line1Direction = line1End - line1Start;
            Vector2 line2Direction = line2End - line2Start;

            if (line2Direction != line1Direction)
            {
                float d = Cross(line1Direction, line2Direction);

                if (d != 0)
                {
                    Vector2 line1StartToLine2Start = line2Start - line1Start;

                    float r = Cross(line1StartToLine2Start, line2Direction) / d;
                    float s = Cross(line1StartToLine2Start, line1Direction) / d;

                    Vector2 intersection = default(Vector2);

                    //两条直线
                    if (!isSegmentLine1 && !isSegmentLine2)
                    {
                        intersection = line1Start + line1Direction * r;
                        result.AddIntersectionPoint(intersection);

                        return result;
                    }


                    //两条线段
                    if (isSegmentLine1 && isSegmentLine2)
                    {
                        if (r >= 0 && r <= 1 && s >= 0 && s <= 1)
                        {
                            intersection = line1Start + line1Direction * r;
                            result.AddIntersectionPoint(intersection);

                            return result;
                        }
                    }

                    //第一个是线段，第二个是直线
                    if (isSegmentLine1 && !isSegmentLine2)
                    {
                        if (r >= 0 && r <= 1)
                        {
                            intersection = line1Start + line1Direction * r;
                            result.AddIntersectionPoint(intersection);

                            return result;
                        }
                    }

                    if (!isSegmentLine1 && isSegmentLine2)
                    {
                        if (r >= 0 && r <= 1)
                        {
                            intersection = line1Start + line1Direction * r;
                            result.AddIntersectionPoint(intersection);

                            return result;
                        }
                    }

                }

            }

            return result;
        }

        /// <summary>
        /// 射线与圆相交交点
        /// </summary>
        /// <param name="rayOrigin"></param>
        /// <param name="rayDirection"></param>
        /// <param name="circleCenter"></param>
        /// <param name="radius"></param>
        /// <returns></returns>
        public static IntersectionResult RayToCircleIntersection(Vector2 rayOrigin, Vector2 rayDirection, Vector2 circleCenter, float radius, bool includeRayExtension = false)
        {
            IntersectionResult result = new IntersectionResult();

            Vector2 d = rayOrigin - circleCenter;
            float a = Vector2.Dot(rayDirection, rayDirection);
            float b = Vector2.Dot(d, rayDirection);
            float c = Vector2.Dot(d, d) - radius * radius;

            float disc = b * b - a * c;
            if (disc < 0.0f)
                return result;

            float sqrtDisc = Mathf.Sqrt(disc);

            float invA = 1.0f / a;

            float t = (-b - sqrtDisc) * invA;
            float s = (-b + sqrtDisc) * invA;

            float invRadius = 1.0f / radius;

            //相切，一个交点
            if (sqrtDisc == 0)
            {
                if (includeRayExtension)
                    result.AddIntersectionPoint(rayOrigin + t * rayDirection);
                else
                {
                    if (t > 0)
                        result.AddIntersectionPoint(rayOrigin + t * rayDirection);
                }

                return result;
            }

            if (includeRayExtension)
            {
                result.AddIntersectionPoint(rayOrigin + t * rayDirection);
                result.AddIntersectionPoint(rayOrigin + s * rayDirection);
            }
            else
            {
                if (t > 0)
                    result.AddIntersectionPoint(rayOrigin + t * rayDirection);

                if (s > 0)
                    result.AddIntersectionPoint(rayOrigin + s * rayDirection);
            }

            return result;
        }

        /// <summary>
        /// Vector2 的叉乘
        /// </summary>
        /// <param name="vector1"></param>
        /// <param name="vector2"></param>
        /// <returns></returns>
        public static float Cross(Vector2 vector1, Vector2 vector2)
        {
            return vector1.x * vector2.y - vector2.x * vector1.y;
        }
    }
}