using System;
using System.Collections.Generic;
using EngineCore;

namespace SeekerGame.NewGuid
{
    /// <summary>
    /// 游戏开始
    /// </summary>
    public class GuidNewFuncOnStartGame : GuidNewFunctionBase
    {
        private float m_delayTime = 0f;
        public override void OnInit(long funcID, string[] param)
        {
            base.OnInit(funcID, param);
            if (param.Length >= 1)
            {
                this.m_delayTime = float.Parse(param[0]);
            }
        }

        public override void OnExecute()
        {
            base.OnExecute();
            GameEvents.MainGameEvents.OnGameInitComplete += OnStartGame;
        }

        public override void OnDestory(FuncState funcState = FuncState.Complete)
        {
            base.OnDestory(funcState);
            GameEvents.MainGameEvents.OnGameInitComplete -= OnStartGame;
        }

        public override void ClearFunc()
        {
            base.ClearFunc();
            GameEvents.MainGameEvents.OnGameInitComplete -= OnStartGame;
            TimeModule.Instance.RemoveTimeaction(OnTimeTick);
        }

        private void OnStartGame()
        {
            if (m_delayTime > 0f)
            {
                TimeModule.Instance.SetTimeout(OnTimeTick, this.m_delayTime);
            }
            else
            {
                OnTimeTick();
            }
        }

        private void OnTimeTick()
        {
            OnDestory();
        }
    }
}
