/********************************************************************
	created:  2018-5-15 13:54:16
	filename: TaskRewardTitle.cs
	author:	  songguangze@outlook.com
	
	purpose:  任务系统-任务奖励称号
*********************************************************************/
namespace SeekerGame
{
    [TaskReward(TaskRewardMode.TITLE)]
    public class TaskRewardTitle : TaskRewardBase
    {
        public TaskRewardTitle(object rewardData) : base(rewardData)
        {

        }

        //Title ID
        new public long RewardData => (long)m_rewardData;
    }
}