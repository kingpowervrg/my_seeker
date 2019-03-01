using EngineCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace SeekerGame
{
    public class ActivityComponent : GameUIComponent
    {
        private TweenPosition tween;
        private TweenScale m_TweenScale;
        private TweenAlpha m_TweenAlpha;
        private GameLabel m_desc_lab;
        private GameImage m_tips_img;
        private GameTexture m_icon_img;
        private GameUIComponent m_click_btn;

        private int m_Index = -1;
        private float m_delayTime;

        private int m_nextIndex = -1;
        private bool m_isChoose = false;

        private float m_normalDuration = 0.3f; //正常情况下移动时间

        public bool GetChooseState()
        {
            return m_isChoose;
        }
        public void SetChooseState(bool choose)
        {
            m_isChoose = choose;
        }

        protected override void OnInit()
        {
            base.OnInit();
            tween = GetComponent<TweenPosition>();
            m_TweenScale = GetComponent<TweenScale>();
            m_TweenAlpha = GetComponent<TweenAlpha>();
            m_icon_img = Make<GameTexture>(gameObject);
            m_click_btn = Make<GameUIComponent>("Btn");
            m_desc_lab = m_icon_img.Make<GameLabel>("Text");
            m_tips_img = m_icon_img.Make<GameImage>("Image");

        }

        public override void OnShow(object param)
        {
            base.OnShow(param);
            m_click_btn.AddClickCallBack(BtnClick);
            m_TweenAlpha.SetTweenCompletedCallback(OnChooseFinish);
            m_normalDuration = tween.Duration;
            //tween.AddOnFinished(OnTweenFinish);
        }

        public override void OnHide()
        {
            base.OnHide();
            m_click_btn.RemoveClickCallBack(BtnClick);
            m_TweenAlpha.SetTweenCompletedCallback(null);
            m_nextIndex = -1;
            m_isChoose = false;
            ResetDuration();
            //tween.RemoveOnFinished(OnTweenFinish);
        }

        public void SetData(ActivityBaseInfo data, Vector3 targetPos, int index, float delayTime)
        {
            if (data == null)
            {
                return;
            }
            m_delayTime = delayTime;
            m_Index = index;
            tween.From = Widget.anchoredPosition3D;
            tween.To = targetPos;
            m_icon_img.TextureName = data.Icon;
            m_desc_lab.Text = LocalizeModule.Instance.GetString(data.Descs);
            m_tips_img.Visible = true;
            if (data.Type == 1)
            {
                m_tips_img.Sprite = "lable_type5_2.png";
            }
            else if (data.Type == 2)
            {
                m_tips_img.Sprite = "lable_type5_1.png";
            }
            else
            {
                m_tips_img.Visible = false;
            }
        }

        private void BtnClick(GameObject obj)
        {
            EngineCoreEvents.AudioEvents.PlayAudio.SafeInvoke(Audio.AudioType.UISound, EngineCommonAudioKey.Button_Click_Common.ToString());

            GameEvents.UIEvents.UI_Activity_Event.OnClick.SafeInvoke(m_Index);
        }

        public void PlayTween()
        {
            if (m_isChoose)
            {
                m_delayTime = tween.Duration;
            }
            //tween.To =Vector3.zero;
            tween.PlayForward();
            TimeModule.Instance.SetTimeout(OnNextTween, m_delayTime);
        }

        public void PlayTween(Vector3 endPos)
        {
            tween.From = Widget.anchoredPosition3D;
            tween.To = endPos;
            tween.PlayForward();
            //tween.
        }

        public void SetEndPos(Vector3 endpos)
        {
            tween.From = Widget.anchoredPosition3D;
            tween.To = endpos;
        }

        public void SetEndPos(Vector3 endPos, int nextIndex)
        {
            SetEndPos(endPos);
            m_nextIndex = nextIndex;
        }

        public int GetNextIndex()
        {
            return m_nextIndex;
        }

        public void PlayChooseEffect()
        {
            m_TweenScale.PlayForward();
            m_TweenAlpha.PlayForward();
        }

        private void OnNextTween()
        {
            GameEvents.UIEvents.UI_Activity_Event.OnTweenFinish.SafeInvoke(m_Index);
        }

        private void OnChooseFinish()
        {
            Visible = false;
            GameEvents.UIEvents.UI_Activity_Event.OnChooseFinish.SafeInvoke(m_Index);
        }

        public void ResetDuration()
        {
            tween.Duration = m_normalDuration;
        }

        public void SetDuration(float duration)
        {
            tween.Duration = duration;
        }
        //private void OnTweenFinish()
        //{

        //}
    }
}
