using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SeekerGame
{
    [TaskTrigger(TaskTriggerMode.TriggerByGotOfficer)]
    public class TaskTriggerByGotOfficer : TaskTriggerCondition
    {
        public TaskTriggerByGotOfficer(object triggerData) : base(triggerData)
        {

        }

        public override bool IsPassCondition()
        {
            return true;
        }
    }
}