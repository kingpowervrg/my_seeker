using EngineCore;
using System;
using System.Collections.Generic;
using UnityEngine;
namespace SeekerGame
{
    public class GiftView : GameUIComponent
    {


        class PushToggleItem : GameUIComponent
        {
            private GameToggleButton m_toggle;
            protected override void OnInit()
            {
                base.OnInit();
                m_toggle = this.Make<GameToggleButton>("Toggle");
            }

            public void EnableItem()
            {
                m_toggle.Checked = true;
            }
        }

        private GameTexture m_tex;
        private GameButton m_left_btn;
        private GameButton m_right_btn;
        private GameButton m_buy_btn;
        private GameLabel m_buy_title;
        private GameLabel m_price_txt;
        private GameUIContainer m_toggle_grid;
        private GameImage m_bought_img;
        private GameButton m_close_btn;
        private GameButton m_detail_btn;
        private GameUIComponent m_detail_root;
        private GameLabel m_detail_txt;

        private List<PushGiftData> m_datas;

        private int m_cur_idx;

        private SafeAction OnClosed;

        //private GOGUI.TweenScale m_tweenPosShow = null;
        //private GOGUI.TweenAlpha m_TweenAlphaShow = null;

        //private GOGUI.TweenScale m_tweenPosHide = null;
        //private GOGUI.TweenAlpha m_TweenAlphaHide = null;

        protected override void OnInit()
        {
            base.OnInit();
            m_tex = this.Make<GameTexture>("RawImage");
            m_left_btn = this.Make<GameButton>("Button");
            m_right_btn = this.Make<GameButton>("Button (1)");
            m_buy_btn = this.Make<GameButton>("btn_buy");
            m_buy_title = m_buy_btn.Make<GameLabel>("Text");
            m_buy_title.Text = LocalizeModule.Instance.GetString("goods_buy");
            m_price_txt = m_buy_btn.Make<GameLabel>("Text_cost");
            m_bought_img = this.Make<GameImage>("Image_purchased");
            m_toggle_grid = this.Make<GameUIContainer>("ScrollView:Viewport");
            m_close_btn = this.Make<GameButton>("Button_close");
            m_detail_btn = this.Make<GameButton>("Button_detail");
            m_detail_root = this.Make<GameUIComponent>("giftdetail");
            m_detail_txt = m_detail_root.Make<GameLabel>("Text");

            //GOGUI.TweenScale[] tweenPos = this.m_tex.GetComponents<GOGUI.TweenScale>();
            //for (int i = 0; i < tweenPos.Length; i++)
            //{
            //    if (tweenPos[i].style == GOGUI.UITweenerBase.Style.Once)
            //    {
            //        m_tweenPosShow = tweenPos[i];
            //    }
            //    else if (tweenPos[i].style == GOGUI.UITweenerBase.Style.OnHide)
            //    {
            //        m_tweenPosHide = tweenPos[i];
            //    }
            //}

            //GOGUI.TweenAlpha[] TweenAlpha = this.m_tex.GetComponents<GOGUI.TweenAlpha>();
            //for (int i = 0; i < TweenAlpha.Length; i++)
            //{
            //    if (TweenAlpha[i].style == GOGUI.UITweenerBase.Style.Once)
            //    {
            //        m_TweenAlphaShow = TweenAlpha[i];
            //    }
            //    else if (TweenAlpha[i].style == GOGUI.UITweenerBase.Style.OnHide)
            //    {
            //        m_TweenAlphaHide = TweenAlpha[i];
            //    }
            //}
        }

        public override void OnShow(object param)
        {
            base.OnShow(param);

            GameEvents.UIEvents.UI_Bonus_Pop_View_Event.Tell_OnBlock.SafeInvoke(true);

            this.m_buy_btn.AddClickCallBack(OnBuyClicked);
            this.m_left_btn.AddClickCallBack(OnLeftClicked);
            this.m_right_btn.AddClickCallBack(OnRightClicked);
            this.m_close_btn.AddClickCallBack(OnCloseClicked);
            this.m_detail_btn.AddClickCallBack(OnDetailClicked);
            this.m_detail_root.AddClickCallBack(OnDetailCloseClicked);
            this.AddClickCallBack(OnDetailCloseClicked);
            GameEvents.IAPEvents.OnTransactionDone += DisableItem;

        }

        public override void OnHide()
        {
            base.OnHide();
            //this.m_tweenPosHide.ResetAndPlay();
            //this.m_TweenAlphaHide.ResetAndPlay();
            this.m_buy_btn.RemoveClickCallBack(OnBuyClicked);
            this.m_left_btn.RemoveClickCallBack(OnLeftClicked);
            this.m_right_btn.RemoveClickCallBack(OnRightClicked);
            this.m_close_btn.RemoveClickCallBack(OnCloseClicked);
            this.m_detail_btn.RemoveClickCallBack(OnDetailClicked);
            this.m_detail_root.RemoveClickCallBack(OnDetailCloseClicked);
            this.RemoveClickCallBack(OnDetailCloseClicked);
            GameEvents.IAPEvents.OnTransactionDone -= DisableItem;

            OnClosed.SafeInvoke();
            GameEvents.UIEvents.UI_Bonus_Pop_View_Event.Tell_OnBlock.SafeInvoke(false);
            GameEvents.UIEvents.UI_Bonus_Pop_View_Event.Tell_OnShow.SafeInvoke();

            m_detail_root.Visible = false;
        }

        public void Refresh(List<Push_Info> push_gifts, Action Closed_)
        {

            OnClosed = Closed_;

            Refresh(push_gifts);
        }

        private void Refresh(List<Push_Info> push_gifts)
        {

            var not_bought = push_gifts.FindAll((item) => false == item.Buyed);

            if (null == push_gifts || 0 == push_gifts.Count || null == not_bought || 0 == not_bought.Count)
            {
                ShowAll(false);
                return;
            }

            ShowAll(true);

            if (1 == push_gifts.Count)
            {
                m_left_btn.Visible = false;
                m_right_btn.Visible = false;
            }
            else
            {
                m_left_btn.Visible = true;
                m_right_btn.Visible = true;
            }

            m_datas = new List<PushGiftData>();

            for (int i = 0; i < push_gifts.Count; ++i)
            {

                Push_Info info = push_gifts[i];
                if (info.Buyed)
                    continue;

                if (0 != info.EndTime && CommonTools.GetCurrentTimeSecond() > info.EndTime)
                {
                    continue;
                }

                Debug.Log(string.Format("cur push id = {0}", info.PushId));

                ConfPush push = ConfPush.Get(info.PushId);

                if (null == push)
                {
                    Debug.Log(string.Format("confpush {0} is null ", info.PushId));
                    continue;
                }

                PushGiftData data = new PushGiftData()
                {
                    m_push_gift_id = push.id,
                    m_tex_name = push.background,
                    m_price_txt = GameEvents.IAPEvents.Sys_GetPriceEvent.SafeInvoke(push.chargeid),
                    m_bought = info.Buyed,
                    m_charge_id = push.chargeid,
                };

                m_datas.Add(data);
            }

            if (0 == m_datas.Count)
                return;

            m_cur_idx = 0;

            this.m_toggle_grid.EnsureSize<PushToggleItem>(m_datas.Count);

            for (int i = 0; i < m_toggle_grid.ChildCount; ++i)
            {
                var item = m_toggle_grid.GetChild<PushToggleItem>(i);
                item.Visible = true;
            }

            EnableIdx(m_cur_idx);
        }

        private void ShowAll(bool v_)
        {
            m_tex.Visible = v_;

            m_left_btn.Visible = v_;
            m_right_btn.Visible = v_;
            m_buy_btn.Visible = v_;
            if (!v_)
                m_toggle_grid.Clear();
            m_bought_img.Visible = v_;
        }

        public void DisableItem(long charge_id)
        {

            //m_datas.ForEach((item) => { if (item.m_charge_desc == charge_desc_ && 0 == ConfPush.Get(item.m_push_gift_id).bolckType) item.m_bought = true; });

            Debug.Log("gift view cur finish charge id = " + charge_id);

            PushGiftManager.Instance.BoughtLoginGift(charge_id);

            foreach (var item in m_datas)
            {
                Debug.Log("gift view cur item push id = " + item.m_push_gift_id);
                Debug.Log("gift view cur item charge id = " + item.m_charge_id);
                Debug.Log("gift view cur item block type = " + ConfPush.Get(item.m_push_gift_id).bolckType);

                if (item.m_charge_id == charge_id
                    && 0 == ConfPush.Get(item.m_push_gift_id).bolckType)
                    item.m_bought = true;
            }

            Debug.Log("cur idx = " + m_cur_idx);
            Debug.Log("cur selected desc = " + m_datas[m_cur_idx].m_charge_id);

            if (m_datas[m_cur_idx].m_charge_id == charge_id && m_datas[m_cur_idx].m_bought)
            {
                m_buy_btn.Visible = false;
                m_bought_img.Visible = true;
            }
        }

        private void EnableIdx(int idx_)
        {
            Debug.Log("cur selected idx = " + idx_);

            PushGiftData data = m_datas[idx_];

            m_tex.TextureName = data.m_tex_name;
            m_price_txt.Text = data.m_price_txt;

            if (data.m_bought)
            {
                m_buy_btn.Visible = false;
                m_bought_img.Visible = true;
            }
            else
            {
                m_buy_btn.Visible = true;
                m_bought_img.Visible = false;
            }

            PushToggleItem item = this.m_toggle_grid.GetChild<PushToggleItem>(idx_);
            item.EnableItem();
        }


        private void OnBuyClicked(GameObject obj)
        {
            EngineCoreEvents.AudioEvents.PlayAudio.SafeInvoke(Audio.AudioType.UISound, EngineCommonAudioKey.Button_Click_Common.ToString());

            long push_id = m_datas[m_cur_idx].m_push_gift_id;

            Debug.LogError(" touched push id = " + push_id);
            GameEvents.NetWorkEvents.SendMsg.SafeInvoke(new CSBuyPushRequest() { PushId = push_id });

        }

        private void OnLeftClicked(GameObject obj)
        {
            m_detail_root.Visible = false;

            if (null == m_datas)
                return;

            --m_cur_idx;

            if (m_cur_idx < 0)
            {
                m_cur_idx = m_datas.Count - 1;
            }

            EnableIdx(m_cur_idx);
        }

        private void OnRightClicked(GameObject obj)
        {
            m_detail_root.Visible = false;

            if (null == m_datas)
                return;

            ++m_cur_idx;

            if (m_cur_idx >= m_datas.Count)
            {
                m_cur_idx = 0;
            }

            EnableIdx(m_cur_idx);
        }

        private void OnCloseClicked(GameObject obj)
        {
            ENUM_PUSH_GIFT_BLOCK_TYPE push_type = PushGiftManager.Instance.GetTurnOnType();

            if (ENUM_PUSH_GIFT_BLOCK_TYPE.E_NONE == push_type)
            {
                this.Visible = false;
                return;
            }

            List<Push_Info> infos = PushGiftManager.Instance.GetPushInfosByTurnOnType(push_type);

            if (null == infos)
            {
                this.Visible = false;
                return;
            }

            this.Refresh(infos, OnClosed);
        }

        private void OnDetailClicked(GameObject obj)
        {
            m_detail_root.Visible = !m_detail_root.Visible;
            m_detail_txt.Text = LocalizeModule.Instance.GetString(ConfPush.Get(m_datas[m_cur_idx].m_push_gift_id).descs);
        }

        private void OnDetailCloseClicked(GameObject obj)
        {
            m_detail_root.Visible = false;
        }

    }
    public class PushGiftData
    {
        public long m_push_gift_id;
        public string m_price_txt;
        public string m_tex_name;
        public bool m_bought;
        public long m_charge_id;
    }
}

