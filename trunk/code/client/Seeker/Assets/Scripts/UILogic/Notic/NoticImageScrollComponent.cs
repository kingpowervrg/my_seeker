using EngineCore;
using System;
using System.Collections.Generic;
using UnityEngine;
namespace SeekerGame
{
    public class NoticImageScrollComponent : GameUIComponent
    {

        private List<NoticeInfo> m_noticInfo = null;
        private GameUIContainer m_container = null;
        protected override void OnInit()
        {
            base.OnInit();
            this.m_container = Make<GameUIContainer>(gameObject);
        }

        public override void OnShow(object param)
        {
            base.OnShow(param);
        }

        public void SetData(List<NoticeInfo> info)
        {
            this.m_noticInfo = info;
            CreatePanel();
        }

        private void CreatePanel()
        {
            int count = this.m_noticInfo.Count;

            int num = count / 2;

            int leftNum = num; //左边个数
            int rightNum = num - 1 + count % 2; //右边个数
            this.m_container.EnsureSize<NoticImageSingleComponent>(this.m_noticInfo.Count);
            for (int i = 0; i < count; i++)
            {
                NoticImageSingleComponent noticImg = this.m_container.GetChild<NoticImageSingleComponent>(i);
                if (i < num)
                {
                    noticImg.SetData(num - i, 0, this.m_noticInfo[i]);
                }
                else
                {
                    int index = i - num;
                    if (index < rightNum)
                    {
                        noticImg.SetData(rightNum - index, 1, this.m_noticInfo[i]);
                    }
                    else
                    {
                        noticImg.SetData(0, 2, this.m_noticInfo[i]);
                    }
                }
                noticImg.SetMaxData(leftNum, rightNum);
                noticImg.Visible = true;
            }
        }
    }

    public class NoticImageSingleComponent : GameUIComponent
    {
        private int index;
        private int type; //0 左边  1右边 2中间

        private float m_scaleFactor = 0.2f;
        private float m_alphaFactor = 0.2f;
        private float m_myPosFactor = 60f;
        private float m_posFactor = 40f;


        private TweenScale m_tweenPos = null;
        private TweenAlpha m_TweenAlpha = null;
        private TweenScale m_TweenScale = null;
        private CanvasGroup m_groupAlpha = null;

        private GameTexture m_texture = null;
        private GameLabel m_timeLab = null;

        private NoticeInfo m_currentNoticInfo = null;

        private int m_leftNum;
        private int m_rightNum;
        protected override void OnInit()
        {
            base.OnInit();
            this.m_tweenPos = GetComponent<TweenScale>();
            this.m_TweenAlpha = GetComponent<TweenAlpha>();
            this.m_TweenScale = GetComponent<TweenScale>();
            this.m_groupAlpha = GetComponent<CanvasGroup>();
            this.m_texture = Make<GameTexture>(gameObject);
        }

        public override void OnShow(object param)
        {
            base.OnShow(param);
            this.m_texture.AddDragEndCallBack(OnDragEnd);
            GameEvents.UIEvents.UI_Activity_Event.OnDragTexure += OnDragTexure;
            this.m_tweenPos.AddTweenCompletedCallback(TweenFinish);
        }

        public override void OnHide()
        {
            base.OnHide();
            this.m_texture.RemoveDragEndCallBack(OnDragEnd);
            GameEvents.UIEvents.UI_Activity_Event.OnDragTexure -= OnDragTexure;
            this.m_tweenPos.RemoveTweenCompletedCallback(TweenFinish);
        }

        public void SetMaxData(int leftNum, int rightNum)
        {
            this.m_leftNum = leftNum;
            this.m_rightNum = rightNum;
        }

        public void SetData(int index, int type, NoticeInfo noticeInfo)
        {
            this.index = index;
            this.type = type;
            this.m_currentNoticInfo = noticeInfo;
            this.m_texture.TextureName = noticeInfo.Picture;

            if (type == 0)
            {
                m_posFactor = -m_myPosFactor;
            }

            Widget.anchoredPosition = Vector2.right * m_posFactor * index + Vector2.up * Widget.anchoredPosition.y;
            this.m_groupAlpha.alpha -= m_alphaFactor * index;
            Widget.localScale = Vector3.one - Vector3.up * m_scaleFactor * index;
            //ReflashTweener();
        }

        private void ReflashTweener()
        {
            float m_posFactor = m_myPosFactor;
            if (type == 0)
            {
                m_posFactor = -m_myPosFactor;
            }
            this.m_tweenPos.From = Widget.anchoredPosition;
            this.m_TweenAlpha.From = this.m_groupAlpha.alpha;
            this.m_TweenScale.From = Widget.localScale;

            this.m_tweenPos.To = Vector2.right * m_posFactor * index + Vector2.up * Widget.anchoredPosition.y;
            this.m_TweenAlpha.To = 1 - m_alphaFactor * index;
            this.m_TweenScale.To = Vector3.one - Vector3.up * m_scaleFactor * index;
            //float m_posFactor = 0f;
            //if (type == 0)
            //{
            //    m_posFactor = -m_myPosFactor;
            //}
            //if (type == 2)
            //{
            //    this.m_tweenPos.From = Widget.anchoredPosition;
            //    this.m_tweenPos.to.x = Widget.anchoredPosition.x + m_myPosFactor;

            //    //this.m_tweenPos.to.x += m_posFactor * (index + 1);
            //    this.m_TweenAlpha.to -= m_alphaFactor * (index + 1);
            //    this.m_TweenScale.to.y -= m_scaleFactor * (index + 1);
            //    Debug.Log("m_tweenPos : " + this.m_tweenPos.to + "   " + index + "  " + m_posFactor + "  " + type);
            //    return;
            //}
            //this.m_tweenPos.From = Widget.anchoredPosition;
            //this.m_tweenPos.to.x = this.m_tweenPos.From.x + m_myPosFactor;

            ////this.m_tweenPos.From.x += m_posFactor * index;
            ////this.m_tweenPos.to.x += m_posFactor * (index - 1);

            //this.m_TweenAlpha.From -= m_alphaFactor * index;
            //this.m_TweenAlpha.to -= m_alphaFactor * (index - 1);


            //this.m_TweenScale.From.y -= m_scaleFactor * index;
            //this.m_TweenScale.to.y -= m_scaleFactor * (index - 1);
            //Debug.Log("m_tweenPos : " + this.m_tweenPos.to + "   " + index + "  " + m_posFactor + "  " + type);
        }

        private void OnDragEnd(GameObject go, Vector2 delta)
        {
            GameEvents.UIEvents.UI_Activity_Event.OnDragTexure.SafeInvoke(0);
        }

        private int m_dragType = 0;

        private void OnDragTexure(int dragtype)
        {
            this.m_dragType = dragtype;
            //Debug.Log("drag ==== ");
            CalculationTypeAndIndex();
            ReflashTweener();
            if (absIndex == 0)
            {
                Widget.SetSiblingIndex(absIndex);
            }
            else if (absIndex == this.m_rightNum + this.m_leftNum)
            {
                //Widget.SetSiblingIndex(absIndex);
                TimeModule.Instance.SetTimeout(() =>
                {
                    //Debug.Log(" ======  absIndex: " + absIndex);
                    Widget.SetAsLastSibling();
                }, this.m_tweenPos.Duration / 2f);
            }
            m_tweenPos.ResetAndPlay();
            m_TweenAlpha.ResetAndPlay();
            m_TweenScale.ResetAndPlay();
        }

        private int absIndex = 0;

        private void CalculationTypeAndIndex()
        {
            if (m_dragType == 0)
            {
                if (this.type == 0)
                {
                    absIndex = this.m_leftNum - this.index;
                    absIndex += 1;
                    this.index--;
                    if (absIndex >= this.m_leftNum) //到边啦
                    {
                        absIndex = this.m_leftNum + this.m_rightNum;
                        this.index = 0;
                        this.type = 2;
                    }
                    //Debug.Log("left ======== " + absIndex);
                }
                else if (this.type == 2)
                {
                    this.index = 1;
                    this.type = 1;
                    absIndex = this.m_leftNum + this.m_rightNum - 1;
                    //Debug.Log("center ======== " + absIndex);
                }
                else if (this.type == 1)
                {
                    absIndex = this.m_rightNum - this.index + this.m_leftNum;
                    absIndex -= 1;
                    this.index++;
                    if (absIndex < this.m_leftNum) //到边啦
                    {
                        absIndex = 0;
                        this.index = this.m_leftNum;
                        this.type = 0;
                    }
                    //Debug.Log("right ======== " + absIndex);
                }

            }
        }

        private void TweenFinish()
        {
            if (absIndex != 0 && absIndex != this.m_rightNum + this.m_leftNum)
            {
                TimeModule.Instance.SetTimeout(() =>
                {
                    //Debug.Log("tweenFinish ===-------- " + index + "  " + type + " absIndex:" + absIndex);
                    Widget.SetSiblingIndex(absIndex);
                }, 0.01f * absIndex);
            }

        }

    }
}
