using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.SceneManagement;
using System.IO;
using conf;

public class SceneFilterWindow : EditorWindow
{

    string sceneConfigPath = "/Res/SceneConfig/";
    string m_sceneName;
    Dictionary<string, SceneItemJson> items = new Dictionary<string,SceneItemJson>();
    Dictionary<string, List<GameObject>> itemJsonObjs = new Dictionary<string, List<GameObject>>();
    Dictionary<long, List<GameObject>> itemObjs = new Dictionary<long, List<GameObject>>();

    List<BaseItem> baseItemsData = new List<BaseItem>();
    //PickUpLighting window;
    static SceneFilterWindow window;

    float errorNumber = 0.1f;
    [MenuItem("Tools/Lighting/场景物件展示")]
    static void CreateWindow()
    {
        Rect rect = new Rect(0, 0, 500, 300);
        window = (SceneFilterWindow)EditorWindow.GetWindowWithRect(typeof(SceneFilterWindow), rect, true, "场景物件展示");
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

        LoadGroups();

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
    int sumItemCount = 0;
    int filterItemCount = 0;
    List<string> groupIDs = new List<string>();
    List<string> groupFilePaths = new List<string>();
    int groupSelectIndex = 0;
    SceneItemJson m_currentItemJson = null;
    List<GameObject> currentItemobjs = new List<GameObject>();
    void OnGUI()
    {
        EditorGUILayout.BeginVertical();
        m_sceneName = EditorGUILayout.TextField("场景名称:", m_sceneName);

        groupSelectIndex = EditorGUILayout.Popup(groupSelectIndex, groupIDs.ToArray());
        sumItemCount = EditorGUILayout.IntField("总共数量：", sumItemCount);
        filterItemCount = EditorGUILayout.IntField("筛选数量：", filterItemCount);

        if (GUILayout.Button("统计总数量"))
        {
            GetAllItemCount();
        }
        if (GUILayout.Button("加载数据"))
        {
            LoadAllItemObj();
        }
        EditorGUILayout.EndVertical();
    }

    void LoadGroups()
    {
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
            string[] allFiles = files[i].Split('\\');
            groupIDs.Add(allFiles[allFiles.Length - 1]);
            groupFilePaths.Add(files[i]);
        }
    }


    void LoadAllItemData()
    {
        //ClearItemObj();
        items.Clear();
        if (groupFilePaths.Count > groupSelectIndex)
        {
            StreamReader reader = new StreamReader(groupFilePaths[groupSelectIndex]);
            string jsonStr = reader.ReadToEnd();
            reader.Close();
            m_currentItemJson = fastJSON.JSON.ToObject<SceneItemJson>(jsonStr);
        }
    }

    void GetAllItemCount()
    {
        LoadAllItemData();
        sumItemCount = m_currentItemJson.items.Count;
    }

    List<int> selectItemInfoJson = new List<int>();

    private void GetRandom()
    {
        int random = Random.Range(0, sumItemCount-1);
        CheckRandom(ref random);
        selectItemInfoJson.Add(random);
    }

    private void CheckRandom(ref int random)
    {
        if (selectItemInfoJson.Contains(random) || m_currentItemJson.items[random].itemPos[0].type == 1)
        {
            random++;
            random = random % sumItemCount;
            CheckRandom(ref random);
        }
    }

    void LoadAllItemObj()
    {
        ClearItemObj();
        selectItemInfoJson.Clear();
        for (int i = 0; i < filterItemCount; i++)
        {
            GetRandom();
        }

        for (int j = 0; j < m_currentItemJson.items.Count; j++)
        {
            if (m_currentItemJson.items[j].itemPos[0].type == 1)
            {
                if (selectItemInfoJson.Contains(j))
                {
                    Debug.LogError("random error : task item");
                    continue;
                }
                selectItemInfoJson.Add(j);
            }
        }

        for (int i = 0; i < selectItemInfoJson.Count; i++)
        {
            ItemInfoJson itemJson = m_currentItemJson.items[selectItemInfoJson[i]];
            int posRan = Random.Range(0,itemJson.itemPos.Count);
            currentItemobjs.Add(LoadModel(itemJson.itemID, itemJson.itemPos[posRan]));
        }


        //foreach (var kv in items)
        //{
        //    List<GameObject> objs = new List<GameObject>();
        //    itemJsonObjs.Add(kv.Key, objs);
        //    for (int i = 0; i < kv.Value.items.Count; i++)
        //    {
        //        ItemInfoJson itemJson = kv.Value.items[i];
        //        for (int j = 0; j < itemJson.itemPos.Count; j++)
        //        {
        //            objs.Add(LoadModel(itemJson.itemID,itemJson.itemPos[j],Path.GetFileName(kv.Key),j));
        //        }
        //    }
        //}
    }

    GameObject LoadModel(long id,ItemPosInfoJson posInfo)
    {
        for (int i = 0; i < baseItemsData.Count; i++)
        {
            if (id == baseItemsData[i].id)
            {
                GameObject parentObj = GameObject.Find(id.ToString() + "_parent");
                if (parentObj == null)
                {
                    parentObj = new GameObject(id.ToString() + "_parent");
                    parentObj.transform.position = Vector3.zero;
                }

                GameObject obj = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/" + baseItemsData[i].model + ".prefab");
                obj = Instantiate(obj) as GameObject;
                obj.name = id.ToString();
                obj.transform.SetParent(parentObj.transform);
                obj.isStatic = true;
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
        for (int i = 0; i < currentItemobjs.Count; i++)
        {
            if (currentItemobjs[i].transform.parent != null)
            {
                GameObject.DestroyImmediate(currentItemobjs[i].transform.parent.gameObject);
            }
        }
        currentItemobjs.Clear();
    }

    void OnDisable()
    {
        ClearItemObj();
    }
}
