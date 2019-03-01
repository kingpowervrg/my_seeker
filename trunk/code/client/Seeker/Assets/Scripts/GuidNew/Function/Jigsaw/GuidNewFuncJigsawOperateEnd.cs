using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SeekerGame.NewGuid
{
    public class GuidNewFuncJigsawOperateEnd : GuidNewFunctionBase
    {
        public override void OnInit(long funcID, string[] param)
        {
            base.OnInit(funcID, param);
        }

        public override void OnExecute()
        {
            base.OnExecute();
            GameEvents.UI_Guid_Event.OnJigsawEnd += OnJigsawEnd;
        }

        public override void OnDestory(FuncState funcState = FuncState.Complete)
        {
            base.OnDestory(funcState);
            GameEvents.UI_Guid_Event.OnJigsawEnd -= OnJigsawEnd;
            GameEvents.UI_Guid_Event.OnEnablePause.SafeInvoke(true);
        }

        private void OnJigsawEnd(int type)
        {
            OnDestory();
        }
    }
}
