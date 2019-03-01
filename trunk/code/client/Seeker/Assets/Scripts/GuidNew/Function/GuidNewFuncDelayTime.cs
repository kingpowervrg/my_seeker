using System;
using System.Collections.Generic;
using EngineCore;

namespace SeekerGame.NewGuid
{
    public class GuidNewFuncDelayTime : GuidNewFunctionBase
    {
        private float delayTime = 0f;
        public override void OnInit(long funcID, string[] param)
        {
            base.OnInit(funcID, param);
            this.delayTime = float.Parse(param[0]);
        }

        public override void OnExecute()
        {
            base.OnExecute();
            if (delayTime <= 0)
            {
                OnDestory();
                return;
            }
            TimeModule.Instance.SetTimeout(Timeout, delayTime);
        }

        private void Timeout()
        {
            OnDestory();
        }

        public override void ClearFunc()
        {
            base.ClearFunc();
            TimeModule.Instance.RemoveTimeaction(Timeout);
        }

        public override void ForceFuncDestory()
        {
            base.ForceFuncDestory();
            TimeModule.Instance.RemoveTimeaction(Timeout);
        }

        public override void ResetFunc(bool isRetainFunc = true)
        {
            base.ResetFunc(isRetainFunc);
            TimeModule.Instance.RemoveTimeaction(Timeout);
        }

    }
}
