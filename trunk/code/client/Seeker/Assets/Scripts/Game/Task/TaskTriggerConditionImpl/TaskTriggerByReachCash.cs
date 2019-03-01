using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SeekerGame
{
    [TaskTrigger(TaskTriggerMode.TriggerByCash)]
    public class TaskTriggerByReachCash : TaskTriggerCondition
    {
        public TaskTriggerByReachCash(object data) : base(data)
        { }


        public int TaskReachCash => (int)this.TaskTriggerData;

        public override bool IsPassCondition()
        {
            return GlobalInfo.MY_PLAYER_INFO.Cash >= TaskReachCash;
        }
    }
}