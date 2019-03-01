using EngineCore;

namespace SeekerGame.NewGuid
{
    public class GuidNewFuncSceneUnLockItemWord : GuidNewFunctionBase
    {
        private long m_entityID;
        private bool m_lock;
        public override void OnInit(long funcID, string[] param)
        {
            base.OnInit(funcID, param);
            this.m_entityID = long.Parse(param[0]);
            this.m_lock = bool.Parse(param[1]);
        }

        public override void OnExecute()
        {
            base.OnExecute();
            GameEvents.UIEvents.UI_GameMain_Event.LockFindItem.SafeInvoke(this.m_entityID, this.m_lock);
            OnDestory();
        }
    }
}
