using System;
using System.Collections.Generic;
using EngineCore;
using UnityEngine;
using DG.Tweening;
namespace SeekerGame
{
    public class ReasonContentComponent : GameUIComponent
    {
        private GameLabel m_descLab = null;
        private GameUIContainer m_container = null;

        private GameLabel m_itemDescLab = null;
        private TextFader m_itemDescFader = null;
        private GameButton m_okBtn = null;

        private GameUIComponent m_itemDescCom = null;
        private GameUIComponent m_togCom = null;
        private ConfNode m_confNode;
        private GameImage m_flyIconImg = null;

        private GameLabel m_feedBackLab = null;

        private int m_currentIndex = 0;
        protected override void OnInit()
        {
            base.OnInit();
            this.m_descLab = Make<GameLabel>("Text");
            this.m_itemDescLab = Make<GameLabel>("Image:Text");
            this.m_itemDescFader = this.m_itemDescLab.GetComponent<TextFader>();
            this.m_okBtn = Make<GameButton>("Image:Button");
            this.m_container = Make<GameUIContainer>("ScrollView:Viewport");
            this.m_itemDescCom = Make<GameUIComponent>("Image");
            this.m_togCom = Make<GameUIComponent>("ScrollView");
            this.m_feedBackLab = Make<GameLabel>("feedBack");
            this.m_flyIconImg = Make<GameImage>("flyIcon");
        }

        public override void OnShow(object param)
        {
            base.OnShow(param);
            this.m_okBtn.Enable = true;
            this.m_okBtn.AddClickCallBack(OnOk);
            this.m_currentIndex = 0;
        }

        public override void OnHide()
        {
            base.OnHide();
            if (this.m_flyTween != null)
            {
                this.m_flyTween.Kill();
            }
            this.m_okBtn.RemoveClickCallBack(OnOk);
            PlayTween(false);
            this.m_flyIconImg.Visible = false;
            this.m_feedBackLab.Visible = false;
            TimeModule.Instance.RemoveTimeaction(PlayOKFeedBack);
            TimeModule.Instance.RemoveTimeaction(PlayErrorFeedBack);
        }

        public void SetData(ConfNode confNode)
        {
            this.m_confNode = confNode;
            this.m_descLab.Text = LocalizeModule.Instance.GetString(confNode.descs);
            this.m_itemDescLab.Text = confNode.cluedescs[0];
            int nodeCount = confNode.clueicons.Length;
            this.m_container.EnsureSize<ReasonContentToggle>(nodeCount);
            for (int i = 0; i < nodeCount; i++)
            {
                ReasonContentToggle reasonTog = this.m_container.GetChild<ReasonContentToggle>(i);
                reasonTog.SetData(confNode.clueicons[i], i, OnToggle);
                reasonTog.Visible = true;
            }
        }

        public void PlayTween(bool visible = true)
        {
            if(visible)
                this.m_okBtn.Enable = true;
            this.m_descLab.Visible = visible;
            this.m_itemDescCom.Visible = visible;
            this.m_togCom.Visible = visible;
        }

        private void PlayErrorFeedBack()
        {
            this.m_feedBackLab.Visible = false;
            //TimeModule.Instance.SetTimeout();
            PlayTween(true);
        }

        private void PlayOKFeedBack()
        {
            this.m_feedBackLab.Visible = false;
            GameEvents.UIEvents.UI_Reason_Event.OnCheckReasonItem.SafeInvoke(true);
        }

        private Tweener m_flyTween = null;
        private void OnOk(GameObject obj)
        {
            this.m_okBtn.Enable = false;
            PlayTween(false);
            if (this.m_currentIndex + 1 == this.m_confNode.answer)
            {
                this.m_feedBackLab.Text = LocalizeModule.Instance.GetString(this.m_confNode.feedback);

                ReasonContentToggle currentTog = this.m_container.GetChild<ReasonContentToggle>(this.m_currentIndex);
                Vector3 startPos = currentTog.IconPosition;
                Vector3 endPos = GameEvents.UIEvents.UI_Reason_Event.OnGetReasonItemPosition.SafeInvoke();
                Vector3[] pathPoint = new Vector3[3];
                pathPoint[0] = startPos;
                pathPoint[1] = (startPos + endPos) / 2f + Vector3.up * 80f;
                pathPoint[2] = endPos;
                string currentIconName = this.m_confNode.clueicons[m_currentIndex];
                this.m_flyIconImg.Position = startPos;
                this.m_flyIconImg.Sprite = currentIconName;
                this.m_flyIconImg.Visible = true;
                currentTog.EnableIcon = false;
                m_flyTween = this.m_flyIconImg.Widget.DOPath(pathPoint, 1.5f, PathType.CatmullRom, PathMode.TopDown2D).SetEase(Ease.OutSine).OnComplete(()=> {
                    if (CachedVisible)
                    {
                        this.m_flyIconImg.Visible = false;
                        GameEvents.UIEvents.UI_Reason_Event.OnReasonIconVisible.SafeInvoke(currentIconName, true);
                    }
                });
                this.m_flyIconImg.Widget.DOScale(0.8f, 1.5f).SetEase(Ease.OutSine);
                TimeModule.Instance.SetTimeout(PlayOKFeedBack, 2.5f);
            }
            else
            {
                this.m_feedBackLab.Text = LocalizeModule.Instance.GetString("reasoning_feedback_fail ");
                GameEvents.UIEvents.UI_Reason_Event.OnCheckReasonItem.SafeInvoke(false);
                TimeModule.Instance.SetTimeout(PlayErrorFeedBack, 2f);
            }
            this.m_feedBackLab.Visible = true;
        }

        private void OnToggle(int index)
        {
            this.m_currentIndex = index;
            this.m_itemDescFader.enabled = false;
            this.m_itemDescFader.enabled = true;
            this.m_itemDescLab.Text = m_confNode.cluedescs[index];
        }

        private class ReasonContentToggle : GameUIComponent
        {
            private GameImage m_iconImg = null;
            private int index = -1;
            private GameToggleButton m_tog = null;
            private GameUIComponent m_arrowImg = null;
            private Action<int> onOk = null;
            protected override void OnInit()
            {
                base.OnInit();
                this.m_iconImg = Make<GameImage>("Toggle_1:Image_icon");
                this.m_tog = Make<GameToggleButton>("Toggle_1");
                this.m_arrowImg = Make<GameUIComponent>("Toggle_1:Background:Checkmark:Image");
            }

            public override void OnShow(object param)
            {
                base.OnShow(param);
                this.m_tog.AddChangeCallBack(OnToggle);
            }

            public override void OnHide()
            {
                base.OnHide();
                this.m_tog.RemoveChangeCallBack(OnToggle);
                this.m_arrowImg.Visible = false;
            }

            public void SetData(string icon,int index,Action<int> onclick)
            {
                this.m_iconImg.Visible = true;
                this.m_arrowImg.Visible = index == 0;
                this.m_tog.Checked = index == 0;
                
                this.m_iconImg.Sprite = icon;
                this.index = index;
                this.onOk = onclick;
            }

            private void OnToggle(bool flag)
            {
                this.m_arrowImg.Visible = flag;
                if (flag && this.onOk != null)
                {
                    this.onOk(index);
                }
            }

            public Vector3 IconPosition
            {
                get
                {
                    return this.m_iconImg.Position;
                }
            }

            public bool EnableIcon
            {
                set { this.m_iconImg.Visible = value; }
            }
        }

    }
}
