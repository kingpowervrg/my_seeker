/********************************************************************
	created:  2019-1-3 11:42:13
	filename: TweenAlpha.cs
	author:	  songguangze@outlook.com
	
	purpose:  Alpha动画
*********************************************************************/
using DG.Tweening;
using UnityEngine;

namespace EngineCore
{
    [ExecuteInEditMode]
    public class TweenAlpha : MaskableGraphicTweener
    {
        public float From = 1f;
        public float To = 1f;

        private CanvasGroup m_cachedCanvasGroup = null;

        protected override void Awake()
        {
            base.Awake();

            m_cachedCanvasGroup = GetComponent<CanvasGroup>();
        }

        public override void Play(bool playForward)
        {
            base.Play(playForward);

            //当同时有CanvasGroup和Image时，TweenAlpha使用CanvasGroup
            if (m_cachedCanvasGroup)
            {
                m_cachedCanvasGroup.alpha = From;
                this.m_uiTweener = m_cachedCanvasGroup.DOFade(To, Duration);
            }
            else if (CachedMaskableGraphic)
            {
                Color currentColor = CachedMaskableGraphic.color;
                Color fromColor = new Color(currentColor.r, currentColor.g, currentColor.b, From);
                CachedMaskableGraphic.color = fromColor;
                this.m_uiTweener = CachedMaskableGraphic.DOFade(To, Duration);
            }

            PlayInternal();
        }

        [ContextMenu("切换到To值状态")]
        public void SetCurrentValueToEnd()
        {
            if (m_cachedCanvasGroup)
                m_cachedCanvasGroup.alpha = To;
            else if (CachedMaskableGraphic)
            {
                Color currentColor = CachedMaskableGraphic.color;
                Color toColor = new Color(currentColor.r, currentColor.g, currentColor.b, To);
                CachedMaskableGraphic.color = toColor;
            }
        }

        [ContextMenu("切换到From值状态")]
        public void SetCurrentValueToStart()
        {
            if (m_cachedCanvasGroup)
                m_cachedCanvasGroup.alpha = From;
            else if (CachedMaskableGraphic)
            {
                Color currentColor = CachedMaskableGraphic.color;
                Color fromColor = new Color(currentColor.r, currentColor.g, currentColor.b, From);
                CachedMaskableGraphic.color = fromColor;
            }
        }

        [ContextMenu("设置当前值为To的值")]
        public void SetEndToCurrentValue()
        {
            if (m_cachedCanvasGroup)
                To = m_cachedCanvasGroup.alpha;
            else if (CachedMaskableGraphic)
                To = CachedMaskableGraphic.color.a;
        }

        [ContextMenu("设置当前值为From的值")]
        public void SetStartToCurrentValue()
        {
            if (m_cachedCanvasGroup)
                From = m_cachedCanvasGroup.alpha;
            else if (CachedMaskableGraphic)
                From = CachedMaskableGraphic.color.a;
        }

        public CanvasGroup CachedCanvasGroup => this.m_cachedCanvasGroup;

        protected override object TweenerStartValue
        {
            get
            {
                return this.From;
            }
        }
    }
}