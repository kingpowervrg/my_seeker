using UnityEngine;
using EngineCore;
using System;

namespace SeekerGame
{


    public class GameSkillBase
    {
        protected ConfSkill m_confSkill;
        private Action m_OnEnd;
        protected int count = 1;
        protected GameSkillCarryBase m_carryBase = null;
        public int SkillType
        {
            get { return m_confSkill.type; }
        }

        public GameSkillBase(long skillId)
        {
            m_confSkill = ConfSkill.Get(skillId);
            if (m_confSkill == null)
            {
                Debug.LogError("skill is not Exist : " + skillId);
            }
        }

        public void SetCarryBase(GameSkillCarryBase carrybase)
        {
            this.m_carryBase = carrybase;
        }

        public void SetReleaseCount(int count)
        {
            this.count = count;
        }

        public void SetOnEnd(Action func)
        {
            m_OnEnd = func;
        }

        public virtual void OnStart()
        {
            this.m_surPlusTime = 0f;
            TimeModule.Instance.SetTimeout(OnEnd, m_confSkill.cd);
        }

        private float m_surPlusTime = 0f;
        public virtual void OnPause()
        {
            TimeModule.TimeFunc timeFunc = TimeModule.Instance.GetDuplicate(OnEnd);
            if (timeFunc == null)
            {
                return;
            }
            this.m_surPlusTime = timeFunc.time;
            TimeModule.Instance.RemoveTimeaction(OnEnd);
        }

        public virtual void OnResume()
        {
            if (m_surPlusTime > 0f)
            {
                TimeModule.Instance.SetTimeout(OnEnd, m_surPlusTime);
            }
        }
         
        public virtual void BreadSkill()
        {
            TimeModule.Instance.RemoveTimeaction(OnEnd);
        }


        protected virtual void OnEnd()
        {
            if (m_OnEnd != null)
            {
                m_OnEnd();
            }
        }
    }
}
