using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SeekerGame.NewGuid
{
    public class GuidNewFuncSceneFindComplete : GuidNewFunctionBase
    {
        private bool m_needShowResult = false;
        public override void OnInit(long funcID, string[] param)
        {
            base.OnInit(funcID, param);
            if (param.Length == 1)
            {
                m_needShowResult = bool.Parse(param[0]);
            }
        }

        public override void OnExecute()
        {
            base.OnExecute();
            GameEvents.UI_Guid_Event.OnFindSceneResult += OnGameOver;
           // GameEvents.MainGameEvents.OnGameOver += OnGameOver;
        }

        private void OnGameOver(bool result)
        {
            if (!result)
            {
                OnDestory(FuncState.CompleteError);
            }
            else
            {
                GuidNewNodeManager.Instance.sceneShowResult = m_needShowResult;
                m_guidBase.CheckGroupComplete();
                OnDestory(FuncState.Complete);
            }
        }

        public override void OnDestory(FuncState funcState = FuncState.Complete)
        {
            // this.m_guidBase.ForceFuncFinish();
            GameEvents.UI_Guid_Event.OnFindSceneResult -= OnGameOver;
            //GameEvents.MainGameEvents.OnGameOver -= OnGameOver;
            base.OnDestory(funcState);
        }

        public override void ClearFunc()
        {
            base.ClearFunc();
            GameEvents.UI_Guid_Event.OnFindSceneResult -= OnGameOver;
        }
    }
}
