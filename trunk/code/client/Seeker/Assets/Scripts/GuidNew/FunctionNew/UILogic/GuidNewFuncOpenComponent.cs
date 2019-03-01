using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SeekerGame.NewGuid
{
    public class GuidNewFuncOpenComponent : GuidNewFunctionBase
    {
        private string m_componentName;
        public override void OnInit(long funcID, string[] param)
        {
            base.OnInit(funcID, param);
            this.m_componentName = param[0];
        }

        public override void OnExecute()
        {
            base.OnExecute();
            GameEvents.UI_Guid_Event.OnpenComponentByName += OnpenComponentByName;
        }

        public override void OnDestory(FuncState funcState = FuncState.Complete)
        {
            base.OnDestory(funcState);
            GameEvents.UI_Guid_Event.OnpenComponentByName -= OnpenComponentByName;
        }

        public override void ClearFunc()
        {
            base.ClearFunc();
            GameEvents.UI_Guid_Event.OnpenComponentByName -= OnpenComponentByName;
        }

        public override void ForceFuncDestory()
        {
            base.ForceFuncDestory();
            GameEvents.UI_Guid_Event.OnpenComponentByName -= OnpenComponentByName;
        }

        private void OnpenComponentByName(string componentName)
        {
            if (m_componentName.Equals(componentName))
            {
                OnDestory();
            }
        }

    }
}
