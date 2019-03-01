using UnityEngine;
using System.Collections;
using UnityEditor;

namespace SimpleNodeEditor
{
    public enum PersuadeTalkType
    {
        Self,
        NPC
    }

    public enum PersuadeType
    {
        普通对话,
        说服阶段,
        正向反馈
    }

    [NodeMenuItem("PersuadeBaseNode", typeof(PersuadeBaseNode))]
    public class PersuadeBaseNode : BaseNode
    {
        [SerializeField]
        Inlet inlet = null;

        [SerializeField]
        Outlet outlet = null;

        Outlet feedbackOutlet = null;

        protected override void Inited()
        {
        }

        public override void Construct()
        {
            Name = "PersuadeBaseNode";
            Size = new Vector2(200, 200);
            inlet = MakeLet<Inlet>("Input");
            outlet = MakeLet<Outlet>("Output");

            feedbackOutlet = MakeLet<Outlet>("正向反馈",20);
            feedbackOutlet.type = 1;
        }
        public PersuadeTalkType m_talkType = PersuadeTalkType.Self;
        public PersuadeType m_type = PersuadeType.普通对话;
        public string content = string.Empty;
        public long evidenceID;
        //public int feedbackID;
        public override void WindowCallback(int id)
        {
            GUI.BeginGroup(new Rect(5, 45, 200, 200));
            GUILayout.BeginVertical();
            m_type = (PersuadeType)EditorGUILayout.EnumPopup(m_type, GUILayout.MaxWidth(180));
            m_talkType = (PersuadeTalkType)EditorGUILayout.EnumPopup(m_talkType, GUILayout.MaxWidth(180));

            GUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("内容:", GUILayout.MaxWidth(50));
            content = GUILayout.TextField(content, GUILayout.MaxWidth(130));
            GUILayout.EndHorizontal();

            if (m_type == PersuadeType.说服阶段 && m_talkType == PersuadeTalkType.NPC)
            {
                GUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("证据ID:", GUILayout.MaxWidth(50));
                evidenceID = EditorGUILayout.LongField(evidenceID, GUILayout.MaxWidth(130));
                GUILayout.EndHorizontal();
            }

            GUILayout.EndVertical();
            //tex = EditorGUILayout.ObjectField(tex, typeof(Texture), true) as Texture;
            GUI.EndGroup();
            base.WindowCallback(id);

        }
    }
}
