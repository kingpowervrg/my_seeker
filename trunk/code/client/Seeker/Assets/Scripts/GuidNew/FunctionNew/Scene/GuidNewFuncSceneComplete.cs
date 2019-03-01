using EngineCore;
namespace SeekerGame.NewGuid
{
    public class GuidNewFuncSceneComplete : GuidNewFunctionBase
    {
        private float m_delayTime = 0f;
        private int m_type = 0; //0 寻物  1拼图
        public override void OnInit(long funcID, string[] param)
        {
            base.OnInit(funcID, param);
            if (param.Length >= 1)
            {
                m_type = int.Parse(param[0]);
            }
            if (param.Length >= 2)
            {
                this.m_delayTime = float.Parse(param[1]);
            }
        }

        public override void OnExecute()
        {
            base.OnExecute();
            if (0 == m_type)
            {
                GameEvents.MainGameEvents.OnGameOver += OnGameOver;
            }
            else if (1 == m_type)
            {
                GameEvents.UIEvents.UI_Jigsaw_Event.OnJigsawFinish += OnJigsawFinish;
            }
        }

        private void GameComplete()
        {
            if (this.m_delayTime > 0f)
            {
                TimeModule.Instance.SetTimeout(TimeDelayAction, this.m_delayTime);
            }
            else
            {
                TimeDelayAction();
            }
        }

        private void TimeDelayAction()
        {
            OnDestory();
        }

        public override void OnDestory(FuncState funcState = FuncState.Complete)
        {
            base.OnDestory(funcState);
            if (0 == m_type)
                GameEvents.MainGameEvents.OnGameOver -= OnGameOver;
            else if (1 == m_type)
            {
                GameEvents.UIEvents.UI_Jigsaw_Event.OnJigsawFinish -= OnJigsawFinish;
            }
            //m_guidBase.ForceFuncFinish();
        }

        public override void ClearFunc()
        {
            base.ClearFunc();
            if (0 == m_type)
                GameEvents.MainGameEvents.OnGameOver -= OnGameOver;
            else if (1 == m_type)
            {
                GameEvents.UIEvents.UI_Jigsaw_Event.OnJigsawFinish -= OnJigsawFinish;
            }
        }

        private void OnGameOver(SceneBase.GameResult result)
        {
            if (result == SceneBase.GameResult.ALL_ITEM_FOUND)
            {
                GameComplete();
            }
        }

        private void OnJigsawFinish(bool isWin)
        {
            GameComplete();
        }
    }
}
