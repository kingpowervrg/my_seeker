/********************************************************************
	created:  2018-5-14 11:32:43
	filename: TaskCompleteByLevel.cs
	author:	  songguangze@outlook.com
	
	purpose:  任务系统-任务完成条件-达到等级
*********************************************************************/
namespace SeekerGame
{
    [TaskComplete(TaskCompleteMode.CompleteByLevel)]
    public class TaskCompleteByLevel : TaskCompleteCondition
    {
        public TaskCompleteByLevel(object data) : base(data) { }

        public override void SetCompleteProgressData(object progressData)
        {
            int currentLevel = (int)progressData;
            this.m_taskCompletedConditionProgress = currentLevel >= TaskCompleteData ? 1 : 0;
        }

        new public int TaskCompleteData => (int)base.TaskCompleteData;
    }
}