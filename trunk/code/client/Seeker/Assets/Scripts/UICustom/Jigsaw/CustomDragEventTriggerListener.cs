using EngineCore;
using GOGUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.EventSystems;

namespace SeekerGame
{

    public class CustomDragEventTriggerListener : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
    {
        GameObject go;

        public SafeAction<PointerEventData> onDrag;
        public SafeAction<PointerEventData> onDragStart;
        public SafeAction<PointerEventData> onDragEnd;

        static public CustomDragEventTriggerListener Get(GameObject go)
        {
            CustomDragEventTriggerListener listener = go.GetComponent<CustomDragEventTriggerListener>();
            if (listener == null) listener = go.AddComponent<CustomDragEventTriggerListener>();
            listener.go = go;
            return listener;
        }

        static public CustomDragEventTriggerListener Get(Transform transform)
        {
            CustomDragEventTriggerListener listener = transform.GetComponent<CustomDragEventTriggerListener>();
            if (listener == null) listener = transform.gameObject.AddComponent<CustomDragEventTriggerListener>();
            return listener;
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (onDrag != null)
                onDrag.SafeInvoke(eventData);
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            if (onDragStart != null)
                onDragStart.SafeInvoke(eventData);
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            if (onDragEnd != null)
                onDragEnd.SafeInvoke(eventData);
        }
    }
}
