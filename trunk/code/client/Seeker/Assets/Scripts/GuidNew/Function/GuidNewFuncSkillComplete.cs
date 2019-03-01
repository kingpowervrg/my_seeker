using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SeekerGame.NewGuid
{
    public class GuidNewFuncSkillComplete : GuidNewFunctionBase
    {
        private long propID;
        public override void OnInit(long funcID, string[] param)
        {
            base.OnInit(funcID, param);
            this.propID = long.Parse(param[0]);
        }

        public override void OnExecute()
        {
            base.OnExecute();
            GameEvents.Skill_Event.OnSkillFinish += OnSkillFinish;
        }

        private void OnSkillFinish(long propID)
        {
            if (this.propID == propID)
            {
                OnDestory();
            }
        }

        public override void OnDestory(FuncState funcState = FuncState.Complete)
        {
            base.OnDestory(funcState);
            GameEvents.Skill_Event.OnSkillFinish -= OnSkillFinish;
        }

        public override void ClearFunc()
        {
            base.ClearFunc();
            GameEvents.Skill_Event.OnSkillFinish -= OnSkillFinish;
        }
    }
}
