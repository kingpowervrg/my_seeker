using UnityEngine;
using System.Collections;
using UnityEditor;

namespace SimpleNodeEditor
{
   // [CustomEditor(typeof(NodeGraph))]
    public class NodeGraphInspector : Editor
    {
        [MenuItem("Tools/SimpleGraph/ChatGraph")]
        public static void CreateChatGraph()
        {
            NodeEditor chatWindow = EditorWindow.GetWindow<NodeEditor>();
            chatWindow.Construct();
            //chatWindow.InitChatGraph();
            chatWindow.Show();
        }

        [MenuItem("Tools/SimpleGraph/Persuade")]
        public static void CreatePersuadeGraph()
        {
            PersuadeEditor persuadeWindow = EditorWindow.GetWindow<PersuadeEditor>();
            persuadeWindow.Construct();
            //chatWindow.InitChatGraph();
            persuadeWindow.Show();
        }
        //public override void OnInspectorGUI()
        //{
        //    base.OnInspectorGUI();

        //    NodeGraph myTarget = (NodeGraph)target;
        //    if( GUILayout.Button("Show Graph") )
        //    {
        //        ShowGraph(myTarget);
        //    }
        //}

        //public void OnShowGraphClicked()
        //{

        //}

        //public void ShowGraph(NodeGraph sender)
        //{
        //    NodeEditor nodeEditor = (NodeEditor) EditorWindow.GetWindow(typeof(NodeEditor));
        //    nodeEditor.ConnectionColor = sender.ConnectionColor;
        //    nodeEditor.Root = sender.gameObject;
        //    nodeEditor.MasterInlet = sender.Inlet;
        //    nodeEditor.MasterOutlet = sender.Outlet;
        //    nodeEditor.Show();
        //}
    }
}
