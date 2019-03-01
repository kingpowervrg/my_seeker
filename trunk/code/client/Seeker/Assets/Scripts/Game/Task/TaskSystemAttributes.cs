using System;

namespace SeekerGame
{
    public class TaskTriggerAttribute : Attribute
    {
        private TaskTriggerMode m_taskTriggerMode;

        public TaskTriggerAttribute(TaskTriggerMode triggerMode)
        {
            this.m_taskTriggerMode = triggerMode;
        }

        public TaskTriggerMode TriggerMode => this.m_taskTriggerMode;
    }

    public class TaskCompleteAttribute : Attribute
    {
        private TaskCompleteMode m_taskCompleteMode;

        public TaskCompleteAttribute(TaskCompleteMode completeMode)
        {
            this.m_taskCompleteMode = completeMode;
        }

        public TaskCompleteMode CompleteMode => this.m_taskCompleteMode;
    }

    public class TaskRewardAttribute : Attribute
    {
        private TaskRewardMode m_taskRewardMode;
        public TaskRewardAttribute(TaskRewardMode rewardMode)
        {
            this.m_taskRewardMode = rewardMode;
        }

        public TaskRewardMode RewardMode => this.m_taskRewardMode;
    }
}