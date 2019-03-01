using UnityEngine;

namespace EngineCore
{
    /// <summary>
    /// 圆角矩阵
    /// </summary>
    public class RoundConnerRectangle
    {
        private Vector2 m_min = new Vector2(float.MaxValue, float.MaxValue);
        private Vector2 m_max = new Vector2(float.MinValue, float.MinValue);

        public Vector2 m_roundCenterMin = new Vector2(float.MaxValue, float.MaxValue);
        public Vector2 m_roundCenterMax = new Vector2(float.MinValue, float.MinValue);

        private Vector2 m_innerRectangelMin = new Vector2(float.MaxValue, float.MaxValue);
        private Vector2 m_innerRectangelMax = new Vector2(float.MinValue, float.MinValue);


        public void UpdatePoint(Vector3 point)
        {
            UpdatePoint(new Vector2(point.x, point.z));
        }

        public void UpdatePoint(Vector2 point)
        {
            UpdateX(point.x);
            UpdateY(point.y);

            UpdateRoundCenterAndRadius();
            UpdateInnerRectange();
        }

        private void UpdateX(float x)
        {
            m_min.x = Mathf.Min(m_min.x, x);
            m_max.x = Mathf.Max(m_max.x, x);

        }

        private void UpdateY(float y)
        {
            m_min.y = Mathf.Min(m_min.y, y);
            m_max.y = Mathf.Max(m_max.y, y);
        }

        public void ResetRectangle()
        {
            this.m_min = new Vector2(float.MaxValue, float.MaxValue);
            this.m_max = new Vector2(float.MinValue, float.MinValue);
        }

        public void ResetRectangle(Vector2 min, Vector2 max)
        {
            ResetRectangle();

            this.m_max = max;
            this.m_min = min;

            UpdateRoundCenterAndRadius();
            UpdateInnerRectange();
        }

        /// <summary>
        /// 更新圆角的圆心及半径
        /// </summary>
        private void UpdateRoundCenterAndRadius()
        {
            this.m_roundCenterMin = this.m_min + new Vector2(ConnerRaidus, ConnerRaidus);
            this.m_roundCenterMax = this.m_max - new Vector2(ConnerRaidus, ConnerRaidus);
        }

        private void UpdateInnerRectange()
        {
            if (RectangeAspect > 1.0)
            {
                m_innerRectangelMin.x = m_min.x + ConnerRaidus;
                m_innerRectangelMin.y = m_min.y;

                m_innerRectangelMax.x = m_max.x - ConnerRaidus;
                m_innerRectangelMax.y = m_max.y;
            }
            else if (RectangeAspect < 1.0)
            {
                m_innerRectangelMin.x = m_min.x;
                m_innerRectangelMin.y = m_min.y + ConnerRaidus;

                m_innerRectangelMax.x = m_max.x;
                m_innerRectangelMax.y = m_max.y - ConnerRaidus;
            }
            else
            {
                m_innerRectangelMin = Center;
                m_innerRectangelMax = Center;
            }
        }

        /// <summary>
        /// 获取点在圆角矩形上的点
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public Vector2 ClosestPointOnRectangle(Vector2 point)
        {
            Vector2 closestPoint = point;

            if (point.x < this.m_min.x)
                closestPoint.x = this.m_min.x;
            else if (point.x > this.m_max.x)
                closestPoint.x = this.m_max.x;

            if (point.y < this.m_min.y)
                closestPoint.y = this.m_min.y;
            else if (point.y > this.m_max.y)
                closestPoint.y = this.m_max.y;


            //内部点拉到最近的边上
            float distanceToYmin = closestPoint.y - this.m_min.y;
            float distanceToYmax = this.m_max.y - closestPoint.y;
            float distanceToXmin = closestPoint.x - this.m_min.x;
            float distanceToXmax = this.m_max.x - closestPoint.x;

            float smallestDistance = Mathf.Min(distanceToXmax, distanceToXmin, distanceToYmax, distanceToYmin);
            if (smallestDistance == distanceToXmin)
                closestPoint.x = this.m_min.x;
            else if (smallestDistance == distanceToXmax)
                closestPoint.x = this.m_max.x;
            else if (smallestDistance == distanceToYmin)
                closestPoint.y = this.m_min.y;
            else
                closestPoint.y = this.m_max.y;



            if (RectangeAspect > 1.0)
            {
                if (closestPoint.x > this.m_innerRectangelMin.x && closestPoint.x < this.m_innerRectangelMax.x)
                {
                    return closestPoint;
                }
            }
            else
            {
                if (closestPoint.y > this.m_innerRectangelMin.y && closestPoint.y < this.m_innerRectangelMax.y)
                {
                    return closestPoint;

                }
            }


            Vector2 closestRaidusCenter = default(Vector2);
            if (RectangeAspect < 1.0)
            {
                if (closestPoint.y < this.m_innerRectangelMin.y)
                    closestRaidusCenter = m_roundCenterMin;
                else if (closestPoint.y > this.m_innerRectangelMax.y)
                    closestRaidusCenter = m_roundCenterMax;
            }
            else
            {
                if (closestPoint.x < this.m_innerRectangelMin.x)
                    closestRaidusCenter = m_roundCenterMin;
                else if (closestPoint.x > this.m_innerRectangelMax.x)
                    closestRaidusCenter = m_roundCenterMax;
            }

            Vector2 pointToRaidusCenter = closestPoint - closestRaidusCenter;
            float crossZ = Vector3.Cross(pointToRaidusCenter, Vector2.right).z;
            float angle = Vector2.Angle(pointToRaidusCenter, Vector2.right);
            angle = crossZ > 0 ? 360 - angle : angle;

            angle *= Mathf.Deg2Rad;

            closestPoint.x = closestRaidusCenter.x + Mathf.Cos(angle) * ConnerRaidus;
            closestPoint.y = closestRaidusCenter.y + Mathf.Sin(angle) * ConnerRaidus;

            return closestPoint;
        }

        /// <summary>
        /// 由中心点及方向，获取圆角矩形边上的点
        /// </summary>
        /// <param name="point"></param>
        /// <param name="centerTarget"></param>
        /// <param name="yValue"></param>
        /// <returns></returns>
        /// <remarks>todo: 由于时间关系，这个方法的不咋的，有时间重构！！！ guangze song</remarks>
        public Vector2 ClosestPointOnRoundRectange(Vector2 point, Vector2 centerTarget)
        {
            Vector2 centerToPoint = (point - centerTarget);

            GeometryUtility.IntersectionResult intersectionResult = null;

            //判断与两边相交情况
            Vector2 segmentStart = m_innerRectangelMin;
            Vector2 segmentEnd = new Vector2(m_innerRectangelMax.x, m_innerRectangelMin.y);

            if (RectangeAspect < 1.0f)
                segmentEnd = new Vector2(m_innerRectangelMin.x, m_innerRectangelMax.y);

            intersectionResult = GeometryUtility.RayToLineSegmentIntersection(centerTarget, centerToPoint, segmentStart, segmentEnd);

            if (intersectionResult.IntersectionPointCount > 0)
                return intersectionResult.IntersectionPointList[0];

            segmentStart = m_innerRectangelMax;
            segmentEnd = new Vector2(m_innerRectangelMin.x, m_innerRectangelMax.y);
            if (RectangeAspect < 1.0f)
                segmentEnd = new Vector2(m_innerRectangelMax.x, m_innerRectangelMin.y);

            intersectionResult = GeometryUtility.RayToLineSegmentIntersection(centerTarget, centerToPoint, segmentStart, segmentEnd);
            if (intersectionResult.IntersectionPointCount > 0)
                return intersectionResult.IntersectionPointList[0];


            //弧相交情况
            intersectionResult = GeometryUtility.RayToCircleIntersection(centerTarget, centerToPoint, this.m_roundCenterMin, ConnerRaidus);

            if (intersectionResult.IntersectionPointCount == 1)
            {
                Vector2 pointOnCircle = intersectionResult.IntersectionPointList[0];
                if (RectangeAspect >= 1.0)
                {
                    if (pointOnCircle.x < m_innerRectangelMin.x)
                        return pointOnCircle;
                }
                else
                {
                    if (pointOnCircle.y < m_innerRectangelMin.y)
                        return pointOnCircle;
                }
            }
            else if (intersectionResult.IntersectionPointCount == 2)
            {
                Vector2 pointOnCircle1 = intersectionResult.IntersectionPointList[0];
                Vector2 pointOnCircle2 = intersectionResult.IntersectionPointList[1];

                Vector2 pointOnCircle = MathUtil.SqrDistanceTo(pointOnCircle1, centerTarget) > MathUtil.SqrDistanceTo(pointOnCircle2, centerTarget) ? pointOnCircle1 : pointOnCircle2;

                if (RectangeAspect >= 1.0)
                {
                    if (pointOnCircle.x < m_innerRectangelMin.x)
                        return pointOnCircle;
                }
                else
                {
                    if (pointOnCircle.y < m_innerRectangelMin.y)
                        return pointOnCircle;
                }
            }

            intersectionResult = GeometryUtility.RayToCircleIntersection(centerTarget, centerToPoint, this.m_roundCenterMax, ConnerRaidus);

            if (intersectionResult.IntersectionPointCount == 1)
            {
                Vector2 pointOnCircle = intersectionResult.IntersectionPointList[0];
                if (RectangeAspect >= 1.0)
                {
                    if (pointOnCircle.x > m_innerRectangelMax.x)
                        return pointOnCircle;
                }
                else
                {
                    if (pointOnCircle.y > m_innerRectangelMax.y)
                        return pointOnCircle;
                }
            }
            else if (intersectionResult.IntersectionPointCount == 2)
            {
                Vector2 pointOnCircle1 = intersectionResult.IntersectionPointList[0];
                Vector2 pointOnCircle2 = intersectionResult.IntersectionPointList[1];

                Vector2 pointOnCircle = MathUtil.SqrDistanceTo(pointOnCircle1, centerTarget) > MathUtil.SqrDistanceTo(pointOnCircle2, centerTarget) ? pointOnCircle1 : pointOnCircle2;

                if (RectangeAspect >= 1.0)
                {
                    if (pointOnCircle.x > m_innerRectangelMax.x)
                        return pointOnCircle;
                }
                else
                {
                    if (pointOnCircle.y > m_innerRectangelMax.y)
                        return pointOnCircle;
                }
            }

            return point;
        }


        public Vector2 Min
        {
            get { return this.m_min; }
        }

        public Vector2 Max
        {
            get { return this.m_max; }
        }

        public Vector2 Center
        {
            get { return (this.m_max + this.m_min) / 2; }
        }

        public Vector2 Size
        {
            get { return this.m_max - this.m_min; }
        }

        public Vector2 Extends
        {
            get { return Size / 2; }
        }

        public float ConnerRaidus
        {
            get { return Mathf.Min(Extends.x, Extends.y); }
        }

        /// <summary>
        /// 矩形的方向
        /// </summary>
        private float RectangeAspect
        {
            get { return Extends.x / Extends.y; }
        }

    }
}
