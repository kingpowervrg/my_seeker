using EngineCore;

namespace SeekerGame.NewGuid
{
    public class GuidNewFuncUnLockMainIcon : GuidNewFunctionBase
    {
        private int index;
        private bool isLock;
        public override void OnInit(long funcID, string[] param)
        {
            base.OnInit(funcID, param);
            this.index = int.Parse(param[0]);
            this.isLock = bool.Parse(param[1]);
        }

        public override void OnExecute()
        {
            base.OnExecute();
            GameEvents.UIEvents.UI_GameEntry_Event.OnLockMainIconComplete += OnLockMainIconComplete;
            GameEvents.UIEvents.UI_GameEntry_Event.OnLockMainIcon.SafeInvoke(this.index, this.isLock);
            //TimeModule.Instance.SetTimeout(TimeDelay,1f);
        }

        private void OnLockMainIconComplete(int index)
        {
            if (this.index == index)
            {
                GameEvents.UIEvents.UI_GameEntry_Event.OnLockMainIconComplete -= OnLockMainIconComplete;
                OnDestory();
            }
            
        }


        public override void ForceFuncDestory()
        {
            base.ForceFuncDestory();
            GameEvents.UIEvents.UI_GameEntry_Event.OnLockMainIconComplete -= OnLockMainIconComplete;
            //TimeModule.Instance.RemoveTimeaction(TimeDelay);
        }

        public override void ClearFunc()
        {
            base.ClearFunc();
            GameEvents.UIEvents.UI_GameEntry_Event.OnLockMainIconComplete -= OnLockMainIconComplete;
        }

        public override void ResetFunc(bool isRetainFunc = true)
        {
            base.ResetFunc(isRetainFunc);
            GameEvents.UIEvents.UI_GameEntry_Event.OnLockMainIconComplete -= OnLockMainIconComplete;
        }

    }
}
