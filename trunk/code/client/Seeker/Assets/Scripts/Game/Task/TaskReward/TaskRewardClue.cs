/********************************************************************
	created:  2018-5-15 13:54:16
	filename: TaskRewardTitle.cs
	author:	  songguangze@outlook.com
	
	purpose:  任务系统-任务奖励称号
*********************************************************************/
namespace SeekerGame
{
    [TaskReward(TaskRewardMode.CLUE)]
    public class TaskRewardClue : TaskRewardBase
    {
        TaskClueMode m_mode;
        public SeekerGame.TaskClueMode Mode
        {
            get { return m_mode; }
            set { m_mode = value; }
        }
        public TaskRewardClue(object rewardData) : base(rewardData)
        {

        }

        public new RewardClueDataWarp RewardData => (RewardClueDataWarp)m_rewardData;
    }

    public class RewardClueDataWarp
    {
        public TaskClueMode m_clueType;

        public long m_id;

        public string m_png;
    }
}