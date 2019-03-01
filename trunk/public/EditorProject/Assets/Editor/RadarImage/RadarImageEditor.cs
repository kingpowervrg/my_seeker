using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UI;
using UnityEngine;

[CustomEditor(typeof(RadarImage))]
public class RadarImageEditor : RawImageEditor
{
    protected SerializedProperty prop0;
    protected SerializedProperty prop1;
    protected SerializedProperty prop2;
    protected SerializedProperty prop3;


    SerializedProperty m_Texture;
    SerializedProperty m_UVRect;
    GUIContent m_UVRectContent;

    protected override void OnEnable()
    {
        base.OnEnable();

        prop0 = serializedObject.FindProperty("prop0");
        prop1 = serializedObject.FindProperty("prop1");
        prop2 = serializedObject.FindProperty("prop2");
        prop3 = serializedObject.FindProperty("prop3");

    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        serializedObject.Update();

        EditorGUILayout.PropertyField(prop0);
        EditorGUILayout.PropertyField(prop1);
        EditorGUILayout.PropertyField(prop2);
        EditorGUILayout.PropertyField(prop3);

        serializedObject.ApplyModifiedProperties();
    }
}

