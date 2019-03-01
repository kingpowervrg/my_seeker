using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace GOGUI
{
   
    /// <summary>
    /// 渐变色
    /// </summary>
    [AddComponentMenu("UI/Effects/GradientSegment")]
    public class GradientSegment : BaseMeshEffect
    {
        [SerializeField]
        private Color32 fromColor = Color.white;
        [SerializeField]
        private Color32 toColor = Color.black;

        public enum GradientType
        {
            Top2Bottom,
            Right2Left
        };

        [SerializeField]
        private GradientType gradientType = GradientType.Top2Bottom;

        [SerializeField]
        private float segmentCount = 0;

        [SerializeField]
        private float fromStartsegment = 1;

        [SerializeField]
        private float toEndsegment = 0;

        RectTransform cahceRectTransform = null;
        float limitDistance = 0.0f;
        
        public override void ModifyMesh(VertexHelper vh)
        {
            if (!IsActive())
            {
                return;
            }

            if (cahceRectTransform == null)
            {
                cahceRectTransform = this.GetComponent<RectTransform>();
                limitDistance = gradientType == GradientType.Top2Bottom ? cahceRectTransform.rect.height : cahceRectTransform.rect.width;
                limitDistance -= 5f; //应该跟据字号去处理  文本框宽度 跟 一行字宽度的误差
            }

            //是字体的时候 list 极大  少用为好
            List<UIVertex> vertexList = new List<UIVertex>();

            vh.GetUIVertexStream(vertexList);

            int count = vertexList.Count;

            if (gradientType == GradientType.Top2Bottom)
            {
                if (count > 0)
                {
                    float bottomY = 0;// vertexList[0].position.y;
                    float topY = 0;// vertexList[0].position.y;

                    float segmentDis = limitDistance / segmentCount;

                    topY = limitDistance / 2 - segmentDis * (fromStartsegment - 1);
                    bottomY = -limitDistance / 2 + segmentDis * (segmentCount - toEndsegment);

                    float uiElementHeight = segmentDis * (Mathf.Abs(toEndsegment - fromStartsegment) + 1);

                    for (int i = 0; i < count; i++)
                    {
                        UIVertex uiVertex = vertexList[i];
                        Color o = uiVertex.color;
                        if (uiVertex.position.y < bottomY)
                        {
                            Color32 b = new Color32((byte)(toColor.r * o.r), (byte)(toColor.g * o.g), (byte)(toColor.b * o.b), (byte)(toColor.a * o.a));
                            uiVertex.color = b;
                            vertexList[i] = uiVertex;
                        }
                        else if (uiVertex.position.y > topY)
                        {
                            Color32 t = new Color32((byte)(fromColor.r * o.r), (byte)(fromColor.g * o.g), (byte)(fromColor.b * o.b), (byte)(fromColor.a * o.a));
                            uiVertex.color = t;
                            vertexList[i] = uiVertex;
                        }
                        else
                        {
                            Color32 b = new Color32((byte)(toColor.r * o.r), (byte)(toColor.g * o.g), (byte)(toColor.b * o.b), (byte)(toColor.a * o.a));
                            Color32 t = new Color32((byte)(fromColor.r * o.r), (byte)(fromColor.g * o.g), (byte)(fromColor.b * o.b), (byte)(fromColor.a * o.a));
                            uiVertex.color = Color32.Lerp(b, t, (uiVertex.position.y - bottomY) / uiElementHeight);
                            vertexList[i] = uiVertex;
                        }
                    }
                    vh.Clear();
                    vh.AddUIVertexTriangleStream(vertexList);
                }
            }
            else {
                if (count > 0)
                {
                    float maxX = 0;// vertexList[0].position.x;
                    float minX = 0;// vertexList[0].position.x;
                    
                    float segmentDis = limitDistance / segmentCount;

                    maxX = limitDistance/2 - segmentDis * (fromStartsegment - 1);
                    minX = -limitDistance/2 + segmentDis * (segmentCount - toEndsegment);

                    float uiElementWidth = segmentDis * (Mathf.Abs(toEndsegment - fromStartsegment) + 1);

                    for (int i = 0; i < count; i++)
                    {
                        UIVertex uiVertex = vertexList[i];
                        Color o = uiVertex.color;
                        if (uiVertex.position.x < minX)
                        {
                            Color32 b = new Color32((byte)(toColor.r * o.r), (byte)(toColor.g * o.g), (byte)(toColor.b * o.b), (byte)(toColor.a * o.a));
                            uiVertex.color = b;
                            vertexList[i] = uiVertex;
                        }
                        else if (uiVertex.position.x > maxX)
                        {
                            Color32 t = new Color32((byte)(fromColor.r * o.r), (byte)(fromColor.g * o.g), (byte)(fromColor.b * o.b), (byte)(fromColor.a * o.a));
                            uiVertex.color = t;
                            vertexList[i] = uiVertex;
                        }
                        else {
                            Color32 b = new Color32((byte)(toColor.r * o.r), (byte)(toColor.g * o.g), (byte)(toColor.b * o.b), (byte)(toColor.a * o.a));
                            Color32 t = new Color32((byte)(fromColor.r * o.r), (byte)(fromColor.g * o.g), (byte)(fromColor.b * o.b), (byte)(fromColor.a * o.a));
                            uiVertex.color = Color32.Lerp(b, t, (uiVertex.position.x - minX) / uiElementWidth);
                            vertexList[i] = uiVertex;
                        }
                    }

                    vh.Clear();
                    vh.AddUIVertexTriangleStream(vertexList);
                }
            }

 
        }
    }

}