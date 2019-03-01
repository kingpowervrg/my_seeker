using EngineCore;
using UnityEngine;
using GOGUI;
using UnityEngine.UI;
namespace SeekerGame.NewGuid
{
    /// <summary>
    /// 特效提示
    /// </summary>
    public class GuidNewFuncEffectMaskByFrame : GuidNewFunctionBase
    {
        private long m_effectID;
        private string frameName;
        private string itemName;
        private string effectRes;

        private float m_delayTime;
        private bool m_needClickItem = false;
        public override void OnInit(long funcID, string[] param)
        {
            base.OnInit(funcID, param);
            this.m_effectID = long.Parse(param[0]);
            this.frameName = param[1];
            this.itemName = param[2].Replace(":","/");
            this.effectRes = param[3];
            this.m_needClickItem = bool.Parse(param[4]);
            this.m_delayTime = float.Parse(param[5]);
        }
        GameObject m_maskRoot = null;
        private RectTransform m_target = null;
        private EventTriggerListener m_targetEvent = null;
        //private Image m_btnImg = null;
        public override void OnExecute()
        {
            base.OnExecute();
            if (m_delayTime > 0f)
            {
                TimeModule.Instance.SetTimeout(TimeDelay, m_delayTime);
            }
            else
            {
                TimeDelay();
            }
           
        }

        private void TimeDelay()
        {
            GameEvents.UIEvents.UI_Bonus_Pop_View_Event.BonusPopViewVisible.SafeInvoke(false);
            GameEvents.UIEvents.UI_Bonus_Pop_View_Event.Tell_OnBlock.SafeInvoke(true);
            GameEvents.UIEvents.UI_GameEntry_Event.OnReceiveRewardAuto.SafeInvoke();
            GameEvents.UI_Guid_Event.OnEnableClick.SafeInvoke(false);
            GameEvents.UI_Guid_Event.OnEventClick += OnEventClick;
            GUIFrame frame = GuidNewModule.Instance.GetFrameByResName(frameName);
            Transform itemTran = frame.FrameRootTransform.Find(itemName);
            if (itemTran == null)
            {
                Debug.LogError("itemName not exist :" + itemName);
                OnDestory();
                return;
            }
            m_target = itemTran.GetComponent<RectTransform>();
            
            m_targetEvent = m_target.GetComponent<EventTriggerListener>();

            GUIFrame frameGuid = GuidNewModule.Instance.GetFrameByResName(UIDefine.UI_GUID);
            if (frameGuid == null || frameGuid.FrameRootTransform == null)
            {
                GuidNewModule.Instance.PushFunction(this);
                return;
            }
            m_maskRoot = frameGuid.FrameRootTransform.Find("guid/mask").gameObject;
            Vector3 entityLocalPos = m_maskRoot.transform.InverseTransformPoint(m_target.position);
            GameEvents.UI_Guid_Event.OnLoadEffect.SafeInvoke(this.m_effectID, this.effectRes, entityLocalPos, Vector2.one, 0f);
        }

        public override void OnDestory(FuncState funcState = FuncState.Complete)
        {
            GameEvents.UI_Guid_Event.OnEnableClick.SafeInvoke(true);
            GameEvents.UI_Guid_Event.OnEventClick -= OnEventClick;
            GameEvents.UI_Guid_Event.OnRemoveEffect.SafeInvoke(this.m_effectID, false);
            TimeModule.Instance.RemoveTimeaction(TimeDelay);
            base.OnDestory(funcState);
           
        }

        public override void ResetFunc(bool isRetainFunc = true)
        {
            base.ResetFunc(isRetainFunc);
            GameEvents.UI_Guid_Event.OnEventClick -= OnEventClick;
            GameEvents.UI_Guid_Event.OnRemoveEffect.SafeInvoke(this.m_effectID, false);
            TimeModule.Instance.RemoveTimeaction(TimeDelay);
        }

        public override void ForceFuncDestory()
        {
            base.ForceFuncDestory();
            GameEvents.UI_Guid_Event.OnEventClick -= OnEventClick;
            GameEvents.UI_Guid_Event.OnRemoveEffect.SafeInvoke(this.m_effectID, false);
            TimeModule.Instance.RemoveTimeaction(TimeDelay);
            GuidNewModule.Instance.RemoveFunction(this);
        }

        public override void ClearFunc()
        {
            base.ClearFunc();
            GameEvents.UI_Guid_Event.OnEventClick -= OnEventClick;
            GameEvents.UI_Guid_Event.OnRemoveEffect.SafeInvoke(this.m_effectID, false);
            TimeModule.Instance.RemoveTimeaction(TimeDelay);
            GuidNewModule.Instance.RemoveFunction(this);
        }


        private void OnEventClick(Vector2 worldPos)
        {
            if (!m_needClickItem)
            {
                OnDestory();
                return;
            }
            Vector2 clickPos = m_target.parent.InverseTransformPoint(worldPos);
            if (clickPos.x >= m_target.localPosition.x - m_target.rect.width / 2f && clickPos.x <= m_target.localPosition.x + m_target.rect.width / 2f)
            {
                if (clickPos.y >= m_target.localPosition.y - m_target.rect.height / 2f && clickPos.y <= m_target.localPosition.y + m_target.rect.height / 2f)
                {
                    m_targetEvent.OnClick();
                    OnDestory();
                }
            }
        }

        public override void Tick(float time)
        {
            base.Tick(time);
            GUIFrame frameGuid = GuidNewModule.Instance.GetFrameByResName(UIDefine.UI_GUID);
            if (frameGuid == null || frameGuid.FrameRootTransform == null)
            {
                return;
            }
            GuidNewModule.Instance.RemoveFunction(this);
            Vector3 entityLocalPos = m_maskRoot.transform.InverseTransformPoint(m_target.position);
            GameEvents.UI_Guid_Event.OnLoadEffect.SafeInvoke(this.m_effectID, this.effectRes, entityLocalPos, Vector2.one, 0f);
        }
    }
}
