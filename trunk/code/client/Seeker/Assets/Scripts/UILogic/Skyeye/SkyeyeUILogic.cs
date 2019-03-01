using System;
using System.Collections.Generic;
using EngineCore;
using UnityEngine;
using DG.Tweening;
namespace SeekerGame
{

    [UILogicHandler(UIDefine.UI_SKYEYE)]
    public class SkyeyeUILogic : UILogicBase
    {
        private SkyeyePersonItem[] m_personItem = null;
        private GameProgressBar m_totalProgress = null;
        private GameUIComponent m_lockCom = null;
        private GameButton m_startUpBtn = null;
        private GameButton m_quitBtn = null;
        private GameImage m_bgImg = null;
        private SkyEyePropDetail m_detailUI = null;
        private SkyeyePersonDetailComponent m_personDetail = null;

        private GameLabel m_totalProgressLab = null;
        public const float PERSONTIME = 0.3f;
        protected override void OnInit()
        {
            base.OnInit();
            this.m_personItem = new SkyeyePersonItem[10];
            for (int i = 0; i < 10; i++)
            {
                this.m_personItem[i] = Make<SkyeyePersonItem>("Panel_down:Panel_head:head_" + (i + 1));
            }
            this.m_totalProgress = Make<GameProgressBar>("Panel_down:totalProgress");
            this.m_lockCom = Make<GameUIComponent>("Panel_down:Text_lock");
            this.m_startUpBtn = Make<GameButton>("Panel_down:Button");
            this.m_quitBtn = Make<GameButton>("RawImage:Button_close");
            this.m_bgImg = Make<GameImage>("Panel_down:Panel:bg");
            this.m_detailUI = Make<SkyEyePropDetail>("Panel_icondetail");
            this.m_personDetail = Make<SkyeyePersonDetailComponent>("Panel_detail");
            this.m_totalProgressLab = Make<GameLabel>("Panel_down:num");
        }

        private int currentNum = 0;
        public override void OnShow(object param)
        {
            base.OnShow(param);

            GameEvents.UIEvents.UI_SkyEye_Event.OnSkyEyeCompleteById += OnSkyEyeCompleteById;
            GameEvents.UIEvents.UI_SkyEye_Event.OnOpenIconDetail += OnOpenIconDetail;
            this.m_quitBtn.AddClickCallBack(OnQuit);
            currentNum = 0;
            for (int i = 0; i < ConfSkyEye.array.Count; i++)
            {
                if (i >= 10)
                    break;
                if (this.m_personItem[i].SetData(ConfSkyEye.array[i],i, m_personDetail))
                    currentNum++;
                this.m_personItem[i].Visible = true;
            }
            this.m_bgImg.FillAmmount = 0f;
            //this.m_totalProgressLab.Visible = num < 10;
            //this.m_totalProgressLab.Text = num.ToString();
            DOTween.To(x => this.m_bgImg.FillAmmount = x, 0, 1, SkyeyeUILogic.PERSONTIME * 10f).SetEase(Ease.InQuad);
            //DOTween.To(x => this.m_totalProgress.Value = x, 0, num / (float)ConfSkyEye.array.Count, 0.4f);
            ReflashMainProgress(0, currentNum);
        }

        public override void OnHide()
        {
            base.OnHide();
            MessageHandler.UnRegisterMessageHandler(MessageDefine.SCSkyEyeReq, OnRes);
            GameEvents.UIEvents.UI_SkyEye_Event.OnSkyEyeCompleteById -= OnSkyEyeCompleteById;
            GameEvents.UIEvents.UI_SkyEye_Event.OnOpenIconDetail -= OnOpenIconDetail;
            this.m_quitBtn.RemoveClickCallBack(OnQuit);
        }

        private void OnOpenIconDetail(string desc,string iconName)
        {
            this.m_detailUI.SetData(desc,iconName);
            this.m_detailUI.Visible = true;
        }

        private void OnQuit(GameObject obj)
        {
            SkyeyeUILogic.Hide();
        }

        private void ReflashMainProgress(int start,int end)
        {
            this.m_totalProgressLab.Text = end.ToString();
            DOTween.To(x => this.m_totalProgress.Value = x, start / (float)ConfSkyEye.array.Count, end / (float)ConfSkyEye.array.Count, 0.4f);
        }

        private void OnSkyEyeCompleteById(long skyeyeId)
        {
            for (int i = 0; i < this.m_personItem.Length; i++)
            {
                if (this.m_personItem[i].SkyEyeID == skyeyeId)
                {
                    this.m_personItem[i].HasReward = true;
                    ReflashMainProgress(currentNum ++, currentNum);
                    break;
                }
            }
        }

        public override FrameDisplayMode UIFrameDisplayMode
        {
            get
            {
                return FrameDisplayMode.FULLSCREEN;
            }
        }

        private static void OnRes(object obj)
        {
            if (obj is SCSkyEyeReq)
            {
                MessageHandler.UnRegisterMessageHandler(MessageDefine.SCSkyEyeReq, OnRes);
                GlobalInfo.MY_PLAYER_INFO.m_skyEyehasCache = true;
                GlobalInfo.MY_PLAYER_INFO.ClearSkyEye();
                SCSkyEyeReq eyeReq = (SCSkyEyeReq)obj;
                for (int i = 0; i < eyeReq.HasRewarded.Count; i++)
                {
                    GlobalInfo.MY_PLAYER_INFO.AddSkyEyeHasRewardById(eyeReq.HasRewarded[i]);
                }
                EngineCoreEvents.UIEvent.ShowUIEvent.SafeInvoke(UIDefine.UI_SKYEYE);
            }
        }

        public static void Show()
        {
            if (!GlobalInfo.MY_PLAYER_INFO.m_skyEyehasCache)
            {
                MessageHandler.UnRegisterMessageHandler(MessageDefine.SCSkyEyeReq, OnRes);
                MessageHandler.RegisterMessageHandler(MessageDefine.SCSkyEyeReq, OnRes);
                CSSkyEyeReq eyeReq = new CSSkyEyeReq();
                GameEvents.NetWorkEvents.SendAsyncMsg.SafeInvoke(eyeReq);
            }
            else
                EngineCoreEvents.UIEvent.ShowUIEvent.SafeInvoke(UIDefine.UI_SKYEYE);
        }

        public static void Hide()
        {
            EngineCoreEvents.UIEvent.HideUIEvent.SafeInvoke(UIDefine.UI_SKYEYE);
        }
    }

    public class SkyeyePersonItem : GameUIComponent
    {
        private GameTexture m_personImg = null;
        private GameLabel m_personName = null;
        private GameProgressBar m_progressBar = null;
        private GameImage m_progressFillImg = null;

        private GameUIComponent m_lockCom = null;
        private GameUIComponent m_personCom = null;

        //private TweenAlpha m_alphaTweener = null;
        private TweenScale m_scaleTweener = null;

        private TweenAlpha m_alphaText = null;
        private TweenPosition m_posText = null;
        private ConfSkyEye m_confSkyeye = null;

        private GameUIComponent m_root = null;

        private SkyeyePersonDetailComponent m_detailUI = null;

        private GameLabel m_tipsLab = null;
        private Color m_srcColor = new Color(0.349f,0.854f,1f);
        private Color m_targetColor = new Color(1f, 0.858f, 0.329f);
        private bool m_hasReward = false;
        public bool HasReward
        {
            set {
                m_hasReward = value;
                this.m_tipsLab.Visible = false;
                this.m_progressFillImg.Color = m_targetColor;
                this.m_personName.color = m_targetColor;
            }
        }
        protected override void OnInit()
        {
            base.OnInit();
            this.m_personImg = Make<GameTexture>("Panel:RawImage");
            this.m_personName = Make<GameLabel>("Text");
            this.m_tipsLab = this.m_personName.Make<GameLabel>("tips");

            this.m_progressBar = Make<GameProgressBar>("Slider");
            this.m_progressFillImg = this.m_progressBar.Make<GameImage>("Fill Area:Fill");
            this.m_lockCom = Make<GameUIComponent>("Image_unlock");
            this.m_personCom = Make<GameUIComponent>("Panel");
            this.m_root = Make<GameUIComponent>(gameObject);
            //this.m_alphaTweener = GetComponent<TweenAlpha>();
            this.m_scaleTweener = GetComponent<TweenScale>();

            this.m_alphaText = this.m_personName.GetComponent<TweenAlpha>();
            this.m_posText = this.m_personName.GetComponent<TweenPosition>();
        }

        public override void OnShow(object param)
        {
            base.OnShow(param);
            this.m_root.AddClickCallBack(OnOpenPersonUI);
            this.m_progressBar.Value = 0f;
            TimeModule.Instance.SetTimeout(()=> {
                DOTween.To(x => this.m_progressBar.Value = x, 0, m_progress, 1f).SetEase(Ease.InOutQuad);
            }, 0.3f + this.m_scaleTweener.Delay);
        }

        public override void OnHide()
        {
            base.OnHide();
            this.m_root.RemoveClickCallBack(OnOpenPersonUI);
        }
        bool isLock = true;
        public long SkyEyeID;
        public bool SetData(ConfSkyEye skyeye,int index,SkyeyePersonDetailComponent detailUI)
        {
            this.m_detailUI = detailUI;
            m_confSkyeye = skyeye;
            this.SkyEyeID = skyeye.id;
            int num = 0;
            this.m_hasReward = GlobalInfo.MY_PLAYER_INFO.IsSkyEyeRewardContainId(skyeye.id);
            //this.m_alphaTweener.Delay = Mathf.Sqrt(index * 0.9f);// index * SkyeyeUILogic.PERSONTIME;
            this.m_scaleTweener.Delay = Mathf.Sqrt(index * 0.9f); ;// index * SkyeyeUILogic.PERSONTIME;

            this.m_alphaText.Delay = this.m_scaleTweener.Delay + this.m_scaleTweener.Duration;
            this.m_posText.Delay = this.m_scaleTweener.Delay + this.m_scaleTweener.Duration;
            isLock = true;
            for (int i = 0; i < m_confSkyeye.collectorIds.Length; i++)
            {
                PlayerPropMsg playerProp = GlobalInfo.MY_PLAYER_INFO.GetBagInfosByID(m_confSkyeye.collectorIds[i]);
                if (playerProp != null && playerProp.Count > 0)
                {
                    num++;
                    if (m_confSkyeye.collectorIds[i] == m_confSkyeye.keyCollectorId)
                        isLock = false;
                }
            }

            this.m_personName.Visible = !isLock;
            this.m_lockCom.Visible = isLock;
            this.m_personCom.Visible = !isLock;
            m_progress = num / (float)m_confSkyeye.collectorIds.Length;
            this.m_tipsLab.Visible = false;
            if (m_progress == 1 && !m_hasReward)
            {
                this.m_tipsLab.Text = "(" + LocalizeModule.Instance.GetString("theeye_tips") + ")";
                this.m_tipsLab.Visible = true;
            }
            
            if (m_hasReward)
            {
                this.m_progressFillImg.Color = m_targetColor;
                this.m_personName.color = m_targetColor;
            }
            else
            {
                this.m_progressFillImg.Color = m_srcColor;
                this.m_personName.color = m_srcColor;
            }
            //this.m_progressBar.Value = m_progress;
            if (!isLock)
            {
                ConfNpc npc = ConfNpc.Get(m_confSkyeye.npcId);
                this.m_personName.Text = LocalizeModule.Instance.GetString(npc.name);
                this.m_personImg.TextureName = npc.icon;
            }
            return m_hasReward;
        }

        private void OnOpenPersonUI(GameObject obj)
        {
            if (m_progress > 0)
            {
                this.m_detailUI.SetData(m_confSkyeye, !isLock,m_hasReward);
                this.m_detailUI.Visible = true;
            }
        }

        private float m_progress = 0f;
    }

    public class SkyEyePropDetail : GameUIComponent
    {
        private GameLabel m_lab = null;
        private GameImage m_icon = null;
        private GameUIComponent m_closeBtn = null;
        protected override void OnInit()
        {
            base.OnInit();
            this.m_lab = Make<GameLabel>("Animator:Text_Desc");
            this.m_icon = Make<GameImage>("Animator:Background:Image");
            this.m_closeBtn = Make<GameUIComponent>("Animator:Button_close");
        }

        public override void OnShow(object param)
        {
            base.OnShow(param);
            this.isCloseing = false;
            this.m_closeBtn.AddClickCallBack(OnClose);
        }

        public override void OnHide()
        {
            base.OnHide();
            this.isCloseing = false;
            this.m_closeBtn.RemoveClickCallBack(OnClose);
        }

        public void SetData(string desc, string iconName)
        {
            this.m_lab.Text = LocalizeModule.Instance.GetString(desc);
            this.m_icon.Sprite = iconName;
        }

        bool isCloseing = false;
        private void OnClose(GameObject obj)
        {
            if (isCloseing)
                return;
            isCloseing = true;
            Visible = false;
        }

    }
}
