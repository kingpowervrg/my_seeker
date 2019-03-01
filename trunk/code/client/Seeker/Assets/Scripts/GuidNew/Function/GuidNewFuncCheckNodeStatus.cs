using System;
using System.Collections.Generic;

namespace SeekerGame.NewGuid
{
    public class GuidNewFuncCheckNodeStatus : GuidNewFunctionBase
    {
        private string m_node;
        private NodeStatus m_nodeStatus = NodeStatus.Complete;
        public override void OnInit(long funcID, string[] param)
        {
            base.OnInit(funcID, param);
            this.m_node = param[0];
            if (param.Length > 1 )
            {
                this.m_nodeStatus = (NodeStatus)int.Parse(param[1]);
            }
        }

        public override void OnExecute()
        {
            base.OnExecute();
            if (GuidNewNodeManager.Instance.GetNodeStatus(m_node) == m_nodeStatus)
            {
                OnDestory();
                return;
            }
            GameEvents.UI_Guid_Event.OnGuidNewNodeStatusChange += OnGuidNewNodeStatusChange;
        }

        public override void OnDestory(FuncState funcState = FuncState.Complete)
        {
            base.OnDestory(funcState);
            GameEvents.UI_Guid_Event.OnGuidNewNodeStatusChange -= OnGuidNewNodeStatusChange;
        }

        private void OnGuidNewNodeStatusChange(string node,NodeStatus status)
        {
            if (node.Equals(m_node) && m_nodeStatus == status)
            {
                OnDestory();
            }
        }

        public override void ClearFunc()
        {
            base.ClearFunc();
            GuidNewNodeManager.Instance.SetNodeStatus(m_node, NodeStatus.Complete);
            GameEvents.UI_Guid_Event.OnGuidNewNodeStatusChange -= OnGuidNewNodeStatusChange;
        }
    }
}
