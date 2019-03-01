using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

namespace EngineCore
{
    public class SQLiteBundlemapLoader : Singleton<SQLiteBundlemapLoader>
    {
        public static string SQLITE_BUNDLEMAP_PATH = string.Empty;

        public BundleIndicesMap.BundleMapDiffResult DiffResult = null;

        private static Dictionary<string, BundleIndexItemInfo> m_downloadIndexDict = new Dictionary<string, BundleIndexItemInfo>();

        //最新的Bundlemap
        private BundleIndicesMap m_remoteBundleIndicesMap = null;

        public void LoadBundleMap(Action<string> OnLoadedBundleMap)
        {

            string bundleMapInStreaming = $"{PathResolver.ApplicationDataPath}/{WWWUtil.RES_PATH_PREFIX_WIN}/{SysConf.SQLITE_BUNDLE_MAP}";

#if UNITY_EDITOR || UNITY_STANDALONE_WIN
            SQLITE_BUNDLEMAP_PATH = bundleMapInStreaming;

            OnLoadedBundleMap(bundleMapInStreaming);
#elif UNITY_ANDROID || UNITY_IOS
            EngineCoreEvents.BridgeEvent.GetGameRootBehaviour().StartCoroutine(SyncBundleMapToPersistentPath((path) =>
           {
               SQLITE_BUNDLEMAP_PATH = path;
               OnLoadedBundleMap(path);
           }));
#endif
        }


        private IEnumerator SyncBundleMapToPersistentPath(Action<string> callback)
        {
            //检查localBundlemap是否是之前安装包的
            string buildinBundlemapHashPath = WWWUtil.GetStreamingAssetsPath("bundlemap.hash");
            WWW www = new WWW(buildinBundlemapHashPath);
            yield return www;

            string buildinBundlemapSha = www.text;
            www.Dispose();

            string bundlemapInPersistenPath = Path.Combine(PathResolver.ApplicationPersistentDataPath, SysConf.SQLITE_BUNDLE_MAP);
            if (File.Exists(bundlemapInPersistenPath))
            {
                BundleIndicesMap bundleMapInPersistent = new BundleIndicesMap(bundlemapInPersistenPath);
                string localBundleIdentify = bundleMapInPersistent.GetBundleMapInfo(BundleIndicesMap.BundleMapKey.BUNDLEMAP_IDENTIFY);
                bundleMapInPersistent.Dispose();

                if (localBundleIdentify == buildinBundlemapSha)
                {
                    callback(bundlemapInPersistenPath);
                    yield break;
                }
                else
                {
                    SyncBuildinBundlemap();
                    callback(bundlemapInPersistenPath);
                }
            }

            else
            {
                SyncBuildinBundlemap();
                callback(bundlemapInPersistenPath);
            }
        }

        private static void SyncBundleMapToPersistentPath()
        {

        }

        private void SyncBuildinBundlemap()
        {
            if (!EngineFileUtil.CopyFromStreamingAssetsToPersistentPath(SysConf.SQLITE_BUNDLE_MAP, SysConf.SQLITE_BUNDLE_MAP))
                Debug.LogError("streaming assets bundlemap not found");
        }

        private void SyncRemoteBundlemap(Action syncRemoteBundlemapCallback)
        {
            string serverBundleMapInPersistentPath = Path.Combine(PathResolver.ApplicationPersistentDataPath, SysConf.SERVER_SQLITE_BUNDLE_MAP);
            if (File.Exists(serverBundleMapInPersistentPath))
            {
                this.m_remoteBundleIndicesMap = new BundleIndicesMap(serverBundleMapInPersistentPath);
                string localRemoteBundlemapIdentify = m_remoteBundleIndicesMap.GetBundleMapInfo(BundleIndicesMap.BundleMapKey.BUNDLEMAP_IDENTIFY);
                ResourceModule.Instance.DownloadBufferFromServer(SysConf.SQLITE_BUNDLE_MAP_HASH, (serverBundlemapHash) =>
                {
                    string latestBundlemapIdentify = Encoding.Default.GetString(serverBundlemapHash);
                    if (latestBundlemapIdentify == localRemoteBundlemapIdentify)
                    {
                        syncRemoteBundlemapCallback();
                    }
                    else
                    {
                        this.m_remoteBundleIndicesMap.Dispose();
                        DownloadRemoteBundleMap(syncRemoteBundlemapCallback);
                    }
                }, (bundlemap) =>
                {
                    SyncRemoteBundlemap(syncRemoteBundlemapCallback);
                });
            }
            else
            {
                DownloadRemoteBundleMap(syncRemoteBundlemapCallback);
            }
        }


        private void DownloadRemoteBundleMap(Action callback)
        {
            ResourceModule.Instance.DownloadBufferFromServer(SysConf.SQLITE_BUNDLE_MAP, (bundleMapBuffer) =>
            {
                string serverBundleMapInPersistentPath = Path.Combine(PathResolver.ApplicationPersistentDataPath, SysConf.SERVER_SQLITE_BUNDLE_MAP);
                EngineFileUtil.WriteBytesToFile(serverBundleMapInPersistentPath, bundleMapBuffer);

                this.m_remoteBundleIndicesMap = new BundleIndicesMap(serverBundleMapInPersistentPath);

                callback();
            }, (remoteBundleMap) =>
            {
                DownloadRemoteBundleMap(callback);
            });
        }


        private void OnFinishUpdateAssets()
        {
            ResourceMgr.Instance.BundleMap.BundleIndicesMapConn.Commit();
        }


        private void OnDownloadedAsset(string assetHashFullName, byte[] assetBuffer)
        {
            BundleIndexItemInfo downloadedItemInfo;
            if (m_downloadIndexDict.TryGetValue(assetHashFullName, out downloadedItemInfo))
            {
                ResourceMgr.Instance.BundleMap.AddBundleIndicesByTransaction(downloadedItemInfo.BundleName, downloadedItemInfo.BundleHash, downloadedItemInfo.BundleDependencyArray, downloadedItemInfo.BundleAssetsArray, downloadedItemInfo.BundleSize);
                m_downloadIndexDict.Remove(assetHashFullName);
            }
            else
            {
                if (this.m_remoteBundleIndicesMap != null)
                {
                    string assetHashName = Path.GetFileName(assetHashFullName);
                    downloadedItemInfo = this.m_remoteBundleIndicesMap.GetBundleInfoByHashName(assetHashName);
                    if (downloadedItemInfo != null)
                        ResourceMgr.Instance.BundleMap.AddBundleIndicesByTransaction(downloadedItemInfo.BundleName, downloadedItemInfo.BundleHash, downloadedItemInfo.BundleDependencyArray, downloadedItemInfo.BundleAssetsArray, downloadedItemInfo.BundleSize);
                }
            }
        }


        public void CheckUpdate(Action<BundleIndicesMap.BundleMapDiffResult> NeedUpdateCallback)
        {
            if (this.m_remoteBundleIndicesMap != null && DiffResult != null)
            {
                NeedUpdateCallback(DiffResult);
            }
            else
            {
                SyncRemoteBundlemap(() =>
                {
                    EngineCoreEvents.ResourceEvent.OnDownloadedAssetFromWebServer += OnDownloadedAsset;
                    EngineCoreEvents.ResourceEvent.OnFinishDownloadUpdateEvent += OnFinishUpdateAssets;

                    DiffResult = ResourceMgr.Instance.BundleMap.Diff(this.m_remoteBundleIndicesMap.BundleIndicesMapConn, false);
                    OnPostDiff(DiffResult);

                    NeedUpdateCallback(DiffResult);
                });
            }
        }


        private void OnPostDiff(BundleIndicesMap.BundleMapDiffResult diffResult)
        {
            for (int i = 0; i < diffResult.ObsoleteBundleIndexList.Count; ++i)
            {
                BundleIndexItemInfo obsoleteBundleInfo = diffResult.ObsoleteBundleIndexList[i];
                string bundleInPersistent;
                if (EngineFileUtil.IsExistInPersistentDataPath(obsoleteBundleInfo.BundleHashName, out bundleInPersistent))
                    File.Delete(bundleInPersistent);
            }

            ResourceMgr.Instance.BundleMap.DeleteBundleList(diffResult.ObsoleteBundleIndexList);

            diffResult.ObsoleteBundleIndexList.Clear();

            //已经下载但没写入bundlemap的资源
            for (int i = 0; i < diffResult.DirtyBundleIndexList.Count; ++i)
            {
                BundleIndexItemInfo dirtyBundleItemInfo = diffResult.DirtyBundleIndexList[i];

                string bundleInPersistent;
                if (EngineFileUtil.IsExistInPersistentDataPath(dirtyBundleItemInfo.BundleHashName, out bundleInPersistent))
                {
                    //OnDownloadedAsset(dirtyBundleItemInfo.BundleHashName, null);

                    ResourceMgr.Instance.BundleMap.AddBundleIndicesByTransaction(dirtyBundleItemInfo.BundleName, dirtyBundleItemInfo.BundleHash, dirtyBundleItemInfo.BundleDependencyArray, dirtyBundleItemInfo.BundleAssetsArray, dirtyBundleItemInfo.BundleSize);

                    diffResult.DirtyBundleIndexList.RemoveAt(i);
                }
            }

            m_downloadIndexDict.Clear();
            RegisterDownloadBundleList(diffResult.DirtyBundleIndexList);
        }

        public List<BundleIndexItemInfo> GetDynamicBundleIndexItemListByAssets(List<string> assetNameList)
        {
            if (RemoteBundleIndicesMap == null && DiffResult == null)
                return null;

            List<BundleIndexItemInfo> dynamicBundleList = new List<BundleIndexItemInfo>();

            for (int i = assetNameList.Count - 1; i >= 0; --i)
            {
                string assetName = assetNameList[i];
                BundleIndexItemInfo dynamicBundleIndexItemInfo;
                dynamicBundleIndexItemInfo = DiffResult.GetAppendBundleItemInfoByBundleName(assetName);
                if (dynamicBundleIndexItemInfo != null)
                {
                    string dynamicBundlePath;
                    if (!EngineFileUtil.IsExistInPersistentDataPath(dynamicBundleIndexItemInfo.BundleHashName, out dynamicBundlePath))
                        dynamicBundleList.Add(dynamicBundleIndexItemInfo);
                    else
                    {
                        //OnDownloadedAsset(dynamicBundleIndexItemInfo.BundleHashName, null);
                        ResourceMgr.Instance.BundleMap.AddBundleIndicesByTransaction(dynamicBundleIndexItemInfo.BundleName, dynamicBundleIndexItemInfo.BundleHash, dynamicBundleIndexItemInfo.BundleDependencyArray, dynamicBundleIndexItemInfo.BundleAssetsArray, dynamicBundleIndexItemInfo.BundleSize);
                    }

                    assetNameList.Remove(assetName);
                }
            }

            List<BundleIndexItemInfo> assetBelongBundleList = RemoteBundleIndicesMap.GetBundleItemInfoListByAssetsList(assetNameList);
            for (int i = 0; i < assetBelongBundleList.Count; ++i)
            {
                BundleIndexItemInfo bundleItemInfo = DiffResult.GetAppendBundleItemInfoByBundleName(assetBelongBundleList[i].BundleName);
                if (bundleItemInfo != null)
                    dynamicBundleList.Add(bundleItemInfo);
            }

            return dynamicBundleList;
        }

        public BundleIndicesMap RemoteBundleIndicesMap => this.m_remoteBundleIndicesMap;

        public void RegisterDownloadBundleList(List<BundleIndexItemInfo> downloadingBundleIndexList)
        {
            for (int i = 0; i < downloadingBundleIndexList.Count; ++i)
            {
                if (!m_downloadIndexDict.ContainsKey(downloadingBundleIndexList[i].BundleHashName))
                    m_downloadIndexDict.Add(downloadingBundleIndexList[i].BundleHashName, downloadingBundleIndexList[i]);
            }
        }


        /// <summary>
        /// 销毁SQLiteBundlemapLoader
        /// </summary>
        public void Dispose()
        {
            EngineCoreEvents.ResourceEvent.OnDownloadedAssetFromWebServer -= OnDownloadedAsset;

            this.m_remoteBundleIndicesMap?.Dispose();
        }
    }
}