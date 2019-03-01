/********************************************************************
	created:  2018-12-28 15:11:51
	filename: TweenScale.cs
	author:	  songguangze@outlook.com
	
	purpose:  UI缩放动画 
*********************************************************************/
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EngineCore
{
    public class TweenScale : TransformTweener
    {
        public Vector3 From = Vector3.one;

        public Vector3 To = Vector3.one;

        public override void Play(bool playForward)
        {
            base.Play(playForward);
            CachedTransfrom.localScale = From;
            this.m_uiTweener = CachedTransfrom.DOScale(To, Duration);

            PlayInternal();
        }

        /// <summary>
        ///  GameObject TweenScale
        /// </summary>
        /// <param name="tweenTarget"></param>
        /// <param name="to"></param>
        /// <param name="duration"></param>
        /// <param name="isWorldSpace"></param>
        /// <returns></returns>
        public static TweenScale BeginTween(GameObject tweenTarget, Vector3 to, float duration, bool isWorldSpace = false)
        {
            TweenScale tweenerOnTarget = tweenTarget.GetOrAddComponent<TweenScale>();
            tweenerOnTarget.DOPause();
            tweenerOnTarget.IsWorldSpace = isWorldSpace;
            tweenerOnTarget.SetStartToCurrentValue();
            tweenerOnTarget.Duration = duration;
            tweenerOnTarget.To = to;
            tweenerOnTarget.PlayForward();

            return tweenerOnTarget;
        }


        [ContextMenu("切换到To值状态")]
        public override void SetCurrentValueToEnd()
        {
            CachedTransfrom.localScale = To;
        }

        [ContextMenu("切换到From值状态")]
        public override void SetCurrentValueToStart()
        {
            CachedTransfrom.localScale = From;
        }

        [ContextMenu("设置当前值为To的值")]
        public override void SetEndToCurrentValue()
        {
            To = CachedTransfrom.localScale;
        }

        [ContextMenu("设置当前值为From的值")]
        public override void SetStartToCurrentValue()
        {
            From = CachedTransfrom.localScale;
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