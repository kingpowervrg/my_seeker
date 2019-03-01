using UnityEngine;

namespace Chat
{
    [System.Serializable]
    public class ChatLink
    {
        private Vector2 m_size = new Vector2(30,15);

        private int m_number = 0;
        private bool m_isLeft = true;

        private Vector2 m_position;
        public Vector2 Position
        {
            set {
                m_position = value;
                m_rect = new Rect(m_position,m_size);
            }
            get { return m_position; }
        }

        private Rect m_rect;

        public void OnInit(int number,bool isLeft)
        {
            this.m_number = number;
            this.m_isLeft = isLeft;
        }


        public void OnDraw(Rect rect)
        {
            float x = rect.x, y = rect.y;
            if (!m_isLeft)
            {
                x = rect.x + rect.width;
            }
            y = rect.y - (m_number + 1) * m_size.y;
            Position = new Vector2(x,y);
            GUI.Box(m_rect, "aaaa");
        }
    }
}
