/********************************************************************
	created:  2019-1-2 11:6:35
	filename: TweenColor.cs
	author:	  songguangze@outlook.com
	
	purpose:  Tween 颜色
*********************************************************************/
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace EngineCore
{
    //[RequireComponent(typeof(MaskableGraphic))]
    public class TweenColor : MaskableGraphicTweener
    {
        public Color From = Color.white;
        public Color To = Color.white;

        public override void Play(bool playForward)
        {
            base.Play(playForward);

            CachedMaskableGraphic.color = From;

            this.m_uiTweener = CachedMaskableGraphic.DOColor(To, Duration);
            PlayInternal();
        }


        [ContextMenu("切换到To值状态")]
        public void SetCurrentValueToEnd()
        {
            CachedMaskableGraphic.color = To;
        }

        [ContextMenu("切换到From值状态")]
        public void SetCurrentValueToStart()
        {
            CachedMaskableGraphic.color = From;
        }

        [ContextMenu("设置当前值为To的值")]
        public void SetEndToCurrentValue()
        {
            To = CachedMaskableGraphic.color;
        }

        [ContextMenu("设置当前值为From的值")]
        public void SetStartToCurrentValue()
        {
            From = CachedMaskableGraphic.color;
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