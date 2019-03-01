using System.Collections.Generic;
using UnityEngine;

namespace EngineCore
{
    public class BundleInfoResource
    {
        private HashSet<BundleInfoResource> referencedBy = new HashSet<BundleInfoResource>();
        List<BundleInfoResource> dependsOn = new List<BundleInfoResource>();
        Dictionary<string, AssetInfo> loadedAssets = new Dictionary<string, AssetInfo>();
        List<AssetInfo> assetList = new List<AssetInfo>();
        bool released = false;
        float hitTime = Time.realtimeSinceStartup;

        private bool m_isBundleUsed = false;


        //资源所属的Bundle信息
        private RuntimeBundleInfo m_resourceBelongBundleInfo = null;

        public BundleInfoResource(RuntimeBundleInfo resourceBelongBundleInfo)
        {
            this.m_resourceBelongBundleInfo = resourceBelongBundleInfo;
        }

        /// <summary>
        /// 最少存活时间，避免依赖包刚加载上了发现没用就被卸载了或者短时间内反复需要
        /// </summary>
        const float MinimalSurvivalTime = 15f;

        public AssetBundle AssetBundle { get; set; }

        public bool HasAssetLoaded { get; set; }



        public bool HasReference { get { return referencedBy.Count > 0; } }


        public bool Released { get { return released; } }

        public float HitTime { get { return hitTime; } }

        public bool IsSceneBundle { get; set; }

        public int AssetCount { get { return assetList.Count; } }

        public HashSet<BundleInfoResource> ReferencedBy { get { return referencedBy; } }

        public int ReleasePriority { get; set; }



        public bool SurvivalTimeExpired
        {
            get
            {
                return (Time.realtimeSinceStartup - hitTime > MinimalSurvivalTime);
            }
        }

        public bool CanReleaseOnLevel
        {
            get
            {
                if (ResourceBelongBundleInfo.BundlePersistent || ResourceBelongBundleInfo.AssetPersistent)
                    return false;
                for (int i = 0; i < assetList.Count; i++)
                {
                    if (!assetList[i].CanRelease(true))
                        return false;
                }
                return true;
            }
        }

        public AssetInfo this[string idx]
        {
            get
            {
                AssetInfo res;
                if (loadedAssets.TryGetValue(idx, out res))
                    return res;
                else
                    return null;
            }
        }

        public void AddDependency(BundleInfoResource bundle)
        {
            dependsOn.Add(bundle);
            bundle.referencedBy.Add(this);
            bundle.ResourceBelongBundleInfo.IsDependency = true;//被依赖的包永不能卸载AssetBundle
        }

        public void AddAsset(string name, AssetInfo asset)
        {
            if (!loadedAssets.ContainsKey(name))
            {
                loadedAssets[name] = asset;
                assetList.Add(asset);
            }
        }

        public bool RemoveAsset(string name, bool force, bool removeInBundleAsset = true)
        {
            AssetInfo asset = this[name];
            if (asset != null)
            {
                bool releaseAsset = asset.CanRelease(false);
                if (!releaseAsset)
                {
                    if (!force)
                        return true;
                    //Logger.GetFile(LogFile.Res).LogError(name + " is referenced by others");
                }
                asset.ReleaseAsset(releaseAsset, removeInBundleAsset);

                //loadedAssets.Remove(name);
                //assetList.Remove(asset);

                return true;
            }
            return false;
        }


        public void Touch()
        {
            hitTime = Time.realtimeSinceStartup;
            this.m_isBundleUsed = true;
        }

        /// <summary>
        /// Bundle被加载的时间
        /// </summary>
        public void TouchOnLoadBundle()
        {
            hitTime = Time.realtimeSinceStartup;
        }

        public bool ContainsAsset(string name)
        {
            return loadedAssets.ContainsKey(name);
        }

        /// <summary>
        /// 尝试释放Unity的AssetBundle
        /// </summary>
        public void ReleaseAssetBundle(bool forceRelease = false)
        {
            if (!ResourceBelongBundleInfo.BundlePersistent && !ResourceBelongBundleInfo.IsDependency && AssetBundle)
            {
                //只有没被引用才能卸载AssetBundle，
                if ((!HasReference && SurvivalTimeExpired) || forceRelease)
                {
                    AssetBundle.Unload(false);
                    AssetBundle = null;
                }
            }
        }

        internal void ReleaseRedudantInstance(bool releaseAll = false)
        {
            for (int i = 0; i < assetList.Count; i++)
            {
                assetList[i].ReleaseRedudantAsset(releaseAll);
            }
        }

        /// <summary>
        /// 释放Bundle以及其包含的Asset的资源
        /// </summary>
        /// <param name="forceRelease">无视Asset引用状况强制释放，用于场景切换</param>
        public void Release(bool forceRelease = false)
        {
            if (!ResourceModule.Instance.UseAssetBundleLoadFromFile)
                ReleaseAssetBundle(forceRelease);
            if (!released)
            {
                if ((CanRelease && !forceRelease) || (forceRelease && CanReleaseOnLevel))
                {
                    for (int i = 0; i < assetList.Count; i++)
                    {
                        bool withAsset = !AssetBundle;
                        assetList[i].ReleaseAsset(withAsset);
                    }
                    assetList.Clear();
                    loadedAssets.Clear();

                    for (int i = 0; i < dependsOn.Count; i++)
                    {
                        dependsOn[i].referencedBy.Remove(this);
                    }
                    dependsOn.Clear();

                    if (HasReference)
                    {
                        Debug.LogWarning(string.Format("Releasing bundle {0} while still referenced by {1} other bundles", Name, referencedBy.Count));
                    }
                    referencedBy.Clear();
                    released = true;
                    if (AssetBundle)
                    {
                        //如果Assetbundle未被卸载，说明是依赖的bundle，需手动删除
                        AssetBundle.Unload(true);
                        AssetBundle = null;
                    }
                }
            }
        }

        public void ReleaseInstances()
        {
            for (int i = 0; i < assetList.Count; i++)
            {
                if (assetList[i].NeedInstance)
                    assetList[i].Reference = 0;
            }
        }

        /// <summary>
        /// 资源所在的Bundle名
        /// </summary>
        public string Name
        {
            get { return ResourceBelongBundleInfo.BundleName; }
        }

        public int Size
        {
            get { return ResourceBelongBundleInfo.BundleSize; }
        }




        /// <summary>
        /// 是否可以被释放
        /// </summary>
        public bool CanRelease
        {
            get
            {
                if (IsSceneBundle || ResourceBelongBundleInfo.BundlePersistent || ResourceBelongBundleInfo.AssetPersistent)
                    return false;

                for (int i = 0; i < assetList.Count; i++)
                {
                    if (!assetList[i].CanRelease(false))
                        return false;
                }

                //只有Bundle被使用过了才能卸载，防止预加载的资源，还没等使用就被释放了
                return !HasReference && SurvivalTimeExpired && m_isBundleUsed;      
            }
        }

        /// <summary>
        /// 资源所属Bundle信息
        /// </summary>
        public RuntimeBundleInfo ResourceBelongBundleInfo
        {
            get { return this.m_resourceBelongBundleInfo; }
        }
    }
}
