using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Resource
{
    public class ResouceWindow : EditorWindow
    {

        static ResouceWindow window;
        ResourceShaderSystem m_shaderSystem;
        ResourceUISystem m_uiSystem;
        ResourceExhibitSystem m_exhibitSystem;
        private Dictionary<string, List<string>> m_groupList = new Dictionary<string, List<string>>();

        [MenuItem("Tools/resource/资源管理")]
        static void CreateWindow()
        {
            Rect rect = new Rect(0, 0, 500, 400);
            window = (ResouceWindow)EditorWindow.GetWindowWithRect(typeof(ResouceWindow), rect, true, "资源管理器");
            if (window.Init())
            {
                window.Show();
            }
        }

        public bool Init()
        {
            m_shaderSystem = new ResourceShaderSystem();
            m_uiSystem = new ResourceUISystem();
            m_exhibitSystem = new ResourceExhibitSystem();
            return true;
        }
        string m_sceneName;
        Vector2 scrollPos = Vector2.zero;
        void OnGUI()
        {
            EditorGUILayout.BeginVertical();
            ResourceData.m_RelativeResourceDir = EditorGUILayout.TextField("资源目录:", ResourceData.m_RelativeResourceDir);
            if (GUILayout.Button("获取物件ID所引用的场景"))
            {
                ResourceExhibitWindow.CreateWindow("获取物件ID所引用的场景");
            }
            if (GUILayout.Button("统计物件表"))
            {
                m_shaderSystem.CensusExhibit();
            }
            if (GUILayout.Button("检测物件表失效物件"))
            {
                m_exhibitSystem.LoadSceneData();
                m_exhibitSystem.CheckSceneData();
                ResourceLoseWindow.CreateWindow(m_exhibitSystem.emptyDataNames, m_exhibitSystem.emptyDataPaths, "物件表失效物件");
                //m_shaderSystem.CensusExhibit();
            }
            if (GUILayout.Button("查看模型资源"))
            {
                ResourceModelWindow.CreateWindow();

            }
            if (GUILayout.Button("检测UI Depth"))
            {
                m_uiSystem.CheckUIDepth();
                m_groupList = m_uiSystem.m_sameUI;
            }
            if (GUILayout.Button("检测shader引用"))
            {
                m_shaderSystem.GetAllShaderByMat();
                m_groupList = m_shaderSystem.m_ShaderCount;
            }
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("统计shader"))
            {
                m_shaderSystem.GetAllShader();
                m_groupList = m_shaderSystem.m_ShaderCount;
            }
            if (GUILayout.Button("替换shader"))
            {
                m_shaderSystem.ReplaceShader();
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Mesh:" + m_shaderSystem.m_MeshRenderCount);
            EditorGUILayout.LabelField("SkinnedMesh:" + m_shaderSystem.m_SkinnedMeshRenderCount);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("不存在:" + m_shaderSystem.m_LoseExhibit.Count))
            {
                ResourceLoseWindow.CreateWindow(m_shaderSystem.m_LoseExhibit, m_shaderSystem.m_LoseExhibit, "不存在");
            }
            if (GUILayout.Button("丢失Mesh:" + m_shaderSystem.m_LoseMesh.Count))
            {
                ResourceLoseWindow.CreateWindow(m_shaderSystem.m_LoseMesh, m_shaderSystem.m_LoseMesh, "丢失Mesh");
            }
            if (GUILayout.Button("丢失材质:" + m_shaderSystem.m_LoseMeshMaterial.Count))
            {
                ResourceLoseWindow.CreateWindow(m_shaderSystem.m_LoseMeshMaterial, m_shaderSystem.m_LoseMeshMaterial, "丢失材质");
            }
            EditorGUILayout.EndHorizontal();

            #region 展示shader列表
            scrollPos = EditorGUILayout.BeginScrollView(scrollPos);

            foreach (var kv in m_groupList)
            {
                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button(kv.Key + ":" + kv.Value.Count))
                {
                    ResourceLoseWindow.CreateWindow(kv.Value, kv.Value, "shader : " + kv.Key);
                }
                EditorGUILayout.EndHorizontal();
            }
            EditorGUILayout.EndScrollView();
            #endregion
            EditorGUILayout.EndVertical();
        }
    }
}


