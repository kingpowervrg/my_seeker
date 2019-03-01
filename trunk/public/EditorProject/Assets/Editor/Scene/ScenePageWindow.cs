using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.SceneManagement;
using System.IO;
using conf;

public class ScenePageWindow : EditorWindow
{
    List<string> m_allAssetName = new List<string>(); //所有本地数据名称
    static ScenePageWindow window;
    public static ScenePageWindow CreateWindow()
    {
        Rect rect = new Rect(0, 0, 500, 200);
        window = (ScenePageWindow)EditorWindow.GetWindowWithRect(typeof(ScenePageWindow), rect, true, "页签");
        
        return window;
    }

    public void SetAllAssetName(List<string> allAssetName)
    {
        this.m_allAssetName = allAssetName;
    }

    int sceneDataIndex0 = 0;
    int sceneDataIndex1 = 0;
    void OnGUI()
    {
        //EditorGUILayout.BeginVertical();
        EditorGUILayout.BeginHorizontal();

        sceneDataIndex0 = EditorGUILayout.Popup(sceneDataIndex0, m_allAssetName.ToArray());
        sceneDataIndex1 = EditorGUILayout.Popup(sceneDataIndex1, m_allAssetName.ToArray());
        if (GUILayout.Button("替换"))
        {
            SceneWindow.window.SyncPageItem(sceneDataIndex0, sceneDataIndex1);
        }
        EditorGUILayout.EndHorizontal();
        //EditorGUILayout.EndVertical();
    }
}
