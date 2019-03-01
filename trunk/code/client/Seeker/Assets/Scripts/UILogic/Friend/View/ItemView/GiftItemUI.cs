using EngineCore;
using UnityEngine;

namespace SeekerGame
{
    class GiftItemUI : GameUIComponent
    {
        GameLabel m_name_text;
        GameLabel m_title_text;
        GameImage m_head_icon;
        GameNetworkRawImage m_head_tex;
        //GameImage m_gender_icon;
        GameLabel m_lvl_text;
        GameImage m_receive_gift_btn;
        GameLabel m_receive_gift_btn_text;
        GameImage m_received_gift_btn;
        GameLabel m_received_gift_btn_txt;

        private long m_gift_id;


        protected override void OnInit()
        {
            m_name_text = this.Make<GameLabel>("Text_name");
            m_title_text = this.Make<GameLabel>("Text_title");
            m_head_icon = this.Make<GameImage>("person:icon_btn:icon");
            m_head_tex = this.Make<GameNetworkRawImage>("person:icon_btn:RawImage_icon");
            //m_gender_icon = this.Make<GameImage>("Icon_Root:ImageG");
            m_lvl_text = this.Make<GameLabel>("person:Text_number");
            m_receive_gift_btn = this.Make<GameImage>("btn_receive");
            m_receive_gift_btn_text = m_receive_gift_btn.Make<GameLabel>("Text");
            m_receive_gift_btn_text.Text = LocalizeModule.Instance.GetString("friend_receive_1");
            m_received_gift_btn = this.Make<GameImage>("btn_received");
            m_received_gift_btn_txt = m_received_gift_btn.Make<GameLabel>("Text");
            m_received_gift_btn_txt.Text = LocalizeModule.Instance.GetString("cheng_jiu_1");

        }

        public override void OnShow(object param)
        {
            m_receive_gift_btn.AddClickCallBack(OnReceiveGiftClick);


        }

        public override void OnHide()
        {
            m_receive_gift_btn.RemoveClickCallBack(OnReceiveGiftClick);
        }


        public void Refresh(FriendGift g_info_)
        {
            m_gift_id = g_info_.GiftId;

            PlayerFriendMsg info_ = g_info_.PlayerFriends;
            m_name_text.Text = info_.Name;

            ConfTitle my_title = ConfTitle.Get(info_.TitleId);
            m_title_text.Text = null != my_title ? LocalizeModule.Instance.GetString(my_title.name) : "";

            if (info_.Icon.Contains("http://"))
            {
                m_head_icon.Visible = false;
                m_head_tex.Visible = true;
                m_head_tex.TextureName = info_.Icon;
            }
            else
            {
                m_head_icon.Visible = true;
                m_head_tex.Visible = false;
                m_head_icon.Sprite = info_.Icon;
            }
            //m_gender_icon.Sprite = CommonTools.GetGenderIcon(info_.Gender);
            m_lvl_text.Text = string.Format("LV{0}", info_.Level);


            m_receive_gift_btn.Enable = FriendDataManager.Instance.Receive_gift_left_num > 0;
            m_receive_gift_btn.SetGray(!(FriendDataManager.Instance.Receive_gift_left_num > 0));
            m_receive_gift_btn.Visible = true;
            m_received_gift_btn.Visible = false;
        }

        public void OnReceiveGiftClick(GameObject obj)
        {
            EngineCoreEvents.AudioEvents.PlayAudio.SafeInvoke(Audio.AudioType.UISound, GameCustomAudioKey.friend_getgift.ToString());
            m_receive_gift_btn.Enable = false;
            m_receive_gift_btn.SetGray(true);
            m_receive_gift_btn.Visible = false;
            m_received_gift_btn.Visible = true;
            GameEvents.UIEvents.UI_Friend_Event.OnGiftChanged.SafeInvoke(this.m_gift_id, ENUM_GIFT_CONTROL.E_ONE);
        }
    }
}
