//#define PLATFORM_ID
using EngineCore;
using GOEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace SeekerGame
{
    public class TaskOnBuildManager : Singleton<TaskOnBuildManager>
    {
        private Dictionary<string, TaskAnchor> all_pool_anchors_dict;
        private HashSet<string> busy_pool_anchors_set;
        private HashSet<string> idle_pool_anchors_set;
        private Dictionary<long, string> pool_task_anchor_dict;


        private Dictionary<string, TaskAnchor> all_branch_anchors_dict;
        private Dictionary<string, long> occupied_branch_anchor_task_dict;


        public TaskOnBuildManager()
        {
            occupied_branch_anchor_task_dict = new Dictionary<string, long>();
        }

        public void SyncRecord()
        {
            pool_task_anchor_dict = new Dictionary<long, string>();

            LoadPoolTaskFromPlayerPref();
        }

        public void SyncTaskAnchors()
        {

            var pool_objs = GameEvents.BigWorld_Event.Listen_GetAllPoolAnchors.SafeInvoke();
            List<TaskAnchor> pool_anchors = new List<TaskAnchor>();
            foreach (var kvp in pool_objs)
            {
                TaskAnchor anchor = new TaskAnchor(kvp.Value, kvp.Key);
                pool_anchors.Add(anchor);
            }


            all_pool_anchors_dict = new Dictionary<string, TaskAnchor>();
            busy_pool_anchors_set = new HashSet<string>();
            idle_pool_anchors_set = new HashSet<string>();

            foreach (var item in pool_anchors)
            {
                all_pool_anchors_dict.Add(item.m_name, item);
                idle_pool_anchors_set.Add(item.m_name);
            }

            foreach (var kvp in pool_task_anchor_dict)
            {
                busy_pool_anchors_set.Add(kvp.Value);
                idle_pool_anchors_set.Remove(kvp.Value);
            }

            var branch_objs = GameEvents.BigWorld_Event.Listen_GetAllBranchAnchors.SafeInvoke();
            List<TaskAnchor> branch_anchors = new List<TaskAnchor>();
            foreach (var kvp in branch_objs)
            {
                TaskAnchor anchor = new TaskAnchor(kvp.Value, kvp.Key);
                branch_anchors.Add(anchor);
            }

            all_branch_anchors_dict = new Dictionary<string, TaskAnchor>();

            foreach (var item in branch_anchors)
            {
                all_branch_anchors_dict.Add(item.m_name, item);
            }

        }

        public HashSet<long> GetExistPoolTaskIDs()
        {
            HashSet<long> ret = new HashSet<long>();

            foreach (var item in pool_task_anchor_dict.Keys)
            {
                ret.Add(item);
            }

            return ret;
        }

        public TaskAnchor DePoolTaskAnchor(long pool_task_id_)
        {
            if (pool_task_anchor_dict.ContainsKey(pool_task_id_))
            {
                string anchor_name = pool_task_anchor_dict[pool_task_id_];

                return all_pool_anchors_dict[anchor_name];
            }

            int idx = UnityEngine.Random.Range(0, idle_pool_anchors_set.Count);
            string suitable_name = idle_pool_anchors_set.ElementAt(idx);

            busy_pool_anchors_set.Add(suitable_name);
            idle_pool_anchors_set.Remove(suitable_name);

            pool_task_anchor_dict.Add(pool_task_id_, suitable_name);

            return all_pool_anchors_dict[suitable_name];

        }



        //public void EnPoolTaskAnchor(long pool_task_id_)
        //{
        //    string quit_name = pool_task_anchor_dict[pool_task_id_];
        //    pool_task_anchor_dict.Remove(pool_task_id_);
        //    SavePoolTaskToPlayerPref();

        //    busy_pool_anchors_set.Remove(quit_name);
        //    idle_pool_anchors_set.Add(quit_name);
        //}


        public void RefreshPoolTaskAnchor(HashSet<long> new_pool_tasks_)
        {
            List<long> expired_task_ids = new List<long>();

            foreach (var item in pool_task_anchor_dict.Keys)
            {
                if (!new_pool_tasks_.Contains(item))
                {
                    //此条记录，过期
                    expired_task_ids.Add(item);
                    string expired_anchor_name = pool_task_anchor_dict[item];
                    busy_pool_anchors_set.Remove(expired_anchor_name);
                    idle_pool_anchors_set.Add(expired_anchor_name);
                }
            }

            foreach (var item in expired_task_ids)
            {
                pool_task_anchor_dict.Remove(item);
            }
        }



        private void LoadPoolTaskFromPlayerPref()
        {
            List<TaskOnBuild> task_bulids = PlayerPrefTool.GetPoolTaskAnchor();

            if (null == task_bulids)
                return;

            foreach (var item in task_bulids)
            {
                pool_task_anchor_dict.Add(item.TaskID, item.Name);

            }
        }


        public void SavePoolTaskToPlayerPref()
        {
            List<TaskOnBuild> pool_task_bulids = new List<TaskOnBuild>();

            foreach (var kvp in pool_task_anchor_dict)
            {
                TaskOnBuild item = new TaskOnBuild()
                {
                    Name = kvp.Value,
                    TaskID = (long)kvp.Key,
                };

                pool_task_bulids.Add(item);
            }

            PlayerPrefTool.SetPoolTaskAnchor(pool_task_bulids);

        }

        public TaskAnchor DeBranchTaskAnchor(long pool_task_id_)
        {

            string anchor_name = ConfTask.Get(pool_task_id_).anchor;

            if (occupied_branch_anchor_task_dict.ContainsKey(anchor_name))
            {
                if (occupied_branch_anchor_task_dict[anchor_name] != pool_task_id_)
                    //有其它分支任务占了
                    return null;
                else
                    //我自己
                    return all_branch_anchors_dict[anchor_name];
            }

            occupied_branch_anchor_task_dict.Add(anchor_name, pool_task_id_);

            return all_branch_anchors_dict[anchor_name];

        }


        public Dictionary<string, long> RefreshOccupiedBranchAnchor(HashSet<long> new_branch_tasks_)
        {
            List<string> expired_anchors = new List<string>();

            foreach (var kvp in occupied_branch_anchor_task_dict)
            {
                if (!new_branch_tasks_.Contains(kvp.Value))
                {
                    //新任务中，没有当前占用的，说明当前占用的已完成
                    expired_anchors.Add(kvp.Key);
                }
            }

            foreach (var item in expired_anchors)
            {
                occupied_branch_anchor_task_dict.Remove(item);
            }

            return occupied_branch_anchor_task_dict;
        }

    }

















   
}
