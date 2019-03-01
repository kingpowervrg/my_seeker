//#define TEST
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EngineCore;
using UnityEngine;
using GOEngine;
using Google.Protobuf;
using DG.Tweening;
using DG.Tweening.Plugins.Options;

namespace SeekerGame
{
    public class ScanResultView : BaseViewComponet<ScanGameUILogic>
    {

        GameLabel m_progress_txt;
        TweenPosition m_hide_tween;

        protected override void OnInit()
        {
            base.OnInit();
            m_progress_txt = Make<GameLabel>("Text (1):Text (2)");

            TweenPosition[] all_ts = this.gameObject.GetComponents<TweenPosition>();

            foreach (var ts in all_ts)
            {
                if (UITweenerBase.TweenTriggerType.OnHide == ts.m_triggerType)
                {
                    m_hide_tween = ts;
                    break;
                }
            }


        }



        public override void OnShow(object param)
        {
            base.OnShow(param);

            m_hide_tween.AddTweenCompletedCallback(OnTweenFinished);
        }

        public override void OnHide()
        {
            base.OnHide();

            m_hide_tween.RemoveTweenCompletedCallback(OnTweenFinished);
        }

        public void Refresh(float progress_)
        {
            int progress = (int)progress_ * 100;
            m_progress_txt.Text = progress.ToString();
        }


        public void OnTweenFinished()
        {
            GameEvents.UIEvents.UI_Scan_Event.Listen_ShowReward.SafeInvoke();
        }

    }


}