using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

//[RequireComponent(typeof(Image))]
public class DragAble : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{

    //private Transform m_parent_parent_trx;
    private Action<DragAbleParent> DragAbleMoveEnd;

    private Dictionary<int, RectTransform> m_DraggingPlanes = new Dictionary<int, RectTransform>();

    private Vector3 m_begin_mouse_pos;
    private Vector3 m_begin_pos;
    private void Awake()
    {
        //this.m_parent_parent_trx = this.transform.parent.parent;
    }

    public void ReisterMoveEnd(Action<DragAbleParent> act_)
    {
        this.DragAbleMoveEnd = act_;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        var canvas = FindInParents<Canvas>(gameObject);
        if (canvas == null)
            return;

        m_DraggingPlanes[eventData.pointerId] = canvas.transform as RectTransform;

        Vector3 globalMousePos;
        if (RectTransformUtility.ScreenPointToWorldPointInRectangle(m_DraggingPlanes[eventData.pointerId], eventData.position, eventData.pressEventCamera, out globalMousePos))
        {
            m_begin_mouse_pos = globalMousePos;
            m_begin_pos = this.transform.parent.transform.position;
        }
        //this.transform.parent.SetParent(canvas.transform, false);
        //SetDraggedPosition(eventData);
    }

    public void OnDrag(PointerEventData eventData)
    {
        SetDraggedPosition(eventData);
    }

    private void SetDraggedPosition(PointerEventData eventData)
    {
        //var rt = m_DraggingIcons[eventData.pointerId].GetComponent<RectTransform>();
        var rt = this.transform.parent;//GetComponent<RectTransform>();
        Vector3 globalMousePos;
        if (RectTransformUtility.ScreenPointToWorldPointInRectangle(m_DraggingPlanes[eventData.pointerId], eventData.position, eventData.pressEventCamera, out globalMousePos))
        {
            Vector3 offset = globalMousePos - m_begin_mouse_pos;

            //rt.position = globalMousePos;
            rt.position = m_begin_pos + offset;
            rt.rotation = m_DraggingPlanes[eventData.pointerId].rotation;
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        //this.transform.parent.SetParent(this.m_parent_parent_trx, false);
        //if (m_DraggingIcons[eventData.pointerId] != null)
        //	Destroy(m_DraggingIcons[eventData.pointerId]);

        //m_DraggingIcons[eventData.pointerId] = null;
        if (null != this.DragAbleMoveEnd)
            this.DragAbleMoveEnd(this.transform.parent.GetComponent<DragAbleParent>());
    }

    static public T FindInParents<T>(GameObject go) where T : Component
    {
        if (go == null) return null;
        var comp = go.GetComponent<T>();

        if (comp != null)
            return comp;

        var t = go.transform.parent;
        while (t != null && comp == null)
        {
            comp = t.gameObject.GetComponent<T>();
            t = t.parent;
        }
        return comp;
    }
}
