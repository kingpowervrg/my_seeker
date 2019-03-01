using UnityEngine;
using UnityEditor;

namespace GOGUI
{
    [CustomEditor(typeof(LayoutContentSizeFitter), true)]
    public class LayoutContentSizeFitterEditor : Editor
    {
        SerializedProperty m_HorizontalFit;
        SerializedProperty m_VerticalFit;

        protected virtual void OnEnable()
        {
            m_HorizontalFit = serializedObject.FindProperty("horizontalFit");
            m_VerticalFit = serializedObject.FindProperty("verticalFit");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            EditorGUILayout.PropertyField(m_HorizontalFit, true);
            EditorGUILayout.PropertyField(m_VerticalFit, true);
            serializedObject.ApplyModifiedProperties();
        }
    }
}