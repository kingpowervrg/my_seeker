using System;
using UnityEngine;

namespace PostFX
{
    [Serializable]
    public class ScreenFlip : PostEffectBase
    {
        public ScreenFlip()
        {
            et = EffectType.ScreenFlip;
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
                Graphics.Blit(source, destination, material);
            }
        }

        protected override void CreateMaterial()
        {
            if (material == null)
            {
                shader = GetShader("PostEffect/ScreenFlip");
                if (shader != null)
                {
                    material = new Material(shader);
                }
            }
        }
    }
}