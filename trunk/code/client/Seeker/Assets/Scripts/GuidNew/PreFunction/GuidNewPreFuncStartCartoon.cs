
using System;

namespace SeekerGame.NewGuid
{
    public class GuidNewPreFuncStartCartoon : GuidNewPreFunctionBase
    {
        public override void OnInit(string[] param)
        {
            base.OnInit(param);
        }

        public override void OnCheck(Action action)
        {
            base.OnCheck(action);
            if (GuidNewNodeManager.Instance.getCanCartoon())
            {
                if (action != null)
                {
                    action();
                }
            }
        }
    }
}
