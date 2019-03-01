using UnityEngine;
using UnityEditor;
using System.Collections;

static public class PrefabExtendTools
{

    [MenuItem("CONTEXT/Transform/SavePrefab")]
    static public void SavePrefab()
    {
        //GameObject source = PrefabUtility.GetPrefabParent(Selection.activeGameObject) as GameObject;
        GameObject source = PrefabUtility.GetPrefabObject(Selection.activeGameObject) as GameObject;

        if (source == null)
        {
            return;
        }
        string prefabPath = AssetDatabase.GetAssetPath(source).ToLower();
        if (prefabPath.EndsWith(".prefab") == false)
        {
            Debug.Log(Application.dataPath);
            //PrefabUtility.CreatePrefab()
            return;
        }
        PrefabUtility.ReplacePrefab(Selection.activeGameObject, source, ReplacePrefabOptions.ConnectToPrefab | ReplacePrefabOptions.ReplaceNameBased);
        
    }
}