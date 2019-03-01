using System;
using System.Collections.Generic;
using EngineCore;
using UnityEngine;

namespace SeekerGame.NewGuid
{
    public class GuidNewFuncLoadEffectByFrame : GuidNewFunctionBase
    {
        private long m_effectID;
        private string m_effectName;
        private string m_frameName;
        private string m_resName;
        private bool m_autoDestory = false;
        private float m_delayTime;
        private Vector2 m_effectScale = Vector2.one;
        public override void OnInit(long funcID, string[] param)
        {
            base.OnInit(funcID, param);
            this.m_effectID = long.Parse(param[0]);
            this.m_effectName = param[1];
            this.m_frameName = param[2];
            this.m_resName = param[3].Replace(":","/");
            this.m_effectScale.x = float.Parse(param[4]);
            this.m_effectScale.y = float.Parse(param[5]);
            if (param.Length > 5)
            {
                this.m_autoDestory = bool.Parse(param[6]);
                this.m_delayTime = float.Parse(param[7]);
            }
        }

        private Transform maskRoot = null;
        private RectTransform m_targetRect = null;
        public override void OnExecute()
        {
            base.OnExecute();
            GUIFrame frame = GuidNewModule.Instance.GetFrameByResName(m_frameName);
            if (frame == null)
            {
                return;
            }
            Transform resTran = frame.FrameRootTransform.Find(m_resName);
            if (resTran == null)
            {
                Debug.LogError("guid mask itemName error  " + m_resName);
                return;
            }
            m_targetRect = resTran.GetComponent<RectTransform>();

            GUIFrame guidTran = GuidNewModule.Instance.GetFrameByResName(UIDefine.UI_GUID);
            maskRoot = guidTran.FrameRootTransform.Find("guid/mask");
            if (this.m_delayTime > 0)
            {
                TimeModule.Instance.SetTimeout(OnTimeDelay, this.m_delayTime);
            }
            else
            {
                OnTimeDelay();
            }
        }

        public override void OnDestory(FuncState funcState = FuncState.Complete)
        {
            base.OnDestory(funcState);
            GameEvents.UI_Guid_Event.OnEventClick -= OnEventClick;
            GameEvents.UI_Guid_Event.OnEnableClick.SafeInvoke(true);
            GameEvents.UI_Guid_Event.OnRemoveEffect.SafeInvoke(this.m_effectID, false);
            TimeModule.Instance.RemoveTimeaction(OnTimeDelay);
        }

        public override void ForceFuncDestory()
        {
            base.ForceFuncDestory();
            GameEvents.UI_Guid_Event.OnEventClick -= OnEventClick;
            GameEvents.UI_Guid_Event.OnRemoveEffect.SafeInvoke(this.m_effectID, false);
            TimeModule.Instance.RemoveTimeaction(OnTimeDelay);
        }

        public override void ResetFunc(bool isRetainFunc = true)
        {
            base.ResetFunc(isRetainFunc);
            GameEvents.UI_Guid_Event.OnEventClick -= OnEventClick;
            GameEvents.UI_Guid_Event.OnRemoveEffect.SafeInvoke(this.m_effectID, false);
            TimeModule.Instance.RemoveTimeaction(OnTimeDelay);
        }

        public override void ClearFunc()
        {
            base.ClearFunc();
            GameEvents.UI_Guid_Event.OnEventClick -= OnEventClick;
            GameEvents.UI_Guid_Event.OnRemoveEffect.SafeInvoke(this.m_effectID, false);
            TimeModule.Instance.RemoveTimeaction(OnTimeDelay);
        }

        private void OnTimeDelay()
        {
            Vector3 centerPos = maskRoot.transform.InverseTransformPoint(m_targetRect.position);
            m_effectScale.x = m_targetRect.sizeDelta.x / m_effectScale.x;
            m_effectScale.y = m_targetRect.sizeDelta.y / m_effectScale.y;
            GameEvents.UI_Guid_Event.OnLoadEffect.SafeInvoke(this.m_effectID, this.m_effectName, centerPos, m_effectScale, 0f);
            if (m_autoDestory)
            {
                OnDestory();
                return;
            }
            GameEvents.UI_Guid_Event.OnEnableClick.SafeInvoke(false);
            GameEvents.UI_Guid_Event.OnEventClick += OnEventClick;

        }

        private void OnEventClick(Vector2 worldPos)
        {
            OnDestory();
        }
    }
}
