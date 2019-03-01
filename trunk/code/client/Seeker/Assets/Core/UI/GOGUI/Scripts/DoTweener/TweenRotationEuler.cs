/********************************************************************
	created:  2019-1-2 10:34:19
	filename: TweenRotationEuler.cs
	author:	  songguangze@outlook.com
	
	purpose:  Tween旋转
*********************************************************************/
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EngineCore
{
    public class TweenRotationEuler : TransformTweener
    {
        public Vector3 From = Vector3.zero;
        public Vector3 To = Vector3.zero;

        public override void Play(bool playForward)
        {
            base.Play(playForward);

            if (IsWorldSpace)
            {
                CachedTransfrom.eulerAngles = From;
                this.m_uiTweener = CachedTransfrom.DORotate(To, Duration);
            }
            else
            {
                CachedTransfrom.localEulerAngles = From;
                this.m_uiTweener = CachedTransfrom.DOLocalRotate(To, Duration, RotateMode.FastBeyond360);
            }
            PlayInternal();
        }

        /// <summary>
        ///  GameObject TweenRotation
        /// </summary>
        /// <param name="tweenTarget"></param>
        /// <param name="to"></param>
        /// <param name="duration"></param>
        /// <param name="isWorldSpace"></param>
        /// <returns></returns>
        public static TweenRotationEuler BeginTween(GameObject tweenTarget, Vector3 to, float duration, bool isWorldSpace = false)
        {
            TweenRotationEuler tweenerOnTarget = tweenTarget.GetOrAddComponent<TweenRotationEuler>();
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
            if (IsWorldSpace)
                CachedTransfrom.eulerAngles = To;
            else
                CachedTransfrom.localEulerAngles = To;
        }

        [ContextMenu("切换到From值状态")]
        public override void SetCurrentValueToStart()
        {
            if (IsWorldSpace)
                CachedTransfrom.eulerAngles = To;
            else
                CachedTransfrom.localEulerAngles = To;
        }

        [ContextMenu("设置当前值为To的值")]
        public override void SetEndToCurrentValue()
        {
            if (IsWorldSpace)
                To = CachedTransfrom.eulerAngles;
            else
                To = CachedTransfrom.localEulerAngles;
        }

        [ContextMenu("设置当前值为From的值")]
        public override void SetStartToCurrentValue()
        {
            if (IsWorldSpace)
                From = CachedTransfrom.eulerAngles;
            else
                From = CachedTransfrom.localEulerAngles;
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