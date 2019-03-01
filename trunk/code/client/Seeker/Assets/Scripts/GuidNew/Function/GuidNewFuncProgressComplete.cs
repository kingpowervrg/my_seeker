using System;
using System.Collections.Generic;

namespace SeekerGame.NewGuid
{
    public class GuidNewFuncProgressComplete : GuidNewFunctionBase
    {
        private long[] progressIndex;
        public override void OnInit(long funcID, string[] param)
        {
            base.OnInit(funcID, param);
            progressIndex = new long[param.Length];
            for (int i = 0; i < param.Length; i++)
            {
                progressIndex[i] = long.Parse(param[i]);
            }
        }

        public override void OnExecute()
        {
            base.OnExecute();
            GameEvents.UI_Guid_Event.OnGuidNewComplete += OnGuidNewComplete;
            bool status = CheckGuidComplete();
            if (status)
            {
                OnDestory();
            }
            //GuidNewModule.Instance.PushFunction(this);
        }

        private void OnGuidNewComplete(long completeID)
        {
            bool status = CheckGuidComplete();
            if (status)
            {
                OnDestory();
            }
        }

        private bool CheckGuidComplete()
        {
            for (int i = 0; i < progressIndex.Length; i++)
            {
                bool status = GuidNewManager.Instance.GetGuidNewStatus(progressIndex[i]);
                if (!status)
                {
                    return false;
                }
            }
            return true;
        }

        public override void OnDestory(FuncState funcState = FuncState.Complete)
        {
            GameEvents.UI_Guid_Event.OnGuidNewComplete -= OnGuidNewComplete;
            base.OnDestory(funcState);
            
        }

        public override void ClearFunc()
        {
            base.ClearFunc();
            GameEvents.UI_Guid_Event.OnGuidNewComplete -= OnGuidNewComplete;
        }
    }
}
