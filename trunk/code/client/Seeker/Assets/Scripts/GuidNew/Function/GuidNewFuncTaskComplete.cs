using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SeekerGame.NewGuid
{
    public class GuidNewFuncTaskComplete : GuidNewFunctionBase
    {
        private long m_taskID;
        public override void OnInit(long funcID, string[] param)
        {
            base.OnInit(funcID, param);
            this.m_taskID = long.Parse(param[0]);
        }

        public override void OnExecute()
        {
            base.OnExecute();
            GlobalInfo.MY_PLAYER_INFO.PlayerTaskSystem.IsCompleteTaskByConfigID(m_taskID,(bool taskComplete) => {
                if (taskComplete)
                {
                    OnDestory();
                    return;
                }
                GameEvents.TaskEvents.OnReceiveTask += OnCompletedTask;
            });
           
        }

        private void OnCompletedTask(TaskBase taskbase)
        {
            if (taskbase.TaskConfID == m_taskID)
            {
                OnDestory();
            }
        }

        public override void OnDestory(FuncState funcState = FuncState.Complete)
        {
            base.OnDestory(funcState);
            GameEvents.TaskEvents.OnReceiveTask -= OnCompletedTask;
        }

        public override void ClearFunc()
        {
            base.ClearFunc();
            GameEvents.TaskEvents.OnReceiveTask -= OnCompletedTask;
        }
    }
}
