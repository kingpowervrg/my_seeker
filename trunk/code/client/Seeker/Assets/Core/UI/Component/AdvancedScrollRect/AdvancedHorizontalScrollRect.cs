using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace EngineCore
{
    public class AdvancedHorizontalScrollRect : AdvancedScrollRect
    {
        protected override float GetSize(RectTransform item)
        {
            float size = ContentSpacing.x;
            if (m_gridLayout != null)
            {
                size += m_gridLayout.cellSize.x;
            }
            else
            {
                size += LayoutUtility.GetPreferredWidth(item);
            }
            return size;
        }


        protected override Vector2 GetVector(float value)
        {
            return new Vector2(-value, 0);
        }


        protected override bool UpdateItems(Bounds viewBounds, Bounds contentBounds)
        {
            bool changed = false;

            if (viewBounds.max.x > contentBounds.max.x)
            {
                float size = NewItemAtEnd();
                if (size > 0)
                {
                    if (threshold < size)
                        threshold = size * 1.1f;

                    changed = true;
                }
            }

            if (viewBounds.max.x < contentBounds.max.x - (TemplateElementSize.x + ContentSpacing.x))
            {
                if (IsValidPosition)
                {
                    float size = DeleteItemAtEnd();
                    if (size > 0)
                        changed = true;
                }
            }

            if (viewBounds.min.x < contentBounds.min.x)
            {
                float size = NewItemAtStart();
                if (size > 0)
                {
                    if (threshold < size)
                        threshold = size * 1.1f;

                    changed = true;
                }
            }

            if (viewBounds.min.x >= contentBounds.min.x + (TemplateElementSize.x + ContentSpacing.x))
            {
                if (IsValidPosition)
                {
                    float size = DeleteItemAtStart();
                    if (size > 0)
                        changed = true;
                }
            }


            return changed;
        }

        protected override bool UpdateEdgeStatus()
        {
            bool isChange = false;
            bool isMoveStartOld = this.m_isMoveToStart;
            float normalizeHorizontalPosition = horizontalNormalizedPosition;
            this.m_isMoveToStart = normalizeHorizontalPosition == 0;
            if (this.m_isMoveToStart != isMoveStartOld)
                isChange = true;

            bool isMoveEndOld = this.m_isMoveToEnd;
            this.m_isMoveToEnd = normalizeHorizontalPosition == 1;
            if (this.m_isMoveToEnd != isMoveEndOld)
                isChange = true;

            return isChange;
        }


        protected override Vector2 ContentSpacing
        {
            get
            {
                if (base.ContentSpacing == Vector2.zero)
                {
                    if (m_horizontalOrVerticalLayout != null)
                        m_contentSpacing.x = m_horizontalOrVerticalLayout.spacing;
                }

                return m_contentSpacing;
            }
        }


        protected override bool IsValidPosition
        {
            get
            {
                float horizontalNormalizedPos = horizontalNormalizedPosition;

                return (horizontalNormalizedPos >= 0f && horizontalNormalizedPos <= 1f) && (m_realContentSize.x >= m_ViewBounds.size.x);
            }
        }

    }
}