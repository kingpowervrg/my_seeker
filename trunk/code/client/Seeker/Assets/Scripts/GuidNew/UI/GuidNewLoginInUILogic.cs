//#define TEST
using EngineCore;
using EngineCore.Utility;
using Facebook.Unity;
using GOEngine;
using GOGUI;
using Google.Protobuf;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SeekerGame
{
    [UILogicHandler(UIDefine.UI_GUIDLOGIN)]
    public class GuidNewLoginInUILogic : UILogicBase
    {

        enum ENUM_BTN_TYPE
        {
            E_ALL,
            E_NO_THIRD,
        }

        private GameButton m_login_btn;
        //private GameButton m_twitter_btn;
        private GameButton m_fackbook_btn;
        //private GameButton m_apple_btn;

        private GameLabel m_contentLab = null;
        private bool m_versionValid = false;

        private bool is_token_checked = false;
        protected override void OnInit()
        {
            base.OnInit();
            NeedUpdateByFrame = true;
            m_login_btn = Make<GameButton>("RawImage:btnLogin");
            //m_twitter_btn = Make<GameButton>("Canvas:btn_1");
            m_fackbook_btn = Make<GameButton>("RawImage:btnFacebook");
            //m_apple_btn = Make<GameButton>("Canvas:btn_3");
            this.m_contentLab = Make<GameLabel>("RawImage:Panel_left:Text:content");

        }
        public override FrameDisplayMode UIFrameDisplayMode
        {
            get
            {
                return FrameDisplayMode.WINDOWED;
            }
        }
        public override void OnShow(object param)
        {
            base.OnShow(param);
            m_is_start_timer = false;
            m_cur_time = 0.0f;
            is_token_checked = false;
            GlobalInfo.GAME_NETMODE = GameNetworkMode.Network;
            CommonHelper.ShowLoading(false);

            m_login_btn.AddClickCallBack(OnNormalVersionClicked);
            m_fackbook_btn.AddClickCallBack(OnFBVersionClicked);

            MessageHandler.RegisterMessageHandler(MessageDefine.SCRegGuestResponse, OnScResponse);
            MessageHandler.RegisterMessageHandler(MessageDefine.SCIdentifyCheckRepsonse, OnScResponse);
            MessageHandler.RegisterMessageHandler(MessageDefine.SCPlayerInfoResponse, OnScResponse);
            //MessageHandler.RegisterMessageHandler(MessageDefine.SCPlayerPropResponse, OnScResponse);
            MessageHandler.RegisterMessageHandler(MessageDefine.SCFBLoginResponse, OnScResponse);
            MessageHandler.RegisterMessageHandler(MessageDefine.SCFBBindResponse, OnScResponse);
            //MessageHandler.RegisterMessageHandler(MessageDefine.SCNoticeListResponse, OnScResponse);
            MessageHandler.RegisterMessageHandler(MessageDefine.SCPlayerGuildResponse, OnScResponse);
            MessageHandler.RegisterMessageHandler(MessageDefine.SCGetPushResponse, OnScResponse);
            MessageHandler.RegisterMessageHandler(MessageDefine.SCIdentifyCheckMuiltyMessage, OnScResponse);

            this.m_contentLab.Text = LocalizeModule.Instance.GetString("guide_0_05");


            GameEvents.UIEvents.UI_FB_Event.Listen_FbLoginStatusChanged += SetFBBtnIcon;

            if (null != PlayerPrefTool.GetUsername(ENUM_LOGIN_TYPE.E_THIRD))
            {
                SwitchBtns(ENUM_BTN_TYPE.E_NO_THIRD);
            }
            else
            {
                SwitchBtns(ENUM_BTN_TYPE.E_ALL);
            }

            if (null != param)
            {
                SwitchBtns(ENUM_BTN_TYPE.E_ALL);

                if (ENUM_ACCOUNT_TYPE.E_GUEST == (ENUM_ACCOUNT_TYPE)(param))
                {
                    LoginUtil.OnAccountChangeToGuest();
                    Debug.LogWarning($"FB : GO TO GUEST IN LOGIN third name is {PlayerPrefTool.GetUsername(ENUM_LOGIN_TYPE.E_THIRD)} , logined { FB.IsLoggedIn}");
                    IntervalSetFBBtnIcon(true);
                    //PregameUILogic.instance.Destory();
                    return;
                }
                else
                {
                    LoginUtil.OnAccountNativeLogin((ENUM_ACCOUNT_TYPE)(param));
                }
            }

            SetFBBtnIcon();
            //PregameUILogic.instance.Destory();
            LocalDataManager.Instance.Is_login = true;
        }

        public override void OnHide()
        {
            base.OnHide();

            m_is_start_timer = false;
            MessageHandler.UnRegisterMessageHandler(MessageDefine.SCRegGuestResponse, OnScResponse);
            MessageHandler.UnRegisterMessageHandler(MessageDefine.SCIdentifyCheckRepsonse, OnScResponse);
            MessageHandler.UnRegisterMessageHandler(MessageDefine.SCPlayerInfoResponse, OnScResponse);
            //MessageHandler.UnRegisterMessageHandler(MessageDefine.SCPlayerPropResponse, OnScResponse);
            MessageHandler.UnRegisterMessageHandler(MessageDefine.SCFBLoginResponse, OnScResponse);
            MessageHandler.UnRegisterMessageHandler(MessageDefine.SCFBBindResponse, OnScResponse);
            //MessageHandler.UnRegisterMessageHandler(MessageDefine.SCNoticeListResponse, OnScResponse);
            MessageHandler.UnRegisterMessageHandler(MessageDefine.SCPlayerGuildResponse, OnScResponse);
            MessageHandler.UnRegisterMessageHandler(MessageDefine.SCGetPushResponse, OnScResponse);
            MessageHandler.UnRegisterMessageHandler(MessageDefine.SCIdentifyCheckMuiltyMessage, OnScResponse);

            GameEvents.UIEvents.UI_FB_Event.Listen_FbLoginStatusChanged -= SetFBBtnIcon;

            m_login_btn.RemoveClickCallBack(OnNormalVersionClicked);
            m_fackbook_btn.RemoveClickCallBack(OnFBVersionClicked);

            LocalDataManager.Instance.Is_login = false;

            PopUpManager.ClosePopUp();

            GameEvents.UIEvents.UI_FB_Event.Listen_FbLoginStatusChanged.SafeInvoke();
        }

        private void OccurEror()
        {
            CommonHelper.ShowLoading(false);
            m_isClick = false;
            m_is_start_timer = false;
            m_cur_time = 0.0f;
        }

        private void OccurOK()
        {
            OccurEror();
        }

        private void OnScResponse(object s)
        {
            if (s is SCRegGuestResponse)
            {

                DebugUtil.Log("receive SCRegGuestResponse");

                var rsp = s as SCRegGuestResponse;

                if (!MsgStatusCodeUtil.OnError(rsp.Status))
                {
                    RecordGuestIdytifier(rsp.GuestIdentify);
                    DebugUtil.Log("GuestIdentify:" + rsp.GuestIdentify);
                    LoginUtil.RequestCheckToken(rsp.GuestIdentify);

                }
                else
                    OccurEror();

            }
            else if (s is SCIdentifyCheckMuiltyMessage)
            {
                var rsp = s as SCIdentifyCheckMuiltyMessage;

                if (!MsgStatusCodeUtil.OnError(rsp.Status))
                {

                    for (int i = 0; i < rsp.Contents.Count; ++i)
                    {
                        IdentifyCheckResponseContent pingResponseContent = rsp.Contents[i];
                        byte[] messageContentBytes = Convert.FromBase64String(pingResponseContent.Data);

                        IMessage message = EngineCore.MessageParser.Parse(pingResponseContent.MsgId, messageContentBytes);

                        MessageHandler.Call(pingResponseContent.MsgId, message);
                    }
                }
                else
                {
                    OccurEror();
                }

            }
            else if (s is SCIdentifyCheckRepsonse)
            {
                var rsp = s as SCIdentifyCheckRepsonse;

                is_token_checked = true;
                if (!MsgStatusCodeUtil.OnError(rsp.Status))
                {
                    Debug.Log("access token = " + rsp.AccessToken);
                    LoginUtil.RecordToken(rsp.AccessToken);

                    //RequestPlayerInfo();

                }
                else
                    OccurEror();
            }
            else if (s is SCPlayerInfoResponse)
            {

                if (is_token_checked)
                {
                    var rsp = s as SCPlayerInfoResponse;


                    GameEvents.UI_Guid_Event.OnClearGuid.SafeInvoke();
                    CommonBonusPopViewMgr.Instance.ClearBonus();

                    GlobalInfo.MY_PLAYER_ID = rsp.PlayerId;
#if UNITY_EDITOR || UNITY_DEBUG
                    UBSBaseData.m_player_id = "fotoable " + GlobalInfo.MY_PLAYER_ID.ToString();
#else
                UBSBaseData.m_player_id = GlobalInfo.MY_PLAYER_ID.ToString();
#endif

#if OFFICER_SYS
                    PoliceDispatchManager.Instance.Load();
#endif

                    DebugUtil.Log("PlayerId:" + rsp.PlayerId);

                    PlayerInfo info = new PlayerInfo(rsp.PlayerId);

                    info.SetCash(rsp.Cash).SetCoin(rsp.Coin).SetExp(rsp.Exp).SetExpMultiple(rsp.ExpMultiple)
                        .SetIcon(rsp.PlayerIcon).SetLaborUnionn(rsp.LaborUnion)
                        .SetLevel(rsp.Level).SetUpgradeExp(rsp.UpgradeExp).SetVit(rsp.Vit);

                    info.PlayerNickName = rsp.PlayerName;
                    info.HasRenamed = rsp.HasRenamed > 1;

                    PlayerInfoManager.Instance.AddPlayerInfo(rsp.PlayerId, info);
                    CommonData.MillisRecoverOneVit = (rsp.MillisRecoverOneVit / 1000) + 1;
                    VitManager.Instance.SetLastAddVitTime(rsp.LastAddVitTime);
                    VitManager.Instance.ReflashInfiniteVitTime(rsp.InfiniteVitRestTime);
                    DebugUtil.Log("个人信息下载完成");

                    //请求新手引导数据
                    //CSPlayerGuildRequest guidReq = new CSPlayerGuildRequest();
                    //GameEvents.NetWorkEvents.SendAsyncMsg.SafeInvoke(guidReq);
                }

                PlayerInfoManager.Instance.Sync();

            }
            else if (s is SCPlayerGuildResponse)
            {
                SCPlayerGuildResponse res = (SCPlayerGuildResponse)s;
                SeekerGame.NewGuid.GuidNewManager.Instance.SyncProgress(int.Parse(res.Guild));
            }
            else if (s is SCGetPushResponse)
            {
                var rsp = s as SCGetPushResponse;

                if (rsp.Infos.Count > 0)
                {
                    PushGiftManager.Instance.Cache(ENUM_PUSH_GIFT_BLOCK_TYPE.E_LOGIN);
                    GameEvents.UIEvents.UI_Bonus_Pop_View_Event.Tell_OnCache.SafeInvoke(EUNM_BONUS_POP_VIEW_TYPE.E_PUSH_GIFT);
                }

                CommonHelper.ShowLoading(false);
                GameRoot.instance.GameFSM.GotoState((int)ClientFSM.ClientState.NORMAL);
            }
            else if (s is SCFBLoginResponse)
            {
                DebugUtil.Log("receive SCFBLoginResponse");

                var rsp = s as SCFBLoginResponse;

                if (!MsgStatusCodeUtil.OnError(rsp.Status))
                {
                    LoginUtil.UseThirdAccount(rsp.Identify);
                }
                else
                {
                    TokenError(rsp.Status);
                    OccurEror();
                }

            }
            else if (s is SCFBBindResponse)
            {
                DebugUtil.Log("receive SCFBBindResponse");
                OccurOK();

                var rsp = s as SCFBBindResponse;

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
                        data.m_OnOK = () => { CommonHelper.ShowLoading(true); m_is_start_timer = true; LoginUtil.UseThirdAccount(rsp.Identify); };

                        BindHelper.ShowBindPromoptView(data);
                    }
                    else
                    {
                        BindRewardData data = new BindRewardData();
                        data.m_count = rsp.CashCount;
                        data.m_OnOK = () => { CommonHelper.ShowLoading(true); m_is_start_timer = true; LoginUtil.UseThirdAccount(rsp.Identify); };

                        BindHelper.ShowBindRewardView(data);
                    }

                    UserBehaviorStatisticsModules.Instance.LogEvent(UBSEventKeyName.user_account);
                }
                else
                {
                    TokenError(rsp.Status);
                    OccurEror();
                }

            }

        }


        private void RequestPlayerInfo()
        {
            CSPlayerInfoRequest req = new CSPlayerInfoRequest();
            //GameEvents.NetWorkEvents.SendMsg.SafeInvoke(req);
            GameEvents.NetWorkEvents.SendAsyncMsg.SafeInvoke(req);

        }

        private void RecordGuestIdytifier(string guest_name_)
        {
            PlayerPrefTool.SetUsername(guest_name_, ENUM_LOGIN_TYPE.E_GUEST);
        }

        private bool m_is_start_timer = false;

        private void OnLoginButtonClick(GameObject go)

        {
            m_is_start_timer = true;
            m_cur_time = 0.0f;

            delta_time = CommonUtils.GetCurTimeMillSenconds() - delta_time;

            Debug.Log("弹出 loading 时差" + delta_time);

            CommonHelper.ShowLoading(true);

            EngineCoreEvents.AudioEvents.PlayAudio.SafeInvoke(Audio.AudioType.UISound, EngineCommonAudioKey.Button_Click_Common.ToString());

            Dictionary<UBSParamKeyName, object> _params = new Dictionary<UBSParamKeyName, object>()
            {
                        { UBSParamKeyName.Description, "reg guest"},
            };
            UserBehaviorStatisticsModules.Instance.LogEvent(UBSEventKeyName.Login_LOGO, null, _params);

            if (null != PlayerPrefTool.GetUsername(ENUM_LOGIN_TYPE.E_THIRD))
            {
#if UNITY_EDITOR || !UNITY_STANDALONE_WIN
                if (!FB.IsInitialized)
                {
                    FB.Init(this.OnInitCompleteWithThirdCheck, LoginUtil.OnHideUnity);
                }
                else
                {
                    OnInitCompleteWithThirdCheck();
                }
#else
                OnInitCompleteWithThirdCheck();
#endif

                return;
            }

            if (null != PlayerPrefTool.GetUsername(ENUM_LOGIN_TYPE.E_GUEST))
            {

#if UNITY_EDITOR || !UNITY_STANDALONE_WIN
                if (!FB.IsInitialized)
                {
                    FB.Init(this.OnInitCompleteWithGuestCheck, LoginUtil.OnHideUnity);
                }
                else
                {
                    this.OnInitCompleteWithGuestCheck();
                }
#else
                this.OnInitCompleteWithGuestCheck();
#endif

                return;
            }
#if UNITY_EDITOR || !UNITY_STANDALONE_WIN
            if (!FB.IsInitialized)
            {
                FB.Init(this.OnInitCompleteWithGuestReg, LoginUtil.OnHideUnity);
            }
            else
            {
                OnInitCompleteWithGuestReg();
            }
#else
            OnInitCompleteWithGuestReg();
#endif

        }

        private void OnFaceBookButtonClick(GameObject go)
        {
            m_is_start_timer = true;
            m_cur_time = 0.0f;
            CommonHelper.ShowLoading(true);

            EngineCoreEvents.AudioEvents.PlayAudio.SafeInvoke(Audio.AudioType.UISound, EngineCommonAudioKey.Button_Click_Common.ToString());

            Dictionary<UBSParamKeyName, object> _params = new Dictionary<UBSParamKeyName, object>()
            {
                        { UBSParamKeyName.Description, "reg facebook"},
            };
            UserBehaviorStatisticsModules.Instance.LogEvent(UBSEventKeyName.Login_LOGO, null, _params);

            LoginUtil.OnFaceBookStart();
            //#if TEST
            //            if (!BindGuestToFacebook())
            //                NativeFaceBookLogin();

            //#else
            //            if (FB.IsInitialized)
            //            {
            //                Debug.Log("FACEBOOK : fb button clicked Initialized");
            //                OnInitComplete();
            //                return;
            //            }

            //            FB.Init(this.OnInitComplete, this.OnHideUnity);
            //#endif
        }


        private ENUM_ACCOUNT_TYPE m_login_type;

        private bool m_isClick = false;
        private long delta_time;
        private void OnNormalVersionClicked(GameObject obj)
        {
            if (m_isClick)
            {
                return;
            }
            this.m_isClick = true;
            delta_time = CommonUtils.GetCurTimeMillSenconds();

            m_login_type = ENUM_ACCOUNT_TYPE.E_GUEST;
            DoTestNetwork();
            //if (ENUM_ACCOUNT_TYPE.E_GUEST == m_login_type)
            //{
            //    OnLoginButtonClick(null);
            //}
            //else
            //{
            //    OnFaceBookButtonClick(null);
            //}
        }

        private void OnFBVersionClicked(GameObject obj)
        {
            if (m_isClick)
            {
                return;
            }
            this.m_isClick = true;
            m_login_type = ENUM_ACCOUNT_TYPE.E_FACEBOOK;
            DoTestNetwork();
            //if (ENUM_ACCOUNT_TYPE.E_GUEST == m_login_type)
            //{
            //    OnLoginButtonClick(null);
            //}
            //else
            //{
            //    OnFaceBookButtonClick(null);
            //}

        }

        private void OnBtnGoToFBClick(GameObject btnSendSuggestion)
        {
            Application.OpenURL(CommonHelper.C_FB_URL);
        }

        //        private void NativeFaceBookLogin()
        //        {
        //            Debug.Log("FACEBOOK : NativeFaceBookLogin");
        //#if TEST
        //            RequestFaceBookLogin(test_token,
        //                        test_id,
        //                        test_expire_time,
        //                        SystemInfo.deviceUniqueIdentifier,
        //                        this.GetOsTypeEnum());
        //#else
        //            if (AccessToken.CurrentAccessToken != null)
        //            {

        //                RequestFaceBookLogin(AccessToken.CurrentAccessToken.TokenString,
        //                        AccessToken.CurrentAccessToken.UserId,
        //                        AccessToken.CurrentAccessToken.ExpirationTime.TotalSeconds(),
        //                        SystemInfo.deviceUniqueIdentifier,
        //                        this.GetOsTypeEnum());
        //            }
        //            else
        //            {
        //                Debug.Log("FACEBOOK : logged in but no token");
        //            }
        //#endif

        //        }

        //        private bool BindGuestToFacebook()
        //        {
        //            if (null != PlayerPrefTool.GetUsername(ENUM_LOGIN_TYPE.E_GUEST))
        //            {
        //                Debug.Log("FACEBOOK : BindGuestToFacebook");

        //                CSFBBindRequest req = new CSFBBindRequest();
        //                req.Identify = PlayerPrefTool.GetUsername(ENUM_LOGIN_TYPE.E_GUEST);

        //#if TEST
        //                req.AccessToken = test_token;
        //                req.FbId = test_id;
        //                req.ExpiresIn = test_expire_time;
        //#else
        //                req.AccessToken = AccessToken.CurrentAccessToken.TokenString;
        //                req.FbId = AccessToken.CurrentAccessToken.UserId;
        //                req.ExpiresIn = AccessToken.CurrentAccessToken.ExpirationTime.TotalSeconds();
        //#endif
        //                GameEvents.NetWorkEvents.SendMsg.SafeInvoke(req);
        //                DebugUtil.Log("send CSFBBindRequest");
        //                return true;
        //            }

        //            return false;
        //        }

        private void OnInitCompleteWithThirdCheck()
        {
            Debug.Log("FACEBOOK : OnInitCompleteWithoutLogin");

#if UNITY_EDITOR || !UNITY_STANDALONE_WIN
            if (!FB.IsInitialized)
            {
                return;
            }
#endif

            LoginUtil.RequestCheckToken(PlayerPrefTool.GetUsername(ENUM_LOGIN_TYPE.E_THIRD));
        }


        private void OnInitCompleteWithGuestCheck()
        {
            Debug.Log("FACEBOOK : OnInitCompleteWithGuestCheck");

#if UNITY_EDITOR || !UNITY_STANDALONE_WIN
            //if (!FB.IsInitialized)
            //{
            //    return;
            //}
#endif

            LoginUtil.RequestCheckToken(PlayerPrefTool.GetUsername(ENUM_LOGIN_TYPE.E_GUEST));
        }

        private void OnInitCompleteWithGuestReg()
        {
            Debug.Log("FACEBOOK : OnInitCompleteWithGuestReg");

#if UNITY_EDITOR || !UNITY_STANDALONE_WIN
            if (!FB.IsInitialized)
            {
                return;
            }
#endif
            LoginUtil.RequestRegisterGuest();

        }

        private bool TokenError(ResponseStatus status_)
        {
            if (null == status_)
                return false;

            if (MsgStatusCodeUtil.FS_ACCESSTOKEN_ERROR == status_.Code || MsgStatusCodeUtil.FS_ACCESSTOKEN_TIMEOUT == status_.Code)
            {
                this.SwitchBtns(ENUM_BTN_TYPE.E_ALL);
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


        private void PopupInfo(string content_)
        {

            PopUpData pd = new PopUpData();
            pd.title = string.Empty;
            pd.content = content_;
            pd.content_param0 = string.Empty;
            pd.isOneBtn = true;
            pd.OneButtonText = "UI.OK";
            pd.twoStr = "UI.OK";
            pd.oneAction = null;
            pd.twoAction = null;

            PopUpManager.OpenPopUp(pd);

        }

        private void SwitchBtns(ENUM_BTN_TYPE type_)
        {
            if (ENUM_BTN_TYPE.E_ALL == type_)
            {
                m_login_btn.Visible = true;
                //m_twitter_btn.Visible = true;
                //m_fackbook_btn.Visible = true;
                //m_apple_btn.Visible = true;


                //m_twitter_btn.Visible = false;
                m_fackbook_btn.Visible = true;
                //m_apple_btn.Visible = false;
            }
            else if (ENUM_BTN_TYPE.E_NO_THIRD == type_)
            {
                m_login_btn.Visible = true;
                //m_twitter_btn.Visible = false;
                m_fackbook_btn.Visible = false;
                //m_apple_btn.Visible = false;
            }
        }


        private void DoTestNetwork()
        {
            FrameMgr.Instance.StartCoroutine(ITestNetwork());
        }


        IEnumerator ITestNetwork()
        {
            //使用Get方式访问HTTP地址  
            WWW www = new WWW("http://game.fotoable-conan.com/api-web/confirm/api");

            //等待服务器的响应  
            yield return www;

            //如果出现错误  
            if (www.error != null)
            {
                m_isClick = false;
                //获取服务器的错误信息  
                Debug.LogError(www.error);
                PopUpManager.OpenNormalOnePop("systen_unusual");

                yield return null;
            }
            else
            {
                Debug.Log("net work is ok");

                //todo:临时写
                //this.GetVersion();

                if (ENUM_ACCOUNT_TYPE.E_GUEST == m_login_type)
                {
                    OnLoginButtonClick(null);
                }
                else
                {
                    OnFaceBookButtonClick(null);
                }
            }
        }

        private float m_cur_time = 0.0f;
        public override void Update()
        {
            base.Update();

            if (m_is_start_timer)
            {
                m_cur_time += Time.deltaTime;

                if (m_cur_time > 30)
                {
                    OccurEror();
                    PopUpManager.OpenNormalOnePop("systen_unusual");
                }
            }
        }

        private void SetFBBtnIcon()
        {
            IntervalSetFBBtnIcon(false);
        }

        private void IntervalSetFBBtnIcon(bool force_visible_ = false)
        {
            this.m_fackbook_btn.Visible = force_visible_ ? force_visible_ : !LoginUtil.IsFbLogin();
        }


    }
}