using System;
using UnityEngine;

namespace PostFX
{
    [Serializable]
    public class FrameFadeout : PostEffectBase
    {
        //淡出时间
        public float FadeoutDuration = 5f;
        //开始过渡时的因子
        [Range(0f, 1f)]
        public float StartFadeFactor = 0f;
        public FadeOutEaseType FadeOutType = FadeOutEaseType.Linear;


        private float m_fadeElapsed = 0f;
        private float m_currentFadeFactor = 0f;
        private bool m_isRenderFadeTexture = false;


        public FrameFadeout()
        {
            et = EffectType.FrameFadeout;
        }

        public override void OnEnable()
        {
            this.m_currentFadeFactor = FadeoutDuration;
            this.m_isRenderFadeTexture = false;
        }

        public override void Update()
        {
            base.Update();
            if (IsApply)
                if (m_isRenderFadeTexture)
                {
                    m_fadeElapsed += Time.deltaTime;
                    m_currentFadeFactor = 1 - CalculateFadeOutFactor(m_fadeElapsed, 0, FadeoutDuration, StartFadeFactor, 1, FadeOutType);

                    if (m_currentFadeFactor <= 0 || m_currentFadeFactor > 1)
                    {
                        IsApply = false;
                        this.m_fadeElapsed = 0f;
                        this.m_isRenderFadeTexture = false;
                    }
                }
        }

        protected override void CreateMaterial()
        {
            if (base.material == null)
            {
                base.shader = GetShader("Hidden/FrameFadeout");
                if (base.shader != null)
                {
                    base.material = new Material(base.shader);
                }
            }
        }
        public override void ToParam(object[] o)
        {
            if (o[0] != null)
            {
                FadeoutDuration = Convert.ToSingle(o[0]);
            }
            if (o[1] != null)
            {
                StartFadeFactor = Convert.ToSingle(o[1]);
            }
            if (o[2] != null)
            {
                FadeOutType = (FadeOutEaseType)Convert.ToInt32(o[2]);
            }
        }

        public override void PreProcess(RenderTexture source, RenderTexture destination)
        {
            if (material == null)
            {
                CreateMaterial();
            }
            if (material != null)// && mGlowEnable)
            {
                RenderTexture m_rtFade = RenderTexturePool.Get(Screen.width, Screen.height, 0);
                if (EarlyOutIfNotSupported(source, destination))
                {
                    return;
                }
                if (m_isRenderFadeTexture)
                {
                    material.SetTexture("_FadeoutTexture", m_rtFade);
                    material.SetFloat("_FadeoutFactor", m_currentFadeFactor);
                    Graphics.Blit(source, destination, material);
                }
                else
                {
                    Graphics.Blit(source, m_rtFade);
                    Graphics.Blit(source, destination);
                    m_isRenderFadeTexture = true;
                }

                RenderTexturePool.Release(m_rtFade);
            }


        }

        private float CalculateFadeOutFactor(float currentValue, float iMin, float iMax, float oMin, float oMax, FadeOutEaseType easeType)
        {
            float finalValue = oMin + (currentValue - iMin) * (oMax - oMin) / (iMax - iMin);
            switch (easeType)
            {
                case FadeOutEaseType.easeInCubic:
                    finalValue = easeInCubic(oMin, oMax, finalValue);
                    break;
                case FadeOutEaseType.easeOutCubic:
                    finalValue = easeOutCubic(oMin, oMax, finalValue);
                    break;
                case FadeOutEaseType.easeOutExpo:
                    finalValue = easeOutExpo(oMin, oMax, finalValue);
                    break;
                case FadeOutEaseType.easeInExpo:
                    finalValue = easeInExpo(oMin, oMax, finalValue);
                    break;
                default:
                    break;

            }
            return finalValue;
        }


        private float easeInCubic(float start, float end, float value)
        {
            end -= start;
            return end * value * value * value + start;
        }

        private float easeOutCubic(float start, float end, float value)
        {
            value--;
            end -= start;
            return end * (value * value * value + 1) + start;
        }

        private static float easeInExpo(float start, float end, float value)
        {
            end -= start;
            return end * Mathf.Pow(2, 10 * (value / 1 - 1)) + start;
        }

        private static float easeOutExpo(float start, float end, float value)
        {
            end -= start;
            return end * (-Mathf.Pow(2, -10 * value / 1) + 1) + start;
        }

    }
    public enum FadeOutEaseType
    {
        Linear,
        easeInCubic,
        easeOutCubic,
        easeInExpo,
        easeOutExpo,

    }
}