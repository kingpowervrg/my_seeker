//#define TEST
using EngineCore;
using Google.Protobuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace SeekerGame
{
    [UILogicHandler(UIDefine.UI_TASK_ON_BUILD)]
    class TaskOnBuildUILogic : BaseViewComponetLogic
    {
        private const int C_MAX_POOL_TASK_NUM = 3;

        private TweenScale m_tweenPos = null;
        TaskOnBuildView m_pool_view;
        TaskOnBuildView m_branch_view;
        UITaskOnBuildData m_old_data = null;

        public override void OnPackageRequest(IMessage imsg, params object[] msg_params)
        {

            base.OnPackageRequest(imsg, msg_params);



        }

        public override void OnScResponse(object s)
        {
            base.OnScResponse(s);



            if (s is SCFriendResponse)
            {
                var rsp = s as SCFriendResponse;

                if (MsgStatusCodeUtil.OnError(rsp.Status))
                    return;


            }
        }
        public override FrameDisplayMode UIFrameDisplayMode
        {
            get
            {
                return FrameDisplayMode.WINDOWED;
            }
        }
        private void RefreshView(UITaskOnBuildData data)
        {
            RefreshPoolView(data.m_pool_task);
            RefreshBranchView(data.m_branch_task);

        }

        protected override void OnInit()
        {
            base.OnInit();
            m_pool_view = Make<TaskOnBuildView>("Pool");
            m_branch_view = Make<TaskOnBuildView>("Branch");
        }

        public override void OnShow(object param)
        {
            base.OnShow(param);

            if (null != param)
            {
                UITaskOnBuildData data = param as UITaskOnBuildData;

                RefreshView(data);


            }


            GameEvents.BigWorld_Event.OnReflashScreen += RefreshPos;
            GameEvents.BigWorld_Event.Listen_ShowTaskOnBuild += RefreshView;


        }

        public override void OnHide()
        {
            base.OnHide();


            //MessageHandler.UnRegisterMessageHandler(MessageDefine.SCFriendResponse, OnScResponse);

            GameEvents.BigWorld_Event.OnReflashScreen -= RefreshPos;
            GameEvents.BigWorld_Event.Listen_ShowTaskOnBuild -= RefreshView;

        }










        private void RefreshPoolView(List<TaskBase> pool_tasks_)
        {
            HashSet<long> new_task_ids = new HashSet<long>();
            Dictionary<long, TaskBase> new_task_datas = new Dictionary<long, TaskBase>();
            pool_tasks_.ForEach((item) => { new_task_ids.Add(item.TaskConfID); new_task_datas.Add(item.TaskConfID, item); });
            TaskOnBuildManager.Instance.RefreshPoolTaskAnchor(new_task_ids);



            HashSet<long> exist_task_ids = TaskOnBuildManager.Instance.GetExistPoolTaskIDs();
            List<TaskBase> valid_tasks = new List<TaskBase>();
            foreach (var item in exist_task_ids)
            {
                valid_tasks.Add(new_task_datas[item]);
                new_task_datas.Remove(item);
            }

            if (C_MAX_POOL_TASK_NUM == exist_task_ids.Count)
            {
                m_pool_view.Refresh(valid_tasks, 3);
                return;
            }

            //少于3个
            int left_count = C_MAX_POOL_TASK_NUM - exist_task_ids.Count;
            left_count = left_count <= new_task_datas.Count ? left_count : new_task_datas.Count;
            for (int i = 0; i < left_count; ++i)
            {
                long task_id = new_task_datas.Keys.ElementAt(0);
                valid_tasks.Add(new_task_datas[task_id]);
                new_task_datas.Remove(task_id);
            }

            m_pool_view.Refresh(valid_tasks, 3);

            TaskOnBuildManager.Instance.SavePoolTaskToPlayerPref();
        }

        private void RefreshBranchView(List<TaskBase> new_branch_tasks_)
        {
            HashSet<long> new_branch_task_ids = new HashSet<long>();
            new_branch_tasks_.ForEach((item) => new_branch_task_ids.Add(item.TaskConfID));

            Dictionary<string, long> occupied_anchors = TaskOnBuildManager.Instance.RefreshOccupiedBranchAnchor(new_branch_task_ids);
            Dictionary<string, long> temp_occupied_anchors = new Dictionary<string, long>(occupied_anchors);

            List<TaskBase> valid_tasks = new List<TaskBase>();

            foreach (var item in new_branch_tasks_)
            {
                NormalTask task = (item as NormalTask);
                string anchor_name = task.TaskData.anchor;

                if (temp_occupied_anchors.ContainsKey(anchor_name))
                {
                    if (temp_occupied_anchors[anchor_name] != task.TaskConfID)
                        continue;
                }
                else
                {
                    temp_occupied_anchors.Add(anchor_name, task.TaskConfID);

                }

                valid_tasks.Add(task);
            }

            m_branch_view.Refresh(valid_tasks, 2);
        }

        private void RefreshPos()
        {
            m_pool_view.RefreshPos();
            m_branch_view.RefreshPos();
        }

        public GameObject GetCanvas()
        {
            return root;
        }
    }
}

