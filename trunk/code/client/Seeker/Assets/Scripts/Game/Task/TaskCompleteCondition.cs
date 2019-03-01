/********************************************************************
	created:  2018-5-15 20:52:3
	filename: TaskCompleteCondition.cs
	author:	  songguangze@outlook.com
	
	purpose:  任务完成条件基类
*********************************************************************/
namespace SeekerGame
{
    public abstract class TaskCompleteCondition
    {
        protected object m_taskCompleteData;

        //完成进度
        protected float m_taskCompletedConditionProgress = 0f;

        public TaskCompleteCondition(object data)
        {
            this.m_taskCompleteData = data;
        }

        /// <summary>
        /// 设置完成进度数据
        /// </summary>
        /// <param name="progressData"></param>
        public abstract void SetCompleteProgressData(object progressData);
        
        public object TaskCompleteData => this.m_taskCompleteData;

        public float TaskCompleteConditionProgress
        {
            get { return this.m_taskCompletedConditionProgress; }
            set { this.m_taskCompletedConditionProgress = value; }
        }

        /// <summary>
        /// 条件是否完成
        /// </summary>
        public virtual bool IsComplete => TaskCompleteConditionProgress >= 1;
    }
}