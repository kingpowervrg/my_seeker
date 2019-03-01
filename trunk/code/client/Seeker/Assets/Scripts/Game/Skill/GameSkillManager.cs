using System;
using System.Collections.Generic;
using EngineCore;
using UnityEngine;

namespace SeekerGame
{
    public class GameSkillManager : Singleton<GameSkillManager>
    {
        private Dictionary<long, GameSkillCarryBase> m_propSkills = new Dictionary<long, GameSkillCarryBase>();
        private int m_gameTotalTime = 0;
        private int m_gameRemainTime = 0;

        private List<GameCacheSkill> m_releaseSkill = new List<GameCacheSkill>(); //道具正在释放的技能
        private List<int> m_needCacheSKillType = new List<int> { 11, 12, 13, 14 }; //释放过程中需要缓存的技能类型
        public GameSkillManager()
        {
            LoadPropSkill();
            GameEvents.Skill_Event.OnSkillCDOver += OnSkillCDOver;
        }

        private void LoadPropSkill()
        {
            List<ConfProp> prop = ConfProp.array;
            for (int i = 0; i < prop.Count; i++)
            {
                ConfProp confProp = prop[i];
                if (confProp.skillId <= 0 || confProp.type != 0 && confProp.type != 1)
                {
                    continue;
                }
                GameSkillCarryBase skillBase = GameSkillCarryFactory.Instance.Create(SkillCarrier.Prop, prop[i].id);
                if (!m_propSkills.ContainsKey(prop[i].id))
                {
                    m_propSkills.Add(prop[i].id, skillBase);
                }
            }
        }

        public void OnStartSkill(long propId)
        {
            OnStartSkill(propId, 1);
        }

        public void OnStartSkill(long propId, int count)
        {
            if (m_propSkills.ContainsKey(propId))
            {
                if (m_needCacheSKillType.Contains(m_propSkills[propId].SkillType))
                {
                    //Debug.Log("add skill to cache : "+ m_propSkills[propId].SkillID);
                    m_releaseSkill.Add(new GameCacheSkill(m_propSkills[propId].SkillID, m_propSkills[propId].SkillType));
                }
                m_propSkills[propId].OnStart(count);
            }
        }

        public void OnBreakPropSkill(long[] props)
        {
            for (int i = 0; i < props.Length; i++)
            {
                OnBreakPropSkill(props[i]);
            }
        }

        private void OnBreakPropSkill(long propId)
        {
            if (m_propSkills.ContainsKey(propId))
            {
                m_propSkills[propId].OnBreak();
            }
        }

        private void OnSkillCDOver(long carryID)
        {
            List<GameCacheSkill> removeCacheSkill = new List<GameCacheSkill>();
            //Debug.Log("OnSkillCDOver : " + carryID);
            if (!m_propSkills.ContainsKey(carryID))
            {
                return;
            }
            for (int i = 0; i < m_releaseSkill.Count; i++)
            {
                if (m_releaseSkill[i].skillID == m_propSkills[carryID].SkillID)
                {
                    removeCacheSkill.Add(m_releaseSkill[i]);

#if OFFICER_SYS

                    List<GamePoliceSkill> removePolice = new List<GamePoliceSkill>();
                    for (int policeIndex = 0; policeIndex < m_cachePolice.Count; policeIndex++)
                    {
                        if (m_cachePolice[policeIndex].SkillType == m_releaseSkill[i].skillType)
                        {
                            //UnityEngine.Debug.Log("release cache police skill : " + m_propSkills[carryID].SkillID);
                            m_cachePolice[policeIndex].OnClientReleaseSkill();
                            removePolice.Add(m_cachePolice[policeIndex]);
                        }
                    }
                    for (int j = 0; j < removePolice.Count; j++)
                    {
                        m_cachePolice.Remove(removePolice[j]);
                    }
                    removePolice.Clear();

#endif
                }
            }
            for (int i = 0; i < removeCacheSkill.Count; i++)
            {
                //Debug.Log("remove cache skill : " + removeCacheSkill[i].skillID + "  skill type : " + removeCacheSkill[i].skillType);
                m_releaseSkill.Remove(removeCacheSkill[i]);
            }
            removeCacheSkill.Clear();
        }

        //警员技能是否能释放
        public bool PoliceSkillCanRelease(long skillType)
        {
            //Debug.Log("check police skillType :" + skillType);
            for (int i = 0; i < m_releaseSkill.Count; i++)
            {
                if (m_releaseSkill[i].skillType == skillType)
                {
                    return false;
                }
            }
            return true;

        }


        public void Start()
        {
            this.m_gameTotalTime = 0;
            this.m_gameRemainTime = 0;
#if OFFICER_SYS
            this.m_isPoliceSkillRelease = false;
            GameEvents.Skill_Event.OnHeroSKillResult += OnPoliceSkillFinish;
#endif
            GameEvents.MainGameEvents.OnGameTimeTick += OnGameTimeTick;


        }

        private void OnGameTimeTick(int remainTime)
        {
            if (remainTime != this.m_gameRemainTime)
            {
                this.m_gameRemainTime = remainTime;
                this.m_gameTotalTime++;

                if (this.m_gameTotalTime > 17)
                {
#if OFFICER_SYS
                    this.m_isPoliceSkillRelease = true;

                    TickPolice();
#endif
                }
            }
        }

        public void OnDestory()
        {
            m_releaseSkill.Clear();

            GameEvents.MainGameEvents.OnGameTimeTick -= OnGameTimeTick;
        }

#if OFFICER_SYS
        private List<GamePoliceSkill> m_cachePolice = new List<GamePoliceSkill>(); //所有缓存待释放的警员

        //存入缓存
        public void PushPoliceToCache(GamePoliceSkill police)
        {
            if (!m_cachePolice.Contains(police))
            {
                m_cachePolice.Add(police);
            }
        }

 public bool m_hasPoliceAddition = false;

        private List<GamePoliceSkill> policeSkills = new List<GamePoliceSkill>();
        public void CreatePoliceSkill(IList<long> polices)
        {
            this.policeSkills.Clear();
            for (int i = 0; i < polices.Count; i++)
            {
                GamePoliceSkill policeSkill = GameSkillCarryFactory.Instance.Create(SkillCarrier.Police, polices[i]) as GamePoliceSkill;
                policeSkills.Add(policeSkill);
            }
        }

        public void AddPoliceSkill(long police)
        {
            GamePoliceSkill policeSkill = GameSkillCarryFactory.Instance.Create(SkillCarrier.Police, police) as GamePoliceSkill;

            policeSkills.Add(policeSkill);
        }


        private void OnPoliceSkillFinish(long carryID, bool result)
        {
            //Debug.Log("police result  ====  " + result);
            this.m_gameRemainTime = 0;
            this.m_gameTotalTime = 0;

        }
        private bool m_isPoliceSkillRelease = false;
       

   private void TickPolice()
        {
            if (policeSkills.Count > 0)
            {
                policeSkills[0].OnStart();
            }
        }

         public void OnDestory()
        {

            for (int i = 0; i < policeSkills.Count; i++)
            {
                policeSkills[i].OnDestory();
            }
            m_releaseSkill.Clear();
            m_cachePolice.Clear();
            
            TimeModule.Instance.RemoveTimeaction(TickPolice);
            this.policeSkills.Clear();
            GameSkillManager.Instance.m_hasPoliceAddition = false;

            GameEvents.MainGameEvents.OnGameTimeTick -= OnGameTimeTick;
            GameEvents.Skill_Event.OnHeroSKillResult -= OnPoliceSkillFinish;
        }
       
#endif


    }

    public class GameCacheSkill
    {
        public long skillID;
        public int skillType;

        public GameCacheSkill(long skillID, int skillType)
        {
            this.skillID = skillID;
            this.skillType = skillType;
        }
    }
}
