using UnityEngine;
using UnityEngine.UI;

namespace UnityEditor.UI
{
    [CustomEditor(typeof(ToggleEx), true)]
    [CanEditMultipleObjects]
    public class ToggleExEditor : SelectableEditor
    {
        SerializedProperty m_OnValueChangedProperty;
        SerializedProperty m_TransitionProperty;
        SerializedProperty m_GraphicProperty;
        SerializedProperty m_GroupProperty;
        SerializedProperty m_IsOnProperty;
        SerializedProperty m_NodeShow;
        SerializedProperty m_NodeHide;

        protected override void OnEnable()
        {
            base.OnEnable();
            m_TransitionProperty = serializedObject.FindProperty("toggleTransition");
            m_GraphicProperty = serializedObject.FindProperty("graphic");
            m_GroupProperty = serializedObject.FindProperty("m_Group");
            m_IsOnProperty = serializedObject.FindProperty("m_IsOn");
            m_OnValueChangedProperty = serializedObject.FindProperty("onValueChanged");
            m_NodeShow = serializedObject.FindProperty("nodeShow");
            m_NodeHide = serializedObject.FindProperty("nodeHide");
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            EditorGUILayout.Space();
            serializedObject.Update();
            EditorGUILayout.PropertyField(m_IsOnProperty);
            EditorGUILayout.PropertyField(m_TransitionProperty);
            EditorGUILayout.PropertyField(m_GraphicProperty);
            EditorGUILayout.PropertyField(m_GroupProperty);
            EditorGUILayout.PropertyField(m_NodeShow);
            EditorGUILayout.PropertyField(m_NodeHide);
            // Draw the event notification options
            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(m_OnValueChangedProperty);
            serializedObject.ApplyModifiedProperties();
        }
    }
}
