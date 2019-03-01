

namespace SeekerGame.NewGuid
{
    /// <summary>
    /// 完成循环任务
    /// </summary>
    public class GuidNewFuncLoopTaskComplete : GuidNewFunctionBase
    {
        public override void OnExecute()
        {
            base.OnExecute();
            GameEvents.TaskEvents.OnReceiveTask += OnCompletedTask;
        }

        private void OnCompletedTask(TaskBase taskbase)
        {
            ConfTask task = ConfTask.Get(taskbase.TaskConfID);
            if (task != null && task.type == 3)
            {
                OnDestory();
            }
        }

        public override void OnDestory(FuncState funcState = FuncState.Complete)
        {
            base.OnDestory(funcState);
            GameEvents.TaskEvents.OnReceiveTask -= OnCompletedTask;
        }

        public override void ClearFunc()
        {
            base.ClearFunc();
            GameEvents.TaskEvents.OnReceiveTask -= OnCompletedTask;
        }
    }
}
