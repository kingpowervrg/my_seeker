using UnityEngine;
using System.Collections;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace SimpleNodeEditor
{
    public enum ChatNodeType
    {
        Normal = 0,
        WORD,
        IMAGE,
        Content_Word,
        Content_Image
    }

    public enum ChatHeadPosition
    {
        Left = 0,
        Right
    }

    [NodeMenuItem("ChatContent", typeof(ButtonNode))]
    public class ButtonNode : BaseNode
    {
        [SerializeField]
        Inlet inlet = null;

        [SerializeField]
        Outlet outlet = null;

        //void OnInletReceived(Signal signal)
        //{
        //    // any signal gets converted to bang
        //    outlet.Send(new SignalArgs());
        //}

        protected override void Inited()
        {
            //inlet.SlotReceivedSignal += OnInletReceived;
        }

        public override void Construct()
        {
            Name = "ChatContent";
            Size = new Vector2(200, 250);
            inlet = MakeLet<Inlet>("Input");
            //outlet = MakeLet<Outlet>("Output", 25);
            outlet = MakeLet<Outlet>("Output");
        }


        public string contentValue;
        public string headValue;

        public Texture imgValue_00;
        public Texture imgValue_01;
        public Texture imgValue_02;
        public Texture imgValue_03;

        public ChatNodeType m_nodeType = ChatNodeType.Normal;
        public ChatHeadPosition m_headPosition = ChatHeadPosition.Left;
#if UNITY_EDITOR
        public override void WindowCallback(int id)
        {
            GUI.BeginGroup(new Rect(5, 25, 200, 300));
            GUILayout.BeginVertical();

            GUILayout.Label("头像:");
            headValue = GUILayout.TextField(headValue, GUILayout.MaxWidth(180));

            //GUILayout.BeginHorizontal();
            GUILayout.Label("键值:");
            contentValue = GUILayout.TextField(contentValue, GUILayout.MaxWidth(180));
            //GUILayout.EndHorizontal();

           // GUILayout.BeginHorizontal();
            GUILayout.Label("关键物证:");
            imgValue_00 = EditorGUILayout.ObjectField(imgValue_00,typeof(Texture), GUILayout.MaxWidth(180)) as Texture;
            imgValue_01 = EditorGUILayout.ObjectField(imgValue_01, typeof(Texture), GUILayout.MaxWidth(180)) as Texture;
            imgValue_02 = EditorGUILayout.ObjectField(imgValue_02, typeof(Texture), GUILayout.MaxWidth(180)) as Texture;
            imgValue_03 = EditorGUILayout.ObjectField(imgValue_03, typeof(Texture), GUILayout.MaxWidth(180)) as Texture;
            m_headPosition = (ChatHeadPosition)EditorGUILayout.EnumPopup(m_headPosition, GUILayout.MaxWidth(180));

            m_nodeType = (ChatNodeType)EditorGUILayout.EnumPopup(m_nodeType, GUILayout.MaxWidth(180));
            GUILayout.EndVertical();

            GUI.EndGroup();
            //

            base.WindowCallback(id);
        }
#endif
    }
}
