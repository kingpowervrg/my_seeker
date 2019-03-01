using EngineCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace SeekerGame
{
    public class TaskHelper
    {
        public static void OnTaskItemClick(NormalTask taskInfo, Action Close_Myself_UI)
        {
            EngineCoreEvents.AudioEvents.PlayAudio.SafeInvoke(Audio.AudioType.UISound, GameCustomAudioKey.task_button.ToString());
            if (taskInfo.TaskCurrentStatus == TaskStatus.PROGRESSING)
            {
                AcceptTask(taskInfo);
            }
            else if (taskInfo.TaskCurrentStatus == TaskStatus.COMPLETED)
            {
                GetReward(taskInfo, Close_Myself_UI);

            }
        }


        public static void AcceptTask(NormalTask taskInfo)
        {


            Dictionary<UBSParamKeyName, object> _param = new Dictionary<UBSParamKeyName, object>();
            _param.Add(UBSParamKeyName.ContentID, taskInfo.TaskConfID);
            UserBehaviorStatisticsModules.Instance.LogEvent(UBSEventKeyName.touch_task, null, _param);

            if (taskInfo.CompleteConditionList.Count > 0)
            {
                if (taskInfo.TaskData.type == 1)
                {
                    OnCheckMainTaskCanStart(taskInfo);
                }
                else
                    OnCheckTaskCanStart(taskInfo);
            }
        }
        public static void GetReward(NormalTask taskInfo, Action Close_Myself_UI, bool delay_show_reward_view_ = false)
        {
            if (taskInfo.TaskCurrentStatus != TaskStatus.COMPLETED)
                return;

            if (delay_show_reward_view_)
            {
                GameEvents.UIEvents.UI_Bonus_Pop_View_Event.Tell_OnCache.SafeInvoke(EUNM_BONUS_POP_VIEW_TYPE.E_TASK_REWARD);
                GameEvents.UIEvents.UI_Bonus_Pop_View_Event.Tell_OnShow.SafeInvoke();

            }
            else
            {
                GameEvents.TaskEvents.OnCollectTaskReward.SafeInvoke(taskInfo);
                Close_Myself_UI();
            }
        }

        /// <summary>
        /// vit,coin,cash,exp
        /// </summary>
        /// <param name="taskConfig"></param>
        /// <returns></returns>
        public static Tuple<int, int, int, int> GetReward(ConfTask taskConfig)
        {
            return Tuple.Create<int, int, int, int>(taskConfig.rewardVit, taskConfig.rewardCoin, taskConfig.rewardCash, taskConfig.rewardExp);
        }

        private static void OnCheckTaskCanStart(NormalTask taskInfo)
        {
            CSCanTaskRequest req = new CSCanTaskRequest();
            req.TaskId = taskInfo.TaskConfID;
            GameEvents.NetWorkEvents.SendMsg.SafeInvoke(req);
        }

        /// <summary>
        /// 此主线任务是否能完成
        /// </summary>
        private static void OnCheckMainTaskCanStart(NormalTask taskInfo)
        {

            ConfTask confTask = ConfTask.Get(taskInfo.TaskConfID);

            if (confTask == null)
            {
                return;
            }

            if (confTask.building != null && confTask.building.Length > 0)
            {
                ConfBuilding building = ConfBuilding.Get(confTask.building[0]);
                if (building == null)
                {
                    return;
                }

                if (!GameEvents.BigWorld_Event.OnCheckBuildStatusByID(building.id))
                {
                    PopUpManager.OpenNormalOnePop("task_unlock_tips", LocalizeModule.Instance.GetString(building.name));
                    return;
                }
            }

            GameEntryUILogic.S_CUR_EXCUTE_TASK_ID = taskInfo.TaskConfID;
            OnExcuteTask(taskInfo);


        }

        //private static void OnExcuteTask(NormalTask taskInfo)
        //{

        //    if (taskInfo.CompleteConditionList.Count <= 1)
        //    { //目录没有组合条件
        //        TaskCompleteCondition taskCompleteCondition = taskInfo.CompleteConditionList[0];
        //        TaskCompleteAttribute taskCompleteAttribute = taskCompleteCondition.GetType().GetCustomAttributes(typeof(TaskCompleteAttribute), true)[0] as TaskCompleteAttribute;
        //        switch (taskCompleteAttribute.CompleteMode)
        //        {
        //            case TaskCompleteMode.CompletedByDialog:
        //                TalkUIHelper.OnStartTalk((long)taskCompleteCondition.TaskCompleteData);

        //                break;
        //            case TaskCompleteMode.CompletedBySceneID:
        //                long sceneID = (long)taskCompleteCondition.TaskCompleteData;
        //                ChapterInfo taskBelongChapterInfo = GetTaskBelongChapter(taskInfo.TaskConfID);
        //                CommonHelper.OpenEnterGameSceneUI(sceneID, taskBelongChapterInfo, taskInfo.TaskConfID);
        //                break;

        //            //case TaskCompleteMode.CompletedByEvents:
        //            //    EventGameUIAssist.BeginEventGame((long)taskCompleteCondition.TaskCompleteData);
        //            //    break;

        //            case TaskCompleteMode.CompletedByItem:
        //                GameEvents.TaskEvents.OnShowCollectionTaskDetail.SafeInvoke(taskInfo, ShowSceneInBigWorld);
        //                break;
        //            case TaskCompleteMode.CompleteByReasonID:
        //                ReasoningUILogic.ShowReasonUIById((long)taskCompleteCondition.TaskCompleteData);
        //                break;
        //            case TaskCompleteMode.CompleteByScanID:
        //                long scanID = (long)taskCompleteCondition.TaskCompleteData;
        //                ChapterInfo scanTaskBelongChapterInfo = GetTaskBelongChapter(taskInfo.TaskConfID);
        //                CommonHelper.OpenEnterGameSceneUI(scanID, scanTaskBelongChapterInfo, taskInfo.TaskConfID);
        //                break;
        //            default:
        //                break;
        //        }
        //    }
        //    else
        //    {
        //        TaskCompleteCondition c_items = taskInfo.CompleteConditionList.Find((item) => item.GetType() == typeof(TaskCompleteItems));

        //        if (null != c_items)
        //        {
        //            TaskCompleteCondition c_scene = taskInfo.CompleteConditionList.Find((item) => item.GetType() == typeof(TaskCompleteByScene));

        //            Action<long> close_act = null;

        //            if (null != c_scene)
        //            {
        //                close_act = (task_conf_id) =>
        //                {
        //                    long task_config_id = task_conf_id;
        //                    long sceneID = (long)c_scene.TaskCompleteData;
        //                    ChapterInfo taskBelongChapterInfo = GetTaskBelongChapter(taskInfo.TaskConfID);
        //                    CommonHelper.OpenEnterGameSceneUI(sceneID, taskBelongChapterInfo, taskInfo.TaskConfID);
        //                };
        //            }

        //            GameEvents.TaskEvents.OnShowCollectionTaskDetail.SafeInvoke(taskInfo, close_act);
        //        }
        //    }


        //}


        private static void OnExcuteTask(NormalTask taskInfo)
        {

            if (taskInfo.CompleteConditionList.Count <= 1)
            { //目录没有组合条件
                TaskCompleteCondition taskCompleteCondition = taskInfo.CompleteConditionList[0];
                TaskCompleteAttribute taskCompleteAttribute = taskCompleteCondition.GetType().GetCustomAttributes(typeof(TaskCompleteAttribute), true)[0] as TaskCompleteAttribute;
                switch (taskCompleteAttribute.CompleteMode)
                {
                    case TaskCompleteMode.CompletedByDialog:
                        {
                            TalkUIHelper.OnStartTalk((long)taskCompleteCondition.TaskCompleteData);

                        }

                        break;
                    case TaskCompleteMode.CompletedBySceneID:
                        {

                            Action<long> close_act = null;

                            close_act = (task_conf_id) =>
                            {
                                long sceneID = (long)taskCompleteCondition.TaskCompleteData;
                                ChapterInfo taskBelongChapterInfo = GetTaskBelongChapter(taskInfo.TaskConfID);
                                CommonHelper.OpenEnterGameSceneUI(sceneID, taskBelongChapterInfo, taskInfo.TaskConfID);
                            };


                            GameEvents.TaskEvents.OnTryShowCollectionTaskDetail.SafeInvoke(taskInfo, close_act);

                        }
                        break;

                    //case TaskCompleteMode.CompletedByEvents:
                    //    EventGameUIAssist.BeginEventGame((long)taskCompleteCondition.TaskCompleteData);
                    //    break;

                    case TaskCompleteMode.CompletedByItem:
                        {
                            GameEvents.TaskEvents.OnTryShowCollectionTaskDetail.SafeInvoke(taskInfo, ShowSceneInBigWorld);
                        }
                        break;
                    case TaskCompleteMode.CompleteByReasonID:
                        {

                            Action<long> close_act = null;

                            close_act = (task_conf_id) =>
                            {
                                ReasoningUILogic.ShowReasonUIById((long)taskCompleteCondition.TaskCompleteData);
                            };

                            GameEvents.TaskEvents.OnTryShowCollectionTaskDetail.SafeInvoke(taskInfo, close_act);
                        }
                        break;
                    case TaskCompleteMode.CompleteByCombinePropID:
                        {

                            Action<long> close_act = null;

                            close_act = (task_conf_id) =>
                            {
                                long[] ids = ConfTask.Get(task_conf_id).conditionPropExIds;

                                if (ids.Length > 0)
                                {
                                    FrameMgr.OpenUIParams uiParams = new FrameMgr.OpenUIParams(UIDefine.UI_COMBINE);
                                    uiParams.Param = ids[0];

                                    EngineCoreEvents.UIEvent.ShowUIEventWithParam.SafeInvoke(uiParams);
                                }
                                else
                                    EngineCoreEvents.UIEvent.ShowUIEvent.SafeInvoke(UIDefine.UI_COMBINE);

                            };

                            GameEvents.TaskEvents.OnTryShowCollectionTaskDetail.SafeInvoke(taskInfo, close_act);
                        }
                        break;
                    case TaskCompleteMode.CompleteByScanID:

                        {
                            Action<long> close_act = null;

                            close_act = (task_conf_id) =>
                            {
                                long scanID = (long)taskCompleteCondition.TaskCompleteData;
                                ChapterInfo scanTaskBelongChapterInfo = GetTaskBelongChapter(taskInfo.TaskConfID);
                                CommonHelper.OpenEnterGameScanUI(scanID, scanTaskBelongChapterInfo, taskInfo.TaskConfID);
                            };

                            GameEvents.TaskEvents.OnTryShowCollectionTaskDetail.SafeInvoke(taskInfo, close_act);
                        }
                        break;
                    default:
                        break;
                }
            }
            else
            {
                TaskCompleteCondition c_items = taskInfo.CompleteConditionList.Find((item) => item.GetType() == typeof(TaskCompleteItems));

                if (null != c_items)
                {
                    TaskCompleteCondition c_scene = taskInfo.CompleteConditionList.Find((item) => item.GetType() == typeof(TaskCompleteByScene));

                    Action<long> close_act = null;

                    if (null != c_scene)
                    {
                        close_act = (task_conf_id) =>
                        {
                            long task_config_id = task_conf_id;
                            long sceneID = (long)c_scene.TaskCompleteData;
                            ChapterInfo taskBelongChapterInfo = GetTaskBelongChapter(taskInfo.TaskConfID);
                            CommonHelper.OpenEnterGameSceneUI(sceneID, taskBelongChapterInfo, taskInfo.TaskConfID);
                        };
                    }

                    GameEvents.TaskEvents.OnTryShowCollectionTaskDetail.SafeInvoke(taskInfo, close_act);
                }
            }


        }



        private static ChapterInfo GetTaskBelongChapter(long taskConfigID)
        {
            return GlobalInfo.MY_PLAYER_INFO.PlayerChapterSystem.FindChapterByTaskID(taskConfigID);
        }

        private static void ShowSceneInBigWorld(long task_config_id_)
        {
            var task = ConfTask.Get(task_config_id_);
            if (3 == task.type)
            {
                GameEvents.UIEvents.UI_GameEntry_Event.OnMaskBGVisible.SafeInvoke(false);
                if (task.sceneids.Length > 0)
                {
                    List<long> all_scene_pre_ids = new List<long>();

                    foreach (var s_id in task.sceneids)
                    {
                        string pre_id_str = s_id.ToString().Substring(0, 5);
                        long pre_id = long.Parse(pre_id_str);
                        all_scene_pre_ids.Add(pre_id);
                    }

                    foreach (var id in all_scene_pre_ids)
                    {
                        GameEvents.BigWorld_Event.OpenBuildTopByHead5NumInSceneID.SafeInvoke(id);
                    }
                }
            }
        }



        public static void OnReponse(object obj, NormalTask taskInfo)
        {
            if (obj is SCCanTaskResponse)
            {
                SCCanTaskResponse res = (SCCanTaskResponse)obj;
                if (res.TaskId == taskInfo.TaskConfID)
                {
                    if (res.CanTask)
                    {
                        GameEntryUILogic.S_CUR_EXCUTE_TASK_ID = res.TaskId;
                        OnExcuteTask(taskInfo);
                    }
                    else
                    {
                        ConfTask confTask = ConfTask.Get(res.TaskId);
                        if (confTask == null)
                        {
                            return;
                        }
                        if (confTask.building != null && confTask.building.Length > 0)
                        {
                            ConfBuilding building = ConfBuilding.Get(confTask.building[0]);
                            if (building == null)
                            {
                                return;
                            }
                            PopUpManager.OpenNormalOnePop("task_unlock_tips", LocalizeModule.Instance.GetString(building.name));
                        }
                    }
                }
            }


        }
    }
}
