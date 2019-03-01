// Colorful FX - Unity Asset
// Copyright (c) 2015 - Thomas Hourdel
// http://www.thomashourdel.com
using System;
using UnityEngine;

namespace PostFX
{
    public enum QualityPreset
    {
        Low = 4,
        Medium = 8,
        High = 12,
        Custom
    }
    [HelpURL("http://www.thomashourdel.com/colorful/doc/blur-effects/lens-distortion-blur.html")]
    [Serializable]
    public class LensDistortionBlur : PostEffectBase
    {
        [Tooltip("Quality preset. Higher means better quality but slower processing.")]
        public QualityPreset Quality = QualityPreset.Medium;

        [Range(2, 32), Tooltip("Sample count. Higher means better quality but slower processing.")]
        public int Samples = 10;

        [Range(-2f, 2f), Tooltip("Spherical distortion factor.")]
        public float Distortion = 0.2f;

        [Range(-2f, 2f), Tooltip("Cubic distortion factor.")]
        public float CubicDistortion = 0.6f;

        [Range(0.01f, 2f), Tooltip("Helps avoid screen streching on borders when working with heavy distortions.")]
        public float Scale = 0.8f;

        public LensDistortionBlur()
        {
            et = EffectType.LensDistortionBlur;
        }

        protected override void CreateMaterial()
        {
            if (material == null)
            {
                shader = GetShader("Hidden/Colorful/LensDistortionBlur");
                if (shader != null)
                {
                    material = new Material(shader);
                }
            }
        }


        public override void PreProcess(RenderTexture source, RenderTexture destination)
        {
            int samples = Quality == QualityPreset.Custom ? Samples : (int)Quality;
            material.SetVector("_Params", new Vector4(samples, Distortion / samples, CubicDistortion / samples, Scale));
            Graphics.Blit(source, destination, material);
        }

        public override void ToParam(object[] o)
        {
            if (o[0] != null)
                this.Quality = (QualityPreset)Convert.ToInt32(o[0]);

            if (o[1] != null)
                this.Samples = Convert.ToInt32(o[1]);
            if (o[2] != null)
                this.Distortion = Convert.ToSingle(o[2]);
            if (o[3] != null)
                this.CubicDistortion = Convert.ToSingle(o[3]);
            if (o[4] != null)
                this.Scale = Convert.ToSingle(o[4]);
        }


    }
}
