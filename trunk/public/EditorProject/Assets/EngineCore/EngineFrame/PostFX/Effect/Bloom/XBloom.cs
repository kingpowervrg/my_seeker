using System;
using UnityEngine;

namespace PostFX
{
    [Serializable]
    public class XBloom : PostEffectBase
    {
        [Range(0f, 1f), Tooltip("This is mThreshhold")]
        public float Threshhold = 0f;

        [Range(0f, 2f), Tooltip("This is mIntensity")]
        public float Intensity = 0.3f;

        [Range(0.25f, 5.5f), Tooltip("This is blurSize")]
        public float blurSize = 1f;

        [Range(1, 2)]
        public int blurIterations = 1;

        const string strParameter = "_Parameter";
        const string strBloom = "_Bloom";

        public XBloom()
        {
            et = EffectType.Bloom;
        }


        protected override void CreateMaterial()
        {
            if (material == null)
            {
                shader = GetShader("Hidden/MobileBloom");
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
            if (material != null)
            {
                if (EarlyOutIfNotSupported(source, destination))
                {
                    return;
                }
                RenderTexture tempRtA = RenderTexturePool.Get(Screen.width / 4, Screen.height / 4, 0, RenderTextureFormat.ARGB32);
                RenderTexture tempRtB = RenderTexturePool.Get(Screen.width / 4, Screen.height / 4, 0, RenderTextureFormat.ARGB32);
                material.SetVector(strParameter, new Vector4(blurSize, 0.0f, Threshhold, Intensity));

                Graphics.Blit(source, tempRtA, material, 1);

                for (int i = 0; i < blurIterations; ++i)
                {
                    material.SetVector(strParameter, new Vector4(blurSize + i, 0.0f, Threshhold, Intensity));

                    Graphics.Blit(tempRtA, tempRtB, material, 2);
                    Graphics.Blit(tempRtB, tempRtA, material, 3);
                }

                material.SetTexture(strBloom, tempRtA);
                Graphics.Blit(source, destination, material, 0);

                RenderTexturePool.Release(tempRtA);
                RenderTexturePool.Release(tempRtB);
            }
        }


        public override void ToParam(object[] o)
        {
            if (o[0] != null)
                this.Threshhold = Convert.ToSingle(o[0]);

            if (o[1] != null)
                this.Intensity = Convert.ToSingle(o[1]);
        }

    }
}