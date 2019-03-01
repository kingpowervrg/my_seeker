using EngineCore;
using GOGUI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SeekerGame
{
    public class ShopItemUIComponent : GameLoopItem
    {
        private GameImage m_icon_img;
        private GameLabel m_count_lab;
        private GameImage m_cash_img;

        private GameImage m_oriRoot_root;
        private GameLabel m_oriPrice_lab;

        private GameLabel m_price_lab;

        private GameImage m_disCount_root;
        private GameLabel m_disCount_lab;

        private GameToggleButton m_self_tog;

        //private TweenRotationEuler m_Rota_tween;

        private ShopItemData m_itemData = null;
        private ShopType m_curType = ShopType.None;
        private ShopType m_lastType = ShopType.None;

        private ShopInfoUIComponent m_infoComponent;
        private GameImage m_countBgImg = null;

        private bool m_isChoose = false;

        protected override void OnInit()
        {
            base.OnInit();
            InitController();
        }

        protected override void OnLoopItemBecameVisible()
        {
            InitItem();

            m_self_tog.AddChangeCallBack(ChangeState);

            m_self_tog.SetValueWithoutOnChangedNotify(m_isChoose);
        }

        protected override void OnLoopItemBecameInvisible()
        {
            m_lastType = ShopType.None;
            m_self_tog.RemoveChangeCallBack(ChangeState);
            m_self_tog.SetValueWithoutOnChangedNotify(false);
        }


        private void InitController()
        {
            m_self_tog = Make<GameToggleButton>(gameObject);
            this.m_countBgImg = Make<GameImage>("Image");
            m_icon_img = Make<GameImage>("icon");
            m_count_lab = this.m_countBgImg.Make<GameLabel>("Text");

            m_oriRoot_root = Make<GameImage>("Image_original");
            m_oriPrice_lab = m_oriRoot_root.Make<GameLabel>("Text_number");

            m_price_lab = Make<GameLabel>("price");

            m_disCount_root = Make<GameImage>("Image_label");
            m_disCount_lab = m_disCount_root.Make<GameLabel>("Text");
            //m_Rota_tween = gameObject.GetComponent<TweenRotationEuler>();
            m_cash_img = Make<GameImage>("Image_cash");
        }


        public void setItemData(ShopItemData itemData, ShopInfoUIComponent infoComponent, ShopType type, bool ischoose)
        {
            this.m_itemData = itemData;
            this.m_infoComponent = infoComponent;
            this.m_curType = type;
            this.m_isChoose = ischoose;

            m_lastType = m_curType;
        }
        private void InitItem()
        {
            if (m_itemData == null || m_curType == ShopType.None)
            {
                return;
            }
            if (m_itemData.m_prop != null)
            {
                m_icon_img.Sprite = m_itemData.m_prop.icon;
            }
            this.m_countBgImg.Visible = m_itemData.m_number > 1;
            if (m_itemData.m_number > 1)
            {
                m_count_lab.Text = string.Format("x{0}", m_itemData.m_number);
            }
            else
            {
                m_count_lab.Text = string.Empty;
            }
            m_cash_img.Sprite = ShopHelper.getCashType(m_itemData.m_costType);
            if (!m_itemData.m_hasDis)
            {
                m_oriRoot_root.Visible = false;
                m_disCount_root.Visible = false;
                m_price_lab.Text = m_itemData.m_oriPrice.ToString();
            }
            else
            {
                m_oriPrice_lab.Text = m_itemData.m_oriPrice.ToString();
                m_disCount_lab.Text = string.Format("-{0}%", m_itemData.m_disCount);
                m_price_lab.Text = (m_itemData.m_disPrice).ToString();
                m_oriRoot_root.Visible = true;
                m_disCount_root.Visible = true;
            }
        }

        private void ChangeState(bool flag)
        {
            if (flag)
            {
                if (ShopType.None != m_lastType)
                    EngineCoreEvents.AudioEvents.PlayAudio.SafeInvoke(Audio.AudioType.UISound, EngineCommonAudioKey.table_change.ToString());

                GameEvents.UIEvents.UI_Shop_Event.OnChooseItem.SafeInvoke(m_itemData.marketID, gameObject.transform);
                if (m_infoComponent != null)
                {
                    //m_Rota_tween.PlayForward();
                    m_infoComponent.setPanel(m_itemData, m_curType);
                }
            }
            //else
            //{
            //    m_Rota_tween.PlayBackward();
            //}
        }
    }
}

