using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

public class LightMapDataCheck : EditorWindow {

    private string SceneConfig = null;
    [MenuItem("Tools/Lighting/检测光照贴图")]
    public static void CreateWindow()
    {
        Rect rect = new Rect(0, 0, 800, 500);
        LightMapDataCheck window = (LightMapDataCheck)EditorWindow.GetWindowWithRect(typeof(LightMapDataCheck), rect, true, "光照检测");
        window.OnInit();
        window.Show();
    }

    private void OnInit()
    {
        this.SceneConfig = Application.dataPath + "/Res/SceneConfig";
        
    }
    List<LightMapMissData> missLightmap = new List<LightMapMissData>();
    private void OnCheck()
    {
        missLightmap.Clear();
        string[] dirtorys = Directory.GetDirectories(this.SceneConfig);
        for (int i = 0; i < dirtorys.Length; i++)
        {
            int dirLast = dirtorys[i].LastIndexOf("\\");
            string DirName = dirtorys[i].Substring(dirLast + 1, dirtorys[i].Length - dirLast - 1);
            string[] files = Directory.GetFiles(dirtorys[i]);

            for (int j = 0; j < files.Length; j++)
            {
                if (files[j].Contains(".json") && !files[j].Contains(".meta"))
                {
                    StreamReader reader = new StreamReader(files[j]);
                    string fileContent = reader.ReadToEnd();
                    reader.Close();
                    SceneItemJson itemjson = fastJSON.JSON.ToObject<SceneItemJson>(fileContent);
                    if (string.IsNullOrEmpty(itemjson.lightMapName))
                    {
                        LightMapMissData missData = new LightMapMissData();
                        missData.lightConfigPath = files[j];
                        int jsonLast = files[j].LastIndexOf("\\");
                        string jsonName = files[j].Substring(jsonLast + 1, files[j].Length - jsonLast - 1);
                        missData.groupName = jsonName;
                        if (File.Exists(this.SceneConfig + "/" + DirName + "/" + DirName + ".exr"))
                        {
                            missData.lightMapPath = DirName + ".exr";
                        }
                        else
                        {
                            missData.lightMapPath = string.Empty;
                        }
                        missData.sceneName = DirName;
                        missLightmap.Add(missData);
                    }
                }
            }
        }
    }
    Vector2 scrollPos = Vector2.zero;
    void OnGUI()
    {
        if (GUILayout.Button("Check"))
        {
            OnCheck();
        }
        EditorGUILayout.LabelField("======================================================================");
        scrollPos = EditorGUILayout.BeginScrollView(scrollPos);
        for (int i = 0; i < missLightmap.Count; i++)
        {
            string lightmap = missLightmap[i].lightMapPath;
            if (string.IsNullOrEmpty(missLightmap[i].lightMapPath))
            {
                lightmap = "miss lightMap";
            }
            if (GUILayout.Button(lightmap + " --  " + missLightmap[i].sceneName + "  --  " + missLightmap[i].groupName))
            {
                StreamReader reader = new StreamReader(missLightmap[i].lightConfigPath);
                string fileContent = reader.ReadToEnd();
                reader.Close();
                SceneItemJson itemjson = fastJSON.JSON.ToObject<SceneItemJson>(fileContent);
                itemjson.lightMapName = missLightmap[i].lightMapPath;
                string jsonContent = fastJSON.JSON.ToJSON(itemjson);
                SceneLightHelper.CreateJson(missLightmap[i].lightConfigPath, jsonContent);
            }
        }
        EditorGUILayout.EndScrollView();
    }

    public class LightMapMissData
    {
        public string lightConfigPath;
        public string lightMapPath;
        public string sceneName;
        public string groupName;
    }
}

