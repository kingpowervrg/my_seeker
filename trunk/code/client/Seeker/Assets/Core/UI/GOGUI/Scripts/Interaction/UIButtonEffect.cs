/********************************************************************
	created:  2019-1-3 20:59:2
	filename: UIButtonEffect.cs
	author:	  songguangze@outlook.com
	
	purpose:  按钮点击动画效果
*********************************************************************/
using DG.Tweening;
using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace EngineCore
{
    public class UIButtonEffect : UIBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        public ButtonEffectType InteractiveEffectType = ButtonEffectType.ScaleEffect;

        //Scale Tweening Params
        public Vector3 OnPressedScale = Vector3.one;
        public Vector3 MaxSizeScale = Vector3.one;
        public float NormalToPressedScaleDuration = 0f;
        public float PressedToMaxSizeDuration = 0f;
        public float MaxSizeToNormalDuration = 0f;

        //Color Tweening Params
        public Color OnPressedColor = Color.white;
        public float NormalToPressedColorDuration = 0f;
        public float PressedToNormalColorDuration = 0f;

        //Rotation Tweening Params
        public Vector3 OnPressedRotation = Vector3.zero;
        public float NormalToPressedRotationDuration = 0f;
        public float PressedToNormalRotationDuration = 0f;

        //Position Tweening Params
        public Vector3 OnPressedPositionOffset = Vector3.zero;
        public float NormalToPressedOffsetDuration = 0f;
        public float PressedToNormalOffsetDuration = 0f;


        //Tweener for scale effect
        private Vector3 m_normalSize = Vector3.one;
        private Tween m_pressedScaleTweener = null;
        private Tween m_pressUpScaleTweener = null;

        //Tweener for rotation effect
        private Vector3 m_normalRoattion = Vector3.zero;
        private Tweener m_pressedRotationTweener = null;
        private Tweener m_pressUpRotationTweener = null;

        //Tweener for position offset effect
        private Vector3 m_normalPosition = Vector3.zero;
        private Tweener m_pressedPositionTweener = null;
        private Tweener m_pressedUpPositionTweener = null;

        //Tweener for color effect
        private Color m_normalColor = Color.white;
        private Graphic m_cachedGraphic = null;
        private Tween m_pressedColorTweener = null;
        private Tween m_pressUpColorTweener = null;

        private bool m_isDirty = true;

        public void OnPointerDown(PointerEventData eventData)
        {
            if ((InteractiveEffectType & ButtonEffectType.ScaleEffect) == ButtonEffectType.ScaleEffect)
            {
                if (m_isDirty)
                    RebuildScaleEffect();

                transform.localScale = m_normalSize;
                m_pressUpScaleTweener.Pause();
                m_pressedScaleTweener.Restart();
            }

            if ((InteractiveEffectType & ButtonEffectType.ColorEffect) == ButtonEffectType.ColorEffect)
            {
                if (m_isDirty)
                    RebuildColorEffect();

                m_cachedGraphic.color = m_normalColor;
                m_pressUpColorTweener.Pause();
                m_pressedColorTweener.Restart();
            }

            if ((InteractiveEffectType & ButtonEffectType.RotateEffect) == ButtonEffectType.RotateEffect)
            {
                if (m_isDirty)
                    RebuildRotationEffect();

                transform.localEulerAngles = m_normalRoattion;
                m_pressUpRotationTweener.Pause();
                m_pressedRotationTweener.Restart();
            }

            if ((InteractiveEffectType & ButtonEffectType.PositionEffect) == ButtonEffectType.PositionEffect)
            {
                if (m_isDirty)
                    RebuildPositionEffect();

                transform.localPosition = m_normalPosition;
                m_pressedUpPositionTweener.Pause();
                m_pressedPositionTweener.Restart();
            }

            m_isDirty = false;
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if ((InteractiveEffectType & ButtonEffectType.ScaleEffect) == ButtonEffectType.ScaleEffect)
            {
                if (m_isDirty)
                    RebuildScaleEffect();

                transform.localScale = OnPressedScale;
                m_pressedScaleTweener.Pause();
                m_pressUpScaleTweener.Restart();
            }

            if ((InteractiveEffectType & ButtonEffectType.ColorEffect) == ButtonEffectType.ColorEffect)
            {
                if (m_isDirty)
                    RebuildColorEffect();

                m_cachedGraphic.color = OnPressedColor;
                m_pressedColorTweener.Pause();
                m_pressUpColorTweener.Restart();
            }

            if ((InteractiveEffectType & ButtonEffectType.RotateEffect) == ButtonEffectType.RotateEffect)
            {
                if (m_isDirty)
                    RebuildRotationEffect();

                transform.localEulerAngles = OnPressedRotation;
                m_pressedRotationTweener.Pause();
                m_pressUpRotationTweener.Restart();
            }

            if ((InteractiveEffectType & ButtonEffectType.PositionEffect) == ButtonEffectType.PositionEffect)
            {
                if (m_isDirty)
                    RebuildPositionEffect();

                transform.localPosition = OnPressedPositionOffset;
                m_pressedPositionTweener.Pause();
                m_pressedUpPositionTweener.Restart();
            }

            m_isDirty = false;
        }
#if UNITY_EDITOR
        protected override void OnValidate()
        {
            m_isDirty = true;
        }
#endif

        private void RebuildScaleEffect()
        {
            m_normalSize = transform.localScale;

            if (m_pressedScaleTweener != null)
                m_pressedScaleTweener.Kill();

            if (m_pressUpScaleTweener != null)
                m_pressUpScaleTweener.Kill();

            m_pressedScaleTweener = transform.DOScale(OnPressedScale, NormalToPressedScaleDuration).SetAutoKill(false).Pause();

            Sequence sequence = DOTween.Sequence();
            Tween pressUpToMax = transform.DOScale(MaxSizeScale, PressedToMaxSizeDuration).SetAutoKill(false).Pause();
            Tween maxToNormal = transform.DOScale(m_normalSize, MaxSizeToNormalDuration).SetAutoKill(false).Pause();
            sequence.Append(pressUpToMax);
            sequence.Append(maxToNormal);
            sequence.SetAutoKill(false);
            sequence.Pause();
            m_pressUpScaleTweener = sequence;
        }

        private void RebuildRotationEffect()
        {
            m_normalRoattion = transform.localEulerAngles;

            if (m_pressedRotationTweener != null)
                m_pressedRotationTweener.Kill();

            if (m_pressUpRotationTweener != null)
                m_pressUpRotationTweener.Kill();

            m_pressedRotationTweener = transform.DOLocalRotate(OnPressedRotation, NormalToPressedRotationDuration).SetAutoKill(false).Pause();
            m_pressUpRotationTweener = transform.DOLocalRotate(m_normalRoattion, PressedToNormalRotationDuration).SetAutoKill(false).Pause();
        }

        private void RebuildPositionEffect()
        {
            m_normalPosition = transform.localPosition;

            if (m_pressedPositionTweener != null)
                m_pressedPositionTweener.Kill();

            if (m_pressedUpPositionTweener != null)
                m_pressedUpPositionTweener.Kill();

            m_pressedPositionTweener = transform.DOLocalMove(OnPressedPositionOffset, NormalToPressedOffsetDuration).SetAutoKill(false).Pause();
            m_pressedUpPositionTweener = transform.DOLocalMove(m_normalPosition, PressedToNormalOffsetDuration).SetAutoKill(false).Pause();
        }

        private void RebuildColorEffect()
        {
            if (m_cachedGraphic == null)
            {
                m_cachedGraphic = GetComponent<Graphic>();
                if (m_cachedGraphic == null)
                    m_cachedGraphic = GetComponentInChildren<Graphic>(true);

                if (m_cachedGraphic == null)
                    Debug.LogError($"no graphic component {gameObject.name}");
            }

            m_normalColor = m_cachedGraphic.color;

            if (m_pressedColorTweener != null)
                m_pressedColorTweener.Kill();

            if (m_pressUpColorTweener != null)
                m_pressUpColorTweener.Kill();

            m_pressedColorTweener = m_cachedGraphic.DOColor(OnPressedColor, NormalToPressedColorDuration).SetAutoKill(false).Pause();

            m_pressUpColorTweener = m_cachedGraphic.DOColor(m_normalColor, PressedToNormalColorDuration).SetAutoKill(false).Pause();
        }
    }
    [Flags]
    public enum ButtonEffectType
    {
        None = 0,
        ScaleEffect = 1,             //缩放效果
        ColorEffect = (1 << 1),      //颜色变化
        RotateEffect = (1 << 2),     //旋转变化
        PositionEffect = (1 << 3),   //位置变化
    }
}