using EngineCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace SeekerGame
{
    public class ClueProgressItemView : GameUIComponent
    {
        int m_scan_type;
        public int Scan_type
        {
            get { return m_scan_type; }
        }
        GameImage m_icon;
        GameLabel m_cur_num_txt;
        GameLabel m_total_num_txt;
        GameProgressBar m_slider;

        int m_cur_num;
        int m_total_num;

        protected override void OnInit()
        {
            m_slider = this.Make<GameProgressBar>("Slider_1");

            m_icon = m_slider.Make<GameImage>("Image");
            m_cur_num_txt = m_slider.Make<GameLabel>("Text");
            m_total_num_txt = m_slider.Make<GameLabel>("Text (2)");
        }

        public override void OnShow(object param)
        {
            base.OnShow(param);
            m_slider.Value = 0.0f;

        }

        public override void OnHide()
        {
            base.OnHide();
        }


        public void Refresh(int scan_type_, string icon_, int total_num_)
        {
            m_scan_type = scan_type_;
            m_icon.Sprite = icon_;
            m_cur_num_txt.Text = "0";
            m_total_num_txt.Text = total_num_.ToString();

            m_total_num = total_num_;
            m_cur_num = 0;
        }

        public void AddProgress(int scan_type_, int add_num_)
        {
            if (m_scan_type != scan_type_)
                return;

            m_cur_num += add_num_;
            m_cur_num_txt.Text = m_cur_num.ToString();
            float progress = (float)m_cur_num / (float)m_total_num;

            m_slider.Value = progress;
        }


    }

}
