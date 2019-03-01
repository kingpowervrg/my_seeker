using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EngineCore;
using UnityEngine;
namespace SeekerGame
{
    class CustomPoliceItem : GameUIComponent
    {

        private BlurGrayTexture m_police_head_tex;
        public BlurGrayTexture Police_head_tex
        {
            get { return m_police_head_tex; }
        }

        public long OfficerID
        {
            get;
            set;
        }
        //private GameButton m_action_btn;

        protected override void OnInit()
        {
            base.OnInit();
            this.FindUIComponent();
        }

        public override void OnShow(object param)
        {
            base.OnShow(param);
        }


        public override void OnHide()
        {
            base.OnHide();
        }

        public void InitTex(string tex_name_)
        {
            m_police_head_tex.TextureName = tex_name_;
        }

        public void InitMaterial()
        {
            m_police_head_tex.InitBlurMaterial();
        }



        private void FindUIComponent()
        {
            m_police_head_tex = this.Make<BlurGrayTexture>(gameObject);
        }
    }
}
