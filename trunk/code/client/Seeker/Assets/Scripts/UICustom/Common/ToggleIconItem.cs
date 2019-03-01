using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EngineCore;
using GOGUI;
using System;

namespace SeekerGame
{
    public class ToggleIconItem : GameUIComponent
    {
        private SafeAction<bool, long> m_onItemIsClicked;

        private GameImage m_icon_img;

        private GameToggleButton m_item_btn;

        private GameImage m_red_point;

        private GameImage m_lock_img;

        private int m_item_idx;
        public int Item_idx
        {
            get { return m_item_idx; }
        }

        private object m_info;
        public object Info
        {
            get { return m_info; }
            set { m_info = value; }
        }
        public bool RedPointVisible
        {
            set
            {
                if (null == m_lock_img)
                {
                    m_lock_img = Make<GameImage>("ImgWarn");

                }
                m_lock_img.Visible = value;
            }
        }

        public bool LockVisible
        {
            set
            {
                if (null == m_red_point)
                {
                    m_red_point = Make<GameImage>("lockmark");

                }
                m_red_point.Visible = value;
            }
        }


        protected override void OnInit()
        {
            base.OnInit();
            m_icon_img = Make<GameImage>(this.gameObject);
            m_item_btn = Make<GameToggleButton>(gameObject);
          
        }

        public override void OnShow(object param)
        {
            base.OnShow(param);

            m_item_btn.AddChangeCallBack(BtnClick);

        }

        public override void OnHide()
        {
            base.OnHide();
            m_item_btn.RemoveChangeCallBack(BtnClick);
        }


        public void Refresh(int idx_, string icon_name_, object info_, bool checked_, Action<bool, long> clicked_, Action<bool, long> pressed_)
        {
            Info = info_;
            this.m_item_idx = idx_;
            this.m_icon_img.Sprite = icon_name_;
            m_item_btn.Checked = false;
            this.m_onItemIsClicked = clicked_;
            m_item_btn.Checked = checked_;
            

        }



        void BtnClick(bool value)
        {
            if (!m_onItemIsClicked.IsNull)
                this.m_onItemIsClicked.SafeInvoke(value, (long)Info);
        }


    }
}

