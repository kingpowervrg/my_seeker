using System;
using UnityEngine;

namespace PostFX
{

    [Serializable]
    public abstract class PostEffectBase
    {
        protected Shader shader;
        protected Material material;

        [Tooltip("is Apply")]
        public bool IsApply;

        [HideInInspector]
        public EffectType et;

        [NonSerialized]
        public bool IsInitialized = false;

        public virtual void OnEnable()
        {
            CreateMaterial();
            IsInitialized = true;
        }

        public virtual void Update() { }

        /// <summary>
        /// 创建PostProcessing材质
        /// </summary>
        protected abstract void CreateMaterial();

        protected bool EarlyOutIfNotSupported(RenderTexture source, RenderTexture destination)
        {
            bool Supported = (SystemInfo.supportsImageEffects && material.shader.isSupported);
            if (!Supported)
            {
                Graphics.Blit(source, destination);
                return true;
            }
            return false;
        }
        public virtual void PreProcess(RenderTexture src, RenderTexture dst) { }

        protected Shader GetShader(string shaderName)
        {
#if RES_PROJECT
            return Shader.Find(shaderName);
#else
            return EngineCore.ShaderModule.Instance.GetShader(shaderName);
#endif
        }


        public virtual void OnDispose()
        {
            shader = null;
            material = null;
        }

        public virtual void ToParam(object[] o)
        {

        }
        public virtual void Refresh()
        {

        }
    }
}