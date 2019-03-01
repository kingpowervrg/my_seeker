using EngineCore;
using System.Collections.Generic;
namespace SeekerGame
{
    public class StartCartoonManager : Singleton<StartCartoonManager>
    {
        private long contextId = -1;
        public long ContextId
        {
            get { return contextId; }
            set { contextId = value; }
        }
        public StartCartoonManager()
        {
            MessageHandler.RegisterMessageHandler(MessageDefine.SCCartoonEnterResponse, OnResponse);
        }

        public void OpenStartCartoonForID(long id)
        {
            ContextId = id;
            ConfCartoonScene confScene = ConfCartoonScene.Get(ContextId);
            if (confScene == null && confScene.sceneInfoIds.Length == 0)
            {
                return;
            }
            CSCartoonEnterRequest enterReq = new CSCartoonEnterRequest();
            enterReq.SceneId = ContextId;
            GameEvents.NetWorkEvents.SendMsg.SafeInvoke(enterReq);
        }

        private void OnResponse(object obj)
        {
            if (obj is SCCartoonEnterResponse)
            {
                SCCartoonEnterResponse res = obj as SCCartoonEnterResponse;
                if (!MsgStatusCodeUtil.OnError(res.Result))
                {
                    Dictionary<UBSParamKeyName, object> enterSceneFromChapterMapDict = new Dictionary<UBSParamKeyName, object>();
                    enterSceneFromChapterMapDict.Add(UBSParamKeyName.EnterSceneFromChapter, this.ContextId);
                    UserBehaviorStatisticsModules.Instance.LogEvent(UBSEventKeyName.carton_star, null, enterSceneFromChapterMapDict);

                    FrameMgr.OpenUIParams param = new FrameMgr.OpenUIParams(UIDefine.UI_COMICS_1);
                    param.Param = ConfCartoonScene.Get(ContextId).sceneInfoIds[0];
                    EngineCoreEvents.UIEvent.ShowUIEventWithParam.SafeInvoke(param);
                }
            }
        }



        public override void Destroy()
        {
            base.Destroy();
            MessageHandler.UnRegisterMessageHandler(MessageDefine.SCCartoonEnterResponse, OnResponse);
        }
    }
}
