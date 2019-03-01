using EngineCore;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SeekerGame
{
    public enum ShopType
    {
        Prop,
        BlackMarket,
        Coin,
        Vit,
        None
    }

    public class ShopPropData
    {
        public List<ShopItemData> m_itemdata;
        public int m_reflashCount;
        public long m_LastReflashTime;
        public long m_NextReflashTime;
        public int m_reflashCost;

        public bool IsDirty
        {
            get
            {
                if (m_NextReflashTime != 0)
                {
                    System.DateTime dt = CommonTools.TimeStampToDateTime(m_NextReflashTime);
                    long elapsedTime = (long)((dt - System.DateTime.Now).TotalSeconds + 2);

                    return elapsedTime <= 5;
                }

                return false;
            }
        }
    }

    public class ShopItemData
    {
        public long marketID;
        public ConfProp m_prop;
        public bool m_hasDis;
        public int m_oriPrice; //原价
        public int m_disPrice; //折扣后
        public int m_limitNumber; //限购次数
        public int m_maxNumber; //最大限购次数
        public int m_disCount; //折扣
        public string m_disIcon;
        public int m_number; //数量
        public CostType m_costType;
    }

    //public class ShopAllData
    //{
    //    public ShopPropData m_propdata;
    //    public ShopPropData m_blackdata;
    //}

    public static class ShopHelper
    {
        private static Dictionary<Category, ShopPropData> ShopDataCache = new Dictionary<Category, ShopPropData>();


        static ShopHelper()
        {
            MessageHandler.RegisterMessageHandler(MessageDefine.MarketResponse, LoadShopData);
        }

        public static void openShop(MarketResponse shopdata)
        {
            ShopPropData propData = getShopData(shopdata);
            FrameMgr.OpenUIParams param = new FrameMgr.OpenUIParams(UIDefine.UI_SHOP);
            param.Param = propData;
            EngineCoreEvents.UIEvent.ShowUIEventWithParam.SafeInvoke(param);
        }


        public static void OpenShop(Category shopCategory)
        {
            ShopPropData shopData;
            if (ShopDataCache.TryGetValue(shopCategory, out shopData))
            {
                FrameMgr.OpenUIParams param = new FrameMgr.OpenUIParams(UIDefine.UI_SHOP);
                param.Param = shopData;
                EngineCoreEvents.UIEvent.ShowUIEventWithParam.SafeInvoke(param);
            }
            else
                Debug.LogError($"no shop {shopCategory.ToString()} data");
        }

        static void LoadShopData(object obj)
        {
            if (obj is MarketResponse)
            {
                MarketResponse shopdata = obj as MarketResponse;
                getShopData(shopdata);
            }
        }

        public static void RefreshAllShopData()
        {
            RefreshCommonShopData();
            TimeModule.Instance.SetTimeout(RefreshVitShopData, 1.0f);
            TimeModule.Instance.SetTimeout(RefreshBlackShopData, 0.5f);
        }


        static void RefreshCommonShopData()
        {
            MarketRequest req = new MarketRequest();
            req.Category = Category.Common;
            GameEvents.NetWorkEvents.SendAsyncMsg.SafeInvoke(req);
        }

        static void RefreshBlackShopData()
        {
            MarketRequest req = new MarketRequest();
            req.Category = Category.Black;
            GameEvents.NetWorkEvents.SendAsyncMsg.SafeInvoke(req);
        }

        static void RefreshVitShopData()
        {
            MarketRequest req = new MarketRequest();
            req.Category = Category.Vit;
            GameEvents.NetWorkEvents.SendAsyncMsg.SafeInvoke(req);
        }

        public static ShopPropData getShopData(MarketResponse shopdata)
        {
            if (shopdata == null)
            {
                return null;
            }
            ShopPropData pdata = new ShopPropData();
            pdata.m_itemdata = new List<ShopItemData>();
            List<ShopItemData> m_giftData = new List<ShopItemData>(); //礼盒
            if (shopdata.Category == Category.Black && shopdata.MarketBlack != null)
            {
                pdata.m_reflashCost = shopdata.MarketBlack.FreshCost;
                pdata.m_reflashCount = shopdata.MarketBlack.FreshLimit;
                pdata.m_LastReflashTime = shopdata.MarketBlack.LastUpdateTime * 10000;
                pdata.m_NextReflashTime = shopdata.MarketBlack.NextUpdateTime * 10000;
            }
            for (int i = 0; i < shopdata.MarketItems.Count; i++)
            {
                ShopItemData itemdata = new ShopItemData();
                MarketItemMsg markitem = shopdata.MarketItems[i];
                if (markitem != null)
                {
                    ConfProp prop = ConfProp.Get(markitem.SellId);
                    itemdata.m_number = markitem.SellCount;
                    itemdata.marketID = markitem.Id;
                    itemdata.m_oriPrice = markitem.Cost;
                    itemdata.m_limitNumber = markitem.BuyLimit;
                    itemdata.m_maxNumber = markitem.MaxLimit;
                    itemdata.m_costType = markitem.CostType;
                    if (markitem.DiscountInfo != null)
                    {
                        System.DateTime startTime = CommonTools.TimeStampToDateTime(markitem.DiscountInfo.DiscountStart * 10000);
                        System.DateTime endTime = CommonTools.TimeStampToDateTime(markitem.DiscountInfo.DiscountEnd * 10000);
                        System.DateTime nowTime = CommonTools.GetCurrentTime();
                        if (markitem.DiscountInfo.Discount == 0 || nowTime < startTime || nowTime > endTime)
                        {
                            itemdata.m_hasDis = false;
                        }
                        else
                        {
                            itemdata.m_hasDis = true;
                            itemdata.m_disPrice = markitem.DiscountInfo.DiscountCost;
                            itemdata.m_disCount = markitem.DiscountInfo.Discount;
                            itemdata.m_disIcon = markitem.DiscountInfo.DiscountIcon;
                        }

                    }
                    else
                    {
                        itemdata.m_hasDis = false;
                    }
                    if (prop != null)
                    {
                        itemdata.m_prop = prop;
                        //todo 临时修改礼盒最前面
                        if (itemdata.m_prop.type == (int)PROP_TYPE.E_GIFT)
                        {
                            m_giftData.Add(itemdata);
                        }
                        else
                        {
                            pdata.m_itemdata.Add(itemdata);
                        }
                    }
                    else
                    {
                        Debug.LogError("item id is not exist:" + markitem.SellId);
                    }


                }
            }
            pdata.m_itemdata.InsertRange(0, m_giftData);

            ShopDataCache[shopdata.Category] = pdata;

            return pdata;
        }

        public static ShopPropData getShopData(Category cat)
        {
            return ShopDataCache.ContainsKey(cat) ? ShopDataCache[cat] : null;
        }

        public static MarketResponse getShopDataTest(Category cat)
        {
            MarketResponse res = new MarketResponse();
            for (int i = 0; i < 30; i++)
            {
                MarketItemMsg item = new MarketItemMsg();
                item.SellId = 1000 + i;
                item.Id = 100 + i;
                item.CostType = CostType.CostCash;
                item.SellCount = Random.Range(0, 50);
                item.Cost = Random.Range(0, 1000000);
                item.BuyLimit = 10 + i;
                if (i % 4 != 0)
                {
                    float zz = (float)(i + 1) / 100f;
                    item.DiscountInfo = new DiscountInfo();
                    item.DiscountInfo.Discount = i + 1; //string.Format("{0}%", (i + 1));
                    item.DiscountInfo.DiscountCost = (int)(item.Cost * zz);
                }
                else
                {
                    item.DiscountInfo = null;
                }
                res.MarketItems.Add(item);
            }
            res.Category = cat;
            if (cat == Category.Black)
            {
                res.MarketBlack = new MarketBlack();
                res.MarketBlack.FreshCost = 100;
                res.MarketBlack.FreshLimit = 3;
            }
            return res;
        }

        public static string getCashType(CostType type)
        {
            string str = "icon_mainpanel_cash_2.png";
            if (type == CostType.CostCoin)
            {
                str = "icon_mainpanel_coin_2.png";
            }
            return str;
        }

        public static void buy(int marketID, int count)
        {
            MarkeBuyRequest req = new MarkeBuyRequest();
            req.MarketItemId = marketID;
            req.Count = count;
            GameEvents.NetWorkEvents.SendMsg.SafeInvoke(req);
        }

        public static ShopItemData getShopDataByID(ShopPropData pdata, long id)
        {
            for (int i = 0; i < pdata.m_itemdata.Count; i++)
            {
                ShopItemData itemdata = pdata.m_itemdata[i];
                if (itemdata.marketID == id)
                {
                    return itemdata;
                }
            }
            return null;
        }

        /// <summary>
        /// 商店数据是否可用
        /// </summary>
        /// <param name="shopCategory"></param>
        /// <returns></returns>
        public static bool IsShopDataAvaliable(Category shopCategory)
        {
            ShopPropData shopData;
            if (ShopDataCache.TryGetValue(shopCategory, out shopData))
                return !shopData.IsDirty;

            return false;
        }
    }
}


