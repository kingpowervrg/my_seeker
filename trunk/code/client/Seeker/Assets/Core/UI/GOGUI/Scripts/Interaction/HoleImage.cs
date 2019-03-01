using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class HoleImage : Image {


    public override Material GetModifiedMaterial(Material baseMaterial)
    {
        var toUse = baseMaterial;

        if (m_ShouldRecalculateStencil)
        {
            var rootCanvas = MaskUtilities.FindRootSortOverrideCanvas(transform);
			m_StencilValue = maskable ? GetStencilDepth(transform, rootCanvas) : 0;
            m_ShouldRecalculateStencil = false;
        }

        // if we have a enabled Mask component then it will
        // generate the mask material. This is an optimisation
        // it adds some coupling between components though :(
		Hole maskComponent = GetComponent<Hole>();
        if (m_StencilValue > 0 && (maskComponent == null || !maskComponent.IsActive()))
        {
            var maskMat = StencilMaterial.Add(toUse, (1 << m_StencilValue) - 1, StencilOp.Keep, CompareFunction.NotEqual, ColorWriteMask.All, (1 << m_StencilValue) - 1, 0);
            StencilMaterial.Remove(m_MaskMaterial);
            m_MaskMaterial = maskMat;
            toUse = m_MaskMaterial;
        }
        return toUse;
    }
		

	public static int GetStencilDepth(Transform transform, Transform stopAfter)
	{
		var depth = 0;
		if (transform == stopAfter)
			return depth;

		var t = transform.parent;
		var components = new List<Hole> ();
		while (t != null)
		{
			t.GetComponents<Hole>(components);
			for (var i = 0; i < components.Count; ++i)
			{
				if (components[i] != null && components[i].MaskEnabled() && components[i].graphic.IsActive())
				{
					++depth;
					break;
				}
			}

			if (t == stopAfter)
				break;

			t = t.parent;
		}
		return depth;
	}
}
