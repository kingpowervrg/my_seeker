using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace GOGUI
{

    /// <summary>
    /// 图文混排
    /// </summary>
    [AddComponentMenu("UI/Empty Graphic", 15)]
    public class EmptyGraphic : Graphic
    {
        protected override void OnPopulateMesh(VertexHelper vh)
        {
            vh.Clear();
        }
    }
}