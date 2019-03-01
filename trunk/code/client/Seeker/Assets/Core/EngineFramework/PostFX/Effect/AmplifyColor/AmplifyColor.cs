using System;
using UnityEngine;


namespace PostFX
{
    [Serializable]
    public class AmplifyColor : PostEffectBase
    {
        [Range(0.0f, 1.0f)]
        public float blendAmount = 0.0f;

        public Texture LutTexture = null;

        public AmplifyColor()
        {
            et = EffectType.AmplifyColor;

        }


        protected override void CreateMaterial()
        {
            if (material == null)
            {
                shader = GetShader("Hidden/AmplifyColor");
                if (shader != null)
                {
                    material = new Material(shader);
                }
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
                if (LutTexture == null)
                {
                    Graphics.Blit(source, destination);
                }
                else
                {
                    material.SetFloat("_LerpAmount", blendAmount);
                    material.SetTexture("_LerpRgbTex", LutTexture);

                    Graphics.Blit(source, destination, material, 0);
                }
            }
        }




        public override void ToParam(object[] o)
        {
            if (o[0] != null)
                this.blendAmount = Convert.ToSingle(o[0]);

            if (o[1] != null)
                this.LutTexture = (Texture)o[1];
        }

    }
}