using UnityEngine;
using EngineCore;
using HedgehogTeam.EasyTouch;
namespace SeekerGame.NewGuid
{
    /// <summary>
    /// 特效提示  圆形
    /// </summary>
    public class GuidNewFuncEffectIndicateCircle : GuidNewFunctionBase
    {
        private long m_itemId = -1;
        private string itemID = string.Empty;
        private float m_radius;

        private long m_effectID;
        private string effectResName;
        private int type = 0; //0表示为物件ID  1表示为指示图标

        private Vector3 m_centerPos = Vector3.zero;
        public override void OnInit(long funcID, string[] param)
        {
            base.OnInit(funcID, param);
            long.TryParse(param[0], out m_itemId);
            if (m_itemId <= 0)
            {
                itemID = param[0];
                this.type = 1;
            }
            this.m_radius = float.Parse(param[1]);
            this.m_effectID = long.Parse(param[2]);
            this.effectResName = param[3];
        }

        public override void OnExecute()
        {
            base.OnExecute();
            GameEvents.UI_Guid_Event.OnEnableClick.SafeInvoke(false);
            if (type == 0)
            {
                SceneItemEntity entity = GameEvents.MainGameEvents.GetSceneItemEntityByID(m_itemId);
                if (entity != null)
                {
                    GetMaskLocalPosition(entity.EntityPosition);
                }
            }
            else if (type == 1)
            {
                GameObject[] cubeObjs = GameObject.FindGameObjectsWithTag("cameraPoint");
                for (int i = 0; i < cubeObjs.Length; i++)
                {
                    if (cubeObjs[i].name.Equals(itemID))
                    {
                        GetMaskLocalPosition(cubeObjs[i].transform.position);
                        break;
                    }
                }
            }
            GameEvents.UI_Guid_Event.OnEventClick += OnEventClick;
        }

        public override void OnDestory(FuncState funcState = FuncState.Complete)
        {
            base.OnDestory(funcState);
            GameEvents.UI_Guid_Event.OnEnableClick.SafeInvoke(true);
            GameEvents.UI_Guid_Event.OnEventClick -= OnEventClick;
            GameEvents.UI_Guid_Event.OnRemoveEffect.SafeInvoke(this.m_effectID, false);
        }
        private Transform m_maskRoot = null;
        private void GetMaskLocalPosition(Vector3 worldPos)
        {
            GUIFrame frame = GuidNewModule.Instance.GetFrameByResName(UIDefine.UI_GUID);
            m_maskRoot = frame.FrameRootTransform.Find("guid/mask");
            m_centerPos = EngineCore.Utility.CameraUtility.WorldPointInCanvasRectTransform(worldPos, m_maskRoot.gameObject);
            m_centerPos = m_maskRoot.InverseTransformPoint(m_centerPos);
            m_centerPos.z = 0;
            GameEvents.UI_Guid_Event.OnLoadEffect.SafeInvoke(this.m_effectID, this.effectResName, m_centerPos, Vector2.one, 0f);
        }
        Gesture gesture = null;
        private void OnEventClick(Vector2 worldPos)
        {
            worldPos = this.m_maskRoot.InverseTransformPoint(worldPos);
            float dis = Vector2.Distance(worldPos, Vector3.right * m_centerPos.x + Vector3.up * m_centerPos.y);
            if (dis <= m_radius)
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
    }
}
