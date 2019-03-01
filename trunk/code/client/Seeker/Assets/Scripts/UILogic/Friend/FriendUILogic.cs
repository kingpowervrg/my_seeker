//#define TEST
using EngineCore;
using Google.Protobuf;
using System.Collections.Generic;

namespace SeekerGame
{
    [UILogicHandler(UIDefine.UI_FRIEND)]
    class FriendUILogic : BaseViewComponetLogic
    {
        private TweenScale m_tweenPos = null;
        FriendDetailUI m_friend_detail_view;
        FriendListUI m_friend_list_view;
        RecommendUI m_recommend_view;

        public override void OnPackageRequest(IMessage imsg, params object[] msg_params)
        {

            base.OnPackageRequest(imsg, msg_params);

            if (imsg is CSFriendRequest)
            {
                FriendReqType r_type = (FriendReqType)(msg_params[0]);
                CSFriendRequest req = imsg as CSFriendRequest;
                req.Type = r_type;
            }
            else if (imsg is CSFriendAddRequest)
            {
                CSFriendAddRequest req = imsg as CSFriendAddRequest;
                req.PlayerId = (long)(msg_params[0]);
            }
            else if (imsg is CSFriendDelRequest)
            {
                CSFriendDelRequest req = imsg as CSFriendDelRequest;
                req.PlayerId = (long)(msg_params[0]);
            }
            else if (imsg is CSFriendAgreeRequest)
            {
                CSFriendAgreeRequest req = imsg as CSFriendAgreeRequest;
                req.PlayerId = (long)(msg_params[0]);
            }
            else if (imsg is CSFriendDelApplyRequest)
            {
                CSFriendDelApplyRequest req = imsg as CSFriendDelApplyRequest;
                req.FriendId = (long)(msg_params[0]);
            }
            else if (imsg is CSFriendGiftRequest)
            {
                CSFriendGiftRequest req = imsg as CSFriendGiftRequest;
            }
            else if (imsg is CSFriendGiftSendRequest)
            {
                CSFriendGiftSendRequest req = imsg as CSFriendGiftSendRequest;
                req.FriendId = (long)(msg_params[0]);
            }
            else if (imsg is CSFriendGiftDrawRequest)
            {
                CSFriendGiftDrawRequest req = imsg as CSFriendGiftDrawRequest;
                req.GiftId = (long)(msg_params[0]);
            }
            else if (imsg is CSFriendViewRequest)
            {
                CSFriendViewRequest req = imsg as CSFriendViewRequest;
                req.FriendId = (long)(msg_params[0]);
            }
            else if (imsg is CSFriendRecommendListRequest)
            {

            }
            else if (imsg is CSFriendRecommendGetRequest)
            {
                CSFriendRecommendGetRequest req = imsg as CSFriendRecommendGetRequest;
                req.RecommendId = (long)(msg_params[0]);
            }
            else if (imsg is CSFriendRecommendApplyRequest)
            {
                CSFriendRecommendApplyRequest req = imsg as CSFriendRecommendApplyRequest;
                req.RecommendId.AddRange((List<long>)(msg_params[0]));
            }
        }

        public override void OnScResponse(object s)
        {
            base.OnScResponse(s);



            if (s is SCFriendResponse)
            {
                var rsp = s as SCFriendResponse;

                if (MsgStatusCodeUtil.OnError(rsp.Status))
                    return;

                if (FriendReqType.Added == rsp.Type)
                {
                    FriendDataManager.Instance.Last_apply = rsp.LastAgree;
                    FriendDataManager.Instance.Is_receive_application = !rsp.AddSwitch;
                }
                else if (FriendReqType.Agreeing == rsp.Type)
                {
                    GameEvents.RedPointEvents.Sys_OnNewApplyReadedEvent.SafeInvoke();
                }
                else if (FriendReqType.Addinfo == rsp.Type)
                {
                    FriendDataManager.Instance.Last_apply = false;
                    GameEvents.RedPointEvents.Sys_OnNewFriendReadedEvent.SafeInvoke();
                }


                if (this.m_friend_list_view.Visible)
                {
                    FriendDataManager.Instance.SetDatas(rsp.Friends, rsp.Type);
                    FriendDataManager.Instance.Max_friend_num = rsp.Limit;
                    FriendDataManager.Instance.Send_gift_left_num = rsp.GiftCountLeft;

                    this.m_friend_list_view.Refresh((FRIEND_UI_TOGGLE_TYPE)rsp.Type);
                }


            }
            else if (s is SCFriendAddResponse)
            {
                var rsp = s as SCFriendAddResponse;
                if (MsgStatusCodeUtil.OnError(rsp.Status))
                    return;



                var raw_req = EngineCoreEvents.SystemEvents.GetRspPairReq.SafeInvoke();
                CSFriendAddRequest req = raw_req as CSFriendAddRequest;
                GameEvents.UIEvents.UI_Friend_Event.Tell_add_recommend_firend_ok.SafeInvoke(req.PlayerId);

                EngineCoreEvents.AudioEvents.PlayAudio.SafeInvoke(Audio.AudioType.UISound, GameCustomAudioKey.friend_send.ToString());
            }
            else if (s is SCFriendDelResponse)
            {
                var rsp = s as SCFriendDelResponse;
                if (MsgStatusCodeUtil.OnError(rsp.Status))
                    return;



                if (this.m_friend_list_view.Visible)
                {

                    this.m_friend_list_view.Refresh(FRIEND_UI_TOGGLE_TYPE.Added);
                }

            }
            else if (s is SCFriendAgreeResponse)
            {
                var rsp = s as SCFriendAgreeResponse;

                if (MsgStatusCodeUtil.OnError(rsp.Status))
                {
                    if (!(MsgStatusCodeUtil.FRIEND_ADDED == rsp.Status.Code))
                        return; //添加已存在的好友，需要刷新申请界面
                }

                //半单机状态，添加好友，要等到服务器新好友提醒消息，才能显示已添加的好友，太慢了。所以在这里主动刷新
                RequestFriendInfos();

                if (this.m_friend_list_view.Visible)
                {
                    this.m_friend_list_view.Refresh(FRIEND_UI_TOGGLE_TYPE.Agreeing);
                }
            }
            else if (s is SCFriendDelApplyResponse)
            {
                var rsp = s as SCFriendDelApplyResponse;
                if (MsgStatusCodeUtil.OnError(rsp.Status))
                    return;

                if (this.m_friend_list_view.Visible)
                {
                    this.m_friend_list_view.Refresh(FRIEND_UI_TOGGLE_TYPE.Agreeing);
                }
            }
            else if (s is SCFriendGiftResponse)
            {
                var rsp = s as SCFriendGiftResponse;
                if (MsgStatusCodeUtil.OnError(rsp.Status))
                    return;

                FriendDataManager.Instance.Receive_gift_max_num = rsp.Limit;
                FriendDataManager.Instance.Receive_gift_left_num = rsp.Limit - rsp.Count;
                FriendDataManager.Instance.SetGifts(rsp.FriendGiftLists);

                if (m_friend_list_view.Visible)
                {
                    this.m_friend_list_view.Refresh(FRIEND_UI_TOGGLE_TYPE.gift);
                    GameEvents.RedPointEvents.Sys_OnNewFriendReadedEvent.SafeInvoke();
                }


            }
            else if (s is SCFriendGiftSendResponse)
            {
                var rsp = s as SCFriendGiftSendResponse;
                if (MsgStatusCodeUtil.OnError(rsp.Status))
                    return;
            }
            else if (s is SCFriendGiftDrawResponse)
            {
                var rsp = s as SCFriendGiftDrawResponse;
                if (MsgStatusCodeUtil.OnError(rsp.Status))
                    return;

                if (m_friend_list_view.Visible)
                {
                    this.m_friend_list_view.Refresh(FRIEND_UI_TOGGLE_TYPE.gift);
                    GameEvents.RedPointEvents.Sys_OnNewFriendReadedEvent.SafeInvoke();
                }

                //if (m_gift_list_view.Visible)
                //{
                //    m_gift_list_view.Refresh();

                //    SCDropResp res = new SCDropResp();

                //    foreach (var item in rsp.PlayerPropMsg)
                //    {
                //        long item_id = item.PropId;
                //        int item_count = item.Count;

                //        DropInfo info = new DropInfo();
                //        info.PropId = item_id;
                //        info.Count = item_count;

                //        res.DropInfos.Add(info);
                //    }

                //    FrameMgr.OpenUIParams param = new FrameMgr.OpenUIParams(UIDefine.UI_GIFTRESULT);
                //    param.Param = res;
                //    EngineCoreEvents.UIEvent.ShowUIEventWithParam.SafeInvoke(param);
                //}

                if (m_friend_list_view.Visible)
                {
                    SCDropResp res = new SCDropResp();

                    foreach (var item in rsp.PlayerPropMsg)
                    {
                        long item_id = item.PropId;
                        int item_count = item.Count;

                        DropInfo info = new DropInfo();
                        info.PropId = item_id;
                        info.Count = item_count;

                        res.DropInfos.Add(info);
                    }

                    GameEvents.UIEvents.UI_GameEntry_Event.Listen_OnCombinePropCollected.SafeInvoke();

                    FrameMgr.OpenUIParams param = new FrameMgr.OpenUIParams(UIDefine.UI_GIFTRESULT);
                    param.Param = res;
                    EngineCoreEvents.UIEvent.ShowUIEventWithParam.SafeInvoke(param);
                }


            }
            else if (s is SCFriendViewResponse)
            {
                var rsp = s as SCFriendViewResponse;
                if (MsgStatusCodeUtil.OnError(rsp.Status))
                    return;

                if (m_friend_detail_view.Visible)
                {
                    m_friend_detail_view.Refresh(rsp.PlayerFriendMsg, rsp.AchievementMsgs);
                    //m_friend_detail_view.Refresh(rsp.PlayerFriendMsg, new List<long>() { 1, 2, 3, 4, 5 });
                }
            }
            else if (s is SCFriendSwitchResponse)
            {
                var rsp = s as SCFriendSwitchResponse;
                if (MsgStatusCodeUtil.OnError(rsp.Status))
                    return;
            }
            else if (s is SCFriendRecommendGetResponse)
            {
                var rsp = s as SCFriendRecommendGetResponse;
                if (MsgStatusCodeUtil.OnError(rsp.Status))
                    return;

                m_friend_detail_view.Refresh(rsp.PlayerFriendMsg, rsp.AchievementMsgs);
                m_friend_detail_view.Visible = true;

            }
            else if (s is SCFriendRecommendListResponse)
            {
                var rsp = s as SCFriendRecommendListResponse;

                if (MsgStatusCodeUtil.OnError(rsp.Status))
                    return;

                FriendDataManager.Instance.Recommends.Clear();

                FriendDataManager.Instance.Recommends.AddRange(rsp.Recommend);

                FriendDataManager.Instance.Recommend_expire_date = System.DateTime.Now.AddSeconds(rsp.TimeDown);

                m_recommend_view.Refresh();


            }
            else if (s is SCFriendRecommendApplyResponse)
            {
                var rsp = s as SCFriendRecommendApplyResponse;

                if (MsgStatusCodeUtil.OnError(rsp.Status))
                    return;

                foreach (var item in rsp.RecommendId)
                {
                    var one_recommend = FriendDataManager.Instance.Recommends.Find((recommend) => recommend.RecommendId == item);
                    one_recommend.Status = (int)ENUM_RECOMMEND_STATUS.E_ADDED;
                    GameEvents.UIEvents.UI_Friend_Event.Tell_add_recommend_firend_ok.SafeInvoke(item);
                }

                var valids = FriendDataManager.Instance.Recommends.FindAll((item) => ENUM_RECOMMEND_STATUS.E_RECOMMEND == (ENUM_RECOMMEND_STATUS)item.Status);

                if (null == valids || 0 == valids.Count)
                {
                    m_recommend_view.Refresh();
                }
            }
            else
            {
                return;
            }

            FriendMsgCodeUtil.OnSuccess(s as IMessage);

        }



        public override void OnShow(object param)
        {
            EngineCoreEvents.AudioEvents.PlayAudio.SafeInvoke(Audio.AudioType.UISound, GameCustomAudioKey.zoom_in.ToString());

            base.OnShow(param);

            //MessageHandler.RegisterMessageHandler(MessageDefine.SCFriendResponse, OnScResponse);
            MessageHandler.RegisterMessageHandler(MessageDefine.SCFriendAddResponse, OnScResponse);
            MessageHandler.RegisterMessageHandler(MessageDefine.SCFriendDelResponse, OnScResponse);
            MessageHandler.RegisterMessageHandler(MessageDefine.SCFriendAgreeResponse, OnScResponse);
            MessageHandler.RegisterMessageHandler(MessageDefine.SCFriendDelApplyResponse, OnScResponse);
            //MessageHandler.RegisterMessageHandler(MessageDefine.SCFriendGiftResponse, OnScResponse);
            MessageHandler.RegisterMessageHandler(MessageDefine.SCFriendGiftSendResponse, OnScResponse);
            MessageHandler.RegisterMessageHandler(MessageDefine.SCFriendGiftDrawResponse, OnScResponse);
            MessageHandler.RegisterMessageHandler(MessageDefine.SCFriendViewResponse, OnScResponse);
            MessageHandler.RegisterMessageHandler(MessageDefine.SCFriendSwitchResponse, OnScResponse);
            MessageHandler.RegisterMessageHandler(MessageDefine.SCFriendRecommendListResponse, OnScResponse);
            MessageHandler.RegisterMessageHandler(MessageDefine.SCFriendRecommendGetResponse, OnScResponse);
            MessageHandler.RegisterMessageHandler(MessageDefine.SCFriendRecommendApplyResponse, OnScResponse);


            GameEvents.UIEvents.UI_Friend_Event.OnRefreshFriendPage += OnRefreshPage;
            GameEvents.UIEvents.UI_Friend_Event.OnInfoChanged += OnInfoChanged;
            GameEvents.UIEvents.UI_Friend_Event.OnApplicationChanged += OnApplicationChanged;
            GameEvents.UIEvents.UI_Friend_Event.OnGiftChanged += OnGiftChanged;
            GameEvents.UIEvents.UI_Friend_Event.Listen_add_recommend_friend += RequestAddRecommend;
            GameEvents.UIEvents.UI_Friend_Event.Listen_check_recommend_info += RequestCheckRecommendInfo;
            GameEvents.UIEvents.UI_Friend_Event.Listen_ShowView += SwitchUI;
            GameEvents.UIEvents.UI_Friend_Event.Listen_AddFriend += RequestAddFriend;


            this.SwitchUI(ENUM_FRIEND_VIEW_TYPE.E_FRIEND_INFO);


            this.RequestFriendInfos();

        }

        public override void OnHide()
        {

            EngineCoreEvents.AudioEvents.PlayAudio.SafeInvoke(Audio.AudioType.UISound, GameCustomAudioKey.zoom_out.ToString());
            base.OnHide();


            MessageHandler.UnRegisterMessageHandler(MessageDefine.SCFriendResponse, OnScResponse);
            MessageHandler.UnRegisterMessageHandler(MessageDefine.SCFriendAddResponse, OnScResponse);
            MessageHandler.UnRegisterMessageHandler(MessageDefine.SCFriendDelResponse, OnScResponse);
            MessageHandler.UnRegisterMessageHandler(MessageDefine.SCFriendAgreeResponse, OnScResponse);
            MessageHandler.UnRegisterMessageHandler(MessageDefine.SCFriendDelApplyResponse, OnScResponse);
            MessageHandler.UnRegisterMessageHandler(MessageDefine.SCFriendGiftResponse, OnScResponse);
            MessageHandler.UnRegisterMessageHandler(MessageDefine.SCFriendGiftSendResponse, OnScResponse);
            MessageHandler.UnRegisterMessageHandler(MessageDefine.SCFriendGiftDrawResponse, OnScResponse);
            MessageHandler.UnRegisterMessageHandler(MessageDefine.SCFriendViewResponse, OnScResponse);
            MessageHandler.UnRegisterMessageHandler(MessageDefine.SCFriendSwitchResponse, OnScResponse);
            MessageHandler.UnRegisterMessageHandler(MessageDefine.SCFriendRecommendListResponse, OnScResponse);
            MessageHandler.UnRegisterMessageHandler(MessageDefine.SCFriendRecommendGetResponse, OnScResponse);
            MessageHandler.UnRegisterMessageHandler(MessageDefine.SCFriendRecommendApplyResponse, OnScResponse);


            GameEvents.UIEvents.UI_Friend_Event.OnRefreshFriendPage -= OnRefreshPage;
            GameEvents.UIEvents.UI_Friend_Event.OnInfoChanged -= OnInfoChanged;
            GameEvents.UIEvents.UI_Friend_Event.OnApplicationChanged -= OnApplicationChanged;
            GameEvents.UIEvents.UI_Friend_Event.OnGiftChanged -= OnGiftChanged;
            GameEvents.UIEvents.UI_Friend_Event.Listen_add_recommend_friend -= RequestAddRecommend;
            GameEvents.UIEvents.UI_Friend_Event.Listen_check_recommend_info -= RequestCheckRecommendInfo;
            GameEvents.UIEvents.UI_Friend_Event.Listen_ShowView -= SwitchUI;
            GameEvents.UIEvents.UI_Friend_Event.Listen_AddFriend -= RequestAddFriend;
        }


        void OnRefreshPage(FriendReqType type_)
        {
            if (m_friend_list_view.Visible)
            {
#if TEST
                        m_friend_list_view.Refresh(type_);

#else
                if (FriendReqType.Added == type_)
                {
                    RequestFriendInfos();
                }
                else if (FriendReqType.Agreeing == type_)
                {

                    RequestApplications();

                }
                else if (FriendReqType.Addinfo == type_)
                {
                    RequestConfirms();
                }

                if (FriendReqType.Added == type_)
                {
                    FriendDataManager.Instance.Last_apply = false;
                }
                else if (FriendReqType.Agreeing == type_)
                {
                    GameEvents.RedPointEvents.Sys_OnNewApplyReadedEvent.SafeInvoke();
                }
                else if (FriendReqType.Addinfo == type_)
                {
                    FriendDataManager.Instance.Last_apply = false;
                    GameEvents.RedPointEvents.Sys_OnNewFriendReadedEvent.SafeInvoke();
                }




#endif
            }

        }

        void OnInfoChanged(long player_id_, ENUM_INFO_CONTROL val_)
        {
            if (ENUM_INFO_CONTROL.E_DEL == val_)
            {
                FriendDataManager.Instance.RemoveData(player_id_, FriendReqType.Added);
#if TEST
                this.m_friend_list_view.Refresh(FriendReqType.Added);
#else
                RequestDelFriend(player_id_);
#endif
            }
            else if (ENUM_INFO_CONTROL.E_DETAIL == val_)
            {
#if TEST
                this.SwitchUI(ENUM_FRIEND_VIEW_TYPE.E_FRIEND_DETAIL);
                this.m_friend_detail_view.Refresh(FriendDataManager.Instance.GetData(player_id_, FriendReqType.Addinfo));
#else
                this.SwitchUI(ENUM_FRIEND_VIEW_TYPE.E_FRIEND_DETAIL);
                RequestDetail(player_id_);
#endif
            }
            else if (ENUM_INFO_CONTROL.E_GIFT == val_)
            {
                if (FriendDataManager.Instance.Send_gift_left_num > 0)
                {
                    FriendDataManager.Instance.GetData(player_id_, FriendReqType.Added).Gift = false;
#if !TEST
                    RequestSendGift(player_id_);
#endif
                }
                else
                {
                    if (this.m_friend_list_view.Visible)
                    {
                        this.m_friend_list_view.Refresh(FRIEND_UI_TOGGLE_TYPE.Added);
                    }

                    ResponseStatus status = new ResponseStatus();
                    status.Code = MsgStatusCodeUtil.FRIEND_GIFT_LIMIT;

                    MsgStatusCodeUtil.OnError(status);
                }
            }
        }

        void OnApplicationChanged(long player_id_, ENUM_APPLICATION_CONTROL val_)
        {
            if (ENUM_APPLICATION_CONTROL.E_DEL == val_)
            {
                FriendDataManager.Instance.RemoveData(player_id_, FriendReqType.Agreeing);
#if TEST
                this.m_friend_list_view.Refresh(FriendReqType.Agreeing);
#else
                this.RequestDelApplication(player_id_);

#endif
            }
            else if (ENUM_APPLICATION_CONTROL.E_DEL_ALL == val_)
            {
                FriendDataManager.Instance.RemoveDatas(FriendReqType.Agreeing);
#if TEST
                this.m_friend_list_view.Refresh(FriendReqType.Agreeing);
#else
                this.RequestDelAllApplication();

#endif
            }
            if (ENUM_APPLICATION_CONTROL.E_OK == val_)
            {
                FriendDataManager.Instance.RemoveData(player_id_, FriendReqType.Agreeing);
#if TEST
                this.m_friend_list_view.Refresh(FriendReqType.Agreeing);
#else
                this.RequestAgreeApplication(player_id_);

#endif
            }
        }



        void OnGiftChanged(long gift_id_, ENUM_GIFT_CONTROL val_)
        {
            if (ENUM_GIFT_CONTROL.E_ONE == val_)
            {
#if TEST
                this.m_gift_list_view.Refresh();
#else
                int count = FriendDataManager.Instance.ReceiveGiftLeftNumReduce(1);

                if (count > 0)
                {
                    FriendDataManager.Instance.RemoveGift(gift_id_);
                    this.RequestReceiveGift(gift_id_);
                }

#endif
            }
            else if (ENUM_GIFT_CONTROL.E_ALL == val_)
            {
#if TEST
                this.m_gift_list_view.Refresh();
#else
                int count = FriendDataManager.Instance.ReceiveGiftLeftNumReduce(FriendDataManager.Instance.GetGifts().Count);

                if (count > 0)
                {
                    if (count < FriendDataManager.Instance.GetGifts().Count)
                    {
                        FriendDataManager.Instance.RemoveRecentGifts(count);
                    }
                    else
                        FriendDataManager.Instance.RemoveAllGifts();

                    this.RequestReceiveAllGift();
                }

#endif
            }

        }

        public override FrameDisplayMode UIFrameDisplayMode
        {
            get
            {
                return FrameDisplayMode.FULLSCREEN;
            }
        }
        protected override void OnInit()
        {
            base.OnInit();

            //m_add_friend_view = this.Make<AddFriendUI>("Panel_search");
            m_friend_detail_view = this.Make<FriendDetailUI>("Panel_detail");
            m_friend_list_view = this.Make<FriendListUI>("Panel_animation:Panel_down");
            this.m_tweenPos = this.m_friend_list_view.GetComponent<TweenScale>();
            //m_gift_list_view = this.Make<GiftListUI>("Panel_gift");
            m_recommend_view = this.Make<RecommendUI>("Panel_recommend");

            this.SetCloseBtnID("Button_close");


#if TEST
            List<PlayerFriendMsg> t_list = new List<PlayerFriendMsg>();

            PlayerFriendMsg t_msg = new PlayerFriendMsg();
            t_msg.PlayerId = 50000L;
            t_msg.Name = t_msg.PlayerId.ToString();
            t_msg.Level = (int)t_msg.PlayerId;
            t_msg.Gender = 1;
            t_msg.Icon = "http://www.betasoft.cn/Public/HOME/images/to_job.jpg";
            t_msg.TitleId = 1;
            t_msg.Gift = true;
            t_msg.StatusTime = 0;

            t_list.Add(t_msg);

            t_msg = new PlayerFriendMsg();
            t_msg.PlayerId = 50001L;
            t_msg.Name = t_msg.PlayerId.ToString();
            t_msg.Level = (int)t_msg.PlayerId;
            t_msg.Gender = 1;
            t_msg.Icon = "image_player_size3_1.png";
            t_msg.TitleId = 2;
            t_msg.Gift = false;
            t_msg.StatusTime = 0;
            t_list.Add(t_msg);

            t_msg = new PlayerFriendMsg();
            t_msg.PlayerId = 50002L;
            t_msg.Name = t_msg.PlayerId.ToString();
            t_msg.Level = (int)t_msg.PlayerId;
            t_msg.Gender = 2;
            t_msg.Icon = "image_player_size3_1.png";
            t_msg.TitleId = 3;
            t_msg.Gift = false;
            t_msg.StatusTime = 0;
            t_list.Add(t_msg);

            FriendDataManager.Instance.SetDatas(t_list, FriendReqType.Added);
            FriendDataManager.Instance.SetDatas(t_list, FriendReqType.Agreeing);
            FriendDataManager.Instance.SetDatas(t_list, FriendReqType.Addinfo);



            List<FriendGift> gift_list = new List<FriendGift>();

            FriendGift gift_msg = new FriendGift();
            gift_msg.GiftId = 1001L;

            PlayerFriendMsg p_msg = new PlayerFriendMsg();
            p_msg.PlayerId = 50000L;
            p_msg.Name = p_msg.PlayerId.ToString();
            p_msg.Level = (int)p_msg.PlayerId;
            p_msg.Gender = 1;
            p_msg.Icon = "image_player_size3_1.png";
            p_msg.TitleId = 1;
            p_msg.Gift = true;
            p_msg.StatusTime = 0;

            gift_msg.PlayerFriends = p_msg;

            gift_list.Add(gift_msg);

            gift_msg = new FriendGift();
            gift_msg.GiftId = 1002L;
            gift_msg.PlayerFriends = p_msg;

            gift_list.Add(gift_msg);

            FriendDataManager.Instance.SetGifts(gift_list);

#endif

        }

        public void RequestFriendInfos()
        {
            CSFriendRequest req = new CSFriendRequest();

            this.OnScAsyncRequest(req, FriendReqType.Added);
        }

        public void RequestApplications()
        {
            CSFriendRequest req = new CSFriendRequest();

            this.OnScAsyncRequest(req, FriendReqType.Agreeing);

        }

        public void RequestConfirms()
        {
            CSFriendRequest req = new CSFriendRequest();


            this.OnScAsyncRequest(req, FriendReqType.Addinfo);


        }

        public void RequestAddFriend(long player_id_)
        {
            CSFriendAddRequest req = new CSFriendAddRequest();

#if !NETWORK_SYNC || UNITY_EDITOR
            this.OnScHalfAsyncRequest(req, player_id_);
#else
            this.OnScRequest(req, player_id_);
#endif

        }

        public void RequestAddRecommend(List<long> player_ids_)
        {
            CSFriendRecommendApplyRequest req = new CSFriendRecommendApplyRequest();

            this.OnScAsyncRequest(req, player_ids_);
        }

        public void RequestCheckRecommendInfo(long player_id_)
        {
            CSFriendRecommendGetRequest req = new CSFriendRecommendGetRequest();
            this.OnScAsyncRequest(req, player_id_);
        }

        public void RequestDelFriend(long player_id_)
        {
            CSFriendDelRequest req = new CSFriendDelRequest();
#if !NETWORK_SYNC || UNITY_EDITOR
            this.OnScAsyncRequest(req, player_id_);
#else
            this.OnScRequest(req, player_id_);
#endif


        }

        public void RequestAgreeApplication(long player_id_)
        {
            CSFriendAgreeRequest req = new CSFriendAgreeRequest();
#if !NETWORK_SYNC || UNITY_EDITOR
            this.OnScAsyncRequest(req, player_id_);
#else
            this.OnScRequest(req, player_id_);
#endif


        }

        public void RequestDetail(long player_id_)
        {
            CSFriendViewRequest req = new CSFriendViewRequest();

#if !NETWORK_SYNC || UNITY_EDITOR
            this.OnScAsyncRequest(req, player_id_);
#else
            this.OnScRequest(req, player_id_);
#endif


        }


        public void RequestDelApplication(long player_id_)
        {
            CSFriendDelApplyRequest req = new CSFriendDelApplyRequest();

#if !NETWORK_SYNC || UNITY_EDITOR
            this.OnScAsyncRequest(req, player_id_);
#else
            this.OnScRequest(req, player_id_);
#endif

        }

        public void RequestDelAllApplication()
        {
            CSFriendDelApplyRequest req = new CSFriendDelApplyRequest();
#if !NETWORK_SYNC || UNITY_EDITOR
            this.OnScAsyncRequest(req, 0L);
#else
            this.OnScRequest(req, 0L);
#endif

        }

        public void RequestViewGift()
        {
            CSFriendGiftRequest req = new CSFriendGiftRequest();

            this.OnScAsyncRequest(req);

        }

        public void RequestSwitchApplication()
        {
            CSFriendSwitchRequest req = new CSFriendSwitchRequest();

#if !NETWORK_SYNC || UNITY_EDITOR
            this.OnScAsyncRequest(req);
#else
            this.OnScRequest(req);
#endif

        }


        public void RequestSendGift(long player_id_)
        {
            CSFriendGiftSendRequest req = new CSFriendGiftSendRequest();

#if !NETWORK_SYNC || UNITY_EDITOR
            this.OnScAsyncRequest(req, player_id_);
#else
            this.OnScRequest(req, player_id_);
#endif



        }

        public void RequestReceiveGift(long gift_id_)
        {
            CSFriendGiftDrawRequest req = new CSFriendGiftDrawRequest();

#if !NETWORK_SYNC || UNITY_EDITOR
            this.OnScAsyncRequest(req, gift_id_);
#else
            this.OnScRequest(req, gift_id_);
#endif

        }

        public void RequestReceiveAllGift()
        {
            CSFriendGiftDrawRequest req = new CSFriendGiftDrawRequest();

#if !NETWORK_SYNC || UNITY_EDITOR
            this.OnScAsyncRequest(req, 0L);
#else
            this.OnScRequest(req, 0L);
#endif

        }

        public void RequestRecommends()
        {
            CSFriendRecommendListRequest req = new CSFriendRecommendListRequest();
            this.OnScAsyncRequest(req);

        }


        public void SwitchUI(ENUM_FRIEND_VIEW_TYPE type_)
        {
            switch (type_)
            {

                case ENUM_FRIEND_VIEW_TYPE.E_FRIEND_DETAIL:
                    {
                        m_friend_detail_view.Visible = true;
                    }
                    break;
                case ENUM_FRIEND_VIEW_TYPE.E_FRIEND_INFO:
                    {

                        m_recommend_view.Visible = false;
                        m_friend_detail_view.Visible = false;
                        m_friend_list_view.Visible = true;


                    }
                    break;
                case ENUM_FRIEND_VIEW_TYPE.E_RECOMMEND:
                    {

                        m_recommend_view.Visible = true;
                        m_friend_detail_view.Visible = false;


                    }
                    break;

            }
        }


        public void ShowRecommendView(bool v_)
        {
            m_recommend_view.Visible = v_;

        }

    }
}

