using EngineCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace SeekerGame
{
    class JigsawEnterUI : BaseViewComponet<JigsawEnterUILogic>
    {

        private GameLabel m_desc_txt;
        private GameLabel m_output_title_txt;
        private GameLabel m_time_txt;

        private GameTexture m_thumbnail_tex;

        private GameUIContainer m_output_grid;

        private GameLabel m_case_name_txt;
        private GameLabel m_action_btn_title_txt;
        private GameUIEffect m_action_btn_effect;
        private GameImage m_ticket_icon;
        private GameLabel m_action_btn_num_txt;
        private GameButton m_action_btn;

        private GameButton m_btnClose = null;

        private GameImage m_btn_play_video = null;
        private GameButton m_btn_close_video = null;
        private GameUIComponent m_video_root = null;
        private GameVideoComponent m_video = null;

        private GameImage m_bg_img;
        private TweenAlpha m_bg_alpha;
        private GameUIComponent m_tween_root;
        private TweenScale m_show_tween_scale;
        private TweenAlpha m_show_tween_alpha;
        private TweenPosition m_show_tween_pos;


        private int m_item_in_bag_num;
        private int m_item_cost_num;
        private Dictionary<int, Dictionary<string, int>> m_exist_output = null;

        private long m_task_id;

        protected override void OnInit()
        {
            base.OnInit();
            m_desc_txt = this.Make<GameLabel>("Panel_down:Image_1:Text");
            m_output_title_txt = this.Make<GameLabel>("Panel_down:RawImage:Panel_output:Text_2");
            m_time_txt = Make<GameLabel>("Panel_down:RawImage:Panel_output:Image:Text");
            m_thumbnail_tex = Make<GameTexture>("Panel_down:RawImage");
            m_output_grid = this.Make<GameUIContainer>("Panel_down:RawImage:Panel_output:Grid");

            m_case_name_txt = this.Make<GameLabel>("Panel_down:RawImage:Image:Text");
            m_action_btn = this.Make<GameButton>("Panel_down:btn_action");
            m_action_btn_title_txt = m_action_btn.Make<GameLabel>("Text");
            m_action_btn_effect = m_action_btn.Make<GameUIEffect>("UI_pintukaishi_anniu");
            m_action_btn_effect.EffectPrefabName = "UI_pintukaishi_anniu.prefab";
            m_ticket_icon = m_action_btn.Make<GameImage>("Image_item");
            m_action_btn_num_txt = m_action_btn.Make<GameLabel>("Text_item");

            this.m_btnClose = Make<GameButton>("Panel_down:Button_close");

            m_video_root = Make<GameUIComponent>("Panel_down:video_Image");
            m_video = m_video_root.Make<GameVideoComponent>("RawImage_Video");


            m_btn_play_video = Make<GameImage>("Panel_down:RawImage:Image_play");
            m_btn_close_video = m_video.Make<GameButton>("Btn_Close_Video");

            m_bg_img = Make<GameImage>("RawImage");
            m_bg_alpha = m_bg_img.GetComponent<TweenAlpha>();
            m_tween_root = Make<GameUIComponent>("Panel_down");
            //m_show_tween_scale = m_tween_root.GetComponent<TweenScale>();
            //m_show_tween_scale.AddTweenCompletedCallback(ShowTweenScaleFinished);
            //var show_tween_poses = m_tween_root.GetComponents<TweenPosition>().Where((i) => UITweenerBase.TweenTriggerType.Manual == i.m_triggerType);
            //m_show_tween_pos = show_tween_poses.First();

            //var show_tween_alphas = m_tween_root.GetComponents<TweenAlpha>().Where((i) => UITweenerBase.TweenTriggerType.OnShow == i.m_triggerType);
            //m_show_tween_alpha = show_tween_alphas.First();
        }

        void ShowTweenScaleFinished()
        {
            m_action_btn_effect.Visible = true;
        }

        void BgShowAlpha(bool tween)
        {
            if (tween)
            {
                m_bg_alpha.From = 0.1f;
                m_bg_alpha.To = 1.0f;
                m_bg_alpha.ResetAndPlay();
            }
            else
            {
                m_bg_img.Color = new Color(m_bg_img.Color.r, m_bg_img.Color.g, m_bg_img.Color.b, 1.0f);
            }
        }

        void PanelDelayShowAlpha(float delay_)
        {
            m_show_tween_alpha.Delay = delay_;
        }

        void PanelDelayShowScale(float delay_)
        {
            m_show_tween_scale.Delay = delay_;
        }

        void PanelTurnToOne(bool is_slow_)
        {
            if (is_slow_)
            {
                m_action_btn_effect.Visible = false;
                m_show_tween_scale.ResetAndPlay();
            }
            else
                m_show_tween_scale.gameObject.transform.localScale = Vector3.one;
        }

        void PanelTurnToZero(bool is_slow_)
        {
            if (is_slow_)
                m_show_tween_scale.ResetAndPlay(false);
            else
                m_show_tween_scale.gameObject.transform.localScale = new Vector3(0, 1, 1);
        }

        void PanelGoDown(bool is_slow_)
        {
            if (is_slow_)
            {
                m_show_tween_pos.Duration = 0.4f;
                m_show_tween_pos.ResetAndPlay();
            }
            else
            {
                m_show_tween_pos.Duration = 0.0f;
                m_show_tween_pos.ResetAndPlay();
            }
        }

        void PanelGoUp(bool is_slow_)
        {
            if (is_slow_)
            {
                m_show_tween_pos.Duration = 0.4f;
                m_show_tween_pos.ResetAndPlay(false);
            }
            else
            {
                m_show_tween_pos.Duration = 0.0f;
                m_show_tween_pos.ResetAndPlay(false);
            }
        }

        public override void OnShow(object param)
        {
            base.OnShow(param);
            m_action_btn.AddClickCallBack(OnClick);
            m_btn_play_video.AddClickCallBack(OnVideoPlayClick);
            m_btn_close_video.AddClickCallBack(OnVideoCloseClick);
            m_btnClose.AddClickCallBack(OnRootCloseClick);
            this.m_btnClose.Visible = GlobalInfo.GAME_NETMODE == GameNetworkMode.Network;

            if (null != param)
            {
                List<long> my_param = param as List<long>;
                this.m_task_id = my_param[1];
            }

            //if (-1 == m_task_id)
            //{
            //    //不是从任务展示界面来的，不用翻转
            //    BgShowAlpha(false);
            //    PanelDelayShowAlpha(0.0f);
            //    PanelDelayShowScale(0.0f);
            //    PanelTurnToOne(false);
            //    PanelGoDown(true);
            //}
            //else
            //{
            //    //需要翻转

            //    BgShowAlpha(true);
            //    PanelDelayShowAlpha(0.2f);
            //    PanelDelayShowScale(0.5f);
            //    PanelGoDown(false);
            //    PanelTurnToZero(false);
            //    PanelTurnToOne(true);
            //}
        }

        public override void OnHide()
        {
            base.OnHide();
            m_action_btn.RemoveClickCallBack(OnClick);
            m_btn_play_video.RemoveClickCallBack(OnVideoPlayClick);
            m_btn_close_video.RemoveClickCallBack(OnVideoCloseClick);
            m_btnClose.RemoveClickCallBack(OnRootCloseClick);
        }

        public void Refresh(long scene_id_, long task_id_ = -1)
        {
            m_task_id = task_id_;
            ConfJigsawScene data = ConfJigsawScene.Get(scene_id_);

            m_desc_txt.Text = LocalizeModule.Instance.GetString(data.desc2);

            m_output_title_txt.Text = LocalizeModule.Instance.GetString("ui.event.Output");

            m_time_txt.Text = CommonTools.SecondToStringMMSS(data.percent4);


            m_thumbnail_tex.TextureName = data.thumbnail;

            RefreshOutputData(data);
            RefreshOutputView();

            //m_drop_title_txt.Text = LocalizeModule.Instance.GetString("ui.event.CommonDrop");

            //if (data.propId > 0)
            //{
            //    List<long> ids = CommonHelper.GetDropOuts(data.propId);

            //    m_drop_grid.EnsureSize<DropItemIcon>(ids.Count);

            //    for (int i = 0; i < m_drop_grid.ChildCount; ++i)
            //    {
            //        m_drop_grid.GetChild<DropItemIcon>(i).InitSprite(ConfProp.Get(ids[i]).icon, 0, ids[i]);
            //        m_drop_grid.GetChild<DropItemIcon>(i).Visible = true;
            //    }

            //}
            //else
            //{
            //    m_drop_grid.Clear();
            //}

            m_case_name_txt.Text = LocalizeModule.Instance.GetString(data.name);

            if (data.costPropIds.Length > 0)
            {
                long prop_id = data.costPropIds[0];
                m_ticket_icon.Sprite = ConfProp.Get(prop_id).icon;
                m_item_in_bag_num = null != GlobalInfo.MY_PLAYER_INFO.GetBagInfosByID(prop_id) ? GlobalInfo.MY_PLAYER_INFO.GetBagInfosByID(prop_id).Count : 0;
                m_item_cost_num = data.costPropNums[0];

                m_action_btn_num_txt.Text = string.Format("{0}/{1}", m_item_in_bag_num, m_item_cost_num);

                if (m_item_in_bag_num >= m_item_cost_num)
                {
                    m_action_btn_num_txt.color = Color.white;
                }
                else
                {
                    m_action_btn_num_txt.color = Color.red;
                }
            }
            else
            {
                m_action_btn_num_txt.Text = "";
            }
        }

        //private void RefreshOutputView()
        //{

        //    m_output_grid.EnsureSize<JigsawOutputIcon>(m_exist_output.Count);

        //    int j = 0;
        //    foreach (KeyValuePair<int, Dictionary<string, int>> kvp in m_exist_output)
        //    {
        //        int output_type = kvp.Key;
        //        int max_num = kvp.Value["max"];
        //        int min_num = kvp.Value["min"];
        //        string icon_name = "";
        //        switch (output_type)
        //        {
        //            case ((int)EUNM_BASE_REWARD.E_COIN):
        //                {
        //                    icon_name = "icon_mainpanel_coin_2.png";
        //                }
        //                break;
        //            case ((int)EUNM_BASE_REWARD.E_CASH):
        //                {

        //                    icon_name = "icon_mainpanel_cash_2.png";

        //                }
        //                break;
        //            case ((int)EUNM_BASE_REWARD.E_EXP):
        //                {

        //                    icon_name = "icon_mainpanel_exp_2.png";

        //                }
        //                break;
        //            case ((int)EUNM_BASE_REWARD.E_VIT):
        //                {

        //                    icon_name = "icon_mainpanel_energy_2.png";

        //                }
        //                break;
        //        }

        //        m_output_grid.GetChild<JigsawOutputIcon>(j).Refresh(icon_name, max_num);
        //        m_output_grid.GetChild<JigsawOutputIcon>(j).Visible = false;
        //        m_output_grid.GetChild<JigsawOutputIcon>(j).Visible = true;
        //        ++j;
        //    }
        //}


        private void RefreshOutputView()
        {

            m_output_grid.EnsureSize<DropItemIcon>(m_exist_output.Count);

            int j = 0;
            foreach (KeyValuePair<int, Dictionary<string, int>> kvp in m_exist_output)
            {
                int output_type = kvp.Key;
                int max_num = kvp.Value["max"];
                int min_num = kvp.Value["min"];
                string icon_name = "";
                switch (output_type)
                {
                    case ((int)EUNM_BASE_REWARD.E_COIN):
                        {
                            icon_name = "icon_mainpanel_coin_2.png";
                        }
                        break;
                    case ((int)EUNM_BASE_REWARD.E_CASH):
                        {

                            icon_name = "icon_mainpanel_cash_2.png";

                        }
                        break;
                    case ((int)EUNM_BASE_REWARD.E_EXP):
                        {

                            icon_name = "icon_mainpanel_exp_2.png";

                        }
                        break;
                    case ((int)EUNM_BASE_REWARD.E_VIT):
                        {

                            icon_name = "icon_mainpanel_energy_2.png";

                        }
                        break;
                }

                m_output_grid.GetChild<DropItemIcon>(j).InitSprite(icon_name, $"{min_num}~{max_num}", 0);
                m_output_grid.GetChild<DropItemIcon>(j).Visible = false;
                m_output_grid.GetChild<DropItemIcon>(j).Visible = true;
                ++j;
            }
        }

        private void RefreshOutputData(ConfJigsawScene data)
        {
            //foreach (var item in m_output_icon_list)
            //{
            //    item.Visible = false;
            //}

            //foreach (var item in m_output_txt_list)
            //{
            //    item.Visible = false;
            //}

            List<int> output_types = new List<int>()
            {
                data.type4 ,
                data.type3 ,
                data.type2 ,
                data.type1 ,
            };

            List<int> output_nums = new List<int>()
            {
                data.num4 ,
                data.num3 ,
                data.num2 ,
                data.num1 ,
            };

            Dictionary<int, Dictionary<string, int>> exist_output = new Dictionary<int, Dictionary<string, int>>();

            for (int i = 0; i < output_types.Count; ++i)
            {
                int cur_type = output_types[i];
                int cur_num = output_nums[i];

                if (exist_output.ContainsKey(cur_type))
                {
                    if (cur_num < exist_output[cur_type]["min"])
                    {
                        int old_min = exist_output[cur_type]["min"];
                        exist_output[cur_type]["min"] = cur_num;
                        exist_output[cur_type]["max"] = old_min;
                    }
                    else
                    {
                        if (cur_num > exist_output[cur_type]["max"])
                        {
                            exist_output[cur_type]["max"] = cur_num;
                        }
                    }
                }
                else
                {
                    exist_output[cur_type] = new Dictionary<string, int>();
                    exist_output[cur_type]["max"] = cur_num;
                    exist_output[cur_type]["min"] = cur_num;
                }

            }

            m_exist_output = exist_output;

            //int j = 0;
            //foreach (KeyValuePair<int, Dictionary<string, int>> kvp in exist_output)
            //{
            //    int output_type = kvp.Key;
            //    int max_num = kvp.Value["max"];
            //    int min_num = kvp.Value["min"];

            //    m_output_txt_list[j].Visible = true;
            //    if (min_num != max_num)
            //        m_output_txt_list[j].Text = string.Format("~{0}", max_num);
            //    else
            //        m_output_txt_list[j].Text = string.Format("~{0}", min_num);

            //    m_output_icon_list[j].Visible = true;
            //    switch (output_type)
            //    {
            //        case ((int)EUNM_BASE_REWARD.E_COIN):
            //            {
            //                m_output_icon_list[j].Sprite = "icon_mainpanel_coin.png";
            //            }
            //            break;
            //        case ((int)EUNM_BASE_REWARD.E_CASH):
            //            {

            //                m_output_icon_list[j].Sprite = "icon_mainpanel_cash.png";

            //            }
            //            break;
            //        case ((int)EUNM_BASE_REWARD.E_EXP):
            //            {

            //                m_output_icon_list[j].Sprite = "icon_mainpanel_exp.png";

            //            }
            //            break;
            //        case ((int)EUNM_BASE_REWARD.E_VIT):
            //            {

            //                m_output_icon_list[j].Sprite = "icon_mainpanel_energy.png";

            //            }
            //            break;
            //    }

            //    ++j;
            //}
        }

        private void OnClick(GameObject obj)
        {
            EngineCoreEvents.AudioEvents.PlayAudio.SafeInvoke(Audio.AudioType.UISound, GameCustomAudioKey.game_star.ToString());

            if (m_item_in_bag_num >= m_item_cost_num)
                CurViewLogic().StartGame();
            else
            {
                PopUpData pd = new PopUpData()
                {
                    title = string.Empty,
                    content = "action_pt_mp",
                    isOneBtn = true,
                    OneButtonText = "UI.OK",
                };

                PopUpManager.OpenPopUp(pd);
            }


        }

        private void OnVideoCloseClick(GameObject obj)
        {
            m_video_root.Visible = false;
            //m_video.Visible = false;
        }


        private void OnRootCloseClick(GameObject obj)
        {
            CurViewLogic().CloseFrame();
        }


        private void OnVideoPlayClick(GameObject obj)
        {
            m_video_root.Visible = true;
            //m_video.Visible = true;
            m_video.VideoName = "pintu_01.mp4";
            m_video.PlayVideo();
        }
    }
}
