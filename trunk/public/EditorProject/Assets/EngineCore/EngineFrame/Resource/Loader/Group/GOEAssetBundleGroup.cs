using GOEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace EngineCore
{
    internal class AssetBundleGroup : ResTypeGroup
    {
        GOEAssetBundleCacheManager cacheMgr;

        public GOEAssetBundleCacheManager CacheManager { get { return cacheMgr; } }

        public AssetBundleGroup()
        {
            cacheMgr = new GOEAssetBundleCacheManager(this);
        }
        internal void EnsureDependencies(string name)
        {
            string bundleName = ResourceMgr.GetBundleName(name);
            if (!string.IsNullOrEmpty(bundleName))
            {
                BundleInfo bundle = ResourceMgr.GetBundle(bundleName);
                if (bundle != null)
                {
                    foreach (var dep in bundle.DependsOn)
                    {
                        BundleInfo depBundle = ResourceMgr.GetBundle(dep);
                        if (depBundle == null)
                            continue;
                        if (!HasLoaded(depBundle.FirstAsset))
                        {
                            ResourceModule.Instance.PreloadBundle(dep, null);
                        }
                    }
                }
            }
        }

        internal void GetAsset(string name, Action<string, UnityEngine.Object> callback, LoadPriority priority = LoadPriority.Default)
        {
            if (name == null || name == string.Empty)
            {
                return;
            }

            if (HasLoaded(name))
            {
                if (callback != null)
                {
                    GetAssetInCache(name, callback, priority);
                }
                return;
            }
#if UNITY_EDITOR
            UnityEngine.Object obj = LoadFromPrefab(name, typeof(UnityEngine.Object));
            if (obj != null)
            {
                SetAsset(name, obj).Reference = 1;
                UnityEngine.Object ins = AssetInfo.IsNeedInstance(obj, name) ? GameObject.Instantiate(obj) : obj;
                if (callback != null)
                    callback(name, ins);
                return;
            }
#endif
            string bundleName = ResourceMgr.GetBundleName(name);
            if (bundleName == string.Empty)
            {
                DebugUtil.LogError("can not find asset: " + name);
                return;
            }

            cacheMgr.AddCallbackToDic(name, callback);
            if (!cacheMgr.HasLoadingInfo(bundleName))
            {
                Resource res = this.GetDownloadResource(bundleName);
                if (res == null)
                {
                    res = this.CreateResource(bundleName, priority);
                    res.LoadRes();
                }
                if (res.InvalidBundle)
                {
                    res.InvalidBundle = false;
                }
                //逻辑加载时，提高优先级//
                if (res.Loader.Priority < priority)
                {
                    this.ResourceMgr.GOELoaderMgr.SetLoaderPriority(res.Loader, priority);
                }
            }
            else
                cacheMgr.CheckAssetLoading(bundleName, name, priority);
        }

        internal ResourceState PreloadBundle(string bundleName, Action<string, AssetBundle> callback, LoadPriority priority = LoadPriority.Default, bool dependencyResource = false)
        {
            BundleInfo bundle = ResourceMgr.Instance().GetBundle(bundleName);
            if (bundle == null)
            {
                var bn = ResourceMgr.GetBundleName(bundleName);
                if (string.IsNullOrEmpty(bn))
                {
                    DebugUtil.LogError("Cannot find asset:" + bundleName);
                    return ResourceState.Failed;
                }
                bundleName = bn;
            }
            var cb = cacheMgr.Cache[bundleName];
            if (cb != null)
            {
                if (callback != null)
                {
                    callback(bundleName, cb.AssetBundle);
                }
                return ResourceState.OK;
            }
            Resource res = this.GetDownloadResource(bundleName);
            if (res == null)
            {
                if (!cacheMgr.HasLoadingInfo(bundleName))
                {
                    res = this.CreateResource(bundleName, priority);
                    res.DependencyResource = dependencyResource;
                    res.LoadRes();
                }
                cacheMgr.AddCallbackToDic(bundleName, callback);
            }
            else if (res.ResOK)
            {
                if (callback != null)
                {
                    callback(bundleName, res.Loader.AssetBundle);
                }
                return ResourceState.OK;
            }
            else
            {
                cacheMgr.AddCallbackToDic(bundleName, callback);
                if (res.Loader.Priority < priority)
                {
                    this.ResourceMgr.GOELoaderMgr.SetLoaderPriority(res.Loader, priority);
                }
            }
            return ResourceState.Wait;
        }
        
        internal override void Update()
        {
            if (!ResourceMgr.IsLoadingScene)
                cacheMgr.Update();
        }

        protected override bool HasLoaded(string name)
        {
            return cacheMgr.Cache.HasLoadedAsset(name);
        }
        private void GetAssetInCache(string name, Action<string, UnityEngine.Object> callback, LoadPriority priority)
        {
            cacheMgr.GetAssetInCache(name, callback, priority);
        }        
        
        internal override void ReleaseAssetbundle()
        {
            cacheMgr.ReleaseAssetbundleOnLevelLoad();
        }

        internal override void SetAssetReleasePriority(string name, int _Priority, UnityEngine.Object obj)
        {
            cacheMgr.SetAssetReleasePriority(name, _Priority, obj);
        }
        

        protected virtual void OnLoadAssetBundle(string name, AssetBundle bundle, bool dependencyResource, LoadPriority priority)
        {
            cacheMgr.OnLoadAssetBundle(name, bundle, dependencyResource, priority);
        }

        internal override bool ReleaseAssetReference(string name, UnityEngine.Object obj)
        {

            return cacheMgr.Cache.ReleaseAssetReference(name, obj);
        }
        internal override AssetInfo SetAsset(string name, UnityEngine.Object obj, string bundleName = null)
        {
            AssetInfo asset = new AssetInfo(obj, name, bundleName);
            cacheMgr.Cache.AddAsset(name, asset);
            return asset;
        }

        internal int PendingBundleCount { get { return cacheMgr.PendingBundles; } }

        internal int MaximalInstantiationPerFrame
        {
            get
            {
                return cacheMgr.MaximalInstantiationPerFrame;
            }
            set
            {
                cacheMgr.MaximalInstantiationPerFrame = value;
            }
        }

        internal void CacheSceneBundleDependency(BundleInfo bundle)
        {
            cacheMgr.Cache.CacheSceneBundleDependency(bundle);
        }

        internal bool IsBundleCached(string name)
        {
            return cacheMgr.Cache[name] != null;
        }
        internal void ReleaseAssetCallback(string name, Action<string, UnityEngine.Object> func)
        {
            cacheMgr.RemoveAssetLoadCallback(name, func);
        }

        internal override bool RemoveAsset(string name, bool force = true, bool removeInBundleAsset = true)
        {
            string bundleName = ResourceMgr.GetBundleName(name);
            BundleInfoResource bundleResource = cacheMgr.Cache[bundleName];
            if (bundleResource != null)
            {
                return bundleResource.RemoveAsset(name, force, removeInBundleAsset);
            }

            return false;
        }

        protected override Resource CreateResource(string name, LoadPriority priority)
        {
            Resource res = base.CreateResource(name, priority);
#if DEBUG_BUNDLE_CACHE
            ResourceMgr.Instance().SampleLoadEvent(name, 1);
#endif                  
            res.AddGotBundleCallback(this.OnLoadAssetBundle);

            return res;

        }

        public int GetCacheSize()
        {
            return cacheMgr.Cache.CachedSize;
        }

        public int GetPreferedCacheSize()
        {
            return cacheMgr.Cache.PreferedCacheSize;
        }

        public void SetPreferedCacheSize(int size)
        {
            cacheMgr.Cache.PreferedCacheSize = size;
        }

        public float RegularGCFactor
        {
            get
            {
                return cacheMgr.Cache.RegularGCFactor;
            }
            set
            {
                cacheMgr.Cache.RegularGCFactor = value;
            }
        }

        public float SceneGCFactor
        {
            get
            {
                return cacheMgr.Cache.SceneGCFactor;
            }
            set
            {
                cacheMgr.Cache.SceneGCFactor = value;
            }
        }

        public int GetCacheCount()
        {
            return cacheMgr.Cache.CachedBundleCount;
        }

        public List<string> DumpCacheInfo()
        {
            return cacheMgr.Cache.DumpCacheInfo();
        }

        public void ForceGCCache()
        {
            cacheMgr.Cache.ForceGC();
        }

        public void TryGCCache()
        {
            cacheMgr.Cache.TryGC();
        }
    }
}
