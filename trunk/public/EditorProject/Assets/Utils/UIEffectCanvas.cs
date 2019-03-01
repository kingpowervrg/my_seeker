using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class UIEffectCanvas : MonoBehaviour
{

    public int CustomSortOrder = 0;

    void OnEnable()
    {
        InitCustomSortOrder();
    }

    private void InitCustomSortOrder()
    {
        Canvas customCanvas = GetComponent<Canvas>();
        if (customCanvas != null)
        {
            CustomSortOrder = customCanvas.sortingOrder;
#if !RES_EDITOR
             GameObject.DestroyImmediate(customCanvas);
#endif

        }
    }
}
