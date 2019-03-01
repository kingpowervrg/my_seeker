using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EngineCore;
using UnityEngine;
using UnityEngine.UI;


namespace SeekerGame
{
    public class TaksOnBuildItemView : GameUIComponent
    {
        private readonly Vector3 C_OFFSET = new Vector3(0.0f, 5.0f, 0.0f);

        private GameButton m_pool_task_btn;
        private GameButton m_pool_reward_btn;

        private GameButton m_branch_tool_task_btn;
        private GameButton m_branch_role_task_btn;
        private GameTexture m_branch_role_task_tex;
        private GameButton m_branch_reward_btn;



        private Dictionary<TASK_ON_BUILD_STATUS, GameUIComponent> m_view_items = new Dictionary<TASK_ON_BUILD_STATUS, GameUIComponent>();



        NormalTask m_taskInfo;
        TaskAnchor m_anchor;

        private GameObject m_canvas_root;
        private RectTransform m_canvas_rt;
        public void SetTaskInfo(NormalTask taskInfo, GameObject canvas_obj_)
        {
            //if (null != this.m_taskInfo && this.m_taskInfo.TaskConfID == taskInfo.TaskConfID && this.m_taskInfo.TaskCurrentStatus == taskInfo.TaskCurrentStatus)
            //    return;

            this.m_taskInfo = taskInfo;
            m_canvas_root = canvas_obj_;
            m_canvas_rt = GetComponent<RectTransform>();
            RefreshTaskItemUI();
        }

        private void RefreshTaskItemUI()
        {


            TaskAnchor anchor = null;
            if (3 == m_taskInfo.TaskData.type)
            {
                if (this.m_taskInfo.TaskCurrentStatus == TaskStatus.COMPLETED)
                {
                    RefreshViewStatus(TASK_ON_BUILD_STATUS.POOL_REWARD);
                }
                else
                {
                    RefreshViewStatus(TASK_ON_BUILD_STATUS.POOL_TASK);
                }

                anchor = TaskOnBuildManager.Instance.DePoolTaskAnchor(m_taskInfo.TaskConfID);
            }
            else if (2 == m_taskInfo.TaskData.type)
            {
                if (this.m_taskInfo.TaskCurrentStatus == TaskStatus.COMPLETED)
                {
                    RefreshViewStatus(TASK_ON_BUILD_STATUS.BRANCH_REWARD);
                }
                else
                {
                    if (!CommonHelper.IsStringValid(m_taskInfo.TaskData.backgroundIcon))
                        RefreshViewStatus(TASK_ON_BUILD_STATUS.BRANCH_TOOL);
                    else
                    {
                        RefreshViewStatus(TASK_ON_BUILD_STATUS.BRANCH_ROLE);

                        m_branch_role_task_tex.TextureName = CommonHelper.IsStringValid(m_taskInfo.TaskData.backgroundIcon) ? m_taskInfo.TaskData.backgroundIcon : "image_role_tanzhang02_jiekuilinluolan_size2.png";
                    }
                }
                anchor = TaskOnBuildManager.Instance.DeBranchTaskAnchor(m_taskInfo.TaskConfID);
            }

            m_anchor = anchor;

            RefreshPos();

            //this.m_lbTaskName.Text = LocalizeModule.Instance.GetString(this.m_taskInfo.TaskData.name);
            //this.m_imgBranchTaskMarker.Visible = this.m_taskInfo.TaskData.type == 2;
            //this.m_imgMainTaskMarker.Visible = this.m_taskInfo.TaskData.type == 1;
            //this.m_imgPoolTaskMarker.Visible = this.m_taskInfo.TaskData.type == 3;
            //this.m_btnCollectReward.Visible = this.m_taskInfo.TaskCurrentStatus == TaskStatus.COMPLETED;
            //this.m_receiveEffect.Visible = this.m_btnCollectReward.Visible;

            //this.m_textureTaskCover.Sprite = this.m_taskInfo.TaskData.backgroundIcon;

            //if (this.m_taskInfo.TaskData.autoSkip && TaskStatus.COMPLETED != this.m_taskInfo.TaskCurrentStatus)
            //{
            //    this.AcceptTask();
            //    return;
            //}
            //else if (this.m_taskInfo.TaskData.type == 1 && this.m_taskInfo.TaskCurrentStatus == TaskStatus.PROGRESSING)
            //{
            //    this.m_mainTaskEffect.EffectPrefabName = "UI_zhuxianrenwu.prefab";
            //    this.m_mainTaskEffect.Visible = true;
            //    this.m_shouEffect.EffectPrefabName = "UI_xinshouyindao_shou.prefab";
            //    this.m_shouEffect.Visible = true;
            //    this.m_taskTips = true;
            //}
            //else
            //{
            //    this.m_mainTaskEffect.Visible = false;
            //    this.m_shouEffect.Visible = false;
            //    this.m_taskTips = false;
            //}


            //if (this.m_taskInfo.TaskCurrentStatus == TaskStatus.COMPLETED && this.m_taskInfo.TaskData.type == 1 && !this.m_taskInfo.TaskData.autoSkip)
            //{

            //    TimeModule.Instance.SetTimeout(() => this.GetReward(true), 0.7f);

            //}

        }

        public void RefreshPos()
        {
            if (null == m_anchor)
                return;

            if (null == Camera.main || !Camera.main.enabled)
                return;

            if (null == m_canvas_root || null == m_canvas_rt)
                return;

            Transform uiObj = this.gameObject.transform;

            Vector3 worldPos = EngineCore.Utility.CameraUtility.WorldPointInCanvasRectTransform(m_anchor.m_root.transform.position, m_canvas_root);

            //float s = Mathf.Max(0, (1.0f - (Camera.main.transform.position.z - min_cam_z) / (max_cam_z - min_cam_z)));
            //Debug.Log("z scale = " + s);

            uiObj.transform.position = worldPos;


            //this.Widget.anchoredPosition = EngineCore.Utility.CameraUtility.World2RectTrf(m_anchor.m_root.transform.position, Camera.main, m_canvas_root, FrameMgr.Instance.UICamera);
        }

        private float min_cam_z = -17.0f;
        private float max_cam_z = -7.0f;

        private void OnReponse(object obj)
        {
            if (obj is SCCanTaskResponse)
            {
                TaskHelper.OnReponse(obj, this.m_taskInfo);

            }
        }
        protected override void OnInit()
        {
            base.OnInit();

            m_pool_task_btn = Make<GameButton>("Task_pool");
            m_pool_reward_btn = Make<GameButton>("Reward_pool");

            m_branch_tool_task_btn = Make<GameButton>("Task_branch_dlc");
            m_branch_role_task_btn = Make<GameButton>("Task_branch_role");
            m_branch_role_task_tex = m_branch_role_task_btn.Make<GameTexture>("Image (2):Image (3):RawImage");
            m_branch_reward_btn = Make<GameButton>("Reward_branch");

            m_view_items.Add(TASK_ON_BUILD_STATUS.POOL_TASK, m_pool_task_btn);
            m_view_items.Add(TASK_ON_BUILD_STATUS.POOL_REWARD, m_pool_reward_btn);
            m_view_items.Add(TASK_ON_BUILD_STATUS.BRANCH_TOOL, m_branch_tool_task_btn);
            m_view_items.Add(TASK_ON_BUILD_STATUS.BRANCH_ROLE, m_branch_role_task_btn);
            m_view_items.Add(TASK_ON_BUILD_STATUS.BRANCH_REWARD, m_branch_reward_btn);

        }

        public override void OnShow(object param)
        {
            base.OnShow(param);

            MessageHandler.RegisterMessageHandler(MessageDefine.SCCanTaskResponse, OnReponse);

            m_pool_task_btn.AddClickCallBack(OnTaskClicked);
            m_branch_role_task_btn.AddClickCallBack(OnTaskClicked);
            m_branch_tool_task_btn.AddClickCallBack(OnTaskClicked);
            m_pool_reward_btn.AddClickCallBack(OnRewardClicked);
            m_branch_reward_btn.AddClickCallBack(OnRewardClicked);
        }

        public override void OnHide()
        {
            base.OnHide();

            MessageHandler.UnRegisterMessageHandler(MessageDefine.SCCanTaskResponse, OnReponse);

            m_pool_task_btn.RemoveClickCallBack(OnTaskClicked);
            m_branch_role_task_btn.RemoveClickCallBack(OnTaskClicked);
            m_branch_tool_task_btn.RemoveClickCallBack(OnTaskClicked);
            m_pool_reward_btn.RemoveClickCallBack(OnRewardClicked);
            m_branch_reward_btn.RemoveClickCallBack(OnRewardClicked);

        }

        private void OnTaskClicked(GameObject obj_)
        {
            TaskHelper.OnTaskItemClick(m_taskInfo, () => this.Visible = false);
        }

        private void OnRewardClicked(GameObject obj_)
        {
            TaskHelper.GetReward(m_taskInfo, () => this.Visible = false);
        }

        private void RefreshViewStatus(TASK_ON_BUILD_STATUS status_)
        {
            foreach (var kvp in m_view_items)
            {
                kvp.Value.Visible = false;
            }

            m_view_items[status_].Visible = true;

        }
    }
}

