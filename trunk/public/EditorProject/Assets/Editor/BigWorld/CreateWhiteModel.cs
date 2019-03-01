using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

public class CreateWhiteModel : MonoBehaviour {

    public const string MaterialRootPath = "Assets/Res/Maps/ChengShiDiTu_01/FBX/Materials/";
    public const string WhiteMaterial = "Assets/Res/Maps/ChengShiDiTu_01/FBX/Materials/DiTu_White.mat";
    public const string BigWorldFile = "bigworld.json";
    public static string BigWorldInfo;

    [MenuItem("Tools/BigWorld/设置白模")]
    public static void WhiteModel()
    {
        BigWorldInfo = Application.streamingAssetsPath + "/" + BigWorldFile;
        GameObject obj = GameObject.FindGameObjectWithTag("sceneRoot");
        WhiteModelSystemEditor system = new WhiteModelSystemEditor(obj);
        system.SetWhiteMaterial();
    }

    [MenuItem("Tools/BigWorld/恢复材质球")]
    public static void SetModelMaterial()
    {
        BigWorldInfo = Application.streamingAssetsPath + "/" + BigWorldFile;
        GameObject obj = GameObject.FindGameObjectWithTag("sceneRoot");
		obj = obj.transform.Find ("ChengShiDiTu_03").gameObject;
        WhiteModelSystemEditor system = new WhiteModelSystemEditor(obj);
        system.SetModelMaterial();
    }

    public class WhiteModelSystemEditor
    {
        private Material m_whiteMat = null;
        private GameObject m_Root = null;
        public WhiteModelSystemEditor(GameObject root)
        {
            this.m_whiteMat = AssetDatabase.LoadAssetAtPath<Material>(CreateWhiteModel.WhiteMaterial);
            this.m_Root = root;
        }

        public void SetWhiteMaterial()
        {
            BigWorldData bigdata = new BigWorldData();
            bigdata.bigdatas = new List<BigWorldModelData>();
            int childCount = m_Root.transform.childCount;
            for (int i = 0; i < childCount; i++)
            {
                Transform childTran = m_Root.transform.GetChild(i);
                if (childTran.name.Contains("Group"))
                {
                    MeshRenderer[] renders = childTran.GetComponentsInChildren<MeshRenderer>();
                    for (int j = 0; j < renders.Length; j++)
                    {
                        if (renders[j].name.Equals("wall"))
                        {
                            continue;
                        }
                        BigWorldModelData modelData = new BigWorldModelData();
                        modelData.path = childTran.name + "/" + renders[j].name;
                        modelData.mat = renders[j].sharedMaterial.name + ".mat";
                        bigdata.bigdatas.Add(modelData);
                        renders[j].sharedMaterial = this.m_whiteMat;
                    }
                }
            }
            AssetDatabase.SaveAssets();
            string jsonStr = fastJSON.JSON.ToJSON(bigdata);
            StreamWriter writer = new StreamWriter(CreateWhiteModel.BigWorldInfo,false,System.Text.Encoding.Default);
            writer.Write(jsonStr);
            writer.Close();
        }

        public void SetModelMaterial()
        {
            StreamReader reader = new StreamReader(CreateWhiteModel.BigWorldInfo);
            string jsonStr = reader.ReadToEnd();
            reader.Close();
            BigWorldData bigData = fastJSON.JSON.ToObject<BigWorldData>(jsonStr);
            for (int i = 0; i < bigData.bigdatas.Count; i++)
            {
                Transform trans = this.m_Root.transform.Find(bigData.bigdatas[i].path);
				if (trans == null) {
					Debug.LogError ("city node error : " + bigData.bigdatas[i].path);
					continue;
				}
                MeshRenderer render = trans.GetComponent<MeshRenderer>();
                Material mat = AssetDatabase.LoadAssetAtPath<Material>(CreateWhiteModel.MaterialRootPath + bigData.bigdatas[i].mat);
                render.sharedMaterial = mat;
            }
        }
    }

    public class BigWorldData
    {
        public List<BigWorldModelData> bigdatas { get; set; }
    }

    public class BigWorldModelData
    {
        public string path { get; set; }
        public string mat { get; set; }
    }
}
