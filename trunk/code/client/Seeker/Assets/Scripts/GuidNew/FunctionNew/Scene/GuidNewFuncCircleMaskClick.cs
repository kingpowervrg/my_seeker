using EngineCore;
using HedgehogTeam.EasyTouch;
using UnityEngine;
namespace SeekerGame.NewGuid
{
    public class GuidNewFuncCircleMaskClick : GuidNewFunctionBase
    {
        public override void OnInit(long funcID, string[] param)
        {
            base.OnInit(funcID, param);
        }

        private Vector4 m_centerPos;
        private Transform m_maskRoot = null;
        public override void OnExecute()
        {
            base.OnExecute();
            GUIFrame frame = GuidNewModule.Instance.GetFrameByResName(UIDefine.UI_GUID);
            this.m_maskRoot = frame.FrameRootTransform.Find("guid/mask");
            m_centerPos = GameEvents.UI_Guid_Event.OnGetCurrentMaskInfo(0);
            GameEvents.UI_Guid_Event.OnMaskClickEvent += OnMaskClickEvent;
        }
        Gesture gesture = null;
        private void OnMaskClickEvent(Vector2 worldPos)
        {
            worldPos = this.m_maskRoot.InverseTransformPoint(worldPos);
            float dis = Vector2.Distance(worldPos, Vector3.right * m_centerPos.x + Vector3.up * m_centerPos.y);
            if (dis <= m_centerPos.z)
            {
                gesture = new Gesture();
                GameObject obj = gesture.GetCurrentPickedObject();
                if (obj == null)
                {
                    return;
                }
                gesture.pickedObject = obj;
                EngineCoreEvents.InputEvent.OnOneFingerTouchup.SafeInvoke(gesture);
                OnDestory();
            }
        }

        public override void OnDestory(FuncState funcState = FuncState.Complete)
        {
            base.OnDestory(funcState);
            GameEvents.UI_Guid_Event.OnMaskClickEvent -= OnMaskClickEvent;

        }

        public override void ClearFunc()
        {
            base.ClearFunc();
            GameEvents.UI_Guid_Event.OnMaskClickEvent -= OnMaskClickEvent;
        }
    }
}
