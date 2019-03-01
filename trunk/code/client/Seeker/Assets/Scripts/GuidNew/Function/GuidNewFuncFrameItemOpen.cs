using System;
using System.Collections.Generic;

namespace SeekerGame.NewGuid
{
    public class GuidNewFuncFrameItemOpen : GuidNewFunctionBase
    {
        private string m_frameName;
        private string m_itemName;
        public override void OnInit(long funcID, string[] param)
        {
            base.OnInit(funcID, param);
            this.m_frameName = param[0];
            this.m_itemName = param[1];
        }

        public override void OnExecute()
        {
            base.OnExecute();
            GameEvents.UI_Guid_Event.OnOnpenFrame += OnOnpenFrame;
        }

        private void OnOnpenFrame(string frameName,string itemName)
        {
            if (frameName.Equals(this.m_frameName) && itemName.Equals(this.m_itemName))
            {
                OnDestory();
            }
        }

        public override void OnDestory(FuncState funcState = FuncState.Complete)
        {
            base.OnDestory(funcState);
            GameEvents.UI_Guid_Event.OnOnpenFrame -= OnOnpenFrame;
        }

        public override void ClearFunc()
        {
            base.ClearFunc();
            GameEvents.UI_Guid_Event.OnOnpenFrame -= OnOnpenFrame;
        }
    }
}
