using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SeekerGame.NewGuid
{
    /// <summary>
    /// 事件普通
    /// </summary>
    public class GuidNewFuncEventNormalState : GuidNewFunctionBase
    {
        private long taskConfId;
        private int eventState;

        public override void OnInit(long funcID, string[] param)
        {
            base.OnInit(funcID, param);
            this.taskConfId = long.Parse(param[0]);
            this.eventState = int.Parse(param[1]);
        }

        public override void OnExecute()
        {
            base.OnExecute();
            GameEvents.UIEvents.UI_EventGame_Event.EventNormalState += EventNormalState;
        }

        public override void OnDestory(FuncState funcState = FuncState.Complete)
        {
            base.OnDestory(funcState);
            GameEvents.UIEvents.UI_EventGame_Event.EventNormalState -= EventNormalState;
        }

        public override void ForceFuncDestory()
        {
            base.ForceFuncDestory();
            GameEvents.UIEvents.UI_EventGame_Event.EventNormalState -= EventNormalState;
        }

        public override void ClearFunc()
        {
            base.ClearFunc();
            GameEvents.UIEvents.UI_EventGame_Event.EventNormalState -= EventNormalState;
        }

        private void EventNormalState(long taskId,int eventState)
        {
            if (this.taskConfId == taskId && this.eventState == eventState)
            {
                m_guidBase.OnSaveCurrentProgress();
                OnDestory();
            }
        }
    }
}
