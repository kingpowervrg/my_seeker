/********************************************************************
	created:  2018-5-14 12:4:35
	filename: TaskFactory.cs
	author:	  songguangze@outlook.com
	
	purpose:  任务工厂
*********************************************************************/
using EngineCore;
using System;
using System.Collections.Generic;

namespace SeekerGame
{
    public class TaskFactory : Singleton<TaskFactory>
    {
        private Dictionary<int, Type> m_taskImplDict = new Dictionary<int, Type>();

        private Dictionary<TaskTriggerMode, Type> m_taskTriggerConditionDict = new Dictionary<TaskTriggerMode, Type>();
        private Dictionary<TaskCompleteMode, Type> m_taskCompleteDict = new Dictionary<TaskCompleteMode, Type>();
        private Dictionary<TaskRewardMode, Type> m_taskRewardDict = new Dictionary<TaskRewardMode, Type>();

        public TaskFactory()
        {
            InitTaskFactory();
        }

        public void InitTaskFactory()
        {
            InitTaskImplements();

            InitTaskTriggerConditions();

            InitTaskCompleteConditions();

            InitTaskRewardInfos();
        }

        private void InitTaskImplements()
        {
            this.m_taskImplDict.Add(1, typeof(NormalTask));
        }


        private void InitTaskTriggerConditions()
        {
            Type[] triggerConditionTypes = ReflectionHelper.GetAssemblyCustomAttributeTypeList<TaskTriggerAttribute>();
            foreach (Type triggerCondition in triggerConditionTypes)
            {
                TaskTriggerAttribute taskTrigger = triggerCondition.GetCustomAttributes(typeof(TaskTriggerAttribute), false)[0] as TaskTriggerAttribute;

                this.m_taskTriggerConditionDict.Add(taskTrigger.TriggerMode, triggerCondition);
            }
        }


        private void InitTaskCompleteConditions()
        {
            Type[] taskCompletedTypes = ReflectionHelper.GetAssemblyCustomAttributeTypeList<TaskCompleteAttribute>();
            foreach (Type taskCompletedImplType in taskCompletedTypes)
            {
                TaskCompleteAttribute taskComplete = taskCompletedImplType.GetCustomAttributes(typeof(TaskCompleteAttribute), false)[0] as TaskCompleteAttribute;

                this.m_taskCompleteDict.Add(taskComplete.CompleteMode, taskCompletedImplType);
            }
        }

        private void InitTaskRewardInfos()
        {
            Type[] taskRewardTypes = ReflectionHelper.GetAssemblyCustomAttributeTypeList<TaskRewardAttribute>();
            foreach (Type taskRewardType in taskRewardTypes)
            {
                TaskRewardAttribute taskReward = taskRewardType.GetCustomAttributes(typeof(TaskRewardAttribute), false)[0] as TaskRewardAttribute;

                this.m_taskRewardDict.Add(taskReward.RewardMode, taskRewardType);
            }
        }


        /// <summary>
        /// 创建任务
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="taskId">任务配置ID</param>
        /// <param name="triggerModeList">任务触发条件参数</param>
        /// <param name="completedModeList">任务完成条件参数</param>
        /// <param name="rewardModeList">任务奖励条件参数</param>
        /// <param name="taskType"></param>
        /// <returns></returns>
        public T CreateTask<T>(long taskId, List<TaskCreaterParams> triggerModeList, List<TaskCreaterParams> completedModeList, List<TaskCreaterParams> rewardModeList, int taskType = 1) where T : TaskBase
        {
            Type taskImplType;
            if (this.m_taskImplDict.TryGetValue(taskType, out taskImplType))
            {
                T taskImpl = Activator.CreateInstance(taskImplType, taskId) as T;

                for (int i = 0; i < triggerModeList.Count; ++i)
                {
                    TaskTriggerMode triggerMode = (TaskTriggerMode)triggerModeList[i].Condition;

                    Type triggerImplType;
                    if (this.m_taskTriggerConditionDict.TryGetValue(triggerMode, out triggerImplType))
                    {
                        TaskTriggerCondition triggerImpl = Activator.CreateInstance(triggerImplType, triggerModeList[i].Data) as TaskTriggerCondition;

                        taskImpl.AddTaskTriggerCondition(triggerImpl);
                    }
                    else
                        throw new Exception("trigger mode :" + triggerMode + " not implement");

                }

                for (int i = 0; i < completedModeList.Count; ++i)
                {
                    TaskCompleteMode completeMode = (TaskCompleteMode)completedModeList[i].Condition;

                    Type triggerCompleteImplType;
                    if (this.m_taskCompleteDict.TryGetValue(completeMode, out triggerCompleteImplType))
                    {
                        TaskCompleteCondition completeTaskConditionImpl = Activator.CreateInstance(triggerCompleteImplType, completedModeList[i].Data) as TaskCompleteCondition;

                        taskImpl.AddTaskCompleteCondition(completeTaskConditionImpl);
                    }
                    else
                        throw new Exception("completed mode :" + completeMode + " not implement");
                }

                for (int i = 0; i < rewardModeList.Count; ++i)
                {
                    TaskRewardMode rewardMode = (TaskRewardMode)rewardModeList[i].Condition;

                    Type rewardImpl;
                    if (this.m_taskRewardDict.TryGetValue(rewardMode, out rewardImpl))
                    {
                        TaskRewardBase taskRewardImpl = Activator.CreateInstance(rewardImpl, rewardModeList[i].Data) as TaskRewardBase;

                        taskImpl.AddTaskRewardInfo(taskRewardImpl);
                    }
                    else
                        throw new Exception("reward mode :" + rewardMode + " not implement");
                }

                return taskImpl;
            }

            return null;
        }
    }

    /// <summary>
    /// 任务创建条件参数
    /// </summary>
    public class TaskCreaterParams
    {
        public int Condition;
        public object Data;
    }
}