/********************************************************************
	created:  2018-4-13 15:48:29
	filename: PackEntry.cs
	author:	  songguangze@outlook.com
	
	purpose:  资源打包入口
*********************************************************************/
using GOEditor;
using LitJson;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;
using FileUtil = EngineCore.EngineFileUtil;

namespace EngineCore.Editor
{
    public static class PackEntry
    {
        //所有打包配置
        private static HashSet<string> m_projectAllPackConfig = new HashSet<string>() {
            "audio_pack.pack.txt",
            "carton_pack.pack.txt",
            //"cartoon_video_pack.pack",
            "comic_tex_pack.pack.txt",
            "effect_pack.pack.txt",
            "game_config.pack.txt",
            "gui_pack.pack.txt",
            "jigsaw_tex_pack.pack.txt",
            "mat.pack.txt",
            "scene_tex.pack.txt",
            "scene_config.pack.txt",
            "scene_item.pack.txt",
            "scene_pack.pack.txt",
            "shader.pack.txt",
        };


        private static HashSet<string> _excludes = new HashSet<string> { };
        private static HashSet<string> _excludesReleaseFast = new HashSet<string> { };
        private static HashSet<string> m_onlyScene = new HashSet<string> { };

        //仅打包配置
        private static HashSet<string> m_onlyConfig = new HashSet<string>()
        {
            "audio_pack.pack.txt",
            "carton_pack.pack.txt",
            //"cartoon_video_pack.pack",
            "comic_tex_pack.pack.txt",
            "effect_pack.pack.txt",
            "gui_pack.pack.txt",
            "jigsaw_tex_pack.pack.txt",
            "mat.pack.txt",
            "scen_tex.pack.txt",
            "scene_config.pack.txt",
            "scene_item.pack.txt",
            "scene_pack.pack.txt",
            "shader.pack.txt",
        };

        public static HashSet<string> RawAsset = new HashSet<string>
        {
            "video_pack.pack.txt"
        };

        [MenuItem("EngineEditor/打包/Windows/打包所有资源-Development")]
        public static void PackAllWindows()
        {
            PackAllResourcesV5(BuildTarget.StandaloneWindows);
        }

        [MenuItem("EngineEditor/打包/Windows/打包所有资源-Release")]
        public static void PackAllWindowsRelease()
        {
            PackAllResourcesV5(BuildTarget.StandaloneWindows, true);
        }

        [MenuItem("EngineEditor/打包/Windows-打包配置表")]
        public static void PackConfig()
        {
            PackAllResourcesV5(BuildTarget.StandaloneWindows, false, m_onlyConfig);
        }

        [MenuItem("EngineEditor/打包/Android/打包所有资源-Development")]
        public static void PackAllAndroid()
        {
            PackAllResourcesV5(BuildTarget.Android);
        }

        [MenuItem("EngineEditor/打包/Android/打包所有资源-Release")]
        public static void PackAllAndroidRelease()
        {
            PackAllResourcesV5(BuildTarget.Android, true);
        }

        [MenuItem("EngineEditor/打包/Android-打包所有配置")]
        public static void PackAndroidConfig()
        {
            PackAllResourcesV5(BuildTarget.Android, false, m_onlyConfig);
        }

        [MenuItem("EngineEditor/打包/IOS-打包所有资源")]
        public static void PackAllIOS()
        {
            PackAllResourcesV5(BuildTarget.iOS);
        }




        [MenuItem("GOE/打包/打包所有资源(IOS)")]
        public static void PackAllIOSx()
        {
            ReleaseStripping.GetStrippingAssets();
            StrippingResourcesForRelease(BuildTarget.StandaloneWindows64, @"D:\workroot\conan\trunk\public\exp\exp_bundle\bundlemap.bytes");
        }



        private static bool isForRelease = false;
        static void PackAllResourcesV5(BuildTarget target, bool isReleaseBuild = false, HashSet<string> exc = null)
        {
            isForRelease = isReleaseBuild;

            //清空GENERATE目录
            ClearGenerate();

            //拷贝外部公共文件
            PreparePackFiles(target);

            AssetDatabase.Refresh();

            Dictionary<string, object> extra;
            string error;

            try
            {
                //todo:后期继续重构
                AssetBundleManifest buildManifest;
                if (!GOEPackUIHelperV5.PackAll(true, target, out error, exc, out extra, out buildManifest) || buildManifest == null)
                {
                    Debug.LogError("Pack failed:" + error);
                    Console.WriteLine("Pack failed:" + error);
                    throw new ApplicationException("Pack failed:" + error); // throw
                }

                string bundleMapPath = GetBuildBundleMapPath(target);

                bool doIndexSucceed = DoSqliteBundleIndexMap(bundleMapPath, target, buildManifest);

                //跟exp_bundle同目录
                string dynamicPath = PathConfig.GetDynamicIndicesBundlePath(target);

                if (Directory.Exists(dynamicPath))
                    EngineCore.EngineFileUtil.DeleteDirectory(dynamicPath);

                Directory.CreateDirectory(dynamicPath);
                EngineCore.EngineFileUtil.CopyDirectory(bundleMapPath, dynamicPath);


                //Release 资源裁减
                if (isReleaseBuild)
                {
                    bool isSuccessed = StrippingResourcesForRelease(target, bundleMapPath);
                    if (!isSuccessed)
                    {
                        Debug.LogError("打包" + GOEPackV5.GetPlatformName(target) + "release 失败,请配置AssetManifest.xls");
                        return;
                    }
                }
                AssetDatabase.Refresh();

                if (doIndexSucceed)
                    Debug.Log("打包" + GOEPackV5.GetPlatformName(target) + "完毕");
            }
            catch (Exception ex)
            {
                Console.WriteLine("pack error:");
                Console.WriteLine("打包" + GOEPackV5.GetPlatformName(target) + $"失败, {ex.Message}");

                throw;
            }
        }


        /// <summary>
        /// 构建基于SQLite的bundlemap
        /// </summary>
        /// <param name="buildTarget"></param>
        /// <param name="builtAssetBundleManifest"></param>
        /// <param name="bundleIndexMapPath"></param>
        /// <param name="fileNameCaseSensitive"></param>
        public static bool DoSqliteBundleIndexMap(string bundleIndexMapPath, BuildTarget buildTarget, AssetBundleManifest builtAssetBundleManifest = null, bool fileNameCaseSensitive = true)
        {
            try
            {
                FileUtil.ClearOrCreateEmptyFolder(bundleIndexMapPath);

                string assetsBuildPath = GOEPackUIHelperV5.GetBundleOutputPath(buildTarget);

                if (!builtAssetBundleManifest)
                {
                    string buildManifestBundlePath = Path.Combine(assetsBuildPath, PathConfig.BUILD_MANIFEST_BUNDLE);

                    //manifest实际上在AssetBundle里,.manifest是给外面看的(只有build出来的manifest有信息，别的都没有)
                    AssetBundle buildManifestBundle = AssetBundle.LoadFromFile(buildManifestBundlePath);
                    builtAssetBundleManifest = buildManifestBundle.LoadAsset<AssetBundleManifest>("assetbundlemanifest");
                    buildManifestBundle.Unload(false);
                }

                string currentBuildBundleMapDbPath = Path.Combine(assetsBuildPath, PathConfig.SQLITE_BUNDLE_MAP_NAME);

                BundleIndicesMap bundleIndexMap = new BundleIndicesMap(currentBuildBundleMapDbPath);
                bundleIndexMap.ClearBundleIndicesData();

                string[] allAssetBundleNames = builtAssetBundleManifest.GetAllAssetBundles();

                for (int i = 0; i < allAssetBundleNames.Length; ++i)
                {
                    string originBundlePath = Path.Combine(assetsBuildPath, allAssetBundleNames[i]);
                    AssetBundle buildBundle = AssetBundle.LoadFromFile(originBundlePath);
                    if (buildBundle == null)
                        throw new Exception($"no bundle:{Path.Combine(assetsBuildPath, allAssetBundleNames[i])}");

                    string bundleHash = builtAssetBundleManifest.GetAssetBundleHash(allAssetBundleNames[i]).ToString();

                    string[] bundleIncludeAssets = buildBundle.GetAllAssetNames();
                    string[] bundleIncludeScenes = buildBundle.GetAllScenePaths();

                    buildBundle.Unload(true);

                    int bundleFileSize = (int)FileUtil.GetFileLength(Path.Combine(assetsBuildPath, allAssetBundleNames[i]));

                    string[] bundleAssets = new string[bundleIncludeAssets.Length + bundleIncludeScenes.Length];

                    int assetIndex = 0;
                    for (int j = 0; j < bundleIncludeAssets.Length; ++j)
                        bundleAssets[assetIndex++] = fileNameCaseSensitive ?
                            Path.GetFileName(FileUtil.GetFileRealNameByFuzzyCaseInsensitivePath(bundleIncludeAssets[j])) : Path.GetFileName(bundleIncludeAssets[j]);

                    for (int j = 0; j < bundleIncludeScenes.Length; ++j)
                        bundleAssets[assetIndex++] = fileNameCaseSensitive ?
                            Path.GetFileName(FileUtil.GetFileRealNameByFuzzyCaseInsensitivePath(bundleIncludeScenes[j])) : Path.GetFileName(bundleIncludeScenes[j]);

                    string[] bundleDependencies = builtAssetBundleManifest.GetAllDependencies(allAssetBundleNames[i]);
                    for (int j = 0; j < bundleDependencies.Length; ++j)
                    {
                        string dependencyFullPath = Path.Combine(assetsBuildPath, bundleDependencies[j]);
                        dependencyFullPath = fileNameCaseSensitive ? FileUtil.GetFileRealNameByFuzzyCaseInsensitivePath(dependencyFullPath) : dependencyFullPath;

                        bundleDependencies[j] = Path.GetFileName(dependencyFullPath);
                    }
                    bundleIndexMap.AddBundleIndicesByTransaction(allAssetBundleNames[i], bundleHash, bundleDependencies, bundleAssets, bundleFileSize);

                    //AssetBundle HashName
                    string bundleHashName = Path.Combine(bundleIndexMapPath, $"{bundleHash}{PathConfig.BUNDLE_POSTFIX}");

                    File.Copy(originBundlePath, bundleHashName);
                }
                AssetBundle.UnloadAllAssetBundles(true);

                bundleIndexMap.ForceCommit();

                string bundleMapDbDstPath = Path.Combine(bundleIndexMapPath, PathConfig.SQLITE_BUNDLE_MAP_NAME);
                File.Copy(currentBuildBundleMapDbPath, bundleMapDbDstPath);

                using (FileStream fs = new FileStream(bundleMapDbDstPath, FileMode.Open))
                {
                    System.Security.Cryptography.SHA1 sha = System.Security.Cryptography.SHA1.Create();
                    byte[] bundleMapHash = sha.ComputeHash(fs);

                    string bundlemapHashFilePath = Path.Combine(bundleIndexMapPath, PathConfig.SQLITE_BUNDLE_MAP_HASH);

                    using (FileStream fs2 = new FileStream(bundlemapHashFilePath, FileMode.CreateNew))
                        fs2.Write(bundleMapHash, 0, bundleMapHash.Length);

                    bundleIndexMap.SetBundleMapInfo(BundleIndicesMap.BundleMapKey.BUNDLEMAP_IDENTIFY, Encoding.UTF8.GetString(bundleMapHash));

                    bundleIndexMap.Dispose();
                }


                //Copy To StreamingAssets
                if (Directory.Exists(PathConfig.CODE_STREAMINGASSET_PATH))
                    File.Copy(currentBuildBundleMapDbPath, Path.Combine(PathConfig.CODE_STREAMINGASSET_PATH, PathConfig.SQLITE_BUNDLE_MAP_NAME), true);

                //只有Windows的Bundle需要拷贝
                //if (buildTarget == BuildTarget.StandaloneWindows64 || buildTarget == BuildTarget.StandaloneWindows || buildTarget == BuildTarget.StandaloneOSX)
                //    CopyGOEBundleToPath(bundleIndexMapPath);
                //else
                //{
                //Copy RawAssets
                string rawAssetDirPath = Path.Combine(assetsBuildPath, PathConfig.RAW_ASSET_PATH);
                DirectoryInfo rawAssetDir = new DirectoryInfo(rawAssetDirPath);
                if (rawAssetDir.Exists)
                {
                    FileInfo[] rawAssetFileInfoList = rawAssetDir.GetFiles();
                    foreach (FileInfo rawAssetFile in rawAssetFileInfoList)
                    {
                        string dstPath = Path.Combine(bundleIndexMapPath, rawAssetFile.Name);
                        rawAssetFile.CopyTo(dstPath);
                    }
                }
                //}


                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        /// <summary>
        /// 准备打包的文件
        /// </summary>
        /// <param name="isRelease"></param>
        private static void PreparePackFiles(BuildTarget buildtarget, bool isRelease = false)
        {
            FileInfo[] filesInDirectory = FileUtil.GetFilesInDirectory(PathConfig.CLIENT_CONFIG, "*.*", SearchOption.AllDirectories);

            foreach (var f in filesInDirectory)
            {
                if (f.Name.Contains(".svn"))
                    continue;

                if (!Directory.Exists(PathConfig.CONFIG_DATA))
                    Directory.CreateDirectory(PathConfig.CONFIG_DATA);

                f.CopyTo(PathConfig.CONFIG_DATA + "/" + f.Name, true);
            }

            AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate);
        }

        /// <summary>
        /// 清空GENERATE目录
        /// </summary>
        private static void ClearGenerate()
        {
            DirectoryInfo devPath = new DirectoryInfo(PathConfig.GENERATE_PATH);
            if (!devPath.Exists)
                devPath.Create();

            FileInfo[] files = devPath.GetFiles("*.*");
            foreach (FileInfo file in files)
                file.Delete();
        }

        /// <summary>
        /// Release 资源剪裁
        /// </summary>
        /// <param name="buildTarget"></param>
        /// <param name="bundleMapPath"></param>
        /// <returns></returns>
        private static bool StrippingResourcesForRelease(BuildTarget buildTarget, string bundleMapPath)
        {
            BundleIndicesMap fullBundleIndexMap = new BundleIndicesMap($"{PathConfig.CODE_STREAMINGASSET_PATH}/bundlemap.bytes");
            List<BundleIndexItemInfo> allBundleInfoList = fullBundleIndexMap.GetAllBundleIndexItemList();
            fullBundleIndexMap.Dispose();

            Dictionary<string, BundleIndexItemInfo> allBundleDict = allBundleInfoList.ToDictionary(bundleIndexItem => bundleIndexItem.BundleName);
            List<string> allBundleKeyList = allBundleDict.Keys.ToList();

            Dictionary<string, object> existStripSet = ReleaseStripping.GetStrippingAssets();
            HashSet<string> appendSet = new HashSet<string>();

            string strippintReportPath = GetStrippingReportPath(buildTarget);

            for (int i = 0; i < allBundleKeyList.Count; ++i)
            {
                string bundleName = allBundleKeyList[i];

                if (existStripSet.ContainsKey(bundleName))
                {
                    existStripSet.Remove(bundleName);
                }
                else
                {
                    appendSet.Add(bundleName);
                }
            }

            if (appendSet.Count == 0 && existStripSet.Count == 0)
            {
                Dictionary<string, object> strippingDict = ReleaseStripping.GetStrippingAssets();
                string buildOutputPath = GetBuildBundleMapPath(buildTarget);
                BundleIndicesMap bundlemapDb = new BundleIndicesMap($"{buildOutputPath}/bundlemap.bytes");
                Dictionary<string, BundleIndexItemInfo> allBundleMapDict = bundlemapDb.GetAllBundleIndexItemList().ToDictionary(item => item.BundleName);
                bundlemapDb.BundleIndicesMapConn.BeginTransaction();
                foreach (var pair in strippingDict)
                {
                    ReleaseStrippingForTheChief.AssetManifestConfig assetManifestConfig = (ReleaseStrippingForTheChief.AssetManifestConfig)pair.Value;

                    if ((ReleaseStrippingForTheChief.AssetLevel)assetManifestConfig.AssetLevel != ReleaseStrippingForTheChief.AssetLevel.BUILDIN)
                    {
                        string strippingBundlePath = Path.Combine(buildOutputPath, allBundleMapDict[pair.Key].BundleHashName);
                        File.Delete(strippingBundlePath);

                        bundlemapDb.RemoveBundleIndices(pair.Key);
                    }
                }

                bundlemapDb.Dispose();

                File.Delete(strippintReportPath);

                return true;
            }
            else
            {
                List<string> obsoluteBundleList = existStripSet.Keys.ToList();
                List<string> appendBundleList = appendSet.ToList();

                StrippingReport report = new StrippingReport();
                report.ObsoluteAssets.AddRange(obsoluteBundleList);
                report.NotIncludeAssets.AddRange(appendBundleList);

                using (FileStream fs = new FileStream(strippintReportPath, FileMode.Create))
                {
                    byte[] writeBuffer = Encoding.Default.GetBytes(JsonMapper.ToJson(report));
                    fs.Write(writeBuffer, 0, writeBuffer.Length);
                }

                return false;
            }
        }


        private class StrippingReport
        {
            public List<string> ObsoluteAssets = new List<string>();
            public List<string> NotIncludeAssets = new List<string>();
        }


        /// <summary>
        /// 处理最后要把资源拷贝到其他的目录
        /// </summary>
        private static void CopyGOEBundleToPath(string orignalGOEBundleListPath)
        {
            //拷贝到程序的开发目录（如果有目录）
            if (Directory.Exists(PathConfig.CODE_ROOT))
            {
                DirectoryInfo devPath = new DirectoryInfo(PathConfig.BIN_RES_DIR_WINDOWS_DEV);
                if (!devPath.Exists)
                    devPath.Create();

                FileInfo[] files = devPath.GetFiles("*.*");
                foreach (FileInfo file in files)
                    file.Delete();

                EngineFileUtil.CopyDirectory(orignalGOEBundleListPath, PathConfig.BIN_RES_DIR_WINDOWS_DEV);
            }
        }

        /// <summary>
        /// 获取目标平台bundle位置
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        private static string GetBuildBundleMapPath(BuildTarget target)
        {
            string buildIndexBundlePath = PathConfig.CODE_STREAMINGASSET_PATH;
            switch (target)
            {
                case BuildTarget.StandaloneWindows:
                case BuildTarget.StandaloneWindows64:
                    buildIndexBundlePath = PathConfig.RELEASE_WINDOWS_DIR;
                    break;
                case BuildTarget.Android:
                case BuildTarget.iOS:
                    buildIndexBundlePath = PathConfig.CODE_STREAMINGASSET_PATH;
                    break;
            }
            return buildIndexBundlePath;
        }

        private static string GetStrippingReportPath(BuildTarget buildTarget)
        {
            string reportPath = string.Empty;
            switch (buildTarget)
            {
                case BuildTarget.StandaloneWindows:
                case BuildTarget.StandaloneWindows64:
                    reportPath = string.Format("{0}/{1}", PathConfig.WINDOWS_BUNDLE_RELATIVE_OUTPUTPATH, PathConfig.STRIPPING_REPORT);
                    break;
                case BuildTarget.Android:
                    reportPath = string.Format("{0}/{1}", PathConfig.ANDROID_BUNDLE_RELATIVE_OUTPUTPATH, PathConfig.STRIPPING_REPORT);
                    break;
                case BuildTarget.iOS:
                    reportPath = string.Format("{0}/{1}", PathConfig.IOS_BUNDLE_RELATIVE_OUTPUTPATH, PathConfig.STRIPPING_REPORT);
                    break;
            }

            return reportPath;
        }


    }
}