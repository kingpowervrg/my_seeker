using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System.IO;
using conf;
namespace Resource
{
    public class ResourceExhibitWindow : EditorWindow
    {
        private SceneDataJson sceneData;
        List<SceneConfig> sceneConfig;
        public static void CreateWindow(string title)
        {
            Rect rect = new Rect(0, 0, 800, 500);
            ResourceExhibitWindow window = (ResourceExhibitWindow)EditorWindow.GetWindowWithRect(typeof(ResourceExhibitWindow), rect, true, title);
            window.OnInit();
            window.Show();
        }

        public void OnInit()
        {
            LoadSceneData();
            LoadResSceneData();
        }
        string exhibitIDStr;
        Vector2 scrollPos;
        void OnGUI()
        {
            exhibitIDStr = EditorGUILayout.TextField("物件ID:", exhibitIDStr);
            if (GUILayout.Button("查找"))
            {
                GetExhibit();
            }

            scrollPos = EditorGUILayout.BeginScrollView(scrollPos);
            for (int i = 0; i < m_scene.Count; i++)
            {
                EditorGUILayout.BeginHorizontal();
                GUILayout.Button(m_scene[i]);
                EditorGUILayout.EndHorizontal();
            }
            EditorGUILayout.EndScrollView();
        }

        private void LoadSceneData()
        {
            if (sceneData != null)
            {
                return;
            }
            StreamReader reader = new StreamReader(ResourceData.sceneDataConfigPath);
            string content = reader.ReadToEnd();
            reader.Close();
            sceneData = fastJSON.JSON.ToObject<SceneDataJson>(content);
        }

        private void LoadResSceneData()
        {
            string configPath = SceneTools.getConfigPath();
            System.Reflection.PropertyInfo[] sceneProInfo;
            ConfHelper.LoadFromExcel<SceneConfig>(configPath + "sceneResEditor.xlsx", 0, out sceneConfig, out sceneProInfo);
        }

        private List<string> m_scene = new List<string>();
        private void GetExhibit()
        {
            m_scene.Clear();
            long exhibitID = long.Parse(exhibitIDStr);
            for (int i = 0; i < sceneData.sceneDatas.Count; i++)
            {
                SceneItemServerJson itemDatas = sceneData.sceneDatas[i];
                for (int j = 0; j < itemDatas.items.Count; j++)
                {
                    if (itemDatas.items[j].itemID == exhibitID)
                    {
                        GetSceneForGroupID(itemDatas.groupId);
                    }
                }
            }
        }

        private void GetSceneForGroupID(long groupID)
        {
            long sceneID = (long)Mathf.Floor(groupID / 100);
            for (int i = 0; i < sceneConfig.Count; i++)
            {
                if (sceneID == sceneConfig[i].id)
                {
                    m_scene.Add(sceneConfig[i].sceneInfo + "  ==  " + groupID);
                }
            }
        }
    }
}
