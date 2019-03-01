using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SeekerGame.NewGuid
{
    /// <summary>
    /// 设置公共变量
    /// </summary>
    public class GuidNewFuncSetCommonParam : GuidNewFunctionBase
    {
        private string m_key;
        private string m_value;
        private int m_paramStatus;  //0表示移除   1表示存储
        public override void OnInit(long funcID, string[] param)
        {
            base.OnInit(funcID, param);
            if (param.Length > 1)
            {
                this.m_paramStatus = int.Parse(param[0]);
                this.m_key = param[1];
                
            }
            if (param.Length > 2)
            {
                this.m_value = param[2];
            }
        }

        public override void OnExecute()
        {
            base.OnExecute();
            if (m_paramStatus == 0)
            {
                GuidNewNodeManager.Instance.RemoveCommonParams(m_key);
            }
            else
            {
                GuidNewNodeManager.Instance.SetCommonParams(m_key,m_value);
            }
            OnDestory();
            
        }

        public override void OnDestory(FuncState funcState = FuncState.Complete)
        {
            base.OnDestory(funcState);
        }
    }
}
