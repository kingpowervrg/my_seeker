using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SeekerGame.NewGuid
{
    public class GuidNewFuncForbidProp : GuidNewFunctionBase
    {
        private long m_propId;
        private bool m_isForbid;
        public override void OnInit(long funcID, string[] param)
        {
            base.OnInit(funcID, param);
            this.m_propId = long.Parse(param[0]);
            this.m_isForbid = bool.Parse(param[1]);
        }

        public override void OnExecute()
        {
            base.OnExecute();
            GameEvents.MainGameEvents.OnAlwaysForbidProp.SafeInvoke(this.m_propId,this.m_isForbid);
            OnDestory();
        }
    }
}
