using EngineCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace SeekerGame
{
    class FriendApplicationItemUI : GameUIComponent
    {
        GameLabel m_name_text;
        GameButton m_del_btn;
        GameNetworkRawImage m_head_tex;
        GameImage m_gender_icon;
        GameLabel m_lvl_text;
        GameButton m_ok_btn;


        private long m_player_id;


        protected override void OnInit()
        {
            m_name_text = this.Make<GameLabel>("Text_name");
            m_del_btn = this.Make<GameButton>("Button");
            m_head_tex = this.Make<GameNetworkRawImage>("Panel:RawImage");
            m_gender_icon = this.Make<GameImage>("ImageG");
            m_lvl_text = this.Make<GameLabel>("Text");
            m_ok_btn = this.Make<GameButton>("Buttongift");
        }

        public override void OnShow(object param)
        {
            m_del_btn.AddClickCallBack(OnDelClick);
            m_ok_btn.AddClickCallBack(OnOKClick);


        }

        public override void OnHide()
        {
            m_del_btn.RemoveClickCallBack(OnDelClick);
            m_ok_btn.RemoveClickCallBack(OnOKClick);
        }


        public void Refresh(PlayerFriendMsg info_)
        {
            m_player_id = info_.PlayerId;
            m_name_text.Text = info_.Name;

            FriendDataManager.Instance.SetHeadIcon(m_head_tex, info_.Icon, info_.PlayerId);
            m_gender_icon.Sprite = CommonTools.GetGenderIcon(info_.Gender);
            m_lvl_text.Text = string.Format("LV{0}", info_.Level);

        }

        public void OnDelClick(GameObject obj)
        {

            EngineCoreEvents.AudioEvents.PlayAudio.SafeInvoke(Audio.AudioType.UISound, EngineCommonAudioKey.Button_Click_Common.ToString());
            GameEvents.UIEvents.UI_Friend_Event.OnApplicationChanged.SafeInvoke(this.m_player_id, ENUM_APPLICATION_CONTROL.E_DEL);
        }

        public void OnOKClick(GameObject obj)
        {

            EngineCoreEvents.AudioEvents.PlayAudio.SafeInvoke(Audio.AudioType.UISound, GameCustomAudioKey.friend_consent.ToString());
            GameEvents.UIEvents.UI_Friend_Event.OnApplicationChanged.SafeInvoke(this.m_player_id, ENUM_APPLICATION_CONTROL.E_OK);
        }
    }
}
