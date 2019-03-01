using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

namespace Resource
{
    public class ResourceLoseWindow : EditorWindow
    {
        Vector2 scrollPos = Vector2.zero;
        List<string> m_assetPaths = new List<string>();
        List<string> m_assetNames = new List<string>();
        public static void CreateWindow(List<string> assetName, List<string> assetPaths,string title)
        {
            Rect rect = new Rect(0, 0, 800, 500);
            ResourceLoseWindow window = (ResourceLoseWindow)EditorWindow.GetWindowWithRect(typeof(ResourceLoseWindow), rect, true, title);
            window.OnInit(assetName,assetPaths);
            window.Show();
        }

        public void OnInit(List<string> assetName,List<string> assetPaths)
        {
            m_assetPaths = assetPaths;
            m_assetNames = assetName;
        }

        void OnGUI()
        {
            scrollPos = EditorGUILayout.BeginScrollView(scrollPos);
            for (int i = 0; i < m_assetPaths.Count; i++)
            {
                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button(m_assetNames[i]))
                {
                    if (m_assetPaths[i].EndsWith(".mat"))
                    {
                        Material obj = AssetDatabase.LoadAssetAtPath(m_assetPaths[i], typeof(Material)) as Material;
                        Selection.activeObject = obj;
                    }
                    else
                    {
                        GameObject obj = AssetDatabase.LoadAssetAtPath(m_assetPaths[i], typeof(GameObject)) as GameObject;
                        Selection.activeGameObject = obj;
                    }
                    //Selection.activeGameObject = obj;
                }
                EditorGUILayout.EndHorizontal();
            }
            EditorGUILayout.EndScrollView();
        }
    }
}
