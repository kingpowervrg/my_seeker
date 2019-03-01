/*********************************************************************
    created:  2018-11-16 12:36:11
	filename: AssetInfo.cs
    author:	  songguangze@outlook.com


    purpose:  资源对象信息，承载运行时资源信息
*********************************************************************/
using System;
using UnityEngine;

namespace EngineCore
{
    /// <summary>
    /// 资源对象信息
    /// </summary>
    public class AssetInfo
    {
        private int mReference = 0;
        public UnityEngine.Object Asset;
        string mName, mBundleName;
        bool needInstanceGet = false;
        bool needInstance = false;

        /// <summary>
        /// 对象池,维护当前对象所有拷贝副本
        /// </summary>
        private MemoryPool<UnityEngine.Object> m_objectInstancePool = new MemoryPool<UnityEngine.Object>(30);

        /// <summary>
        /// GC后如果存活，保留的实例数
        /// </summary>
        const int OptimalCacheSize = 5;
        public string Name
        {
            get { return mName; }
        }
        public string BundleName
        {
            get { return mBundleName; }

        }

        internal LoadPriority Priority { get; set; }
        internal int Reference
        {
            set
            {
                mReference = value;
            }
            get { return mReference; }
        }

        internal bool HasNoReference { get { return mReference <= 0; } }

        internal bool AssetValid { get { return Asset; } }

#if DEBUG_BUNDLE_CACHE
        string getAssetName;
        String GetAssetName
        {
            get
            {
                if (getAssetName == null)
                    getAssetName = string.Format("GOEAsset.GetAsset:{0}.{1}", BundleName, Name);
                return getAssetName;
            }
        }
#endif
        internal bool GetAsset(Action<string, UnityEngine.Object> callback)
        {
#if DEBUG_BUNDLE_CACHE
            Profiler.BeginSample(GetAssetName);
#endif
            bool fromPool;
            var res = GetAsset(out fromPool);
#if DEBUG_BUNDLE_CACHE
            Profiler.EndSample();
#endif

            if (callback != null)
            {
#if DEBUG_BUNDLE_CACHE
                Profiler.BeginSample(callback.Method.ToString());
#endif
                callback(Name, res);
#if DEBUG_BUNDLE_CACHE
                Profiler.EndSample();
#endif
            }
            return fromPool;
        }


        public int PoolCount { get { return m_objectInstancePool.Count; } }
        internal UnityEngine.Object GetAsset(out bool fromPool)
        {
            fromPool = false;
            if (AssetValid)
            {
                Reference++;
                UnityEngine.Object obj = m_objectInstancePool.Alloc();
                if (obj != null)
                {
                    fromPool = true;
                    if (obj is GameObject)
                        (obj as GameObject).SetActive(true);


                    return obj;
                }
                return InstanceAsset(Asset, Name);
            }
            else
                return null;
        }

        internal bool NeedInstance
        {
            get
            {
                return GetNeedInstance(Asset, Name);
            }
        }
        /// <summary>
        /// 对于取的东西是否instantiate，可以自己设定规则
        /// </summary>
        internal UnityEngine.Object InstanceAsset(UnityEngine.Object obj, string name)
        {
            if (GetNeedInstance(obj, name))
            {
                return GameObject.Instantiate(obj);
            }
            return obj;
        }

        private bool GetNeedInstance(UnityEngine.Object obj, string name)
        {
            if (obj == null)
                return false;
            if (!needInstanceGet)
            {
                needInstanceGet = true;
                if (name.EndsWithFast(".exr"))
                {
                    needInstance = false;
                }
                else
                {
                    if (obj is GameObject)
                    {
                        if (EngineDelegateCore.PrefabUnInstantiateRule != null && EngineDelegateCore.PrefabUnInstantiateRule(obj as GameObject))
                            needInstance = false;
                        else
                            needInstance = true;
                    }
                    else
                        needInstance = false;
                }
            }
            return needInstance;
        }

        internal static bool IsNeedInstance(UnityEngine.Object obj, string name)
        {
            if (name.EndsWithFast(".exr"))
            {
                return false;
            }
            else
            {
                if (obj is GameObject)
                {
                    if (EngineDelegateCore.PrefabUnInstantiateRule != null && EngineDelegateCore.PrefabUnInstantiateRule(obj as GameObject))
                        return false;
                    return true;
                }
                else
                    return false;
            }
        }

        internal void ReleaseAssetReference(UnityEngine.Object obj)
        {
            if (obj)
            {
                if (Reference > 0)
                {
                    Reference--;
                    if (obj is GameObject)
                    {
                        (obj as GameObject).SetActive(false);
                        (obj as GameObject).transform.SetParent(null);
                    }
                    if (GetNeedInstance(obj, Name))
                    {
                        if (m_objectInstancePool.Free(obj))
                        {
                            return;
                        }
                        UnityEngine.Object.Destroy(obj);
                    }
                }
                else if (GetNeedInstance(obj, Name))
                {
                    UnityEngine.Object.Destroy(obj);
                }
            }
        }

        internal AssetInfo(UnityEngine.Object obj, string name, string bundleName)
        {
            mName = name;
            mBundleName = bundleName;
            Asset = obj;
        }

        private bool gotConfig;
        ResConfig config = null;
        internal bool CanRelease(bool onLevel)
        {
            if (!Asset)
                return true;
            if (!gotConfig)
            {
                if (Asset is GameObject)
                {
                    if ((Asset as GameObject).activeSelf)
                        config = (Asset as GameObject).GetComponent<ResConfig>();
                    else
                    {
                        ResConfig[] configs = (Asset as GameObject).GetComponentsInChildren<ResConfig>(true);
                        if (configs.Length > 0)
                            config = configs[0];
                    }
                }
                gotConfig = true;
            }
            if (Asset is Font)
                return false;
            /* if (onLevel)
             {
                 return (config == null || config.ReleaseOnLevelLoaded);
             }
             else
             {*/
            return HasNoReference;
            //}
        }

        internal void ReleaseRedudantAsset(bool releaseAll)
        {
            int targetCnt = releaseAll ? 0 : OptimalCacheSize;
            while (m_objectInstancePool.Count > targetCnt)
            {
                UnityEngine.Object obj = m_objectInstancePool.Alloc();
                if (obj != null)
                {
                    UnityEngine.Object.Destroy(obj);
                }
                else
                {
                    break;
                }
            }
        }

        internal void ReleaseAsset(bool withAsset, bool withInbundleAsset = true)
        {
            if (!AssetValid)
                return;
            while (true)
            {
                UnityEngine.Object obj = m_objectInstancePool.Alloc();
                if (obj != null)
                {
                    UnityEngine.Object.Destroy(obj);
                }
                else
                {
                    break;
                }
            }
            bool isIneditor = false;
            if (!withAsset || isIneditor)
                return;
            if (Asset)
            {
                if (Asset is GameObject)
                {
                    GameObject.DestroyImmediate(Asset, true);
                    m_objectInstancePool.Dispose();
                }
                else
                {
#if !UNITY_EDITOR
                    if(Asset is Sprite)
                    {
                        var tex = ((Sprite)Asset).texture;
                        if (tex)
                            Resources.UnloadAsset(tex);
                    }
                    Resources.UnloadAsset(Asset);
#endif
                }
            }

            Asset = null;
        }
    }
}
