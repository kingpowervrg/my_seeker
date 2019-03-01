using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

namespace Resource
{
    public enum ResourceModelType
    {
        None,
        UV2,
        RW,
        BlendShape
    }

    public class ResourceModelWindow : EditorWindow
    {
        public ResourceModelSystem m_modelSystem;

        public static void CreateWindow()
        {
            Rect rect = new Rect(0, 0, 800, 500);
            ResourceModelWindow window = (ResourceModelWindow)EditorWindow.GetWindowWithRect(typeof(ResourceModelWindow), rect, true, "模型资源");
            window.OnInit();
            window.Show();
        }

        public void OnInit()
        {
            m_modelSystem = new ResourceModelSystem();
            m_modelSystem.LoadModel();
        }
        Vector2 scrollPos = Vector2.zero;
        private List<string> m_showItems = new List<string>();
        private bool[] m_showToggle = new bool[0];
        private bool m_chooseAll = false;
        private bool m_chooseAllLast = false;
        private string m_ChangeName = string.Empty;
        ResourceModelType modelType = ResourceModelType.None;
        private bool isChange = false;
        void OnGUI()
        {
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("检查UV2  数量:" + m_modelSystem.m_resourceModelData.m_allNoLightmapUV.Count))
            {
                isChange = true;
                m_showToggle = new bool[m_modelSystem.m_resourceModelData.m_allNoLightmapUV.Count];
                SetToggle(false);
                m_ChangeName = "勾上UV2";
                modelType = ResourceModelType.UV2;
                m_showItems = m_modelSystem.m_resourceModelData.m_allNoLightmapUV;
                isChange = false;

            }
            if (GUILayout.Button("检查Read/Write  数量:" + m_modelSystem.m_resourceModelData.m_allOnWrite.Count))
            {
                isChange = true;
                m_showToggle = new bool[m_modelSystem.m_resourceModelData.m_allOnWrite.Count];
                SetToggle(false);
                m_ChangeName = "去掉Read/Write";
                modelType = ResourceModelType.RW;
                m_showItems = m_modelSystem.m_resourceModelData.m_allOnWrite;
                isChange = false;

            }
            if (GUILayout.Button("检查BlendShape  数量:" + m_modelSystem.m_resourceModelData.m_BlendShape.Count))
            {
                isChange = true;
                m_showToggle = new bool[m_modelSystem.m_resourceModelData.m_BlendShape.Count];
                SetToggle(false);
                m_ChangeName = "去掉BlendShape";
                modelType = ResourceModelType.BlendShape;
                m_showItems = m_modelSystem.m_resourceModelData.m_BlendShape;
                isChange = false;

            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            m_chooseAll = EditorGUILayout.Toggle("", m_chooseAll);
            if (m_chooseAllLast != m_chooseAll)
            {
                m_chooseAllLast = m_chooseAll;
                SetToggle(m_chooseAll);
            }
            if (GUILayout.Button(m_ChangeName))
            {
                m_modelSystem.OnChangeResource(modelType, m_showToggle);
            }
            EditorGUILayout.EndHorizontal();

            scrollPos = EditorGUILayout.BeginScrollView(scrollPos);
            GUIItem();
            EditorGUILayout.EndScrollView();
        }

        void GUIItem()
        {
            if (isChange)
            {
                return;
            }
            for (int i = 0; i < m_showItems.Count; i++)
            {
                EditorGUILayout.BeginHorizontal();
                m_showToggle[i] = EditorGUILayout.Toggle("", m_showToggle[i]);
                if (GUILayout.Button(m_showItems[i]))
                {
                    GameObject obj = AssetDatabase.LoadAssetAtPath(m_showItems[i], typeof(GameObject)) as GameObject;
                    Selection.activeGameObject = obj;
                }
                EditorGUILayout.EndHorizontal();
            }
        }

        private void SetToggle(bool b)
        {
            for (int i = 0; i < m_showToggle.Length; i++)
            {
                m_showToggle[i] = b;
            }
        }
    }
}
