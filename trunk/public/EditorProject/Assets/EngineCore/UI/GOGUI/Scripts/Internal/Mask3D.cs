using System;
using UnityEngine.EventSystems;
using UnityEngine.Rendering;
using UnityEngine.Serialization;

namespace UnityEngine.UI
{
    [AddComponentMenu("UI/Mask3D", 13)]
    [ExecuteInEditMode]
    [RequireComponent(typeof(RectTransform))]
    public class Mask3D : Mask
    {
        [NonSerialized]
        private Material m_MaskMaterial;
        protected override void OnEnable()
        {
            base.OnEnable();
            if (graphic != null)
            {
                graphic.SetMaterialDirty();
            }

            MaskUtilities.NotifyStencilStateChanged(this);
        }

        protected override void OnDisable()
        {
            // we call base OnDisable first here
            // as we need to have the IsActive return the
            // correct value when we notify the children
            // that the mask state has changed.
            base.OnDisable();
            if (graphic != null)
            {
                graphic.SetMaterialDirty();
            }

            StencilMaterial.Remove(m_MaskMaterial);
            m_MaskMaterial = null;
            MaskUtilities.NotifyStencilStateChanged(this);
        }
        /// Stencil calculation time!
        public override Material GetModifiedMaterial(Material baseMaterial)
        {
            if (graphic == null)
                return baseMaterial;

            var rootSortCanvas = MaskUtilities.FindRootSortOverrideCanvas(transform);
            var stencilDepth = MaskUtilities.GetStencilDepth(transform, rootSortCanvas);
            if (stencilDepth >= 8)
            {
                Debug.LogError("Attempting to use a stencil mask with depth > 8", gameObject);
                return baseMaterial;
            }

            int desiredStencilBit = 1 << stencilDepth;

            // if we are at the first level...
            // we want to destroy what is there
            if (desiredStencilBit == 1)
            {
                var maskMaterial = StencilMaterial.Add(baseMaterial, 1, StencilOp.Replace, CompareFunction.Always, showMaskGraphic ? ColorWriteMask.All : 0);
                StencilMaterial.Remove(m_MaskMaterial);
                m_MaskMaterial = maskMaterial;
                return m_MaskMaterial;
            }

            //otherwise we need to be a bit smarter and set some read / write masks
            var maskMaterial2 = StencilMaterial.Add(baseMaterial, desiredStencilBit | (desiredStencilBit - 1), StencilOp.Replace, CompareFunction.Equal, showMaskGraphic ? ColorWriteMask.All : 0, desiredStencilBit - 1, desiredStencilBit | (desiredStencilBit - 1));
            StencilMaterial.Remove(m_MaskMaterial);
            m_MaskMaterial = maskMaterial2;

            return m_MaskMaterial;
        }
    }
}
