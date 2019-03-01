#if UNITY_EDITOR
using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class UIEffectPreview : MonoBehaviour
{
    public bool IsUIEffect = true;
    public int SortingOrder = 0;

    private Renderer[] renders = null;

    void Awake()
    {
        renders = GetComponentsInChildren<Renderer>();
        Canvas canvas = GetComponentInParent<Canvas>();
        if (canvas != null)
            SortingOrder = canvas.sortingOrder;
    }

    void OnEnable()
    {
        foreach (Renderer render in renders)
        {
            render.sortingOrder = SortingOrder;
            render.sortingLayerName = "Default";
        }
    }

    void Update()
    {
        foreach (Renderer render in renders)
        {
            render.sortingOrder = SortingOrder;
        }
    }
}
#endif