using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


namespace GOGUI
{
   
    /// <summary>
    /// 图文混排
    /// </summary>
    [AddComponentMenu("Layout/Layout Content Size Fitter", 142)]
    public class LayoutContentSizeFitter : UIBehaviour, ILayoutSelfController, ILayoutElement, ILayoutIgnorer
    {
        [SerializeField]
        ContentSizeFitter.FitMode horizontalFit = ContentSizeFitter.FitMode.Unconstrained;
        [SerializeField]
        ContentSizeFitter.FitMode verticalFit = ContentSizeFitter.FitMode.Unconstrained;

        [System.NonSerialized]
        RectTransform rectTrans;

        [System.NonSerialized]
        float minX = -1;
        [System.NonSerialized]
        float minY = -1;
        [System.NonSerialized]
        float preferedX = -1;
        [System.NonSerialized]
        float preferedY = -1;
        [System.NonSerialized]
        float flexibleX = -1;
        [System.NonSerialized]
        float flexibleY = -1;
        public ContentSizeFitter.FitMode HorizontalFit
        {
            get
            {
                return horizontalFit;
            }
            set
            {
                if (horizontalFit != value)
                {
                    horizontalFit = value;
                    SetDirty();
                }
            }
        }

        public ContentSizeFitter.FitMode VerticalFit
        {
            get { return verticalFit; }
            set
            {
                if (verticalFit != value)
                {
                    verticalFit = value;
                    SetDirty();
                }
            }
        }

        public RectTransform RectTransform
        {
            get
            {
                if (!rectTrans)
                    rectTrans = GetComponent<RectTransform>();
                return rectTrans;
            }
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            SetDirty();
        }

        protected override void OnDisable()
        {
            LayoutRebuilder.MarkLayoutForRebuild(RectTransform);
            base.OnDisable();
        }

        protected override void OnRectTransformDimensionsChange()
        {
            SetDirty();
        }

        protected override void OnTransformParentChanged()
        {
            SetDirty();
        }
        protected override void OnDidApplyAnimationProperties()
        {
            SetDirty();
        }

        protected override void OnBeforeTransformParentChanged()
        {
            SetDirty();
        }
        void HandleSelfFittingAlongAxis(int axis)
        {
            ContentSizeFitter.FitMode fitting = (axis == 0 ? horizontalFit : verticalFit);
            if (fitting == ContentSizeFitter.FitMode.Unconstrained)
                return;

            if (axis == 0)
            {
                minX = LayoutUtility.GetMinWidth(RectTransform);
                if (fitting != ContentSizeFitter.FitMode.MinSize)
                    preferedX = LayoutUtility.GetPreferredSize(RectTransform, 0);
                else
                    preferedX = minX;
            }
            else
            {
                minY = LayoutUtility.GetMinHeight(RectTransform);
                if (fitting != ContentSizeFitter.FitMode.MinSize)
                    preferedY = LayoutUtility.GetPreferredSize(RectTransform, 1);
                else
                    preferedY = minY;
            }
        }

        public void SetLayoutHorizontal()
        {
            HandleSelfFittingAlongAxis(0);
        }

        public void SetLayoutVertical()
        {
            HandleSelfFittingAlongAxis(1);
        }

        void SetDirty()
        {
            if (!IsActive())
                return;

            LayoutRebuilder.MarkLayoutForRebuild(RectTransform);
        }

        public void CalculateLayoutInputHorizontal()
        {
            HandleSelfFittingAlongAxis(0);
        }

        public void CalculateLayoutInputVertical()
        {
            HandleSelfFittingAlongAxis(1);
        }

        public float flexibleHeight
        {
            get { return flexibleY; }
        }

        public float flexibleWidth
        {
            get { return flexibleX; }
        }

        public int layoutPriority
        {
            get { return 1; }
        }

        public float minHeight
        {
            get { return minY; }
        }

        public float minWidth
        {
            get { return minX; }
        }

        public float preferredHeight
        {
            get { return preferedY; }
        }

        public float preferredWidth
        {
            get { return preferedX; }
        }

        public bool ignoreLayout
        {
            get { return false; }
        }
    }
}