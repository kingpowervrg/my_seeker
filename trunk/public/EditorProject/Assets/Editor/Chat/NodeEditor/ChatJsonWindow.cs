using UnityEditor;
using UnityEngine;
namespace SimpleNodeEditor
{
    public enum JsonWindow
    {
        Chat,
        Persuade
    }

    public class ChatJsonWindow : EditorWindow
    {
        private static JsonWindow m_type = 0;
        static ChatJsonWindow window = null;
        public static void Open(JsonWindow type = JsonWindow.Chat)
        {
            m_type = type;
            Rect rect = new Rect(0,0,300,500);
            window = (ChatJsonWindow)EditorWindow.GetWindowWithRect(typeof(ChatJsonWindow), rect, true, "对话数据");
            window.Show();
        }

        Vector2 scrollPos = Vector2.zero;
        void OnGUI()
        {
            EditorGUILayout.BeginVertical();
            scrollPos = EditorGUILayout.BeginScrollView(scrollPos);
            if (m_type == JsonWindow.Chat)
            {
                foreach (var kv in ChatSystemManager.Instacne.editorChatGroup.m_chats)
                {
                    if (GUILayout.Button(kv.Value.m_chatID + "\n" + kv.Value.m_describe, GUILayout.Width(200), GUILayout.Height(50)))
                    {
                        NodeEditor.changeNewChat(kv.Value);
                        this.Close();
                    }
                }
            }
            else if (m_type == JsonWindow.Persuade)
            {
                foreach (var kv in PersuadeSystem.Instacne.editorChatGroup.persuadeGroup)
                {
                    if (GUILayout.Button(kv.Value.id + "\n" + kv.Value.m_describe, GUILayout.Width(200), GUILayout.Height(50)))
                    {
                        PersuadeEditor.changeNewChat(kv.Value);
                        this.Close();
                    }
                }
            }
            EditorGUILayout.EndScrollView();
            EditorGUILayout.EndVertical();
        }
    }
}
