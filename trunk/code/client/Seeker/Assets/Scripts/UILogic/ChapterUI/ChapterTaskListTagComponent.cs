using System.Linq;
using System.Collections.Generic;
using EngineCore;
namespace SeekerGame
{
    /// <summary>
    /// 章节任务列表组件
    /// </summary>
    public class ChapterTaskListTagComponent : GameUIComponent
    {
        private GameUIContainer m_taskContainer = null;
        private GameScrollView m_taskScrollView = null;

        private ChapterTaskBodyComponent m_taskDead = null;
        private ChapterTaskBodyComponent m_taskSuspect = null;
        private ChapterTaskBodyComponent m_taskWeapon = null;

        private long m_lastChapterId = -1;
        protected override void OnInit()
        {
            this.m_taskContainer = Make<GameUIContainer>("TaskScrollView:grid");

            this.m_taskDead = Make<ChapterTaskBodyComponent>("clueScrollView:grid:deayBody");
            this.m_taskSuspect = Make<ChapterTaskBodyComponent>("clueScrollView:grid:suspect");
            this.m_taskWeapon = Make<ChapterTaskBodyComponent>("clueScrollView:grid:weapon");
            //this.m_taskScrollView = Make<GameScrollView>("Panel_3");
        }

        public override void OnShow(object param)
        {
            base.OnShow(param);

            //this.m_taskScrollView.ScrollToBottom();

            UserBehaviorStatisticsModules.Instance.LogEvent(UBSEventKeyName.file_task);
        }

        public void SetTaskList(IList<ChapterTaskInfo> taskInfos,long currentChapterId)
        {
            //只显示完成的及正在做的
            List<ChapterTaskInfo> taskInfoList = new List<ChapterTaskInfo>();
            for (int i = 0; i < taskInfos.Count; ++i)
            {
                ChapterTaskInfo taskInfo = taskInfos[i];
                if (taskInfo.Status == 1 || taskInfo.Status == 2)
                    taskInfoList.Add(taskInfo);
            }

            //排序
            taskInfoList = taskInfoList.OrderByDescending(chapterTaskInfo => -chapterTaskInfo.Status).ToList();

            this.m_taskContainer.EnsureSize<ChapterTaskIntroComponent>(taskInfoList.Count);
            for (int i = 0; i < taskInfoList.Count; ++i)
            {
                ChapterTaskIntroComponent introComponent = this.m_taskContainer.GetChild<ChapterTaskIntroComponent>(i);
                introComponent.SetTaskIntro(taskInfoList[i].TaskId, taskInfoList[i].Status == 2);
                introComponent.Visible = true;
            }

            //m_lastChapterId = currentChapterId;
            ConfChapter chapter = ConfChapter.Get(currentChapterId);
            List<long> deadBody = new List<long>();
            List<long> suspect = new List<long>();
            List<long> weapon = new List<long>();
            for (int i = 0; i < chapter.actorIds.Length; i++)
            {
                ConfNpc confNpc = ConfNpc.Get(chapter.actorIds[i]);
                if (confNpc == null)
                {
                    UnityEngine.Debug.LogError("chapter npc not exist : " + chapter.actorIds[i]);
                    continue;
                }
                bool isTaskComplete = taskInfoList.Find(s => s.TaskId == confNpc.unlockTaskId && s.Status == 2) != null || confNpc.unlockTaskId == 0;
                if (confNpc.identityType == 4 && isTaskComplete) //凶手
                {
                    suspect.Add(chapter.actorIds[i]);
                }
                else if (confNpc.identityType == 2 && isTaskComplete) //死者
                {
                    deadBody.Add(chapter.actorIds[i]);
                }
                else if (confNpc.identityType == 3 && isTaskComplete) //凶器
                {
                    weapon.Add(chapter.actorIds[i]);
                }

            }
            if (this.m_taskDead.Visible)
            {
                this.m_taskDead.SetNpcData(deadBody, 0);
            }
            if (this.m_taskSuspect.Visible)
            {
                this.m_taskSuspect.SetNpcData(suspect, 1);
            }
            if (this.m_taskWeapon.Visible)
            {
                this.m_taskWeapon.SetNpcData(weapon, 2);
            }
        }


        /// <summary>
        /// 任务详情组件
        /// </summary>
        private class ChapterTaskIntroComponent : GameUIComponent
        {
            private GameToggleButton m_isTaskComplete = null;
            private GameLabel m_lbTaskName = null;
            private GameLabel m_lbTaskIntroContent = null;
            private GameImage m_TaskBgImg = null;

            private long m_taskId = 0;

            protected override void OnInit()
            {
                base.OnInit();

                this.m_isTaskComplete = Make<GameToggleButton>("Toggle");
                this.m_lbTaskName = Make<GameLabel>("Toggle:Label");
                this.m_lbTaskIntroContent = Make<GameLabel>("Toggle:Label (1)");
                this.m_TaskBgImg = Make<GameImage>("Image");
            }

            public void SetTaskIntro(long taskId, bool status)
            {
                this.m_taskId = taskId;
                ConfTask confTaskData = ConfTask.Get(taskId);

                this.m_isTaskComplete.Checked = !status;
                this.m_TaskBgImg.Visible = status;
                this.m_lbTaskName.Text = LocalizeModule.Instance.GetString(confTaskData.name);
                this.m_lbTaskIntroContent.Text = LocalizeModule.Instance.GetString(confTaskData.descs);
            }

        }

        /// <summary>
        /// 任务嫌疑人、死者、凶器
        /// </summary>
        private class ChapterTaskBodyComponent : GameUIComponent
        {
            private GameTexture m_personTex = null; //人物
            private GameLabel m_nameLab = null;
            private GameImage m_btnItem = null;
            //private GameLabel m_contentLab = null;

            private GameImage m_exhibitImg = null;
            private GameImage m_emptyImg = null;

            private GameUIComponent m_weaponDetail = null;
            private GameLabel m_weaponLab = null;

            private List<long> m_npcIds = null;
            private int m_currentNpcIndex = 0;

            private bool m_isTick = false;
            private int m_type = 0; //0死者  1凶手   2凶器
            protected override void OnInit()
            {
                base.OnInit();
                this.m_personTex = Make<GameTexture>("Image_rlole:RawImage");
                this.m_nameLab = Make<GameLabel>("Image_bg_2:Text");
                this.m_exhibitImg = Make<GameImage>("Image_icon");
                this.m_emptyImg = Make<GameImage>("Image_?");
                this.m_btnItem = Make<GameImage>("Image_bg");
            }

            public override void OnShow(object param)
            {
                base.OnShow(param);
                this.m_btnItem.AddClickCallBack(OnItemClick);
                this.m_btnItem.AddPressDownCallBack(OnPressDown);
                this.m_btnItem.AddPressUpCallBack(OnPressUp);
                //if (!m_isTick)
                //{
                //    TimeModule.Instance.SetTimeout(OnChangeNpc, 1f,true);
                //}
            }

            public void SetNpcData(List<long> param,int type)
            {
                this.m_type = type;
                if (type == 2)
                {
                    this.m_weaponDetail = Make<GameUIComponent>("Panel_weapondetail");
                    this.m_weaponLab = this.m_weaponDetail.Make<GameLabel>("Text");
                }
                this.m_isTick = false;
                //TimeModule.Instance.RemoveTimeaction(OnChangeNpc);
                this.m_npcIds = param;
                OnShowNpc();
            }

            public override void OnHide()
            {
                base.OnHide();
                this.m_btnItem.RemoveClickCallBack(OnItemClick);
                this.m_btnItem.RemovePressDownCallBack(OnPressDown);
                this.m_btnItem.RemovePressUpCallBack(OnPressUp);
                //TimeModule.Instance.RemoveTimeaction(OnChangeNpc);
                this.m_isTick = false;
            }

            private void OnShowNpc()
            {
                if (m_npcIds.Count > m_currentNpcIndex)
                {
                    OnShowNpc(m_npcIds[m_currentNpcIndex]);
                    this.m_isTick = true;
                    this.m_emptyImg.Visible = false;
                    if (m_npcIds.Count > 0)
                    {
                        //TimeModule.Instance.RemoveTimeaction(OnChangeNpc);
                        //TimeModule.Instance.SetTimeInterval(OnChangeNpc, 2f);
                    }
                }
                else
                {
                    //无NPC
                    this.m_emptyImg.Visible = true;
                    this.m_exhibitImg.Visible = false;
                    this.m_personTex.Visible = false;
                    this.m_nameLab.Text = "";
                }
            }

            private void OnShowNpc(long npcId)
            {
                ConfNpc confNpc = ConfNpc.Get(npcId);
                if (this.m_personTex == null || this.m_nameLab == null || this.m_exhibitImg == null)
                {
                    //TimeModule.Instance.RemoveTimeaction(OnChangeNpc);
                    return;
                }
                this.m_nameLab.Text = LocalizeModule.Instance.GetString(confNpc.name);
                if (confNpc.identityType == 4 || confNpc.identityType == 2)
                {
                    this.m_personTex.TextureName = confNpc.icon;
                    this.m_exhibitImg.Visible = false;
                    this.m_personTex.Visible = true;
                }
                else if(confNpc.identityType == 3)
                {
                    this.m_exhibitImg.Sprite = confNpc.icon;
                    this.m_exhibitImg.Visible = true;
                    this.m_personTex.Visible = false;
                    if (this.m_type == 2)
                    {
                        this.m_weaponLab.Text = LocalizeModule.Instance.GetString(confNpc.otherFeatures);
                    }
                }
            }

            private void OnChangeNpc()
            {
                this.m_currentNpcIndex++;
                if (m_npcIds.Count == 0)
                {
                    //TimeModule.Instance.RemoveTimeaction(OnChangeNpc);
                    return;
                }
                this.m_currentNpcIndex = this.m_currentNpcIndex % m_npcIds.Count;
                OnShowNpc(m_npcIds[this.m_currentNpcIndex]);
            }

            private void OnItemClick(UnityEngine.GameObject obj)
            {
                if (this.m_npcIds.Count > 0)
                {
                    if ((this.m_type == 0 || this.m_type == 1))
                    {
                        //npc列表
                        GameEvents.ChapterEvents.OnpenChapterUIByIndex.SafeInvoke(2);
                    }
                }
                
            }

            private void OnPressDown(UnityEngine.GameObject obj)
            {
                if (this.m_type == 2 && this.m_npcIds.Count > 0)
                {
                    this.m_weaponDetail.Visible = true;
                }
            }

            private void OnPressUp(UnityEngine.GameObject obj)
            {
                if (this.m_type == 2 && this.m_npcIds.Count > 0)
                {
                    this.m_weaponDetail.Visible = false;
                }
            }
        }
    }
}
