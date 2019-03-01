using EngineCore;
using EngineCore.Editor;
using GOEngine.Implement;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace GOEditor
{
    //打包UIHelper//
    [JsonObjectType(JsonObjectHelperTypes.PackV5)]
    public class GOEPackUIHelperV5 : JsonObjectUIHelper
    {
        public static string BUNDLE_DIR_NAME = "exp_bundle";


        bool dirty = false;
        bool mbStandaloneWindows, mbAndroid, mbiOS, mbMac64;
        internal static string output = "../exp", outputAndroid = "../expAndroid", outputIOS = "../expiOS";

        GOEPackV5 curPack;
        PackBundleSetting lastDetailBundle;
        List<string> lastDetailList;
        public GOEPackUIHelperV5()
        {

        }

        //是否需要先启动游戏//
        public override bool DoesNeedStarted() { return false; }
        //搜索目录还是文件//
        public override bool SearchForDir() { return false; }
        //搜索目录//
        public override string GetSearchDir() { return "Assets/EngineCoreEditor/Editor/PackV5/Definitions/"; }
        //扩展名//
        public override string GetFileExt() { return ".pack.txt"; }

        public override bool CanMultiple
        {
            get
            {
                return true;
            }
        }

        public override string MultipleActionName
        {
            get
            {
                return "打包";
            }
        }

        public override bool IsDirty
        {
            get
            {
                return dirty;
            }
        }

        bool IsPackPC
        {
            get { return mbStandaloneWindows; }
            set
            {
                if (mbStandaloneWindows != value)
                {
                    mbStandaloneWindows = value;
                    dirty = true;
                }
            }
        }

        bool IsPackAndroid
        {
            get { return mbAndroid; }
            set
            {
                if (mbAndroid != value)
                {
                    mbAndroid = value;
                    dirty = true;
                }
            }
        }

        bool IsPackiOS
        {
            get { return mbiOS; }
            set
            {
                if (mbiOS != value)
                {
                    mbiOS = value;
                    dirty = true;
                }
            }
        }

        bool IsPackMac64
        {
            get { return mbMac64; }
            set
            {
                if (mbMac64 != value)
                {
                    mbMac64 = value;
                    dirty = true;
                }
            }
        }

        string OutputDir
        {
            get { return output; }
            set
            {
                if (output != value)
                {
                    output = value;
                    dirty = true;
                }
            }
        }

        string OutputDirAndroid
        {
            get { return outputAndroid; }
            set
            {
                if (outputAndroid != value)
                {
                    outputAndroid = value;
                    dirty = true;
                }
            }
        }

        string OutputDirIOS
        {
            get { return outputIOS; }
            set
            {
                if (outputIOS != value)
                {
                    outputIOS = value;
                    dirty = true;
                }
            }
        }
        //new//
        public override bool CanNew() { return true; }
        public override object OnNew()
        {
            return new GOEPackV5();
        }
        //save//
        public override bool CanSave() { return true; }
        //delete//
        public override bool CanDelete() { return true; }
        //select//
        public override object OnSelect(string strFullName)
        {
            //清空当前数据//
            curPack = null;
            //mDep.Reset();

            //读文件//
            GOEPackV5 si = JsonUtil.ReadJsonObject(strFullName) as GOEPackV5;
            if (null != si)
            {
                curPack = si;
            }
            else
            {
                Debug.Log("parse error:");
            }

            return curPack;
        }

        public override Dictionary<string, string> EnumOptions(object target, string paramName)
        {
            if (target.GetType() == typeof(PackBundleSetting))
            {
                //打包//
                JsonFieldTypes pc = JsonFieldAttribute.GetFieldFlag(target, paramName);
                if (pc != JsonFieldTypes.Null)
                {
                    if (pc == JsonFieldTypes.PackType)
                    {
                        //分包方式//
                        return GOEPackV5.msArrayPackType;
                    }
                }
            }
            return null;
        }

        public override void OnLoadConfig(Dictionary<string, string> dic)
        {
            string val;
            if (dic.TryGetValue("PACK_PC", out val))
                mbStandaloneWindows = bool.Parse(val);
            if (dic.TryGetValue("PACK_ANDROID", out val))
                mbAndroid = bool.Parse(val);
            if (dic.TryGetValue("PACK_IOS", out val))
                mbiOS = bool.Parse(val);
            if (dic.TryGetValue("PACK_MAC", out val))
                mbMac64 = bool.Parse(val);

            if (!dic.TryGetValue("PACK_OUTPUT", out output))
                output = "../exp";
            if (!dic.TryGetValue("PACK_OUTPUT_ANDROID", out outputAndroid))
                outputAndroid = "../expAndroid";
            if (!dic.TryGetValue("PACK_OUTPUT_IOS", out outputIOS))
                outputIOS = "../expiOS";
        }

        public override void OnSaveConfig(Dictionary<string, string> dic)
        {
            dic["PACK_PC"] = mbStandaloneWindows.ToString();
            dic["PACK_ANDROID"] = mbAndroid.ToString();
            dic["PACK_IOS"] = mbiOS.ToString();
            dic["PACK_MAC"] = mbMac64.ToString();
            dic["PACK_OUTPUT"] = output;
            dic["PACK_OUTPUT_ANDROID"] = outputAndroid;
            dic["PACK_OUTPUT_IOS"] = outputIOS;
            dirty = false;
        }

        public override void MakeEditUI(object target)
        {
            GOEPackV5 pack = target as GOEPackV5;
            if (null == pack)
                return;

            GUILayout.BeginVertical();

            //打包//
            MakeMultipleActionUI();

            if (GUILayout.Button("打包"))
            {
                string error;
                Dictionary<string, object> e;
                AssetBundleManifest buildManifest;
                DoPack(false, getBestPackOrder()[0], out error, null, out e, out buildManifest);
            }

            //打包项编辑//
            GUILayout.Label("编辑Bundles");
            for (int i = 0; i < pack.PackItems.Count; i++)
            {
                PackBundleSetting bundle = pack.PackItems[i];
                GUILayout.BeginHorizontal();
                if (GUILayout.Button("X", GUILayout.Width(20)))
                {//两个宽度30的按钮＝一个宽度64的按钮//
                    //删除打包项//
                    pack.PackItems.RemoveAt(i);
                }
                GUILayout.Label(bundle.BundleName);

                //todo:如类型为场景则提供逐个打包选项//
                if (bundle.PackType == "Scene")
                {
                    //if (GUILayout.Button("列举场景", GUILayout.Width(140)))
                    //{
                    //    lastDetailBundle = bundle;
                    //    lastDetailList = bundle.BuildSrcFileList(pack.SrcDir);
                    //}

                    //GUILayout.EndHorizontal();
                    //if (lastDetailBundle == bundle)
                    //{
                    //    foreach (string scn in lastDetailList)
                    //    {
                    //        string scnFN = Path.GetFileName(scn);
                    //        if (GUILayout.Button(scnFN))
                    //        {
                    //            DoPackScene(scn);
                    //        }
                    //    }
                    //}
                }
                else
                    GUILayout.EndHorizontal();
            }
            //添加//
            if (GUILayout.Button("添加Bundle", GUILayout.Width(140)))
            {
                pack.PackItems.Add(new PackBundleSetting());
                //更新属性窗口//
                JsonObjectPropertyWindow wp = EditorWindow.GetWindow(typeof(JsonObjectPropertyWindow), false, "Property") as JsonObjectPropertyWindow;
                wp.Show();
                wp.SetCurStringInterface(pack, this);
            }

            GUILayout.EndVertical();
        }

        public override void MakeMultipleActionUI()
        {
            GUILayout.Label("打包平台");
            IsPackAndroid = GUILayout.Toggle(mbAndroid, "Android");
            IsPackiOS = GUILayout.Toggle(mbiOS, "iOS");
            IsPackPC = GUILayout.Toggle(mbStandaloneWindows, "PC版");
            IsPackMac64 = GUILayout.Toggle(mbMac64, "Mac");

            GUILayout.Label("输出目录");
            OutputDir = GUILayout.TextField(output);

            GUILayout.Label("Android输出目录");
            OutputDirAndroid = GUILayout.TextField(outputAndroid);

            GUILayout.Label("iOS输出目录");
            OutputDirIOS = GUILayout.TextField(outputIOS);
            //../../expV5
        }
        List<BuildTarget> getBestPackOrder()
        {
            BuildTarget[] allTargets = { BuildTarget.StandaloneWindows,
                                         BuildTarget.Android,
                                         BuildTarget.iOS,
                                         BuildTarget.StandaloneOSX
                                        };

            List<BuildTarget> packOrders = new List<BuildTarget>();
            //先打当前平台
            packOrders.Add(GOEPack.buildTarget);
            //再打其它平台
            for (int i = 0; i < allTargets.Length; i++)
            {
                if (allTargets[i] == GOEPack.buildTarget)
                    continue;
                packOrders.Add(allTargets[i]);
            }
            return packOrders;
        }


        bool DoPack(bool silent, BuildTarget target, out string error, HashSet<string> excludePackSet, out Dictionary<string, object> extra, out AssetBundleManifest buildManifest)
        {
            extra = new Dictionary<string, object>();
            buildManifest = null;

            BuildTargetGroup buildTargetGroup = ConvertBuildTarget(target);

            if (!EditorUserBuildSettings.SwitchActiveBuildTarget(buildTargetGroup, target))
            {
                error = "buildtarget platform : " + target.ToString();
                return false;
            }

            if (!silent && !UnityEditor.EditorUtility.DisplayDialog("确认打包", "此操作会对所有修改过的资源进行打包，继续吗？", "是", "否"))
            {

                error = null;
                return true;
            }

            AssetDatabase.SaveAssets();

            string[] files = Directory.GetFiles("Assets/EngineCoreEditor/Editor/PackV5/Definitions/", "*.pack.txt");

            List<AssetBundleBuild> builds = new List<AssetBundleBuild>();
            List<string> rawAssetList = new List<string>();

            string outputDir = GetBundleOutputPath(target);

            string currentSceneBeforeBuild = EditorSceneManager.GetActiveScene().path;

            //打开一个空场景
            EditorSceneManager.OpenScene("Assets/EmptyScene.unity");

            foreach (string packConfig in files)
            {
                bool shoudlSkip = excludePackSet != null && excludePackSet.Contains(Path.GetFileName(packConfig));
                if (shoudlSkip)
                    continue;

                //打包选项配置
                GOEPackV5 si = JsonUtil.ReadJsonObject(packConfig) as GOEPackV5;
                string packingResFolder = si.SrcDir;
                if (!Directory.Exists(packingResFolder))
                {
                    Debug.LogWarningFormat("{0} target resource folder:{1} not exist,ignore build", packConfig, packingResFolder);
                    continue;
                }

                if (si == null)
                {
                    error = "无法读取文件：" + packConfig;
                    if (!silent)
                        EditorUtility.DisplayDialog("打包错误", error, "确定");
                    return false;
                }

                List<AssetBundleBuild> b;
                HashSet<string> allAssetSet = null;

                if (!si.ConfigureAssetImporter(silent, out error, out b, extra, shoudlSkip, out allAssetSet))
                {
                    return false;
                }

                if (PackEntry.RawAsset.Contains(Path.GetFileName(packConfig)))
                {
                    rawAssetList.AddRange(allAssetSet);
                }
                else
                {
                    if (!shoudlSkip)
                        builds.AddRange(b);
                }
            }

            AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate);

            //UnityEditor.Sprites.Packer.SelectedPolicy = "DefaultPackerPolicy1024";

            string currentPackBundleMapPath = Path.Combine(outputDir, BUNDLE_DIR_NAME);         //manifest的bundle没有后缀

            if (builds.Count == 0)
            {
                error = "无打包资源，请检查.pack文件，及.pack文件的Filter";
                return false;
            }

            buildManifest = BuildPipeline.BuildAssetBundles(outputDir, builds.ToArray(), BuildAssetBundleOptions.ChunkBasedCompression, target);

            string rawAssetDir = Path.Combine(outputDir, PathConfig.RAW_ASSET_PATH);
            EngineFileUtil.ClearOrCreateEmptyFolder(rawAssetDir);

            foreach (string rawAssetPath in rawAssetList)
            {
                string fileName = Path.GetFileName(rawAssetPath);
                string dstPath = Path.Combine(rawAssetDir, fileName);

                File.Copy(rawAssetPath, dstPath, true);
            }

            if (buildManifest == null)
            {
                error = "打包失败，manifest 文件为空";
                return false;
            }

            DeleteUselessBundleFiles(buildManifest, outputDir);

            //todo:guangze song  2018-9-5 21:45:39:Editor下，切Scene会把堆对象清空(切Scene之后Manifest被清空)
            //if (!string.IsNullOrEmpty(currentSceneBeforeBuild))
            //    EditorSceneManager.OpenScene(currentSceneBeforeBuild, OpenSceneMode.Single);

            error = string.Empty;
            return true;
        }


        /// <summary>
        /// 获取指定平台Bundle生成后的路径
        /// </summary>
        /// <param name="targetPlatform"></param>
        /// <returns></returns>
        public static string GetBundleOutputPath(BuildTarget targetPlatform, bool isRelativePath = true)
        {
            string outputPath = string.Empty;
            if (!isRelativePath)
            {
                switch (targetPlatform)
                {
                    case BuildTarget.StandaloneWindows:
                    case BuildTarget.StandaloneWindows64:
                        outputPath = Path.Combine(PathConfig.WINDOWS_BUNDLE_OUTPUTPATH, PathConfig.BUNDLE_DIR_NAME);
                        break;
                    case BuildTarget.Android:
                        outputPath = Path.Combine(PathConfig.ANDROID_BUNDLE_OUTPUTPATH, PathConfig.BUNDLE_DIR_NAME);
                        break;
                    case BuildTarget.iOS:
                        outputPath = Path.Combine(PathConfig.IOS_BUNDLE_OUTPUTPATH, PathConfig.BUNDLE_DIR_NAME);
                        break;
                }
            }
            else
            {
                switch (targetPlatform)
                {
                    case BuildTarget.StandaloneWindows:
                    case BuildTarget.StandaloneWindows64:
                        outputPath = string.Format("{0}/{1}", PathConfig.WINDOWS_BUNDLE_RELATIVE_OUTPUTPATH, PathConfig.BUNDLE_DIR_NAME);
                        break;
                    case BuildTarget.Android:
                        outputPath = string.Format("{0}/{1}", PathConfig.ANDROID_BUNDLE_RELATIVE_OUTPUTPATH, PathConfig.BUNDLE_DIR_NAME);
                        break;
                    case BuildTarget.iOS:
                        outputPath = string.Format("{0}/{1}", PathConfig.IOS_BUNDLE_RELATIVE_OUTPUTPATH, PathConfig.BUNDLE_DIR_NAME);
                        break;
                }
            }


            DirectoryInfo outputDirInfo = new DirectoryInfo(outputPath);
            if (!outputDirInfo.Exists)
            {
                outputDirInfo.Create();
                Debug.Log("[dir : " + outputDirInfo.FullName + " not exist，create now]");
            }

            return outputPath;
        }


        int PlatformSwitches
        {
            get
            {
                int cnt = 0;
                if (mbStandaloneWindows && GOEPack.buildTarget != BuildTarget.StandaloneWindows)
                    cnt++;
                if (mbAndroid && GOEPack.buildTarget != BuildTarget.Android)
                    cnt++;
                if (mbiOS && GOEPack.buildTarget != BuildTarget.iOS)
                    cnt++;
                if (mbMac64 && GOEPack.buildTarget != BuildTarget.StandaloneOSX)
                    cnt++;
                return cnt;
            }
        }

        public override bool MultipleAction(string fileName, out string error)
        {
            error = null;
            OnSelect(fileName);
            Dictionary<string, object> e;
            AssetBundleManifest buildManifest;
            return DoPack(true, getBestPackOrder()[0], out error, null, out e, out buildManifest);
        }

        public static bool PackAll(bool silent, BuildTarget target, out string error, HashSet<string> exc, out Dictionary<string, object> extra, out AssetBundleManifest buildManifest)
        {
            GOEPackUIHelperV5 helper = new GOEPackUIHelperV5();
            GOESelectWindow.LoadUIHelperConfig(helper);
            return helper.DoPack(silent, target, out error, exc, out extra, out buildManifest);
        }


        /// <summary>
        /// 删除指定的Bundle及相关的manifest文件
        /// </summary>
        /// <param name="bundlePath"></param>
        private static void DeleteBundleFile(string bundlePath)
        {
            FileInfo deleteBundleFileInfo = new FileInfo(bundlePath);
            FileInfo associateManifestFileInfo = new FileInfo(bundlePath + ".manifest");

            if (deleteBundleFileInfo.Exists)
                deleteBundleFileInfo.Delete();

            if (associateManifestFileInfo.Exists)
                associateManifestFileInfo.Delete();
        }


        /// <summary>
        /// 清理过期废弃的资源
        /// </summary>
        /// <param name="buildManifest">本次Build生成的Manifest</param>
        /// <param name="buildBundleDir">Build的路径</param>
        private int DeleteUselessBundleFiles(AssetBundleManifest buildManifest, string buildBundleDir)
        {
            string[] bundlesInFolder = Directory.GetFiles(buildBundleDir, "*.bundle");
            HashSet<string> buildAssetBundleList = new HashSet<string>(buildManifest.GetAllAssetBundles());

            int deleteObsoleteBundleCount = 0;

            foreach (string bundleInDir in bundlesInFolder)
            {
                string bundleFileName = Path.GetFileName(bundleInDir);

                if (bundleFileName.StartsWith(buildBundleDir, StringComparison.OrdinalIgnoreCase))
                    continue;

                if (!buildAssetBundleList.Contains(bundleFileName.ToLower()))
                {
                    DeleteBundleFile(bundleInDir);
                    deleteObsoleteBundleCount++;
                }
            }

            return deleteObsoleteBundleCount;
        }


        /// <summary>
        /// log 本次打包打的资源
        /// </summary>
        /// <param name="currentBuildManifest"></param>
        /// <param name="logPath"></param>
        public static void WriteBuildAssetsLog(AssetBundleManifest currentBuildManifest, string logPath)
        {
            string[] buildAssetBundles = currentBuildManifest.GetAllAssetBundles();
            FileInfo logFile = new FileInfo(Path.Combine(logPath, "build_log.txt"));
            if (logFile.Exists)
                logFile.Delete();

            Debug.Log("buildAssetBundles:");

            using (StreamWriter writer = new StreamWriter(logFile.FullName, false, Encoding.UTF8))
            {
                foreach (string assetBundle in buildAssetBundles)
                {
                    writer.WriteLine(assetBundle);
                    Debug.Log(assetBundle);
                }
            }
        }

        /// <summary>
        /// Convert BuildTarget to BuildTargetGroup
        /// </summary>
        /// <param name="buildTarget"></param>
        /// <returns></returns>
        public static BuildTargetGroup ConvertBuildTarget(BuildTarget buildTarget)
        {
            switch (buildTarget)
            {
                case BuildTarget.StandaloneOSX:
                case BuildTarget.iOS:
                    return BuildTargetGroup.iOS;
                case BuildTarget.StandaloneWindows:
                case BuildTarget.StandaloneLinux:
                case BuildTarget.StandaloneWindows64:
                case BuildTarget.StandaloneLinux64:
                case BuildTarget.StandaloneLinuxUniversal:
                    return BuildTargetGroup.Standalone;
                case BuildTarget.Android:
                    return BuildTargetGroup.Android;
                case BuildTarget.WebGL:
                    return BuildTargetGroup.WebGL;
                case BuildTarget.WSAPlayer:
                    return BuildTargetGroup.WSA;
                case BuildTarget.Tizen:
                    return BuildTargetGroup.Tizen;
                case BuildTarget.PSP2:
                    return BuildTargetGroup.PSP2;
                case BuildTarget.PS4:
                    return BuildTargetGroup.PS4;
                case BuildTarget.PSM:
                    return BuildTargetGroup.PSM;
                case BuildTarget.XboxOne:
                    return BuildTargetGroup.XboxOne;
                case BuildTarget.N3DS:
                    return BuildTargetGroup.N3DS;
                case BuildTarget.WiiU:
                    return BuildTargetGroup.WiiU;
                case BuildTarget.tvOS:
                    return BuildTargetGroup.tvOS;
                case BuildTarget.Switch:
                    return BuildTargetGroup.Switch;
                case BuildTarget.NoTarget:
                default:
                    return BuildTargetGroup.Standalone;
            }
        }
    }
}