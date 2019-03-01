using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EngineCore;
using UnityEngine;

namespace SeekerGame
{
    class RecommendItemUI : GameUIComponent
    {
        /*
          string name=1;
	//fb绑定 ,0:未绑定，1fb 绑定
	int32 bind=2;
	//好友推荐状态，1：推荐，2：加好友
	int32 status=3;
	int32 level=4;
	string icon=5;
          */

        private GameLabel m_name_txt;
        private GameLabel m_lvl_txt;
        private GameNetworkRawImage m_head_tex;
        private GameImage m_fb_icon;
        private GameImage m_add_btn;
        private GameLabel m_add_btn_txt;
        private GameImage m_mask_img;
        private long m_player_id;
        private int m_fb_bind_state;
        private ENUM_RECOMMEND_STATUS m_rcommend_state;

        protected override void OnInit()
        {
            base.OnInit();
            m_name_txt = Make<GameLabel>("Image:Text");
            m_lvl_txt = Make<GameLabel>("Image:lv");
            m_head_tex = Make<GameNetworkRawImage>("Panel:RawImage");
            m_add_btn = Make<GameImage>("Button");
            m_add_btn_txt = m_add_btn.Make<GameLabel>("Text");
            m_add_btn_txt.Text = LocalizeModule.Instance.GetString("friend_invite_btn");
            m_mask_img = Make<GameImage>("Image_Mask");
            m_fb_icon = Make<GameImage>("Image_fb");
        }

        public override void OnShow(object param)
        {
            base.OnShow(param);

            m_add_btn.AddClickCallBack(OnAddBtnClicked);
            m_head_tex.AddClickCallBack(OnPlayerIconClicked);

            GameEvents.UIEvents.UI_Friend_Event.Tell_add_recommend_firend_ok += OnChangeRecommendStatus;
        }

        public override void OnHide()
        {
            base.OnHide();

            m_add_btn.RemoveClickCallBack(OnAddBtnClicked);
            m_head_tex.RemoveClickCallBack(OnPlayerIconClicked);

            GameEvents.UIEvents.UI_Friend_Event.Tell_add_recommend_firend_ok -= OnChangeRecommendStatus;
        }


        public void Refresh(FriendRecommendMsg info_)
        {
            m_fb_bind_state = info_.Bind;
            m_fb_icon.Visible = 1 == m_fb_bind_state;
            m_rcommend_state = (ENUM_RECOMMEND_STATUS)info_.Status;
            m_player_id = info_.RecommendId;

            m_add_btn.Visible = (int)ENUM_RECOMMEND_STATUS.E_RECOMMEND == info_.Status;
            m_mask_img.Visible = !m_add_btn.Visible;
            m_name_txt.Text = info_.Name;
            m_lvl_txt.Text = info_.Level.ToString();

            FriendDataManager.Instance.SetHeadIcon(m_head_tex, info_.Icon, info_.RecommendId);

        }

        private void RefreshRecommendStatus(ENUM_RECOMMEND_STATUS status_)
        {
            m_rcommend_state = status_;
            m_add_btn.Visible = ENUM_RECOMMEND_STATUS.E_RECOMMEND == status_;
            m_mask_img.Visible = !m_add_btn.Visible;
        }

        private void OnChangeRecommendStatus(long player_id_)
        {
            if (m_player_id != player_id_)
                return;

            RefreshRecommendStatus(ENUM_RECOMMEND_STATUS.E_ADDED);

        }

        private void OnAddBtnClicked(GameObject obj)
        {
            GameEvents.UIEvents.UI_Friend_Event.Listen_add_recommend_friend.SafeInvoke(new List<long>() { m_player_id });
        }

        private void OnPlayerIconClicked(GameObject obj)
        {
            GameEvents.UIEvents.UI_Friend_Event.Listen_check_recommend_info.SafeInvoke(m_player_id);
        }

    }
}
