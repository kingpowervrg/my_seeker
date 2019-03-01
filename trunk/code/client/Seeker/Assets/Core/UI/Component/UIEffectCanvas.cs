using EngineCore;
using UnityEngine;

[ExecuteInEditMode]
public class UIEffectCanvas : MonoBehaviour
{
    public int CustomSortOrder = 0;

    [HideInInspector]
    public string BuildinEffectName = string.Empty;

#if RES_PROJECT
    public void Update()
    {
        Renderer[] effectRenderers = gameObject.GetComponentsInChildren<Renderer>(true);
        for (int i = 0; i < effectRenderers.Length; ++i)
        {
            Renderer effectRender = effectRenderers[i];
            effectRender.sortingOrder = CustomSortOrder;
            effectRender.gameObject.layer = LayerMask.NameToLayer("UI");
        }
    }
#endif

    public bool IsBuildinEffect => BuildinEffectName.EndsWithFast(".prefab");
}
