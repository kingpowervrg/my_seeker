using EngineCore;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SeekerGame
{
    [UILogicHandler(UIDefine.UI_SHOPENERGY)]
    public class ShopEnergyUILogic : ShopCashUILogic
    {
        //局内商店组件
        private EnergyInternalShopComponent m_gameInternalShopComponent = null;



        protected override void OnInit()
        {
            base.OnInit();
            cate = Category.Vit;
            shopType = ShopType.Vit;

            this.m_gameInternalShopComponent = this.Make<EnergyInternalShopComponent>("Panel_Buy");
        }

        public override void OnShow(object param)
        {
            base.OnShow(param);
            GameEvents.UIEvents.UI_Shop_Event.Listen_ShowBuyEnergyPopView += ShowBuyPopView;
        }

        public override void OnHide()
        {
            base.OnHide();

            GameEvents.UIEvents.UI_Shop_Event.Listen_ShowBuyEnergyPopView -= ShowBuyPopView;
        }


        protected override void InitItemView(ShopPropData propdata)
        {
            int count = propdata.m_itemdata.Count;
            m_grid_com.EnsureSize<ShopEnergyItemUIComponent>(count);
            for (int i = 0; i < count; i++)
            {
                if (propdata.m_itemdata[i] != null)
                {
                    ShopEnergyItemUIComponent com = m_grid_com.GetChild<ShopEnergyItemUIComponent>(i);
                    com.setData(propdata.m_itemdata[i], shopType, i, is_fast_buy);
                    com.Visible = true;
                }
            }
        }



        public void RefreshItemCount(long itemID, int itemNum)
        {
            for (int i = 0; i < m_grid_com.ChildCount; ++i)
            {
                var item = m_grid_com.GetChild<ShopEnergyItemUIComponent>(i);

                if (item.Prop_ID == itemID)
                {
                    item.RefeshItemInbagNum();
                }
            }
        }

        private void ShowBuyPopView(ShopItemData data_)
        {
            m_gameInternalShopComponent.SetBuyItemInfo(data_);
            m_gameInternalShopComponent.Visible = true;
        }



    }

    public class EnergyInternalShopComponent : GameInternalShopComponent
    {

        private ShopItemData m_itemdata;

        protected override int CurrentBuyNum
        {
            get { return this.m_currentBuyNum; }
            set
            {
                this.m_currentBuyNum = value;
                this.m_lbBuyNum.Text = value.ToString();
                this.m_lbOriginPrice.Text = $"{ this.m_itemdata.m_oriPrice * value}";
                this.m_originPriceComponent.Visible = this.m_itemdata.m_hasDis;
                if (this.m_itemdata.m_hasDis)
                    this.m_lbTotalPriceAfterDiscount.Text = $"{ this.m_itemdata.m_disPrice * value}";
                else
                    this.m_lbTotalPriceAfterDiscount.Text = $"{ this.m_itemdata.m_oriPrice * value}";
            }
        }

        protected override void OnBtnBuyClick(GameObject btnItem)
        {

            BuyManager.Instance.ShopBuy(m_itemdata.marketID, CurrentBuyNum, CurrentPropNum, ShopType.Vit, this.m_itemdata.m_oriPrice, this.m_itemdata.m_costType);

            MessageHandler.RegisterMessageHandler(MessageDefine.MarkeBuyResponse, OnBuyPropResponse);


        }


        protected override void OnBuyPropResponse(object buyResultMsg)
        {
            MessageHandler.UnRegisterMessageHandler(MessageDefine.MarkeBuyResponse, OnBuyPropResponse);
            MarkeBuyResponse resp = buyResultMsg as MarkeBuyResponse;
            if (!MsgStatusCodeUtil.OnError(resp.ResponseStatus))
            {
                if (resp.Props != null || resp.Props.Count > 0)
                {
                    long itemID = resp.Props.PropId;
                    //int itemNum = resp.Props.Count;

                    //GlobalInfo.MY_PLAYER_INFO.AddSingleBagInfo(itemID, itemNum);

                    (LogicHandler as ShopEnergyUILogic).RefreshItemCount(itemID, 0);
                }
            }
            Visible = false;
        }

        protected override void OnBtnExitClick(GameObject btnExit)
        {
            EngineCoreEvents.AudioEvents.PlayAudio.SafeInvoke(Audio.AudioType.UISound, EngineCommonAudioKey.Close_Window.ToString());

            Visible = false;
        }

        public void SetBuyItemInfo(ShopItemData itemdata)
        {
            m_itemdata = itemdata;

            this.m_itemConf = m_itemdata.m_prop;


            RefreshUI();
        }

        protected override void RefreshUI()
        {
            this.m_lbItemName.Text = LocalizeModule.Instance.GetString(this.m_itemConf.name);
            this.m_lbItemDesc.Text = LocalizeModule.Instance.GetString(this.m_itemConf.description);
            this.m_imgItemIcon.Sprite = this.m_itemConf.icon;
            this.m_buyNumLimit = this.m_itemdata.m_limitNumber;
            this.m_buylimitComponent.Visible = this.m_buyNumLimit != -1;
            this.m_lbBuyMaxLimit.Visible = this.m_buyNumLimit != -1;

            if (m_itemdata.m_limitNumber < 0)
                this.m_buyNumLimit = 9999;

            this.m_lbBuyMaxLimit.Text = this.m_buyNumLimit.ToString();

            switch (this.m_itemdata.m_costType)
            {
                case CostType.CostCash:
                    this.m_imgMoneyTypeIcon.Sprite = GameConst.CASH_ICON;
                    break;
                case CostType.CostCoin:
                    this.m_imgMoneyTypeIcon.Sprite = GameConst.COIN_ICON;
                    break;
            }

            CurrentBuyNum = 1;
            CurrentPropNum = m_itemdata.m_number;
            if (m_itemdata.m_hasDis)
            {
                this.m_lbDiscount.Text = string.Format("-{0}%", (100 - m_itemdata.m_disCount));
                this.m_lbDiscount.Visible = true;

                //this.m_imgDiscountFlag.Sprite = this.m_itemInfo.MarketItems.DiscountInfo.DiscountIcon;

                m_imgDiscountFlag.Visible = true;
            }
            else
            {
                this.m_lbDiscount.Visible = false;
                this.m_imgDiscountFlag.Visible = false;
            }
        }

        protected override void OnBtnMinusBuyNumberClick(GameObject btnMinus, float time)
        {
            EngineCoreEvents.AudioEvents.PlayAudio.SafeInvoke(Audio.AudioType.UISound, GameCustomAudioKey.shop_add.ToString());

            if (CurrentBuyNum <= 1)
                return;
            else
                CurrentBuyNum--;
        }
    }
}

