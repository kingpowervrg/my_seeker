using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EngineCore;

namespace SeekerGame
{
    public class NoticImgPanel : GameUIComponent
    {
        private NoticImg[] m_noticImgs;

        protected override void OnInit()
        {
            base.OnInit();
            m_noticImgs = new NoticImg[4];
            for (int i = 0; i < 4; i++)
            {
                m_noticImgs[i] = Make<NoticImg>(string.Format("Panel_{0}",i));
            }
        }

        public override void OnShow(object param)
        {
            base.OnShow(param);
        }

        public override void OnHide()
        {
            base.OnHide();
        }

        public void setData(NoticeInfo info0,NoticeInfo info1,bool style)
        {
            if (style)
            {
                m_noticImgs[2].Visible = false;
                m_noticImgs[3].Visible = false;
                setData(m_noticImgs[0],info0);
                setData(m_noticImgs[1], info1);
            }
            else
            {
                m_noticImgs[0].Visible = false;
                m_noticImgs[1].Visible = false;
                setData(m_noticImgs[2], info0);
                setData(m_noticImgs[3], info1);
            }
        }

        private void setData(NoticImg noticImg,NoticeInfo info)
        {
            if (info == null)
            {
                noticImg.Visible = false;
            }
            else
            {
                noticImg.Visible = true;
                noticImg.setData(info);
            }
        }
    }

    public class NoticImg : GameUIComponent
    {
        private GameLabel m_name_lab;
        private GameLabel m_time_lab;
        private GameNetworkRawImage m_img_netImg;

        protected override void OnInit()
        {
            base.OnInit();
            m_img_netImg = Make<GameNetworkRawImage>("RawImage");
            m_name_lab = Make<GameLabel>("RawImage:Text_1");
            m_time_lab = Make<GameLabel>("RawImage:Text_2");
        }

        public void setData(NoticeInfo info)
        {
            this.m_time_lab.Visible = false;
            this.m_name_lab.Visible = false;
            //m_name_lab.Text = info.Title;
            System.DateTime startTime = CommonTools.TimeStampToDateTime(info.StartTime * 10000);
            System.DateTime endTime = CommonTools.TimeStampToDateTime(info.EndTime * 10000);
            //m_time_lab.Text = string.Format("{0}~{1}", startTime.ToString("yyyy.MM.dd"), endTime.ToString("yyyy.MM.dd"));
            m_img_netImg.TextureName = info.Picture;
        }
    }
}


