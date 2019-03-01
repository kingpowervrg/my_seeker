using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SeekerGame.NewGuid
{
    public class GuidNewFuncSceneInit : GuidNewFunctionBase
    {
        public override void OnExecute()
        {
            base.OnExecute();
            GuidNewNodeManager.Instance.sceneShowResult = false;
        }
    }
}
