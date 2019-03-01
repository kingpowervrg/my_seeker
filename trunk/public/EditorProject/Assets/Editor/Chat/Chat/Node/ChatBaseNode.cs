using UnityEngine;
using System;
using System.Collections.Generic;
namespace Chat
{
    public abstract class ChatBaseNode
    {
        public int id;

        private Vector2 m_Position;
        public Vector2 Positon
        {
            set {
                m_Position = value;
                m_rect = new Rect(m_Position, m_size);
            }

            get
            {
                return m_Position;
            }
        }

        protected Vector2 m_size = Vector2.one * 100;

        protected Rect m_rect;

        public string NodeName;

        protected List<ChatLink> m_chatLink = new List<ChatLink>();

        protected T MakeLet<T>(bool isLeft) where T : ChatLink
        {
            T t = Activator.CreateInstance(typeof(T)) as T;
            t.OnInit(m_chatLink.Count, isLeft);
            m_chatLink.Add(t);
            return t;
        }

        public abstract void Construct();

        public void OnDraw()
        {
            m_rect = GUI.Window(id, m_rect, WindowCallback, NodeName);
            Vector2 newPos = Vector2.right * m_rect.x + Vector2.up * m_rect.y;

            
        }


        protected virtual void WindowCallback(int id)
        {

            for (int i = 0; i < m_chatLink.Count; i++)
            {
                m_chatLink[i].OnDraw(m_rect);
            }
            GUI.DragWindow();
        }
    }
}
