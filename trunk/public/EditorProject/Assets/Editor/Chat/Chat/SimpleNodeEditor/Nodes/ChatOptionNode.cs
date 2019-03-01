using UnityEngine;
using System.Collections;
using UnityEditor;


namespace SimpleNodeEditor
{
    [NodeMenuItem("ChatOptionNode", typeof(ChatOptionNode))]
    public class ChatOptionNode : BaseNode
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
        public Texture tex = null;
        public override void WindowCallback(int id)
        {
            Rect textureRect = new Rect(5,25,100,80);
            GUI.BeginGroup(new Rect(5, 25, 200, 80));
            tex = EditorGUILayout.ObjectField(tex, typeof(Texture), true) as Texture;
            GUI.EndGroup();
            base.WindowCallback(id);

        }
    }
}
