using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System;
using GOEngine.Implement;
using EngineCore.Editor;

//#define UNITY_EDITOR 1
#if UNITY_EDITOR
using UnityEditor;
namespace GOEditor
//打包//
{
    public class GOEPackV5
    {
        //分包方式//
        public static Dictionary<string, string> msArrayPackType = new Dictionary<string, string>(){
            {"OneBunlde","单一Bundle"},
            {"Per File","逐文件"},
            {"Per Dir","逐目录"},
            //{"Per Prefix","逐前缀"},
            {"Per Atlas","逐图集"},
            {"Scene","场景"},
            {"UIPrefab","UI Prefab"},
            //{"AudioPrefab","声音Prefab"},
            //{"Per Dir Physic", "物理角色" },
            {"Raw Asset", "原始文件(不打包)" },
        };
        #region extentions
        //扩展名//        
        public const string m_sceneObjExpDirPostfix = "/exp_scene_obj";
        public const string m_bundleExpDirPostfix = "/exp_bundle";

        private const string ANI_EXTRA_FOLDER = "extra";
        private const string ANI_GENERATE_FOLDER = "anim";
        private const string ANI_SUFFIX = ".anim";
        #endregion

        List<PackBundleSetting> mPackItems = new List<PackBundleSetting>();

        [DisplayName("源目录")]
        public string SrcDir
        {
            get;
            set;
        }
        [JsonField(JsonFieldTypes.HasChildren)]
        public List<PackBundleSetting> PackItems
        {
            get { return mPackItems; }
            set { mPackItems = value; }
        }
        public static string GetPlatformName(UnityEditor.BuildTarget target)
        {
            string ret = "";
            switch (target)
            {
                case UnityEditor.BuildTarget.StandaloneWindows:
                    return "StandaloneWindows";
                case UnityEditor.BuildTarget.Android:
                    return "Android";
                case UnityEditor.BuildTarget.iOS:
                    return "iOS";
                case UnityEditor.BuildTarget.StandaloneOSX:
                    return "Mac";
            }
            return ret;
        }

        public bool AssetProcessor(out string error)
        {
            //List<string> folderList = new List<string>();
            bool needRefresh = false;
            foreach (var i in mPackItems)
            {
                if (i.PackType == "Per Anim")
                {
                    string folder = SrcDir + "/" + i.SubFolder;
                    var files = Directory.GetFiles(folder, "*.*", i.SearchSubDir ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly).
                        Where(s => s.ToLower().EndsWith(".fbx") && Directory.GetParent(s).Name == ANI_EXTRA_FOLDER);

                    string outputPath = null;
                    AnimationClip clipTemp = null;
                    //HashSet<string> generatedFiles = new HashSet<string>();
                    foreach (string file in files)
                    {
                        DirectoryInfo dirInfo = Directory.GetParent(file);
                        DirectoryInfo rootDirInfo = Directory.GetParent(dirInfo.FullName);
                        string tarPath = rootDirInfo.FullName + "/" + ANI_GENERATE_FOLDER;
                        outputPath = tarPath;

                        outputPath = outputPath + "/" + Path.GetFileNameWithoutExtension(file) + ANI_SUFFIX;
                        string hash, md5File;
                        if (ShouldGenerateAsset(file, outputPath, out hash, out md5File))
                        {
                            clipTemp = AssetDatabase.LoadAssetAtPath<AnimationClip>(file);
                            if (clipTemp == null) continue;

                            if (!Directory.Exists(tarPath))
                            /*{
                                if (!folderList.Contains(outputPath))
                                {
                                    Directory.Delete(outputPath, true);
                                    Directory.CreateDirectory(outputPath);
                                    folderList.Add(outputPath);
                                }
                            }
                            else*/
                            {
                                Directory.CreateDirectory(outputPath);
                                //folderList.Add(outputPath);
                            }

                            string clipName = clipTemp.name;
                            outputPath = outputPath.Replace("\\", "/").Replace(Application.dataPath, "Assets");
                            //outputPath += "/" + clipName + ANI_SUFFIX;
                            if (File.Exists(outputPath))
                                File.Delete(outputPath);
                            clipTemp = GameObject.Instantiate(clipTemp);
                            clipTemp.name = clipName;
                            AssetDatabase.CreateAsset(clipTemp, outputPath);

                            using (System.IO.StreamWriter sw = new StreamWriter(md5File, false, System.Text.Encoding.ASCII))
                            {
                                sw.WriteLine(hash);
                                sw.Flush();
                            }

                            needRefresh = true;
                        }
                    }
                }
            }

            if (needRefresh)
                AssetDatabase.Refresh();

            error = null;

            return true;
        }


        /// <summary>
        /// 获取要打包的资源
        /// </summary>
        /// <param name="silent"></param>
        /// <param name="error"></param>
        /// <param name="builds"></param>
        /// <returns></returns>
        /// <remarks>
        ///  1. Unity 5.X 普通的资源还有场景的打包接口都可以使用一个BuildPipeline.BuildAssetBundles()
        ///  2. NavMesh 不需要每次打包都生成，所以需要在打包之前先手动运行导航网络的生成，然后使用打包选项把NavMeshObj打入到对应的Bundle中
        /// </remarks>
        public bool ConfigureAssetImporter(bool silent, out string error, out List<AssetBundleBuild> builds, Dictionary<string, object> extra, bool checkOnly, out HashSet<string> assetsFileHash)
        {
            Dictionary<string, List<string>> fileMapping = new Dictionary<string, List<string>>();
            builds = null;
            assetsFileHash = new HashSet<string>();
            Dictionary<string, bool> canForceRelease;
            if (!extra.ContainsKey("CanForceRelease"))
            {
                canForceRelease = new Dictionary<string, bool>();
                extra["CanForceRelease"] = canForceRelease;
            }
            else
                canForceRelease = (Dictionary<string, bool>)extra["CanForceRelease"];


            foreach (PackBundleSetting packBundleSetting in mPackItems)
            {
                PackHandlerBase packHandler = AssetPackSystem.CreatePackHalder(packBundleSetting.PackType, this, packBundleSetting);
                if (packHandler == null)
                {
                    error = "can't find target packtype:" + packBundleSetting.PackType;
                    return false;
                }

                Dictionary<string, List<string>> packBundleFilesMap = packHandler.GetAssetsDictByPackSetting();

                foreach (KeyValuePair<string, List<string>> pair in packBundleFilesMap)
                {
                    fileMapping.Add(pair.Key, pair.Value);
                    assetsFileHash.UnionWith(pair.Value);
                }
            }
            int idx = 0;

            builds = new List<AssetBundleBuild>();
            //清理不应该被打包进去的文件
            foreach (var i in fileMapping)
            {
                AssetBundleBuild build = new AssetBundleBuild();
                build.assetBundleName = i.Key;

                build.assetNames = i.Value.ToArray();
                builds.Add(build);
                idx++;
            }
            error = null;
            return true;
        }

        string GetGeneratedPath(string srcDir, string path, string suffix = "")
        {
            string folder = GetGeneratedFolderPath(srcDir, suffix);
            return folder + "/" + System.IO.Path.GetFileName(path);
        }

        string GetGeneratedFolderPath(string srcDir, string suffix = "")
        {
            return srcDir + "/generate" + suffix;
        }

        bool ShouldGenerateAsset(string file, string genPath, out string hash, out string md5File)
        {
            string genFolder = System.IO.Path.GetDirectoryName(genPath);
            string genFileName = System.IO.Path.GetFileNameWithoutExtension(genPath);
            md5File = genFolder + "/" + genFileName + ".md5";
            using (System.IO.FileStream fs = new FileStream(file, FileMode.Open, FileAccess.Read))
            {
                byte[] buffer = new byte[fs.Length];
                fs.Read(buffer, 0, buffer.Length);
                var md5 = System.Security.Cryptography.MD5.Create();
                hash = Convert.ToBase64String(md5.ComputeHash(buffer));
            }
            if (System.IO.File.Exists(md5File))
            {
                if (System.IO.File.Exists(genPath))
                {
                    using (System.IO.StreamReader sr = new StreamReader(md5File, System.Text.Encoding.ASCII))
                    {
                        if (hash != sr.ReadLine())
                            return true;
                        else
                            return false;
                    }
                }
                else
                    return true;
            }
            else
                return true;
        }
    }
}

#endif
