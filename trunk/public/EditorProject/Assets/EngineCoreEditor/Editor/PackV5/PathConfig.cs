/********************************************************************
	created: 2014-9-3 15:22:40
	filename:PathConfig.cs
	author:	 songguangze@outlook.com
	
	purpose: 打包工程各种路径配置
*********************************************************************/
using System.IO;
using UnityEditor;
using UnityEngine;

#if UNITY_EDITOR
namespace EngineCore.Editor
{
    /// <summary>
    /// 打包相关路径配置
    /// </summary>
    public static class PathConfig
    {
        //trunk目录
        public static string PROJECT_ROOT = Path.Combine(Application.dataPath, "../../..");

        /* 资源生成bin 目录(客户端直接读取的目录) */
        public static string BIN_RES_DIR_WINDOWS = Path.Combine(PROJECT_ROOT, @"bin/res");
        public static string BIN_RES_DIR_ANDROID = string.Empty;
        public static string BIN_RES_DIR_IOS = string.Empty;

        //只给程序使用的资源路径
        public static string CODE_ROOT = Path.Combine(PROJECT_ROOT, "code");
        public static string BIN_RES_DIR_WINDOWS_DEV = Path.Combine(PROJECT_ROOT, "code/bin/res");

        //IOS直接生成到public\expIOS\iPhoneScript目录，只有程序生成及打包的
        public static string CODE_STREAMINGASSET_PATH = Path.Combine(CODE_ROOT, "client/Seeker/Assets/StreamingAssets");

        /* public 目录 */
        public static string PROJECT_PUBLIC = Path.Combine(PROJECT_ROOT, "public");

        //bytes 文件路径
        public static string CONFIG_DATA = "Assets/Res/Config";

        //generate 文件路径
        public static string GENERATE_PATH = "Assets/Res/Gui/generate";

        //导航网络生成工具路径
        public static string NAVIMESH_EXPORTER_ROOT = "ExportedObj";

        //客户端其它配置文本目录(language.txt 屏蔽字 ..)
        public static string CLIENT_CONFIG = Path.Combine(PROJECT_PUBLIC, "clientConfig");

        //生成AssetBundle 目录配置
        public static string WINDOWS_BUNDLE_OUTPUTPATH = Path.Combine(PROJECT_PUBLIC, "exp");
        public static string ANDROID_BUNDLE_OUTPUTPATH = Path.Combine(PROJECT_PUBLIC, "expAndroid");
        public static string IOS_BUNDLE_OUTPUTPATH = Path.Combine(PROJECT_PUBLIC, "expIOS");

        public static string BUNDLE_DIR_NAME = "exp_bundle";

        //相对路径
        public const string WINDOWS_BUNDLE_RELATIVE_OUTPUTPATH = "../exp";
        public const string ANDROID_BUNDLE_RELATIVE_OUTPUTPATH = "../expAndroid";
        public const string IOS_BUNDLE_RELATIVE_OUTPUTPATH = "../expIOS";

        public const string ANDROID_DYNAMIC_INDEX_BUNDLE_PATH = "../expAndroid/dynamic";
        public const string IOS_DYNAMIC_INDEX_BUNDLE_PATH = "../expIOS/dynamic";
        public const string WINDOWS_DYNAMIC_INDEX_BUNDLE_PATH = "../exp/dynamic";

        //Release 的资源目录
        public const string RELEASE_MAC_DIR = "../../release/client/bin/res";
        public const string RELEASE_WINDOWS_DIR = "../../code/bin/res";

        public const string SQLITE_BUNDLE_MAP_NAME = "bundlemap.bytes";


        /************New Path****************/

        //Bundle之后自动生成包含Manifest的Bundle(没有后缀)
        public const string BUILD_MANIFEST_BUNDLE = "exp_bundle";

        //bundle后缀
        public const string BUNDLE_POSTFIX = ".bundle";

        //Bundlemap HashCode
        public const string SQLITE_BUNDLE_MAP_HASH = "bundlemap.hash";

        //裁减资源
        public const string RELEASE_STRIPPING_SEARCH_OPTION = "*.prefab|*.png|*.jpg|*.tga|*.mat|*.shader|*.json|*.exr|*.unity";

        public const string STRIPPING_REPORT = "release_strip_report.json";

        public const string RAW_ASSET_PATH = "RawAssets";


        /// <summary>
        /// 获取打包资源的动态更新目录
        /// </summary>
        /// <param name="buildTarget"></param>
        /// <remarks>都使用相对目录</remarks>
        public static string GetDynamicIndicesBundlePath(BuildTarget buildTarget)
        {
            string dynamicIndicesPath = string.Empty;
            switch (buildTarget)
            {
                case BuildTarget.Android:
                    dynamicIndicesPath = ANDROID_DYNAMIC_INDEX_BUNDLE_PATH;
                    break;
                case BuildTarget.iOS:
                    dynamicIndicesPath = IOS_DYNAMIC_INDEX_BUNDLE_PATH;
                    break;
                case BuildTarget.StandaloneWindows:
                case BuildTarget.StandaloneWindows64:
                    dynamicIndicesPath = WINDOWS_DYNAMIC_INDEX_BUNDLE_PATH;
                    break;
            }

            return dynamicIndicesPath;
        }
    }
}
#endif
