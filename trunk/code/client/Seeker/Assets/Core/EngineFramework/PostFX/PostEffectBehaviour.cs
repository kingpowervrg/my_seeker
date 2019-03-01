using System.Collections.Generic;
using UnityEngine;


namespace PostFX
{
    [RequireComponent(typeof(Camera))]
    [ExecuteInEditMode]
    public class PostEffectBehaviour : MonoBehaviour
    {
        public PostEffectSettings postEffect = new PostEffectSettings();

        private List<PostEffectBase> peblist = null;
        // Use this for initialization
        public static Camera newCamera;

        void OnEnable()
        {
#if !RES_PROJECT
            //第一次使用后效系统时,预热所有后效Shader
            EngineCore.ShaderModule.Instance.WarmupShaderVariablesCollection("PostFXShaderCollection");
#endif

            newCamera = GetComponent<Camera>();
            peblist = postEffect.Initialization();
        }

        void Update()
        {
            foreach (var p in peblist)
                p.Update();
        }

        void OnRenderImage(RenderTexture source, RenderTexture destination)
        {
            RenderTexture buffer0 = RenderTexturePool.Get(Screen.width, Screen.height);
            Graphics.Blit(source, buffer0);

            foreach (var p in peblist)
            {
                if (!p.IsApply)
                    continue;

                if (!p.IsInitialized)
                {
                    p.OnEnable();
                    continue;
                }

                RenderTexture buffer1 = RenderTexturePool.Get(Screen.width, Screen.height);
                p.PreProcess(buffer0, buffer1);
                RenderTexturePool.Release(buffer0);
                buffer0 = buffer1;
            }
            Graphics.Blit(buffer0, destination);
            RenderTexturePool.Release(buffer0);
        }

        void OnDisable()
        {
            foreach (var p in peblist)
            {
                p.OnDispose();
            }
            RenderTexturePool.ReleaseAll();
        }

        void OnDestroy()
        {
            if (peblist != null)
            {
                peblist.Clear();
                peblist = null;
            }
            // postEffect = null;
        }


        public List<PostEffectBase> GetPostEffectsList()
        {
            return peblist;
        }

    }

}