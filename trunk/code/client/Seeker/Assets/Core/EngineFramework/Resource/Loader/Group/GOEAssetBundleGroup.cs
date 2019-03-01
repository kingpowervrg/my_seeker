using GOEngine;
using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace EngineCore
{
    public class AssetBundleGroup : ResTypeGroup
    {
        GOEAssetBundleCacheManager cacheMgr;

        public GOEAssetBundleCacheManager CacheManager { get { return cacheMgr; } }

        public AssetBundleGroup()
        {
            cacheMgr = new GOEAssetBundleCacheManager(this);
        }

        internal void EnsureDependencies(string name)
        {
            RuntimeBundleInfo assetBelongBundleInfo = ResourceMgr.GetBundleInfo(name);
            if (assetBelongBundleInfo == null)
            {
                Debug.LogError($"can't find asset {name} belong bundle");
                return;
            }

            for (int i = 0; i < assetBelongBundleInfo.BundleDependencyArray.Length; ++i)
            {
                string dependencyBundleName = assetBelongBundleInfo.BundleDependencyArray[i];
                if (!HasLoaded(dependencyBundleName))
                {
                    ResourceModule.Instance.PreloadBundle(dependencyBundleName, null);
                }
            }
        }

        internal void GetAsset(string name, Action<string, UnityEngine.Object> callback, LoadPriority priority = LoadPriority.Default)
        {
            if (string.IsNullOrEmpty(name))
                return;


            if (HasLoaded(name))
            {
                if (callback != null)
                {
                    GetAssetInCache(name, callback, priority);
                }
                return;
            }
            /*
            #if UNITY_EDITOR
                        UnityEngine.Object obj = LoadFromPrefab(name, typeof(UnityEngine.Object));
                        if (obj != null)
                        {
                            SetAsset(name, obj).Reference = 1;z
                            UnityEngine.Object ins = AssetInfo.IsNeedInstance(obj, name) ? GameObject.Instantiate(obj) : obj;
                            if (callback != null)
                                callback(name, ins);
                            return;
                        }
            #endif*/
            string bundleName = ResourceMgr.GetBundleName(name);
            if (bundleName == string.Empty)
            {
                DebugUtil.LogError("can not find asset: " + name);
                callback?.Invoke(name, null);           //update: guangze song: 资源没找到时，回调正常调用，防止有的资源(LazyImageLoader)状态还在加载中
                return;
            }

            cacheMgr.AddCallbackToDic(name, callback);
            BundleInfoResource cachedBundleInfo = cacheMgr.Cache[bundleName];
            if (cachedBundleInfo != null)
            {
                cacheMgr.DoEnqueuePending(bundleName, LoadPriority.Default, cachedBundleInfo.AssetBundle);

            }
            else
            {
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
        }

        private void PreloadBundleFromHttpServer(string assetName, Action<string, AssetBundle> callback, LoadPriority priority = LoadPriority.Default, bool dependencyResource = false)
        {
            BundleIndexItemInfo bundleInfo = null;
            if (SQLiteBundlemapLoader.Instance.RemoteBundleIndicesMap == null)
            {
                SQLiteBundlemapLoader.Instance.CheckUpdate((needUpdate) =>
                {
                    bundleInfo = SQLiteBundlemapLoader.Instance.RemoteBundleIndicesMap.GetBundleItemInfoByAssetName(assetName);
                    if (bundleInfo != null)
                    {
                        PreloadBundleFromHttpServer(assetName, callback, priority, dependencyResource);
                        return;
                    }
                }
                );
                return;
            }
            else
            {
                bundleInfo = SQLiteBundlemapLoader.Instance.RemoteBundleIndicesMap.GetBundleItemInfoByAssetName(assetName);
            }

            string bundlePath = SysConf.GAME_RES_URL + "/" + bundleInfo.BundleHashName;
            ResourceModule.Instance.DownloadFilesFromServer(new string[] { bundlePath }, (v, b) => PreloadBundle(assetName, callback, priority, dependencyResource), null, null, null, true);
        }

        internal ResourceState PreloadBundle(string assetName, Action<string, AssetBundle> callback, LoadPriority priority = LoadPriority.Default, bool dependencyResource = false)
        {
            BundleIndexItemInfo preloadBundleInfo = ResourceMgr.Instance.GetBundleInfo(assetName);
            string bundleName = preloadBundleInfo.BundleName;

            //add: guangze song     本地资源不存在
            if (preloadBundleInfo == null)
            {
                PreloadBundleFromHttpServer(bundleName, callback, priority, dependencyResource);
                return ResourceState.Wait;
            }

            //todo:add guangze song 

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
                cacheMgr.AddAssetBundleCallbackToDic(bundleName, callback);
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
                cacheMgr.AddAssetBundleCallbackToDic(bundleName, callback);
                if (res.Loader.Priority < priority)
                {
                    this.ResourceMgr.GOELoaderMgr.SetLoaderPriority(res.Loader, priority);
                }
            }
            return ResourceState.Wait;
        }

        /// <summary>
        /// 预加载资源
        /// </summary>
        /// <param name="assetName"></param>
        /// <param name="OnPreloadFinishCallback"></param>
        /// <returns></returns>
        internal ResourceState PreloadAsset(string assetName, Action<string, Object> OnPreloadFinishCallback)
        {
            AssetInfo assetInfo = cacheMgr.Cache.GetAssetInCache(assetName);
            if (assetInfo != null && assetInfo.AssetValid)
            {
                bool assetValid = assetInfo.GetAsset(OnPreloadFinishCallback);

                return ResourceState.OK;
            }

            string assetBelongBundleName = ResourceMgr.Instance.GetBundleName(assetName);
            BundleInfoResource cachedBundleInfo = cacheMgr.Cache[assetBelongBundleName];

            //Bundle已经加载，但没加载资源
            if (cachedBundleInfo != null)
            {
                cacheMgr.AddCallbackToDic(assetName, OnPreloadFinishCallback);

                cacheMgr.DoEnqueuePending(cachedBundleInfo.Name, LoadPriority.Default);

                return ResourceState.Wait;
            }
            else
            {
                PreloadBundle(assetBelongBundleName, (bundleName, assetBundle) =>
                {
                    PreloadAsset(assetName, OnPreloadFinishCallback);
                });
                return ResourceState.Wait;
            }
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

        //internal void CacheSceneBundleDependency(BundleInfo bundle)
        //{
        //    cacheMgr.Cache.CacheSceneBundleDependency(bundle);
        //}

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
