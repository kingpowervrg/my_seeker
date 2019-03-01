using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace GOEditor
{
    public static class ParseScene
    {
        public const string GROUP_NAME = "group";
        public const string TERRAIN_NAME = "terrain";

        public const string PFB_EXT = ".prefab";
        public const string BUNDLE_EXT = ".bundle";
        public const string TERRAIN_EXT = ".unity3d";
        public const string SCN_INFO_BIN_EXT = ".bytes";
        public const string SCN_INFO_TXT_EXT = ".xml";

        public const string PFB_PREFIX = "scn_pfb_";
        public const string SCN_INFO_PREFIX = "scn_info_";
        public const string RES_ASSET_PREFIX = "scn_res_";

        public const string BIN_FLAG = "scn_info";

        private static readonly char[] trim = new char[] { '\0', ' ', '\t', '\r', '\n' };

        public static readonly Vector2 DEF_SPOWN = Vector2.zero;
        public static readonly Vector2 DEF_GROUP_SIZE = new Vector2(48, 48);

        public const string EXP_PFB_PATH = "Assets/gen_pfb/";
        public static Vector2 m_groupSize = DEF_GROUP_SIZE;

        private static Bounds m_sceneBound = new Bounds();

        public static string m_groupBundle = string.Empty;

        private static List<GameObject> m_groupGoList = new List<GameObject>();
        public static List<string> m_groupNameList = new List<string>();

        private static List<Bounds> m_groupBdList = new List<Bounds>();
        private static List<UnityEngine.Object> m_groupAssetList = new List<UnityEngine.Object>();

        private static List<List<string>> m_depBundleList = new List<List<string>>();
        private static List<List<string>> m_depAssetPathList = new List<List<string>>();

        private static Dictionary<string, List<string>> m_depBundleAssetMap =
            new Dictionary<string, List<string>>();

        private static List<string> m_pfbDepList = new List<string>();

        #region Inner class

        class HeightMapInfo
        {
            private Vector3 m_SceneMin = Vector3.zero;
            private Vector3 m_SceneMax = Vector3.zero;

            private int m_Hor;
            private int m_Ver;
            public float[] mData = null;

            // 世界坐标下，地形坐标的最小值 ， xz方向 //
            private Vector2 m_MinPosition;

            // 世界坐标下，地形坐标的最大值 ， xz方向 //
            private Vector2 m_MaxPosition;

            //private HeightData[] m_dataArray;

            public HeightMapInfo(int nHor, int nVer, Vector3 sceneMin, Vector3 sceneMax)
            {
                m_SceneMin = sceneMin;
                m_SceneMax = sceneMax;
                m_Hor = nHor;
                m_Ver = nVer;
                mData = new float[m_Hor * m_Ver];

                //  m_dataArray = new HeightData[m_Hor * m_Ver];

                //			for (int i = 0; i < HeightData.Length; ++i)
                //            {
                //                m_dataArray[i] = new HeightData();
                //                m_dataArray[i].layerInfo = 0;
                //                m_dataArray[i].data = null;
                //            }
            }

            /*
            public bool LoadFromFile(string strFileName)
            {
                StreamReader file = File.OpenRead(strFileName);
                // read hor
                int nIdx = 0;
                char[] temp = new char[2];
                string strTemp = string.Empty;
                string strBuff = file.ReadToEnd();
                strTemp = strBuff.Substring(nIdx, sizeof(int));
                nIdx += sizeof(int);
                m_Hor = int.Parse(strTemp);

                strTemp = strBuff.Substring(nIdx, sizeof(int));
                nIdx += sizeof(int);
                m_Ver = int.Parse(strTemp);

                for (int i=0; i<m_Hor * m_Ver; ++i)
                {
                    strTemp = strBuff.Substring(nIdx, sizeof(byte));
                    nIdx += sizeof(byte);
                    m_dataArray[i].layerInfo = byte.Parse(strTemp);
                    bool bHasTerrainLayer = false;
                    bool bHasWalkSurface = false;
                    float fTerrainHeight;
                    float fWalkSurfaceHeight;
                    int nArrayCount = 0;
                    if( (m_dataArray[i].layerInfo & (byte)LayerMask.eTerrainMask) > 0 )
                    {
                        bHasTerrainLayer = true;
                        nArrayCount++;
                    }
                    if( (m_dataArray[i].layerInfo & (byte)LayerMask.eWalkSurfaceMask) > 0 )
                    {
                        bHasWalkSurface = true;
                        nArrayCount++;
                    }

                    m_dataArray[i].data = new float[nArrayCount];
                    int nArrayIdx = 0;
                    if (bHasTerrainLayer)
                    {
                        strTemp = strBuff.Substring(nIdx, sizeof(float));
                        nIdx += sizeof(float);
                        m_dataArray[i].data[nArrayIdx++] = float.Parse(strTemp);

                    }
                    if (bHasWalkSurface)
                    {
                        strTemp = strBuff.Substring(nIdx, sizeof(float));
                        nIdx += sizeof(float);
                        m_dataArray[i].data[nArrayIdx++] = float.Parse(strTemp);
                    }

                }

                return true;
            }
            */
            public void SaveToFile()
            {
                string strSceneName = EditorApplication.currentScene;
                string strFilePath = strSceneName + ".txt";
                //strFilePath = "Assets/../hmd/" + Path.GetFileName(strFilePath);


                //Debug.Log(strFilePath);

                // write to file
                FileStream fs = new FileStream(strFilePath, FileMode.Create);
                StreamWriter file = new StreamWriter(fs);
                //BinaryWriter file = new BinaryWriter(fs);
                //hor

                //file.Write(System.Net.IPAddress.HostToNetworkOrder(m_Hor));
                //ver
                //file.Write(System.Net.IPAddress.HostToNetworkOrder(m_Ver));

                file.Write(m_SceneMin.x);
                file.Write(",");
                file.Write(m_SceneMin.y);
                file.Write(",");
                file.Write(m_SceneMin.z);
                file.Write("\n");
                file.Write(m_SceneMax.x);
                file.Write(",");
                file.Write(m_SceneMax.y);
                file.Write(",");
                file.Write(m_SceneMax.z);
                file.Write("\n");

                file.Write(m_Hor);
                file.Write(",");
                file.Write(m_Ver);
                file.Write("\n");


                //Debug.Log("min = " + m_SceneMin.ToString());
                //Debug.Log("max = " + m_SceneMax.ToString());

                int nDataCount = mData.Length;
                for (int i = 0; i < nDataCount; ++i)
                {
                    file.Write(mData[i]);
                    file.Write(",");
                }

                //close
                file.Close();
            }

            public void SetData(int nHor, int nVer, float fData)
            {
                if (nHor >= m_Hor || nVer >= m_Ver)
                    return;

                int nIdx = m_Hor * nVer + nHor;
                mData[nIdx] = fData;
            }


            public void SetMinAndMaxPos(float minX, float minZ, float maxX, float maxZ)
            {
                m_MinPosition.x = minX;
                m_MinPosition.y = minZ;
                m_MaxPosition.x = maxX;
                m_MaxPosition.y = maxZ;
            }

            //        public HeightMapInfo.HeightData GetHeightData(int hor, int ver)
            //        {
            //            int nIdx = hor * ver + hor;
            //            if (nIdx < (m_Hor * m_Ver))
            //            {
            //                return m_dataArray[nIdx];
            //            }
            //            return null;
            //        }

            //        // 从世界坐标的x,z获得y //
            //        public HeightMapInfo.HeightData GetHeightData(Vector3 worldPos)
            //        {
            //            if (worldPos.x > m_MinPosition.x && worldPos.x < m_MaxPosition.x && worldPos.z > m_MinPosition.y && worldPos.z < m_MaxPosition.y)
            //            {
            //                float x = (worldPos.x - m_MinPosition.x) / (m_MaxPosition.x - m_MinPosition.x) * (float)m_Hor;
            //                float y = (worldPos.z - m_MinPosition.y) / (m_MaxPosition.y - m_MinPosition.y) * (float)m_Ver;
            //                GetHeightData((int)x, (int)y);
            //            }
            //            return null;
            //        }

            //        public static bool HasLayerData(HeightMapInfo.HeightData data, int layer)
            //        {
            //            if (layer == (int)Layer.eTerrain)
            //            {
            //                if ((data.layerInfo & (byte)LayerMask.eTerrainMask) > 0)
            //                {
            //                    return true;
            //                }
            //            }
            //            else if (layer == (int)Layer.eWalkSurface)
            //            {
            //                if ((data.layerInfo & (byte)LayerMask.eWalkSurfaceMask) > 0)
            //                {
            //                    return true;
            //                }
            //            }
            //
            //            return false;
            //        }

            //	public static float GetLayerData(HeightData data, int layer)
            //	{
            //		
            //	}
        }

        #endregion



        /// <summary>
        /// 获得场景中，所有根节点 
        /// </summary>
        /// <returns></returns>
        private static List<GameObject> GetSceneRootObjectList()
        {
            List<GameObject> goList = new List<GameObject>();
            UnityEngine.Object[] objArr = UnityEngine.Object.FindObjectsOfType<GameObject>();
            foreach (UnityEngine.Object obj in objArr)
            {
                GameObject go = obj as GameObject;
                if (go.transform.parent == null)
                {
                    MeshFilter[] filters = go.GetComponentsInChildren<MeshFilter>() as MeshFilter[];
                    MeshRenderer[] renders = go.GetComponentsInChildren<MeshRenderer>() as MeshRenderer[];
                    if (filters.Length > 0 && renders.Length > 0)
                    {
                        goList.Add(go);
                    }
                }
            }
            return goList;
        }

        static Bounds GetSceneBounds(GameObject[] objs)
        {
            Bounds bounds = new Bounds(Vector3.zero, Vector3.zero);
            if (objs == null || objs.Length <= 0)
            {
                return bounds;
            }

            float minX = float.MaxValue;
            float minY = float.MaxValue;
            float minZ = float.MaxValue;
            float maxX = float.MinValue;
            float maxY = float.MinValue;
            float maxZ = float.MinValue;

            //            if (Terrain.activeTerrain != null)
            //            {
            //                TerrainCollider tc = Terrain.activeTerrain.GetComponent<TerrainCollider>();
            //                Vector3 min = tc.bounds.min;
            //                Vector3 max = tc.bounds.max;
            //                if (min.x < minX)
            //                    minX = min.x;
            //                if (min.y < minY)
            //                    minY = min.y;
            //                if (min.z < minZ)
            //                    minZ = min.z;
            //
            //                if (max.x > maxX)
            //                    maxX = max.x;
            //                if (max.y > maxY)
            //                    maxY = max.y;
            //                if (max.z > maxZ)
            //                    maxZ = max.z;
            //            }
            //            else
            //            {
            //                Debug.LogError("no terrain!");
            //            }

            foreach (GameObject obj in objs)
            {
                MeshFilter[] filters = obj.GetComponentsInChildren<MeshFilter>() as MeshFilter[];
                for (int i = 0; i < filters.Length; ++i)
                {
                    MeshFilter filter = filters[i];
                    Mesh mesh = filter.sharedMesh;
                    if (mesh == null)
                        continue;

                    if (filter.GetComponent<Collider>() == null)
                        continue;
                    Vector3 min = filter.GetComponent<Collider>().bounds.min;
                    Vector3 max = filter.GetComponent<Collider>().bounds.max;
                    if (min.x < minX)
                        minX = min.x;
                    if (min.y < minY)
                        minY = min.y;
                    if (min.z < minZ)
                        minZ = min.z;

                    if (max.x > maxX)
                        maxX = max.x;
                    if (max.y > maxY)
                        maxY = max.y;
                    if (max.z > maxZ)
                        maxZ = max.z;
                    /*
                    for ( int j=0; j<mesh.vertexCount; ++j )
                    {
                        Vector3 vertex = mesh.vertices[j];
                        GameObject objectFilter = filter.gameObject;
                        vertex = objectFilter.transform.TransformPoint( vertex );
					
                        if ( vertex.x < minX )
                        {
                            minX =  vertex.x;
                        }
                        if ( vertex.y < minY )
                        {
                            minY =  vertex.y;
                        }
                        if ( vertex.z < minZ )
                        {
                            minZ =  vertex.z;
                        }
					
                        if ( vertex.x > maxX )
                        {
                            maxX =  vertex.x;
                        }
                        if ( vertex.y > maxY )
                        {
                            maxY =  vertex.y;
                        }
                        if ( vertex.z > maxZ )
                        {
                            maxZ =  vertex.z;
                        }
                    }
    */
                }
            }





            bounds.min = new Vector3(minX, minY, minZ);
            bounds.max = new Vector3(maxX, maxY, maxZ);
            return bounds;
        }

        private static void Init(string terrainDestDir, string infoDestDir)
        {
            m_sceneBound = new Bounds();

            m_groupGoList.Clear();
            m_groupBdList.Clear();
            m_groupAssetList.Clear();
            m_groupNameList.Clear();
            m_groupBundle = string.Empty;

            m_depBundleList.Clear();
            m_depAssetPathList.Clear();

            m_pfbDepList.Clear();

            CreatePath(terrainDestDir);
            CreatePath(infoDestDir);
        }

        static string GetTerrainBundleName()
        {
            return EditorSceneExporter.GetSceneNameWithoutExtension() + TERRAIN_EXT;
        }
        private static void GenTerrain(string destDir)
        {
            string scnname = EditorApplication.currentScene;

            string[] names = new string[] { scnname };
            string bundleName = GetTerrainBundleName();
            string dstname = Path.Combine(destDir, bundleName);

            Terrain[] ters = GameObject.Find("terrain").GetComponentsInChildren<Terrain>(true);
            foreach (Terrain ter in ters)
            {
                Debug.Log("delete terrain by unity");
                GameObject.DestroyImmediate(ter.gameObject);
            }/*
            BuildPipeline.PushAssetDependencies();
            Debug.Log("add depend");
            PackEntry.ExecutePack("Assets/GOEditor/Pack/Definitions/shader.pack.txt", GOEPack.buildTarget);
            //AssetDatabase.ExportPackage(scnname, dstname, ExportPackageOptions.IncludeDependencies | ExportPackageOptions.IncludeLibraryAssets);
            string errmsg = BuildPipeline.BuildStreamedSceneAssetBundle(names, dstname, GOEPack.buildTarget);
            //build失败时弹出对话框//
            if (errmsg.Length > 0)
            {
                Debug.Log("GenTerrain error : " + errmsg);
                EditorUtility.DisplayDialog("error",
                                            "GenTerrain error : " + errmsg,
                                            "ok");
            }
            BuildPipeline.PopAssetDependencies();*/
        }

        private static float GetMinDis(Bounds bd, Vector2 pos)
        {
            Vector2 center = new Vector2(bd.center.x, bd.center.z);
            return Vector2.Distance(center, pos);
        }

        private static bool GetDepAssetPathList(GameObject go,
                                            out List<string> assetList,
                                            out UnityEngine.Object mainAsset,
                                            out string mainAssetName)
        {
            mainAssetName = string.Empty;
            mainAsset = null;
            assetList = new List<string>();

            if (go == null)
            {
                Debug.Log("go is null for get dep asset");
                return false;
            }

            string name = PFB_PREFIX + EditorSceneExporter.GetSceneNameWithoutExtension() + "_" + GetGoName(go) + PFB_EXT;
            string path = Path.Combine(EXP_PFB_PATH, name);

            UnityEngine.Object prefab = PrefabUtility.CreateEmptyPrefab(path);
            PrefabUtility.ReplacePrefab(go, prefab);
            AssetDatabase.Refresh();
            UnityEngine.Object asset = AssetDatabase.LoadMainAssetAtPath(path);
            UnityEngine.Object[] deps = EditorUtility.CollectDependencies(new Object[] { asset });
            foreach (Object dep in deps)
            {
                if (null == dep)
                    Debug.Log("null dep of asset : " + asset.name);
                else
                {

                    if (dep.GetType() == typeof(Mesh)
                       || dep.GetType() == typeof(Material)
                       || dep.GetType() == typeof(Texture2D)
                       || dep.GetType() == typeof(Shader)
                       || dep.GetType() == typeof(AnimationClip))
                    {
                        string dir = AssetDatabase.GetAssetPath(dep);
                        assetList.Add(dir);
                    }

                    m_pfbDepList.Add(dep.name + ", " + dep.GetType()
                                + ", " + AssetDatabase.GetAssetPath(dep));
                }
            }
            mainAsset = asset;
            mainAssetName = name;
            return true;
        }

        private static void CreatePath(string path)
        {
            string absname = Path.GetFullPath(path.Trim(trim));
            if (!Directory.Exists(absname))
                Directory.CreateDirectory(absname);
        }

        private static string GetGoName(GameObject go)
        {
            return go.name.Trim(trim).Replace(" ", "_");
        }

        private static void ExpResAssetBundle(string terrainDestDir)
        {
            m_depBundleList.Clear();

            Dictionary<string, string> assetPathBundleMap = new Dictionary<string, string>();

            BuildAssetBundleOptions assetOp = BuildAssetBundleOptions.CollectDependencies |
                BuildAssetBundleOptions.CompleteAssets;
            int idx = 0;
            foreach (List<string> pathList in m_depAssetPathList)
            {
                List<UnityEngine.Object> expAssetList = new List<UnityEngine.Object>();

                string bundleName = RES_ASSET_PREFIX + EditorSceneExporter.GetSceneNameWithoutExtension()
                    + "_" + idx + BUNDLE_EXT;
                string bundlePath = Path.Combine(terrainDestDir, bundleName);
                List<string> depBundles = new List<string>();
                foreach (string assetName in pathList)
                {
                    if (assetPathBundleMap.ContainsKey(assetName))
                    {
                        string existBundleName = assetPathBundleMap[assetName];
                        if (!depBundles.Contains(existBundleName))
                            depBundles.Add(existBundleName);
                    }
                    else
                    {
                        assetPathBundleMap.Add(assetName, bundleName);
                        if (!depBundles.Contains(bundleName))
                            depBundles.Add(bundleName);

                        UnityEngine.Object asset = AssetDatabase.LoadMainAssetAtPath(assetName);
                        expAssetList.Add(asset);
                    }
                }
                m_depBundleList.Add(depBundles);

                if (expAssetList.Count <= 0)
                    continue;

                BuildPipeline.BuildAssetBundle(null, expAssetList.ToArray(),
                                               bundlePath, assetOp,
                                               GOEPack.buildTarget);
                idx++;
            }

            m_depBundleAssetMap.Clear();
            foreach (string asset in assetPathBundleMap.Keys)
            {
                string[] arr = asset.Split(char.Parse("/"));
                string name = arr[arr.Length - 1];
                string bundle = assetPathBundleMap[asset];
                if (m_depBundleAssetMap.ContainsKey(bundle))
                {
                    List<string> assets = m_depBundleAssetMap[bundle];
                    if (!assets.Contains(name))
                        m_depBundleAssetMap[bundle].Add(name);
                }
                else
                {
                    m_depBundleAssetMap.Add(bundle, new List<string>());
                    m_depBundleAssetMap[bundle].Add(name);
                }
            }
        }

        private static void ExpPfbAssetBundle(string terrainDestDir)
        {
            m_groupBundle = PFB_PREFIX + EditorSceneExporter.GetSceneNameWithoutExtension() + BUNDLE_EXT;
            string path = Path.Combine(terrainDestDir, m_groupBundle);
            BuildAssetBundleOptions pfbOp = BuildAssetBundleOptions.CollectDependencies |
                BuildAssetBundleOptions.CompleteAssets;
            BuildPipeline.PushAssetDependencies();
            BuildPipeline.BuildAssetBundleExplicitAssetNames(m_groupAssetList.ToArray(),
                                                             m_groupNameList.ToArray(),
                                                             path, pfbOp,
                                                             GOEPack.buildTarget);
            BuildPipeline.PopAssetDependencies();
        }
        private static void GenScnInfo(string infoDestDir)
        {
            //.byte//
            string name = SCN_INFO_PREFIX + EditorSceneExporter.GetSceneNameWithoutExtension() + SCN_INFO_BIN_EXT;
            string absname = Path.GetFullPath(Path.Combine(infoDestDir, name));
            ExpBinScnInfo(absname);
            //sceneobj.txt//
        }

        private static void ExpTxtScnInfo(string path)
        {
            FileStream fs = new FileStream(path, FileMode.Create);
            StreamWriter writer = new StreamWriter(fs);

            string indent = "";
            writer.Write(indent + "<scene_info>" + "\n");
            indent = "	";
            writer.Write(indent + "<scene"
                         + " name=" + "\"" + EditorSceneExporter.GetSceneNameWithoutExtension() + "\""
                         + " terrain_bundle=" + "\"" + GetTerrainBundleName() + "\""
                         + " group_bundle=" + "\"" + m_groupBundle + "\""
                         + " min=" + "\"" + m_sceneBound.min + "\""
                         + " max=" + "\"" + m_sceneBound.max + "\""
                         + " />" + "\n");

            indent = "	";
            for (int i = 0; i < m_groupGoList.Count; ++i)
            {
                writer.Write(indent + "<group"
                             + " name=" + "\"" + m_groupGoList[i].name + "\""
                             + " asset=" + "\"" + m_groupNameList[i] + "\""
                             + " min=" + "\"" + m_groupBdList[i].min + "\""
                             + " max=" + "\"" + m_groupBdList[i].max + "\""
                             + ">" + "\n");
                indent = "		";
                foreach (string dep in m_depBundleList[i])
                {
                    writer.Write(indent + "<dep"
                                 + " name=" + "\"" + dep + "\""
                                 + "/>" + "\n");
                }

                indent = "	";
                writer.Write(indent + "</group>" + "\n");
            }
            indent = "	";

            foreach (string bundle in m_depBundleAssetMap.Keys)
            {
                writer.Write(indent + "<dep_bundle"
                             + " name= " + "\"" + bundle + "\""
                             + ">" + "\n");

                indent = "		";
                foreach (string asset in m_depBundleAssetMap[bundle])
                {
                    writer.Write(indent + "<asset"
                                 + " name=" + "\"" + asset + "\""
                                 + "/>" + "\n");
                }

                indent = "	";
                writer.Write(indent + "</dep_bundle>" + "\n");
            }

            indent = "	";
            foreach (string str in m_pfbDepList)
            {
                writer.Write(indent + "<dep_asset name=" + "\"" + str + "\"" + "/>" + "\n");
            }

            indent = "";
            writer.Write("</scene_info>" + "\n");

            writer.Flush();
            writer.Close();
            fs.Close();
        }

        private static void ExpBinScnInfo(string path)
        {
            FileStream fs = new FileStream(path, FileMode.Create);
            BinaryWriter writer = new BinaryWriter(fs);

            byte[] val = Encoding.UTF8.GetBytes(BIN_FLAG);
            writer.Write((short)val.Length);
            writer.Write(val);

            val = Encoding.UTF8.GetBytes(EditorSceneExporter.GetSceneNameWithoutExtension());
            writer.Write((short)val.Length);
            writer.Write(val);

            val = Encoding.UTF8.GetBytes(GetTerrainBundleName());
            writer.Write((short)val.Length);
            writer.Write(val);

            val = Encoding.UTF8.GetBytes(m_groupBundle);
            writer.Write((short)val.Length);
            writer.Write(val);

            Bounds bd = m_sceneBound;
            writer.Write((float)bd.min.x);
            writer.Write((float)bd.min.y);
            writer.Write((float)bd.min.z);
            writer.Write((float)bd.max.x);
            writer.Write((float)bd.max.y);
            writer.Write((float)bd.max.z);

            writer.Write(m_groupAssetList.Count);
            for (int i = 0; i < m_groupAssetList.Count; ++i)
            {
                val = Encoding.UTF8.GetBytes(m_groupNameList[i]);
                writer.Write((short)val.Length);
                writer.Write(val);

                bd = m_groupBdList[i];
                writer.Write((float)bd.min.x);
                writer.Write((float)bd.min.y);
                writer.Write((float)bd.min.z);
                writer.Write((float)bd.max.x);
                writer.Write((float)bd.max.y);
                writer.Write((float)bd.max.z);

                writer.Write(m_depBundleList[i].Count);
                foreach (string bundle in m_depBundleList[i])
                {
                    val = Encoding.UTF8.GetBytes(bundle);
                    writer.Write((short)val.Length);
                    writer.Write(val);
                }
            }
            //exp
            writer.Flush();
            writer.Close();
            fs.Close();
        }

        private static Bounds GetGoBound(GameObject go)
        {
            if (go == null)
            {
                Debug.Log("go is null, no bound");
                return new Bounds();
            }

            Bounds bd = new Bounds();
            bool initbd = false;
            foreach (Renderer r in go.GetComponentsInChildren<Renderer>())
            {
                if (initbd)
                {
                    bd.Encapsulate(r.bounds);
                }
                else
                {
                    bd = r.bounds;
                    initbd = true;
                }
            }
            return bd;
        }
    }
}
