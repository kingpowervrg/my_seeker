using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EngineCore;
using GOGUI;
using System;

namespace SeekerGame
{
    public class ButtonTextItem : GameUIComponent
    {
        private SafeAction<long> m_onItemIsClicked;

        private GameButton m_btn;
        private GameLabel m_txt;

        private long m_item_id;
        public long ITEM_ID
        {
            get { return m_item_id; }
            private set { m_item_id = value; }
        }

        protected override void OnInit()
        {
            base.OnInit();
            m_btn = Make<GameButton>("Btn");
            m_txt = Make<GameLabel>("Text");
        }

        public override void OnShow(object param)
        {
            base.OnShow(param);

            m_btn.AddClickCallBack(BtnClick);



        }

        public override void OnHide()
        {
            base.OnHide();
            m_btn.RemoveClickCallBack(BtnClick);



        }


        public void Refresh(long id_, string info_, Action<long> clicked_)
        {
            this.ITEM_ID = id_;
            this.m_txt.Text = info_;
            this.m_onItemIsClicked = clicked_;


        }

        void BtnClick(GameObject obj)
        {

            if (!m_onItemIsClicked.IsNull)
                this.m_onItemIsClicked.SafeInvoke(this.ITEM_ID);
        }

    }
}

