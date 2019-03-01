///////////////////////////////////////////
//  CameraFilterPack v2.0 - by VETASOFT 2015 ///
///////////////////////////////////////////
using System;
using UnityEngine;
namespace PostFX
{
    [Serializable]
    public class CameraFilterPack_TV_WideScreenVertical : PostEffectBase
    {

        private float TimeX = 1.0f;
        private Vector4 ScreenResolution;

        [Range(0f, 0.8f)]
        public float Size = 0.55f;
        [Range(0.001f, 0.4f)]
        public float Smooth = 0.01f;
        [Range(0f, 10f)]
        private float StretchX = 1f;
        [Range(0f, 10f)]
        private float StretchY = 1f;

        public CameraFilterPack_TV_WideScreenVertical()
        {
            et = EffectType.TV_WideScreenVertical;
        }

        protected override void CreateMaterial()
        {
            if (base.material == null)
            {
                base.shader = GetShader("CameraFilterPack/TV_WideScreenVertical");
                if (base.shader != null)
                {
                    base.material = new Material(base.shader);
                }
            }
        }

        public override void ToParam(object[] o)
        {
            if (o[0] != null)
                Size = Convert.ToSingle(o[0]);
            if (o[1] != null)
                Smooth = Convert.ToSingle(o[1]);
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
                material.SetFloat("_Value", Size);
                material.SetFloat("_Value2", Smooth);
                material.SetFloat("_Value3", StretchX);
                material.SetFloat("_Value4", StretchY);
                material.SetVector("_ScreenResolution", new Vector4(Screen.width, Screen.height, 0.0f, 0.0f));
                Graphics.Blit(source, destination, material);
            }
        }

    }
}