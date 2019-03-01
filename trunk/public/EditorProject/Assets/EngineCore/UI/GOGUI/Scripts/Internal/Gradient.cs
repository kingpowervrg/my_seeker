using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace GOGUI
{
    /// <summary>
    /// 图文混排
    /// </summary>
    [AddComponentMenu("UI/Effects/Gradient")]
    public class Gradient : BaseMeshEffect
    {
        [SerializeField]
        private Color32 firstColor = Color.white;
        [SerializeField]
        private Color32 secondColor = Color.black;
        [SerializeField]
        private GradientMode Mode = GradientMode.SingleLine;
        [SerializeField]
        private GradientDirection Direction = GradientDirection.Vertical;

        public override void ModifyMesh(VertexHelper vh)
        {
            if (!IsActive())
            {
                return;
            }

            UIVertex uiVertex = new UIVertex();
            int vertexCount = vh.currentVertCount;
            if (Mode == GradientMode.SingleLine)
            {
                for (int i = 0; i < vertexCount; ++i)
                {
                    vh.PopulateUIVertex(ref uiVertex, i);

                    //垂直颜色渐变
                    if (Direction == GradientDirection.Vertical)
                    {
                        if (i % 4 == 0 || (i - 1) % 4 == 0)
                            uiVertex.color = firstColor;
                        else
                            uiVertex.color = secondColor;
                    }
                    else if (Direction == GradientDirection.Horizontal)
                    {
                        if (i % 4 == 0 || (i + 1) % 4 == 0)
                            uiVertex.color = firstColor;
                        else
                            uiVertex.color = secondColor;
                    }

                    vh.SetUIVertex(uiVertex, i);
                }
            }
            else if (Mode == GradientMode.Segment)
            {
                if (vertexCount >= 4)
                {
                    UIVertex firstVertex = new UIVertex();
                    vh.PopulateUIVertex(ref firstVertex, 0);
                    UIVertex lastVertex = new UIVertex();
                    vh.PopulateUIVertex(ref lastVertex, vertexCount - 2);

                    float factorA = Direction == GradientDirection.Vertical ? firstVertex.position.y : firstVertex.position.x;
                    float factorB = Direction == GradientDirection.Vertical ? lastVertex.position.y : lastVertex.position.x;

                    float distance = factorA - factorB;

                    for (int i = 0; i < vertexCount; ++i)
                    {
                        vh.PopulateUIVertex(ref uiVertex, i);
                        if (Direction == GradientDirection.Vertical)
                        {
                            uiVertex.color = Color32.Lerp(secondColor, firstColor, (uiVertex.position.y - lastVertex.position.y) / distance);
                        }
                        else
                            uiVertex.color = Color32.Lerp(secondColor, firstColor, (uiVertex.position.x - lastVertex.position.x) / distance);

                        vh.SetUIVertex(uiVertex, i);
                    }

                }
            }
        }
    }

    /// <summary>
    /// 渐变模式
    /// </summary>
    public enum GradientMode
    {
        SingleLine,         //单行渐变
        Segment,            //整体渐变
    }

    /// <summary>
    /// 渐变方向
    /// </summary>
    public enum GradientDirection
    {
        Vertical,           //垂直
        Horizontal          //水平
    }

}