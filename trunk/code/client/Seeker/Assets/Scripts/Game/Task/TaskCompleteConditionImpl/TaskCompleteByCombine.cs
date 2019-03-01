using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SeekerGame
{
    [TaskComplete(TaskCompleteMode.CompleteByCombinePropID)]
    public class TaskCompleteByCombine : TaskCompleteCondition
    {
        public TaskCompleteByCombine(object param) : base(param)
        {
            long[] itemIds = param as long[];

            this.m_taskCompleteData = itemIds;
        }

        public override void SetCompleteProgressData(object progressData)
        {

        }

        new public long[] TaskCompleteData => this.m_taskCompleteData as long[];

    }
}
