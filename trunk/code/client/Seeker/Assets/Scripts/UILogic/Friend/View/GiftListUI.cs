//#define TEST
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EngineCore;
using UnityEngine;

namespace SeekerGame
{
    class GiftListUI : BaseViewComponet<FriendUILogic>
    {
        //private GameLabel m_title_text;
        private GameUIContainer m_gift_grid;
        //private GameScrollView m_gift_scroll;
        private GameButton m_receive_all_btn;
        private GameLabel m_receive_all_btn_text;
        //private GameLabel m_gift_num_text;

        private List<FriendGift> gifts;
        protected override void OnInit()
        {
            //    m_title_text = this.Make<GameLabel>("Panel_tipsanimate:Text_title");
            //    m_title_text.Text = LocalizeModule.Instance.GetString("friend_receive");
            m_gift_grid = this.Make<GameUIContainer>("Panel_tipsanimate:Panel_friend:Panel_friend-received:Content");
            //m_gift_scroll = this.Make<GameScrollView>("Panel_friend:Content");
            m_receive_all_btn = this.Make<GameButton>("Panel_tipsanimate:btn_1");
            m_receive_all_btn.SetGray(true);
            m_receive_all_btn_text = m_receive_all_btn.Make<GameLabel>("Text");
            m_receive_all_btn_text.Text = LocalizeModule.Instance.GetString("friend_receive_all");
            //m_gift_num_text = this.Make<GameLabel>("Panel_tipsanimate:Text_detail");
            this.SetCloseBtnID("Panel_tipsanimate:Button_close");
        }

        public override void OnShow(object param)
        {
            m_receive_all_btn.AddClickCallBack(OnReceiveAllClick);
#if TEST
            this.Refresh();
#else
            m_gift_grid.Clear();
#endif
        }

        public override void OnHide()
        {
            m_receive_all_btn.RemoveClickCallBack(OnReceiveAllClick);
        }


        public void Refresh()
        {

            Dictionary<long, FriendGift> datas = FriendDataManager.Instance.GetGifts();
            int gift_num = datas.Count;
            int receive_num = FriendDataManager.Instance.Receive_gift_left_num;

            m_receive_all_btn.Enable = gift_num > 0 && receive_num > 0;
            m_receive_all_btn.SetGray(!(gift_num > 0 && receive_num > 0));
            //m_gift_num_text.Text = string.Format("{0}/{1}", FriendDataManager.Instance.Receive_gift_max_num - FriendDataManager.Instance.Receive_gift_left_num, FriendDataManager.Instance.Receive_gift_max_num);
            if (null == datas || 0 == datas.Count)
            {
                m_gift_grid.Clear();
                return;
            }

            gifts = new List<FriendGift>(datas.Values);

            gifts.Sort((a, b) => { if (a.PlayerFriends.StatusTime > b.PlayerFriends.StatusTime) return -1; else return 1; });

            m_gift_grid.EnsureSize<GiftItemUI>(gifts.Count);
            //m_gift_scroll.ScrollToTop();
            for (int i = 0; i < m_gift_grid.ChildCount; ++i)
            {
                m_gift_grid.GetChild<GiftItemUI>(i).Visible = false;
                m_gift_grid.GetChild<GiftItemUI>(i).Visible = true;
                m_gift_grid.GetChild<GiftItemUI>(i).Refresh(gifts[i]);
            }




        }

        private void OnReceiveAllClick(GameObject obj_)
        {
            EngineCoreEvents.AudioEvents.PlayAudio.SafeInvoke(Audio.AudioType.UISound, EngineCommonAudioKey.Button_Click_Common.ToString());

            this.m_gift_grid.Clear();

            GameEvents.UIEvents.UI_Friend_Event.OnGiftChanged.SafeInvoke(0, ENUM_GIFT_CONTROL.E_ALL);
        }
    }
}
