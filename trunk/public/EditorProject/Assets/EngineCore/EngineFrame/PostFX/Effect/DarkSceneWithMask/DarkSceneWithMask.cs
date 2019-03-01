using System;
using UnityEngine;

namespace PostFX
{
    [Serializable]
    public class DarkSceneWithMask : PostEffectBase
    {

        [Tooltip("This is maskTex")]
        public Texture maskTex;

        [Range(0,1)]
        public float factor = 1;

        public DarkSceneWithMask()
        {
            et = EffectType.DarkSceneWithMask;
        }

        public override void ToParam(object[] o)
        {
            if (o[0] != null)
                maskTex = (Texture)o[1];
        }

        public override void PreProcess(RenderTexture source, RenderTexture destination)
        {
            material.SetTexture("_MaskTex", maskTex);
            material.SetFloat("_factor", factor);
            Graphics.Blit(source, destination, material);
        }

        protected override void CreateMaterial()
        {
            if (material == null)
            {
                shader = GetShader("Hidden/PostFx/BlendWithMask");
                if (shader != null)
                {
                    material = new Material(shader);
                }
            }
        }

    }
}