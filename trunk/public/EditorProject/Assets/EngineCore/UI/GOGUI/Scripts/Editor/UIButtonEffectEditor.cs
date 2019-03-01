
using UnityEditor;
using UnityEngine;
using Editor = UnityEditor.Editor;

namespace EngineCore.Editor
{

    [CustomPropertyDrawer(typeof(ButtonEffectType))]
    public class UIButtonEffectTypeDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            label = EditorGUI.BeginProperty(position, label, property);
            EditorGUI.BeginChangeCheck();
            var newValue = EditorGUI.EnumFlagsField(position, label, (ButtonEffectType)property.intValue);
            if (EditorGUI.EndChangeCheck())
                property.intValue = (int)(ButtonEffectType)newValue;
            EditorGUI.EndProperty();
        }
    }

}