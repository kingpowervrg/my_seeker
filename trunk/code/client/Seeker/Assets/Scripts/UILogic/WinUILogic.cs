////#define TRY_AGAIN
//using EngineCore;
//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using GOEngine;

//namespace SeekerGame
//{
//    //[UILogicHandler(UIDefine.UI_WIN)]
//    public class WinUILogic : UILogicBase
//    {
//        private const string C_EXP = "exp", C_VIT = "vit", C_COIN = "coin", C_CASH = "cash";

//        private GameUIComponent m_win;
//        private GameUIComponent m_fail;
//        private GameButton m_end_btn;
//        private GameButton m_continue_btn;

//        private GameLabel m_win_text;
//        private GameLabel m_fail_text;
//        private GameLabel m_fail_info_text;

//        private Dictionary<string, GameLabel> m_output_txt_dict;
//        private List<GameUIEffect> m_output_effects;
//        private GameUIEffect m_win_title_effect;
//        private GameUIEffect m_fail_title_effect;

//        private GameUIContainer m_drop_grid;

//        private GameLabel m_score_title;
//        private GameLabel m_score_num;

//        private List<string> m_exclude_ui = new List<string>();

//        private ENUM_SEARCH_MODE m_mode = ENUM_SEARCH_MODE.E_INVALID;


//        private List<System.Action> m_effect_act_list;

//        protected override void OnInit()
//        {
//            base.OnInit();
//            m_exclude_ui.Add(UIDefine.UI_WIN);
//            m_win = Make<GameUIComponent>("Panel_win");
//            m_fail = Make<GameUIComponent>("Panel_lose");
//            m_end_btn = Make<GameButton>("Panel_win:Button_continue");
//            m_continue_btn = Make<GameButton>("Panel_lose:Button_continue");

//            m_win_text = Make<GameLabel>("Panel_win:Text_win");
//            m_fail_text = Make<GameLabel>("Panel_lose:Text_win");
//            m_fail_info_text = Make<GameLabel>("Panel_lose:Text_1");

//            m_output_txt_dict = new Dictionary<string, GameLabel>()
//            {
//                { C_EXP, Make<GameLabel>("Panel_win:Image_1:Text_1")},
//                { C_VIT, Make<GameLabel>("Panel_win:Image_2:Text_1")},
//                { C_COIN, Make<GameLabel>("Panel_win:Image_3:Text_1")},
//                { C_CASH, Make<GameLabel>("Panel_win:Image_4:Text_1")},
//            };

//            m_output_effects = new List<GameUIEffect>()
//            {
//                Make<GameUIEffect>("Panel_win:Image_1:UI_chanchuneirong"),
//                Make<GameUIEffect>("Panel_win:Image_2:UI_chanchuneirong"),
//                Make<GameUIEffect>("Panel_win:Image_3:UI_chanchuneirong"),
//                Make<GameUIEffect>("Panel_win:Image_4:UI_chanchuneirong"),
//            };



//            foreach (var item in m_output_effects)
//            {
//                item.EffectPrefabName = "UI_chanchuneirong.prefab";
//                item.Visible = false;
//            }

//            m_win_title_effect = Make<GameUIEffect>("Panel_win:Text_win:UI_chujing_chenggong");
//            m_win_title_effect.EffectPrefabName = "UI_chujing_chenggong.prefab";
//            m_win_title_effect.Visible = false;

//            m_fail_title_effect = Make<GameUIEffect>("Panel_lose:Text_win:UI_chujing_shibai");
//            m_fail_title_effect.EffectPrefabName = "UI_chujing_shibai.prefab";
//            m_fail_title_effect.Visible = false;

//            m_drop_grid = Make<GameUIContainer>("Panel_win:Scroll View:Viewport");
//            m_score_title = Make<GameLabel>("Panel_win:Text_score");
//            m_score_title.Text = string.Format("{0}:", LocalizeModule.Instance.GetString("ui.eventIngame1.score"));
//            m_score_num = Make<GameLabel>("Panel_win:Text_score:Text_1");
//        }

//        public override void OnShow(object param)
//        {
//            base.OnShow(param);

//            m_end_btn.AddClickCallBack(QuitClicked);
//            m_continue_btn.AddClickCallBack(QuitClicked);

//            m_output_txt_dict[C_EXP].Text = "0";
//            m_output_txt_dict[C_VIT].Text = "0";
//            m_output_txt_dict[C_COIN].Text = "0";
//            m_output_txt_dict[C_CASH].Text = "0";

//            if (null != param)
//            {
//                var ret = param as WinFailData;

//                this.m_mode = ret.m_mode;

//                string txt;


//                m_win_title_effect.Visible = false;
//                m_fail_title_effect.Visible = false;
//                m_score_num.Visible = true;
//                m_score_title.Visible = true;
//                if (ENUM_SEARCH_MODE.E_EVENTGAME == this.m_mode)
//                {
//                    var msg = ret.m_msg as SCEventRewardResponse;

//                    if (1 == msg.Valuation || 2 == msg.Valuation)
//                    {
//                        m_win.Visible = true;
//                        m_fail.Visible = false;

//                        if (1 == msg.Valuation)
//                        {
//                            txt = "UI_sence_win.normalwin";
//                        }
//                        else
//                        {
//                            txt = "UI_sence_win.perfectwin";
//                        }

//                        m_win_text.Text = LocalizeModule.Instance.GetString(txt);

//                        m_output_txt_dict[C_EXP].Text = msg.Exp.ToString();
//                        m_output_txt_dict[C_VIT].Text = msg.Vit.ToString();
//                        m_output_txt_dict[C_COIN].Text = msg.Coin.ToString();
//                        m_output_txt_dict[C_CASH].Text = msg.Cash.ToString();

//                        m_drop_grid.EnsureSize<DropItemIcon>(msg.Rewards.Count);

//                        for (int i = 0; i < m_drop_grid.ChildCount; ++i)
//                        {
//                            var item = m_drop_grid.GetChild<DropItemIcon>(i);
//                            item.InitSprite(ConfProp.Get(msg.Rewards[i].PropId).icon);
//                            item.Visible = false;
//                            item.Visible = true;
//                        }


//                        m_score_num.Text = msg.Score.ToString();

//                        m_win_title_effect.ReplayEffect();
//                        ShowOutputEffect();
//                    }
//                    else
//                    {
//                        m_win.Visible = false;
//                        m_fail.Visible = true;

//                        txt = "UI_sence_fail";
//                        m_fail_text.Text = LocalizeModule.Instance.GetString(txt);
//                        txt = "ui.UI_event_ingame_2.fail";
//                        m_fail_info_text.Text = LocalizeModule.Instance.GetString(txt);

//                        m_fail_title_effect.ReplayEffect();
//                    }
//                }
//                else if (ENUM_SEARCH_MODE.E_JIGSAW == this.m_mode)
//                {


//                    var msg = ret.m_msg as SCFinishResponse;
//                    ShowOutputEffect();
//                    if (1 == msg.Result)
//                    {
//                        m_win.Visible = true;
//                        m_fail.Visible = false;

//                        m_win_title_effect.ReplayEffect();

//                        foreach (var item in msg.Rewards)
//                        {
//                            switch (item.Type)
//                            {
//                                case (int)EUNM_BASE_REWARD.E_CASH:
//                                    m_output_txt_dict[C_CASH].Text = item.Num.ToString();
//                                    break;
//                                case (int)EUNM_BASE_REWARD.E_COIN:

//                                    m_output_txt_dict[C_COIN].Text = item.Num.ToString();

//                                    break;
//                                case (int)EUNM_BASE_REWARD.E_EXP:
//                                    m_output_txt_dict[C_EXP].Text = item.Num.ToString();
//                                    break;
//                                case (int)EUNM_BASE_REWARD.E_VIT:
//                                    m_output_txt_dict[C_VIT].Text = item.Num.ToString();
//                                    break;
//                            }
//                        }

//                        ConfProp prop = ConfProp.Get(msg.PropId);

//                        if (null != prop)
//                        {
//                            m_drop_grid.EnsureSize<DropItemIcon>(1);

//                            for (int i = 0; i < m_drop_grid.ChildCount; ++i)
//                            {
//                                var item = m_drop_grid.GetChild<DropItemIcon>(i);
//                                item.InitSprite(prop.icon);
//                                item.Visible = false;
//                                item.Visible = true;
//                            }
//                        }
//                    }
//                    else
//                    {
//                        m_win.Visible = false;
//                        m_fail.Visible = true;

//                        txt = "UI_sence_fail";
//                        m_fail_text.Text = LocalizeModule.Instance.GetString(txt);
//                        txt = "pintu_fail";
//                        m_fail_info_text.Text = LocalizeModule.Instance.GetString(txt);


//                        m_fail_title_effect.ReplayEffect();
//                    }
//                    m_score_num.Visible = false;
//                    m_score_title.Visible = false;
//                }
//                else if (ENUM_SEARCH_MODE.E_CARTOON == this.m_mode)
//                {
//                    var msg = ret.m_msg as SCCartoonRewardReqsponse;

//                    m_output_txt_dict[C_EXP].Text = msg.Exp.ToString();
//                    m_output_txt_dict[C_VIT].Text = msg.Vit.ToString();
//                    m_output_txt_dict[C_COIN].Text = msg.Coin.ToString();
//                    m_output_txt_dict[C_CASH].Text = msg.Cash.ToString();


//                    m_win.Visible = true;
//                    m_fail.Visible = false;

//                    m_win_title_effect.Visible = true;

//                    ShowOutputEffect();

//                }
//            }

//        }

//        public override void OnHide()
//        {
//            base.OnHide();

//            m_end_btn.RemoveClickCallBack(QuitClicked);
//            m_continue_btn.RemoveClickCallBack(QuitClicked);

//            if (null != m_effect_act_list)
//            {
//                m_effect_act_list.ForEach(item => TimeModule.Instance.RemoveTimeaction(item));
//                m_effect_act_list.Clear();
//            }
//            m_output_effects.ForEach((item) => { item.Visible = false; });
//            m_win_title_effect.Visible = false;
//            m_fail_title_effect.Visible = false;


//            if (ENUM_SEARCH_MODE.E_JIGSAW == this.m_mode)
//            {

//                EngineCoreEvents.UIEvent.HideUIWithParamEvent.SafeInvoke(new FrameMgr.HideUIParams(UIDefine.UI_JIGSAW) { DestroyFrame = true, DestoryFrameDelayTime = 0 });
//#if TRY_AGAIN
//                EngineCoreEvents.UIEvent.ShowUIEventWithParam.SafeInvoke(new FrameMgr.OpenUIParams(UIDefine.UI_ENGER_GAME_UI)
//                {
//                    Param = this.m_rsp_scene_id
//                });
//#endif
//            }
//            else if (ENUM_SEARCH_MODE.E_SEARCH_ROOM == this.m_mode)
//            {
//                FrameMgr.Instance.HideAllFrames(m_exclude_ui);
//                EngineCoreEvents.UIEvent.ShowUIEvent.SafeInvoke(UIDefine.UI_ENGER_GAME_UI);
//            }
//            else if (ENUM_SEARCH_MODE.E_EVENTGAME == this.m_mode)
//            {
//                EngineCoreEvents.UIEvent.HideUIEvent.SafeInvoke(UIDefine.UI_EVENT_INGAME_SCORE);
//            }

//            GameEvents.PlayerEvents.RequestLatestPlayerInfo();
//        }

//        private void OnScResponse(object s)
//        {

//        }

//        private void QuitClicked(GameObject go)
//        {
//            this.CloseFrame();
//        }





//        private void ShowOutputEffect()
//        {
//            if (null != m_effect_act_list)
//            {
//                m_effect_act_list.ForEach(item => TimeModule.Instance.RemoveTimeaction(item));
//            }

//            m_effect_act_list = new List<System.Action>();

//            float interval_time = 0.1f;

//            for (int i = 0; i < m_output_effects.Count; ++i)
//            {
//                //TimeModule.Instance.SetTimeout(() => { m_output_effects[i].Visible = false; m_output_effects[i].Visible = true; }, interval_time * i + 0.1f);
//                GameUIEffect _eff = m_output_effects[i];

//                System.Action one_effect = () => { TimeToShow(_eff); };
//                m_effect_act_list.Add(one_effect);

//                TimeModule.Instance.SetTimeout(one_effect, interval_time * i + 0.1f);
//            }
//        }



//        void TimeToShow(GameUIEffect eff_)
//        {
//            eff_.Visible = false;
//            eff_.Visible = true;
//        }

//    }
//}