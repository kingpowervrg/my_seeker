using UnityEngine;
using System.Collections;
using UnityEditor;

namespace SimpleNodeEditor
{
    [NodeMenuItem("ChatOptionContentNode", typeof(ChatOptionContentNode))]
    public class ChatOptionContentNode : BaseNode
    {
        [SerializeField]
        Inlet inlet = null;

        [SerializeField]
        Outlet outlet = null;

        protected override void Inited()
        {
        }

        public override void Construct()
        {
            Name = "ChatOptionNode";
            Size = new Vector2(200, 80);
            inlet = MakeLet<Inlet>("Input");
            outlet = MakeLet<Outlet>("Output");
        }
        public string contentValue;
        public override void WindowCallback(int id)
        {
            GUI.BeginGroup(new Rect(5, 25, 200, 80));
            contentValue = GUILayout.TextField(contentValue, GUILayout.MaxWidth(180));
            GUI.EndGroup();
            base.WindowCallback(id);
        }
    }
}
