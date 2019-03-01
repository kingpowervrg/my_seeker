namespace SeekerGame.NewGuid
{
    /// <summary>
    /// 监听物品是否被找到
    /// </summary>
    public class GuidNewFuncSceneListenPropFind : GuidNewFunctionBase
    {
        private long m_propID;
        public override void OnInit(long funcID, string[] param)
        {
            base.OnInit(funcID, param);
            this.m_propID = long.Parse(param[0]);
        }

        public override void OnExecute()
        {
            base.OnExecute();
            GameEvents.MainGameEvents.OnPickedSceneObject += OnPickedSceneItem;
            GameEvents.MainGameEvents.OnGameOver += OnGameOver;
        }

        public override void OnDestory(FuncState funcState = FuncState.Complete)
        {
            base.OnDestory(funcState);
            GameEvents.MainGameEvents.OnPickedSceneObject -= OnPickedSceneItem;
            GameEvents.MainGameEvents.OnGameOver -= OnGameOver;
        }

        private void OnPickedSceneItem(SceneItemEntity entity)
        {
            if (entity.EntityData.itemID == m_propID)
            {
                OnDestory();
            }
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
