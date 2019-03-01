using System;
using System.Collections.Generic;
using EngineCore;
namespace SeekerGame.NewGuid
{
    public class GuidNewFuncScenePropHint : GuidNewFuncLoadMask
    {
        private bool m_isComplete = false;
        public override void OnInit(long funcID, string[] param)
        {
            base.OnInit(funcID, param);
        }

        public override void OnExecute()
        {
            GameEvents.MainGameEvents.OnGameOver += OnGameOver;
            TimeModule.Instance.SetTimeout(()=>{
                if (!m_isComplete)
                {
                    base.OnExecute();
                }
                else
                {
                    OnDestory();
                }
            },1f);
            
        }

        private void OnGameOver(SceneBase.GameResult result)
        {
            this.m_isComplete = true;
        }

        public override void OnDestory(FuncState funcState = FuncState.Complete)
        {
            base.OnDestory(funcState);
            GameEvents.MainGameEvents.OnGameOver -= OnGameOver;
        }
    }
}
