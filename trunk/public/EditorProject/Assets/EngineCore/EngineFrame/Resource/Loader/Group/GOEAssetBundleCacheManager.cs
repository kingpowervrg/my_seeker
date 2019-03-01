using GOEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

    internal class GOEAssetBundleCacheManager
    {
        class AssetRequestInfo
        {
            public UnityEngine.Object Asset;
            public AssetBundleRequest Request;
            public string AssetName;
        }
        class AssetBundleAssetLoadingInfo
        {
            public string BundleName;
            public bool IsAsync;
            public AssetBundle AssetBundle;
            public List<AssetRequestInfo> Requests;
            public LoadPriority Priority;
            public bool IsNull
            {
                get
                {
                    return BundleName == null;
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
                        if (Requests[j].Request != null && !Requests[j].Request.isDone)
                        {
                            allDone = false;
                            break;
                        }
                    }
                    return allDone;
                }
            }
        }
        public GOEAssetBundleCacheManager(AssetBundleGroup group)
        {
            this.group = group;
        }

        GOEAssetBundleCache cache = new GOEAssetBundleCache();
        AssetBundleGroup group;

        const int MaximalLoadingAssetBundles = 1;
        int maximalInstantiationPerFrame = 1;

        List<AssetBundleAssetLoadingInfo> assetLoading = new List<AssetBundleAssetLoadingInfo>();
        LinkedList<AssetBundleAssetLoadingInfo> pendingBundle = new LinkedList<AssetBundleAssetLoadingInfo>();
        LinkedList<AssetInfo> instantiationQueue = new LinkedList<AssetInfo>();
        Dictionary<string, LinkedList<Action<string, UnityEngine.Object>>> assetLoadedCallbacks = new Dictionary<string, LinkedList<Action<string, UnityEngine.Object>>>();
        Dictionary<string, Action<string, AssetBundle>> bundleLoadedCallbacks = new Dictionary<string, Action<string, AssetBundle>>();

        public GOEAssetBundleCache Cache { get { return cache; } }

        public int MaximalInstantiationPerFrame
        {
            get { return maximalInstantiationPerFrame; }
            set
            {
                maximalInstantiationPerFrame = value;
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
                        res = new BundleInfoResource();
                        res.AssetBundle = info.AssetBundle;
                        res.BundleInfo = ResourceMgr.Instance().GetBundle(info.BundleName);
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
                        UnityEngine.Object obj = isAsync && req.Request != null ? req.Request.asset : req.Asset;

                        string assetName = req.AssetName;
                        AssetInfo asset = null;
                        if (isNewBundle)
                        {
                            asset = OnGotAsset(assetName, obj, info.BundleName);
                            res.AddAsset(assetName, asset);
                        }
                        else
                        {
                            asset = res[assetName];
                            if (asset.AssetValid && obj != asset.Asset)
                                throw new NotSupportedException();
                            if (obj)
                                asset.Asset = obj;
                        }
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
                var bundleName = info.BundleName;
                BundleInfo gBundle = ResourceMgr.Instance().GetBundle(bundleName);
                var bundle = info.AssetBundle;
                if (!bundle)
                {
                    DebugUtil.LogError(string.Format("AssetBundle {0} is null", gBundle.mName));
                    return;
                }
                info.Requests = new List<AssetRequestInfo>();
                bool shouldAsync = true;
                info.IsAsync = shouldAsync;
                bool isPreload = bundleLoadedCallbacks.ContainsKey(bundleName) || !ResourceModule.Instance.UseAssetBundleLoadFromFile;
#if DEBUG_BUNDLE_CACHE
                ResourceMgr.Instance().SampleLoadEvent(bundleName, 5);
#endif
                var cb = cache[bundleName];
                foreach (string str in gBundle.Files)
                {
                    AssetRequestInfo ar = null;
                    bool isPending = assetLoadedCallbacks.ContainsKey(str);
                    bool canContinue = true;
                    if (cb != null)
                    {
                        var a = cb[str];
                        if (a != null && a.AssetValid)
                        {
                            if (isPending)
                            {
                                ar = new AssetRequestInfo();
                                ar.AssetName = str;
                                ar.Asset = a.Asset;
                                info.Requests.Add(ar);
                            }
                            continue;
                        }
                        else
                            canContinue = false;
                    }
                    else
                        canContinue = false;
                    isPending = isPending || isPreload;
                    if (!isPending && canContinue)
                        continue;

                    ar = new AssetRequestInfo();
                    ar.AssetName = str;

                    //只加载请求中的资源
                    if (isPending)
                    {
                        bool isSprite = str.IndexOf('.') < 0;
                        if (shouldAsync)
                        {
                            ar.Request = isSprite ? bundle.LoadAssetAsync<Sprite>(str) : bundle.LoadAssetAsync(str);
                        }
                        else
                        {
                            ar.Asset = isSprite ? bundle.LoadAsset<Sprite>(str) : bundle.LoadAsset(str);
                        }
                    }
                    info.Requests.Add(ar);
                }
                assetLoading.Add(info);
            }
            catch
            {
                DebugUtil.LogError("Cannot load assetbundle:" + info.BundleName);
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
                    if (instantiated >= maximalInstantiationPerFrame)
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

        void DoEnqueuePending(string bundleName, LoadPriority priority, AssetBundle bundle = null)
        {
            BundleInfoResource BundleInfoResource = cache[bundleName];
            if (BundleInfoResource != null && BundleInfoResource.AssetBundle)
            {
                AssetBundleAssetLoadingInfo info = new AssetBundleAssetLoadingInfo();
                info.AssetBundle = BundleInfoResource.AssetBundle;
                info.BundleName = bundleName;
                info.IsAsync = true;
                info.Priority = priority;
                EnqueuePendingAssetBunlde(info);
            }
            else
            {
                if (bundle == null)
                    throw new NullReferenceException("Cannot find Bundle for " + bundleName);
                AssetBundleAssetLoadingInfo info = new AssetBundleAssetLoadingInfo();
                info.AssetBundle = bundle;
                info.BundleName = bundleName;
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
                            var req = requests[j];
                            if (req.AssetName == name)
                            {
                                if (req.Request == null)
                                {
                                    //补充加载
                                    req.Request = loading.AssetBundle.LoadAssetAsync(name);
                                }
                                return true;
                            }
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
            foreach (var i in res.BundleInfo.DependsOn)
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
            AssetBundleAssetLoadingInfo info = new AssetBundleAssetLoadingInfo();
            info.AssetBundle = bundle;
            info.BundleName = name;
            info.Priority = priority;
            if (!dependencyResource)
            {
                EnqueuePendingAssetBunlde(info);
            }
            else
            {
                BundleInfoResource res = new BundleInfoResource();
                res.AssetBundle = info.AssetBundle;
                res.BundleInfo = ResourceMgr.Instance().GetBundle(info.BundleName);
                cache.CacheAssetBundle(res);
                InvokeBundleLoaded(info);
                MarkBundleDependency(res);
            }
        }

        void EnqueuePendingAssetBunlde(AssetBundleAssetLoadingInfo info)
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
        }

        public void AddCallbackToDic(string name, Action<string, AssetBundle> action)
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

#if DEBUG_BUNDLE_CACHE
    class GOEAssetBundleCacheProfiler
    {
        Dictionary<string, GOEAssetBundleProfileInfo> info = new Dictionary<string, GOEAssetBundleProfileInfo>();
        float lastStatisticTime;
        int loadingBundle;
        int lastLoadingBundle;

        public int LastLoadingBundle
        {
            get
            {
                return lastLoadingBundle;
            }
        }
        public void SampleLoadEvent(string bn, int eventID)
        {
            GOEAssetBundleProfileInfo pi = null;
            float time = Time.realtimeSinceStartup;
            if (info.TryGetValue(bn, out pi))
            {
                switch (eventID)
                {
                    case 2:
                        pi.WWWStartTime = time;
                        break;
                    case 3:
                        pi.WWWFinishTime = time;
                        break;
                    case 4:
                        pi.ResourceReadyTime = time;
                        break;
                    case 5:
                        pi.AssetLoadStartTime = time;
                        break;
                    case 6:
                        pi.AssetLoadFinishTime = time;
                        break;
                    case 7:
                        pi.EndTime = time;
                        break;
                }
            }
            else if (eventID == 1)
            {
                pi = new GOEAssetBundleProfileInfo();
                pi.BundleName = bn;
                BundleInfo bundle = ResourceMgr.Instance().GetBundle(bn);
                pi.BundleSize = bundle.Size;
                pi.StartTime = time;
                info[bn] = pi;
                loadingBundle++;
            }
        }

        public void Update()
        {
            if (Time.realtimeSinceStartup - lastStatisticTime > 3)
            {
                lastStatisticTime = Time.realtimeSinceStartup;
                lastLoadingBundle = loadingBundle;
                loadingBundle = 0;
            }
        }

        public void Reset()
        {
            info.Clear();
        }

        public string DumpCacheStatistics()
        {
            StringBuilder sb = new StringBuilder();
            int totalSize = 0;
            int cnt = 0;
            float totalTime = 0, totalWWWTime = 0, totalAssetTime = 0, totalWaitTime = 0;

            foreach (var i in info)
            {
                sb.Append(i.Key);
                sb.Append(":\t\t\t\tStartTime:");
                sb.Append(i.Value.StartTime.ToString("0.##"));
                sb.Append("\tTotalLoadTime:");
                sb.Append(i.Value.TotalLoadTime.ToString("0.##"));
                sb.Append("\tWWWLoadTime:");
                sb.Append(i.Value.WWWLoadTime.ToString("0.##"));
                sb.Append("\tAssetLoadTime:");
                sb.Append(i.Value.AssetLoadTime.ToString("0.##"));
                sb.Append("\tWaitTime:");
                sb.AppendLine(i.Value.WaitTime.ToString("0.##"));

                totalSize += i.Value.BundleSize;
                totalTime += i.Value.TotalLoadTime;
                totalWWWTime += i.Value.WWWLoadTime;
                totalAssetTime += i.Value.AssetLoadTime;
                totalWaitTime += i.Value.WaitTime;
                cnt++;
            }

            sb.Append("TotalCount:");
            sb.Append(cnt);
            sb.Append(" TotalTime:");
            sb.Append(totalTime.ToString("0.##"));
            sb.Append(" TotalSize:");
            sb.Append((totalSize / 1024 / 1024f).ToString("0.##MB"));
            sb.Append(" TotalWWWTime:");
            sb.Append(totalWWWTime.ToString("0.##"));
            sb.Append(" TotalAssetTime:");
            sb.Append(totalAssetTime.ToString("0.##"));
            sb.Append(" TotalWaitTime:");
            sb.Append(totalWaitTime.ToString("0.##"));

            return sb.ToString();
        }
    }

    class GOEAssetBundleProfileInfo
    {
        public string BundleName { get; set; }
        public int BundleSize { get; set; }
        public float StartTime { get; set; }
        public float WWWStartTime { get; set; }
        public float WWWFinishTime { get; set; }
        public float ResourceReadyTime { get; set; }
        public float AssetLoadStartTime { get; set; }

        public float AssetLoadFinishTime { get; set; }
        public float EndTime { get; set; }

        float ValidEndTime { get { return EndTime > 0 ? EndTime : (ResourceReadyTime > 0 ? ResourceReadyTime : StartTime); } }

        public float TotalLoadTime { get { return WWWLoadTime + AssetLoadTime; } }

        public float WWWLoadTime { get { return ResourceReadyTime > 0 ? ResourceReadyTime - WWWStartTime : 0; } }
        public float AssetLoadTime { get { return AssetLoadStartTime > 0 ? ValidEndTime - AssetLoadStartTime : 0; } }

        public float WaitTime { get { return ValidEndTime - StartTime - WWWLoadTime - AssetLoadTime; } }
    }
#endif
}
