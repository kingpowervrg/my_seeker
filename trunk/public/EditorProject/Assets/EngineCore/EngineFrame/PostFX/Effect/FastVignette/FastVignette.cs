using System;
using UnityEngine;

namespace PostFX
{
    public enum ColorMode
    {
        Classic,
        Desaturate,
        Colored
    }
    [Serializable]
    public class FastVignette : PostEffectBase
    {
        [Tooltip("Vignette type.")]
        public ColorMode Mode = ColorMode.Classic;

        [ColorUsage(false), Tooltip("The color to use in the vignette area.")]
        public Color Color = Color.red;

        [Tooltip("Center point.")]
        public Vector2 Center = new Vector2(0.5f, 0.5f);

        [Range(-100f, 100f), Tooltip("Smoothness of the vignette effect.")]
        public float Sharpness = 10f;

        [Range(0f, 100f), Tooltip("Amount of vignetting on screen.")]
        public float Darkness = 30f;


        public FastVignette()
        {
            et = EffectType.FastVignette;
        }


        protected override void CreateMaterial()
        {
            if (base.material == null)
            {
                base.shader = GetShader("Hidden/postFX/FastVignette");
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
                Mode = (ColorMode)Convert.ToInt32(o[0]);
            }
            if (o[1] != null)
            {
                Color = (Color)(o[1]);
            }
            if (o[2] != null)
            {
                Center = (Vector2)(o[2]);
            }
            if (o[3] != null)
            {
                Sharpness = Convert.ToSingle(o[3]);
            }
            if (o[4] != null)
            {
                Darkness = Convert.ToSingle(o[4]);
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
                material.SetVector("_Params", new Vector4(Center.x, Center.y, Sharpness * 0.01f, Darkness * 0.02f));
                material.SetColor("_Color", Color);
                Graphics.Blit(source, destination, material, (int)Mode);
            }
        }


    }
}