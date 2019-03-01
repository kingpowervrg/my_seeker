using UnityEngine.EventSystems;

namespace EngineCore
{
    public interface IUIEventSystemInterface : IPointerEnterHandler, IPointerDownHandler, IPointerClickHandler, IPointerUpHandler, IPointerExitHandler, ISelectHandler, IDeselectHandler
    {

    }
}