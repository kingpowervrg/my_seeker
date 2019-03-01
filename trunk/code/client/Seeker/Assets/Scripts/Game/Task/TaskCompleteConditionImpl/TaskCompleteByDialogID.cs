/********************************************************************
	created:  2018-5-16 11:6:9
	filename: TaskCompleteByDialogID.cs
	author:	  songguangze@outlook.com
	
	purpose:  任务系统-任务完成条件-完成对话
*********************************************************************/
using System;

namespace SeekerGame
{
    [TaskComplete(TaskCompleteMode.CompletedByDialog)]
    public class TaskCompleteByDialogID : TaskCompleteCondition
    {
        public TaskCompleteByDialogID(object data) : base(data) { }

        public override void SetCompleteProgressData(object progressData)
        {
            this.m_taskCompletedConditionProgress = Convert.ToSingle(progressData);
        }

        new public long TaskCompleteData => Convert.ToInt64(base.TaskCompleteData);
    }
}