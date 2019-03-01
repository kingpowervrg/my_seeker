using System;
using System.Collections.Generic;
using EngineCore;
using UnityEngine;

namespace SeekerGame
{
    [UILogicHandler(UIDefine.UI_WaveTips)]
    public class WaveTipUILogic : UILogicBase
    {
        private GameUIContainer m_grid = null;

        private Queue<WaveTipComponent> m_unUseTip = new Queue<WaveTipComponent>();
        protected override void OnInit()
        {
            base.OnInit();
            this.m_grid = Make<GameUIContainer>("grid");
        }

        public override void OnShow(object param)
        {
            base.OnShow(param);
            if (param != null)
            {
                WaveTipData waveData = param as WaveTipData;
                TimeModule.Instance.SetTimeout(()=> {
                    ShowTips(waveData.content, waveData.postion, waveData.value);
                }, 0.1f);
            }
            GameEvents.UIEvents.UI_WaveTip_Event.OnShowTips += OnShowTips;
        }

        public override void OnHide()
        {
            base.OnHide();
            GameEvents.UIEvents.UI_WaveTip_Event.OnShowTips -= OnShowTips;
        }

        private void OnShowTips(WaveTipData waveData)
        {
            ShowTips(waveData.content, waveData.postion,waveData.value);
        }

        private WaveTipComponent getUseTips()
        {
            if (m_unUseTip.Count > 0)
            {
                return m_unUseTip.Dequeue();
            }
            else
            {
                if (this.m_grid.ChildCount >= 10)
                    return null;
                return this.m_grid.AddChild<WaveTipComponent>();
            }
        }

        private void ShowTips(string content,Vector3 postion,float value)
        {
            WaveTipComponent tipCon = getUseTips();
            if (tipCon == null)
                return;
            Vector2 localPos;
            Vector2 screenPos = RectTransformUtility.WorldToScreenPoint(CameraManager.Instance.UICamera, postion);
            RectTransformUtility.ScreenPointToLocalPointInRectangle(m_grid.Widget, screenPos, CameraManager.Instance.UICamera, out localPos);
            tipCon.Widget.anchoredPosition = localPos;
            OnStartTips(tipCon, content, value);
            tipCon.Visible = true;
            
        }

        private void OnStartTips(WaveTipComponent tipcon,string content,float value)
        {
            tipcon.SetData(content,value, () =>
            {
                tipcon.Visible = false;
                this.m_unUseTip.Enqueue(tipcon);
            });
        }

        public override FrameDisplayMode UIFrameDisplayMode
        {
            get
            {
                return FrameDisplayMode.WINDOWED;
            }
        }


        public class WaveTipComponent : GameUIComponent
        {
            private GameLabel m_contentLab = null;
            private TweenPosition m_tweenPos = null;
            //private TweenAlpha[] m_TweenAlphas = null;
            //private TweenScale m_tweenScale = null;
            private float m_hei = 50f;
            private Action m_complete = null;
            protected override void OnInit()
            {
                base.OnInit();
                this.m_contentLab = Make<GameLabel>("Text");
                this.m_tweenPos = GetComponent<TweenPosition>();
                //this.m_TweenAlphas = GetComponents<TweenAlpha>();
                //this.m_tweenScale = GetComponent<TweenScale>();
            }

            public override void OnHide()
            {
                base.OnHide();
                //TimeModule.Instance.RemoveTimeaction(OnComplete);
            }

            public void SetData(string content,float value,Action callback)
            {
                if (value >= 0)
                {
                    m_contentLab.Text = LocalizeModule.Instance.GetString(content, value);
                }
                else
                {
                    m_contentLab.Text = LocalizeModule.Instance.GetString(content);
                }
                m_tweenPos.From = Widget.anchoredPosition;
                m_tweenPos.To = Widget.anchoredPosition.x * Vector3.right + (Widget.anchoredPosition.y + m_hei) * Vector3.up;
                //m_tweenPos.ResetAndPlay();
                //this.m_tweenScale.ResetAndPlay();
                //for (int i = 0; i < m_TweenAlphas.Length; i++)
                //{
                //    m_TweenAlphas[i].ResetAndPlay();
                //}
                this.m_complete = callback;
                TimeModule.Instance.SetTimeout(OnComplete, 2f);
            }

            private void OnComplete()
            {
                if (this.m_complete != null)
                {
                    this.m_complete();
                }
            }
        }
    }

    public class WaveTipData
    {
        public string content;
        public Vector3 postion;
        public float value;
        public WaveTipData(string content, Vector3 postion,float value)
        {
            this.content = content;
            this.postion = postion;
            this.value = value;
        }
    }

    
}
