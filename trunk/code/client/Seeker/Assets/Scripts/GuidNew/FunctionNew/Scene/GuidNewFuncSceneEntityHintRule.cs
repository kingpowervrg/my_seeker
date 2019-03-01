
namespace SeekerGame.NewGuid
{
    public class GuidNewFuncSceneEntityHintRule : GuidNewFunctionBase
    {
        private float m_sustainTime; //间隔时间
        public override void OnInit(long funcID, string[] param)
        {
            base.OnInit(funcID, param);
            this.m_sustainTime = float.Parse(param[0]);
        }

        private GameSceneBase m_sceneBase = null;
        public override void OnExecute()
        {
            base.OnExecute();
            m_sceneBase = (GameSceneBase)SceneModule.Instance.CurrentScene;
            GameEvents.MainGameEvents.OnGameOver += OnGameOver;
            GuidNewModule.Instance.PushFunction(this);
            GameEvents.MainGameEvents.OnPickedSceneObject += OnPickedSceneItem;
        }

        public override void OnDestory(FuncState funcState = FuncState.Complete)
        {
            base.OnDestory(funcState);
            GuidNewModule.Instance.RemoveFunction(this);
            GameEvents.MainGameEvents.OnGameOver -= OnGameOver;
            GameEvents.MainGameEvents.OnPickedSceneObject -= OnPickedSceneItem;
        }

        public override void ClearFunc()
        {
            base.ClearFunc();
            OnDestory();
        }

        public override void ForceFuncDestory()
        {
            base.ForceFuncDestory();
            OnDestory();
        }

        private float timesection = 0f;
        public override void Tick(float time)
        {
            base.Tick(time);
            if (m_sceneBase == null)
            {
                OnDestory();
                return;
            }
            if (m_sceneBase.CurGameStatus == SceneBase.GameStatus.GAMING)
            {
                timesection += time;
                if (timesection >= this.m_sustainTime)
                {
                    UnityEngine.Debug.Log("start hint -----");
                    //提示
                    timesection = 0f;
                    GameEvents.MainGameEvents.RequestHintSceneItemNoCameraFollow(1);
                }

            }
        }

        private void OnPickedSceneItem(SceneItemEntity entity)
        {
            timesection = 0f;
        }

        private void OnGameOver(SceneBase.GameResult result)
        {
            if (result == SceneBase.GameResult.ALL_ITEM_FOUND)
            {
                OnDestory();
            }
        }
    }
}
