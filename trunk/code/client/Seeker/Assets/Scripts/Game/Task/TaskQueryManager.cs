using System;
using System.Collections.Generic;
using EngineCore;
namespace SeekerGame
{
    public class TaskQueryManager : Singleton<TaskQueryManager>
    {
        public class CompleteTasks
        {
            public long m_last_complete_main_task_id = 0L;

            public HashSet<long> m_last_complete_other_task_ids = new HashSet<long>();

            public void Clear()
            {
                m_last_complete_other_task_ids.Clear();
                m_last_complete_main_task_id = 0L;
            }
        }

        private CompleteTasks m_allCompleteTask = new CompleteTasks();
        public SeekerGame.TaskQueryManager.CompleteTasks AllCompleteTask
        {
            get { return m_allCompleteTask; }
        }
        private bool getCompleteLock = false;

        public const long ERRORID = -1;

        public TaskQueryManager()
        {
            GameEvents.TaskEvents.OnTaskFinish += OnCompletedTask;
        }

        public bool IsNeedUpdate()
        {
            return 0 == AllCompleteTask.m_last_complete_main_task_id;
        }

        public bool QueryTaskComplete(long task_id_)
        {
            ConfTask t = ConfTask.Get(task_id_);

            if ((int)EUNM_TASK_TYPE.E_MAIN == t.type)
            {
                return AllCompleteTask.m_last_complete_main_task_id >= task_id_;
            }
            else
            {
                return AllCompleteTask.m_last_complete_other_task_ids.Contains(task_id_);
            }
        }

        public void AddCompleteTask(long task_id_, int task_type_)
        {


            if (1 == task_type_)
            {
                AllCompleteTask.m_last_complete_main_task_id = task_id_;
            }
            else
            {
                if (!AllCompleteTask.m_last_complete_other_task_ids.Contains(task_id_))
                {
                    AllCompleteTask.m_last_complete_other_task_ids.Add(task_id_);
                }
            }

        }

        public float GetMainTaskProgress()
        {
            long chapter_id = AllCompleteTask.m_last_complete_main_task_id / 10000;
            List<long> tasks = new List<long>(ConfChapter.Get(chapter_id).taskIds);

            int rewarded_count = tasks.IndexOf(AllCompleteTask.m_last_complete_main_task_id) + 1;

            return (float)rewarded_count / (float)tasks.Count;
        }

        private void OnCompletedTask(TaskBase task)
        {
            UnityEngine.Debug.Log("task complete === " + task.TaskConfID);
            AddCompleteTask(task.TaskConfID, ConfTask.Get(task.TaskConfID).type);
            GameEvents.BigWorld_Event.OnReflashBuidTask.SafeInvoke();
        }

        public long GetCurrentMainTask()
        {
            TaskSystem taskSystem = GlobalInfo.MY_PLAYER_INFO.PlayerTaskSystem;
            if (taskSystem == null)
            {
                return ERRORID;
            }
            for (int i = 0; i < taskSystem.CurrentTaskList.Count; i++)
            {
                NormalTask normalTask = taskSystem.CurrentTaskList[i] as NormalTask;
                if (normalTask.TaskData.type == 1)
                {
                    return normalTask.TaskConfID;
                }
            }
            return ERRORID;

        }


        public NormalTask GetCurrentMainTaskInfo()
        {
            TaskSystem taskSystem = GlobalInfo.MY_PLAYER_INFO.PlayerTaskSystem;
            if (taskSystem == null)
            {
                return null;
            }
            for (int i = 0; i < taskSystem.CurrentTaskList.Count; i++)
            {
                NormalTask normalTask = taskSystem.CurrentTaskList[i] as NormalTask;
                if (normalTask.TaskData.type == 1)
                {
                    return normalTask;
                }
            }
            return null;

        }

        public void RequestTaskComplete()
        {
            TaskQueryManager.Instance.AllCompleteTask.Clear();
            CSGetAllRewardedTasksRequest req = new CSGetAllRewardedTasksRequest();
            GameEvents.NetWorkEvents.SendAsyncMsg.SafeInvoke(req);
        }

        public bool QueryMainTaskComplete(long taskID)
        {
            return AllCompleteTask.m_last_complete_main_task_id >= taskID;
        }

        public long CheckBuildIsInTask(long sceneID)
        {
            TaskSystem taskSystem = GlobalInfo.MY_PLAYER_INFO.PlayerTaskSystem;
            if (taskSystem == null)
            {
                return ERRORID;
            }
            for (int i = 0; i < taskSystem.CurrentTaskList.Count; i++)
            {
                NormalTask normalTask = taskSystem.CurrentTaskList[i] as NormalTask;
                if (normalTask.TaskData.type == 1)
                {
                    //目前只有场景寻物
                    if (normalTask.TaskData.conditionSceneId / 1000 == sceneID)
                    {
                        return normalTask.TaskData.conditionSceneId;
                    }
                }
            }
            return ERRORID;
        }

        public override void Destroy()
        {
            base.Destroy();
            AllCompleteTask.Clear();
            GameEvents.TaskEvents.OnTaskFinish -= OnCompletedTask;
        }

    }
}
