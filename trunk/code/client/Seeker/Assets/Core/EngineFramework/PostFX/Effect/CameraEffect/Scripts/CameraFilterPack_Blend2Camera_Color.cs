///////////////////////////////////////////
//  CameraFilterPack v2.0 - by VETASOFT 2015 ///
///////////////////////////////////////////
using System;
using UnityEngine;

namespace PostFX
{

    [Serializable]
    public class CameraFilterPack_Blend2Camera_Color : PostEffectBase
    {
        private float TimeX = 1.0f;
        private RenderTexture Camera2tex;
        private Vector4 ScreenResolution;


        public Camera Camera2;
        [Range(0f, 1f)]
        public float SwitchCameraToCamera2 = 0f;
        [Range(0f, 1f)]
        public float BlendFX = 0.5f;

        public CameraFilterPack_Blend2Camera_Color()
        {
            et = EffectType.Blend2Camera_Color;
        }

        protected override void CreateMaterial()
        {
            if (base.material == null)
            {
                base.shader =GetShader("CameraFilterPack/Blend2Camera_Color");
                if (base.shader != null)
                {
                    base.material = new Material(base.shader);
                }
            }
        }

        public override void ToParam(object[] o)
        {
            if (o[0] != null)
                Camera2 = (Camera)o[0];
            if (o[1] != null)
                SwitchCameraToCamera2 = Convert.ToSingle(o[1]);
            if (o[2] != null)
                BlendFX = Convert.ToSingle(o[2]);
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
                RenderTexture Camera2tex = RenderTexturePool.Get(Screen.width, Screen.height, 24);
                if (Camera2 != null)
                {
                    Camera2.targetTexture = Camera2tex;
                    material.SetTexture("_MainTex2", Camera2tex);
                }
                material.SetFloat("_TimeX", TimeX);
                material.SetFloat("_Value", BlendFX);
                material.SetFloat("_Value2", SwitchCameraToCamera2);
                material.SetVector("_ScreenResolution", new Vector4(Screen.width, Screen.height, 0.0f, 0.0f));
                Graphics.Blit(source, destination, material);
                RenderTexturePool.Release(Camera2tex);
            }
        }

    }
}