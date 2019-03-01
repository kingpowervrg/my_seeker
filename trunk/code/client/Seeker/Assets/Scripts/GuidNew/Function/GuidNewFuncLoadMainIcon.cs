using System;
using System.Collections.Generic;
using SeekerGame.NewGuid;

namespace SeekerGame.NewGuid
{
    public class GuidNewFuncLoadMainIcon : GuidNewFunctionBase
    {
        public override void OnInit(long funcID, string[] param)
        {
            base.OnInit(funcID, param);
        }

        public override void OnExecute()
        {
            base.OnExecute();
            GameEvents.UI_Guid_Event.OnMainIcon.SafeInvoke();
            OnDestory();
        }

    }
}
