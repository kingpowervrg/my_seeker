using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class SceneParamHierarchy {

    [MenuItem("GameObject/设置摄像机数据/左下角_0",priority = 0)]
    public static void SetValueToLeftBottm()
    {
        GameObject selectObj = Selection.activeGameObject;
        if (selectObj == null)
        {
            UnityEditor.EditorUtility.DisplayDialog("Title", "先选择对象", "是");
            return;
        }
        GameObject cameraObj = GameObject.FindGameObjectWithTag("MainCamera");
        SceneCameraParams_New cameraParams = cameraObj.GetComponent<SceneCameraParams_New>();
        if (cameraParams == null)
        {
            cameraParams = cameraObj.AddComponent<SceneCameraParams_New>();
        }
        cameraParams.maxAngleX_0 = selectObj.transform.localEulerAngles.x;
        cameraParams.minAngleY_0 = selectObj.transform.localEulerAngles.y;

    }

    [MenuItem("GameObject/设置摄像机数据/右上角_0", priority = 0)]
    public static void SetValueToRightUp()
    {
        GameObject selectObj = Selection.activeGameObject;
        if (selectObj == null)
        {
            UnityEditor.EditorUtility.DisplayDialog("Title", "先选择对象", "是");
            return;
        }
        GameObject cameraObj = GameObject.FindGameObjectWithTag("MainCamera");
        SceneCameraParams_New cameraParams = cameraObj.GetComponent<SceneCameraParams_New>();
        if (cameraParams == null)
        {
            cameraParams = cameraObj.AddComponent<SceneCameraParams_New>();
        }
        cameraParams.minAngleX_0 = selectObj.transform.localEulerAngles.x;
        cameraParams.maxAngleY_0 = selectObj.transform.localEulerAngles.y;
    }

    [MenuItem("GameObject/设置摄像机数据/左下角_1", priority = 0)]
    public static void SetValueToLeftBottm_1()
    {
        GameObject selectObj = Selection.activeGameObject;
        if (selectObj == null)
        {
            UnityEditor.EditorUtility.DisplayDialog("Title", "先选择对象", "是");
            return;
        }
        GameObject cameraObj = GameObject.FindGameObjectWithTag("MainCamera");
        SceneCameraParams_New cameraParams = cameraObj.GetComponent<SceneCameraParams_New>();
        if (cameraParams == null)
        {
            cameraParams = cameraObj.AddComponent<SceneCameraParams_New>();
        }
        cameraParams.maxAngleX_1 = selectObj.transform.localEulerAngles.x;
        cameraParams.minAngleY_1 = selectObj.transform.localEulerAngles.y;

    }

    [MenuItem("GameObject/设置摄像机数据/右上角_1", priority = 0)]
    public static void SetValueToRightUp_1()
    {
        GameObject selectObj = Selection.activeGameObject;
        if (selectObj == null)
        {
            UnityEditor.EditorUtility.DisplayDialog("Title", "先选择对象", "是");
            return;
        }
        GameObject cameraObj = GameObject.FindGameObjectWithTag("MainCamera");
        SceneCameraParams_New cameraParams = cameraObj.GetComponent<SceneCameraParams_New>();
        if (cameraParams == null)
        {
            cameraParams = cameraObj.AddComponent<SceneCameraParams_New>();
        }
        cameraParams.minAngleX_1 = selectObj.transform.localEulerAngles.x;
        cameraParams.maxAngleY_1 = selectObj.transform.localEulerAngles.y;
    }

    [MenuItem("GameObject/设置摄像机数据/拷贝节点数据", priority = 0)]
    public static void SetValueToRightUp_copy()
    {
        GameObject selectObj = Selection.activeGameObject;
        if (selectObj == null)
        {
            UnityEditor.EditorUtility.DisplayDialog("Title", "先选择对象", "是");
            return;
        }
        GameObject cameraObj = GameObject.FindGameObjectWithTag("MainCamera");
        SceneCameraParams_New cameraParams = cameraObj.GetComponent<SceneCameraParams_New>();
        if (cameraParams == null)
        {
            cameraParams = cameraObj.AddComponent<SceneCameraParams_New>();
        }
        cameraParams.minAngleX_1 = cameraParams.minAngleX_0;
        cameraParams.maxAngleY_1 = cameraParams.maxAngleY_0;
        cameraParams.maxAngleX_1 = cameraParams.maxAngleX_0;
        cameraParams.minAngleY_1 = cameraParams.minAngleY_0;

    }

    [MenuItem("GameObject/复位摄像机", priority = 0)]
    public static void ResetMainCamera()
    {
        GameObject mainCamera = GameObject.Find("Main Camera");

        Camera[] allCamera = GameObject.FindObjectsOfType<Camera>();
        for (int i = 0; i < allCamera.Length; i++)
        {
            if (!allCamera[i].name.Equals("Main Camera"))
            {
                allCamera[i].gameObject.SetActive(false);
            }
        }


        GameObject[] objs = GameObject.FindGameObjectsWithTag("MainCamera");
        for (int i = 0; i < objs.Length; i++)
        {
            if (objs[i].name.Equals("Main Camera"))
            {
                continue;
            }
            objs[i].tag = "Untagged";
        }
        if (mainCamera == null)
        {
            UnityEditor.EditorUtility.DisplayDialog("Title", "先打开主摄像机,并设置为MainCamera", "是");
        }
        else
        {
            if (!mainCamera.CompareTag("MainCamera"))
            {
                mainCamera.tag = "MainCamera";
            }
        }
    }

    [MenuItem("GameObject/设置子节点", priority = 0)]
    public static void SetNode()
    {
        GameObject selectObj = Selection.activeGameObject;
        if (selectObj == null)
        {
            UnityEditor.EditorUtility.DisplayDialog("Title", "先选择对象", "是");
            return;
        }
        GameObject mainCamera = GameObject.Find("Main Camera");
        if (mainCamera != null)
        {
            mainCamera.SetActive(false);
        }
        GameObject[] objs = GameObject.FindGameObjectsWithTag("MainCamera");
        for (int i = 0; i < objs.Length; i++)
        {
            if (objs[i].name.Equals("Main Camera"))
            {
                continue;
            }
            objs[i].tag = "Untagged";
        }
        selectObj.tag = "MainCamera";
    }
}
