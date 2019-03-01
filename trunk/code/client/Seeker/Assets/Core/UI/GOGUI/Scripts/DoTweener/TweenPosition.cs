using DG.Tweening;
using UnityEngine;

namespace EngineCore
{
    [ExecuteInEditMode]
    public class TweenPosition : TransformTweener
    {
        public Vector3 From;
        public Vector3 To;

        public override void Play(bool playForward)
        {
            base.Play(playForward);

            if (IsWorldSpace)
            {
                CachedTransfrom.position = From;
                this.m_uiTweener = CachedTransfrom.DOMove(To, Duration);
            }
            else
            {
                CachedTransfrom.anchoredPosition3D = From;
                this.m_uiTweener = CachedTransfrom.DOAnchorPos3D(To, Duration);
            }
            PlayInternal();
        }

        /// <summary>
        ///  GameObject TweenPosition
        /// </summary>
        /// <param name="tweenTarget"></param>
        /// <param name="to"></param>
        /// <param name="duration"></param>
        /// <param name="isWorldSpace"></param>
        /// <returns></returns>
        public static TweenPosition BeginTween(GameObject tweenTarget, Vector3 to, float duration, bool isWorldSpace = false)
        {
            TweenPosition tweenerOnTarget = tweenTarget.GetOrAddComponent<TweenPosition>();
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
                CachedTransfrom.position = To;
            else
                CachedTransfrom.anchoredPosition3D = To;
        }

        [ContextMenu("切换到From值状态")]
        public override void SetCurrentValueToStart()
        {
            if (IsWorldSpace)
                CachedTransfrom.position = From;
            else
                CachedTransfrom.anchoredPosition3D = From;
        }

        [ContextMenu("设置当前值为To的值")]
        public override void SetEndToCurrentValue()
        {
            if (IsWorldSpace)
                To = CachedTransfrom.position;
            else
                To = CachedTransfrom.anchoredPosition3D;
        }

        [ContextMenu("设置当前值为From的值")]
        public override void SetStartToCurrentValue()
        {
            if (IsWorldSpace)
                From = CachedTransfrom.position;
            else
                From = CachedTransfrom.anchoredPosition3D;
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
