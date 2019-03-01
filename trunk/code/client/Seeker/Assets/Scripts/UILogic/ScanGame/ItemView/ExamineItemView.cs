using EngineCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace SeekerGame
{
    public class ExamineItemView : GameUIComponent
    {

        GameImage m_icon;
        GameLabel m_desc_txt;

        protected override void OnInit()
        {

            m_icon = this.Make<GameImage>("Image (1)");
            m_desc_txt = m_icon.Make<GameLabel>("Text (1)");


        }

        public override void OnShow(object param)
        {
            base.OnShow(param);


        }

        public override void OnHide()
        {
            base.OnHide();
        }


        public void Refresh(string icon_, string desc_)
        {
            m_icon.Sprite = icon_;
            m_desc_txt.Text = desc_;

        }



    }

}
