using DG.Tweening;
using EngineCore;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SeekerGame
{
    [UILogicHandler(UIDefine.UI_ACCEPT_TASK)]
    public class AcceptTaskUILogic : UILogicBase
    {
        readonly Vector2 reward_up_pos = new Vector2(0, 20);
        readonly Vector2 reward_down_pos = Vector2.zero;

        GameLabel m_name_txt;
        GameLabel m_desc_txt;
        GameLabel m_tips_txt;
        GameUIComponent m_tips_btn;
        GameUIContainer m_aim_grid; //AimItemView
        GameUIComponent m_reward_root;
        GameUIContainer m_reward_grid; //DropItemIcon
        GameLabel m_title_txt;
        GameUIComponent m_titleGet = null;
        GameButton m_btn;
        GameButton m_close_btn;

        GameLabel m_btnLable = null;
        //GameImage m_bg_img;
        //TweenAlpha m_bg_tween_alpha;
        GameUIComponent m_tween_root;
        //TweenPosition m_hide_tween_pos;
        //TweenScale m_hide_tween_scale;
        //TweenAlpha m_hide_tween_alpha;
        private NormalTask m_taskInfo = null;
        private Action<long> m_btn_act = null;

        private GameUIComponent m_completeTaskTipsLabel = null;
        protected override void OnInit()
        {
            base.OnInit();

            m_name_txt = Make<GameLabel>("Panel_down:Image_sence:Text_Name");
            m_desc_txt = Make<GameLabel>("Panel_down:Image_sence:Text_Desc");
            m_tips_txt = Make<GameLabel>("Panel_down:Image_sence:Text_Tips");
            m_completeTaskTipsLabel = Make<GameUIComponent>("Panel_down:Image_sence:Text_missioncomplete");
            m_tips_btn = Make<GameUIComponent>("Panel_down:Image_sence:Btn_Tips");
            m_reward_root = Make<GameUIComponent>("Panel_down:Image_sence:Image (1):ScrollView");
            m_aim_grid = Make<GameUIContainer>("Panel_down:Image_sence:Image (2):ScrollView (1):Viewport");//IconNameNumItemView
            m_reward_grid = Make<GameUIContainer>("Panel_down:Image_sence:Image (1):ScrollView:Viewport"); //DropItemIcon
            m_title_txt = Make<GameLabel>("Panel_down:Image_sence:Image (1):Text");
            m_titleGet = m_title_txt.Make<GameUIComponent>("Image");
            m_btn = Make<GameButton>("Panel_down:Button_action");
            m_close_btn = Make<GameButton>("Button_close");
            this.m_btnLable = this.m_btn.Make<GameLabel>("Text");
            //m_bg_img = Make<GameImage>("RawImage");
            //m_bg_tween_alpha = m_bg_img.GetComponent<TweenAlpha>();
            m_tween_root = Make<GameUIComponent>("Panel_down");
            //var tween_poses = m_tween_root.GetComponents<TweenPosition>().Where((i) => UITweenerBase.TweenTriggerType.Manual == i.m_triggerType);
            //m_hide_tween_pos = tween_poses.First();
            //m_hide_tween_scale = m_tween_root.GetComponent<TweenScale>();
            //m_hide_tween_alpha = m_tween_root.GetComponents<TweenAlpha>().Where((i) => UITweenerBase.TweenTriggerType.OnHide == i.m_triggerType).First();
            //m_hide_tween_scale.AddTweenCompletedCallback(ScaleTweenFinished);
            m_tween_root.gameObject.transform.localScale = Vector3.one;
        }

        void BgHideAlpha(bool transparent)
        {
            if (transparent)
            {
                //m_bg_tween_alpha.From = 1.0f;
                //m_bg_tween_alpha.To = 0.1f;
                //m_bg_tween_alpha.ResetAndPlay();
            }
            else
            {
                //m_bg_img.Color = new Color(m_bg_img.Color.r, m_bg_img.Color.g, m_bg_img.Color.b, 1.0f);
            }
        }

        void PanelHideAlphaTo(float t_)
        {
           // m_hide_tween_alpha.To = t_;
        }

        void PanelTurnToOne(bool is_slow_)
        {
            //if (is_slow_)
            //    m_hide_tween_scale.ResetAndPlay(false);
            //else
            //    m_tween_root.gameObject.transform.localScale = Vector3.one;
        }

        void PanelTurnToZero(bool is_slow_)
        {
            //if (is_slow_)
            //    m_hide_tween_scale.ResetAndPlay();
            //else
            //    m_tween_root.gameObject.transform.localScale = new Vector3(0, 1, 1);
        }

        public override FrameDisplayMode UIFrameDisplayMode
        {
            get
            {
                return FrameDisplayMode.WINDOWED;
            }
        }

        void PanelGoUp(bool is_slow_)
        {
            if (is_slow_)
            {
                //m_hide_tween_pos.Duration = 0.4f;
                //m_hide_tween_pos.ResetAndPlay();
            }
            else
            {
                //m_hide_tween_pos.Duration = 0.0f;
                //m_hide_tween_pos.ResetAndPlay();
            }
        }

        void PanelGoDown(bool is_slow_)
        {
            if (is_slow_)
            {
                //m_hide_tween_pos.Duration = 0.4f;
                //m_hide_tween_pos.ResetAndPlay(false);
            }
            else
            {
                //m_hide_tween_pos.Duration = 0.0f;
                //m_hide_tween_pos.ResetAndPlay(false);
            }
        }

        bool m_isAcceptTask = true;
        public override void OnShow(object param)
        {
            base.OnShow(param);

            GameEvents.UIEvents.UI_GameEntry_Event.OnMaskBGVisible.SafeInvoke(true);
            //GameEvents.UIEvents.UI_Common_Event.OnCommonUIVisible.SafeInvoke(false);
            BgHideAlpha(false);
            PanelTurnToOne(false);

            m_btn.AddClickCallBack(OnClicked);
            m_tips_btn.AddClickCallBack(OnClicked);
            m_close_btn.AddClickCallBack(OnCloseClicked);
            if (null != param)
            {
                var input_param = param as AcceptTaskParam;
                m_taskInfo = input_param.m_taskInfo;
                m_btn_act = input_param.m_close_act;
                m_isAcceptTask = input_param.isAcceptTask;
            }
            this.m_completeTaskTipsLabel.Visible = !m_isAcceptTask;
            this.m_tips_txt.Visible = m_isAcceptTask;
            if (m_isAcceptTask)
                this.m_btnLable.Text = "Action !";
            else
                this.m_btnLable.Text = "Complete !";
            ConfTask confTask = ConfTask.Get(this.m_taskInfo.TaskConfID);
            this.m_desc_txt.Text = LocalizeModule.Instance.GetString(confTask.descs);
            this.m_name_txt.Text = LocalizeModule.Instance.GetString(confTask.name);
            this.m_tips_txt.Text = LocalizeModule.Instance.GetString("activity_drop_taskdec_1");


            List<RewardWrapper> task_rewards = new List<RewardWrapper>();
            if (m_isAcceptTask)
                task_rewards = GetAcceptTaskDropItem(confTask);
            else
                task_rewards = GetCompleteTaskDropItem();

            if (task_rewards.Count > 0)
            {
                m_reward_grid.EnsureSize<DropItemIcon>(task_rewards.Count);
                for (int i = 0; i < m_reward_grid.ChildCount; ++i)
                {
                    var view = m_reward_grid.GetChild<DropItemIcon>(i);
                    view.InitSprite(task_rewards[i].m_icon, task_rewards[i].m_num, task_rewards[i].m_id);
                    view.Visible = true;
                }
            }
            else
            {
                m_reward_grid.Clear();
            }



            List<iconAndName> conditions = new List<iconAndName>();

            if (m_taskInfo.CompleteConditionList.Count > 0)
            {
                foreach (var complete_condition in m_taskInfo.CompleteConditionList)
                {
                    if (complete_condition.GetType() == typeof(TaskCompleteItems))
                    {
                        //收集物品
                        TaskCompleteItems collectDataInfo = complete_condition as TaskCompleteItems;

                        List<ItemWrapper> collectItemList = collectDataInfo.TaskCompleteData;

                        string collect_icon, collect_name;
                        int collect_type;

                        foreach (var collect in collectItemList)
                        {
                            GetIconAndName(collect.ItemID, out collect_icon, out collect_name, out collect_type);

                            conditions.Add(new iconAndName()
                            {
                                m_icon = collect_icon,
                                m_name = collect_name,
                                m_type = collect_type,
                            });
                        }

                    }
                    else if (complete_condition.GetType() == typeof(TaskCompleteByScan))
                    {
                        //尸检
                        TaskCompleteByScan collectDataInfo = complete_condition as TaskCompleteByScan;
                        long scan_id = (long)collectDataInfo.TaskCompleteData;

                        var all_types_clues = ScanDataManager.Instance.Examin_clue_datas(scan_id);

                        var all_clues = new HashSet<long>();

                        foreach (var kvp in all_types_clues)
                        {
                            all_clues.UnionWith(kvp.Value);
                        }

                        var scan_clues = from i in all_clues
                                         select new iconAndName()
                                         {
                                             m_icon = ConfFindClue.Get(i).icon,
                                             m_name = LocalizeModule.Instance.GetString(ConfFindClue.Get(i).name),
                                         };

                        foreach (var item in scan_clues)
                            conditions.Add(item);

                    }
                }

            }


            m_aim_grid.EnsureSize<AimItemView>(conditions.Count);

            for (int i = 0; i < m_aim_grid.ChildCount; ++i)
            {
                m_aim_grid.GetChild<AimItemView>(i).Refresh(conditions[i].m_icon, conditions[i].m_name);
                m_aim_grid.GetChild<AimItemView>(i).Visible = true;
            }

            if (m_isAcceptTask)
            {
                m_titleGet.Visible = false;
                ShowAcceptTaskTips();
                ShowTaskTitle(confTask.rewardTitleId);
            }

        }

        #region 接受任务
        private List<RewardWrapper> GetAcceptTaskDropItem(ConfTask confTask)
        {
            List<RewardWrapper> task_rewards = new List<RewardWrapper>();
            long[] rewardItemIds = confTask.rewardPropIds;
            int[] rewardItemNums = confTask.rewardPropNums;

            for (int i = 0; i < rewardItemIds.Length; ++i)
            {
                var reward = ConfProp.Get(rewardItemIds[i]);

                task_rewards.Add(new RewardWrapper()
                {
                    m_icon = reward.icon,
                    m_num = rewardItemNums[i],
                    m_id = rewardItemIds[i],
                }
                );
            }

            var output = TaskHelper.GetReward(confTask);

            if (output.Item1 > 0)
            {
                RewardWrapper rw = new RewardWrapper()
                {
                    m_id = 0,
                    m_icon = CommonHelper.GetOutputIconName(EUNM_BASE_REWARD.E_VIT),
                    m_num = output.Item1,
                };
                task_rewards.Add(rw);
            }

            if (output.Item2 > 0)
            {
                RewardWrapper rw = new RewardWrapper()
                {
                    m_id = 0,
                    m_icon = CommonHelper.GetOutputIconName(EUNM_BASE_REWARD.E_COIN),
                    m_num = output.Item2,
                };
                task_rewards.Add(rw);
            }

            if (output.Item3 > 0)
            {
                RewardWrapper rw = new RewardWrapper()
                {
                    m_id = 0,
                    m_icon = CommonHelper.GetOutputIconName(EUNM_BASE_REWARD.E_CASH),
                    m_num = output.Item3,
                };
                task_rewards.Add(rw);
            }

            if (output.Item4 > 0)
            {
                RewardWrapper rw = new RewardWrapper()
                {
                    m_id = 0,
                    m_icon = CommonHelper.GetOutputIconName(EUNM_BASE_REWARD.E_EXP),
                    m_num = output.Item4,
                };
                task_rewards.Add(rw);

            }
            return task_rewards;
        }

        private void ShowAcceptTaskTips()
        {
            var taskTotalCompleteMode = TaskCompleteMode.NONE;


            foreach (var cc in m_taskInfo.CompleteConditionList)
            {

                TaskCompleteAttribute taskCompleteAttribute = cc.GetType().GetCustomAttributes(typeof(TaskCompleteAttribute), true)[0] as TaskCompleteAttribute;
                taskTotalCompleteMode |= taskCompleteAttribute.CompleteMode;
            }

            if (TaskCompleteMode.CompletedBySceneID == (taskTotalCompleteMode & TaskCompleteMode.CompletedBySceneID))
            {
                //场景（寻物，拼图）
                TaskCompleteByScene collectDataInfo = m_taskInfo.CompleteConditionList[0] as TaskCompleteByScene;

                long scene_id = collectDataInfo.TaskCompleteData;


                if (CommonData.C_JIGSAW_SCENE_START_ID == scene_id / CommonData.C_SCENE_TYPE_ID)
                {
                    //拼图
                    m_tips_txt.Text = LocalizeModule.Instance.GetString("mission_UI_tips", CommonHelper.GetModeName(scene_id));
                }
                else if (CommonData.C_SEEK_SCENE_START_ID == scene_id / CommonData.C_SCENE_TYPE_ID)
                {
                    //寻物场景
                    string scene_name = LocalizeModule.Instance.GetString(ConfScene.Get(scene_id).name);
                    m_tips_txt.Text = LocalizeModule.Instance.GetString("mission_UI_tips1", scene_name, CommonHelper.GetModeName(scene_id));
                }
            }
            else if ((TaskCompleteMode.CompletedBySceneID | TaskCompleteMode.CompletedByItem) == (taskTotalCompleteMode & (TaskCompleteMode.CompletedBySceneID | TaskCompleteMode.CompletedByItem)))
            {
                //场景+寻物（必然是场景）
                foreach (var complete_condition in m_taskInfo.CompleteConditionList)
                {
                    if (complete_condition.GetType() == typeof(TaskCompleteByScene))
                    {
                        TaskCompleteByScene collectDataInfo = complete_condition as TaskCompleteByScene;

                        long scene_id = collectDataInfo.TaskCompleteData;

                        //寻物场景
                        string scene_name = LocalizeModule.Instance.GetString(ConfScene.Get(scene_id).name);
                        m_tips_txt.Text = LocalizeModule.Instance.GetString("mission_UI_tips1", scene_name, CommonHelper.GetModeName(scene_id));

                        break;
                    }
                }
            }
            else if (TaskCompleteMode.CompleteByScanID == (taskTotalCompleteMode & TaskCompleteMode.CompleteByScanID))
            {
                //尸检
                m_tips_txt.Text = LocalizeModule.Instance.GetString("mission_UI_tips", LocalizeModule.Instance.GetString("tips_autopsy"));

            }
            else if (TaskCompleteMode.CompleteByReasonID == (taskTotalCompleteMode & TaskCompleteMode.CompleteByReasonID))
            {
                //推理
                m_tips_txt.Text = LocalizeModule.Instance.GetString("mission_UI_tips", LocalizeModule.Instance.GetString("tips_detective"));
            }
            else if (TaskCompleteMode.CompleteByCombinePropID == (taskTotalCompleteMode & TaskCompleteMode.CompleteByCombinePropID))
            {
                //合成
                m_tips_txt.Text = LocalizeModule.Instance.GetString("mission_UI_tips", LocalizeModule.Instance.GetString("tips_combine"));
            }
            else
            {
                this.m_tips_txt.Text = string.Empty;
            }
        }

        private void ShowTaskTitle(long rewardTitleId)
        {
            if (rewardTitleId > 0)
            {
                m_title_txt.Visible = true;
                m_title_txt.Text = ConfTitle.Get(rewardTitleId).name;
                m_reward_root.Widget.anchoredPosition = reward_up_pos;
            }
            else
            {
                m_title_txt.Visible = false;
                m_reward_root.Widget.anchoredPosition = reward_down_pos;
            }
        }
        #endregion

        #region 完成任务
        int rewardCash = 0;
        int rewardCoin = 0;
        int rewardExp = 0;
        int rewardVit = 0;
        private List<RewardWrapper> GetCompleteTaskDropItem()
        {
            rewardCash = 0;
            rewardCoin = 0;
            rewardExp = 0;
            rewardVit = 0;
            m_title_txt.Visible = false;
            List<RewardWrapper> task_rewards = new List<RewardWrapper>();
            for (int i = 0; i < m_taskInfo.RewardList.Count; ++i)
            {
                TaskRewardBase taskReward = m_taskInfo.RewardList[i];
                TaskRewardMode taskRewardType = (taskReward.GetType().GetCustomAttributes(typeof(TaskRewardAttribute), true)[0] as TaskRewardAttribute).RewardMode;
                switch (taskRewardType)
                {
                    case TaskRewardMode.ITEM:
                        TaskRewardItem rewardItem = taskReward as TaskRewardItem;
                        if(rewardItem.RewardData != null)
                            task_rewards.Add(GetRewardWrapperByTaskRewardData(rewardItem.RewardData));
                        break;
                    case TaskRewardMode.TITLE:
                        TaskRewardTitle rewardTitle = taskReward as TaskRewardTitle;
                        m_titleGet.Visible = rewardTitle.RewardData > 0;
                        ShowTaskTitle(rewardTitle.RewardData);
                        break;
                }
            }
            return task_rewards;
        }

        private RewardWrapper GetRewardWrapperByTaskRewardData(RewardItemDataWrap reward)
        {
            RewardWrapper rw = new RewardWrapper();
            rw.m_num = reward.ItemNum;
            rw.m_id = 0;
            switch (reward.ItemType)
            {
                case RewardItemType.CASH:
                    rw.m_icon = CommonHelper.GetOutputIconName(EUNM_BASE_REWARD.E_CASH);
                    rewardCash += rw.m_num;
                    break;
                case RewardItemType.COIN:
                    rw.m_icon = CommonHelper.GetOutputIconName(EUNM_BASE_REWARD.E_COIN);
                    rewardCoin += rw.m_num;
                    break;
                case RewardItemType.EXP:
                    rw.m_icon = CommonHelper.GetOutputIconName(EUNM_BASE_REWARD.E_EXP);
                    rewardExp += rw.m_num;
                    break;
                case RewardItemType.VIT:
                    rw.m_icon = CommonHelper.GetOutputIconName(EUNM_BASE_REWARD.E_VIT);
                    rewardVit += rw.m_num;
                    break;
                case RewardItemType.ITEM:
                    rw.m_id = reward.ItemID;
                    rw.m_icon = ConfProp.Get(reward.ItemID).icon;
                    break;
            }
            return rw;
        }

        #endregion
        void ShowReward(ConfTask confTask)
        {

        }

        void GetIconAndName(long ItemID, out string icon, out string itemName, out int item_type)
        {
            icon = string.Empty;
            itemName = string.Empty;
            item_type = 0;
            if (ItemID >= 9999)
            {
                Confexhibit exhibitConfig = Confexhibit.Get(ItemID);
                icon = exhibitConfig.iconName;
                itemName = LocalizeModule.Instance.GetString(exhibitConfig.name);
                item_type = 1;
            }
            else
            {
                ConfProp itemConfig = ConfProp.Get(ItemID);
                icon = itemConfig.icon;
                itemName = LocalizeModule.Instance.GetString(itemConfig.name);
            }
        }

        public override void OnHide()
        {
            base.OnHide();

            
            m_btn.RemoveClickCallBack(OnClicked);
            m_tips_btn.RemoveClickCallBack(OnClicked);
            m_close_btn.RemoveClickCallBack(OnCloseClicked);
        }

        void OnClicked(GameObject obj)
        {

            BgHideAlpha(true);
            PanelHideAlphaTo(1.0f);
            PanelTurnToZero(true);
            this.CloseFrame();
            if (!m_isAcceptTask)
            {
                GameEvents.UIEvents.UI_GameEntry_Event.OnMaskBGVisible.SafeInvoke(false);
            }
            if (null != m_btn_act)
            {
                m_btn_act(m_taskInfo.TaskConfID);
            }

        }


        void OnCloseClicked(GameObject obj)
        {
            GameEvents.UIEvents.UI_GameEntry_Event.OnMaskBGVisible.SafeInvoke(false);
            PanelHideAlphaTo(0.0f);
            PanelGoUp(true);
            this.CloseFrame();
            if (!m_isAcceptTask)
            {
                GameEvents.UIEvents.UI_GameEntry_Event.OnMaskBGVisible.SafeInvoke(false);
                if (null != m_btn_act)
                {
                    m_btn_act(m_taskInfo.TaskConfID);
                }
            }
            
        }


    }

    class AimItemView : GameUIComponent
    {
        private GameImage m_icon;
        private GameLabel m_name_txt;



        protected override void OnInit()
        {
            base.OnInit();
            m_icon = Make<GameImage>("Image");
            m_name_txt = Make<GameLabel>("Text");

        }

        public override void OnShow(object param)
        {
            base.OnShow(param);


        }

        public override void OnHide()
        {
            base.OnHide();

        }


        public void Refresh(string icon_, string name_)
        {
            m_icon.Sprite = icon_;
            m_name_txt.Text = name_;


        }
    }


    struct iconAndName
    {
        public string m_icon;
        public string m_name;
        public int m_type; //0: prop  1: exhabit
    }

    struct RewardWrapper
    {
        public long m_id;
        public string m_icon;
        public int m_num;
    }

    public class AcceptTaskParam
    {
        public NormalTask m_taskInfo;
        public Action<long> m_close_act;
        public bool isAcceptTask = true;
    }

}
