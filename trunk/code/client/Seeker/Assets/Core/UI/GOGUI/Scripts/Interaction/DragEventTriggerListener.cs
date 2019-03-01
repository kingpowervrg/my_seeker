using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

namespace GOGUI
{
    public class DragEventTriggerListener : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
    {
        GameObject go;
        //public event EventTriggerListener.Vector2Delegate onDrag;
        //public event EventTriggerListener.VectorDelegate onDragStart;
        //public event EventTriggerListener.VectorDelegate onDragEnd;
        public EventTriggerListener.Vector2Delegate onDrag;
        public EventTriggerListener.VectorDelegate onDragStart;
        public EventTriggerListener.VectorDelegate onDragEnd;

        static public DragEventTriggerListener Get(GameObject go)
        {
            DragEventTriggerListener listener = go.GetComponent<DragEventTriggerListener>();
            if (listener == null) listener = go.AddComponent<DragEventTriggerListener>();
            listener.go = go;
            return listener;
        }

        static public DragEventTriggerListener Get(Transform transform)
        {
            DragEventTriggerListener listener = transform.GetComponent<DragEventTriggerListener>();
            if (listener == null) listener = transform.gameObject.AddComponent<DragEventTriggerListener>();
            return listener;
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (onDrag != null)
                onDrag(go, eventData.delta, eventData.position);
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            if (onDragStart != null)
                onDragStart(go, eventData.position);
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            if (onDragEnd != null)
                onDragEnd(go, eventData.position);
        }
    }
}