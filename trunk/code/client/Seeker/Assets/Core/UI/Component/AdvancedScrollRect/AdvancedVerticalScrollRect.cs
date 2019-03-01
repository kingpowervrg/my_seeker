using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace EngineCore
{
    public class AdvancedVerticalScrollRect : AdvancedScrollRect
    {
        protected override float GetSize(RectTransform item)
        {
            float size = ContentSpacing.y;
            if (m_gridLayout != null)
            {
                size += m_gridLayout.cellSize.y;
            }
            else
            {
                size += LayoutUtility.GetPreferredHeight(item);
            }
            return size;
        }


        protected override Vector2 GetVector(float value)
        {
            return new Vector2(0, value);
        }


        protected override bool UpdateItems(Bounds viewBounds, Bounds contentBounds)
        {
            bool changed = false;
            if (viewBounds.min.y < contentBounds.min.y)
            {
                float size = NewItemAtEnd();
                if (size > 0)
                {
                    if (threshold < size)
                    {
                        threshold = size * 1.1f;
                    }
                    changed = true;
                }
            }
            else if (viewBounds.min.y > contentBounds.min.y + (TemplateElementSize.y + ContentSpacing.y))
            {
                if (IsValidPosition)
                {
                    float size = DeleteItemAtEnd();
                    if (size > 0)
                    {
                        changed = true;
                    }
                }
            }
            if (viewBounds.max.y > contentBounds.max.y)
            {
                float size = NewItemAtStart();
                if (size > 0)
                {
                    if (threshold < size)
                    {
                        threshold = size * 1.1f;
                    }
                    changed = true;
                }
            }
            else if (viewBounds.max.y <= contentBounds.max.y - (TemplateElementSize.y + ContentSpacing.y))
            {
                if (IsValidPosition)
                {
                    float size = DeleteItemAtStart();
                    if (size > 0)
                    {
                        changed = true;
                    }
                }
            }
            return changed;
        }

        protected override bool IsValidPosition
        {
            get
            {
                float vertialNormalizePos = verticalNormalizedPosition;

                return (vertialNormalizePos >= 0f && vertialNormalizePos <= 1f) && (m_realContentSize.y >= m_ViewBounds.size.y);
            }
        }

        protected override Vector2 ContentSpacing
        {
            get
            {
                if (base.ContentSpacing == Vector2.zero)
                {
                    if (m_horizontalOrVerticalLayout != null)
                        m_contentSpacing.y = m_gridLayout.spacing.y;
                }

                return m_contentSpacing;
            }
        }
    }
}