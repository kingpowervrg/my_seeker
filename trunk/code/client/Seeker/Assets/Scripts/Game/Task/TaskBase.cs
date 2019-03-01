/********************************************************************
	created:  2018-5-14 11:32:43
	filename: TaskBase.cs
	author:	  songguangze@outlook.com
	
	purpose:  任务基类
*********************************************************************/
using System;
using System.Collections.Generic;
using UnityEngine;

namespace SeekerGame
{
    public abstract class TaskBase
    {
        private long m_taskId = -1;
        private long m_taskUUID = -1;
        protected object m_taskData = null;

        //任务状态
        protected TaskStatus m_taskStatus = TaskStatus.NONE;

        //同步状态
        protected TaskSyncStatus m_taskSyncStatus = TaskSyncStatus.LOCAL;

        private List<TaskTriggerCondition> m_taskTriggerConditionList = new List<TaskTriggerCondition>();
        private List<TaskCompleteCondition> m_taskCompleteConditionList = new List<TaskCompleteCondition>();
        private List<TaskRewardBase> m_taskRewardList = new List<TaskRewardBase>();

        private int m_redirectionId = 0;            //跳转ID

        public TaskBase(long taskId)
        {
            this.m_taskId = taskId;
        }

        //public void PrintContent()
        //{
        //    Debug.Log(string.Format("当前任务id = {0} , uuid = {1}", this.m_taskId, this.m_taskUUID));
        //}

        /// <summary>
        /// 添加任务触发条件
        /// </summary>
        /// <param name="taskTriggerCondition"></param>
        public void AddTaskTriggerCondition(TaskTriggerCondition taskTriggerCondition)
        {
            this.m_taskTriggerConditionList.Add(taskTriggerCondition);
        }

        /// <summary>
        /// 添加 任务完成条件
        /// </summary>
        /// <param name="taskCompleteCondition"></param>
        public void AddTaskCompleteCondition(TaskCompleteCondition taskCompleteCondition)
        {
            this.m_taskCompleteConditionList.Add(taskCompleteCondition);
        }

        /// <summary>
        /// 添加任务奖励信息
        /// </summary>
        /// <param name="taskRewardInfo"></param>
        public void AddTaskRewardInfo(TaskRewardBase taskRewardInfo)
        {
            this.m_taskRewardList.Add(taskRewardInfo);
        }

        /// <summary>
        /// 同步任务的详细信息
        /// </summary>
        public void SyncTaskInfo(TaskInfo syncTaskInfo)
        {
            bool status_seted = false;

            for (int i = 0; i < this.m_taskCompleteConditionList.Count; ++i)
            {
                TaskCompleteCondition completeCondition = this.m_taskCompleteConditionList[i];
                switch ((TaskStatus)syncTaskInfo.Status)
                {
                    case TaskStatus.COMPLETED:
                    case TaskStatus.REWARDED:
                        completeCondition.TaskCompleteConditionProgress = 1f;
                        break;
                    case TaskStatus.PROGRESSING:
                        {
                            if (completeCondition is TaskCompleteByLevel)
                                completeCondition.SetCompleteProgressData(syncTaskInfo.Level);
                            else if (completeCondition is TaskCompleteByScene)
                            {
                                completeCondition.SetCompleteProgressData(syncTaskInfo.SceneProgress);
                            }
                            //else if (completeCondition is TaskCompleteByEventID)
                            //{
                            //    completeCondition.SetCompleteProgressData(syncTaskInfo.EventProgress);
                            //}
                            else if (completeCondition is TaskCompleteByDialogID)
                            {
                                //对话任务
                                //TODO:防止任务状态同步消息，未按指定顺序到达。完成的任务，不能在重置为进行中。
                                this.m_taskStatus = syncTaskInfo.Status > (int)(this.m_taskStatus) ? (TaskStatus)syncTaskInfo.Status : this.m_taskStatus;
                                status_seted = true;
                                completeCondition.SetCompleteProgressData(syncTaskInfo.DialogueProgress);
                            }
                            else if (completeCondition is TaskCompleteItems)
                            {
                                completeCondition.SetCompleteProgressData(syncTaskInfo.PropProgresss);
                                completeCondition.SetCompleteProgressData(syncTaskInfo.ExhibitProgress);
                            }
                        }
                        break;
                    default:
                        break;
                }
            }

            //TODO:防止任务状态同步消息，未按指定顺序到达。完成的任务，不能在重置为进行中。
            if (!status_seted)
                this.m_taskStatus = (TaskStatus)syncTaskInfo.Status;

            this.m_taskSyncStatus = TaskSyncStatus.SYNCED;
        }

        public List<TaskTriggerCondition> TriggerConditionList => this.m_taskTriggerConditionList;
        public List<TaskCompleteCondition> CompleteConditionList => this.m_taskCompleteConditionList;
        public List<TaskRewardBase> RewardList => this.m_taskRewardList;


        public object TaskData
        {
            get { return this.m_taskData; }
            set { this.m_taskData = value; }
        }

        public long TaskUUID
        {
            get { return this.m_taskUUID; }
            set { this.m_taskUUID = value; }
        }

        public TaskStatus TaskCurrentStatus
        {
            get { return this.m_taskStatus; }
            set { this.m_taskStatus = value; }
        }

        public TaskSyncStatus TaskSyncStatus
        {
            get { return this.m_taskSyncStatus; }
            set { this.m_taskSyncStatus = value; }
        }

        public long TaskConfID
        {
            get { return this.m_taskId; }
        }
    }

    /// <summary>
    /// 任务状态
    /// </summary>
    public enum TaskStatus
    {
        NONE,
        PROGRESSING,    //进行中
        COMPLETED,      //已完成
        REWARDED,       //已领奖
        TOTAL,
    }

    /// <summary>
    /// 任务同步状态
    /// </summary>
    public enum TaskSyncStatus
    {
        LOCAL,          //本地状态
        SYNCED,         //已经同步
        LOCAL_SYNCED,
    }

    /// <summary>
    /// 任务触发类型
    /// </summary>
    public enum TaskTriggerMode
    {
        TriggerByLevel = 1,     //等级触发
        TriggerByCompleteTask,  //完成指定任务
        TriggerByAchieve,       //成就到达
        TriggerByTitle,         //称号到达
        TriggerByCoin,          //金币到达
        TriggerByCash,          //现金到达
        TriggerByPlayer,        //玩家主动触发 
        TriggerByGotOfficer = 9,   //获取警员触发
        TriggerByOfficerReachLevel, //警员达到等级
    }

    /// <summary>
    /// 任务完成类型
    /// </summary>

    [Flags]
    public enum TaskCompleteMode
    {
        NONE = 0x0,
        CompleteByLevel = 0x1,
        CompletedByItem = 0x2,
        CompletedByDialog = 0x4,
#if OFFICER_SYS
        CompletedByEvents,
#endif
        CompletedBySceneID = 0x8,
        CompleteByReasonID = 0x10,
        CompleteByScanID = 0x20,
        CompleteByCombinePropID = 0x40,
        
    }

    /// <summary>
    /// 任务奖励类型
    /// </summary>
    public enum TaskRewardMode
    {
        ITEM,           //金币，钞票，经验，体力，道具都抽象为Item
        TITLE,
        CLUE, // 解锁的 场景，npc， 拼图线索
    }

    public enum TaskClueMode
    {
        SCENE,
        NPC,
        JIGSAW,
    }
}