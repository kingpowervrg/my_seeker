using EngineCore;

namespace SeekerGame
{
    public class SkillTools
    {
        public static long GetSkillIdByLevel(ConfOfficer confOffice, int level)
        {
            if (1 == level) { return confOffice.skillId; }
            else if (2 == level) { return confOffice.level2SkillId; }
            else if (3 == level) { return confOffice.level3SkillId; }
            else if (4 == level) { return confOffice.level4SkillId; }
            else if (5 == level) { return confOffice.level5SkillId; }
            else if (6 == level) { return confOffice.level6SkillId; }
            else if (7 == level) { return confOffice.level7SkillId; }
            else if (8 == level) { return confOffice.level8SkillId; }
            else if (9 == level) { return confOffice.level9SkillId; }
            else { return confOffice.level10SkillId; }
        }
    }
}
