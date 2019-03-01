using EngineCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace SeekerGame
{
    public class FlyIconItemView : GameUIComponent
    {
        long m_clue_id;
        public long Clue_id
        {
            get { return m_clue_id; }
        }
        GameImage m_icon;
        TweenPosition m_tween_pos;

        protected override void OnInit()
        {

            m_icon = this.Make<GameImage>("Image (1)");
            m_tween_pos = this.gameObject.GetComponent<TweenPosition>();
        }

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


        public void Refresh(long clue_id_, string icon_, Vector3 pos_from_, Vector3 pos_to_)
        {
            m_clue_id = clue_id_;
            m_icon.Sprite = icon_;
            m_tween_pos.From = pos_from_;
            m_tween_pos.To = pos_to_;
        }

        void TweenFinished()
        {
            GameEvents.UIEvents.UI_Scan_Event.Listen_AddClueProgress.SafeInvoke(m_clue_id);
            GameEvents.UIEvents.UI_Scan_Event.Listen_RecycleFlyIconItemView.SafeInvoke(this);
        }

    }
}