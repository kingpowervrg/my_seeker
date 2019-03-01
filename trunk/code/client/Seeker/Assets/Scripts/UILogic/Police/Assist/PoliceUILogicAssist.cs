#if OFFICER_SYS
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace SeekerGame
{
    public class UIOfficerInfo
    {
        public int m_lvl;
        public ConfOfficer m_data;
    }



    public class PoliceUILogicAssist
    {
        private static int OfficerCompare(UIOfficerInfo a, UIOfficerInfo b)
        {

            if (a.m_data.id == b.m_data.id)
                return 0;

            //Debug.Log(string.Format("a的 id = {0}, lvl = {1}, quality = {2} ; b的 id = {3}, lvl = {4}, quality = {5}", a.m_data.id, a.m_lvl, a.m_data.quality, b.m_data.id, b.m_lvl, b.m_data.quality));

            if (a.m_lvl > 0 && 0 == b.m_lvl)
            {
                //Debug.Log(string.Format("a{0} 在 b{1}前面",a.m_data.id,b.m_data.id));
                return -1;
            }
            else if (0 == a.m_lvl && b.m_lvl > 0)
            {
                //Debug.Log(string.Format("a{0} 在 b{1}后面", a.m_data.id, b.m_data.id));
                return 1;
            }
            else
            {
                if (a.m_data.quality > b.m_data.quality)
                {
                    //Debug.Log(string.Format("a{0} 在 b{1}前面", a.m_data.id, b.m_data.id));
                    return -1;
                }
                else if (a.m_data.quality < b.m_data.quality)
                {
                    //Debug.Log(string.Format("a{0} 在 b{1}后面", a.m_data.id, b.m_data.id));
                    return 1;
                }
                else
                {
                    if (a.m_lvl > b.m_lvl)
                    {
                        //Debug.Log(string.Format("a{0} 在 b{1}前面", a.m_data.id, b.m_data.id));
                        return -1;
                    }
                    else if (a.m_lvl < b.m_lvl)
                    {
                        //Debug.Log(string.Format("a{0} 在 b{1}后面", a.m_data.id, b.m_data.id));
                        return 1;
                    }
                    else
                    {
                        if (a.m_data.id < b.m_data.id)
                        {
                            //Debug.Log(string.Format("a{0} 在 b{1}前面", a.m_data.id, b.m_data.id));
                            return -1;
                        }
                        else if (a.m_data.id > b.m_data.id)
                        {
                            //Debug.Log(string.Format("a{0} 在 b{1}后面", a.m_data.id, b.m_data.id));
                            return 1;
                        }
                        else
                        {
                            //Debug.Log(string.Format("a{0} 在 b{1}前面", a.m_data.id, b.m_data.id));
                            return -1;
                        }
                    }
                }
            }



        }


        public static int OfficerCompare(OfficerInfo a, OfficerInfo b)
        {
            if (a.OfficerId == b.OfficerId)
                return 0;

            if (a.Level > 0 && 0 == b.Level)
            {
                return -1;
            }
            else if (0 == a.Level && b.Level > 0)
            {
                return 1;
            }
            else
            {
                if (ConfOfficer.Get(a.OfficerId).quality > ConfOfficer.Get(b.OfficerId).quality)
                {
                    return -1;
                }
                else if (ConfOfficer.Get(a.OfficerId).quality < ConfOfficer.Get(b.OfficerId).quality)
                {
                    return 1;
                }
                else
                {
                    if (a.Level > b.Level)
                    {
                        return -1;
                    }
                    else if (a.Level < b.Level)
                    {
                        return 1;
                    }
                    else
                    {
                        if (a.OfficerId < b.OfficerId)
                        {
                            return -1;
                        }
                        else if (a.OfficerId > b.OfficerId)
                        {
                            return 1;
                        }
                        else
                        {
                            return -1;
                        }
                    }
                }
            }



        }
        /// <summary>
        /// 警衔
        /// </summary>
        /// <param name="level_"></param>
        /// <returns></returns>
        public static string GetQualityString(int quality_)
        {
            switch (quality_)
            {
                case 1:
                    return "N";
                case 2:
                    return "R";
                case 3:
                    return "SR";
                case 4:
                    return "SSR";
            }

            return "";

        }


        public static List<UIOfficerInfo> GetOfficerByPageType(ENUM_PAGE_TYPE type_)
        {
            if (ENUM_PAGE_TYPE.E_ALL == type_)
            {
                return GetAllOfficers();
            }
            else
            {
                ENUM_POLICE_TYPE pt = ConvertOfficerType(type_);

                if (ENUM_POLICE_TYPE.E_INVALID == pt)
                {
                    return null;
                }

                return GetOfficerByPoliceType(pt);
            }
        }


        private static ENUM_POLICE_TYPE ConvertOfficerType(ENUM_PAGE_TYPE page_type_)
        {

            ENUM_POLICE_TYPE ret = ENUM_POLICE_TYPE.E_INVALID;

            switch (page_type_)
            {
                case ENUM_PAGE_TYPE.E_SPECIAL_POLICE:
                    ret = ENUM_POLICE_TYPE.E_SPECIAL_POLICE;
                    break;
                case ENUM_PAGE_TYPE.E_PATROL_MEN:
                    ret = ENUM_POLICE_TYPE.E_PATROL_MEN;
                    break;
                case ENUM_PAGE_TYPE.E_INSPECTOR:
                    ret = ENUM_POLICE_TYPE.E_INSPECTOR;
                    break;
                case ENUM_PAGE_TYPE.E_BAU:
                    ret = ENUM_POLICE_TYPE.E_BAU;
                    break;
                case ENUM_PAGE_TYPE.E_CSI:
                    ret = ENUM_POLICE_TYPE.E_CSI;
                    break;
                case ENUM_PAGE_TYPE.E_FORENSIC:
                    ret = ENUM_POLICE_TYPE.E_FORENSIC;
                    break;

            }

            return ret;
        }


        private static List<UIOfficerInfo> TransformConf2UI(List<ConfOfficer> conf_)
        {
            List<UIOfficerInfo> ret = new List<UIOfficerInfo>();

            List<ConfOfficer> ori_data = conf_;

            foreach (var item in ori_data)
            {
                OfficerInfo server_info = GlobalInfo.MY_PLAYER_INFO.GetOfficerInfo(item.id);

                if (null != server_info)
                {
                    UIOfficerInfo info = new UIOfficerInfo()
                    {
                        m_lvl = server_info.Level,
                        m_data = item,
                    };

                    ret.Add(info);
                }
                else
                {
                    UIOfficerInfo info = new UIOfficerInfo()
                    {
                        m_lvl = 0,
                        m_data = item,
                    };

                    ret.Add(info);
                }
            }
            ret.Sort(OfficerCompare);
            return ret;
        }

        public static List<UIOfficerInfo> GetAllOfficers()
        {
            List<ConfOfficer> ori_data = ConfOfficer.array;

            List<UIOfficerInfo> ret = TransformConf2UI(ori_data);

            return ret;
        }

        public static List<UIOfficerInfo> GetOfficerByPoliceType(ENUM_POLICE_TYPE type_)
        {
            List<ConfOfficer> ori_data;

            ori_data = ConfOfficer.array.FindAll((o) => { return o.profession == (int)type_; });

            List<UIOfficerInfo> ret = TransformConf2UI(ori_data);

            return ret;

        }

        public static OfficerInfo GetOfficerServerInfo(ConfOfficer ori_officer_)
        {
            long office_id_ = ori_officer_.id;
            return GlobalInfo.MY_PLAYER_INFO.GetOfficerInfo(office_id_);
        }

        public static ConfCombineFormula GetCombineInfo(ConfOfficer ori_officer_, int lvl_)
        {
            switch (lvl_)
            {
                case 1:
                    return ConfCombineFormula.Get(ori_officer_.unlockFormula);
                case 2:
                    return ConfCombineFormula.Get(ori_officer_.up2Formula);
                case 3:
                    return ConfCombineFormula.Get(ori_officer_.up3Formula);
                case 4:
                    return ConfCombineFormula.Get(ori_officer_.up4Formula);
                case 5:
                    return ConfCombineFormula.Get(ori_officer_.up5Formula);
                case 6:
                    return ConfCombineFormula.Get(ori_officer_.up6Formula);
                case 7:
                    return ConfCombineFormula.Get(ori_officer_.up7Formula);
                case 8:
                    return ConfCombineFormula.Get(ori_officer_.up8Formula);
                case 9:
                    return ConfCombineFormula.Get(ori_officer_.up9Formula);
                case 10:
                    return ConfCombineFormula.Get(ori_officer_.up10Formula);

            }

            return null;
        }

        public static string GetPoliceRankIcon(int lvl_)
        {
            lvl_ = 0 == lvl_ ? 1 : lvl_;
            return ConfPoliceRankIcon.Get(lvl_).icon;
        }

        
        private static readonly Color color_quality1 = new Color(137.0f / 255.0f, 254.0f / 255.0f, 110.0f / 255.0f);
        private static readonly Color color_quality2 = new Color(63.0f / 255.0f, 203.0f / 255.0f, 255.0f / 255.0f);
        private static readonly Color color_quality3 = new Color(255.0f / 255.0f, 86.0f / 255.0f, 255.0f / 255.0f);
        private static readonly Color color_quality4 = new Color(255.0f / 255.0f, 161.0f / 255.0f, 51.0f / 255.0f);

        public static Color GetPoliceQualityColor(int quality_)
        {
            switch (quality_)
            {
                case 1:
                    return color_quality1;
                case 2:
                    return color_quality2;
                case 3:
                    return color_quality3;
                case 4:
                    return color_quality4;
                default:
                    return Color.white;
            }
        }

        public static List<String> GetKeyWordsIcon(ConfOfficer info_)
        {
            List<String> ret = new List<string>();

            for (int i = 0; i < info_.features.Length; i++)
            {
                ret.Add(LocalizeModule.Instance.GetString(ConfKeyWords.Get(info_.features[i]).icon));
            }

            return ret;
        }

        public static List<String> GetKeyWords(ConfOfficer info_)
        {
            List<String> ret = new List<string>();

            for (int i = 0; i < info_.features.Length; i++)
            {
                ret.Add(LocalizeModule.Instance.GetString(ConfKeyWords.Get(info_.features[i]).word));
            }

            //string features = info_.features;

            //string[] ids = features.Split(';');

            //foreach (string id in ids)
            //{
            //    if (string.IsNullOrEmpty(id))
            //        continue;

            //    int id_int = int.Parse(id);

            //    ret.Add(LocalizeModule.Instance.GetString(ConfKeyWords.Get(id_int).word));
            //}

            return ret;
        }

    }


}
#endif