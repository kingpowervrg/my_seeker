using EngineCore;
using System;
using System.Collections.Generic;
using UnityEngine;
namespace SeekerGame
{
    public class BuildTopIconComponent : GameUIComponent
    {
        private GameButton m_btn;
        //private long m_buidID;
        private bool m_isGuid = false;
        private BuidAchorData m_achorData = null;
        private ConfBuilding m_confBuilding = null;
        private GameImage m_panelRoot = null;
        private BuidTopSceneIcon[] m_sceneImg = null;
        private GameImage m_normalIcon = null;
        //private GameImage m_lockIcon = null;
        private GameImage m_canLockIcon = null; //取消
        private GameImage m_imgDownload = null;

        private GameUIEffect m_lockeffect = null;
        private GameUIEffect m_unLockEffect = null;
        private GameUIEffect m_sceneOutLightingEffect = null;
        //private int m_buildStatus = 0;
        private bool m_visible = false;
        protected override void OnInit()
        {
            base.OnInit();
            this.m_btn = Make<GameButton>(gameObject);
            this.m_panelRoot = Make<GameImage>("Panel");
            this.m_sceneImg = new BuidTopSceneIcon[8];
            this.m_normalIcon = Make<GameImage>("Image");
            //this.m_lockIcon = Make<GameImage>("Image_Lock");
            //this.m_canLockIcon = Make<GameImage>("Image_CanLock");
            this.m_normalIcon.Visible = false;
            //this.m_lockIcon.Visible = false;
            //this.m_canLockIcon.Visible = false;
            this.m_lockeffect = Make<GameUIEffect>("lockEffect");
            this.m_unLockEffect = Make<GameUIEffect>("unLockEffect");
            this.m_imgDownload = Make<GameImage>("ImageDownload");
            //this.m_sceneOutLightingEffect = Make<GameUIEffect>("UI_faliang");
            for (int i = 0; i < 8; i++)
            {
                this.m_sceneImg[i] = Make<BuidTopSceneIcon>("scence_" + i);

                //this.m_sceneImg[i].SetData(m_achorData);
                this.m_sceneImg[i].Visible = false;
            }
            this.m_panelRoot.Visible = true;
        }

        public override void OnShow(object param)
        {
            base.OnShow(param);
            //this.m_normalIcon.Visible = false;
            //this.m_lockIcon.Visible = true;
            //this.m_sceneOutLightingEffect.EffectPrefabName = "UI_faliang.prefab";
            //this.m_sceneOutLightingEffect.Visible = false;
            GameEvents.BigWorld_Event.OnReflashBuidTask += OnReflashBuildIcon;
            this.m_btn.AddClickCallBack(OnClick);
        }

        private void HighLight(long head_5_num_in_scene_id_)
        {
            m_visible = true;
            //this.m_panelRoot.Visible = !this.m_panelRoot.Visible;
            PlayBuildTopIcon(m_visible);
            HightLightBuildTopIcon(head_5_num_in_scene_id_);
            GameEvents.BigWorld_Event.OnBuildTopActive.SafeInvoke(this, m_visible);
        }

        public void OnClick(GameObject obj)
        {
            if (this.m_isGuid)
            {
                CommonHelper.OpenEnterGameSceneUI(GameConst.GUIDESCENEID);
            }
            else
            {
                if (OnClickBuildIcon())
                {
                    m_visible = !m_visible;
                    //this.m_panelRoot.Visible = !this.m_panelRoot.Visible;
                    if (m_visible)
                    {
                        EngineCoreEvents.AudioEvents.PlayAudio.SafeInvoke(Audio.AudioType.UISound, GameCustomAudioKey.map_flower.ToString());
                    }
                    PlayBuildTopIcon(m_visible);
                    GameEvents.BigWorld_Event.OnBuildTopActive.SafeInvoke(this, m_visible);
                }

            }
        }

        private void HightLightBuildTopIcon(long head_5_num_in_scene_id_)
        {
            for (int i = 0; i < m_achorData.m_enterData.Count; i++)
            {
                if (this.m_sceneImg[i].CanHighLight(head_5_num_in_scene_id_))
                {
                    this.m_sceneImg[i].HighLight();
                    return;
                }
                //this.m_sceneImg[i].HighLight(head_5_num_in_scene_id_);
            }
        }

        private void PlayBuildTopIcon(bool visible)
        {
            for (int i = 0; i < m_achorData.m_enterData.Count; i++)
            {
                if (visible)
                {
                    this.m_sceneImg[i].PlayTween();
                }
                else
                {
                    //this.m_sceneOutLightingEffect.Visible = false;
                    this.m_sceneImg[i].PlayReset();
                }
            }
        }
        public void SetData(BuidAchorData achordata, bool isGuid)
        {
            //this.m_buidID = buildID;
            this.m_isGuid = isGuid;
            this.m_achorData = achordata;
            if (this.m_achorData != null)
            {
                this.m_normalIcon.Sprite = "icon_position_type4.png";
                this.m_confBuilding = ConfBuilding.Get(this.m_achorData.m_buidID);
                SetBuildStatus(this.m_achorData.BuildStatus);
            }
            else if (isGuid)
            {
                this.m_normalIcon.Sprite = "icon_position_type2.png";
                this.m_normalIcon.Visible = true; //已解锁

                //this.m_lockIcon.Visible = false;  //加锁
                //this.m_canLockIcon.Visible = false; //可解锁
            }
            List<BuidTopSceneIcon> buildTopIcon = new List<BuidTopSceneIcon>();
            if (achordata != null)
            {
                for (int i = 0; i < achordata.m_enterData.Count; i++)
                {
                    this.m_sceneImg[i].SetData(achordata.m_enterData[i], i);
                    buildTopIcon.Add(this.m_sceneImg[i]);
                    //this.m_sceneImg[i].Visible = true;
                }
                buildTopIcon.Sort(SortSceneIcon);
                for (int i = 0; i < buildTopIcon.Count; i++)
                {
                    buildTopIcon[i].SetIndex(i, Mathf.Lerp(0.2f, 0.05f, i / (float)buildTopIcon.Count));
                }
            }

        }

        private int SortSceneIcon(BuidTopSceneIcon sceneIcon0, BuidTopSceneIcon sceneIcon1)
        {
            if (sceneIcon0.Index > sceneIcon1.Index)
            {
                return 1;
            }
            else if (sceneIcon0.Index == sceneIcon1.Index)
            {
                return 0;
            }
            else
            {
                return -1;
            }
        }

        public bool isExist(long buildID)
        {
            //新手引导
            if (m_achorData == null)
            {
                return true;
            }
            return this.m_achorData.m_buidID == buildID;
        }

        private void OnReflashBuildIcon()
        {
            if (m_achorData == null)
            {
                return;
            }
            for (int i = 0; i < m_achorData.m_enterData.Count; i++)
            {
                this.m_sceneImg[i].OnReflashBuildIcon();
            }
        }

        public override void OnHide()
        {
            base.OnHide();
            GameEvents.BigWorld_Event.OnReflashBuidTask -= OnReflashBuildIcon;
            this.m_btn.RemoveClickCallBack(OnClick);
            //this.m_sceneOutLightingEffect.Visible = false;
        }

        public void OnHideGuid()
        {
            if (m_isGuid)
            {
                Visible = false;
            }
        }

        public bool OpenBuildTopIconBySceneID(long head_5_num_in_sceneID)
        {
            if (m_achorData == null)
            {
                return false;
            }
            for (int i = 0; i < m_achorData.m_enterData.Count; i++)
            {
                string pre_id_str = m_achorData.m_enterData[i].m_sceneID.ToString().Substring(0, 5);
                long pre_id = long.Parse(pre_id_str);
                if (pre_id == head_5_num_in_sceneID)
                {
                    HighLight(head_5_num_in_sceneID);
                    return true;
                }
            }
            return false;
        }

        public void SetBuildStatus(int status)
        {
            if (m_isGuid)
            {
                return;
            }
            if (this.m_achorData.BuildStatus == 2 && status == 1)
            {
                TimeModule.Instance.SetTimeout(() =>
                {
                    PlayUnLock();
                }, 0.5f);
                TimeModule.Instance.SetTimeout(() =>
                {
                    SetBuildTopStatus(status);
                }, 1f);
            }
            else
            {
                SetBuildTopStatus(status);
            }
            this.m_achorData.BuildStatus = status;

        }

        private void SetBuildTopStatus(int status)
        {
            Debug.Log("build status " + status + "  " + this.m_achorData.m_buidID);
            //this.m_normalIcon.Visible = (status == 1); //已解锁
            CurrentStatus = status;
            //this.m_lockeffect.Visible = (status == 2);
            //this.m_normalIcon.Visible = (status == 1);
            //if (status == 3 || status == 2)
            //{
            //    this.m_normalIcon.Sprite = "";
            //}
            //this.m_lockIcon.Visible = (status == 3 || status == 2);  //加锁
            //this.m_canLockIcon.Visible = (status == 2); //可解锁
        }

        private bool OnClickBuildIcon()
        {
            if (m_achorData == null)
            {
                return false;
            }
            if (m_achorData.BuildStatus == 2)
            {
                if (IsChapterValid())
                {

                    FrameMgr.OpenUIParams param = new FrameMgr.OpenUIParams(UIDefine.UI_UNLOCK);
                    param.Param = m_achorData.m_buidID;
                    EngineCoreEvents.UIEvent.ShowUIEventWithParam.SafeInvoke(param);
                    return false;
                }
                else
                {
                    GameEvents.ChapterEvents.OnChapterDownloadFinish += OnChapterDownloaded;
                    FrameMgr.OpenUIParams uiParams = new FrameMgr.OpenUIParams(UIDefine.UI_ChapterMap);
                    uiParams.Param = m_confBuilding.id;

                    EngineCoreEvents.UIEvent.ShowUIEventWithParam.SafeInvoke(uiParams);

                    return false;
                }
            }
            else if (m_achorData.BuildStatus == 3)
            {
                if (m_confBuilding.unlockTask > 0)
                {
                    GlobalInfo.MY_PLAYER_INFO.PlayerTaskSystem.IsCompleteTaskByConfigID(m_confBuilding.unlockTask, (bool taskComplete) =>
                    {
                        if (!taskComplete)
                        {
                            string taskName = string.Empty;
                            ConfTask confTask = ConfTask.Get(m_confBuilding.unlockTask);
                            if (confTask != null)
                            {
                                taskName = LocalizeModule.Instance.GetString(confTask.name);
                            }
                            if (GlobalInfo.MY_PLAYER_INFO.Level < m_confBuilding.unlockLevel)
                            {
                                PopUpManager.OpenNormalOnePop(LocalizeModule.Instance.GetString("map_clock_level_mission", m_confBuilding.unlockLevel, taskName));
                            }
                            else
                            {
                                PopUpManager.OpenNormalOnePop(LocalizeModule.Instance.GetString("group_unlock_tips", taskName));
                            }
                            return;
                        }

                    });
                }
                else
                    PopUpManager.OpenNormalOnePop(LocalizeModule.Instance.GetString("map_clock_level", m_confBuilding.unlockLevel));
                return false;
            }
            return true;
        }

        private void OnChapterDownloaded(ChapterInfo chapterInfo)
        {
            GameEvents.ChapterEvents.OnChapterDownloadFinish -= OnChapterDownloaded;
            if (chapterInfo.ChapterID == m_confBuilding.id)
            {
                CurrentStatus = 2;
            }
        }


        private bool IsChapterValid()
        {
            return GlobalInfo.MY_PLAYER_INFO.PlayerChapterSystem.IsChapterAssetExist(this.m_confBuilding.id);
        }

        private int CurrentStatus
        {
            set
            {
                if (value == 2) //可解锁
                {
                    this.m_lockeffect.EffectPrefabName = "UI_chengshijiesuo_suo.prefab";
                    this.m_unLockEffect.EffectPrefabName = "UI_chengshijiesuo_suo02.prefab";
                    bool isChapterValid = IsChapterValid();
                    this.m_imgDownload.Visible = !isChapterValid;
                    this.m_lockeffect.Visible = isChapterValid;
                    this.m_unLockEffect.Visible = false;

                    //this.m_lockIcon.Visible = true;
                }
                else if (value == 1)
                {
                    this.m_lockeffect.Visible = false;
                    this.m_unLockEffect.Visible = false;
                    this.m_normalIcon.Sprite = "icon_position_type2.png";
                    this.m_normalIcon.Visible = true; //已解锁
                    this.m_imgDownload.Visible = false;
                }
                //else if (value == 3)
                //{
                //    this.m_lockeffect.Visible = false;
                //    this.m_unLockEffect.EffectPrefabName = "UI_chengshijiesuo_suo.prefab";
                //    this.m_unLockEffect.Visible = true;
                //    this.m_lockIcon.Visible = true;
                //}
            }
        }

        public void PlayUnLock()
        {
            if (m_achorData == null)
            {
                return;
            }
            this.m_lockeffect.Visible = false;
            this.m_unLockEffect.ReplayEffect();
            //this.m_unLockEffect.Visible = true;
            //this.m_lockIcon.Visible = true;
        }

        public void SetIconVisible(bool visible)
        {

            SetVisible(visible);
            if (!visible)
            {
                m_visible = false;
                PlayBuildTopIcon(false);
            }
        }
    }

    public class BuidTopSceneIcon : GameUIComponent
    {
        private GameTexture m_sceneTex = null;
        private GameLabel m_sceneLab = null;
        private GameButton m_btn;
        private BuidEnterData m_enterdata;
        private GameImage m_lockImg = null;
        private GameImage m_difficultImg = null;
        private GameUIEffect m_effect = null;


        private int m_buildState = 0; //0未解锁  1 普通场景  2 任务场景
        private UITweenerBase[] m_allTweener;
        private float m_delayTime = 0.2f;

        private int m_index = 0;

        public int Index
        {
            get { return m_index; }
        }
        protected override void OnInit()
        {
            base.OnInit();
            this.m_sceneTex = Make<GameTexture>("RawImage");
            this.m_sceneLab = Make<GameLabel>("Text");
            this.m_btn = Make<GameButton>(gameObject);
            this.m_lockImg = Make<GameImage>("Image_lock");
            m_difficultImg = Make<GameImage>("Image_Difficult");
            this.m_effect = Make<GameUIEffect>("UI_faliang");
            this.m_allTweener = Widget.GetComponentsInChildren<UITweenerBase>();
            this.m_index = GetComponent<BigWorldBuildTopParam>().m_index;
        }

        public override void OnShow(object param)
        {
            base.OnShow(param);
            this.m_effect.EffectPrefabName = "UI_faliang.prefab";
            this.m_effect.Visible = false;
            //GameEvents.BigWorld_Event.OnReflashBuidTask += OnReflashBuildIcon;
            this.m_btn.AddClickCallBack(OnClick);
        }

        public override void OnHide()
        {
            base.OnHide();
            //GameEvents.BigWorld_Event.OnReflashBuidTask -= OnReflashBuildIcon;
            this.m_btn.RemoveClickCallBack(OnClick);

            //关闭高亮
            this.m_effect.Visible = false;
        }

        public void SetData(BuidEnterData enterdata, int index)
        {
            this.m_enterdata = enterdata;
            OnReflashBuildIcon();
        }

        public void SetIndex(int index, float delayTime)
        {
            m_index = index;// (index < m_index) ? index : m_index;
            m_delayTime = m_index * delayTime;
            for (int i = 0; i < m_allTweener.Length; i++)
            {
                m_allTweener[i].Delay += m_delayTime;
            }
        }

        public void PlayTween()
        {
            Visible = true;
            for (int i = 0; i < m_allTweener.Length; i++)
            {
                m_allTweener[i].ResetAndPlay();
                //m_allTweener[i].PlayForward();
            }
        }

        public void PlayReset()
        {
            //for (int i = 0; i < m_allTweener.Length; i++)
            //{
            //    m_allTweener[i].PlayBackward();
            //}
            Visible = false;
        }

        public void OnReflashBuildIcon()
        {
            ConfScene confScene = null;
            bool flag = TaskQueryManager.Instance.QueryTaskComplete(this.m_enterdata.m_taskID);
            if (!flag)
            {
                this.m_sceneTex.SetGray(true);
                this.m_sceneLab.color = new Color(0.54f, 0.54f, 0.54f);
                confScene = ConfScene.Get(this.m_enterdata.m_sceneID);
                this.m_buildState = 0;
            }
            else
            {
                this.m_sceneLab.color = Color.white;
                this.m_sceneTex.SetGray(false);
                long sceneID = TaskQueryManager.Instance.CheckBuildIsInTask(this.m_enterdata.m_sceneID / 1000);
                if (sceneID != TaskQueryManager.ERRORID)
                {
                    confScene = ConfScene.Get(sceneID);
                    this.m_buildState = 2;
                }
                else
                {
                    confScene = ConfScene.Get(this.m_enterdata.m_sceneID);
                    this.m_buildState = 1;
                }
            }
            this.m_lockImg.Visible = (this.m_buildState == 0);
            m_difficultImg.Visible = !m_lockImg.Visible;
            int? difficult = FindObjSceneDataManager.Instance.GetDataBySceneID(this.m_enterdata.m_sceneID)?.m_lvl;
            m_difficultImg.Sprite = difficult > 0 ? ConfPoliceRankIcon.Get(difficult.Value).icon : ConfPoliceRankIcon.Get(1).icon;
            this.m_sceneTex.TextureName = confScene.thumbnail;
            this.m_sceneLab.Text = LocalizeModule.Instance.GetString(confScene.name);
        }

        public bool CanHighLight(long head_5_num_in_scene_id_)
        {
            return m_enterdata.m_sceneID / 1000 == head_5_num_in_scene_id_;
        }

        public void HighLight()
        {
            if (0 == m_buildState)
            {
                this.m_effect.Visible = false;
                return;
            }
            this.m_effect.Visible = true;
        }

        private void CheckCurrentTask()
        {
            bool flag = TaskQueryManager.Instance.QueryTaskComplete(this.m_enterdata.m_taskID);
            if (!flag)
            {
                this.m_buildState = 0;
            }
            else
            {
                long sceneID = TaskQueryManager.Instance.CheckBuildIsInTask(this.m_enterdata.m_sceneID / 1000);
                if (sceneID != TaskQueryManager.ERRORID)
                {
                    this.m_buildState = 2;
                }
                else
                {
                    this.m_buildState = 1;
                }
            }

        }

        private void OnClick(GameObject objs)
        {
            CheckCurrentTask();
            if (this.m_buildState == 1)
            {
                CommonHelper.OpenEnterGameSceneUI(this.m_enterdata.m_sceneID);
            }
            else if (this.m_buildState == 2)
            {
                NormalTask taskInfo = TaskQueryManager.Instance.GetCurrentMainTaskInfo();
                TaskHelper.AcceptTask(taskInfo);

                //CSCanTaskRequest req = new CSCanTaskRequest();
                //req.TaskId = taskInfo.TaskConfID;
                //GameEvents.NetWorkEvents.SendMsg.SafeInvoke(req);
               
            }
            else
            {

                WaveTipHelper.LoadWaveContent("scene_locked", Widget.position);
            }
        }
    }
}
