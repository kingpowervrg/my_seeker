//using EngineCore;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using UnityEngine;
//using GOEngine;
//using GOGUI;

//namespace SeekerGame
//{
//    public class EventGamePlayUIView : BaseViewComponet<EventGamePlayUILogic>
//    {
//        private class ImageWithID
//        {
//            public long m_officer_id;
//            public GameImage m_root;
//            public GameImage m_icon;
//        }

//        private readonly Color MATCH_WORD_COLOR = new Color(40 / 255.0f, 178 / 255.0f, 255 / 255.0f);

//        private GameButton m_btnPause;
//        #region 上边信息
//        private GameImage m_type_icon;
//        private GameLabel m_score_title;
//        private GameLabel m_cur_score_num;
//        private GameLabel m_full_score_num;
//        private GameProgressBar m_score_progress;
//        private GameImage m_normal_img;
//        private GameUIEffect m_score_p_effect;
//        #endregion

//        #region 左边信息
//        private GameLabel m_key_word_title;
//        private List<GameToggleButton> m_key_word_toggles;
//        private List<GameLabel> m_key_word_texts;
//        private List<GameUIEffect> m_key_word_effects;
//        private List<GameImage> m_key_word_imgs;
//        private List<GameUIComponent> m_key_word_anchors;
//        private List<GameImage> m_dispatched_officer_icons_root;
//        private List<GameImage> m_dispatched_officer_icons;
//        private GameLabel m_desc;
//        private GameButton m_ok_btn;
//        private GameLabel m_ok_btn_title;
//        #endregion

//        #region 右边信息
//        private GameLabel m_tips;
//        private GameLabel m_empty_tips;
//        private GameUIContainer m_officer_grid;
//        #endregion


//        #region 结算提醒
//        private GameImage m_finish_bg;


//        private GameImage m_finish_tips_bg;
//        private GameLabel m_finish_left_tip;
//        private GameUIContainer m_finish_tips_grid;

//        private GameImage m_valuation_tips_bg;
//        private GameUIContainer m_valuation_normal_tips_grid;
//        private GameUIContainer m_valuation_perfect_tips_grid;
//        private GameUIEffect m_valuation_normal_effect;
//        private GameUIEffect m_valuation_perfect_effect;
//        #endregion


//        #region 对话
//        EventGameTweenDialogUIView m_dialog_view;
//        #endregion

//        #region 数据
//        Dictionary<int, string> m_phase_key_word_dict;

//        Dictionary<int, GameToggleButton> m_key_word_toggle_dict;

//        Dictionary<int, GameImage> m_key_word_img_dict;

//        Dictionary<int, GameLabel> m_key_word_label_dict;

//        Dictionary<int, GameUIEffect> m_key_word_effect_dict;

//        //private long m_cur_choose_index;

//        Dictionary<long, List<long>> m_officer_key_word_id_dict;


//        Dictionary<int, ImageWithID> m_dispatched_officer_icon_dict;

//        private string m_phase_finish_str;
//        private string m_phase_result_perfect_str;
//        private string m_phase_result_normal_str;
//        private int m_full_num;

//        #endregion




//        protected override void OnInit()
//        {
//            base.OnInit();

//            m_btnPause = this.Make<GameButton>("Button_pause");


//            m_type_icon = this.Make<GameImage>("Panel_top:Image");
//            m_score_title = this.Make<GameLabel>("Panel_top:Text");
//            m_cur_score_num = this.Make<GameLabel>("Panel_top:Text (1)");
//            m_full_score_num = this.Make<GameLabel>("Panel_top:Text (2)");
//            m_score_progress = this.Make<GameProgressBar>("Panel_top:Slider");
//            m_normal_img = this.Make<GameImage>("Panel_top:Image_normal");
//            m_score_p_effect = m_score_progress.Make<GameUIEffect>("UI_shijian_fenshu_tuowei");
//            m_score_p_effect.EffectPrefabName = "UI_shijian_fenshu_tuowei.prefab";

//            m_key_word_title = this.Make<GameLabel>("Panel_left:Image:Image:Text");
//            m_key_word_toggles = new List<GameToggleButton>
//            {
//                this.Make<GameToggleButton>("Panel_left:Panel_keywords:Toggle"),
//                this.Make<GameToggleButton>("Panel_left:Panel_keywords:Toggle (1)"),
//                this.Make<GameToggleButton>("Panel_left:Panel_keywords:Toggle (2)"),
//                this.Make<GameToggleButton>("Panel_left:Panel_keywords:Toggle (3)"),
//                this.Make<GameToggleButton>("Panel_left:Panel_keywords:Toggle (4)"),
//                this.Make<GameToggleButton>("Panel_left:Panel_keywords:Toggle (5)"),
//            };

//            m_key_word_texts = new List<GameLabel>
//            {
//                this.Make<GameLabel>("Panel_left:Panel_keywords:Toggle:Label"),
//                this.Make<GameLabel>("Panel_left:Panel_keywords:Toggle (1):Label"),
//                this.Make<GameLabel>("Panel_left:Panel_keywords:Toggle (2):Label"),
//                this.Make<GameLabel>("Panel_left:Panel_keywords:Toggle (3):Label"),
//                this.Make<GameLabel>("Panel_left:Panel_keywords:Toggle (4):Label"),
//                this.Make<GameLabel>("Panel_left:Panel_keywords:Toggle (5):Label"),
//            };

//            m_key_word_effects = new List<GameUIEffect>
//            {
//                this.Make<GameUIEffect>("Panel_left:Panel_keywords:Toggle:Background:Image:UI_dengpao"),
//                this.Make<GameUIEffect>("Panel_left:Panel_keywords:Toggle (1):Background:Image:UI_dengpao"),
//                this.Make<GameUIEffect>("Panel_left:Panel_keywords:Toggle (2):Background:Image:UI_dengpao"),
//                this.Make<GameUIEffect>("Panel_left:Panel_keywords:Toggle (3):Background:Image:UI_dengpao"),
//                this.Make<GameUIEffect>("Panel_left:Panel_keywords:Toggle (4):Background:Image:UI_dengpao"),
//                this.Make<GameUIEffect>("Panel_left:Panel_keywords:Toggle (5):Background:Image:UI_dengpao"),
//            };

//            for (int i = 0; i < m_key_word_effects.Count; ++i)
//            {
//                GameUIEffect effect = m_key_word_effects[i];
//                effect.EffectPrefabName = string.Format("UI_dengpao_0{0}.prefab", i + 1);
//            }

//            m_key_word_imgs = new List<GameImage>
//            {
//                this.Make<GameImage>("Panel_left:Panel_keywords:Toggle:Background"),
//                this.Make<GameImage>("Panel_left:Panel_keywords:Toggle (1):Background"),
//                this.Make<GameImage>("Panel_left:Panel_keywords:Toggle (2):Background"),
//                this.Make<GameImage>("Panel_left:Panel_keywords:Toggle (3):Background"),
//                this.Make<GameImage>("Panel_left:Panel_keywords:Toggle (4):Background"),
//                this.Make<GameImage>("Panel_left:Panel_keywords:Toggle (5):Background"),
//            };

//            m_key_word_anchors = new List<GameUIComponent>
//            {
//                this.Make<GameUIComponent>("Panel_left:Panel_keywords:Toggle:Anchor"),
//                this.Make<GameUIComponent>("Panel_left:Panel_keywords:Toggle (1):Anchor"),
//                this.Make<GameUIComponent>("Panel_left:Panel_keywords:Toggle (2):Anchor"),
//                this.Make<GameUIComponent>("Panel_left:Panel_keywords:Toggle (3):Anchor"),
//                this.Make<GameUIComponent>("Panel_left:Panel_keywords:Toggle (4):Anchor"),
//                this.Make<GameUIComponent>("Panel_left:Panel_keywords:Toggle (5):Anchor"),
//            };


//            foreach (var item in m_key_word_imgs)
//            {
//                TweenScale tp = item.GetComponent<TweenScale>();
//                tp.worldSpace = true;
//                tp.From = item.gameObject.transform.position; // new Vector3(item.gameObject.transform.position.x, item.gameObject.transform.position.y, 0.0f);
//                tp.to = m_cur_score_num.gameObject.transform.position; // new Vector3(m_cur_score_num.gameObject.transform.position.x, m_cur_score_num.gameObject.transform.position.y, 0.0f);
//                tp.ResetAndPlay();
//                tp.enabled = false;
//            }

//            m_dispatched_officer_icons_root = new List<GameImage>
//            {
//                 this.Make<GameImage>("Panel_left:Panel:Image"),
//                 this.Make<GameImage>("Panel_left:Panel:Image (1)"),
//                 this.Make<GameImage>("Panel_left:Panel:Image (2)"),
//                 this.Make<GameImage>("Panel_left:Panel:Image (3)"),
//                 this.Make<GameImage>("Panel_left:Panel:Image (4)"),
//                 this.Make<GameImage>("Panel_left:Panel:Image (5)"),
//            };

//            m_dispatched_officer_icons = new List<GameImage>
//            {
//                 this.Make<GameImage>("Panel_left:Panel:Image:Image (1)"),
//                 this.Make<GameImage>("Panel_left:Panel:Image (1):Image (1)"),
//                 this.Make<GameImage>("Panel_left:Panel:Image (2):Image (1)"),
//                 this.Make<GameImage>("Panel_left:Panel:Image (3):Image (1)"),
//                 this.Make<GameImage>("Panel_left:Panel:Image (4):Image (1)"),
//                 this.Make<GameImage>("Panel_left:Panel:Image (5):Image (1)"),
//            };




//            m_desc = this.Make<GameLabel>("Panel_left:Image:Text");
//            m_ok_btn = this.Make<GameButton>("Panel_left:btn_ok");
//            m_ok_btn_title = this.Make<GameLabel>("Panel_left:btn_ok:Text");
//            m_officer_grid = this.Make<GameUIContainer>("Image:Panel:grid");
//            m_empty_tips = this.Make<GameLabel>("Image:Panel:Text");
//            m_empty_tips.Text = LocalizeModule.Instance.GetString("bag_nothing");
//            m_tips = this.Make<GameLabel>("Image:Text");
//            m_tips.Text = LocalizeModule.Instance.GetString("EventUI.shuoming");

//            m_phase_finish_str = LocalizeModule.Instance.GetString("ui.event.phasedone");
//            m_phase_result_normal_str = LocalizeModule.Instance.GetString("ui.UI_event_ingame_2.normal");
//            m_phase_result_perfect_str = LocalizeModule.Instance.GetString("ui.UI_event_ingame_2.perfect");

//            m_finish_bg = this.Make<GameImage>("Image_bg");
//            m_finish_tips_bg = this.Make<GameImage>("Image_complete");
//            m_finish_left_tip = m_finish_tips_bg.Make<GameLabel>("Text");
//            m_finish_left_tip.Text = m_phase_finish_str;
//            m_finish_tips_grid = m_finish_tips_bg.Make<GameUIContainer>("grid");
//            this.InitGridTips(m_phase_finish_str, m_finish_tips_grid);

//            m_valuation_tips_bg = this.Make<GameImage>("Image_perfect");
//            m_valuation_normal_tips_grid = m_valuation_tips_bg.Make<GameUIContainer>("grid_normal");
//            m_valuation_perfect_tips_grid = m_valuation_tips_bg.Make<GameUIContainer>("grid_perfect");
//            m_valuation_normal_effect = m_valuation_tips_bg.Make<GameUIEffect>("UI_shijian_jieguo_01");
//            m_valuation_normal_effect.EffectPrefabName = "UI_shijian_jieguo_01.prefab";
//            m_valuation_perfect_effect = m_valuation_tips_bg.Make<GameUIEffect>("UI_shijian_jieguo_02");
//            m_valuation_perfect_effect.EffectPrefabName = "UI_shijian_jieguo_02.prefab";



//            this.InitGridTips(m_phase_result_perfect_str, m_valuation_perfect_tips_grid);
//            this.InitGridTips(m_phase_result_normal_str, m_valuation_normal_tips_grid);

//            m_dialog_view = this.Make<EventGameTweenDialogUIView>("Panel_end");
//        }

//        private void InitGridTips(string tips_, GameUIContainer tips_grid_)
//        {
//            string valuation_str = tips_;
//            tips_grid_.EnsureSize<GameLabel>(valuation_str.Length);
//            char[] chars = valuation_str.ToCharArray();
//            for (int i = 0; i < tips_grid_.ChildCount; ++i)
//            {
//                GameLabel lbl = tips_grid_.GetChild<GameLabel>(i);
//                lbl.Visible = true;
//                lbl.Text = chars[i].ToString();

//                UITweenerBase[] tweeners = lbl.GetComponents<UITweenerBase>();
//                foreach (var item in tweeners)
//                {
//                    if (item.delay > 1.05f)
//                    {
//                        item.delay += i * 0.05f;

//                        if (item is TweenScale)
//                        {
//                            TweenScale tp = item as TweenScale;
//                            tp.worldSpace = true;
//                            tp.From = item.gameObject.transform.position;
//                            tp.to = m_cur_score_num.gameObject.transform.position;
//                        }
//                    }


//                }
//            }
//        }

//        public override void OnHide()
//        {
//            base.OnHide();

//            this.RemoveClick();

//            TimeModule.Instance.RemoveTimeaction(DelayShowValuation);
//            TimeModule.Instance.RemoveTimeaction(DelayShowKeywords);
//            TimeModule.Instance.RemoveTimeaction(DelayShowDialog);
//            m_valuation_normal_effect.Visible = false;
//            m_valuation_perfect_effect.Visible = false;
//        }


//        public void Reset(object s)
//        {
//            OnHide();
//            OnShow(s);
//        }

//        private void ResetTween(GameUIComponent tween_root)
//        {
//            UITweenerBase[] tweeners = tween_root.GetComponents<UITweenerBase>();

//            foreach (var item in tweeners)
//            {
//                item.ResetAndPlay();
//                item.enabled = true;
//            }

//        }


//        public override void OnShow(object s)
//        {
//            base.OnShow(s);

//            #region 数据

//            this.m_officer_key_word_id_dict = new Dictionary<long, List<long>>();
//            List<long> officers = EventGameManager.Instance.GetRetainOfficers();
//            for (int i = 0; i < officers.Count; ++i)
//            {
//                m_officer_key_word_id_dict.Add(officers[i], EventGameUIAssist.GetPoliceKeyWordByOfficerID(officers[i]));
//            }

//            var data = s as List<long>;
//            long event_id = data[0];
//            long phase_id = data[1];
//            long cur_score = data[2];

//            ConfEvent event_data = ConfEvent.Get(event_id);
//            ConfEventPhase phase_data = ConfEventPhase.Get(phase_id);

//            m_phase_key_word_dict = EventGameUIAssist.GetPhaseKeyWords(phase_data);
//            EventGameManager.Instance.CurPhaseKeyWordCount = m_phase_key_word_dict.Count;

//            //<根据关键词数量,控制坑位
//            m_dispatched_officer_icons_root.ForEach(item => item.Visible = false);
//            m_dispatched_officer_icon_dict = new Dictionary<int, ImageWithID>();
//            for (int i = 0; i < m_phase_key_word_dict.Count; ++i)
//            {
//                m_dispatched_officer_icons_root[i].Visible = true;
//                m_dispatched_officer_icon_dict.Add(i, new ImageWithID { m_officer_id = -1, m_root = m_dispatched_officer_icons_root[i], m_icon = m_dispatched_officer_icons[i] });
//            }
//            //>

//            m_is_clicked = false;
//            #endregion


//            #region View
//            m_type_icon.Sprite = ConfEventAttribute.Get(event_data.type).icon;
//            m_score_title.Text = LocalizeModule.Instance.GetString("ui.eventIngame1.score");
//            m_cur_score_num.Text = cur_score.ToString();
//            m_full_num = event_data.perfectMark;
//            m_full_score_num.Text = string.Format("/{0}", event_data.perfectMark.ToString());
//            m_score_progress.Value = ((float)cur_score) / event_data.perfectMark;

//            Vector3[] corners = new Vector3[4];
//            m_score_progress.Widget.GetWorldCorners(corners);
//            Vector3 bottomLeftConner = corners[0];
//            Vector3 topRightConner = corners[2];

//            float normal_progress = (float)event_data.passMark / (float)event_data.perfectMark;

//            float normal_worldpos_x = bottomLeftConner.x + (topRightConner.x - bottomLeftConner.x) * normal_progress;
//            m_normal_img.Widget.transform.position = new Vector3(normal_worldpos_x, m_normal_img.Widget.transform.position.y, m_normal_img.Widget.transform.position.z);

//            //m_key_word_title.Text = LocalizeModule.Instance.GetString("ui.eventIngame1.keyWords");
//            m_key_word_title.Text = string.Format("{0} / {1}", EventGameManager.Instance.GetCurPhaseNum(), EventGameManager.Instance.GetTotalPhasesCount());

//            foreach (var item in m_key_word_texts)
//            {
//                item.Visible = false;
//            }

//            foreach (var item in m_key_word_imgs)
//            {
//                item.Visible = false;
//            }

//            foreach (var item in m_key_word_toggles)
//            {
//                item.Visible = false;
//                item.Enabled = true;
//            }

//            foreach (var item in m_key_word_effects)
//            {
//                item.Visible = false;
//            }

//            m_key_word_toggle_dict = new Dictionary<int, GameToggleButton>();
//            m_key_word_img_dict = new Dictionary<int, GameImage>();
//            m_key_word_label_dict = new Dictionary<int, GameLabel>();
//            m_key_word_effect_dict = new Dictionary<int, GameUIEffect>();

//            for (int i = 0; i < m_key_word_imgs.Count; ++i)
//            {
//                TweenScale tp = m_key_word_imgs[i].GetComponent<TweenScale>();
//                tp.worldSpace = true;
//                tp.From = m_key_word_anchors[i].gameObject.transform.position; // new Vector3(item.gameObject.transform.position.x, item.gameObject.transform.position.y, 0.0f);
//                tp.to = m_cur_score_num.gameObject.transform.position; // new Vector3(m_cur_score_num.gameObject.transform.position.x, m_cur_score_num.gameObject.transform.position.y, 0.0f);
//                tp.ResetAndPlay();
//                tp.enabled = false;
//            }


//            int j = 0;
//            foreach (KeyValuePair<int, string> pair in m_phase_key_word_dict)
//            {
//                if (j >= m_key_word_toggles.Count || j >= m_key_word_texts.Count)
//                    break;

//                m_key_word_texts[j].Visible = true;
//                m_key_word_texts[j].Text = pair.Value;
//                m_key_word_label_dict.Add(pair.Key, m_key_word_texts[j]);

//                m_key_word_toggles[j].Visible = true;
//                m_key_word_toggles[j].Checked = false;
//                m_key_word_toggle_dict.Add(pair.Key, m_key_word_toggles[j]);

//                m_key_word_effects[j].Visible = false;
//                m_key_word_effect_dict.Add(pair.Key, m_key_word_effects[j]);

//                m_key_word_imgs[j].Visible = true;
//                m_key_word_imgs[j].Sprite = ConfKeyWords.Get(pair.Key).icon;

//                m_key_word_img_dict.Add(pair.Key, m_key_word_imgs[j]);

//                ++j;
//            }





//            m_desc.Text = LocalizeModule.Instance.GetString(ConfEventPhase.Get(phase_id).descs);

//            m_ok_btn_title.Text = LocalizeModule.Instance.GetString("UI.OK");

//            if (0 != EventGameManager.Instance.GetRetainOfficers().Count)
//            {
//                m_empty_tips.Visible = false;
//            }
//            else
//            {
//                m_empty_tips.Visible = true;
//            }
//            m_officer_grid.EnsureSize<EventGameOfficerItemView>(EventGameManager.Instance.GetRetainOfficers().Count);


//            RefreshPortrait(-1, false);
//            RefreshKeyWordToggle();
//            #endregion

//            this.AddClick();

//            m_finish_bg.Visible = false;
//            m_finish_bg.Color = new Color(m_finish_bg.Color.r, m_finish_bg.Color.g, m_finish_bg.Color.b, 1.0f);
//            m_finish_tips_bg.Visible = false;
//            m_valuation_tips_bg.Visible = false;
//            m_dialog_view.Visible = false;
//            m_valuation_normal_effect.Visible = false;
//            m_valuation_perfect_effect.Visible = false;
//            m_score_p_effect.Visible = false;

//        }



//        void AddClick()
//        {
//            this.m_btnPause.AddClickCallBack(EventGameUIAssist.OnBtnGamePauseClick);
//            this.m_ok_btn.AddClickCallBack(OnClicked);

//            for (int i = 0; i < m_officer_grid.ChildCount; ++i)
//            {
//                long iconID = EventGameUIAssist.GetPoliceIDByIndex(i);
//                EventGameOfficerItemView toggle_item = m_officer_grid.GetChild<EventGameOfficerItemView>(i);
//                toggle_item.gameObject.name = iconID.ToString();
//                toggle_item.EnableClick = true;
//                toggle_item.Refresh(EventGameManager.Instance.GetRetainOfficers()[i], false, (id_, checked_) =>
//                {

//                    EngineCoreEvents.AudioEvents.PlayAudio.SafeInvoke(Audio.AudioType.UISound,EngineCommonAudioKey.table_change.ToString(), null);
//                    OnPoliceItemClicked(id_, checked_);

//                });

//                toggle_item.Visible = false;
//                toggle_item.Visible = true;
//            }


//        }

//        void RemoveClick()
//        {
//            this.m_btnPause.RemoveClickCallBack(EventGameUIAssist.OnBtnGamePauseClick);
//            this.m_ok_btn.RemoveClickCallBack(OnClicked);
//        }




//        private const float C_TWEEN_TIME = 1.5f;



//        private void ShowComplete(SCEventPhaseFeedbackResponse phase_finish_msg_)
//        {

//            this.m_finish_bg.Visible = true;
//            this.m_finish_tips_bg.Visible = true;

//            UITweenerBase[] tweens = m_finish_left_tip.GetComponents<UITweenerBase>();
//            foreach (var item in tweens)
//            {
//                item.ResetAndPlay();
//            }


//            this.ResetTween(this.m_finish_tips_bg);

//            int delta_score = phase_finish_msg_.TotalScore;
//            EventGameManager.Instance.Score = EventGameManager.Instance.LastScore + delta_score;
//            AddScore(EventGameManager.Instance.LastScore, EventGameManager.Instance.Score);
//            EventGameManager.Instance.LastScore = EventGameManager.Instance.Score;


//        }

//        private void RefreshValuationGrid(bool v_)
//        {
//            this.m_valuation_perfect_tips_grid.Visible = false;
//            this.m_valuation_normal_tips_grid.Visible = false;

//            if (v_)
//            {
//                this.m_valuation_perfect_tips_grid.Visible = true;
//                this.m_valuation_perfect_effect.Visible = true;
//            }
//            else
//            {
//                this.m_valuation_normal_tips_grid.Visible = true;
//                this.m_valuation_normal_effect.Visible = true;
//            }

//        }


//        private long m_finished_phase_id;
//        private SCEventPhaseFeedbackResponse m_phase_finish_msg;

//        private void DelayShowValuation()
//        {
//            ShowValuation(m_finished_phase_id, m_phase_finish_msg);
//        }


//        private void ShowValuation(long finished_phase_id_, SCEventPhaseFeedbackResponse phase_finish_msg_)
//        {
//            m_finish_tips_bg.Visible = false;
//            m_valuation_tips_bg.Visible = true;
//            ResetTween(m_valuation_tips_bg);
//            RefreshValuationGrid(phase_finish_msg_.Valuation);

//            int delta_score = phase_finish_msg_.TotalScore;
//            EventGameManager.Instance.Score = EventGameManager.Instance.LastScore + delta_score;
//            AddScore(EventGameManager.Instance.LastScore, EventGameManager.Instance.Score);
//            EventGameManager.Instance.LastScore = EventGameManager.Instance.Score;


//            TimeModule.Instance.SetTimeout(DelayShowKeywords, 3.0f);
//        }

//        private void DelayShowKeywords()
//        {
//            ShowKeywords(m_finished_phase_id, m_phase_finish_msg);
//        }

//        private void ShowKeywords(long finished_phase_id_, SCEventPhaseFeedbackResponse phase_finish_msg_)
//        {
//            m_finish_bg.Color = new Color(m_finish_bg.Color.r, m_finish_bg.Color.g, m_finish_bg.Color.b, 0.05f);
//            m_finish_tips_bg.Visible = false;
//            m_valuation_tips_bg.Visible = false;

//            foreach (KeyValuePair<int, GameImage> kvp in m_key_word_img_dict)
//            {
//                if (!m_key_word_toggle_dict[kvp.Key].Checked)
//                    continue;

//                GameImage item = kvp.Value;
//                TweenScale tp = item.GetComponent<TweenScale>();
//                tp.enabled = true;
//                tp.ResetAndPlay();
//            }

//            int delta_score = 0;

//            foreach (var phase in phase_finish_msg_.PhaseInfos)
//            {
//                foreach (var item in phase.MatchWordScoreInfo)
//                    delta_score += item.KeyWordMatchScore;
//            }

//            EventGameManager.Instance.Score = EventGameManager.Instance.LastScore + delta_score;
//            AddScore(EventGameManager.Instance.LastScore, EventGameManager.Instance.Score);
//            EventGameManager.Instance.LastScore = EventGameManager.Instance.Score;

//            TimeModule.Instance.SetTimeout(DelayShowDialog, 1.0f);

//        }

//        private void DelayShowDialog()
//        {
//            ShowDialog(m_finished_phase_id, m_phase_finish_msg);
//        }

//        private void ShowDialog(long finished_phase_id_, SCEventPhaseFeedbackResponse phase_finish_msg_)
//        {
//            m_valuation_normal_effect.Visible = false;
//            m_valuation_perfect_effect.Visible = false;

//            m_finish_bg.Color = new Color(m_finish_bg.Color.r, m_finish_bg.Color.g, m_finish_bg.Color.b, 1.0f);
//            //m_dialog_view.Refresh(finished_phase_id_, phase_finish_msg_.Valuation ? 1 : 0, phase_finish_msg_.BestOfficer);
//            m_dialog_view.Visible = true;
//            EventGameManager.Instance.LastScore = EventGameManager.Instance.Score = phase_finish_msg_.TotalScore;
//            m_cur_score_num.Label.text = EventGameManager.Instance.LastScore.ToString();
//            m_score_progress.Value = ((float)EventGameManager.Instance.LastScore) / m_full_num;


//        }



//        private void AddScore(int cur_num_, int aim_num_)
//        {
//            TimeModule.Instance.SetTimeout(
//                () =>
//                {
//                    TweenText.Begin(m_cur_score_num.Label, C_TWEEN_TIME, 0.1f, cur_num_, aim_num_);
//                    m_score_progress.Value = ((float)cur_num_) / m_full_num;
//                    TweenProgressBarValue.Begin(m_score_progress.gameObject, C_TWEEN_TIME, ((float)cur_num_) / m_full_num, ((float)aim_num_) / m_full_num);
//                }, 1.0f
//            );
//            TimeModule.Instance.SetTimeout(() => CommonHelper.UpdateEffectPosByProgressbar(m_score_progress, m_score_p_effect, 0.01f, C_TWEEN_TIME), 1.25f);
//        }

//        public void PhaseFinish(long finished_phase_id_, SCEventPhaseFeedbackResponse phase_finish_msg_)
//        {
//            ShowComplete(phase_finish_msg_);
//            m_finished_phase_id = finished_phase_id_;
//            m_phase_finish_msg = phase_finish_msg_;

//            TimeModule.Instance.SetTimeout(DelayShowValuation, 3.0f);
//            //TimeModule.Instance.SetTimeout(() => ShowKeywords(phase_finish_msg_), 6.0f);
//            //TimeModule.Instance.SetTimeout(() => ShowDialog(finished_phase_id_, phase_finish_msg_), 7.5f);
//        }

//        private bool m_is_clicked = false;

//        public void OnClicked(GameObject obj)
//        {
//            if (m_is_clicked)
//            {
//                PopUpManager.OpenNormalOnePop("request_hourly");
//                return;
//            }

//            m_is_clicked = true;

//            EngineCoreEvents.AudioEvents.PlayAudio.SafeInvoke(Audio.AudioType.UISound,GameCustomAudioKey.event_choice.ToString(), null);

//            CurViewLogic().DispathPolice(EventGameManager.Instance.Will_dispatched_officer_id);

//            TimeModule.Instance.RemoveTimeaction(ResetIsClicked);
//            TimeModule.Instance.SetTimeout(ResetIsClicked, 5.0f);

//        }

//        private void ResetIsClicked()
//        {
//            m_is_clicked = false;
//        }

//        void OnPoliceItemClicked(long officer_id_, bool checked_)
//        {

//            DebugUtil.Log("点选警员 " + officer_id_);

//            if (checked_)
//                EventGameManager.Instance.Will_dispatched_officer_id.Add(officer_id_);
//            else
//                EventGameManager.Instance.Will_dispatched_officer_id.Remove(officer_id_);

//            this.RefreshKeyWordToggle();
//            this.RefreshPortrait(officer_id_, checked_);

//            FullCheck();
//        }

//        void FullCheck()
//        {
//            if (EventGameManager.Instance.CurPhaseKeyWordCount <= EventGameManager.Instance.Will_dispatched_officer_id.Count)
//            {
//                for (int i = 0; i < m_officer_grid.ChildCount; ++i)
//                {
//                    var item = m_officer_grid.GetChild<EventGameOfficerItemView>(i);

//                    if (EventGameManager.Instance.Will_dispatched_officer_id.Contains(item.Officer_id))
//                        item.EnableClick = true;
//                    else
//                        item.EnableClick = false;
//                }
//            }
//            else
//            {
//                for (int i = 0; i < m_officer_grid.ChildCount; ++i)
//                {
//                    var item = m_officer_grid.GetChild<EventGameOfficerItemView>(i);
//                    item.EnableClick = true;
//                }
//            }
//        }


//        private void RefreshKeyWordToUnChecked()
//        {
//            foreach (var item in m_key_word_toggle_dict.Values)
//            {
//                item.Checked = false;
//            }

//            foreach (var item in m_key_word_label_dict.Values)
//            {
//                item.color = Color.white;
//            }

//            foreach (var item in m_key_word_effect_dict.Values)
//            {
//                item.Visible = false;
//            }
//        }

//        private void RefreshKeyWordToggle()
//        {
//            if (EventGameManager.Instance.GetRetainOfficers().Count > 0 && 0 == EventGameManager.Instance.Will_dispatched_officer_id.Count)
//            {
//                m_ok_btn.Visible = false;
//                RefreshKeyWordToUnChecked();
//                return;
//            }
//            else
//            {
//                m_ok_btn.Visible = true;
//            }

//            RefreshKeyWordToUnChecked();

//            foreach (var officer_id in EventGameManager.Instance.Will_dispatched_officer_id)
//            {

//                List<long> cur_key_word = m_officer_key_word_id_dict[officer_id];

//                foreach (int key_id in cur_key_word)
//                {
//                    if (!m_key_word_toggle_dict.ContainsKey(key_id))
//                        continue;

//                    if (m_key_word_toggle_dict[key_id].Checked)
//                        continue;

//                    if (m_key_word_toggle_dict.ContainsKey(key_id))
//                    {
//                        m_key_word_toggle_dict[key_id].Checked = true;
//                    }

//                    if (m_key_word_label_dict.ContainsKey(key_id))
//                    {
//                        m_key_word_label_dict[key_id].color = MATCH_WORD_COLOR;
//                    }

//                    if (m_key_word_effect_dict.ContainsKey(key_id))
//                    {
//                        m_key_word_effect_dict[key_id].Visible = true;
//                    }
//                }
//            }
//        }


//        private void RefreshDropIcons(GameUIContainer grid_, List<long> item_ids)
//        {
//            for (int i = 0; i < item_ids.Count && i < grid_.ChildCount; ++i)
//            {
//                var item = grid_.GetChild<DropItemIcon>(i);
//                item.InitSprite(ConfProp.Get(item_ids[i]).icon, 0, item_ids[i]);
//                item.Visible = true;

//            }
//        }

//        private void RefreshPortrait(long officer_id_, bool checked_)
//        {
//            if (-1 == officer_id_)
//            {
//                m_dispatched_officer_icons.ForEach((item) => item.Visible = false);
//                return;
//            }

//            bool is_finded = false;
//            foreach (var item in m_dispatched_officer_icon_dict)
//            {
//                if (officer_id_ == item.Value.m_officer_id)
//                {
//                    item.Value.m_icon.Visible = checked_;

//                    is_finded = true;
//                    break;
//                }
//            }

//            if (is_finded)
//                return;

//            if (!checked_)
//                return;

//            for (int i = 0; i < m_dispatched_officer_icon_dict.Count; ++i)
//            {
//                if (!m_dispatched_officer_icon_dict[i].m_icon.Visible)
//                {
//                    m_dispatched_officer_icon_dict[i].m_officer_id = officer_id_;
//                    m_dispatched_officer_icon_dict[i].m_icon.Sprite = ConfOfficer.Get(officer_id_).icon;
//                    m_dispatched_officer_icon_dict[i].m_icon.Visible = true;
//                    break;
//                }
//            }
//        }

//        private readonly Vector3 READONLY_TIPS_OFFSET = new Vector3(-15.0f, 0.0f, 0.0f);

//        private void RefreshSelectedIcon(int idx)
//        {
//            for (int i = 0; i < this.m_officer_grid.ChildCount; ++i)
//            {
//                if (i == idx)
//                    continue;

//                ButtonIconItem item = this.m_officer_grid.GetChild<ButtonIconItem>(i);
//                item.UnChecked();
//            }
//        }




//    }
//}
