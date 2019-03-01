using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace EngineCore
{
    internal class ResourceMgr : ComponentObject
    {
#if DEBUG_BUNDLE_CACHE
        GOEAssetBundleCacheProfiler profiler = new GOEAssetBundleCacheProfiler();
#endif
        private static ResourceMgr m_instance;

        AssetBundleGroup abg;
        SceneBundleGroup sbg;
        WWWFileGroup wfg;
        WWWAudioGroup wag;
        GOEStringDelegate gsd;
        GOELoaderMgr glm;

        private BundleIndicesMap bundleMapLocal = null;

        //资源更新信息
        private AssetUpdateInfo m_updateInfo = null;

        public bool HasInitialized = false;

        public AssetBundleGroup AssetBundleGroup { get { return abg; } }

        public SceneBundleGroup SceneBundleGroup { get { return sbg; } }

        public WWWFileGroup WWWFileGroup { get { return wfg; } }

        public WWWAudioGroup WWWAudioGroup { get { return wag; } }

        public GOEStringDelegate GOEStringDelegate { get { return gsd; } }

        public GOELoaderMgr GOELoaderMgr { get { return glm; } }

        //资源路径解析
        private PathResolver m_assetPathResolver = null;

        internal override void Start()
        {
            abg = AddComponent<AssetBundleGroup>();
            sbg = AddComponent<SceneBundleGroup>();
            wfg = AddComponent<WWWFileGroup>();
            wag = AddComponent<WWWAudioGroup>();
            gsd = AddComponent<GOEStringDelegate>();
            glm = AddComponent<GOELoaderMgr>();

            InitRuntimePlatformPathResolver();

            LoadSQLiteBundleMap();
        }


        private Action<AssetUpdateInfo> OnUpdateFinishCallback = null;

        private Action<AssetUpdateInfo> UpdateProgressCallback = null;
        public bool IsUpdating = false;

        /// <summary>
        /// 初始化当前平台资源地址解析器
        /// </summary>
        private void InitRuntimePlatformPathResolver()
        {
            switch (Application.platform)
            {
                case RuntimePlatform.WindowsEditor:
                case RuntimePlatform.WindowsPlayer:
                case RuntimePlatform.OSXEditor:
                case RuntimePlatform.OSXPlayer:
                    this.m_assetPathResolver = new EditorPathResolver();
                    break;
                case RuntimePlatform.Android:
                    this.m_assetPathResolver = new AndroidPathResolver();
                    break;
                case RuntimePlatform.IPhonePlayer:
                    this.m_assetPathResolver = new IOSPathResolver();
                    break;
            }
        }

        public void BeginUpdateAssets(Action<AssetUpdateInfo> UpdateFinishCallback, Action<AssetUpdateInfo> ProgressCallback)
        {
            if (IsUpdating)
                return;

            this.OnUpdateFinishCallback = UpdateFinishCallback;
            this.UpdateProgressCallback = ProgressCallback;
            if (this.m_updateInfo != null && this.m_updateInfo.RemainDownloadAssetCount > 0)
            {
                this.m_updateInfo.StartDownloadTime = Time.time;

                ResourceModule.Instance.DownloadFilesFromServer(this.m_updateInfo.RemainDownloadFileList.ToArray(), OnDownloadedAsset, null, errorList =>
                {
                    if (errorList.Count > 0)
                    {
                        IsUpdating = false;
                        this.m_updateInfo.RetryErrorAssets(errorList);
                        BeginUpdateAssets(this.OnUpdateFinishCallback, ProgressCallback);
                    }
                    else
                    {
                        CheckUpdateEnd();
                    }
                }, null, true);
                IsUpdating = true;

            }
            else
            {
                CheckUpdateEnd();
            }
        }


        private void OnDownloadedAsset(string name, byte[] downloadedDataBuffer)
        {
            lock (this.m_updateInfo)
            {
                this.m_updateInfo.SetAssetFinishDownload(name);
                UpdateProgressCallback?.Invoke(this.m_updateInfo);
            }
        }

        private void CheckUpdateEnd()
        {
            if (this.m_updateInfo != null)
            {
                if (this.m_updateInfo.RemainDownloadAssetCount <= 0)
                {
                    bundleMapLocal.ForceCommit();
                    OnUpdateFinishCallback?.Invoke(this.m_updateInfo);
                    OnUpdateFinishCallback = null;
                    UpdateProgressCallback = null;
                    HasInitialized = true;

                    this.m_updateInfo = null;
                    IsUpdating = false;
                }
                else
                {
                    UpdateProgressCallback?.Invoke(this.m_updateInfo);
                }
            }
            else
            {
                bundleMapLocal.ForceCommit();

                OnUpdateFinishCallback?.Invoke(this.m_updateInfo);

                OnUpdateFinishCallback = null;
                UpdateProgressCallback = null;

                IsUpdating = false;
                HasInitialized = true;

            }
        }

        /// <summary>
        /// 检查资源更新
        /// </summary>
        /// <param name="UpdateAssetsCount"></param>
        internal void CheckUpdate(Action<int> UpdateAssetsCount)
        {
            SQLiteBundlemapLoader.Instance.CheckUpdate((diffResult) =>
            {
                if (diffResult != null)
                {
                    if (diffResult.DirtyBundleIndexList.Count > 0)
                    {
                        this.m_updateInfo = new AssetUpdateInfo();
                        for (int i = 0; i < diffResult.DirtyBundleIndexList.Count; ++i)
                            this.m_updateInfo.AddUpdateBundleIndexItem(diffResult.DirtyBundleIndexList[i]);
                    }

                    //if (diffResult.ObsoleteBundleIndexList.Count > 0)
                    //    RemoveObsoleteBundle(diffResult.ObsoleteBundleIndexList);
                }

                UpdateAssetsCount(diffResult == null ? 0 : diffResult.DirtyBundleIndexList.Count);
            });
        }


        public void StartDownloadAssets(AssetUpdateInfo assetUpdateInfo)
        {
            if (this.m_updateInfo == null)
                this.m_updateInfo = assetUpdateInfo;

            if (this.m_updateInfo != null && this.m_updateInfo.RemainDownloadAssetCount > 0)
            {
                this.m_updateInfo.StartDownloadTime = Time.time;

                ResourceModule.Instance.DownloadFilesFromServer(this.m_updateInfo.RemainDownloadFileList.ToArray(), OnDownloadedAsset, null, errorList =>
                {
                    if (errorList.Count > 0)
                    {
                        this.m_updateInfo.RetryErrorAssets(errorList);
                        StartDownloadAssets(assetUpdateInfo);
                    }
                    else
                    {
                        CheckUpdateEnd();
                    }
                }, null, true);
                IsUpdating = true;
            }
            else
            {
                CheckUpdateEnd();
            }
        }


        /// <summary>
        /// 请求下载动态资源
        /// </summary>
        /// <param name="assetNameList"></param>
        public void RequestDynamicAssets(string[] assetNameList, Action<AssetUpdateInfo> DownloadProgressHandler, Action<AssetUpdateInfo> OnDownloadFinishCallback)
        {
            if (!this.IsUpdating)
            {
                List<BundleIndexItemInfo> dynamicBundleList = SQLiteBundlemapLoader.Instance.GetDynamicBundleIndexItemListByAssets(new List<string>(assetNameList));
                this.OnUpdateFinishCallback = OnDownloadFinishCallback;

                if (dynamicBundleList.Count > 0)
                {
                    this.UpdateProgressCallback = DownloadProgressHandler;

                    SQLiteBundlemapLoader.Instance.RegisterDownloadBundleList(dynamicBundleList);
                    AssetUpdateInfo downloadAssetsInfo = new AssetUpdateInfo();

                    for (int i = 0; i < dynamicBundleList.Count; ++i)
                        downloadAssetsInfo.AddUpdateBundleIndexItem(dynamicBundleList[i]);

                    StartDownloadAssets(downloadAssetsInfo);
                }
                else
                {
                    CheckUpdateEnd();
                }
            }
        }

        /// <summary>
        /// 加载Bundlemap
        /// </summary>
        /// <param name="onLoad"></param>
        internal void LoadSQLiteBundleMap(Action onLoad = null)
        {
            SQLiteBundlemapLoader.Instance.LoadBundleMap(path =>
            {
                bundleMapLocal = new BundleIndicesMap(path);

                this.BundleMapAdapter = new BundleIndicesMapAdapter(bundleMapLocal);

                HasInitialized = true;

                onLoad?.Invoke();
            });
        }

        public void OnLeaveScene()
        {
            ResComponent res;
            for (int i = 0; i < mListComponent.Count; i++)
            {
                res = mListComponent[i] as ResComponent;
                res.OnLeaveScene();
                if (res is ResTypeGroup)
                {
                    (res as ResTypeGroup).RemoveAllResource();
                    (res as ResTypeGroup).RemoveAssetOnLevel();
                    (res as ResTypeGroup).ReleaseAssetbundle();
                }
            }

            //Clear Json Cache
            fastJSON.Reflection.Instance.ClearParseCache();
        }

        /// <summary>
        /// 获取资源所在Bundle名
        /// </summary>
        /// <param name="assetname"></param>
        /// <returns></returns>
        public string GetBundleName(string assetname)
        {
            BundleIndexItemInfo assetBelongBundleMapItemInfo = GetBundleInfo(assetname);
            if (assetBelongBundleMapItemInfo == null)
            {
                Debug.LogError($"can't find asset {assetname} belong bundle");
                return string.Empty;
            }
            return assetBelongBundleMapItemInfo.BundleName;
        }

        /// <summary>
        /// 获取资源所在Bundle信息
        /// </summary>
        /// <param name="assetName">资源名或Bundle名</param>
        /// <returns></returns>
        public RuntimeBundleInfo GetBundleInfo(string assetName)
        {
            RuntimeBundleInfo assetBelongBundleInfo = BundleMapAdapter.GetBundleIndexItemInfo(assetName);
            if (assetBelongBundleInfo == null)
                Debug.LogError($"can't find asset :{assetName} belong bundle");

            return assetBelongBundleInfo;
        }

        public void ReleaseAssetCallback(string name, Action<string, UnityEngine.Object> func)
        {
            if (name == null || name == string.Empty)
            {
                return;
            }

            AssetBundleGroup group = abg;
            group.ReleaseAssetCallback(name, func);
        }

        public bool IsLoadingScene
        {
            get
            {
                SceneBundleGroup group = sbg;
                return group.IsLoading;
            }
        }


        public bool IsBundleCached(string name)
        {
            AssetBundleGroup group = abg;
            return group.IsBundleCached(name);
        }

        public bool IsResCached(string name)
        {
            ResTypeGroup[] groups = GetComponents<ResTypeGroup>();
            foreach (ResTypeGroup group in groups)
            {
                if (group.IsResCached(name))
                {
                    return true;
                }
            }

            return false;
        }

        public void RemoveAsset(string name, bool force = true, bool removeInBundleAsset = true)
        {
            ResTypeGroup[] groups = GetComponents<ResTypeGroup>();
            foreach (ResTypeGroup group in groups)
            {
                if (group.RemoveAsset(name, force, removeInBundleAsset))
                    return;
            }
        }

        public void RemoveAllAsset()
        {
            ResTypeGroup[] groups = GetComponents<ResTypeGroup>();
            foreach (ResTypeGroup group in groups)
            {
                group.RemoveAllAsset(false);
            }
        }

        public void ReleaseAsset(string name, UnityEngine.Object obj)
        {
            for (int i = 0; i < mListComponent.Count; i++)
            {
                ResTypeGroup group = mListComponent[i] as ResTypeGroup;
                if (group == null)
                    continue;
                if (group.ReleaseAssetReference(name, obj))
                    return;
            }
            if (obj is GameObject)
                UnityEngine.Object.Destroy(obj);
#if !UNITY_EDITOR
            else
                UnityEngine.Resources.UnloadAsset(obj);
#endif
        }


        /// <summary>
        /// 删除过期的资源
        /// </summary>
        /// <param name="obsoleteBundles"></param>
        private void RemoveObsoleteBundle(List<BundleIndexItemInfo> obsoleteBundles)
        {
            for (int i = 0; i < obsoleteBundles.Count; ++i)
            {
                string obsoleteBundlePath;
                if (EngineFileUtil.IsExistInPersistentDataPath(obsoleteBundles[i].BundleHashName, out obsoleteBundlePath))
                    File.Delete(obsoleteBundlePath);
            }

            bundleMapLocal.DeleteBundleList(obsoleteBundles);
        }

        public void SetAssetReleasePriority(string name, int _Priority, UnityEngine.Object obj)
        {
            for (int i = 0; i < mListComponent.Count; i++)
            {
                ResTypeGroup group = mListComponent[i] as ResTypeGroup;
                if (group == null)
                    continue;
                group.SetAssetReleasePriority(name, _Priority, obj);
            }
        }


        public void ReleaseAndRemoveAsset(string name, UnityEngine.Object obj)
        {
            ReleaseAsset(name, obj);
            RemoveAsset(name);
        }

        /// <summary>
        /// 查询出本地不存在的BundleName列表
        /// </summary>
        /// <param name="queryBundleNameList"></param>
        /// <returns></returns>
        public List<string> QueryLocalNotIncludeBundleList(List<string> queryBundleNameList)
        {
            List<string> localNotIncludeBundleList = BundleMap.GetNotIncludeBundleList(queryBundleNameList);
            return localNotIncludeBundleList;
        }


        public static ResourceMgr Instance
        {
            get
            {
                return m_instance = m_instance ?? new ResourceMgr();
            }
        }

        public bool LowAsyncLoadPriority { get; set; }

        public PathResolver RuntimeAssetPathResolver => this.m_assetPathResolver;

        public BundleIndicesMap BundleMap => this.bundleMapLocal;

        public BundleIndicesMapAdapter BundleMapAdapter
        {
            get;
            private set;
        }


#if DEBUG_BUNDLE_CACHE
        public void SampleLoadEvent(string bn, int eventID)
        {
            profiler.SampleLoadEvent(bn, eventID);
        }

        /// <summary>
        /// 
        /// </summary>
        public void ResetCacheProfiler()
        {
            profiler.Reset();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public string DumpCacheProfilerInfo()
        {
            return profiler.DumpCacheStatistics();
        }

        public int GetLoadingBundles()
        {
            return profiler.LastLoadingBundle;
        }
#endif
    }
}