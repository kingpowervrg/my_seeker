using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using conf;
using UnityEngine.SceneManagement;

public class SceneDataCheck : EditorWindow {

    List<BaseItem> baseItemsData = new List<BaseItem>();
    string dataDir;
    SceneObjLightData objLightData; //光照数据
    SceneItemJson sceneItem; //场景数据

    //[MenuItem("Tools/查看场景数据")]
    static void CreateWindow()
    {
        Rect rect = new Rect(0, 0, 400, 400);
        SceneDataCheck window = (SceneDataCheck)EditorWindow.GetWindowWithRect(typeof(SceneDataCheck), rect, true, "查看场景数据");
        window.Init();
        window.Show();
    }
    string sceneName;
    void Init()
    {
        string configPath = SceneTools.getConfigPath();
        System.Reflection.PropertyInfo[] proInfo;
        ConfHelper.LoadFromExcel<BaseItem>(configPath + "BaseItem.xlsx", 0, out baseItemsData, out proInfo);
        Scene scene = SceneManager.GetActiveScene();
        if (scene == null)
        {
            return;
        }
        sceneName = scene.name;
        dataDir = Application.streamingAssetsPath + "/SceneData/" + sceneName + "/";
        string lightJsonStr = SceneTools.getJsonFile(dataDir + "lightData/" + sceneName + "_0_light.json");
        //object ooo = fastJSON.JSON.Parse(lightJsonStr);
        //objLightData = (SceneObjLightData)fastJSON.JSON.Parse(lightJsonStr);
        objLightData = fastJSON.JSON.ToObject<SceneObjLightData>(lightJsonStr);
        string itemJsonStr = SceneTools.getJsonFile(dataDir + sceneName + "_0.json");
        sceneItem = fastJSON.JSON.ToObject<SceneItemJson>(itemJsonStr);

        //sceneItem = (SceneItemJson)fastJSON.JSON.ToObject(itemJsonStr);
        for (int i = 0; i < sceneItem.items.Count; i++)
        {
            itemState.Add(false);
        }
        tex = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/Res/Scene/" + sceneName + "/Lightmap-0_comp_light.exr");

    }

    void OnGUI()
    {
        EditorGUILayout.BeginVertical();
        if (GUILayout.Button("确认"))
        {
            Check();
        }
        CreateDataItem();
        EditorGUILayout.EndVertical();
    }
    List<bool> itemState = new List<bool>();
    void CreateDataItem()
    {
        if (sceneItem == null)
        {
            return;
        }
        for (int i = 0; i < sceneItem.items.Count; i++)
        {
            ItemInfoJson itemInfo = sceneItem.items[i];
            EditorGUILayout.BeginHorizontal();
            itemState[i] = EditorGUILayout.Toggle(itemInfo.itemID + "/" + itemInfo.itemName + "/" + itemInfo.itemPos.Count, itemState[i]);
            EditorGUILayout.EndHorizontal();
        }
    }
    Texture2D tex;
    void Check()
    {
        if (tex == null)
        {
            return;
        }
        for (int i = 0; i < sceneItem.items.Count; i++)
        {
            if (itemState[i])
            {
                ItemInfoJson itemInfo = sceneItem.items[i];
                BaseItem baseItem = getBaseItemByID(itemInfo.itemID);
                for (int j = 0; j < itemInfo.itemPos.Count; j++)
                {
                    ItemPosInfoJson posInfo = itemInfo.itemPos[j];
                    Object obj = AssetDatabase.LoadAssetAtPath("Assets/" + baseItem.model + ".prefab", typeof(Object));
                    GameObject gameObj = Instantiate(obj) as GameObject;
                    gameObj.transform.position = new Vector3(posInfo.pos.x,posInfo.pos.y,posInfo.pos.z);
                    gameObj.transform.eulerAngles = new Vector3(posInfo.rotate.x,posInfo.rotate.y,posInfo.rotate.z);
                    gameObj.name = itemInfo.itemName;

                    ObjLightMapData mapdata = objLightData.groups[i].objLightDatas[j];
                    MeshRenderer render = gameObj.GetComponent<MeshRenderer>();
                    render.lightmapIndex = (int)mapdata.lightIndex;
                    render.lightmapScaleOffset = new Vector4(mapdata.tilingX, mapdata.tilingY, mapdata.offsetX, mapdata.offsetY);
                    //MColor[] mc = mapdata.color;
                    //creatNewLightMap(mapdata,tex);
                }
            }
        }
    }

    void creatNewLightMap(ObjLightMapData mapdata,Texture2D tex)
    {
        int lightWidth = tex.width;
        int lightHeigh = tex.height;
        int startX = (int)(Mathf.Abs(mapdata.offsetX) * lightWidth);
        int startY = (int)(Mathf.Abs(mapdata.offsetY) * lightHeigh);
        int scaleX = (int)(mapdata.tilingX * lightWidth);
        int scaleY = (int)(mapdata.tilingY * lightHeigh);
        for (int i = startX; i < startX + scaleX; i++)
        {
            for (int j = startY; j < startY + scaleY; j++)
            {
                int index = (i - startX) * scaleY + (j - startY);
                MColor mc = mapdata.color[index];
                tex.SetPixel(i, j, new Color(mc.r,mc.g,mc.b));
            }
        }
        tex.Apply();
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
}
