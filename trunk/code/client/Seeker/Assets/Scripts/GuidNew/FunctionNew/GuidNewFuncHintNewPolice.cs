#if OFFICER_SYS
using EngineCore;
using System.Collections.Generic;
using UnityEngine;
using GOGUI;
using UnityEngine.EventSystems;
namespace SeekerGame.NewGuid
{
    public class GuidNewFuncHintNewPolice : GuidNewFunctionBase
    {
        private long officerID;
        private int state = -1;
        private EventTriggerListener m_targetEvent = null;
        public override void OnExecute()
        {
            base.OnExecute();
            List<long> officer = RedPointManager.Instance.Could_comined_officers;
            for (int i = 0; i < officer.Count; i++)
            {
                ConfOfficer confOfficer = ConfOfficer.Get(officer[i]);
                if (state < confOfficer.quality)
                {
                    state = confOfficer.quality;
                    officerID = confOfficer.id;
                }
            }
            Transform m_target = GameEvents.UI_Guid_Event.GetPoliceItemById(officerID);
            if (m_target == null)
            {
                OnDestory();
            }
            m_targetRect = m_target.GetComponent<RectTransform>();

            m_targetEvent = m_target.GetComponent<EventTriggerListener>();

            GUIFrame guidTran = GuidNewModule.Instance.GetFrameByResName(UIDefine.UI_GUID);
            Transform maskRoot = guidTran.FrameRootTransform.Find("guid/mask");
            Vector3 centerPos = maskRoot.transform.InverseTransformPoint(m_targetRect.position);
            Vector3 m_effectScale = Vector3.one;

            GameEvents.UI_Guid_Event.OnLoadEffect.SafeInvoke(21000, "UI_xinshouyindao_shou.prefab", centerPos, m_effectScale, 0f);

            GameEvents.UI_Guid_Event.OnEnableClick.SafeInvoke(false);
            GameEvents.UI_Guid_Event.OnEventClick += OnEventClick;
        }

        public override void OnDestory(FuncState funcState = FuncState.Complete)
        {
            GameEvents.UI_Guid_Event.OnEnableClick.SafeInvoke(true);
            GameEvents.UI_Guid_Event.OnEventClick -= OnEventClick;
            GameEvents.UI_Guid_Event.OnRemoveEffect.SafeInvoke(21000, false);
            base.OnDestory(funcState);
           
        }

        public override void ResetFunc(bool isRetainFunc = true)
        {
            GameEvents.UI_Guid_Event.OnEnableClick.SafeInvoke(true);
            GameEvents.UI_Guid_Event.OnEventClick -= OnEventClick;
            GameEvents.UI_Guid_Event.OnRemoveEffect.SafeInvoke(21000, false);
            base.ResetFunc(isRetainFunc);
            
        }

        public override void ForceFuncDestory()
        {
            GameEvents.UI_Guid_Event.OnEnableClick.SafeInvoke(true);
            GameEvents.UI_Guid_Event.OnEventClick -= OnEventClick;
            GameEvents.UI_Guid_Event.OnRemoveEffect.SafeInvoke(21000, false);
            base.ForceFuncDestory();
            
        }

        RectTransform m_targetRect = null;
        private void OnEventClick(Vector2 worldPos)
        {
            Vector2 clickPos = m_targetRect.parent.InverseTransformPoint(worldPos);
            if (clickPos.x >= m_targetRect.localPosition.x - m_targetRect.rect.width / 2f && clickPos.x <= m_targetRect.localPosition.x + m_targetRect.rect.width / 2f)
            {
                if (clickPos.y >= m_targetRect.localPosition.y - m_targetRect.rect.height / 2f && clickPos.y <= m_targetRect.localPosition.y + m_targetRect.rect.height / 2f)
                {
                    UnityEngine.UI.Toggle tog = m_targetRect.GetComponent<UnityEngine.UI.Toggle>();

                    tog.isOn = true;

                    OnDestory();
                }
            }
        }
    }
}
#endif