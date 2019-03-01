#if OFFICER_SYS
using System;
using System.Collections.Generic;
using EngineCore;

namespace SeekerGame.NewGuid
{
    public class GuidNewFuncSelectPoliceParam : GuidNewFunctionBase
    {
        public override void OnInit(long funcID, string[] param)
        {
            base.OnInit(funcID, param);

        }

        public override void OnExecute()
        {
            base.OnExecute();
            GameEvents.UI_Guid_Event.OnCloseUI += CloseUI;
            GameEvents.UIEvents.UI_Enter_Event.OnLimitPoliceNum.SafeInvoke(1);
        }

        private void CloseUI(GUIFrame frame)
        {
            if (frame.ResName.Equals("UI_start_1.prefab"))
            {
                GameEvents.UIEvents.UI_Enter_Event.OnLimitPoliceNum.SafeInvoke(0);
                OnDestory();
            }
        }


        public override void OnDestory(FuncState funcState = FuncState.Complete)
        {
            base.OnDestory(funcState);
            GameEvents.UI_Guid_Event.OnCloseUI -= CloseUI;
        }

        public override void ClearFunc()
        {
            base.ClearFunc();
            GameEvents.UI_Guid_Event.OnCloseUI -= CloseUI;
        }
    }
}
#endif