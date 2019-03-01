using EngineCore;

namespace SeekerGame.NewGuid
{
    /// <summary>
    /// 离开场景
    /// </summary>
    public class GuidNewFuncLeaveScene : GuidNewFunctionBase
    {
        public override void OnInit(long funcID, string[] param)
        {
            base.OnInit(funcID, param);
        }

        public override void OnExecute()
        {
            base.OnExecute();
            GameEvents.SceneEvents.OnLeaveSceneComplete += OnLeaveScene;
        }

        public override void OnDestory(FuncState funcState = FuncState.Complete)
        {
            base.OnDestory(funcState);
            GameEvents.SceneEvents.OnLeaveSceneComplete -= OnLeaveScene;
        }

        public override void ClearFunc()
        {
            base.ClearFunc();
            GameEvents.SceneEvents.OnLeaveSceneComplete -= OnLeaveScene;
        }

        private void OnLeaveScene()
        {
            OnDestory();
        }
    }
}
