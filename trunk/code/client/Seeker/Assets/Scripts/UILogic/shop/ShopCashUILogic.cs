using EngineCore;
using System.Collections.Generic;
using UnityEngine;
namespace SeekerGame
{
    [UILogicHandler(UIDefine.UI_SHOPCOIN)]
    public class ShopCashUILogic : UILogicBase
    {
        protected GameUIContainer m_grid_com;
        private GameButton m_close_btn;

        protected Category cate = Category.Coin;
        protected ShopType shopType = ShopType.Coin;

        protected bool is_fast_buy = false;

        protected override void OnInit()
        {
            base.OnInit();
            m_grid_com = Make<GameUIContainer>("Panel:Panel_prop:grid");
            m_close_btn = Make<GameButton>("Button_close");
            //this.tweener = Transform.GetComponentsInChildren<UITweener>(true);
        }

        public override FrameDisplayMode UIFrameDisplayMode
        {
            get
            {
                return FrameDisplayMode.FULLSCREEN;
            }
        }

        public override void OnShow(object param)
        {
            base.OnShow(param);

            is_fast_buy = false;

            if (null != param)
            {
                is_fast_buy = (bool)param;
            }
            MainPanelInGameUILogic.Show();


            //MessageHandler.RegisterMessageHandler(MessageDefine.MarkeBuyResponse, OnResponse);
            MessageHandler.RegisterMessageHandler(MessageDefine.MarketResponse, OnResponse);
            m_close_btn.AddClickCallBack(btnClose);

            if (!ShopHelper.IsShopDataAvaliable(cate))
                requestData();
            else
            {
                ShopPropData crashdata = ShopHelper.getShopData(cate);
                InitData(crashdata);
            }
        }

        public override void OnHide()
        {
            base.OnHide();
            MainPanelInGameUILogic.Hide();
            //MessageHandler.UnRegisterMessageHandler(MessageDefine.MarkeBuyResponse, OnResponse);
            MessageHandler.UnRegisterMessageHandler(MessageDefine.MarketResponse, OnResponse);
            m_close_btn.RemoveClickCallBack(btnClose);
            m_grid_com.Clear();
        }

        public override void Dispose()
        {
            base.Dispose();
        }

        protected virtual void InitData(ShopPropData propdata)
        {
            if (is_fast_buy)
            {
                List<ShopItemData> temp_datas = propdata.m_itemdata.FindAll((item) => { return 1 == item.m_number && false == item.m_hasDis; });

                ShopPropData temp = new ShopPropData()
                {
                    m_itemdata = temp_datas,
                    m_reflashCount = propdata.m_reflashCount,
                    m_LastReflashTime = propdata.m_LastReflashTime,
                    m_NextReflashTime = propdata.m_NextReflashTime,
                    m_reflashCost = propdata.m_reflashCost,
                };

                propdata = temp;
            }

            this.InitItemView(propdata);
        }

        protected virtual void InitItemView(ShopPropData propdata)
        {
            int count = propdata.m_itemdata.Count;
            m_grid_com.EnsureSize<ShopCoinItemUIComponent>(count);
            for (int i = 0; i < count; i++)
            {
                if (propdata.m_itemdata[i] != null)
                {
                    ShopCoinItemUIComponent com = m_grid_com.GetChild<ShopCoinItemUIComponent>(i);
                    com.setData(propdata.m_itemdata[i], shopType, i, is_fast_buy);
                    com.Visible = true;
                }
            }
        }

        private void requestData()
        {
#if Test
            MarketResponse res = ShopHelper.getShopDataTest(cate);
            OnResponse(res);
#else
            MarketRequest request = new MarketRequest();
            request.Category = cate;
            GameEvents.NetWorkEvents.SendAsyncMsg.SafeInvoke(request);
#endif
        }

        private void OnResponse(object obj)
        {
            if (obj == null)
            {
                return;
            }
            if (obj is MarketResponse)
            {
                var res = (MarketResponse)obj;
                if (res.Category == cate)
                {
                    ShopPropData m_crashdata = ShopHelper.getShopData(res);
                    InitData(m_crashdata);
                }
            }
        }

        private void btnClose(GameObject obj)
        {
            EngineCoreEvents.AudioEvents.PlayAudio.SafeInvoke(Audio.AudioType.UISound, EngineCommonAudioKey.Close_Window.ToString());
            if (cate == Category.Cash)
            {
                EngineCoreEvents.UIEvent.HideUIEvent.SafeInvoke(UIDefine.UI_IAPCASH);
            }
            else if (cate == Category.Vit)
            {
                EngineCoreEvents.UIEvent.HideUIEvent.SafeInvoke(UIDefine.UI_SHOPENERGY);
            }
        }
    }
}


