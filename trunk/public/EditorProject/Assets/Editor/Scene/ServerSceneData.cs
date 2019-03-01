using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

public class ServerSceneData : MonoBehaviour {


    [MenuItem("Tools/CreateServerSceneData")]
    public static void CreateServerSceneData()
    {
        string path = Application.dataPath + "/Res/SceneConfig/";
        SceneDataJson serverJson = new SceneDataJson(); //服务端数据
        serverJson.sceneDatas = new List<SceneItemServerJson>();
        GetData(path,serverJson);
        //服务端数据
        string serverJsonStr = fastJSON.JSON.ToJSON(serverJson);
        SceneLightHelper.CreateServerJson("../config/scenedata.json", serverJsonStr);
    }

    private static void GetData(string path, SceneDataJson serverJson)
    {
        string[] dirs = Directory.GetDirectories(path);
        for (int i = 0; i < dirs.Length; i++)
        {
            GetData(dirs[i], serverJson);
        }
        string[] files = Directory.GetFiles(path);
        for (int i = 0; i < files.Length; i++)
        {
            if (!files[i].Contains(".json") || files[i].Contains("meta"))
            {
                continue;
            }
            FileInfo fileinfo = new FileInfo(files[i]);
            StreamReader reader = new StreamReader(files[i]);
            string jsonStr = reader.ReadToEnd();
            reader.Close();
            SceneItemJson groupData = fastJSON.JSON.ToObject<SceneItemJson>(jsonStr);
            string fileName = fileinfo.Name.Replace(".json", "");
            long groupID = -1;
            long.TryParse(fileName, out groupID);
            if (groupID > 0)
            {
                SceneItemServerJson itemServerJson = new SceneItemServerJson(groupData);
                itemServerJson.groupId = groupID;
                serverJson.sceneDatas.Add(itemServerJson);
            }
        }
    }

    [MenuItem("Tools/设置物件默认概率为100")]
    public static void SetSceneItemPercent()
    {
        string path = Application.dataPath + "/Res/SceneConfig/";
        //SceneDataJson serverJson = new SceneDataJson(); //服务端数据
        //serverJson.sceneDatas = new List<SceneItemServerJson>();
        SetSceneData(path);
        //服务端数据
        //string serverJsonStr = fastJSON.JSON.ToJSON(serverJson);
        //SceneLightHelper.CreateServerJson("../config/scenedata.json", serverJsonStr);
    }

    private static void SetSceneData(string path)
    {
        string[] dirs = Directory.GetDirectories(path);
        for (int i = 0; i < dirs.Length; i++)
        {
            SetSceneData(dirs[i]);
        }
        string[] files = Directory.GetFiles(path);
        for (int i = 0; i < files.Length; i++)
        {
            if (!files[i].Contains(".json") || files[i].Contains("meta"))
            {
                continue;
            }
            //FileInfo fileinfo = new FileInfo(files[i]);
            StreamReader reader = new StreamReader(files[i]);
            string jsonStr = reader.ReadToEnd();
            reader.Close();
            SceneItemJson groupData = fastJSON.JSON.ToObject<SceneItemJson>(jsonStr);
            for (int j = 0; j < groupData.items.Count; j++)
            {
                groupData.items[j].itemPercent = 100;
            }
            string groupDataJson = fastJSON.JSON.ToJSON(groupData);
            SceneLightHelper.CreateJson(files[i], groupDataJson);

            //string fileName = fileinfo.Name.Replace(".json", "");
            //long groupID = -1;
            //long.TryParse(fileName, out groupID);
            //if (groupID > 0)
            //{
            //    SceneItemServerJson itemServerJson = new SceneItemServerJson(groupData);
            //    itemServerJson.groupId = groupID;
            //    serverJson.sceneDatas.Add(itemServerJson);
            //}
        }
    }

}
