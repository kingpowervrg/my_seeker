using EngineCore;
using System;
using System.Collections.Generic;

namespace SeekerGame.NewGuid
{
    public class GuidNewStartCartoon : GuidNewBase
    {
        public override void OnStart()
        {
            base.OnStart();

            if (!GuidNewModule.Instance.IsFrameOpen(UIDefine.UI_GAMESTART_1))
            {
                Dictionary<UBSParamKeyName, object> _params = new Dictionary<UBSParamKeyName, object>()
                {
                            { UBSParamKeyName.ContentID,m_currentID},
                };
                UserBehaviorStatisticsModules.Instance.LogEvent(UBSEventKeyName.Login_carton, null, _params);

                FrameMgr.OpenUIParams param = new FrameMgr.OpenUIParams(UIDefine.UI_GAMESTART_1);
                param.Param = m_currentConf;
                EngineCoreEvents.UIEvent.ShowUIEventWithParam.SafeInvoke(param);
            }
        }

        public override void OnEnd()
        {
            if (m_currentID == 10006)
            {
                EngineCoreEvents.UIEvent.HideFrameThenDestroyEvent.SafeInvoke(UIDefine.UI_GAMESTART_1);

                BigWorldManager.Instance.LoadBigWorld("GiftView");
                EngineCoreEvents.UIEvent.HideUIEvent.SafeInvoke(UIDefine.UI_GUEST_LOGIN);
            }
            base.OnEnd();
        }
    }
}
