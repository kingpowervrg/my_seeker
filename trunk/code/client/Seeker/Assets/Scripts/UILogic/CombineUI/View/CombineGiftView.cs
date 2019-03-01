using DG.Tweening;
using EngineCore;
using GOGUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace SeekerGame
{
    public class CombineGiftView : BaseViewComponet<CombineUILogic>
    {

        CombineGiftItemView[] m_gifts = new CombineGiftItemView[3];
        GameUIComponent m_close_btn;

        protected override void OnInit()
        {
            base.OnInit();
            m_gifts[0] = Make<CombineGiftItemView>("Image_sence:Background");
            m_gifts[1] = Make<CombineGiftItemView>("Image_sence:Background (1)");
            m_gifts[2] = Make<CombineGiftItemView>("Image_sence:Background (2)");
            this.SetCloseBtnID("Image_sence:Button_close");
            m_close_btn = Make<GameUIComponent>("Image");
        }

        public override void OnShow(object param)
        {
            base.OnShow(param);
            m_close_btn.AddClickCallBack(OnCloseClicked);
        }

        public override void OnHide()
        {
            base.OnHide();
            m_close_btn.RemoveClickCallBack(OnCloseClicked);
        }

        public void Refresh(IEnumerable<CombineGiftData> datas)
        {
            int i = 0;

            foreach (CombineGiftData data in datas)
            {
                if (i >= m_gifts.Length)
                    break;

                m_gifts[i].Refresh(ConfProp.Get(data.m_id).icon, data.m_num);
                ++i;
            }
        }

        void OnCloseClicked(GameObject obj)
        {
            this.Visible = false;
        }
    }
}
