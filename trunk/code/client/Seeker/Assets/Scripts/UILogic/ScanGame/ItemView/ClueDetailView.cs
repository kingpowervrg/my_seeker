using EngineCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace SeekerGame
{
    public class ClueDetailView : GameUIComponent
    {
        long m_clue_id;
        public long Clue_id
        {
            get { return m_clue_id; }
        }
        GameImage m_icon;
        GameLabel m_desc_txt;

        TweenPosition m_show_tween;
        protected override void OnInit()
        {

            m_icon = this.Make<GameImage>("Image (1)");
            m_desc_txt = this.Make<GameLabel>("Text");

            m_show_tween = this.GetComponent<TweenPosition>();

        }

        public override void OnShow(object param)
        {
            base.OnShow(param);
            m_show_tween.AddTweenCompletedCallback(TweenFinished);


        }

        public override void OnHide()
        {
            base.OnHide();
            m_show_tween.RemoveTweenCompletedCallback(TweenFinished);
        }


        public void Refresh(long clue_id_, string icon_, string desc_)
        {
            m_clue_id = clue_id_;
            m_icon.Sprite = icon_;
            m_desc_txt.Text = desc_;

        }

        void TweenFinished()
        {
            GameEvents.UIEvents.UI_Scan_Event.Listen_ShowFlyIconItemView.SafeInvoke(m_clue_id);
            GameEvents.UIEvents.UI_Scan_Event.Listen_RecycleDetailItemView.SafeInvoke(this);
        }


    }

}
