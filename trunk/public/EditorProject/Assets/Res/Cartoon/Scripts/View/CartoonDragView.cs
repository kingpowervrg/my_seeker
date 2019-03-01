using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

//[RequireComponent(typeof(Image))]
public class CartoonDragView : CartoonFixedView, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public CartoonDragView()
    {

    }


    protected Action<CartoonDragView, CartoonFixed> DragMoveEnd;
    protected Action<CartoonDragView, CartoonFixed> Dragging;

    RectTransform m_canvas;

    private Vector3 m_begin_mouse_pos;
    private Vector3 m_begin_pos;

    public override void SetModel(CartoonFixed model_)
    {
        base.SetModel(model_);
    }

    public void ReisterMoveEnd(Action<CartoonDragView, CartoonFixed> act_)
    {
        this.DragMoveEnd = act_;
    }

    public void ReisterMoving(Action<CartoonDragView, CartoonFixed> act_)
    {
        this.Dragging= act_;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        var canvas = FindInParents<Canvas>(gameObject);
        if (canvas == null)
            return;

        m_canvas = canvas.transform as RectTransform;

        Vector3 globalMousePos;
        if (RectTransformUtility.ScreenPointToWorldPointInRectangle(m_canvas, eventData.position, eventData.pressEventCamera, out globalMousePos))
        {
            m_begin_mouse_pos = globalMousePos;
            m_begin_pos = this.transform.position;
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        SetDraggedPosition(eventData);
        if (null != this.Dragging)
            this.Dragging(this, m_model);
    }

    private void SetDraggedPosition(PointerEventData eventData)
    {
        var rt = this.transform;
        Vector3 globalMousePos;
        if (RectTransformUtility.ScreenPointToWorldPointInRectangle(m_canvas, eventData.position, eventData.pressEventCamera, out globalMousePos))
        {
            Vector3 offset = globalMousePos - m_begin_mouse_pos;

            rt.position = m_begin_pos + offset;
            rt.rotation = m_canvas.rotation;
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (null != this.DragMoveEnd)
            this.DragMoveEnd(this, m_model);
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
