using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EngineCore;
using SeekerGame.NewGuid;
using GOGUI;

namespace SeekerGame
{
    public enum MaskEmptyType
    {
        None,
        Circle,
        Rect
    }

    public enum MaskAnimType
    {
        None,
        ToOut, //往外扩张
        ToInner //向里收缩
    }

    [UILogicHandler(UIDefine.UI_GUID)]
    public class GuidNewUILogic : UILogicBase
    {
        private GameImage m_maskImg;
        private Vector2[] m_emptyPos; //空白坐标
        private float m_EmptySpeed = 1f;
        private Material m_maskMat = null;
        private EventTriggerListener eventListener;
        private GameUIContainer m_effectRoot = null;

        private GuidNewMaskSystem m_maskSystem = null;
        private GuidNewTalkComponent m_talkComponent = null;
        private GuidNewEffectSystem m_effectSystem = null;
        private GameImage m_maskEvent = null;
        private EventTriggerListener m_maskEventTrigger;
        private GameImage m_uiMask = null;
        private bool EnableClickMask
        {
            get {
                return this.m_maskEvent.Visible;
            }
            set
            {
                this.m_maskEvent.Visible = value;
            }
        }

        private bool UIMaskStatus = false;

        private bool EnableUIClickMask
        {
            get
            {
                return this.m_uiMask.Visible;
            }
            set
            {
                this.m_uiMask.Visible = value;
            }
        }

        private bool m_NeedLongClick = false;
        public override FrameDisplayMode UIFrameDisplayMode
        {
            get
            {
                return FrameDisplayMode.WINDOWED;
            }
        }
        protected override void OnInit()
        {
            base.OnInit();
            this.m_maskImg = Make<GameImage>("guid:mask");
            this.m_maskMat = this.m_maskImg.GetSprite().material;
            this.eventListener = EventTriggerListener.Get(m_maskImg.gameObject);
            this.m_talkComponent = Make<GuidNewTalkComponent>("talk");
            this.m_maskSystem = new GuidNewMaskSystem(this.m_maskMat, this.m_maskImg, eventListener);
            this.m_effectRoot = Make<GameUIContainer>("guid:art");
            this.m_effectSystem = new GuidNewEffectSystem(m_effectRoot);
            this.m_maskEvent = Make<GameImage>("guid:maskEvent");
            this.m_maskEventTrigger = EventTriggerListener.Get(m_maskEvent.gameObject);
            this.m_uiMask = Make<GameImage>("maskBG");
            this.AutoDestroy = false;
        }

        public override void OnShow(object param)
        {
            base.OnShow(param);
            GameEvents.UI_Guid_Event.OnShowMask += OnShowMask;
            GameEvents.UI_Guid_Event.OnMaskTickComplete += OnMaskTickComplete;
            GameEvents.UI_Guid_Event.OnEnableClick += OnEnableClick;
            GameEvents.UI_Guid_Event.OnUIEnableClick += OnUIEnableClick;
            GameEvents.UI_Guid_Event.OnTalkEvent += OnTalkEvent;
            GameEvents.UI_Guid_Event.OnGetUIMaskStatus += OnGetUIMaskStatus;
            GameEvents.UI_Guid_Event.OnMaskTalkVisible += OnMaskTalkVisible;
            GameEvents.UI_Guid_Event.OnMaskEnableClick += OnMaskEnableClick;
            GameEvents.UI_Guid_Event.OnClearGuid += OnClearGuid;
            this.m_maskImg.AddClickCallBack(OnMaskClick);
            this.m_uiMask.AddLongPressCallBack(MaskPressDownCallBack);
            this.m_uiMask.AddLongPressEndCallBack(MaskPressUpCallBack);
            //this.m_maskImg.AddLongClickCallBack(OnMaskLongClick);
            this.m_uiMask.AddClickCallBack(OnTalkClick);
            this.m_maskEvent.AddClickCallBack(OnEventClick);
        }

        public void ShowSingleEmpty(Vector2[] emptyPos, MaskEmptyType emptyType,MaskAnimType animType,string eventName)
        {
            if (emptyPos.Length != 4)
            {
                return;
            }
            this.m_maskImg.Visible = true;
            this.m_maskSystem.AddSingleEmpty(emptyPos,emptyType, animType, eventName);
        }


        public override void OnHide()
        {
            base.OnHide();
            GameEvents.UI_Guid_Event.OnShowMask -= OnShowMask;
            GameEvents.UI_Guid_Event.OnMaskTickComplete -= OnMaskTickComplete;
            GameEvents.UI_Guid_Event.OnEnableClick -= OnEnableClick;
            GameEvents.UI_Guid_Event.OnUIEnableClick -= OnUIEnableClick;
            GameEvents.UI_Guid_Event.OnTalkEvent -= OnTalkEvent;
            GameEvents.UI_Guid_Event.OnGetUIMaskStatus -= OnGetUIMaskStatus;
            GameEvents.UI_Guid_Event.OnMaskTalkVisible -= OnMaskTalkVisible;
            GameEvents.UI_Guid_Event.OnMaskEnableClick -= OnMaskEnableClick;
            GameEvents.UI_Guid_Event.OnClearGuid -= OnClearGuid;
            this.m_maskImg.RemoveClickCallBack(OnMaskClick);
            this.m_uiMask.RemoveClickCallBack(OnTalkClick);
            this.m_uiMask.RemoveLongPressCallBack(MaskPressDownCallBack);
            this.m_uiMask.RemoveLongPressEndCallBack(MaskPressUpCallBack);
            this.m_maskEvent.RemoveClickCallBack(OnEventClick);
        }

        public override void Dispose()
        {
            base.Dispose();
            this.m_effectSystem.OnDestory();
            this.m_maskSystem.OnDestory();
        }
        private Camera m_uiCamera;
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

        private void OnClearGuid()
        {
            this.EnableClickMask = false;
            this.UIMaskStatus = false;
            this.EnableUIClickMask = false;
            this.m_NeedLongClick = false;
        }

        private void OnShowMask(List<Vector2[]> pos,List<MaskEmptyType> type, MaskAnimType animType, string eventName,bool needLongClick)
        {
            this.m_NeedLongClick = needLongClick;
            OnMaskEnableClick(!this.m_NeedLongClick);
            if (pos == null)
            {
                this.m_maskImg.Visible = true;
                this.EnableClickMask = false;
                //this.m_maskSystem.ClearMask();
            }
            else if (pos.Count == 1)
            {
                this.EnableClickMask = true;
                ShowSingleEmpty(pos[0], type[0], animType, eventName);
                
            }
            else if (pos.Count > 1)
            {
                this.EnableClickMask = true;
                this.m_maskImg.Visible = true;
                this.m_maskSystem.AddMutliEmpty(pos, type);
            }
            
        }

        private void OnMaskClick(GameObject obj)
        {
            if (!m_NeedLongClick)
            {
                Vector2 worldPos = GuidTools.ScreenToWorldPos(UICamera, eventListener.PointerEventData.position);
                m_maskSystem.OnMaskClick(worldPos);
                GameEvents.UI_Guid_Event.OnMaskClickEvent.SafeInvoke(worldPos);
            }
            
        }

        private void MaskPressDownCallBack(GameObject obj, Vector2 delta)
        {
            if (m_NeedLongClick)
            {
                Vector2 worldPos = GuidTools.ScreenToWorldPos(UICamera, eventListener.PointerEventData.position);
                m_maskSystem.OnMaskPressDown(worldPos);
            }
            
        }

        private void MaskPressUpCallBack(GameObject obj)
        {
            if (m_NeedLongClick)
            {
                Vector2 worldPos = GuidTools.ScreenToWorldPos(UICamera, eventListener.PointerEventData.position);
                m_maskSystem.OnMaskPressUp(worldPos);
            }
        }

        private void OnMaskTickComplete()
        {
            this.EnableClickMask = false;
        }

        private void OnEnableClick(bool enable)
        {
            this.EnableClickMask = !enable;
        }

        private void OnUIEnableClick(bool enable)
        {
            this.EnableUIClickMask = !enable;
            this.UIMaskStatus = !enable;
        }

        private void OnTalkClick(GameObject obj)
        {
            GameEvents.UI_Guid_Event.OnTalkClick.SafeInvoke();
        }

        private void OnEventClick(GameObject obj)
        {
            Vector2 worldPos = GuidTools.ScreenToWorldPos(UICamera, this.m_maskEventTrigger.PointerEventData.position);
            GameEvents.UI_Guid_Event.OnEventClick.SafeInvoke(worldPos);
        }

        private bool OnGetUIMaskStatus()
        {
            return this.UIMaskStatus;
        }

        private void OnTalkEvent(string content, int type, Vector2 startPos, Vector2 endPos,int needClick)
        {
            if (needClick == 2)
            {
                this.EnableUIClickMask = true;
            }
        }

        private void OnMaskTalkVisible(bool visible)
        {
            if (!this.UIMaskStatus)
            {
                this.EnableUIClickMask = false;
            }
        }

        private void OnMaskEnableClick(bool enableClick)
        {
            m_maskImg.EnableClick = enableClick;
        }
    }
}

