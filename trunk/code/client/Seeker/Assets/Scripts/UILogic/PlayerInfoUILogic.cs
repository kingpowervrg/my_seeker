/********************************************************************
	created:  2018-4-17 11:11:34
	filename: PlayerInfoUILogic.cs
	author:	  songguangze@outlook.com
	
	purpose:  用户信息UI
*********************************************************************/
using EngineCore;
using GOEngine;
using GOGUI;
using System;
using System.Collections.Generic;
using UnityEngine;
using Utf8Json;

namespace SeekerGame
{
    [UILogicHandler(UIDefine.UI_PLAYER_INFO)]
    public class PlayerInfoUILogic : UILogicBase
    {
        private GameUIComponent m_leftRootComponent = null;

        private GameLabel m_lbPlayerNick = null;
        private GameLabel m_lbPlayerTitleName = null;
        private GameLabel m_lbPlayerIdNumber = null;
        private GameLabel m_lbPlayerLevel = null;
        private GameProgressBar m_ExpProgress;

        private GameNetworkRawImage m_imgPlayerIcon = null;

        private GameUIContainer m_containerPlayerAchievement = null;

        private GameButton m_btnChangeNick = null;
        private GameButton m_btnChangeIcon = null;
        private GameButton m_btnChangeTitle = null;
        private GameButton m_btnAchievementDetail = null;
        private GameButton m_btnCopyIdNumber = null;

        private CopyIDComponent m_panelCopyIdTip = null;
        private RenameComponent m_panelRename = null;
        private PlayerChangeIconComponent m_panelPlayerChangeIcon = null;

        private GameLabel m_rank_lab;
        private GameButton m_rank_btn;
        private PlayerTitleComponent m_playTitle_com;
        private GameSettingUIComponent m_gameSettingComponent = null;

        private long m_chooseID = -1;
        //private UITweenerBase[] tweener = null;
        public override FrameDisplayMode UIFrameDisplayMode
        {
            get
            {
                return FrameDisplayMode.WINDOWED;
            }
        }
        protected override void OnInit()
        {
            this.m_leftRootComponent = Make<GameUIComponent>("Panel_main:Panel_animation:Panel_lift");
            this.m_rank_btn = m_leftRootComponent.Make<GameButton>("Image_rank:Dropdown");
            this.m_rank_lab = m_rank_btn.Make<GameLabel>("Label");
            this.m_playTitle_com = Make<PlayerTitleComponent>("Panel_exchangetitle");
            this.m_lbPlayerNick = this.m_leftRootComponent.Make<GameLabel>("Image_name:Text (1)");
            this.m_btnChangeNick = this.m_leftRootComponent.Make<GameButton>("Image_name:Button");
            //this.m_progressExp = thi

            this.m_lbPlayerTitleName = this.m_leftRootComponent.Make<GameLabel>("Image_rank:Text (1)");
            this.m_btnChangeTitle = this.m_leftRootComponent.Make<GameButton>("Image_rank:Button");

            this.m_lbPlayerIdNumber = this.m_leftRootComponent.Make<GameLabel>("Image_id:Text (1)");
            this.m_btnCopyIdNumber = this.m_leftRootComponent.Make<GameButton>("Image_id:Button");

            this.m_imgPlayerIcon = this.m_leftRootComponent.Make<GameNetworkRawImage>("RawImage_head");
            this.m_btnChangeIcon = this.m_imgPlayerIcon.Make<GameButton>("Button");
            this.m_lbPlayerLevel = this.m_imgPlayerIcon.Make<GameLabel>("Slider:Text_number");
            m_ExpProgress = this.m_imgPlayerIcon.Make<GameProgressBar>("Slider");

            this.m_containerPlayerAchievement = this.m_leftRootComponent.Make<GameUIContainer>("Image_achievement:Scroll View:Viewport");
            this.m_btnAchievementDetail = this.m_leftRootComponent.Make<GameButton>("Image_achievement:Button");

            this.m_gameSettingComponent = Make<GameSettingUIComponent>("Panel_main:Panel_animation:Panel_right");

            this.m_panelCopyIdTip = Make<CopyIDComponent>("Panel_copyID");
            this.m_panelRename = Make<RenameComponent>("Panel_rename_1");
            this.m_panelPlayerChangeIcon = Make<PlayerChangeIconComponent>("Panel_exchangeimage");

            this.SetCloseBtnID("Panel_main:Panel_animation:Button_close");
            //this.tweener = Transform.GetComponentsInChildren<UITweenerBase>(false);
        }

        public override void OnShow(object param)
        {
            base.OnShow(param);
            //SetCloseBtnID("Panel_main:Panel_animation:Button_close");
            GameEvents.UIEvents.UI_Common_Event.OnCommonUIVisible.SafeInvoke(false);
            if (GlobalInfo.MY_PLAYER_INFO != null)
                RefreshPlayerInfo();
            else
                Debug.Log("player info is not initialize,please log in");

            #region Button Events
            this.m_btnAchievementDetail.AddClickCallBack(OnBtnShowAchievementDetailClick);

            this.m_imgPlayerIcon.AddClickCallBack(OnBtnChangePlayerIconClick);
            this.m_btnCopyIdNumber.AddClickCallBack(OnBtnCopyPlayerIDClick);
            this.m_btnChangeTitle.AddClickCallBack(OnBtnChangePlayerTitleClick);
            this.m_btnChangeNick.AddClickCallBack(OnBtnChangePlayerNameClick);

            this.m_rank_btn.AddClickCallBack(OnBtnRank);
            #endregion
            this.m_gameSettingComponent.SetPlayerInfoStatus(true);

            PlayerInfoManager.OnPlayerInfoUpdatedEvent += OnPlayerInfoUpdated;
            GameEvents.UIEvents.UI_PlayerTitle_Event.OnChoose += OnRankChoose;

            MessageHandler.RegisterMessageHandler(MessageDefine.SCAchievementResponse, OnResponse);
            MessageHandler.RegisterMessageHandler(MessageDefine.SCTitleGetResponse, OnResponse);
            MessageHandler.RegisterMessageHandler(MessageDefine.SCFBBindResponse, OnResponse);
            MessageHandler.RegisterMessageHandler(MessageDefine.SCIdentifyCheckRepsonse, OnResponse);

            OnAchievement(AchievementManager.Instance.Data);

            RefreshTitle(GlobalInfo.MY_PLAYER_INFO.TitleID);

        }

        private void OnResponse(object obj)
        {
            if (obj is SCAchievementResponse)
            {
                SCAchievementResponse res = (SCAchievementResponse)obj;

                if (res.Status == null)
                {
                    OnAchievement(res);
                }
            }
            else if (obj is SCTitleGetResponse)
            {
                SCTitleGetResponse res = (SCTitleGetResponse)obj;
                RefreshTitle(res);
            }
            else if (obj is SCFBBindResponse)
            {
                DebugUtil.Log("receive SCFBBindResponse");

                var rsp = obj as SCFBBindResponse;

                if (!MsgStatusCodeUtil.OnError(rsp.Status))
                {
                    if (rsp.HasBeenBinded)
                    {
                        BindPromoptData data = new BindPromoptData();
                        data.m_icon_name = rsp.Icon;
                        data.m_lvl = rsp.Level;
                        data.m_name = rsp.Name;
                        data.m_user_id = rsp.Id;
                        data.m_identify = rsp.Identify;
                        data.m_OnOK = () =>
                        {
                            LoginUtil.UseThirdAccount(rsp.Identify, false);

                            if (!LoginUtil.IsThereGuset())
                            {
                                EngineCoreEvents.UIEvent.HideUIEvent.SafeInvoke(UIDefine.UI_PLAYER_INFO);
                                LoginUtil.BackToLogin(ENUM_ACCOUNT_TYPE.E_FACEBOOK);
                            }
                        };

                        BindHelper.ShowBindPromoptView(data);
                    }
                    else
                    {
                        BindRewardData data = new BindRewardData();
                        data.m_count = rsp.CashCount;
                        data.m_OnOK = () => LoginUtil.UseThirdAccount(rsp.Identify);

                        BindHelper.ShowBindRewardView(data);
                    }

                    //LoginUtil.UseThirdAccount(rsp.Identify);
                    UserBehaviorStatisticsModules.Instance.LogEvent(UBSEventKeyName.user_account);
                }
            }
            else if (obj is SCIdentifyCheckRepsonse)
            {

                var rsp = obj as SCIdentifyCheckRepsonse;

                if (!MsgStatusCodeUtil.OnError(rsp.Status))
                {
                    LoginUtil.RecordToken(rsp.AccessToken);

                    GameEvents.PlayerEvents.RequestLatestPlayerInfo.SafeInvoke();
                }

            }

        }

        void RefreshTitle(SCTitleGetResponse res)
        {
            if (res.Status == null && res.Title != null)
            {
                RefreshTitle(res.Title.TitleId);
            }
            else
            {
                RefreshTitle(0L);
            }
        }
        void RefreshTitle(long title_id_)
        {

            if (0 != title_id_)
            {
                m_chooseID = title_id_;
                ConfTitle title = ConfTitle.Get(title_id_);
                if (title != null)
                {
                    m_rank_lab.Text = LocalizeModule.Instance.GetString(title.name);
                }
            }
            else
            {
                m_rank_lab.Text = string.Empty;
            }
        }

        private void OnRankChoose(TitleComponent com, long id)
        {
            m_chooseID = id;
            ConfTitle conf = ConfTitle.Get(id);
            if (conf != null)
            {
                m_rank_lab.Text = LocalizeModule.Instance.GetString(conf.name);
            }
            else
            {
                m_rank_lab.Text = "No";
            }
        }
        private void OnBtnRank(GameObject obj)
        {
            EngineCoreEvents.AudioEvents.PlayAudio.SafeInvoke(Audio.AudioType.UISound, EngineCommonAudioKey.Button_Click_Common.ToString());
            GameEvents.UIEvents.UI_PlayerTitle_Event.OnOpen.SafeInvoke();
            m_playTitle_com.SetChooseTitle(m_chooseID);
            m_playTitle_com.Visible = true;
        }

        private void OnAchievement(SCAchievementResponse res)
        {
            if (null == res)
                return;

            List<AchievementMsg> msgs = new List<AchievementMsg>();
            int count = res.Achievements.Count;
            for (int i = 0; i < res.Achievements.Count; i++)
            {
                AchievementMsg msg = res.Achievements[i];
                ConfAchievement confAchieve = ConfAchievement.Get(msg.Id);
                if (msg.Progress < confAchieve.progress1)
                {
                    continue;
                }
                long recentTime = GetRecentTime(msg);
                msg.FinishTime = recentTime;
                bool isInsert = false;
                for (int j = 0; j < msgs.Count; j++)
                {
                    if (msgs[j].FinishTime <= msg.FinishTime)
                    {
                        msgs.Insert(j, msg);
                        isInsert = true;
                        break;
                    }
                }
                if (!isInsert)
                {
                    msgs.Add(msg);
                }
            }
            int msgCount = msgs.Count;
            if (msgCount > 4)
            {
                msgCount = 4;
            }
            m_containerPlayerAchievement.EnsureSize<PlayerAchievementComponent>(msgCount);
            for (int i = 0; i < msgCount; i++)
            {
                PlayerAchievementComponent achieve = m_containerPlayerAchievement.GetChild<PlayerAchievementComponent>(i);
                ConfAchievement confAchieve = ConfAchievement.Get(msgs[i].Id);
                if (confAchieve != null)
                {
                    if (msgs[i].Progress >= confAchieve.progress3)
                    {
                        achieve.SetData(confAchieve.rewardicon3, confAchieve.name, msgs[i].FinishTime);
                    }
                    else if (msgs[i].Progress >= confAchieve.progress2)
                    {
                        achieve.SetData(confAchieve.rewardicon2, confAchieve.name, msgs[i].FinishTime2);
                    }
                    else if (msgs[i].Progress >= confAchieve.progress1)
                    {
                        achieve.SetData(confAchieve.rewardicon1, confAchieve.name, msgs[i].FinishTime1);
                    }
                }
                achieve.Visible = true;
            }
        }

        private long GetRecentTime(AchievementMsg msg)
        {
            if (msg.FinishTime > 0)
            {
                return msg.FinishTime;
            }
            else if (msg.FinishTime1 > 0)
            {
                return msg.FinishTime1;
            }
            else if (msg.FinishTime2 > 0)
            {
                return msg.FinishTime2;
            }
            return 0;
        }

        private void OnPlayerInfoUpdated(PlayerInfo playerInfo)
        {
            this.RefreshPlayerInfo();
        }


        /// <summary>
        /// 刷新玩家信息
        /// </summary>
        private void RefreshPlayerInfo()
        {
            this.m_lbPlayerNick.Text = GlobalInfo.MY_PLAYER_INFO.PlayerNickName;

            if (CommonTools.IsNeedDownloadIcon(GlobalInfo.MY_PLAYER_INFO.PlayerIcon))
            {
                if (this.m_imgPlayerIcon.TextureName != GlobalInfo.MY_PLAYER_INFO.PlayerIcon)
                {
                    this.m_imgPlayerIcon.TextureName = GlobalInfo.MY_PLAYER_INFO.PlayerIcon;
                }
            }
            else
            {
                if (this.m_imgPlayerIcon.TextureName != CommonData.GetSize1BySize2(GlobalInfo.MY_PLAYER_INFO.PlayerIcon))
                {
                    this.m_imgPlayerIcon.TextureName = CommonData.GetSize1BySize2(GlobalInfo.MY_PLAYER_INFO.PlayerIcon);
                }
            }
            int currentMaxLevel = Confetl.array.Count;
            if (GlobalInfo.MY_PLAYER_INFO.Level == currentMaxLevel + 1)
                this.m_ExpProgress.Value = 1;
            else
            {
                float nextLevelExp = Confetl.array.Find(conf => conf.level == GlobalInfo.MY_PLAYER_INFO.Level).exp;
                float currentDeltaExp = nextLevelExp - GlobalInfo.MY_PLAYER_INFO.UpgradeExp;
                this.m_ExpProgress.Value = currentDeltaExp / nextLevelExp;
                Debug.Log("经验还差" + (nextLevelExp - currentDeltaExp));
            }

            this.m_lbPlayerLevel.Text = LocalizeModule.Instance.GetString("UI_start_1.lvl", GlobalInfo.MY_PLAYER_INFO.Level);
            this.m_lbPlayerIdNumber.Text = GlobalInfo.MY_PLAYER_INFO.Player_id.ToString();

        }

        /// <summary>
        /// 查看详细成就
        /// </summary>
        /// <param name="btnAchievementDetail"></param>
        private void OnBtnShowAchievementDetailClick(GameObject btnAchievementDetail)
        {
            EngineCoreEvents.AudioEvents.PlayAudio.SafeInvoke(Audio.AudioType.UISound, EngineCommonAudioKey.Button_Click_Common.ToString());
            EngineCoreEvents.UIEvent.HideUIEvent.SafeInvoke(UIDefine.UI_PLAYER_INFO);
            GameEvents.UIEvents.UI_GameEntry_Event.OnOpenPanel.SafeInvoke(UIDefine.UI_ACHIEVEMENT);
        }


        /// <summary>
        /// 更换头像
        /// </summary>
        /// <param name="btnPlayerChangeIcon"></param>
        private void OnBtnChangePlayerIconClick(GameObject btnPlayerChangeIcon)
        {
            EngineCoreEvents.AudioEvents.PlayAudio.SafeInvoke(Audio.AudioType.UISound, EngineCommonAudioKey.Button_Click_Common.ToString());
            this.m_panelPlayerChangeIcon.Visible = true;
        }

        /// <summary>
        /// Copy玩家ID
        /// </summary>
        /// <param name="btnCopyPlayerId"></param>
        private void OnBtnCopyPlayerIDClick(GameObject btnCopyPlayerId)
        {
            EngineCoreEvents.AudioEvents.PlayAudio.SafeInvoke(Audio.AudioType.UISound, EngineCommonAudioKey.Button_Click_Common.ToString());
            GameEvents.UIEvents.UI_PlayerTitle_Event.OnOpen.SafeInvoke();
            GUIUtility.systemCopyBuffer = this.m_lbPlayerIdNumber.Text;
            this.m_panelCopyIdTip.Visible = true;
        }

        /// <summary>
        /// 改称号
        /// </summary>
        /// <param name="btnChangePlayerTitle"></param>
        private void OnBtnChangePlayerTitleClick(GameObject btnChangePlayerTitle)
        {
            EngineCoreEvents.AudioEvents.PlayAudio.SafeInvoke(Audio.AudioType.UISound, EngineCommonAudioKey.Button_Click_Common.ToString());
        }

        /// <summary>
        /// 改名
        /// </summary>
        /// <param name="btnChangePlayerName"></param>
        private void OnBtnChangePlayerNameClick(GameObject btnChangePlayerName)
        {
            EngineCoreEvents.AudioEvents.PlayAudio.SafeInvoke(Audio.AudioType.UISound, EngineCommonAudioKey.Button_Click_Common.ToString());
            this.m_panelRename.Visible = true;
            GameEvents.UIEvents.UI_PlayerTitle_Event.OnOpen.SafeInvoke();
        }





        public override void OnHide()
        {
            base.OnHide();

            this.m_btnAchievementDetail.RemoveClickCallBack(OnBtnShowAchievementDetailClick);

            this.m_imgPlayerIcon.RemoveClickCallBack(OnBtnChangePlayerIconClick);
            this.m_btnCopyIdNumber.RemoveClickCallBack(OnBtnCopyPlayerIDClick);
            this.m_btnChangeTitle.RemoveClickCallBack(OnBtnChangePlayerTitleClick);
            this.m_btnChangeNick.RemoveClickCallBack(OnBtnChangePlayerNameClick);

            this.m_rank_btn.RemoveClickCallBack(OnBtnRank);

            PlayerInfoManager.OnPlayerInfoUpdatedEvent -= OnPlayerInfoUpdated;
            GameEvents.UIEvents.UI_PlayerTitle_Event.OnChoose -= OnRankChoose;
            MessageHandler.UnRegisterMessageHandler(MessageDefine.SCAchievementResponse, OnResponse);
            MessageHandler.UnRegisterMessageHandler(MessageDefine.SCTitleGetResponse, OnResponse);
            MessageHandler.UnRegisterMessageHandler(MessageDefine.SCNoticeListResponse, OnResponse);
            MessageHandler.UnRegisterMessageHandler(MessageDefine.SCFBBindResponse, OnResponse);
            MessageHandler.UnRegisterMessageHandler(MessageDefine.SCIdentifyCheckRepsonse, OnResponse);
            GameEvents.UIEvents.UI_Common_Event.OnCommonUIVisible.SafeInvoke(true);
        }





        private class CopyIDComponent : GameUIComponent
        {
            protected override void OnInit()
            {
                SetCloseBtnID("Button");
                SetCloseBtnID("Button_close (1)");
            }

            public override void OnHide()
            {
                base.OnHide();
                GameEvents.UIEvents.UI_PlayerTitle_Event.OnClose.SafeInvoke();
            }

        }



        /// <summary>
        /// 改名
        /// </summary>
        private class RenameComponent : GameUIComponent
        {
            private GameLabel m_lbRenameCost = null;
            private GameInputField m_inputName = null;
            private GameButton m_btnRename = null;
            private GameLabel m_lbRenameTips = null;

            private GameImage m_moneyIconImg = null;
            private GameLabel m_text_costLab = null;

            private RenameResultComponent m_renameResultComponent = null;
            private int m_renameCostMoney = 0;

            private string m_lastInputText = string.Empty;
            private int m_charCount = 0;

            protected override void OnInit()
            {
                this.m_lbRenameCost = Make<GameLabel>("text_costnumber");
                this.m_btnRename = Make<GameButton>("Button");
                this.m_inputName = Make<GameInputField>("InputField");
                this.m_lbRenameTips = Make<GameLabel>("notice");

                this.m_moneyIconImg = Make<GameImage>("Panel_animation_02:Image");
                this.m_text_costLab = Make<GameLabel>("Panel_animation_02:text_cost");

                this.m_renameResultComponent = Make<RenameResultComponent>("Panel_renameResult");
            }

            public override void OnShow(object param)
            {
                base.OnShow(param);
                m_inputName.AddChangeCallBack(OnTxtRenameValueChange);
                m_inputName.Text = string.Empty;

                this.m_btnRename.AddClickCallBack(OnBtnRenameClick);
                this.m_btnRename.Visible = false;

                SetCloseBtnID("Button_close");
                this.m_renameCostMoney = GlobalInfo.MY_PLAYER_INFO.HasRenamed ? int.Parse(ConfServiceConfig.Get(7).fieldValue) : 0;
                this.m_moneyIconImg.Visible = GlobalInfo.MY_PLAYER_INFO.HasRenamed;
                this.m_lbRenameCost.Visible = GlobalInfo.MY_PLAYER_INFO.HasRenamed;
                this.m_text_costLab.Visible = GlobalInfo.MY_PLAYER_INFO.HasRenamed;
                if (!GlobalInfo.MY_PLAYER_INFO.HasRenamed)
                {
                    this.m_lbRenameTips.Text = LocalizeModule.Instance.GetString("UI_PLAYERINFO_RENAME_TIPS", ConfServiceConfig.Get(7).fieldValue);
                }
                else
                {
                    this.m_lbRenameTips.Text = LocalizeModule.Instance.GetString("UI_PLAYERINFO_RENAME_TIPS_1", ConfServiceConfig.Get(7).fieldValue);
                    this.m_lbRenameCost.Text = m_renameCostMoney.ToString();
                }


                m_lastInputText = string.Empty;

                MessageHandler.RegisterMessageHandler(MessageDefine.SCRenameResponse, OnRenameResponse);
            }

            private void OnTxtRenameValueChange(string value)
            {
                if (!string.IsNullOrWhiteSpace(value))
                {
                    if (string.IsNullOrEmpty(m_lastInputText))
                        m_lastInputText = value;

                    int chineseCount = StringUtils.ChineseCountInString(value);
                    int totalCount = chineseCount * 2 + (value.Length - chineseCount);
                    if (totalCount > GameConst.MAX_NAME_CHAR_COUNT)
                        this.m_inputName.Text = m_lastInputText;
                    else
                        m_lastInputText = value;

                    this.m_btnRename.Visible = true;
                }
            }

            private void OnBtnRenameClick(GameObject btnRename)
            {
                EngineCoreEvents.AudioEvents.PlayAudio.SafeInvoke(Audio.AudioType.UISound, EngineCommonAudioKey.Button_Click_Common.ToString());

                if (!StringUtils.IsEnginshOrChineseStr(this.m_inputName.Text))
                {
                    PopUpManager.OpenNormalOnePop("Rename Error");
                    return;
                }

                if (this.m_renameCostMoney > GlobalInfo.MY_PLAYER_INFO.Cash)
                {
                    PopUpManager.OpenNormalOnePop(LocalizeModule.Instance.GetString("UI_PLAYERINFO_CASH_NOT_ENOUTH"));
                }
                else
                {
                    string nick = this.m_inputName.Text;

                    CSRenameRequest c2sRename = new CSRenameRequest();
                    c2sRename.NewName = nick;
                    c2sRename.PlayerId = GlobalInfo.MY_PLAYER_ID;

#if !NETWORK_SYNC || UNITY_EDITOR
                    GameEvents.NetWorkEvents.SendHalfSyncMsg.SafeInvoke(c2sRename);
#else
                    GameEvents.NetWorkEvents.SendMsg.SafeInvoke(c2sRename);
#endif

                }
            }


            /// <summary>
            /// 重命名返回
            /// </summary>
            /// <param name="msgResponse"></param>
            private void OnRenameResponse(object msgResponse)
            {
                SCRenameResponse msg = msgResponse as SCRenameResponse;

                //this.m_renameResultComponent.SetResult(msg.Result == 0);
                if (!MsgStatusCodeUtil.OnError(msg.Result))
                {
                    GlobalInfo.MY_PLAYER_INFO.HasRenamed = true;
                    PopUpManager.OpenNormalOnePop(LocalizeModule.Instance.GetString("UI_PLAYERINFO_RENAMESEC_TIPS"));
                    GlobalInfo.MY_PLAYER_INFO.PlayerNickName = msg.NewName;
                    PlayerInfoManager.OnPlayerInfoUpdatedEvent(GlobalInfo.MY_PLAYER_INFO);

                    this.Visible = false;
                }
                else
                {
                    PopUpManager.OpenNormalOnePop("Rename Error");
                }

                //switch (msg.Result)
                //{
                //    case 1:





                //        break;
                //    case 2:
                //        PopUpManager.OpenNormalOnePop(LocalizeModule.Instance.GetString("UI_PLAYERINFO_NICK_NULL"));
                //        break;
                //    case 3:
                //        PopUpManager.OpenNormalOnePop(LocalizeModule.Instance.GetString("UI_PLAYERINFO_NICK_OVERFLOW"));
                //        break;
                //    case 4:
                //        PopUpManager.OpenNormalOnePop(LocalizeModule.Instance.GetString("UI_PLAYERINFO_RENAMEFAIL_TIPS"));
                //        break;
                //    case 5:
                //        PopUpManager.OpenNormalOnePop(LocalizeModule.Instance.GetString("UI_PLAYERINFO_CASH_NOT_ENOUTH"));
                //        break;
                //    default:
                //        break;
                //}
            }


            public override void OnHide()
            {
                base.OnHide();
                GameEvents.UIEvents.UI_PlayerTitle_Event.OnClose.SafeInvoke();
                this.m_btnRename.RemoveClickCallBack(OnBtnRenameClick);
                m_inputName.RemoveChangeCallBack(OnTxtRenameValueChange);
                MessageHandler.UnRegisterMessageHandler(MessageDefine.SCRenameResponse, OnRenameResponse);
            }


            private class RenameResultComponent : GameUIComponent
            {
                private GameLabel m_renameSuccessed = null;
                private GameLabel m_renameFailure = null;

                protected override void OnInit()
                {
                    base.OnInit();

                    this.m_renameFailure = Make<GameLabel>("Text_1 (1)");
                    this.m_renameSuccessed = Make<GameLabel>("Text_1 (2)");

                }

                public override void OnShow(object param)
                {
                    base.OnShow(param);

                    SetCloseBtnID("Button_close");
                    SetCloseImageID("Image_back");
                }

                public void SetResult(bool isSuccessed)
                {
                    this.m_renameSuccessed.Visible = isSuccessed;
                    this.m_renameFailure.Visible = !isSuccessed;

                }
            }
        }

        /// <summary>
        /// 换头像
        /// </summary>
        private class PlayerChangeIconComponent : GameUIComponent
        {
            private GameUIContainer m_playerIconContainer = null;
            private GameButton m_btnChangeIcon = null;

            private static PlayerIconComponent SELECTED_ICON_COMPONENT = null;
            private int m_selectedIcon = 0;



            private static Action<int> OnSelectIcon;

            protected override void OnInit()
            {
                base.OnInit();

                this.m_playerIconContainer = Make<GameUIContainer>("Scroll View:Viewport:Content");
                this.m_btnChangeIcon = Make<GameButton>("Button");

                SetCloseBtnID("Button_close");
                SetCloseImageID("Image_back");
            }


            public override void OnShow(object param)
            {
                OnSelectIcon = OnSelectIconByIndex;
                this.m_playerIconContainer.EnsureSize<PlayerIconComponent>(CommonData.DEFAULT_PLAYER_IMAGE_LIST.Count);

                for (int i = 0; i < this.m_playerIconContainer.ChildCount; ++i)
                {
                    PlayerIconComponent playerIconComponent = this.m_playerIconContainer.GetChild<PlayerIconComponent>(i);
                    playerIconComponent.ImageIconName = CommonData.DEFAULT_PLAYER_IMAGE_LIST[i];
                    playerIconComponent.IconIndex = i;

                    if (playerIconComponent.ImageIconName == GlobalInfo.MY_PLAYER_INFO.PlayerIcon)
                        m_selectedIcon = i;

                    playerIconComponent.Visible = true;
                }

                OnSelectIconByIndex(m_selectedIcon);


                this.m_btnChangeIcon.AddClickCallBack(OnBtnChangeIconClick);
            }

            private void OnSelectIconByIndex(int index)
            {
                this.m_selectedIcon = index;
                for (int i = 0; i < this.m_playerIconContainer.ChildCount; ++i)
                {
                    PlayerIconComponent iconComponent = this.m_playerIconContainer.GetChild<PlayerIconComponent>(i);
                    iconComponent.IsSelected = i == index;
                }

            }

            private void OnBtnChangeIconClick(GameObject btnChange)
            {
                EngineCoreEvents.AudioEvents.PlayAudio.SafeInvoke(Audio.AudioType.UISound, EngineCommonAudioKey.Button_Click_Common.ToString());

                string selectIcon = this.m_playerIconContainer.GetChild<PlayerIconComponent>(this.m_selectedIcon).ImageIconName;
                if (selectIcon != GlobalInfo.MY_PLAYER_INFO.PlayerIcon)
                {
                    MessageHandler.RegisterMessageHandler(MessageDefine.SCPlayerRenewIconResp, OnChangedIconResponse);

                    CSPlayerRenewIconReq changeIconReq = new CSPlayerRenewIconReq();

                    changeIconReq.NewIcon = selectIcon;
#if !NETWORK_SYNC || UNITY_EDITOR
                    GameEvents.NetWorkEvents.SendHalfSyncMsg.SafeInvoke(changeIconReq);
#else
                    GameEvents.NetWorkEvents.SendMsg.SafeInvoke(changeIconReq);
#endif

                }
                Visible = false;
            }

            private void OnChangedIconResponse(object msg)
            {
                MessageHandler.UnRegisterMessageHandler(MessageDefine.SCPlayerRenewIconResp, OnChangedIconResponse);
                SCPlayerRenewIconResp responseMsg = msg as SCPlayerRenewIconResp;
                if (responseMsg.Result == 1)
                {
                    GameEvents.PlayerEvents.RequestLatestPlayerInfo.SafeInvoke();

                }

                Visible = false;
            }

            public override void OnHide()
            {
                base.OnHide();
                this.m_btnChangeIcon.RemoveClickCallBack(OnBtnChangeIconClick);
            }

            private class PlayerIconComponent : GameUIComponent
            {
                private GameTexture m_imgHeadIcon = null;
                private GameImage m_imgSelectMark = null;
                private int m_imgId = 0;

                protected override void OnInit()
                {
                    base.OnInit();

                    this.m_imgHeadIcon = Make<GameTexture>("Background");
                    this.m_imgSelectMark = Make<GameImage>("Background:Image");
                }

                public override void OnShow(object param)
                {
                    this.m_imgHeadIcon.AddClickCallBack(OnPlayerChangeIconClick);
                }

                private void OnPlayerChangeIconClick(GameObject btnChangeIcon)
                {
                    PlayerChangeIconComponent.OnSelectIcon(IconIndex);
                }

                public override void OnHide()
                {
                    base.OnHide();
                    this.m_imgHeadIcon.RemoveClickCallBack(OnPlayerChangeIconClick);
                }

                public bool IsSelected
                {
                    get { return this.m_imgSelectMark.Visible; }
                    set { this.m_imgSelectMark.Visible = value; }
                }

                public string ImageIconName
                {
                    set { m_imgHeadIcon.TextureName = value; }
                    get
                    {
                        string iconName = this.m_imgHeadIcon.TextureName;
                        if (!iconName.EndsWithFast(".png"))
                            iconName += ".png";

                        return iconName;
                    }
                }

                public int IconIndex { get; set; }


            }
        }
    }

    /// <summary>
    /// 用户成就组件
    /// </summary>
    public class PlayerAchievementComponent : GameUIComponent
    {
        private GameImage m_imgAchievementIcon = null;
        private GameLabel m_lbTime = null;
        private GameLabel m_name = null;

        protected override void OnInit()
        {
            m_imgAchievementIcon = Make<GameImage>("Image");
            m_lbTime = TryMake<GameLabel>("time");
            m_name = TryMake<GameLabel>("Text");
        }

        public void SetData(string icon, string name, long finishTime)
        {
            m_imgAchievementIcon.Sprite = icon;
            System.DateTime dt = CommonTools.TimeStampToDateTime(finishTime * 10000);
            if (m_lbTime != null && dt != null)
            {
                m_lbTime.Text = string.Format("{0:D2}.{1:D2}.{2:D2}", dt.Year, dt.Month, dt.Day);
            }

            if (null != m_name)
                m_name.Text = LocalizeModule.Instance.GetString(name);
        }

        public void SetData(string icon, string name)
        {
            m_imgAchievementIcon.Sprite = icon;
        }

    }

    public class GameSettingUIComponentInLogin : GameSettingUIComponent
    {
        protected override void OnInit()
        {
            base.OnInit();

            this.SetCloseBtnID("Image_right:Button_close");
        }
    }

    /// <summary>
    /// 游戏设置界面
    /// </summary>
    public class GameSettingUIComponent : GameUIComponent
    {
        private GameSliderButton m_sliderBtnMusic = null;
        private GameSliderButton m_sliderBtnSound = null;
        private GameSliderButton m_sliderBtnPurchase = null;

        private GameButton m_btnNotice = null;
        private GameUIEffect m_noticUIEffect = null;
        private GameButton m_btnHelp = null;
        private GameButton m_btnSuggestion = null;

        private GameUIComponent m_btn4_root;
        private GameButton m_btnShareToApple = null;
        private GameButton m_btnShareToFacebook = null;
        private GameImage m_btnShareToFacebookIcon = null;
        private GameButton m_btnShareToTwitter = null;
        private GameButton m_btnCleanAccount = null;
        private GameButton m_Change_ID_btn;
        private SuggestionComponent m_panelSendSuggestion = null;
        private SuggestionFeedbackComponent m_suggestionReplyComponent = null;
        private ChangeIDConfirmComponent m_change_account_confirm_ui;
        private GameHelpComponent m_gameHelpComponent = null;

        public static Action ShowSuggestionFeedback;

        //private bool m_hasNotic = false;
        private bool m_isPlayerInfo = false;

        protected override void OnInit()
        {
            this.m_sliderBtnSound = Make<GameSliderButton>("Image_right:Image_sound");
            this.m_sliderBtnMusic = Make<GameSliderButton>("Image_right:Image_music");
            this.m_sliderBtnPurchase = Make<GameSliderButton>("Image_right:Image_limit");

            this.m_btn4_root = Make<GameUIComponent>("Image_right:Panel");
            this.m_btnShareToApple = Make<GameButton>("Image_right:Panel:Button_share_1");
            this.m_btnShareToFacebook = Make<GameButton>("Image_right:Panel:Button_share_2");
            this.m_btnShareToFacebookIcon = Make<GameImage>("Image_right:Panel:Button_share_2");
            this.m_btnShareToTwitter = Make<GameButton>("Image_right:Panel:Button_share_3");
            this.m_btnCleanAccount = Make<GameButton>("Image_right:Panel:Button_share_4");

            this.m_btnSuggestion = Make<GameButton>("Image_right:Button_contactus");
            this.m_btnHelp = Make<GameButton>("Image_right:Button_help");
            this.m_btnNotice = Make<GameButton>("Image_right:Button_notice");
            this.m_noticUIEffect = m_btnNotice.Make<GameUIEffect>("UI_gonggao_anniu");
            this.m_panelSendSuggestion = Make<SuggestionComponent>("Panel_contactus");
            this.m_suggestionReplyComponent = Make<SuggestionFeedbackComponent>("Panel_contactus_receipt");
            this.m_Change_ID_btn = Make<GameButton>("Image_right:Button_changeID");
            this.m_change_account_confirm_ui = Make<ChangeIDConfirmComponent>("Panel_creatID");
            this.m_gameHelpComponent = Make<GameHelpComponent>("Panel_help");
        }

        public override void OnShow(object param)
        {
            base.OnShow(param);

            this.m_change_account_confirm_ui.Visible = false;



            this.m_sliderBtnMusic.AddChangeCallBack(OnSliderBtnMusicValueChange);
            this.m_sliderBtnSound.AddChangeCallBack(OnSliderBtnSoundValueChange);
            this.m_sliderBtnPurchase.AddChangeCallBack(OnSliderBtnPurchaseValueChange);
            this.m_btnHelp.AddClickCallBack(OnBtnShowHelpClick);
            this.m_btnNotice.AddClickCallBack(OnBtnNoticeClick);

            this.m_btnSuggestion.AddClickCallBack(OnBtnGiveSuggestionClick);
            this.m_btnShareToFacebook.AddClickCallBack(OnBtnShareToFBClick);

            this.m_btnShareToTwitter.AddClickCallBack(OnBtnShareToTwitterClick);
            this.m_btnCleanAccount.AddClickCallBack(OnBtnClearAccountClick);
            this.m_Change_ID_btn.AddClickCallBack(OnBtnChangeIDClick);

            GameEvents.UIEvents.UI_PlayerTitle_Event.OnOpen += OnOpen;
            GameEvents.UIEvents.UI_PlayerTitle_Event.OnClose += OnClose;
            GameEvents.UIEvents.UI_FB_Event.Listen_FbLoginStatusChanged += SetFBBtnIcon;


            MessageHandler.RegisterMessageHandler(MessageDefine.SCFBBindResponse, OnResponse);

            this.m_sliderBtnMusic.Checked = GlobalInfo.Enable_Music;
            this.m_sliderBtnSound.Checked = GlobalInfo.Enable_Sound;
            this.m_sliderBtnPurchase.Checked = GlobalInfo.Enable_Purchase;

            ShowSuggestionFeedback = () =>
            {
                m_suggestionReplyComponent.Visible = true;
            };

            if (null != PlayerPrefTool.GetUsername(ENUM_LOGIN_TYPE.E_THIRD))
            {
                this.SwitchButton(ENUM_SETTING_BTN_TYPE.E_CHANGEID);
            }
            else
            {
                this.SwitchButton(ENUM_SETTING_BTN_TYPE.E_THREE);
            }

            if (UserBehaviorStatisticsModules.Instance != null)
                UserBehaviorStatisticsModules.Instance.LogEvent(UBSEventKeyName.user_in, 1);

            GameEvents.UIEvents.UI_FB_Event.Listen_FbLoginStatusChanged.SafeInvoke();
        }

        public void SetPlayerInfoStatus(bool isPlayerInfo)
        {
            this.m_isPlayerInfo = isPlayerInfo;
            if (!m_isPlayerInfo)
            {
                return;
            }
            this.m_noticUIEffect.EffectPrefabName = "UI_gonggao_anniu.prefab";
            this.m_noticUIEffect.Visible = false;
            m_noticUIEffect.Visible = LocalDataManager.Instance.m_HasNotic && isPlayerInfo;


        }

        private void OnOpen()
        {
            m_noticUIEffect.Visible = false;
        }

        private void OnClose()
        {
            m_noticUIEffect.Visible = LocalDataManager.Instance.m_HasNotic;
        }
        public void SetFBBtnIcon()
        {
            this.m_btnShareToFacebookIcon.Sprite = LoginUtil.IsFbLogin() ? "btn_share_type3_2_1.png" : "btn_share_type1_2.png";
        }


        private void OnResponse(object s)
        {
            //if (obj is SCNoticeListResponse && m_isPlayerInfo)
            //{
            //    SCNoticeListResponse res = (SCNoticeListResponse)obj;

            //    m_hasNotic = LocalDataManager.Instance.HasNewNotic(res.Notices);
            //    m_noticUIEffect.Visible = m_hasNotic;
            //}

            if (s is SCFBBindResponse)
            {

                var rsp = s as SCFBBindResponse;

                if (!MsgStatusCodeUtil.OnError(rsp.Status))
                {
                    GameEvents.UIEvents.UI_FB_Event.Listen_FbLoginStatusChanged.SafeInvoke();
                }

            }
        }

        private bool TokenError(ResponseStatus status_)
        {
            if (null == status_)
                return false;

            if (MsgStatusCodeUtil.FS_ACCESSTOKEN_ERROR == status_.Code || MsgStatusCodeUtil.FS_ACCESSTOKEN_TIMEOUT == status_.Code)
            {
                CommonHelper.FBLogout();
                return true;
            }
            else if (MsgStatusCodeUtil.USER_BIND == status_.Code || MsgStatusCodeUtil.EXISTED_USER == status_.Code)
            {


                LoginUtil.RemoveGuestIdytifier();
                return true;
            }
            return false;

        }
        private void SwitchButton(ENUM_SETTING_BTN_TYPE type)
        {
            this.m_Change_ID_btn.Visible = false;
            this.m_btn4_root.Visible = true;
            this.m_btnShareToApple.Visible = false;
            this.m_btnShareToFacebook.Visible = true;
            this.m_btnShareToTwitter.Visible = false;
            this.m_btnCleanAccount.Visible = true;


            //if (ENUM_SETTING_BTN_TYPE.E_THREE == type)
            //{
            //    this.m_Change_ID_btn.Visible = false;
            //    this.m_btn4_root.Visible = true;
            //    this.m_btnShareToApple.Visible = true;
            //    this.m_btnShareToFacebook.Visible = true;
            //    this.m_btnShareToTwitter.Visible = true;
            //    this.m_btnCleanAccount.Visible = false;
            //}
            //else if (ENUM_SETTING_BTN_TYPE.E_FOUR == type)
            //{
            //    this.m_Change_ID_btn.Visible = false;
            //    this.m_btn4_root.Visible = true;
            //    this.m_btnShareToApple.Visible = true;
            //    this.m_btnShareToFacebook.Visible = true;
            //    this.m_btnShareToTwitter.Visible = true;
            //    this.m_btnCleanAccount.Visible = true;
            //}
            //else
            //{
            //    this.m_btn4_root.Visible = false;
            //    this.m_Change_ID_btn.Visible = true;
            //}
        }

        /// <summary>
        /// 开启、关闭音乐
        /// </summary>
        /// <param name="value"></param>
        private void OnSliderBtnMusicValueChange(bool value)
        {
            EngineCoreEvents.AudioEvents.PlayAudio.SafeInvoke(Audio.AudioType.UISound, EngineCommonAudioKey.slider_btn.ToString());
            GlobalInfo.Enable_Music = value;
        }

        /// <summary>
        /// 开启、关闭音效
        /// </summary>
        /// <param name="value"></param>
        private void OnSliderBtnSoundValueChange(bool value)
        {

            GlobalInfo.Enable_Sound = value;
            EngineCoreEvents.AudioEvents.PlayAudio.SafeInvoke(Audio.AudioType.UISound, EngineCommonAudioKey.slider_btn.ToString());
        }


        private void OnSliderBtnPurchaseValueChange(bool value)
        {
            EngineCoreEvents.AudioEvents.PlayAudio.SafeInvoke(Audio.AudioType.UISound, EngineCommonAudioKey.slider_btn.ToString());
            GlobalInfo.Enable_Purchase = value;
        }

        /// <summary>
        /// 通知
        /// </summary>
        /// <param name="btnNotice"></param>
        private void OnBtnNoticeClick(GameObject btnNotice)
        {
            EngineCoreEvents.AudioEvents.PlayAudio.SafeInvoke(Audio.AudioType.UISound, EngineCommonAudioKey.Button_Click_Common.ToString());
            if (m_isPlayerInfo)
            {
                LocalDataManager.Instance.ReflashNotic();
                m_noticUIEffect.Visible = false;
            }

            EngineCoreEvents.UIEvent.ShowUIEvent.SafeInvoke(UIDefine.UI_NOTIC);

            UserBehaviorStatisticsModules.Instance.LogEvent(UBSEventKeyName.user_notice);
        }

        /// <summary>
        /// Facebook分享
        /// </summary>
        /// <param name="btnShareToFB"></param>
        private void OnBtnShareToFBClick(GameObject btnShareToFB)
        {
            EngineCoreEvents.AudioEvents.PlayAudio.SafeInvoke(Audio.AudioType.UISound, EngineCommonAudioKey.Button_Click_Common.ToString());
            if (LoginUtil.IsFbLogin())
            {

                //TODO : 改为登出fb
                this.m_change_account_confirm_ui.Account_type = ENUM_ACCOUNT_TYPE.E_INVALID;
                this.m_change_account_confirm_ui.Desc_type = ENUM_ACCOUNT_TYPE.E_INVALID;
                this.m_change_account_confirm_ui.Visible = true;


            }
            else
            {
                LoginUtil.OnFaceBookStart();
            }
        }

        /// <summary>
        /// Twitter分享
        /// </summary>
        /// <param name="btnShareToTwitter"></param>
        private void OnBtnShareToTwitterClick(GameObject btnShareToTwitter)
        {
            EngineCoreEvents.AudioEvents.PlayAudio.SafeInvoke(Audio.AudioType.UISound, EngineCommonAudioKey.Button_Click_Common.ToString());
            //this.m_change_account_confirm_ui.Account_type = ENUM_ACCOUNT_TYPE.E_TWITTER;
        }


        private void OnBtnClearAccountClick(GameObject btn)
        {
            EngineCoreEvents.AudioEvents.PlayAudio.SafeInvoke(Audio.AudioType.UISound, EngineCommonAudioKey.Button_Click_Common.ToString());
            if (null != PlayerPrefTool.GetUsername(ENUM_LOGIN_TYPE.E_THIRD))
            {
                this.m_change_account_confirm_ui.Account_type = ENUM_ACCOUNT_TYPE.E_GUEST;
                this.m_change_account_confirm_ui.Desc_type = ENUM_ACCOUNT_TYPE.E_FACEBOOK;
                this.m_change_account_confirm_ui.Visible = true;
            }
            else
            {
                this.m_change_account_confirm_ui.Account_type = ENUM_ACCOUNT_TYPE.E_GUEST;
                this.m_change_account_confirm_ui.Desc_type = ENUM_ACCOUNT_TYPE.E_GUEST;
                this.m_change_account_confirm_ui.Visible = true;
            }
        }

        /// <summary>
        /// 显示帮助
        /// </summary>
        /// <param name="btnShowHelp"></param>
        private void OnBtnShowHelpClick(GameObject btnShowHelp)
        {
            EngineCoreEvents.AudioEvents.PlayAudio.SafeInvoke(Audio.AudioType.UISound, EngineCommonAudioKey.Button_Click_Common.ToString());
            this.m_gameHelpComponent.Visible = true;
        }

        /// <summary>
        /// 写建议
        /// </summary>
        /// <param name="btnSuggestion"></param>
        private void OnBtnGiveSuggestionClick(GameObject btnSuggestion)
        {
            EngineCoreEvents.AudioEvents.PlayAudio.SafeInvoke(Audio.AudioType.UISound, EngineCommonAudioKey.Button_Click_Common.ToString());
            this.m_panelSendSuggestion.Visible = true;
        }


        private void OnBtnChangeIDClick(GameObject btnShareToTwitter)
        {
            EngineCoreEvents.AudioEvents.PlayAudio.SafeInvoke(Audio.AudioType.UISound, EngineCommonAudioKey.Button_Click_Common.ToString());
            this.SwitchButton(ENUM_SETTING_BTN_TYPE.E_FOUR);
        }

        public override void OnHide()
        {
            base.OnHide();

            this.m_sliderBtnMusic.RemoveChangeCallBack(OnSliderBtnMusicValueChange);
            this.m_sliderBtnSound.RemoveChangeCallBack(OnSliderBtnSoundValueChange);
            this.m_sliderBtnPurchase.RemoveChangeCallBack(OnSliderBtnPurchaseValueChange);
            this.m_btnHelp.RemoveClickCallBack(OnBtnShowHelpClick);
            this.m_btnNotice.RemoveClickCallBack(OnBtnNoticeClick);
            this.m_btnSuggestion.RemoveClickCallBack(OnBtnGiveSuggestionClick);
            this.m_btnShareToFacebook.RemoveClickCallBack(OnBtnShareToFBClick);
            this.m_btnShareToTwitter.RemoveClickCallBack(OnBtnShareToTwitterClick);
            this.m_btnCleanAccount.RemoveClickCallBack(OnBtnClearAccountClick);
            this.m_Change_ID_btn.RemoveClickCallBack(OnBtnChangeIDClick);

            GameEvents.UIEvents.UI_PlayerTitle_Event.OnOpen -= OnOpen;
            GameEvents.UIEvents.UI_PlayerTitle_Event.OnClose -= OnClose;
            GameEvents.UIEvents.UI_FB_Event.Listen_FbLoginStatusChanged -= SetFBBtnIcon;

            MessageHandler.UnRegisterMessageHandler(MessageDefine.SCFBBindResponse, OnResponse);
        }




        /// <summary>
        /// 意见反馈
        /// </summary>
        public class SuggestionFeedbackComponent : GameUIComponent
        {
            public override void OnShow(object param)
            {
                base.OnShow(param);
                SetCloseBtnID("Animator:Button");
            }
        }

        public class ChangeIDConfirmComponent : GameUIComponent
        {
            private GameButton m_ok_btn;
            private GameLabel m_desc;


            private ENUM_ACCOUNT_TYPE m_account_type;
            public SeekerGame.ENUM_ACCOUNT_TYPE Account_type
            {
                get { return m_account_type; }
                set { m_account_type = value; }
            }

            private ENUM_ACCOUNT_TYPE m_desc_type;

            public SeekerGame.ENUM_ACCOUNT_TYPE Desc_type
            {
                get { return m_desc_type; }
                set { m_desc_type = value; }
            }

            public override void OnShow(object param)
            {
                base.OnShow(param);

                this.ShowDesc();

                this.m_ok_btn.AddClickCallBack(OnOkClick);
            }

            protected override void OnInit()
            {
                base.OnInit();

                this.m_ok_btn = this.Make<GameButton>("Animator:Button (1)");
                this.m_desc = this.Make<GameLabel>("Animator:Text_1");
                SetCloseBtnID("Animator:Button");
            }

            public override void OnHide()
            {
                base.OnHide();
                this.m_ok_btn.RemoveClickCallBack(OnOkClick);

            }


            private void OnOkClick(GameObject obj)
            {


                switch (Account_type)
                {
                    case ENUM_ACCOUNT_TYPE.E_APPLE:
                        break;
                    case ENUM_ACCOUNT_TYPE.E_FACEBOOK:
                        LoginUtil.OnAccountChangeToThird(ENUM_ACCOUNT_TYPE.E_FACEBOOK);
                        break;
                    case ENUM_ACCOUNT_TYPE.E_TWITTER:
                        break;
                    case ENUM_ACCOUNT_TYPE.E_GUEST:

                        LoadingProgressManager.Instance.LoadFacebook();
                        HttpPingModule.Instance.Enable = false;

                        TimeModule.Instance.SetTimeout(() =>
                        {
                            NewGuid.GuidNewManager.Instance.OnClearLocalProgress();
                            NewGuid.GuidNewManager.Instance.OnClearGuid();
                            SignInManager.Clear();
                            BigWorldManager.Instance.ClearBigWorld();

                            EngineCoreEvents.ResourceEvent.LeaveScene.SafeInvoke();

                            FrameMgr.Instance.HideAllFrames(new List<string>() { UIDefine.UI_FB_Loading, UIDefine.UI_GUID }, true);

                            TimeModule.Instance.SetTimeout(() =>
                            {
                                //if (GameRoot.instance.GameFSM.GotoStateWithParam((int)ClientFSM.ClientState.LOGIN, Account_type))
                                //    return;
                                GameRoot.instance.GameFSM.GotoState((int)ClientFSM.ClientState.PROLOGUE);
                                PlayerPrefTool.SetUsername("", ENUM_LOGIN_TYPE.E_THIRD);
                                PlayerPrefTool.SetUsername("", ENUM_LOGIN_TYPE.E_GUEST);
                                //LoginUtil.OnAccountChangeToGuest();
                            }, 1.0f
                            );
                        }, 1.0f);
                        break;
                    case ENUM_ACCOUNT_TYPE.E_INVALID:
                        {
                            //登出fb

                            LoadingProgressManager.Instance.LoadFacebook();
                            LoginUtil.FBLogout();
                            LoginUtil.OnAccountChangeToGuest(true);
                            GameEvents.UIEvents.UI_FB_Event.Listen_FbLoginStatusChanged.SafeInvoke();
                            EngineCoreEvents.UIEvent.HideUIEvent.SafeInvoke(UIDefine.UI_PLAYER_INFO);

                            LoginUtil.BackToLogin();
                        }
                        break;
                    default:
                        break;
                }

                this.Visible = false;


            }

            private void ShowDesc()
            {
                switch (Desc_type)
                {
                    case ENUM_ACCOUNT_TYPE.E_APPLE:
                        break;
                    case ENUM_ACCOUNT_TYPE.E_FACEBOOK:
                        this.m_desc.Text = LocalizeModule.Instance.GetString("facebook.newaccount");
                        break;
                    case ENUM_ACCOUNT_TYPE.E_TWITTER:
                        break;
                    case ENUM_ACCOUNT_TYPE.E_GUEST:
                        this.m_desc.Text = LocalizeModule.Instance.GetString("guest.newaccount");
                        break;
                    case ENUM_ACCOUNT_TYPE.E_INVALID:
                        {
                            this.m_desc.Text = LocalizeModule.Instance.GetString("UI_Logout_FB");
                        }
                        break;
                    default:
                        break;
                }
            }
        }

        /// <summary>
        /// 帮助面板
        /// </summary>
        public class GameHelpComponent : GameUIComponent
        {
            private GameCheckBox m_cbIntroduce = null;
            private GameCheckBox m_cbAbout = null;
            private GameCheckBox m_cbPrivacy = null;

            private GameHelpIntroduceComponent m_introduceComponent = null;
            private GameHelpAboutusCompoment m_aboutusComponent = null;
            private GameHelpPrivacyComponent m_privacyComponent = null;

            protected override void OnInit()
            {
                this.m_cbIntroduce = Make<GameCheckBox>("Animator:leftBtn:0");
                this.m_cbAbout = Make<GameCheckBox>("Animator:leftBtn:1");
                this.m_cbPrivacy = Make<GameCheckBox>("Animator:leftBtn:2");

                this.m_introduceComponent = Make<GameHelpIntroduceComponent>("Animator:Panel_0");
                this.m_aboutusComponent = Make<GameHelpAboutusCompoment>("Animator:Panel_1");
                this.m_privacyComponent = Make<GameHelpPrivacyComponent>("Animator:Panel_2");

                this.SetCloseBtnID("Animator:Image_bg_2:Button");
            }

            public override void OnShow(object param)
            {

                this.m_cbIntroduce.AddChangeCallBack(OnCheckBoxIntroduceTagChanged);
                this.m_cbAbout.AddChangeCallBack(OnCheckBoxAboutusTagChanged);
                this.m_cbPrivacy.AddChangeCallBack(OnCheckBoxPrivacyTagChanged);


                SetDefaultTagVisible();
                UserBehaviorStatisticsModules.Instance.LogEvent(UBSEventKeyName.user_help);
            }

            private void SetDefaultTagVisible()
            {
                this.m_introduceComponent.Visible = true;
                this.m_aboutusComponent.Visible = false;
                this.m_privacyComponent.Visible = false;

                this.m_cbIntroduce.Checked = true;
                this.m_cbPrivacy.Checked = false;
                this.m_cbAbout.Checked = false;
            }


            private void OnCheckBoxIntroduceTagChanged(bool value)
            {
                this.m_introduceComponent.Visible = value;
            }

            private void OnCheckBoxAboutusTagChanged(bool value)
            {
                this.m_aboutusComponent.Visible = value;
            }

            private void OnCheckBoxPrivacyTagChanged(bool value)
            {
                this.m_privacyComponent.Visible = value;
            }

            public override void OnHide()
            {
                base.OnHide();
                this.m_cbIntroduce.RemoveChangeCallBack(OnCheckBoxIntroduceTagChanged);
                this.m_cbAbout.RemoveChangeCallBack(OnCheckBoxAboutusTagChanged);
                this.m_cbPrivacy.RemoveChangeCallBack(OnCheckBoxPrivacyTagChanged);

            }


            private class GameHelpIntroduceComponent : GameUIComponent
            {
                private GameLabel m_lbIntroduce = null;

                protected override void OnInit()
                {
                    base.OnInit();
                    this.m_lbIntroduce = Make<GameLabel>("Scroll View:Viewport:Content:Text");
                }

                public override void OnShow(object param)
                {
                    base.OnShow(param);
                    this.m_lbIntroduce.Text = LocalizeModule.Instance.GetString("introduce");
                }
            }

            private class GameHelpAboutusCompoment : GameUIComponent
            {
                private GameLabel m_lbAboutus = null;

                protected override void OnInit()
                {
                    base.OnInit();
                    this.m_lbAboutus = Make<GameLabel>("Scroll View:Viewport:Content");
                }

                public override void OnShow(object param)
                {
                    base.OnShow(param);
                    this.m_lbAboutus.Text = LocalizeModule.Instance.GetString("about us_dec");
                }
            }

            private class GameHelpPrivacyComponent : GameUIComponent
            {
                private GameHyperlinkButton m_privacyBtn1 = null;
                private GameHyperlinkButton m_privacyBtn2 = null;
                private GameHyperlinkButton m_privacyBtn3 = null;

                protected override void OnInit()
                {
                    base.OnInit();
                    this.m_privacyBtn1 = Make<GameHyperlinkButton>("Scroll View:Viewport:Content:Button");
                    this.m_privacyBtn2 = Make<GameHyperlinkButton>("Scroll View:Viewport:Content:Button (1)");
                    this.m_privacyBtn3 = Make<GameHyperlinkButton>("Scroll View:Viewport:Content:Button (2)");
                }
            }
        }

    }

    /// <summary>
    /// 意见
    /// </summary>
    public class SuggestionComponent : GameUIComponent
    {
        private GameButton m_btnSendSuggestion = null;
        private GameButton m_btnFB = null;
        private GameInputField m_inputSuggestion = null;
        private GameInputField m_inputEmail = null;


        protected override void OnInit()
        {
            SetCloseBtnID("Animator:Button_close");

            this.m_btnSendSuggestion = Make<GameButton>("Animator:Button");
            this.m_btnFB = Make<GameButton>("Animator:Button_FB");
            this.m_inputSuggestion = Make<GameInputField>("Animator:InputField");
            this.m_inputEmail = Make<GameInputField>("Animator:InputField_Email");
        }

        public override void OnShow(object param)
        {
            base.OnShow(param);

            this.m_btnSendSuggestion.AddClickCallBack(OnBtnSendSuggestionClick);
            this.m_btnFB.AddClickCallBack(OnBtnGoToFBClick);
            this.m_inputSuggestion.AddChangeCallBack(OnSuggestionValueChanged);
            this.m_btnSendSuggestion.Visible = false;

            MessageHandler.RegisterMessageHandler(MessageDefine.SCPlayerFeedbackResponse, OnFeedbackResponse);

            UserBehaviorStatisticsModules.Instance.LogEvent(UBSEventKeyName.user_emil);
        }

        private void OnBtnGoToFBClick(GameObject btnSendSuggestion)
        {
            Application.OpenURL(CommonHelper.C_FB_URL);
        }
        private void OnBtnSendSuggestionClick(GameObject btnSendSuggestion)
        {
            EngineCoreEvents.AudioEvents.PlayAudio.SafeInvoke(Audio.AudioType.UISound, EngineCommonAudioKey.Button_Click_Common.ToString());

            CSPlayerFeedbackRequest sendFeedback = new CSPlayerFeedbackRequest();
            sendFeedback.Platform = Application.platform.ToString();
            sendFeedback.Release = GlobalInfo.GAME_VERSION;

            sendFeedback.Content = m_inputSuggestion.Text;

            if (!string.IsNullOrEmpty(m_inputEmail.Text))
                sendFeedback.Email = m_inputEmail.Text;


            string guest_id = null != PlayerPrefTool.GetUsername(ENUM_LOGIN_TYPE.E_GUEST) ? PlayerPrefTool.GetUsername(ENUM_LOGIN_TYPE.E_GUEST) : "";
            string fb_id = null != PlayerPrefTool.GetUsername(ENUM_LOGIN_TYPE.E_THIRD) ? PlayerPrefTool.GetUsername(ENUM_LOGIN_TYPE.E_THIRD) : "";
            string player_id = GlobalInfo.MY_PLAYER_ID.ToString();

            FeedbackDetailData userDetailData = new FeedbackDetailData();
            userDetailData.GuestID = guest_id;
            userDetailData.PlayerID = player_id;
            userDetailData.FacebookID = fb_id;

            sendFeedback.Information = JsonSerializer.ToJsonString<FeedbackDetailData>(userDetailData);

#if !NETWORK_SYNC || UNITY_EDITOR
            GameEvents.NetWorkEvents.SendAsyncMsg.SafeInvoke(sendFeedback);
#else
            GameEvents.NetWorkEvents.SendMsg.SafeInvoke(sendFeedback);
#endif


            if (GameSettingUIComponent.ShowSuggestionFeedback != null)
            {
                GameSettingUIComponent.ShowSuggestionFeedback();
            }
            else
            {
                PopUpManager.OpenNormalOnePop("suggest_ok");
            }

            UserBehaviorStatisticsModules.Instance.LogEvent(UBSEventKeyName.emil_send);

            Visible = false;
        }

        private void OnSuggestionValueChanged(string value)
        {
            this.m_btnSendSuggestion.Visible = !string.IsNullOrEmpty(value);
        }

        private void OnFeedbackResponse(object feedbackResponseMsg)
        {
            Visible = false;
        }

        public override void OnHide()
        {
            this.m_inputSuggestion.Text = string.Empty;
            base.OnHide();
            this.m_btnSendSuggestion.RemoveClickCallBack(OnBtnSendSuggestionClick);
            this.m_btnFB.RemoveClickCallBack(OnBtnGoToFBClick);
            this.m_inputSuggestion.RemoveChangeCallBack(OnSuggestionValueChanged);

            MessageHandler.UnRegisterMessageHandler(MessageDefine.SCPlayerFeedbackResponse, OnFeedbackResponse);
        }

    }
}