using EngineCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace SeekerGame
{
    public class FlyVitEffectItemView : GameUIComponent
    {
        int m_vit_add;
        public int Vit_add
        {
            get { return m_vit_add; }
        }
        protected GameUIEffect m_effect;
        protected TweenPosition m_tween_pos;



        public override void OnShow(object param)
        {
            base.OnShow(param);
            m_tween_pos.AddTweenCompletedCallback(TweenFinished);

        }

        public override void OnHide()
        {
            base.OnHide();
            m_tween_pos.RemoveTweenCompletedCallback(TweenFinished);
        }


        public void Refresh(int add_vit_, Vector3 pos_from_, Vector3 pos_to_)
        {
            m_vit_add = add_vit_;
            m_tween_pos.From = pos_from_;
            m_tween_pos.To = pos_to_;
        }

        void TweenFinished()
        {
            GameEvents.UIEvents.UI_Scan_Event.Listen_VitEffectFinishFly.SafeInvoke(this);
        }

    }
}