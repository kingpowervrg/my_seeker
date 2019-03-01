using System;
using System.Collections.Generic;

namespace SeekerGame.NewGuid
{
    public class GuidNewFuncSceneStart : GuidNewFunctionBase
    {
        public override void OnInit(long funcID, string[] param)
        {
            base.OnInit(funcID, param);
        }

        public override void OnExecute()
        {
            base.OnExecute();
            GameEvents.MainGameEvents.OnStartGame += OnStartGame;
        }

        private void OnStartGame()
        {
            OnDestory();
        }

        public override void OnDestory(FuncState funcState = FuncState.Complete)
        {
            base.OnDestory(funcState);
            GameEvents.MainGameEvents.OnStartGame -= OnStartGame;
        }

        public override void ClearFunc()
        {
            base.ClearFunc();
            GameEvents.MainGameEvents.OnStartGame -= OnStartGame;
        }
    }
}
