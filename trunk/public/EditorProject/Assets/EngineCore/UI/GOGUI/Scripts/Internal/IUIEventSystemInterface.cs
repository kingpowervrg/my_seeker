using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace EngineCore
{
    public interface IUIEventSystemInterface : IPointerEnterHandler, IPointerDownHandler, IPointerClickHandler, IPointerUpHandler, IPointerExitHandler, ISelectHandler, IDeselectHandler
    {

    }
}