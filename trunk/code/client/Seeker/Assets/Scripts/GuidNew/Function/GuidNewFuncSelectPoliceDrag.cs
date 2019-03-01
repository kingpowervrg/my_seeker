using System;
using System.Collections.Generic;

namespace SeekerGame.NewGuid
{
    public class GuidNewFuncSelectPoliceDrag : GuidNewFunctionBase
    {
        private float m_first_delta_pos;
        private int count = 0;
        private float m_first_delta_posOrigin = 0f;
        private int maxCount = 2;
        public override void OnInit(long funcID, string[] param)
        {
            base.OnInit(funcID, param);
            this.m_first_delta_pos = float.Parse(param[0]);
            this.maxCount = int.Parse(param[1]);
        }

        public override void OnExecute()
        {
            base.OnExecute();
            count = 0;
            GameEvents.UI_Guid_Event.OnSelectPolice += OnSelectPolice;
            this.m_first_delta_posOrigin = GuidNewNodeManager.Instance.first_delta_pos;
            GuidNewNodeManager.Instance.first_delta_pos = m_first_delta_pos;
            //OnDestory();
        }

        private void OnSelectPolice(long officer_id)
        {
            count++;
            if (count >= maxCount)
            {
                OnDestory();
            }
        }

        public override void OnDestory(FuncState funcState = FuncState.Complete)
        {
            base.OnDestory(funcState);
            GameEvents.UI_Guid_Event.OnSelectPolice -= OnSelectPolice;
            GuidNewNodeManager.Instance.first_delta_pos = m_first_delta_posOrigin;
        }

        public override void ClearFunc()
        {
            base.ClearFunc();
            GameEvents.UI_Guid_Event.OnSelectPolice -= OnSelectPolice;
            GuidNewNodeManager.Instance.first_delta_pos = m_first_delta_posOrigin;
        }
    }
}
