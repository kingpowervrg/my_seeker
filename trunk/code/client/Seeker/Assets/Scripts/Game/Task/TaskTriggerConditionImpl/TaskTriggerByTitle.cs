/********************************************************************
	created:  2018-6-26 15:25:30
	filename: GameReadyUILogic.cs
	author:	  songguangze@fotoable.com
	
	purpose:  任务系统-任务触发条件-达到成就
*********************************************************************/

namespace SeekerGame
{
    [TaskTrigger(TaskTriggerMode.TriggerByTitle)]
    public class TaskTriggerByTitle : TaskTriggerCondition
    {
        public TaskTriggerByTitle(object data) : base(data) { }

        public override bool IsPassCondition()
        {
            //todo:目前是服务器直接给，客户端逻辑需要的话直接填充此结构
            return true;
        }

    }
}