using EngineCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace SeekerGame
{
    public enum SlotPageType
    {
        PageCoin,
        PageCash,
        Max
    }

    [UILogicHandler(UIDefine.UI_SLOTS)]
    public class SlotUILogic : UILogicBase
    {
        private readonly string[] m_page_str = new string[] { "Toggle_coin", "Toggle_cash" };
        private GameToggleButton[] m_page_btn;

        private GameImage[] m_itemBg_img;
        private GameImage[] m_itemIcon_img;
        private GameLabel[] m_itemName_lab;
        private GameLabel[] m_itemCount_lab;
        private GameImage[] m_special_root;
        //private GameImage[] m_effect_img;

        private GameLabel m_luckValue_lab;

        private GameButton m_buy_btn;
        private GameLabel m_buyCount_lab;
        private GameImage m_moneyIcon_img;

        private GameButton m_detail_btn;
        private GameButton m_close_btn;

        private GameButton m_detail_root;
        private GameLabel m_detail_lab;

        private SlotValue m_slotValue;

        private GameLabel m_luckText_lab;

        private SlotResultCom m_slotResult_com;

        private long m_lottoID = -1;
        private int m_costValue = 0;
        private readonly int ITEMMAXSIZE = 14;

        private readonly string m_SpecialBG_Str = "db_slot_3.png";
        private readonly string m_NormalBG_Str = "db_slot_4.png";

        private GameUIEffect m_BgEffect01;
        private GameUIEffect m_BgEffect02;
        private GameUIEffect m_ChooseEffect;
        private GameUIEffect m_SpecialEffect;
        private GameUIEffect m_buyEffect;
        private bool m_CanBuy = true;

        private CostType m_cost_type;
        public override FrameDisplayMode UIFrameDisplayMode
        {
            get
            {
                return FrameDisplayMode.FULLSCREEN;
            }
        }
        private void InitController()
        {
            int pageCount = (int)SlotPageType.Max;
            m_page_btn = new GameToggleButton[pageCount];
            for (int i = 0; i < pageCount; i++)
            {
                m_page_btn[i] = Make<GameToggleButton>(string.Format("Panel_animation:pageGroup:{0}", m_page_str[i]));
            }

            m_itemBg_img = new GameImage[ITEMMAXSIZE];
            m_special_root = new GameImage[ITEMMAXSIZE];
            m_itemIcon_img = new GameImage[ITEMMAXSIZE];
            //m_itemName_lab = new GameLabel[ITEMMAXSIZE];
            m_itemCount_lab = new GameLabel[ITEMMAXSIZE];
            //m_effect_img = new GameImage[ITEMMAXSIZE];
            for (int i = 0; i < ITEMMAXSIZE; i++)
            {
                m_itemBg_img[i] = Make<GameImage>(string.Format("Panel_animation:Image_{0}", i + 1));
                m_special_root[i] = m_itemBg_img[i].Make<GameImage>("Image");
                m_special_root[i].Visible = false;
                m_itemIcon_img[i] = m_itemBg_img[i].Make<GameImage>("Image_icon");
                //m_itemName_lab[i] = m_itemBg_img[i].Make<GameLabel>("Text_name");
                m_itemCount_lab[i] = m_itemBg_img[i].Make<GameLabel>("Text_number");
                //m_effect_img[i] = m_itemBg_img[i].Make<GameImage>("effect");
                //m_effect_img[i].Visible = false;
            }
            m_luckValue_lab = Make<GameLabel>("Panel_animation:Text_number");

            m_buy_btn = Make<GameButton>("Panel_animation:btn_play");
            m_buyCount_lab = m_buy_btn.Make<GameLabel>("Text");
            m_moneyIcon_img = m_buy_btn.Make<GameImage>("Image");

            m_detail_btn = Make<GameButton>("Panel_animation:Button_detail");
            m_close_btn = Make<GameButton>("Panel_animation:Button_close");
            m_detail_root = Make<GameButton>("Panel_animation:Image_keywords");
            m_detail_lab = m_detail_root.Make<GameLabel>("maskBtn:Text");
            m_luckText_lab = Make<GameLabel>("Panel_animation:Textdetail");
            m_slotValue = Transform.GetComponent<SlotValue>();
            m_BgEffect01 = Make<GameUIEffect>("UI_choujiang_changjing");
            m_BgEffect02 = Make<GameUIEffect>("UI_choujiang_paomadeng");
            m_ChooseEffect = Make<GameUIEffect>("UI_choujiang_xuanzhong");
            m_SpecialEffect = Make<GameUIEffect>("UI_choujiang_teshuwupin");
            m_buyEffect = m_buy_btn.Make<GameUIEffect>("UI_choujiang_anniu");
            m_slotResult_com = Make<SlotResultCom>("Panel_animation:BalanceWin");
            NeedUpdateByFrame = true;

        }

        private void RegistListener()
        {
            MessageHandler.RegisterMessageHandler(MessageDefine.LottoBuyResponse, OnResponse);
            MessageHandler.RegisterMessageHandler(MessageDefine.LottoResponse, OnResponse);
            m_page_btn[0].AddChangeCallBack(BtnCoinPage);
            m_page_btn[1].AddChangeCallBack(BtnCashPage);
            m_buy_btn.AddClickCallBack(BtnBuy);
            m_detail_btn.AddClickCallBack(BtnDetail);
            m_close_btn.AddClickCallBack(BtnClose);
            m_detail_root.AddClickCallBack(BtnDetail);
            GameEvents.UIEvents.UI_SLots_Events.OnAgain += OnAgain;
            GameEvents.UIEvents.UI_SLots_Events.OnOK += OnOK;
            //GameEvents.UIEvents.UI_SLots_Events.OnResultOpen += OnResultOpen;
        }

        private void UnRegistListener()
        {
            MessageHandler.UnRegisterMessageHandler(MessageDefine.LottoBuyResponse, OnResponse);
            MessageHandler.UnRegisterMessageHandler(MessageDefine.LottoResponse, OnResponse);
            m_page_btn[0].RemoveChangeCallBack(BtnCoinPage);
            m_page_btn[1].RemoveChangeCallBack(BtnCashPage);
            m_buy_btn.RemoveClickCallBack(BtnBuy);
            m_detail_btn.RemoveClickCallBack(BtnDetail);
            m_close_btn.RemoveClickCallBack(BtnClose);
            m_detail_root.RemoveClickCallBack(BtnDetail);
            GameEvents.UIEvents.UI_SLots_Events.OnAgain -= OnAgain;
            GameEvents.UIEvents.UI_SLots_Events.OnOK -= OnOK;
            //GameEvents.UIEvents.UI_SLots_Events.OnResultOpen -= OnResultOpen;
        }

        protected override void OnInit()
        {
            base.OnInit();
            InitController();
        }

        public override void OnShow(object param)
        {
            base.OnShow(param);

            GlobalInfo.Instance.PlayMusic(GameCustomeMusicKey.Event_01.ToString());

            UserBehaviorStatisticsModules.Instance.LogEvent(UBSEventKeyName.lotto_in, 1f, null);

            m_luckText_lab.Text = LocalizeModule.Instance.GetString("slots_luckTxt");
            m_detail_lab.Text = LocalizeModule.Instance.GetString("activity_slots_dec");
            RegistListener();
            m_detail_root.Visible = false;
            m_slotResult_com.Visible = false;
            if (m_page_btn[0].Checked)
            {
                BtnCoinPage(true);
            }
            else
            {
                m_page_btn[0].Checked = true;
            }
            m_BgEffect01.EffectPrefabName = "UI_choujiang_changjing.prefab";
            m_BgEffect02.EffectPrefabName = "UI_choujiang_paomadeng.prefab";
            m_SpecialEffect.EffectPrefabName = "UI_choujiang_teshuwupin.prefab";
            m_ChooseEffect.EffectPrefabName = "UI_choujiang_xuanzhong.prefab";
            m_buyEffect.EffectPrefabName = "UI_choujiang_anniu.prefab";
            m_BgEffect01.Visible = true;
            m_BgEffect02.Visible = true;
            m_buyEffect.Visible = true;
            SetPageState(true);
            m_CanBuy = true;
            m_buy_btn.Visible = true;
            m_CurrentPage = SlotPageType.PageCoin;
        }

        public override void OnHide()
        {
            base.OnHide();

            GameEvents.System_Events.PlayMainBGM.SafeInvoke(true);
            UnRegistListener();
            m_BgEffect01.Visible = false;
            m_BgEffect02.Visible = false;
            m_ChooseEffect.Visible = false;
            m_SpecialEffect.Visible = false;
            m_buyEffect.Visible = false;
            m_startBuy = false;
            m_CanBuy = true;
            totoalTimeSection = 0f;
            timesection = 0f;
            m_startSure = false;
        }


        private void BtnCoinPage(bool b)
        {
            if (!b)
            {
                return;
            }

            EngineCoreEvents.AudioEvents.PlayAudio.SafeInvoke(Audio.AudioType.UISound, EngineCommonAudioKey.table_change.ToString());

            RequestSlot(SlotPageType.PageCoin);
        }

        private void BtnCashPage(bool b)
        {
            if (!b)
            {
                return;
            }

            EngineCoreEvents.AudioEvents.PlayAudio.SafeInvoke(Audio.AudioType.UISound, EngineCommonAudioKey.table_change.ToString());

            RequestSlot(SlotPageType.PageCash);
        }

        private void BtnDetail(GameObject obj)
        {
            EngineCoreEvents.AudioEvents.PlayAudio.SafeInvoke(Audio.AudioType.UISound, EngineCommonAudioKey.Button_Click_Common.ToString());
            m_detail_root.Visible = !m_detail_root.Visible;
        }

        private void BtnClose(GameObject obj)
        {
            EngineCoreEvents.AudioEvents.PlayAudio.SafeInvoke(Audio.AudioType.UISound, EngineCommonAudioKey.Close_Window.ToString());

            if (!m_startBuy && m_canClose)
            {
                EngineCoreEvents.UIEvent.HideUIEvent.SafeInvoke(UIDefine.UI_SLOTS);
            }
        }

        private void BtnBuy(GameObject obj)
        {
            EngineCoreEvents.AudioEvents.PlayAudio.SafeInvoke(Audio.AudioType.UISound, GameCustomAudioKey.shop_buycoin.ToString());
            if (m_startBuy || !m_CanBuy)
            {
                return;
            }
            if (m_lottoID < 0)
            {
                return;
            }




            LottoBuyRequest req = new LottoBuyRequest();
            req.LottoId = m_lottoID;
            GameEvents.NetWorkEvents.SendMsg.SafeInvoke(req);

        }

        private void OnResponse(object obj)
        {
            if (obj == null)
            {
                return;
            }
            if (obj is LottoResponse)
            {
                m_ChooseEffect.Visible = false;
                LottoResponse res = obj as LottoResponse;
                m_lottoRes = res;
                m_curLucky = m_lottoRes.Lucky;
                m_lottoID = res.LottoId;
                InitSlots(res);
            }
            else if (obj is LottoBuyResponse)
            {
                OnLottoBuyResponse(obj as LottoBuyResponse);
            }
        }

        private void RequestSlot(SlotPageType pageType)
        {
            m_CurrentPage = pageType;
            LottoRequest req = new LottoRequest();
            if (pageType == SlotPageType.PageCoin)
            {
                req.CostType = CostType.CostCoin;
            }
            else if (pageType == SlotPageType.PageCash)
            {
                req.CostType = CostType.CostCash;
            }
#if !NETWORK_SYNC || UNITY_EDITOR
            GameEvents.NetWorkEvents.SendAsyncMsg.SafeInvoke(req);
#else
            GameEvents.NetWorkEvents.SendMsg.SafeInvoke(req);
#endif



        }

        private void InitSlots(LottoResponse res)
        {
            m_luckValue_lab.Text = res.Lucky.ToString();
            m_cost_type = res.CostType;

            if (res.CostType == CostType.CostCoin)
            {
                m_moneyIcon_img.Sprite = "icon_mainpanel_coin_2.png";
                SetBuyContent(res.CostValue, GlobalInfo.MY_PLAYER_INFO.Coin);
            }
            else if (res.CostType == CostType.CostCash)
            {
                m_moneyIcon_img.Sprite = "icon_mainpanel_cash_2.png";
                SetBuyContent(res.CostValue, GlobalInfo.MY_PLAYER_INFO.Cash);

            }
            if (res.Items.Count != ITEMMAXSIZE)
            {
                Debug.Log("item 太少了");
            }

            for (int i = 0; i < ITEMMAXSIZE; i++)
            {
                if (res.Items.Count > i)
                {
                    LottoItemProto lottoItem = res.Items[i];
                    if (lottoItem == null)
                    {
                        continue;
                    }
                    ConfProp prop = ConfProp.Get(lottoItem.PropId);
                    if (prop == null)
                    {
                        continue;
                    }
                    m_itemIcon_img[i].Sprite = prop.icon;
                    //m_itemName_lab[i].Text = LocalizeModule.Instance.GetString(prop.name);
                    m_itemCount_lab[i].Text = string.Format("x{0}", lottoItem.Count);
                    m_special_root[i].Visible = lottoItem.Special;
                    if (lottoItem.Special)
                    {
                        m_itemBg_img[i].Sprite = m_SpecialBG_Str;
                        m_SpecialEffect.gameObject.transform.SetParent(m_itemBg_img[i].gameObject.transform, false);
                        m_SpecialEffect.Visible = true;
                    }
                    else
                    {
                        m_itemBg_img[i].Sprite = m_NormalBG_Str;
                    }
                }
            }
        }

        private void SetBuyContent(float cost, float totalMoney)
        {
            if (totalMoney < cost)
            {
                m_buyCount_lab.color = Color.red;
            }
            else
            {
                m_buyCount_lab.color = Color.white;
            }
            m_buyCount_lab.Text = string.Format("{0}/{1}", cost, totalMoney);
        }

        private float m_singleTime; //一圈花费的时间
        private float m_totalTime; //总时间

        private float timesection = 0f;
        private float totoalTimeSection = 0f;
        private int m_curEffect = 0;
        private bool m_startBuy = false;

        private int m_chooseItemIndex = -1;
        private LottoResponse m_lottoRes = null;

        private bool m_startSure = false; //开始定位选中物品

        private bool m_canClose = true;

        private int m_curLucky = 0;

        private Audio m_playingAudio;

        private SlotPageType m_CurrentPage = SlotPageType.PageCoin;
        public override void Update()
        {
            base.Update();
            if (m_startBuy)
            {
                if (!m_startSure)
                {
                    totoalTimeSection += Time.deltaTime;
                    if (totoalTimeSection >= m_totalTime)
                    {
                        m_startSure = true;
                        return;
                    }
                }

                timesection += Time.deltaTime;
                m_singleTime = m_slotValue.getSpeed(totoalTimeSection * (1f / m_slotValue.m_totalTime));
                float itemTime = m_singleTime / (float)ITEMMAXSIZE;
                if (timesection >= itemTime)
                {
                    m_curEffect = (m_curEffect + 1 + ITEMMAXSIZE) % ITEMMAXSIZE;
                    //m_effect_img[(m_curEffect - 1 + ITEMMAXSIZE) % ITEMMAXSIZE].Visible = false;
                    //m_effect_img[m_curEffect].Visible = true;
                    m_ChooseEffect.gameObject.transform.SetParent(m_itemBg_img[m_curEffect].gameObject.transform, false);
                    m_ChooseEffect.Visible = true;
                    if (m_startSure && m_curEffect == m_chooseItemIndex)
                    {
                        AnimOver();
                    }
                    timesection = 0f;
                }
            }

        }

        private void OnLottoBuyResponse(LottoBuyResponse res)
        {
            m_chooseItemIndex = -1;
            if (res.ResponseStatus != null || m_lottoRes == null)
            {
                if (m_CurrentPage == SlotPageType.PageCash)
                {
                    //PopUpManager.OpenCashBuyError();
                    PopUpManager.OpenGoToCashShop();
                }
                else if (m_CurrentPage == SlotPageType.PageCoin)
                {
                    //PopUpManager.OpenCoinBuyError();
                    //PopUpManager.OpenGoToCoinShop();
                    PushGiftManager.Instance.TurnOn(ENUM_PUSH_GIFT_BLOCK_TYPE.E_COIN);
                }
                //PopUpManager.OpenNormalOnePop(res.ResponseStatus.Msg);
                return;
            }
            for (int i = 0; i < m_lottoRes.Items.Count; i++)
            {
                if (m_lottoRes.Items[i].Id == res.Id) //mmp
                {
                    m_chooseItemIndex = i;
                    Debug.Log("chooseItemIdex " + m_chooseItemIndex + "  " + res.Id);
                    break;
                }
            }
            if (m_chooseItemIndex >= 0)
            {
                EngineCoreEvents.AudioEvents.PlayAndGetAudio.SafeInvoke(Audio.AudioType.Sound, GameCustomAudioKey.lotto_playing.ToString(), (audio) =>
                {
                    m_playingAudio = audio;
                });

                if (m_lottoRes.CostType == CostType.CostCoin)
                {
                    GlobalInfo.MY_PLAYER_INFO.ChangeCoin(-m_lottoRes.CostValue);
                    SetBuyContent(m_lottoRes.CostValue, GlobalInfo.MY_PLAYER_INFO.Coin);
                    //m_buyCount_lab.Text = string.Format("{0}/{1}", m_lottoRes.CostValue, GlobalInfo.MY_PLAYER_INFO.Coin);
                }
                else if (m_lottoRes.CostType == CostType.CostCash)
                {
                    GlobalInfo.MY_PLAYER_INFO.ChangeCash(-m_lottoRes.CostValue);
                    SetBuyContent(m_lottoRes.CostValue, GlobalInfo.MY_PLAYER_INFO.Cash);
                    //m_buyCount_lab.Text = string.Format("{0}/{1}", m_lottoRes.CostValue, GlobalInfo.MY_PLAYER_INFO.Cash);
                }

                Dictionary<UBSParamKeyName, object> _params = new Dictionary<UBSParamKeyName, object>()
                    {
                        { UBSParamKeyName.ContentType, m_CurrentPage},
                        { UBSParamKeyName.Currency, m_lottoRes.CostType},
                        { UBSParamKeyName.ContentID, res.Props[0].PropId},
                        { UBSParamKeyName.NumItems, res.LottoCount},
                    };
                UserBehaviorStatisticsModules.Instance.LogEvent(UBSEventKeyName.lotto_play, 1f, _params);

                SetPageState(false);

                m_startBuy = true;
                m_canClose = false;
                m_CanBuy = false;
                m_totalTime = m_slotValue.m_totalTime;
                m_curEffect = 0;
                m_buy_btn.Visible = false;
                //m_effect_img[0].Visible = true;
            }
            else
            {
                Debug.LogError("No chooseItemIdex ");
            }
        }

        private void SetPageState(bool state)
        {
            for (int i = 0; i < m_page_btn.Length; i++)
            {
                m_page_btn[i].Enabled = state;
            }
        }

        private void AnimOver()
        {
            totoalTimeSection = 0f;
            timesection = 0f;
            if (m_chooseItemIndex < m_lottoRes.Items.Count)
            {
                LottoItemProto itemProto = m_lottoRes.Items[m_chooseItemIndex];
                if (itemProto.Special)
                {
                    m_curLucky = 0;
                }
                else
                {
                    m_curLucky += itemProto.Lucky;
                }
                ConfProp rewardProp = ConfProp.Get(itemProto.PropId);
                if (rewardProp != null)
                {
                    if (rewardProp.type == (int)GamePropType.Cash)
                    {
                        GlobalInfo.MY_PLAYER_INFO.ChangeCash(itemProto.Count);
                        if (m_cost_type == CostType.CostCash)
                        {
                            SetBuyContent(m_lottoRes.CostValue, GlobalInfo.MY_PLAYER_INFO.Cash);
                        }
                    }
                    else if (rewardProp.type == (int)GamePropType.Coin)
                    {
                        GlobalInfo.MY_PLAYER_INFO.ChangeCoin(itemProto.Count);
                        if (m_cost_type == CostType.CostCoin)
                        {
                            SetBuyContent(m_lottoRes.CostValue, GlobalInfo.MY_PLAYER_INFO.Coin);
                        }
                    }
                    else if (rewardProp.type == (int)GamePropType.BindingEnergy)
                    {
                        GlobalInfo.MY_PLAYER_INFO.ChangeVit(itemProto.Count);
                    }
                }
                m_luckValue_lab.Text = m_curLucky.ToString();
                if (m_playingAudio != null)
                    m_playingAudio.Stop();

                //EngineCoreEvents.AudioEvents.s(GameCustomAudioKey.shop_buycoin.ToString(), null);
                TimeModule.Instance.SetTimeout(() =>
                {
                    m_buy_btn.Visible = true;
                    m_canClose = true;
                    OnActiveAllEffect(false);
                    m_slotResult_com.Visible = true;
                    m_slotResult_com.SetData(itemProto, m_lottoRes.CostType, m_lottoRes.CostValue);

                }, 1.2f);

            }
            m_startBuy = false;
            m_startSure = false;
            SetPageState(true);


        }

        private void OnAgain()
        {
            m_CanBuy = true;
            OnActiveAllEffect(true);
            BtnBuy(null);
        }

        private void OnOK()
        {
            m_CanBuy = true;
            OnActiveAllEffect(true);
        }

        private void OnActiveAllEffect(bool flag)
        {
            m_BgEffect01.Visible = flag;
            m_BgEffect02.Visible = flag;
            m_ChooseEffect.Visible = flag;
            m_SpecialEffect.Visible = flag;
            m_buyEffect.Visible = flag;

        }

        //private void OnResultOpen()
        //{

        //}
    }


}
