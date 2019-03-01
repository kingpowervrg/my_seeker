using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace EngineCore
{
    internal class SceneBundleGroup : ResTypeGroup
    {
        private string mCurScene = string.Empty;
        private bool mIsAdditive = false;
        bool loading = false;
        Action getSceneCallback;
        public bool IsLoading
        {
            get
            {
                return loading;
            }
        }
        internal void GetScene(string name, Action callBack, LoadPriority priority = LoadPriority.Default,bool isAdditive = false)
        {
            mIsAdditive = isAdditive;

            string bundleName = this.ResourceMgr.GetBundleName(name);
            if (string.IsNullOrEmpty(bundleName))
                bundleName = this.ResourceMgr.GetBundleName(name + ".unity");
            if (bundleName == string.Empty)
            {
                Debug.LogError("can not find scene: " + name);
                return;
            }
            bool isCached = ResourceMgr.IsBundleCached(bundleName);
            if (/*GOERootCore.IsEditor ||*/ HasLoaded(name) || isCached)
            {
                if (mCurScene.ToLower() != name.ToLower())
                {
                    if (isCached)
                    {
                        var bundle = ResourceMgr.AssetBundleGroup.CacheManager.Cache[bundleName];
                        bundle.IsSceneBundle = true;//Set to active scene bundle
                        bundle.Touch();
                    }
                    LoadScene(name);
                    if (callBack != null)
                        callBack();
                    return;
                }
                else
                {
                    removeBundle(name);
                }
            }
            
            mCurScene = name;
            getSceneCallback = callBack;

            ResourceMgr.AssetBundleGroup.PreloadBundle(bundleName, OnLoadAssetBundle, LoadPriority.MostPrior, true);
            /*Resource res = this.GetDownloadResource(bundleName);
            if (res == null)
            {
                res = this.CreateResource(bundleName, priority);
                res.LoadRes();
            }

            //逻辑加载时，提高优先级//
            if (res.Loader.Priority < priority)
            {
                this.ResourceMgr.GOELoaderMgr.SetLoaderPriority(res.Loader, priority);
            }
            res.AddGotSceneCallback(callBack);*/
        }

        private void removeBundle(string name)
        {
            int index = cachedNames.IndexOf(name);
            if (index != -1)
            {
                cachedNames.RemoveAt(index);
                AssetBundle bundle = cachedBundles[index];                
                cachedBundles.RemoveAt(index);
                bundle.Unload(false);
            }
        }

        void OnLoadAssetBundle(string name, AssetBundle www)
        {
            if (www == null)
            {
                //Logger.GetFile(LogFile.Res).LogError(name + " assetBundel == null");
                return;
            }
            if (getSceneCallback != null)
            {
                getSceneCallback();
                getSceneCallback = null;
            }
            string sceneName = GetSceneName(name);
            var bundle = ResourceMgr.AssetBundleGroup.CacheManager.Cache[name];
            bundle.IsSceneBundle = true;
            bundle.BundleInfo.IsDependency = true;//Scene bundle's Assetbundle shouldn't be released
            bundle.Touch();
            
            //cachedBundles.Add(www);
            //cachedNames.Add(sceneName);
            //BundleInfo bundle = ResourceMgr.GetBundle(name);
            //ResourceMgr.CacheSceneBundleDependency(bundle);
            if (sceneName.ToLower() == mCurScene.ToLower())
            {
                this.LoadScene(mCurScene);
            }
        }

        private string GetSceneName(string bname)
        {
            return bname.Replace(".bundle", "");
        }

        private void LoadScene(string name)
        {
            mCurScene = name;
           
            if (mIsAdditive)
            {
                loading = false;
                UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(name, UnityEngine.SceneManagement.LoadSceneMode.Additive);
            }
            else
            {
                loading = true;
                UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(name, UnityEngine.SceneManagement.LoadSceneMode.Single);
            }
            UnityEngine.SceneManagement.SceneManager.sceneLoaded += SceneManager_sceneLoaded;
            if (PoolCount == 0)
                return;
            int index = cachedNames.IndexOf(name);
            if (index != cachedNames.Count - 1)
            {
                cachedNames.RemoveAt(index);
                cachedNames.Add(name);
                AssetBundle www = cachedBundles[index];
                cachedBundles.RemoveAt(index);
                cachedBundles.Add(www);
            }
        }

        private void SceneManager_sceneLoaded(UnityEngine.SceneManagement.Scene arg0, UnityEngine.SceneManagement.LoadSceneMode arg1)
        {
            UnityEngine.SceneManagement.SceneManager.sceneLoaded -= SceneManager_sceneLoaded;
            while (cachedBundles.Count > PoolCount)
            {
                cachedBundles[0].Unload(false);
                cachedBundles.RemoveAt(0);
                cachedNames.RemoveAt(0);
            }
            loading = false;
        }

        public void OnAdditiveLevelLoaded()
        {
            while (cachedBundles.Count > PoolCount)
            {
                cachedBundles[0].Unload(false);
                cachedBundles.RemoveAt(0);
                cachedNames.RemoveAt(0);
            }
        }

        private List<AssetBundle> cachedBundles = new List<AssetBundle>();
        private List<string> cachedNames = new List<string>();

        public int PoolCount = 2;

        protected override bool HasLoaded(string name)
        {
            if (cachedNames.Contains(name))
                return true;
            return false;
        }


        /*protected override Resource CreateResource(string name, LoadPriority priority)
        {
            Resource res = base.CreateResource(name, priority);

            res.AddGotBundleCallback(this.OnLoadAssetBundle);

            return res;
        }*/
    }
}
