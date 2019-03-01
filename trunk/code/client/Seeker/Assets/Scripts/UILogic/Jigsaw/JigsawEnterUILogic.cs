using EngineCore;
using Google.Protobuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SeekerGame
{
    [UILogicHandler(UIDefine.UI_ENTER_JIGSAW)]
    public class JigsawEnterUILogic : BaseViewComponetLogic
    {
        JigsawEnterUI m_view;

        private long m_scene_id;
        private long m_task_id;
        private bool m_is_started;
        public override void OnPackageRequest(IMessage imsg, params object[] msg_params)
        {
            base.OnPackageRequest(imsg, msg_params);
        }

        public override void OnScResponse(object s)
        {
            base.OnScResponse(s);

            if (s is SCEnterResponse)
            {
                var rsp = s as SCEnterResponse;

                m_is_started = false;

                if (!MsgStatusCodeUtil.OnError(rsp.Result))
                {
                    this.RefreshCostItemNum();

                    Dictionary<UBSParamKeyName, object> _params = new Dictionary<UBSParamKeyName, object>()
                    {
                        { UBSParamKeyName.Success, 1},
                        { UBSParamKeyName.SceneID, this.m_scene_id},
                        { UBSParamKeyName.ContentID, ConfJigsawScene.Get(this.m_scene_id).sceneInfoId},
                    };
                    UserBehaviorStatisticsModules.Instance.LogEvent(UBSEventKeyName.Pintu_begin, null, _params);

                    FrameMgr.OpenUIParams ui_param = new FrameMgr.OpenUIParams(UIDefine.UI_JIGSAW);

                    ui_param.Param = new List<long> { this.m_scene_id, ConfJigsawScene.Get(this.m_scene_id).sceneInfoId };

                    EngineCoreEvents.UIEvent.ShowUIEventWithParam.SafeInvoke(ui_param);

                    this.CloseFrame();
                    EngineCoreEvents.UIEvent.HideUIEvent.SafeInvoke(UIDefine.UI_CHAPTER);
                }
                else
                {
                    //#if UNITY_DEBUG
                    //                    FrameMgr.OpenUIParams ui_param = new FrameMgr.OpenUIParams(UIDefine.UI_JIGSAW);

                    //                    ui_param.Param = new List<long> { this.m_scene_id, ConfJigsawScene.Get(this.m_scene_id).sceneInfoId };

                    //                    EngineCoreEvents.UIEvent.ShowUIEventWithParam.SafeInvoke(ui_param);

                    //                    this.CloseFrame();
                    //                    EngineCoreEvents.UIEvent.HideUIEvent.SafeInvoke(UIDefine.UI_CHAPTER);
                    //#else

                    Dictionary<UBSParamKeyName, object> _params = new Dictionary<UBSParamKeyName, object>()
                    {
                        { UBSParamKeyName.Success, 0},
                        { UBSParamKeyName.Description, rsp.Result},
                        { UBSParamKeyName.SceneID, this.m_scene_id},
                        { UBSParamKeyName.ContentID, ConfJigsawScene.Get(this.m_scene_id).sceneInfoId},
                    };
                    UserBehaviorStatisticsModules.Instance.LogEvent(UBSEventKeyName.Pintu_begin, null, _params);
                    //#endif
                }
            }
        }

        public override void OnShow(object param)
        {
            base.OnShow(param);

            GameEvents.UIEvents.UI_GameEntry_Event.OnMaskBGVisible.SafeInvoke(true);

            MessageHandler.RegisterMessageHandler(MessageDefine.SCEnterResponse, OnScResponse);



            this.m_task_id = -1;

            if (null != param)
            {
                List<long> my_param = param as List<long>;
                this.m_scene_id = my_param[0];
                this.m_task_id = my_param[1];
                m_view.Refresh(this.m_scene_id,this.m_task_id);

                Dictionary<UBSParamKeyName, object> _params = new Dictionary<UBSParamKeyName, object>()
                    {
                        { UBSParamKeyName.ContentID, ConfJigsawScene.Get(this.m_scene_id).sceneInfoId},
                    };
                UserBehaviorStatisticsModules.Instance.LogEvent(UBSEventKeyName.Pintu_start, null, _params);
            }

            m_is_started = false;

        }

        public override void OnHide()
        {
            base.OnHide();
            GameEvents.UIEvents.UI_GameEntry_Event.OnMaskBGVisible.SafeInvoke(false);
            MessageHandler.UnRegisterMessageHandler(MessageDefine.SCEnterResponse, OnScResponse);

            m_is_started = false;
        }

        protected override void OnInit()
        {
            base.OnInit();

            m_view = this.Make<JigsawEnterUI>(root.name);
        }
        public override FrameDisplayMode UIFrameDisplayMode
        {
            get
            {
                return FrameDisplayMode.FULLSCREEN;
            }
        }
        public void StartGame()
        {
            if (m_is_started)
                return;

            m_is_started = true;

            CSEnterRequest req = new CSEnterRequest();
            req.SceneId = this.m_scene_id;

            GameEvents.NetWorkEvents.SendMsg.SafeInvoke(req);

            //System.Action<UILogicBase> OnClose = (popui) =>
            //{
            //    CSEnterRequest req = new CSEnterRequest();
            //    req.SceneId = this.m_scene_id;

            //    GameEvents.NetWorkEvents.SendMsg.SafeInvoke(req);

            //};

            //EngineCoreEvents.UIEvent.ShowUIEventWithParam.SafeInvoke(new FrameMgr.OpenUIParams(UIDefine.UI_JIGSAW_STUDY) { Param = OnClose/*OnCloseCallback = OnClose*/ });
        }

        private void RefreshCostItemNum()
        {
            ConfJigsawScene data = ConfJigsawScene.Get(m_scene_id);

            if (data.costPropIds.Length > 0)
            {
                long prop_id = data.costPropIds[0];
                int item_in_bag_num = null != GlobalInfo.MY_PLAYER_INFO.GetBagInfosByID(prop_id) ? GlobalInfo.MY_PLAYER_INFO.GetBagInfosByID(prop_id).Count : 0;

                if (item_in_bag_num >= 1)
                {
                    GlobalInfo.MY_PLAYER_INFO.ReducePropForBag(prop_id);
                }
            }
        }
    }
}
