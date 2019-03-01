/********************************************************************
	created:  2018-4-9 20:53:14
	filename: SceneBlur.cs
	author:	  songguangze@outlook.com
	
	purpose:  场景虚化处理
*********************************************************************/
using PostFX;
using UnityEngine;

namespace SeekerGame
{
    public class SceneBlur
    {
        public const int DOWNLOAD_SAMPLE = 2;
        public const float BLUR_SIZE = 4f;
        public const int BLUR_ITERATIONS = 2;

        private PostEffectBehaviour m_cameraPostEffectBehaviour = null;

        private BlurWithMask m_gaussianBlurEffect = null;
        private bool m_gaussianBlurEffectNormalStatus = false;
        private int m_currentSceneBlurSetting_downloadSample = 0;
        private float m_currentSceneBlurSetting_blurSize = 0f;
        private int m_currentSceneBlurSetting_blurIterations = 0;

        private Texture m_originMaskTex = null;
        private bool m_originMaskable = false;

        private Texture m_replacementTextureTemp = null;

        public SceneBlur()
        {
            EngineCore.EngineCoreEvents.UIEvent.BlurUIBackground = (isBlur) =>
            {
                if (isBlur)
                {
                    if (ReplacementMaskTexture != null)
                        m_replacementTextureTemp = ReplacementMaskTexture;

                    ReplacementMaskTexture = null;
                    HandleBlurScene(true);
                }
                else
                {
                    if (this.m_replacementTextureTemp != null)
                    {
                        ReplacementMaskTexture = this.m_replacementTextureTemp;
                        HandleBlurScene(true);
                    }
                    else
                        HandleBlurScene(false);
                }
            };
        }

        private bool isFirst = false;
        public void HandleBlurScene(bool isBlur)
        {
            if (Camera.main == null)
                return;

            //todo 临时修改
            m_cameraPostEffectBehaviour = Camera.main.gameObject.GetComponent<PostEffectBehaviour>();

            m_gaussianBlurEffect = m_cameraPostEffectBehaviour.GetPostEffectsList().Find(postEffectBase => postEffectBase.et == EffectType.BlurWithMask) as BlurWithMask;
            if (!isFirst)
            {
                m_gaussianBlurEffectNormalStatus = m_gaussianBlurEffect.IsApply;
                isFirst = true;
            }
            //if (m_cameraPostEffectBehaviour == null)
            //{

            //}
            //Debug.Log("blur ==== " + isBlur + "  camera : " + m_cameraPostEffectBehaviour.name);
            if (isBlur)
            {
                //备份现有Blur的参数
                this.m_currentSceneBlurSetting_blurIterations = m_gaussianBlurEffect.blurIterations;
                this.m_currentSceneBlurSetting_blurSize = m_gaussianBlurEffect.blurSize;
                this.m_currentSceneBlurSetting_downloadSample = m_gaussianBlurEffect.downsample;
                //this.m_originMaskTex = m_gaussianBlurEffect.m_MaskTexture;
                this.m_originMaskable = m_gaussianBlurEffect.masked;

                m_gaussianBlurEffect.factor = 1f;
                m_gaussianBlurEffect.downsample = DOWNLOAD_SAMPLE;
                m_gaussianBlurEffect.blurIterations = BLUR_ITERATIONS;
                m_gaussianBlurEffect.blurSize = BLUR_SIZE;

                m_gaussianBlurEffect.IsApply = true;

                if (ReplacementMaskTexture != null)
                {
                    m_gaussianBlurEffect.masked = true;
                    m_gaussianBlurEffect.m_MaskTexture = ReplacementMaskTexture;
                    m_gaussianBlurEffect.blurIterations = GameConst.FOGGY_DENSITY;
                }
            }
            else
            {
                m_gaussianBlurEffect.downsample = this.m_currentSceneBlurSetting_downloadSample;
                m_gaussianBlurEffect.blurSize = this.m_currentSceneBlurSetting_blurSize;
                m_gaussianBlurEffect.blurIterations = this.m_currentSceneBlurSetting_blurIterations;
                m_gaussianBlurEffect.masked = this.m_originMaskable;
                //m_gaussianBlurEffect.m_MaskTexture = this.m_originMaskTex;

                m_gaussianBlurEffect.IsApply = m_gaussianBlurEffectNormalStatus;
            }
        }

        public Texture ReplacementMaskTexture
        {
            get;
            set;
        }

        public float Factor
        {
            get {
                return m_gaussianBlurEffect.factor;
            }
            set
            {
                m_gaussianBlurEffect.factor = value;
            }
        }

    }


}

