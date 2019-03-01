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
        private static ResourceMgr resourceMgr;
        public static ResourceMgr Instance()
        {
            if (resourceMgr == null)
            {
                resourceMgr = new ResourceMgr();
            }
            return resourceMgr;
        }
        LoadingProgress loadingProgress;
        AssetBundleGroup abg;
        SceneBundleGroup sbg;
        WWWFileGroup wfg;
        WWWAudioGroup wag;
        GOEStringDelegate gsd;
        GOELoaderMgr glm;
        ResBundleList rbl;
        ResIdxMap rim;
        BundleRename brn;
        public bool HasInitialized = false;

        public LoadingProgress LoadingProgress { get { return loadingProgress; } }

        public AssetBundleGroup AssetBundleGroup { get { return abg; } }

        public SceneBundleGroup SceneBundleGroup { get { return sbg; } }

        public WWWFileGroup WWWFileGroup { get { return wfg; } }

        public WWWAudioGroup WWWAudioGroup { get { return wag; } }

        public GOEStringDelegate GOEStringDelegate { get { return gsd; } }

        public GOELoaderMgr GOELoaderMgr { get { return glm; } }

        public ResBundleList ResBundleList { get { return rbl; } }

        public ResIdxMap ResIdxMap { get { return rim; } }

        public BundleRename BundleRename { get { return brn; } }

        internal override void Start()
        {

            loadingProgress = AddComponent<LoadingProgress>();
            abg = AddComponent<AssetBundleGroup>();
            sbg = AddComponent<SceneBundleGroup>();
            wfg = AddComponent<WWWFileGroup>();
            wag = AddComponent<WWWAudioGroup>();
            gsd = AddComponent<GOEStringDelegate>();
            glm = AddComponent<GOELoaderMgr>();
            rbl = AddComponent<ResBundleList>();
            rim = AddComponent<ResIdxMap>();
            brn = AddComponent<BundleRename>();

            initialize();
        }

        private void initialize()
        {
            if (EngineDelegateCore.DynamicResource)
                synchronizeRes();
            else
            {
                clearAllLoadedFiles();
                LoadBundleMap();
            }
        }

        private Action onBundleMapEnd;

        #region Synchronize Resource
        private void synchronizeRes()
        {
            ResourceModule.Instance.GetText(SysConf.BUNDLEMAP_FILE, onLoadBundleFromStream);
            ResourceModule.Instance.getWWWFromServer(SysConf.BUNDLEMAP_FILE_SERVER, onLoadBundleFromServer);
        }

        private string _localMapText;
        private string _serverMapText;
        private int loadNum = 0;
        private int errorNum = 0;
        private void onLoadBundleFromStream(string name, string map)
        {
            if (map == null || map == string.Empty)
            {
                LoadBundleMap();
                return;
            }
            _localMapText = map;
            ResourceModule.Instance.RemoveAsset(name);//Release cached Bundle map string
            beginCampare();
        }

        private void onLoadBundleFromServer(string name, WWW file)
        {
            if (file == null || file.text == string.Empty || (!string.IsNullOrEmpty(file.error)))
            {
                if (!string.IsNullOrEmpty(file.error))
                {
                    errorNum++;
                    if (errorNum > 20)
                    {
                        if (EngineDelegateCore.NetWorkError != null)
                        {
                            EngineDelegateCore.NetWorkError.Invoke(true);
                        }
                        errorNum = 0;
                        return;
                    }
                }
                ResourceModule.Instance.getWWWFromServer(SysConf.BUNDLEMAP_FILE_SERVER, onLoadBundleFromServer);

                return;
            }
            _serverMapText = file.text;
            beginCampare();
        }

        private void clearAllLoadedFiles()
        {
            string[] files = System.IO.Directory.GetFiles(Application.persistentDataPath, "*.bundle");
            foreach (var fi in files)
            {
                File.Delete(fi);
            }
        }

        private List<string> upatingRes;
        private Dictionary<string, string> upatingMap;
        private Dictionary<string, string> BundleNameToFile;
        BundleRename sbr;
        private void beginCampare()
        {
            if (_localMapText == null || _serverMapText == null)
                return;
            sbr = brn;
            sbr.Read(_serverMapText);
            BundleRename lbr = new BundleRename();
            lbr.Read(_localMapText, false);
            upatingRes = new List<string>();
            upatingMap = new Dictionary<string, string>();
            BundleNameToFile = new Dictionary<string, string>();
            int totalSize = 0;
            foreach (BundleInfoMapItem item in sbr.bundlemap.BundleMap.Values)
            {
                if (lbr.GetBundleNameFromOriginalName(item.Name) == item.FinalName)
                    continue;
                string path = Application.persistentDataPath + "/" + item.FinalName;
                if (File.Exists(path))
                {
                    FileInfo fileInfo = new FileInfo(path);
                    BundleInfo bundle = resourceMgr.GetBundle(item.Name);
                    if (bundle != null)
                    {
                        int _size = bundle.Size;
                        if (_size == fileInfo.Length)
                        {
                            continue;
                        }
                    }
                    else
                    {
                        if (item.Size == fileInfo.Length)
                        {
                            continue;
                        }
                    }

                    File.Delete(path);
                }
                loadNum++;
                totalSize += item.Size;
                upatingRes.Add(item.Name);
                upatingMap.Add(item.FinalName, item.Name);
                BundleNameToFile.Add(item.Name, item.FinalName);
                //ResourceModule.Instance.getWWWFromServer(item.FinalName, onGotWWW);ResourceModule.Instance.getWWWFromServer(item.FinalName, onGotWWW);
            }
            if (totalSize > 0)
                EngineDelegateCore.OnShowUpdateAlert.Invoke(totalSize);
            else
            {
                GogetWWWFromServer();
            }
            //if (upatingRes.Count > 0 && EngineDelegateCore.OnUpdateResource != null)
            //    EngineDelegateCore.OnUpdateResource.Invoke(upatingRes.ToArray());
            //sbr.AppendRead(_localMapText);
            //_localMapText = null;
            //_serverMapText = null;
            //checkCampareEnd();
        }
        internal string GetFileNameByBundleName(string bundlename)
        {
            if (BundleNameToFile != null && BundleNameToFile.ContainsKey(bundlename))
            {
                return BundleNameToFile[bundlename];
            }
            return null;
        }
        public void GogetWWWFromServer()
        {
            foreach (var a in upatingMap)
            {
                ResourceModule.Instance.getWWWFromServer(a.Key, onGotWWW);
            }
            if (upatingRes.Count > 0 && EngineDelegateCore.OnUpdateResource != null)
                EngineDelegateCore.OnUpdateResource.Invoke(upatingRes.ToArray());
            if (sbr != null)
                sbr.AppendRead(_localMapText);
            _localMapText = null;
            _serverMapText = null;
            EngineDelegateCore.StratOrEndDownload.Invoke(true);
            checkCampareEnd();
        }
        private List<string> needReloadReses = new List<string>();
        private List<float> reloadTime = new List<float>();
        internal override void Update()
        {
            base.Update();
#if DEBUG_BUNDLE_CACHE
            profiler.Update();
#endif
            if (needReloadReses.Count <= 0)
                return;
            List<string> removing = new List<string>();
            for (int i = 0; i < needReloadReses.Count; i++)
            {
                reloadTime[i] = reloadTime[i] - Time.deltaTime;
                if (reloadTime[i] <= 0)
                {
                    removing.Add(needReloadReses[i]);
                }

            }
            for (int i = 0; i < removing.Count; i++)
            {
                ResourceModule.Instance.getWWWFromServer(removing[i], onGotWWW);
                int index = needReloadReses.IndexOf(removing[i]);

                needReloadReses.RemoveAt(index);
                reloadTime.RemoveAt(index);
            }
            removing.Clear();
        }

        private void onGotWWW(string name, WWW www)
        {
            if (www == null || www.bytes.Length == 0 || (!string.IsNullOrEmpty(www.error)))
            {
                www.Dispose();
                needReloadReses.Add(name);
                reloadTime.Add(0.2f);
                return;
            }
            string _value = string.Empty;
            if (upatingMap.TryGetValue(name, out _value) && resourceMgr.GetBundle(_value).Size == www.bytes.Length)
            {
                string path = Application.persistentDataPath + "/" + name;
                FileStream fileStream = new FileStream(path, FileMode.Create);
                fileStream.Write(www.bytes, 0, www.bytes.Length);
                fileStream.Close();
                www.Dispose();
                loadNum--;
                checkCampareEnd();
            }
            else
            {
                www.Dispose();
                needReloadReses.Add(name);
                reloadTime.Add(0.2f);
            }

        }
        private void checkCampareEnd()
        {
            if (loadNum <= 0)
            {
                upatingRes.Clear();
                upatingMap.Clear();
                HasInitialized = true;
                EngineDelegateCore.StratOrEndDownload.Invoke(false);
            }
        }

        #endregion
        internal void SetProgress(string[] bundleNames, string[] assetNames, Action<float> handler, Action onEnd = null)
        {
            LoadingProgress pb = loadingProgress;
            pb.Clear();
            if (bundleNames != null)
            {
                for (int i = 0; i < bundleNames.Length; i++)
                {
                    pb.AddBundle(bundleNames[i]);
                }
            }
            if (assetNames != null)
            {
                for (int i = 0; i < assetNames.Length; i++)
                {
                    pb.AddAsset(assetNames[i]);
                }
            }
            pb.Start();
            pb.ProgressHandler += handler;
            pb.OnEnd = onEnd;
        }

        internal void SetProgress(string[] wwwNames, Action<float> handler, Action onEnd = null)
        {
            LoadingProgress pb = loadingProgress;
            pb.Clear();
            if (wwwNames != null)
            {
                for (int i = 0; i < wwwNames.Length; i++)
                {
                    pb.AddWWW(wwwNames[i]);
                }
            }

            pb.Start();
            pb.ProgressHandler += handler;
            pb.OnEnd = onEnd;
        }

        internal void LoadBundleMap(Action onLoad = null)
        {
            RemoveAsset(SysConf.BUNDLEMAP_FILE);
            if (onLoad != null)
                onBundleMapEnd += onLoad;
            ResourceModule.Instance.GetText(SysConf.BUNDLEMAP_FILE, OnLoadBundleMap);
        }

        private void OnLoadBundleMap(string name, string data)
        {
            if (data == null)
            {
                //Logger.GetFile(LogFile.Res).LogError("load bundlemap failed");
                return;
            }
            brn.Read(data);
            RemoveAsset(name);
            if (onBundleMapEnd != null)
            {
                onBundleMapEnd();
                onBundleMapEnd = null;
            }

            HasInitialized = true;

            //EngineDelegateCore.OnShowUpdateAlert.Invoke();
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

        public void OnEnterScene()
        {

        }

        public void RegisterBundleIdx(string asset, string bundleName, int size = 0)
        {
            rbl.RegisterBundleIdx(asset, bundleName, size);
            rim.RegResIdx(asset, bundleName);
        }

        public void UnRegisterIdxByBundleName(string bundleName)
        {
            BundleInfo bundle = rbl.GetBundle(bundleName);
            ResIdxMap idxMap = rim;
            if (bundle != null)
            {
                foreach (string asset in bundle.Files)
                {
                    idxMap.UnRegisterByAssetName(asset);
                }
            }

            rbl.UnRegisterByBundleName(bundleName);
        }

        public BundleInfo GetBundle(string name)
        {
            return rbl.GetBundle(name);
        }

        public List<BundleInfo> GetBundleByBeginName(string beginName)
        {
            return rbl.GetBundleByBeginName(beginName);
        }
        public Resource GetDownloadResource(string name)
        {
            Resource res = null;
            AssetBundleGroup bundlegroup = abg;
            res = bundlegroup.GetDownloadResource(name);
            if (res != null)
            {
                return res;
            }

            SceneBundleGroup scenegroup = sbg;
            res = scenegroup.GetDownloadResource(name);
            if (res != null)
            {
                return res;
            }
            WWWFileGroup filegroup = wfg;
            res = filegroup.GetDownloadResource(GetFileNameByBundleName(name));
            return res;
        }

        public Resource GetDownloadResourceByAssetName(string _name)
        {
            string name = this.GetBundleName(_name);
            if (name == string.Empty)
            {
                return null;

            }
            Resource res = null;
            AssetBundleGroup bundlegroup = abg;
            res = bundlegroup.GetDownloadResource(name);
            if (res != null)
            {
                return res;
            }

            SceneBundleGroup scenegroup = sbg;
            res = scenegroup.GetDownloadResource(name);

            return res;
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

        public void CacheSceneBundleDependency(BundleInfo bundle)
        {
            AssetBundleGroup group = abg;
            group.CacheSceneBundleDependency(bundle);
        }

        public bool IsLoadingScene
        {
            get
            {
                SceneBundleGroup group = sbg;
                return group.IsLoading;
            }
        }
        public string GetBundleName(string assetname)
        {
            return rim.GetBundleName(assetname);
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

        public bool LowAsyncLoadPriority { get; set; }

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
