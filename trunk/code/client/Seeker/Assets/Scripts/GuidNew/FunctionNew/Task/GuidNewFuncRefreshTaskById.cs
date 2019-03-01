using EngineCore;

namespace SeekerGame.NewGuid
{
    //刷新任务
    public class GuidNewFuncRefreshTaskById : GuidNewFunctionBase
    {
        private long m_oldTaskId;
        private long m_newTaskId;
        public override void OnInit(long funcID, string[] param)
        {
            base.OnInit(funcID, param);
            this.m_oldTaskId = long.Parse(param[0]);
            this.m_newTaskId = long.Parse(param[1]);
        }

        public override void OnExecute()
        {
            base.OnExecute();
            CSTaskListRequest request = new CSTaskListRequest();
            request.Type = 1;
            request.TaskIds.Add(this.m_oldTaskId);
            request.TaskIds.Add(this.m_newTaskId);
            GameEvents.NetWorkEvents.SendMsg.SafeInvoke(request);
            OnDestory();
        }

    }
}
