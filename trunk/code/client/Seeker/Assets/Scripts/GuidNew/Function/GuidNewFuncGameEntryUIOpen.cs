using System;
using System.Collections.Generic;

namespace SeekerGame.NewGuid
{
    public class GuidNewFuncGameEntryUIOpen : GuidNewFunctionBase
    {
        private string m_panelName = null;
        public override void OnInit(long funcID, string[] param)
        {
            base.OnInit(funcID, param);
            this.m_panelName = param[0];
        }

        public override void OnExecute()
        {
            base.OnExecute();
            string currentPanelName = GameEvents.UIEvents.UI_GameEntry_Event.GetCurrentGameEntryUI();
            if (currentPanelName.Equals(m_panelName))
            {
                OnDestory();
                return;
            }
            GameEvents.UIEvents.UI_GameEntry_Event.OnGameEntryOpen += OnGameEntryOpen;
        }

        private void OnGameEntryOpen(string paneName)
        {
            if (paneName.Equals(m_panelName))
            {
                OnDestory();
            }
        }

        public override void OnDestory(FuncState funcState = FuncState.Complete)
        {
            base.OnDestory(funcState);
            GameEvents.UIEvents.UI_GameEntry_Event.OnGameEntryOpen -= OnGameEntryOpen;
        }

        public override void ClearFunc()
        {
            base.ClearFunc();
            GameEvents.UIEvents.UI_GameEntry_Event.OnGameEntryOpen -= OnGameEntryOpen;
        }
    }
}
