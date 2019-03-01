using BestHTTP;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;


namespace EngineCore
{
    /// <summary>
    /// 资源核心类
    /// </summary>
    [EngineCoreModule(EngineCore.ModuleType.RESOURCE_MODULE)]
    public class ResourceModule : AbstractModule, IProjectResource
    {
        private ResourceMgr resourceMgr;

        private static ResourceModule m_instance;

        private ResourceModule()
        {
            AutoStart = true;
            m_instance = this;
        }

        public override void Start()
        {
            resourceMgr = ResourceMgr.Instance;
            resourceMgr.Start();

            UseAssetBundleLoadFromFile = true;

            RegisterInternelEvents();
        }

        /// <summary>
        /// 注册资源Module对外事件
        /// </summary>
        private void RegisterInternelEvents()
        {
            EngineCoreEvents.ResourceEvent.GetAssetEvent += GetAsset;
            EngineCoreEvents.ResourceEvent.ReleaseAssetEvent += ReleaseAsset;
            EngineCoreEvents.ResourceEvent.RemoveAssetEvent += RemoveAsset;
            EngineCoreEvents.ResourceEvent.RemoveAllAssetEvent += RemoveAllAsset;
            EngineCoreEvents.ResourceEvent.TryGCCacheEvent += TryGCCache;
            EngineCoreEvents.ResourceEvent.LoadAdditiveScene += LoadAdditiveScene;
            EngineCoreEvents.ResourceEvent.LeaveScene += OnLeaveScene;
            EngineCoreEvents.ResourceEvent.GetTextAssetEvent += GetText;
            EngineCoreEvents.ResourceEvent.PreloadAssetEvent += PreloadAssetInternal;
            EngineCoreEvents.ResourceEvent.ReleaseAndRemoveAssetEvent += ReleaseAndRemoveAsset;
            EngineCoreEvents.ResourceEvent.CheckUpdate += CheckUpdate;
            EngineCoreEvents.ResourceEvent.BeginUpdateAssets += OnBeginUpdateAssets;
            HTTPManager.MaxConnectionPerServer = 20;
            HTTPManager.ConnectTimeout = TimeSpan.FromSeconds(30);
        }


        internal bool IsReady
        {
            get { return resourceMgr.HasInitialized; }
        }

        public override void Update()
        {
            m_isStarted = resourceMgr.HasInitialized;

            resourceMgr.Update();
        }


        /// <summary>
        /// 切换场景前清理资源
        /// </summary>
        public void OnLeaveScene()
        {
            Scene currentActiveScene = SceneManager.GetActiveScene();
            if (currentActiveScene.name != EngineDelegateCore.GameClientEntrySceneName)
            {
                UnLoadAdditiveScene(currentActiveScene.name);
                resourceMgr.OnLeaveScene();

                //出场景强制GC
                ForceGCCache();
            }

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
            if (StringUtils.IsNullEmptyOrWhiteSpace(name))
                callback(name, null);
            else
                resourceMgr.AssetBundleGroup.GetAsset(name, callback, priority);
        }

        /// <summary>
        /// 加载依赖包
        /// </summary>
        /// <param name="name"></param>
        public virtual void EnsureDependencies(string name)
        {
            resourceMgr.AssetBundleGroup.EnsureDependencies(name);
        }

        /// <summary>
        /// 预加载资源
        /// </summary>
        /// <param name="assetName"></param>
        /// <param name="OnPreloadAssetCallback"></param>
        private void PreloadAssetInternal(string assetName, Action<bool> OnPreloadAssetCallback)
        {
            resourceMgr.AssetBundleGroup.PreloadBundle(assetName, (name, assetBundleWithAsset) =>
            {
                if (assetBundleWithAsset != null)
                    OnPreloadAssetCallback?.Invoke(true);

            }, LoadPriority.Default, false);
        }


        /// <summary>
        /// 预加载assetbundle
        /// </summary>
        /// <param name="name"></param>
        /// <param name="callback"></param>
        /// <param name="priority"></param>
        public virtual void PreloadBundle(string name, Action<string, AssetBundle> callback, LoadPriority priority = LoadPriority.Default, bool dependencyResouece = false)
        {
            if (StringUtils.IsNullEmptyOrWhiteSpace(name))
                callback(name, null);
            else
                resourceMgr.AssetBundleGroup.PreloadBundle(name, callback, priority, dependencyResouece);
        }

        /// <summary>
        /// 预加载资源
        /// </summary>
        /// <param name="assetName"></param>
        /// <param name="OnPreloadFinish"></param>
        public void PreloadAsset(string assetName, Action<string, Object> OnPreloadFinish)
        {
            if (StringUtils.IsNullEmptyOrWhiteSpace(assetName))
                OnPreloadFinish(assetName, null);
            else
                resourceMgr.AssetBundleGroup.PreloadAsset(assetName, OnPreloadFinish);
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
        /// 加载Additive场景
        /// </summary>
        /// <param name="name"></param>
        /// <param name="callBack"></param>
        public void LoadAdditiveScene(string name, Action callBack = null)
        {
            LoadAdditiveScene(name, callBack, LoadPriority.MostPrior);
        }

        /// <summary>
        ///  获取Addivie场景
        /// </summary>
        /// <param name="name"></param>
        /// <param name="callBack"></param>
        /// <param name="priority"></param>
        public void GetAdditiveScene(string name, Action callBack = null, LoadPriority priority = LoadPriority.MostPrior)
        {
            resourceMgr.SceneBundleGroup.GetScene(name, callBack);
        }

        /// <summary>
        /// 加载Additive场景
        /// </summary>
        /// <param name="name"></param>
        /// <param name="OnSceneLoaded"></param>
        /// <param name="priority"></param>
        public void LoadAdditiveScene(string name, Action OnSceneLoaded = null, LoadPriority priority = LoadPriority.MostPrior)
        {
            //卸载非GameClient的场景
            OnLeaveScene();

            resourceMgr.SceneBundleGroup.GetScene(name, null, priority, true);

            UnityAction<Scene, LoadSceneMode> internelOnSceneLoadedHandler = null;
            internelOnSceneLoadedHandler = (loadedScene, LoadedSceneMode) =>
            {
                if (loadedScene.name == name)
                {
                    SceneManager.sceneLoaded -= internelOnSceneLoadedHandler;
                    SceneManager.SetActiveScene(loadedScene);

                    EngineCoreEvents.ResourceEvent.OnLoadAdditiveScene.SafeInvoke(loadedScene, loadedScene.GetRootGameObjects());

                    if (OnSceneLoaded != null)
                        OnSceneLoaded();

                }
            };
            SceneManager.sceneLoaded += internelOnSceneLoadedHandler;
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
        internal void GetWWWFromServer(string name, Action<string, WWW> callback)
        {
            resourceMgr.WWWFileGroup.getWWWFileFromServer(name, callback);
        }


        /// <summary>
        /// 从远程服务器下载文件
        /// </summary>
        /// <param name="bufferFileName"></param>
        /// <param name="OnDownloaded"></param>
        public void DownloadBufferFromServer(string bufferFileName, Action<byte[]> OnDownloaded, Action<string> OnDownloadError)
        {
            HttpDownloader downloader = new HttpDownloader();
            downloader.DownloadBufferFromServer(bufferFileName, OnDownloaded, OnDownloadError);
        }


        /// <summary>
        /// 从远程服务器下载大量文件 
        /// </summary>
        /// <param name="assetsUrl"></param>
        /// <param name="OnDownloadedAssets"></param>
        /// <param name="DownloadProgress"></param>
        /// <param name="OnDownloadFinish"></param>
        /// <param name="forceSave"></param>
        public void DownloadFilesFromServer(string[] assetsUrl, Action<string, byte[]> OnDownloadedAssets, Action<float> DownloadProgress = null, Action<List<string>> OnDownloadFinish = null, Action<string> OnDownloadAssetErrorCallback = null, bool forceSave = false)
        {
            HttpDownloader downloader = new HttpDownloader();
            downloader.DownloadMultipleFileFromServer(assetsUrl, OnDownloadedAssets, DownloadProgress, OnDownloadFinish, OnDownloadAssetErrorCallback, forceSave);
        }

        /// <summary>
        /// 从远程服务器下载文本
        /// </summary>
        /// <param name="textFileFullUrl"></param>
        /// <param name="OnDownloaded"></param>
        public void DownloadTextFromServer(string textFileFullUrl, Action<string> OnDownloaded, Action<string> OnDownloadError)
        {
            HttpDownloader downloader = new HttpDownloader();
            downloader.DownloadTextFromServer(textFileFullUrl, OnDownloaded, OnDownloadError);
        }


        /// <summary>
        /// 检查资源更新
        /// </summary>
        private void CheckUpdate(Action<int> NeedUpdateAssetsCount)
        {
            resourceMgr.CheckUpdate(NeedUpdateAssetsCount);
        }

        /// <summary>
        /// 请求开始更新
        /// </summary>
        /// <param name="OnFinishUpdateCallback">更新完成回调</param>
        /// <param name="OnUpdatingProgressCallback">更新进度回调</param>
        private void OnBeginUpdateAssets(Action<AssetUpdateInfo> OnFinishUpdateCallback, Action<AssetUpdateInfo> OnUpdatingProgressCallback)
        {
            resourceMgr.BeginUpdateAssets(OnFinishUpdateCallback, OnUpdatingProgressCallback);
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

        /// <summary>
        /// 释放并删除资源
        /// </summary>
        /// <param name="assetName">资源名称</param>
        /// <param name="releaseObject">释放对象</param>
        private void ReleaseAndRemoveAsset(string assetName, UnityEngine.Object releaseObject)
        {
            this.resourceMgr.ReleaseAndRemoveAsset(assetName, releaseObject);
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
            var bundle = resourceMgr.GetBundleInfo(bundleName);
            bundle.BundlePersistent = true;
        }

        public void SetAssetPersistent(string bundleName)
        {
            var bundle = resourceMgr.GetBundleInfo(bundleName);
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
            EngineCoreEvents.ResourceEvent.GetAssetEvent -= GetAsset;
            EngineCoreEvents.ResourceEvent.ReleaseAssetEvent -= ReleaseAsset;
            EngineCoreEvents.ResourceEvent.RemoveAssetEvent -= RemoveAsset;
            EngineCoreEvents.ResourceEvent.RemoveAllAssetEvent -= RemoveAllAsset;
            EngineCoreEvents.ResourceEvent.TryGCCacheEvent -= TryGCCache;
            EngineCoreEvents.ResourceEvent.LoadAdditiveScene -= LoadAdditiveScene;
            EngineCoreEvents.ResourceEvent.LeaveScene -= OnLeaveScene;
            EngineCoreEvents.ResourceEvent.GetTextAssetEvent -= GetText;
            EngineCoreEvents.ResourceEvent.PreloadAssetEvent -= PreloadAssetInternal;
            EngineCoreEvents.ResourceEvent.ReleaseAndRemoveAssetEvent -= ReleaseAndRemoveAsset;
            EngineCoreEvents.ResourceEvent.CheckUpdate -= CheckUpdate;
            EngineCoreEvents.ResourceEvent.BeginUpdateAssets -= OnBeginUpdateAssets;
            ResourceMgr.Instance.BundleMap.Dispose();
            SQLiteBundlemapLoader.Instance.Dispose();
        }


        public static ResourceModule Instance
        {
            get { return m_instance; }
        }
    }
}
