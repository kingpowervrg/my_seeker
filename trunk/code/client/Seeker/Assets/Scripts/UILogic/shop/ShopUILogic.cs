//#define Test
using EngineCore;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SeekerGame
{
    [UILogicHandler(UIDefine.UI_SHOP)]
    public class ShopUILogic : BasePageTweenUILogic
    {
        private readonly float[] scrollY = { -22.4f, 0f }; //{ -15.5f,7.5f };
        private readonly float[] m_EffectMaskY = { -21.33f, 1.8f };//{ 43f, 76f };
        private readonly float[] scrollH = { 509.8f, 465f };//{ 380f, 320f };
        private readonly float[] m_EffectMaskHeigh = { 560.3f, 510.2034f };//{ 422f, 351f };

        private GameUIComponent m_panel_down;
        private TweenScale m_tweenPos = null;

        private GameScrollView m_scroll;

        private GameButton m_reflash_btn;
        private GameLabel m_tradingTime_lab;
        private GameLabel m_reflashCount_lab;
        private GameLabel m_price_lab;

        private GameLoopUIContainer<ShopItemUIComponent> m_grid_con;

        private ShopInfoUIComponent m_shopInfo_com;
        private GameUIEffect m_ChooseEffect;
        private GameLabel m_nothingTips = null;

        private ShopType m_curType = ShopType.Prop;
        private ShopType m_lastType = ShopType.None;

        private static ShopPropData m_propdata;
        private static ShopPropData m_blackdata;

        private Transform m_Mask;
        private Transform m_MaskRoot;

        private long m_curChooseID = -1;

        private bool isFirst = true;

        protected override void InitPageBtnStr()
        {
            base.InitPageBtnStr();
            m_pageStr = "Panel_down:leftBtn:";
            m_pageBtnName = new string[] { "btnProp", "btnBlackMarket" };
            m_toggleName = new string[] { LocalizeModule.Instance.GetString("access_to_shop"), LocalizeModule.Instance.GetString("access_to_blackmarket") };
        }

        protected override void InitController()
        {
            base.InitController();

            m_panel_down = this.Make<GameUIComponent>("Panel_down");
            this.m_tweenPos = this.m_panel_down.GetComponent<TweenScale>();

            m_grid_con = Make<GameLoopUIContainer<ShopItemUIComponent>>("Panel_down:Panel_prop:grid");

            m_tradingTime_lab = Make<GameLabel>("Panel_down:downTime");
            m_reflash_btn = m_tradingTime_lab.Make<GameButton>("btn_refresh");
            m_price_lab = m_reflash_btn.Make<GameLabel>("Text_price");

            m_shopInfo_com = Make<ShopInfoUIComponent>("Panel_down:detail");
            m_scroll = Make<GameScrollView>("Panel_down:Panel_prop");
            m_ChooseEffect = Make<GameUIEffect>("UI_xuanzhong_shangcheng");
            m_MaskRoot = Transform.Find("Panel_down");
            m_Mask = Transform.Find("Panel_down/SpriteMask");
            this.m_nothingTips = Make<GameLabel>("nothingTips");
            this.m_nothingTips.Text = LocalizeModule.Instance.GetString("shop_h_nothing");
            this.m_nothingTips.Visible = false;
        }


        protected override void OnInit()
        {
            base.OnInit();
            NeedUpdateByFrame = true;
        }

        protected override void InitListener()
        {
            base.InitListener();

            MessageHandler.RegisterMessageHandler(MessageDefine.MarketResponse, OnResponse);
            MessageHandler.RegisterMessageHandler(MessageDefine.MarkeFreshResponse, OnResponse);
            GameEvents.BuyEvent.OnShopRes += OnBuyCallback;
            GameEvents.UIEvents.UI_Shop_Event.OnChooseItem += chooseItem;
            GameEvents.BuyEvent.OnShopReq += OnRequestCallback;
            m_reflash_btn.AddClickCallBack(BtnReflash);
        }
        protected override void RemoveListener()
        {
            base.RemoveListener();
            MessageHandler.UnRegisterMessageHandler(MessageDefine.MarketResponse, OnResponse);
            MessageHandler.UnRegisterMessageHandler(MessageDefine.MarkeBuyRequest, OnResponse);
            MessageHandler.UnRegisterMessageHandler(MessageDefine.MarkeFreshResponse, OnResponse);
            GameEvents.BuyEvent.OnShopRes -= OnBuyCallback;
            GameEvents.BuyEvent.OnShopReq -= OnRequestCallback;
            GameEvents.UIEvents.UI_Shop_Event.OnChooseItem -= chooseItem;
            m_reflash_btn.RemoveClickCallBack(BtnReflash);
        }

        //public override void OnShow(object param)
        //{

        //    if (param != null)
        //    {
        //        m_propdata = (ShopPropData)param;
        //    }

        //    base.OnShow(param);



        //    MainPanelInGameUILogic.Show();
        //    SetCloseBtnID("Button_close");




        //    m_ChooseEffect.EffectPrefabName = "UI_xuanzhong_shangcheng.prefab";
        //}

        public override void OnShow(object param)
        {
            m_propdata = ShopHelper.getShopData(Category.Common);
            m_blackdata = ShopHelper.getShopData(Category.Black);

            base.OnShow(param);

            MainPanelInGameUILogic.Show();
            SetCloseBtnID("Button_close");




            m_ChooseEffect.EffectPrefabName = "UI_xuanzhong_shangcheng.prefab";
        }

        public override void OnHide()
        {
            base.OnHide();
            MainPanelInGameUILogic.Hide();
            this.m_ChooseEffect.Visible = false;

            m_curType = ShopType.Prop;
            m_lastType = ShopType.None;


        }

        protected override void OnPageChangeClick(int i)
        {
            base.OnPageChangeClick(i);

            changePageType(i, true);
        }

        protected override void OnPageChangeCanel(int i)
        {
            base.OnPageChangeCanel(i);
        }


        private void changePageType(int i, bool value)
        {
            if (value)
            {
                if (ShopType.None != m_lastType)
                    EngineCoreEvents.AudioEvents.PlayAudio.SafeInvoke(Audio.AudioType.UISound, EngineCommonAudioKey.table_change.ToString());
                m_curType = (ShopType)i;
                if (m_lastType == m_curType)
                {
                    return;
                }
                m_ChooseEffect.gameObject.transform.SetParent(m_MaskRoot, false);
                //m_ChooseEffect.Visible = false;
                m_lastType = m_curType;
                if (m_curType == ShopType.BlackMarket)
                {
                    GameEvents.UI_Guid_Event.OnOnpenFrame.SafeInvoke(UIDefine.UI_SHOP, "btnBlackMarket");
                    if (m_blackdata != null && !m_blackdata.IsDirty)
                        ReflashShop(m_blackdata);
                    else
                        requestShopData(false, Category.Black);

                }

            }
            ChangePage(value, i);

        }

        private void ChangePage(bool value, int i)
        {
            if (m_curType == ShopType.None)
            {
                return;
            }
            if (value)
            {
                int curTypeIndex = (int)m_curType;
                m_scroll.Widget.anchoredPosition = m_scroll.Widget.anchoredPosition.x * Vector2.right + scrollY[curTypeIndex] * Vector2.up;
                //m_scrollBar.Y = scrollY[curTypeIndex];
                m_scroll.Widget.sizeDelta = new Vector2(m_scroll.Widget.sizeDelta.x, scrollH[curTypeIndex]);
                //m_scrollBar.Widget.sizeDelta = new Vector2(m_scrollBar.Widget.sizeDelta.x, scrollH[curTypeIndex]);
                m_Mask.localScale = new Vector3(m_Mask.localScale.x, m_EffectMaskHeigh[curTypeIndex]);
                m_Mask.localPosition = new Vector3(m_Mask.localPosition.x, m_EffectMaskY[curTypeIndex]);
                m_grid_con.Widget.anchoredPosition = new Vector2(m_grid_con.Widget.anchoredPosition.x, 0f);
                if (m_curType == ShopType.Prop)
                {
                    m_tradingTime_lab.Visible = false;
                    ReflashShop(m_propdata);
                    UserBehaviorStatisticsModules.Instance.LogEvent(UBSEventKeyName.shop_in, 1.0f, null);
                }
                else if (m_curType == ShopType.BlackMarket)
                {
                    m_tradingTime_lab.Visible = true;
                    UserBehaviorStatisticsModules.Instance.LogEvent(UBSEventKeyName.blackmarket_in, 1.0f, null);
                }
            }
        }

        private void ReflashShop(ShopPropData propdata)
        {

            if (propdata == null || m_curType == ShopType.None)
            {
                m_ChooseEffect.Visible = false;
                return;
            }
            //m_grid_con.Clear();
            int count = propdata.m_itemdata.Count;
            if (0 == count)
            {
                m_shopInfo_com.Visible = false;
                m_ChooseEffect.Visible = false;
                this.m_nothingTips.Visible = true;
            }
            else
            {
                m_shopInfo_com.Visible = true;
                this.m_nothingTips.Visible = false;
            }
            m_grid_con.EnsureSize(count);
            for (int i = 0; i < count; i++)
            {
                ShopItemUIComponent item = m_grid_con.GetChild(i);
                item.setItemData(propdata.m_itemdata[i], m_shopInfo_com, m_curType, 0 == i);
                item.Visible = true;
            }
            if (m_curType == ShopType.BlackMarket)
            {
                m_price_lab.Text = propdata.m_reflashCost.ToString();

            }
        }

        private void OnResponse(object msg)
        {
            if (msg == null)
            {
                return;
            }
            if (msg is MarketResponse)
            {
                MarketResponse res = (MarketResponse)msg;
                if (res.Category == Category.Black)
                {
                    m_ChooseEffect.gameObject.transform.SetParent(m_MaskRoot, false);
                    m_blackdata = ShopHelper.getShopData(res);
                    m_NeedReflash = true;
                    System.DateTime dt = CommonTools.TimeStampToDateTime(m_blackdata.m_NextReflashTime);
                    m_TotalSecond = (float)(dt - System.DateTime.Now).TotalSeconds + 2;
                    m_curTime = Time.time;
                    m_tradingTime_lab.Text = CommonTools.SecondToStringDDMMSS(m_TotalSecond);
                    ReflashShop(m_blackdata);
                }
                else if (res.Category == Category.Common)
                {
                    m_ChooseEffect.gameObject.transform.SetParent(m_MaskRoot, false);
                    m_propdata = ShopHelper.getShopData((res));
                    ReflashShop(m_propdata);
                }
            }
            else if (msg is MarkeFreshResponse)
            {
                MarkeFreshResponse res = (MarkeFreshResponse)msg;
                if (res.ResponseStatus == null)
                {
                    m_ChooseEffect.gameObject.transform.SetParent(m_MaskRoot, false);
                    GlobalInfo.MY_PLAYER_INFO.ChangeCash(-m_blackdata.m_reflashCost);

                    MarketResponse shopRes = new MarketResponse();
                    shopRes.MarketBlack = res.MarketBlack;
                    shopRes.MarketItems.Clear();
                    shopRes.MarketItems.Add(res.MarketItems);
                    shopRes.Category = Category.Black;
                    m_blackdata = ShopHelper.getShopData(shopRes);
                    ReflashShop(m_blackdata);
                }
                else
                {
                    ResponseStatus resStatus = res.ResponseStatus;
                    PopUpManager.OpenNormalOnePop("shop_limite_times");
                }

            }
        }

        public void RefreshPageByCacheData(Category cat)
        {
            if (cat == Category.Black)
            {
                m_ChooseEffect.gameObject.transform.SetParent(m_MaskRoot, false);
                m_blackdata = ShopHelper.getShopData(cat);
                m_NeedReflash = true;
                System.DateTime dt = CommonTools.TimeStampToDateTime(m_blackdata.m_NextReflashTime);
                m_TotalSecond = (float)(dt - System.DateTime.Now).TotalSeconds + 2;
                m_curTime = Time.time;
                m_tradingTime_lab.Text = CommonTools.SecondToStringDDMMSS(m_TotalSecond);
                ReflashShop(m_blackdata);
            }
            else if (cat == Category.Common)
            {
                m_ChooseEffect.gameObject.transform.SetParent(m_MaskRoot, false);
                m_propdata = ShopHelper.getShopData((cat));
                ReflashShop(m_propdata);
            }
        }


        private void requestShopData(bool isAsyn, Category cat)
        {

            MarketRequest request = new MarketRequest();
            request.Category = cat;

            GameEvents.NetWorkEvents.SendAsyncMsg.SafeInvoke(request);

        }

        private long request_ID;
        private int request_Market_Count;
        private int total_prop_count;
        private ShopType request_Type;

        private void OnRequestCallback(long id, int market_count, int total_prop_count_, ShopType type)
        {
            request_ID = id;
            request_Market_Count = market_count;
            this.total_prop_count = total_prop_count_;
            request_Type = type;
        }
        /// <summary>
        /// 购买回调
        /// </summary>
        /// <param name="res"></param>
        private void OnBuyCallback(MarkeBuyResponse res)
        {
            if (request_Type != m_curType)
            {
                return;
            }
            if (res.ResponseStatus != null)
            {
                PopUpManager.OpenNormalOnePop("shop_rmb_no");
                requestShopData();
            }
            else
            {
                if (request_ID == m_curChooseID)
                {
                    request_Type = ShopType.None;
                    ShopItemData curItemdata = getShopDataByID();
                    if (curItemdata != null)
                    {
                        //PopUpManager.OpenNormalOnePop("shop_rmb_ok");
                        ConfProp prop = ConfProp.Get(curItemdata.m_prop.id);
                        if (prop != null && prop.type != 3)
                        {
                            ResultWindowData windowDatas = new ResultWindowData(new List<ResultItemData> { new ResultItemData(curItemdata.m_prop.id, total_prop_count) });
                            FrameMgr.OpenUIParams param = new FrameMgr.OpenUIParams(UIDefine.UI_GIFTRESULT);
                            param.Param = windowDatas;
                            EngineCoreEvents.UIEvent.ShowUIEventWithParam.SafeInvoke(param);
                        }
                        //CommonHelper.OpenGift(curItemdata.m_prop.id, request_Market_Count);
                        if (curItemdata.m_limitNumber > 0)
                        {
                            curItemdata.m_limitNumber -= request_Market_Count;
                        }
                        m_shopInfo_com.setPanel(curItemdata, m_curType);
                        int cost = 0;
                        if (curItemdata.m_hasDis)
                        {
                            cost = curItemdata.m_disPrice * request_Market_Count;
                        }
                        else
                        {
                            cost = curItemdata.m_oriPrice * request_Market_Count;
                        }
                        GlobalInfo.MY_PLAYER_INFO.AddSingleBagInfo(curItemdata.m_prop.id, total_prop_count);

                        GameEvents.UIEvents.UI_GameEntry_Event.Listen_OnCombinePropCollected.SafeInvoke();

                        if (curItemdata.m_costType == CostType.CostCash)
                        {
                            GlobalInfo.MY_PLAYER_INFO.ChangeCash(-cost);
                        }
                        else if (curItemdata.m_costType == CostType.CostCoin)
                        {
                            GlobalInfo.MY_PLAYER_INFO.ChangeCoin(-cost);
                        }
                        Dictionary<UBSParamKeyName, object> _params = new Dictionary<UBSParamKeyName, object>()
                            {
                                        { UBSParamKeyName.ContentID, curItemdata.marketID},
                                        { UBSParamKeyName.NumItems,request_Market_Count}
                            };

                        if (m_curType == ShopType.Prop)
                        {
                            UserBehaviorStatisticsModules.Instance.LogEvent(UBSEventKeyName.shop_buy, null, _params);
                        }

                        if (curItemdata.m_limitNumber == 0 && m_curType == ShopType.Prop)
                        {
                            requestShopData();
                        }
                        else if (m_curType == ShopType.BlackMarket)
                        {
                            requestShopData();
                            UserBehaviorStatisticsModules.Instance.LogEvent(UBSEventKeyName.blackmarket_buy, null, _params);
                        }
                        //GlobalInfo.MY_PLAYER_INFO.SetCoin(res.PlayerBaseInfo.Coin);
                    }
                }
            }

        }

        private ShopItemData getShopDataByID()
        {
            if (m_curChooseID < 0)
            {
                return null;
            }
            if (m_curType == ShopType.BlackMarket)
            {
                return ShopHelper.getShopDataByID(m_blackdata, m_curChooseID);
            }
            else if (m_curType == ShopType.Prop)
            {
                return ShopHelper.getShopDataByID(m_propdata, m_curChooseID);
            }
            return null;
        }

        private void chooseItem(long id, Transform itemTrans)
        {
            m_curChooseID = id;
            m_ChooseEffect.gameObject.transform.SetParent(itemTrans, false);
            m_ChooseEffect.gameObject.transform.localPosition = Vector3.zero;
            m_ChooseEffect.Visible = true;
        }

        //请求网络数据
        private void requestShopData()
        {
            if (m_curType == ShopType.Prop)
            {
                requestShopData(false, Category.Common);
            }
            else if (m_curType == ShopType.BlackMarket)
            {
                requestShopData(false, Category.Black);
            }
        }

        private void NetRequestReflash()
        {
            if (m_blackdata.m_reflashCost > GlobalInfo.MY_PLAYER_INFO.Cash)
            {
                PopUpManager.OpenCashBuyError();
                return;
            }
            MarkeFreshRequest refreshReq = new MarkeFreshRequest();
#if !NETWORK_SYNC || UNITY_EDITOR
            GameEvents.NetWorkEvents.SendHalfSyncMsg.SafeInvoke(refreshReq);
#else
            GameEvents.NetWorkEvents.SendMsg.SafeInvoke(refreshReq);
#endif


#if Test

#else
#endif
        }

        private void BtnReflash(GameObject obj)
        {
            EngineCoreEvents.AudioEvents.PlayAudio.SafeInvoke(Audio.AudioType.UISound, GameCustomAudioKey.shop_refresh.ToString());
            UserBehaviorStatisticsModules.Instance.LogEvent(UBSEventKeyName.blackmarket_refresh, 1.0f, null);
            NetRequestReflash();
        }

        //private float m_CurrentReflashTime = 0;
        private float m_TotalSecond = 0;
        private bool m_NeedReflash = false;
        private float m_curTime = 0f;

        public override void Update()
        {
            base.Update();
            if (m_curType == ShopType.BlackMarket && m_NeedReflash && m_TotalSecond > 0)
            {
                if (Time.time - m_curTime >= 1f)
                {
                    m_TotalSecond = m_TotalSecond - 1f;
                    if (m_TotalSecond <= 0f)
                    {
                        m_NeedReflash = false;
                        m_curTime = 0f;
                        requestShopData(false, Category.Black);
                        //NetRequestReflash();
                    }
                    m_tradingTime_lab.Text = CommonTools.SecondToStringDDMMSS(m_TotalSecond);
                    m_curTime = Time.time;
                }

            }
        }

        public override FrameDisplayMode UIFrameDisplayMode => FrameDisplayMode.FULLSCREEN;
    }
}


