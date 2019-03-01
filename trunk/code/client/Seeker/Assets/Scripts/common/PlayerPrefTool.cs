using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Utf8Json;

namespace SeekerGame
{
    public enum ENUM_LOGIN_TYPE
    {
        E_GUEST,
        E_THIRD,
    }

    public enum ENUM_PREF_KEY
    {
        E_FRIEND_UI_APPLICATION_TOGGLE,
        E_GUEST_USERNAME,
        E_THIRD_USERNAME,
        E_TOKEN,
        E_FRIEND_APPLICATION_TIME,
        E_NEW_FRIEND,
        E_NEW_APPLY,
        E_NEW_GIFT,
        E_NEW_EMAIL,
        E_NEW_ACTIVITY,
        E_NEW_ACHIEVEMENT,
        E_NEW_NOTICE,
        E_NEW_POLICE,
        E_PLAYERID,
        E_SERVERIP,
        E_ENABLE_SOUND,
        E_ENABLE_MUSIC,
        E_ENABLE_PURCHASE,
        E_DISPATCH_0,
        E_DISPATCH_1,
        E_DISPATCH_2,
        E_DISPATCH_3,
        E_NET_ERROR,
        E_FIRST_TIME_LAUNCH,         //是否第一次启动游戏
        E_AD_CHANNEL, //推广渠道
        E_NEW_CHAPTER_NPC, //章节NPC
        E_NEW_CHAPTER_CLUE, //章节
        E_NEW_CHAPTER_SCENE, //章节场景
        E_POOL_TASK_ANCHOR, //池任务大地图，图标记录位置
        E_TASK_SCENE_ID, //任务存储的场景id
        E_BUILD_TOP_SCENE_ID, //建筑物场景存储的场景id
    }

    public class PlayerPrefTool
    {
        public const string BuildKeyLock = "BuildKeyLock_";
        public const string SceneTips = "SceneTips_";

        private static Dictionary<ENUM_LOGIN_TYPE, string> m_username_keys = new Dictionary<ENUM_LOGIN_TYPE, string>()
        {
            { ENUM_LOGIN_TYPE.E_GUEST, ENUM_PREF_KEY.E_GUEST_USERNAME.ToString()},
            { ENUM_LOGIN_TYPE.E_THIRD, ENUM_PREF_KEY.E_THIRD_USERNAME.ToString()},

        };


        public static void SetFriendApplication(bool on_)
        {
            int val = true == on_ ? 1 : 0;
            PlayerPrefs.SetInt(ENUM_PREF_KEY.E_FRIEND_UI_APPLICATION_TOGGLE.ToString(), val);
        }

        public static bool GetFriendApplication()
        {
            if (PlayerPrefs.HasKey(ENUM_PREF_KEY.E_FRIEND_UI_APPLICATION_TOGGLE.ToString()))
            {
                return 1 == PlayerPrefs.GetInt(ENUM_PREF_KEY.E_FRIEND_UI_APPLICATION_TOGGLE.ToString()) ? true : false;
            }

            return true;
        }

        public static string GetUsername(ENUM_LOGIN_TYPE type_)
        {
            //string _key = string.Format("{0}:{1}", m_username_keys[type_], GlobalInfo.SERVER_ADDRESS);
            string _key = string.Format("{0}", m_username_keys[type_]);

            if (PlayerPrefs.HasKey(_key))
            {
                return PlayerPrefs.GetString(_key);
            }

            return null;


        }

        public static void SetUsername(string user_name_, ENUM_LOGIN_TYPE type_)
        {
            //string _key = string.Format("{0}:{1}", m_username_keys[type_], GlobalInfo.SERVER_ADDRESS);
            string _key = string.Format("{0}", m_username_keys[type_]);

            if (string.IsNullOrEmpty(user_name_))
            {
                PlayerPrefs.DeleteKey(_key);
            }
            else
                PlayerPrefs.SetString(_key, user_name_);
        }


        public static void SetToken(string token_)
        {
            string _key = string.Format("{0}:{1}", ENUM_PREF_KEY.E_TOKEN.ToString(), GlobalInfo.SERVER_ADDRESS);

            if (string.IsNullOrEmpty(token_))
                PlayerPrefs.DeleteKey(_key);
            else
                PlayerPrefs.SetString(_key, token_);
        }

        public static string GetToken()
        {
            string _key = string.Format("{0}:{1}", ENUM_PREF_KEY.E_TOKEN.ToString(), GlobalInfo.SERVER_ADDRESS);
            if (PlayerPrefs.HasKey(_key))
            {
                return PlayerPrefs.GetString(_key);
            }

            return null;
        }

        /// <summary>
        /// 设置Bool类型PlayerPrefs
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public static void SetBool(string key, bool value)
        {
            PlayerPrefs.SetInt(key, value ? 1 : 0);
        }

        /// <summary>
        /// 获取Bool类型PlayerPrefs
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static bool GetBool(string key, bool defaultValue = true)
        {
            if (PlayerPrefs.HasKey(key))
                return PlayerPrefs.GetInt(key) == 1;

            if (defaultValue)
                SetBool(key, defaultValue);

            return defaultValue;
        }

        /// <summary>
        /// 是否有Key
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static bool HasKey(string key)
        {
            return PlayerPrefs.HasKey(key);
        }


        /// <summary>
        /// 刷新好友申请提示时间
        /// </summary>
        /// <returns></returns>
        public static bool RefreshApplicationNoticeTime()
        {
            String last_date_str = PlayerPrefs.GetString(ENUM_PREF_KEY.E_FRIEND_APPLICATION_TIME.ToString());
            DateTime now_date = System.DateTime.Now;

            if (!string.IsNullOrEmpty(last_date_str))
            {
                DateTime last_date = DateTime.Parse(last_date_str);

                int diff_hours = now_date.Subtract(last_date).Hours;

                if (diff_hours > 24)

                //int diff_minutes = now_date.Subtract(last_date).Minutes;

                //if(diff_minutes > 5)
                {
                    PlayerPrefs.SetString(ENUM_PREF_KEY.E_FRIEND_APPLICATION_TIME.ToString(), now_date.ToString());
                    return true;
                }
                else
                {
                    return false;
                }
            }


            PlayerPrefs.SetString(ENUM_PREF_KEY.E_FRIEND_APPLICATION_TIME.ToString(), now_date.ToString());
            return true;
        }


        public static bool GetIsThereNewFriend()
        {
            if (PlayerPrefs.HasKey(ENUM_PREF_KEY.E_NEW_FRIEND.ToString()))
            {

                return 1 == int.Parse(PlayerPrefs.GetString(ENUM_PREF_KEY.E_NEW_FRIEND.ToString()));
            }

            return false;
        }

        public static void SetIsThereNewFriend(bool val_)
        {
            string val = (true == val_ ? 1 : 0).ToString();
            PlayerPrefs.SetString(ENUM_PREF_KEY.E_NEW_FRIEND.ToString(), val);
        }


        public static bool GetIsThereNewApply()
        {
            if (PlayerPrefs.HasKey(ENUM_PREF_KEY.E_NEW_APPLY.ToString()))
            {

                return 1 == int.Parse(PlayerPrefs.GetString(ENUM_PREF_KEY.E_NEW_APPLY.ToString()));
            }

            return false;
        }

        public static void SetIsThereNewApply(bool val_)
        {
            string val = (true == val_ ? 1 : 0).ToString();
            PlayerPrefs.SetString(ENUM_PREF_KEY.E_NEW_APPLY.ToString(), val);
        }

        public static bool GetIsThereNewGift()
        {
            if (PlayerPrefs.HasKey(ENUM_PREF_KEY.E_NEW_GIFT.ToString()))
            {

                return 1 == int.Parse(PlayerPrefs.GetString(ENUM_PREF_KEY.E_NEW_GIFT.ToString()));
            }

            return false;
        }

        public static void SetIsThereNewGift(bool val_)
        {
            string val = (true == val_ ? 1 : 0).ToString();
            PlayerPrefs.SetString(ENUM_PREF_KEY.E_NEW_GIFT.ToString(), val);
        }


        public static bool GetIsThereNewEmail()
        {
            if (PlayerPrefs.HasKey(ENUM_PREF_KEY.E_NEW_EMAIL.ToString()))
            {
                return 1 == int.Parse(PlayerPrefs.GetString(ENUM_PREF_KEY.E_NEW_EMAIL.ToString()));
            }

            return false;
        }

        public static void SetIsThereNewEmail(bool val_)
        {
            string val = (true == val_ ? 1 : 0).ToString();
            PlayerPrefs.SetString(ENUM_PREF_KEY.E_NEW_EMAIL.ToString(), val);
        }


        public static bool GetIsThereNewActivity()
        {
            if (PlayerPrefs.HasKey(ENUM_PREF_KEY.E_NEW_ACTIVITY.ToString()))
            {
                return 1 == int.Parse(PlayerPrefs.GetString(ENUM_PREF_KEY.E_NEW_ACTIVITY.ToString()));
            }

            return false;
        }

        public static void SetIsThereNewActivity(bool val_)
        {
            string val = (true == val_ ? 1 : 0).ToString();
            PlayerPrefs.SetString(ENUM_PREF_KEY.E_NEW_ACTIVITY.ToString(), val);
        }


        public static bool GetIsThereNewAchievement()
        {
            if (PlayerPrefs.HasKey(ENUM_PREF_KEY.E_NEW_ACHIEVEMENT.ToString()))
            {
                return 1 == int.Parse(PlayerPrefs.GetString(ENUM_PREF_KEY.E_NEW_ACHIEVEMENT.ToString()));
            }

            return false;
        }

        public static void SetIsThereNewAchievement(bool val_)
        {
            string val = (true == val_ ? 1 : 0).ToString();
            PlayerPrefs.SetString(ENUM_PREF_KEY.E_NEW_ACHIEVEMENT.ToString(), val);
        }


        public static bool GetIsThereNewNotice()
        {
            if (PlayerPrefs.HasKey(ENUM_PREF_KEY.E_NEW_NOTICE.ToString()))
            {
                return 1 == int.Parse(PlayerPrefs.GetString(ENUM_PREF_KEY.E_NEW_NOTICE.ToString()));
            }

            return false;
        }

        public static void SetIsThereNewNotice(bool val_)
        {
            string val = (true == val_ ? 1 : 0).ToString();
            PlayerPrefs.SetString(ENUM_PREF_KEY.E_NEW_NOTICE.ToString(), val);
        }

        public static bool GetIsThereNewPolice()
        {
            if (PlayerPrefs.HasKey(ENUM_PREF_KEY.E_NEW_POLICE.ToString()))
            {
                return 1 == int.Parse(PlayerPrefs.GetString(ENUM_PREF_KEY.E_NEW_POLICE.ToString()));
            }

            return false;
        }

        public static void SetIsThereNewPolice(bool val_)
        {
            string val = (true == val_ ? 1 : 0).ToString();
            PlayerPrefs.SetString(ENUM_PREF_KEY.E_NEW_POLICE.ToString(), val);
        }

        #region 章节
        public static void SetIsThereNewChapterNPC(bool val_)
        {
            string val = (true == val_ ? 1 : 0).ToString();
            PlayerPrefs.SetString(ENUM_PREF_KEY.E_NEW_CHAPTER_NPC.ToString(), val);
        }
        public static bool GetIsThereNewChapterNPC()
        {
            if (PlayerPrefs.HasKey(ENUM_PREF_KEY.E_NEW_CHAPTER_NPC.ToString()))
            {
                return 1 == int.Parse(PlayerPrefs.GetString(ENUM_PREF_KEY.E_NEW_CHAPTER_NPC.ToString()));
            }

            return false;
        }

        public static void SetIsThereNewChapterClue(bool val_)
        {
            string val = (true == val_ ? 1 : 0).ToString();
            PlayerPrefs.SetString(ENUM_PREF_KEY.E_NEW_CHAPTER_CLUE.ToString(), val);
        }
        public static bool GetIsThereNewChapterClue()
        {
            if (PlayerPrefs.HasKey(ENUM_PREF_KEY.E_NEW_CHAPTER_CLUE.ToString()))
            {
                return 1 == int.Parse(PlayerPrefs.GetString(ENUM_PREF_KEY.E_NEW_CHAPTER_CLUE.ToString()));
            }

            return false;
        }

        public static void SetIsThereNewChapterScene(bool val_)
        {
            string val = (true == val_ ? 1 : 0).ToString();
            PlayerPrefs.SetString(ENUM_PREF_KEY.E_NEW_CHAPTER_SCENE.ToString(), val);
        }
        public static bool GetIsThereNewChapterScene()
        {
            if (PlayerPrefs.HasKey(ENUM_PREF_KEY.E_NEW_CHAPTER_SCENE.ToString()))
            {
                return 1 == int.Parse(PlayerPrefs.GetString(ENUM_PREF_KEY.E_NEW_CHAPTER_SCENE.ToString()));
            }

            return false;
        }
        #endregion

        //保存建筑信息
        public static bool GetBuildLockCache(long buildID)
        {
            string key = GlobalInfo.MY_PLAYER_ID + PlayerPrefTool.BuildKeyLock + buildID;
            if (PlayerPrefs.HasKey(key))
            {
                return true;
            }
            PlayerPrefs.SetInt(key, (int)buildID);
            return false;
        }

        private static bool m_needSceneTips = true;
        public static bool NeedSceneTips()
        {
            if (!m_needSceneTips)
            {
                return false;
            }
            string key = SceneTips + GlobalInfo.MY_PLAYER_ID;
            if (!PlayerPrefs.HasKey(key))
            {
                PlayerPrefs.SetInt(key, 1);
                m_needSceneTips = false;
                return true;
            }
            return false;
        }

        private static string GetKeyByIndex(int idx)
        {
            string key;

            switch (idx)
            {
                case 0:
                    key = ENUM_PREF_KEY.E_DISPATCH_0.ToString();
                    break;
                case 1:
                    key = ENUM_PREF_KEY.E_DISPATCH_1.ToString();
                    break;
                case 2:
                    key = ENUM_PREF_KEY.E_DISPATCH_2.ToString();
                    break;
                case 3:
                    key = ENUM_PREF_KEY.E_DISPATCH_3.ToString();
                    break;
                default:
                    key = string.Empty;
                    break;

            }

            return key;
        }

        public static void SaveDispatchOfficer(long player_id_, Dictionary<int, long> dict_)
        {
            {
                string key = string.Format("{0}#{1}", player_id_, ENUM_PREF_KEY.E_DISPATCH_0.ToString());
                PlayerPrefs.DeleteKey(key);
                key = string.Format("{0}#{1}", player_id_, ENUM_PREF_KEY.E_DISPATCH_1.ToString());
                PlayerPrefs.DeleteKey(key);
                key = string.Format("{0}#{1}", player_id_, ENUM_PREF_KEY.E_DISPATCH_2.ToString());
                PlayerPrefs.DeleteKey(key);
                key = string.Format("{0}#{1}", player_id_, ENUM_PREF_KEY.E_DISPATCH_3.ToString());
                PlayerPrefs.DeleteKey(key);
            }


            foreach (var item in dict_)
            {
                string idx_key = GetKeyByIndex(item.Key);
                if (string.IsNullOrEmpty(idx_key))
                    continue;
                string key = string.Format("{0}#{1}", player_id_, idx_key);

                PlayerPrefs.SetString(key, item.Value.ToString());
            }
        }

        public static Dictionary<int, long> LoadDispatchOfficer(long player_id_)
        {
            Dictionary<int, long> ret = new Dictionary<int, long>();

            string key = string.Format("{0}#{1}", player_id_, ENUM_PREF_KEY.E_DISPATCH_0.ToString());

            if (PlayerPrefs.HasKey(key))
            {
                long officer_id = long.Parse(PlayerPrefs.GetString(key));
                ret.Add(0, officer_id);
            }

            key = string.Format("{0}#{1}", player_id_, ENUM_PREF_KEY.E_DISPATCH_1.ToString());

            if (PlayerPrefs.HasKey(key))
            {
                long officer_id = long.Parse(PlayerPrefs.GetString(key));
                ret.Add(1, officer_id);
            }

            key = string.Format("{0}#{1}", player_id_, ENUM_PREF_KEY.E_DISPATCH_2.ToString());

            if (PlayerPrefs.HasKey(key))
            {
                long officer_id = long.Parse(PlayerPrefs.GetString(key));
                ret.Add(2, officer_id);
            }

            key = string.Format("{0}#{1}", player_id_, ENUM_PREF_KEY.E_DISPATCH_3.ToString());

            if (PlayerPrefs.HasKey(key))
            {
                long officer_id = long.Parse(PlayerPrefs.GetString(key));
                ret.Add(3, officer_id);
            }

            return ret;
        }

        public static void SetNetError(bool v_)
        {
            string key = ENUM_PREF_KEY.E_NET_ERROR.ToString();
            PlayerPrefs.SetInt(key, v_ ? 1 : 0);
        }

        public static bool GetNetError()
        {
            string key = ENUM_PREF_KEY.E_NET_ERROR.ToString();

            if (PlayerPrefs.HasKey(key))
            {
                int ret = PlayerPrefs.GetInt(key);

                return 1 == ret ? true : false;
            }

            return false;
        }

        public static void SetADChannel(string ad_id_)
        {
            PlayerPrefs.SetString(ENUM_PREF_KEY.E_AD_CHANNEL.ToString(), ad_id_);
        }

        public static string GetADChannel()
        {
            if (PlayerPrefs.HasKey(ENUM_PREF_KEY.E_AD_CHANNEL.ToString()))
            {
                return PlayerPrefs.GetString(ENUM_PREF_KEY.E_AD_CHANNEL.ToString());
            }

            return string.Empty;
        }


        public static void SetPoolTaskAnchor(List<TaskOnBuild> pool_tasks_)
        {
            string poolTaskData = JsonSerializer.ToJsonString<List<TaskOnBuild>>(pool_tasks_);
            PlayerPrefs.SetString(ENUM_PREF_KEY.E_POOL_TASK_ANCHOR.ToString(), poolTaskData);
        }


        public static List<TaskOnBuild> GetPoolTaskAnchor()
        {
            string json;
            List<TaskOnBuild> data = null;

            if (PlayerPrefs.HasKey(ENUM_PREF_KEY.E_POOL_TASK_ANCHOR.ToString()))
            {
                json = PlayerPrefs.GetString(ENUM_PREF_KEY.E_POOL_TASK_ANCHOR.ToString());
                data = JsonSerializer.Deserialize<List<TaskOnBuild>>(json);
            }

            return data;
        }


        static void SaveSceneIDs(string player_ref_key_, Dictionary<long, long> task2scene_dict)
        {
            if (0 == task2scene_dict.Count)
            {
                PlayerPrefs.DeleteKey(player_ref_key_);
                return;
            }

            using (MemoryStream stream = new MemoryStream())
            {
                // convert string to stream
                StreamWriter writer = new StreamWriter(stream);

                foreach (var kvp in task2scene_dict)
                {
                    string t2s_str = $"{kvp.Key}:{kvp.Value};";
                    writer.Write(t2s_str);
                }

                writer.Flush();

                // convert stream to string
                stream.Position = 0;
                StreamReader reader = new StreamReader(stream);
                string save_str = reader.ReadToEnd();
                PlayerPrefs.SetString(player_ref_key_, save_str);
            }
        }
        static void SaveSceneID(string player_ref_key_, long task_id_, long scene_id_)
        {
            Dictionary<long, long> task2scene_dict = GetSceneIDs(player_ref_key_);

            if (task2scene_dict.ContainsKey(task_id_))
            {
                task2scene_dict[task_id_] = scene_id_;
            }
            else
            {
                task2scene_dict.Add(task_id_, scene_id_);
            }


            SaveSceneIDs(player_ref_key_, task2scene_dict);
        }


        static Dictionary<long, long> GetSceneIDs(string player_ref_key_)
        {
            Dictionary<long, long> task2scene_dict = new Dictionary<long, long>();

            if (PlayerPrefs.HasKey(player_ref_key_))
            {
                //"task_id:scene_id;task_id:scene_id;"
                string task2scenes = PlayerPrefs.GetString(player_ref_key_);

                string[] task2scenes_array = task2scenes.Split(';');

                foreach (var task2sceneStr in task2scenes_array)
                {
                    if (string.IsNullOrEmpty(task2sceneStr))
                        continue;
                    //"task_id:scene_id"
                    string[] id_str = task2sceneStr.Split(':');

                    long task_id = long.Parse(id_str[0]);
                    long scene_id = long.Parse(id_str[1]);

                    task2scene_dict.Add(task_id, scene_id);
                }
            }

            return task2scene_dict;
        }


        static void RemoveSceneID(string player_ref_key_, long key_id_)
        {
            Dictionary<long, long> task2scene_dict = GetSceneIDs(player_ref_key_);

            if (0 == task2scene_dict.Count)
                return;

            if (task2scene_dict.ContainsKey(key_id_))
            {
                task2scene_dict.Remove(key_id_);
            }

            SaveSceneIDs(player_ref_key_, task2scene_dict);
        }


        public static Dictionary<long, long> GetTaskSceneIDs()
        {
            return GetSceneIDs(ENUM_PREF_KEY.E_TASK_SCENE_ID.ToString());
        }
        /// <summary>
        /// "task_id:scene_id;task_id:scene_id;"
        /// </summary>
        /// <param name="task_id_"></param>
        /// <param name="scene_id_"></param>
        public static void SaveTaskSceneID(long task_id_, long scene_id_)
        {
            SaveSceneID(ENUM_PREF_KEY.E_TASK_SCENE_ID.ToString(), task_id_, scene_id_);
        }


        public static void SaveTaskSceneIDs(Dictionary<long, long> task2scene_dict)
        {
            SaveSceneIDs(ENUM_PREF_KEY.E_TASK_SCENE_ID.ToString(), task2scene_dict);
        }



        public static long LoadTaskSceneID(long task_id_)
        {
            Dictionary<long, long> task2scene_dict = GetSceneIDs(ENUM_PREF_KEY.E_TASK_SCENE_ID.ToString());

            if (task2scene_dict.ContainsKey(task_id_))
            {
                return task2scene_dict[task_id_];
            }

            return -1;

        }
        public static void RemoveTaskSceneID(long task_id_)
        {
            RemoveSceneID(ENUM_PREF_KEY.E_TASK_SCENE_ID.ToString(), task_id_);

        }




        public static void SaveBuildTopSceneID(long scene_group_id_, long scene_id_)
        {
            SaveSceneID(ENUM_PREF_KEY.E_BUILD_TOP_SCENE_ID.ToString(), scene_group_id_, scene_id_);
        }



        public static long LoadBuildTopSceneID(long scene_group_id_)
        {
            Dictionary<long, long> task2scene_dict = GetSceneIDs(ENUM_PREF_KEY.E_BUILD_TOP_SCENE_ID.ToString());

            if (task2scene_dict.ContainsKey(scene_group_id_))
            {
                return task2scene_dict[scene_group_id_];
            }

            return -1;

        }
        public static void RemoveBuildTopSceneID(long scene_group_id_)
        {
            RemoveSceneID(ENUM_PREF_KEY.E_BUILD_TOP_SCENE_ID.ToString(), scene_group_id_);

        }



    }
}
