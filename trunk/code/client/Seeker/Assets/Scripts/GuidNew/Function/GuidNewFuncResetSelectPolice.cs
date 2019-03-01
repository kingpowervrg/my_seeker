#if OFFICER_SYS
using System;
using System.Collections.Generic;
using EngineCore;
namespace SeekerGame.NewGuid
{
    public class GuidNewFuncResetSelectPolice : GuidNewFunctionBase
    {

        public override void OnInit(long funcID, string[] param)
        {
            base.OnInit(funcID, param);
        }

        public override void OnExecute()
        {
            base.OnExecute();
            GUIFrame frame = GuidNewModule.Instance.GetFrameByResName(UIDefine.UI_ENGER_GAME_UI);
            if (frame == null)
            {
                GuidNewModule.Instance.PushFunction(this);
                return;
            }
            GameEvents.UIEvents.UI_Enter_Event.OnClearDispatchPoclie.SafeInvoke();
            OnDestory();
        }

        public override void Tick(float time)
        {
            base.Tick(time);
            GUIFrame frame = GuidNewModule.Instance.GetFrameByResName(UIDefine.UI_ENGER_GAME_UI);
            if (frame != null)
            {
                GameEvents.UIEvents.UI_Enter_Event.OnClearDispatchPoclie.SafeInvoke();
                GuidNewModule.Instance.RemoveFunction(this);
                OnDestory();
            }
        }

        public override void ClearFunc()
        {
            base.ClearFunc();
            GuidNewModule.Instance.RemoveFunction(this);
        }

        public override void ForceFuncDestory()
        {
            base.ForceFuncDestory();
            GuidNewModule.Instance.RemoveFunction(this);
        }
    }
}
#endif