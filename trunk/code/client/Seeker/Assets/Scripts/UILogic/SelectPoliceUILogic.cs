#if IN_USE
using EngineCore;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SeekerGame
{
    [UILogicHandler(UIDefine.UI_SELECT_POLICE)]
    public class SelectPoliceUILogic : UILogicBase
    {

        private CustomDragMoveByCurve m_curve_move_panel;
        private GameUIContainer m_police_item_pool;
        private GameLabel m_police_name;
        private GameImage m_skill_icon;
        private GameLabel m_skill_label;
        private GameButton m_send_btn;
        private GameImage m_closeBtn;
        private GameLabel m_num;

        private List<OfficerInfo> m_officers = new List<OfficerInfo>();
        List<long> m_selected_IDs;

        private long m_officer_timestamp = 0L;

        protected override void OnInit()
        {
            base.OnInit();

            this.FindUIComponent();
            this.PreUILogic();

        }

        public override void OnShow(object param)
        {
            base.OnShow(param);

            RefreshOfficerView();

            this.AddUILogic();
            this.MoveStopped(0);
        }

        public override void OnGuidShow(int type = 0)
        {
            base.OnGuidShow(type);
            this.m_send_btn.Visible = false;
            this.m_closeBtn.Visible = false;
        }

        public override void OnHide()
        {
            base.OnHide();
            this.RemoveUILogic();
        }

        private void OnScResponse(object s)
        {
        }

        private void FindUIComponent()
        {
            this.m_curve_move_panel = this.Make<CustomDragMoveByCurve>("Panel_Curve");
            this.m_curve_move_panel.RegisterStopAction(MoveStopped);
            this.m_curve_move_panel.RegisterMovingAction(Moving);
            this.m_police_item_pool = this.Make<GameUIContainer>("Panel_Pool/Police_Pool");
            m_police_name = this.Make<GameLabel>("Text_name");
            m_skill_icon = this.Make<GameImage>("Image_bufficon");
            m_skill_label = this.Make<GameLabel>("Text_buff"); ;
            this.m_send_btn = this.Make<GameButton>("Button_Send_Root:Button_send");
            this.SetCloseBtnID("Button_close");
            this.m_closeBtn = this.Make<GameImage>("Button_close");
            m_num = this.Make<GameLabel>("Text_number");
            this.m_closeBtn.Visible = true;

        }


        private void RefreshNum(int selected_idx_)
        {
            m_num.Text = string.Format("{0}/{1}", selected_idx_, m_officers.Count);
        }

        private void PreUILogic()
        {
            RefreshOfficerView();
        }

        private void RefreshOfficerView()
        {
            //if (m_officer_timestamp != GlobalInfo.MY_PLAYER_INFO.OfficerTimestamp)

            List<OfficerInfo> temp = GlobalInfo.MY_PLAYER_INFO.Officer_infos;

            if (0 != PlayerInfoManager.Instance.LimitNum)
            {
                temp = temp.GetRange(0, PlayerInfoManager.Instance.LimitNum);
            }

            if (m_officers.Count != temp.Count)
            {
                m_officers = temp;
                m_officer_timestamp = GlobalInfo.MY_PLAYER_INFO.OfficerTimestamp;
                int police_num = m_officers.Count;


                this.m_police_item_pool.EnsureSize<CustomPoliceItem>(police_num);

                for (int i = 0; i < police_num; ++i)
                {
                    OfficerInfo info = m_officers[i];
                    var item = this.m_police_item_pool.GetChild<CustomPoliceItem>(i);
                    item.OfficerID = info.OfficerId;
                    item.InitTex(ConfOfficer.Get(info.OfficerId).portrait);
                    item.InitMaterial();
                    item.gameObject.name = i.ToString();

                }

                List<BlurGrayTexture> polices = new List<BlurGrayTexture>();

                for (int i = 0; i < this.m_police_item_pool.ChildCount; ++i)
                {
                    polices.Add(this.m_police_item_pool.GetChild<CustomPoliceItem>(i).Police_head_tex);
                }

                this.m_curve_move_panel.InitCustomItems(polices);

                RefreshNum(1);
            }
        }

        private void AddUILogic()
        {
            m_police_name.Visible = false;
            m_skill_icon.Visible = false;
            m_skill_label.Visible = false;
            this.m_send_btn.Visible = false;

            this.m_send_btn.AddClickCallBack(OnClickSend);
            this.m_curve_move_panel.AddClickCallBackPosition(OnClickedPosition);
            this.m_curve_move_panel.AddPressDownCallBack(OnPressDown);

            m_selected_IDs = PoliceDispatchManager.Instance.GetAllDispathOfficersID();

            this.RefreshTex();
        }

        private void RemoveUILogic()
        {
            this.m_send_btn.RemoveClickCallBack(OnClickSend);
            this.m_curve_move_panel.RemoveClickCallBackPosition(OnClickedPosition);
            this.m_curve_move_panel.RemovePressDownCallBack(OnPressDown);
        }

        private void OnClickSend(GameObject go)
        {
            EngineCoreEvents.AudioEvents.PlayAudio.SafeInvoke(Audio.AudioType.UISound,GameCustomAudioKey.task_policechoice.ToString(), null);
            int selected_idx = this.m_curve_move_panel.Cur_selectd_police_idx;
            GameEvents.UIEvents.UI_Enter_Event.EVT_SELECT_POLICE.SafeInvoke(selected_idx);
            EngineCoreEvents.UIEvent.HideUIEvent.SafeInvoke("UI_select_police2.prefab");
        }

        private void OnClickedPosition(GameObject go, Vector2 screen_pos_)
        {
            for (int i = 0; i < m_police_item_pool.ChildCount; ++i)
            {
                var item = m_police_item_pool.GetChild<CustomPoliceItem>(i);

                if (RectTransformUtility.RectangleContainsScreenPoint(item.Widget, screen_pos_, CameraManager.Instance.UICamera))
                {
                    GameEvents.UIEvents.UI_Select_Police_Event.OnIndexChosen.SafeInvoke(i);
                    break;
                }
            }
        }

        private void OnPressDown(GameObject go)
        {
            GameEvents.UIEvents.UI_Select_Police_Event.OnIndexChosen.SafeInvoke(-1);
        }

        private void MoveStopped(int select_index_)
        {
            m_police_name.Text = LocalizeModule.Instance.GetString(ConfOfficer.Get(m_officers[select_index_].OfficerId).name);
            m_skill_label.Text = LocalizeModule.Instance.GetString(ConfOfficer.Get(m_officers[select_index_].OfficerId).descs);
            m_police_name.Visible = true;
            m_skill_icon.Visible = true;
            m_skill_label.Visible = true;
            this.m_send_btn.Visible = true;
            
            //Debug.Log("========================" + select_index_);
            string icon, desc;

            if (SkillUtil.GetCurLevelSkillIconAndDesc(m_officers[select_index_].OfficerId, m_officers[select_index_].Level, out icon, out desc))
            {
                m_skill_icon.Sprite = icon;
                m_skill_label.Text = desc;
            }

            if (m_selected_IDs.Contains(m_officers[select_index_].OfficerId))
            {
                this.m_send_btn.SetGray(true);
                this.m_send_btn.Enable = false;
            }
            else
            {
                GameEvents.UI_Guid_Event.OnSelectPolice.SafeInvoke(select_index_);
                this.m_send_btn.SetGray(false);
                this.m_send_btn.Enable = true;
            }

            RefreshNum(select_index_ + 1);
        }

        private void Moving()
        {
            m_police_name.Visible = false;
            m_skill_icon.Visible = false;
            m_skill_label.Visible = false;
            m_send_btn.Visible = false;
        }

        private void RefreshTex()
        {

            int police_num = m_officers.Count;


            for (int i = 0; i < police_num; ++i)
            {
                var item = this.m_police_item_pool.GetChild<CustomPoliceItem>(i);

                if (m_selected_IDs.Contains(item.OfficerID))
                {
                    this.GraySelectdTex(i, true);
                }
                else
                {
                    this.GraySelectdTex(i, false);
                }
            }
        }

        private void GraySelectdTex(int index_, bool gray_)
        {
            this.m_police_item_pool.GetChild<CustomPoliceItem>(index_).Police_head_tex.SetGray(gray_);
        }
    }
}
#endif