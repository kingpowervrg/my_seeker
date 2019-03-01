/********************************************************************
	created:  2018-12-12 20:16:28
	filename: InteractiveGraphicEditor.cs
	author:	  songguangze@outlook.com
	
	purpose:  可交互Panel,可响应事件Editor
*********************************************************************/
using UnityEditor;
using UnityEditor.UI;
using UnityEngine;

namespace GOGUI
{
    [CanEditMultipleObjects, CustomEditor(typeof(InteractiveGraphic), false)]
    public class InteractiveGraphicEditor : GraphicEditor
    {
        public override void OnInspectorGUI()
        {
            base.serializedObject.Update();
            EditorGUILayout.PropertyField(base.m_Script, new GUILayoutOption[0]);
            base.RaycastControlsGUI();
            base.serializedObject.ApplyModifiedProperties();
        }
    }
}