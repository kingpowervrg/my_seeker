using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SeekerGame.NewGuid
{
    public class GuidNewPreFuncId : GuidNewPreFunctionBase
    {

        private long id = -1;
        private int index = -1;
        public override void OnInit(string[] param)
        {
            base.OnInit(param);
            if (param.Length == 1)
            {
                id = long.Parse(param[0]);
                ConfGuidNew confguid = ConfGuidNew.Get(id);
                index = confguid.groupId;
            }
        }

        public override void OnCheck(Action action)
        {
            base.OnCheck(action);
            if (GuidNewManager.Instance.GetProgressByIndex(index) || GuidNewManager.Instance.GetGuidNewStatus(id))
            {
                if (action != null)
                {
                    action();
                }
            }
        }
    }
}
