
namespace SeekerGame
{
    [TaskTrigger(TaskTriggerMode.TriggerByLevel)]
    public class TaskTriggerByLevel : TaskTriggerCondition
    {
        public TaskTriggerByLevel(object data) : base(data) { }

        public override bool IsPassCondition()
        {
            int playerLevel = GlobalInfo.MY_PLAYER_INFO.Level;

            return playerLevel >= TaskLevelLimit;
        }


        public int TaskLevelLimit => (int)this.TaskTriggerData;
    }
}