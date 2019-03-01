using EngineCore.Editor;
using GOEditor;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

internal class EditorSceneExporter//ScriptableObject
{

    public const string mcSceneSrcDir = "Assets/Res/Scene";

    const string mcInfoDir = "Assets/Res/info";
    const string mcSceneBundleTempDir = "bundle";
    const string mcMeshObjDir = "ExportedObj/Meshes";

    static int tCount;
    static int counter;
    static int totalCount;
    static int progressUpdateInterval = 10000;

    private static bool m_bExportAllScene = true;
    private static bool m_bNewExportMethod = false;

    //各种输出目录//
    private static string m_sceneObjExpDir;
    private static string m_sceneExpDir;
    private static string m_bundleExpDir;
    private static string m_idxExpDir;

    public static void SetDirs(string expObjDir, string expSceneDir, string expBundleDir, string resIdxFileDir)
    {
        m_sceneObjExpDir = expObjDir;
        m_sceneExpDir = expSceneDir;
        m_bundleExpDir = expBundleDir;
        m_idxExpDir = resIdxFileDir;
    }

    public static bool IfExportAllScene
    {
        get { return m_bExportAllScene; }
    }




    static bool ValidateMissingScriptSceneInCurrentScene()
    {
        GameObject[] gos = UnityEngine.Object.FindObjectsOfType<GameObject>();
        bool b = false;
        foreach (GameObject go in gos)
        {
            if (ValidateMissingScript(go))
            {
                b = true;
            }
        }
        return b;
    }

    static bool ValidateMissingScript(GameObject go)
    {
        bool b = false;
        if (null != go)
        {
            Component[] comps = go.GetComponents<Component>();
            foreach (Component comp in comps)
            {
                if (null == comp)
                {
                    Debug.LogError("发现丢失的脚本在: " + go.name);
                    b = true;
                }
            }
        }
        return b;
    }


    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public static string GetSceneNameWithoutExtension()
    {
        return Path.GetFileNameWithoutExtension(EditorSceneManager.GetActiveScene().path);
    }

    public static string GetCurrentSceneNameWithExtension()
    {
        return EditorSceneManager.GetActiveScene().name + ".unity3d";
    }

    /// <summary>
    /// 获取当前打开场景名称
    /// </summary>
    /// <returns></returns>
    public static string GetCurrentSceneName()
    {
        return EditorSceneManager.GetActiveScene().name;
    }

    /// <summary>
    /// 获取当前打开场景路径
    /// </summary>
    /// <returns></returns>
    public static string GetCurrentScenePath()
    {
        return EditorSceneManager.GetActiveScene().path;
    }

    public static GameObject GetGameObjectByName(string name, List<GameObject> golist)
    {
        GameObject goret = null;
        foreach (GameObject go in golist)
        {
            if (go.name == name)
            {
                if (goret)
                    Debug.Log("repeat game object : " + name);
                else
                    goret = go;
            }
        }
        if (!goret)
        {
            string msg = "can not find game object : " + name;
            EditorUtility.DisplayDialog("Error!", msg, "OK");
            Debug.Log(msg);
        }
        return goret;
    }

    public static GameObject GetTerrainGameObject()
    {
        //得到group对象//
        List<GameObject> rootGoLlist = GOEPack.GetRootGoList();
        return GetGameObjectByName(ParseScene.TERRAIN_NAME, rootGoLlist);
    }

    private static Bounds GetGoBound(GameObject go)
    {
        if (go == null)
        {
            Debug.Log("go is null, no bound");
            return new Bounds();
        }

        Bounds bd = new Bounds();
        bool initbd = false;
        foreach (Renderer r in go.GetComponentsInChildren<Renderer>())
        {
            if (initbd)
            {
                bd.Encapsulate(r.bounds);
            }
            else
            {
                bd = r.bounds;
                initbd = true;
            }
        }
        return bd;
    }



    public static Terrain GetActiveTerrainAndValid()
    {
        Terrain terrainObject = Terrain.activeTerrain;
        if (!terrainObject)
        {
            Debug.Log("no activeTerrain.");
            return null;
        }
        else
        {
            return terrainObject;
        }
    }

    private static bool CreateTargetFolder()
    {
        try
        {
            System.IO.Directory.CreateDirectory(PathConfig.NAVIMESH_EXPORTER_ROOT);
        }
        catch
        {
            EditorUtility.DisplayDialog("Error!", "Failed to create target folder!", "");
            return false;
        }

        return true;
    }



    static void UpdateProgress()
    {
        if (counter++ == progressUpdateInterval)
        {
            counter = 0;
            EditorUtility.DisplayProgressBar("Exporting...", "", Mathf.InverseLerp(0, totalCount, ++tCount));
        }
    }


    public static void RemoveAnimator(GameObject go)
    {
        if (go == null)
        {
            return;
        }

        Animator[] comps = go.GetComponentsInChildren<Animator>(true);
        foreach (Animator comp in comps)
        {
            comp.enabled = false;
            UnityEngine.Object.DestroyImmediate(comp);
        }
    }

}