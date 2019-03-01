using System;
using System.Collections.Generic;
using EngineCore;
namespace SeekerGame.NewGuid
{
    public class GuidNewFuncFrameCloseListener : GuidNewFunctionBase
    {
        private string frameName;
        public override void OnInit(long funcID, string[] param)
        {
            base.OnInit(funcID, param);
            this.frameName = param[0];
        }

        public override void OnExecute()
        {
            base.OnExecute();
            GameEvents.UI_Guid_Event.OnCloseUI += OnCloseUI;
        }

        private void OnCloseUI(GUIFrame frame)
        {
            if (frame.ResName.Equals(this.frameName))
            {
                OnDestory();
            }
        }

        public override void OnDestory(FuncState funcState = FuncState.Complete)
        {
            base.OnDestory(funcState);
            GameEvents.UI_Guid_Event.OnCloseUI -= OnCloseUI;
        }

        public override void ClearFunc()
        {
            base.ClearFunc();
            GameEvents.UI_Guid_Event.OnCloseUI -= OnCloseUI;
        }
    }
}
