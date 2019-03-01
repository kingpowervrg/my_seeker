using System;
using System.Collections.Generic;

namespace SeekerGame.NewGuid
{
    public class GuidNewFuncMaskEnable : GuidNewFunctionBase
    {
        private bool enable;
        public override void OnInit(long funcID, string[] param)
        {
            base.OnInit(funcID, param);
            this.enable = bool.Parse(param[0]);
        }

        public override void OnExecute()
        {
            base.OnExecute();
            GameEvents.UI_Guid_Event.OnMaskEnableClick.SafeInvoke(enable);
            OnDestory();
        }
    }
}
