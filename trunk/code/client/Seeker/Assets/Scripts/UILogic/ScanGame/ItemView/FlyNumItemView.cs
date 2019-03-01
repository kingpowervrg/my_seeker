using EngineCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace SeekerGame
{
    public class FlyNumItemView : GameUIComponent
    {


        GameLabel m_num_txt;
        TweenAlpha m_tween_alpha;

        protected override void OnInit()
        {
            m_num_txt = this.Make<GameLabel>("Text");
            m_tween_alpha = this.GetComponent<TweenAlpha>();
        }

        public override void OnShow(object param)
        {
            base.OnShow(param);

            m_tween_alpha.AddTweenCompletedCallback(OnTweenFinished);
        }

        public override void OnHide()
        {
            base.OnHide();

            m_tween_alpha.RemoveTweenCompletedCallback(OnTweenFinished);
        }


        public void Refresh(int num_)
        {
            m_num_txt.Text = $"+{num_.ToString()}";

        }


        void OnTweenFinished()
        {
            GameEvents.UIEvents.UI_Scan_Event.Listen_RecycleFlyVitNumItemView.SafeInvoke(this);
        }

    }

}
