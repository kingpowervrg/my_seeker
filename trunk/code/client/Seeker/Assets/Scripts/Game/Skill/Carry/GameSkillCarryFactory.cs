using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SeekerGame
{
    public class GameSkillCarryFactory : Singleton<GameSkillCarryFactory>
    {
        private Dictionary<SkillCarrier, Type> types = new Dictionary<SkillCarrier, Type>();

        public GameSkillCarryFactory()
        {
            types.Add(SkillCarrier.Prop, typeof(GamePropSKill));
#if OFFICER_SYS
            types.Add(SkillCarrier.Police, typeof(GamePoliceSkill));
#endif
        }

        public GameSkillCarryBase Create(SkillCarrier skillCarrier, long carryId)
        {
            GameSkillCarryBase carryBase = null;
            if (types.ContainsKey(skillCarrier))
            {
                carryBase = Activator.CreateInstance(types[skillCarrier], carryId) as GameSkillCarryBase;
            }
            return carryBase;
        }
    }
}
