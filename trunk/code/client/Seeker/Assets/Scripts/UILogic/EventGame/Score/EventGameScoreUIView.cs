//using EngineCore;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using UnityEngine;
//using GOEngine;
//using GOGUI;
//using DG.Tweening;

//namespace SeekerGame
//{
//    public class EventGameScoreUIView : BaseGameView<EventGameScoreUILogic>
//    {
//        private const float C_TWEEN_TIME = 2.5f;

//        private GameButton m_btnPause;

//        #region 上边信息
//        private GameImage m_type_icon;
//        private GameLabel m_score_title;
//        private GameLabel m_cur_score_num;
//        private GameLabel m_full_score_num;
//        private GameProgressBar m_score_progress;
//        private GameUIEffect m_score_p_effect;
//        #endregion

//        #region 其它信息
//        private GameLabel m_phase_add_score_num;
//        private GameLabel m_valuation_text;
//        private GameUIEffect m_valuation_effect;
//        private GameLabel m_feedback_text;
//        private GameLabel m_dialogue_text;
//        private GameTexture m_police_tex;

//        private GameButton m_ok_btn;
//        #endregion





//        #region 数据

//        #endregion

//        public override void PreLoadView(EventGameScoreUILogic t)
//        {
//            base.PreLoadView(t);

//            EventGameScoreUILogic cur_logic = CurViewLogic();
//            m_btnPause = cur_logic.MakeView<GameButton>("Button_pause");

//            m_type_icon = cur_logic.MakeView<GameImage>("Panel_top:Image");
//            m_score_title = cur_logic.MakeView<GameLabel>("Panel_top:Text");
//            m_cur_score_num = cur_logic.MakeView<GameLabel>("Panel_top:Text (1)");
//            m_cur_score_num.Text = "0";
//            m_full_score_num = cur_logic.MakeView<GameLabel>("Panel_top:Text (2)");
//            m_score_progress = cur_logic.MakeView<GameProgressBar>("Panel_top:Slider");
//            m_score_progress.Value = 0;
//            m_score_p_effect = cur_logic.MakeView<GameUIEffect>("Panel_top:Slider:UI_fenshutiao");
//            m_score_p_effect.EffectPrefabName = "UI_fenshutiao.prefab";
//            m_score_p_effect.Visible = false;

//            m_phase_add_score_num = cur_logic.MakeView<GameLabel>("Image:Text");
//            m_phase_add_score_num.Text = "0";
//            m_valuation_text = cur_logic.MakeView<GameLabel>("Image:Text (1)");
//            m_valuation_effect = cur_logic.MakeView<GameUIEffect>("Image:UI_shijian");
//            m_feedback_text = cur_logic.MakeView<GameLabel>("Text");
//            m_dialogue_text = cur_logic.MakeView<GameLabel>("Image_Dig:Text (1)");
//            m_police_tex = cur_logic.MakeView<GameTexture>("RawImage");

//            m_ok_btn = cur_logic.MakeView<GameButton>("btnSale");
//        }

//        public override void HideView()
//        {
//            base.HideView();

//            this.RemoveClick();

//            m_score_p_effect.Visible = false;
//        }



//        public override void ShowView(object s)
//        {
//            base.ShowView(s);

//            List<long> param_lst = (List<long>)s;

//            long finished_phase_id = param_lst[0];
//            int phase_score = (int)param_lst[1];
//            int valuation = (int)param_lst[2];

//            ConfEvent event_data = ConfEvent.Get(EventGameManager.Instance.Event_id);

//            m_type_icon.Sprite = ConfEventAttribute.Get(event_data.type).icon;
//            m_score_title.Text = LocalizeModule.Instance.GetString("ui.eventIngame1.score");
//            //m_cur_score_num.Text = EventGameManager.Instance.Score.ToString();
//            TweenText.Begin(m_cur_score_num.Label, C_TWEEN_TIME, 0.1f, EventGameManager.Instance.LastScore, EventGameManager.Instance.Score);
//            m_full_score_num.Text = string.Format("/{0}", event_data.perfectMark.ToString());
//            m_score_progress.Value = ((float)EventGameManager.Instance.LastScore) / event_data.perfectMark;
//            TweenProgressBarValue.Begin(m_score_progress.gameObject, C_TWEEN_TIME, ((float)EventGameManager.Instance.LastScore) / event_data.perfectMark, ((float)EventGameManager.Instance.Score) / event_data.perfectMark);

//            TimeModule.Instance.SetTimeout(()=> CommonHelper.UpdateEffectPosByProgressbar(m_score_progress, m_score_p_effect, 0.01f, C_TWEEN_TIME),0.25f);

//            TweenText.Begin(m_phase_add_score_num.Label, C_TWEEN_TIME, 0.1f, phase_score, 0).SetOnFinished(() => m_phase_add_score_num.Text = "");
//            m_valuation_text.Text = 0 == valuation ? LocalizeModule.Instance.GetString("ui.UI_event_ingame_2.normal") : LocalizeModule.Instance.GetString("ui.UI_event_ingame_2.perfect");
//            m_valuation_effect.EffectPrefabName = 0 == valuation ? "UI_shijian_zhengchang.prefab" : "UI_shijian_wanmei.prefab";
//            m_valuation_effect.Visible = true;
//            string t_feedback, t_dialogue;
//            EventGameUIAssist.GetFeedBackAndDialogue(EventGameManager.Instance.Most_valuable_officer_id, finished_phase_id, valuation, out t_feedback, out t_dialogue);

//            m_feedback_text.Text = t_feedback;
//            m_dialogue_text.Text = t_dialogue;

//            if (0 != EventGameManager.Instance.Most_valuable_officer_id)
//            {
//                m_police_tex.TextureName = ConfOfficer.Get(EventGameManager.Instance.Most_valuable_officer_id).hollowPortrait;
//            }
//            else
//            {
//                m_police_tex.TextureName = GlobalInfo.MY_PLAYER_INFO.PlayerIcon;
//            }

//            this.AddClick();

//            EventGameManager.Instance.LastScore = EventGameManager.Instance.Score;
//        }

//        void AddClick()
//        {
//            this.m_btnPause.AddClickCallBack(EventGameUIAssist.OnBtnGamePauseClick);
//            this.m_ok_btn.AddClickCallBack(Clicked);

//        }

//        void RemoveClick()
//        {
//            this.m_btnPause.RemoveClickCallBack(EventGameUIAssist.OnBtnGamePauseClick);
//            this.m_ok_btn.RemoveClickCallBack(Clicked);
//        }

//        public void Clicked(GameObject obj)
//        {
//            EngineCoreEvents.AudioEvents.PlayAudio.SafeInvoke(Audio.AudioType.UISound, EngineCommonAudioKey.Button_Click_Common.ToString());
//            CurViewLogic().NextPhaseOnClicked();
//        }

//    }
//}
