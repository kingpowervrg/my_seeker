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
    public class LoseGiftItemView : GameUIComponent
    {
        private GameLabel m_gift_name;
        private GameButton m_buy_btn;
        private GameLabel m_price_txt;
        private GameImage m_bought_img;
        private GameUIContainer m_gift_grid;

        private PushGiftData m_data;
        protected override void OnInit()
        {
            base.OnInit();
            m_gift_name = this.Make<GameLabel>("RawImage:Text");
            m_buy_btn = this.Make<GameButton>("RawImage:btnSure");
            m_price_txt = m_buy_btn.Make<GameLabel>("Text");
            m_bought_img = this.Make<GameImage>("Image_purchased");
            m_gift_grid = Make<GameUIContainer>("RawImage:ScrollView:Viewport");
        }

        public override void OnShow(object param)
        {
            base.OnShow(param);
            this.m_buy_btn.AddClickCallBack(OnBuyClicked);
            GameEvents.IAPEvents.OnTransactionDone += DisableItem;
        }

        public override void OnHide()
        {
            base.OnHide();
            this.m_buy_btn.RemoveClickCallBack(OnBuyClicked);

            GameEvents.IAPEvents.OnTransactionDone -= DisableItem;

        }

        public void Refresh(Push_Info push_gift)
        {
            if (null == push_gift)
                return;


            Push_Info info = push_gift;

            ConfPush push = ConfPush.Get(info.PushId);

            PushGiftData data = new PushGiftData()
            {
                m_push_gift_id = push.id,
                m_tex_name = push.icon,
                m_price_txt = GameEvents.IAPEvents.Sys_GetPriceEvent.SafeInvoke(push.chargeid),
                m_bought = info.Buyed,
                m_charge_id = push.chargeid,
            };
            ConfProp gift_prop = ConfProp.Get(push.giftid);
            m_data = data;
            m_gift_name.Text = LocalizeModule.Instance.GetString(gift_prop.name);
            m_buy_btn.Visible = true;
            m_bought_img.Visible = false;
            m_price_txt.Text = m_data.m_price_txt;

            var prop_json_infos = CommonHelper.GetFixedDropOuts(gift_prop.dropout);

            m_gift_grid.EnsureSize<DropItemIcon>(prop_json_infos.Count);

            for (int i = 0; i < m_gift_grid.ChildCount; ++i)
            {
                DropItemIcon child = m_gift_grid.GetChild<DropItemIcon>(i);
                DropOutJsonData prop_json = prop_json_infos[i];
                ConfProp prop = ConfProp.Get(prop_json.value);

                child.InitSprite(prop.icon, prop_json.count, prop_json.value);
                child.Visible = true;
            }
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
