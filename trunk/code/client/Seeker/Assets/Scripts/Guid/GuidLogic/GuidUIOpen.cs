using EngineCore;

namespace SeekerGame
{
    public class GuidUIOpen : GuidUI
    {

        protected override void OnOpenUI(GUIFrame uiLogic)
        {
            base.OnOpenUI(uiLogic);
            FrameMgr.OpenUIParams param = new FrameMgr.OpenUIParams(UIDefine.UI_GUID);
            param.OnShowFinishCallback = OnShowMask;
            EngineCoreEvents.UIEvent.ShowUIEventWithParam.SafeInvoke(param);
        }

        protected virtual void OnShowMask(UILogicBase logic)
        {
            m_uiLogic = logic;
        }
    }
}
