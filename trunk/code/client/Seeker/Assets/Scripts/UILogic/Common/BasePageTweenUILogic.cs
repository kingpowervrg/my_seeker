using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using EngineCore;
using GOGUI;

namespace SeekerGame
{
    public class BasePageTweenUILogic : BasePageUILogic
    {
        //private GameImage[] m_Arrow_img;
        //private GameLabel[] m_PageTog_lab;

        //private readonly Color m_destColor = Color.white;
        //private readonly Color m_oriColor = new Color(173f / 255f, 226f / 255f, 1f);

        protected override void InitController()
        {
            base.InitController();
            //m_Arrow_img = new GameImage[m_pageCount];
            //m_PageTog_lab = new GameLabel[m_pageCount];
            //for (int i = 0; i < m_pageCount; i++)
            //{
            //    m_Arrow_img[i] = m_pageBtn[i].Make<GameImage>("Background:Arrow");
            //    m_Arrow_img[i].Visible = false;
            //    m_PageTog_lab[i] = m_pageBtn[i].Make<GameLabel>("Label");
            //    m_PageTog_lab[i].color = m_destColor;
            //}
        }

        protected override void OnPageChangeClick(int i)
        {
            base.OnPageChangeClick(i);
            //m_Arrow_img[i].Visible = true;
            //m_PageTog_lab[i].color = m_destColor;

            //m_pageEffect[i].Visible = true;
        }

        protected override void OnPageChangeCanel(int i)
        {
            base.OnPageChangeCanel(i);
            //m_Arrow_img[i].Visible = false;
            //m_PageTog_lab[i].color = m_oriColor;

            //m_pageEffect[i].Visible = false;
        }
    }
}
