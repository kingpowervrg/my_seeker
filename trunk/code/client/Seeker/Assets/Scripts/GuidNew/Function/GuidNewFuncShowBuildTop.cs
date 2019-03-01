using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SeekerGame.NewGuid
{
    public class GuidNewFuncShowBuildTop : GuidNewFunctionBase
    {
        private long buildID;
        private string buildNode;
        private int buidState = 1;
        public override void OnInit(long funcID,string[] param)
        {
            base.OnInit(funcID, param);
            if (param.Length == 3)
            {
                buildID = long.Parse(param[0]);
                buildNode = param[1];
                buidState = int.Parse(param[2]);
            }
        }

        public override void OnExecute()
        {
            base.OnExecute();
            if (buidState == 1)
            {
                GameEvents.BigWorld_Event.OnShowBuildTopUI.SafeInvoke(buildID, buildNode,true);
            }
            else
            {
                GameEvents.BigWorld_Event.OnHideBuidTopUI.SafeInvoke(buildID, buildNode);
            }
            OnDestory();
        }
    }
}
