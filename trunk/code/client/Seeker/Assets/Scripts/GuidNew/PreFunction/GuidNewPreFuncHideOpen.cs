using System;
using System.Collections.Generic;
using EngineCore;

namespace SeekerGame.NewGuid
{
    public class GuidNewPreFuncHideOpen : GuidNewPreFunctionBase
    {
        string resName;
        int hideType = 0;
        public override void OnInit(string[] param)
        {
            base.OnInit(param);
            this.resName = param[0];
            if (param.Length > 1)
            {
                this.hideType = int.Parse(param[1]);
            }
        }

        public override void OnCheck(Action action)
        {
            base.OnCheck(action);
            if (resName == null)
            {
                return;
            }
            GUIFrame frame = GuidNewModule.Instance.GetFrameByResName(resName);
            if (frame != null && action != null)
            {
                frame.LogicHandler.OnGuidShow(this.hideType);
                action();
            }
        }
    }
}
