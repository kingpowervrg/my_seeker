using EngineCore;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SeekerGame
{
    public class ShopInfoUIComponent : GameUIComponent
    {
        private GameImage m_icon_img;
        private GameLabel m_count_lab;
        private GameImage m_disCount_img;
        private GameLabel m_disCount_lab;

        private GameLabel m_itemName_lab;
        private GameLabel m_itemContent_lab;

        private GameButton m_reduce_btn;
        private GameButton m_add_btn;
        private GameLabel m_number_lab;

        private GameLabel m_limit_root;
        private GameLabel m_limit_lab;

        private GameButton m_buy_btn;
        private GameLabel m_price_lab;
        private GameLabel m_oriRoot_root;
        private GameLabel m_oriPrice_lab;
        private GameImage m_cashType_img;
        private GameUIEffect m_buy_Effect;

        private GameImage m_NumberRoot;
        private GameImage m_sumBgImg = null;

        private int m_buyNumber = 1;
        private int m_buyPropCount = 1;
        private ShopItemData m_itemdata;

        private ShopType m_currentType = ShopType.None;

        private int m_cost = 0;

        protected override void OnInit()
        {
            base.OnInit();
            InitController();
        }

        public override void OnShow(object param)
        {
            base.OnShow(param);
            InitListener();
            m_buy_Effect.EffectPrefabName = "UI_goumai_anniu.prefab";
        }

        public override void OnHide()
        {
            base.OnHide();
            UnInitListener();
        }

        public override void Dispose()
        {
            base.Dispose();

        }

        private void InitController()
        {
            this.m_sumBgImg = Make<GameImage>("Image:sumBg");
            m_icon_img = Make<GameImage>("Image:icon");
            m_count_lab = Make<GameLabel>("Image:sum");
            m_itemName_lab = Make<GameLabel>("Image:title");
            m_itemContent_lab = Make<GameLabel>("Image:content");

            m_NumberRoot = Make<GameImage>("Imagenumber");
            m_reduce_btn = Make<GameButton>("Imagenumber:btnReduce");
            m_add_btn = Make<GameButton>("Imagenumber:btnAdd");
            m_number_lab = Make<GameLabel>("Imagenumber:Text");

            m_disCount_img = Make<GameImage>("Image:Image");
            m_disCount_lab = m_disCount_img.Make<GameLabel>("Text");

            m_limit_root = Make<GameLabel>("Imagenumber:income");
            m_limit_lab = m_limit_root.Make<GameLabel>("number");

            m_buy_btn = Make<GameButton>("btn_buy");
            m_cashType_img = m_buy_btn.Make<GameImage>("Image");

            m_price_lab = m_buy_btn.Make<GameLabel>("Text");
            m_oriRoot_root = Make<GameLabel>("Text");
            m_oriPrice_lab = m_oriRoot_root.Make<GameLabel>("Text_number");
            m_buy_Effect = m_buy_btn.Make<GameUIEffect>("UI_goumai_anniu");

        }

        private void InitListener()
        {
            m_add_btn.AddLongClickCallBack(btnLongAddNumber);
            m_reduce_btn.AddLongClickCallBack(btnReduceNumber);
            //m_add_btn.AddClickCallBack(btnAddNumber);
            m_buy_btn.AddClickCallBack(btnBuy);
        }

        private void UnInitListener()
        {
            m_add_btn.CancelLongClick();
            m_reduce_btn.RemoveLongClickCallBack(btnReduceNumber);
            m_add_btn.RemoveLongClickCallBack(btnLongAddNumber);
            m_buy_btn.RemoveClickCallBack(btnBuy);
        }

        public void setPanel(ShopItemData itemdata, ShopType currentType)
        {
            m_itemdata = itemdata;
            if (itemdata == null)
            {
                return;
            }
            m_currentType = currentType;
            m_NumberRoot.Visible = (m_currentType != ShopType.BlackMarket);

            m_buyNumber = 1;
            m_buyPropCount = itemdata.m_number;
            m_cashType_img.Sprite = ShopHelper.getCashType(itemdata.m_costType);
            if (itemdata.m_prop != null)
            {
                m_icon_img.Sprite = itemdata.m_prop.icon;
                m_itemName_lab.Text = LocalizeModule.Instance.GetString(itemdata.m_prop.name);
                m_itemContent_lab.Text = LocalizeModule.Instance.GetString(itemdata.m_prop.description);
            }
            m_number_lab.Text = m_buyNumber.ToString();
            this.m_sumBgImg.Visible = itemdata.m_number > 1;
            if (itemdata.m_number > 1)
            {
                m_count_lab.Text = string.Format("x{0}", itemdata.m_number);
            }
            else
            {
                m_count_lab.Text = string.Empty;
            }
            if (!itemdata.m_hasDis)
            {
                m_disCount_img.Visible = false;
                m_oriRoot_root.Visible = false;
                m_cost = itemdata.m_oriPrice * m_buyNumber;
                m_price_lab.Text = m_cost.ToString();
            }
            else
            {
                m_disCount_lab.Text = string.Format("-{0}%", itemdata.m_disCount);
                m_oriPrice_lab.Text = itemdata.m_oriPrice.ToString();
                m_cost = itemdata.m_disPrice * m_buyNumber;
                m_price_lab.Text = m_cost.ToString();
                m_oriRoot_root.Visible = true;
                m_disCount_img.Visible = true;
            }

            if (itemdata.m_limitNumber < 0)
            {
                m_limit_root.Visible = false;
            }
            else
            {
                m_limit_root.Visible = true;
                m_limit_lab.Text = itemdata.m_limitNumber.ToString();
            }
            checkBtnState();
        }

        private void btnReduceNumber(GameObject obj, float time)
        {
            EngineCoreEvents.AudioEvents.PlayAudio.SafeInvoke(Audio.AudioType.UISound, GameCustomAudioKey.shop_add.ToString());

            if (m_buyNumber > 1)
            {
                //m_buyNumber--;
                m_buyNumber -= Mathf.CeilToInt(Mathf.Clamp(time, 1, 4));
                if (m_buyNumber < 0)
                {
                    m_buyNumber = 0;
                }
                m_number_lab.Text = m_buyNumber.ToString();
            }
            checkBtnState();
            reflashCost();
        }

        private void btnAddNumber(GameObject obj, float time)
        {
            if (m_itemdata == null)
            {
                return;
            }

            EngineCoreEvents.AudioEvents.PlayAudio.SafeInvoke(Audio.AudioType.UISound, GameCustomAudioKey.shop_add.ToString());

            if (m_buyNumber < CommonData.SHOPMAXBUY)
            {
                if (m_itemdata.m_limitNumber > 0)
                {
                    if (m_buyNumber < m_itemdata.m_limitNumber)
                    {
                        m_buyNumber += Mathf.CeilToInt(Mathf.Clamp(time, 1, 4));
                        m_buyNumber = Mathf.Clamp(m_buyNumber, 0, m_itemdata.m_limitNumber);
                        m_number_lab.Text = m_buyNumber.ToString();
                    }
                }
                else if (m_itemdata.m_limitNumber < 0)
                {
                    //m_buyNumber++;
                    m_buyNumber += Mathf.CeilToInt(Mathf.Clamp(time, 1, 4));
                    m_buyNumber = m_buyNumber > CommonData.SHOPMAXBUY ? CommonData.SHOPMAXBUY : m_buyNumber;
                    m_number_lab.Text = m_buyNumber.ToString();
                }
            }
            else
            {
                m_add_btn.CancelLongClick();
            }


            checkBtnState();
            reflashCost();
        }

        private void checkBtnState()
        {
            if (m_buyNumber <= 1)
            {
                setBtnEnable(m_reduce_btn, false);
            }
            else
            {
                setBtnEnable(m_reduce_btn, true);
            }

            if (m_itemdata.m_limitNumber >= 0 && m_buyNumber >= m_itemdata.m_limitNumber || m_buyNumber >= CommonData.SHOPMAXBUY)
            {
                setBtnEnable(m_add_btn, false);
            }
            else
            {
                setBtnEnable(m_add_btn, true);
            }
        }

        private void setBtnEnable(GameButton btn, bool b)
        {
            btn.SetGray(!b);
            btn.SetGrayAndUnClick(b);
        }

        private void btnLongAddNumber(GameObject obj, float time)
        {
            btnAddNumber(obj, time);
        }
        private void btnLongReduceNumber(GameObject obj, float time)
        {
            btnReduceNumber(obj, time);
        }


        private void btnBuy(GameObject obj)
        {
            EngineCoreEvents.AudioEvents.PlayAudio.SafeInvoke(Audio.AudioType.UISound, GameCustomAudioKey.shop_buycoin.ToString());
            BuyManager.Instance.ShopBuy(m_itemdata.marketID, m_buyNumber, m_buyPropCount, m_currentType, m_cost, m_itemdata.m_costType);
        }

        private void reflashCost()
        {
            if (m_itemdata == null)
            {
                return;
            }
            if (m_itemdata.m_hasDis)
            {
                m_price_lab.Text = (m_itemdata.m_disPrice * m_buyNumber).ToString();
            }
            else
            {
                m_price_lab.Text = (m_itemdata.m_oriPrice * m_buyNumber).ToString();
            }
        }
    }
}


