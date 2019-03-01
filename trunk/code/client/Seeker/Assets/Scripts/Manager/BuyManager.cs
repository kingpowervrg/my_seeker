//#define Test
using EngineCore;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SeekerGame
{
    public class BuyManager : Singleton<BuyManager>
    {

        public BuyManager()
        {
            MessageHandler.RegisterMessageHandler(MessageDefine.MarkeBuyResponse, MarkeBuyResponse);
        }

        public override void Destroy()
        {
            base.Destroy();
            MessageHandler.UnRegisterMessageHandler(MessageDefine.MarkeBuyResponse, MarkeBuyResponse);
        }

        public void PopUpBuyConfirm(long marketID, int market_count, int prop_count, ShopType type, int cost)
        {
            System.Action act = () =>
            {
                GameEvents.BuyEvent.OnShopReq.SafeInvoke(marketID, market_count, market_count * prop_count, type);
#if Test
            MarkeBuyResponse res = new global::MarkeBuyResponse();
            res.ResponseStatus = new ResponseStatus();
            res.ResponseStatus.Code = 0;
            MarkeBuyResponse(res);
#else
                MarkeBuyRequest req = new MarkeBuyRequest();
                req.MarketItemId = (int)marketID;
                req.Count = market_count;
                GameEvents.NetWorkEvents.SendMsg.SafeInvoke(req);
#endif
            };

            PopUpData pd = new PopUpData();
            pd.title = "goods_buy";
            pd.content = "shop_now_buying";
            pd.twoStr = "shop_no";
            pd.isOneBtn = false;
            pd.oneAction = act;
            PopUpManager.OpenPopUp(pd);


        }

        public void ShopBuy(long marketID, int market_count, int prop_count, ShopType type, int cost, CostType costType)
        {
            if (costType == CostType.CostCash)
            {
                EngineCoreEvents.AudioEvents.PlayAudio.SafeInvoke(Audio.AudioType.UISound, GameCustomAudioKey.shop_buycash.ToString());
                if (GlobalInfo.MY_PLAYER_INFO.Cash < cost)
                {
                    //PopUpManager.OpenCashBuyError();
                    PopUpManager.OpenGoToCashShop();
                    return;
                }

                if (GlobalInfo.Enable_Purchase)
                {
                    PopUpBuyConfirm(marketID, market_count, prop_count, type, cost);
                    return;
                }

            }
            else if (costType == CostType.CostCoin)
            {
                EngineCoreEvents.AudioEvents.PlayAudio.SafeInvoke(Audio.AudioType.UISound, GameCustomAudioKey.shop_buycoin.ToString());
                if (GlobalInfo.MY_PLAYER_INFO.Coin < cost)
                {
                    //PopUpManager.OpenCoinBuyError();
                    //PopUpManager.OpenGoToCoinShop();
                    PushGiftManager.Instance.TurnOn(ENUM_PUSH_GIFT_BLOCK_TYPE.E_COIN);
                    return;
                }
            }
            GameEvents.BuyEvent.OnShopReq.SafeInvoke(marketID, market_count, market_count * prop_count, type);
#if Test
            MarkeBuyResponse res = new global::MarkeBuyResponse();
            res.ResponseStatus = new ResponseStatus();
            res.ResponseStatus.Code = 0;
            MarkeBuyResponse(res);
#else
            MarkeBuyRequest req = new MarkeBuyRequest();
            req.MarketItemId = (int)marketID;
            req.Count = market_count;
            GameEvents.NetWorkEvents.SendMsg.SafeInvoke(req);
#endif

        }

        private void MarkeBuyResponse(object obj)
        {
            if (obj is MarkeBuyResponse)
            {
                MarkeBuyResponse res = (MarkeBuyResponse)obj;

                GameEvents.BuyEvent.OnShopRes.SafeInvoke(res);
            }
        }
    }
}


