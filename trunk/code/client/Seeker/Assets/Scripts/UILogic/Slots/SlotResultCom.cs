using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EngineCore;
using UnityEngine;

namespace SeekerGame
{
    public class SlotResultCom : GameUIComponent
    {
        private GameImage m_icon_img;
        private GameLabel m_itemCount_lab;
        private GameLabel m_itemName_lab;
        private GameImage m_costType_img;
        private GameLabel m_costValue_lab;

        private GameButton m_again_btn;
        private GameButton m_sure_btn;
        private GameUIEffect m_effect;  //特殊物品特效
        private GameUIEffect m_iconEffect;
        private GameUIEffect m_normalEffect; //普通物品特效

        private void InitController()
        {
            m_icon_img = Make<GameImage>("Image:icon");
            m_itemCount_lab = m_icon_img.Make<GameLabel>("Text");
            m_itemName_lab = m_icon_img.Make<GameLabel>("name");
            m_costType_img = Make<GameImage>("btnAgain:costType");
            m_costValue_lab = Make<GameLabel>("btnAgain:costValue");

            m_again_btn = Make<GameButton>("btnAgain");
            m_sure_btn = Make<GameButton>("btnSure");

            this.m_effect = Make<GameUIEffect>("Image:UI_choujiang_jieguo");
            this.m_iconEffect = Make<GameUIEffect>("Image:UI_choujiang_jieguo03");
            this.m_normalEffect = Make<GameUIEffect>("Image:UI_choujiang_jieguo02");
        }

        private void RegistListener()
        {
            m_again_btn.AddClickCallBack(BtnAgain);
            m_sure_btn.AddClickCallBack(BtnSure);
        }

        private void UnRegistListener()
        {
            m_again_btn.RemoveClickCallBack(BtnAgain);
            m_sure_btn.RemoveClickCallBack(BtnSure);
        }

        protected override void OnInit()
        {
            base.OnInit();
            InitController();
        }

        public override void OnShow(object param)
        {
            base.OnShow(param);
            RegistListener();
            m_effect.EffectPrefabName = "UI_choujiang_jieguo.prefab";
            m_iconEffect.EffectPrefabName = "UI_choujiang_jieguo03.prefab";
            m_normalEffect.EffectPrefabName = "UI_choujiang_jieguo02.prefab";
            this.m_iconEffect.Visible = true;
            this.m_normalEffect.Visible = false;
            this.m_effect.Visible = false;
            //GameEvents.UIEvents.UI_SLots_Events.OnResultOpen.SafeInvoke();
            EngineCoreEvents.AudioEvents.PlayAudio.SafeInvoke(Audio.AudioType.UISound,GameCustomAudioKey.lotto_show.ToString());

        }

        public override void OnHide()
        {
            base.OnHide();
            UnRegistListener();
            this.m_iconEffect.Visible = false;
            this.m_normalEffect.Visible = false;
            this.m_effect.Visible = false;
        }

        private void BtnAgain(GameObject obj)
        {
            EngineCoreEvents.AudioEvents.PlayAudio.SafeInvoke(Audio.AudioType.UISound,GameCustomAudioKey.shop_buycoin.ToString());
            GameEvents.UIEvents.UI_SLots_Events.OnAgain.SafeInvoke();
            SetVisible(false);
        }

        private void BtnSure(GameObject obj)
        {
            EngineCoreEvents.AudioEvents.PlayAudio.SafeInvoke(Audio.AudioType.UISound, EngineCommonAudioKey.Button_Click_Common.ToString());
            GameEvents.UIEvents.UI_SLots_Events.OnOK.SafeInvoke();
            SetVisible(false);
        }

        public void SetData(LottoItemProto itemProto, CostType costType, float costValue)
        {
            this.m_normalEffect.Visible = !itemProto.Special;
            this.m_effect.Visible = itemProto.Special;
            if (itemProto.Special)
            {

            }

            ConfProp prop = ConfProp.Get(itemProto.PropId);
            if (prop == null)
            {
                return;
            }
            m_icon_img.Sprite = prop.icon;
            m_itemCount_lab.Text = string.Format("x{0}", itemProto.Count);
            m_itemName_lab.Text = LocalizeModule.Instance.GetString(prop.name);
            if (costType == CostType.CostCash)
            {
                m_costType_img.Sprite = "icon_mainpanel_cash_2.png";
            }
            else if (costType == CostType.CostCoin)
            {
                m_costType_img.Sprite = "icon_mainpanel_coin_2.png";
            }
            m_costValue_lab.Text = costValue.ToString();
            GlobalInfo.MY_PLAYER_INFO.AddSingleBagInfo(itemProto.PropId, itemProto.Count);

            GameEvents.UIEvents.UI_GameEntry_Event.Listen_OnCombinePropCollected.SafeInvoke();
        }
    }
}
