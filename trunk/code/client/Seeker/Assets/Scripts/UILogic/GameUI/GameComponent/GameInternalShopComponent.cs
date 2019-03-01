using System;
using System.Collections.Generic;
using EngineCore;
using UnityEngine;
using GOEngine;
namespace SeekerGame
{
    /// <summary>
    /// 游戏内部道具购买组件
    /// </summary>
    public class GameInternalShopComponent : GameUIComponent
    {
        protected GameImage m_imgItemIcon = null;
        protected GameLabel m_lbItemName = null;
        protected GameLabel m_lbItemDesc = null;

        protected GameLabel m_lbOriginPrice = null;

        protected GameButton m_btnAddNum = null;
        protected GameButton m_btnMinus = null;
        protected GameLabel m_lbBuyNum = null;
        protected GameLabel m_lbBuyMaxLimit = null;

        protected GameLabel m_lbDiscount = null;
        protected GameImage m_imgDiscountFlag = null;
        protected GameLabel m_lbTotalPriceAfterDiscount = null;
        protected GameButton m_btnBuy = null;
        protected GameButton m_btnClose = null;
        protected GameImage m_imgMoneyTypeIcon = null;
        protected GameUIComponent m_buylimitComponent = null;
        protected GameUIComponent m_originPriceComponent = null;

        protected int m_currentBuyNum = 0;
        private SCMarketItemResponse m_itemInfo = null;
        protected ConfProp m_itemConf = null;
        protected int m_buyNumLimit = 999;

        protected override void OnInit()
        {
            this.m_imgItemIcon = Make<GameImage>("Image_back:Image:icon");
            this.m_lbItemName = Make<GameLabel>("Image_back:Image:title");
            this.m_lbItemDesc = Make<GameLabel>("Image_back:Image:content");

            this.m_btnAddNum = Make<GameButton>("Image_back:Imagenumber:btnAdd");
            this.m_btnMinus = Make<GameButton>("Image_back:Imagenumber:btnReduce");
            this.m_lbBuyNum = Make<GameLabel>("Image_back:Imagenumber:Text");
            this.m_lbBuyMaxLimit = Make<GameLabel>("Image_back:Imagenumber:income:number");
            this.m_buylimitComponent = Make<GameUIComponent>("Image_back:Imagenumber:income");
            this.m_originPriceComponent = Make<GameUIComponent>("Image_back:Text");
            this.m_lbOriginPrice = Make<GameLabel>("Image_back:Text:Text_number");
            this.m_lbDiscount = Make<GameLabel>("Image_back:Image:Image:Text");
            this.m_imgDiscountFlag = Make<GameImage>("Image_back:Image:Image");

            this.m_btnBuy = Make<GameButton>("Image_back:btn_buy");
            this.m_btnClose = Make<GameButton>("Image_back:btn_quit");
            this.m_lbTotalPriceAfterDiscount = Make<GameLabel>("Image_back:btn_buy:Text");
            this.m_imgMoneyTypeIcon = Make<GameImage>("Image_back:btn_buy:Image");

        }

        public override void OnShow(object param)
        {
            base.OnShow(param);

            this.m_btnBuy.AddClickCallBack(OnBtnBuyClick);
            this.m_btnClose.AddClickCallBack(OnBtnExitClick);
            this.m_btnAddNum.AddLongClickCallBack(OnBtnAddBuyNumberClick);
            this.m_btnMinus.AddLongClickCallBack(OnBtnMinusBuyNumberClick);

        }

        public void SetBuyItemInfo(SCMarketItemResponse marketItemInfo)
        {
            this.m_itemInfo = marketItemInfo;
            this.m_itemConf = ConfProp.Get(marketItemInfo.MarketItems.SellId);
            if (this.m_itemConf == null)
                DebugUtil.LogError($"no item:{marketItemInfo.MarketItems.SellId}");

            RefreshUI();
        }

        protected virtual void RefreshUI()
        {
            this.m_lbItemName.Text = LocalizeModule.Instance.GetString(this.m_itemConf.name);
            this.m_lbItemDesc.Text = LocalizeModule.Instance.GetString(this.m_itemConf.description);
            this.m_imgItemIcon.Sprite = this.m_itemConf.icon;
            this.m_buyNumLimit = this.m_itemInfo.MarketItems.BuyLimit;
            this.m_buylimitComponent.Visible = this.m_buyNumLimit != -1;
            this.m_lbBuyMaxLimit.Visible = this.m_buyNumLimit != -1;

            if (this.m_buyNumLimit == -1)
                this.m_buyNumLimit = 9999;

            this.m_lbBuyMaxLimit.Text = this.m_buyNumLimit.ToString();

            switch (this.m_itemInfo.MarketItems.CostType)
            {
                case CostType.CostCash:
                    this.m_imgMoneyTypeIcon.Sprite = GameConst.CASH_ICON;
                    break;
                case CostType.CostCoin:
                    this.m_imgMoneyTypeIcon.Sprite = GameConst.COIN_ICON;
                    break;
            }

            CurrentBuyNum = 1;
            CurrentPropNum = 1;
            if (this.m_itemInfo.MarketItems.DiscountInfo.Discount != 0)
            {
                this.m_lbDiscount.Text = $"{this.m_itemInfo.MarketItems.DiscountInfo.Discount}%";
                this.m_lbDiscount.Visible = true;

                this.m_imgDiscountFlag.Sprite = this.m_itemInfo.MarketItems.DiscountInfo.DiscountIcon;

                m_imgDiscountFlag.Visible = true;
            }
            else
            {
                this.m_lbDiscount.Visible = false;
                this.m_imgDiscountFlag.Visible = false;
            }

        }

        /// <summary>
        /// 道具购买
        /// </summary>
        /// <param name="btnItem"></param>
        protected virtual void OnBtnBuyClick(GameObject btnItem)
        {

            BuyManager.Instance.ShopBuy(this.m_itemInfo.MarketItems.Id, CurrentBuyNum, CurrentPropNum, ShopType.Prop, this.m_itemInfo.MarketItems.Cost, this.m_itemInfo.MarketItems.CostType);

            //MarkeBuyRequest req = new MarkeBuyRequest();
            //req.MarketItemId = this.m_itemInfo.MarketItems.Id;
            //req.Count = CurrentBuyNum;

            //GameEvents.NetWorkEvents.SendMsg.SafeInvoke(req);

            MessageHandler.RegisterMessageHandler(MessageDefine.MarkeBuyResponse, OnBuyPropResponse);


        }

        /// <summary>
        /// 局内道具购买回调
        /// </summary>
        /// <param name="buyResultMsg"></param>
        protected virtual void OnBuyPropResponse(object buyResultMsg)
        {
            MessageHandler.UnRegisterMessageHandler(MessageDefine.MarkeBuyResponse, OnBuyPropResponse);
            MarkeBuyResponse resp = buyResultMsg as MarkeBuyResponse;
            if (!MsgStatusCodeUtil.OnError(resp.ResponseStatus))
            {
                if (resp.Props != null || resp.Props.Count > 0)
                {
                    long itemID = resp.Props.PropId;
                    int itemNum = resp.Props.Count;

                    GlobalInfo.MY_PLAYER_INFO.AddSingleBagInfo(itemID, itemNum);

                    GameEvents.UIEvents.UI_GameEntry_Event.Listen_OnCombinePropCollected.SafeInvoke();

                    (LogicHandler as GameMainUILogic).RefreshPlayerPropItems();
                    (LogicHandler as GameMainUILogic).UseItemAfterBuy(itemID);

                    {
                        Dictionary<UBSParamKeyName, object> internalBuyItemKeypoint = new Dictionary<UBSParamKeyName, object>();
                        internalBuyItemKeypoint.Add(UBSParamKeyName.Description, UBSDescription.PROPBUY);
                        internalBuyItemKeypoint.Add(UBSParamKeyName.PropItem_ID, itemID);
                        internalBuyItemKeypoint.Add(UBSParamKeyName.PropItem_Num, itemNum);
                        UserBehaviorStatisticsModules.Instance.LogEvent(UBSEventKeyName.game_propbuy, itemNum, internalBuyItemKeypoint);
                    }

                    if (4 == itemID)
                    {
                        ((GameMainUILogic)LogicHandler).Buy_time_prop_count += itemNum;
                    }
                }
            }
            GameEvents.UIEvents.UI_Pause_Event.OnClosePauseFrame.SafeInvoke();
            if (CachedVisible)
                Visible = false;
        }

        private void OnBtnAddBuyNumberClick(GameObject btnAdd, float time)
        {
            EngineCoreEvents.AudioEvents.PlayAudio.SafeInvoke(Audio.AudioType.UISound, GameCustomAudioKey.shop_add.ToString());

            if (CurrentBuyNum >= this.m_buyNumLimit)
                return;
            else
                CurrentBuyNum++;
        }

        protected virtual void OnBtnMinusBuyNumberClick(GameObject btnMinus, float time)
        {
            EngineCoreEvents.AudioEvents.PlayAudio.SafeInvoke(Audio.AudioType.UISound, GameCustomAudioKey.shop_add.ToString());

            if (CurrentBuyNum <= 0)
                return;
            else
                CurrentBuyNum--;
        }

        protected virtual void OnBtnExitClick(GameObject btnExit)
        {
            EngineCoreEvents.AudioEvents.PlayAudio.SafeInvoke(Audio.AudioType.UISound, EngineCommonAudioKey.Close_Window.ToString());

            if (CachedVisible)
                Visible = false;
            GameEvents.UIEvents.UI_Pause_Event.OnClosePauseFrame.SafeInvoke();
        }

        protected virtual int CurrentBuyNum
        {
            get { return this.m_currentBuyNum; }
            set
            {
                this.m_currentBuyNum = value;
                this.m_lbBuyNum.Text = value.ToString();
                this.m_lbOriginPrice.Text = $"{ this.m_itemInfo.MarketItems.Cost * value}";
                this.m_originPriceComponent.Visible = this.m_itemInfo.MarketItems.DiscountInfo.DiscountCost != 0;
                if (this.m_itemInfo.MarketItems.DiscountInfo.DiscountCost != 0)
                    this.m_lbTotalPriceAfterDiscount.Text = $"{ this.m_itemInfo.MarketItems.DiscountInfo.DiscountCost * value}";
                else
                    this.m_lbTotalPriceAfterDiscount.Text = $"{ this.m_itemInfo.MarketItems.Cost * value}";
            }
        }

        protected int CurrentPropNum
        {
            get; set;
        }

        public override void OnHide()
        {
            base.OnHide();

            this.m_btnBuy.RemoveClickCallBack(OnBtnBuyClick);
            this.m_btnClose.RemoveClickCallBack(OnBtnExitClick);
            this.m_btnAddNum.RemoveLongClickCallBack(OnBtnAddBuyNumberClick);
            this.m_btnMinus.RemoveLongClickCallBack(OnBtnMinusBuyNumberClick);
        }
    }
}
