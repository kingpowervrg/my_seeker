using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SeekerGame
{
    [TaskTrigger(TaskTriggerMode.TriggerByCompleteTask)]
    public class TaskTriggerByCompleteTask : TaskTriggerCondition
    {
        public TaskTriggerByCompleteTask(object data) : base(data) { }

        public override bool IsPassCondition()
        {
            return true;
        }


        public long CompletedTaskID => Convert.ToInt64(base.TaskTriggerData);

    }
}