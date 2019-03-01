namespace SeekerGame
{
    public abstract class TaskRewardBase
    {
        protected object m_rewardData;

        public TaskRewardBase(object rewardData)
        {
            this.m_rewardData = rewardData;
        }

        public object RewardData => this.m_rewardData;
    }

    /// <summary>
    /// 奖励物品类型
    /// </summary>
    public enum RewardItemType
    {
        COIN,
        EXP,
        VIT,
        CASH,
        ITEM,
    }
}