using System;
using System.Collections.Generic;
using EngineCore;
namespace SeekerGame.NewGuid
{
    public class GuidNewFuncFrameOpen : GuidNewFunctionBase
    {
        private float m_delayTime;
        private string m_FrameName;
        public override void OnInit(long funcID, string[] param)
        {
            base.OnInit(funcID, param);
            this.m_delayTime = float.Parse(param[1]);
            this.m_FrameName = param[0];
        }

        public override void OnExecute()
        {
            base.OnExecute();
            GUIFrame frame = GuidNewModule.Instance.GetFrameByResName(this.m_FrameName);
            if (frame == null)
            {
                GameEvents.UI_Guid_Event.OnOpenUI += OpenUI;
                return;
            }
            OnDestory();
        }


        private void OpenUI(GUIFrame frame)
        {
            if (frame.ResName.Equals(this.m_FrameName))
            {
                GameEvents.UI_Guid_Event.OnOpenUI -= OpenUI;
                if (m_delayTime > 0)
                {
                    TimeModule.Instance.SetTimeout(TimeTick, m_delayTime);
                }
                else
                {
                    TimeTick();
                }
            }
        }

        private void TimeTick()
        {
            OnDestory();
        }


        public override void ClearFunc()
        {
            base.ClearFunc();
            GameEvents.UI_Guid_Event.OnOpenUI -= OpenUI;
            TimeModule.Instance.RemoveTimeaction(TimeTick);
        }

        public override void ForceFuncDestory()
        {
            base.ForceFuncDestory();
            GameEvents.UI_Guid_Event.OnOpenUI -= OpenUI;
            TimeModule.Instance.RemoveTimeaction(TimeTick);
        }
    }
}
