using System;
using System.IO;
using UnityEngine;

namespace EngineCore
{
    public enum LoaderState
    {
        None,
        Wait,
        Loading,
        Over,
    }

    public enum WWWType
    {
        AssetBundle,
        Text,
        Audio,
        Byte,
        WWW,
    }

    public enum LoadPriority
    {
        Default = 0,
        MostPrior = 20,
        Prior = 10,
        HighPrior = 15,
        PostLoad = -10,
    }
}
namespace EngineCore
{
    //#if UNITY_EDITOR
    //    public
    //#else
    //    internal
    //#endif
    public class WWWLoader
    {
        protected string mName = string.Empty;
        protected float mBeginLoadTime = 0;
        private LoaderState mState = LoaderState.None;
        protected WWWType mWWWType = WWWType.AssetBundle;

        private Resource mResource = null;

        private LoadPriority mPriority = LoadPriority.Default;

        protected string m_fileName = string.Empty;

        public int Size
        {
            get { return mResource.Size; }
        }

        public LoaderState State
        {
            set { mState = value; }
            get { return mState; }
        }

        public WWWType WWWType
        {
            set { mWWWType = value; }
            get { return mWWWType; }
        }

        public LoadPriority Priority
        {
            set { mPriority = value; }
            get { return mPriority; }
        }

        public Resource Resource
        {
            set { mResource = value; }
            get { return mResource; }
        }

        public string Name
        {
            set { mName = value; }
            get { return mName; }
        }

        public float BeginLoadTime
        {
            get { return mBeginLoadTime; }
        }
    }
}
namespace EngineCore
{
    /// <summary>
    /// 下载Assetbundle的工具类
    /// </summary>
    public class WWWUtil
    {
        public static string RES_PATH_PREFIX_WIN = "../../../bin/res";

        /// <summary>
        /// bin/res下的bundle是否是原始（未MD5）的名称
        /// </summary>
        public static bool RES_BUNDLE_ORIGINAL_NAME = false;

        public static string GetVideoUrl(string name)
        {
            string path = Path.Combine(PathResolver.ApplicationPersistentDataPath, name);
            if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer)
            {
                if (File.Exists(path))
                    return name;
                else
                    return Path.Combine(Application.streamingAssetsPath, name);
            }
            else
                return $"{PathResolver.ApplicationDataPath}/{RES_PATH_PREFIX_WIN}/{name}";
        }

        /// <summary>
        /// 禁用Unity的Cache，pc上等需要双开的环境需要禁用
        /// </summary>
        public static bool DisableLoadCache { get; set; }

        private static WWW createWWW(string path, bool isRaw)
        {
            WWW www = isRaw || DisableLoadCache ? new WWW(path) : WWW.LoadFromCacheOrDownload(path, 1);
            if (ResourceMgr.Instance.LowAsyncLoadPriority)
                www.threadPriority = UnityEngine.ThreadPriority.Low;
            else
                www.threadPriority = UnityEngine.ThreadPriority.High;
            return www;
        }


        /// <summary>
        /// 获取StreamingAssets的www地址
        /// </summary>
        /// <param name="assetFileName"></param>
        /// <returns></returns>
        public static string GetStreamingAssetsPath(string assetFileName)
        {
            string assetInStreamingAssetPath = Path.Combine(PathResolver.ApplicationStreamingAssetsPath, assetFileName);
            switch (Application.platform)
            {
                case RuntimePlatform.IPhonePlayer:
                    assetInStreamingAssetPath = "file:///" + assetInStreamingAssetPath;
                    break;
                case RuntimePlatform.Android:
                case RuntimePlatform.WindowsPlayer:
                    break;
            }

            return assetInStreamingAssetPath;
        }
    }
}
