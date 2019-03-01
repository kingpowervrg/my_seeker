using System;
using System.Collections.Generic;
using Utf8Json;

namespace SeekerGame
{
    public class SkillUtil
    {
        public static bool GetCurLevelSkillIconAndDesc(long officer_id_, int lvl_, out string icon_, out string desc_)
        {
            icon_ = string.Empty;
            desc_ = string.Empty;

            lvl_ = 0 == lvl_ ? 1 : lvl_;
            int cur_level = lvl_;

            ConfOfficer exl_data = ConfOfficer.Get(officer_id_);

            long skill_id = -1;

            switch (cur_level)
            {
                case 1:
                    skill_id = exl_data.skillId;
                    break;
                case 2:
                    skill_id = exl_data.level2SkillId;
                    break;
                case 3:
                    skill_id = exl_data.level3SkillId;
                    break;
                case 4:
                    skill_id = exl_data.level4SkillId;
                    break;
                case 5:
                    skill_id = exl_data.level5SkillId;
                    break;
                case 6:
                    skill_id = exl_data.level6SkillId;
                    break;
                case 7:
                    skill_id = exl_data.level7SkillId;
                    break;
                case 8:
                    skill_id = exl_data.level8SkillId;
                    break;
                case 9:
                    skill_id = exl_data.level9SkillId;
                    break;
                case 10:
                    skill_id = exl_data.level10SkillId;
                    break;

            }

            if (skill_id > 0)
            {
                ConfSkill skill = ConfSkill.Get(skill_id);
                icon_ = skill.icon;
                desc_ = LocalizeModule.Instance.GetString(skill.descs);

                return true;
            }

            return false;
        }


        private static long GetCurLevelSkillID(long officer_id_, int lvl_)
        {

            lvl_ = 0 == lvl_ ? 1 : lvl_;
            int cur_level = lvl_;

            ConfOfficer exl_data = ConfOfficer.Get(officer_id_);

            long skill_id = -1;

            switch (cur_level)
            {
                case 1:
                    skill_id = exl_data.skillId;
                    break;
                case 2:
                    skill_id = exl_data.level2SkillId;
                    break;
                case 3:
                    skill_id = exl_data.level3SkillId;
                    break;
                case 4:
                    skill_id = exl_data.level4SkillId;
                    break;
                case 5:
                    skill_id = exl_data.level5SkillId;
                    break;
                case 6:
                    skill_id = exl_data.level6SkillId;
                    break;
                case 7:
                    skill_id = exl_data.level7SkillId;
                    break;
                case 8:
                    skill_id = exl_data.level8SkillId;
                    break;
                case 9:
                    skill_id = exl_data.level9SkillId;
                    break;
                case 10:
                    skill_id = exl_data.level10SkillId;
                    break;

            }

            return skill_id;
        }

        public static int GetOfficerVitCost(long officer_id_, int lvl_)
        {
            ConfOfficer officer = ConfOfficer.Get(officer_id_);

            int base_vit_cost = officer.vitConsume;
            int delta_vit = officer.upGainSceneVit;

            return base_vit_cost + (lvl_ - 1) * delta_vit;
        }

        /// <summary>
        /// 警员技能减少体力绝对值
        /// </summary>
        /// <param name="officer_id_"></param>
        /// <param name="lvl_"></param>
        /// <returns></returns>
        public static int GetOfficerSkillVitReduceInt(long officer_id_, int lvl_)
        {
            long skill_id = GetCurLevelSkillID(officer_id_, lvl_);

            if (skill_id < 0)
                return 0;

            var skill = ConfSkill.Get(skill_id);

            if (3 == skill.type)
            {
                return Math.Abs(skill.gain);
            }

            return 0;
        }


        /// <summary>
        /// 警员节能减少体力万分比
        /// </summary>
        /// <param name="officer_id_"></param>
        /// <param name="lvl_"></param>
        /// <returns></returns>
        public static int GetOfficerSkillVitReducePercent(long officer_id_, int lvl_)
        {
            long skill_id = GetCurLevelSkillID(officer_id_, lvl_);

            if (skill_id < 0)
                return 0;

            var skill = ConfSkill.Get(skill_id);

            if (4 == skill.type)
            {
                return Math.Abs(skill.gain);
            }

            return 0;
        }

        public static int GetOfficerTimeAddition(long officer_id_, int lvl_)
        {
            ConfOfficer officer = ConfOfficer.Get(officer_id_);

            int base_time_addition = officer.secondGain;
            int delta_time = officer.upGainSceneSceond;

            return base_time_addition + (lvl_ - 1) * delta_time;
        }


        public static int GetOfficerSkillTimeAddInt(long officer_id_, int lvl_)
        {
            long skill_id = GetCurLevelSkillID(officer_id_, lvl_);

            if (skill_id < 0)
                return 0;

            var skill = ConfSkill.Get(skill_id);

            if (1 == skill.type)
            {
                return skill.gain;
            }

            return 0;
        }


        public static int GetOfficerSkillTimeAddPercent(long officer_id_, int lvl_)
        {
            long skill_id = GetCurLevelSkillID(officer_id_, lvl_);

            if (skill_id < 0)
                return 0;

            var skill = ConfSkill.Get(skill_id);

            if (2 == skill.type)
            {
                return skill.gain;
            }

            return 0;
        }


        public static int GetTitleTimeAdditon(long title_id_)
        {
            if (0 == title_id_)
                return 0;

            ConfTitle title = ConfTitle.Get(title_id_);

            if (null == title)
                return 0;

            string benifit = title.benefit;


            List<TitleBenifitData> benifits = JsonSerializer.Deserialize<List<TitleBenifitData>>(benifit);

            foreach (var item in benifits)
            {
                if (3 == int.Parse(item.type))
                {
                    return int.Parse(item.value);
                }
            }

            return 0;
        }


        public static int GetTitleVitReduce(long title_id_)
        {
            if (0 == title_id_)
                return 0;

            ConfTitle title = ConfTitle.Get(title_id_);

            if (null == title)
                return 0;

            string benifit = title.benefit;

            List<TitleBenifitData> benifits = JsonSerializer.Deserialize<List<TitleBenifitData>>(benifit);

            foreach (var item in benifits)
            {
                if (2 == int.Parse(item.type))
                {
                    return Math.Abs(int.Parse(item.value));
                }
            }

            return 0;
        }
    }
}
