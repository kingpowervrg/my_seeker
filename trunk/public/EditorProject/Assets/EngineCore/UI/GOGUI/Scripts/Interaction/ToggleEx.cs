using System;
using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;

namespace UnityEngine.UI
{
    /// <summary>
    /// Simple toggle -- something that has an 'on' and 'off' states: checkbox, toggle button, radio button, etc.
    /// </summary>
    [AddComponentMenu("UI/ToggleEx", 31)]
    [RequireComponent(typeof(RectTransform))]
    public class ToggleEx : Toggle
    {
        public RectTransform nodeShow = null;
        public RectTransform nodeHide = null;

        protected override void Start()
        {
            base.Start();
            if (nodeShow != null || nodeHide != null)
            {
                UnityAction<bool> valueChange = new UnityAction<bool>(ToggleExOnValueChange);
                this.onValueChanged.AddListener(valueChange);
                ToggleExOnValueChange(isOn);
            }

        }
        protected override void OnEnable()
        {
            base.OnEnable();
            ToggleExOnValueChange(isOn);
        }
        public void ToggleExOnValueChange(bool flag)
        {
            bool instant = toggleTransition == ToggleTransition.None;
            if(nodeShow!=null)
            {
                nodeShow.gameObject.SetActive(flag);
            }
            if (nodeHide != null)
            {
                nodeHide.gameObject.SetActive(!flag);
            }
        }

    }
}
