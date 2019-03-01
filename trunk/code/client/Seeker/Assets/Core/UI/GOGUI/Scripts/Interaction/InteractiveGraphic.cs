/********************************************************************
	created:  2018-12-12 20:5:30
	filename: InteractiveGraphic.cs
	author:	  songguangze@outlook.com
	
	purpose:  可交互Panel,可响应事件
*********************************************************************/
using UnityEngine.UI;

namespace GOGUI
{
    public class InteractiveGraphic : Graphic
    {
        public override void SetVerticesDirty()
        {
        }

        public override void SetMaterialDirty()
        {
        }

        protected override void OnPopulateMesh(UnityEngine.UI.VertexHelper vh)
        {
            vh.Clear();
        }
    }
}