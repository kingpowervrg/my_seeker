//#define PLATFORM_ID
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EngineCore;

namespace SeekerGame
{
    public class IAPcashItemUIView : GameUIComponent
    {
        private GameUIEffect m_icon_effect;
        private GameImage m_icon_img;
        private GameLabel m_count_lab;

        private GameImage m_praise_icon;

        private GameButton m_buy_btn;
        private GameLabel m_cost_lab;

        private ConfCharge m_itemdata;

        protected override void OnInit()
        {
            base.OnInit();

            m_icon_img = Make<GameImage>("icon");
            m_count_lab = m_icon_img.Make<GameLabel>("count");

            m_praise_icon = Make<GameImage>("Image_label (1)");

            m_buy_btn = Make<GameButton>("btnbuy");
            m_cost_lab = m_buy_btn.Make<GameLabel>("Text");


            m_icon_effect = Make<GameUIEffect>("UI_chaopiao");
        }



        public override void OnShow(object param)
        {
            base.OnShow(param);
            m_buy_btn.AddClickCallBack(btnBuy);
        }

        public override void OnHide()
        {
            base.OnHide();
            m_buy_btn.RemoveClickCallBack(btnBuy);
            m_itemdata = null;
        }

        public void Refresh(ConfCharge itemdata, int index_)
        {
            m_itemdata = itemdata;

            m_icon_img.Sprite = itemdata.icon;

            if (!string.IsNullOrEmpty(itemdata.discountIcon) && !string.IsNullOrWhiteSpace(itemdata.discountIcon))
                m_praise_icon.Sprite = itemdata.discountIcon;
            else
                m_praise_icon.Visible = false;

            m_count_lab.Text = string.Format("x{0}", itemdata.cashCount + itemdata.addCount);

            string price = string.Format("{0:N2}", (float)itemdata.dollar / 100.0f);
#if  UNITY_IOS && PLATFORM_ID
            //price = GameEvents.IAPEvents.Sys_GetPriceIOSEvent.SafeInvoke(itemdata.chargeSouceId);
#else
            //price = GameEvents.IAPEvents.Sys_GetPriceEvent.SafeInvoke(itemdata.id);
#endif
            m_cost_lab.Text = string.IsNullOrEmpty(price) ? "" : price;


            if (null != m_icon_effect)
                m_icon_effect.EffectPrefabName = string.Format("UI_chaopiao_0{0}.prefab", index_ + 1);
        }

        private void btnBuy(GameObject obj)
        {
            EngineCoreEvents.AudioEvents.PlayAudio.SafeInvoke(Audio.AudioType.UISound, EngineCommonAudioKey.Button_Click_Common.ToString());

#if UNITY_IOS && PLATFORM_ID
                    GameEvents.IAPEvents.Sys_BuyProductIOSEvent.SafeInvoke(m_itemdata.chargeSouceId);
#else
            GameEvents.IAPEvents.Sys_BuyProductEvent.SafeInvoke(m_itemdata.id);
#endif


        }
    }
}

