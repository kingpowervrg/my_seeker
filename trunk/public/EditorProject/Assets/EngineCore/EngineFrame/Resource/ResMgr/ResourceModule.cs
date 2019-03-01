using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


namespace EngineCore
{
    /// <summary>
    /// 资源核心类
    /// </summary>
    [GameModule(EngineCore.ModuleType.RESOURCE_MODULE)]
    public class ResourceModule : AbstractModule, IProjectResource
    {
        public Dictionary<string, BundleInfo> mBundleFileList = new Dictionary<string, BundleInfo>();
        public Action<string, Action<string>> DoPreLoadActAsset;
        private ResourceMgr resourceMgr;

        private static ResourceModule m_instance;

        private ResourceModule()
        {
            m_instance = this;
        }

        public override void Start()
        {
            resourceMgr = ResourceMgr.Instance();
            resourceMgr.Start();
        }

        private void RegisterInternelEvents()
        {
            EngineCoreEvents.EngineEvent.GetAssetEvent += GetAsset;
            EngineCoreEvents.EngineEvent.ReleaseAssetEvent += ReleaseAsset;
            EngineCoreEvents.EngineEvent.RemoveAssetEvent += RemoveAsset;
            EngineCoreEvents.EngineEvent.RemoveAllAssetEvent += RemoveAllAsset;
            EngineCoreEvents.EngineEvent.TryGCCacheEvent += TryGCCache;
        }


        internal virtual void Shutdown()
        {

        }

        internal bool IsReady
        {
            get { return resourceMgr.HasInitialized; }
        }

        public override void Update()
        {
            resourceMgr.Update();
        }

        internal void OnEnterScene()
        {
        }

        /// <summary>
        /// 切换场景前清理资源
        /// </summary>
        public void OnLeaveScene()
        {
            resourceMgr.OnLeaveScene();
        }

        public int MaxLoaderCount
        {
            set { resourceMgr.GOELoaderMgr.MaxLoader = value; }
        }

        public int ScenePoolCount
        {
            set { resourceMgr.SceneBundleGroup.PoolCount = value; }
        }

        public int LoadMaxTime
        {
            set { ResourceLoader.MaxLoadTime = value; }
        }

        public string GetBundleName(string assetname)
        {
            return resourceMgr.GetBundleName(assetname);
        }


        /// <summary>
        /// 从assetbundle中加载asset
        /// </summary>
        /// <param name="name"></param>
        /// <param name="callback"></param>
        /// <param name="priority"></param>
        public virtual void GetAsset(string name, Action<string, UnityEngine.Object> callback, LoadPriority priority = LoadPriority.Default)
        {
            resourceMgr.AssetBundleGroup.GetAsset(name, callback, priority);
        }

        public virtual void EnsureDependencies(string name)
        {
            resourceMgr.AssetBundleGroup.EnsureDependencies(name);
        }

        /// <summary>
        /// 预加载动作资源
        /// </summary>
        /// <param name="name">动作资源名</param>
        /// <param name="callBack">回掉</param>
        public virtual void PreLoadActAsset(string name, Action<string> callBack = null)
        {
            if (DoPreLoadActAsset != null)
                DoPreLoadActAsset(name, callBack);
        }

        /// <summary>
        /// 预加载assetbundle
        /// </summary>
        /// <param name="name"></param>
        /// <param name="callback"></param>
        /// <param name="priority"></param>
        public virtual void PreloadBundle(string name, Action<string, AssetBundle> callback, LoadPriority priority = LoadPriority.Default, bool dependencyResouece = false)
        {
            resourceMgr.AssetBundleGroup.PreloadBundle(name, callback, priority, dependencyResouece);
        }

        public virtual void SetProgress(string[] bundleNames, string[] assetNames, Action<float> handler, Action onEnd = null)
        {
            resourceMgr.SetProgress(bundleNames, assetNames, handler, onEnd);
        }


        public virtual void SetProgress(string[] wwwNames, Action<float> handler, Action onEnd = null)
        {
            resourceMgr.SetProgress(wwwNames, handler, onEnd);
        }

        public virtual void RegisterAsset(string name, UnityEngine.Object obj)
        {
            resourceMgr.AssetBundleGroup.SetAsset(name, obj);
        }

        public virtual void GetScene(string name, Action callBack = null, LoadPriority priority = LoadPriority.MostPrior)
        {
            resourceMgr.SceneBundleGroup.GetScene(name, callBack, priority);
        }

        /// <summary>
        ///  加载Addivie场景
        /// </summary>
        /// <param name="name"></param>
        /// <param name="callBack"></param>
        /// <param name="priority"></param>
        public virtual void GetAdditiveScene(string name, Action callBack = null, LoadPriority priority = LoadPriority.MostPrior)
        {
            resourceMgr.SceneBundleGroup.GetScene(name, callBack, priority, true);
        }

        /// <summary>
        ///  卸载Additive 场景
        /// </summary>
        /// <param name="name"></param>
        public void UnLoadAdditiveScene(string name)
        {
            SceneManager.UnloadSceneAsync(name);
        }

        /// <summary>
        /// 获取当前主场景名
        /// </summary>
        /// <returns></returns>
        public string GetCurLevelName()
        {
            Scene curScene = SceneManager.GetActiveScene();
            return curScene.name;
        }

        internal void OnAdditiveLevelLoaded()
        {
            resourceMgr.SceneBundleGroup.OnAdditiveLevelLoaded();
        }

        /// <summary>
        /// 从远程服务器上加载文件，用于热更新
        /// </summary>
        /// <param name="name"></param>
        /// <param name="callback"></param>
        internal void getWWWFromServer(string name, Action<string, WWW> callback)
        {
            resourceMgr.WWWFileGroup.getWWWFileFromServer(name, callback);
        }
        /// <summary>
        /// 客户端同意从远程服务器上加载文件，用于热更新
        /// </summary>
        public void GoGetWWWFromServer()
        {
            resourceMgr.GogetWWWFromServer();
        }


        /// <summary>
        /// 从本地加载二进制文件
        /// </summary>
        /// <param name="name"></param>
        /// <param name="callback"></param>
        public void GetBytes(string name, Action<string, byte[]> callback, bool fromStream = false)
        {
            resourceMgr.WWWFileGroup.GetByteFile(name, callback, fromStream);
        }

        /// <summary>
        /// 从文本文件中读取字符串
        /// </summary>
        /// <param name="name"></param>
        /// <param name="callback"></param>
        public void GetText(string name, Action<string, string> callback, bool fromStream = false)
        {
            resourceMgr.WWWFileGroup.GetTextFile(name, callback, fromStream);
        }

        /// <summary>
        /// 从assetbundle中读取字符串
        /// </summary>
        /// <param name="name"></param>
        /// <param name="callback"></param>
        /// <returns></returns>
        public virtual void GetString(string name, Action<string, string> callback)
        {
            resourceMgr.GOEStringDelegate.GetString(name, callback);
        }

        public void LoadBundleMap(Action onLoad)
        {
            resourceMgr.LoadBundleMap(onLoad);
        }

        public void ReleaseAsset(string name, UnityEngine.Object obj)
        {
            resourceMgr.ReleaseAsset(name, obj);
        }

        public void RemoveAsset(string name, bool force = true, bool removeInBundleAsset = true)
        {
            resourceMgr.RemoveAsset(name, force, removeInBundleAsset);
        }

        public void RemoveAllAsset()
        {
            resourceMgr.RemoveAllAsset();
        }

        public void SetAssetReleasePriority(string name, int _Priority, UnityEngine.Object obj = null)
        {
            resourceMgr.SetAssetReleasePriority(name, _Priority, obj);
        }

        public void ReleaseAssetCallback(string name, Action<string, UnityEngine.Object> func)
        {
            resourceMgr.ReleaseAssetCallback(name, func);
        }


        public int GetCurrentCacheSize()
        {
            return resourceMgr.AssetBundleGroup.GetCacheSize();
        }

        public int GetCacheCount()
        {
            return resourceMgr.AssetBundleGroup.GetCacheCount();
        }

        public int GetPreferedCacheSize()
        {
            return resourceMgr.AssetBundleGroup.GetPreferedCacheSize();
        }

        public void SetPreferedCacheSize(int size)
        {
            resourceMgr.AssetBundleGroup.SetPreferedCacheSize(size);
        }

        /// <summary>
        /// 常规GC量
        /// </summary>
        public float RegularGCFactor
        {
            get
            {
                return resourceMgr.AssetBundleGroup.RegularGCFactor;
            }
            set
            {
                resourceMgr.AssetBundleGroup.RegularGCFactor = value;
            }
        }
        /// <summary>
        /// 切场景GC量
        /// </summary>
        public float SceneGCFactor
        {
            get
            {
                return resourceMgr.AssetBundleGroup.SceneGCFactor;
            }
            set
            {
                resourceMgr.AssetBundleGroup.SceneGCFactor = value;
            }
        }

        public void SetLowAsyncLoadPriority(bool value)
        {
            resourceMgr.LowAsyncLoadPriority = value;
            if (value)
            {
                Application.backgroundLoadingPriority = ThreadPriority.Low;
            }
            else
            {
                Application.backgroundLoadingPriority = ThreadPriority.High;
            }
        }

        public List<string> DumpAssetBundleCacheInfo()
        {
            return resourceMgr.AssetBundleGroup.DumpCacheInfo();
        }

        public void SetAssetBundlePersistent(string bundleName)
        {
            var bundle = resourceMgr.GetBundle(bundleName);
            bundle.BundlePersistent = true;
        }
        public void SetAssetPersistent(string bundleName)
        {
            var bundle = resourceMgr.GetBundle(bundleName);
            if (bundle != null)
            {
                bundle.AssetPersistent = true;
            }
        }
        public void ForceGCCache()
        {
            resourceMgr.AssetBundleGroup.ForceGCCache();
        }

        public void TryGCCache()
        {
            resourceMgr.AssetBundleGroup.TryGCCache();
        }

        /// <summary>
        /// 
        /// </summary>
        public void ResetCacheProfiler()
        {
#if DEBUG_BUNDLE_CACHE
            resourceMgr.ResetCacheProfiler();
#endif
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public string DumpCacheProfilerInfo()
        {
#if DEBUG_BUNDLE_CACHE
            return resourceMgr.DumpCacheProfilerInfo();
#else
            return "Profiler Off";
#endif
        }

        public int GetPendingBundleCount()
        {
            return resourceMgr.AssetBundleGroup.PendingBundleCount;
        }

        public int GetLoadingBundleIn10Seconds()
        {
#if DEBUG_BUNDLE_CACHE
            return resourceMgr.GetLoadingBundles();
#else
            return 0;
#endif
        }
        public int MaximalInstantiationPerFrame
        {
            get
            {
                return resourceMgr.AssetBundleGroup.MaximalInstantiationPerFrame;
            }
            set
            {
                resourceMgr.AssetBundleGroup.MaximalInstantiationPerFrame = value;
            }
        }

        public bool UseAssetBundleLoadFromFile { get; set; }

        public override void Dispose()
        {
            EngineCoreEvents.EngineEvent.GetAssetEvent -= GetAsset;
            EngineCoreEvents.EngineEvent.ReleaseAssetEvent -= ReleaseAsset;
            EngineCoreEvents.EngineEvent.RemoveAssetEvent -= RemoveAsset;
            EngineCoreEvents.EngineEvent.RemoveAllAssetEvent -= RemoveAllAsset;
            EngineCoreEvents.EngineEvent.TryGCCacheEvent -= TryGCCache;
        }

        public static ResourceModule Instance
        {
            get { return m_instance; }
        }
    }
}
