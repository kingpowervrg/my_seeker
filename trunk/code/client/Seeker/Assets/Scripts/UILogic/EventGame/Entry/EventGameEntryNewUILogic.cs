//#define TEST
using EngineCore;
using GOGUI;
using Google.Protobuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SeekerGame
{
    [UILogicHandler(UIDefine.UI_EVENT_INGAME_ENTRY)]
    class EventGameEntryNewUILogic : BaseViewComponetLogic
    {
        EventGameEntryNewUIView m_view;

        private long m_event_id;
        public long Event_id
        {
            get { return m_event_id; }
        }
        private int m_cur_dispath_index;
        public override void OnPackageRequest(IMessage imsg, params object[] msg_params)
        {

            base.OnPackageRequest(imsg, msg_params);

            if (imsg is CSEventPhaseFeedbackRequest)
            {
                long event_id = (long)(msg_params[0]);
                List<long> officer_player_ids = (List<long>)(msg_params[1]);
                CSEventPhaseFeedbackRequest req = imsg as CSEventPhaseFeedbackRequest;

                req.EventId = event_id;
                req.PlayerOfficerId.AddRange(officer_player_ids);
            }
            else if (imsg is CSEventEnterRequest)
            {
                CSEventEnterRequest req = imsg as CSEventEnterRequest;
                req.EventId = m_event_id;

            }

        }

        public override void OnScResponse(object s)
        {
            base.OnScResponse(s);



            if (s is SCEventPhaseFeedbackResponse)
            {
                var rsp = s as SCEventPhaseFeedbackResponse;

                if (!MsgStatusCodeUtil.OnError(rsp.ResponseStatus))
                {
                    EventGamePlayData data = new EventGamePlayData()
                    {
                        EventID = m_event_id,
                        Msg = rsp,
                    };

                    FrameMgr.OpenUIParams open_data = new FrameMgr.OpenUIParams(UIDefine.UI_EVENT_INGAME_PLAY);
                    open_data.Param = data;

                    EngineCoreEvents.UIEvent.ShowUIEventWithParam.SafeInvoke(open_data);

                    this.CloseFrame();
                }
            }
            else if (s is SCEventEnterResponse)
            {
                var rsp = s as SCEventEnterResponse;

                if (!MsgStatusCodeUtil.OnError(rsp.Result))
                {
                    RequestStartPhase();

                    {
                        Dictionary<UBSParamKeyName, object> _params = new Dictionary<UBSParamKeyName, object>()
                    {
                        { UBSParamKeyName.Success, 1},
                        {UBSParamKeyName.ContentID, m_event_id }
                    };
                        UserBehaviorStatisticsModules.Instance.LogEvent(UBSEventKeyName.event_begin, null, _params);
                    }

                }
                else
                {
                    if (MsgStatusCodeUtil.VIT_OUT == rsp.Result)
                    {
                        Dictionary<UBSParamKeyName, object> _params = new Dictionary<UBSParamKeyName, object>()
                    {
                        { UBSParamKeyName.Success, 0},
                        { UBSParamKeyName.Description, UBSDescription.NOT_ENOUGH_VIT},
                        {UBSParamKeyName.ContentID, m_event_id }
                    };
                        UserBehaviorStatisticsModules.Instance.LogEvent(UBSEventKeyName.event_begin, null, _params);
                    }
                    else
                    {
                        Dictionary<UBSParamKeyName, object> _params = new Dictionary<UBSParamKeyName, object>()
                    {
                        { UBSParamKeyName.Success, 0},
                        { UBSParamKeyName.Description, rsp.Result},
                            {UBSParamKeyName.ContentID, m_event_id }
                    };
                        UserBehaviorStatisticsModules.Instance.LogEvent(UBSEventKeyName.event_begin, null, _params);
                    }
                }

            }

        }



        public override void OnShow(object param)
        {
            base.OnShow(param);

            if (param is EventGameEntryData)
            {
                m_event_id = ((EventGameEntryData)param).M_event_id;
            }

            MessageHandler.RegisterMessageHandler(MessageDefine.SCEventPhaseFeedbackResponse, OnScResponse);
            MessageHandler.RegisterMessageHandler(MessageDefine.SCEventEnterResponse, OnScResponse);
#if OFFICER_SYS
            GameEvents.UIEvents.UI_EventGame_Event.Listen_NeedAnOfficer += NeedAnOfficerForEventGame;
            GameEvents.UIEvents.UI_EventGame_Event.Listen_RemoveAnOfficer += RemoveAnOfficerForEventGame;

            GameEvents.UIEvents.UI_Enter_Event.EVT_SELECT_POLICE += Evt_Select_Police_Obersver;
#endif


            m_view.Refresh(ConfEvent.Get(Event_id).phases.Length);

        }

        public override void OnHide()
        {
            base.OnHide();


            MessageHandler.UnRegisterMessageHandler(MessageDefine.SCEventPhaseFeedbackResponse, OnScResponse);
            MessageHandler.UnRegisterMessageHandler(MessageDefine.SCEventEnterResponse, OnScResponse);
#if OFFICER_SYS
            GameEvents.UIEvents.UI_EventGame_Event.Listen_NeedAnOfficer -= NeedAnOfficerForEventGame;
            GameEvents.UIEvents.UI_EventGame_Event.Listen_RemoveAnOfficer -= RemoveAnOfficerForEventGame;

            GameEvents.UIEvents.UI_Enter_Event.EVT_SELECT_POLICE -= Evt_Select_Police_Obersver;
                        EventGamePoliceDispatchManager.Instance.Clear();
#endif

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

            m_view = Make<EventGameEntryNewUIView>("Panel_down");
            this.SetCloseBtnID("Panel_down:Button_close");
        }

        public void RequestEnter()
        {
#if OFFICER_SYS
            List<long> officeIDList = EventGamePoliceDispatchManager.Instance.GetAllDispathOfficersID();
            if (officeIDList.Count == 0)
            {
                Dictionary<UBSParamKeyName, object> _params = new Dictionary<UBSParamKeyName, object>()
                    {
                        { UBSParamKeyName.Success, 0},
                        { UBSParamKeyName.Description, UBSDescription.NO_OFFICER_SELECTED},
                        {UBSParamKeyName.SceneID, m_event_id }
                    };
                UserBehaviorStatisticsModules.Instance.LogEvent(UBSEventKeyName.event_begin, null, _params);

                PopUpManager.OpenPopUp(new PopUpData()
                {
                    isOneBtn = true,
                    content = "UI_ENTER_GAME_NO_OFFICE",
                });

                return;
            }
#endif

            CSEventEnterRequest req = new CSEventEnterRequest();

            this.OnScHalfAsyncRequest(req, m_event_id);

        }

        public void RequestStartPhase()
        {
            CSEventPhaseFeedbackRequest req = new CSEventPhaseFeedbackRequest();
#if OFFICER_SYS
            List<long> officer_player_ids = EventGameUIAssist.GetOfficerPlayerIdsByOfficerIDs(EventGamePoliceDispatchManager.Instance.GetAllDispathOfficersID());
            this.OnScHalfAsyncRequest(req, m_event_id, officer_player_ids);
#else
            this.OnScHalfAsyncRequest(req, m_event_id, new List<long>());
#endif


        }


#if OFFICER_SYS
        private void NeedAnOfficerForEventGame(int idx_)
        {
            m_cur_dispath_index = idx_;

            if (EventGamePoliceDispatchManager.Instance.GetAllDispathOfficersID().Count == GlobalInfo.MY_PLAYER_INFO.Officer_infos.Count)
            {
                PopUpManager.OpenNormalOnePop("action_start_no_police");
                return;
            }

            GameEvents.UIEvents.UI_Enter_Event.Tell_GetAllDispatchedOfficerID += EventGamePoliceDispatchManager.Instance.GetAllDispathOfficersID;
            GameEvents.UIEvents.UI_Enter_Event.Tell_GetGameType += EventGamePoliceDispatchManager.Instance.GetGameType;
            EngineCoreEvents.UIEvent.ShowUIEvent.SafeInvoke(UIDefine.UI_SELECT_POLICE_GRID);
        }

        private void RemoveAnOfficerForEventGame(long officer_id_)
        {
            EventGamePoliceDispatchManager.Instance.RemoveDispatch(officer_id_);
            m_view.RefreshDispatch(0, 0, false);
        }

        private void Evt_Select_Police_Obersver(long officer_id_)
        {
            if (!EventGamePoliceDispatchManager.Instance.AddDispatch(this.m_cur_dispath_index, officer_id_))
                return;

            m_view.RefreshDispatch(this.m_cur_dispath_index, officer_id_);
        }
#endif
    }
}

