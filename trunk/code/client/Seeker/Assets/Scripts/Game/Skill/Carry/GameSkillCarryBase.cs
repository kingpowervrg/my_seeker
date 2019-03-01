
using EngineCore;

namespace SeekerGame
{
    public class GameSkillCarryBase
    {
        protected long m_carryID; //携带者ID
        protected SkillStatus m_skillStatus;
        protected long m_skillID = -1;
        protected GameSkillBase m_skillBase;
        public GameSkillCarryBase(long carryID)
        {
            m_carryID = carryID;
            m_skillStatus = SkillStatus.Normal;
            InitCarryBase();
        }

        public int SkillType
        {
            get { return m_skillBase.SkillType; }
        }

        public long SkillID
        {
            get { return m_skillID; }
        }

        public long CarryID
        {
            get { return m_carryID; }
        }

        protected virtual void InitCarryBase()
        {
            m_skillBase = GameSkillFactory.Instance.CreateSkill(m_skillID, OnEnd);
            m_skillBase.SetCarryBase(this);
        }

        public virtual void OnStart(int count = 1)
        {
            OnRequestSkill(count);
        }

        protected virtual void OnEnd()
        {
            m_skillStatus = SkillStatus.Normal;
            GameEvents.Skill_Event.OnSkillReset.SafeInvoke(m_carryID);
            GameEvents.Skill_Event.OnSkillCDOver.SafeInvoke(m_carryID);
        }

        protected virtual bool OnRequestSkill(int count)
        {
            if (m_skillID < 0 || m_skillStatus == SkillStatus.Release || m_skillBase == null)
            {
                return false;
            }



            return true;
        }

        public void OnBreak()
        {
            m_skillBase.BreadSkill();
            m_skillStatus = SkillStatus.Normal;
        }

        protected virtual void OnRes(object obj)
        {
            if (m_skillID < 0 || m_skillBase == null)
            {
                return;
            }
        }

        public virtual void OnDestory()
        {
            m_skillBase.BreadSkill();
        }
    }
}
