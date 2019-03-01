/********************************************************************
	created:  2018-5-14 11:32:43
	filename: TaskSystem.cs
	author:	  songguangze@outlook.com
	
	purpose:  任务系统
*********************************************************************/

using EngineCore;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

namespace SeekerGame
{
    public class TaskSystem : IDisposable
    {
        public const long C_FIRST_TASK_ID = 10001;

        private PlayerInfo m_playerInfo = null;
        private TaskSet m_playerTaskSet = null;

        //当前正在进行的Task
        private List<TaskBase> m_currentTaskList = new List<TaskBase>();

        private long m_queryIsCompletedTaskID = -1;
        private Action<bool> QueryIsTaskCompletedCallback = null;

        public TaskSystem(PlayerInfo playerInfo)
        {
            this.m_playerInfo = playerInfo;
            this.m_playerTaskSet = new TaskSet();

            MessageHandler.RegisterMessageHandler(MessageDefine.SCTaskIdListResponse, OnSyncPlayerTaskList);
            MessageHandler.RegisterMessageHandler(MessageDefine.SCAcceptTaskNotice, OnAcceptNewTasks);
            MessageHandler.RegisterMessageHandler(MessageDefine.SCTaskListResponse, OnSyncTaskProgress);
            MessageHandler.RegisterMessageHandler(MessageDefine.SCTaskCommitResponse, OnCommitTask);
            MessageHandler.RegisterMessageHandler(MessageDefine.SCTaskStatusChangeNotice, OnTaskStatusChanged);
            MessageHandler.RegisterMessageHandler(MessageDefine.SCGetAllRewardedTasksResponse, OnSyncCompletedTaskList);
            MessageHandler.RegisterMessageHandler(MessageDefine.SCChatFinishResponse, OnRes);
            MessageHandler.RegisterMessageHandler(MessageDefine.SCCartoonEnterResponse, OnRes);
            MessageHandler.RegisterMessageHandler(MessageDefine.SCCartoonRewardReqsponse, OnRes);
            MessageHandler.RegisterMessageHandler(MessageDefine.SCCombineResponse, OnRes);

            GameEvents.TaskEvents.OnAcceptNewTaskLocal += OnAcceptNewTasksByLocalData;
        }


        public bool IsCompleteTask(long taskUUID)
        {
            TaskBase task = this.m_playerTaskSet.GetTaskById(taskUUID);
            if (task != null)
                return task.TaskCurrentStatus == TaskStatus.REWARDED;

            return false;
        }

        /// <summary>
        /// 是否完成任务
        /// </summary>
        /// <param name="taskConfigID">配置ID</param>
        /// <returns></returns>
        public void IsCompleteTaskByConfigID(long taskConfigID, Action<bool> queryResultCallback)
        {
            this.m_queryIsCompletedTaskID = taskConfigID;
            this.QueryIsTaskCompletedCallback = queryResultCallback;

            if (!TaskQueryManager.Instance.IsNeedUpdate())
            {
                QueryIsTaskCompletedCallback?.Invoke(TaskQueryManager.Instance.QueryTaskComplete(this.m_queryIsCompletedTaskID));
                QueryIsTaskCompletedCallback = null;
            }
            else
            {
                CSGetAllRewardedTasksRequest requestCompletedTask = new CSGetAllRewardedTasksRequest();
                GameEvents.NetWorkEvents.SendMsg.SafeInvoke(requestCompletedTask);
            }
        }

        /// <summary>
        /// 恢复玩家任务数据
        /// </summary>
        public void RetrievePlayerTaskList()
        {
            CSTaskIdListRequest request = new CSTaskIdListRequest();
            GameEvents.NetWorkEvents.SendMsg.SafeInvoke(request);
        }

        /// <summary>
        /// 恢复玩家任务数据(基本数据)
        /// </summary>
        /// <param name="message"></param>
        private void OnSyncPlayerTaskList(object message)
        {
            SCTaskIdListResponse retrievedPlayerTaskList = message as SCTaskIdListResponse;

            for (int i = 0; i < retrievedPlayerTaskList.TaskIdInfos.Count; ++i)
            {
                TaskIdInfo retrieveTaskInfo = retrievedPlayerTaskList.TaskIdInfos[i];
                NormalTask task = CreateNormalTaskInstance(retrieveTaskInfo.TaskId);
                task.TaskUUID = retrieveTaskInfo.PlayerTaskId;

                this.m_playerTaskSet.AddTask(retrieveTaskInfo.PlayerTaskId, task);
            }

            List<long> taskIdList = retrievedPlayerTaskList.TaskIdInfos.Select(x => x.PlayerTaskId).ToList();

            SyncTaskDetailInfo(0, taskIdList.ToArray());
        }

        /// <summary>
        /// 请求同步任务详细信息
        /// </summary>
        /// <param name="taskType">0-所有任务  1-主线任务</param>
        /// <param name="taskUUIDList"></param>
        public void SyncTaskDetailInfo(int taskType, params long[] taskUUIDList)
        {
            IList<long> requestSyncTaskUUID = taskUUIDList.ToList();
            CSTaskListRequest req = new CSTaskListRequest();
            req.Type = taskType;
            req.TaskIds.AddRange(requestSyncTaskUUID);

            GameEvents.NetWorkEvents.SendMsg.SafeInvoke(req);
        }


        /// <summary>
        /// 任务状态改变 
        /// </summary>
        /// <param name="msg"></param>
        private void OnTaskStatusChanged(object msg)
        {
            SCTaskStatusChangeNotice message = msg as SCTaskStatusChangeNotice;
            TaskBase task = this.m_playerTaskSet.GetTaskById(message.PlayerTaskId);
            if (task != null)
            {
                //TODO:防止任务状态同步消息，未按指定顺序到达。完成的任务，不能在重置为进行中。
                task.TaskCurrentStatus = message.Status > (int)(task.TaskCurrentStatus) ? (TaskStatus)message.Status : task.TaskCurrentStatus;

                if (task.TaskCurrentStatus == TaskStatus.REWARDED)
                {
                    Dictionary<UBSParamKeyName, object> _param = new Dictionary<UBSParamKeyName, object>();
                    _param.Add(UBSParamKeyName.ContentID, task.TaskConfID);
                    UserBehaviorStatisticsModules.Instance.LogEvent(UBSEventKeyName.get_task_reward, null, _param);

                    this.m_currentTaskList.Remove(task);
                    GameEvents.TaskEvents.OnReceiveTask.SafeInvoke(task);
                    BigWorldManager.Instance.OnBigWorldCanUnLock(task.TaskConfID);
                }
                else if (task.TaskCurrentStatus == TaskStatus.PROGRESSING)
                {
                    OnSyncTaskProgress(message.TaskInfo);
                }
                else if (task.TaskCurrentStatus == TaskStatus.COMPLETED)
                {
                    FindObjSceneDataManager.RemoveSceneIDForTask(task.TaskConfID);
                }


                GameEvents.TaskEvents.OnSyncedTaskList.SafeInvoke(this.m_currentTaskList.Count);
            }
        }

        /// <summary>
        /// 同步任务详细数据
        /// </summary>
        /// <param name="message"></param>
        private void OnSyncTaskProgress(object message)
        {
            SCTaskListResponse msg = message as SCTaskListResponse;

            HashSet<long> currentProgressingTaskIDs = new HashSet<long>();

            for (int i = 0; i < msg.TaskInfos.Count; ++i)
            {
                long taskUUID = msg.TaskInfos[i].PlayerTaskId;

                TaskBase task = this.m_playerTaskSet.GetTaskById(taskUUID);
                if (task != null)
                {
                    task.SyncTaskInfo(msg.TaskInfos[i]);

                    if (task.TaskCurrentStatus == TaskStatus.PROGRESSING || task.TaskCurrentStatus == TaskStatus.COMPLETED)
                    {
                        if (task.TaskCurrentStatus == TaskStatus.PROGRESSING)
                            currentProgressingTaskIDs.Add(task.TaskConfID);

                        if (!this.m_currentTaskList.Contains(task))
                            this.m_currentTaskList.Add(task);
                    }
                }
            }

            if (currentProgressingTaskIDs.Count > 0)
                FindObjSceneDataManager.RefreshSceneIDsForTasks(currentProgressingTaskIDs);

            if (this.m_currentTaskList.Count > 0)
            {
                GameEvents.TaskEvents.OnSyncedTaskList.SafeInvoke(this.m_currentTaskList.Count);
            }
        }

        private void OnSyncTaskProgress(TaskInfo info_)
        {
            long taskUUID = info_.PlayerTaskId;

            TaskBase task = this.m_playerTaskSet.GetTaskById(taskUUID);
            if (task != null)
            {
                task.SyncTaskInfo(info_);
            }
        }

        /// <summary>
        /// 接收新的任务
        /// </summary>
        /// <param name="message"></param>
        private void OnAcceptNewTasks(object message)
        {
#if true
            TimeModule.Instance.SetTimeout(
                () =>
            {

                SCAcceptTaskNotice msg = message as SCAcceptTaskNotice;

                IList<AcceptTaskInfo> acceptTaskInfos = msg.AcceptTasks;
                for (int i = 0; i < acceptTaskInfos.Count; ++i)
                {
                    AcceptTaskInfo taskInfo = acceptTaskInfos[i];
                    long taskUUID = taskInfo.PlayerTaskId;

                    this.m_currentTaskList.RemoveAll(item => item.TaskConfID == taskInfo.TaskId && TaskSyncStatus.LOCAL == item.TaskSyncStatus);

                    TaskBase task = m_playerTaskSet.GetTaskById(taskUUID);
                    if (task == null)
                    {
                        task = CreateNormalTaskInstance(taskInfo.TaskId);
                        task.TaskUUID = taskUUID;
                        m_playerTaskSet.AddTask(taskUUID, task);
                    }

                    task.TaskCurrentStatus = (TaskStatus)taskInfo.Status;
                    task.TaskSyncStatus = TaskSyncStatus.SYNCED;

                    if (this.m_currentTaskList.Where(item => item.TaskUUID == task.TaskUUID).Count() > 0)
                        Debug.LogWarning($"accept new task duplicate task{task.TaskUUID}");
                    else
                    {
                        this.m_currentTaskList.Add(task);

                        GameEvents.TaskEvents.OnAcceptNewTask.SafeInvoke(taskUUID);
                        GameEvents.TaskEvents.OnSyncedTaskList.SafeInvoke(1);
                    }
                }
            }, 3.0f);
#else
            SCAcceptTaskNotice msg = message as SCAcceptTaskNotice;

            IList<AcceptTaskInfo> acceptTaskInfos = msg.AcceptTasks;
            for (int i = 0; i < acceptTaskInfos.Count; ++i)
            {
                AcceptTaskInfo taskInfo = acceptTaskInfos[i];
                long taskUUID = taskInfo.PlayerTaskId;

                this.m_currentTaskList.RemoveAll(item => item.TaskConfID == taskInfo.TaskId && TaskSyncStatus.LOCAL == item.TaskSyncStatus);

                TaskBase task = m_playerTaskSet.GetTaskById(taskUUID);
                if (task == null)
                {
                    task = CreateNormalTaskInstance(taskInfo.TaskId);
                    task.TaskUUID = taskUUID;
                    m_playerTaskSet.AddTask(taskUUID, task);
                }

                task.TaskCurrentStatus = (TaskStatus)taskInfo.Status;
                task.TaskSyncStatus = TaskSyncStatus.SYNCED;

                if (this.m_currentTaskList.Where(item => item.TaskUUID == task.TaskUUID).Count() > 0)
                    Debug.LogWarning($"accept new task duplicate task{task.TaskUUID}");
                else
                {
                    this.m_currentTaskList.Add(task);

                    GameEvents.TaskEvents.OnAcceptNewTask.SafeInvoke(taskUUID);
                    GameEvents.TaskEvents.OnSyncedTaskList.SafeInvoke(1);
                }
            }
#endif
        }


        private void OnAcceptNewTasksByLocalData(long task_config_id_)
        {

            if (this.m_currentTaskList.Where(item => item.TaskConfID == task_config_id_).Count() > 0)
                return;

            TaskBase task = CreateNormalTaskInstance(task_config_id_);
            task.TaskUUID = -1L;

            //m_playerTaskSet.AddTask(task.TaskUUID, task);

            task.TaskCurrentStatus = TaskStatus.PROGRESSING;
            task.TaskSyncStatus = TaskSyncStatus.LOCAL;

            this.m_currentTaskList.Add(task);

            GameEvents.TaskEvents.OnAcceptNewTask.SafeInvoke(task.TaskUUID);
            GameEvents.TaskEvents.OnSyncedTaskList.SafeInvoke(1);


        }

        /// <summary>
        /// 创建任务实体
        /// </summary>
        /// <param name="taskId"></param>
        /// <returns></returns>
        private NormalTask CreateNormalTaskInstance(long taskId)
        {
            ConfTask confTask = ConfTask.Get(taskId);

            if (confTask == null)
            {
                Debug.LogError($"create task error, no task config :{taskId}");
                return null;
            }

            List<TaskCreaterParams> taskTriggerConditionParams = ParseTaskTriggerConditions(confTask);
            List<TaskCreaterParams> taskCompleteConditionParams = ParseTaskCompleteConditions(confTask);
            List<TaskCreaterParams> taskRewardParams = ParseTaskRewardInfo(confTask);

            NormalTask task = TaskFactory.Instance.CreateTask<NormalTask>(confTask.id, taskTriggerConditionParams, taskCompleteConditionParams, taskRewardParams);
            task.TaskData = confTask;

            return task;
        }


        /// <summary>
        /// 交任务相应
        /// </summary>
        /// <param name="message"></param>
        private void OnCommitTask(object message)
        {
            SCTaskCommitResponse msg = message as SCTaskCommitResponse;

            if (!MsgStatusCodeUtil.OnError(msg.Result))
            {
                TaskBase commitTaskInfo = this.m_playerTaskSet.GetTaskById(msg.TaskId);
                if (commitTaskInfo == null)
                    Debug.LogError($"commit task :{msg.TaskId} not found");
                GameEvents.TaskEvents.OnReceiveTask.SafeInvoke(commitTaskInfo);

                GameEvents.PlayerEvents.OnExpChanged.SafeInvoke(null, msg.Exp);

                commitTaskInfo.TaskCurrentStatus = TaskStatus.REWARDED;
                commitTaskInfo.TaskSyncStatus = TaskSyncStatus.SYNCED;

                if (this.m_currentTaskList.Contains(commitTaskInfo))
                    this.m_currentTaskList.Remove(commitTaskInfo);

                //同步玩家最新信息
                GameEvents.PlayerEvents.RequestLatestPlayerInfo.SafeInvoke();

                //同步背包
                //GlobalInfo.MY_PLAYER_INFO.SyncPlayerBag();
                NormalTask taskInfo = commitTaskInfo as NormalTask;
                for (int i = 0; i < taskInfo.RewardList.Count; ++i)
                {
                    TaskRewardBase taskReward = taskInfo.RewardList[i];
                    TaskRewardMode taskRewardType = (taskReward.GetType().GetCustomAttributes(typeof(TaskRewardAttribute), true)[0] as TaskRewardAttribute).RewardMode;

                    if (TaskRewardMode.ITEM == taskRewardType)
                    {

                        TaskRewardItem rewardItem = taskReward as TaskRewardItem;
                        RewardItemDataWrap rewardItemData = rewardItem.RewardData;

                        if (RewardItemType.ITEM == rewardItemData.ItemType)
                        {
                            GlobalInfo.MY_PLAYER_INFO.AddSingleBagInfo(rewardItemData.ItemID, rewardItemData.ItemNum);
                        }
                    }
                }

                GameEvents.UIEvents.UI_GameEntry_Event.Listen_OnCombinePropCollected.SafeInvoke();
                //>

                BigWorldManager.Instance.EnterBigWorld();
                GameEvents.TaskEvents.OnCompletedTask.SafeInvoke(msg.Result, commitTaskInfo);

                HttpPingModule.Instance.SendPing();

                //TODO : 放在主界面onshow
                //if (C_FIRST_TASK_ID == commitTaskInfo.TaskConfID)
                //{
                //    SignInManager sm = new SignInManager();
                //}


            }
            else
                GameEvents.TaskEvents.OnSyncedTaskList.SafeInvoke(1);
        }

        private void OnSyncCompletedTaskList(object msg)
        {
            TaskQueryManager.Instance.AllCompleteTask.Clear();
            SCGetAllRewardedTasksResponse res = msg as SCGetAllRewardedTasksResponse;
            for (int i = 0; i < res.Tasks.Count; i++)
            {
                TaskQueryManager.Instance.AddCompleteTask(res.Tasks[i].TaskId, res.Tasks[i].TaskType);
            }

            QueryIsTaskCompletedCallback?.Invoke(TaskQueryManager.Instance.QueryTaskComplete(this.m_queryIsCompletedTaskID));
            QueryIsTaskCompletedCallback = null;
        }

        private void OnRes(object obj)
        {
            if (obj is SCChatFinishResponse)
            {
                SCChatFinishResponse res = (SCChatFinishResponse)obj;
                if (res.Status == null)
                {
                    var req = EngineCoreEvents.SystemEvents.GetRspPairReq.SafeInvoke();
                    CSChatFinishRequest req_msg = req as CSChatFinishRequest;
                    long chat_id = req_msg.ChatId;

                    foreach (var task in m_currentTaskList)
                    {
                        NormalTask taskInfo = task as NormalTask;

                        if (taskInfo.CompleteConditionList.Count > 1 || 0 == taskInfo.CompleteConditionList.Count)
                            continue;

                        TaskCompleteCondition taskCompleteCondition = taskInfo.CompleteConditionList[0];
                        TaskCompleteAttribute taskCompleteAttribute = taskCompleteCondition.GetType().GetCustomAttributes(typeof(TaskCompleteAttribute), true)[0] as TaskCompleteAttribute;

                        if (TaskCompleteMode.CompletedByDialog != taskCompleteAttribute.CompleteMode)
                            continue;

                        long task_dialog_id = (long)taskCompleteCondition.TaskCompleteData;

                        if (chat_id != task_dialog_id)
                            continue;

                        taskInfo.TaskCurrentStatus = TaskStatus.COMPLETED;

                    }

                    GameEvents.TaskEvents.OnSyncedTaskList.SafeInvoke(this.m_currentTaskList.Count);
                }
            }
            else if (obj is SCCombineResponse)
            {
                SCCombineResponse res = (SCCombineResponse)obj;
                if (res.Success)
                {
                    var req = EngineCoreEvents.SystemEvents.GetRspPairReq.SafeInvoke();
                    CSCombineRequest req_msg = req as CSCombineRequest;
                    long combine_id = req_msg.CombineId;
                    long prop_id = ConfCombineFormula.Get(combine_id).outputId;
                    bool tsk_finished = false;
                    foreach (var task in m_currentTaskList)
                    {
                        NormalTask taskInfo = task as NormalTask;

                        if (taskInfo.CompleteConditionList.Count > 1 || 0 == taskInfo.CompleteConditionList.Count)
                            continue;

                        TaskCompleteCondition taskCompleteCondition = taskInfo.CompleteConditionList[0];
                        TaskCompleteAttribute taskCompleteAttribute = taskCompleteCondition.GetType().GetCustomAttributes(typeof(TaskCompleteAttribute), true)[0] as TaskCompleteAttribute;

                        if (TaskCompleteMode.CompleteByCombinePropID != taskCompleteAttribute.CompleteMode)
                            continue;
                        long[] all_tsk_combine_prop_id = (long[])taskCompleteCondition.TaskCompleteData;

                        if (0 == all_tsk_combine_prop_id.Length)
                        {
                            Debug.LogError($"合成任务{taskInfo.TaskConfID}没有配置完成物件");
                            continue;
                        }

                        long task_combine_prop_id = all_tsk_combine_prop_id[0];

                        if (prop_id != task_combine_prop_id)
                            continue;

                        tsk_finished = true;
                        taskInfo.TaskCurrentStatus = TaskStatus.COMPLETED;

                    }
                    if (tsk_finished)
                    {
                        GameEvents.TaskEvents.OnSyncedTaskList.SafeInvoke(this.m_currentTaskList.Count);
                        EngineCoreEvents.UIEvent.HideUIEvent.SafeInvoke(UIDefine.UI_COMBINE);
                    }
                }
            }
            else if (obj is SCCartoonEnterResponse)
            {


                SCCartoonEnterResponse res = obj as SCCartoonEnterResponse;
                if (!MsgStatusCodeUtil.OnError(res.Result))
                {
                    var req = EngineCoreEvents.SystemEvents.GetRspPairReq.SafeInvoke();
                    CSCartoonEnterRequest req_msg = req as CSCartoonEnterRequest;
                    long cur_scene_id = req_msg.SceneId;

                    foreach (var task in m_currentTaskList)
                    {
                        NormalTask taskInfo = task as NormalTask;

                        if (taskInfo.CompleteConditionList.Count > 1 || 0 == taskInfo.CompleteConditionList.Count)
                            continue;

                        TaskCompleteCondition taskCompleteCondition = taskInfo.CompleteConditionList[0];
                        TaskCompleteAttribute taskCompleteAttribute = taskCompleteCondition.GetType().GetCustomAttributes(typeof(TaskCompleteAttribute), true)[0] as TaskCompleteAttribute;

                        if (TaskCompleteMode.CompletedBySceneID != taskCompleteAttribute.CompleteMode)
                            continue;

                        long scene_id = (long)taskCompleteCondition.TaskCompleteData;

                        if (CommonData.C_CARTOON_SCENE_START_ID != scene_id / CommonData.C_SCENE_TYPE_ID)
                            continue;

                        if (cur_scene_id != scene_id)
                            continue;

                        taskInfo.TaskCurrentStatus = TaskStatus.COMPLETED;

                    }
                }

            }
            else if (obj is SCCartoonRewardReqsponse)
            {
                GameEvents.TaskEvents.OnSyncedTaskList.SafeInvoke(this.m_currentTaskList.Count);
            }
        }

        #region Task Creater Params Parser
        /// <summary>
        /// 解析任务配置-任务触发条件
        /// </summary>
        /// <param name="taskConfig"></param>
        /// <returns></returns>
        private List<TaskCreaterParams> ParseTaskTriggerConditions(ConfTask taskConfig)
        {
            List<TaskCreaterParams> taskTriggerCreaterParamList = new List<TaskCreaterParams>();
            TaskCreaterParams triggerParam = new TaskCreaterParams() { Condition = taskConfig.triggerKey, Data = taskConfig.triggerValue };

            taskTriggerCreaterParamList.Add(triggerParam);

            return taskTriggerCreaterParamList;
        }

        /// <summary>
        /// 解析任务配置-任务完成条件
        /// </summary>
        /// <param name="taskConfig"></param>
        /// <returns></returns>
        private List<TaskCreaterParams> ParseTaskCompleteConditions(ConfTask taskConfig)
        {
            List<TaskCreaterParams> taskCompleteCondition = new List<TaskCreaterParams>();

            if (taskConfig.conditionLevel > 0)
                taskCompleteCondition.Add(new TaskCreaterParams() { Condition = (int)TaskCompleteMode.CompleteByLevel, Data = taskConfig.conditionLevel });

            if (taskConfig.conditionSceneId > 0)
                taskCompleteCondition.Add(new TaskCreaterParams() { Condition = (int)TaskCompleteMode.CompletedBySceneID, Data = taskConfig.conditionSceneId });

            if (taskConfig.conditionDialogueId > 0)
                taskCompleteCondition.Add(new TaskCreaterParams() { Condition = (int)TaskCompleteMode.CompletedByDialog, Data = taskConfig.conditionDialogueId });

            if (taskConfig.conditionReasoningId > 0)
                taskCompleteCondition.Add(new TaskCreaterParams() { Condition = (int)TaskCompleteMode.CompleteByReasonID, Data = taskConfig.conditionReasoningId });

            if (taskConfig.conditionPropExIds.Length > 0)
            {
                long[] itemIds = taskConfig.conditionPropExIds;

                TaskCreaterParams taskCreaterParams = new TaskCreaterParams() { Condition = (int)TaskCompleteMode.CompleteByCombinePropID, Data = itemIds };
                taskCompleteCondition.Add(taskCreaterParams);
            }

            //if (taskConfig.condition > 0)
            //    taskCompleteCondition.Add(new TaskCreaterParams() { Condition = (int)TaskCompleteMode.CompleteByReasonID, Data = taskConfig.conditionReasoningId });

            if (taskConfig.conditionFindId > 0)
                taskCompleteCondition.Add(new TaskCreaterParams() { Condition = (int)TaskCompleteMode.CompleteByScanID, Data = taskConfig.conditionFindId });

            //if (taskConfig.conditionEventId > 0)
            //    taskCompleteCondition.Add(new TaskCreaterParams() { Condition = (int)TaskCompleteMode.CompletedByEvents, Data = taskConfig.conditionEventId });


            if (taskConfig.conditionPropIds.Length > 0 && taskConfig.conditionPropNums.Length > 0)
            {
                long[] itemIds = taskConfig.conditionPropIds;
                long[] itemNums = new long[taskConfig.conditionPropNums.Length];
                Array.Copy(taskConfig.conditionPropNums, itemNums, itemNums.Length);

                if (itemIds.Length != itemNums.Length)
                {
                    Debug.LogWarning($"task: {taskConfig.id} error ,complete item nums error,item id:{itemIds.Length} ,item nums :{itemNums.Length}");

                    if (itemNums.Length > itemIds.Length)
                        itemNums = new ArraySegment<long>(itemNums, 0, itemIds.Length).Array;
                    else if (itemNums.Length < itemIds.Length)
                    {
                        long[] itemNumsCompatibility = new long[itemIds.Length];
                        Array.Copy(itemNums, itemNumsCompatibility, itemNums.Length);

                        for (int i = itemNums.Length - 1; i < itemIds.Length; ++i)
                            itemNumsCompatibility[i] = itemNums[itemNums.Length - 1];

                        itemNums = itemNumsCompatibility;
                    }

                }

                TaskCreaterParams taskCreaterParams = new TaskCreaterParams() { Condition = (int)TaskCompleteMode.CompletedByItem, Data = new List<long[]> { itemIds, itemNums } };
                taskCompleteCondition.Add(taskCreaterParams);
            }
            else if (taskConfig.conditionExhibits.Length > 0 && taskConfig.conditionExhibitsNum.Length > 0)
            {
                long[] itemIds = taskConfig.conditionExhibits;
                long[] itemNums = new long[taskConfig.conditionExhibitsNum.Length];
                Array.Copy(taskConfig.conditionExhibitsNum, itemNums, itemNums.Length);

                if (itemIds.Length != itemNums.Length)
                    Debug.LogError($"task: {taskConfig.id} error ,complete exhibit nums error,item id:{itemIds.Length} ,item nums :{itemNums.Length}");

                TaskCreaterParams taskCreaterParams = new TaskCreaterParams() { Condition = (int)TaskCompleteMode.CompletedByItem, Data = new List<long[]> { itemIds, itemNums } };
                taskCompleteCondition.Add(taskCreaterParams);
            }

            return taskCompleteCondition;
        }

        /// <summary>
        /// 解析任务配置-任务奖励信息
        /// </summary>
        /// <param name="taskConfig"></param>
        /// <returns></returns>
        private List<TaskCreaterParams> ParseTaskRewardInfo(ConfTask taskConfig)
        {
            List<TaskCreaterParams> taskCreaterParamList = new List<TaskCreaterParams>();

            //称号奖励
            if (taskConfig.rewardTitleId != 0)
            {
                TaskCreaterParams titleParam = new TaskCreaterParams() { Condition = (int)TaskRewardMode.TITLE, Data = taskConfig.rewardTitleId };
                taskCreaterParamList.Add(titleParam);
            }

            //体力奖励
            if (taskConfig.rewardVit != 0)
            {
                TaskCreaterParams vitParam = new TaskCreaterParams() { Condition = (int)TaskRewardMode.ITEM, Data = new RewardItemDataWrap() { ItemType = RewardItemType.VIT, ItemNum = taskConfig.rewardVit } };
                taskCreaterParamList.Add(vitParam);
            }

            //金币奖励
            if (taskConfig.rewardCoin != 0)
            {
                TaskCreaterParams coinParam = new TaskCreaterParams() { Condition = (int)TaskRewardMode.ITEM, Data = new RewardItemDataWrap() { ItemType = RewardItemType.COIN, ItemNum = taskConfig.rewardCoin } };
                taskCreaterParamList.Add(coinParam);
            }

            //现金奖励
            if (taskConfig.rewardCash != 0)
            {
                TaskCreaterParams cashParam = new TaskCreaterParams() { Condition = (int)TaskRewardMode.ITEM, Data = new RewardItemDataWrap() { ItemType = RewardItemType.CASH, ItemNum = taskConfig.rewardCash } };
                taskCreaterParamList.Add(cashParam);
            }

            //经验奖励
            if (taskConfig.rewardExp != 0)
            {
                TaskCreaterParams expParam = new TaskCreaterParams() { Condition = (int)TaskRewardMode.ITEM, Data = new RewardItemDataWrap() { ItemType = RewardItemType.EXP, ItemNum = taskConfig.rewardExp } };
                taskCreaterParamList.Add(expParam);
            }

            //long chapter_id = GlobalInfo.MY_PLAYER_INFO.PlayerChapterSystem.CurrentChapterInfo.ChapterID;

            //ConfChapter chapter_data = ConfChapter.Get(chapter_id);

            //int jigsaw_idx = chapter_data.clueUnlockTaskIds.ToList().IndexOf(taskConfig.id);

            //if (jigsaw_idx >= 0)
            //{
            //    TaskCreaterParams expParam = new TaskCreaterParams() { Condition = (int)TaskRewardMode.CLUE, Data = new RewardClueDataWarp() { m_clueType = TaskClueMode.JIGSAW, m_png = chapter_data.clueIds[jigsaw_idx] } };
            //    taskCreaterParamList.Add(expParam);
            //}

            //int npc_idx = chapter_data.actorUnlockTaskIds.ToList().IndexOf(taskConfig.id);

            //if (npc_idx >= 0)
            //{
            //    TaskCreaterParams expParam = new TaskCreaterParams() { Condition = (int)TaskRewardMode.CLUE, Data = new RewardClueDataWarp() { m_clueType = TaskClueMode.NPC, m_id = chapter_data.actorIds[npc_idx] } };
            //    taskCreaterParamList.Add(expParam);
            //}

            //int scene_idx = chapter_data.sceneUnlockTaskIds.ToList().IndexOf(taskConfig.id);

            //if (scene_idx >= 0)
            //{
            //    TaskCreaterParams expParam = new TaskCreaterParams() { Condition = (int)TaskRewardMode.CLUE, Data = new RewardClueDataWarp() { m_clueType = TaskClueMode.SCENE, m_id = chapter_data.scenceIds[scene_idx] } };
            //    taskCreaterParamList.Add(expParam);
            //}



            //道具奖励
            if (taskConfig.rewardPropIds.Length > 0 && taskConfig.rewardPropNums.Length > 0)
            {
                long[] rewardItemIds = taskConfig.rewardPropIds;
                int[] rewardItemNums = taskConfig.rewardPropNums;

                if (rewardItemIds.Length != rewardItemNums.Length)
                    Debug.LogError($"task {taskConfig.id} reward item error,item ids length {rewardItemIds.Length}, item nums length {rewardItemNums.Length}");

                for (int i = 0; i < rewardItemIds.Length; ++i)
                {
                    RewardItemDataWrap rewardItemData = new RewardItemDataWrap() { ItemNum = rewardItemNums[i], ItemID = rewardItemIds[i], ItemType = RewardItemType.ITEM };

                    taskCreaterParamList.Add(new TaskCreaterParams() { Condition = (int)TaskRewardMode.ITEM, Data = rewardItemData });
                }
            }

            return taskCreaterParamList;
        }

        public void Dispose()
        {
            MessageHandler.UnRegisterMessageHandler(MessageDefine.SCTaskIdListResponse, OnSyncPlayerTaskList);
            MessageHandler.UnRegisterMessageHandler(MessageDefine.SCAcceptTaskNotice, OnAcceptNewTasks);
            MessageHandler.UnRegisterMessageHandler(MessageDefine.SCTaskListResponse, OnSyncTaskProgress);
            MessageHandler.UnRegisterMessageHandler(MessageDefine.SCTaskCommitResponse, OnCommitTask);
            MessageHandler.UnRegisterMessageHandler(MessageDefine.SCTaskStatusChangeNotice, OnTaskStatusChanged);
            MessageHandler.UnRegisterMessageHandler(MessageDefine.SCGetAllRewardedTasksResponse, OnSyncCompletedTaskList);

            GameEvents.TaskEvents.OnAcceptNewTaskLocal -= OnAcceptNewTasksByLocalData;

        }
        #endregion


        public TaskSet PlayerTaskSet
        {
            get { return this.m_playerTaskSet; }
        }

        public List<TaskBase> CurrentTaskList => this.m_currentTaskList;
    }
}
