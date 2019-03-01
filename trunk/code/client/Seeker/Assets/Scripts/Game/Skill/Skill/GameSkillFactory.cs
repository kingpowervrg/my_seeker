using System;
using System.Collections.Generic;
using UnityEngine;

namespace SeekerGame
{
    public class GameSkillFactory : Singleton<GameSkillFactory>
    {
        private Dictionary<int, Type> types = new Dictionary<int, Type>();
        public GameSkillFactory()
        {
            types.Add(1,typeof(GameCollectSkill));
            types.Add(2, typeof(GameCollectSkill));
            types.Add(3, typeof(GameSkillBase));
            types.Add(4, typeof(GameSkillBase));

            types.Add(5, typeof(GameSkillBase));
            types.Add(6, typeof(GameSkillBase));
            types.Add(7, typeof(GameSkillBase));
            types.Add(8, typeof(GameSkillBase));
            types.Add(9, typeof(GameSkillBase));
            types.Add(10, typeof(GameSkillBase));

            types.Add(11, typeof(GameCollectSkill));
            types.Add(12, typeof(GameCollectSkill));
            types.Add(13, typeof(GameCollectSkill));
            types.Add(14, typeof(GameCollectSkill));
            types.Add(15, typeof(GameCollectSkill));

            types.Add(16, typeof(GameNormalSkill));
            types.Add(17, typeof(GameNormalSkill));
            types.Add(18, typeof(GameNormalSkill));
            types.Add(19, typeof(GameNormalSkill));
        }

        public GameSkillBase CreateSkill(long skillId,Action func)
        {
            GameSkillBase skillBase = null;
            ConfSkill confSkill = ConfSkill.Get(skillId);
            if (confSkill == null)
            {
                Debug.LogError("skill is not exist: " + skillId);
                return null;
            }
            if (types.ContainsKey(confSkill.type))
            {
                skillBase = Activator.CreateInstance(types[confSkill.type], skillId) as GameSkillBase;
                skillBase.SetOnEnd(func);
            }
            return skillBase;
        }
    }
}
