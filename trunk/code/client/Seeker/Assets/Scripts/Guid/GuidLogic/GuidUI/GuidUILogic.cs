using EngineCore;
using GOGUI;
using HedgehogTeam.EasyTouch;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
namespace SeekerGame
{

    //[UILogicHandler(UIDefine.UI_GUID)]
    public class GuidUILogic : UILogicBase
    {
        private Transform m_root;
        private RectTransform m_RectRoot;
        private GameButton m_skip_btn;
        private GameTexture m_mask;
        private EventTriggerListener eventListener;
        private Camera m_uiCamera;
        private GameUIContainer m_artGrid;

        private GuidUIData m_data;

        private Camera UICamera
        {
            get
            {
                if (m_uiCamera == null)
                {
                    m_uiCamera = Canvas.worldCamera;
                }
                return m_uiCamera;
            }
        }

        private Material m_material;

        private Material material
        {
            get
            {
                if (m_material == null)
                {
                    Shader grayShader = ShaderModule.Instance.GetShader("UI/Guid");
                    m_material = new Material(grayShader);
                }
                return m_material;
            }
        }

        protected override void OnInit()
        {
            base.OnInit();

            m_root = Transform.Find("root");
            m_skip_btn = Make<GameButton>("btnSkip");
            m_mask = Make<GameTexture>("RawImage");
            m_artGrid = Make<GameUIContainer>("art");
            m_mask.RawImage.material = material;
            m_RectRoot = m_root.GetComponent<RectTransform>();
            eventListener = EventTriggerListener.Get(m_mask.gameObject);
        }
        public override FrameDisplayMode UIFrameDisplayMode
        {
            get
            {
                return FrameDisplayMode.WINDOWED;
            }
        }
        public override void OnShow(object param)
        {
            base.OnShow(param);
            m_skip_btn.AddClickCallBack(BtnSkip);
            m_mask.AddClickCallBack(BtnMask);

            m_mask.AddDragStartCallBack(DragStart);
            m_mask.AddDragEndCallBack(DragEnd);
            m_mask.AddDragCallBack(Draging);
            if (param != null)
            {
                GuidMaskCommonData maskData = (GuidMaskCommonData)param;
                m_skip_btn.Visible = maskData.m_confGuid.isSkip;
                m_data = new GuidUIData(maskData.hasEvent, maskData.m_operaType, maskData.m_TypeValue, maskData.eventPassType);
                InitMask(maskData.m_maskdata);
                InitArt(maskData.m_artPos, maskData.m_confGuid);
            }
            ///////////
            m_mask.RawImage.raycastTarget = (m_data.OperaType != GuidEnum.Guid_DragScene);
            if (m_data.OperaType == GuidEnum.Guid_DragScene)
            {
                EngineCoreEvents.InputEvent.OnSwipeBegin += OnSwipeBegin;
                EngineCoreEvents.InputEvent.OnSwipeEnd += OnSwipeEnd;
            }
        }

        public override void OnHide()
        {
            base.OnHide();
            m_skip_btn.RemoveClickCallBack(BtnSkip);
            m_mask.RemoveClickCallBack(BtnMask);
            m_data.Clear();
            ///
            m_mask.RemoveDragStartCallBack(DragStart);
            m_mask.RemoveDragEndCallBack(DragEnd);
            m_mask.RemoveDragCallBack(Draging);
            if (m_data.OperaType == GuidEnum.Guid_DragScene)
            {
                EngineCoreEvents.InputEvent.OnSwipeBegin -= OnSwipeBegin;
                EngineCoreEvents.InputEvent.OnSwipeEnd -= OnSwipeEnd;
            }
            //m_rectData.Clear();
            //m_CircleData.Clear();
        }

        public void SetData(Transform tran)
        {
            tran.SetParent(m_root);
        }

        private void BtnSkip(GameObject obj)
        {
            GameEvents.UI_Guid_Event.OnNextGuid.SafeInvoke();
        }

        private void InitMask(List<GuidMaskData> maskDatas)
        {
            GuidMaskBoardData board = GuidTools.InitMask(maskDatas, m_RectRoot, m_data);
            m_material.SetVectorArray("circleCenter", board.circleCenter);
            m_material.SetFloatArray("circleRadius", board.circleRadius);
            m_material.SetVectorArray("rectCenter", board.rectCenter);
            m_material.SetFloatArray("rectWidth", board.rectWidth);
            m_material.SetFloatArray("rectHeigh", board.rectHeigh);

        }

        private void InitArt(List<Vector2> pos, ConfGuid confGuid) //, List<string> path
        {
            m_artGrid.EnsureSize<GameUIEffect>(pos.Count);
            for (int i = 0; i < pos.Count; i++)
            {
                ConfGuidArt guidArt = ConfGuidArt.Get(confGuid.artIDs[i]);
                GameUIEffect uiEffect = m_artGrid.GetChild<GameUIEffect>(i);
                //uiEffect.SetRealScale(new Vector3(guidArt.artScale[0],guidArt.artScale[1],1));
                uiEffect.EffectPrefabName = guidArt.artPath;
                Vector3 localPos = GuidTools.WordToLocalPos(m_RectRoot, pos[i]);
                uiEffect.gameObject.transform.localPosition = new Vector3(localPos.x, localPos.y, 0);
                uiEffect.Visible = true;
            }
        }

        private void BtnMask(GameObject obj)
        {
            Vector2 worldPos = GuidTools.ScreenToWorldPos(UICamera, eventListener.PointerEventData.position);
            string btnName = m_data.CheckLegalPoint(worldPos);
            if (btnName == null)
            {
                return;
            }

            if (!m_data.CheckBtnLegal())
            {
                return;
            }

            if (!string.IsNullOrEmpty(btnName))
            {
                MaskClick(btnName);
            }
        }

        private void MaskClick(string btnName)
        {
            GuidTools.PassEvent(btnName, eventListener.PointerEventData, ExecuteEvents.pointerClickHandler, true);
        }



        #region
        private bool isDrag = false;
        private Vector2 m_startPos = Vector2.zero;
        #endregion

        private void DragStart(GameObject go, Vector2 delta)
        {
            if (!CheckDrag(m_data.OperaType))
            {
                return;
            }
            m_startPos = GuidTools.ScreenToWorldPos(UICamera, delta);
            string btnName = m_data.CheckLegalPoint(m_startPos);
            if (btnName != null)
            {
                isDrag = true;
                GuidTools.PassEvent(btnName, eventListener.PointerEventData, ExecuteEvents.beginDragHandler, false);
            }
        }

        private void DragEnd(GameObject go, Vector2 delta)
        {
            if (!CheckDrag(m_data.OperaType))
            {
                return;
            }
            isDrag = false;
            if (m_data.OperaType == GuidEnum.Guid_Drag)
            {
                GuidTools.PassEvent("", eventListener.PointerEventData, ExecuteEvents.endDragHandler, false);
            }
            Vector2 worldPos = GuidTools.ScreenToWorldPos(UICamera, delta);
            string btnName = m_data.CheckLegalPoint(worldPos);
            if (btnName != null)
            {
                float dis = Vector2.Distance(m_startPos, worldPos);
                if (dis >= m_data.TypeValue)
                {
                    GameEvents.UI_Guid_Event.OnNextGuid.SafeInvoke();
                }
            }
        }

        private void Draging(GameObject go, Vector2 delta, Vector2 pos)
        {
            if (!CheckDrag(m_data.OperaType))
            {
                return;
            }
            GuidTools.PassEvent("", eventListener.PointerEventData, ExecuteEvents.dragHandler, false);
        }

        private bool CheckDrag(GuidEnum operaType)
        {
            if (operaType != GuidEnum.Guid_Drag)
            {
                return false;
            }
            return true;
        }

        private bool CheckDragScene(GuidEnum operaType)
        {
            if (operaType != GuidEnum.Guid_DragScene)
            {
                return false;
            }
            return true;
        }

        private void OnSwipeBegin(Gesture gesture)
        {
            if (!CheckDragScene(m_data.OperaType))
            {
                return;
            }
            m_startPos = GuidTools.ScreenToWorldPos(UICamera, gesture.position);
            string btnName = m_data.CheckLegalPoint(m_startPos);
            if (btnName != null)
            {
                isDrag = true;
            }
        }

        private void OnSwipeEnd(Gesture gesture)
        {
            if (!CheckDragScene(m_data.OperaType))
            {
                return;
            }
            isDrag = false;
            Vector2 worldPos = GuidTools.ScreenToWorldPos(UICamera, gesture.position);
            string btnName = m_data.CheckLegalPoint(worldPos);
            if (btnName != null)
            {
                float dis = Vector2.Distance(m_startPos, worldPos);
                if (dis >= m_data.TypeValue)
                {
                    GameEvents.UI_Guid_Event.OnNextGuid.SafeInvoke();
                }
            }
        }
    }
}
