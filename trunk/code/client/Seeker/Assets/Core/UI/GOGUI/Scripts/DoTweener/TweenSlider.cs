/********************************************************************
	created:  2019-1-2 10:34:19
	filename: TweenSlider.cs
	author:	  songguangze@outlook.com
	
	purpose:  Tween 进度条
*********************************************************************/
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace EngineCore
{
    [RequireComponent(typeof(Slider))]
    public class TweenSlider : UITweenerBase
    {
        public float From = 0f;
        public float To = 1f;

        private Slider m_cachedSlider = null;

        protected override void Awake()
        {
            m_cachedSlider = GetComponent<Slider>();
        }

        public override void Play(bool playForward)
        {
            base.Play(playForward);

            CachedSilder.value = From;
            this.m_uiTweener = CachedSilder.DOValue(To, Duration);

            PlayInternal();
        }

        /// <summary>
        ///  GameObject TweenSlider
        /// </summary>
        /// <param name="tweenTarget"></param>
        /// <param name="to"></param>
        /// <param name="duration"></param>
        /// <param name="isWorldSpace"></param>
        /// <returns></returns>
        public static TweenSlider BeginTween(GameObject tweenTarget, float to, float duration)
        {
            TweenSlider tweenerOnTarget = tweenTarget.GetOrAddComponent<TweenSlider>();
            tweenerOnTarget.DOPause();
            tweenerOnTarget.Duration = duration;
            tweenerOnTarget.From = tweenerOnTarget.CachedSilder.value;
            tweenerOnTarget.To = to;
            tweenerOnTarget.PlayForward();

            return tweenerOnTarget;
        }

        public Slider CachedSilder
        {
            get
            {
                if (!this.m_cachedSlider)
                    Awake();

                return this.m_cachedSlider;
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