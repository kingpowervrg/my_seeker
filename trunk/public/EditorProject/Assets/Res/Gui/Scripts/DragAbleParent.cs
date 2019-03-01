using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

//[RequireComponent(typeof(Image))]
public class DragAbleParent : MonoBehaviour
{
    public List<RectTransform> m_child_logic_rect_list;
    public List<DragAble> m_child_icon_list;
    public RectTransform m_self_rtf;
    void Awake()
    {
        m_self_rtf = this.GetComponent<RectTransform>();

        RectTransform[] rtfs = this.GetComponentsInChildren<RectTransform>();

        foreach (RectTransform rtf in rtfs)
        {
            if (rtf.transform == this.transform)
                continue;

            DragAble da = rtf.GetComponent<DragAble>();

            if (null != da)
                m_child_icon_list.Add(da);
            else
            {
                rtf.gameObject.name = this.gameObject.name;
                m_child_logic_rect_list.Add(rtf);
            }
        }
    }
}
