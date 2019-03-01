using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace EngineCore
{
    public class GOEAssetBundleCache
    {
        Dictionary<string, BundleInfoResource> cachedBundles = new Dictionary<string, BundleInfoResource>();
        Dictionary<string, AssetInfo> addedAssets = new Dictionary<string, AssetInfo>();

        int cachedSize = 0;

        int preferedCacheSize = 16 * 1024 * 1024;
        float regularGCFactor = 0.8f;
        float sceneGCFactor = 0.8f;

        float minimalGCInterval = 5f;
        float lastGCTime = 0;
        bool needGC = false;

        /// <summary>
        /// 当前缓存量，bundle的字节数，不包含解压后的资源尺寸
        /// </summary>
        public int CachedSize { get { return cachedSize; } }

        /// <summary>
        /// 希望的缓存尺寸，超出后在GC时会尝试释放暂时不用的资源到该尺寸
        /// </summary>
        public int PreferedCacheSize { get { return preferedCacheSize; } set { preferedCacheSize = value; } }

        /// <summary>
        /// 最小GC间隔，避免太频繁GC压力过高
        /// </summary>
        public float MinimalGCInterval { get { return minimalGCInterval; } set { minimalGCInterval = value; } }

        /// <summary>
        /// 常规GC量
        /// </summary>
        public float RegularGCFactor { get { return regularGCFactor; } set { regularGCFactor = value; } }
        /// <summary>
        /// 切场景GC量
        /// </summary>
        public float SceneGCFactor { get { return sceneGCFactor; } set { sceneGCFactor = value; } }

        public int CachedBundleCount { get { return cachedBundles.Count; } }

        /// <summary>
        /// 根据指定名称获取缓存的bundle，如果没缓存该bundle则返回null
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public BundleInfoResource this[string name]
        {
            get
            {
                BundleInfoResource res;
                if (cachedBundles.TryGetValue(name, out res))
                    return res;
                else
                    return null;
            }
        }


        public bool HasLoadedAsset(string asset)
        {
            var bName = ResourceMgr.Instance.GetBundleName(asset);
            BundleInfoResource b;
            if (cachedBundles.TryGetValue(bName, out b))
            {
                var a = b[asset];
                if (a != null)
                {
                    return a.AssetValid || b.AssetBundle;
                }
                else
                    return false;
            }
            else
                return addedAssets.ContainsKey(asset);
        }
        public AssetInfo GetAssetInCache(string name)
        {
            var bName = ResourceMgr.Instance.GetBundleName(name);
            BundleInfoResource b;
            if (cachedBundles.TryGetValue(bName, out b))
            {
                var asset = b[name];
                b.Touch();
                if (asset != null)
                    return asset;
                else
                {
                    if (addedAssets.TryGetValue(name, out asset))
                        return asset;
                }
            }
            else
            {
                AssetInfo asset;
                if (addedAssets.TryGetValue(name, out asset))
                    return asset;
            }
            return null;
        }

        public void AddAsset(string name, AssetInfo asset)
        {
            AssetInfo old;
            if (addedAssets.TryGetValue(name, out old))
                old.ReleaseAsset(true);
            addedAssets[name] = asset;
        }
        public bool ReleaseAssetReference(string name, UnityEngine.Object obj)
        {
            var bName = ResourceMgr.Instance.GetBundleName(name);
            BundleInfoResource b;
            if (cachedBundles.TryGetValue(bName, out b))
            {
                var asset = b[name];
                if (asset != null)
                {
                    asset.ReleaseAssetReference(obj);
                    return true;
                }
            }
            return false;
        }

        public bool CacheAssetBundle(BundleInfoResource bundle)
        {
            if (!cachedBundles.ContainsKey(bundle.Name))
            {
                cachedBundles[bundle.Name] = bundle;
                cachedSize += bundle.Size;
                bundle.TouchOnLoadBundle();
                needGC = true;
                return true;
            }
            return false;
        }

        /*public void CacheSceneBundleDependency(BundleInfo bundle)
        {
            BundleInfoResource newRes = new BundleInfoResource();
            newRes.BundleInfo = bundle;
            newRes.IsSceneBundle = true;
            foreach (var i in bundle.DependsOn)
            {
                BundleInfoResource res;
                if (cachedBundles.TryGetValue(i, out res))
                {
                    newRes.AddDependency(res);
                }
            }
            CacheAssetBundle(newRes);
        }*/

        /// <summary>
        /// 尝试进行垃圾回收
        /// </summary>
        /// <param name="suppress">是否忽视最佳缓存尺寸释放所有可释放资源</param>
        public void DoGC(bool suppress = false, bool force = false)
        {
#if DEBUG_BUNDLE_CACHE
            Profiler.BeginSample("GOEAssetBundleCache.DoGC");
#endif
            if ((needGC && ResourceMgr.Instance.GOELoaderMgr.IsFree) || force)
            {
                bool shouldDoGC = cachedSize > preferedCacheSize || suppress;

                if ((Time.realtimeSinceStartup - lastGCTime > minimalGCInterval) || force)
                {
#if DEBUG_BUNDLE_CACHE
                    int oldCacheSize = cachedSize;
                    int oldCacheCount = CachedBundleCount;
#endif
                    bool needUnloadUnused = false;
                    List<BundleInfoResource> bundleCanRelease = null;
                    do
                    {
                        bundleCanRelease = null;
                        foreach (var i in cachedBundles)
                        {
                            var bundle = i.Value;

                            if (bundle.ReleasePriority != (int)ReleasePriority.SaveBundle && (!ResourceModule.Instance.UseAssetBundleLoadFromFile || bundle.ResourceBelongBundleInfo.CanForceRelease))
                                bundle.ReleaseAssetBundle();
                            /*#if DEBUG_BUNDLE_CACHE
                                                    if (hasBundle && !bundle.AssetBundle)
                                                        DebugUtil.LogWarning(string.Format("{2:0.##}:Bundle {0} released webstream, new CacheSize={1:0.##}MB", bundle.Name, cachedSize / 1024f / 1024f, Time.realtimeSinceStartup));
                            #endif*/
                            if (shouldDoGC && bundle.CanRelease)
                            {
                                if (bundle.Name.Contains("police"))
                                    Debug.Log("can release  :" + bundle.Name + " " + Time.realtimeSinceStartup + " srver time " + bundle.SurvivalTimeExpired);

                                if (bundleCanRelease == null)
                                    bundleCanRelease = new List<BundleInfoResource>();
                                if (bundle.ReleasePriority <= 0)
                                    bundleCanRelease.Add(bundle);
                            }
                            else if (shouldDoGC || force)
                            {
                                //GC时释放多余的缓存实例
                                bundle.ReleaseRedudantInstance(force);
                            }
                        }
                        if (!shouldDoGC)
                        {
                            needGC = false;
#if DEBUG_BUNDLE_CACHE
                        Profiler.EndSample();
#endif
                            return;
                        }
                        if (bundleCanRelease != null)
                        {
                            if (bundleCanRelease.Count > 0)
                            {
                                bundleCanRelease.Sort(
                                            (a, b) =>
                                            {
                                                if (a.ReleasePriority != b.ReleasePriority)
                                                {
                                                    return a.ReleasePriority > b.ReleasePriority ? 1 : -1;
                                                }
                                                else
                                                {
                                                    return Mathf.CeilToInt((a.HitTime - b.HitTime) * 10);
                                                }
                                            }
                                    );
                                HashSet<string> bundleRemoved = new HashSet<string>();
                                for (int i = 0; i < bundleCanRelease.Count; i++)
                                {
                                    var bundle = bundleCanRelease[i];
                                    bundle.Release(false);
                                    if (bundle.Released)
                                    {
                                        needUnloadUnused = true;
                                        cachedSize -= bundle.Size;
                                        /*#if DEBUG_BUNDLE_CACHE
                                                                            DebugUtil.LogWarningFormat("{2:0.##}:Bundle {0} released, new CacheSize={1:0.##}MB", bundle.Name, cachedSize / 1024f / 1024f, Time.realtimeSinceStartup);
                                        #endif*/
                                        bundleRemoved.Add(bundle.Name);
                                    }
                                    if (cachedSize < preferedCacheSize * regularGCFactor && !force)
                                    {
                                        bundleCanRelease = null;
                                        break;
                                    }
                                }
                                foreach (var i in bundleRemoved)
                                {
                                    cachedBundles.Remove(i);
                                }
                                //GC.Collect();
                            }
                        }
                    }
                    while (bundleCanRelease != null && bundleCanRelease.Count > 0);
                    //if (needUnloadUnused && !ResourceModule.Instance.UseAssetBundleLoadFromFile)
                    if (needUnloadUnused && !ResourceModule.Instance.UseAssetBundleLoadFromFile)
                    {
#if DEBUG_BUNDLE_CACHE
                        Profiler.BeginSample("Resources.UnloadUnusedAssets");
#endif
                        //GC.Collect();
                        Resources.UnloadUnusedAssets();
#if DEBUG_BUNDLE_CACHE
                        Profiler.EndSample();
#endif
                    }
                    needGC = false;
                    lastGCTime = Time.realtimeSinceStartup;
#if DEBUG_BUNDLE_CACHE
                    DebugUtil.LogFormat("{0:0.##}:Cache GC, released {1} bundles, freed {2:0.##} MB", Time.realtimeSinceStartup, oldCacheCount - CachedBundleCount, (oldCacheSize - cachedSize) / 1024f / 1024f);
#endif
                }
            }
#if DEBUG_BUNDLE_CACHE
            Profiler.EndSample();
#endif
        }

        /// <summary>
        /// 彻底整理GC
        /// </summary>
        public void DoGCOnLoadLevel()
        {
            bool released = false;
            do
            {
                released = ReleaseCacheHasNoReference();
            } while (released);
        }

        bool ReleaseCacheHasNoReference()
        {
            HashSet<string> bundleRemoved = null;
            List<BundleInfoResource> bundleCanRelease = null;
            bool shouldGC = cachedSize >= preferedCacheSize * sceneGCFactor;
            foreach (var i in cachedBundles)
            {
                var b = i.Value;
                if (b.IsSceneBundle)
                {
                    if (shouldGC)
                    {
                        b.Release(true);
                        if (b.Released)
                        {
                            cachedSize -= b.Size;
                            if (bundleRemoved == null)
                                bundleRemoved = new HashSet<string>();
                            bundleRemoved.Add(i.Key);
                        }
                    }
                    else
                    {
                        b.IsSceneBundle = false;//Set to false to indicate, this is an inactive scene bundle
                        b.Touch();//Mark last used time
                    }
                }
                else if (shouldGC && !b.HasReference && b.CanReleaseOnLevel)
                {
                    if (bundleCanRelease == null)
                        bundleCanRelease = new List<BundleInfoResource>();
                    if (b.ReleasePriority <= 0)
                        bundleCanRelease.Add(b);
                }
                else if (shouldGC)
                {
                    b.ReleaseRedudantInstance(true);
                }

                if (b.CanReleaseOnLevel)
                {
                    b.ReleaseInstances();
                }
            }

            if (bundleCanRelease != null)
            {
                if (bundleCanRelease.Count > 0)
                {
                    bundleCanRelease.Sort(
                            (a, b) =>
                            {
                                if (a.ReleasePriority != b.ReleasePriority)
                                {
                                    return a.ReleasePriority > b.ReleasePriority ? 1 : -1;
                                }
                                else
                                {
                                    return Mathf.CeilToInt((a.HitTime - b.HitTime) * 10);
                                }
                            }
                    );
                    for (int i = 0; i < bundleCanRelease.Count; i++)
                    {
                        var bundle = bundleCanRelease[i];
                        bundle.Release(true);
                        if (bundle.Released)
                        {
                            cachedSize -= bundle.Size;
                            if (bundleRemoved == null)
                                bundleRemoved = new HashSet<string>();
                            bundleRemoved.Add(bundle.Name);
                        }
                        if (cachedSize < preferedCacheSize * sceneGCFactor)
                            break;
                    }
                }
            }

            if (bundleRemoved != null)
            {
                foreach (var i in bundleRemoved)
                {
                    cachedBundles.Remove(i);
                }
                return bundleRemoved.Count > 0;
            }
            else
                return false;
        }

        public List<string> DumpCacheInfo()
        {

            List<string> info = new List<string>();
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("Current Cache:" + (cachedSize / 1024f / 1024f).ToString("0.##MB"));
            sb.AppendLine("Cached Bundles: " + CachedBundleCount);
            List<BundleInfoResource> bundles = new List<BundleInfoResource>();
            Dictionary<string, int> sizes = new Dictionary<string, int>();
            Dictionary<string, int> cnts = new Dictionary<string, int>();
            foreach (var i in cachedBundles)
            {
                bundles.Add(i.Value);
            }
            int canReleaseCount = 0;
            foreach (var i in cachedBundles)
            {
                if (i.Value.CanRelease)
                {
                    canReleaseCount++;
                }
            }
            sb.AppendLine("CanReleaseCount:");
            sb.Append(canReleaseCount);

            bundles.Sort((a, b) => b.Size - a.Size);
            int bundleIndex = 0;
            foreach (var bundle in bundles)
            {
                bundleIndex++;
                string prefix = bundle.Name.Split('_')[0];
                int oldSize = 0;
                sizes.TryGetValue(prefix, out oldSize);
                oldSize += bundle.Size;
                sizes[prefix] = oldSize;
                if (cnts.ContainsKey(prefix))
                    cnts[prefix]++;
                else
                    cnts[prefix] = 1;
                sb.Append(bundleIndex + " AssetBundle:");
                sb.Append(bundle.Name);
                sb.Append(" Size:");
                float bundleSize = bundle.Size / 1024f / 1024f;
                if (bundleSize > 0.4f)
                {
                    sb.Append((bundle.Size / 1024f / 1024f).ToString("0.##MB"));
                }
                else
                {
                    sb.Append((bundle.Size / 1024f / 1024f).ToString("0.##MB"));
                }

                sb.Append(" CanRelease:");
                if (bundle.CanRelease)
                {
                    sb.Append(bundle.CanRelease);
                }
                else
                {
                    sb.Append(bundle.CanRelease);
                }

                sb.Append(" AssetCount:");
                sb.Append(bundle.AssetCount);
                sb.Append(" HasAssetBundle:");
                sb.Append((bool)bundle.AssetBundle);
                sb.Append(" ReferenceCount:");
                sb.Append(bundle.ReferencedBy.Count);
                sb.Append(" SurvivalTimeExpired:");
                sb.Append(bundle.SurvivalTimeExpired);
                sb.Append(" IsSceneBundle:");
                sb.AppendLine(bundle.IsSceneBundle.ToString());
                foreach (var j in bundle.ReferencedBy)
                {
                    sb.Append("    -");
                    sb.Append(j.Name);
                    sb.Append("  ");
                    sb.AppendLine((j.Size / 1024f / 1024f).ToString("0.##MB"));
                }

                if (sb.Length > 10000)
                {
                    info.Add(sb.ToString());
                    sb.Length = 0;
                }

            }
            sb.AppendLine("Groups:");
            foreach (var i in sizes)
            {
                sb.AppendLine(string.Format(" Name:{0} Size:{1} Count:{2}", i.Key, (i.Value / 1024f / 1024f).ToString("0.##MB"), cnts[i.Key]));
            }
            info.Add(sb.ToString());

            return info;
        }

        public void ForceGC()
        {
            needGC = true;
            DoGC(true, true);
        }

        public void TryGC()
        {
            needGC = true;
            DoGC(true, false);
        }

        public void SetAssetReleasePriority(string name, int _Priority, UnityEngine.Object obj)
        {
            var bName = ResourceMgr.Instance.GetBundleName(name);
            BundleInfoResource b;
            if (cachedBundles.TryGetValue(bName, out b))
            {
                b.ReleasePriority = _Priority;
                if (obj != null)
                {
                    var asset = b[name];
                    if (asset != null)
                    {
                        asset.ReleaseAssetReference(obj);
                    }
                    else
                    {
                        UnityEngine.Object.Destroy(obj);
                    }
                }
            }
        }
    }
}
