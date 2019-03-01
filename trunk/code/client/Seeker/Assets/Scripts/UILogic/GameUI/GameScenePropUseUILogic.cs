using System;
using System.Collections.Generic;
using EngineCore;
using UnityEngine;

namespace SeekerGame
{
    public class GameScenePropUseUILogic : GameUIComponent
    {
        private GameButton m_buy = null;
        private GameButton m_quit = null;
        private GameButton m_buyOnly = null;

        private GameLabel m_buyLable = null;
        private GameLabel m_contentLable = null;
        private GameLabel m_propCountLable = null;
        private GameImage m_numImg = null;

        public PlayerPropMsg GetCurrentProp()
        {
            return GlobalInfo.MY_PLAYER_INFO.GetBagInfosByID(4);
        }

        private SCMarketItemResponse m_itemRes = null;
        protected override void OnInit()
        {
            base.OnInit();
            this.m_buy = Make<GameButton>("btn_buy");
            this.m_quit = Make<GameButton>("btn_quit");
            this.m_buyOnly = Make<GameButton>("btn_buyOnly");
            this.m_buyLable = this.m_buy.Make<GameLabel>("Text");
            this.m_contentLable = Make<GameLabel>("Image:content");
            this.m_propCountLable = Make<GameLabel>("Image:Button_4:Image:Text");
            this.m_numImg = Make<GameImage>("Image:Button_4:Image");
        }

        public override void OnShow(object param)
        {
            base.OnShow(param);
            this.m_buy.AddClickCallBack(OnUseBtn);
            this.m_quit.AddClickCallBack(OnQuit);
            this.m_buyOnly.AddClickCallBack(OnUseBtn);
            PlayerPropMsg m_Prop = GetCurrentProp();
            if (m_Prop == null)
            {
                this.m_buyLable.Text = LocalizeModule.Instance.GetString("goods_buy");
                this.m_contentLable.Text = LocalizeModule.Instance.GetString("balance_no_time");
                this.m_numImg.Visible = false;
            }
            else
            {
                this.m_buyLable.Text = LocalizeModule.Instance.GetString("goods_use");
                this.m_contentLable.Text = LocalizeModule.Instance.GetString("balance_have_time");
                this.m_propCountLable.Text = m_Prop.Count.ToString();
                this.m_numImg.Visible = true;
            }
            bool twoContinue = GlobalInfo.GAME_NETMODE == GameNetworkMode.Network;//NewGuid.GuidNewManager.Instance.GetProgressByIndex(5);
            this.m_buy.Visible = twoContinue;
            this.m_quit.Visible = twoContinue;
            this.m_buyOnly.Visible = !twoContinue;
            if (GlobalInfo.GAME_NETMODE == GameNetworkMode.Network)
            {
                TimeModule.Instance.SetTimeout(TimeOut, 60f);
            }
        }

        public override void OnHide()
        {
            base.OnHide();
            this.m_buy.RemoveClickCallBack(OnUseBtn);
            this.m_quit.RemoveClickCallBack(OnQuit);
            this.m_buyOnly.RemoveClickCallBack(OnUseBtn);
            GameEvents.Skill_Event.OnSkillFinish -= OnSkillFinish;
            MessageHandler.UnRegisterMessageHandler(MessageDefine.MarkeBuyResponse, OnBuyPropResponse);
            MessageHandler.UnRegisterMessageHandler(MessageDefine.SCMarketItemResponse, OnRes);
            TimeModule.Instance.RemoveTimeaction(TimeOut);
        }

        private void TimeOut()
        {
            OnQuit(null);
            Visible = false;
        }

        private void OnUseBtn(GameObject obj)
        {
            PlayerPropMsg m_Prop =  GetCurrentProp();
            if (m_Prop != null)
            {
                //使用
                OnUseProp();
               
            }
            else //购买
            {
                MessageHandler.RegisterMessageHandler(MessageDefine.SCMarketItemResponse,OnRes);
                CSMarketItemRequest req = new CSMarketItemRequest();
                req.PropId = 4;

#if !NETWORK_SYNC || UNITY_EDITOR
                GameEvents.NetWorkEvents.SendAsyncMsg.SafeInvoke(req);
#else
            GameEvents.NetWorkEvents.SendMsg.SafeInvoke(req);
#endif

                //BuyManager.Instance.ShopBuy(4, CurrentBuyNum, CurrentPropNum, ShopType.Prop, this.m_itemInfo.MarketItems.Cost, this.m_itemInfo.MarketItems.CostType);
            }
        }

        private void OnBuyPropResponse(object res)
        {
            MessageHandler.UnRegisterMessageHandler(MessageDefine.MarkeBuyResponse, OnBuyPropResponse);
            MarkeBuyResponse marketItem = (MarkeBuyResponse)res;
            if (!MsgStatusCodeUtil.OnError(marketItem.ResponseStatus))
            {
                if (this.m_itemRes.MarketItems.CostType == CostType.CostCash)
                {
                    GlobalInfo.MY_PLAYER_INFO.ChangeCash(-this.m_itemRes.MarketItems.Cost);
                }
                else if (this.m_itemRes.MarketItems.CostType == CostType.CostCoin)
                {
                    GlobalInfo.MY_PLAYER_INFO.ChangeCoin(-this.m_itemRes.MarketItems.Cost);
                }
                GlobalInfo.MY_PLAYER_INFO.AddSingleBagInfo(4,1);

                GameEvents.UIEvents.UI_GameEntry_Event.Listen_OnCombinePropCollected.SafeInvoke();

                OnUseProp();

            }
        }

        private void OnRes(object obj)
        {
            if (obj is SCMarketItemResponse)
            {
                m_itemRes = (SCMarketItemResponse)obj;
                MessageHandler.UnRegisterMessageHandler(MessageDefine.SCMarketItemResponse,OnRes);
                MessageHandler.RegisterMessageHandler(MessageDefine.MarkeBuyResponse, OnBuyPropResponse);
                BuyManager.Instance.ShopBuy(this.m_itemRes.MarketItems.Id, 1, 1, ShopType.Prop, this.m_itemRes.MarketItems.Cost, this.m_itemRes.MarketItems.CostType);
            }
        }

        private void OnUseProp()
        {
            GameEvents.Skill_Event.OnSkillFinish += OnSkillFinish;
            GameEvents.MainGameEvents.OnGameStatusChange.SafeInvoke(SceneBase.GameStatus.GAMING);
            GameSkillManager.Instance.OnStartSkill(4);
        }

        private void OnSkillFinish(long Id)
        {
            GameEvents.Skill_Event.OnSkillFinish -= OnSkillFinish;

            //Dictionary<UBSParamKeyName, object> internalBuyItemKeypoint = new Dictionary<UBSParamKeyName, object>();
            //internalBuyItemKeypoint.Add(UBSParamKeyName.PropItem_ID, 4);
            //internalBuyItemKeypoint.Add(UBSParamKeyName.PropItem_Num, 1);
            //UserBehaviorStatisticsModules.Instance.LogEvent(UBSEventKeyName.game_use_propitem, 1, internalBuyItemKeypoint);
            
            GameEvents.UIEvents.UI_Pause_Event.OnClosePauseFrame.SafeInvoke();
            Visible = false;
            GameEvents.SceneEvents.OnScenePropWinVisible.SafeInvoke(false);
        }

        private void OnQuit(GameObject obj)
        {
            TimeModule.Instance.SetTimeout(() => { EngineCoreEvents.UIEvent.HideUIEvent.SafeInvoke(UIDefine.UI_GAME_MAIN); }, 0.1f);
            Visible = false;
            SCSceneRewardResponse msg = new SCSceneRewardResponse();
            msg.SceneId = -1;
            WinFailData data = new WinFailData(ENUM_SEARCH_MODE.E_SEARCH_ROOM, msg);

            FrameMgr.OpenUIParams param = new FrameMgr.OpenUIParams(UIDefine.UI_WIN);

            param.Param = data;

            EngineCoreEvents.UIEvent.ShowUIEventWithParam.SafeInvoke(param);

            Dictionary<UBSParamKeyName, object> _param = new Dictionary<UBSParamKeyName, object>();
            _param.Add(UBSParamKeyName.Success, 0);
            _param.Add(UBSParamKeyName.SceneID, SceneModule.Instance.SceneData.id);

            UserBehaviorStatisticsModules.Instance.LogEvent(UBSEventKeyName.game_finish, null, _param);

            //PushGiftManager.Instance.TurnOn(ENUM_PUSH_GIFT_BLOCK_TYPE.E_SEEK);
        }
    }
}
