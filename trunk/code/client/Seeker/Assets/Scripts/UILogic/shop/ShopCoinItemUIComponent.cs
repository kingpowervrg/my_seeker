using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EngineCore;

namespace SeekerGame
{
    public class ShopCoinItemUIComponent : GameUIComponent
    {
        //private GameUIEffect m_icon_effect;
        private GameImage m_icon_img;
        private GameLabel m_count_lab;

        private GameImage m_disCount_root;
        private GameLabel m_disCount_lab;

        private GameImage m_oriPrice_root;
        private GameLabel m_oriPrice_lab;

        private GameButton m_buy_btn;

        private GameLabel m_cost_lab;

        private GameUIEffect m_effect;

        private ShopItemData m_itemdata;
        private ShopType m_curShopType = ShopType.None;
        private int m_cost = 0;

        private bool m_is_fast_buy = false;
        private int m_idx;
        protected override void OnInit()
        {
            base.OnInit();
            InitController();
        }

        private void InitController()
        {

            m_icon_img = Make<GameImage>("icon");
            m_effect = m_icon_img.Make<GameUIEffect>("UI_zengjiatili");
            m_count_lab = m_icon_img.Make<GameLabel>("Image:count");

            m_disCount_root = m_icon_img.Make<GameImage>("Image_label");
            m_disCount_lab = m_disCount_root.Make<GameLabel>("Text");
            m_oriPrice_root = m_icon_img.Make<GameImage>("Image_original");
            m_oriPrice_lab = m_oriPrice_root.Make<GameLabel>("Text_number");


            m_buy_btn = Make<GameButton>("btnbuy");

            m_cost_lab = m_buy_btn.Make<GameLabel>("Text_cost");


        }

        public override void OnShow(object param)
        {
            base.OnShow(param);

            m_buy_btn.AddClickCallBack(btnBuy);
            GameEvents.BuyEvent.OnShopRes += OnBuyCallback;
            GameEvents.BuyEvent.OnShopReq += OnRequestCallback;
            GameEvents.Skill_Event.OnSkillFinish += OneUseReqCallback;

            this.SwitchButton();
            if (ShopType.Vit == m_curShopType)
            {
                if (11 == m_itemdata.m_prop.id)
                    this.m_effect.EffectPrefabName = "UI_zengjiatili_pisa.prefab";
                else if (12 == m_itemdata.m_prop.id)
                    this.m_effect.EffectPrefabName = "UI_zengjiatili_huoguo.prefab";
                else
                    this.m_effect.EffectPrefabName = "UI_zengjiatili_tongyong.prefab";
            }

            this.m_effect.Visible = true;
        }

        public override void OnHide()
        {
            base.OnHide();
            m_buy_btn.RemoveClickCallBack(btnBuy);
            GameEvents.BuyEvent.OnShopRes -= OnBuyCallback;
            GameEvents.BuyEvent.OnShopReq -= OnRequestCallback;
            GameEvents.Skill_Event.OnSkillFinish -= OneUseReqCallback;
            m_itemdata = null;
            m_curShopType = ShopType.None;

            this.m_effect.Visible = false;
        }

        private void SwitchButton()
        {
            if (null == m_itemdata)
                return;

            if (m_is_fast_buy)
            {
                //因不足的原因，弹出快速购买
                PlayerPropMsg item = GlobalInfo.MY_PLAYER_INFO.GetBagInfosByID(m_itemdata.m_prop.id);

                if (null != item && item.Count > 0)
                {
                    m_disCount_root.Visible = false;
                    m_oriPrice_root.Visible = false;
                    m_buy_btn.Visible = false;
                    m_count_lab.Text = string.Format("x{0}", item.Count);

                    return;
                }
            }

            if (ShopType.Vit == m_curShopType && 1 == m_itemdata.m_number && !m_itemdata.m_hasDis)
            {
                //道具购买次数限制触发
                PlayerPropMsg item = GlobalInfo.MY_PLAYER_INFO.GetBagInfosByID(m_itemdata.m_prop.id);

                if (null != item && item.Count > 0)
                {
                    m_disCount_root.Visible = false;
                    m_oriPrice_root.Visible = false;
                    m_buy_btn.Visible = false;
                    m_count_lab.Text = string.Format("x{0}", item.Count);

                    return;
                }
            }

            m_buy_btn.Visible = true;
            m_count_lab.Text = string.Format("x{0}", m_itemdata.m_number);

            if (m_itemdata.m_hasDis)
            {
                m_cost = m_itemdata.m_disPrice;
                m_disCount_root.Visible = true;
                m_oriPrice_root.Visible = true;
                m_disCount_lab.Text = string.Format("-{0}%", m_itemdata.m_disCount);
                m_oriPrice_lab.Text = m_itemdata.m_oriPrice.ToString();
            }
            else
            {
                m_cost = m_itemdata.m_oriPrice;
                m_disCount_root.Visible = false;
                m_oriPrice_root.Visible = false;
                if (ShopType.Vit == m_curShopType)
                    m_count_lab.Text = "";
            }

            //if (ShopType.Coin == m_curShopType && null == m_icon_effect)
            //{
            //    m_icon_effect = Make<GameUIEffect>("UI_jinbi_goumai01");
            //}

            //if (null != m_icon_effect)
            //    m_icon_effect.EffectPrefabName = string.Format("UI_jinbi_goumai0{0}.prefab", m_idx + 1);



        }

        public void setData(ShopItemData itemdata, ShopType shopType, int index_, bool is_fast_buy = false)
        {
            m_itemdata = itemdata;
            m_curShopType = shopType;
            m_is_fast_buy = is_fast_buy;
            m_idx = index_;
            if (itemdata == null)
            {
                return;
            }
            if (itemdata.m_prop != null)
            {
                m_icon_img.Sprite = itemdata.m_prop.icon;
            }



        }

        private void btnBuy(GameObject obj)
        {
            if (m_itemdata == null)
            {
                return;
            }
            BuyManager.Instance.ShopBuy(m_itemdata.marketID, 1, m_itemdata.m_number, m_curShopType, m_cost, m_itemdata.m_costType);
        }

        private void btnUse(GameObject obj)
        {
            if (m_itemdata == null)
            {
                return;
            }

            GameSkillManager.Instance.OnStartSkill(m_itemdata.m_prop.id, 1);
        }

        private void OnBuyCallback(MarkeBuyResponse res)
        {
            if (m_itemdata == null || m_requestType != m_curShopType)
            {
                return;
            }
            if (res.ResponseStatus != null)  ////////////////TTTTT
            {
                PopUpManager.OpenNormalOnePop("shop_rmb_no");
            }
            else
            {
                if (m_itemdata.marketID == m_requestID)
                {
                    m_requestType = ShopType.None;
                    int cost = 0;
                    if (m_itemdata.m_hasDis)
                    {
                        cost = m_itemdata.m_disPrice * m_requestCount;
                    }
                    else
                    {
                        cost = m_itemdata.m_oriPrice * m_requestCount;
                    }
                    if (m_itemdata.m_costType == CostType.CostCash)
                    {
                        GlobalInfo.MY_PLAYER_INFO.ChangeCash(-cost);
                    }
                    else if (m_itemdata.m_costType == CostType.CostCoin)
                    {
                        GlobalInfo.MY_PLAYER_INFO.ChangeCoin(-cost);
                    }
                    PopUpManager.OpenNormalOnePop("shop_rmb_ok");
                    //CommonHelper.OpenGift(m_itemdata.m_prop.id, m_requestCount);

                    if (m_is_fast_buy)
                    {
                        GameSkillManager.Instance.OnStartSkill(m_itemdata.m_prop.id, 1);
                    }
                    else
                    {
                        if (ShopType.Vit == m_curShopType)
                        {
                            ConfProp confProp = ConfProp.Get(m_itemdata.m_prop.id);
                            if (confProp != null && confProp.skillId != 26)
                            {
                                GlobalInfo.MY_PLAYER_INFO.AddSingleBagInfo(m_itemdata.m_prop.id, m_itemdata.m_number);
                                GameEvents.UIEvents.UI_Bag_Event.OnPropCost.SafeInvoke(m_itemdata.m_prop.id);

                                GameEvents.UIEvents.UI_GameEntry_Event.Listen_OnCombinePropCollected.SafeInvoke();
                            }
                            else if (confProp != null)
                            {
                                ConfSkill confskill = ConfSkill.Get(confProp.skillId);
                                VitManager.Instance.ReflashInfiniteVitTime(confskill.duration * 1000);
                            }
                        }
                    }

                    if (m_itemdata.m_limitNumber > 0)
                    {
                        --m_itemdata.m_limitNumber;

                        this.SwitchButton();

                        return;
                    }


                    if (1 == m_itemdata.m_number && false == m_itemdata.m_hasDis)
                    {
                        this.SwitchButton();
                    }



                }
            }
        }

        private long m_requestID = -1;
        private int m_requestCount = 0;
        private int m_total_prop_count = 0;
        private ShopType m_requestType = ShopType.None;

        private void OnRequestCallback(long id, int market_count, int total_prop_count_, ShopType type)
        {
            m_requestID = id;
            m_requestCount = market_count;
            m_total_prop_count = total_prop_count_;
            m_requestType = type;
        }

        private void OneUseReqCallback(long id)
        {
            if (m_itemdata == null || null == m_itemdata.m_prop)
                return;

            if (id == m_itemdata.m_prop.id && 1 == m_itemdata.m_number)
            {
                ConfProp confProp = ConfProp.Get(m_itemdata.m_prop.id);
                if (confProp != null && (confProp.type == 0 || confProp.type == 8))
                {
                    ConfSkill confSkill = ConfSkill.Get(confProp.skillId);
                    if (confSkill != null)
                    {
                        WaveTipHelper.LoadWaveContent("food_add_1", confSkill.gain);
                    }
                }
                GlobalInfo.MY_PLAYER_INFO.ReducePropForBag(id);
                GameEvents.UIEvents.UI_Bag_Event.OnPropCost.SafeInvoke(m_itemdata.m_prop.id);
                this.SwitchButton();
            }
        }
    }
}

