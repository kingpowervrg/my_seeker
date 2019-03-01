using EngineCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace SeekerGame
{
    public class ReceivePointerTex : GameTexture, IUIEventSystemInterface
    {
        public SafeAction<Vector2> PointerDown;
        public SafeAction PointerUp;

        public void OnPointerEnter(UnityEngine.EventSystems.PointerEventData eventData)
        {

        }

        public void OnPointerDown(UnityEngine.EventSystems.PointerEventData eventData)
        {
            Vector2 pos = eventData.position;
            //显示特效
            PointerDown.SafeInvoke(pos);
        }

        public void OnPointerClick(UnityEngine.EventSystems.PointerEventData eventData)
        {

        }

        public void OnPointerUp(UnityEngine.EventSystems.PointerEventData eventData)
        {
            //隐藏特效
            PointerUp.SafeInvoke();
        }

        public void OnPointerExit(UnityEngine.EventSystems.PointerEventData eventData)
        {

        }

        public void OnSelect(UnityEngine.EventSystems.BaseEventData eventData)
        {

        }

        public void OnDeselect(UnityEngine.EventSystems.BaseEventData eventData)
        {

        }
    }

}
