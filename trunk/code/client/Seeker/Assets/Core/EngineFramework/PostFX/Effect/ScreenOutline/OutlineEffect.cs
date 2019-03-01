using System;
using UnityEngine;
using UnityEngine.Rendering;

namespace PostFX
{
    [Serializable]
    public class OutlineEffect : PostEffectBase
    {
        [Range(0.1f, 1.0f)]
        public float sample = 0.25f;
        [Range(0.0f, 10.0f)]
        public float spread = 0.5f;
        [Range(0, 4)]
        public int iteration = 2;
        [Range(0.0f, 10.0f)]
        public float intensity = 1.0f;
        public Color color;

        public Renderer[] m_SrcRenders;

        public OutlineEffect()
        {
            et = EffectType.OutlineEffect;
        }

        private RenderTexture solidSilhouette;
        private Material m_BlurMaterial = null;
        private Material blurMaterial
        {
            get
            {
                if (m_BlurMaterial == null)
                {
                    m_BlurMaterial = new Material(GetShader("GOE/Outline/Blur"));
                    m_BlurMaterial.hideFlags = HideFlags.HideAndDontSave;
                }

                return m_BlurMaterial;
            }
        }

        private CommandBuffer m_RenderCommand;


        protected override void CreateMaterial()
        {
            if (material == null)
            {
                shader = GetShader("GOE/Outline/Outline");
                if (shader != null)
                {
                    material = new Material(shader);
                }
            }
        }
        public override void OnEnable()
        {
            m_RenderCommand = new CommandBuffer();
            m_RenderCommand.name = "Render Solid Color Silhouette";
            Refresh();
        }


        public override void Refresh()
        {
            m_RenderCommand.ClearRenderTarget(true, true, Color.clear);

            if (m_SrcRenders != null)
            {
                for (int i = 0; i < m_SrcRenders.Length; ++i)
                {
                    Material replaceMat = new Material(GetShader("GOE/Outline/OutlineReplace"));
                    replaceMat.SetColor("_Color", color);
                    m_RenderCommand.DrawRenderer(m_SrcRenders[i], replaceMat);
                }
            }
        }


        public override void ToParam(object[] o)
        {
            if (o[0] != null)
            {
                sample = Convert.ToSingle(o[0]);
            }
            if (o[1] != null)
            {
                spread = Convert.ToSingle(o[1]);
            }
            if (o[2] != null)
            {
                iteration = Convert.ToInt32(o[2]);
            }
            if (o[3] != null)
            {
                intensity = Convert.ToSingle(o[3]);
            }
            if (o[4] != null)
            {
                color = (Color)o[4];
            }
            if (o[5] != null)
            {
                m_SrcRenders = (Renderer[])o[5];
            }
        }


        public override void PreProcess(RenderTexture source, RenderTexture destination)
        {
            /*RenderTexture*/
            solidSilhouette = RenderTexturePool.Get(Screen.width, Screen.height);
            Graphics.SetRenderTarget(solidSilhouette);


            Graphics.ExecuteCommandBuffer(m_RenderCommand);

            int width = (int)(Screen.width * sample);
            int height = (int)(Screen.height * sample);
            RenderTexture mBlurSilhouette = RenderTexturePool.Get(width, height);
            Graphics.Blit(solidSilhouette, mBlurSilhouette);

            for (int i = 0; i < iteration; ++i)
            {
                RenderTexture blurTemp = RenderTexturePool.Get(width, height);
                float offset = spread * i + 0.5f;
                blurMaterial.SetVector("_Offset", new Vector4(offset / width, offset / height));
                Graphics.Blit(mBlurSilhouette, blurTemp, blurMaterial);
                RenderTexturePool.Release(mBlurSilhouette);
                mBlurSilhouette = blurTemp;
            }

            material.SetTexture("_StencilMap", solidSilhouette);
            material.SetTexture("_BlurMap", mBlurSilhouette);
            material.SetFloat("_Intensity", intensity);
            Graphics.Blit(source, destination, material);

            RenderTexturePool.Release(mBlurSilhouette);
            RenderTexturePool.Release(solidSilhouette);
        }

        public override void OnDispose()
        {
            base.OnDispose();
            if (m_RenderCommand != null)
            {
                m_RenderCommand.Clear();
            }
            m_BlurMaterial = null;
        }

    }
}