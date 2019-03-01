using UnityEditor;
using UnityEngine;

namespace EngineCore.Editor
{
    public class TopMenu : MonoBehaviour
    {
        [MenuItem("EngineEditor/显示编辑器主窗口", false, 1)]
        public static void ShowEditorWindowForRes()
        {
            EditorWindow.GetWindow(typeof(GOEEditorMainWindow), false, "选择编辑器").Show();
        }
    }
}