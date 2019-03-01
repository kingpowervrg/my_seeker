//using EngineCore;
//using System.Collections.Generic;
//using UnityEngine;

//namespace SeekerGame
//{
//    public class EventGameEntryUIView : BaseGameView<EventGameEntryUILogic>
//    {
//        #region 左边信息
//        //private GameLabel m_full_score_title;
//        //private GameLabel m_full_score_num;
//        //private GameLabel m_pass_score_title;
//        //private GameLabel m_pass_score_num;
//        //private GameLabel m_cost_title;
//        //private GameLabel m_cost_num;
//        private GameUIContainer m_keywords_grid;
//        private GameLabel m_vit_cost_num_txt;
//        private GameLabel m_output_title;
//        private GameLabel m_keywords_title;
//        private GameLabel m_coin_num_txt;
//        private GameLabel m_gift_name_txt;
//        //private List<OutPutItemView> m_outputs;

//        //private List<GameLabel> m_output_nums;
//        //private GameLabel m_common_drop_title;
//        //private GameUIContainer m_common_drop_grid;
//        //private GameLabel m_full_score_drop_title;
//        //private GameUIContainer m_full_score_drop_grid;
//        #endregion

//        #region 右边信息
//        private GameImage m_event_icon;
//        private GameLabel m_event_name;
//        private GameLabel m_event_desc;
//        private GameLabel m_case_name;
//        private GameTexture m_case_tex;
//        private GameButton m_ok_btn;
//        #endregion


//        public override void PreLoadView(EventGameEntryUILogic t)
//        {
//            base.PreLoadView(t);

//            EventGameEntryUILogic cur_logic = CurViewLogic();

//            //m_full_score_title = cur_logic.MakeView<GameLabel>("Panel_down:Image_left:Panel_full:Text_2");
//            //m_full_score_num = cur_logic.MakeView<GameLabel>("Panel_down:Image_left:Panel_full:Text_1");
//            //m_pass_score_title = cur_logic.MakeView<GameLabel>("Panel_down:Image_left:Panel_pass:Text_2");
//            //m_pass_score_num = cur_logic.MakeView<GameLabel>("Panel_down:Image_left:Panel_pass:Text_1");
//            //m_cost_title = cur_logic.MakeView<GameLabel>("Panel_down:Image_left:Panel_cost:Text_2");
//            //m_cost_num = cur_logic.MakeView<GameLabel>("Panel_down:Image_left:Panel_cost:Text_1");
//            m_keywords_title = cur_logic.MakeView<GameLabel>("Panel_down:Image_left:Panel_keyword:Text_2");
//            m_keywords_title.Text = LocalizeModule.Instance.GetString("ui.event.keyword");
//            m_keywords_grid = cur_logic.MakeView<GameUIContainer>("Panel_down:Image_left:Panel_keyword:ScrollView:Viewport");
//            m_vit_cost_num_txt = cur_logic.MakeView<GameLabel>("Panel_down:Button_action:Image:Text");
//            m_coin_num_txt = cur_logic.MakeView<GameLabel>("Panel_down:Image_left:Panel_output:Image_1:Text_1");
//            m_output_title = cur_logic.MakeView<GameLabel>("Panel_down:Image_left:Panel_output:Text_2");
//            m_gift_name_txt = cur_logic.MakeView<GameLabel>("Panel_down:Image_left:Panel_output:Image_2:Text_1");
//            m_gift_name_txt.Text = LocalizeModule.Instance.GetString("ui.event.giftname");

//            //m_outputs = new List<OutPutItemView>();

//            //for (int i = 1; i < 5; ++i)
//            //{
//            //    OutPutItemView output_item = new OutPutItemView();
//            //    output_item.m_icon = cur_logic.MakeView<GameImage>(string.Format("Panel_down:Image_left:Panel_output:Image_{0}", i));
//            //    output_item.m_num = output_item.m_icon.Make<GameLabel>("Text_1");
//            //    m_outputs.Add(output_item);
//            //}

//            //m_output_nums = new List<GameLabel>
//            //{
//            //    cur_logic.MakeView<GameLabel>("Panel_down:Image_left:Panel_output:Image_1:Text_1"),
//            //    cur_logic.MakeView<GameLabel>("Panel_down:Image_left:Panel_output:Image_2:Text_1"),
//            //    cur_logic.MakeView<GameLabel>("Panel_down:Image_left:Panel_output:Image_3:Text_1"),
//            //    cur_logic.MakeView<GameLabel>("Panel_down:Image_left:Panel_output:Image_4:Text_1"),
//            // };
//            //m_common_drop_title = cur_logic.MakeView<GameLabel>("Panel_down:Image_left:Panel_drop1:Text_2");
//            //m_common_drop_grid = cur_logic.MakeView<GameUIContainer>("Panel_down:Image_left:Panel_drop1:Scroll View:Viewport:Grid");
//            //m_full_score_drop_title = cur_logic.MakeView<GameLabel>("Panel_down:Image_left:Panel_drop2:Text_2");
//            //m_full_score_drop_grid = cur_logic.MakeView<GameUIContainer>("Panel_down:Image_left:Panel_drop2:Scroll View:Viewport:Grid");

//            m_event_icon = cur_logic.MakeView<GameImage>("Panel_down:Image_event:Image_title:Image");
//            m_event_name = cur_logic.MakeView<GameLabel>("Panel_down:Image_event:Image_title:Text");
//            m_event_desc = cur_logic.MakeView<GameLabel>("Panel_down:Image_event:Text");
//            m_case_name = cur_logic.MakeView<GameLabel>("Panel_down:Image_event:RawImage:Text");
//            m_case_tex = cur_logic.MakeView<GameTexture>("Panel_down:Image_event:RawImage");
//            m_ok_btn = cur_logic.MakeView<GameButton>("Panel_down:Button_action");
//        }

//        public override void HideView()
//        {
//            base.HideView();

//            this.RemoveClick();
//        }



//        public override void ShowView(object s)
//        {
//            base.ShowView(s);

//            if (s is EventGameEntryData)
//            {
//                var data = s as EventGameEntryData;

//                ConfEvent event_data = ConfEvent.Get(data.M_event_id);

//                //m_full_score_title.Text = LocalizeModule.Instance.GetString("ui.event.FullScore");
//                //m_full_score_num.Text = event_data.perfectMark.ToString();
//                //m_pass_score_title.Text = LocalizeModule.Instance.GetString("ui.event.PassScore");
//                //m_pass_score_num.Text = event_data.passMark.ToString();
//                //m_cost_title.Text = LocalizeModule.Instance.GetString("ui.event.Cost");
//                //m_cost_num.Text = event_data.vitConsume.ToString();
//                m_vit_cost_num_txt.Text = event_data.vitConsume.ToString();
//                m_output_title.Text = LocalizeModule.Instance.GetString("ui.event.Output");
//                m_coin_num_txt.Text = event_data.coinGain.ToString();

//                List<string> pngs = new List<string>();

//                foreach (var phase_id in event_data.phases)
//                {
//                    var phase = ConfEventPhase.Get(phase_id);

//                    foreach (var key_id in phase.keyWords)
//                    {
//                        pngs.Add(ConfKeyWords.Get(key_id).icon);
//                    }
//                }

//                m_keywords_grid.EnsureSize<GameImage>(pngs.Count);

//                for (int i = 0; i < m_keywords_grid.ChildCount; ++i)
//                {
//                    m_keywords_grid.GetChild<GameImage>(i).Sprite = pngs[i];
//                    m_keywords_grid.GetChild<GameImage>(i).Visible = true;
//                }

//                //CommonHelper.ShowOutput(m_outputs, event_data.expGain, event_data.coinGain, event_data.cashGain, event_data.vitGain);

//                //m_common_drop_title.Text = LocalizeModule.Instance.GetString("ui.event.CommonDrop");

//                //if (event_data.normalDropId > 0)
//                //{
//                //    List<long> ids = CommonHelper.GetDropOuts(event_data.normalDropId);

//                //    m_common_drop_grid.EnsureSize<DropItemIcon>(ids.Count);


//                //    this.RefreshDropIcons(m_common_drop_grid, ids);
//                //}
//                //else
//                //{
//                //    m_common_drop_grid.Clear();
//                //}
//                //m_full_score_drop_title.Text = LocalizeModule.Instance.GetString("ui.event.FullScoreDrop");

//                //if (event_data.perfectDropId > 0)
//                //{
//                //    List<long> ids = CommonHelper.GetDropOuts(event_data.normalDropId);

//                //    m_full_score_drop_grid.EnsureSize<DropItemIcon>(ids.Count);
//                //    this.RefreshDropIcons(m_full_score_drop_grid, ids);
//                //}
//                //else
//                //{
//                //    m_full_score_drop_grid.Clear();
//                //}

//                m_event_icon.Sprite = ConfEventAttribute.Get(event_data.type).icon;
//                m_event_name.Text = LocalizeModule.Instance.GetString(event_data.name);
//                m_event_desc.Text = LocalizeModule.Instance.GetString(event_data.descs);
//                m_case_name.Text = LocalizeModule.Instance.GetString(ConfEventAttribute.Get(event_data.type).name);
//                m_case_tex.TextureName = event_data.sceneInfo;


//                Dictionary<UBSParamKeyName, object> _params = new Dictionary<UBSParamKeyName, object>()
//                        {
//                            { UBSParamKeyName.Success, 1},
//                            { UBSParamKeyName.ContentID, data.M_event_id},
//                        };
//                UserBehaviorStatisticsModules.Instance.LogEvent(UBSEventKeyName.event_begin, null, _params);

//            }
//            else if (s is CartoonGameEntryData)
//            {
//                var data = s as CartoonGameEntryData;

//                ConfCartoonScene cartoon = ConfCartoonScene.Get(data.M_cartoon_id);

//                if (null != cartoon)
//                {
//                    //CommonHelper.ShowOutput(m_outputs, cartoon.rewardExp, cartoon.rewardCoin, cartoon.rewardCash, cartoon.rewardVit);
//                    m_case_tex.TextureName = cartoon.thumbnail;
//                    m_event_name.Text = LocalizeModule.Instance.GetString(cartoon.name);
//                }
//            }

//            this.AddClick();
//        }

//        void AddClick()
//        {
//            this.m_ok_btn.AddClickCallBack(Clicked);
//        }

//        void RemoveClick()
//        {
//            this.m_ok_btn.RemoveClickCallBack(Clicked);
//        }

//        public void Clicked(GameObject obj)
//        {
//            EngineCoreEvents.AudioEvents.PlayAudio.SafeInvoke(Audio.AudioType.UISound,GameCustomAudioKey.game_star.ToString(), null);
//            CurViewLogic().OnStartGameClicked();
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



//    }
//}
