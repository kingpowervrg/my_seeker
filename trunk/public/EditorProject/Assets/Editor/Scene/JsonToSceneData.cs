using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using UnityEngine.SceneManagement;

public class JsonToSceneData : MonoBehaviour {

    [MenuItem("Tools/JsonToAsssets")]
    public static void JsonToAssets()
    {
        UnityEngine.Object[] objs = Selection.GetFiltered(typeof(UnityEngine.Object),SelectionMode.DeepAssets);
        for (int i = 0; i < objs.Length; i++)
        {
            string objPath = AssetDatabase.GetAssetPath(objs[i]);
            if (objPath.Contains(".json"))
            {
                int subIndex = objPath.IndexOf('/');
                string objrealPath = objPath.Substring(subIndex);
                string jsonFullPath = Application.dataPath + objrealPath;
                SetAssetData(jsonFullPath);
                //Debug.Log(jsonFullPath);
            }            
        }
    }

    static void SetAssetData(string jsonFullPath)
    {
        StreamReader reader = new StreamReader(jsonFullPath);
        string jsonStr = reader.ReadToEnd();
        reader.Close();
        SceneItemJson groupData = fastJSON.JSON.ToObject<SceneItemJson>(jsonStr);
        SceneItemInfo itemInfos = ScriptableObject.CreateInstance<SceneItemInfo>();
        setAssetData(groupData, itemInfos);

        FileInfo fileInfo = new FileInfo(jsonFullPath);
        Scene scene = SceneManager.GetActiveScene();
        if (scene == null)
        {
            return;
        }
        string sceneName = scene.name;//fileInfo.Name.Substring(0, index);

        int dotIndex = fileInfo.Name.LastIndexOf('.');
        string fileName = fileInfo.Name.Substring(0, dotIndex);
        int nameIndex = -1;
        int.TryParse(fileName, out nameIndex);
        if (nameIndex == -1)
        {
            return;
        }
        nameIndex = nameIndex % 100;
        //Debug.Log(sceneName);
        if (!Directory.Exists(Application.dataPath + "/Res/SceneData/" + sceneName))
        {
            Directory.CreateDirectory(Application.dataPath + "/Res/SceneData/" + sceneName);
        }
        AssetDatabase.CreateAsset(itemInfos, "Assets/Res/SceneData/" + sceneName + "/" + sceneName + "_" + nameIndex + ".asset");
        AssetDatabase.Refresh();
    }

    static SceneItemInfo SetAssetDataJson(string jsonFullPath)
    {
        StreamReader reader = new StreamReader(jsonFullPath);
        string jsonStr = reader.ReadToEnd();
        reader.Close();
        SceneItemJson groupData = fastJSON.JSON.ToObject<SceneItemJson>(jsonStr);
        SceneItemInfo itemInfos = new SceneItemInfo();
        setAssetData(groupData, itemInfos);
        return itemInfos;
    }

    public static List<SceneItemInfo> RevertAsset()
    {
        Scene scene = SceneManager.GetActiveScene();
        if (scene == null)
        {
            return null;
        }
        string sceneName = scene.name;
        string rootPath = Application.dataPath + "/Res/SceneConfig/" + sceneName;
        if (!Directory.Exists(rootPath))
        {
            Directory.CreateDirectory(rootPath);
        }
        List<SceneItemInfo> itemInfos = new List<SceneItemInfo>();
        string[] files = Directory.GetFiles(rootPath);
        for (int i = 0; i < files.Length; i++)
        {
            FileInfo fileInfo = new FileInfo(files[i]);
            if (fileInfo.Name.Contains(".json") && !fileInfo.Name.Contains(".meta"))
            {
                SceneItemInfo info = SetAssetDataJson(files[i]);
                info.jsonName = fileInfo.Name;
                itemInfos.Add(info);
            } 
        }
        return itemInfos;
    }

    static void setAssetData(SceneItemJson groupData, SceneItemInfo itemInfos)
    {
        itemInfos.lightMapName = groupData.lightMapName;
        itemInfos.items = new List<ItemInfo>();
        for (int i = 0; i < groupData.items.Count; i++)
        {
            ItemInfoJson infoJson = groupData.items[i];
            ItemInfo itemInfo = new ItemInfo();
            itemInfo.itemID = infoJson.itemID;
            itemInfo.itemPercent = infoJson.itemPercent;
            itemInfo.itemName = infoJson.itemName;
            itemInfo.itemPos = new List<ItemPosInfo>();
            itemInfos.items.Add(itemInfo);
            for (int j = 0; j < infoJson.itemPos.Count; j++)
            {
                ItemPosInfo posInfo = new ItemPosInfo();
                posInfo.lightIndex = infoJson.itemPos[j].lightIndex;
                posInfo.offsetX = infoJson.itemPos[j].offsetX;
                posInfo.offsetY = infoJson.itemPos[j].offsetY;
                posInfo.percent = infoJson.itemPos[j].percent;
                posInfo.pos = new Vector3(infoJson.itemPos[j].pos.x, infoJson.itemPos[j].pos.y, infoJson.itemPos[j].pos.z);
                posInfo.rotate = new Vector3(infoJson.itemPos[j].rotate.x, infoJson.itemPos[j].rotate.y, infoJson.itemPos[j].rotate.z);
                posInfo.scale = new Vector3(infoJson.itemPos[j].scale.x, infoJson.itemPos[j].scale.y, infoJson.itemPos[j].scale.z);
                posInfo.m_cameraNode = infoJson.itemPos[j].cameraNode;
                posInfo.tilingX = infoJson.itemPos[j].tilingX;
                posInfo.tilingY = infoJson.itemPos[j].tilingY;
                posInfo.type = (ItemType)infoJson.itemPos[j].type;
                itemInfo.itemPos.Add(posInfo);
            }
        }
    }

    [MenuItem("Tools/Lighting/恢复光照依赖")]
    public static void LightMapRecover()
    {
        //string dirPath = Application.dataPath + "/Res/SceneConfig";
        Object[] objs = Selection.GetFiltered(typeof(Object), SelectionMode.Assets);
        for (int i = 0; i < objs.Length; i++)
        {
            string objName = objs[i].name;
            
            string dirFullPath = Application.dataPath.Substring(0, Application.dataPath.LastIndexOf('/')) + "/" + AssetDatabase.GetAssetPath(objs[i]);
            string[] filePaths = Directory.GetFiles(dirFullPath);
            if (!File.Exists(dirFullPath + "/" + objName + ".exr"))
            {
                continue;
            }
            Debug.Log("name : " + objName);
            for (int j = 0; j < filePaths.Length; j++)
            {
                if (filePaths[j].Contains(".meta") || filePaths[j].Contains(".exr"))
                {
                    continue;
                }
                Debug.Log(filePaths[j]);
                StreamReader reader = new StreamReader(filePaths[j]);
                string fileContent = reader.ReadToEnd();
                reader.Close();
                SceneItemJson itemjson = fastJSON.JSON.ToObject<SceneItemJson>(fileContent);
                itemjson.lightMapName = objName + ".exr";
                string jsonContent = fastJSON.JSON.ToJSON(itemjson);
                SceneLightHelper.CreateJson(filePaths[j], jsonContent);
            }
        }
    }
}
