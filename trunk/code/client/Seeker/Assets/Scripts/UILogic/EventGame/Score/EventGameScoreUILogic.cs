
//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using EngineCore;
//using GOEngine;
//using GOGUI;
//using Google.Protobuf;

//namespace SeekerGame
//{
//    //[UILogicHandler(UIDefine.UI_EVENT_INGAME_SCORE)]
//    public class EventGameScoreUILogic : BaseGameViewLogic<EventGameScoreUILogic>
//    {
//        private bool m_is_game_end;

//        protected override void OnInit()
//        {
//            base.OnInit();

//            SetView<EventGameScoreUIView>(new EventGameScoreUIView());

//            ACT_PRELOAD_VIEW.SafeInvoke(this);

//        }

//        public override void OnShow(object param)
//        {
//            base.OnShow(param);

//            ACT_SHOW_VIEW.SafeInvoke(param);

//            GameEvents.UIEvents.UI_Pause_Event.OnQuit += OnClose;

//            MessageHandler.RegisterMessageHandler(MessageDefine.SCEventRewardResponse, OnScResponse);


//            m_is_game_end = false;
//        }

//        public override void OnHide()
//        {
//            base.OnHide();
//            ACT_HIDE_VIEW.SafeInvoke();

//            GameEvents.UIEvents.UI_Pause_Event.OnQuit -= OnClose;

//            MessageHandler.UnRegisterMessageHandler(MessageDefine.SCEventRewardResponse, OnScResponse);

//        }

//        private void OnScResponse(object s)
//        {
//            var imsg = s as IMessage;
//            DebugUtil.Log("收到消息 " + imsg.Descriptor.Name);

//            if (s is SCEventRewardResponse)
//            {
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

//                    //if (0 == rsp.Valuation)
//                        //PushGiftManager.Instance.TurnOn(ENUM_PUSH_GIFT_BLOCK_TYPE.E_EVENT);
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


//        void OnScRequest(IMessage imsg, params object[] msg_params)
//        {
//            DebugUtil.Log("发送消息 " + imsg.Descriptor.Name);


//            if (imsg is CSEventRewardRequest)
//            {
//                CSEventRewardRequest req = imsg as CSEventRewardRequest;
//                req.EventId = EventGameManager.Instance.Event_id;
//            }

//            GameEvents.NetWorkEvents.SendMsg.SafeInvoke(imsg);
//        }

//        public void NextPhaseOnClicked()
//        {
//            if (m_is_game_end)
//                return;

//            this.CloseFrame();

//            if (!EventGameUIAssist.GoToNextPhase())
//            {
//                m_is_game_end = true;
//                TimeModule.Instance.SetTimeout(() => { m_is_game_end = false; }, 10.0f);
//                //结算
//                OnScRequest(new CSEventRewardRequest());

//            }
//            else
//            {
//                this.CloseFrame();
//            }
//        }

//        private void OnClose()
//        {
//            this.CloseFrame();
//        }

//    }
//}

