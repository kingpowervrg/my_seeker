using System;
using System.Collections.Generic;
using EngineCore;
namespace SeekerGame.NewGuid
{
    public class GuidNewFuncUIEnableClick:GuidNewFunctionBase
    {
        private bool m_enableClick = true;
        public override void OnInit(long funcID, string[] param)
        {
            base.OnInit(funcID, param);
            this.m_enableClick = bool.Parse(param[0]);
        }

        public override void OnExecute()
        {
            base.OnExecute();
            GUIFrame frame = GuidNewModule.Instance.GetFrameByResName(UIDefine.UI_GUID);
            if (frame == null)
            {
                GuidNewModule.Instance.PushFunction(this);
                return;
            }
            GameEvents.UI_Guid_Event.OnUIEnableClick.SafeInvoke(this.m_enableClick);
            OnDestory();
        }

        public override void Tick(float time)
        {
            base.Tick(time);
            GUIFrame frame = GuidNewModule.Instance.GetFrameByResName(UIDefine.UI_GUID);
            if (frame == null)
            {
                return;
            }
            GuidNewModule.Instance.RemoveFunction(this);
            GameEvents.UI_Guid_Event.OnUIEnableClick.SafeInvoke(this.m_enableClick);
            OnDestory();
        }

        public override void ClearFunc()
        {
            base.ClearFunc();
            GuidNewModule.Instance.RemoveFunction(this);
        }
    }
}
