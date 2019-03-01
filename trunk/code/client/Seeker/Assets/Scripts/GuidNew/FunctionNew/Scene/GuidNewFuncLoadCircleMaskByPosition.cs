using System;
using System.Collections.Generic;
using UnityEngine;

namespace SeekerGame.NewGuid
{
    public class GuidNewFuncLoadCircleMaskByPosition : GuidNewFunctionBase
    {
        protected Vector4 m_centerPos;
        protected float m_radius;
        protected int m_type; //0出现遮罩  1遮罩消失

        protected bool m_needbaseInit = true;
        public override void OnInit(long funcID, string[] param)
        {
            base.OnInit(funcID, param);
            if (param.Length >= 1)
            {
                this.m_type = int.Parse(param[0]);
            }
            if (m_needbaseInit && param.Length >= 4)
            {
                float x = float.Parse(param[1]);
                float y = float.Parse(param[2]);
                this.m_centerPos = Vector2.right * x + Vector2.up * y;
                this.m_radius = float.Parse(param[3]);
            }
            this.m_maxRadius = Screen.width;
            if (this.m_type == 1)
            {
                this.m_maxRadius *= 2;
            }
        }

        public override void OnExecute()
        {
            base.OnExecute();
            this.m_timesection = 0f;
            if (this.m_type == 1)
            {
                m_centerPos = GameEvents.UI_Guid_Event.OnGetCurrentMaskInfo(0);
                this.m_radius = this.m_centerPos.z;
            }
            GuidNewModule.Instance.PushFunction(this);
        }

        public override void OnDestory(FuncState funcState = FuncState.Complete)
        {
            base.OnDestory(funcState);
        }

        public override void ClearFunc()
        {
            base.ClearFunc();
            GuidNewModule.Instance.RemoveFunction(this);
            this.m_timesection = 0f;
            this.m_tempRadius = 0f;
        }

        public override void ResetFunc(bool isRetainFunc = true)
        {
            base.ResetFunc(isRetainFunc);
            ClearFunc();
        }

        private float m_timesection = 0f;
        private float m_costTime = 1.3f;
        private float m_tempRadius = 0f;
        private float m_maxRadius = 0f;
        public override void Tick(float time)
        {
            base.Tick(time);
            m_timesection += time;
            if (0 == m_type)
            {
                m_tempRadius = Mathf.Lerp(this.m_maxRadius, this.m_radius, Mathf.Clamp01(m_timesection / m_costTime));
            }
            else
            {
                m_tempRadius = Mathf.Lerp(this.m_radius, this.m_maxRadius, Mathf.Clamp01(m_timesection / m_costTime));
            }
            this.m_centerPos.z = m_tempRadius;
            GameEvents.UI_Guid_Event.OnReflashCircleMask.SafeInvoke(this.m_centerPos,0);
            if (m_timesection >= m_costTime)
            {
                GuidNewModule.Instance.RemoveFunction(this);
                this.m_timesection = 0f;
                this.m_tempRadius = 0f;
                OnDestory();
            }
        }
    }
}
