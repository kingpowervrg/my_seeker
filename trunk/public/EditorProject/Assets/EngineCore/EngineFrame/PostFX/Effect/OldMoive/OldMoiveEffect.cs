using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace PostFX
{
    [Serializable]
    public class OldMoiveEffect : PostEffectBase
    {
        //public Texture maskTex;
        public Texture snowTex;

        private float randomValue = 1f;
        public float snowValueX = 1f;
        public float snowValueY = 1f;

        [Range(0, 1)]
        public float snowPower = 1f;
        public float minBrightValue = 0.8f;
        public float maxBrightValue = 1.5f;
        private float randomBrightValue = 1f;
        [Range(0, 1)]
        public float saturation = 1f;

        public OldMoiveEffect()
        {
            et = EffectType.OldMoive;
        }

        protected override void CreateMaterial()
        {
            if (material == null)
            {
                shader = GetShader("Hidden/OldMoive");
                if (shader != null)
                {
                    material = new Material(shader);
                }
                if (snowTex != null) material.SetTexture("_SnowTex", snowTex);
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
                if (EarlyOutIfNotSupported(source, destination))
                {
                    return;
                }
                //TODO:
                if (snowTex == null)
                {
                    Graphics.Blit(source, destination);
                }
                else
                {
                    material.SetFloat("_RandomValue", randomValue);
                    material.SetFloat("_SnowValueX", snowValueX);
                    material.SetFloat("_SnowValueY", snowValueY);
                    material.SetFloat("_SnowPower", snowPower);
                    material.SetFloat("_Saturation", saturation);
                    material.SetFloat("_RandomBrightValue", randomBrightValue);

                    Graphics.Blit(source, destination, material, 0);
                }
            }
        }

        public override void Update()
        {
            base.Update();
            randomValue = UnityEngine.Random.Range(-1f, 1f);
            randomBrightValue = UnityEngine.Random.Range(minBrightValue, maxBrightValue);
        }

    }

}

