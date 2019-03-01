using System.Collections.Generic;
using EngineCore;

namespace SeekerGame
{
    //UI相关
    public class GuidUI : AbsGuid
    {
        protected UILogicBase m_uiLogic;
        public override void StartGuid()
        {
            base.StartGuid();
            GuidModule.Instance.AddGuidUICache(this);
            //GameEvents.UI_Guid_Event.OnOpenUI += OnOpenUIAction;
        }

        public string GetUIName()
        {
            return m_CurConf.uiName;
        }

        public void OnOpenUIAction(GUIFrame uiLogic)
        {
            if (m_RootConf.uiName.Equals(uiLogic.ResName))
            {
                OnOpenUI(uiLogic);
            }
        }

        protected virtual void OnOpenUI(GUIFrame uiLogic)
        {
            
        }
        

        protected override void EndGuid()
        {
            base.EndGuid();
            EngineCoreEvents.UIEvent.HideUIEvent.SafeInvoke(UIDefine.UI_GUID);
        }

        protected override void Destory()
        {
            base.Destory();
            //GameEvents.UI_Guid_Event.OnOpenUI -= OnOpenUIAction;
        }
    }
}
