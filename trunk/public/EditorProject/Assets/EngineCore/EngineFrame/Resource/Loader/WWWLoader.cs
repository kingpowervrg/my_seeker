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
#if UNITY_EDITOR
    public
#else
    internal
#endif
        class WWWLoader
    {
        protected string mName = string.Empty;
        protected float mBeginLoadTime = 0;
        private LoaderState mState = LoaderState.None;
        protected WWWType mWWWType = WWWType.AssetBundle;

        private Resource mResource = null;

        private LoadPriority mPriority = LoadPriority.Default;

        protected string mRealName = string.Empty;

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
        public static string RES_PATH_PREFIX_WIN = "/../../../bin/res/";

        /// <summary>
        /// bin/res下的bundle是否是原始（未MD5）的名称
        /// </summary>
        public static bool RES_BUNDLE_ORIGINAL_NAME = false;
        /// <summary>
        /// 异步从文件加载AssetBundle
        /// </summary>
        /// <param name="name">bundle文件名</param>
        /// <param name="inStream">是否从StreamingAssets加载</param>
        /// <param name="fromWeb"></param>
        /// <returns></returns>
        public static AssetBundleCreateRequest CreateAssetbundleAsync(string name, bool inStream = false, bool fromWeb = false)
        {
            if (fromWeb)
                throw new NotSupportedException();
            string path = GetPath(name, inStream, fromWeb, false);
            var res = AssetBundle.LoadFromFileAsync(path);
            return res;
        }

        static string GetPath(string name, bool inStream = false, bool fromWeb = false, bool isWWW = true)
        {
            string path = null;
            if (fromWeb)
            {
                return SysConf.GAME_RES_URL + "/" + name;
            }
            if (EngineDelegateCore.DynamicResource)
            {
                path = Application.persistentDataPath + "/" + name;
                if (File.Exists(path))
                {
                    if (isWWW)
                        path = "file:///" + path;
                    return path;
                }
            }
            if (inStream || Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer)
            {
                path = Application.streamingAssetsPath + "/" + name;
                if (Application.platform != RuntimePlatform.Android && isWWW)
                    path = "file:///" + path;
            }
            else
            {
                if (!EngineDelegateCore.IsEditorRuntime)
                {
                    if (!EngineDelegateCore.TestMobile)
                        path = Application.dataPath + RES_PATH_PREFIX_WIN + name;
                    else
                        path = Application.dataPath + "/StreamingAssets/" + name;
                }
                else
                {
                    path = Application.dataPath + RES_PATH_PREFIX_WIN + name;
                }

                if (isWWW)
                    path = "file:///" + path;
            }
            return path;
        }
        /// <summary>
        /// 禁用Unity的Cache，pc上等需要双开的环境需要禁用
        /// </summary>
        public static bool DisableLoadCache { get; set; }
        public static WWW CreateWWW(string name, bool isRawResource, bool inStream = false, bool fromWeb = false)
        {
            string path = GetPath(name, inStream, fromWeb);
            if (fromWeb || EngineDelegateCore.DynamicResource)
                return createWWW(path, true);
            return createWWW(path, isRawResource);
        }

        private static WWW createWWW(string path, bool isRaw)
        {
            WWW www = isRaw || DisableLoadCache ? new WWW(path) : WWW.LoadFromCacheOrDownload(path, 1);
            if (ResourceMgr.Instance().LowAsyncLoadPriority)
                www.threadPriority = UnityEngine.ThreadPriority.Low;
            else
                www.threadPriority = UnityEngine.ThreadPriority.High;
            return www;
        }

        public static WWW CreateWWWForDynamic(string name)
        {
            string path = SysConf.GAME_RES_URL + "/" + name;
            return createWWW(path, true);
        }
    }
}
