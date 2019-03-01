using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace PostFX
{
    [Serializable]
    public class BlurOptimized : PostEffectBase
    {

        [Range(0, 2)]
        public int downsample = 1;

        [Range(0.0f, 10.0f)]
        public float blurSize = 3.0f;

        [Range(1, 4)]
        public int blurIterations = 2;

        [Range(0, 1)]
        public float fade = 1;

        public bool masked = false;

        public List<GameObject> m_maskGameObjectList = new List<GameObject>();

        private CommandBuffer m_renderMaskCommandBuffer = null;
        private RenderTexture m_blurMaskTexture = null;
        private bool m_lastMasked = false;

        public BlurOptimized()
        {
            et = EffectType.BlurOptimized;
        }

        protected override void CreateMaterial()
        {
            if (base.material == null)
            {
                base.shader = GetShader("Hidden/GOE/FastBlur");
                if (base.shader != null)
                {
                    base.material = new Material(base.shader);
                }
            }
        }


        public override void Update()
        {
            if (masked != m_lastMasked)
            {
                if (masked)
                    SetMaskGameObjects(this.m_maskGameObjectList);
            }

            m_lastMasked = masked;
        }

        public void SetMaskGameObjects(List<GameObject> maskGameObjectList)
        {
            if (m_renderMaskCommandBuffer == null)
            {
                m_renderMaskCommandBuffer = new CommandBuffer();
                m_renderMaskCommandBuffer.name = "Render Blur Mask Buffer";
            }

            this.m_maskGameObjectList = maskGameObjectList;
            if (this.m_maskGameObjectList == null || this.m_maskGameObjectList.Count == 0)
                masked = false;
            else
            {
                if (m_blurMaskTexture == null)
                    m_blurMaskTexture = new RenderTexture(Screen.width, Screen.height, 0);

                m_renderMaskCommandBuffer.ClearRenderTarget(true, true, Color.white);
                for (int i = 0; i < m_maskGameObjectList.Count; ++i)
                {
                    GameObject maskGameObject = m_maskGameObjectList[i];
                    Renderer[] maskGameObjectRenders = maskGameObject.GetComponentsInChildren<Renderer>();

                    for (int j = 0; j < maskGameObjectRenders.Length; ++j)
                        m_renderMaskCommandBuffer.DrawRenderer(maskGameObjectRenders[j], material, 0, 4);
                }
            }
        }


        public override void OnDispose()
        {
            if (material != null)
                GameObject.DestroyImmediate(material);

            GameObject.DestroyImmediate(m_blurMaskTexture);
            base.OnDispose();
        }

        public override void PreProcess(RenderTexture source, RenderTexture destination)
        {
            if (material == null)
                CreateMaterial();
            else
            {
                if (masked && m_maskGameObjectList.Count == 0)
                    masked = false;

                if (masked)
                {
                    Graphics.SetRenderTarget(m_blurMaskTexture);
                    Graphics.ExecuteCommandBuffer(m_renderMaskCommandBuffer);
                }

                float widthMod = 1.0f / (1.0f * (1 << downsample));

                material.SetVector("_Parameter", new Vector4(blurSize * widthMod, -blurSize * widthMod, 0.0f, 0.0f));
                source.filterMode = FilterMode.Bilinear;

                int rtW = source.width >> downsample;
                int rtH = source.height >> downsample;

                material.SetFloat("_Fade", fade);
                material.SetTexture("_OriginTex", source);
                if (masked)
                {
                    //Texture blendTex = Resources.Load<Texture>("Blom_Mask");
                    material.SetTexture("_BlendTex", m_blurMaskTexture);
                }
                else
                {
                    material.SetTexture("_BlendTex", null);
                }

                // downsample
                RenderTexture rt = RenderTexture.GetTemporary(rtW, rtH, 0, source.format);

                rt.filterMode = FilterMode.Bilinear;
                Graphics.Blit(source, rt, material, 0);

                var passOffs = 0;

                for (int i = 0; i < blurIterations; i++)
                {
                    float iterationOffs = (i * 1.0f);
                    material.SetVector("_Parameter", new Vector4(blurSize * widthMod + iterationOffs, -blurSize * widthMod - iterationOffs, 0.0f, 0.0f));

                    // vertical blur
                    RenderTexture rt2 = RenderTexture.GetTemporary(rtW, rtH, 0, source.format);
                    rt2.filterMode = FilterMode.Bilinear;
                    Graphics.Blit(rt, rt2, material, 1 + passOffs);
                    RenderTexture.ReleaseTemporary(rt);
                    rt = rt2;

                    // horizontal blur
                    rt2 = RenderTexture.GetTemporary(rtW, rtH, 0, source.format);
                    rt2.filterMode = FilterMode.Bilinear;
                    Graphics.Blit(rt, rt2, material, 2 + passOffs);
                    RenderTexture.ReleaseTemporary(rt);
                    rt = rt2;
                }

                Graphics.Blit(rt, destination, material, 3);

                RenderTexture.ReleaseTemporary(rt);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="o">[blur迭代次数,Blur大小，downsampler]</param>
        public override void ToParam(object[] o)
        {
            if (o[0] != null)
                blurIterations = Convert.ToInt32(o[0]);

            if (o[1] != null)
                blurSize = Convert.ToSingle(o[1]);

            if (o[2] != null)
                downsample = Convert.ToInt32(o[2]);

            if (o[3] != null)
                fade = Convert.ToSingle(o[3]);

            if (o[4] != null)
                masked = Convert.ToBoolean(o[4]);

            if (o[5] != null)
                SetMaskGameObjects(o[5] as List<GameObject>);
        }
    }
}
