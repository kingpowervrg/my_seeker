using EngineCore;
using UnityEngine;
namespace SeekerGame.NewGuid
{
    /// <summary>
    /// 提示道具使用
    /// </summary>
    public class GuidNewFuncScenePropHintRule : GuidNewFunctionBase
    {
        private long m_propId;
        private int m_continueTime;
        private float m_spaceTime;

        public override void OnInit(long funcID, string[] param)
        {
            base.OnInit(funcID, param);
            this.m_propId = long.Parse(param[0]);
            this.m_continueTime = int.Parse(param[1]);
            this.m_spaceTime = float.Parse(param[2]);
        }
        private GameSceneBase m_sceneBase = null;
        public override void OnExecute()
        {
            base.OnExecute();
            m_sceneBase = (GameSceneBase)SceneModule.Instance.CurrentScene;
            GameEvents.MainGameEvents.OnGameOver += OnGameOver;
            GuidNewModule.Instance.PushFunction(this);
            GameEvents.MainGameEvents.OnPickedSceneObject += OnPickedSceneItem;
            //GameEvents.MainGameEvents.OnPropUseTips.SafeInvoke(this.m_propId,1);
            //OnDestory();
            //Transform m_ GameEvents.UIEvents.UI_GameMain_Event.GetPropItemById.SafeInvoke(this.m_propId);
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
                if (timesection >= this.m_spaceTime)
                {
                    //提示
                    timesection = 0f;
                    GameEvents.MainGameEvents.OnGuidPropUseTips.SafeInvoke(this.m_propId,this.m_continueTime);
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
