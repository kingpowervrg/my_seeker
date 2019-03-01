using System;
using System.Collections.Generic;

namespace SeekerGame.NewGuid
{
    public class GuidNewFuncSceneSucessResult : GuidNewFunctionBase
    {
        public override void OnExecute()
        {
            base.OnExecute();
            GameEvents.UI_Guid_Event.OnSceneShowResult.SafeInvoke();
            GuidNewNodeManager.Instance.sceneShowResult = true;
        }
    }
}
