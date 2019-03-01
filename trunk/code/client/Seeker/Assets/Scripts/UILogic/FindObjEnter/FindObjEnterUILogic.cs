//#define TEST
using DG.Tweening;
using EngineCore;
using GOEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


namespace SeekerGame
{
    [UILogicHandler(UIDefine.UI_FIND_OBJ_ENTER_UI)]
    public class FindObjEnterUILogic : UILogicBase
    {
        private const int COIN_TYPE = 0;
        private const int CASH_TYPE = 1;
        private const int EXP_TYPE = 2;
        private const int VIT_TYPE = 3;

        private const int MAX_TIME = 10 * 60; //秒
        private const int MAX_VIT_COST = 40; //秒


        #region 组件



        //right
        private GameImage m_play_mode_img; //模式图标
        private GameTexture m_scene_tex; //场景图标
        private GameLabel m_scene_name_label; //场景名称
        private GameLabel m_cost_time_num_label; //消耗时间值
        private GameUIComponent m_cost_vit_root;
        TweenAlpha m_vit_tweenalpha;
        TweenScale m_vit_tweenscale;
        private GameLabel m_cost_VIT_num_label; //消耗体力值

        private GameScrollView m_outputs_scroll;
        private GameUIContainer m_outputs_grid;

        private GameUIContainer m_drop_grid; //掉落物品grid
        private GameScrollView m_drop_scroll;
        private GameButton m_action_btn; //开始游戏按钮
        private GameImage m_infinity_vit_icon;
        private GameUIEffect m_action_btn_effect;

        private GameImage m_difficult_icon;
        private GameLabel m_difficlut_step;
        private GameProgressBar m_exp_progress;
        private GameLabel m_exp_txt;
        private GameImage m_exp_reward_icon;

        private GameImage m_bg_img;
        // private TweenAlpha m_bg_alpha;
        private GameUIComponent m_tween_root;
        // private TweenScale m_show_tween_scale;
        //private TweenAlpha m_show_tween_alpha;
        private TweenPosition m_show_tween_pos;


        #endregion

        private bool m_is_action_btn_touched = false;

        private long m_scene_id;
        public long Scene_id
        {
            get { return m_scene_id; }
        }

        long m_special_scene_id = 0;

        ConfScene m_s_info = null;

        long m_scene_group_id;
        FindObjSceneData m_group_data;

        private long m_taskConfID = -1;


        int m_vit_cost_value;
        int m_time_cost_value;

        List<GroupToolTipsData> m_gifts;
        protected override void OnInit()
        {
            base.OnInit();

            //IsFullScreen = true;

            this.m_play_mode_img = this.Make<GameImage>("Panel_down:Image_sence:Image_mode");
            this.m_scene_name_label = this.Make<GameLabel>("Panel_down:Image_sence:Text");
            this.m_scene_tex = this.Make<GameTexture>("Panel_down:Image_sence:Image:RawImage");

            this.m_cost_time_num_label = m_play_mode_img.Make<GameLabel>("Text");

            this.m_cost_vit_root = this.Make<GameUIComponent>("Panel_down:Vit_Root");
            m_vit_tweenalpha = m_cost_vit_root.GetComponent<TweenAlpha>();
            m_vit_tweenscale = m_cost_vit_root.GetComponent<TweenScale>();
            m_vit_tweenscale.AddTweenCompletedCallback(OnVitTweenFinished);
            this.m_cost_VIT_num_label = m_cost_vit_root.Make<GameLabel>("Text2");

            m_outputs_scroll = this.Make<GameScrollView>("Panel_down:Panel_reward:ScrollView");
            m_outputs_grid = this.Make<GameUIContainer>("Panel_down:Panel_reward:ScrollView:Viewport");
            m_drop_scroll = this.Make<GameScrollView>("Panel_down:Panel_drop:ScrollView");
            this.m_drop_grid = this.Make<GameUIContainer>("Panel_down:Panel_drop:ScrollView:Viewport");

            this.m_action_btn = this.Make<GameButton>("Panel_down:Button_action");
            this.m_infinity_vit_icon = m_action_btn.Make<GameImage>("Image_free");
            m_action_btn_effect = m_action_btn.Make<GameUIEffect>("UI_tongyong_anniu");
            m_action_btn_effect.EffectPrefabName = "UI_tongyong_anniu.prefab";

            m_difficult_icon = Make<GameImage>("Panel_down:Panel_level:Image");
            m_difficlut_step = this.Make<GameLabel>("Panel_down:Panel_level:Text");
            m_exp_progress = Make<GameProgressBar>("Panel_down:Panel_level:Slider");
            m_exp_txt = this.Make<GameLabel>("Panel_down:Panel_level:Text (1)");
            m_exp_reward_icon = Make<GameImage>("Panel_down:Panel_level:Image_gift");

            this.SetCloseBtnID("Panel_down:Button_close");

            m_bg_img = Make<GameImage>("RawImage");
            //m_bg_alpha = m_bg_img.GetComponent<TweenAlpha>();
            m_tween_root = Make<GameUIComponent>("Panel_down");
            //m_show_tween_scale = m_tween_root.GetComponent<TweenScale>();
            //m_show_tween_scale.AddTweenCompletedCallback(ShowTweenScaleFinished);
            //var show_tween_poses = m_tween_root.GetComponents<TweenPosition>().Where((i) => UITweenerBase.TweenTriggerType.Manual == i.m_triggerType);
            //m_show_tween_pos = show_tween_poses.First();

            //var show_tween_alphas = m_tween_root.GetComponents<TweenAlpha>().Where((i) => UITweenerBase.TweenTriggerType.OnShow == i.m_triggerType);
            //m_show_tween_alpha = show_tween_alphas.First();

            var show_tween_poses = m_tween_root.GetComponents<TweenPosition>().Where((i) => UITweenerBase.TweenTriggerType.OnShow == i.m_triggerType);
            m_show_tween_pos = show_tween_poses.First();
            m_show_tween_pos.AddTweenCompletedCallback(ShowTweenPosFinished);
        }
        public override FrameDisplayMode UIFrameDisplayMode
        {
            get
            {
                return FrameDisplayMode.WINDOWED;
            }
        }
        void ShowTweenPosFinished()
        {
            m_drop_grid.Widget.offsetMax = new Vector2(m_drop_grid.Widget.offsetMax.x, -400.0f);
            m_drop_grid.Visible = true;
            m_drop_scroll.scrollView.elasticity = 0.3f;

            m_outputs_grid.Widget.offsetMax = new Vector2(m_outputs_grid.Widget.offsetMax.x, -400.0f);
            m_outputs_grid.Visible = true;
            m_outputs_scroll.scrollView.elasticity = 0.3f;

            TimeModule.Instance.SetTimeout(() => { m_drop_scroll.scrollView.elasticity = 0.1f; m_outputs_scroll.scrollView.elasticity = 0.1f; }, 2.0f);
        }

        void ShowTweenScaleFinished()
        {
            m_action_btn_effect.Visible = true;
        }

        void BgShowAlpha(bool tween)
        {
            if (tween)
            {
                //m_bg_alpha.From = 0.1f;
                //m_bg_alpha.To = 1.0f;
                //m_bg_alpha.ResetAndPlay();
            }
            else
            {
                //m_bg_img.Color = new Color(m_bg_img.Color.r, m_bg_img.Color.g, m_bg_img.Color.b, 1.0f);
            }
        }

        void PanelDelayShowAlpha(float delay_)
        {
            //m_show_tween_alpha.Delay = delay_;
        }

        void PanelDelayShowScale(float delay_)
        {
            //m_show_tween_scale.Delay = delay_;
        }

        void PanelTurnToOne(bool is_slow_)
        {
            //if (is_slow_)
            //{
            //    m_action_btn_effect.Visible = false;
            //    m_show_tween_scale.ResetAndPlay();
            //}
            //else
            //    m_show_tween_scale.gameObject.transform.localScale = Vector3.one;
        }

        void PanelTurnToZero(bool is_slow_)
        {
            //if (is_slow_)
            //    m_show_tween_scale.ResetAndPlay(false);
            //else
            //    m_show_tween_scale.gameObject.transform.localScale = new Vector3(0, 1, 1);
        }

        void PanelGoDown(bool is_slow_)
        {
            //if (is_slow_)
            //{
            //    m_show_tween_pos.Duration = 0.4f;
            //    m_show_tween_pos.ResetAndPlay();
            //}
            //else
            //{
            //    m_show_tween_pos.Duration = 0.0f;
            //    m_show_tween_pos.ResetAndPlay();
            //}
        }

        void PanelGoUp(bool is_slow_)
        {
            //if (is_slow_)
            //{
            //    m_show_tween_pos.Duration = 0.4f;
            //    m_show_tween_pos.ResetAndPlay(false);
            //}
            //else
            //{
            //    m_show_tween_pos.Duration = 0.0f;
            //    m_show_tween_pos.ResetAndPlay(false);
            //}
        }

        public override void OnShow(object param)
        {
            base.OnShow(param);
            GameEvents.UIEvents.UI_GameEntry_Event.OnMaskBGVisible.SafeInvoke(true);
            GameEvents.UIEvents.UI_GameEntry_Event.OnInfiniteVit += OnInfiniteVit;
            MessageHandler.RegisterMessageHandler(MessageDefine.SCSceneEnterResponse, OnScResponse);
            this.m_action_btn.AddClickCallBack(OnBtnStartGameClick);
            this.m_exp_reward_icon.AddPressDownCallBack(OnSceneGroupGiftPressDown);
            this.m_exp_reward_icon.AddPressUpCallBack(OnSceneGroupGiftPressUp);

            EnterSceneData enterSceneData = null;
            m_scene_id = -1;
            m_scene_group_id = -1;
            m_enter_msg = null;
            if (null != param)
            {
                enterSceneData = param as EnterSceneData;
                //this.m_scene_id = (int)enterSceneData.SceneID;
                this.m_taskConfID = enterSceneData.taskConfID;
            }

            if (-1 == m_taskConfID)
            {
                //不是从任务展示界面来的，不用翻转
                BgShowAlpha(false);
                PanelDelayShowAlpha(0.0f);
                PanelDelayShowScale(0.0f);
                PanelTurnToOne(false);
                PanelGoDown(true);
            }
            else
            {
                //需要翻转
                BgShowAlpha(true);
                PanelDelayShowAlpha(0.2f);
                PanelDelayShowScale(0.5f);
                PanelGoDown(false);
                PanelTurnToZero(false);
                PanelTurnToOne(true);

            }

            ConfTask task = null;

            if (this.m_taskConfID < 0)
            {
                //build top 进入
                this.m_scene_group_id = FindObjSceneDataManager.ConvertSceneIdToSceneGroupId(enterSceneData.SceneID);
                m_group_data = new FindObjSceneData(this.m_scene_group_id, 1, 0);
                BuildTopCreateSceneByDifficult(this.m_scene_group_id);
            }
            else
            {
                task = ConfTask.Get(m_taskConfID);

                long tsk_scene_id = null != task ? task.conditionSceneId : enterSceneData.SceneID;
                m_special_scene_id = 0;
                //初始化一个默认的group data;
                m_group_data = new FindObjSceneData(FindObjSceneDataManager.ConvertSceneIdToSceneGroupId(tsk_scene_id), 1, 0);

                this.m_scene_group_id = FindObjSceneDataManager.ConvertSceneIdToSceneGroupId(tsk_scene_id);
                var temp_data = FindObjSceneDataManager.Instance.GetDataBySceneGroupID(m_scene_group_id);
                if (null != temp_data)
                    m_group_data = temp_data;

                if (tsk_scene_id > 99999)
                {
                    //配置的场景id
                    if (1 == ConfScene.Get(tsk_scene_id).keyExhibit)
                    {
                        //关键证据关卡
                        CreateSceneBySpecial(tsk_scene_id);
                    }
                    else
                    {
                        //非关键关卡
                        //读取难度表
                        TaskCreateSceneByDifficult(this.m_taskConfID);
                    }
                }
                else
                {
                    //配置的场景组id,一定是非关键线索关卡
                    TaskCreateSceneByDifficult(this.m_taskConfID);
                }
            }

            m_play_mode_img.Sprite = this.GetModeIconName(m_scene_id);
            m_scene_tex.TextureName = m_s_info.thumbnail;
            m_scene_name_label.Text = LocalizeModule.Instance.GetString(m_s_info.name);

            this.ShowOutPut(m_s_info.outputExp, m_s_info.outputMoney, m_s_info.outputCash, m_s_info.outputVit);
            List<long> ids = CommonHelper.GetDropOuts(m_s_info.dropId);

            var props = from id in ids
                        select new DropWrapper()
                        {
                            DropType = ENUM_DROP_TYPE.PROP,
                            ItemID = id,
                            ItemNum = 0,
                            Icon = ConfProp.Get(id).icon,
                        };

            List<DropWrapper> totalToShow = new List<DropWrapper>();

            if (null != task && task.conditionExhibits.Length > 0)
            {
                var exhibits = from id in task.conditionExhibits
                               select new DropWrapper()
                               {
                                   DropType = ENUM_DROP_TYPE.EXHABIT,
                                   ItemID = id,
                                   ItemNum = 0,
                                   Icon = Confexhibit.Get(id).iconName,
                               };

                totalToShow.AddRange(exhibits);
            }
            //DropWrapper te = new DropWrapper()
            //{
            //    DropType = ENUM_DROP_TYPE.EXHABIT,
            //    ItemID = 10972,
            //    ItemNum = 0,
            //    Icon = Confexhibit.Get(10972).iconName,
            //};
            //totalToShow.Add(te);
            totalToShow.AddRange(props);

            m_drop_grid.EnsureSize<DropItemIconEffect>(totalToShow.Count);
            for (int i = 0; i < m_drop_grid.ChildCount; ++i)
            {
                var show = totalToShow[i];

                DropItemIconEffect di = m_drop_grid.GetChild<DropItemIconEffect>(i);
                m_drop_grid.GetChild<DropItemIconEffect>(i).InitSprite(show.Icon, show.ItemNum, show.ItemID, ENUM_DROP_TYPE.EXHABIT == show.DropType);
                if (ENUM_DROP_TYPE.PROP == show.DropType)
                {
                    m_drop_grid.GetChild<DropItemIconEffect>(i).EnableTips(true);
                    m_drop_grid.GetChild<DropItemIconEffect>(i).InitToolTipOffset(new Vector2(-280.0f, 0.0f));
                }
                else
                {
                    m_drop_grid.GetChild<DropItemIconEffect>(i).EnableTips(false);
                }
                m_drop_grid.GetChild<DropItemIconEffect>(i).Visible = true;
            }
            m_drop_grid.Visible = false;

            long scene_difficult_id = FindObjSceneDataManager.GetSceneDifficultID(m_scene_group_id, m_group_data.m_lvl);
            ConfSceneDifficulty difficult_data = ConfSceneDifficulty.Get(scene_difficult_id);

            m_gifts = new List<GroupToolTipsData>();
            for (int i = 0; i < difficult_data.rewards.Length; ++i)
            {
                GroupToolTipsData reward = new GroupToolTipsData()
                {
                    ItemID = difficult_data.rewards[i],
                    CurCount = difficult_data.rewardNums[i],
                };

                m_gifts.Add(reward);
            }

            m_difficult_icon.Sprite = ConfPoliceRankIcon.Get(m_group_data.m_lvl).icon;
            m_difficlut_step.Text = LocalizeModule.Instance.GetString(difficult_data.name);
            m_exp_progress.Value = (float)m_group_data.m_exp / (float)m_group_data.m_full_exp;
            m_exp_txt.Text = $"{m_group_data.m_exp}/{m_group_data.m_full_exp}";
            Debug.Log($"关卡进入 关卡组id{m_group_data.m_scene_group_id}");
            m_cost_time_num_label.Text = CommonTools.SecondToStringMMSS(this.m_time_cost_value);
            SetVitLabel();

            m_is_action_btn_touched = false;
            this.m_action_btn.Enable = true;


            Dictionary<UBSParamKeyName, object> _params = new Dictionary<UBSParamKeyName, object>()
                            {
                                { UBSParamKeyName.Success, 1},
                                { UBSParamKeyName.SceneID, this.m_scene_id},
                            };
            UserBehaviorStatisticsModules.Instance.LogEvent(UBSEventKeyName.game_start, null, _params);


        }

        void CreateSceneBySpecial(long tsk_scene_id_)
        {
            this.m_scene_id = tsk_scene_id_;
            m_s_info = ConfScene.Get(m_scene_id);
            int cur_difficult = m_group_data.m_lvl;
            int play_mode = m_s_info.playMode;
            //场景特殊表
            ConfSceneSpecial special_scene = ConfSceneSpecial.array.Find((item) => item.playMode == play_mode && item.difficulty == cur_difficult);
            m_special_scene_id = special_scene.id;
            m_time_cost_value = special_scene.secondGain;
            m_vit_cost_value = special_scene.vitConsume;
        }

        void TaskCreateSceneByDifficult(long tsk_id_)
        {
            long scene_id = FindObjSceneDataManager.LoadSceneIDForTask(tsk_id_);

            if (scene_id < 0)
            {
                long scene_difficult_id = FindObjSceneDataManager.GetSceneDifficultID(m_scene_group_id, m_group_data.m_lvl);
                long[] sceneid4random = ConfSceneDifficulty.Get(scene_difficult_id).sceneIds;
                int[] weights4random = ConfSceneDifficulty.Get(scene_difficult_id).sceneWeights;

                FindObjSceneData my_difficult_data = FindObjSceneDataManager.Instance.GetDataBySceneGroupID(m_scene_group_id);
                int? my_difficult_lvl = my_difficult_data?.m_lvl;
                int? my_difficult_exp = my_difficult_data?.m_exp;

                if ((1 == my_difficult_lvl || 0 == my_difficult_lvl) && 0 == my_difficult_exp)
                {
                    //首次使用普通玩法
                    scene_id = sceneid4random[0];
                }
                else
                {
                    //二次随机
                    List<RateScene> scenes4rdm = new List<RateScene>();
                    for (int i = 0; i < sceneid4random.Length; ++i)
                    {
                        RateScene rs = new RateScene()
                        {
                            SceneID = sceneid4random[i],
                            Rate = weights4random[i],
                        };
                        scenes4rdm.Add(rs);
                    }

                    int idx = randomGetIdex(scenes4rdm);

                    if (idx >= 0)
                    {
                        RateScene selected_scene = scenes4rdm[idx];
                        scene_id = selected_scene.SceneID;

                        FindObjSceneDataManager.SaveSceneIDForTask(tsk_id_, scene_id);

                        if (1 == ConfTask.Get(tsk_id_).type)
                        {
                            //主线任务，影响bulid top的场景id
                            FindObjSceneDataManager.SaveSceneIDForBuildTop(FindObjSceneDataManager.ConvertSceneIdToSceneGroupId(scene_id), scene_id);
                        }
                    }
                }
            }

            if (scene_id > 0)
            {
                this.m_scene_id = scene_id;
                m_s_info = ConfScene.Get(m_scene_id);
                m_time_cost_value = m_s_info.secondGain;
                m_vit_cost_value = m_s_info.vitConsume;

            }
            else
            {
                m_s_info = null;
                m_time_cost_value = 0;
                m_vit_cost_value = 0;
            }
        }

        void BuildTopCreateSceneByDifficult(long build_top_scene_group_id_)
        {
            long scene_id = FindObjSceneDataManager.LoadSceneIDForBuildTop(build_top_scene_group_id_);

            var temp_group_data = FindObjSceneDataManager.Instance.GetDataBySceneGroupID(build_top_scene_group_id_);
            if (null != temp_group_data)
                m_group_data = temp_group_data;

            if (scene_id < 0)
            {

                long scene_difficult_id = FindObjSceneDataManager.GetSceneDifficultID(build_top_scene_group_id_, m_group_data.m_lvl);
                long[] sceneid4random = ConfSceneDifficulty.Get(scene_difficult_id).sceneIds;
                int[] weights4random = ConfSceneDifficulty.Get(scene_difficult_id).sceneWeights;

                List<RateScene> scenes4rdm = new List<RateScene>();
                for (int i = 0; i < sceneid4random.Length; ++i)
                {
                    RateScene rs = new RateScene()
                    {
                        SceneID = sceneid4random[i],
                        Rate = weights4random[i],
                    };
                    scenes4rdm.Add(rs);
                }

                int idx = randomGetIdex(scenes4rdm);

                if (idx >= 0)
                {
                    RateScene selected_scene = scenes4rdm[idx];
                    scene_id = selected_scene.SceneID;
                    FindObjSceneDataManager.SaveSceneIDForBuildTop(build_top_scene_group_id_, scene_id);
                }

            }

            if (scene_id > 0)
            {
                this.m_scene_id = scene_id;
                m_s_info = ConfScene.Get(m_scene_id);
                m_time_cost_value = m_s_info.secondGain;
                m_vit_cost_value = m_s_info.vitConsume;

            }
            else
            {
                m_s_info = null;
                m_time_cost_value = 0;
                m_vit_cost_value = 0;
            }
        }

        public override void OnHide()
        {
            base.OnHide();
            GameEvents.UIEvents.UI_GameEntry_Event.OnMaskBGVisible.SafeInvoke(false);
            MessageHandler.UnRegisterMessageHandler(MessageDefine.SCSceneEnterResponse, OnScResponse);
            GameEvents.UIEvents.UI_GameEntry_Event.OnInfiniteVit -= OnInfiniteVit;
            this.m_action_btn.RemoveClickCallBack(OnBtnStartGameClick);
            this.m_exp_reward_icon.RemovePressDownCallBack(OnSceneGroupGiftPressDown);
            this.m_exp_reward_icon.RemovePressUpCallBack(OnSceneGroupGiftPressUp);

            EngineCoreEvents.UIEvent.HideUIEvent.SafeInvoke(UIDefine.UI_GROUP_TOOL_TIPS);
            EngineCoreEvents.UIEvent.HideUIEvent.SafeInvoke(UIDefine.UI_TOOL_TIPS);

            if (null != m_enter_msg)
                GameEvents.SceneEvents.OnEnterSeekScene.SafeInvoke(m_enter_msg);
        }

        SCSceneEnterResponse m_enter_msg;

        private void OnScResponse(object s)
        {

            if (s is SCSceneEnterResponse)
            {
                SCSceneEnterResponse msg = s as SCSceneEnterResponse;
                if (!MsgStatusCodeUtil.OnError(msg.ResponseStatus))
                {
                    Dictionary<UBSParamKeyName, object> _params = new Dictionary<UBSParamKeyName, object>()
                        {
                            { UBSParamKeyName.Success, 1},
                            {UBSParamKeyName.SceneID, this.Scene_id }
                        };
                    UserBehaviorStatisticsModules.Instance.LogEvent(UBSEventKeyName.game_begin, null, _params);

                    GameEvents.PlayerEvents.RequestLatestPlayerInfo.SafeInvoke();
                    EngineCoreEvents.UIEvent.HideUIEvent.SafeInvoke(UIDefine.UI_CHAPTER);

                    m_enter_msg = msg;

                    this.CloseFrame();
                }
                else
                {
                    if (MsgStatusCodeUtil.VIT_OUT == msg.ResponseStatus.Code)
                    {
                        Dictionary<UBSParamKeyName, object> _params = new Dictionary<UBSParamKeyName, object>()
                            {
                                { UBSParamKeyName.Success, 0},
                                { UBSParamKeyName.Description, UBSDescription.NOT_ENOUGH_VIT},
                                {UBSParamKeyName.SceneID, this.Scene_id }
                            };
                        UserBehaviorStatisticsModules.Instance.LogEvent(UBSEventKeyName.game_begin, null, _params);
                    }
                    else
                    {
                        Dictionary<UBSParamKeyName, object> _params = new Dictionary<UBSParamKeyName, object>()
                            {
                                { UBSParamKeyName.Success, 0},
                                { UBSParamKeyName.Description, msg.ResponseStatus.Code},
                                {UBSParamKeyName.SceneID, this.Scene_id }
                            };
                        UserBehaviorStatisticsModules.Instance.LogEvent(UBSEventKeyName.game_begin, null, _params);
                    }

                    this.m_action_btn.Enable = true;
                    m_is_action_btn_touched = false;
                    this.m_action_btn.Enable = true;
                }
            }

        }




        /// <summary>
        /// 开始游戏
        /// </summary>
        /// <param name="btnStartGame"></param>
        private void OnBtnStartGameClick(GameObject btnStartGame)
        {

            EngineCoreEvents.AudioEvents.PlayAudio.SafeInvoke(Audio.AudioType.UISound, GameCustomAudioKey.game_star.ToString());

            if (m_is_action_btn_touched)
                return;

            m_is_action_btn_touched = true;

            m_vit_tweenalpha.ResetAndPlay();
            m_vit_tweenscale.ResetAndPlay();

            StartGame();
        }

        private void OnSceneGroupGiftPressDown(GameObject btnStartGame)
        {
            //显示礼物列表
            GroupToolTipsDatas data = new GroupToolTipsDatas()
            {
                Datas = m_gifts,
                ScreenPos = RectTransformUtility.WorldToScreenPoint(FrameMgr.Instance.UICamera, m_exp_reward_icon.Position) - new Vector2(0.0f, -60.0f * m_gifts.Count - 30.0f),
            };


            FrameMgr.OpenUIParams ui_data = new FrameMgr.OpenUIParams(UIDefine.UI_GROUP_TOOL_TIPS)
            {
                Param = data,
            };

            EngineCoreEvents.UIEvent.ShowUIEventWithParam.SafeInvoke(ui_data);
        }


        private void OnSceneGroupGiftPressUp(GameObject btnStartGame)
        {
            //隐藏礼物列表
            EngineCoreEvents.UIEvent.HideUIEvent.SafeInvoke(UIDefine.UI_GROUP_TOOL_TIPS);
        }


        void OnVitTweenFinished()
        {
            SendStarGame();
        }

        private void SendStarGame()
        {

            CSSceneEnterRequest enterSceneRequest = new CSSceneEnterRequest();
            enterSceneRequest.SceneId = this.m_scene_id;


            GameEvents.NetWorkEvents.SendMsg.SafeInvoke(enterSceneRequest);

            //刷新体力
            GameEvents.PlayerEvents.RequestLatestPlayerInfo.SafeInvoke();
        }

        void StartGame()
        {
            this.m_action_btn.Enable = false;

            SceneModule.Instance.TryCreateGameScene(this.m_scene_id, this.m_taskConfID);
        }



        private void GetPlayMode(int scene_play_mode_, out int wan, out int qian, out int ban)
        {
            wan = scene_play_mode_ / 10000;  //0普通(不组合)，1迷雾，2黑天
            qian = scene_play_mode_ / 1000 % 10;  //1普通，2反词，3剪影
            ban = scene_play_mode_ / 100 % 10;  //0普通， 1多物
        }


        private string GetModeIconName(long scene_id)
        {

            string prefix = "icon_playmode";
            if (scene_id < 20000000)
            {
                int play_mode = ConfScene.Get(scene_id).playMode;

                int wan, qian, bai;

                this.GetPlayMode(play_mode, out wan, out qian, out bai);


                string wan_str = string.Empty;

                if (0 == wan)
                {
                    wan_str = "pt";
                }
                else if (1 == wan)
                {
                    wan_str = "miwu";
                }
                else if (2 == wan)
                {
                    wan_str = "hei";
                }


                string qian_str = string.Empty;

                if (1 == qian)
                {
                    qian_str = "pt";
                }
                if (2 == qian)
                {
                    qian_str = "fan";
                }
                else if (3 == qian)
                {
                    qian_str = "jian";
                }

                string bai_str = string.Empty;

                if (0 == bai)
                {
                    bai_str = "pt";
                }
                if (1 == bai)
                {
                    bai_str = "duo";
                }


                return string.Format("{0}_{1}_{2}_{3}.png", prefix, wan_str, qian_str, bai_str);

            }
            else
            {
                prefix = "icon_playmode_jigsaw";
                return LocalizeModule.Instance.GetString("icon_playmode_jigsaw");
            }



        }



        private void ShowOutPut(int exp_, int coin_, int cash_, int vit_)
        {
            CommonHelper.ShowOutput(m_outputs_grid, exp_, coin_, cash_, vit_);
            m_outputs_grid.Visible = false;
        }

        private void OnInfiniteVit(float time)
        {
            SetVitLabel();
        }

        private void SetVitLabel()
        {
            m_vit_tweenalpha.ResetAndPlay(false);
            this.m_cost_vit_root.gameObject.transform.localScale = Vector3.one;

            if (VitManager.Instance.IsInfiniteVit())
            {
                this.m_infinity_vit_icon.Visible = true;
                this.m_cost_vit_root.Visible = false;
            }
            else
            {
                this.m_infinity_vit_icon.Visible = false;
                this.m_cost_vit_root.Visible = true;
                m_cost_VIT_num_label.Text = Math.Abs(m_vit_cost_value).ToString();
            }
        }





        class RateScene : RateInterface
        {
            public long SceneID { get; set; }
            public int Rate { get; set; }

            public int getRate()
            {
                return Rate;
            }
        }


        interface RateInterface
        {
            int getRate();
        }
        static int randomGetIdex<T>(List<T> datas) where T : RateInterface
        {
            int total = 0;
            datas.ForEach((item) => total += item.getRate());

            int cur_rate = 0;
            int randomValue = UnityEngine.Random.Range(0, total);

            for (int i = 0; i < datas.Count; ++i)
            {
                cur_rate += datas[i].getRate();

                if (randomValue < cur_rate)
                {
                    return i;
                }
            }

            return -1;
        }

    }
}