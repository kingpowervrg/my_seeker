///////////////////////////////////////////
//  CameraFilterPack v2.0 - by VETASOFT 2015 ///
///////////////////////////////////////////
using System;
using UnityEngine;
namespace PostFX
{
    [Serializable]
    public class CameraFilterPack_FX_EarthQuake : PostEffectBase
    {

        private float TimeX = 1.0f;
        private Vector4 ScreenResolution;

        [Range(0f, 100f)]
        public float Speed = 15f;
        [Range(0f, 0.2f)]
        public float X = 0.008f;
        [Range(0f, 0.2f)]
        public float Y = 0.008f;
        [Range(0f, 1f)]
        private float Value4 = 1f;

        public CameraFilterPack_FX_EarthQuake()
        {
            et = EffectType.FX_EarthQuake;
        }

        protected override void CreateMaterial()
        {
            if (base.material == null)
            {
                base.shader = GetShader("CameraFilterPack/FX_EarthQuake");
                if (base.shader != null)
                {
                    base.material = new Material(base.shader);
                }
            }
        }


        public override void ToParam(object[] o)
        {
            if (o[0] != null)
                Speed = Convert.ToSingle(o[0]);
            if (o[1] != null)
                X = Convert.ToSingle(o[1]);
            if (o[2] != null)
                Y = Convert.ToSingle(o[2]);
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
                TimeX += Time.deltaTime;
                if (TimeX > 100) TimeX = 0;
                material.SetFloat("_TimeX", TimeX);
                material.SetFloat("_Value", Speed);
                material.SetFloat("_Value2", X);
                material.SetFloat("_Value3", Y);
                material.SetFloat("_Value4", Value4);
                material.SetVector("_ScreenResolution", new Vector4(Screen.width, Screen.height, 0.0f, 0.0f));
                Graphics.Blit(source, destination, material);
            }

        }

    }
}