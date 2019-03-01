///////////////////////////////////////////
//  CameraFilterPack v2.0 - by VETASOFT 2015 ///
///////////////////////////////////////////
using System;
using UnityEngine;
namespace PostFX
{
    [Serializable]
    public class CameraFilterPack_Colors_Adjust_ColorRGB : PostEffectBase
    {
        private float TimeX = 1.0f;
        private Vector4 ScreenResolution;


        [Range(-2f, 2f)]
        public float Red = 0f;
        [Range(-2f, 2f)]
        public float Green = 0f;
        [Range(-2f, 2f)]
        public float Blue = 0f;
        [Range(-1f, 1f)]
        public float Brightness = 0f;

        public CameraFilterPack_Colors_Adjust_ColorRGB()
        {
            et = EffectType.Colors_Adjust_ColorRGB;
        }

        protected override void CreateMaterial()
        {
            if (base.material == null)
            {
                base.shader = GetShader("CameraFilterPack/Colors_Adjust_ColorRGB");
                if (base.shader != null)
                {
                    base.material = new Material(base.shader);
                }
            }
        }

        public override void ToParam(object[] o)
        {
            if (o[0] != null)
                Red = Convert.ToSingle(o[0]);
            if (o[1] != null)
                Green = Convert.ToSingle(o[1]);
            if (o[2] != null)
                Blue = Convert.ToSingle(o[2]);
            if (o[3] != null)
                Brightness = Convert.ToSingle(o[3]);
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
                material.SetFloat("_Value", Red);
                material.SetFloat("_Value2", Green);
                material.SetFloat("_Value3", Blue);
                material.SetFloat("_Value4", Brightness);
                material.SetVector("_ScreenResolution", new Vector4(Screen.width, Screen.height, 0.0f, 0.0f));
                Graphics.Blit(source, destination, material);

            }

        }

    }
}