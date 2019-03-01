using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SeekerGame
{
    [TaskComplete(TaskCompleteMode.CompleteByReasonID)]
    public class TaskCompleteByReasoning : TaskCompleteCondition
    {
        public TaskCompleteByReasoning(object data) : base(data) { }

        public override void SetCompleteProgressData(object progressData)
        {
            this.m_taskCompletedConditionProgress = Convert.ToSingle(progressData);
        }

        new public long TaskCompleteData => Convert.ToInt64(base.TaskCompleteData);
    }
}
