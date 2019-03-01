/********************************************************************
	created:  2018-5-16 19:58:56
	filename: MailUILogic.cs
	author:	  songguangze@fotoable.com
	
	purpose:  邮件UI逻辑
*********************************************************************/
#define NOFRIEND
using EngineCore;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace SeekerGame
{

    [UILogicHandler(UIDefine.UI_MAIL)]
    public class MailUILogic : UILogicBase
    {
        public static Action<int> OnBrowseDetailMail;

        private ToggleWithArrowTween m_btnSystemMail = null;
#if !NOFRIEND
        private ToggleWithArrowTween m_btnFriendMail = null;
#endif
        private GameScrollView m_gameScrollView = null;

        private GameUIContainer m_mailItemContainer = null;
        private MailDetailComponent m_mailDetailComponent = null;

        private MailSystem m_mailSystem = null;

        private MailType m_currentMailTag = MailType.SYSTEM;
        private List<Mail> m_currentMailDataList = new List<Mail>();
        private GameLabel m_lbNomail = null;


        protected override void OnInit()
        {
#if !NOFRIEND
            this.m_btnFriendMail = Make<ToggleWithArrowTween>("Panel_down:leftBtn:btn_friend");
#endif
            this.m_btnSystemMail = Make<ToggleWithArrowTween>("Panel_down:leftBtn:btn_system");
            this.m_gameScrollView = Make<GameScrollView>("Panel_down:Panel");
            this.m_mailItemContainer = Make<GameUIContainer>("Panel_down:Panel:Viewport:Content");
            this.m_mailDetailComponent = Make<MailDetailComponent>("Panel_MailDetail");
            this.m_mailSystem = GlobalInfo.MY_PLAYER_INFO.PlayerMailSystem;
            this.m_lbNomail = Make<GameLabel>("Panel_down:Panel:Text_noMail");
            this.SetCloseBtnID("Button_close");
        }

        public override void OnShow(object param)
        {
            base.OnShow(param);

            EngineCoreEvents.AudioEvents.PlayAudio.SafeInvoke(Audio.AudioType.UISound, GameCustomAudioKey.zoom_in.ToString());

            this.m_btnSystemMail.Refresh((int)(MailType.SYSTEM), LocalizeModule.Instance.GetString("mail_system"), true, OnBtnSystemMailTagClick);
#if !NOFRIEND
            this.m_btnFriendMail.Refresh((int)(MailType.FRIEND), LocalizeModule.Instance.GetString("mail_system"), false, OnBtnFriendMailTagClick);
#endif

            OnBrowseDetailMail = OnBrowseDetailMailByIndex;
            GameEvents.MailEvents.OnMailListChanged += OnMailListChanged;

            this.m_lbNomail.Text = LocalizeModule.Instance.GetString("mail_nothing");

            InitUnreadMailTag();


            UserBehaviorStatisticsModules.Instance.LogEvent(UBSEventKeyName.post_in);

            OnBtnSystemMailTagClick(null);
        }

        /// <summary>
        /// 初始化未读邮件的Tag
        /// </summary>
        private void InitUnreadMailTag()
        {
            int systemMailUnreadCount = m_mailSystem.FilterUnreadMailByType(MailType.SYSTEM).Count;
            if (systemMailUnreadCount > 0)
                RefreshMailList(MailType.SYSTEM);
            else
            {
                int friendMailUnreadCount = m_mailSystem.FilterUnreadMailByType(MailType.FRIEND).Count;
                if (friendMailUnreadCount > 0)
                    RefreshMailList(MailType.FRIEND);
                else
                    RefreshMailList(MailType.SYSTEM);
            }

            UserBehaviorStatisticsModules.Instance.LogEvent(UBSEventKeyName.post_read, systemMailUnreadCount);
        }
        public override FrameDisplayMode UIFrameDisplayMode
        {
            get
            {
                return FrameDisplayMode.FULLSCREEN;
            }
        }


        private void OnMailListChanged(MailType mailType, List<Mail> mailList)
        {
            if (mailType == this.m_currentMailTag)
            {
                RefreshMailList(mailType);
            }
        }

        private void OnBtnSystemMailTagClick(bool v_, int idx_)
        {
            if (!v_)
                return;

            this.m_btnSystemMail.PlayUITweener(UITweenerBase.TweenTriggerType.Manual);
            if (this.m_currentMailTag != MailType.SYSTEM)
                RefreshMailList(MailType.SYSTEM);
        }

        private void OnBtnFriendMailTagClick(bool v_, int idx_)
        {
            if (!v_)
                return;

            if (this.m_currentMailTag != MailType.FRIEND)
                RefreshMailList(MailType.FRIEND);
        }

        private void OnBtnSystemMailTagClick(GameObject btnSystemMail)
        {
            this.m_btnSystemMail.PlayUITweener(UITweenerBase.TweenTriggerType.Manual);
            if (this.m_currentMailTag != MailType.SYSTEM)
                RefreshMailList(MailType.SYSTEM);
        }

        private void OnBtnFriendMailTagClick(GameObject btnSystemMail)
        {
            if (this.m_currentMailTag != MailType.FRIEND)
                RefreshMailList(MailType.FRIEND);
        }


        private void OnBrowseDetailMailByIndex(int mailIndex)
        {
            if (mailIndex < 0 || mailIndex >= this.m_currentMailDataList.Count)
                return;

            Mail browseMail = this.m_currentMailDataList[mailIndex];
            if (browseMail != null)
            {
                this.m_mailDetailComponent.SetMailData(mailIndex, browseMail);
                this.m_mailDetailComponent.Visible = true;
                this.m_mailSystem.OnReadMail(browseMail.MailUUID);
            }
        }




        /// <summary>
        /// 刷新邮件列表
        /// </summary>
        /// <param name="mailType"></param>
        private void RefreshMailList(MailType mailType)
        {
            this.m_currentMailTag = mailType;
            //m_currentMailDataList = this.m_mailSystem.FilterUnreadMailByType(this.m_currentMailTag);
            this.m_currentMailDataList = this.m_mailSystem.SortMailList(this.m_currentMailTag);
            this.m_mailItemContainer.EnsureSize<MailItemComponent>(m_currentMailDataList.Count);

            this.m_lbNomail.Visible = m_currentMailDataList.Count == 0;

            for (int i = 0; i < m_currentMailDataList.Count; ++i)
            {
                MailItemComponent mailItem = this.m_mailItemContainer.GetChild<MailItemComponent>(i);
                mailItem.SetMailBasicInfo(i, m_currentMailDataList[i]);
                mailItem.Visible = true;
            }
            this.m_gameScrollView.ScrollToTop();
        }

        public override void OnHide()
        {
            //    this.m_btnSystemMail.RemoveClickCallBack(OnBtnSystemMailTagClick);
            //    this.m_btnFriendMail.RemoveClickCallBack(OnBtnFriendMailTagClick);
            EngineCoreEvents.AudioEvents.PlayAudio.SafeInvoke(Audio.AudioType.UISound, GameCustomAudioKey.zoom_out.ToString());
            GameEvents.MailEvents.OnMailListChanged -= OnMailListChanged;

        }

        /// <summary>
        /// 邮件项
        /// </summary>
        private class MailItemComponent : GameUIComponent
        {
            private GameLabel m_lbMailSendTime = null;
            private GameLabel m_lbMailTitle = null;
            private GameUIComponent m_readMarkComponent = null;
            private GameImage m_imgMailWithReward = null;

            private Mail m_mailInfo = null;
            private int m_mailIndex = 0;

            protected override void OnInit()
            {
                this.m_lbMailSendTime = Make<GameLabel>("Text_time");
                this.m_lbMailTitle = Make<GameLabel>("Text_title");
                this.m_readMarkComponent = Make<GameUIComponent>("Image_unread");
                this.m_imgMailWithReward = Make<GameImage>("Image_bg");
            }

            public override void OnShow(object param)
            {
                base.OnShow(param);
                this.AddClickCallBack(OnBtnViewMailDetailClick);
            }

            /// <summary>
            /// 设置邮件基本信息
            /// </summary>
            /// <param name="mainInfo"></param>
            public void SetMailBasicInfo(int mailIndex, Mail mainInfo)
            {
                this.m_mailInfo = mainInfo;
                this.m_mailIndex = mailIndex;

                RefreshMainBasicInfo();
            }

            public void RefreshMainBasicInfo()
            {
                this.m_lbMailTitle.Text = LocalizeModule.Instance.GetString(this.m_mailInfo.MailTitle);
                this.m_lbMailSendTime.Text = CommonTools.TimeStampToDateTime(this.m_mailInfo.MailSendTime).ToString("yyyy.MM.dd");
                this.m_readMarkComponent.Visible = this.m_mailInfo.MailStatus == MailStatus.Unread;
                this.m_imgMailWithReward.Sprite = this.m_mailInfo.HasRewardItem ? "db_mail_2.png" : "db_mail_1.png";
            }

            private void OnBtnViewMailDetailClick(GameObject btnViewDetail)
            {
                EngineCoreEvents.AudioEvents.PlayAudio.SafeInvoke(Audio.AudioType.UISound, GameCustomAudioKey.mail_openletter.ToString());
                MailUILogic.OnBrowseDetailMail(this.m_mailIndex);
            }

            public override void OnHide()
            {
                base.OnHide();
                this.RemoveClickCallBack(OnBtnViewMailDetailClick);
            }


        }

        /// <summary>
        /// 邮件详情
        /// </summary>
        private class MailDetailComponent : GameUIComponent
        {
            private Mail m_mailInfo = null;
            private int m_currentMainIndex = 0;

            private GameButton m_btnCollectRewardItem = null;
            private GameUIContainer m_rewardItemContainer = null;
            private GameButton m_btnViewPreviousDetail = null;
            private GameButton m_btnViewNextDetail = null;
            private GameLabel m_lbMailTitle = null;
            private GameLabel m_lbMailExpireTime = null;
            private GameLabel m_lbMailContent = null;


            protected override void OnInit()
            {
                base.OnInit();

                this.m_btnViewPreviousDetail = Make<GameButton>("Button_lift");
                this.m_btnViewNextDetail = Make<GameButton>("Button_right");
                this.m_btnCollectRewardItem = Make<GameButton>("btnUse");
                this.m_rewardItemContainer = Make<GameUIContainer>("Panel:grid");
                this.m_lbMailContent = Make<GameLabel>("Image_bg_2:Scroll View:Viewport:Content:Text_title (2)");
                this.m_lbMailExpireTime = Make<GameLabel>("Text_title (1)");
                this.m_lbMailTitle = Make<GameLabel>("Text_title");
            }

            public override void OnShow(object param)
            {
                base.OnShow(param);

                this.m_btnViewPreviousDetail.AddClickCallBack(OnBtnViewPreviousMailClick);
                this.m_btnViewNextDetail.AddClickCallBack(OnBtnViewNextMailClick);
                this.m_btnCollectRewardItem.AddClickCallBack(OnBtnCollectRewardClick);

                SetCloseBtnID("Button_close");
            }

            public void SetMailData(int mailIndex, Mail viewMailData)
            {
                this.m_currentMainIndex = mailIndex;
                this.m_mailInfo = viewMailData;

                RefreshMailInfo();
            }

            private void RefreshMailInfo()
            {
                this.m_lbMailTitle.Text = LocalizeModule.Instance.GetString(this.m_mailInfo.MailTitle);
                this.m_lbMailExpireTime.Text = $"{(CommonTools.TimeStampToDateTime(this.m_mailInfo.MailExpireTime) - DateTime.Now).Days + 1}d";
                this.m_lbMailContent.Text = LocalizeModule.Instance.GetString(this.m_mailInfo.MailDesc);
                this.m_btnCollectRewardItem.Visible = this.m_mailInfo.HasRewardItem;

                if (this.m_mailInfo.HasRewardItem)
                {
                    MessageHandler.RegisterMessageHandler(MessageDefine.SCEmailRewardResponse, OnMailReward);
                    this.m_rewardItemContainer.EnsureSize<MailRewardItemComponent>(this.m_mailInfo.MailRewardItemList.Count);
                    for (int i = 0; i < this.m_mailInfo.MailRewardItemList.Count; ++i)
                    {
                        MailRewardItemComponent mailRewardItem = this.m_rewardItemContainer.GetChild<MailRewardItemComponent>(i);
                        mailRewardItem.SetRewardItemData(this.m_mailInfo.MailRewardItemList[i]);
                        mailRewardItem.Visible = true;
                    }
                }
                else
                {
                    this.m_rewardItemContainer.EnsureSize<MailRewardItemComponent>(0);
                    MessageHandler.UnRegisterMessageHandler(MessageDefine.SCEmailRewardResponse, OnMailReward);
                }

                this.m_btnViewPreviousDetail.Visible = !(this.m_currentMainIndex == 0);
                this.m_btnViewNextDetail.Visible = !(this.m_currentMainIndex == (GlobalInfo.MY_PLAYER_INFO.PlayerMailSystem.FilterUnreadMail().Count - 1));
            }

            private void OnMailReward(object message)
            {
                SCEmailRewardResponse rewardMsg = message as SCEmailRewardResponse;
                if (rewardMsg.Id == this.m_mailInfo.MailUUID && rewardMsg.Result == 1)
                {

                }

                Visible = false;
            }

            /// <summary>
            /// 查看前一封邮件
            /// </summary>
            /// <param name="btnViewPrevious"></param>
            private void OnBtnViewPreviousMailClick(GameObject btnViewPrevious)
            {
                EngineCoreEvents.AudioEvents.PlayAudio.SafeInvoke(Audio.AudioType.UISound, GameCustomAudioKey.mail_change.ToString());
                MailUILogic.OnBrowseDetailMail(--this.m_currentMainIndex);
            }

            /// <summary>
            /// 查看下一封
            /// </summary>
            /// <param name="btnViewNext"></param>
            public void OnBtnViewNextMailClick(GameObject btnViewNext)
            {
                EngineCoreEvents.AudioEvents.PlayAudio.SafeInvoke(Audio.AudioType.UISound, GameCustomAudioKey.mail_change.ToString());
                if (this.m_mailInfo.IsRead)
                    MailUILogic.OnBrowseDetailMail(this.m_currentMainIndex);
                else
                    MailUILogic.OnBrowseDetailMail(++this.m_currentMainIndex);
            }

            private void OnBtnCollectRewardClick(GameObject btnCollectReward)
            {

                EngineCoreEvents.AudioEvents.PlayAudio.SafeInvoke(Audio.AudioType.UISound, GameCustomAudioKey.mail_giftget.ToString());
                if (this.m_mailInfo.HasRewardItem)
                {
                    CSEmailRewardRequest reqCollectReward = new CSEmailRewardRequest();
                    reqCollectReward.Id = this.m_mailInfo.MailUUID;
#if !NETWORK_SYNC || UNITY_EDITOR
                    GameEvents.NetWorkEvents.SendHalfSyncMsg.SafeInvoke(reqCollectReward);
#else
                    GameEvents.NetWorkEvents.SendMsg.SafeInvoke(reqCollectReward);
#endif

                }
            }

            public override void OnHide()
            {
                base.OnHide();

                this.m_btnViewPreviousDetail.RemoveClickCallBack(OnBtnViewPreviousMailClick);
                this.m_btnViewNextDetail.RemoveClickCallBack(OnBtnViewNextMailClick);
                this.m_btnCollectRewardItem.RemoveClickCallBack(OnBtnCollectRewardClick);
                MessageHandler.UnRegisterMessageHandler(MessageDefine.SCEmailRewardResponse, OnMailReward);
            }




            private class MailRewardItemComponent : GameUIComponent
            {
                private GameImage m_itemIcon = null;
                private GameLabel m_itemName = null;
                private GameLabel m_itemNum = null;

                private ItemWrapper m_rewardItemData = null;

                protected override void OnInit()
                {
                    base.OnInit();

                    this.m_itemIcon = Make<GameImage>("icon");
                    this.m_itemNum = Make<GameLabel>("sum");
                    this.m_itemName = Make<GameLabel>("title");
                }


                public void SetRewardItemData(ItemWrapper rewardItem)
                {
                    this.m_rewardItemData = rewardItem;

                    RefreshRewardItem();
                }

                public void RefreshRewardItem()
                {
                    ConfProp confProp = ConfProp.Get(this.m_rewardItemData.ItemID);
                    if (confProp == null)
                        Debug.LogError($"item {this.m_rewardItemData.ItemID} not found");

                    this.m_itemIcon.Sprite = confProp.icon;
                    this.m_itemName.Text = LocalizeModule.Instance.GetString(confProp.name);
                    this.m_itemNum.Text = $"x{m_rewardItemData.ItemNum}";
                }
            }
        }
    }
}