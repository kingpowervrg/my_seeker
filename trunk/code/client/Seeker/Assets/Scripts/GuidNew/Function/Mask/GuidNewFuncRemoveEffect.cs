using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SeekerGame.NewGuid
{
    public class GuidNewFuncRemoveEffect : GuidNewFunctionBase
    {
        private long m_effectID;
        public override void OnInit(long funcID, string[] param)
        {
            base.OnInit(funcID, param);
            this.m_effectID = long.Parse(param[0]);
        }

        public override void OnExecute()
        {
            base.OnExecute();
            GameEvents.UI_Guid_Event.OnRemoveEffect.SafeInvoke(m_effectID,true);
            OnDestory();
        }

        //public override void OnDestory()
        //{
        //    base.OnDestory();
        //}
    }
}
