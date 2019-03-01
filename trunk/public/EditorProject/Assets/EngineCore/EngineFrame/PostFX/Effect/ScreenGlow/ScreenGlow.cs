using System;
using UnityEngine;

namespace PostFX
{
    [Serializable]
    public class ScreenGlow : PostEffectBase
    {

        [Range(-1, 1), Tooltip("This is threshold")]
        public float threshold = 1f;

        [Tooltip("This is maskTex")]
        public Texture maskTex;

        public ScreenGlow()
        {
            et = EffectType.ScreenGlow;
        }

        public override void ToParam(object[] o)
        {
            if (o[0] != null)
                threshold = Convert.ToSingle(o[0]);
            if (o[1] != null)
                maskTex = (Texture)o[1];
        }
        public override void PreProcess(RenderTexture source, RenderTexture destination)
        {
            Screenglow(source, destination);
        }

        protected override void CreateMaterial()
        {
            if (material == null)
            {
                shader = GetShader("Shader Forge/ScreenGlow");
                if (shader != null)
                {
                    material = new Material(shader);
                }
            }
        }

        void Screenglow(RenderTexture source, RenderTexture destination)
        {
            if (material == null)
            {
                CreateMaterial();
            }
            if (material != null)// && mGlowEnable)
            {
                if (EarlyOutIfNotSupported(source, destination))
                {
                    return;
                }
                material.SetFloat("_Threshold", threshold);
                material.SetTexture("_MaskTex", maskTex);
                Graphics.Blit(source, destination, material);
            }
        }

    }
}