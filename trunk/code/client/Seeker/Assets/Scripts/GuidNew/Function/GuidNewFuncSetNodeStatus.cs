using System;
using System.Collections.Generic;

namespace SeekerGame.NewGuid
{
    public class GuidNewFuncSetNodeStatus : GuidNewFunctionBase
    {
        private string m_node;
        public NodeStatus m_nodeStatus = NodeStatus.Complete;
        public override void OnInit(long funcID, string[] param)
        {
            base.OnInit(funcID, param);
            this.m_node = param[0];
            this.m_nodeStatus = (NodeStatus)int.Parse(param[1]);
        }

        public override void OnExecute()
        {
            base.OnExecute();
            GuidNewNodeManager.Instance.SetNodeStatus(m_node, m_nodeStatus);
            GameEvents.UI_Guid_Event.OnGuidNewNodeStatusChange.SafeInvoke(m_node,m_nodeStatus);
            OnDestory();
        }

        public override void ClearFunc()
        {
            base.ClearFunc();
            GuidNewNodeManager.Instance.SetNodeStatus(m_node, NodeStatus.Complete);
        }

        public override void ForceFuncDestory()
        {
            base.ForceFuncDestory();
            GuidNewNodeManager.Instance.SetNodeStatus(m_node, NodeStatus.Complete);
        }
    }
}
