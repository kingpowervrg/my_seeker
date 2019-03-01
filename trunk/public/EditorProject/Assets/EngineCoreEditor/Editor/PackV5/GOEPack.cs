using EngineCore;
using GOEngine.Implement;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

#if UNITY_EDITOR
namespace GOEditor
{
    //打包//
    public class GOEPack
    {
        //分包方式//
        public static Dictionary<string, string> msArrayPackType = new Dictionary<string, string>(){
            {"Copy","复制"},
            {"Manual","手动"},
            {"Per File","逐文件"},
            {"Per Dir","逐目录"},
            {"Limit Size","Limit Size"},
            {"Scene","场景"},
            {"Connectted Graph","Connectted Graph"},
            {"Connectted Graph Cluster","Connectted Graph Cluster"},
            {"Preprocessed","预处理生成"},
            {"PreprocessedPrefile","预处理后逐文件生成"},
            {"AudioPrefab","声音Prefab"},
        };
        //IndexMap分隔字符//
        public const string msIndexMapSpliter = ":";
        #region extentions
        //扩展名//
        public const string m_fbxExt = ".fbx";
        public const string m_matExt = ".mat";
        public const string m_maxExt = ".max";
        public const string m_prefabExt = ".prefab";
        public const string m_animExt = ".anim";
        public const string m_animSrcFileContain = "@";
        public const string m_txtExt = ".txt";
        public const string m_md5Ext = ".md5.txt";
        public const string m_packExt = ".pack.txt";
        public const string m_curveAnimExt = ".ca.txt";
        public const string m_sceneDataExt = ".scene.txt";
        public const string m_unityExt = ".unity";
        public const string m_sceneExt = ".unity3d";
        public const string m_bundleExt = ".bundle";
        public const string m_bytesExt = ".bytes";
        public const string m_reportExt = ".report.txt";
        public const string m_oggExt = ".ogg";
        public const string m_assetExt = ".asset";
        public const string m_txtidxExt = ".txt_idx";
        public const string m_idxExtPost = "_idx";

        public const string m_navPrefix = "navmesh_";
        public const string m_pfbPrefix = "scn_pfb_";
        public const string m_infoPrefix = "scn_info_";
        public const string m_groupPostfix = "_group";

        public const string m_clientConfig = "../../clientConfig";

        public const string m_sceneExpDirPostfix = "/exp_scene";
        public const string m_sceneObjExpDirPostfix = "/exp_scene_obj";
        public const string m_bundleExpDirPostfix = "/exp_bundle";
        public const string m_idxExpDirPostfix = "/exp_res_idx";
        public const string m_MD5ExpDirPostfix = "/exp_md5";

        public const string m_sceneExpDir = "/exp_scene";
        public const string m_bundleExpDir = "/exp_bundle";
        public const string m_idxExpDir = "/exp_res_idx";

        public const string m_binresDir = "../../../trunk/code/client/goe/client/bin/res";
        public const string m_binresDirAndroid = "../../../trunk/code/client/goe/client/bin/resandroid";
        public const string m_binresDirIOS = "../../../trunk/code/client/goe/client/bin/resios";
        public const string m_streamAssetsDiriOS = "../../../trunk/client/goe/client/project/Assets/StreamingAssets";
        public const string m_streamAssetsDir = "../../../trunk/code/client/goe/client/project/Assets/StreamingAssets";
        public const string m_streamAssetsDirDyn = "../../../trunk/code/client/goe/client/loader/Assets/StreamingAssets";
        public const string m_buildDirForWin = "../../release/client/bin/res";
        #endregion
        //目标平台//
        public static UnityEditor.BuildTarget buildTarget = UnityEditor.EditorUserBuildSettings.selectedStandaloneTarget;

        //预处理/后处理//
        public delegate bool PackProcessFunc(GOEPack pack);		//原型//
        static public Dictionary<string, PackProcessFunc> msDictPackProcessFunc = new Dictionary<string, PackProcessFunc>();
        public static Dictionary<string, string> msDictPackProcessNames = new Dictionary<string, string>();
        //合法性检测//
        public delegate bool PackValidateFunc(GOEPack pack, out string report);		//原型//
        static public Dictionary<string, PackValidateFunc> msDictValidateFunc = new Dictionary<string, PackValidateFunc>();
        public static Dictionary<string, string> msDictValidateNames = new Dictionary<string, string>();
        private string mSrcDir = "";
        private string mOutputDir = "";
        private string mOutputSubDir = "";
        private string mAndroidOutputDir = "";
        private string miOSOutputDir = "";
        private string mMacOutputDir = "";
        private string mIdxOutputFileName = "";
        private string mPackType = "";
        private int mLimitSize = 1024;		//限制尺寸//
        private int mClusterCount = 10;		//同一连通子图Cluster目标数量//
        private string mPreProcess = "Null";
        private string mValidateFunc = "Null";
        private string mPostProcess = "Null";
        private string mMD5FileName = "";	//用于检验版本的MD5文件路径//


        //场景导出不能直接调用，必须由Helper传入//
        static public PackProcessFunc mPackScene = null;
        static public PackProcessFunc mPackScene2 = null;

        [DisplayName("源目录")]
        public string SrcDir
        {
            set { mSrcDir = value; }
            get { return mSrcDir; }
        }
        [DisplayName("目标目录")]
        public string OutputDir
        {
            set { mOutputDir = value; }
            get { return mOutputDir; }
        }
        [DisplayName("子目录")]
        public string OutputSubDir
        {
            set { mOutputSubDir = value; }
            get { return mOutputSubDir; }
        }
        [DisplayName("Android平台目标目录")]
        public string AndroidOutputDir
        {
            set { mAndroidOutputDir = value; }
            get { return mAndroidOutputDir; }
        }
        [DisplayName("iOS平台目标目录")]
        public string iOSOutputDir
        {
            get { return miOSOutputDir; }
            set { miOSOutputDir = value; }
        }
        [DisplayName("Mac平台目标目录")]
        public string MacOutputDir
        {
            get { return mMacOutputDir; }
            set { mMacOutputDir = value; }
        }
        [DisplayName("Idx输出文件名")]
        public string IdxOutputFileName
        {
            set { mIdxOutputFileName = value; }
            get { return mIdxOutputFileName; }
        }
        [JsonFieldAttribute(JsonFieldTypes.PackType)]
        [DisplayName("打包类型")]
        public string PackType
        {
            set { mPackType = value; }
            get { return mPackType; }
        }
        public int LimitSize
        {
            set { mLimitSize = value; }
            get { return mLimitSize; }
        }
        public int ClusterCount
        {
            set { mClusterCount = value; }
            get { return mClusterCount; }
        }
        [JsonFieldAttribute(JsonFieldTypes.PreProcess)]
        [DisplayName("预处理")]
        public string PreProcess
        {
            set { mPreProcess = value; }
            get { return mPreProcess; }
        }
        [JsonFieldAttribute(JsonFieldTypes.ValidFuncs)]
        [DisplayName("验证")]
        public string ValidateFunc
        {
            set { mValidateFunc = value; }
            get { return mValidateFunc; }
        }
        [JsonFieldAttribute(JsonFieldTypes.PostProcess)]
        [DisplayName("后处理")]
        public string PostProcess
        {
            set { mPostProcess = value; }
            get { return mPostProcess; }
        }
        [DisplayName("MD5文件名")]
        public string MD5FileName
        {
            set
            {
                mMD5FileName = value;
            }
            get { return mMD5FileName; }
        }


        public string SettedFileName = string.Empty;


        public static string[] GetDepsOn(string[] pathNames)
        {
            //这个调用并不能修正获取Dependency的异常//
            //UnityEditor.AssetDatabase.Refresh();
            return UnityEditor.AssetDatabase.GetDependencies(pathNames);
        }


        //返回使用了多少行多少列//
        public bool mbScaleByDep = false;

        //合法性检测//
        public bool Validate(out string report)
        {
            string str = string.Empty;
            PackValidateFunc vfunc = null;
            if (ValidateFunc.Length > 0 && msDictValidateFunc.TryGetValue(ValidateFunc, out vfunc))
            {
                if (null != vfunc)
                {
                    if (vfunc(this, out str))
                    {
                        //成功//
                        report = str;
                        return true;
                    }
                    else
                    {
                        report = str;
                        return false;
                    }
                }
            }
            report = str;
            return true;
        }
        public bool ValidateAndShowResult()
        {
            string strValidate;
            if (Validate(out strValidate))
            {
                //成功//
                UnityEditor.EditorUtility.DisplayDialog("Pack",
                                            "[Validate success : " + ValidateFunc + "]",
                                            "OK");
                return true;
            }
            else
            {
                //失败//
                UnityEditor.EditorUtility.DisplayDialog("Pack",
                                            "[Validate failed : " + ValidateFunc + "]\r\n" + strValidate,
                                            "OK");
                return false;
            }
        }


        public string GetOutputDir(UnityEditor.BuildTarget target)
        {
            string ret = "";
            switch (target)
            {
                case UnityEditor.BuildTarget.StandaloneWindows:
                    return OutputDir;
                case UnityEditor.BuildTarget.Android:
                    return AndroidOutputDir;
                case UnityEditor.BuildTarget.iOS:
                    return iOSOutputDir;
                case UnityEditor.BuildTarget.StandaloneOSX:
                    return MacOutputDir;
            }
            return ret;
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




        //添加对象、文件名到列表中//
        private static bool AddAsset(string bundleFile, string srcFile,
                                    List<UnityEngine.Object> objs, List<string> names,
                                    Dictionary<string, string> map, bool depByother)
        {
            string absname = srcFile;
            //加载对象//
            UnityEngine.Object obj = UnityEditor.AssetDatabase.LoadMainAssetAtPath(absname);
            if (obj == null)
                throw new System.Exception("load asset failed: " + absname);
            GameObject go = obj as GameObject;
            if (go != null)
                DeActiveCamera(go);
            //检查名字是否重复//
            string name = Path.GetFileName(srcFile);
            if (names.Contains(name) || map.ContainsKey(name))
            {
                UnityEditor.EditorUtility.DisplayDialog("error",
                                            "dupliacate file name: " + name,
                                            "OK");
                throw new System.Exception("dupliacate file name: " + name);
            }
            //添加//
            objs.Add(obj);
            names.Add(name);
            map.Add(name, bundleFile);
            setDependFlag(depByother, map, bundleFile);

            return true;
        }
        private static void setDependFlag(bool depByother, Dictionary<string, string> map, string bundleFile)
        {
            if (depByother && !map.ContainsKey(bundleFile))
                map.Add(bundleFile, "Dpen");
        }


        /// <summary>
        /// 添加文件名到Map中
        /// </summary>
        public static bool AddToIndexMap(string path, string bundleFile,
            Dictionary<string, string> map, bool dep)
        {
            string filename = Path.GetFileName(path);
            if (map.ContainsKey(filename))
            {
                UnityEditor.EditorUtility.DisplayDialog("error",
                                            "dupliacate file name: " + filename,
                                            "OK");
                return false;
            }
            map.Add(filename, bundleFile);
            setDependFlag(dep, map, bundleFile);
            return true;
        }
        /// <summary>
        /// 添加多个文件名到Map中
        /// </summary>
        public static bool AddToIndexMap(string[] paths, string bundleFile,
            Dictionary<string, string> map, bool dep)
        {
            foreach (string path in paths)
            {
                if (!AddToIndexMap(path, bundleFile, map, dep))
                    return false;
            }
            return true;
        }

        public static string GetOrCreateAbsPath(string path)
        {
            string absname = Path.GetFullPath(path);
            if (!Directory.Exists(absname))
            {
                Directory.CreateDirectory(absname);
            }
            return absname;
        }

        private static void DeActiveCamera(GameObject go)
        {
            Camera[] cams = go.GetComponentsInChildren<Camera>();
            foreach (Camera cam in cams)
            {
                cam.enabled = false;
            }
        }
        private static bool ExpIdxFile(Dictionary<string, string> idxMap, string idxPath)
        {
            if (idxMap == null) return false;
            if (idxMap.Count <= 0) return false;
            string dstFile = idxPath;//Path.Combine(dp, idxFile.Trim(PackRes_Def.trim));
            string path = Path.GetDirectoryName(dstFile);
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
            FileStream fs = new FileStream(dstFile, FileMode.Create);//如已存在则打开，否则创建//
            StreamWriter writer = new StreamWriter(fs);
            //写入//
            foreach (KeyValuePair<string, string> kvp in idxMap)
                writer.Write(kvp.Key + msIndexMapSpliter + kvp.Value + "\n");
            writer.Flush();
            //关闭文件//
            writer.Close();
            fs.Close();

            return true;
        }

        public static List<GameObject> GetRootGoList()
        {
            List<GameObject> goList = new List<GameObject>();
            UnityEngine.Object[] objArr = UnityEngine.Object.FindObjectsOfType(typeof(GameObject));
            foreach (UnityEngine.Object obj in objArr)
            {
                GameObject go = obj as GameObject;
                if (go.transform.parent == null)
                    goList.Add(go);
            }
            return goList;
        }
    }
}

#endif