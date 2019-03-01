#if OFFICER_SYS
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SeekerGame.NewGuid
{
    public class GuidNewFuncListenNewPolice : GuidNewFunctionBase
    {
        public override void OnExecute()
        {
            base.OnExecute();
            GameEvents.UI_Guid_Event.OnNewPolice += OnNewPolice;
        }

        public override void OnDestory(FuncState funcState = FuncState.Complete)
        {
            GameEvents.UI_Guid_Event.OnNewPolice -= OnNewPolice;
            base.OnDestory(funcState);
        }

        public override void ClearFunc()
        {
            base.ClearFunc();
            GameEvents.UI_Guid_Event.OnNewPolice -= OnNewPolice;
        }

        public override void ForceFuncDestory()
        {
            base.ForceFuncDestory();
            GameEvents.UI_Guid_Event.OnNewPolice -= OnNewPolice;
        }

        public override void ResetFunc(bool isRetainFunc = true)
        {
            base.ResetFunc(isRetainFunc);
            GameEvents.UI_Guid_Event.OnNewPolice -= OnNewPolice;
        }

        private void OnNewPolice(bool hasNew)
        {
            OnDestory();
        }
    }
}
#endif