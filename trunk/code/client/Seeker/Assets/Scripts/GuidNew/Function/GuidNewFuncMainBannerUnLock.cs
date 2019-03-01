using System;
using System.Collections.Generic;

namespace SeekerGame.NewGuid
{
    public class GuidNewFuncMainBannerUnLock : GuidNewFunctionBase
    {
        private int iconIndex = 0;
        public override void OnInit(long funcID, string[] param)
        {
            base.OnInit(funcID, param);
            this.iconIndex = int.Parse(param[0]);
        }

        public override void OnExecute()
        {
            base.OnExecute();
            GameEvents.UI_Guid_Event.OnMainIconUnLockComplete += OnMainIconUnLockComplete;
            GameEvents.UI_Guid_Event.OnMainIconUnLock.SafeInvoke(iconIndex);
        }

        private void OnMainIconUnLockComplete(int index)
        {
            if (index == iconIndex)
            {
                UnityEngine.Debug.Log("unlock complete ===== " + index);
                OnDestory();
            }
        }

        public override void OnDestory(FuncState funcState = FuncState.Complete)
        {
            GameEvents.UI_Guid_Event.OnMainIconUnLockComplete -= OnMainIconUnLockComplete;
            base.OnDestory(funcState);
        }

        public override void ClearFunc()
        {
            base.ClearFunc();
            GameEvents.UI_Guid_Event.OnMainIconUnLockComplete -= OnMainIconUnLockComplete;
        }

    }
}
