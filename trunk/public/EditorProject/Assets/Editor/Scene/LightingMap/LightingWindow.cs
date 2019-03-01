using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.SceneManagement;

public class LightingWindow : EditorWindow {

    static LightingWindow window;
    private LightSystem lightSystem;
    private LightRecoverSystem recoverSystem;

    [MenuItem("Tools/Lighting/光照")]
    static void CreateWindow()
    {
        Rect rect = new Rect(0, 0, 500, 300);
        window = (LightingWindow)EditorWindow.GetWindowWithRect(typeof(LightingWindow), rect, true, "光照");
        if (window.Init())
        {
            window.Show();
        }
    }

    public bool Init()
    {
        string sceneConfigPath = Application.dataPath + "/" + LightingData.constConfigPath;
        string scenePath = Application.dataPath + "/Res/Scene/";
        Scene curScene = SceneManager.GetActiveScene();
        if (curScene == null)
        {
            window.Close();
            window = null;
            UnityEditor.EditorUtility.DisplayDialog("Title", "请先打开场景", "是");
            return false;
        }
        m_sceneName = curScene.name;
        lightSystem = new LightSystem(new LightingData(curScene.name,sceneConfigPath, SceneTools.getConfigPath(), scenePath));
        recoverSystem = new LightRecoverSystem(new LightingData(curScene.name, sceneConfigPath, SceneTools.getConfigPath(), scenePath));
        lightSystem.OnInit();
        Lightmapping.completed = lightSystem.BakeOver;
        return true;
    }
    string m_sceneName;
    void OnGUI()
    {

        EditorGUILayout.BeginVertical();
        EditorGUILayout.TextField("场景名称:", m_sceneName);
        if (GUILayout.Button("加载所有物件"))
        {
            lightSystem.LoadModel();
        }
        if (GUILayout.Button("烘焙"))
        {
            lightSystem.BakeScene();
        }
        if (GUILayout.Button("保存数据"))
        {
            lightSystem.SaveLightMap();
        }
        if (GUILayout.Button("加载光照"))
        {
            recoverSystem.SetLightMap();
        }
        EditorGUILayout.EndVertical();
    }

    void OnDisable()
    {
        lightSystem.RecoverSceneLightMap();
        lightSystem.ClearGameObject();
        recoverSystem.ClearGameObject();
    }
}
