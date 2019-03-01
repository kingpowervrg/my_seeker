using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace EngineCore
{
#if UNITY_EDITOR
    public
#else
    internal
#endif
 class WWWAudioGroup : ResTypeGroup
    {
        internal void GetAudioClip(string name, Action<string, AudioClip> func, LoadPriority priority)
        {
            if (name == null || name == string.Empty)
            {
                return;
            }

            if (HasLoaded(name))
            {
                AudioClip www = this.GetAudio(name);
                if (func != null)
                {
                    func(name, www);
                }
                return;
            }

#if UNITY_EDITOR
            UnityEngine.Object obj = LoadFromPrefab(name, typeof(AudioClip));
            if (obj != null)
            {
                SetAsset(name, obj).Reference = 1;
                if (func != null)
                    func(name, GameObject.Instantiate(obj) as AudioClip);
                return;
            }
#endif

            Resource res = this.GetDownloadResource(name);
            if (res == null)
            {
                res = this.CreateResource(name, priority);
                res.Loader.WWWType = WWWType.Audio;
                res.LoadRes();
            }

            res.AddGotAudioCallback(func);

            //逻辑加载时，提高优先级//
            if (res.Loader.Priority < priority)
            {
                this.ResourceMgr.GOELoaderMgr.SetLoaderPriority(res.Loader, priority);
            }
        }


        protected virtual void OnLoadAudio(string name, UnityEngine.Object obj)
        {
            AudioClip audioclip = obj as AudioClip;
            AssetInfo old;
            if (mDicAsset.TryGetValue(name, out old))
            {
                //Logger.GetFile(LogFile.Res).LogError(name + " already in mDicAudio");
            }
            Resource res = GetDownloadResource(name);
            AssetInfo asset = SetAsset(name, audioclip);
        }

        private AudioClip GetAudio(string name)
        {
            return base.GetAsset(name) as AudioClip;
        }

        protected override Resource CreateResource(string name, LoadPriority priority)
        {
            Resource res = base.CreateResource(name, priority);

            res.AddGotAudioCallback(this.OnLoadAudio);

            return res;
        }
    }
}
