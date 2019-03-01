#if OFFICER_SYS
using EngineCore;

namespace SeekerGame.NewGuid
{
    public class GuidNewFuncNewPoliceMainPanel : GuidNewFunctionBase
    {
        public override void OnExecute()
        {
            base.OnExecute();
            GUIFrame frame = GuidNewModule.Instance.GetFrameByResName(UIDefine.UI_GAMEENTRY);
            if (frame != null)
            {
                OnDestory();
                return;
            }
            GameEvents.UI_Guid_Event.OnOpenUI += OnOpenUI;
        }

        public override void OnDestory(FuncState funcState = FuncState.Complete)
        {
            GameEvents.UI_Guid_Event.OnOpenUI -= OnOpenUI;
            GameEvents.UIEvents.UI_GameEntry_Event.OnNewPoliceEffect.SafeInvoke(true);
            base.OnDestory(funcState);
        }

        private void OnOpenUI(GUIFrame frame)
        {
            if (frame.ResName.Equals(UIDefine.UI_GAMEENTRY))
            {
                OnDestory();
            }
        }
    }
}
#endif