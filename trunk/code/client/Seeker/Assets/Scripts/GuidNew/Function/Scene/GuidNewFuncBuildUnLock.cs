using System;
using System.Collections.Generic;

namespace SeekerGame.NewGuid
{
    public class GuidNewFuncBuildUnLock : GuidNewFunctionBase
    {
        private string m_buildName = string.Empty;
        public override void OnInit(long funcID, string[] param)
        {
            base.OnInit(funcID, param);
            this.m_buildName = param[0];
        }

        public override void OnExecute()
        {
            base.OnExecute();
            GameEvents.BigWorld_Event.OnBuildUnLockComplete += OnBuildUnLockComplete;
            GameEvents.BigWorld_Event.OnUnLock.SafeInvoke(m_buildName);
        }

        public override void OnDestory(FuncState funcState = FuncState.Complete)
        {
            base.OnDestory(funcState);
            GameEvents.BigWorld_Event.OnBuildUnLockComplete -= OnBuildUnLockComplete;
        }

        private void OnBuildUnLockComplete(long buildID)
        {
            OnDestory();
        }
    }
}
