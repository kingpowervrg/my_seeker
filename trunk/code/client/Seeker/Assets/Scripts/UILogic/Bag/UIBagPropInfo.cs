using EngineCore;
using GOGUI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SeekerGame
{

    public enum PropInfoTypeEnum
    {
        Sale, //出售
        Use, //使用
        Shop, //商店
        None
    }

    public class UIBagPropInfo : GameUIContainer
    {
        #region 右边面板
        private GameImage m_Icon_img;
        private GameLabel m_Title_lab;
        private GameLabel m_Sum_lab;
        private GameLabel m_Content_lab;

        //private GameObject m_DetailNode_obj; //道具详细面板节点
        private GameImage m_MsgNode_obj; //道具详细信息节点

        private Transform m_miaoShu;
        private GameButton m_MultiFunc_btn;
        private GameLabel m_MultiFunc_lab;
        private GameButton m_Sale_btn;
        private GameButton m_Use_btn;
        private GameLabel m_SaleUseNode_obj;
        private GameLabel m_Money_lab;
        private GameLabel m_NoSaleTip_lab;
        private GameLabel m_ShopTips_lab;
        PropInfoTypeEnum m_currentInfo_enum = PropInfoTypeEnum.None;

        private PropData m_curPropData;
        #endregion

        public bool RecentlyInfoEnable
        {
            set
            {
                m_miaoShu.gameObject.SetActive(value);
            }
        }

        protected override void OnInit()
        {
            base.OnInit();
            InitController();
        }

        public override void OnShow(object param)
        {
            base.OnShow(param);
            InitEventListener();
        }

        void InitController()
        {
            m_Icon_img = Make<GameImage>("icon");
            m_Content_lab = Make<GameLabel>("content");
            m_Title_lab = m_Content_lab.Make<GameLabel>("title");
            m_Sum_lab = m_Content_lab.Make<GameLabel>("sum");


            m_miaoShu = Widget.transform;//gameObject.transform.Find("miaosu");
            m_MultiFunc_btn = Make<GameButton>("btnMultiFunc");
            m_MultiFunc_lab = m_MultiFunc_btn.Make<GameLabel>("Text");
            m_Sale_btn = Make<GameButton>("btn_sale");
            m_Use_btn = Make<GameButton>("btn_use");
            m_SaleUseNode_obj = Make<GameLabel>("saleUse");

            m_Money_lab = m_SaleUseNode_obj.Make<GameLabel>("labMoney");
            m_NoSaleTip_lab = Make<GameLabel>("noSaleTips");
            m_ShopTips_lab = Make<GameLabel>("shopTip");

            m_MsgNode_obj = Make<GameImage>("Image");
        }

        void InitEventListener()
        {
            m_Sale_btn.AddClickCallBack(btnSale);
            m_Use_btn.AddClickCallBack(btnUse);
            m_MultiFunc_btn.AddClickCallBack(btnMutliFunc);
        }
        public void setInfoData(PropData propData)
        {
            m_curPropData = propData;
            if (propData != null)
            {
                m_Icon_img.Sprite = propData.prop.icon;
                m_Title_lab.Text = LocalizeModule.Instance.GetString(propData.prop.name);
                m_Sum_lab.Text = string.Format("x{0}", propData.num);
                m_Content_lab.Text = LocalizeModule.Instance.GetString(propData.prop.description);
                m_Money_lab.Text = propData.prop.price.ToString();
                long skillId = propData.prop.skillId;
                int tradeLimit = propData.prop.tradeLimit;
                if (skillId > 0)
                {
                    ConfSkill skill = ConfSkill.Get(skillId);
                    if (skill != null && skill.phase <= 3)
                    {
                        if (tradeLimit == 0)
                        {
                            tradeLimit = 1;
                        }
                        else if (tradeLimit == 2)
                        {
                            tradeLimit = 3;
                        }
                    }
                }

                SetPropInfoVisible(true);
                if (tradeLimit == 0)
                {
                    m_MsgNode_obj.SetActive(true);
                    m_SaleUseNode_obj.SetActive(true);
                    m_Sale_btn.SetActive(true);
                    m_Use_btn.SetActive(true);
                    m_NoSaleTip_lab.SetActive(false);
                    m_MultiFunc_btn.SetActive(false);
                    m_ShopTips_lab.SetActive(false);
                    m_currentInfo_enum = PropInfoTypeEnum.None;
                }
                else if (tradeLimit == 1)
                {
                    m_MsgNode_obj.SetActive(true);
                    m_MultiFunc_btn.SetActive(true);
                    m_SaleUseNode_obj.SetActive(true);
                    m_Sale_btn.SetActive(false);
                    m_Use_btn.SetActive(false);
                    m_NoSaleTip_lab.SetActive(false);
                    m_ShopTips_lab.SetActive(false);
                    m_MultiFunc_lab.Text = "Sale";
                    m_currentInfo_enum = PropInfoTypeEnum.Sale;
                }
                else if (tradeLimit == 2)
                {
                    m_MultiFunc_btn.SetActive(true);
                    m_MsgNode_obj.SetActive(true);
                    m_SaleUseNode_obj.SetActive(false);
                    m_Sale_btn.SetActive(false);
                    m_Use_btn.SetActive(false);
                    m_NoSaleTip_lab.SetActive(true);
                    m_ShopTips_lab.SetActive(false);
                    m_MultiFunc_lab.Text = "Use";
                    m_currentInfo_enum = PropInfoTypeEnum.Use;
                }
                else if (tradeLimit == 3)
                {
                    m_MsgNode_obj.SetActive(true);
                    m_SaleUseNode_obj.SetActive(false);
                    m_Sale_btn.SetActive(false);
                    m_Use_btn.SetActive(false);
                    m_NoSaleTip_lab.SetActive(false);
                    m_MultiFunc_btn.SetActive(false);
                    m_ShopTips_lab.SetActive(false);
                    m_currentInfo_enum = PropInfoTypeEnum.None;
                }
            }
        }

        public void setNoInfoData()
        {
            m_MultiFunc_btn.SetActive(true);
            m_SaleUseNode_obj.SetActive(false);
            m_Sale_btn.SetActive(false);
            m_Use_btn.SetActive(false);
            m_MsgNode_obj.SetActive(false);
            m_ShopTips_lab.SetActive(true);
            m_NoSaleTip_lab.Visible = false;
            SetPropInfoVisible(false);
            m_MultiFunc_lab.Text = "Shop";
            m_currentInfo_enum = PropInfoTypeEnum.Shop;
        }

        private void SetPropInfoVisible(bool visible)
        {
            this.m_Content_lab.Visible = visible;
            this.m_Icon_img.Visible = visible;
        }

        void btnMutliFunc(GameObject obj)
        {
            if (m_currentInfo_enum == PropInfoTypeEnum.Sale)
            {
                btnSale(obj);
            }
            else if (m_currentInfo_enum == PropInfoTypeEnum.Shop)
            {
                btnShop(obj);
            }
            else if (m_currentInfo_enum == PropInfoTypeEnum.Use)
            {
                btnUse(obj);
            }
        }

        void btnSale(GameObject obj)
        {
            EngineCoreEvents.AudioEvents.PlayAudio.SafeInvoke(Audio.AudioType.UISound, EngineCommonAudioKey.Button_Click_Common.ToString());

            PropInfoTypeEnum tempTypeEnum = m_currentInfo_enum;
            if (tempTypeEnum == PropInfoTypeEnum.None)
            {
                tempTypeEnum = PropInfoTypeEnum.Sale;
            }
            BagUseData d = new BagUseData(m_curPropData, tempTypeEnum);
            BagUseDialogHelper.EnterBagUseDialog(d);
        }

        void btnUse(GameObject obj)
        {
            EngineCoreEvents.AudioEvents.PlayAudio.SafeInvoke(Audio.AudioType.UISound, EngineCommonAudioKey.Button_Click_Common.ToString());

            PropInfoTypeEnum tempTypeEnum = m_currentInfo_enum;
            if (tempTypeEnum == PropInfoTypeEnum.None)
            {
                tempTypeEnum = PropInfoTypeEnum.Use;
            }
            BagUseData d = new BagUseData(m_curPropData, tempTypeEnum);
            BagUseDialogHelper.EnterBagUseDialog(d);
        }

        void btnShop(GameObject obj)
        {
            if (!SeekerGame.NewGuid.GuidNewManager.Instance.GetProgressByIndex(8))
            {
                PopUpManager.OpenNormalOnePop("guid_shop_no");
                return;
            }
            if (GameEvents.UIEvents.UI_Bag_Event.GetCurrentBagType() == BagTypeEnum.Energy)
            {
                EngineCoreEvents.AudioEvents.PlayAudio.SafeInvoke(Audio.AudioType.UISound, EngineCommonAudioKey.Button_Click_Common.ToString());
                //EngineCoreEvents.UIEvent.ShowUIEvent.SafeInvoke(UIDefine.UI_SHOPENERGY);
                //EngineCoreEvents.UIEvent.ShowUIByOther.SafeInvoke(UIDefine.UI_SHOPENERGY, UIDefine.UI_BAG);
                EngineCoreEvents.UIEvent.ShowUIByOther.SafeInvoke(UIDefine.UI_SHOP, UIDefine.UI_BAG);
            }
            else
            {
                //GameEvents.UIEvents.UI_GameEntry_Event.OnOpenPanel.SafeInvoke(UIDefine.UI_SHOP);
                EngineCoreEvents.UIEvent.ShowUIByOther.SafeInvoke(UIDefine.UI_SHOP, UIDefine.UI_BAG);
            }
  
        }

        public override void OnHide()
        {
            base.OnHide();
            m_Sale_btn.RemoveClickCallBack(btnSale);
            m_Use_btn.RemoveClickCallBack(btnUse);
            m_MultiFunc_btn.RemoveClickCallBack(btnMutliFunc);
            m_currentInfo_enum = PropInfoTypeEnum.None;
            m_curPropData = null;
        }
    }
}

