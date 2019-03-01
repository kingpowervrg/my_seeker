using EngineCore;
using GOGUI;
using UnityEngine;
namespace SeekerGame.NewGuid
{
    /// <summary>
    /// 特效提示UI
    /// </summary>
    public class GuidNewFuncEffectIndicate : GuidNewFunctionBase
    {
        private string m_resPrefab;
        private string m_resName;

        private long m_effectID;
        private string m_effectName;
        private bool m_needChoose = true;

        public override void OnInit(long funcID, string[] param)
        {
            base.OnInit(funcID, param);
            this.m_resPrefab = param[0];
            this.m_resName = param[1];

            this.m_effectID = long.Parse(param[2]);
            this.m_effectName = param[3];
            this.m_needChoose = bool.Parse(param[4]);
        }

        public override void OnExecute()
        {
            base.OnExecute();
            GUIFrame frame = GuidNewModule.Instance.GetFrameByResName(this.m_resPrefab);
            if (frame == null)
            {
                GameEvents.UI_Guid_Event.OnOpenUI += OpenUI;
            }
        }

        public override void OnDestory(FuncState funcState = FuncState.Complete)
        {
            base.OnDestory(funcState);
            GuidNewModule.Instance.RemoveFunction(this);
        }

        private void OpenUI(GUIFrame frame)
        {
            GameEvents.UI_Guid_Event.OnOpenUI -= OpenUI;
            if (!frame.Equals(this.m_resPrefab))
            {
                return;
            }

        }

        private void OnResExcute(GUIFrame frame)
        {
            Transform m_targetTrans = frame.FrameRootTransform.Find(this.m_resName);
            if (m_needChoose)
            {
                EventTriggerListener trigger = m_targetTrans.GetComponent<EventTriggerListener>();
                trigger.onGuidClick = onGuidClick;
            }
        }

        private void onGuidClick(GameObject obj)
        {
            OnDestory();
        }
    }
}
