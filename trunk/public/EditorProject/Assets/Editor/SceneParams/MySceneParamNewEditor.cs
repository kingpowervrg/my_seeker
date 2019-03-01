using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(SceneCameraParams_New))]
[ExecuteInEditMode]
public class MySceneParamNewEditor : Editor {

    public static SceneCameraParams_New targetSelf;

    public override void OnInspectorGUI()
    {
        SceneCameraParams_New cameraParams = (SceneCameraParams_New)target;
        targetSelf = cameraParams;
        EditorGUILayout.LabelField("左下角度_0");
        cameraParams.maxAngleX_0 = EditorGUILayout.FloatField("maxAngleX_0", cameraParams.maxAngleX_0);
        cameraParams.minAngleY_0 = EditorGUILayout.FloatField("minAngleY_0", cameraParams.minAngleY_0);

        EditorGUILayout.LabelField("右上角度_0");
        cameraParams.minAngleX_0 = EditorGUILayout.FloatField("minAngleX_0", cameraParams.minAngleX_0);
        cameraParams.maxAngleY_0 = EditorGUILayout.FloatField("maxAngleY_0", cameraParams.maxAngleY_0);
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("--------------");

        EditorGUILayout.LabelField("左下角度_1");
        cameraParams.maxAngleX_1 = EditorGUILayout.FloatField("maxAngleX_1", cameraParams.maxAngleX_1);
        cameraParams.minAngleY_1 = EditorGUILayout.FloatField("minAngleY_1", cameraParams.minAngleY_1);

        EditorGUILayout.LabelField("右上角度_1");
        cameraParams.minAngleX_1 = EditorGUILayout.FloatField("minAngleX_1", cameraParams.minAngleX_1);
        cameraParams.maxAngleY_1 = EditorGUILayout.FloatField("maxAngleY_1", cameraParams.maxAngleY_1);

        EditorGUILayout.LabelField("Z轴");
        cameraParams.ZNear = EditorGUILayout.FloatField("ZNear", cameraParams.ZNear);
    }

    
}
