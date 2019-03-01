using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EngineCore;
using UnityEngine;

namespace SeekerGame
{
    class FriendConfirmItemUI : GameUIComponent
    {

        GameLabel m_time_text;
        GameLabel m_info_text;


        private long m_player_id;


        protected override void OnInit()
        {
            m_time_text = this.Make<GameLabel>("Text_time");
            m_info_text = this.Make<GameLabel>("Text_detail");

        }


        public void Refresh(PlayerFriendMsg info_)
        {
            m_player_id = info_.PlayerId;
            System.DateTime dataTime = CommonTools.TimeStampToDateTime(info_.StatusTime * 10000);
            m_time_text.Text = string.Format("{0}", dataTime.ToString("yyyy.MM.dd"));

            m_info_text.Text = string.Format(LocalizeModule.Instance.GetString("friend_invite_ok", info_.Name));

        }
    }
}
