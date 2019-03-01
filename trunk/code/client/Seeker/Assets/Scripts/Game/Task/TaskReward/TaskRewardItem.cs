/********************************************************************
	created:  2018-5-15 13:38:0
	filename: TaskRewardItem.cs
	author:	  songguangze@outlook.com
	
	purpose:  任务系统-任务奖励物品
*********************************************************************/
using System.Collections.Generic;

namespace SeekerGame
{
    [TaskReward(TaskRewardMode.ITEM)]
    public class TaskRewardItem : TaskRewardBase
    {
        public TaskRewardItem(object reward) : base(reward)
        {

        }

        //new public List<RewardItemDataWrap> RewardData => base.m_rewardData as List<RewardItemDataWrap>;
        new public RewardItemDataWrap RewardData => base.m_rewardData as RewardItemDataWrap;
    }

    /// <summary>
    /// 奖励物品包装
    /// </summary>
    public class RewardItemDataWrap
    {
        public RewardItemType ItemType;
        public int ItemNum;
        public long ItemID = 0;
    }


}