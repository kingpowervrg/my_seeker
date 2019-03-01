using System;
using System.Collections.Generic;
using EngineCore;
using UnityEngine;
namespace SeekerGame
{
    public class PresuadeScroll3DComponent : Scroll3DComponent
    {

        public void SetData(long[] ids)
        {
            ResetItemCom();
            //this.m_container.Clear();
            count = ids.Length;
            itemArray = new ChooseItemComponent[count];
            this.m_container.EnsureSize<ChooseItemComponent>(count);
            for (int i = 0; i < count; i++)
            {
                ChooseItemComponent item = this.m_container.GetChild<ChooseItemComponent>(i);
                item.SetData(ids[i]);
                item.SetData(count, i, m_centerPos, this.m_leftParent);
                itemArray[i] = item;
                item.Visible = true;
            }
        }

        
    }

    public class ChooseItemComponent : Scroll3DItem
    {
        private GameImage m_iconImg = null;
        private GameLabel m_contentLab = null;
        private GameImage m_btn;
        private GameLabel m_titleLab = null;
        private long m_id;
        private Action<long> chooseAction;
        protected override void OnInit()
        {
            base.OnInit();
            //this.m_btn = Make<GameImage>("Image");
            this.m_iconImg = Make<GameImage>("Image_icon:icon");
            this.m_contentLab = Make<GameLabel>("Text");
            this.m_titleLab = Make<GameLabel>("title");
            this.xSpace = 150f;
        }

        public override void OnShow(object param)
        {
            base.OnShow(param);
            //this.m_btn.AddClickCallBack(OnChoose);
        }

        public override void OnHide()
        {
            base.OnHide();
            //this.m_btn.RemoveClickCallBack(OnChoose);
        }

        public void SetData(long id)
        {
            //chooseAction = cb;
            this.m_id = id;
            ConfProp confProp = ConfProp.Get(id);
            this.m_iconImg.Sprite = confProp.icon;
            this.m_contentLab.Text = LocalizeModule.Instance.GetString(confProp.description);
            this.m_titleLab.Text = LocalizeModule.Instance.GetString(confProp.name);
        }

        //private void OnChoose(GameObject obj)
        //{
        //    //chooseAction(m_id);
        //}
    }
}
