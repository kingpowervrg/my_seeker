using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using EngineCore;

namespace SeekerGame
{
    public class BezierFlyIcons : GameUIComponent
    {
        public int resolution;

        GameUIContainer m_fly_icon_grid;

        GameUIComponent anchor_from;
        GameUIComponent anchor_middle1;
        GameUIComponent anchor_middle2;
        GameUIComponent anchor_to;

        /// <summary>
        /// middle距离出发点的X距离
        /// </summary>
        float anchor_dis_x_from2to;
        float anchor_dis_x_from2middle1;
        float anchor_dis_x_from2middle2;

        protected override void OnInit()
        {
            base.OnInit();
        }

        public override void OnShow(object param)
        {
            throw new NotImplementedException();
        }

        public override void OnHide()
        {
            throw new NotImplementedException();
        }


        void InitLerp()
        {
            anchor_dis_x_from2to = Mathf.Abs(anchor_to.gameObject.transform.position.x - anchor_from.gameObject.transform.position.x);
            anchor_dis_x_from2middle1 = Mathf.Abs(anchor_middle1.gameObject.transform.position.x - anchor_from.gameObject.transform.position.x);
            anchor_dis_x_from2middle2 = Mathf.Abs(anchor_middle2.gameObject.transform.position.x - anchor_from.gameObject.transform.position.x);

        }
    }
}
