using System;
using System.Collections.Generic;
using UnityEngine;
namespace SeekerGame.NewGuid
{
    public class GuidNewFuncLoadEffect : GuidNewFunctionBase
    {
        private long m_effectID;
        private string m_resName;
        private Vector2 m_localPos;
        private float m_rotation = 0f; //旋转
        public override void OnInit(long funcID, string[] param)
        {
            base.OnInit(funcID, param);
            this.m_effectID = long.Parse(param[0]);
            this.m_resName = param[1];
            this.m_localPos = new Vector2(float.Parse(param[2]),float.Parse(param[3]));
            if (param.Length == 5)
            {
                this.m_rotation = float.Parse(param[4]);
            }
        }

        public override void OnExecute()
        {
            base.OnExecute();
            GameEvents.UI_Guid_Event.OnLoadEffect.SafeInvoke(this.m_effectID,this.m_resName,this.m_localPos,Vector2.one,m_rotation);
            OnDestory();
        }

        //public override void OnDestory(FuncState funcState = FuncState.Complete)
        //{
        //    base.OnDestory(funcState);
        //}
    }
}
