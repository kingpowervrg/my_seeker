using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

namespace GOGUI
{
    public class EventTriggerListener : UIBehaviour, IPointerDownHandler, IPointerEnterHandler, IPointerExitHandler, IPointerUpHandler,ISelectHandler,IDeselectHandler
    {
        private const float DURATION_THRESHOLD = 0.5f;

        protected GameObject go;
        public delegate void VoidDelegate(GameObject go);
        public delegate void BoolDelegate(GameObject go, bool state);
        public delegate void FloatDelegate(GameObject go, float delta);
        public delegate void IntDelegate(GameObject go, int value);
        public delegate void VectorDelegate(GameObject go, Vector2 delta);
        public delegate void Vector2Delegate(GameObject go, Vector2 delta, Vector2 pos);
        public delegate void ObjectDelegate(GameObject go, GameObject obj);
        public delegate void KeyCodeDelegate(GameObject go, KeyCode key);

        public VoidDelegate onClick;
        public VoidDelegate onGuidClick;
        public VoidDelegate onDown;
        public VoidDelegate onEnter;
        public VoidDelegate onExit;
        public VoidDelegate onUp;
        public BoolDelegate onUpAndState;
        public VoidDelegate onSelect;
        public VoidDelegate onDeselect;
        public VoidDelegate onUpdateSelect;
        public VectorDelegate onClickPosition;
        public VectorDelegate OnLongPress;
        public VoidDelegate OnLongPressEnd;

        public FloatDelegate onLongClick;

        public object parameter;
        public PointerEventData PointerEventData;
        public static VoidDelegate GlobalClickCallback { get; set; }

        private bool m_onPressEventTriggered = false;
        private bool m_onPressing = false;
        private float m_pressStartTime = 0f;
        private PointerEventData m_onPressPointerData;

        static public EventTriggerListener Get(GameObject go)
        {
            EventTriggerListener listener = go.GetComponent<EventTriggerListener>();
            if (listener == null) listener = go.AddComponent<EventTriggerListener>();
            listener.go = go;
            return listener;
        }

        static public EventTriggerListener Get(Transform transform)
        {
            EventTriggerListener listener = transform.GetComponent<EventTriggerListener>();
            if (listener == null) listener = transform.gameObject.AddComponent<EventTriggerListener>();
            return listener;
        }
        //public void OnPointerClick(PointerEventData eventData)
        //{
            //PointerEventData = eventData;
            //if (GlobalClickCallback != null) GlobalClickCallback(go);
            //if (onClick != null) onClick(go);
            //if (onClickPosition != null) onClickPosition(go, eventData.position);
        //}


        public void OnPointerDown(PointerEventData eventData)
        {
            PointerEventData = eventData;
            if (onDown != null)
                onDown(go);
            this.m_pressStartTime = Time.time;
            this.m_onPressing = true;
            this.m_onPressPointerData = eventData;
            if (onClickPosition != null) onClickPosition(go, eventData.position);
        }
        public void OnPointerEnter(PointerEventData eventData)
        {
            PointerEventData = eventData;
            if (onEnter != null) onEnter(go);
        }
        public void OnPointerExit(PointerEventData eventData)
        {
            PointerEventData = eventData;
            if (onExit != null) onExit(go);
        }
        public void OnPointerUp(PointerEventData eventData)
        {
            PointerEventData = eventData;

            if (onUp != null)
                onUp(go);

            if (onUpAndState != null)
                onUpAndState(go, eventData.pointerCurrentRaycast.gameObject == eventData.pointerPressRaycast.gameObject);
            if (this.m_onPressing)
            {            
                if (OnLongPressEnd != null && this.m_onPressEventTriggered)
                    OnLongPressEnd.Invoke(go);

                if (Time.time - m_pressStartTime < DURATION_THRESHOLD && !eventData.dragging)
                {
                    OnClick();
                    OnLongClick(Time.time - m_pressStartTime);
                }
            }
            this.m_onPressing = false;
            this.m_onPressEventTriggered = false;

        }
        
        public void OnSelect(BaseEventData eventData)
        {
            PointerEventData = null;
            if (onSelect != null) onSelect(go);
        }

        public void OnDeselect(BaseEventData eventData)
        {
            PointerEventData = null;
            if (onDeselect != null) onDeselect(go);
        }

        public void OnUpdateSelected(BaseEventData eventData)
        {
            PointerEventData = null;
            if (onUpdateSelect != null) onUpdateSelect(go);
        }

        public void OnLongPressBegin()
        {
            this.m_onPressEventTriggered = true;
            if (OnLongPress != null)
                OnLongPress.Invoke(go, PointerEventData.position);
        }

        public void OnMyLongPressEnd()
        {
            if (this.m_onPressing)
            {
                if (OnLongPressEnd != null && this.m_onPressEventTriggered)
                    OnLongPressEnd.Invoke(go);
                this.m_onPressing = false;
                this.m_onPressEventTriggered = false;
            }
        }

        public void OnClick()
        {
            if (onClick != null)
                onClick.Invoke(go);
            if (onGuidClick != null)
            {
                onGuidClick.Invoke(go);
            }
        }

        private const float DURATION_LONGCLICK = 0.2f;
        private float m_curPressTime = 0f;
        private bool m_multiClick = false;

        private void OnLongClick(float time)
        {
            if (onLongClick != null)
            {
                onLongClick.Invoke(go, time);
            }
        }

        public void CancelLongCllick()
        {
            m_onPressing = false;
        }

        public void Update()
        {
            if (m_onPressing && !m_onPressEventTriggered)
            {
                if (Time.time - this.m_pressStartTime >= DURATION_THRESHOLD && !m_onPressEventTriggered)
                    OnLongPressBegin();
            }
            if (onLongClick != null)
            {
                if (m_onPressing)
                {
                    if (Time.time - this.m_pressStartTime >= DURATION_THRESHOLD && !m_multiClick)
                    {
                        m_multiClick = true;
                        m_curPressTime = Time.time;
                    }
                    if (m_multiClick && Time.time - this.m_curPressTime >= DURATION_LONGCLICK)
                    {
                        OnLongClick(Time.time - m_pressStartTime);
                        m_curPressTime = Time.time;
                    }
                }
                else
                {
                    m_multiClick = false;
                    m_curPressTime = 0f;
                }
            }
            
        }

    }
}
