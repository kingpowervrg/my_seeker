using UnityEngine;
using UnityEditor;
using System.IO;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class SceneLightMappingTrans : EditorWindow
{
    private  string rootPath;

    [MenuItem("Tools/转移光照信息")]
    public static void LightMapingTrans()
    {
        Rect rect = new Rect(0, 0, 500, 300);
        SceneLightMappingTrans window = (SceneLightMappingTrans)EditorWindow.GetWindowWithRect(typeof(SceneLightMappingTrans), rect, true, "转移光照信息");
        window.Init();
        window.Show();
    }

    public void Init()
    {
        rootPath = Application.dataPath + "/Res/SceneConfig/";
    }

    string srcPath = string.Empty;
    string destPath = string.Empty;

    string srcNumber = string.Empty;
    string destNumber = string.Empty;
    void OnGUI()
    {

        srcPath = EditorGUILayout.TextField("源json：",srcPath);
        destPath = EditorGUILayout.TextField("目标json：", destPath);
        if (GUILayout.Button("转移"))
        {
            TransLightMap();
        }

        if (GUILayout.Button("单独提取信息"))
        {
            PickUpLightMap();
        }

        if (GUILayout.Button("获取物体数量"))
        {
            GetObjNumber();
        }
        srcNumber = EditorGUILayout.TextField("Src:",srcNumber);
        destNumber = EditorGUILayout.TextField("Dest:", destNumber);
    }

    private void TransLightMap()
    {
        string srcJsonStr = GetJsonString(rootPath + srcPath);
        SceneItemJson groupData = fastJSON.JSON.ToObject<SceneItemJson>(srcJsonStr);

        string destJsonStr = GetJsonString(rootPath + destPath);
        SceneItemJson destGroupData = fastJSON.JSON.ToObject<SceneItemJson>(destJsonStr);
        CopyToDestLightMap(groupData, destGroupData);

        string newJson = fastJSON.JSON.ToJSON(destGroupData);
        SceneLightHelper.CreateJson(rootPath + destPath, newJson);

    }

    private void PickUpLightMap()
    {
        string srcJsonStr = GetJsonString(rootPath + srcPath);
        SceneItemJson groupData = fastJSON.JSON.ToObject<SceneItemJson>(srcJsonStr);
        CopyLightMap(groupData);
    }

    private string GetJsonString(string jsonFullPath)
    {
        StreamReader reader = new StreamReader(jsonFullPath);
        string jsonStr = reader.ReadToEnd();
        reader.Close();
        return jsonStr;
    }

    private string GetLightName(string srcName)
    {
        string[] singleNames = srcName.Split('.');
        if (singleNames.Length == 2)
        {
            return singleNames[0] + "_Single.json";
        }
        return "-1.json";
    }


    private void CopyLightMap(SceneItemJson itemJson)
    {
        TotalLightMapData destJson = new TotalLightMapData();
        destJson.SetData(itemJson);
        string jsonStr = fastJSON.JSON.ToJSON(destJson);
        string newSingleName = GetLightName(srcPath);
        SceneLightHelper.CreateJson(rootPath + newSingleName, jsonStr);
    }

    private void CopyToDestLightMap(SceneItemJson srcItemJson, SceneItemJson destItemJson)
    {
        destItemJson.lightMapName = srcItemJson.lightMapName;
        for (int i = 0; i < srcItemJson.items.Count; i++)
        {
            for (int j = 0; j < destItemJson.items.Count; j++)
            {
                if (srcItemJson.items[i].itemID == destItemJson.items[j].itemID)
                {
                    for (int k = 0; k < destItemJson.items[j].itemPos.Count; k++)
                    {
                        if (srcItemJson.items.Count > k)
                        {
                            ItemPosInfoJson srcJson = srcItemJson.items[i].itemPos[k];
                            ItemPosInfoJson destJson = destItemJson.items[i].itemPos[k];
                            destJson.lightIndex = srcJson.lightIndex;
                            destJson.offsetX = srcJson.offsetX;
                            destJson.offsetY = srcJson.offsetY;
                            destJson.tilingX = srcJson.tilingX;
                            destJson.tilingY = srcJson.tilingY;
                        }
                    }
                    break;
                }
            }
        }
    }



    private void GetObjNumber()
    {
        string srcJsonStr = GetJsonString(rootPath + srcPath);
        SceneItemJson groupData = fastJSON.JSON.ToObject<SceneItemJson>(srcJsonStr);

        string destJsonStr = GetJsonString(rootPath + destPath);
        SceneItemJson destGroupData = fastJSON.JSON.ToObject<SceneItemJson>(destJsonStr);

        srcNumber = GetObjNumber(groupData).ToString();
        destNumber = GetObjNumber(destGroupData).ToString();
    }

    private int GetObjNumber(SceneItemJson groupData)
    {
        int count = 0;
        for (int i = 0; i < groupData.items.Count; i++)
        {
            for (int j = 0; j < groupData.items[i].itemPos.Count; j++)
            {
                count++;
            }
        }
        return count;
    }
}

public class TotalLightMapData
{
    public string m_lightMapName { get; set; }
    public List<LightMapData> m_data { get; set; }

    public void SetData(SceneItemJson json)
    {
        m_lightMapName = json.lightMapName;
        m_data = new List<LightMapData>();
        for (int i = 0; i < json.items.Count; i++)
        {
            LightMapData mapdata = new LightMapData();
            mapdata.m_data = new List<SingleLightMapData>();

            ItemInfoJson itemjson = json.items[i];
            mapdata.itemID = itemjson.itemID;
            mapdata.itemName = itemjson.itemName;
            for (int j = 0; j < itemjson.itemPos.Count; j++)
            {
                SingleLightMapData singleData = new SingleLightMapData();
                ItemPosInfoJson posJson = itemjson.itemPos[j];
                singleData.lightIndex = posJson.lightIndex;
                singleData.offsetX = posJson.offsetX;
                singleData.offsetY = posJson.offsetY;
                singleData.tilingX = posJson.tilingX;
                singleData.tilingY = posJson.tilingY;
                mapdata.m_data.Add(singleData);
            }
            m_data.Add(mapdata);
        }
    }
}


public class LightMapData
{
    public long itemID { get; set; }
    public string itemName { get; set; }
    public List<SingleLightMapData> m_data { get; set; }
}

public class SingleLightMapData
{
    public int lightIndex { get; set; }
    public float tilingX { get; set; }
    public float tilingY { get; set; }
    public float offsetX { get; set; }
    public float offsetY { get; set; }
}