using DG.Tweening;
using EngineCore;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SeekerGame
{

    public partial class GameEntryUILogic //: UILogicBase
    {
        /// <summary>
        /// 任务面板
        /// </summary>
        private class PlayerTaskComponent : GameUIComponent
        {

            private GameImage m_touch_mask = null;
            private GameUIContainer m_taskItemContainer = null;
            private TaskSystem m_playerTaskSystem = null;
            private TaskRewardTweenComponent taskRewardComponent = null;
            private CollectionTaskDetailComponent m_collectionTaskDetailComponent = null;

            private List<TaskBase> m_currentTaskList = null;

            protected override void OnInit()
            {
                this.m_taskItemContainer = Make<GameUIContainer>("Panel:Content");

                this.m_touch_mask = Make<GameImage>("Panel:TouchMask");
                this.taskRewardComponent = Make<TaskRewardTweenComponent>("Panel_TaskReward_new");
                this.m_collectionTaskDetailComponent = Make<CollectionTaskDetailComponent>("Panel_Taskdetail");
            }

            public override void OnShow(object param)
            {
                base.OnShow(param);

                GameEvents.TaskEvents.OnSyncedTaskList += OnSyncedTaskList;
                GameEvents.TaskEvents.OnBlockSyncTask += OnBlockSyncTask;
                GameEvents.TaskEvents.OnCompletedTask += OnCompleteTask;

                m_playerTaskSystem = GlobalInfo.MY_PLAYER_INFO.PlayerTaskSystem;
                GameEvents.TaskEvents.OnCollectTaskReward += CollectTaskReward;
                GameEvents.TaskEvents.OnTryShowCollectionTaskDetail += TryShowCollectTaskDetail;
                //GameEvents.TaskEvents
                GameEvents.UIEvents.UI_GameEntry_Event.OnBlockTaskTouch += OnBlockTaskTouch;


                TaskOnBuildManager.Instance.SyncTaskAnchors();
                GameEvents.ChapterEvents.OnUnlockChapter += OnUnlockChapter;

                RefreshTaskList();


                TimeModule.Instance.SetTimeInterval(UpdateCheckMainTaskExist, 10.0f);

                OnBlockTaskTouch(1.0f);

            }

            private void OnBlockTaskTouch(float t_)
            {
                this.m_touch_mask.Visible = true;

                TimeModule.Instance.SetTimeout(() => this.m_touch_mask.Visible = false, t_);
            }

            private void OnCompleteTask(int msgCode, TaskBase task)
            {
                RefreshTaskList();
            }

            private bool m_block_sync_task = false;

            public void OnSyncedTaskList(int taskCount)
            {
                if (m_block_sync_task)
                    return;

                if (taskCount > 0)
                    RefreshTaskList();
            }


            /// <summary>
            /// 定是检测主线任务，防止丢失。
            /// </summary>
            private void UpdateCheckMainTaskExist()
            {
                if (m_block_sync_task)
                    return;

                foreach (var item in m_currentTaskList)
                {
                    NormalTask taskInfo = item as NormalTask;
                    if (1 == taskInfo.TaskData.type)
                    {
                        //主线存在
                        return;
                    }
                }

                GlobalInfo.MY_PLAYER_INFO.PlayerTaskSystem.RetrievePlayerTaskList();
            }

            private void OnBlockSyncTask(bool block_)
            {
                m_block_sync_task = block_;
            }
            int comparison(TaskBase t1, TaskBase t2)
            {
                NormalTask nt1 = t1 as NormalTask;
                NormalTask nt2 = t2 as NormalTask;

                if (1 == nt1.TaskData.type)
                    return -1;

                if (1 == nt2.TaskData.type)
                    return 1;

                if (TaskSyncStatus.LOCAL_SYNCED == nt1.TaskSyncStatus)
                    return -1;

                if (TaskSyncStatus.LOCAL_SYNCED == nt2.TaskSyncStatus)
                    return 1;

                if (TaskStatus.COMPLETED == nt1.TaskCurrentStatus)
                {
                    return -1;
                }

                if (TaskStatus.COMPLETED == nt2.TaskCurrentStatus)
                {
                    return 1;
                }

                int delta_type = nt1.TaskData.type - nt2.TaskData.type;

                if (0 != delta_type)
                    return delta_type;

                return nt2.TaskCurrentStatus - nt1.TaskCurrentStatus;
            }
            public void RefreshTaskList()
            {
#if MORE_MSG
                BigWorldManager.Instance.EnterBigWorld();
#endif
                m_currentTaskList = this.m_playerTaskSystem.CurrentTaskList;

                m_currentTaskList.Sort(comparison);

                List<TaskBase> main_tasks = m_currentTaskList.FindAll((item) => { NormalTask task = item as NormalTask; return 1 == task.TaskData.type; });

                this.m_taskItemContainer.EnsureSize<TaskItemComponent>(main_tasks.Count);
                for (int i = 0; i < main_tasks.Count; ++i)
                {
                    NormalTask taskInfo = main_tasks[i] as NormalTask;
                    TaskItemComponent taskItemComponent = this.m_taskItemContainer.GetChild<TaskItemComponent>(i);
                    taskItemComponent.Visible = false;
                    taskItemComponent.SetTaskInfo(taskInfo);

                    taskItemComponent.Visible = true;
                }


                //<大地图，支线任务，池任务，图标
                List<TaskBase> pool_tasks = m_currentTaskList.FindAll((item) => { NormalTask task = item as NormalTask; return 3 == task.TaskData.type; });
                List<TaskBase> branch_tasks = m_currentTaskList.FindAll((item) => { NormalTask task = item as NormalTask; return 2 == task.TaskData.type; });

                OpenTaskOnBuildUI(pool_tasks, branch_tasks);
                //>
            }

            private void OpenTaskOnBuildUI(List<TaskBase> pool_task_, List<TaskBase> branch_task_)
            {
                UITaskOnBuildData data = new UITaskOnBuildData();
                data.m_pool_task = pool_task_;
                data.m_branch_task = branch_task_;

                GUIFrame task_on_build_ui = EngineCoreEvents.UIEvent.GetFrameEvent.SafeInvoke(UIDefine.UI_TASK_ON_BUILD);

                if (null != task_on_build_ui && task_on_build_ui.Visible)
                {
                    GameEvents.BigWorld_Event.Listen_ShowTaskOnBuild.SafeInvoke(data);
                    return;
                }

                FrameMgr.OpenUIParams uiparam = new FrameMgr.OpenUIParams(UIDefine.UI_TASK_ON_BUILD);
                uiparam.Param = data;
                EngineCoreEvents.UIEvent.ShowUIEventWithParam.SafeInvoke(uiparam);
            }

            /// <summary>
            /// 解锁新章节
            /// </summary>
            /// <param name="unlockChapter"></param>
            private void OnUnlockChapter(ChapterInfo unlockChapter)
            {
                FrameMgr.OpenUIParams uiParams = new FrameMgr.OpenUIParams(UIDefine.UI_ChapterMap);
                uiParams.Param = unlockChapter;

                EngineCoreEvents.UIEvent.ShowUIEventWithParam.SafeInvoke(uiParams);
            }

            public override void OnHide()
            {
                base.OnHide();
                GameEvents.TaskEvents.OnSyncedTaskList -= OnSyncedTaskList;
                GameEvents.TaskEvents.OnBlockSyncTask -= OnBlockSyncTask;
                GameEvents.TaskEvents.OnCollectTaskReward -= CollectTaskReward;
                GameEvents.TaskEvents.OnCompletedTask -= OnCompleteTask;
                GameEvents.TaskEvents.OnTryShowCollectionTaskDetail -= TryShowCollectTaskDetail;
                GameEvents.UIEvents.UI_GameEntry_Event.OnBlockTaskTouch -= OnBlockTaskTouch;
                GameEvents.ChapterEvents.OnUnlockChapter -= OnUnlockChapter;

                this.m_collectionTaskDetailComponent.Visible = false;

                TimeModule.Instance.RemoveTimeaction(UpdateCheckMainTaskExist);

                this.m_touch_mask.Visible = false;

            }


            private void CollectTaskReward(NormalTask taskInfo)
            {
                AcceptTaskParam atp_param = new AcceptTaskParam()
                {
                    m_taskInfo = taskInfo,
                    m_close_act = delegate (long id)
                    {
                        taskRewardComponent.SetRewardTaskInfo(taskInfo);
                        if (taskRewardComponent.HasRewardClue)
                            taskRewardComponent.Visible = true;
                        else
                            taskRewardComponent.ForceCommitTask();

                    },
                    isAcceptTask = false
                };

                FrameMgr.OpenUIParams param = new FrameMgr.OpenUIParams(UIDefine.UI_ACCEPT_TASK);
                param.Param = atp_param;
                EngineCoreEvents.UIEvent.ShowUIEventWithParam.SafeInvoke(param);

            }

            private NormalTask m_cacheTaskInfo = null;
            private Action<long> m_cacheClose_Act = null;
            private void TryShowCollectTaskDetail(NormalTask taskInfo, Action<long> close_act_)
            {
                if (CommonHelper.NeedOpenTalkUI(taskInfo))
                {
                    m_cacheTaskInfo = taskInfo;
                    m_cacheClose_Act = close_act_;
                    GameEvents.UIEvents.UI_Talk_Event.OnTalkFinish -= OnTalkFinish;
                    GameEvents.UIEvents.UI_Talk_Event.OnTalkFinish += OnTalkFinish;
                    TalkUIHelper.OnStartTalk(taskInfo.TaskData.dialogBegin, TalkDialogEnum.AchiveTalk);
                }
                else
                {
                    ShowCollectTaskDetail(taskInfo, close_act_);
                }

            }

            private void OnTalkFinish(long talkId)
            {
                GameEvents.UIEvents.UI_Talk_Event.OnTalkFinish -= OnTalkFinish;
                if (m_cacheTaskInfo != null)
                {
                    ShowCollectTaskDetail(m_cacheTaskInfo, m_cacheClose_Act);
                }
            }

            private void ShowCollectTaskDetail(NormalTask taskInfo, Action<long> close_act_)
            {
                if (taskInfo.TaskData != null && !CommonHelper.NeedOpenAcceptTaskUI(taskInfo.TaskData.conditionSceneId))
                {
                    return;
                }
                AcceptTaskParam atp_param = new AcceptTaskParam()
                {
                    m_taskInfo = taskInfo,
                    m_close_act = close_act_,
                    isAcceptTask = true
                };

                FrameMgr.OpenUIParams param = new FrameMgr.OpenUIParams(UIDefine.UI_ACCEPT_TASK);
                param.Param = atp_param;
                EngineCoreEvents.UIEvent.ShowUIEventWithParam.SafeInvoke(param);
            }

            /// <summary>
            /// 任务项
            /// </summary>
            private class TaskItemComponent : GameUIComponent
            {
                private GameLabel m_lbTaskName = null;
                private GameImage m_textureTaskCover = null;
                private GameImage m_btnCollectReward = null;
                private GameImage m_btnDownloadChapter = null;

                private NormalTask m_taskInfo = null;
                //private GameImage m_taskItemBtn = null;
                private ChapterInfo m_taskChapterInfo = null;
                private GameUIEffect m_effect = null;
                private bool m_taskTips = false;
                protected override void OnInit()
                {
                    //this.m_taskItemBtn = Make<GameImage>(gameObject);
                    this.m_lbTaskName = Make<GameLabel>("Text");
                    this.m_textureTaskCover = Make<GameImage>("RawImage");
                    this.m_btnCollectReward = Make<GameImage>("Image_gift");
                    this.m_btnDownloadChapter = Make<GameImage>("Btn_Download");
                    this.m_effect = Make<GameUIEffect>("UI_zhuxianrenwu");
                }

                public override void OnShow(object param)
                {
                    //this.m_taskItemBtn.AddClickCallBack(OnTaskItemClick);
                    this.m_textureTaskCover.AddClickCallBack(OnTaskItemClick);
                    this.m_btnCollectReward.AddClickCallBack(OnTaskRewardItemClick);
                    this.m_btnDownloadChapter.AddClickCallBack(OnDownloadChapterClick);
                    MessageHandler.RegisterMessageHandler(MessageDefine.SCCanTaskResponse, OnReponse);
                    this.m_effect.EffectPrefabName = "UI_zhuxianrenwu.prefab";
                    GameEvents.UIEvents.UI_GameEntry_Event.OnFirstTimeEnterGame += OnCheckChapter;

                    if (1 == this.m_taskInfo.TaskData.type)
                        GameEvents.UIEvents.UI_Bonus_Pop_View_Event.Listen_OnShow += OnShowBonusPopView;
                }

                public void SetTaskInfo(NormalTask taskInfo)
                {
                    this.m_taskInfo = taskInfo;
                    this.gameObject.name = taskInfo.TaskConfID.ToString();
                    RefreshTaskItemUI();
                }



                private void RefreshTaskItemUI()
                {
                    this.m_lbTaskName.Text = LocalizeModule.Instance.GetString(this.m_taskInfo.TaskData.name);

                    this.m_btnCollectReward.Visible = this.m_taskInfo.TaskCurrentStatus == TaskStatus.COMPLETED;


                    this.m_textureTaskCover.Sprite = this.m_taskInfo.TaskData.backgroundIcon;


                    if (this.m_taskInfo.TaskData.autoSkip && TaskStatus.COMPLETED != this.m_taskInfo.TaskCurrentStatus && TaskSyncStatus.SYNCED == this.m_taskInfo.TaskSyncStatus)
                    {

                        TaskHelper.AcceptTask(this.m_taskInfo);
                        this.m_effect.Visible = false;
                        return;
                    }
                    else if (this.m_taskInfo.TaskData.type == 1 && this.m_taskInfo.TaskCurrentStatus == TaskStatus.PROGRESSING)
                    {
                        this.m_effect.Visible = true;
                        this.m_taskTips = true;
                    }
                    else
                    {
                        this.m_effect.Visible = false;
                        this.m_taskTips = false;
                    }


                    if (this.m_taskInfo.TaskCurrentStatus == TaskStatus.COMPLETED
                        && this.m_taskInfo.TaskData.type == 1
                        && !this.m_taskInfo.TaskData.autoSkip
                        && TaskSyncStatus.LOCAL_SYNCED != this.m_taskInfo.TaskSyncStatus)
                    {
                        this.m_effect.Visible = false;
                        TimeModule.Instance.SetTimeout(() => TaskHelper.GetReward(this.m_taskInfo, () => this.Visible = false, true), 0.7f);
                    }

                    m_taskChapterInfo = GlobalInfo.MY_PLAYER_INFO.PlayerChapterSystem.FindChapterByTaskID(this.m_taskInfo.TaskConfID);
                    if (m_taskChapterInfo != null)
                    {
                        bool isChapterValid = GlobalInfo.MY_PLAYER_INFO.PlayerChapterSystem.IsChapterAssetExist(m_taskChapterInfo.ChapterID);
                        this.m_btnDownloadChapter.Visible = !isChapterValid;
                        this.m_textureTaskCover.SetGray(!isChapterValid);

                        GameEvents.ChapterEvents.OnChapterDownloadFinish += OnChapterDownloadFinish;
                    }
                }

                private void DisableTaskItemClick()
                {
                    this.m_textureTaskCover.EnableClick = false;
                    this.m_btnCollectReward.Enable = false;
                }

                private void EnableTaskItemClick()
                {
                    this.m_textureTaskCover.EnableClick = true;
                    this.m_btnCollectReward.Enable = true;
                }

                private void OnShowBonusPopView(EUNM_BONUS_POP_VIEW_TYPE t_)
                {
                    if (EUNM_BONUS_POP_VIEW_TYPE.E_TASK_REWARD == t_)
                    {

                        TaskHelper.GetReward(this.m_taskInfo, () => this.Visible = false);
                    }
                }

                private void OnChapterDownloadFinish(ChapterInfo downloadedChapterInfo)
                {
                    GameEvents.ChapterEvents.OnChapterDownloadFinish -= OnChapterDownloadFinish;
                    GameEvents.TaskEvents.OnSyncedTaskList.SafeInvoke(1);
                }

                private void OnTaskItemClick(GameObject taskItem)
                {
                    GameEvents.BigWorld_Event.OnClickScreen.SafeInvoke();
                    NewGuid.GuidNewNodeManager.Instance.taskConfId = m_taskInfo.TaskConfID;
                    TaskHelper.OnTaskItemClick(m_taskInfo, () => this.Visible = false);
                }

                private void OnCheckChapter()
                {

                    bool isChapterValid = GlobalInfo.MY_PLAYER_INFO.PlayerChapterSystem.IsChapterAssetExist(m_taskChapterInfo.ChapterID);
                    if (!isChapterValid)
                        OnDownloadChapterClick(null);
                }

                private void OnDownloadChapterClick(GameObject btnDownloadChapter)
                {
                    FrameMgr.OpenUIParams uiParams = new FrameMgr.OpenUIParams(UIDefine.UI_ChapterMap);
                    uiParams.Param = m_taskChapterInfo.ChapterID;

                    EngineCoreEvents.UIEvent.ShowUIEventWithParam.SafeInvoke(uiParams);
                }


                private void OnReponse(object obj)
                {
                    if (obj is SCCanTaskResponse)
                    {
                        TaskHelper.OnReponse(obj, this.m_taskInfo);
                    }
                }

                private void OnTaskRewardItemClick(GameObject btnCollectReward)
                {

                    TaskHelper.GetReward(this.m_taskInfo, () => this.Visible = false);
                }

                public override void OnHide()
                {
                    this.m_textureTaskCover.RemoveClickCallBack(OnTaskItemClick);
                    this.m_btnCollectReward.RemoveClickCallBack(OnTaskRewardItemClick);
                    this.m_btnDownloadChapter.RemoveClickCallBack(OnDownloadChapterClick);
                    MessageHandler.UnRegisterMessageHandler(MessageDefine.SCCanTaskResponse, OnReponse);
                    this.m_effect.Visible = false;
                    GameEvents.UIEvents.UI_Bonus_Pop_View_Event.Listen_OnShow -= OnShowBonusPopView;
                    GameEvents.UIEvents.UI_GameEntry_Event.OnFirstTimeEnterGame -= OnCheckChapter;
                }

                private ChapterInfo GetTaskBelongChapter(long taskConfigID)
                {
                    return GlobalInfo.MY_PLAYER_INFO.PlayerChapterSystem.FindChapterByTaskID(taskConfigID);
                }
            }

            /// <summary>
            /// 任务领奖励面板
            /// </summary>
            private class TaskRewardTweenComponent : GameUIComponent
            {
                private GameImage m_bg_img;
                private GameButton m_btnReward = null;
                private TaskRewardItemsTweenComponent m_rewardItemComponent = null;
                private TaskRewardClueGridComponent m_rewardClueComponent = null;
                private TaskKeyClueComponent m_keyClueComponent = null;

                private NormalTask m_rewardTask = null;
                private GameUIComponent m_currentComponent = null;

                // private GameSpine m_portrait;

                private bool isLastPanel = false;
                private bool m_is_commited = false;

                private long m_task_id;

                private string m_portrait_name;
                protected override void OnInit()
                {
                    this.m_bg_img = Make<GameImage>("Image_bg");
                    this.m_btnReward = Make<GameButton>("Panel_Animation:Button");
                    this.m_rewardItemComponent = Make<TaskRewardItemsTweenComponent>("Panel_Animation:Panel");
                    this.m_rewardClueComponent = Make<TaskRewardClueGridComponent>("Panel_Animation:grid");
                    this.m_keyClueComponent = Make<TaskKeyClueComponent>("Panel_Animation:Panel_clue");

                    //this.m_portrait = Make<GameSpine>("Panel_Animation:RawImage_nps");

                    m_portrait_name = string.Empty;

                    RefreshPlayerInfo();
                }

                private void RefreshPlayerInfo()
                {
                    if (GlobalInfo.MY_PLAYER_INFO.PlayerIcon == m_portrait_name)
                        return;

                    m_portrait_name = GlobalInfo.MY_PLAYER_INFO.PlayerIcon;

                    //if (!CommonTools.IsNeedDownloadIcon(GlobalInfo.MY_PLAYER_INFO.PlayerIcon))
                    //    this.m_portrait.SpineName = CommonData.GetSpineHead(GlobalInfo.MY_PLAYER_INFO.PlayerIcon);
                }

                public override void OnShow(object param)
                {
                    base.OnShow(param);

                    GameEvents.UIEvents.UI_Bonus_Pop_View_Event.Tell_OnBlock.SafeInvoke(true);
                    GameEvents.UIEvents.UI_GameEntry_Event.OnReceiveRewardAuto += OnReceiveRewardAuto;
                    isLastPanel = false;
                    m_is_commited = false;

                    this.m_btnReward.AddClickCallBack(OnBtnRewardClick);

                    m_bg_img.Visible = true;
                    m_bg_img.Color = new Color(m_bg_img.Color.r, m_bg_img.Color.g, m_bg_img.Color.b, 1.0f);
                    m_btnReward.Visible = true;
                    //m_currentComponent = this.m_rewardClueComponent;

                    //m_currentComponent.Visible = true;

                    m_rewardClueComponent.Visible = false;
                    m_keyClueComponent.Visible = false;
                    if (this.m_rewardClueComponent.HasContent)
                    {
                        this.m_currentComponent = this.m_rewardClueComponent;

                        CommonHelper.ResetTween(this.m_rewardClueComponent.gameObject);

                        this.m_rewardClueComponent.Visible = true;
                    }
                    else if (this.m_keyClueComponent.HasContent)
                    {
                        this.m_currentComponent = this.m_keyClueComponent;

                        CommonHelper.ResetTween(this.m_keyClueComponent.gameObject);

                        this.m_keyClueComponent.Visible = true;
                    }
                    //if (this.m_rewardTitleComponent.HasContent)
                    //    this.isLastPanel = false;
                    if (this.m_rewardClueComponent.HasContent && this.m_keyClueComponent.HasContent)
                    {
                        this.isLastPanel = false;
                    }
                    else
                        this.isLastPanel = true;


                    RefreshPlayerInfo();
                }

                public bool HasRewardClue
                {
                    get
                    {
                        return (this.m_rewardClueComponent.HasContent || this.m_keyClueComponent.HasContent);
                    }
                }


                public void ForceCommitTask()
                {
                    m_is_commited = true;
                    if (m_rewardTask.TaskData.dialogEnd > 0)
                    {
                        TalkUIHelper.OnStartTalk(m_rewardTask.TaskData.dialogEnd, TalkDialogEnum.TaskEndTalk);
                    }

                    TimeModule.Instance.RemoveTimeaction(CommitTask);
                    TimeModule.Instance.RemoveTimeaction(NextRewardStep);

                    ShowNextTaskByLocalData(this.m_rewardTask);

                    CSTaskCommitRequest request = new CSTaskCommitRequest();
                    request.PlayerTaskId = this.m_rewardTask.TaskUUID;
                    GameEvents.NetWorkEvents.SendAsyncMsg.SafeInvoke(request);

                    GameEvents.TaskEvents.OnTaskFinish.SafeInvoke(this.m_rewardTask);
                    GameEvents.TaskEvents.OnReceiveTask.SafeInvoke(this.m_rewardTask);

                    Visible = false;

                }
                private void CommitTask()
                {
                    if (false == Visible)
                        return;

                    if (m_is_commited)
                        return;

                    ForceCommitTask();

                }


                /// <summary>
                /// 提前显示下一个任务
                /// </summary>
                /// <param name="cur_task_"></param>
                private void ShowNextTaskByLocalData(NormalTask cur_task_)
                {
                    if (1 != cur_task_.TaskData.type)
                        return;

                    cur_task_.TaskSyncStatus = TaskSyncStatus.LOCAL_SYNCED; //当前领取奖励的任务，同步状态为“本地确认”，既不可以再领奖了。
                    long next_task_config_id = cur_task_.TaskData.nextTaskId;

                    GameEvents.TaskEvents.OnAcceptNewTaskLocal.SafeInvoke(next_task_config_id);
                }


                private void OnBtnRewardClick(GameObject btnTaskReward)
                {


                    EngineCoreEvents.AudioEvents.PlayAudio.SafeInvoke(Audio.AudioType.UISound, GameCustomAudioKey.task_confrim.ToString());

                    if (isLastPanel)
                    {
                        CommitTask();
                    }
                    else
                    {
                        if (this.m_currentComponent == m_rewardClueComponent)
                        {
                            if (this.m_keyClueComponent.HasContent)
                            {
                                //TimeModule.Instance.RemoveTimeaction(NextRewardStep);

                                m_rewardClueComponent.Visible = false;

                                if (this.m_keyClueComponent.HasContent)
                                {
                                    this.m_currentComponent = this.m_keyClueComponent;

                                    CommonHelper.ResetTween(this.m_keyClueComponent.gameObject);

                                    this.m_keyClueComponent.Visible = true;

                                    TimeModule.Instance.SetTimeout(CommitTask, 3.0f);
                                }

                                this.isLastPanel = true;
                            }
                        }
                        else
                        {
                            this.isLastPanel = true;
                        }
                        //if (this.m_currentComponent == m_rewardItemComponent)
                        //{
                        //    if (this.m_rewardClueComponent.HasContent)
                        //    {
                        //        this.m_rewardItemComponent.Visible = false;
                        //        this.m_currentComponent = this.m_rewardClueComponent;

                        //        CommonHelper.ResetTween(this.m_rewardClueComponent.gameObject);

                        //        this.m_rewardClueComponent.Visible = true;

                        //        TimeModule.Instance.SetTimeout(() => m_bg_img.Color = new Color(m_bg_img.Color.r, m_bg_img.Color.g, m_bg_img.Color.b, 0.05f), 2.5f);

                        //        if (this.m_keyClueComponent.HasContent)
                        //        {
                        //            TimeModule.Instance.SetTimeout(NextRewardStep, 3.0f);
                        //        }
                        //        else
                        //        {
                        //            this.isLastPanel = true;
                        //            TimeModule.Instance.SetTimeout(CommitTask, 3.0f);
                        //        }

                        //    }
                        //    else if (this.m_keyClueComponent.HasContent)
                        //    {
                        //        this.m_rewardItemComponent.Visible = false;
                        //        this.m_currentComponent = this.m_keyClueComponent;

                        //        CommonHelper.ResetTween(this.m_keyClueComponent.gameObject);

                        //        this.m_keyClueComponent.Visible = true;

                        //        this.isLastPanel = true;

                        //        TimeModule.Instance.SetTimeout(CommitTask, 3.0f);
                        //    }
                        //    else
                        //    {
                        //        this.isLastPanel = true;
                        //    }
                        //}
                        //else if (this.m_currentComponent == m_rewardClueComponent)
                        //{
                        //    TimeModule.Instance.RemoveTimeaction(NextRewardStep);

                        //    m_rewardClueComponent.Visible = false;

                        //    if (this.m_keyClueComponent.HasContent)
                        //    {
                        //        this.m_currentComponent = this.m_keyClueComponent;

                        //        CommonHelper.ResetTween(this.m_keyClueComponent.gameObject);

                        //        this.m_keyClueComponent.Visible = true;

                        //        TimeModule.Instance.SetTimeout(CommitTask, 3.0f);
                        //    }

                        //    this.isLastPanel = true;
                        //}
                        //else
                        //{
                        //    this.isLastPanel = true;
                        //}

                    }
                }

                private void NextRewardStep()
                {
                    if (false == Visible)
                        return;

                    OnBtnRewardClick(null);
                }

                public void SetRewardTaskInfo(NormalTask taskInfo)
                {
                    this.m_rewardTask = taskInfo;


                    //List<RewardItemDataWrap> rewardItemList = new List<RewardItemDataWrap>();
                    List<RewardClueDataWarp> clueList = new List<RewardClueDataWarp>();


                    long chapter_id = GlobalInfo.MY_PLAYER_INFO.PlayerChapterSystem.CurrentChapterInfo.ChapterID;
                    ConfTask taskConfig = taskInfo.TaskData;
                    ConfChapter chapter_data = ConfChapter.Get(chapter_id);

                    List<int> idxes = new List<int>();

                    for (int i = 0; i < chapter_data.clueUnlockTaskIds.Length; ++i)
                    {
                        if (taskConfig.id == chapter_data.clueUnlockTaskIds[i])
                        {
                            idxes.Add(i);
                        }
                    }

                    RewardClueDataWarp warp;
                    foreach (var jigsaw_idx in idxes)
                    {
                        warp = new RewardClueDataWarp() { m_clueType = TaskClueMode.JIGSAW, m_png = chapter_data.clueIds[jigsaw_idx] };
                        clueList.Add(warp);
                    }

                    idxes.Clear();
                    for (int i = 0; i < chapter_data.actorUnlockTaskIds.Length; ++i)
                    {
                        if (taskConfig.id == chapter_data.actorUnlockTaskIds[i])
                        {
                            idxes.Add(i);
                        }
                    }

                    foreach (var npc_idx in idxes)
                    {
                        warp = new RewardClueDataWarp() { m_clueType = TaskClueMode.NPC, m_id = chapter_data.actorIds[npc_idx] };
                        clueList.Add(warp);
                    }

                    idxes.Clear();
                    for (int i = 0; i < chapter_data.sceneUnlockTaskIds.Length; ++i)
                    {
                        if (taskConfig.id == chapter_data.sceneUnlockTaskIds[i])
                        {
                            idxes.Add(i);
                        }
                    }

                    foreach (var scene_idx in idxes)
                    {
                        warp = new RewardClueDataWarp() { m_clueType = TaskClueMode.SCENE, m_id = chapter_data.scenceIds[scene_idx] };
                        clueList.Add(warp);
                    }



                    //金钱奖励肯定会有
                    for (int i = 0; i < taskInfo.RewardList.Count; ++i)
                    {
                        TaskRewardBase taskReward = taskInfo.RewardList[i];
                        TaskRewardMode taskRewardType = (taskReward.GetType().GetCustomAttributes(typeof(TaskRewardAttribute), true)[0] as TaskRewardAttribute).RewardMode;

                        switch (taskRewardType)
                        {
                            //case TaskRewardMode.ITEM:
                            //    TaskRewardItem rewardItem = taskReward as TaskRewardItem;
                            //    rewardItemList.Add(rewardItem.RewardData);
                            //    break;
                            //case TaskRewardMode.TITLE:
                            //    //this.m_rewardTitleComponent.SetTitle((taskReward as TaskRewardTitle).RewardData);
                            //    this.m_rewardItemComponent.SetTitle((taskReward as TaskRewardTitle).RewardData);
                            //    break;
                            case TaskRewardMode.CLUE:
                                TaskRewardClue clueItem = taskReward as TaskRewardClue;
                                clueList.Add(clueItem.RewardData);
                                break;
                        }

                    }
                    string content = LocalizeModule.Instance.GetString(ConfTask.Get(taskInfo.TaskData.id).descs);
                    //this.m_rewardItemComponent.RefreshTaskDesc(content);
                    //this.m_rewardItemComponent.SetRewardItemData(rewardItemList);

#if TEST
                    warp = new RewardClueDataWarp() { m_clueType = TaskClueMode.JIGSAW, m_png = "jigsaw_1002_1.png" };
                    clueList.Add(warp);

                    warp = new RewardClueDataWarp() { m_clueType = TaskClueMode.NPC, m_id = 2 };
                    clueList.Add(warp);

                    warp = new RewardClueDataWarp() { m_clueType = TaskClueMode.SCENE, m_id = 10003 };
                    clueList.Add(warp);

                    this.m_rewardItemComponent.SetTitle(2L);
#endif

                    this.m_rewardClueComponent.Refresh(clueList);
                    this.m_keyClueComponent.Refresh(clueList, m_rewardTask); ;
                }

                private void OnReceiveRewardAuto()
                {
                    Visible = false;
                }

                public override void OnHide()
                {
                    base.OnHide();
                    this.m_is_commited = false;
                    this.m_rewardItemComponent.Visible = false;
                    this.m_rewardClueComponent.Visible = false;
                    GameEvents.UIEvents.UI_GameEntry_Event.OnReceiveRewardAuto -= OnReceiveRewardAuto;
                    this.m_btnReward.RemoveClickCallBack(OnBtnRewardClick);
                    GameEvents.UIEvents.UI_Bonus_Pop_View_Event.Tell_OnBlock.SafeInvoke(false);
                    GameEvents.UIEvents.UI_Bonus_Pop_View_Event.Tell_OnShow.SafeInvoke();
                }



                private class TaskRewardItemsTweenComponent : GameUIComponent
                {

                    private class RewardTitleTweenComponet : GameUIComponent
                    {
                        private GameLabel m_root_lbl;
                        private GameImage m_title_icon;
                        private GameLabel m_title_txt;

                        protected override void OnInit()
                        {
                            m_root_lbl = Make<GameLabel>(gameObject.name);
                            string get_str = LocalizeModule.Instance.GetString("over_dialogue_get_1");
                            string title_str = LocalizeModule.Instance.GetString("title_name");
                            m_root_lbl.Text = string.Format("{0}{1}", get_str, title_str);
                            m_title_icon = Make<GameImage>("Image");
                            m_title_txt = Make<GameLabel>("Text (1)");
                        }

                        public void Refresh(long titleID)
                        {
                            m_title_icon.Sprite = ConfTitle.Get(titleID).icon;
                            m_title_txt.Text = LocalizeModule.Instance.GetString(ConfTitle.Get(titleID).name);
                        }
                    }

                    //private List<TaskRewardBaseItemComponent> m_rewardBaseItemList = new List<TaskRewardBaseItemComponent>();

                    //private GameLabel m_desc;
                    //private GameUIComponent m_output_root;
                    //private GameUIComponent m_drop_root;
                    //public GameUIContainer m_outputItemContainer = null;
                    //private GameUIContainer m_rewardItemContainer = null;
                    RewardTitleTweenComponet m_rewardTitle = null;


                    protected override void OnInit()
                    {

                        //for (int i = 1; i <= 4; ++i)
                        //{
                        //    TaskRewardBaseItemComponent taskRewardBaseItemComponent = Make<TaskRewardBaseItemComponent>("Image_" + i);
                        //    taskRewardBaseItemComponent.Visible = false;
                        //    this.m_rewardBaseItemList.Add(taskRewardBaseItemComponent);
                        //}

                        //this.m_desc = Make<GameLabel>("Text_Desc");


                        //this.m_output_root = Make<GameUIContainer>("Scroll View_1");
                        //this.m_drop_root = Make<GameUIContainer>("Scroll View_2");
                        //this.m_output_root.Visible = false;
                        //this.m_drop_root.Visible = false;
                        //this.m_outputItemContainer = Make<GameUIContainer>("Scroll View_1:Viewport");
                        //this.m_rewardItemContainer = Make<GameUIContainer>("Scroll View_2:Viewport");
                        this.m_rewardTitle = Make<RewardTitleTweenComponet>("Text");

                    }

                    public override void OnShow(object param)
                    {
                        base.OnShow(param);
                        //this.m_output_root.Visible = false;
                        //this.m_drop_root.Visible = false;
                        m_rewardTitle.Visible = false;
                    }

                    public void RefreshTaskDesc(string content_)
                    {
                        //m_desc.Text = content_;
                    }

                    private void ResetOutputTween(bool enable_delay_tween_ = true)
                    {
                        //this.m_output_root.Visible = true;

                        //UITweenerBase[] tweens = this.m_output_root.GetComponents<UITweenerBase>();

                        //foreach (var item in tweens)
                        //{
                        //    if (!enable_delay_tween_ && item.Delay > 1.5f)
                        //    {
                        //        item.Stop();
                        //        continue;
                        //    }

                        //    item.ResetAndPlay();
                        //}
                    }


                    private void ResetDropTween()
                    {
                        //this.m_drop_root.Visible = true;

                        //UITweenerBase[] tweens = this.m_drop_root.GetComponents<UITweenerBase>();

                        //foreach (var item in tweens)
                        //{
                        //    item.ResetAndPlay();
                        //}
                    }

                    public void SetRewardItemData(List<RewardItemDataWrap> rewardList)
                    {


                        List<RewardItemDataWrap> specialItemList = new List<RewardItemDataWrap>();
                        List<RewardItemDataWrap> baseItemList = new List<RewardItemDataWrap>();
                        for (int i = 0; i < rewardList.Count; ++i)
                        {
                            RewardItemDataWrap rewardItemData = rewardList[i];
                            switch (rewardItemData.ItemType)
                            {
                                case RewardItemType.CASH:
                                case RewardItemType.EXP:
                                case RewardItemType.COIN:
                                case RewardItemType.VIT:
                                    baseItemList.Add(rewardItemData);
                                    break;
                                case RewardItemType.ITEM:
                                    specialItemList.Add(rewardItemData);
                                    break;
                            }
                        }

                        //this.m_rewardItemContainer.EnsureSize<TaskRewardSpecialItemComponent>(specialItemList.Count);
                        //for (int i = 0; i < this.m_rewardItemContainer.ChildCount; ++i)
                        //{
                        //    TaskRewardSpecialItemComponent rewardItem = this.m_rewardItemContainer.GetChild<TaskRewardSpecialItemComponent>(i);
                        //    rewardItem.SetRewardSpecialItemData(specialItemList[i]);
                        //    rewardItem.Visible = true;
                        //}

                        //this.m_drop_root.Visible = 0 != specialItemList.Count ? true : false;

                        baseItemList = baseItemList.OrderBy(x => (int)(x.ItemType)).ToList();

                        //this.m_outputItemContainer.EnsureSize<TaskRewardBaseItemComponent>(baseItemList.Count);
                        //for (int i = 0; i < this.m_outputItemContainer.ChildCount; ++i)
                        //{
                        //    TaskRewardBaseItemComponent rewardItem = this.m_outputItemContainer.GetChild<TaskRewardBaseItemComponent>(i);
                        //    rewardItem.SetOutputItemData(baseItemList[i]);
                        //    rewardItem.Visible = true;
                        //}

                        if (baseItemList.Count > 0)
                        {
                            ResetOutputTween(specialItemList.Count > 0);
                        }
                        else
                        {
                            if (specialItemList.Count > 0)
                            {
                                ResetDropTween();
                            }
                        }

                    }


                    public void SetTitle(long titleID)
                    {
                        m_rewardTitle.Refresh(titleID);

                        CommonHelper.ResetTween(m_rewardTitle.gameObject);

                        m_rewardTitle.Visible = true;

                    }



                    private class TaskRewardBaseItemComponent : GameUIComponent
                    {
                        private GameImage m_imgItemType = null;
                        private GameLabel m_lbRewardNum = null;
                        private RewardItemDataWrap rewardItemData;

                        protected override void OnInit()
                        {
                            base.OnInit();

                            this.m_imgItemType = Make<GameImage>("Background:Image");
                            this.m_lbRewardNum = Make<GameLabel>("Background:Image (1):Text");
                        }

                        public void SetOutputItemData(RewardItemDataWrap data)
                        {
                            this.rewardItemData = data;
                            string iconName = "icon_mainpanel_";
                            this.m_imgItemType.Sprite = CommonHelper.GetOutputIconName(data.ItemType); ;
                            this.m_lbRewardNum.Text = data.ItemNum.ToString();
                        }
                    }


                    private class TaskRewardSpecialItemComponent : GameUIComponent
                    {
                        private GameImage m_imgItemIcon;
                        private GameLabel m_lbItemCount;

                        protected override void OnInit()
                        {
                            this.m_imgItemIcon = Make<GameImage>("Background:Image");
                            this.m_lbItemCount = Make<GameLabel>("Background:Image (1):Text");
                        }


                        public void SetRewardSpecialItemData(RewardItemDataWrap itemDataWrap)
                        {
                            ConfProp itemConf = ConfProp.Get(itemDataWrap.ItemID);
                            if (itemConf != null)
                            {
                                this.m_imgItemIcon.Sprite = itemConf.icon;
                                this.m_lbItemCount.Text = string.Format("x{0}", itemDataWrap.ItemNum.ToString());
                                this.Visible = true;
                            }
                        }

                    }

                }



                /// <summary>
                /// 解锁展示
                /// </summary>
                private class TaskRewardClueGridComponent : GameUIComponent
                {

                    private class ClueItem : GameUIComponent
                    {
                        protected GameTexture m_tex;
                        protected GameLabel m_txt;
                        protected override void OnInit()
                        {
                            this.m_tex = Make<GameTexture>("rawimage");
                            this.m_txt = Make<GameLabel>("Text");
                        }

                        public override void OnShow(object param)
                        {
                            base.OnShow(param);


                        }

                        public virtual void Refresh(string png_name_, string content_ = "")
                        {
                            m_tex.TextureName = png_name_;
                            m_txt.Text = content_;

                        }
                    }


                    private class ClueJigsawItem : ClueItem
                    {
                        protected override void OnInit()
                        {
                            this.m_tex = Make<GameTexture>("Panel:rawimage");
                            this.m_txt = Make<GameLabel>("Text");
                        }
                    }


                    private class NpcItem : ClueItem
                    {
                        GameImage m_img_icon;
                        protected override void OnInit()
                        {
                            base.OnInit();
                            m_img_icon = Make<GameImage>("Image");
                        }



                        public override void Refresh(string png_name_, string content_ = "")
                        {
                            if (png_name_.StartsWith("image_suspect"))
                            {
                                m_tex.TextureName = png_name_;
                                m_tex.Visible = true;
                                m_img_icon.Visible = false;
                            }
                            else
                            {
                                m_img_icon.Sprite = png_name_;
                                m_img_icon.Visible = true;
                                m_tex.Visible = false;
                            }

                        }
                    }


                    private GameUIContainer m_grid_piece;
                    private GameUIContainer m_grid_role;
                    private GameUIContainer m_grid_scene;
                    private GameUIContainer m_grid_jigsaw;

                    private List<RewardClueDataWarp> pieces;
                    private List<RewardClueDataWarp> roles;
                    private List<RewardClueDataWarp> scenes;

                    protected override void OnInit()
                    {
                        this.m_grid_piece = this.Make<GameUIContainer>("grid:grid_piece");
                        this.m_grid_role = this.Make<GameUIContainer>("grid:grid_role");
                        this.m_grid_scene = this.Make<GameUIContainer>("grid:grid_scene");
                        this.m_grid_jigsaw = this.Make<GameUIContainer>("grid:grid_jingsaw");
                    }

                    public override void OnShow(object param)
                    {
                        base.OnShow(param);

                        this.SetDestPosToChapterButton(this);

                    }

                    public void Refresh(List<RewardClueDataWarp> clueList)
                    {
                        if (clueList.Count > 0)
                            HasContent = true;
                        else
                        {
                            HasContent = false;
                        }

                        pieces = new List<RewardClueDataWarp>();
                        roles = new List<RewardClueDataWarp>();
                        scenes = new List<RewardClueDataWarp>();


                        for (int i = 0; i < clueList.Count; ++i)
                        {
                            RewardClueDataWarp item_data = clueList[i];

                            switch (item_data.m_clueType)
                            {
                                case TaskClueMode.JIGSAW:
                                    {
                                        pieces.Add(item_data);
                                    }
                                    break;
                                case TaskClueMode.NPC:
                                    {
                                        roles.Add(item_data);
                                    }
                                    break;
                                case TaskClueMode.SCENE:
                                    {
                                        scenes.Add(item_data);
                                    }
                                    break;
                                default:
                                    break;
                            }
                        }

                        m_grid_piece.Visible = false;
                        m_grid_role.Visible = false;
                        m_grid_scene.Visible = false;
                        m_grid_jigsaw.Visible = false;


                        if (pieces.Count > 0)
                        {
                            m_grid_piece.Visible = true;

                            m_grid_piece.EnsureSize<ClueItem>(pieces.Count);

                            for (int i = 0; i < m_grid_piece.ChildCount; ++i)
                            {
                                ClueItem item = m_grid_piece.GetChild<ClueItem>(i);

                                string content = LocalizeModule.Instance.GetString("archive_fragment");
                                item.Refresh(pieces[i].m_png, content);
                                item.Visible = false;
                                item.Visible = true;
                            }
                        }

                        if (roles.Count > 0)
                        {
                            m_grid_role.Visible = true;

                            m_grid_role.EnsureSize<NpcItem>(roles.Count);

                            for (int i = 0; i < m_grid_role.ChildCount; ++i)
                            {
                                NpcItem item = m_grid_role.GetChild<NpcItem>(i);

                                ConfNpc npc = ConfNpc.Get(roles[i].m_id);
                                string content = LocalizeModule.Instance.GetString("archive_NPC", LocalizeModule.Instance.GetString(npc.name));

                                string icon_name = npc.icon;

                                if (icon_name.StartsWith("image_suspect"))
                                    icon_name = npc.icon.Replace("size2", "size1");

                                item.Refresh(icon_name, content);
                                item.Visible = false;
                                item.Visible = true;
                            }
                        }

                        if (scenes.Count > 0)
                        {
                            List<RewardClueDataWarp> scene_lst = scenes.FindAll((item) => item.m_id / CommonData.C_SCENE_TYPE_ID < CommonData.C_JIGSAW_SCENE_START_ID);
                            List<RewardClueDataWarp> jigsaw_lst = scenes.FindAll((item) => item.m_id / CommonData.C_SCENE_TYPE_ID >= CommonData.C_JIGSAW_SCENE_START_ID && item.m_id / CommonData.C_SCENE_TYPE_ID < CommonData.C_CARTOON_SCENE_START_ID);

                            if (null != scene_lst && scene_lst.Count > 0)
                            {
                                m_grid_scene.Visible = true;
                                m_grid_scene.EnsureSize<ClueItem>(scene_lst.Count);

                                for (int i = 0; i < m_grid_scene.ChildCount; ++i)
                                {
                                    ClueItem item = m_grid_scene.GetChild<ClueItem>(i);

                                    ConfScene scene = ConfScene.Get(scene_lst[i].m_id);
                                    if (scene != null)
                                    {
                                        string content = LocalizeModule.Instance.GetString("archive_scene", LocalizeModule.Instance.GetString(scene.name));
                                        item.Refresh(scene.thumbnail, content);
                                    }

                                    item.Visible = false;
                                    item.Visible = true;
                                }
                            }

                            if (null != jigsaw_lst && jigsaw_lst.Count > 0)
                            {
                                m_grid_jigsaw.Visible = true;
                                m_grid_jigsaw.EnsureSize<ClueJigsawItem>(jigsaw_lst.Count);

                                for (int i = 0; i < m_grid_jigsaw.ChildCount; ++i)
                                {
                                    ClueJigsawItem item = m_grid_jigsaw.GetChild<ClueJigsawItem>(i);


                                    ConfJigsawScene scene = ConfJigsawScene.Get(jigsaw_lst[i].m_id);
                                    string content = LocalizeModule.Instance.GetString(scene.name);
                                    item.Refresh(scene.thumbnail, content);

                                    item.Visible = false;
                                    item.Visible = true;
                                }
                            }
                        }
                    }

                    public bool HasContent { get; private set; } = false;

                    private void SetDestPosToChapterButton(GameUIComponent ui_root)
                    {
                        TweenPosition[] poses = ui_root.gameObject.GetComponents<TweenPosition>();

                        foreach (var item in poses)
                        {
                            if (item.Delay > 2.0f)
                            {
                                item.To = GameEntryUILogic.S_UI_CHAPTER_BTN_POS;
                                break;
                            }
                        }
                    }
                }

                /// <summary>
                /// 关键无证
                /// </summary>
                private class TaskKeyClueComponent : GameUIComponent
                {
                    private GameTexture m_clue_tex;
                    private GameLabel m_case_num_txt;
                    private GameLabel m_case_name;
                    private GameTexture m_case_icon;
                    private GameProgressBar m_task_progress_slider;
                    private GameLabel m_task_progress_txt;

                    public bool HasContent { get; private set; } = false;

                    protected override void OnInit()
                    {
                        base.OnInit();

                        m_clue_tex = Make<GameTexture>("KeyClueImg");
                        m_case_num_txt = Make<GameLabel>("Image:Text (2)");
                        m_case_name = Make<GameLabel>("Image:Text (1)");
                        m_case_icon = Make<GameTexture>("Image:RawImage (2)");
                        m_task_progress_slider = Make<GameProgressBar>("Image:Slider");
                        m_task_progress_txt = Make<GameLabel>("Image:Slider:Text");
                    }

                    public void Refresh(List<RewardClueDataWarp> clueList, NormalTask task_)
                    {
                        if (clueList.Count == 0)
                        {
                            HasContent = false;
                            return;
                        }

                        HasContent = false;

                        for (int i = 0; i < clueList.Count; ++i)
                        {
                            RewardClueDataWarp item_data = clueList[i];

                            if (TaskClueMode.JIGSAW != item_data.m_clueType)
                            {
                                continue;
                            }

                            m_clue_tex.TextureName = item_data.m_png;

                            ConfChapter c_data = GlobalInfo.MY_PLAYER_INFO.PlayerChapterSystem.CurrentChapterInfo.ChapterConfData;

                            //m_case_num_txt.Text = $"{LocalizeModule.Instance.GetString("task_name_50087")} No.{ c_data.taskIds.ToList<long>().IndexOf(task_.TaskConfID)} !";
                            m_case_num_txt.Text = $"{LocalizeModule.Instance.GetString("task_name_50087")} No.{c_data.id} !";
                            m_case_name.Text = LocalizeModule.Instance.GetString(c_data.name);
                            m_case_icon.TextureName = c_data.cover;
                            float progress_value = GlobalInfo.MY_PLAYER_INFO.PlayerChapterSystem.CurrentChapterInfo.taskProgressValue;
                            m_task_progress_slider.Value = progress_value / 100.0f;
                            m_task_progress_txt.Text = $"{progress_value}%";

                            HasContent = true;

                            break;

                        }

                    }
                }


                private class TaskRewardTitleComponent : GameUIComponent
                {
                    private GameLabel m_lbTitleName = null;
                    private long m_rewardTitleID = 0;
                    protected override void OnInit()
                    {
                        this.m_lbTitleName = Make<GameLabel>("Text (1)");
                    }

                    public void SetTitle(long titleID)
                    {
                        if (titleID != 0)
                            HasContent = true;

                        this.m_rewardTitleID = titleID;
                        this.m_lbTitleName.Text = LocalizeModule.Instance.GetString(ConfTitle.Get(titleID).name);
                    }

                    public bool HasContent { get; private set; } = false;
                }
            }

            /// <summary>
            /// 收集性任务任务项
            /// </summary>
            private class CollectionTaskDetailComponent : GameUIComponent
            {
                private GameLabel m_lbTaskName = null;
                private GameLabel m_lbTaskDesc = null;
                private GameLabel m_lbCollectItems = null;
                private GameLabel m_lbCollectText = null;
                private GameImage m_output_icon = null;
                private GameLabel m_exp_txt = null;

                private GameLabel m_site_txt;
                private GameUIContainer m_collectItemContainer = null;
                private NormalTask m_taskInfo = null;
                private GameButton m_close_btn = null;
                //private GameButton m_ok_btn = null;

                private GameTexture m_task_bg;
                private GameTexture m_task_role_tex;

                private GameUIContainer m_nav_grid;

                private Action<long> m_close_act = null;
                protected override void OnInit()
                {
                    base.OnInit();
                    this.m_lbTaskName = Make<GameLabel>("Panel_02:Text_title");
                    this.m_lbTaskDesc = Make<GameLabel>("Panel_02:Text_detail");
                    this.m_site_txt = Make<GameLabel>("Panel_02:Text");
                    this.m_collectItemContainer = Make<GameUIContainer>("Panel_02:Image_bg_1:Viewport");
                    this.m_lbCollectText = Make<GameLabel>("Panel_02:Text_title (1)");
                    this.m_output_icon = Make<GameImage>("Panel_02:Image:Image");
                    this.m_exp_txt = Make<GameLabel>("Panel_02:Image:Text_Exp");
                    m_close_btn = Make<GameButton>("Panel_02:Button_close");
                    //m_ok_btn = Make<GameButton>("Panel_02:Btn-OK");
                    m_task_bg = Make<GameTexture>("Panel_02:background");
                    m_task_role_tex = Make<GameTexture>("Panel_02:RawImage");
                    m_nav_grid = Make<GameUIContainer>("Panel_02:NavGrid");
                }

                public override void OnShow(object param)
                {
                    base.OnShow(param);
                    m_close_btn.AddClickCallBack(OnCloseClicked);
                    //m_ok_btn.AddClickCallBack(OnCloseClicked);
                }

                public override void OnHide()
                {
                    base.OnHide();
                    m_close_btn.RemoveClickCallBack(OnCloseClicked);
                    //m_ok_btn.RemoveClickCallBack(OnCloseClicked);
                }

                private void OnCloseClicked(GameObject obj_)
                {
                    this.Visible = false;
                    if (null != m_close_act)
                        m_close_act(this.m_taskInfo.TaskConfID);
                }

                public void SetTask(NormalTask taskInfo, Action<long> close_act_)
                {
                    this.m_taskInfo = taskInfo;
                    m_close_act = close_act_;
                    ConfTask confTask = ConfTask.Get(this.m_taskInfo.TaskConfID);
                    this.m_lbTaskDesc.Text = LocalizeModule.Instance.GetString(confTask.descs);
                    this.m_lbTaskName.Text = LocalizeModule.Instance.GetString(confTask.name);
                    this.m_lbCollectText.Text = LocalizeModule.Instance.GetString("activity_drop_taskdec_1");

                    if (3 == confTask.type)
                    {
                        long[] rewardItemIds = confTask.rewardPropIds;

                        if (rewardItemIds.Length > 0)
                        {
                            long id = rewardItemIds[0];
                            m_output_icon.Visible = true;
                            m_output_icon.Sprite = ConfProp.Get(id).icon;
                        }
                        else
                        {
                            m_output_icon.Visible = false;
                        }

                        this.m_exp_txt.Visible = false;
                        GameEvents.UI_Guid_Event.OnpenComponentByName.SafeInvoke("Panel_Taskdetail");
                    }
                    else
                    {
                        m_output_icon.Sprite = "icon_mainpanel_exp_2.png";
                        m_output_icon.Visible = true;
                        this.m_exp_txt.Visible = true;
                        this.m_exp_txt.Text = $"X{confTask.rewardExp}";
                    }
                    m_task_bg.TextureName = !string.IsNullOrEmpty(confTask.loopBackIcon) ? confTask.loopBackIcon : "image_task_1.png";
                    m_task_role_tex.TextureName = !string.IsNullOrEmpty(confTask.loopManIcon) ? confTask.loopManIcon : "image_suspect_15wenniwalun_size2.png";

                    this.m_site_txt.Visible = false;

                    if (1 == taskInfo.CompleteConditionList.Count)
                    {
                        if (taskInfo.CompleteConditionList[0].GetType() == typeof(TaskCompleteItems))
                        {
                            TaskCompleteItems collectDataInfo = taskInfo.CompleteConditionList[0] as TaskCompleteItems;
                            List<ItemWrapper> collectItemList = collectDataInfo.TaskCompleteData;

                            this.m_collectItemContainer.EnsureSize<CollectItemComponent>(collectItemList.Count);

                            for (int i = 0; i < this.m_collectItemContainer.ChildCount; ++i)
                            {
                                CollectItemComponent collectItemComponent = this.m_collectItemContainer.GetChild<CollectItemComponent>(i);
                                ItemWrapper itemWrapper = collectItemList[i];
                                collectItemComponent.SetCollectItemInfo(itemWrapper);
                                collectItemComponent.Visible = true;
                            }


                        }

                    }
                    else if (taskInfo.CompleteConditionList.Count > 1)
                    {
                        TaskCompleteCondition c_items = this.m_taskInfo.CompleteConditionList.Find((item) => item.GetType() == typeof(TaskCompleteItems));

                        if (null != c_items)
                        {
                            TaskCompleteItems collectDataInfo = (TaskCompleteItems)c_items;
                            List<ItemWrapper> collectItemList = collectDataInfo.TaskCompleteData;

                            this.m_collectItemContainer.EnsureSize<CollectItemComponent>(collectItemList.Count);

                            for (int i = 0; i < this.m_collectItemContainer.ChildCount; ++i)
                            {
                                CollectItemComponent collectItemComponent = this.m_collectItemContainer.GetChild<CollectItemComponent>(i);
                                ItemWrapper itemWrapper = collectItemList[i];
                                collectItemComponent.SetCollectItemInfo(itemWrapper);
                                collectItemComponent.Visible = true;
                            }

                        }
                    }

                    if (confTask.sceneids.Length > 0)
                        this.m_site_txt.Visible = true;

                    m_nav_grid.EnsureSize<ButtonTextItem>(confTask.sceneids.Length);

                    for (int i = 0; i < m_nav_grid.ChildCount; ++i)
                    {
                        var item = m_nav_grid.GetChild<ButtonTextItem>(i);
                        long scene_id = confTask.sceneids[i];
                        string scene_name = LocalizeModule.Instance.GetString(ConfScene.Get(scene_id).name);
                        item.Refresh(scene_id, scene_name, EnterGame);
                        item.Visible = true;
                    }
                }

                private void EnterGame(long scene_id_)
                {
                    ChapterInfo info = GlobalInfo.MY_PLAYER_INFO.PlayerChapterSystem.FindChapterByTaskID(m_taskInfo.TaskConfID);

                    CommonHelper.OpenEnterGameSceneUI(scene_id_, info, m_taskInfo.TaskConfID);

                    this.Visible = false;
                }


                private class CollectItemComponent : GameUIComponent
                {
                    private GameImage m_imgItemIcon = null;
                    private GameLabel m_lbItemNum = null;
                    private GameLabel m_lbItemName = null;
                    private GameProgressBar m_slider = null;
                    private ItemWrapper m_collectItemInfo = null;

                    protected override void OnInit()
                    {
                        this.m_imgItemIcon = Make<GameImage>("item");
                        this.m_lbItemNum = m_imgItemIcon.Make<GameLabel>("Text (1)");
                        this.m_lbItemName = m_imgItemIcon.Make<GameLabel>("Text");
                        this.m_slider = m_imgItemIcon.Make<GameProgressBar>("Slider");
                    }


                    public void SetCollectItemInfo(ItemWrapper taskCollectItems)
                    {
                        m_collectItemInfo = taskCollectItems;
                        string icon = string.Empty;
                        string itemName = string.Empty;
                        if (taskCollectItems.ItemID >= 9999)
                        {
                            Confexhibit exhibitConfig = Confexhibit.Get(taskCollectItems.ItemID);
                            icon = exhibitConfig.iconName;
                            itemName = LocalizeModule.Instance.GetString(exhibitConfig.name);
                        }
                        else
                        {
                            ConfProp itemConfig = ConfProp.Get(taskCollectItems.ItemID);
                            icon = itemConfig.icon;
                            itemName = LocalizeModule.Instance.GetString(itemConfig.name);
                        }

                        this.m_imgItemIcon.Sprite = icon;
                        this.m_lbItemName.Text = itemName;

                        //string collectNumTextFormat = "<color={0}>" + taskCollectItems.CurrentItemNum + "</color><color=#ffffff> / " + taskCollectItems.ItemNum + "</color>";
                        //this.m_lbItemNum.Text = IsCollectFinish ? string.Format(collectNumTextFormat, "#5aff81") : string.Format(collectNumTextFormat, "#ff5a5a");
                        this.m_lbItemNum.Text = $"{taskCollectItems.CurrentItemNum} / {taskCollectItems.ItemNum}";
                        float s = (float)taskCollectItems.CurrentItemNum / (float)taskCollectItems.ItemNum;
                        m_slider.Value = s;
                    }

                    public bool IsCollectFinish
                    {
                        get { return this.m_collectItemInfo.CurrentItemNum >= this.m_collectItemInfo.ItemNum; }
                    }
                }

            }

        }
    }
}
