using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EngineCore;
using GOGUI;
using System;

namespace SeekerGame
{
    public class IconNameNumItemView : GameUIComponent
    {

        private GameImage m_icon;
        private GameLabel m_name_txt;
        private GameLabel m_num_txt;

        public long ITEM_ID
        {
            private set;
            get;
        }

        protected override void OnInit()
        {
            base.OnInit();
            m_icon = Make<GameImage>("Image_BG:Image_Icon");
            m_name_txt = Make<GameLabel>("Text_Name");
            m_num_txt = Make<GameLabel>("Text_Number");
        }

        public override void OnShow(object param)
        {
            base.OnShow(param);


        }

        public override void OnHide()
        {
            base.OnHide();

        }


        public void Refresh(long id_, int num_)
        {
            ITEM_ID = id_;
            var data = ConfProp.Get(id_);
            m_icon.Sprite = data.icon;
            m_name_txt.Text = LocalizeModule.Instance.GetString(data.name);
            m_num_txt.Text = 0 == num_ ? "" : num_.ToString();
        }





    }
}

