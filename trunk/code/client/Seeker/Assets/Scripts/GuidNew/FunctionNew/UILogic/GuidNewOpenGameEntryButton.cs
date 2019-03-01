using EngineCore;

namespace SeekerGame.NewGuid
{
    public class GuidNewOpenGameEntryButton : GuidNewFunctionBase
    {

        public override void OnExecute()
        {
            base.OnExecute();
            GameEvents.UIEvents.UI_GameEntry_Event.OnOpenBottomButton.SafeInvoke();
            TimeModule.Instance.SetTimeout(()=> {
                OnDestory();
            }, 0.4f);
        }

        public override void ForceFuncDestory()
        {
            base.ForceFuncDestory();
            TimeModule.Instance.RemoveTimeaction(() =>
            {
                OnDestory();
            });
        }
    }
}
