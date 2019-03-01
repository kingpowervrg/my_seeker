using EngineCore;
using System;
using System.Collections.Generic;

namespace SeekerGame.NewGuid
{
    public class GuidNewFuncHideNode : GuidNewFunctionBase
    {
        string resName;
        private bool m_tick = false;
        public override void OnInit(long funcID, string[] param)
        {
            base.OnInit(funcID, param);
            this.resName = param[0];
            this.m_tick = false;
        }

        public override void OnLoadRes()
        {
            base.OnLoadRes();
            if (resName == null)
            {
                return;
            }
            GUIFrame frame = GuidNewModule.Instance.GetFrameByResName(resName);
            if (frame == null)
            {
                this.m_tick = true;
                GuidNewModule.Instance.PushFunction(this);
                return;
            }
            frame.LogicHandler.OnGuidShow();
            OnDestory();
            //OnExecute();
        }

        public override void Tick(float time)
        {
            base.Tick(time);
            GUIFrame frame = GuidNewModule.Instance.GetFrameByResName(resName);
            if (frame == null)
            {
                return;
            }
            frame.LogicHandler.OnGuidShow();
            GuidNewModule.Instance.RemoveFunction(this);
            OnDestory();
        }

        public override void OnExecute()
        {
            base.OnExecute();
            this.OnLoadRes();
        }

        public override void ClearFunc()
        {
            base.ClearFunc();
            GuidNewModule.Instance.RemoveFunction(this);
        }
    }
}
