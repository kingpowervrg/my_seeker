using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.SceneManagement;
using System.IO;
using conf;

public class PickUpLighting : EditorWindow
{

    string sceneConfigPath = "/Res/SceneConfig/";
    string m_sceneName;
    Dictionary<string, SceneItemJson> items = new Dictionary<string,SceneItemJson>();
    Dictionary<string, List<GameObject>> itemJsonObjs = new Dictionary<string, List<GameObject>>();
    Dictionary<long, List<GameObject>> itemObjs = new Dictionary<long, List<GameObject>>();

    List<BaseItem> baseItemsData = new List<BaseItem>();
    //PickUpLighting window;
    static PickUpLighting window;

    float errorNumber = 0.1f;
    [MenuItem("Tools/Lighting/提取所有光照信息")]
    static void CreateWindow()
    {
        Rect rect = new Rect(0, 0, 500, 300);
        window = (PickUpLighting)EditorWindow.GetWindowWithRect(typeof(PickUpLighting), rect, true, "场景编辑器");
        if (window.Init())
        {
            window.Show();
        }
    }

    private bool Init()
    {
        sceneConfigPath = Application.dataPath + "/" + sceneConfigPath;
        Scene curScene = SceneManager.GetActiveScene();
        if (curScene == null)
        {
            window.Close();
            window = null;
            UnityEditor.EditorUtility.DisplayDialog("Title", "请先打开场景", "是");
            return false;
        }
        m_sceneName = curScene.name;

        string configPath = SceneTools.getConfigPath();
        try
        {
            baseItemsData.Clear();
            System.Reflection.PropertyInfo[] proInfo;
            ConfHelper.LoadFromExcel<BaseItem>(configPath + "exhibit.xlsx", 0, out baseItemsData, out proInfo);
        }
        catch (IOException ex)
        {
            window.Close();
            window = null;
            UnityEditor.EditorUtility.DisplayDialog("Title", "先关表!!!", "是");
            return false;
        }
        return true;
    }

    void OnGUI()
    {
        EditorGUILayout.BeginVertical();
        m_sceneName = EditorGUILayout.TextField("场景名称:", m_sceneName);
        if (GUILayout.Button("加载所有物件"))
        {
            LoadAllItemData();
            LoadAllItemObj();
        }
        if (GUILayout.Button("烘焙"))
        {

        }
        EditorGUILayout.EndVertical();
    }

    void LoadAllItemData()
    {
        ClearItemObj();
        items.Clear();
        string mSceneConfigPath = sceneConfigPath + m_sceneName;
        if (!Directory.Exists(mSceneConfigPath))
        {
            return;
        }
        string[] files = Directory.GetFiles(mSceneConfigPath);
        for (int i = 0; i < files.Length; i++)
        {
            if (!files[i].Contains(".json") || files[i].Contains("meta"))
            {
                continue;
            }
            StreamReader reader = new StreamReader(files[i]);
            string jsonStr = reader.ReadToEnd();
            reader.Close();
            SceneItemJson groupData = fastJSON.JSON.ToObject<SceneItemJson>(jsonStr);
            if (!items.ContainsKey(files[i]))
            {
                items.Add(files[i],groupData);
            }
        }
    }

    void LoadAllItemObj()
    {
        itemJsonObjs.Clear();
        itemObjs.Clear();
        foreach (var kv in items)
        {
            List<GameObject> objs = new List<GameObject>();
            itemJsonObjs.Add(kv.Key, objs);
            for (int i = 0; i < kv.Value.items.Count; i++)
            {
                ItemInfoJson itemJson = kv.Value.items[i];
                for (int j = 0; j < itemJson.itemPos.Count; j++)
                {
                    objs.Add(LoadModel(itemJson.itemID,itemJson.itemPos[j],Path.GetFileName(kv.Key),j));
                }
            }
        }
    }

    GameObject LoadModel(long id,ItemPosInfoJson posInfo,string fileName,int index)
    {
        for (int i = 0; i < baseItemsData.Count; i++)
        {
            if (id == baseItemsData[i].id)
            {
                GameObject parentObj = GameObject.Find(id.ToString() + "_parent");
                if (parentObj == null)
                {
                    parentObj = new GameObject(id.ToString()+ "_parent");
                    parentObj.transform.position = Vector3.zero;
                }

                if (itemObjs.ContainsKey(id))
                {
                    for (int j = 0; j < itemObjs[id].Count; j++)
                    {
                        GameObject mObj = itemObjs[id][j];
                        if (Mathf.Abs(mObj.transform.position.x - posInfo.pos.x) < errorNumber && Mathf.Abs(mObj.transform.position.y - posInfo.pos.y)< errorNumber && Mathf.Abs(mObj.transform.position.z - posInfo.pos.z) < errorNumber)
                        {
                            if (Mathf.Abs(mObj.transform.eulerAngles.x - posInfo.rotate.x) < errorNumber && Mathf.Abs(mObj.transform.eulerAngles.y - posInfo.rotate.y) < errorNumber && Mathf.Abs(mObj.transform.eulerAngles.z - posInfo.rotate.z) < errorNumber)
                            {
                                if (Mathf.Abs(mObj.transform.localScale.x - posInfo.scale.x) < errorNumber && Mathf.Abs(mObj.transform.localScale.y - posInfo.scale.y) < errorNumber && Mathf.Abs(mObj.transform.localScale.z - posInfo.scale.z) < errorNumber)
                                {
                                    return mObj;
                                }
                            }
                        }
                    }
                }

                GameObject obj = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/" + baseItemsData[i].model + ".prefab");
                obj = Instantiate(obj) as GameObject;
                obj.name = id.ToString()+ "/" + index + "/" + fileName;
                obj.transform.SetParent(parentObj.transform);
                obj.isStatic = true;
                MeshRenderer render = obj.GetComponent<MeshRenderer>();
                if (render != null)
                {
                    render.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
                    render.receiveShadows = false;
                    render.lightProbeUsage = UnityEngine.Rendering.LightProbeUsage.Off;
                    render.reflectionProbeUsage = UnityEngine.Rendering.ReflectionProbeUsage.Off;
                }
                obj.transform.position = new Vector3( posInfo.pos.x, posInfo.pos.y, posInfo.pos.z);
                obj.transform.eulerAngles = new Vector3(posInfo.rotate.x, posInfo.rotate.y, posInfo.rotate.z);
                obj.transform.localScale = new Vector3(posInfo.scale.x, posInfo.scale.y, posInfo.scale.z);
                if (itemObjs.ContainsKey(id))
                {
                    itemObjs[id].Add(obj);
                }
                else
                {
                    itemObjs.Add(id, new List<GameObject>() { obj });
                }
                return obj;
            }
        }
        return null;
    }

    void ClearItemObj()
    {
        foreach (var kv in itemObjs)
        {
            for (int i = 0; i < kv.Value.Count; i++)
            {
                if (kv.Value[i] == null)
                {
                    continue;
                }
                if (kv.Value[i].transform.parent != null)
                {
                    GameObject.DestroyImmediate(kv.Value[i].transform.parent.gameObject);
                }
            }
        }

    }

    void OnDisable()
    {
        ClearItemObj();
    }
}
