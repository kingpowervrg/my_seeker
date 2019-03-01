using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace EngineCore
{
    public class TweenFillAmount : MaskableGraphicTweener
    {
        public float From = 1f;
        public float To = 1f;

        public override void Play(bool playForward)
        {
            base.Play(playForward);
            CachedImage.fillAmount = From;

            this.m_uiTweener = CachedImage.DOFillAmount(To, Duration);

            PlayInternal();
        }

        [ContextMenu("切换到To值状态")]
        public void SetCurrentValueToEnd()
        {
            CachedImage.fillAmount = To;
        }

        [ContextMenu("切换到From值状态")]
        public void SetCurrentValueToStart()
        {
            CachedImage.fillAmount = From;
        }

        [ContextMenu("设置当前值为To的值")]
        public void SetEndToCurrentValue()
        {
            To = CachedImage.fillAmount;
        }

        [ContextMenu("设置当前值为From的值")]
        public void SetStartToCurrentValue()
        {
            From = CachedImage.fillAmount;
        }

        public Image CachedImage
        {
            get
            {
                if (!(CachedMaskableGraphic is Image))
                    throw new System.Exception("can't find image component");

                return CachedMaskableGraphic as Image;
            }
        }

        protected override object TweenerStartValue
        {
            get
            {
                return this.From;
            }
        }

    }
}