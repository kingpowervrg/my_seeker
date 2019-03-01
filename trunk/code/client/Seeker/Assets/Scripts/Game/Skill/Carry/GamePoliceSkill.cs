#if OFFICER_SYS
using EngineCore;
namespace SeekerGame
{
    public class GamePoliceSkill : GameSkillCarryBase
    {
        public GamePoliceSkill(long policeID) : base(policeID)
        {

        }

        protected override void InitCarryBase()
        {
            OfficerInfo officerInfo = GlobalInfo.MY_PLAYER_INFO.GetOfficerInfoByPlayId(m_carryID);
            ConfOfficer confOffice = ConfOfficer.Get(officerInfo.OfficerId);
            if (confOffice != null)
            {
                m_skillID = SkillTools.GetSkillIdByLevel(confOffice, officerInfo.Level);
                ConfSkill confSkill = ConfSkill.Get(m_skillID);
                if (confSkill != null)
                {
                    if (confSkill.phase == 3)
                        GameSkillManager.Instance.m_hasPoliceAddition = true;
                }
                MessageHandler.RegisterMessageHandler(MessageDefine.SCSkillTimerEmitResp, OnRes);
                base.InitCarryBase();
            }
        }



        protected override bool OnRequestSkill(int count)
        {
            if (!base.OnRequestSkill(count))
            {
                return false;
            }

            CSSkillTimerEmitReq req = new CSSkillTimerEmitReq();
            GameEvents.NetWorkEvents.SendAsyncMsg.SafeInvoke(req);
            return true;
        }

        protected override void OnRes(object obj)
        {
            base.OnRes(obj);
            if (obj is SCSkillTimerEmitResp)
            {
                SCSkillTimerEmitResp res = (SCSkillTimerEmitResp)obj;
                if (res.SkillId == m_skillID && res.PlayerOfficerId == m_carryID && res.Result == 1)
                {
                    //UnityEngine.Debug.Log("start police skill ====  " + m_carryID);
                    if (GameSkillManager.Instance.PoliceSkillCanRelease(SkillType))
                    {
                        UnityEngine.Debug.Log("release police skill : " + m_skillID);
                        OnClientReleaseSkill();
                    }
                    else
                    {
                        UnityEngine.Debug.Log("cache police skill : " + m_skillID);
                        GameSkillManager.Instance.PushPoliceToCache(this);
                    }
                }
                GameEvents.Skill_Event.OnHeroSKillResult.SafeInvoke(m_carryID, res.Result == 1);
            }
        }

        public void OnClientReleaseSkill()
        {
            //开始释放
            EngineCoreEvents.AudioEvents.PlayAudio.SafeInvoke(Audio.AudioType.UISound, GameCustomAudioKey.policeman_skill.ToString());
            m_skillStatus = SkillStatus.Release;
            GameEvents.Skill_Event.OnSkillFinish.SafeInvoke(m_carryID);
            m_skillBase.OnStart();
        }

        protected override void OnEnd()
        {
            base.OnEnd();
        }

        public override void OnDestory()
        {
            base.OnDestory();
            MessageHandler.UnRegisterMessageHandler(MessageDefine.SCSkillTimerEmitResp, OnRes);
            //UnityEngine.Debug.Log("Destory police skill ");
        }

    }
}
#endif