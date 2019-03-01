using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EngineCore;
using GOGUI;
using System;

namespace SeekerGame
{
    public class IconGrayTransparent : GameUIComponent
    {
        private GameImage m_icon_bg;
        private CanvasGroup m_canvasgroup;
        private GameImage m_icon_img;
        private GameUIEffect m_effect;

        private float m_disable_alpha = 0.5f;
        public float Disable_alpha
        {
            get { return m_disable_alpha; }
            set { m_disable_alpha = value; }
        }
        protected override void OnInit()
        {
            base.OnInit();

            m_icon_bg = Make<GameImage>(this.gameObject);
            m_canvasgroup = m_icon_bg.GetComponent<CanvasGroup>();
            m_icon_img = Make<GameImage>("Icon");
            m_effect = Make<GameUIEffect>("UI_jingyuansuipian");
            if (null != m_effect)
            {
                m_effect.EffectPrefabName = "UI_jingyuansuipian.prefab";
                m_effect.Visible = false;
            }

        }

        public override void OnShow(object param)
        {
            base.OnShow(param);
            
        }

        public override void OnHide()
        {
            base.OnHide();
            if (null != m_effect)
                m_effect.Visible = false;
        }

        public void Refresh(string icon_name_, bool disable_)
        {
            this.Refresh(icon_name_, disable_ ? true : false, disable_ ? Disable_alpha : 1.0f);
        }

        public void Refresh(string icon_name_, bool gray_, float alpha_)
        {
            this.m_icon_img.Sprite = icon_name_;
            if (null != m_effect)
                m_effect.Visible = !gray_;
            this.m_icon_bg.SetGray(gray_);
            m_icon_img.SetGray(gray_);

            m_canvasgroup.alpha = alpha_;

        }

    }
}

