//using EngineCore;
//using GOEngine;
//using Google.Protobuf;
//using System.Collections.Generic;

//namespace SeekerGame
//{
//    //[UILogicHandler(UIDefine.UI_EVENT_INGAME_ENTRY)]
//    public class EventGameEntryUILogic : BaseGameViewLogic<EventGameEntryUILogic>
//    {

//        private enum ENUM_GAME_TYPE
//        {
//            E_EVENT,
//            E_CARTOON,
//        }

//        ENUM_GAME_TYPE m_type;
//        private long m_id;

//        protected override void OnInit()
//        {
//            base.OnInit();

//            SetView<EventGameEntryUIView>(new EventGameEntryUIView());

//            ACT_PRELOAD_VIEW.SafeInvoke(this);

//        }

//        public override void OnShow(object param)
//        {
//            base.OnShow(param);

//            GameEvents.UI_Guid_Event.OnSeekOpenClose.SafeInvoke(true);
//            if (param is EventGameEntryData)
//            {
//                m_type = ENUM_GAME_TYPE.E_EVENT;
//            }
//            else if (param is CartoonGameEntryData)
//            {
//                m_type = ENUM_GAME_TYPE.E_CARTOON;
//                m_id = ((CartoonGameEntryData)param).M_cartoon_id;
//            }

//            ACT_SHOW_VIEW.SafeInvoke(param);

//            MessageHandler.RegisterMessageHandler(MessageDefine.SCEventEnterResponse, OnScResponse);
//            MessageHandler.RegisterMessageHandler(MessageDefine.SCCartoonEnterResponse, OnScResponse);



//        }

//        public override void OnHide()
//        {
//            base.OnHide();
//            ACT_HIDE_VIEW.SafeInvoke();

//            MessageHandler.UnRegisterMessageHandler(MessageDefine.SCEventEnterResponse, OnScResponse);
//            MessageHandler.UnRegisterMessageHandler(MessageDefine.SCCartoonEnterResponse, OnScResponse);

//        }

//        private void OnScResponse(object s)
//        {
//            var imsg = s as IMessage;
//            DebugUtil.Log("收到消息 " + imsg.Descriptor.Name);

//            if (s is SCEventEnterResponse)
//            {
//                var rsp = s as SCEventEnterResponse;

//                if (!MsgStatusCodeUtil.OnError(rsp.Result))
//                {

//                    {
//                        Dictionary<UBSParamKeyName, object> _params = new Dictionary<UBSParamKeyName, object>()
//                    {
//                        { UBSParamKeyName.Success, 1},
//                        {UBSParamKeyName.ContentID, EventGameManager.Instance.Event_id }
//                    };
//                        UserBehaviorStatisticsModules.Instance.LogEvent(UBSEventKeyName.event_start, null, _params);
//                    }

//                    DebugUtil.Log("进入事件玩法，成功");
//                    EventGameManager.Instance.Init();

//                    EventGameUIAssist.GoToNextPhase();

//                    this.CloseFrame();
//                }
//                else
//                {
//                    if (MsgStatusCodeUtil.VIT_OUT == rsp.Result)
//                    {
//                        Dictionary<UBSParamKeyName, object> _params = new Dictionary<UBSParamKeyName, object>()
//                    {
//                        { UBSParamKeyName.Success, 0},
//                        { UBSParamKeyName.Description, UBSDescription.NOT_ENOUGH_VIT},
//                        {UBSParamKeyName.ContentID, EventGameManager.Instance.Event_id }
//                    };
//                        UserBehaviorStatisticsModules.Instance.LogEvent(UBSEventKeyName.event_start, null, _params);
//                    }
//                    else
//                    {
//                        DebugUtil.LogError("进入事件玩法，失败");
//                        Dictionary<UBSParamKeyName, object> _params = new Dictionary<UBSParamKeyName, object>()
//                    {
//                        { UBSParamKeyName.Success, 0},
//                        { UBSParamKeyName.Description, rsp.Result},
//                            {UBSParamKeyName.ContentID, EventGameManager.Instance.Event_id }
//                    };
//                        UserBehaviorStatisticsModules.Instance.LogEvent(UBSEventKeyName.event_start, null, _params);
//                    }
//                }

//                //if (0 == rsp.Result)
//                //{
//                //    DebugUtil.LogError("进入事件玩法，失败");
//                //    Dictionary<UBSParamKeyName, object> _params = new Dictionary<UBSParamKeyName, object>()
//                //    {
//                //        { UBSParamKeyName.Success, 0},
//                //    };
//                //    UserBehaviorStatisticsModules.Instance.LogEvent(UBSEventKeyName.event_start, null, _params);
//                //}
//                //else if (2 == rsp.Result)
//                //{
//                //    Dictionary<UBSParamKeyName, object> _params = new Dictionary<UBSParamKeyName, object>()
//                //    {
//                //        { UBSParamKeyName.Success, 0},
//                //        { UBSParamKeyName.Description, UBSDescription.NOT_ENOUGH_VIT},
//                //    };
//                //    UserBehaviorStatisticsModules.Instance.LogEvent(UBSEventKeyName.event_start, null, _params);

//                //    PopUpManager.OpenPopUp(new PopUpData()
//                //    {
//                //        isOneBtn = true,
//                //        content = "UI_ENTER_GAME_NO_ENERGY",
//                //    });
//                //    return;
//                //}
//                //else if (1 == rsp.Result)
//                //{
//                //    Dictionary<UBSParamKeyName, object> _params = new Dictionary<UBSParamKeyName, object>()
//                //    {
//                //        { UBSParamKeyName.Success, 1},
//                //    };
//                //    UserBehaviorStatisticsModules.Instance.LogEvent(UBSEventKeyName.event_start, null, _params);

//                //    DebugUtil.Log("进入事件玩法，成功");
//                //    EventGameManager.Instance.Init();

//                //    EventGameUIAssist.GoToNextPhase();

//                //    this.CloseFrame();
//                //}
//                //else
//                //{
//                //    DebugUtil.LogError("进入事件玩法，未知错误");
//                //}

//            }
//            else if (s is SCCartoonEnterResponse)
//            {
//                var rsp = s as SCCartoonEnterResponse;

//                if (!MsgStatusCodeUtil.OnError(rsp.Result))
//                {
//                    DebugUtil.Log("进入事件玩法，成功");


//                    FrameMgr.OpenUIParams open_data = new FrameMgr.OpenUIParams(UIDefine.UI_CARTOON);
//                    open_data.Param = m_id;

//                    EngineCoreEvents.UIEvent.ShowUIEventWithParam.SafeInvoke(open_data);

//                    this.CloseFrame();
//                }
//                else
//                {
//                    DebugUtil.LogError("进入玩法，失败");
//                }

//                //    if (0 == rsp.Result)
//                //{
//                //    DebugUtil.LogError("进入玩法，失败");
//                //}
//                //else if (1 == rsp.Result)
//                //{
//                //    DebugUtil.Log("进入玩法，成功");


//                //    FrameMgr.OpenUIParams open_data = new FrameMgr.OpenUIParams(UIDefine.UI_CARTOON);
//                //    open_data.Param = m_id;

//                //    EngineCoreEvents.UIEvent.ShowUIEventWithParam.SafeInvoke(open_data);

//                //    this.CloseFrame();
//                //}
//                //else
//                //{
//                //    DebugUtil.LogError("进入漫画玩法，未知错误");
//                //}
//            }

//        }

//        void OnScRequest(IMessage imsg, params object[] msg_params)
//        {
//            DebugUtil.Log("发送消息 " + imsg.Descriptor.Name);

//            if (imsg is CSEventEnterRequest)
//            {
//                CSEventEnterRequest req = imsg as CSEventEnterRequest;
//                req.EventId = EventGameManager.Instance.Event_id;


//            }
//            else if (imsg is CSCartoonEnterRequest)
//            {
//                CSCartoonEnterRequest req = imsg as CSCartoonEnterRequest;
//                req.SceneId = m_id;
//            }

//            GameEvents.NetWorkEvents.SendMsg.SafeInvoke(imsg);
//        }


//        public void OnStartGameClicked()
//        {
//            if (ENUM_GAME_TYPE.E_EVENT == m_type)
//            {
//                if (GlobalInfo.MY_PLAYER_INFO.Officer_infos.Count >= EventGameUIAssist.GetPhaseIDs(EventGameManager.Instance.Event_id).Count)
//                {
//                    //if (ConfEvent.Get(EventGameManager.Instance.Event_id).vitConsume > GlobalInfo.MY_PLAYER_INFO.Vit)
//                    //{
//                    //    Dictionary<UBSParamKeyName, object> _params = new Dictionary<UBSParamKeyName, object>()
//                    //    {
//                    //        { UBSParamKeyName.Success, 0},
//                    //        { UBSParamKeyName.Description, UBSDescription.NOT_ENOUGH_VIT},
//                    //    };
//                    //    UserBehaviorStatisticsModules.Instance.LogEvent(UBSEventKeyName.event_begin, null, _params);

//                    //    //PopUpManager.OpenPopUp(new PopUpData()
//                    //    //{
//                    //    //    isOneBtn = true,
//                    //    //    content = "UI_ENTER_GAME_NO_ENERGY",
//                    //    //});

//                    //    PopUpManager.OpenGoToVitShop();
//                    //}
//                    //else
//                    //{
//                    this.OnScRequest(new CSEventEnterRequest());
//                    //}
//                }
//                else
//                {
//                    Dictionary<UBSParamKeyName, object> _params = new Dictionary<UBSParamKeyName, object>()
//                        {
//                            { UBSParamKeyName.Success, 0},
//                            { UBSParamKeyName.Description, UBSDescription.NOT_ENOUGH_OFFICER},
//                            {UBSParamKeyName.ContentID, EventGameManager.Instance.Event_id }
//                        };
//                    UserBehaviorStatisticsModules.Instance.LogEvent(UBSEventKeyName.event_start, null, _params);

//                    PopUpManager.OpenPopUp(new PopUpData()
//                    {
//                        isOneBtn = true,
//                        content = "event_nopolice",
//                    });
//                }

//            }
//            else if (ENUM_GAME_TYPE.E_CARTOON == m_type)
//            {
//                this.OnScRequest(new CSCartoonEnterRequest());
//            }
//        }

//    }
//}


