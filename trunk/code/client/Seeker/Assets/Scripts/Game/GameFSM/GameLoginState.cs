
using EngineCore;
using System;
using System.Collections.Generic;
namespace SeekerGame
{
    class GameLoginState : SimpleFSMStateBase
    {
        public GameLoginState()
        {
        }

        public override void BeginState(int stateFlag)
        {
            
            if (null == Params)
            {
                //GameEvents.UIEvents.UI_Loading_Event.OnStartLoadingComplete.SafeInvoke();
#if !UNITY_DEBUG
                //EngineCoreEvents.UIEvent.ShowUIEvent.SafeInvoke(UIDefine.UI_NOTIC);
#endif
                EngineCoreEvents.UIEvent.ShowUIEvent.SafeInvoke(UIDefine.UI_NOTIC);
                EngineCoreEvents.UIEvent.ShowUIEvent.SafeInvoke(UIDefine.UI_GUEST_LOGIN);
                BigWorldManager.Instance.ClearBigWorld();

#if UNITY_DEBUG
                EngineCoreEvents.UIEvent.ShowUIEvent.SafeInvoke(UIDefine.UI_GM);
#endif

                PushGiftManager.Instance.Sync();
            }
            else if ((ENUM_ACCOUNT_TYPE)Params == ENUM_ACCOUNT_TYPE.E_GUID)
            {
                EngineCoreEvents.UIEvent.ShowUIEvent.SafeInvoke(UIDefine.UI_GUIDLOGIN);
            }
            else
            {
                EngineCoreEvents.UIEvent.HideFrameThenDestroyEvent.SafeInvoke(UIDefine.UI_COMICS_GUID);
                FrameMgr.Instance.HideAllFrames(new List<string>() { UIDefine.UI_GM, UIDefine.UI_FB_Loading, UIDefine.UI_GUID });

                //FrameMgr.Instance.DestroyAllHiddenFrames
                FrameMgr.OpenUIParams ui_param = new FrameMgr.OpenUIParams(UIDefine.UI_GUEST_LOGIN);
                ui_param.Param = Params;

                EngineCoreEvents.UIEvent.ShowUIEventWithParam.SafeInvoke(ui_param);
            }
        }

        public override void EndState(int nextState)
        {
            //EngineCoreEvents.UIEvent.HideUIEvent.SafeInvoke(UIDefine.UI_GUEST_LOGIN);

            //背包
            if (GlobalInfo.MY_PLAYER_INFO != null)
            {
                GlobalInfo.MY_PLAYER_INFO.SyncPlayerBag();
                //公告
                CSNoticeListRequest res = new CSNoticeListRequest();
                res.Type = 2;
                GameEvents.NetWorkEvents.SendAsyncMsg.SafeInvoke(res);
            }

            
        }


    }
}
