//#define TEST
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EngineCore;
using GOGUI;
using UnityEngine;
using DG.Tweening;

namespace SeekerGame
{
    class EventGamePlayNewUIView : BaseViewComponet<EventGamePlayNewUILogic>
    {
        private const float C_TWEEN_TIME = 1.0f;

        private GameImage m_type_icon;
        private GameLabel m_cur_score_num;
        private GameProgressBar m_score_progress;
        private GameUIEffect m_score_effect;
        private GameImage m_normal_img;
        private GameUIComponent m_desc_root;
        private GameTexture m_officer_tex;
        private GameLabel m_desc_txt;
        private GameUIComponent m_case_root;
        private GameLabel m_case_txt;
        SCEventPhaseFeedbackResponse m_data;

        Queue<PhaseInfo> m_phases = new Queue<PhaseInfo>();



        private int m_cur_score;
        private int m_full_score;
        protected override void OnInit()
        {
            base.OnInit();

            m_type_icon = this.Make<GameImage>("Panel_top:Image");
            m_cur_score_num = this.Make<GameLabel>("Panel_top:Text (1)");
            m_score_progress = this.Make<GameProgressBar>("Panel_top:Slider");
            m_score_effect = this.Make<GameUIEffect>("Panel_top:Slider:UI_fenshutiao");
            m_score_effect.EffectPrefabName = "UI_fenshutiao.prefab";
            m_score_effect.Visible = false;
            m_normal_img = this.Make<GameImage>("Panel_top:Image_normal");
            m_officer_tex = this.Make<GameTexture>("Panel_Talk:RawImage");
            m_desc_root = this.Make<GameUIComponent>("Panel_Talk");
            m_desc_txt = this.Make<GameLabel>("Panel_Talk:Text:content");
            m_case_root = this.Make<GameUIComponent>("Image_enemy");
            m_case_txt = this.Make<GameLabel>("Image_enemy:Text");

        }

        public override void OnShow(object param)
        {
            base.OnShow(param);

            m_score_effect.Visible = false;

            Dictionary<UBSParamKeyName, object> _params = new Dictionary<UBSParamKeyName, object>()
                    {
                        { UBSParamKeyName.Success, 1},
                        {UBSParamKeyName.ContentID, CurViewLogic().Event_id }
                    };
            UserBehaviorStatisticsModules.Instance.LogEvent(UBSEventKeyName.event_start, null, _params);
        }

        public void Refresh(SCEventPhaseFeedbackResponse data_)
        {
            m_data = (SCEventPhaseFeedbackResponse)data_;

            foreach (var item in m_data.PhaseInfos)
            {
                m_phases.Enqueue(item);
            }

            ConfEvent event_data = ConfEvent.Get(CurViewLogic().Event_id);

            m_type_icon.Sprite = ConfEventAttribute.Get(event_data.type).icon;
            m_cur_score_num.Text = event_data.perfectMark.ToString();
            m_full_score = m_cur_score = event_data.perfectMark;
            m_score_progress.Value = ((float)m_cur_score) / event_data.perfectMark;

            Vector3[] corners = new Vector3[4];
            m_score_progress.Widget.GetWorldCorners(corners);
            Vector3 bottomLeftConner = corners[0];
            Vector3 topRightConner = corners[2];

            float normal_progress = (float)(event_data.perfectMark - event_data.passMark) / (float)event_data.perfectMark;

            float normal_worldpos_x = bottomLeftConner.x + (topRightConner.x - bottomLeftConner.x) * normal_progress;
            m_normal_img.Widget.transform.position = new Vector3(normal_worldpos_x, m_normal_img.Widget.transform.position.y, m_normal_img.Widget.transform.position.z);

            NextStep();
        }


        public override void OnHide()
        {
            base.OnHide();

            TimeModule.Instance.RemoveTimeaction(NextStep);
            TimeModule.Instance.RemoveTimeaction(GetReward);
        }

        private void NextStep()
        {
            if (false == this.Visible)
                return;

            PhaseInfo info_ = m_phases.Dequeue();

            if (null == info_)
            {
                this.GetReward();
                return;
            }

            Dictionary<UBSParamKeyName, object> _params = new Dictionary<UBSParamKeyName, object>()
                    {
                        { UBSParamKeyName.Success, info_.Perfect ? 1 : 0},
                        {UBSParamKeyName.ContentID, CurViewLogic().Event_id },
                        {UBSParamKeyName.PhaseID, info_.PhaseTemplateId },
                                {UBSParamKeyName.OfficerID, info_.OfficerTemplateId }

                    };
            UserBehaviorStatisticsModules.Instance.LogEvent(UBSEventKeyName.event_Phase, null, _params);

            m_desc_root.Visible = false;

            m_case_txt.Text = LocalizeModule.Instance.GetString(ConfEventPhase.Get(info_.PhaseTemplateId).descs);

            Action OnCase = () => PlayCase(info_.PhaseTemplateId);
            Action OnTalk = () =>
            {
                PlayTalk(info_.OfficerTemplateId, info_.PhaseTemplateId, false == info_.Perfect ? 0 : 1);
                AddScore(m_cur_score, m_cur_score - info_.PhaseScore);
            };

            Action OnEndPhase = () =>
            {
                m_case_root.Visible = false;
            };

            Action OnNext = () =>
            {
                //if (0 != m_phases.Count)
                //    TimeModule.Instance.SetTimeout(NextStep, C_TWEEN_TIME + 0.5f);
                //else
                //    TimeModule.Instance.SetTimeout(GetReward, C_TWEEN_TIME + 0.5f);

                if (0 != m_phases.Count)
                    NextStep();
                else
                    GetReward();
            };

            TimeModule.Instance.SetTimeout(OnCase, C_TWEEN_TIME);
            TimeModule.Instance.SetTimeout(OnTalk, C_TWEEN_TIME + 0.5f);
            TimeModule.Instance.SetTimeout(OnEndPhase, C_TWEEN_TIME + 2.5f);
            TimeModule.Instance.SetTimeout(OnNext, C_TWEEN_TIME + 3.5f);
        }

        private void AddScore(int cur_num_, int aim_num_)
        {/*
            aim_num_ = aim_num_ >= 0 ? aim_num_ : 0;
            m_cur_score_num.Label.DOText(aim_num_.ToString(), C_TWEEN_TIME);
            //TweenText.Begin(m_cur_score_num.Label, C_TWEEN_TIME, 0.1f, cur_num_, aim_num_);
            m_score_progress.Value = ((float)m_cur_score) / m_full_score;
            TweenProgressBarValue.Begin(m_score_progress.gameObject, C_TWEEN_TIME, ((float)m_cur_score) / m_full_score, ((float)aim_num_) / m_full_score).AddOnFinished(ShowScoreEffect);
            m_cur_score = aim_num_;*/
        }

        private void ShowScoreEffect()
        {
            m_score_effect.Visible = false;
            m_score_effect.Visible = true;
            CommonHelper.EffectProgressbarValueSync(m_score_progress, m_score_effect);
        }

        private void PlayCase(long phase_id_)
        {
            if (!m_case_root.Visible)
                m_case_root.Visible = true;

        }

        private void PlayTalk(long officer_id_, long phase_id_, int valuation)
        {
            m_desc_root.Visible = true;

            if (0 != officer_id_)
            {
                m_officer_tex.TextureName = ConfOfficer.Get(officer_id_).hollowPortrait;
            }
            else
            {
                m_officer_tex.TextureName = CommonData.GetBigPortrait(GlobalInfo.MY_PLAYER_INFO.PlayerIcon);
            }

            string t_feedback, t_dialogue;
            EventGameUIAssist.GetFeedBackAndDialogue(officer_id_, phase_id_, valuation, out t_feedback, out t_dialogue);
            m_desc_txt.Text = t_dialogue;
        }

        private void GetReward()
        {
            if (false == this.Visible)
                return;

            CurViewLogic().RequestReward();
        }

    }


}
