using UnityEngine;
using UnityEditor;

namespace GOGUI
{
    [CustomEditor(typeof(AdvancedText), true)]
    public class AdvancedTextEditor : UnityEditor.UI.GraphicEditor
    {
        bool showSyms = true;
        string newName = "";
        Sprite newSprite;
        SerializedProperty m_Text;
        SerializedProperty m_ImageFont;
        SerializedProperty m_IsPureEmoji;
        SerializedProperty m_ImageSize;
        SerializedProperty m_FontData;

        protected override void OnEnable()
        {
            base.OnEnable();

            m_Text = serializedObject.FindProperty("originalText");
            m_ImageSize = serializedObject.FindProperty("imageSize");
            m_ImageFont = serializedObject.FindProperty("m_ImageFont");
            m_FontData = serializedObject.FindProperty("m_FontData");
            m_IsPureEmoji = serializedObject.FindProperty("isPureEmoji");
        }
        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            AdvancedText img = target as AdvancedText;
            EditorGUILayout.PropertyField(m_Text);
            EditorGUILayout.PropertyField(m_ImageSize);
            EditorGUILayout.PropertyField(m_IsPureEmoji);
            img.RayCastTarget = EditorGUILayout.Toggle("Raycast Target", img.RayCastTarget);
            EditorGUILayout.PropertyField(m_ImageFont);
            EditorGUILayout.PropertyField(m_FontData);
            AppearanceControlsGUI();
            serializedObject.ApplyModifiedProperties();
        }
    }
}