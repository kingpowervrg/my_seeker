
//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using EngineCore;
//using GOEngine;
//using GOGUI;
//using Google.Protobuf;

//namespace SeekerGame
//{
//    //[UILogicHandler(UIDefine.UI_EVENT_INGAME_PLAY)]
//    public class EventGamePlayUILogic : BaseViewComponetLogic
//    {
//        private EventGamePlayUIView m_view;
//        private bool m_is_game_end;

//        protected override void OnInit()
//        {
//            base.OnInit();

//            m_view = Make<EventGamePlayUIView>(root.name);
//        }

//        public override void OnShow(object param)
//        {
//            base.OnShow(param);

//            var data = param as List<long>;
//            long event_id = data[0];
//            long phase_id = data[1];

//            ConfEvent event_data = ConfEvent.Get(event_id);
//            ConfEventPhase phase_data = ConfEventPhase.Get(phase_id);
//            Dictionary<int, string> phase_key_word_dict = EventGameUIAssist.GetPhaseKeyWords(phase_data);
//            EventGameManager.Instance.CurPhaseKeyWordCount = phase_key_word_dict.Count;


//            GameEvents.UIEvents.UI_Pause_Event.OnQuit += OnClose;

//            MessageHandler.RegisterMessageHandler(MessageDefine.SCEventPhaseFeedbackResponse, OnScResponse);
//            MessageHandler.RegisterMessageHandler(MessageDefine.SCEventRewardResponse, OnScResponse);
//            GameEvents.PlayerEvents.RequestLatestPlayerInfo.SafeInvoke();

//            m_is_game_end = false;
//        }

//        public override void OnHide()
//        {
//            base.OnHide();

//            GameEvents.UIEvents.UI_Pause_Event.OnQuit -= OnClose;

//            MessageHandler.UnRegisterMessageHandler(MessageDefine.SCEventPhaseFeedbackResponse, OnScResponse);
//            MessageHandler.UnRegisterMessageHandler(MessageDefine.SCEventRewardResponse, OnScResponse);
//        }

//        public override void OnScResponse(object s)
//        {
//            var imsg = s as IMessage;
//            DebugUtil.Log("收到消息 " + imsg.Descriptor.Name);

//            if (s is SCEventPhaseFeedbackResponse)
//            {
//                var rsp = s as SCEventPhaseFeedbackResponse;

//                long finished_phase_id;
//                EventGameManager.Instance.PhaseFinished(out finished_phase_id);

//                if (!MsgStatusCodeUtil.OnError(rsp.ResponseStatus))
//                {

//                    EventGameManager.Instance.Score = rsp.TotalScore;

//                    m_view.PhaseFinish(finished_phase_id, rsp);

//                    Dictionary<UBSParamKeyName, object> _params = new Dictionary<UBSParamKeyName, object>()
//                    {
//                        { UBSParamKeyName.ContentID, finished_phase_id},
//                        { UBSParamKeyName.OfficerID, EventGameManager.Instance.Will_dispatched_officer_id},
//                        { UBSParamKeyName.Success, rsp.Valuation },
//                    };
//                    UserBehaviorStatisticsModules.Instance.LogEvent(UBSEventKeyName.event_Phase, null, _params);
//                }
//                else
//                {
//                    long phase_id;
//                    if (EventGameManager.Instance.FetchCurPhaseID(out phase_id))
//                    {
//                        DebugUtil.LogError(string.Format("事件 {0} 阶段 {1},反馈失败{2}", EventGameManager.Instance.Event_id, phase_id, rsp.Valuation));
//                    }
//                    else
//                    {
//                        DebugUtil.LogError(string.Format("事件 {0} 反馈失败{1}", EventGameManager.Instance.Event_id, rsp.Valuation));
//                    }

//                    Dictionary<UBSParamKeyName, object> _params = new Dictionary<UBSParamKeyName, object>()
//                    {
//                        { UBSParamKeyName.ContentID, finished_phase_id},
//                        { UBSParamKeyName.OfficerID, EventGameManager.Instance.Will_dispatched_officer_id},
//                        { UBSParamKeyName.Success, 0 },
//                        { UBSParamKeyName.Description, rsp.Valuation },
//                    };
//                    UserBehaviorStatisticsModules.Instance.LogEvent(UBSEventKeyName.event_Phase, null, _params);
//                }

//            }
//            else if (s is SCEventRewardResponse)
//            {
//                this.CloseFrame();

//                var rsp = s as SCEventRewardResponse;

//                if (!MsgStatusCodeUtil.OnError(rsp.ResponseStatus))
//                {
//                    WinFailData data = new WinFailData(ENUM_SEARCH_MODE.E_EVENTGAME, rsp);


//                    FrameMgr.OpenUIParams param = new FrameMgr.OpenUIParams(UIDefine.UI_WIN);


//                    param.Param = data;

//                    EngineCoreEvents.UIEvent.ShowUIEventWithParam.SafeInvoke(param);


//                    Dictionary<UBSParamKeyName, object> _params = new Dictionary<UBSParamKeyName, object>()
//                    {
//                        { UBSParamKeyName.ContentID, EventGameManager.Instance.Event_id},
//                        { UBSParamKeyName.Success, rsp.Valuation },
//                    };
//                    UserBehaviorStatisticsModules.Instance.LogEvent(UBSEventKeyName.event_finish, null, _params);

//                    if( 0 == rsp.Valuation)
//                    {
//                        //PushGiftManager.Instance.TurnOn(ENUM_PUSH_GIFT_BLOCK_TYPE.E_EVENT);
//                    }
//                }
//                else
//                {
//                    rsp.Valuation = 0;
//                    rsp.Rewards.Clear();

//                    WinFailData data = new WinFailData(ENUM_SEARCH_MODE.E_EVENTGAME, rsp);
//                    FrameMgr.OpenUIParams param = new FrameMgr.OpenUIParams(UIDefine.UI_WIN);
//                    param.Param = data;

//                    EngineCoreEvents.UIEvent.ShowUIEventWithParam.SafeInvoke(param);


//                    Dictionary<UBSParamKeyName, object> _params = new Dictionary<UBSParamKeyName, object>()
//                    {
//                        { UBSParamKeyName.ContentID, EventGameManager.Instance.Event_id},
//                        { UBSParamKeyName.Success, 0 },
//                        { UBSParamKeyName.Description, rsp.ResponseStatus.Code }

//                    };
//                    UserBehaviorStatisticsModules.Instance.LogEvent(UBSEventKeyName.event_finish, null, _params);

//                    //PushGiftManager.Instance.TurnOn(ENUM_PUSH_GIFT_BLOCK_TYPE.E_EVENT);
//                }

//            }
//        }

//        public override void OnPackageRequest(IMessage imsg, params object[] msg_params)

//        {
//            base.OnPackageRequest(imsg, msg_params);

//            if (imsg is CSEventPhaseFeedbackRequest)
//            {
//                List<long> officer_player_ids = (List<long>)(msg_params[0]);
//                CSEventPhaseFeedbackRequest req = imsg as CSEventPhaseFeedbackRequest;
//                req.EventId = EventGameManager.Instance.Event_id;
//                //long phase_id;
//                //if (EventGameManager.Instance.FetchCurPhaseID(out phase_id))
//                //    req.PhaseId = phase_id;
//                //else
//                //{
//                //    DebugUtil.LogError("发送事件阶段结果， 阶段到了结尾");
//                //    return;
//                //}

//                officer_player_ids.ForEach((item) => req.PlayerOfficerId.Add(item));

//            }
//            else if (imsg is CSEventRewardRequest)
//            {
//                CSEventRewardRequest req = imsg as CSEventRewardRequest;
//                req.EventId = EventGameManager.Instance.Event_id;
//            }

//        }

//        public void DispathPolice(List<long> officer_ids_)
//        {
//            CSEventPhaseFeedbackRequest msg = new CSEventPhaseFeedbackRequest();

//            List<long> officer_player_ids = new List<long>();

//            if (officer_ids_.Count > 0)
//            {
//                officer_player_ids = EventGameUIAssist.GetOfficerPlayerIdsByOfficerIDs(officer_ids_);
//            }

//            this.OnScRequest(msg, officer_player_ids);
//        }

//        private void OnClose()
//        {
//            this.CloseFrame();
//        }

//        public void NextPhaseOnClicked()
//        {
//            if (m_is_game_end)
//                return;


//            if (!EventGameUIAssist.GoToNextPhase(m_view))
//            {
//                m_is_game_end = true;
//                TimeModule.Instance.SetTimeout(() => { m_is_game_end = false; }, 10.0f);
//                //结算
//                OnScRequest(new CSEventRewardRequest());

//            }
//            else
//            {
//                //this.CloseFrame();
//            }

//        }
//    }
//}

