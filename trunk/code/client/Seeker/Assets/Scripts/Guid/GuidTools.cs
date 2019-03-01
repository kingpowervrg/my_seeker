using EngineCore;
using UnityEngine.EventSystems;
using UnityEngine;
using System.Collections.Generic;
using GOGUI;
using HedgehogTeam.EasyTouch;

namespace SeekerGame
{
    public class GuidTools
    {
        public static bool isSkipGuid = false;

        public static bool isInnerRect(GuidMaskData maskdata,Vector2 pos)
        {
            if (pos.x >= maskdata.leftBottom.x && pos.x <= maskdata.rightBottom.x 
                && pos.y >= maskdata.leftBottom.y && pos.y <= maskdata.rightTop.y)
            {
                return true;
            }
            return false;
        }

        public static bool isInnerCircle(GuidCircleData circle,Vector2 pos)
        {
            float dis = Vector2.Distance(pos,circle.centerPos);
            if (dis <= circle.radius)
            {
                return true;
            }
            return false;
        }

        public static GuidMaskBoardData InitMask(List<GuidMaskData> maskDatas, RectTransform m_RectRoot, GuidUIData uiData)
        {
            Vector4[] circleCenter = new Vector4[4];
            float[] circleRadius = new float[4];

            Vector4[] rectCenter = new Vector4[4];
            float[] rectWidth = new float[4];
            float[] rectHeigh = new float[4];
            int circleIndex = 0;
            int rectIndex = 0;
            for (int i = 0; i < maskDatas.Count; i++)
            {

                GuidMaskData maskData = maskDatas[i];
                Vector2 localLeftBottom = WordToLocalPos(m_RectRoot,maskData.leftBottom);
                Vector2 localLeftTop = WordToLocalPos(m_RectRoot,maskData.leftTop);
                Vector2 localRightBottom = WordToLocalPos(m_RectRoot,maskData.rightBottom);
                Vector2 localRightTop = WordToLocalPos(m_RectRoot,maskData.rightTop);

                float centerx = (localLeftBottom.x + localRightBottom.x) / 2f;
                float centery = (localLeftBottom.y + localRightTop.y) / 2f;

                Vector2 center = new Vector2(centerx, centery);
                //Vector2 center = WordToLocalPos(maskData.centerPos);
                float width = localRightBottom.x - localLeftBottom.x;//maskData.width;
                float heigh = localRightTop.y - localLeftBottom.y;//maskData.heigh;

                if (maskData.maskType == GuidMaskType.Rect)
                {
                    rectCenter[rectIndex] = center;
                    rectWidth[rectIndex] = width / 2f;
                    rectHeigh[rectIndex] = heigh / 2f;
                    rectIndex++;
                    uiData.AddRectData(maskData);
                }
                else if (maskData.maskType == GuidMaskType.Circle)
                {

                    circleCenter[circleIndex] = center;
                    circleRadius[circleIndex] = Mathf.Max(width, heigh) / 2f;
                    circleIndex++;

                    GuidCircleData circleData = new GuidCircleData();
                    circleData.centerPos = new Vector2((maskData.leftBottom.x + maskData.rightBottom.x) / 2f, (maskData.leftBottom.y + maskData.rightTop.y) / 2f);
                    circleData.radius = Mathf.Max(maskData.rightBottom.x - maskData.leftBottom.x, maskData.rightTop.y - maskData.leftBottom.y);
                    circleData.btnName = maskData.btnName;
                    uiData.AddCircleData(circleData);
                }
            }
            GuidMaskBoardData board = new GuidMaskBoardData(circleCenter,circleRadius,rectCenter,rectWidth,rectHeigh);
            return board;
        }

        public static Vector2 WordToLocalPos(RectTransform m_RectRoot, Vector2 world)
        {
            Vector2 position = Vector2.zero;
            position = m_RectRoot.InverseTransformPoint(world);
            return position;
        }

        public static Vector2 ScreenToWorldPos(Camera UICamera, Vector2 screenPos)
        {
            return UICamera.ScreenToWorldPoint(screenPos);
        }

        public static void PassEvent<T>(string btnName, PointerEventData data, ExecuteEvents.EventFunction<T> function, bool isClick) where T : IEventSystemHandler
        {
            List<RaycastResult> results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(data, results);
           
            GameObject current = data.pointerCurrentRaycast.gameObject;
            for (int i = 0; i < results.Count; i++)
            {
                GameObject resultGameObject = results[i].gameObject;
                if (current != resultGameObject)
                {
                    if (isClick)
                    {
                        if (resultGameObject.name.Equals(btnName))
                        {
                            ExecuteEvents.Execute(results[i].gameObject, data, function);
                            break;
                        }
                    }
                    else
                    {
                        ExecuteEvents.Execute(results[i].gameObject, data, function);
                    }
                }
            }
        }

        //获取图片四个角
        public static Vector2[] getCornPos(RectTransform tran,float pixel = 0f)
        {
            Vector2[] cornPos = new Vector2[4];
            Vector3[] localCorner = new Vector3[4];
            tran.GetWorldCorners(localCorner);
            cornPos[0] = new Vector2(localCorner[0].x + pixel, localCorner[0].y + pixel);
            cornPos[1] = new Vector2(localCorner[1].x + pixel, localCorner[1].y - pixel);
            cornPos[2] = new Vector2(localCorner[2].x - pixel, localCorner[2].y - pixel);
            cornPos[3] = new Vector2(localCorner[3].x - pixel, localCorner[3].y + pixel);
            //for (int i = 0; i < localCorner.Length; i++)
            //{
            //    cornPos[i] = new Vector2(localCorner[i].x,localCorner[i].y);
            //}
            //Transform srcParent = tran.parent;
            //Vector2[] cornPos = new Vector2[4];
            //cornPos[0] = srcParent.TransformPoint(localCorner[0]); //leftBottom
            //cornPos[1] = srcParent.TransformPoint(localCorner[1]); //leftTop
            //cornPos[2] = srcParent.TransformPoint(localCorner[2]); //rightTop
            //cornPos[3] = srcParent.TransformPoint(localCorner[3]); //rightBottom


            //float width = tran.rect.width / 2f;
            //float heigh = tran.rect.height / 2f;
            //Vector3 localPos = tran.localPosition;
            //Transform srcParent = tran.parent;
            //Vector2[] cornPos = new Vector2[4];
            //cornPos[0] = srcParent.TransformPoint(localPos.x - width, localPos.y - heigh, localPos.z); //leftBottom
            //cornPos[1] = srcParent.TransformPoint(localPos.x - width, localPos.y + heigh, localPos.z); //leftTop
            //cornPos[2] = srcParent.TransformPoint(localPos.x + width, localPos.y + heigh, localPos.z); //rightTop
            //cornPos[3] = srcParent.TransformPoint(localPos.x + width, localPos.y - heigh, localPos.z); //rightBottom
            return cornPos;
        }

        //根据挂点获取特效位置
        public static List<Vector2> getArtPos(Transform tran,ConfGuid confguid)
        {
            List<Vector2> artPos = new List<Vector2>();
            if (confguid.artIDs == null || confguid.artIDs.Length == 0)
            {
                return artPos;
            }
            for (int i = 0; i < confguid.artIDs.Length; i++)
            {
                ConfGuidArt guidArt = ConfGuidArt.Get(confguid.artIDs[i]);

                string btnStr = guidArt.artAnchor.Replace(":", "/");
                Transform artTran = tran.Find(btnStr);
                if (artTran != null)
                {
                    Vector2 pos = artTran.position;
                    if (guidArt.artShift.Length == 2)
                    {
                        pos += new Vector2(guidArt.artShift[0],guidArt.artShift[1]);
                    }
                    artPos.Add(pos);
                }
            }
            return artPos;
        }
    }
}
