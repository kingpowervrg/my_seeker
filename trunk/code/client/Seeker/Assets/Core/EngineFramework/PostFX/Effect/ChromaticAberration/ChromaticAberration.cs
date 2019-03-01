// Colorful FX - Unity Asset
// Copyright (c) 2015 - Thomas Hourdel
// http://www.thomashourdel.com
using System;
using UnityEngine;
namespace PostFX
{

    [HelpURL("http://www.thomashourdel.com/colorful/doc/camera-effects/chromatic-aberration.html")]
    [Serializable]
    public class ChromaticAberration : PostEffectBase
    {
        [Range(0.9f, 1.1f), Tooltip("Indice of refraction for the red channel.")]
        public float RedRefraction = 1f;

        [Range(0.9f, 1.1f), Tooltip("Indice of refraction for the green channel.")]
        public float GreenRefraction = 1.005f;

        [Range(0.9f, 1.1f), Tooltip("Indice of refraction for the blue channel.")]
        public float BlueRefraction = 1.01f;

        [Tooltip("Enable this option if you need the effect to keep the alpha channel from the original render (some effects like Glow will need it). Disable it otherwise for better performances.")]
        public bool PreserveAlpha = false;

        public ChromaticAberration()
        {
            et = EffectType.ChromaticAberration;
        }


        protected override void CreateMaterial()
        {
            if (material == null)
            {
                shader = GetShader("Hidden/Colorful/Chromatic Aberration");
                if (shader != null)
                {
                    material = new Material(shader);
                }
            }
        }

        public override void PreProcess(RenderTexture source, RenderTexture destination)
        {
            material.SetVector("_Refraction", new Vector3(RedRefraction, GreenRefraction, BlueRefraction));
            Graphics.Blit(source, destination, material, PreserveAlpha ? 1 : 0);
        }
        public override void ToParam(object[] o)
        {
            if (o[0] != null)
                this.RedRefraction = Convert.ToSingle(o[0]);
            if (o[1] != null)
                this.GreenRefraction = Convert.ToInt32(o[1]);
            if (o[2] != null)
                this.BlueRefraction = Convert.ToSingle(o[2]);
            if (o[3] != null)
                this.PreserveAlpha = Convert.ToBoolean(o[3]);

        }

    }
}
