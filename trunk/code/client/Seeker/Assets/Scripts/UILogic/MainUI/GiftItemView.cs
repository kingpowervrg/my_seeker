using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EngineCore;
using GOGUI;
using UnityEngine;
namespace SeekerGame
{
    public class GiftItemView : GameUIComponent
    {
        private GameTexture m_tex;
        private GameButton m_buy_btn;
        private GameLabel m_buy_title;
        private GameLabel m_price_txt;
        private GameImage m_bought_img;
        private GameUIEffect m_buy_effect;
        private PushGiftData m_data;

        protected override void OnInit()
        {
            base.OnInit();
            m_tex = this.Make<GameTexture>("RawImage");
            m_buy_btn = this.Make<GameButton>("btn_buy");
            m_buy_effect = m_buy_btn.Make<GameUIEffect>("UI_shibai_goumai");
            m_buy_effect.EffectPrefabName = "UI_shibai_goumai.prefab";
            m_buy_title = m_buy_btn.Make<GameLabel>("Text");
            m_buy_title.Text = LocalizeModule.Instance.GetString("goods_buy");
            m_price_txt = m_buy_btn.Make<GameLabel>("Text_cost");
            m_bought_img = this.Make<GameImage>("Image_purchased");
        }

        public override void OnShow(object param)
        {
            base.OnShow(param);
            this.m_buy_btn.AddClickCallBack(OnBuyClicked);
            GameEvents.IAPEvents.OnTransactionDone += DisableItem;
            this.m_buy_effect.Visible = true;
        }

        public override void OnHide()
        {
            base.OnHide();
            this.m_buy_btn.RemoveClickCallBack(OnBuyClicked);

            GameEvents.IAPEvents.OnTransactionDone -= DisableItem;

            this.m_buy_effect.Visible = false;
        }

        public void Refresh(Push_Info push_gift)
        {
            if (null == push_gift)
                return;


            Push_Info info = push_gift;

            //if (CommonTools.GetCurrentTimeSecond() > info.EndTime)
            //{
            //    return;
            //}

            ConfPush push = ConfPush.Get(info.PushId);

            PushGiftData data = new PushGiftData()
            {
                m_push_gift_id = push.id,
                m_tex_name = push.icon,
                m_price_txt = GameEvents.IAPEvents.Sys_GetPriceEvent.SafeInvoke(push.chargeid),
                m_bought = info.Buyed,
                m_charge_id = push.chargeid,
            };

            m_data = data;

            m_buy_btn.Visible = true;
            m_bought_img.Visible = false;
            m_tex.TextureName = m_data.m_tex_name;
            m_price_txt.Text = m_data.m_price_txt;
        }

        public void DisableItem(long charge_id_)
        {
            if (m_data.m_charge_id != charge_id_)
            {
                return;

            }
            if (0 == ConfPush.Get(m_data.m_push_gift_id).bolckType)
                m_data.m_bought = true;

            m_buy_btn.Visible = false;
            m_bought_img.Visible = true;
        }

        private void OnBuyClicked(GameObject obj)
        {
            EngineCoreEvents.AudioEvents.PlayAudio.SafeInvoke(Audio.AudioType.UISound, EngineCommonAudioKey.Button_Click_Common.ToString());

            long push_id = m_data.m_push_gift_id;

            GameEvents.NetWorkEvents.SendMsg.SafeInvoke(new CSBuyPushRequest() { PushId = push_id });

        }

    }
}
