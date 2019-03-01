using EngineCore;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace SeekerGame
{
    public class FindObjSceneDataManager : Singleton<FindObjSceneDataManager>
    {
        public const int MAX_LVL = 10;

        Dictionary<long, FindObjSceneData> m_dict = new Dictionary<long, FindObjSceneData>();

        public FindObjSceneDataManager()
        {
            MessageHandler.RegisterMessageHandler(MessageDefine.SCSceneDifficultyResp, OnScResponse);
            MessageHandler.RegisterMessageHandler(MessageDefine.SCSceneRewardResponse, OnScResponse);
        }

        public static long ConvertSceneIdToSceneGroupId(long scene_id_)
        {
            if (scene_id_ > 99999)
                return scene_id_ / 1000;
            else
                return scene_id_;
        }

        public static int? GetFullExp(long group_id_, int lvl_)
        {
            if (lvl_ != FindObjSceneDataManager.MAX_LVL)
            {
                string difficult = string.Format("{0:D2}", lvl_);
                string difficult_id_str = $"{group_id_}{difficult}";
                long difficult_id = long.Parse(difficult_id_str);
                return ConfSceneDifficulty.Get(difficult_id)?.count;
            }

            return 0;
        }

        public static long GetSceneDifficultID(long group_id_, int lvl_)
        {

            string difficult = string.Format("{0:D2}", lvl_);
            string difficult_id_str = $"{group_id_}{difficult}";
            long difficult_id = long.Parse(difficult_id_str);

            return difficult_id;
        }

        void OnScResponse(object s)
        {

            if (s is SCSceneDifficultyResp)
            {
                var rsp = s as SCSceneDifficultyResp;

                m_dict.Clear();

                foreach (SceneDifficultyInfo info in rsp.Infos)
                {
                    FindObjSceneData data = new FindObjSceneData(info.BigId, info.Difficulty, info.Exp);
                    m_dict.Add(data.m_scene_group_id, data);

                    Debug.Log($"同步关卡难度 关卡组id{info.BigId}  关卡当前难度{info.Difficulty}  关卡当前经验{info.Exp}");
                }
            }
            else if (s is SCSceneRewardResponse)
            {
                var rsp = s as SCSceneRewardResponse;

                long scene_id = rsp.SceneId;

                FindObjSceneData data = GetDataBySceneID(scene_id);


                Debug.Log($"关卡结算 关卡id{scene_id}  关卡当前难度{rsp.Difficulty}  关卡当前经验{rsp.Exp}");

                if (null != data)
                {
                    data.m_lvl = rsp.Difficulty;
                    data.m_exp = rsp.Exp;
                }
                else
                {
                    long scene_group_id = ConvertSceneIdToSceneGroupId(scene_id);
                    data = new FindObjSceneData(scene_group_id, rsp.Difficulty, rsp.Exp);
                    m_dict.Add(scene_group_id, data);
                }

                //删除保存在player ref 里的场景id记录
                //删除build top相关的场景记录(任务的记录，交给任务系统清空)
                RemoveSceneIDForBuildTop(ConvertSceneIdToSceneGroupId(scene_id));
            }

        }


        public FindObjSceneData GetDataBySceneGroupID(long scene_group_id_)
        {
            if (!m_dict.ContainsKey(scene_group_id_))
                return null;
            return m_dict[scene_group_id_];
        }

        public FindObjSceneData GetDataBySceneID(long scene_id_)
        {
            long scene_group_id_ = ConvertSceneIdToSceneGroupId(scene_id_);

            if (!m_dict.ContainsKey(scene_group_id_))
                return null;

            return m_dict[scene_group_id_];
        }

        public void RefreshAllInfo(long cur_chapter_id_)
        {
            HashSet<long> scene_group_ids = new HashSet<long>();

            HashSet<long> one_chapter_scene_group_ids = new HashSet<long>();
            for (int i = 1; i <= cur_chapter_id_; ++i)
            {
                one_chapter_scene_group_ids.Clear();
                long[] scene_ids = ConfChapter.Get(i).scenceIds;

                for (int j = 0; j < scene_ids.Length; ++j)
                {
                    long scene_id = scene_ids[j];
                    long scene_group_id = ConvertSceneIdToSceneGroupId(scene_id);
                    one_chapter_scene_group_ids.Add(scene_group_id);
                }

                scene_group_ids.UnionWith(one_chapter_scene_group_ids);
            }

            CSSceneDifficultyReq req = new CSSceneDifficultyReq();
            req.BigIds.AddRange(scene_group_ids);
            GameEvents.NetWorkEvents.SendAsyncMsg.SafeInvoke(req);
        }

        public static void SaveSceneIDForTask(long task_id_, long scene_id_)
        {
            PlayerPrefTool.SaveTaskSceneID(task_id_, scene_id_);
        }

        public static long LoadSceneIDForTask(long task_id_)
        {
            return PlayerPrefTool.LoadTaskSceneID(task_id_);
        }

        public static void RemoveSceneIDForTask(long task_id_)
        {
            PlayerPrefTool.RemoveTaskSceneID(task_id_);
        }


        public static void RefreshSceneIDsForTasks(HashSet<long> currentProgressingTaskIDs_set)
        {
            var task_scenes = PlayerPrefTool.GetTaskSceneIDs();

            if (0 == task_scenes.Count)
                return;

            var saved_task_ids = task_scenes.Keys;

            HashSet<long> saved_task_ids_set = new HashSet<long>(saved_task_ids);

            //找出无效任务id
            saved_task_ids_set.ExceptWith(currentProgressingTaskIDs_set);
            HashSet<long> invalid_task_ids_set = saved_task_ids_set;

            foreach (var id in invalid_task_ids_set)
            {
                task_scenes.Remove(id);
            }

            PlayerPrefTool.SaveTaskSceneIDs(task_scenes);
        }



        public static void SaveSceneIDForBuildTop(long scene_group_id_, long scene_id_)
        {
            PlayerPrefTool.SaveBuildTopSceneID(scene_group_id_, scene_id_);
        }

        public static long LoadSceneIDForBuildTop(long scene_group_id_)
        {
            return PlayerPrefTool.LoadBuildTopSceneID(scene_group_id_);
        }

        public static void RemoveSceneIDForBuildTop(long scene_group_id_)
        {
            PlayerPrefTool.RemoveBuildTopSceneID(scene_group_id_);
        }
    }


    public class FindObjSceneData
    {
        public long m_scene_group_id;
        public int m_lvl;
        public int m_exp;
        public int m_full_exp;

        public FindObjSceneData(long group_scene_id_, int lvl_, int exp_)
        {
            m_scene_group_id = group_scene_id_;
            m_lvl = lvl_;
            m_exp = exp_;
            m_full_exp = FindObjSceneDataManager.GetFullExp(m_scene_group_id, m_lvl).GetValueOrDefault();
        }



        public void RefreshLvl(int lvl_)
        {
            if (m_lvl != lvl_)
            {
                //更新full exp
                m_lvl = lvl_;

                m_full_exp = FindObjSceneDataManager.GetFullExp(m_scene_group_id, m_lvl).GetValueOrDefault();
            }
        }

    }
}
