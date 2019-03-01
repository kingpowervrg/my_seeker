using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EngineCore;

namespace SeekerGame
{
    public class ShopEnergyItemUIComponent : ShopNormalItemUIComponent
    {
        GameLabel m_item_in_bag_count_txt;

        public long Prop_ID
        {
            get
            {
                return m_itemdata.m_prop.id;
            }
        }


        protected override void OnInit()
        {
            base.OnInit();
            m_item_in_bag_count_txt = Make<GameLabel>("icon:count");
        }



        public override void OnShow(object param)
        {
            base.OnShow(param);


        }

        public override void OnHide()
        {
            base.OnHide();

        }

        public override void setData(ShopItemData itemdata, ShopType shopType, int index_, bool is_fast_buy = false)
        {
            base.setData(itemdata, shopType, index_, is_fast_buy);

            RefeshItemInbagNum();
        }



        protected override void btnBuy(GameObject obj)
        {
            if (m_itemdata == null)
            {
                return;
            }

            if (m_is_fast_buy)
                base.btnBuy(obj);
            else
                GameEvents.UIEvents.UI_Shop_Event.Listen_ShowBuyEnergyPopView.SafeInvoke(m_itemdata);

            //BuyManager.Instance.ShopBuy(m_itemdata.marketID, 1, m_itemdata.m_number, m_curShopType, m_cost, m_itemdata.m_costType);
        }

        public void RefeshItemInbagNum()
        {
            var item_in_bag = GlobalInfo.MY_PLAYER_INFO.GetBagInfosByID(m_itemdata.m_prop.id);
            int count = null != item_in_bag ? item_in_bag.Count : 0;
            m_item_in_bag_count_txt.Text = LocalizeModule.Instance.GetString("user_have_icon", count);
        }

        protected override void OneUseReqCallback(long id)
        {
            base.OneUseReqCallback(id);
            this.RefeshItemInbagNum();
        }


    }
}

