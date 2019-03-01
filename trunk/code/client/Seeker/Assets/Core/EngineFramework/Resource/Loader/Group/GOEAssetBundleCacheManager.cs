using GOEngine;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace EngineCore
{

    /// <summary>
    /// 释放优先级
    /// </summary>
    public enum ReleasePriority
    {
        /// <summary>
        /// 走原来的释放逻辑
        /// </summary>
        Default = 0,
        /// <summary>
        /// 从bundle中load出来的Asset一直保存 用于Hero技能特效
        /// </summary>
        SaveAsset = 1,
        /// <summary>
        /// bundle 也不释放
        /// </summary>
        SaveBundle = 2
    };

    public class GOEAssetBundleCacheManager
    {
        //最多一次加载的AssetBundle数
        public const int MaximalLoadingAssetBundles = 8;

        //每帧数最多可实例化的Object数量
        public int MaximalInstantiationPerFrame = 2;

        //加载Bundle所有文件最大数量
        public int MaxLoadAllAssetThreshold = 15;


        public class AssetRequestInfo
        {
            private AssetBundleRequest m_assetLoadRequest = null;
            private string[] m_requestAssetNameArray = null;
            private UnityEngine.Object[] m_assets;
            private AssetBundle m_assetInBundle = null;
            private bool m_isLoadAsync = true;
            private bool m_isLoadFinish = false;
            private HashSet<string> m_requestAssetNameSet = null;

            /// <summary>
            /// 加载单个资源
            /// </summary>
            /// <param name="assetName"></param>
            /// <param name="assetInBundle"></param>
            public AssetRequestInfo(string assetName, AssetBundle assetInBundle)
            {
                this.m_requestAssetNameArray = new string[] { assetName };
                this.m_assetInBundle = assetInBundle;
                this.m_requestAssetNameSet = new HashSet<string>(this.m_requestAssetNameArray);
            }

            /// <summary>
            /// 加载所有资源
            /// </summary>
            /// <param name="allAssetName"></param>
            /// <param name="assetInBundle"></param>
            public AssetRequestInfo(string[] allAssetName, AssetBundle assetInBundle)
            {
                this.m_requestAssetNameArray = allAssetName;
                this.m_assetInBundle = assetInBundle;
                this.m_requestAssetNameSet = new HashSet<string>(this.m_requestAssetNameArray);
            }

            public AssetRequestInfo(string assetName, UnityEngine.Object existObject)
            {
                this.m_requestAssetNameArray = new string[] { assetName };
                Asset = existObject;
                this.m_requestAssetNameSet = new HashSet<string>(this.m_requestAssetNameArray);
                this.m_isLoadFinish = true;
            }

            /// <summary>
            /// 请求开始加载资源
            /// </summary>
            /// <param name="isAsyncLoad">是否异步加载</param>
            public void RequestLoadAsset(bool isAsyncLoad)
            {
                if (!this.m_isLoadFinish)
                {
                    this.m_isLoadAsync = isAsyncLoad;
                    if (m_requestAssetNameArray.Length > 1)
                    {
                        if (isAsyncLoad)
                        {
                            this.m_assetLoadRequest = this.m_assetInBundle.LoadAllAssetsAsync();
                            this.m_assetLoadRequest.completed += OnAsyncLoadAssetsCompleted;
                        }
                        else
                        {
                            Assets = this.m_assetInBundle.LoadAllAssets();
                            this.m_isLoadFinish = true;
                            this.m_assetInBundle = null;
                        }
                    }
                    else
                    {
                        if (isAsyncLoad)
                        {
                            this.m_assetLoadRequest = this.m_assetInBundle.LoadAssetAsync(RequestAssetName);
                            this.m_assetLoadRequest.completed += OnAsyncLoadAssetsCompleted;
                        }
                        else
                        {
                            Asset = this.m_assetInBundle.LoadAsset(RequestAssetName);
                            this.m_isLoadFinish = true;
                            this.m_assetInBundle = null;
                        }
                    }
                }
            }

            private void OnAsyncLoadAssetsCompleted(AsyncOperation asyncOperation)
            {
                if (asyncOperation.isDone)
                {
                    if (this.m_requestAssetNameArray.Length > 1)
                        Assets = this.m_assetLoadRequest.allAssets;
                    else
                        Asset = this.m_assetLoadRequest.asset;

                    this.m_isLoadFinish = true;
                    this.m_assetInBundle = null;
                    this.m_assetLoadRequest.completed -= OnAsyncLoadAssetsCompleted;
                }
            }

            /// <summary>
            /// 是否包含指定资源
            /// </summary>
            /// <param name="assetName"></param>
            /// <returns></returns>
            public bool IsContainAsset(string assetName)
            {
                return this.m_requestAssetNameSet.Contains(assetName);
            }

            /// <summary>
            /// 是否加载完成
            /// </summary>
            public bool IsLoadFinish
            {
                get
                {
                    return this.m_isLoadFinish;
                }
            }

            public string RequestAssetName
            {
                get
                {
                    if (this.m_requestAssetNameArray.Length == 1)
                        return this.m_requestAssetNameArray[0];
                    else
                    {
                        Debug.LogError($"RequestAsset is null");
                        return null;
                    }
                }
            }

            public UnityEngine.Object Asset
            {
                get
                {
                    if (this.m_assets.Length == 1)
                        return this.m_assets[0];
                    else
                    {
                        Debug.LogError($"Asset is null");
                        return null;
                    }
                }
                set
                {
                    this.m_assets = new UnityEngine.Object[] { value };
                    this.m_isLoadFinish = true;
                }
            }

            public UnityEngine.Object[] Assets
            {
                get { return this.m_assets; }
                private set { this.m_assets = value; }
            }

            public string[] RequestAssetsName
            {
                get { return this.m_requestAssetNameArray; }
            }

            public bool IsAsyncLoad
            {
                get { return this.m_isLoadAsync; }
                set { this.m_isLoadAsync = value; }
            }
        }

        public class AssetBundleAssetLoadingInfo
        {
            private string m_bundleName;
            public bool IsAsync;
            public AssetBundle AssetBundle;
            public List<AssetRequestInfo> Requests;
            public LoadPriority Priority;
            public bool IsLoadAllAsset = false;             //是否加载所有资源
            private BundleIndexItemInfo m_bundleIndexInfo = null;

            public AssetBundleAssetLoadingInfo(BundleIndexItemInfo bundleIndexInfo)
            {
                this.m_bundleIndexInfo = bundleIndexInfo;
                this.m_bundleName = bundleIndexInfo.BundleName;

                if (EngineCoreEvents.ResourceEvent.IsLoadAssetBundleAllAssets != null)
                    IsLoadAllAsset = EngineCoreEvents.ResourceEvent.IsLoadAssetBundleAllAssets.SafeInvoke(this.m_bundleName);
            }


            public bool IsNull
            {
                get
                {
                    return m_bundleName == null || this.m_bundleIndexInfo == null;
                }
            }

            public bool AllDone
            {
                get
                {
                    if (!IsAsync)
                        return true;
                    bool allDone = true;
                    for (int j = 0; j < Requests.Count; j++)
                    {
                        //if (Requests[j].Request != null && !Requests[j].Request.isDone)
                        if (!Requests[j].IsLoadFinish)
                        {
                            allDone = false;
                            break;
                        }
                    }
                    return allDone;
                }
            }

            public BundleIndexItemInfo BundleIndexInfo
            {
                get { return this.m_bundleIndexInfo; }
            }

            public string BundleName
            {
                get { return this.m_bundleName; }
            }
        }
        public GOEAssetBundleCacheManager(AssetBundleGroup group)
        {
            this.group = group;
        }

        GOEAssetBundleCache cache = new GOEAssetBundleCache();
        AssetBundleGroup group;



        List<AssetBundleAssetLoadingInfo> assetLoading = new List<AssetBundleAssetLoadingInfo>();
        LinkedList<AssetBundleAssetLoadingInfo> pendingBundle = new LinkedList<AssetBundleAssetLoadingInfo>();
        LinkedList<AssetInfo> instantiationQueue = new LinkedList<AssetInfo>();
        Dictionary<string, LinkedList<Action<string, UnityEngine.Object>>> assetLoadedCallbacks = new Dictionary<string, LinkedList<Action<string, UnityEngine.Object>>>();
        Dictionary<string, Action<string, AssetBundle>> bundleLoadedCallbacks = new Dictionary<string, Action<string, AssetBundle>>();

        //加载中资源与Bundle的索引关系
        private Dictionary<string, HashSet<string>> m_pendingAssetIndexDict = new Dictionary<string, HashSet<string>>();


        public GOEAssetBundleCache Cache { get { return cache; } }

        public int MaxInstantiationPerFrame
        {
            get { return MaximalInstantiationPerFrame; }
            set
            {
                MaximalInstantiationPerFrame = value;
            }
        }

        public int PendingBundles
        {
            get { return pendingBundle.Count; }
        }

        public bool HasLoadingInfo(string bundleName)
        {
            foreach (var i in pendingBundle)
            {
                if (i.BundleName == bundleName)
                    return true;
            }
            for (int i = 0; i < assetLoading.Count; i++)
            {
                var res = assetLoading[i];
                if (res.BundleName == bundleName)
                {
                    return true;
                }
            }
            return false;
        }
        internal void Update()
        {
#if DEBUG_BUNDLE_CACHE
            Profiler.BeginSample("GOEAssetBundleCacheManager.Update");
#endif
            while (assetLoading.Count < MaximalLoadingAssetBundles && pendingBundle.Count > 0)
            {
                var info = pendingBundle.First.Value;
                pendingBundle.RemoveFirst();

                StartLoadAssets(info);
            }

            for (int i = 0; i < assetLoading.Count; i++)
            {
                AssetBundleAssetLoadingInfo info = assetLoading[i];

                if (info.AllDone)
                {
#if DEBUG_BUNDLE_CACHE
                    ResourceMgr.Instance().SampleLoadEvent(info.BundleName, 6);
#endif
                    BundleInfoResource res = cache[info.BundleName];
                    bool isNewBundle = false;

                    if (res == null)
                    {
                        RuntimeBundleInfo resourceBelongBundleInfo = ResourceMgr.Instance.GetBundleInfo(info.BundleName);
                        res = new BundleInfoResource(resourceBelongBundleInfo);
                        res.AssetBundle = info.AssetBundle;
                        cache.CacheAssetBundle(res);
                        isNewBundle = true;
                    }
                    else
                    {
                        if (!res.AssetBundle)
                            res.AssetBundle = info.AssetBundle;
                    }

                    bool isAsync = info.IsAsync;

                    for (int j = 0; j < info.Requests.Count; j++)
                    {
                        AssetRequestInfo req = info.Requests[j];

                        for (int k = 0; k < req.Assets.Length; ++k)
                        {
                            UnityEngine.Object obj = req.Assets[k];
                            string assetName = req.RequestAssetsName[k];

                            if (!obj)
                            {
                                Debug.LogError($"can't load asset: {assetName}");
                                continue;
                            }


                            /*if (!assetName.StartsWithFast(obj.name))
                                Debug.LogError($"get object not match objname:{obj.name},request name:{assetName}");
                            else
                            {*/
                            AssetInfo asset = res[assetName];
                            if (asset == null)
                            {
                                asset = OnGotAsset(assetName, obj, info.BundleName);
                                res.AddAsset(assetName, asset);
                            }

                            if (asset.AssetValid && obj != asset.Asset)
                                throw new Exception($"{assetName} not valid");

                            if (obj)
                                asset.Asset = obj;

                            asset.Priority = info.Priority;
                            if (obj)
                            {
                                if (asset.NeedInstance && asset.PoolCount == 0)
                                {
                                    if (assetLoadedCallbacks.ContainsKey(assetName))
                                    {
                                        EnqueueInstantiateAsset(asset);
                                    }
                                }
                                else
                                    InvokeAssetLoaded(asset);
                            }
                            /*}*/
                        }
                    }

                    if (isNewBundle)
                    {
                        InvokeBundleLoaded(info);
                        MarkBundleDependency(res);
                    }

                    assetLoading.RemoveAt(i);
#if DEBUG_BUNDLE_CACHE
                    ResourceMgr.Instance().SampleLoadEvent(info.BundleName, 7);
                    Profiler.EndSample();
#endif
                    break;//一个Update只删除一个已完成Bundle
                }
            }

            if (instantiationQueue.Count > 0)
            {
                var asset = instantiationQueue.First.Value;

                if (!InvokeAssetLoaded(asset))
                    instantiationQueue.RemoveFirst();
            }

            //不在加载资源时GC
            bool hasLoading = assetLoading.Count > 0 || pendingBundle.Count > 0 || instantiationQueue.Count > 0;
            if (!hasLoading)
                cache.DoGC();
#if DEBUG_BUNDLE_CACHE
            Profiler.EndSample();
#endif
        }

        internal void RemoveAssetLoadCallback(string name, Action<string, UnityEngine.Object> func)
        {
            LinkedList<Action<string, UnityEngine.Object>> lst;
            if (assetLoadedCallbacks.TryGetValue(name, out lst))
            {
                var cur = lst.First;
                while (cur != null)
                {
                    if (cur.Value == func)
                    {
                        lst.Remove(cur);
                        break;
                    }
                    cur = cur.Next;
                }
            }
        }


        void StartLoadAssets(AssetBundleAssetLoadingInfo info)
        {
            try
            {
                BundleIndexItemInfo assetBundleInfo = info.BundleIndexInfo;
                var bundle = info.AssetBundle;

                if (!bundle)
                {
                    DebugUtil.LogError(string.Format("AssetBundle {0} is null", info.BundleName));
                    return;
                }
                info.Requests = new List<AssetRequestInfo>();
                bool shouldAsync = true;
                info.IsAsync = shouldAsync;

                BundleInfoResource cachedBundleInfo = cache[info.BundleName];


                //是否全部加载
                if (bundleLoadedCallbacks.ContainsKey(info.BundleName) || info.IsLoadAllAsset)
                {
                    AssetRequestInfo bundleAllAssetsRequest = new AssetRequestInfo(info.BundleIndexInfo.BundleAssetsArray, info.AssetBundle);
                    bundleAllAssetsRequest.RequestLoadAsset(info.IsAsync);
                    info.Requests.Add(bundleAllAssetsRequest);
                }
                else
                {
                    HashSet<string> pendingBundleAssets = null;
                    if (this.m_pendingAssetIndexDict.TryGetValue(info.BundleName, out pendingBundleAssets))
                    {
                        foreach (string pendingAsset in pendingBundleAssets)
                        {
                            AssetRequestInfo requestInfo = GetPendingAssetRequestInfo(pendingAsset, info.BundleName, info.AssetBundle);
                            requestInfo.RequestLoadAsset(info.IsAsync);
                            info.Requests.Add(requestInfo);
                        }

                        this.m_pendingAssetIndexDict[info.BundleName].Clear();
                    }
                }
                assetLoading.Add(info);
            }
            catch
            {
                DebugUtil.LogError("Cannot load assetbundle:" + info.BundleName);
            }
        }


        /// <summary>
        /// 获取加载信息
        /// </summary>
        /// <param name="assetName"></param>
        /// <param name="assetBelongBundleName"></param>
        /// <param name="assetInBundle"></param>
        /// <param name="isAsyncLoad"></param>
        /// <returns></returns>
        private AssetRequestInfo GetPendingAssetRequestInfo(string assetName, string assetBelongBundleName, AssetBundle assetInBundle)
        {
            BundleInfoResource cachedBundleInfo = cache[assetBelongBundleName];
            if (cachedBundleInfo != null)
            {
                AssetInfo cachedAssetInfo = cachedBundleInfo[assetName];
                if (cachedAssetInfo != null && cachedAssetInfo.AssetValid)
                {
                    AssetRequestInfo requestInfo = new AssetRequestInfo(assetName, cachedAssetInfo.Asset);

                    return requestInfo;
                }
                else
                {
                    AssetRequestInfo requestInfo = new AssetRequestInfo(assetName, assetInBundle);

                    return requestInfo;
                }
            }
            else
            {
                AssetRequestInfo requestInfo = new AssetRequestInfo(assetName, assetInBundle);

                return requestInfo;
            }
        }

        void InvokeBundleLoaded(AssetBundleAssetLoadingInfo info)
        {
            Action<string, AssetBundle> cb = null;
            if (bundleLoadedCallbacks.TryGetValue(info.BundleName, out cb))
            {
                try
                {
                    cb(info.BundleName, info.AssetBundle);
                }
                catch (Exception ex)
                {
                    DebugUtil.LogError(ex.ToString());
                }
                bundleLoadedCallbacks.Remove(info.BundleName);
            }
        }

        bool InvokeAssetLoaded(AssetInfo asset)
        {
            LinkedList<Action<string, UnityEngine.Object>> list = null;
            int instantiated = 0;
            if (assetLoadedCallbacks.TryGetValue(asset.Name, out list))
            {
                LinkedListNode<Action<string, UnityEngine.Object>> cur = list.First;
                while (cur != null)
                {
                    bool fromPool = false;
                    try
                    {
                        fromPool = asset.GetAsset(cur.Value);
                    }
                    catch (Exception ex)
                    {
                        DebugUtil.LogError(ex.ToString());
                    }
                    list.RemoveFirst();
                    cur = list.First;

                    if (asset.NeedInstance && !fromPool)
                        instantiated++;
                    if (instantiated >= MaximalInstantiationPerFrame)
                        return true;
                }
                assetLoadedCallbacks.Remove(asset.Name);
            }
            return false;
        }

        public void GetAssetInCache(string name, Action<string, UnityEngine.Object> callback, LoadPriority priority)
        {
            var asset = cache.GetAssetInCache(name);
            if (asset != null)
            {
                if (!asset.AssetValid)
                {
                    //Asset还未加载，需要回调
                    AddCallbackToDic(name, callback);
                    string bundleName = asset.BundleName;
                    //在已经加载的队列中寻找
                    if (CheckAssetLoading(bundleName, name, priority))
                        return;

                    DoEnqueuePending(bundleName, priority);
                    return;
                }

                if (asset.NeedInstance && asset.PoolCount == 0)
                {
                    AddCallbackToDic(name, callback);
                    EnqueueInstantiateAsset(asset);
                }
                else
                {
                    asset.GetAsset(callback);
                }
            }
        }

        /// <summary>
        /// 在AssetBundle已加载的条件下加载资源
        /// </summary>
        /// <param name="bundleName"></param>
        /// <param name="priority"></param>
        /// <param name="bundle"></param>
        public void DoEnqueuePending(string bundleName, LoadPriority priority, AssetBundle bundle = null)
        {
            BundleInfoResource BundleInfoResource = cache[bundleName];
            BundleIndexItemInfo bundleInfo = ResourceMgr.Instance.GetBundleInfo(bundleName);

            if (BundleInfoResource != null && BundleInfoResource.AssetBundle)
            {
                AssetBundleAssetLoadingInfo info = new AssetBundleAssetLoadingInfo(bundleInfo);
                info.AssetBundle = BundleInfoResource.AssetBundle;
                info.IsAsync = true;
                info.Priority = priority;
                EnqueuePendingAssetBunlde(info);
            }
            else
            {
                if (bundle == null)
                    throw new NullReferenceException("Cannot find Bundle for " + bundleName);

                AssetBundleAssetLoadingInfo info = new AssetBundleAssetLoadingInfo(bundleInfo);
                info.AssetBundle = bundle;
                info.IsAsync = true;
                info.Priority = priority;
                EnqueuePendingAssetBunlde(info);
            }
        }



        public bool CheckAssetLoading(string bundleName, string name, LoadPriority priority)
        {
            for (int i = 0; i < assetLoading.Count; i++)
            {
                var loading = assetLoading[i];
                if (loading.BundleName == bundleName)
                {
                    if (!loading.AllDone)
                    {
                        var requests = loading.Requests;
                        for (int j = 0; j < requests.Count; j++)
                        {
                            AssetRequestInfo assetRequestInfo = requests[j];
                            if (assetRequestInfo.IsContainAsset(name))
                                return true;

                            /*
                                                        if (req.RequestAssetName == name)
                                                        {
                                                            if (req.Request == null)
                                                            {
                                                                //补充加载
                                                                req.Request = loading.AssetBundle.LoadAssetAsync(name);
                                                            }
                                                            return true;
                                                        }*/
                        }

                    }
                    DoEnqueuePending(bundleName, priority, loading.AssetBundle);
                    return true;
                }
            }
            return false;

        }
        void MarkBundleDependency(BundleInfoResource res)
        {
            foreach (var i in res.ResourceBelongBundleInfo.BundleDependencyArray)
            {
                var bundle = cache[i];
                if (bundle != null)
                {
                    res.AddDependency(bundle);
                }
                else
                {
                    DebugUtil.LogError(string.Format("Cannot find dependency bundle {0} for bundle {1}.", i, res.Name));
                }
            }
        }

        public void ReleaseAssetbundleOnLevelLoad()
        {
            cache.DoGCOnLoadLevel();
            return;         //todo:add by guangze song ,当loading时，切换场景会把之前的callback都clear 会导致图没加载出来的问题，所以下一步修改 2018-10-22 17:47:7

            //取消所有进行中的资源加载
            foreach (var i in pendingBundle)
            {
                if (i.AssetBundle && cache[i.BundleName] == null)
                    i.AssetBundle.Unload(true);
            }
            pendingBundle.Clear();
            foreach (var i in assetLoading)
            {
                if (i.AssetBundle && cache[i.BundleName] == null)
                    i.AssetBundle.Unload(true);
            }
            assetLoading.Clear();

            assetLoadedCallbacks.Clear();
            bundleLoadedCallbacks.Clear();

            instantiationQueue.Clear();
        }

        public void OnLoadAssetBundle(string name, AssetBundle bundle, bool dependencyResource, LoadPriority priority)
        {
#if DEBUG_BUNDLE_CACHE
            ResourceMgr.Instance().SampleLoadEvent(name, 4);
#endif
            RuntimeBundleInfo resourceBelongBundleInfo = ResourceMgr.Instance.GetBundleInfo(name);
            AssetBundleAssetLoadingInfo info = new AssetBundleAssetLoadingInfo(resourceBelongBundleInfo);


            info.AssetBundle = bundle;
            info.Priority = priority;
            if (!dependencyResource)
            {
                EnqueuePendingAssetBunlde(info);
            }
            else
            {
                BundleInfoResource res = new BundleInfoResource(resourceBelongBundleInfo);
                res.AssetBundle = info.AssetBundle;
                cache.CacheAssetBundle(res);
                InvokeBundleLoaded(info);
                MarkBundleDependency(res);
            }
        }

        public void EnqueuePendingAssetBunlde(AssetBundleAssetLoadingInfo info)
        {
            LinkedListNode<AssetBundleAssetLoadingInfo> cur = pendingBundle.First;
            LinkedListNode<AssetBundleAssetLoadingInfo> found = null;
            LoadPriority priority = info.Priority;

            while (cur != null)
            {
                if (cur.Value.BundleName == info.BundleName)
                {
                    return;
                }
                if (found == null)
                {
                    if (cur.Value.Priority < priority)
                    {
                        found = cur;
                    }
                }
                cur = cur.Next;
            }
            if (cur != null)
                pendingBundle.AddAfter(cur, info);
            else
                pendingBundle.AddLast(info);
        }

        void EnqueueInstantiateAsset(AssetInfo asset)
        {
            LinkedListNode<AssetInfo> cur = instantiationQueue.First;
            var priority = asset.Priority;
            LinkedListNode<AssetInfo> found = null;
            while (cur != null)
            {
                if (cur.Value == asset)
                    return;
                if (found == null)
                {
                    if (cur.Value.Priority < priority)
                    {
                        found = cur;
                    }
                }
                cur = cur.Next;
            }
            if (found != null)
                instantiationQueue.AddAfter(found, asset);
            else
                instantiationQueue.AddLast(asset);
        }

        private AssetInfo OnGotAsset(string name, UnityEngine.Object obj, string bundleName)
        {
            AssetInfo asset = new AssetInfo(obj, name, bundleName);
            return asset;
        }

        public void AddCallbackToDic(string name, Action<string, UnityEngine.Object> action)
        {
            LinkedList<Action<string, UnityEngine.Object>> old = null;
            if (!assetLoadedCallbacks.TryGetValue(name, out old))
            {
                old = new LinkedList<Action<string, UnityEngine.Object>>();
                assetLoadedCallbacks[name] = old;
            }
            old.AddLast(action);

            string assetBelongBundleName = ResourceMgr.Instance.GetBundleName(name);
            if (string.IsNullOrEmpty(assetBelongBundleName))
            {
                Debug.LogError($"can't find asset {name} belong Bundle");
                return;
            }

            HashSet<string> bundlePendingAssetList = null;
            if (!m_pendingAssetIndexDict.TryGetValue(assetBelongBundleName, out bundlePendingAssetList))
            {
                bundlePendingAssetList = new HashSet<string>();
                m_pendingAssetIndexDict.Add(assetBelongBundleName, bundlePendingAssetList);
            }

            m_pendingAssetIndexDict[assetBelongBundleName].Add(name);
        }

        public void AddAssetBundleCallbackToDic(string name, Action<string, AssetBundle> action)
        {
            if (action == null)
                return;
            Action<string, AssetBundle> old = null;
            Action<string, AssetBundle> newDel = action;
            if (bundleLoadedCallbacks.TryGetValue(name, out old))
            {
                newDel = old + action;
            }

            bundleLoadedCallbacks[name] = newDel;

        }

        public void SetAssetReleasePriority(string name, int _Priority, UnityEngine.Object obj)
        {
            cache.SetAssetReleasePriority(name, _Priority, obj);
        }
    }
}
