using System;
using System.Collections.Generic;
using EngineCore;

namespace SeekerGame.NewGuid
{
    public class GuidNewPreFuncFrameOpen : GuidNewPreFunctionBase
    {
        float time = 0f;
        string[] resName;
        public override void OnInit(string[] param)
        {
            base.OnInit(param);
            this.time = float.Parse(param[0]);
            this.resName = param;
            
        }

        public override void OnCheck(Action action)
        {
            base.OnCheck(action);
            int temp = 0;
            for (int i = 1; i < resName.Length; i++)
            {
                GUIFrame frame = GuidNewModule.Instance.GetFrameByResName(resName[i]);
                if (frame != null)
                {
                    temp++;
                }
            }
            if (temp == resName.Length - 1 && action != null)
            {
                if (time > 0f)
                {
                    TimeModule.Instance.SetTimeout(() =>
                    {
                        action();
                    }, time);
                }
                else
                {
                    action();
                }
            }
        }
    }
}
