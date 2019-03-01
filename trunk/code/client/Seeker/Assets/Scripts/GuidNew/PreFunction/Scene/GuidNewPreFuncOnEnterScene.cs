using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SeekerGame.NewGuid
{
    public class GuidNewPreFuncOnEnterScene : GuidNewPreFunctionBase
    {
        private int m_taskID; //场景类型 同SceneMode
        public override void OnInit(string[] param)
        {
            base.OnInit(param);
            this.m_taskID = int.Parse(param[0]);
        }

        public override void OnCheck(Action action)
        {
            base.OnCheck(action);
            if (GameMainHelper.Instance.currentTaskID == m_taskID)
            {
                action();
            }
        }
    }
}
