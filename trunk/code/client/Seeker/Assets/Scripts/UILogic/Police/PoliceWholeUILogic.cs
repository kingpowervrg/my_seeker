#if OFFICER_SYS
using EngineCore;
using GOEngine;
using GOGUI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SeekerGame
{
    [UILogicHandler(UIDefine.UI_POLICE)]
    public class PoliceWholeUILogic : UILogicBase
    {

        private enum ENUM_TWEEN_PROGRESS
        {
            E_INVALID = -1,
            E_UPGRADE_TO_DETAIL = 0,
            E_DETAIL_TO_UPGRADE,
        }

        private enum ENUM_RIGHT_WINDOW_TYPE
        {
            E_INVALID = -1,
            E_UPGRADE = 0,
            E_DETAIL = 1,
        }

        private enum ENUM_SELECT_TYPE
        {
            NONE, //没有任何警员被选中,默认选择第一个
            SELECTED, //有警员被选中
            RESELECT, //强制选择当前打开升级面板的
        }

        private readonly Color R_TOGGLE_DISABLE_COLOR = new Color(151.0f / 255.0f, 181.0f / 255.0f, 194.0f / 255.0f);

        private GameUIComponent m_panel_down;
        private PoliceUpgradeUI m_upgrade_window;
        private PoliceDetailUI m_detail_window;


        List<string> m_btnPage_Str;
#region 左边按钮
        private List<ToggleWithArrowTween> m_page_toggles;
        private List<GameImage> m_page_red_points;
#endregion

#region 中间信息
        private GameScrollView m_scroll_view;
        //private GameTileLoop m_unlock_grid;
        private GameLoopUIContainer m_lock_grid;
        private GameLabel m_NothingTip_lab;
#endregion

#region 右边旋转控制
        private TweenScale m_right_window_tween;
        private ENUM_TWEEN_PROGRESS m_cur_tween_progress;
#endregion

#region 数据
        ENUM_PAGE_TYPE m_currentPageType;
        private long m_choose_officer_id;
        private long m_last_choose_officer_id = -1;
        ENUM_RIGHT_WINDOW_TYPE m_cur_right_type;
        private long m_data_timestamp = 0L;

        //E_SPECIAL_POLICE, //特警
        //E_PATROL_MEN, //巡警
        //E_INSPECTOR, //探长
        //E_BAU,
        //E_CSI,
        //E_FORENSIC, //法医

        private List<UIOfficerInfo> m_all_officer;
        private List<UIOfficerInfo> m_spcial_polices;
        private List<UIOfficerInfo> m_patrol_men;
        private List<UIOfficerInfo> m_inspectors;
        private List<UIOfficerInfo> m_baus;
        private List<UIOfficerInfo> m_csis;
        private List<UIOfficerInfo> m_forensics;



#endregion




        protected override void OnInit()
        {
            base.OnInit();
            IsFullScreen = true;
            PreUILogic();
            //GameEvents.UI_Guid_Event.GetPoliceItemById = GetPoliceItemById;
            m_panel_down = this.Make<GameUIComponent>("Panel_animation");
            m_upgrade_window = this.Make<PoliceUpgradeUI>("Panel_animation:Right_Root:Upgrade");
            m_upgrade_window.RegisterBack(OpenDetailWindow);

            m_detail_window = this.Make<PoliceDetailUI>("Panel_animation:Right_Root:detail");
            m_detail_window.RegisterBack(OpenUpgradeWindow);


            this.SetCloseBtnID("Panel_animation:Button_close");
        }

        public override void OnShow(object param)
        {
            base.OnShow(param);
            this.AddUILogic();

            MessageHandler.RegisterMessageHandler(MessageDefine.SCOfficerCombineResponse, OnScResponse);
            MessageHandler.RegisterMessageHandler(MessageDefine.SCOfficerUpdateResponse, OnScResponse);
            MessageHandler.RegisterMessageHandler(MessageDefine.SCOfficerListResponse, OnScResponse);
            MessageHandler.RegisterMessageHandler(MessageDefine.SCPlayerPropResponse, OnScResponse);

            UserBehaviorStatisticsModules.Instance.LogEvent(UBSEventKeyName.police_in, 1.0f, null);

        }

        public override void OnHide()
        {
            base.OnHide();
            this.RemoveUILogic();

            MessageHandler.UnRegisterMessageHandler(MessageDefine.SCOfficerCombineResponse, OnScResponse);
            MessageHandler.UnRegisterMessageHandler(MessageDefine.SCOfficerUpdateResponse, OnScResponse);
            MessageHandler.UnRegisterMessageHandler(MessageDefine.SCOfficerListResponse, OnScResponse);
            MessageHandler.UnRegisterMessageHandler(MessageDefine.SCPlayerPropResponse, OnScResponse);

            //m_pageUIEffect.ForEach(item => item.Visible = false);
        }

        private Transform GetPoliceItemById(long id)
        {
            for (int i = 0; i < m_lock_grid.ChildCount; i++)
            {
                ToggleIconItem iconItem = m_lock_grid.GetChild<ToggleIconItem>(i);
                if (iconItem != null)
                {
                    if ((long)iconItem.Info == id)
                    {
                        return iconItem.Widget;
                    }
                }

            }
            return null;
        }

        void PreUILogic()
        {
#region 数据
            RefreshOfficerDatas();
#endregion


#region 控件

            //this.m_right_window_tween.SetOnFinished(OnFinishRightWindowTween);
            m_btnPage_Str = new List<string>();

            m_btnPage_Str.Add(LocalizeModule.Instance.GetString("UI_Police.all"));
            m_btnPage_Str.Add(LocalizeModule.Instance.GetString("UI_Police.special"));
            m_btnPage_Str.Add(LocalizeModule.Instance.GetString("UI_Police.patrol"));
            m_btnPage_Str.Add(LocalizeModule.Instance.GetString("UI_Police.inspector"));
            m_btnPage_Str.Add(LocalizeModule.Instance.GetString("UI_Police.bau"));
            m_btnPage_Str.Add(LocalizeModule.Instance.GetString("UI_Police.csi"));
            m_btnPage_Str.Add(LocalizeModule.Instance.GetString("UI_Police.forensic"));

            m_page_toggles = new List<ToggleWithArrowTween>();
            m_page_red_points = new List<GameImage>();
            for (int i = 0; i < m_btnPage_Str.Count; i++)
            {
                ToggleWithArrowTween tb = Make<ToggleWithArrowTween>(string.Format("Panel_animation:leftBtn:btn{0}", i));

                m_page_toggles.Add(tb);
                tb.Refresh(i, m_btnPage_Str[i], 0 == i ? true : false, ChangePageType);
                m_page_red_points.Add(Make<GameImage>(string.Format("Panel_animation:leftBtn:btn{0}:ImgWarn", i)));
            }

            m_scroll_view = Make<GameScrollView>("Panel_animation:Panel");
            //m_unlock_grid = Make<GameTileLoop>("Panel_animation:Panel:grid:grid_unlock");
            //m_unlock_grid.onUpdateItem += (a, b) =>
            //{
            //    Debug.Log(a);
            //};
            m_lock_grid = Make<GameLoopUIContainer>("Panel_animation:Panel:grid:grid_lock");
            m_lock_grid.onUpdateItem += (a, b) => { Debug.Log(a); };
#endregion
        }

        private void RefreshOfficerDatas()
        {
            if (m_data_timestamp == GlobalInfo.MY_PLAYER_INFO.OfficerTimestamp)
            {
                return;
            }
            m_data_timestamp = GlobalInfo.MY_PLAYER_INFO.OfficerTimestamp;

            m_all_officer = PoliceUILogicAssist.GetOfficerByPageType(ENUM_PAGE_TYPE.E_ALL);
            m_spcial_polices = PoliceUILogicAssist.GetOfficerByPageType(ENUM_PAGE_TYPE.E_SPECIAL_POLICE);
            m_patrol_men = PoliceUILogicAssist.GetOfficerByPageType(ENUM_PAGE_TYPE.E_PATROL_MEN);
            m_inspectors = PoliceUILogicAssist.GetOfficerByPageType(ENUM_PAGE_TYPE.E_INSPECTOR);
            m_baus = PoliceUILogicAssist.GetOfficerByPageType(ENUM_PAGE_TYPE.E_BAU);
            m_csis = PoliceUILogicAssist.GetOfficerByPageType(ENUM_PAGE_TYPE.E_CSI);
            m_forensics = PoliceUILogicAssist.GetOfficerByPageType(ENUM_PAGE_TYPE.E_FORENSIC);
        }

        private void AddUILogic()
        {
            m_currentPageType = ENUM_PAGE_TYPE.E_INVALID;
            this.m_choose_officer_id = 0;
            m_last_choose_officer_id = -1;
            m_cur_right_type = ENUM_RIGHT_WINDOW_TYPE.E_UPGRADE;

            this.m_page_toggles[0].Checked = false;
            this.m_page_toggles[0].Checked = true;


            RefreshRedPoints();
        }

        private void RemoveUILogic()
        {
            m_currentPageType = ENUM_PAGE_TYPE.E_INVALID;
        }

        private void RefreshRedPoints()
        {
            this.PreRefreshRedPoints();
            this.RefreshLeftRedPoints();
            this.RefreshRightRedPoints();
        }

        private void PreRefreshRedPoints()
        {
            RedPointManager.Instance.RefreshAllOfficersListCouldCombine();
        }

        private void RefreshLeftRedPoints()
        {
            m_page_red_points.ForEach((item) => { item.Visible = false; });

            bool is_red = false;

            foreach (var item in m_spcial_polices)
            {
                if (RedPointManager.Instance.IsThisOfficerInCombinedList(item.m_data.id))
                {
                    m_page_red_points[(int)ENUM_PAGE_TYPE.E_SPECIAL_POLICE].Visible = true;
                    is_red = true;
                }

            }

            foreach (var item in m_patrol_men)
            {
                if (RedPointManager.Instance.IsThisOfficerInCombinedList(item.m_data.id))
                {
                    m_page_red_points[(int)ENUM_PAGE_TYPE.E_PATROL_MEN].Visible = true;
                    is_red = true;
                }

            }

            foreach (var item in m_inspectors)
            {
                if (RedPointManager.Instance.IsThisOfficerInCombinedList(item.m_data.id))
                {
                    m_page_red_points[(int)ENUM_PAGE_TYPE.E_INSPECTOR].Visible = true;
                    is_red = true;
                }

            }

            foreach (var item in m_baus)
            {
                if (RedPointManager.Instance.IsThisOfficerInCombinedList(item.m_data.id))
                {
                    m_page_red_points[(int)ENUM_PAGE_TYPE.E_BAU].Visible = true;
                    is_red = true;
                }

            }

            foreach (var item in m_csis)
            {
                if (RedPointManager.Instance.IsThisOfficerInCombinedList(item.m_data.id))
                {
                    m_page_red_points[(int)ENUM_PAGE_TYPE.E_CSI].Visible = true;
                    is_red = true;
                }

            }

            foreach (var item in m_forensics)
            {
                if (RedPointManager.Instance.IsThisOfficerInCombinedList(item.m_data.id))
                {
                    m_page_red_points[(int)ENUM_PAGE_TYPE.E_FORENSIC].Visible = true;
                    is_red = true;
                }

            }

            if (is_red)
            {
                m_page_red_points[(int)ENUM_PAGE_TYPE.E_ALL].Visible = true;
            }

        }

        private void RefreshRightRedPoints()
        {
            //RefreshRightRedPoints(m_unlock_grid);
            //RefreshRightRedPoints(m_lock_grid);
        }

        private void RefreshRightRedPoints(GameLoopUIContainer grid_)
        {
            for (int i = 0; i < grid_.ChildCount; ++i)
            {
                var item = grid_.GetChild<ToggleIconItem>(i);

                long officer_id = (long)item.Info;

                if (RedPointManager.Instance.IsThisOfficerInCombinedList(officer_id))
                {
                    item.RedPointVisible = true;
                }
                else
                {
                    item.RedPointVisible = false;
                }
            }
        }



        //void BtnPageClick(int i, ToggleWithArrowTween btn)
        //{

        //    btn.AddChangeCallBack((v_) =>
        //    {
        //        ChangePageType(i, v_);
        //    });
        //}

        void ChangePageType(bool value, int i)
        {
            if (value)
            {
                if (ENUM_PAGE_TYPE.E_INVALID != m_currentPageType)
                    EngineCoreEvents.AudioEvents.PlayAudio.SafeInvoke(Audio.AudioType.UISound, EngineCommonAudioKey.table_change.ToString());
                if (m_currentPageType == (ENUM_PAGE_TYPE)i)
                {
                    return;
                }
                m_currentPageType = (ENUM_PAGE_TYPE)i;
                this.m_choose_officer_id = 0;
                m_last_choose_officer_id = -1;

            }
            ChangePage(i, value);
        }

        void ChangePage(int i, bool value)
        {
            if (value)
            {

                this.m_detail_window.Visible = false;

                this.RefreshOfficerDatas();
                this.Refresh(this.GetData(m_currentPageType), ENUM_SELECT_TYPE.NONE);

            }


        }





        private void OnScResponse(object s)
        {
            if (s is SCOfficerCombineResponse)
            {
                DebugUtil.Log("SCOfficerCombineResponse");
                var rsp = s as SCOfficerCombineResponse;

                if (1 != rsp.Result)
                {
                    DebugUtil.LogError("雇佣警员失败");
                }
                else
                {

                    ReqServerOfficerInfo();


                    Dictionary<UBSParamKeyName, object> _params = new Dictionary<UBSParamKeyName, object>()
                    {
                        { UBSParamKeyName.ContentID, rsp.OfficeId},
                    };

                    UserBehaviorStatisticsModules.Instance.LogEvent(UBSEventKeyName.police_get, null, _params);
                }
            }
            else if (s is SCOfficerUpdateResponse)
            {
                DebugUtil.Log("SCOfficerUpdateResponse");

                var rsp = s as SCOfficerUpdateResponse;

                if (1 != rsp.Result)
                {
                    DebugUtil.LogError("升级警员失败");
                }
                else
                {

                    Dictionary<UBSParamKeyName, object> _params = new Dictionary<UBSParamKeyName, object>()
                    {
                        { UBSParamKeyName.ContentID, rsp.OfficeId},
                        { UBSParamKeyName.NumItems, rsp.Level},

                    };

                    UserBehaviorStatisticsModules.Instance.LogEvent(UBSEventKeyName.police_promote, null, _params);

                    ReqServerOfficerInfo();


                }
            }
            else if (s is SCOfficerListResponse)
            {
                DebugUtil.Log("SCOfficerListResponse");

                var rsp = s as SCOfficerListResponse;

                GlobalInfo.MY_PLAYER_INFO.SetOfficerInfos(rsp.Officers);


                this.RefreshOfficerDatas();
                this.Refresh(this.GetData(m_currentPageType), ENUM_SELECT_TYPE.RESELECT);

                GlobalInfo.MY_PLAYER_INFO.SyncPlayerBag();
            }
            else if (s is SCPlayerPropResponse)
            {
                DebugUtil.Log("SCPlayerPropResponse");

                var rsp = s as SCPlayerPropResponse;

                this.OpenUpgradeWindow(true);

                RefreshRedPoints();
            }

        }

        void ReqServerOfficerInfo()
        {
            CSOfficerListRequest req = new CSOfficerListRequest();
            req.PlayerId = GlobalInfo.MY_PLAYER_ID;
#if !NETWORK_SYNC || UNITY_EDITOR
            GameEvents.NetWorkEvents.SendAsyncMsg.SafeInvoke(req);
#else
            GameEvents.NetWorkEvents.SendMsg.SafeInvoke(req);
#endif


        }


        void OnPoliceItemClicked(long id_)
        {
            DebugUtil.Log("点选警员 " + id_);
            this.m_choose_officer_id = id_;
            m_last_choose_officer_id = this.m_choose_officer_id;
            RefreshRightWindow(m_cur_right_type);

        }

        void RefreshRightWindow(ENUM_RIGHT_WINDOW_TYPE type_)
        {

            if (ENUM_RIGHT_WINDOW_TYPE.E_UPGRADE == type_)
            {
                OpenUpgradeWindow();
            }
            else if (ENUM_RIGHT_WINDOW_TYPE.E_DETAIL == type_)
            {
                OpenDetailWindow();
            }
            else
            {
                CloseRightWindow();
            }
        }

        void OpenDetailWindow()
        {
            m_cur_right_type = ENUM_RIGHT_WINDOW_TYPE.E_DETAIL;
            this.m_upgrade_window.Visible = false;
            UIOfficerInfo data = this.GetOfficerData(this.m_currentPageType, this.m_choose_officer_id);
            this.m_detail_window.Refresh(data.m_data, PoliceUILogicAssist.GetOfficerServerInfo(data.m_data));
            this.m_detail_window.Visible = true;
        }

        void OpenUpgradeWindow(bool with_effect_ = false)
        {
            m_cur_right_type = ENUM_RIGHT_WINDOW_TYPE.E_UPGRADE;
            this.m_detail_window.Visible = false;
            UIOfficerInfo data = this.GetOfficerData(this.m_currentPageType, this.m_choose_officer_id);
            this.m_upgrade_window.Refresh(data.m_data, PoliceUILogicAssist.GetOfficerServerInfo(data.m_data), with_effect_);
            this.m_upgrade_window.Visible = true;
        }

        void CloseRightWindow()
        {
            this.m_detail_window.Visible = false;
            this.m_upgrade_window.Visible = false;
        }


        void OnOpenDetailWindow()
        {
            if (this.m_detail_window.Visible)
            {
                UIOfficerInfo data = this.GetOfficerData(this.m_currentPageType, this.m_choose_officer_id);
                this.m_detail_window.Refresh(data.m_data, PoliceUILogicAssist.GetOfficerServerInfo(data.m_data));
                return;
            }

            this.m_cur_tween_progress = ENUM_TWEEN_PROGRESS.E_UPGRADE_TO_DETAIL;

            this.m_right_window_tween.PlayForward();
        }



        void OnOpenUpgradeWindow()
        {
            if (this.m_upgrade_window.Visible)
            {
                UIOfficerInfo data = this.GetOfficerData(this.m_currentPageType, this.m_choose_officer_id);
                this.m_upgrade_window.Refresh(data.m_data, PoliceUILogicAssist.GetOfficerServerInfo(data.m_data));
                return;
            }

            this.m_cur_tween_progress = ENUM_TWEEN_PROGRESS.E_DETAIL_TO_UPGRADE;

            this.m_right_window_tween.PlayForward();
        }

        void OnFinishRightWindowTween()
        {
            if (ENUM_TWEEN_PROGRESS.E_UPGRADE_TO_DETAIL == this.m_cur_tween_progress)
            {
                UIOfficerInfo data = this.GetOfficerData(this.m_currentPageType, this.m_choose_officer_id);
                this.m_detail_window.Refresh(data.m_data, PoliceUILogicAssist.GetOfficerServerInfo(data.m_data));
                this.m_upgrade_window.Visible = true;
                this.m_right_window_tween.PlayBackward();
            }
            else if (ENUM_TWEEN_PROGRESS.E_DETAIL_TO_UPGRADE == this.m_cur_tween_progress)
            {
                this.m_detail_window.Visible = true;
                UIOfficerInfo data = this.GetOfficerData(this.m_currentPageType, this.m_choose_officer_id);
                this.m_upgrade_window.Refresh(data.m_data, PoliceUILogicAssist.GetOfficerServerInfo(data.m_data));
                this.m_right_window_tween.PlayBackward();
            }

            this.m_cur_tween_progress = ENUM_TWEEN_PROGRESS.E_INVALID;

        }

        void Refresh(List<UIOfficerInfo> officers, ENUM_SELECT_TYPE type_)
        {

            int totalCount = officers.Count;

            if (totalCount == 0)
            {
                this.RefreshRightWindow(ENUM_RIGHT_WINDOW_TYPE.E_INVALID);
            }
            else
            {
            }

            List<UIOfficerInfo> unlock_officers = officers.FindAll((item) => item.m_lvl > 0);
            List<UIOfficerInfo> lock_officers = officers.FindAll((item) => 0 == item.m_lvl);

            //type_ = RefreshOfficers(unlock_officers, m_unlock_grid, type_);
            RefreshOfficers(lock_officers, m_lock_grid, type_);

            this.RefreshRedPoints();

            m_scroll_view.ScrollToTop();
        }

        private ENUM_SELECT_TYPE RefreshOfficers(List<UIOfficerInfo> officers_, GameLoopUIContainer grid_, ENUM_SELECT_TYPE selected_type_)
        {
            int totalCount = officers_.Count;

            grid_.EnsureSize<ToggleIconItem>(totalCount);

            ENUM_SELECT_TYPE cur_type = selected_type_;

            if (0 == totalCount)
            {
                grid_.Visible = false;
                return cur_type;
            }
            grid_.Visible = true;


            for (int i = 0; i < totalCount; i++)
            {
                ConfOfficer prop = officers_[i].m_data;
                ToggleIconItem toggle_item = grid_.GetChild<ToggleIconItem>(i);
                toggle_item.Visible = false;
                toggle_item.Visible = true;

                bool be_selected = false;

                if (ENUM_SELECT_TYPE.NONE == cur_type)
                {
                    be_selected = 0 == i;
                }
                else if (ENUM_SELECT_TYPE.SELECTED == cur_type)
                {
                    be_selected = false;
                }
                else if (ENUM_SELECT_TYPE.RESELECT == cur_type)
                {
                    be_selected = prop.id == this.m_choose_officer_id;
                }

                if (be_selected && ENUM_SELECT_TYPE.SELECTED != cur_type)
                {
                    cur_type = ENUM_SELECT_TYPE.SELECTED;
                }

                toggle_item.Refresh(i, prop.icon, prop.id, be_selected, (clicked_, index_) =>
                {
                    if (clicked_)
                    {
                        if (-1 != this.m_last_choose_officer_id)
                            EngineCoreEvents.AudioEvents.PlayAudio.SafeInvoke(Audio.AudioType.UISound, EngineCommonAudioKey.table_change.ToString());
                        OnPoliceItemClicked(index_);
                    }
                }, null);

                toggle_item.LockVisible = 0 == officers_[i].m_lvl;
            }

            return cur_type;
        }

        private List<UIOfficerInfo> GetData(ENUM_PAGE_TYPE t_)
        {


            switch (t_)
            {
                case ENUM_PAGE_TYPE.E_ALL:
                    return m_all_officer;
                case ENUM_PAGE_TYPE.E_SPECIAL_POLICE: //特警
                    return m_spcial_polices;
                case ENUM_PAGE_TYPE.E_PATROL_MEN: //巡警
                    return m_patrol_men;
                case ENUM_PAGE_TYPE.E_INSPECTOR: //探长
                    return m_inspectors;
                case ENUM_PAGE_TYPE.E_BAU:
                    return m_baus;
                case ENUM_PAGE_TYPE.E_CSI:
                    return m_csis;
                case ENUM_PAGE_TYPE.E_FORENSIC: //法医
                    return m_forensics;
                default:
                    return null;
            }
        }


        private UIOfficerInfo GetOfficerData(ENUM_PAGE_TYPE t_, long officer_id_)
        {
            List<UIOfficerInfo> datas = GetData(t_);

            return datas.Find((item) => item.m_data.id == officer_id_);
        }

    }
}

#endif