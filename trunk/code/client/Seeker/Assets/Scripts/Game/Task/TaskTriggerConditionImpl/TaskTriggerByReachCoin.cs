using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SeekerGame
{
    [TaskTrigger(TaskTriggerMode.TriggerByCoin)]
    public class TaskTriggerByReachCoin : TaskTriggerCondition
    {
        public TaskTriggerByReachCoin(object taskTriggerData) : base(taskTriggerData)
        {
        }

        public override bool IsPassCondition()
        {
            int playerCoin = GlobalInfo.MY_PLAYER_INFO.Coin;

            return playerCoin >= TaskReachCoin;
        }

        public int TaskReachCoin => (int)this.TaskTriggerData;
    }
}