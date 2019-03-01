using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.SceneManagement;
using System.IO;
using conf;

public class EditorSceneObj
{
    public GameObject rootObj;
    public List<GameObject> itemObj;
}

/// <summary>
/// 场景数据保存命名规则
/// 场景文件夹
///     场景数据_1
///     场景数据_2
/// </summary>
public class SceneWindow : EditorWindow {

    SceneItemInfo itemInfos;
    //SceneData m_sceneData; //该场景所有数据

    private string pathDir = "/Res/SceneData/";
    private string jsonPath = "/Res/SceneConfig/";

    List<EditorSceneObj> itemObjs = new List<EditorSceneObj>();
    //List<GameObject> itemobjs = new List<GameObject>(); //所有测试对象

    List<string> m_allAssetPath = new List<string>(); //所有本地数据路径
    List<string> m_allAssetName = new List<string>(); //所有本地数据名称

    List<BaseItem> baseItemsData = new List<BaseItem>(); 
    private string sceneName;
    private long sceneID = -1;
    public GUIStyle btnSelectStyle;
    public static SceneWindow window;

    private List<string> m_allCameraNode = new List<string>();

    private List<SceneItemInfo> m_allItemInfo = new List<SceneItemInfo>();
    [MenuItem("Tools/场景编辑器")]
	static void CreateWindow()
	{
		Rect rect = new Rect (0,0,800,700);
		window = (SceneWindow)EditorWindow.GetWindowWithRect (typeof(SceneWindow),rect,true,"场景编辑器");
        if (window.Init())
        {
            window.Show();
        }
    }

	bool Init()
	{
        //load base item for table
        Scene scene = SceneManager.GetActiveScene();
        if (scene != null)
        {
            sceneName = scene.name;
            pathDir = "Assets" + pathDir + scene.name + "/"; //+ ".asset";
        }

        if (string.IsNullOrEmpty(sceneName) || sceneName.Equals("EmptyScene") || sceneName.Equals("main_ui"))
        {
            UnityEditor.EditorUtility.DisplayDialog("Title", "当前场景为空!!!", "是");
            return false;
        }

        m_allItemInfo = JsonToSceneData.RevertAsset();

        lightTempPath += sceneName;
        scenePath += sceneName;

        jsonPath = Application.dataPath + jsonPath + sceneName + "/";
        string configPath = SceneTools.getConfigPath();
        try {
            System.Reflection.PropertyInfo[] proInfo;
            ConfHelper.LoadFromExcel<BaseItem>(configPath + "exhibit.xlsx", 0, out baseItemsData, out proInfo);

            List<SceneConfig> sceneConfig;
            System.Reflection.PropertyInfo[] sceneProInfo;
            ConfHelper.LoadFromExcel<SceneConfig>(configPath + "sceneResEditor.xlsx", 0, out sceneConfig, out sceneProInfo);
            sceneID = getSceneID(sceneConfig);
            //if (!Directory.Exists(pathDir))
            //{
            //    Directory.CreateDirectory(pathDir);
            //    //创建asset 
            //    itemInfos = ScriptableObject.CreateInstance<SceneItemInfo>();
            //    itemInfos.items = new List<ItemInfo>();
            //    AssetDatabase.CreateAsset(itemInfos, pathDir + sceneName + "_0.asset");
            //    AssetDatabase.Refresh();
            //}
            //string[] allAssetPath = Directory.GetFiles(pathDir, "*.asset");
            for (int i = 0; i < m_allItemInfo.Count; i++)
            {
                m_allAssetName.Add(m_allItemInfo[i].jsonName);
            }
            LoadAssetData();

            GameObject[] cameraPoint = GameObject.FindGameObjectsWithTag("cameraPoint");
            m_allCameraNode.Add("MainCamera");
            for (int i = 0; i < cameraPoint.Length; i++)
            {
                m_allCameraNode.Add(cameraPoint[i].name);
            }
            UserLog.OpenSceneLog();
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

    /// <summary>
    /// 获取场景ID
    /// </summary>
    /// <param name="sceneConfig"></param>
    /// <returns></returns>
    long getSceneID(List<SceneConfig> sceneConfig)
    {
        for (int i = 0; i < sceneConfig.Count; i++)
        {
            if (sceneConfig[i].sceneInfo.Equals(sceneName))
            {
                return sceneConfig[i].id;
            }
        }
        return -1;
    }

    /// <summary>
    /// 根据ID获取物品
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    BaseItem getBaseItemByID(long id)
    {
        if (baseItemsData == null)
        {
            return null;
        }
        for (int i = 0; i < baseItemsData.Count; i++)
        {
            if (baseItemsData[i].id == id)
            {
                return baseItemsData[i];
            }
        }
        return null;
    }
    //加载标签页数据
    void LoadAssetData()
    {
        if (m_allItemInfo.Count > sceneDataIndex)
        {
            itemInfos = m_allItemInfo[sceneDataIndex];//AssetDatabase.LoadAssetAtPath<SceneItemInfo>(m_allAssetPath[sceneDataIndex]);
            selectDataIndex = sceneDataIndex;
        }
        selectIndex = -1;
        selectItem = null;
        DestoryItemObjs();
        CreateItemObjs();
    }

    //添加新标签页
    void AddDataPage()
    {
        string pageName = string.Empty;
        getNewDataPageName(m_allAssetName.Count,ref pageName);
        SceneItemInfo sceneInfo = new SceneItemInfo();//ScriptableObject.CreateInstance<SceneItemInfo>();
        sceneInfo.items = new List<ItemInfo>();
        sceneInfo.jsonName = pageName;
        m_allItemInfo.Add(sceneInfo);
        m_allAssetName.Add(pageName);
        UserLog.AddPageSceneLog(pageName);
    }

    //删除标签页
    void RemoveDataPage()
    {
        if (selectDataIndex < 0)
        {
            return;
        }
        if (m_allItemInfo.Count < 2)
        {
            return;
        }
        UserLog.RemovePageSceneLog(m_allItemInfo[selectDataIndex].jsonName);
        selectIndex = -1;
        selectItem = null;
        DestoryItemObjs();
        m_allItemInfo.RemoveAt(selectDataIndex);
        //File.Delete(m_allAssetPath[selectDataIndex]);
        //m_allAssetPath.RemoveAt(selectDataIndex);
        m_allAssetName.RemoveAt(selectDataIndex);
        sceneDataIndex = 0;
        selectDataIndex = 0;
        LoadAssetData();
        //AssetDatabase.Refresh();
    }
    //获取新页名称  名称自增
    void getNewDataPageName(int count,ref string pageName)
    {
        string tempPath = jsonPath + sceneID * 100 + count + ".json";
        if (!File.Exists(tempPath))
        {
            pageName = sceneID * 100 + count + ".json";
            return;
        }
        getNewDataPageName(++count,ref pageName);
    }

    #region 测试对象相关
    /// <summary>
    /// 创建对象
    /// </summary>
    void CreateItemObjs()
    {
        if (itemInfos == null)
        {
            return;
        }
        for (int i = 0; i < itemInfos.items.Count; i++)
        {
            ItemInfo info = itemInfos.items[i];
            AddSingleItemObj(info);
        }
    }

    void AddSingleItemObj(ItemInfo info)
    {
        GameObject objParent = new GameObject(info.itemName);
        objParent.transform.position = Vector3.zero;
        EditorSceneObj sceneObj = new EditorSceneObj();
        sceneObj.rootObj = objParent;
        sceneObj.itemObj = new List<GameObject>();
        itemObjs.Add(sceneObj);

        for (int j = 0; j < info.itemPos.Count; j++)
        {
            //加载资源
            AddNodeObj(info, info.itemPos[j], sceneObj,false);
        }
    }

    //增加位置节点
    void AddNodeObj(ItemInfo info,ItemPosInfo posInfo,EditorSceneObj sceneObj,bool isNew)
    {
        BaseItem baseItem = getBaseItemByID(info.itemID);
        if (baseItem == null)
        {
            return;
        }
        //加载资源
        GameObject obj = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/" + baseItem.model + ".prefab");
        obj = Instantiate(obj) as GameObject;
        obj.name = info.itemName;
        obj.transform.SetParent(sceneObj.rootObj.transform);
        obj.isStatic = true;
        if (posInfo != null)
        {
            obj.transform.position = posInfo.pos;
            obj.transform.eulerAngles = posInfo.rotate;
            obj.transform.localScale = (posInfo.scale == Vector3.zero)? Vector3.one:posInfo.scale;
        }
        else
        {
            obj.transform.position = Vector3.zero;
            obj.transform.eulerAngles = Vector3.zero;
        }
        sceneObj.itemObj.Add(obj);
        if(isNew)
            UserLog.AddNodeSceneLog(m_allAssetName[selectDataIndex], info.itemID, sceneObj.itemObj.Count - 1);
    }

    void DestoryItemObjs()
    {
        for (int i = 0; i < itemObjs.Count; i++)
        {
            if (itemObjs[i] != null)
            {
                GameObject.DestroyImmediate(itemObjs[i].rootObj);
            }
        }
        itemObjs.Clear();
    }
    #endregion

    Vector2 itemBeginScroll = new Vector2(0,20);
    Vector2 itemInfoBenginScroll = new Vector2(250,20);
    int sceneDataIndex = 0;
    int selectDataIndex = 0;  //当前选中标签页
    bool isRunning = false;
	void OnGUI()
	{
        if (string.IsNullOrEmpty(sceneName))
        {
            EditorGUILayout.HelpBox("请先保存场景", MessageType.Error);
            return;
        }
        //if (lightMapTex == null)
        //{
        //    EditorGUILayout.HelpBox("请先烘焙场景",MessageType.Error);
        //}

        if (!Lightmapping.isRunning && isRunning)
        {
            isRunning = false;
            SaveLightData();
        }

        EditorGUILayout.BeginVertical();
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("当前场景名称:" + sceneName);
        //lightMapTex = (Texture2D)EditorGUILayout.ObjectField(lightMapTex, typeof(Texture2D), true);
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        
        if (selectDataIndex >= 0 && m_allAssetName.Count > selectDataIndex && m_allAssetName[selectDataIndex] != null)
        {
            EditorGUILayout.LabelField("当前场景数据:" + m_allAssetName[selectDataIndex]);
        }
        sceneDataIndex = EditorGUILayout.Popup(sceneDataIndex, m_allAssetName.ToArray());
        if (selectDataIndex != sceneDataIndex)
        {
            LoadAssetData();
        }
        if (GUILayout.Button("添加分页"))
        {
            AddDataPage();
        }
        if (GUILayout.Button("删除分页"))
        {
            RemoveDataPage();
        }

        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("添加物品"))
        {
            AddItem();
        }
        if (GUILayout.Button("添加节点"))
        {
            AddItemNode();
        }
        if (GUILayout.Button("移除物品"))
        {
            RemoveItem();
        }
        if (GUILayout.Button("输出"))
        {
            SaveItem();
        }

        if (GUILayout.Button("提取场景光照"))
        {
            SceneBake();
        }

        if (GUILayout.Button("同步所有数据"))
        {
            // SceneBake();
            if (UnityEditor.EditorUtility.DisplayDialog("Title", "别点错页签了，下手谨慎点----", "是", "取消"))
            {
                Debug.Log("SyncItem ===");
                SyncAllItem();
            }
        }

        if (GUILayout.Button("同步页签数据"))
        {
            // SceneBake();
            ScenePageWindow pageWin = ScenePageWindow.CreateWindow();
            pageWin.SetAllAssetName(m_allAssetName);
            pageWin.Show();
        }

        EditorGUILayout.EndHorizontal();

        #region 具体物品
        EditorGUILayout.BeginHorizontal();

        EditorGUILayout.BeginVertical();
        itemBeginScroll = EditorGUILayout.BeginScrollView(itemBeginScroll, GUILayout.Width(250),GUILayout.Height(580));
        InitItem();
        EditorGUILayout.EndScrollView();
        EditorGUILayout.EndVertical();

        GUILayout.BeginVertical();
        itemInfoBenginScroll = EditorGUILayout.BeginScrollView(itemInfoBenginScroll, GUILayout.Width(550), GUILayout.Height(580));
        ItemClick(selectItem);
        EditorGUILayout.EndScrollView();
        GUILayout.EndVertical();

        EditorGUILayout.EndHorizontal();
        #endregion
        EditorGUILayout.EndVertical();
	}

    void InitItem()
    {
        for (int i = 0; i < itemInfos.items.Count; i++)
        {
            ItemInfo iteminfo = itemInfos.items[i];
            if (GUILayout.Button(iteminfo.itemID + "/" + iteminfo.itemName))
            {
                setSelectItem(iteminfo,i);
            }
        }
    }

    void setSelectItem(ItemInfo info,int index)
    {
        selectItem = info;
        selectIndex = index;
    }

    ItemInfo selectItem = null; //当前列表选中的物品
    int selectIndex = -1;
    GameObject objs = null;
    void ItemClick(ItemInfo info)
    {
        if (info == null)
        {
            return;
        }
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("物件概率");
        info.itemPercent = EditorGUILayout.FloatField(info.itemPercent);
        EditorGUILayout.EndHorizontal();

        for (int i = 0; i < info.itemPos.Count; i++)
        {
            ItemPosInfo posInfo = info.itemPos[i];
            ItemInfoShow(posInfo,i);
        }
    }

    void ItemInfoShow(ItemPosInfo posInfo,int i)
    {
        if (posInfo == null)
        {
            return;
        }
        EditorGUILayout.BeginVertical();

        if (selectIndex >= 0 && itemObjs[selectIndex] != null)
        {
            itemObjs[selectIndex].itemObj[i] = (GameObject)EditorGUILayout.ObjectField(itemObjs[selectIndex].itemObj[i], typeof(GameObject), true);
        }
        posInfo.type = (ItemType)EditorGUILayout.EnumPopup("物品类型:", posInfo.type);

        posInfo.pos = EditorGUILayout.Vector3Field("位置",posInfo.pos);
        posInfo.rotate = EditorGUILayout.Vector3Field("旋转", posInfo.rotate);
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("位置概率");
        posInfo.percent = EditorGUILayout.FloatField(posInfo.percent);
        
        EditorGUILayout.EndHorizontal();
        int cameraIndex = 0;
        for (int m = 0; m < m_allCameraNode.Count; m++)
        {
            if (m_allCameraNode[m].Equals(posInfo.m_cameraNode))
            {
                cameraIndex = m;
                break;
            }
        }
        posInfo.m_cameraNode = m_allCameraNode[EditorGUILayout.Popup(cameraIndex, m_allCameraNode.ToArray())];
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("设置坐标"))
        {
            if (selectItem != null && selectIndex >= 0 && itemObjs[selectIndex] != null)
            {
                posInfo.pos = itemObjs[selectIndex].itemObj[i].transform.position;
                posInfo.rotate = itemObjs[selectIndex].itemObj[i].transform.eulerAngles;
            }
        }

        if (GUILayout.Button("跳转"))
        {
            if (selectItem != null && selectIndex >= 0 && itemObjs[selectIndex] != null)
            {
                itemObjs[selectIndex].itemObj[i].transform.position = posInfo.pos;
                itemObjs[selectIndex].itemObj[i].transform.eulerAngles = posInfo.rotate;
            }
        }

        if (GUILayout.Button("同步"))
        {
            if (selectItem == null)
            {
                return;
            }
            if (selectItem != null && selectIndex >= 0 && itemObjs[selectIndex] != null)
            {
                Transform itemTran = itemObjs[selectIndex].itemObj[i].transform;
                posInfo.pos = itemTran.position;
                posInfo.rotate = itemTran.eulerAngles;
                SysncItem(selectItem.itemID, i, itemTran, posInfo.m_cameraNode);
            }
        }

        if (GUILayout.Button("同步单个"))
        {
            if (selectItem == null)
            {
                return;
            }
            if (selectItem != null && selectIndex >= 0 && itemObjs[selectIndex] != null)
            {
                Transform itemTran = itemObjs[selectIndex].itemObj[i].transform;
                SceneSyncItemWindow syncItemWin = SceneSyncItemWindow.CreateWindow();
                syncItemWin.SetData(m_allItemInfo, selectItem.itemID,i, itemTran.gameObject,selectDataIndex,selectItem.itemPos[i].m_cameraNode);
                //posInfo.pos = itemTran.position;
                //posInfo.rotate = itemTran.eulerAngles;
                //SysncItem(selectItem.itemID, i, itemTran.position, itemTran.localScale, itemTran.eulerAngles);
            }
        }

        if (GUILayout.Button("删除"))
        {
            if (selectItem != null)
            {
                selectItem.itemPos.Remove(posInfo);
                if (selectItem != null && selectIndex >= 0 && itemObjs[selectIndex] != null)
                {
                    UserLog.RemoveNodeSceneLog(m_allAssetName[selectDataIndex], selectItem.itemID, i);
                    GameObject.DestroyImmediate(itemObjs[selectIndex].itemObj[i]);
                    itemObjs[selectIndex].itemObj.RemoveAt(i);
                }
            }
        }

        if (GUILayout.Button("删除整组"))
        {
            if (selectItem != null)
            {
                SysncDeleteItem(selectItem.itemID, i);
                if (selectItem != null && selectIndex >= 0 && itemObjs[selectIndex] != null)
                {
                    GameObject.DestroyImmediate(itemObjs[selectIndex].itemObj[i]);
                    itemObjs[selectIndex].itemObj.RemoveAt(i);
                }
            }
        }

        EditorGUILayout.EndHorizontal();
        EditorGUILayout.EndVertical();
        EditorGUILayout.LabelField("----------------------------------------------------------------------------------------------------------");
        EditorGUILayout.Space();
    }

    void SysncItem(long id, int index,Transform trans,string cameraPoint) //Vector3 pos,Vector3 scale,Vector3 rotation
    {
        List<SceneLogSyncData> logData = new List<SceneLogSyncData>();
        SceneLogSyncData ld = new SceneLogSyncData(m_allAssetName[selectDataIndex], id, index);
        logData.Add(ld);
        for (int i = 0; i < m_allItemInfo.Count; i++)
        {
            SceneItemInfo scenePageData = m_allItemInfo[i]; //AssetDatabase.LoadAssetAtPath<SceneItemInfo>(m_allAssetPath[i]);
            for (int j = 0; j < scenePageData.items.Count; j++)
            {
                if (scenePageData.items[j].itemID == id)
                {
                    if (scenePageData.items[j].itemPos.Count <= index)
                    {
                        Debug.LogError("sysnc error index : " + id + "  " + index + "  page:" + m_allItemInfo[i].jsonName);
                        continue;
                    }
                    SceneLogSyncData ld1 = new SceneLogSyncData(m_allItemInfo[i].jsonName,id,index);
                    logData.Add(ld1);
                    scenePageData.items[j].itemPos[index].pos = trans.position;//pos;
                    scenePageData.items[j].itemPos[index].rotate = trans.eulerAngles;//rotation;
                    scenePageData.items[j].itemPos[index].scale = trans.localScale;//scale;
                    scenePageData.items[j].itemPos[index].m_cameraNode = cameraPoint;
                    break;
                } 
            }
        }
        UserLog.SyncSceneLog(logData, trans.position, trans.eulerAngles, trans.localScale);
    }

    void SysncDeleteItem(long id, int index)
    {
        List<SceneLogSyncData> logData = new List<SceneLogSyncData>();
        for (int i = 0; i < m_allItemInfo.Count; i++)
        {
            SceneItemInfo scenePageData = m_allItemInfo[i];//AssetDatabase.LoadAssetAtPath<SceneItemInfo>(m_allAssetPath[i]);
            for (int j = 0; j < scenePageData.items.Count; j++)
            {
                if (scenePageData.items[j].itemID == id)
                {
                    if (scenePageData.items[j].itemPos.Count > index)
                    {
                        SceneLogSyncData ld = new SceneLogSyncData(m_allItemInfo[i].jsonName,id,index);
                        logData.Add(ld);
                        scenePageData.items[j].itemPos.RemoveAt(index);
                    }
                    break;
                }
            }
        }
        UserLog.RemovePageItemSceneLog(logData);
    }

    void AddItem()
    {
        Rect rect = new Rect(0, 0, 250, 500);
        SceneAddItem window = (SceneAddItem)EditorWindow.GetWindowWithRect(typeof(SceneAddItem), rect, true, "场景编辑器");
        window.Init(baseItemsData);
        window.delSelectItem = AddNewItem;
        window.Show();
    }

    void AddNewItem(List<BaseItem> selectItems)
    {
        for (int i = 0; i < selectItems.Count; i++)
        {
            BaseItem item = selectItems[i];
            if (item != null)
            {
                ItemInfo itemInfo = new ItemInfo();
                itemInfo.itemID = item.id;
                itemInfo.itemName = item.name;
                itemInfo.itemPos = new List<ItemPosInfo>();
                if (itemInfos != null)
                {
                    UserLog.AddItemSceneLog(m_allAssetName[selectDataIndex],item.id);
                    itemInfos.items.Add(itemInfo);
                    AddSingleItemObj(itemInfo);
                }
            }
        }

    }

    /// <summary>
    ///保存所有对象位置数据
    /// </summary>
    void SetAllObjPos()
    {
        for (int i = 0; i < itemInfos.items.Count; i++)
        {
            for (int j = 0; j < itemInfos.items[i].itemPos.Count; j++)
            {
                ItemPosInfo posInfo = itemInfos.items[i].itemPos[j];
                posInfo.pos = itemObjs[i].itemObj[j].transform.position;
                posInfo.rotate = itemObjs[i].itemObj[j].transform.localEulerAngles;
                posInfo.scale = itemObjs[i].itemObj[j].transform.localScale;
            }
        }
    }

    void SaveItem()
    {
        SetAllObjPos();
        SaveJsonData();
        UserLog.SaveSceneLog();
        UnityEditor.EditorUtility.DisplayDialog("Title", "保存成功！！！", "是");
    }

    public Texture2D lightMapTex;  //当前场景LightMap
    string lightTempPath = "/Res/SceneConfig/";
    string scenePath = "/Res/Scene/";
    const string norLightMapName = "Lightmap-0_comp_light.exr";  //基础场景光照贴图
    //const string tempLightMapName = "LightmapTemp.exr"; //临时lightmap
    //const string tempTargetLightName = "LightmapTarget.exr"; //第二张贴图
    void SceneBake()
    {
        SetAllObjPos();
        if (LightmapSettings.lightmaps != null && LightmapSettings.lightmaps.Length > 0)
        {
            lightMapTex = LightmapSettings.lightmaps[0].lightmapColor;
            if (!Directory.Exists(Application.dataPath + lightTempPath))
            {
                Directory.CreateDirectory(Application.dataPath + lightTempPath);
            }
            string lightPath = Application.dataPath + scenePath + "/" + norLightMapName;
            string tempLightPath = Application.dataPath + lightTempPath + "/" + norLightMapName;
            if (File.Exists(lightPath) && !File.Exists(tempLightPath))
            {
                File.Move(lightPath, tempLightPath);
            }
            
        }
        AssetDatabase.Refresh();
        SaveBaseSceneLight();
        //重新烘焙场景
        SceneLightHelper.SceneBake();
        isRunning = true;
    }

    /// <summary>
    /// 恢复基础场景烘焙
    /// </summary>
    void RecoverLightMap()
    {
        string lightPath = Application.dataPath + scenePath + "/" + norLightMapName;
        string tempLightPath = Application.dataPath + lightTempPath + "/" + norLightMapName;
        //string lightTargetPath = Application.dataPath + lightTempPath + "/" + tempTargetLightName;
        //string tempLightPath = Application.dataPath + lightTempPath + "/" + tempLightMapName;
        if (!File.Exists(tempLightPath))
        {
            return;
        }
        if (File.Exists(lightPath))
        {
            File.Delete(lightPath);
        }
        File.Move(tempLightPath,lightPath);
        AssetDatabase.Refresh();
        Texture2D tex = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/" + scenePath + "/" + norLightMapName);
        LightmapSettings.lightmaps[0].lightmapColor = tex;
    }

    /// <summary>
    /// 保存光照信息
    /// </summary>
    void SaveLightData()
    {
        #region  老版本
        /*
        //提前光照信息
        if (selectDataIndex >= 0 && m_allAssetName.Count > selectDataIndex)
        {
            string lightPath = "Assets/Res/Scene/" + sceneName + "/Lightmap-0_comp_light.exr";
            TextureImporter textureImporter = AssetImporter.GetAtPath(lightPath) as TextureImporter;
            if (textureImporter == null)
            {
                return;
            }
            textureImporter.isReadable = true;
            AssetDatabase.ImportAsset(lightPath);
            Texture2D tex = AssetDatabase.LoadAssetAtPath<Texture2D>(lightPath);
            string[] assetName = m_allAssetName[selectDataIndex].Split('.');
            if (assetName.Length == 2 && tex != null)
            {
                string mapPath = jsonPath + "lightData/";
                if (!Directory.Exists(mapPath))
                {
                    Directory.CreateDirectory(mapPath);
                }
                SceneLightHelper.createObjLightData(itemInfos, mapPath + assetName[0] + "_light.json", itemObjs, tex);
            }
        }*/
        #endregion

        if (itemInfos == null || itemObjs == null)
        {
            return;
        }
        string lightPath = Application.dataPath + scenePath + "/" + norLightMapName;
        string tempLightPath = Application.dataPath + lightTempPath + "/" + norLightMapName;
        if (File.Exists(tempLightPath))
        {
            if (selectDataIndex >= 0 && selectDataIndex < m_allAssetName.Count)
            {
                string[] assetNames = m_allAssetName[selectDataIndex].Split('.');
                if (assetNames.Length == 2)
                {
                    string groupLightPath = Application.dataPath + lightTempPath + "/" + assetNames[0] + ".exr";
                    if (itemInfos != null)
                    {
                        itemInfos.lightMapName = assetNames[0] + ".exr";
                    }
                    File.Copy(lightPath, groupLightPath,true);
                }
            }
        }
        AssetDatabase.Refresh();
        SceneLightHelper.setLightMapData(itemInfos,itemObjs);
        SaveJsonData();
    }

    /// <summary>
    /// 保存位置数据
    /// </summary>
    void SaveJsonData()
    {
        if (!Directory.Exists(jsonPath))
        {
            Directory.CreateDirectory(jsonPath);
        }
        ///获取服务端数据
        //SceneItemServerJson serverJson = fastJSON.JSON.ToObject<SceneItemServerJson>();
        SceneDataJson serverJson = getServerData();
        if (serverJson == null)
        {
            serverJson = new SceneDataJson(); //服务端数据
            serverJson.sceneDatas = new List<SceneItemServerJson>();
        }
        //SceneDataJson sceneDataJson = new SceneDataJson(); //服务端数据
        //sceneDataJson.sceneDatas = new List<SceneItemServerJson>();
        for (int i = 0; i < m_allItemInfo.Count; i++)
        {
            SceneItemInfo scenePageData = m_allItemInfo[i];//AssetDatabase.LoadAssetAtPath<SceneItemInfo>(m_allAssetPath[i]);
            if (scenePageData != null)
            {
                SceneItemJson itemJson = new SceneItemJson(scenePageData);
                //sceneDataJson.sceneDatas.Add(itemJson);
                string jsonStr = fastJSON.JSON.ToJSON(itemJson);
                SceneLightHelper.CreateJson(jsonPath + scenePageData.jsonName, jsonStr);

                int groupID = int.Parse(scenePageData.jsonName.Split('.')[0]);
                SceneItemServerJson itemServerJson = new SceneItemServerJson(scenePageData);
                itemServerJson.groupId = groupID;
                //sceneDataJson.sceneDatas.Add(itemServerJson);
                bool isexist = false;
                for (int j = 0; j < serverJson.sceneDatas.Count; j++)
                {
                    SceneItemServerJson serverJson00 = serverJson.sceneDatas[j];
                    if (serverJson00.groupId == groupID)
                    {
                        serverJson.sceneDatas[j] = itemServerJson;
                        isexist = true;
                        break;
                    }
                }
                if (!isexist)
                {
                    serverJson.sceneDatas.Add(itemServerJson);
                }
            }
        }

        //服务端数据
        string serverJsonStr = fastJSON.JSON.ToJSON(serverJson);
        SceneLightHelper.CreateJson(Application.streamingAssetsPath + "/SceneData/scenedata.json", serverJsonStr);
    }


    SceneDataJson getServerData()
    {
        string serverPath = Application.streamingAssetsPath + "/SceneData/scenedata.json";
        if (!File.Exists(serverPath))
        {
            return new SceneDataJson();
        }
        StreamReader reader = new StreamReader(serverPath);
        string datastr = reader.ReadToEnd();
        reader.Close();
        SceneDataJson serverJson = fastJSON.JSON.ToObject<SceneDataJson>(datastr);
        return serverJson;
    }

    long getGroupID(int i)
    {
        if (sceneID >= 0)
        {
            return sceneID * 100 + i;
        }
        return -1;
    }

    List<ItemPosInfo> srcObjLightData = new List<ItemPosInfo>(); //记录基础场景烘焙的数据

    void SaveBaseSceneLight()
    {
        GameObject[] rootObjs = GameObject.FindGameObjectsWithTag("LightingMap");
        srcObjLightData.Clear();
        for (int j = 0; j < rootObjs.Length; j++)
        {
            GameObject rootObj = rootObjs[j];
            if (rootObj == null)
            {
                continue;
            }
            MeshRenderer[] srcRender = rootObj.transform.GetComponentsInChildren<MeshRenderer>();
            for (int i = 0; i < srcRender.Length; i++)
            {
                ItemPosInfo posInfo = new ItemPosInfo();
                SceneLightHelper.SetLightData(srcRender[i], posInfo);
                srcObjLightData.Add(posInfo);
            }
        }
        //GameObject rootObj = GameObject.Find("Scene");
        
    }

    void SetBaseSceneLight()
    {
        GameObject rootObj = GameObject.Find("Scene");
        if (rootObj == null)
        {
            return;
        }
        MeshRenderer[] srcRender = rootObj.transform.GetComponentsInChildren<MeshRenderer>();
        if (srcRender.Length != srcObjLightData.Count)
        {
            return;
        }
        for (int i = 0; i < srcRender.Length; i++)
        {
            if (srcRender[i] == null)
            {
                continue;
            }
            SceneLightHelper.getLightData(srcRender[i], srcObjLightData[i]);
        }
    }

    void RemoveItem()
    {
        if (selectItem != null)
        {
            UserLog.RemoveItemSceneLog(m_allAssetName[selectDataIndex], selectItem.itemID);
            itemInfos.items.Remove(selectItem);
        }
        if (selectIndex >= 0 && itemObjs[selectIndex] != null)
        {
            GameObject.DestroyImmediate(itemObjs[selectIndex].rootObj);
            itemObjs.RemoveAt(selectIndex);
        }
        selectItem = null;
        selectIndex = -1;
    }

    /// <summary>
    /// 增加位置节点
    /// </summary>
    void AddItemNode()
    {
        if (selectItem == null || selectIndex < 0)
        {
            return;
        }
        ItemPosInfo posInfo = new ItemPosInfo();
        BaseItem baseItem = getBaseItemByID(selectItem.itemID);
        if (baseItem == null)
        {
            return;
        }
        if (baseItem.isStory  == 1 && selectItem.itemPos.Count >= 1)
        {
            return;
        }
        posInfo.type = (ItemType)baseItem.isStory;
        selectItem.itemPos.Add(posInfo);
        AddNodeObj(selectItem, posInfo, itemObjs[selectIndex],true);
    }

    void OnDisable()
    {
        DestoryItemObjs();
        RecoverLightMap();
        SetBaseSceneLight();
    }

    public void SyncPageItem(int index0,int index1)
    {
        SceneItemInfo scenePageData0 = m_allItemInfo[index0];//AssetDatabase.LoadAssetAtPath<SceneItemInfo>(m_allAssetPath[index0]);
        for (int i = 0; i < scenePageData0.items.Count; i++)
        {
            for (int j = 0; j < scenePageData0.items[i].itemPos.Count; j++)
            {
                SyncSinglePage(scenePageData0.items[i].itemID, j, scenePageData0.items[i].itemPos[j],index1);
            }
        }
        UserLog.SyncAllPageSceneLog(m_allAssetName[index0],m_allAssetName[index1]);
    }

    private void SyncSinglePage(long id, int index, ItemPosInfo posInfo,int pageIndex)
    {
        SceneItemInfo scenePageData = m_allItemInfo[pageIndex];//AssetDatabase.LoadAssetAtPath<SceneItemInfo>(m_allAssetPath[pageIndex]);
        for (int j = 0; j < scenePageData.items.Count; j++)
        {
            if (scenePageData.items[j].itemID == id)
            {
                if (scenePageData.items[j].itemPos.Count <= index)
                {
                    continue;
                }
                scenePageData.items[j].itemPos[index].pos = posInfo.pos;
                scenePageData.items[j].itemPos[index].rotate = posInfo.rotate;
                scenePageData.items[j].itemPos[index].scale = posInfo.scale;
                scenePageData.items[j].itemPos[index].m_cameraNode = posInfo.m_cameraNode;
                break;
            }
        }
    }

    private void SyncAllItem()
    {
        
        for (int i = 0; i < itemObjs.Count; i++)
        {
            for (int j = 0; j < itemObjs[i].itemObj.Count; j++)
            {
                Transform itemTran = itemObjs[i].itemObj[j].transform;
                //posInfo.pos = itemTran.position;
                //posInfo.rotate = itemTran.eulerAngles;
                int itemIndex = i;//i * itemObjs.Count + j;
                if (itemIndex >= itemInfos.items.Count)
                {
                    Debug.Log("syncAllitem error itemindex " + itemIndex);
                    continue;
                }
                //Debug.Log("id : " + itemInfos.items[itemIndex].itemID + "  index : " + j);
                SysncItem(itemInfos.items[itemIndex].itemID, j, itemTran, itemInfos.items[itemIndex].itemPos[j].m_cameraNode);
            }
        }
        UserLog.SyncAllItemSceneLog();
        
    }
}
