using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SeekerGame
{
    [TaskTrigger(TaskTriggerMode.TriggerByOfficerReachLevel)]
    public class TaskTriggerByOfficerReachLevel : TaskTriggerCondition
    {
        public TaskTriggerByOfficerReachLevel(object triggerData) : base(triggerData)
        {

        }

        public override bool IsPassCondition()
        {
            return true;
        }
    }
}