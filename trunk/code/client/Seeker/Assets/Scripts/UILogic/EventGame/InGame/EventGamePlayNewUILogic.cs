//#define TEST
using EngineCore;
using Google.Protobuf;
using System.Collections.Generic;

namespace SeekerGame
{
    [UILogicHandler(UIDefine.UI_EVENT_INGAME_PLAY)]
    class EventGamePlayNewUILogic : BaseViewComponetLogic
    {
        EventGamePlayNewUIView m_view;

        private long m_event_id;
        public long Event_id
        {
            get { return m_event_id; }
        }

        private SCEventPhaseFeedbackResponse m_phase_data;

        private int m_cur_dispath_index;
        public override void OnPackageRequest(IMessage imsg, params object[] msg_params)
        {

            base.OnPackageRequest(imsg, msg_params);

            if (imsg is CSEventRewardRequest)
            {
                CSEventRewardRequest req = imsg as CSEventRewardRequest;
                req.EventId = m_event_id;
            }

        }

        public override void OnScResponse(object s)
        {
            base.OnScResponse(s);



            if (s is SCEventRewardResponse)
            {
                var rsp = s as SCEventRewardResponse;

                if (!MsgStatusCodeUtil.OnError(rsp.ResponseStatus))
                {
                    this.CloseFrame();

                    if (!MsgStatusCodeUtil.OnError(rsp.ResponseStatus))
                    {
                        WinFailData data = new WinFailData(ENUM_SEARCH_MODE.E_EVENTGAME, rsp);


                        FrameMgr.OpenUIParams param = new FrameMgr.OpenUIParams(UIDefine.UI_WIN);


                        param.Param = data;

                        EngineCoreEvents.UIEvent.ShowUIEventWithParam.SafeInvoke(param);


                        Dictionary<UBSParamKeyName, object> _params = new Dictionary<UBSParamKeyName, object>()
                    {
                        { UBSParamKeyName.ContentID, EventGameManager.Instance.Event_id},
                        { UBSParamKeyName.Success, rsp.Valuation },
                    };
                        UserBehaviorStatisticsModules.Instance.LogEvent(UBSEventKeyName.event_finish, null, _params);

                        if (0 == rsp.Valuation)
                        {
                            //PushGiftManager.Instance.TurnOn(ENUM_PUSH_GIFT_BLOCK_TYPE.E_EVENT);
                        }
                    }
                    else
                    {
                        rsp.Valuation = 0;
                        rsp.Rewards.Clear();

                        WinFailData data = new WinFailData(ENUM_SEARCH_MODE.E_EVENTGAME, rsp);
                        FrameMgr.OpenUIParams param = new FrameMgr.OpenUIParams(UIDefine.UI_WIN);
                        param.Param = data;

                        EngineCoreEvents.UIEvent.ShowUIEventWithParam.SafeInvoke(param);


                        Dictionary<UBSParamKeyName, object> _params = new Dictionary<UBSParamKeyName, object>()
                    {
                        { UBSParamKeyName.ContentID, EventGameManager.Instance.Event_id},
                        { UBSParamKeyName.Success, 0 },
                        { UBSParamKeyName.Description, rsp.ResponseStatus.Code }

                    };
                        UserBehaviorStatisticsModules.Instance.LogEvent(UBSEventKeyName.event_finish, null, _params);

                        //PushGiftManager.Instance.TurnOn(ENUM_PUSH_GIFT_BLOCK_TYPE.E_EVENT);
                    }
                }
            }

        }



        public override void OnShow(object param)
        {
            base.OnShow(param);

            if (null != param)
            {
                EventGamePlayData datas = (EventGamePlayData)param;

                m_event_id = datas.EventID;
                m_phase_data = datas.Msg;
            }

            MessageHandler.RegisterMessageHandler(MessageDefine.SCEventRewardResponse, OnScResponse);

            m_view.Refresh(m_phase_data);
        }

        public override void OnHide()
        {
            base.OnHide();

            MessageHandler.UnRegisterMessageHandler(MessageDefine.SCEventRewardResponse, OnScResponse);
#if OFFICER_SYS
            EventGamePoliceDispatchManager.Instance.Clear();
#endif
        }


        protected override void OnInit()
        {
            base.OnInit();

            m_view = Make<EventGamePlayNewUIView>("Panel_animation");
            if (GlobalInfo.GAME_NETMODE == GameNetworkMode.Network)
            {
                this.SetCloseBtnID("Panel_animation:Button_pause");
            }
        }

        public override FrameDisplayMode UIFrameDisplayMode
        {
            get
            {
                return FrameDisplayMode.FULLSCREEN;
            }
        }

        public void EndGame()
        {
            this.CloseFrame();
        }

        public void RequestReward()
        {
            CSEventRewardRequest req = new CSEventRewardRequest();

            this.OnScHalfAsyncRequest(req, m_event_id);
        }




    }
}

