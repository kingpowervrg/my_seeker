using System.Collections.Generic;
using UnityEngine;

namespace EngineCore
{
    [EngineCoreModule(EngineCore.ModuleType.SHADER_MODULE)]
    public class ShaderModule : AbstractModule
    {
        private Dictionary<string, Shader> m_shaderDict = new Dictionary<string, Shader>();

        private Dictionary<string, ShaderVariantCollection> m_shaderVariantCollectionDict = new Dictionary<string, ShaderVariantCollection>();

        private static ShaderModule m_instance;

        public ShaderModule()
        {
            AutoStart = true;
            m_instance = this;

            EngineCoreEvents.ShaderEvent.GetShaderByName = GetShader;
        }

        public void AddShaderBundle(Shader[] addShaders)
        {
            for (int i = 0; i < addShaders.Length; ++i)
            {
                if (!this.m_shaderDict.ContainsKey(addShaders[i].name))
                    this.m_shaderDict.Add(addShaders[i].name, addShaders[i]);
            }
        }

        public void AddShaderVariablesCollections(ShaderVariantCollection shaderVariantCollection, bool warmup = false)
        {
            if (!this.m_shaderVariantCollectionDict.ContainsKey(shaderVariantCollection.name))
            {
                this.m_shaderVariantCollectionDict.Add(shaderVariantCollection.name, shaderVariantCollection);
                if (warmup)
                    shaderVariantCollection.WarmUp();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="shaderVariantCollectionName"></param>
        /// <remarks>todo:貌似预加载preload后会自动 warmup</remarks>
        public void WarmupShaderVariablesCollection(string shaderVariantCollectionName)
        {
            if (this.m_shaderVariantCollectionDict.ContainsKey(shaderVariantCollectionName))
            {
                ShaderVariantCollection shaderCollection = this.m_shaderVariantCollectionDict[shaderVariantCollectionName];
                if (!shaderCollection.isWarmedUp)
                    shaderCollection.WarmUp();
            }
        }

        /// <summary>
        /// 获取Shader
        /// </summary>
        /// <param name="shaderName"></param>
        /// <returns></returns>
        /// <remarks>默认在第一次使用是WarmUp</remarks>
        public Shader GetShader(string shaderName)
        {
            Shader shader = this.m_shaderDict.ContainsKey(shaderName) ? this.m_shaderDict[shaderName] : Shader.Find(shaderName);

            if (shader != null)
                AddShader(shader);

            return shader;
        }

        public void AddShader(Shader shader)
        {
            if (!this.m_shaderDict.ContainsKey(shader.name))
                this.m_shaderDict.Add(shader.name, shader);
        }


        public static ShaderModule Instance
        {
            get { return m_instance; }
        }

    }

}

