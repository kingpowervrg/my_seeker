namespace SeekerGame
{
    public abstract class TaskTriggerCondition
    {
        private TaskBase m_ownerTask = null;
        private object m_taskTriggerData = null;

        public TaskTriggerCondition(object taskTriggerData)
        {
            this.m_taskTriggerData = taskTriggerData;
        }


        /// <summary>
        /// 是否达到触发要求
        /// </summary>
        /// <returns></returns>
        public abstract bool IsPassCondition();


        public object TaskTriggerData
        {
            get { return this.m_taskTriggerData; }
        }

        public TaskBase OwnerTask => this.m_ownerTask;
    }
}