using EngineCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace SeekerGame
{
    class FriendListUI : BaseViewComponet<FriendUILogic>
    {
        private readonly Vector2 C_TOGGLE_OFFSET = new Vector2(-20.0f, 0.0f);

        ToggleWithArrowTween m_view_gifts_toggle;
        ToggleWithArrowTween m_add_friend_toggle;
        ToggleWithArrowTween m_toggle_info;
        ToggleWithArrowTween m_toggle_application;
        ToggleWithArrowTween m_toggle_confirm;
        GameImage m_apply_red_point;
        GameImage m_confirm_red_point;
        GameImage m_gift_red_point;

        GameUIComponent m_friend_info_list_root;
        GameLabel m_friend_num_text;
        GameUIContainer m_friend_info_grid;


        GameUIComponent m_friend_application_root;
        GameUIContainer m_friend_application_grid;
        GameLabel m_friend_application_tips;

        GameSliderButton m_application_switch;
        GameImage m_ignore_all_btn;
        GameLabel m_ignore_all_btn_text;
        GameLabel m_application_tip_text;


        GameUIComponent m_friend_confirm_root;
        GameUIContainer m_friend_confirm_grid;

        AddFriendNewUI m_add_friend_root;
        GiftListNewUI m_gift_root;

        GameButton m_recommend_btn;
        GameLabel m_recommend_txt;
        GameLabel m_recommend_tips;

        private int m_last_toggle_id = -1;

        protected override void OnInit()
        {
            m_view_gifts_toggle = this.Make<ToggleWithArrowTween>("leftBtn:btn_GIFT");
            m_add_friend_toggle = this.Make<ToggleWithArrowTween>("leftBtn:btn_search");
            m_toggle_info = this.Make<ToggleWithArrowTween>("leftBtn:btn_friend");
            m_toggle_application = this.Make<ToggleWithArrowTween>("leftBtn:btn_application");
            m_toggle_confirm = this.Make<ToggleWithArrowTween>("leftBtn:btn_NOTICE");
            //m_toggle_seleted_effect = this.Make<GameUIEffect>("leftBtn:UI_haoyou");
            //m_toggle_seleted_effect.EffectPrefabName = "UI_haoyou.prefab";
            m_apply_red_point = this.Make<GameImage>("leftBtn:btn_application:Canvas:ImgWarn");
            m_confirm_red_point = this.Make<GameImage>("leftBtn:btn_NOTICE:Canvas:ImgWarn");
            m_gift_red_point = this.Make<GameImage>("leftBtn:btn_GIFT:Canvas:ImgWarn");
            m_friend_info_list_root = this.Make<GameUIComponent>("Panel_friend");
            m_friend_num_text = this.Make<GameLabel>("Panel_friend:Text_1");
            //m_friend_info_scrollview = this.Make<GameScrollView>("Panel_friend:Panel_friend");
            m_friend_info_grid = this.Make<GameUIContainer>("Panel_friend:Panel_friend:Content");

            m_friend_application_root = this.Make<GameUIComponent>("Panel_application");
            m_friend_application_grid = this.Make<GameUIContainer>("Panel_application:Panel_application:Content");
            m_friend_application_tips = m_friend_application_root.Make<GameLabel>("Text_Tips");
            m_friend_application_tips.Text = LocalizeModule.Instance.GetString("friend_ask_seven");
            m_application_switch = this.Make<GameSliderButton>("Panel_application:Toggle_1");
            m_ignore_all_btn = this.Make<GameImage>("Panel_application:btn_ignore");
            m_ignore_all_btn_text = m_ignore_all_btn.Make<GameLabel>("Text");
            m_ignore_all_btn_text.Text = LocalizeModule.Instance.GetString("friend_neglect_all");
            m_application_tip_text = m_friend_application_root.Make<GameLabel>("Text");
            m_application_tip_text.Text = LocalizeModule.Instance.GetString("friend_receive_ask");

            m_friend_confirm_root = this.Make<GameUIComponent>("Panel_notice");
            m_friend_confirm_grid = this.Make<GameUIContainer>("Panel_notice:Viewport");

            m_add_friend_root = this.Make<AddFriendNewUI>("Panel_search");
            m_gift_root = this.Make<GiftListNewUI>("Panel_gift");
            m_gift_root.Visible = false;

            m_recommend_btn = Make<GameButton>("Button");
            m_recommend_txt = m_recommend_btn.Make<GameLabel>("Text");
            m_recommend_txt.Text = LocalizeModule.Instance.GetString("friend_recommend");
            m_recommend_tips = m_recommend_btn.Make<GameLabel>("Text (1)");
            m_recommend_tips.Text = LocalizeModule.Instance.GetString("friend_recommend_dec");
        }

        public override void OnShow(object param)
        {
            ShowConfirmRedPoint(false);
            ShowApplyRedPoint(PlayerPrefTool.GetIsThereNewApply());
            ShowGiftRedPoint(PlayerPrefTool.GetIsThereNewGift());
            this.SwitchUI(FRIEND_UI_TOGGLE_TYPE.Added);

            m_toggle_info.Refresh((int)FRIEND_UI_TOGGLE_TYPE.Added, LocalizeModule.Instance.GetString("friend_1"), true, OnToggleClick);
            m_toggle_application.Refresh((int)FRIEND_UI_TOGGLE_TYPE.Agreeing, LocalizeModule.Instance.GetString("friend_2"), false, OnToggleClick);
            m_toggle_confirm.Refresh((int)FRIEND_UI_TOGGLE_TYPE.Addinfo, LocalizeModule.Instance.GetString("friend_3"), false, OnToggleClick);
            m_view_gifts_toggle.Refresh((int)FRIEND_UI_TOGGLE_TYPE.gift, LocalizeModule.Instance.GetString("friend_receive"), false, OnToggleClick);
            m_add_friend_toggle.Refresh((int)FRIEND_UI_TOGGLE_TYPE.scarch, LocalizeModule.Instance.GetString("friend_invite_btn"), false, OnToggleClick);
            m_application_switch.AddClickCallBack(OnApplicationToggleClicked);
            m_ignore_all_btn.AddClickCallBack(OnIgnoreAllClick);
            m_recommend_btn.AddClickCallBack(OnRecommendClick);

        }

        public override void OnHide()
        {
            m_last_toggle_id = -1;

            m_application_switch.RemoveClickCallBack(OnApplicationToggleClicked);
            m_ignore_all_btn.RemoveClickCallBack(OnIgnoreAllClick);
            m_recommend_btn.RemoveClickCallBack(OnRecommendClick);
        }

        public void Refresh(FRIEND_UI_TOGGLE_TYPE type_)
        {
            //this.SwitchUI(type_);

            ShowConfirmRedPoint(FriendDataManager.Instance.Last_apply);
            ShowApplyRedPoint(PlayerPrefTool.GetIsThereNewApply());

            m_recommend_btn.Visible = false;

            if (FRIEND_UI_TOGGLE_TYPE.Added == type_ || FRIEND_UI_TOGGLE_TYPE.Addinfo == type_ || FRIEND_UI_TOGGLE_TYPE.Agreeing == type_)
            {
                int temp_type = (int)type_;
                Dictionary<long, PlayerFriendMsg> datas = FriendDataManager.Instance.GetDatas((FriendReqType)temp_type);


                if (FRIEND_UI_TOGGLE_TYPE.Added == type_ && m_friend_info_list_root.Visible)
                {
                    if (null == datas)
                    {
                        m_friend_info_grid.Clear();
                        return;
                    }

                    List<PlayerFriendMsg> friends = new List<PlayerFriendMsg>(datas.Values);

                    friends.Sort((a, b) => { if (a.Level > b.Level) return -1; else return 1; });

                    m_friend_info_grid.EnsureSize<FriendInfoItemUI>(friends.Count);

                    for (int i = 0; i < m_friend_info_grid.ChildCount; ++i)
                    {
                        m_friend_info_grid.GetChild<FriendInfoItemUI>(i).Visible = false;
                        m_friend_info_grid.GetChild<FriendInfoItemUI>(i).Visible = true;
                        m_friend_info_grid.GetChild<FriendInfoItemUI>(i).Refresh(friends[i]);
                    }

                    m_friend_num_text.Text = string.Format("{0}/{1}", friends.Count, FriendDataManager.Instance.Max_friend_num);

                    m_recommend_btn.Visible = 0 == friends.Count;
                    m_recommend_tips.Visible = m_recommend_btn.Visible;

                }
                else if (FRIEND_UI_TOGGLE_TYPE.Agreeing == type_ && m_friend_application_root.Visible)
                {
                    if (m_application_switch.Checked != FriendDataManager.Instance.Is_receive_application)
                        m_application_switch.Checked = FriendDataManager.Instance.Is_receive_application;

                    if (null == datas || 0 == datas.Count)
                    {
                        m_friend_application_grid.Clear();
                        m_ignore_all_btn.Enable = false;
                        m_ignore_all_btn.SetGray(true);
                        return;
                    }

                    m_ignore_all_btn.Enable = true;
                    m_ignore_all_btn.SetGray(false);

                    List<PlayerFriendMsg> friends = new List<PlayerFriendMsg>(datas.Values);

                    friends.Sort((a, b) => { if (a.StatusTime > b.StatusTime) return -1; else return 1; });

                    m_friend_application_grid.EnsureSize<FriendApplicationItemUI>(friends.Count);

                    for (int i = 0; i < m_friend_application_grid.ChildCount; ++i)
                    {
                        m_friend_application_grid.GetChild<FriendApplicationItemUI>(i).Visible = false;
                        m_friend_application_grid.GetChild<FriendApplicationItemUI>(i).Visible = true;
                        m_friend_application_grid.GetChild<FriendApplicationItemUI>(i).Refresh(friends[i]);
                    }


                }
                else if (FRIEND_UI_TOGGLE_TYPE.Addinfo == type_ && m_friend_confirm_root.Visible)
                {
                    if (null == datas)
                    {
                        m_friend_confirm_grid.Clear();
                        return;
                    }

                    List<PlayerFriendMsg> friends = new List<PlayerFriendMsg>(datas.Values);

                    friends.Sort((a, b) => { if (a.StatusTime > b.StatusTime) return -1; else return 1; });

                    m_friend_confirm_grid.EnsureSize<FriendConfirmItemUI>(friends.Count);

                    for (int i = 0; i < m_friend_confirm_grid.ChildCount; ++i)
                    {
                        m_friend_confirm_grid.GetChild<FriendConfirmItemUI>(i).Visible = false;
                        m_friend_confirm_grid.GetChild<FriendConfirmItemUI>(i).Visible = true;
                        m_friend_confirm_grid.GetChild<FriendConfirmItemUI>(i).Refresh(friends[i]);
                    }
                }
            }
            else if (FRIEND_UI_TOGGLE_TYPE.gift == type_)
            {
                m_gift_root.Refresh();
            }
        }

        public void ShowContent(bool v_)
        {
            m_friend_info_list_root.Visible = false;
            m_friend_application_root.Visible = false;
            m_friend_confirm_root.Visible = false;
            m_add_friend_root.Visible = false;
            m_gift_root.Visible = false;

            if (v_)
            {
                ShowConfirmRedPoint(false);
                ShowApplyRedPoint(PlayerPrefTool.GetIsThereNewApply());
                ShowGiftRedPoint(PlayerPrefTool.GetIsThereNewGift());
                this.SwitchUI(FRIEND_UI_TOGGLE_TYPE.Added);
            }
        }

        private void SwitchUI(FRIEND_UI_TOGGLE_TYPE type_)
        {
            m_friend_info_list_root.Visible = false;
            m_friend_application_root.Visible = false;
            m_friend_confirm_root.Visible = false;
            m_add_friend_root.Visible = false;
            m_gift_root.Visible = false;
            m_recommend_btn.Visible = false;

            switch (type_)
            {
                case FRIEND_UI_TOGGLE_TYPE.Added:
                    m_friend_info_list_root.Visible = true;
                    break;
                case FRIEND_UI_TOGGLE_TYPE.Agreeing:
                    m_friend_application_root.Visible = true;
                    break;
                case FRIEND_UI_TOGGLE_TYPE.Addinfo:
                    m_friend_confirm_root.Visible = true;
                    break;
                case FRIEND_UI_TOGGLE_TYPE.gift:
                    m_gift_root.Visible = true;
                    break;
                case FRIEND_UI_TOGGLE_TYPE.scarch:
                    {
                        m_add_friend_root.Visible = true;
                        m_recommend_btn.Visible = true;
                        m_recommend_tips.Visible = false;
                    }
                    break;
            }
        }


        private void ShowConfirmRedPoint(bool b_)
        {
            if (b_)
            {
                m_confirm_red_point.Visible = true;
            }
            else
            {
                m_confirm_red_point.Visible = false;
            }
        }

        private void ShowGiftRedPoint(bool b_)
        {
            if (b_)
            {
                m_gift_red_point.Visible = true;
            }
            else
            {
                m_gift_red_point.Visible = false;
            }
        }

        private void ShowApplyRedPoint(bool b_)
        {
            if (b_)
            {
                m_apply_red_point.Visible = true;
            }
            else
            {
                m_apply_red_point.Visible = false;
            }
        }

        private void ShowGiftView()
        {
            GameEvents.RedPointEvents.Sys_OnNewGiftReadedEvent.SafeInvoke();
            ShowGiftRedPoint(PlayerPrefTool.GetIsThereNewGift());

            this.Refresh(FRIEND_UI_TOGGLE_TYPE.gift);
            GameEvents.RedPointEvents.Sys_OnNewFriendReadedEvent.SafeInvoke();

            CurViewLogic().RequestViewGift();
        }

        //public void OnAddFriendClick(GameObject obj)
        //{
        //    EngineCoreEvents.AudioEvents.PlayAudio.SafeInvoke(Audio.AudioType.UISound,GameCustomAudioKey.friend_addfriend.ToString(), null);
        //    this.ShowSearchView();
        //}

        //private void ShowSearchView()
        //{
        //    CurViewLogic().SwitchUI(ENUM_FRIEND_VIEW_TYPE.E_ADD_FRIEND);
        //}

        void OnApplicationToggleChanged(bool val_)
        {
            //PlayerPrefTool.SetFriendApplication(val_);
            FriendDataManager.Instance.Is_receive_application = val_;
            CurViewLogic().RequestSwitchApplication();
        }

        void OnApplicationToggleClicked(GameObject obj)
        {
            EngineCoreEvents.AudioEvents.PlayAudio.SafeInvoke(Audio.AudioType.UISound, GameCustomAudioKey.friend_consentapply.ToString());
            FriendDataManager.Instance.Is_receive_application = !FriendDataManager.Instance.Is_receive_application;
            CurViewLogic().RequestSwitchApplication();
        }

        void OnIgnoreAllClick(GameObject obj)
        {

            EngineCoreEvents.AudioEvents.PlayAudio.SafeInvoke(Audio.AudioType.UISound, EngineCommonAudioKey.Button_Click_Common.ToString());
            GameEvents.UIEvents.UI_Friend_Event.OnApplicationChanged.SafeInvoke(0, ENUM_APPLICATION_CONTROL.E_DEL_ALL);
        }

        private void OnRecommendClick(GameObject obj)
        {
            GameEvents.UIEvents.UI_Friend_Event.Listen_ShowView.SafeInvoke(ENUM_FRIEND_VIEW_TYPE.E_RECOMMEND);
        }

        private void OnToggleClick(bool val_, int id_)
        {
            if (val_)
            {
                if (-1 != m_last_toggle_id)
                    EngineCoreEvents.AudioEvents.PlayAudio.SafeInvoke(Audio.AudioType.UISound, EngineCommonAudioKey.table_change.ToString());

                if (m_last_toggle_id == id_)
                    return;

                m_last_toggle_id = id_;

                FRIEND_UI_TOGGLE_TYPE cur_type = (FRIEND_UI_TOGGLE_TYPE)id_;
                SwitchUI(cur_type);

                if (FRIEND_UI_TOGGLE_TYPE.Added == cur_type || FRIEND_UI_TOGGLE_TYPE.Addinfo == cur_type || FRIEND_UI_TOGGLE_TYPE.Agreeing == cur_type)
                {
                    GameEvents.UIEvents.UI_Friend_Event.OnRefreshFriendPage.SafeInvoke((FriendReqType)id_);
                    this.Refresh(cur_type);
                }
                else if (FRIEND_UI_TOGGLE_TYPE.gift == (FRIEND_UI_TOGGLE_TYPE)id_)
                {
                    this.ShowGiftView();
                }
            }
        }
    }

}
