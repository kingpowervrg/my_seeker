using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EngineCore;
using GOGUI;
using System;

namespace SeekerGame
{
    public class ButtonIconItem : GameUIComponent
    {
        private SafeAction<bool, int> m_onItemIsClicked;
        private SafeAction<bool, int> m_onItemIsLongPressed;

        private GameImage m_icon_img;
        private GameImage m_selected_icon;

        private int m_item_id;
        public int Item_id
        {
            get { return m_item_id; }
        }

        protected override void OnInit()
        {
            base.OnInit();
            m_icon_img = Make<GameImage>("Background:Icon");
            m_selected_icon = Make<GameImage>("Background:Checkmark");
        }

        public override void OnShow(object param)
        {
            base.OnShow(param);

            m_icon_img.AddClickCallBack(BtnClick);


            m_icon_img.AddLongPressCallBack(LongPressDown);
            m_icon_img.AddLongPressEndCallBack(LongPressUp);
        }

        public override void OnHide()
        {
            base.OnHide();
            m_icon_img.RemoveClickCallBack(BtnClick);


            m_icon_img.RemoveLongPressCallBack(LongPressDown);
            m_icon_img.RemoveLongPressEndCallBack(LongPressUp);
        }


        public void Refresh(int id_, string icon_name_, string info_, bool checked_, Action<bool, int> clicked_, Action<bool, int> pressed_)
        {
            this.m_item_id = id_;
            this.m_icon_img.Sprite = icon_name_;

            this.m_onItemIsClicked = clicked_;

            this.m_onItemIsLongPressed = pressed_;

            if (checked_)
            {
                m_selected_icon.Visible = true;
                m_onItemIsClicked.SafeInvoke(checked_, this.m_item_id);
            }
            else
            {
                m_selected_icon.Visible = false;
            }
        }

        public void UnChecked()
        {
            m_selected_icon.Visible = false;
        }

        void BtnClick(GameObject obj)
        {


            //if (m_selected_icon.Visible)
            //{
            //    UnChecked();

            //    if (!m_onItemIsClicked.IsNull)
            //        this.m_onItemIsClicked.SafeInvoke(false, this.m_item_id);

            //    return;
            //}

            m_selected_icon.Visible = true;

            if (!m_onItemIsClicked.IsNull)
                this.m_onItemIsClicked.SafeInvoke(true, this.m_item_id);
        }


        void LongPressDown(GameObject obj, Vector2 delta)
        {


            if (!m_onItemIsLongPressed.IsNull)
                this.m_onItemIsLongPressed.SafeInvoke(true, this.m_item_id);
        }

        void LongPressUp(GameObject obj)
        {
            if (!m_onItemIsLongPressed.IsNull)
                this.m_onItemIsLongPressed.SafeInvoke(false, this.m_item_id);
        }

    }
}

