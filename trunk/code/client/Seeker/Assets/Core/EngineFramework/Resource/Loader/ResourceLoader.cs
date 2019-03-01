using System.Collections.Generic;
using UnityEngine;

namespace EngineCore
{
    //#if UNITY_EDITOR
    //    public
    //#else
    //    internal
    //#endif
    public class ResourceLoader : WWWLoader
    {
        public enum MultiLoaderState
        {
            None,
            Preload,
            Load,
            Done,
        }

        internal static int MaxLoadTime = 30;
        private WWW loadWWW;
        AssetBundleCreateRequest abRequest;
        private float mLastProgress = 0;

        private MultiLoaderState mState = MultiLoaderState.None;

        public bool IsFromWeb = false;
        public bool IsFromStream = false;

        //依赖包Set,在加载时要保证依赖包有效
        private HashSet<string> m_pendingDependencies = new HashSet<string>();

        public bool IsLoading { get { return mState == MultiLoaderState.Load; } }

        public bool IsRawResource { get; set; }

        public bool IsDestroyed { get; set; }

        public bool IsDone
        {
            get
            {
                return this.IsFinished();
            }
        }

        public float Progress
        {
            get
            {
                return this.GetProgerss();
            }
        }

        public WWW WWW
        {
            get
            {
                if (!IsDone)
                {
                    //Logger.GetFile(LogFile.Res).LogError("www Get where !IsDone");
                }

                return loadWWW;
            }
        }

        public string Text
        {
            get
            {
                if (!IsDone)
                {
                    //Logger.GetFile( LogFile.Res ).LogError( "Text.Get where !IsDone" );
                }

                return loadWWW.text;
            }
        }

        public AudioClip AudioClip
        {
            get
            {
                if (!IsDone)
                {
                    //Logger.GetFile( LogFile.Res ).LogError( "AudioClip.Get where !IsDone" );
                }

                return loadWWW.GetAudioClip();
            }
        }

        public byte[] Byte
        {
            get
            {
                if (!IsDone)
                {
                    //Logger.GetFile( LogFile.Res ).LogError( "Byte.Get where !IsDone" );
                }

                return loadWWW.bytes;
            }
        }
        AssetBundle bundle;

        public AssetBundle AssetBundle
        {
            get
            {
                if (!IsDone)
                {
                    //Logger.GetFile( LogFile.Res ).LogError( "AssetBundle.Get where !IsDone" );
                }
                if (loadWWW != null)
                {
                    //bundle只能被获取一次
                    if (bundle == null)
                        bundle = loadWWW.assetBundle;
                }
                if (abRequest != null)
                {
                    if (bundle == null)
                        bundle = abRequest.assetBundle;
                }
                return bundle;
            }
        }

        /// <summary>
        /// 是否加载完成
        /// </summary>
        /// <returns></returns>
        private bool IsFinished()
        {
            return mState == MultiLoaderState.Done;
        }


        private float fakeValue = 0;
        private float GetProgerss()
        {
            if (IsFinished())
                return 1;
            float loadCount = 0;
            if (loadWWW != null)
                loadCount = loadWWW.progress;
            return loadCount;
        }

        public void BeginLoad()
        {

#if DEBUG_BUNDLE_CACHE
            ResourceMgr.Instance().SampleLoadEvent(mName, 2);
#endif
            BundleIndexItemInfo item = GetAssetBelongBundleIndexItemInfo(mName, IsFromWeb);
            if (item == null)
                throw new System.Exception($"{mName} belong bundle not found");

            m_fileName = item.BundleHashName;
            mName = item.BundleName;

            string name = WWWUtil.RES_BUNDLE_ORIGINAL_NAME ? mName : m_fileName;

            if (ResourceModule.Instance.UseAssetBundleLoadFromFile && mWWWType == WWWType.AssetBundle)
            {
                //abRequest = WWWUtil.CreateAssetbundleAsync(name, IsFromStream, IsFromWeb);
                abRequest = CreateAsyncAssetBundleRequest(name);
            }
            else
                loadWWW = CreateWWWRequest(name);

            mBeginLoadTime = 0;
        }

        /// <summary>
        /// 获取Asset所在Bundle
        /// </summary>
        /// <param name="assetName"></param>
        /// <param name="fromWeb"></param>
        /// <returns></returns>
        public BundleIndexItemInfo GetAssetBelongBundleIndexItemInfo(string assetName, bool fromWeb = false)
        {
            BundleIndexItemInfo bundleIndexItemInfo = ResourceMgr.Instance.BundleMapAdapter.GetBundleIndexItemInfo(assetName);

            return bundleIndexItemInfo;
        }


        private bool IsAllDone()
        {
            if (ResourceModule.Instance.UseAssetBundleLoadFromFile && mWWWType == WWWType.AssetBundle)
                return abRequest != null && abRequest.isDone;
            else
                return loadWWW != null && loadWWW.isDone;
        }

        /// <summary>
        /// 加载依赖包
        /// </summary>
        void BeginPreload()
        {
            BundleIndexItemInfo loadBundleInfo = GetAssetBelongBundleIndexItemInfo(mName);
            m_pendingDependencies.Clear();
            if (loadBundleInfo != null)
            {
                foreach (string bundleDependency in loadBundleInfo.BundleDependencyArray)
                {
                    m_pendingDependencies.Add(bundleDependency);
                    ResourceModule.Instance.PreloadBundle(bundleDependency, OnPreloadBundle, Priority, true);
                }
            }
        }

        void OnPreloadBundle(string name, AssetBundle bundle)
        {
            if (m_pendingDependencies != null)
                m_pendingDependencies.Remove(name);
        }

        public void Update()
        {
            //第一步：加载依赖包
            if (mState == MultiLoaderState.None)
            {
                BeginPreload();
                NextState();
            }
            else if (mState == MultiLoaderState.Preload)        //第二步：依赖包都有效的情况下，加载本资源
            {
                if (m_pendingDependencies == null || m_pendingDependencies.Count == 0)
                {
                    BeginLoad();
                    NextState();
                }
            }
            if (mState == MultiLoaderState.Load)        //第三步：检查当前资源是否加载完成
            {
                if (IsAllDone())
                {
#if DEBUG_BUNDLE_CACHE
                    ResourceMgr.Instance().SampleLoadEvent(mName, 3);
#endif
                    NextState();
                }
                float pro = Progress;

                if (pro != mLastProgress)
                {
                    mLastProgress = pro;
                    mBeginLoadTime = 0;
                }
                else
                {
                    mBeginLoadTime += Time.deltaTime;
                }

                if (mBeginLoadTime >= MaxLoadTime)
                {
                    mState = MultiLoaderState.None;
                }
            }
        }

        protected void NextState()
        {
            mState += 1;
        }

        public void Destroy()
        {
            if (WWWType != WWWType.WWW && loadWWW != null)
            {
                loadWWW.Dispose();
                loadWWW = null;
            }
            if (abRequest != null)
                abRequest = null;
            mState = MultiLoaderState.None;
            IsDestroyed = true;
        }


        /// <summary>
        /// 创建异步AssetBundle 请求
        /// </summary>
        /// <returns></returns>
        private AssetBundleCreateRequest CreateAsyncAssetBundleRequest(string assetBundleName)
        {
            string assetPath = ResourceMgr.Instance.RuntimeAssetPathResolver.GetPath(assetBundleName, false);

            return AssetBundle.LoadFromFileAsync(assetPath);
        }

        /// <summary>
        /// 创建WWW 请求
        /// </summary>
        /// <param name="assetName"></param>
        /// <returns></returns>
        private WWW CreateWWWRequest(string assetName)
        {
            string assetPath = ResourceMgr.Instance.RuntimeAssetPathResolver.GetPath(assetName, true);

            return new WWW(assetPath);
        }
    }
}
