using EngineCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace SeekerGame
{
    class FriendInfoItemUI : GameUIComponent
    {
        GameImage m_detail_btn;
        GameLabel m_name_text;
        GameLabel m_title_text;
        GameButton m_del_btn;
        GameNetworkRawImage m_head_tex;
        GameImage m_gender_icon;
        GameLabel m_lvl_text;
        GameImage m_send_gift_btn;


        private long m_player_id;


        protected override void OnInit()
        {
            m_detail_btn = this.Make<GameImage>("Button_Detail");
            m_name_text = this.Make<GameLabel>("Text_name");
            m_title_text = this.Make<GameLabel>("Text_title");
            m_del_btn = this.Make<GameButton>("Button");
            m_head_tex = this.Make<GameNetworkRawImage>("Panel:RawImage");
            m_gender_icon = this.Make<GameImage>("ImageG");
            m_gender_icon.Visible = false;
            m_lvl_text = this.Make<GameLabel>("Text");
            m_send_gift_btn = this.Make<GameImage>("Buttongift");
        }

        public override void OnShow(object param)
        {
            m_detail_btn.AddClickCallBack(OnDetailClick);
            m_del_btn.AddClickCallBack(OnDelClick);
            m_send_gift_btn.AddClickCallBack(OnSendGiftClick);


        }

        public override void OnHide()
        {
            m_detail_btn.RemoveClickCallBack(OnDetailClick);
            m_del_btn.RemoveClickCallBack(OnDelClick);
            m_send_gift_btn.RemoveClickCallBack(OnSendGiftClick);
        }


        public void Refresh(PlayerFriendMsg info_)
        {
            m_player_id = info_.PlayerId;
            m_name_text.Text = info_.Name;
            ConfTitle my_title = ConfTitle.Get(info_.TitleId);

            m_title_text.Text = null != my_title ? LocalizeModule.Instance.GetString(my_title.name) : "";

            FriendDataManager.Instance.SetHeadIcon(m_head_tex, info_.Icon, info_.PlayerId);


            m_gender_icon.Sprite = CommonTools.GetGenderIcon(info_.Gender);
            m_lvl_text.Text = string.Format("LV{0}", info_.Level);

            bool enable_gift = FriendDataManager.Instance.Send_gift_left_num <= 0 ? false : info_.Gift;

            m_send_gift_btn.Enable = enable_gift;
            m_send_gift_btn.SetGray(!enable_gift);
        }

        public void OnDetailClick(GameObject obj)
        {
            EngineCoreEvents.AudioEvents.PlayAudio.SafeInvoke(Audio.AudioType.UISound, GameCustomAudioKey.friend_heads.ToString());
            GameEvents.UIEvents.UI_Friend_Event.OnInfoChanged.SafeInvoke(this.m_player_id, ENUM_INFO_CONTROL.E_DETAIL);
        }

        public void OnDelClick(GameObject obj)
        {
            EngineCoreEvents.AudioEvents.PlayAudio.SafeInvoke(Audio.AudioType.UISound, EngineCommonAudioKey.Button_Click_Common.ToString());
            DelPopUp();
        }

        public void OnSendGiftClick(GameObject obj)
        {

            EngineCoreEvents.AudioEvents.PlayAudio.SafeInvoke(Audio.AudioType.UISound, GameCustomAudioKey.friend_gift.ToString());
            if (FriendDataManager.Instance.SendGiftLeftNumReduceOne() >= 0)
            {
                this.m_send_gift_btn.Enable = false;
                this.m_send_gift_btn.SetGray(true);

            }

            GameEvents.UIEvents.UI_Friend_Event.OnInfoChanged.SafeInvoke(this.m_player_id, ENUM_INFO_CONTROL.E_GIFT);
        }


        private void DelPopUp()
        {
            PopUpData pd = new PopUpData();
            pd.title = string.Empty;
            pd.content = "friend_delete";
            pd.isOneBtn = false;
            pd.OneButtonText = "shop_no";
            pd.twoStr = "UI.OK";
            pd.oneAction = null;
            pd.twoAction = delegate ()
            {

                EngineCoreEvents.AudioEvents.PlayAudio.SafeInvoke(Audio.AudioType.UISound, GameCustomAudioKey.friend_unfriend.ToString());
                GameEvents.UIEvents.UI_Friend_Event.OnInfoChanged.SafeInvoke(this.m_player_id, ENUM_INFO_CONTROL.E_DEL);
            };

            PopUpManager.OpenPopUp(pd);
        }

        private void IconLoaded(Texture tex_)
        {

        }
    }
}
