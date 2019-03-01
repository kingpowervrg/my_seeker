
namespace SeekerGame
{
    public class GameNormalSkill : GameSkillBase
    {
        public GameNormalSkill(long skillId) : base(skillId)
        {

        }

        public override void OnStart()
        {
            base.OnStart();
            if (m_confSkill.type == 16)
            {
                GlobalInfo.MY_PLAYER_INFO.ChangeVit(m_confSkill.gain * count);
            }
            else if (m_confSkill.type == 18)
            {
                int vit = GlobalInfo.MY_PLAYER_INFO.Vit;
                GlobalInfo.MY_PLAYER_INFO.ChangeVit(vit >= (int)CommonData.MAXVIT ? 0 : (int)CommonData.MAXVIT - vit);
            }
            else if (m_confSkill.type == 19)
            {
                GameEvents.Skill_Event.OnUnlimitedVitSkillStart.SafeInvoke();
            }
        }

        protected override void OnEnd()
        {
            base.OnEnd();
        }
    }
}
