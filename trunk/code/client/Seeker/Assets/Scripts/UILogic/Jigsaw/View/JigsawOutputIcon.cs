using EngineCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace SeekerGame
{
    public class JigsawOutputIcon : GameUIComponent
    {

        public GameLabel m_min_txt;
        public GameImage m_min_icon;
        public GameLabel m_max_txt;
        public GameImage m_max_icon;


        private string m_icon_name;
        private int m_max_num;
        protected override void OnInit()
        {
            m_min_icon = this.Make<GameImage>("Image_2");
            m_min_txt = m_min_icon.Make<GameLabel>("Text");
            m_min_txt.Text = "1";

            m_max_icon = this.Make<GameImage>("Image_1");
            m_max_txt = m_max_icon.Make<GameLabel>("Text");
        }

        public override void OnShow(object param)
        {
            m_min_icon.Sprite = m_icon_name;

            if (1 != m_max_num)
            {
                m_max_icon.Sprite = m_icon_name;
                m_max_txt.Text = String.Format("~{0}", m_max_num.ToString());
            }
            else
            {
                m_max_icon.Visible = false;
                m_max_txt.Text = "";
            }
        }

        public override void OnHide()
        {
        }

        public void Refresh(string icon_name_, int max_num_)
        {
            m_icon_name = icon_name_;
            m_max_num = max_num_;
        }
    }
}
