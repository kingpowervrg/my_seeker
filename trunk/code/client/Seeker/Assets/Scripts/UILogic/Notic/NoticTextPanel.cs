using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EngineCore;

namespace SeekerGame
{
    public class NoticTextPanel : GameUIComponent
    {
        private GameLabel m_title_lab;
        private GameLabel m_time_lab;
        private GameLabel m_content_lab;

        protected override void OnInit()
        {
            base.OnInit();
            m_title_lab = Make<GameLabel>("Text_title:Text_title");
            m_time_lab = Make<GameLabel>("Text_title:Text_time");
            m_content_lab = Make<GameLabel>("Text_detail");
        }

        public override void OnShow(object param)
        {
            base.OnShow(param);
        }

        public override void OnHide()
        {
            base.OnHide();
        }

        public void setData(NoticeInfo info)
        {
            m_title_lab.Text = LocalizeModule.Instance.GetString(info.Title);
            m_time_lab.Text = CommonTools.TimeStampToDateTime(info.StartTime * 10000).ToString("yyyy.MM.dd");
            m_content_lab.Text = LocalizeModule.Instance.GetString(info.Content);
        }
    }
}

