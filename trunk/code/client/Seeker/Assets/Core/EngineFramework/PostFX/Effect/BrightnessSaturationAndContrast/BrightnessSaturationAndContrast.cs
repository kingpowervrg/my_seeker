using System;
using UnityEngine;

namespace PostFX
{
    [Serializable]
    public class BrightnessSaturationAndContrast : PostEffectBase
    {
        [Range(0.0f, 3.0f)]
        public float brightness = 1.0f;

        [Range(0.0f, 3.0f)]
        public float saturation = 1.0f;

        [Range(0.0f, 3.0f)]
        public float contrast = 1.0f;

        public BrightnessSaturationAndContrast()
        {
            et = EffectType.BrightnessSaturationAndContrast;
        }

        protected override void CreateMaterial()
        {
            if (material == null)
            {
                shader = GetShader("Hidden/BrightnessSaturationAndContrast");
                if (shader != null)
                {
                    material = new Material(shader);
                }
            }
        }


        public override void PreProcess(RenderTexture source, RenderTexture destination)
        {
            if (material != null)
            {
                material.SetFloat("_Brightness", brightness);
                material.SetFloat("_Saturation", saturation);
                material.SetFloat("_Contrast", contrast);

                Graphics.Blit(source, destination, material);
            }
            else
            {
                Graphics.Blit(source, destination);
            }
        }

        public override void ToParam(object[] o)
        {
            if (o[0] != null)
                this.brightness = Convert.ToSingle(o[0]);

            if (o[1] != null)
                this.saturation = Convert.ToSingle(o[1]);
            if (o[2] != null)
                this.contrast = Convert.ToSingle(o[2]);
        }
    }
}