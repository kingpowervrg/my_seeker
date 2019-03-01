using DG.Tweening;
using EngineCore;
using System;
using System.Collections.Generic;
using UnityEngine;
namespace SeekerGame
{
    [UILogicHandler(UIDefine.UI_REASONING)]
    public class ReasoningUILogic : UILogicBase
    {
        private GameLabel m_titleLab = null;
        private GameLabel m_currentProgressLab = null;
        private GameUIComponent m_btnClose = null;

        private GameUIContainer m_container = null;
        private ReasonContentComponent reasonContentCom = null;
        private ConfReasoning m_confReason = null;
        public static float m_spaceX = 140f;
        public static float m_spaceY = 140f;

        public static int m_currentIndex;

        public static Vector2 m_centerPos;
        protected override void OnInit()
        {
            base.OnInit();
            this.m_titleLab = Make<GameLabel>("Panel_animation:Panel_top:ChapterName");
            this.m_currentProgressLab = Make<GameLabel>("Panel_animation:Panel_top:TaskNum");

            this.m_container = Make<GameUIContainer>("Panel_animation:Panel_lift");
            this.reasonContentCom = Make<ReasonContentComponent>("Panel_animation:Panel_right");
            this.m_btnClose = Make<GameUIComponent>("Panel_animation:Panel_top:Button_close");
        }

        public override void OnShow(object param)
        {
            base.OnShow(param);
            GameEvents.UIEvents.UI_Reason_Event.OnCheckReasonItem += OnNextReasonItem;
            GameEvents.UIEvents.UI_Reason_Event.OnGetReasonItemPosition = OnGetReasonItemPosition;
            GameEvents.UIEvents.UI_Reason_Event.OnReasonIconVisible += OnReasonIconVisible;
            this.m_btnClose.AddClickCallBack(OnClose);
            m_currentIndex = 0;
            long currentReasonId = (long)param;
            ConfReasoning confReason = ConfReasoning.Get(currentReasonId);
            this.m_titleLab.Text = LocalizeModule.Instance.GetString(confReason.name);
            this.m_currentProgressLab.Text = (m_currentIndex + 1) + "/" + confReason.nodes.Length;
            InitData(confReason);
            TimeModule.Instance.SetTimeout(() =>
            {
                reasonContentCom.PlayTween(true);
            }, 0.8f);
        }
        public override FrameDisplayMode UIFrameDisplayMode
        {
            get
            {
                return FrameDisplayMode.FULLSCREEN;
            }
        }

        public override void OnHide()
        {
            base.OnHide();
            GameEvents.UIEvents.UI_Reason_Event.OnCheckReasonItem -= OnNextReasonItem;
            GameEvents.UIEvents.UI_Reason_Event.OnReasonIconVisible -= OnReasonIconVisible;
            this.m_btnClose.RemoveClickCallBack(OnClose);
            m_currentIndex = 0;
            //this.m_currentProgressLab.Text = (m_currentIndex + 1) + "/" + m_confReason.nodes.Length;
        }

        private void OnClose(GameObject obj)
        {
            PopUpManager.OnCloseSureUI("reasoning_exit_tips", "reasoning_exit_ta", "reasoning_exit_quit", () =>
            {
                HideReasonUI();
            });
            
        }

        public void InitData(ConfReasoning confReason)
        {
            this.m_confReason = confReason;
            int nodesLength = confReason.nodes.Length;
            m_centerPos = this.m_container.ContainerTemplate.GetComponent<RectTransform>().anchoredPosition;
            this.m_container.EnsureSize<ReasoningIcon>(nodesLength);
            for (int i = 0; i < nodesLength; i++)
            {
                if (i == 0)
                {
                    this.reasonContentCom.SetData(ConfNode.Get(confReason.nodes[i]));
                }
                ReasoningIcon reasonIcon = this.m_container.GetChild<ReasoningIcon>(i);
                reasonIcon.SetData(ConfNode.Get(confReason.nodes[i]), i, i == nodesLength - 1);
                reasonIcon.Visible = true;
            }
        }

        private void OnNextReasonItem(bool flag)
        {
            if (flag)
            {
                m_currentIndex++;
                if (m_currentIndex >= m_confReason.nodes.Length)
                {
                    CSReasoningRewardReq req = new CSReasoningRewardReq();
                    req.ReasoningId = m_confReason.id;
                    GameEvents.NetWorkEvents.SendAsyncMsg.SafeInvoke(req);
                    ReasoningUILogic.HideReasonUI();
                    return;
                }
                this.m_currentProgressLab.Text = (m_currentIndex + 1) + "/" + m_confReason.nodes.Length;
                for (int i = 0; i < m_confReason.nodes.Length; i++)
                {
                    ReasoningIcon reasonIcon = this.m_container.GetChild<ReasoningIcon>(i);
                    reasonIcon.OnNextReasonItem();
                }
                TimeModule.Instance.SetTimeout(() =>
                {
                    this.reasonContentCom.SetData(ConfNode.Get(m_confReason.nodes[m_currentIndex]));
                    this.reasonContentCom.PlayTween(true);
                }, 0.5f);
            }
            else
            {
                ReasoningIcon reasonIcon = this.m_container.GetChild<ReasoningIcon>(m_currentIndex);
                reasonIcon.PlayErrorTips();
            }

        }

        private Vector3 OnGetReasonItemPosition()
        {
            ReasoningIcon reasonIcon = this.m_container.GetChild<ReasoningIcon>(m_currentIndex);
            reasonIcon.EnableName = false;
            return reasonIcon.IconPosition;
        }

        private void OnReasonIconVisible(string iconName, bool flag)
        {
            ReasoningIcon reasonIcon = this.m_container.GetChild<ReasoningIcon>(m_currentIndex);
            reasonIcon.IconName = iconName;
            reasonIcon.EnableIcon = flag;
            reasonIcon.PlayIconScaleAnimator();
        }

        public static void ShowReasonUIById(long id)
        {
            FrameMgr.OpenUIParams param = new FrameMgr.OpenUIParams(UIDefine.UI_REASONING);
            param.Param = id;
            EngineCoreEvents.UIEvent.ShowUIEventWithParam.SafeInvoke(param);
        }

        public static void HideReasonUI()
        {
            EngineCoreEvents.UIEvent.HideUIEvent.SafeInvoke(UIDefine.UI_REASONING);
        }

        private class ReasoningIcon : GameUIComponent
        {
            private GameLabel m_nameLab = null;
            private GameImage m_iconImg = null;
            private GameImage m_lineImg = null;
            private GameImage m_bgImg = null;
            private GameImage m_iconEffect = null;

            private ConfNode m_confNode = null;
            private long m_reasonItemId;
            private int m_reasonItemIndex;
            private string m_Name;
            private bool m_isEnd = false;

            private Vector2 m_targetPos;
            private TweenPosition m_tweenPos = null;
            private TweenAlpha m_tweenAlpha = null;

            private TweenColor m_bgTweenColor = null;

            private TweenScale m_iconTweenScale = null;
            private Tweener tweener = null;
            private float m_delayTime = 0f;
            protected override void OnInit()
            {
                base.OnInit();
                this.m_nameLab = Make<GameLabel>("Text");
                this.m_iconImg = Make<GameImage>("Image");
                this.m_lineImg = Make<GameImage>("Line");
                this.m_bgImg = Make<GameImage>("BG");
                this.m_iconEffect = Make<GameImage>("IconEffect");

                this.m_tweenPos = GetComponent<TweenPosition>();
                this.m_tweenAlpha = GetComponent<TweenAlpha>();
                this.m_bgTweenColor = this.m_bgImg.GetComponent<TweenColor>();

                this.m_iconTweenScale = this.m_iconImg.GetComponent<TweenScale>();
                this.m_delayTime = this.m_tweenPos.Delay;
            }

            public override void OnHide()
            {
                base.OnHide();
                if (tweener != null)
                {
                    tweener.Kill();
                }

                if (tweenerColor != null)
                {
                    tweenerColor.Kill();
                }
                this.m_iconImg.Visible = false;
                //if (iconTweenScale != null)
                //{
                //    iconTweenScale.Kill();
                //}
            }

            public void SetData(ConfNode confNode, int index, bool isEnd)
            {
                this.m_confNode = confNode;
                this.m_reasonItemId = this.m_confNode.id;
                this.m_lineImg.Visible = !isEnd;
                this.m_isEnd = isEnd;
                SetData(this.m_confNode.name, index);
            }

            private void SetData(string name, int index)
            {
                this.m_Name = name;
                this.m_reasonItemIndex = index;
                OnReflash();
                this.m_tweenPos.Delay = this.m_delayTime + index * 0.2f;
                this.m_tweenPos.From = m_targetPos - Vector2.right * (m_targetPos.x + 166f);
                this.m_tweenPos.To = m_targetPos;

                this.m_tweenAlpha.Delay = this.m_delayTime + index * 0.2f;
            }

            private void OnReflash()
            {
                int tempIndex = ReasoningUILogic.m_currentIndex - this.m_reasonItemIndex;
                bool isLeft = IsLeft();
                if (isLeft)
                {
                    this.m_lineImg.Widget.localEulerAngles = Vector3.forward * 90;
                    this.m_lineImg.Widget.anchoredPosition = Vector2.right * 100f;
                    m_targetPos = ReasoningUILogic.m_centerPos + tempIndex * ReasoningUILogic.m_spaceY * Vector2.up;
                }
                else
                {
                    this.m_lineImg.Widget.localEulerAngles = Vector3.zero;
                    this.m_lineImg.Widget.anchoredPosition = -Vector2.up * 100f;
                    m_targetPos = ReasoningUILogic.m_centerPos + new Vector2(140f, tempIndex * ReasoningUILogic.m_spaceY);
                }

                this.m_bgImg.SetGray(tempIndex < 0);
                this.m_nameLab.SetGray(tempIndex < 0);
                this.m_nameLab.Visible = tempIndex <= 0;
                this.m_iconEffect.Visible = tempIndex == 0;
                if (tempIndex == 0)
                {
                    this.m_bgImg.Sprite = "db_common_114.png";
                    this.m_iconImg.Visible = false;
                    this.m_nameLab.FontSize = 30;
                    this.m_nameLab.color = Color.white;
                    this.m_nameLab.Text = LocalizeModule.Instance.GetString(this.m_Name);
                }
                else if (tempIndex > 0)
                {
                    this.m_bgImg.Sprite = "db_common_115.png";
                    this.m_iconImg.Visible = true;
                    this.m_iconImg.Sprite = this.m_confNode.clueicons[this.m_confNode.answer - 1];
                }
                else
                {
                    this.m_nameLab.FontSize = 100;
                    this.m_nameLab.Text = "?";
                    this.m_nameLab.color = new Color(0.498f, 0.796f, 1f);
                    this.m_iconImg.Visible = false;
                }
            }

            public void OnNextReasonItem()
            {
                OnReflash();
                this.m_lineImg.Visible = false;
                if (tweener != null)
                {
                    tweener.Kill();
                }
                tweener = Widget.DOAnchorPos3D(m_targetPos, 0.3f).OnComplete(() =>
                {
                    this.m_lineImg.Visible = !this.m_isEnd;

                });
            }

            private bool IsLeft()
            {
                return Mathf.Abs(ReasoningUILogic.m_currentIndex - this.m_reasonItemIndex) % 2 == 0;
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

            public string IconName
            {
                set { this.m_iconImg.Sprite = value; }
            }

            public bool EnableName
            {
                set { this.m_nameLab.Visible = value; }
            }

            Tweener tweenerColor = null;
            public void PlayErrorTips()
            {
                if (tweenerColor != null)
                {
                    tweenerColor.Kill();
                }
                tweenerColor = this.m_bgImg.GetSprite().DOColor(Color.red, 0.3f).SetLoops(6, LoopType.Yoyo).SetEase(Ease.InOutQuad);
                //this.m_bgTweenColor.ResetAndPlay();
            }

            //Tweener iconTweenScale = null;
            public void PlayIconScaleAnimator()
            {
                this.m_iconTweenScale.ResetAndPlay();
                //if (iconTweenScale != null)
                //{
                //    iconTweenScale.Kill();
                //}
                //this.m_iconImg.Widget.localScale = Vector2.one * 0.8f; //.SetLoops(5,LoopType.Yoyo)
                //iconTweenScale = this.m_iconImg.Widget.DOScale(1f,0.2f).SetLoops(5, LoopType.Yoyo).SetEase(Ease.InOutCubic);
            }
        }
    }
}
