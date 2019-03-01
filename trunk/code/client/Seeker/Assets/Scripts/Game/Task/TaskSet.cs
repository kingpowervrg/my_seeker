/********************************************************************
	created:  2018-5-15 13:55:5
	filename: TaskSet.cs
	author:	  songguangze@outlook.com
	
	purpose:  任务集
*********************************************************************/
using System.Collections.Generic;

namespace SeekerGame
{
    public class TaskSet
    {
        private Dictionary<long, TaskBase> m_taskDict = new Dictionary<long, TaskBase>();
        private Dictionary<long, TaskBase> m_taskIndexDict = new Dictionary<long, TaskBase>();
        private HashSet<long> m_completedMainTaskSet = new HashSet<long>();

        /// <summary>
        /// 添加任务
        /// </summary>
        /// <param name="taskUUID"></param>
        /// <param name="taskBase"></param>
        public void AddTask(long taskUUID, TaskBase taskBase)
        {
            if (!this.m_taskDict.ContainsKey(taskUUID))
            {
                this.m_taskDict.Add(taskUUID, taskBase);
                this.m_taskIndexDict.Add(taskBase.TaskConfID, taskBase);
            }
        }

        /// <summary>
        /// 移除任务
        /// </summary>
        /// <param name="taskId"></param>
        public void RemoveTask(long taskId)
        {
            if (this.m_taskDict.ContainsKey(taskId))
            {
                TaskBase removeTask = this.m_taskDict[taskId];
                this.m_taskDict.Remove(taskId);
                this.m_taskIndexDict.Remove(removeTask.TaskConfID);
            }
        }

        /// <summary>
        /// 查工任务
        /// </summary>
        /// <param name="taskId"></param>
        /// <returns></returns>
        public TaskBase GetTaskById(long taskId)
        {
            TaskBase task;
            if (this.m_taskDict.TryGetValue(taskId, out task))
                return task;

            return null;
        }

        public TaskBase GetTaskByTaskConfigID(long confTaskID)
        {
            TaskBase taskInfo;
            if (this.m_taskIndexDict.TryGetValue(confTaskID, out taskInfo))
                return taskInfo;

            return null;
        }

    }
}