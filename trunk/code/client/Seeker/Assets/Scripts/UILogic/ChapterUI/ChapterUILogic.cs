/********************************************************************
	created:  2018-5-23 20:26:8
	filename: ChapterUILogic.cs
	author:	  songguangze@fotoable.com
	
	purpose:  档案(章节)UI逻辑
*********************************************************************/
using EngineCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using DG.Tweening;

namespace SeekerGame
{
    [UILogicHandler(UIDefine.UI_CHAPTER)]
    public class ChapterUILogic : UILogicBase
    {
        public static Action<long, ChapterNpcInfo> OnShowNPCDetailInfo;

        private GameTexture m_chapterThumbTexture = null;
        private GameButton m_btnShowChapterList = null;
        private GameCheckBox localTagButton = null;
        private GameCheckBox clueTagButton = null;
        private GameCheckBox taskTagButton = null;
        private GameCheckBox chapterDescTag = null;
        private GameCheckBox npcTagButton = null;

        private GameImage m_localRedDot = null;
        private GameImage m_clueRedDot = null;
        private GameImage m_npcRedDot = null;

        private GameUIComponent m_panel_down;
        private ChapterSectionTagComponent m_chapterSectionListComponent = null;
        private NPCListTagComponent m_npcListComponent = null;
        private ChapterDescTagComponent m_chapterDescTagComponent = null;
        private ChapterClueListTagCompoment m_chapterClueListComponent = null;
        private ChapterTaskListTagComponent m_chapterTaskListTagComponent = null;

        private NPCDetailComponent m_npcDetailComponent = null;

        private ChapterSystem m_playerChapterSystem = null;
        private ChapterInfo m_currentDisplayChapterInfo = null;


        //private TweenScale m_tweenPos = null;
        #region 头部信息
        private GameLabel m_totalTaskNumLabel;
        private GameProgressBar m_taskProgressSlider;
        private GameLabel m_chapterNameLabel = null;
        private GameLabel m_currentChapterNum = null;
        #endregion
        protected override void OnInit()
        {
            this.m_btnShowChapterList = Make<GameButton>("Panel_down:Panel_top:ChapterIconBtn"); //章节点击按钮
            this.m_chapterThumbTexture = this.m_btnShowChapterList.Make<GameTexture>("RawImage_case"); //章节图片
            this.m_totalTaskNumLabel = Make<GameLabel>("Panel_down:Panel_top:Total");
            this.m_taskProgressSlider = Make<GameProgressBar>("Panel_down:Panel_top:Slider");
            this.m_chapterNameLabel = Make<GameLabel>("Panel_down:Panel_top:ChapterName");
            this.m_currentChapterNum = this.m_chapterNameLabel.Make<GameLabel>("TaskNum");

            this.m_npcDetailComponent = Make<NPCDetailComponent>("Panel_detail");

            this.m_panel_down = Make<GameUIComponent>("Panel_down");
            //
            this.m_chapterSectionListComponent = Make<ChapterSectionTagComponent>("Panel_down:Panel_0");
            //local
            this.m_npcListComponent = Make<NPCListTagComponent>("Panel_down:Panel_2");
            //case review
            this.m_chapterDescTagComponent = Make<ChapterDescTagComponent>("Panel_down:Panel_4");
            //clue
            this.m_chapterClueListComponent = Make<ChapterClueListTagCompoment>("Panel_down:Panel_1");
            //任务 task
            this.m_chapterTaskListTagComponent = Make<ChapterTaskListTagComponent>("Panel_down:Panel_3");

            this.localTagButton = this.m_panel_down.Make<GameCheckBox>("BtnDown:leftBtn:LocalTag");
            this.m_localRedDot = this.localTagButton.Make<GameImage>("redDot");
            this.m_localRedDot.Visible = false;

            this.clueTagButton = this.m_panel_down.Make<GameCheckBox>("BtnDown:leftBtn:ClueTag");
            this.m_clueRedDot = this.clueTagButton.Make<GameImage>("redDot");
            this.m_clueRedDot.Visible = false;

            this.taskTagButton = this.m_panel_down.Make<GameCheckBox>("BtnDown:leftBtn:TaskTag");
            this.chapterDescTag = this.m_panel_down.Make<GameCheckBox>("BtnDown:leftBtn:ChapterDescTag");

            this.npcTagButton = this.m_panel_down.Make<GameCheckBox>("BtnDown:leftBtn:NPCTag");
            this.m_npcRedDot = this.npcTagButton.Make<GameImage>("redDot");
            this.m_npcRedDot.Visible = false;

            //this.m_tweenPos = this.m_panel_down.GetComponent<TweenScale>();

            this.m_playerChapterSystem = GlobalInfo.MY_PLAYER_INFO.PlayerChapterSystem;
        }

        public override FrameDisplayMode UIFrameDisplayMode
        {
            get
            {
                return FrameDisplayMode.FULLSCREEN;
            }
        }
        public override void OnShow(object param)
        {
            EngineCoreEvents.AudioEvents.PlayAudio.SafeInvoke(Audio.AudioType.UISound, GameCustomAudioKey.zoom_in.ToString());
            base.OnShow(param);
            long chapterID;
            if (param == null)
                chapterID = this.m_playerChapterSystem.CurrentChapterInfo.ChapterID;
            else
                chapterID = Convert.ToInt64(param);
            SetCloseBtnID("Panel_down:Panel_top:Button");
            GameEvents.ChapterEvents.OnUnlockChapter += OnUnlockNewChapter;
            GameEvents.ChapterEvents.OnChapterInfoUpdated += OnChapterInfoUpdated;
            GameEvents.ChapterEvents.OnpenChapterUIByIndex += OnpenChapterUIByIndex;
            GameEvents.RedPointEvents.User_OnNewChapterEvent += User_OnNewChapterEvent;
            GameEvents.RedPointEvents.Sys_OnNewChapterEvent.SafeInvoke(2);
            RedPointManager.Instance.ReflashArchivesRedPoint();
            SetChapterID(chapterID);
            OnShowNPCDetailInfo = ShowNPCDetailInfo;
            this.m_isHide = false;
            this.localTagButton.AddChangeCallBack(OnLocalTagValueChanged);
            this.clueTagButton.AddChangeCallBack(OnClueTagValueChanged);
            this.npcTagButton.AddChangeCallBack(OnNpcTagValueChanged);
            this.taskTagButton.AddChangeCallBack(OnTaskTagChanged);
            this.chapterDescTag.AddChangeCallBack(OnChapterDescTagChanged);

            this.m_btnShowChapterList.AddClickCallBack(OnBtnShowChapterListClick);

            this.taskTagButton.Activated = true;
            OnLocalTagValueChangedFirst(true);


            UserBehaviorStatisticsModules.Instance.LogEvent(UBSEventKeyName.file_in);
            GameEvents.UIEvents.UI_Common_Event.OnCommonUIVisible.SafeInvoke(false);
            //CommonHelper.UItween(this.m_tweenPos);
        }

        private void User_OnNewChapterEvent(int type)
        {
            if (type == 0)
            {
                this.m_npcRedDot.Visible = true;
            }
            else if (type == 1)
            {
                this.m_clueRedDot.Visible = true;
            }
            //else if (type == 2)
            //{
            //    this.m_localRedDot.Visible = true;
            //}
        }

        private void OnpenChapterUIByIndex(int index)
        {
            if (0 == index)
                this.taskTagButton.Checked = true;
            else if (1 == index)
                this.localTagButton.Checked = true;
            else if (2 == index)
                this.npcTagButton.Checked = true;
            else if (3 == index)
                this.clueTagButton.Checked = true;
            else if (4 == index)
                this.chapterDescTag.Checked = true;
        }

        /// <summary>
        /// 显示NPC详细信息
        /// </summary>
        /// <param name="npcID"></param>
        private void ShowNPCDetailInfo(long npcID, ChapterNpcInfo npcInfo)
        {
            this.m_npcDetailComponent.SetNpcInfo(npcID, npcInfo);
            this.m_npcDetailComponent.Visible = true;
        }

        private void OnChapterInfoUpdated(ChapterInfo info)
        {
            this.m_currentDisplayChapterInfo = info;
            //this.chapterDescTag.Visible = this.m_currentDisplayChapterInfo.ChapterStatus == ChapterStatus.DONE;

            //if (this.m_chapterDescTagComponent.Visible)
            //    this.m_chapterDescTagComponent.Visible = this.m_currentDisplayChapterInfo.ChapterStatus == ChapterStatus.DONE;

            if (this.m_chapterTaskListTagComponent.Visible)
                this.m_chapterTaskListTagComponent.SetTaskList(info.TaskList, info.ChapterID);
            else if (this.m_chapterClueListComponent.Visible)
                this.m_chapterClueListComponent.SetClueItemList(info);
            else if (this.m_npcListComponent.Visible)
                this.m_npcListComponent.SetNpcList(info.NpcInfoList);
            else if (this.m_chapterSectionListComponent.Visible)
                this.m_chapterSectionListComponent.SetChapterInfo(info);
            else if (this.m_chapterDescTagComponent.Visible)
                this.m_chapterDescTagComponent.SetChapterDescData(info);
            else
            {
                this.localTagButton.Checked = false;
                this.npcTagButton.Checked = false;
                this.clueTagButton.Checked = false;
                this.chapterDescTag.Checked = false;
                this.taskTagButton.Checked = true;
                OnLocalTagValueChangedFirst(true);
            }


        }

        /// <summary>
        /// 显示所有章节列表
        /// </summary>
        /// <param name="btnShowChapterList"></param>
        private void OnBtnShowChapterListClick(GameObject btnShowChapterList)
        {
            EngineCoreEvents.AudioEvents.PlayAudio.SafeInvoke(Audio.AudioType.UISound, EngineCommonAudioKey.Button_Click_Common.ToString());

            FrameMgr.OpenUIParams param = new FrameMgr.OpenUIParams(UIDefine.UI_ChapterMap);


            if (this.m_currentDisplayChapterInfo != null)
                param.Param = this.m_currentDisplayChapterInfo.ChapterID;

            EngineCoreEvents.UIEvent.ShowUIEventWithParam.SafeInvoke(param);
        }

        private void OnUnlockNewChapter(ChapterInfo newChapterInfo)
        {
            //this.m_chapterListComponent.IsDirty = true;
        }

        /// <summary>
        /// 设置章节ID
        /// </summary>
        /// <param name="chapterID"></param>
        public void SetChapterID(long chapterID)
        {
            this.m_currentDisplayChapterInfo = this.m_playerChapterSystem.GetChapterLatestInfo(chapterID);
            this.m_chapterThumbTexture.TextureName = this.m_currentDisplayChapterInfo.ChapterConfData.cover;
            this.m_totalTaskNumLabel.Text = string.Format("{0}%", this.m_currentDisplayChapterInfo.taskProgressValue);
            this.m_taskProgressSlider.Value = this.m_currentDisplayChapterInfo.taskProgressValue / 100f;
            this.m_chapterNameLabel.Text = LocalizeModule.Instance.GetString(this.m_currentDisplayChapterInfo.ChapterConfData.name);
            this.m_currentDisplayChapterInfo.m_SyncComplete = ChapterSyncComplete;
            //int chapterNum = chapterID;
            //for (int i = 0; i < ConfChapter.array.Count; i++)
            //{
            //    if (ConfChapter.array[i].id == chapterID)
            //    {
            //        chapterNum++;
            //        break;
            //    }
            //}
            this.m_currentChapterNum.Text = "NO." + chapterID.ToString();

            if (m_currentDisplayChapterInfo.ChapterSyncStatus == SyncStatus.SYNCED)
                OnChapterInfoUpdated(this.m_currentDisplayChapterInfo);
        }


        private void ChapterSyncComplete()
        {
            this.m_totalTaskNumLabel.Text = string.Format("{0}%", this.m_currentDisplayChapterInfo.taskProgressValue);
            this.m_taskProgressSlider.Value = this.m_currentDisplayChapterInfo.taskProgressValue / 100f;
        }

        #region Toggle Switch

        private void OnLocalTagValueChangedFirst(bool value)
        {
            OnTaskTagChanged(value);
            //if (value)
            //{
            //    //this.m_chapterSectionListComponent.SetChapterInfo(this.m_currentDisplayChapterInfo);
            //    //this.m_chapterSectionListComponent.SetChapterMap(this.m_currentDisplayChapterInfo.ChapterConfData.cover);

            //    //this.m_chapterSectionListComponent.Visible = true;
            //}
            //else
            //    this.m_chapterTaskListTagComponent.Visible = false;

        }
        private void OnLocalTagValueChanged(bool value)
        {
            if (value)
            {
                //Debug.Log("OnLocalTagValueChanged ======================");
                EngineCoreEvents.AudioEvents.PlayAudio.SafeInvoke(Audio.AudioType.UISound, EngineCommonAudioKey.table_change.ToString());
                this.m_chapterSectionListComponent.SetChapterInfo(this.m_currentDisplayChapterInfo);
                this.m_chapterSectionListComponent.SetChapterMap(this.m_currentDisplayChapterInfo.ChapterConfData.cover);

                this.m_chapterSectionListComponent.Visible = true;
                this.m_npcDetailComponent.Visible = false;
                GameEvents.RedPointEvents.Sys_OnNewChapterEvent.SafeInvoke(2);
                this.m_localRedDot.Visible = false;
                RedPointManager.Instance.ReflashArchivesRedPoint();
                if (!this.m_isHide)
                {
                    OnChangeTagValueAnimator(this.m_chapterSectionListComponent.Widget, 1);
                    this.m_currentTarget = this.m_chapterSectionListComponent.Widget;
                    this.m_currentTagIndex = 1;
                }

            }
            else
            {
                TimeModule.Instance.SetTimeout(()=> { this.m_chapterSectionListComponent.Visible = false; },0.3f);
            }
                
        }

        private void OnClueTagValueChanged(bool value)
        {
            if (value)
            {
                EngineCoreEvents.AudioEvents.PlayAudio.SafeInvoke(Audio.AudioType.UISound, EngineCommonAudioKey.table_change.ToString());
                this.m_chapterClueListComponent.SetClueItemList(this.m_currentDisplayChapterInfo);
                this.m_chapterClueListComponent.Visible = true;
                GameEvents.RedPointEvents.Sys_OnNewChapterEvent.SafeInvoke(1);
                this.m_clueRedDot.Visible = false;
                this.m_npcDetailComponent.Visible = false;
                RedPointManager.Instance.ReflashArchivesRedPoint();
                OnChangeTagValueAnimator(this.m_chapterClueListComponent.Widget, 3);
                this.m_currentTagIndex = 3;
                this.m_currentTarget = this.m_chapterClueListComponent.Widget;
            }
            else
            {
                TimeModule.Instance.SetTimeout(()=> { this.m_chapterClueListComponent.Visible = false; },0.3f);
            }
                
        }

        private void OnNpcTagValueChanged(bool value)
        {
            if (value)
            {
                EngineCoreEvents.AudioEvents.PlayAudio.SafeInvoke(Audio.AudioType.UISound, EngineCommonAudioKey.table_change.ToString());
                m_npcListComponent.SetNpcList(this.m_currentDisplayChapterInfo.NpcInfoList);
                m_npcListComponent.Visible = true;
                GameEvents.RedPointEvents.Sys_OnNewChapterEvent.SafeInvoke(0);
                this.m_npcRedDot.Visible = false;
                RedPointManager.Instance.ReflashArchivesRedPoint();
                OnChangeTagValueAnimator(this.m_npcListComponent.Widget, 2);
                this.m_currentTarget = this.m_npcListComponent.Widget;
                this.m_currentTagIndex = 2;
            }
            else
            {
                TimeModule.Instance.SetTimeout(()=> { this.m_npcListComponent.Visible = false; },0.3f);
            }
        }

        private void OnChapterDescTagChanged(bool value)
        {
            if (value)
            {
                EngineCoreEvents.AudioEvents.PlayAudio.SafeInvoke(Audio.AudioType.UISound, EngineCommonAudioKey.table_change.ToString());
                this.m_chapterDescTagComponent.SetChapterDescData(this.m_currentDisplayChapterInfo);
                this.m_chapterDescTagComponent.Visible = true;
                this.m_npcDetailComponent.Visible = false;
                OnChangeTagValueAnimator(this.m_chapterDescTagComponent.Widget, 4);
                this.m_currentTarget = this.m_chapterDescTagComponent.Widget;
                this.m_currentTagIndex = 4;
            }
            else
            {
                TimeModule.Instance.SetTimeout(()=> { m_chapterDescTagComponent.Visible = false; },0.3f);
            }
                
        }

        private void OnTaskTagChanged(bool value)
        {
            if (value)
            {
                EngineCoreEvents.AudioEvents.PlayAudio.SafeInvoke(Audio.AudioType.UISound, EngineCommonAudioKey.table_change.ToString());
                m_chapterTaskListTagComponent.SetTaskList(this.m_currentDisplayChapterInfo.TaskList, this.m_currentDisplayChapterInfo.ChapterID);
                m_chapterTaskListTagComponent.Visible = true;
                this.m_npcDetailComponent.Visible = false;
                if (!this.m_isHide)
                {
                    OnChangeTagValueAnimator(this.m_chapterTaskListTagComponent.Widget, 0);
                    this.m_currentTarget = this.m_chapterTaskListTagComponent.Widget;
                    this.m_currentTagIndex = 0;
                }
                    
            }
            else
            {
                TimeModule.Instance.SetTimeout(()=> { m_chapterTaskListTagComponent.Visible = false; },0.3f);
            }
                
        }

        private RectTransform m_currentTarget = null;
        private int m_currentTagIndex = 0;
        private void OnChangeTagValueAnimator(RectTransform target,int index)
        {
            if (m_currentTagIndex > index)
            {
                m_currentTarget.DOLocalMoveX(1143f, 0.3f).SetEase(Ease.InQuad);
                target.anchoredPosition = new Vector2(-1143f, target.anchoredPosition.y);
                target.DOLocalMoveX(0f, 0.3f).SetEase(Ease.InQuad);
            }
            else if (m_currentTagIndex < index)
            {
                m_currentTarget.DOLocalMoveX(-1143f, 0.3f).SetEase(Ease.InQuad);
                target.anchoredPosition = new Vector2(1143f, target.anchoredPosition.y);
                target.DOLocalMoveX(0f, 0.3f).SetEase(Ease.InQuad);
            }
            //
        }

        #endregion

        private bool m_isHide = false;
        public override void OnHide()
        {
            EngineCoreEvents.AudioEvents.PlayAudio.SafeInvoke(Audio.AudioType.UISound, GameCustomAudioKey.zoom_out.ToString());

            base.OnHide();
            GameEvents.UIEvents.UI_Common_Event.OnCommonUIVisible.SafeInvoke(true);
            this.m_isHide = true;
            this.m_currentTarget = null;
            localTagButton.Checked = false;
            clueTagButton.Checked = false;
            taskTagButton.Checked = true;
            chapterDescTag.Checked = false;
            npcTagButton.Checked = false;

            OnLocalTagValueChanged(false);
            OnTaskTagChanged(true);
            OnChapterDescTagChanged(false);
            OnClueTagValueChanged(false);
            OnNpcTagValueChanged(false);
            this.m_currentTagIndex = -1;
            this.m_chapterSectionListComponent.Visible = false;
            this.m_npcListComponent.Visible = false;
            this.m_chapterDescTagComponent.Visible = false;
            this.m_chapterClueListComponent.Visible = false;
            this.m_chapterTaskListTagComponent.Visible = false;

            this.localTagButton.RemoveChangeCallBack(OnLocalTagValueChanged);
            this.clueTagButton.RemoveChangeCallBack(OnClueTagValueChanged);
            this.npcTagButton.RemoveChangeCallBack(OnNpcTagValueChanged);
            this.taskTagButton.RemoveChangeCallBack(OnTaskTagChanged);
            this.chapterDescTag.RemoveChangeCallBack(OnChapterDescTagChanged);

            this.m_btnShowChapterList.RemoveClickCallBack(OnBtnShowChapterListClick);

            GameEvents.ChapterEvents.OnUnlockChapter -= OnUnlockNewChapter;
            GameEvents.ChapterEvents.OnChapterInfoUpdated -= OnChapterInfoUpdated;
            GameEvents.RedPointEvents.User_OnNewChapterEvent -= User_OnNewChapterEvent;
            GameEvents.ChapterEvents.OnpenChapterUIByIndex -= OnpenChapterUIByIndex;
        }

        /// <summary>
        /// 章节详细关卡列表组件
        /// </summary>
        private class ChapterSectionTagComponent : GameUIComponent
        {
            private GameUIContainer m_sectionListContainer = null;
            //private GameTexture m_chapterMapTexture = null;
            //private GameObject m_existMountObj = null;
            //private GameButton m_chapterIntroComic = null;
            private ChapterInfo m_chapterInfo = null;

            protected override void OnInit()
            {
                //this.m_chapterMapTexture = Make<GameTexture>("SceneMap");
                this.m_sectionListContainer = Make<GameUIContainer>("ScrollView:Viewport");
                //this.m_chapterIntroComic = Make<GameButton>("Button");
            }

            public override void OnShow(object param)
            {
                base.OnShow(param);

                //this.m_chapterIntroComic.AddClickCallBack(OnBtnChapterIntroClick);
            }

            public void SetChapterMap(string chapterMap)
            {
                //this.m_chapterMapTexture.TextureName = chapterMap;
            }

            public void SetChapterInfo(ChapterInfo chapterInfo)
            {
                this.m_chapterInfo = chapterInfo;
                //string chapterMapPrefabName = $"Panel_case{chapterInfo.ChapterID}.prefab";
                //if (this.m_existMountObj != null)
                //{
                //    if (this.m_existMountObj.name == $"Panel_case{chapterInfo.ChapterID}")
                //    {
                //        m_existMountObj.SetActive(true);
                //        SetSectionList(chapterInfo.SceneInfoList, true);
                //        return;
                //    }
                //    else
                //    {
                //        GameObject.Destroy(this.m_existMountObj);
                //        this.m_existMountObj = null;
                //    }
                //}

                List<ChapterSceneInfo> unlockChapterList = chapterInfo.SceneInfoList.Where(info => info.Status == 1).ToList();

                SetSectionList(unlockChapterList, true);

                //GOGUITools.GetAssetAction.SafeInvoke(chapterMapPrefabName, (prefabName, obj) =>
                //{
                //    m_existMountObj = obj as GameObject;
                //    m_existMountObj.transform.SetParent(Widget);
                //    m_existMountObj.transform.localPosition = Vector3.zero;
                //    m_existMountObj.transform.localRotation = Quaternion.identity;
                //    m_existMountObj.transform.localScale = Vector3.one;

                //    for (int i = 0; i < this.m_sectionListContainer.ChildCount; ++i)
                //    {
                //        SectionComponent sectionComponent = this.m_sectionListContainer.GetChild<SectionComponent>(i);
                //        long sceneID = sectionComponent.SceneID;

                //        Transform mountTransform = m_existMountObj.transform.Find(sceneID.ToString());
                //        if (mountTransform == null)
                //            Debug.LogError($"scene {sceneID} chapter map mount not exist");

                //        sectionComponent.SetSectionPosition(mountTransform.position, mountTransform.eulerAngles);
                //        sectionComponent.Visible = true;
                //    }

                //}, LoadPriority.Default);
            }

            private void SetSectionList(IList<ChapterSceneInfo> sectionList, bool isVisible)
            {
                this.m_sectionListContainer.EnsureSize<SectionComponent>(sectionList.Count);
                for (int i = 0; i < sectionList.Count; ++i)
                {
                    SectionComponent sectionComponent = this.m_sectionListContainer.GetChild<SectionComponent>(i);
                    sectionComponent.SetSectionInfo(sectionList[i].SceneId, sectionList[i].Status == 1);
                    sectionComponent.Visible = isVisible;
                    sectionComponent.SectionChapterInfo = this.m_chapterInfo;
                }

            }

            private void OnBtnChapterIntroClick(GameObject btnChapterIntro)
            {
                EngineCoreEvents.AudioEvents.PlayAudio.SafeInvoke(Audio.AudioType.UISound, EngineCommonAudioKey.Button_Click_Common.ToString());
                long chapterIntroComicsID = this.m_chapterInfo.ChapterConfData.startSceneId;
                if (chapterIntroComicsID / 10000000 != 4)
                    Debug.LogError($"章节:{this.m_chapterInfo.ChapterID} ,开场漫画ID: {chapterIntroComicsID} 错误");
                else
                    StartCartoonManager.Instance.OpenStartCartoonForID(chapterIntroComicsID);

                Dictionary<UBSParamKeyName, object> enterSceneFromChapterMapDict = new Dictionary<UBSParamKeyName, object>();
                enterSceneFromChapterMapDict.Add(UBSParamKeyName.EnterSceneFromChapter, chapterIntroComicsID);
                UserBehaviorStatisticsModules.Instance.LogEvent(UBSEventKeyName.file_start_cartoon, null, enterSceneFromChapterMapDict);


            }

            public override void OnHide()
            {
                //this.m_chapterIntroComic.RemoveClickCallBack(OnBtnChapterIntroClick);
            }


            /// <summary>
            /// 关卡详情
            /// </summary>
            private class SectionComponent : GameUIComponent
            {
                protected class BaseScene : GameUIComponent
                {
                    protected GameTexture m_sectionThumb = null;
                    protected GameLabel m_lbSectionLabel = null;

                    public void SetSectionInfo(long sceneID, bool isFinish)
                    {
                        string sceneName = string.Empty;
                        string sceneThumb = string.Empty;

                        int sceneType = (int)(sceneID / 10000000);
                        switch (sceneType)
                        {
                            case 1:
                                ConfScene sceneConfig = ConfScene.Get(sceneID);
                                sceneName = sceneConfig.name;
                                sceneThumb = sceneConfig.thumbnail;
                                break;
                            case 2:
                                ConfJigsawScene jigSawScene = ConfJigsawScene.Get(sceneID);
                                sceneName = jigSawScene.name;
                                sceneThumb = jigSawScene.thumbnail;
                                break;
                            case 3:
                                ConfCartoonScene cartoonScene = ConfCartoonScene.Get(sceneID);
                                sceneName = cartoonScene.name;
                                sceneThumb = cartoonScene.thumbnail;
                                break;
                            default:
                                break;
                        }

                        this.m_lbSectionLabel.Text = LocalizeModule.Instance.GetString(sceneName);
                        this.m_sectionThumb.TextureName = sceneThumb;
                    }
                }

                private class SeekScene : BaseScene
                {
                    protected override void OnInit()
                    {
                        base.OnInit();
                        this.m_lbSectionLabel = Make<GameLabel>("Text");
                        this.m_sectionThumb = Make<GameTexture>("RawImage");
                    }
                }


                private class JigsawScene : BaseScene
                {
                    protected override void OnInit()
                    {
                        base.OnInit();
                        this.m_lbSectionLabel = Make<GameLabel>("Text");
                        this.m_sectionThumb = Make<GameTexture>("Panel:RawImage");
                    }
                }

                private SeekScene m_seek_scene = null;
                private JigsawScene m_jigsaw_scene = null;
                private long m_sceneId = 0;

                protected override void OnInit()
                {
                    this.m_seek_scene = Make<SeekScene>("Panel_scence");
                    this.m_jigsaw_scene = Make<JigsawScene>("Panel_jingsaw");
                }

                public override void OnShow(object param)
                {
                    base.OnShow(param);

                    m_seek_scene.AddClickCallBack(OnBtnEnterSceneClick);
                    m_jigsaw_scene.AddClickCallBack(OnBtnEnterSceneClick);
                }

                public void SetSectionPosition(Vector3 positionOnCanvas, Vector3 rotation)
                {
                    this.Widget.position = positionOnCanvas;
                    this.Widget.localEulerAngles = rotation;
                }

                public void SetSectionInfo(long sceneID, bool isFinish)
                {
                    m_seek_scene.Visible = false;
                    m_jigsaw_scene.Visible = false;

                    this.m_sceneId = sceneID;

                    int sceneType = (int)(sceneID / 10000000);
                    switch (sceneType)
                    {
                        case 1:
                            m_seek_scene.SetSectionInfo(SceneID, isFinish);
                            m_seek_scene.Visible = true;
                            break;
                        case 2:
                            m_jigsaw_scene.SetSectionInfo(SceneID, isFinish);
                            m_jigsaw_scene.Visible = true;
                            break;
                        case 3:

                            break;
                        default:
                            break;
                    }
                }

                private void OnBtnEnterSceneClick(GameObject btnEnterScene)
                {
                    EngineCoreEvents.AudioEvents.PlayAudio.SafeInvoke(Audio.AudioType.UISound, EngineCommonAudioKey.Button_Click_Common.ToString());
                    //EngineCoreEvents.UIEvent.ShowUIEventWithParam.SafeInvoke(new FrameMgr.OpenUIParams(UIDefine.UI_ENGER_GAME_UI) { Param = this.m_sceneId });

                    Dictionary<UBSParamKeyName, object> enterSceneFromChapterMapDict = new Dictionary<UBSParamKeyName, object>();
                    enterSceneFromChapterMapDict.Add(UBSParamKeyName.EnterSceneFromChapter, this.m_sceneId);
                    UserBehaviorStatisticsModules.Instance.LogEvent(UBSEventKeyName.file_start, null, enterSceneFromChapterMapDict);
                    EngineCoreEvents.UIEvent.HideUIEvent.SafeInvoke(UIDefine.UI_CHAPTER);
                    CommonHelper.OpenEnterGameSceneUI(this.m_sceneId, SectionChapterInfo);
                }

                public override void OnHide()
                {
                    base.OnHide();
                    m_seek_scene.RemoveClickCallBack(OnBtnEnterSceneClick);
                    m_jigsaw_scene.RemoveClickCallBack(OnBtnEnterSceneClick);
                }

                public ChapterInfo SectionChapterInfo { get; set; }

                public long SceneID => this.m_sceneId;
            }
        }

        /// <summary>
        /// 章节物品(线索)列表组件
        /// </summary>
        private class ChapterClueListTagCompoment : GameUIComponent
        {
            private GameUIContainer m_chapterItemsContainer = null;
            private GameLabel m_lbClueItemProgress = null;
            private GameButton m_btnEndCase = null;
            private ChapterInfo m_currentChapterInfo = null;
            private GameLabel m_noTipsLabel = null;
            protected override void OnInit()
            {
                this.m_chapterItemsContainer = Make<GameUIContainer>("ClueView:grid");
                this.m_lbClueItemProgress = Make<GameLabel>("Text");
                this.m_btnEndCase = Make<GameButton>("Button");
                this.m_noTipsLabel = Make<GameLabel>("noTips");
            }

            public override void OnShow(object param)
            {
                base.OnShow(param);

                this.m_btnEndCase.AddClickCallBack(OnBtnEndCaseClick);

                UserBehaviorStatisticsModules.Instance.LogEvent(UBSEventKeyName.file_clue);
            }

            public void SetClueItemList(ChapterInfo chapterInfo)
            {
                this.m_currentChapterInfo = chapterInfo;
                this.m_chapterItemsContainer.EnsureSize<ChapterClueItemComponent>(chapterInfo.ClueInfoLIst.Count);
                int m_totalClueCount = chapterInfo.ClueInfoLIst.Count;
                int m_foundCount = 0;
                for (int i = 0; i < chapterInfo.ClueInfoLIst.Count; ++i)
                {
                    ChapterClueItemComponent clueItem = this.m_chapterItemsContainer.GetChild<ChapterClueItemComponent>(i);
                    ChapterClueInfo clueInfo = chapterInfo.ClueInfoLIst[i];

                    string clueItemIcon = clueInfo.ClueId;
                    bool isCollect = clueInfo.Status == 1;
                    clueItem.SetClueItemData(clueItemIcon, isCollect);
                    if (isCollect)
                        m_foundCount++;

                    clueItem.Visible = isCollect;
                }
                this.m_noTipsLabel.Visible = m_foundCount == 0;
                this.m_lbClueItemProgress.Text = $"{(float)m_foundCount} / {m_totalClueCount}";
                if (m_foundCount != 0 && m_foundCount == chapterInfo.ClueInfoLIst.Count)
                    this.m_btnEndCase.Visible = true;
                else
                    this.m_btnEndCase.Visible = false;

            }


            private void OnBtnEndCaseClick(GameObject btnEndCase)
            {
                //进入章节结尾的拼图玩法
                long endCaseSceneID = this.m_currentChapterInfo.ChapterConfData.endSceneId;
                //EngineCoreEvents.UIEvent.ShowUIEventWithParam.SafeInvoke(new FrameMgr.OpenUIParams(UIDefine.UI_ENGER_GAME_UI) { Param = endCaseSceneID });

                Dictionary<UBSParamKeyName, object> enterFinishSceneKeypoint = new Dictionary<UBSParamKeyName, object>();
                enterFinishSceneKeypoint.Add(UBSParamKeyName.EnterChapterFinishSceneFromChapter, endCaseSceneID);
                UserBehaviorStatisticsModules.Instance.LogEvent(UBSEventKeyName.file_clue, null, enterFinishSceneKeypoint);

                CommonHelper.OpenEnterGameSceneUI(endCaseSceneID);
            }

            public override void OnHide()
            {
                this.m_btnEndCase.RemoveClickCallBack(OnBtnEndCaseClick);
            }



            /// <summary>
            /// 物品
            /// </summary>
            private class ChapterClueItemComponent : GameUIComponent
            {
                private GameTexture m_imgIcon = null;
                private GameLabel m_lbItemName = null;

                protected override void OnInit()
                {
                    this.m_imgIcon = Make<GameTexture>("Image");
                }

                public void SetClueItemData(string itemIconName, bool isCollected)
                {
                    this.m_imgIcon.TextureName = itemIconName;
                }
            }

        }

        /// <summary>
        /// NPC 列表
        /// </summary>
        private class NPCListTagComponent : GameUIComponent
        {
            private GameUIContainer m_npcIntroContailer = null;
            private Transform m_chatPivot = null;
            private ScrollRectComponent m_scrollRect = null;
            private GameScrollView m_gameScrollView = null;

            protected override void OnInit()
            {
                base.OnInit();

                this.m_scrollRect = Make<ScrollRectComponent>("grid");
                this.m_chatPivot = Widget.Find("chatPivot");
                this.m_gameScrollView = Make<GameScrollView>(gameObject);
            }

            public override void OnShow(object param)
            {
                base.OnShow(param);
                OnForbidNpcListScroll(false);
                GameEvents.UIEvents.UI_Archives_Event.OnForbidNpcListScroll += OnForbidNpcListScroll;
                //GameEvents.UIEvents.UI_Archives_Event.OnClickChatPerson += OnClickChatPerson;
            }

            public override void OnHide()
            {
                base.OnHide();
                GameEvents.UIEvents.UI_Archives_Event.OnForbidNpcListScroll -= OnForbidNpcListScroll;
            }

            public void SetNpcList(IList<ChapterNpcInfo> npcInfoList)
            {
                IList<ChapterNpcInfo> unlockChapterNpcList = npcInfoList.Where(npcInfo => npcInfo.Status == 1 && ConfNpc.Get(npcInfo.NpcId).identityType != 3).ToList();
                m_scrollRect.InitList(unlockChapterNpcList, this.m_chatPivot);
                //this.m_npcIntroContailer.EnsureSize<NPCIntroComponent>(unlockChapterNpcList.Count);

                //for (int i = 0; i < this.m_npcIntroContailer.ChildCount; ++i)
                //{
                //    NPCIntroComponent npcIntroComponent = this.m_npcIntroContailer.GetChild<NPCIntroComponent>(i);
                //    npcIntroComponent.SetNpcID(unlockChapterNpcList[i].NpcId, unlockChapterNpcList[i], this.m_chatPivot);
                //    npcIntroComponent.Visible = false;
                //    npcIntroComponent.Visible = true;
                //}

                //TimeModule.Instance.SetTimeout(() => { m_gameScrollView.ScrollToTop(); }, 0.05f);

            }

            private void OnForbidNpcListScroll(bool enable)
            {
                this.m_gameScrollView.scrollView.enabled = !enable;
            }
        }


        /// <summary>
        /// 章节详情Tag
        /// </summary>
        private class ChapterDescTagComponent : GameUIComponent
        {
            private GameLabel m_lbChapterTitle = null;
            private GameLabel m_lbChapterContent = null;
            private GameScrollView m_scrollView = null;
            private ChapterInfo m_chapterInfo = null;

            private GameToggleButton m_comicTog = null;
            private GameToggleButton m_storyTog = null;

            private GameUIComponent m_comicCom = null;
            private GameLabel m_pageLable = null;
            private GameUIComponent m_comicRoot = null;
            private GameImage m_btnComicNext = null;
            private GameImage m_comicNextImgBg = null;

            private int m_maxPage = 0;
            private GameVideoComponent m_video = null;
            //private StartCartoonCapter m_Capter = null;
            private int m_curPage = 0;
            protected override void OnInit()
            {
                this.m_scrollView = Make<GameScrollView>("CaseDocView");
                this.m_lbChapterTitle = this.m_scrollView.Make<GameLabel>("Viewport:Content:Text");
                this.m_lbChapterContent = this.m_scrollView.Make<GameLabel>("Viewport:Content:Text_content");

                this.m_comicTog = Make<GameToggleButton>("top:Toggle_1");
                this.m_storyTog = Make<GameToggleButton>("top:Toggle_2");

                this.m_comicCom = Make<GameUIComponent>("top:Image");
                this.m_comicNextImgBg = Make<GameImage>("top:Image:Image");
                this.m_pageLable = this.m_comicCom.Make<GameLabel>("Text");
                this.m_comicRoot = Make<GameUIComponent>("Comic");
                this.m_video = this.m_comicRoot.Make<GameVideoComponent>("RawImage");
                this.m_btnComicNext = Make<GameImage>("bg");
            }

            public override void OnShow(object param)
            {
                m_scrollView.ScrollToTop();
                this.m_comicTog.AddChangeCallBack(OnToggleChange_mic);
                this.m_storyTog.AddChangeCallBack(OnToggleChange_story);
                this.m_btnComicNext.AddClickCallBack(OnBtnComicsNext);
                GameEvents.UIEvents.UI_StartCartoon_Event.OnNext += OnNextComic;
                this.m_comicTog.Checked = true;
                OnPlayComics();
                UserBehaviorStatisticsModules.Instance.LogEvent(UBSEventKeyName.file_story);
            }

            public override void OnHide()
            {
                base.OnHide();
                this.m_comicTog.RemoveChangeCallBack(OnToggleChange_mic);
                this.m_storyTog.RemoveChangeCallBack(OnToggleChange_story);
                this.m_btnComicNext.RemoveClickCallBack(OnBtnComicsNext);
                GameEvents.UIEvents.UI_StartCartoon_Event.OnNext -= OnNextComic;
            }

            public void SetChapterDescData(ChapterInfo chapterInfo)
            {
                this.m_chapterInfo = chapterInfo;
                this.m_storyTog.Visible = chapterInfo.ChapterStatus == ChapterStatus.DONE;
                this.m_scrollView.Visible = chapterInfo.ChapterStatus == ChapterStatus.DONE && this.m_storyTog.Checked;

                this.m_lbChapterTitle.Text = LocalizeModule.Instance.GetString(chapterInfo.ChapterConfData.name);
                this.m_lbChapterContent.Text = LocalizeModule.Instance.GetString(chapterInfo.ChapterConfData.document);
                //if (this.m_scrollView.Visible)
                //{

                //}
            }

            private void OnToggleChange_mic(bool value)
            {
                m_comicCom.Visible = value;
                this.m_comicRoot.Visible = value;
            }

            private void OnToggleChange_story(bool value)
            {
                this.m_scrollView.Visible = m_chapterInfo.ChapterStatus == ChapterStatus.DONE && value;
            }

            string comicsResName = string.Empty;

            private void OnPlayComics()
            {
                EngineCoreEvents.AudioEvents.PlayAudio.SafeInvoke(Audio.AudioType.UISound, EngineCommonAudioKey.Button_Click_Common.ToString());
                long chapterIntroComicsID = this.m_chapterInfo.ChapterConfData.startSceneId;
                if (chapterIntroComicsID / 10000000 != 4)
                    Debug.LogError($"章节:{this.m_chapterInfo.ChapterID} ,开场漫画ID: {chapterIntroComicsID} 错误");
                else
                {
                    ConfCartoonScene confScene = ConfCartoonScene.Get(chapterIntroComicsID);

                    if (comicsResName.Equals(confScene.sceneInfoIds[0]))
                    {
                        return;
                    }

                    //if (!string.IsNullOrEmpty(comicsResName) && m_Capter != null)
                    //{
                    //    if (m_Capter != null)
                    //    {
                    //        GameObject.DestroyImmediate(m_Capter.gameObject);
                    //    }
                    //}
                    if (comicsResName.Contains(".mp4"))
                    {
                        this.m_video.VideoName = confScene.sceneInfoIds[0];
                    }
                    //comicsResName = confScene.sceneInfoIds[0];
                    //GOGUI.GOGUITools.GetAssetAction.SafeInvoke(comicsResName, (prefabName, obj) =>
                    //{
                    //    prefabName = prefabName.Replace(".prefab", "");
                    //    GameObject capterObj = obj as GameObject;
                    //    capterObj.transform.localPosition = Vector3.zero;
                    //    capterObj.transform.localScale = Vector3.one;
                    //    capterObj.transform.eulerAngles = Vector3.one;
                    //    capterObj.transform.SetParent(this.m_comicRoot.Widget, false);
                    //    capterObj.transform.name = prefabName;
                    //    this.m_maxPage = capterObj.transform.childCount;
                    //    m_Capter = Make<StartCartoonCapter>(prefabName);
                    //    m_Capter.Visible = true;
                    //    this.m_curPage = 1;
                    //    this.m_pageLable.Text = string.Format("{0}/{1}", this.m_curPage, this.m_maxPage);
                    //    this.m_pageLable.Visible = true;
                    //}, LoadPriority.Default);
                }
                //StartCartoonManager.Instance.OpenStartCartoonForID(chapterIntroComicsID);

                Dictionary<UBSParamKeyName, object> enterSceneFromChapterMapDict = new Dictionary<UBSParamKeyName, object>();
                enterSceneFromChapterMapDict.Add(UBSParamKeyName.EnterSceneFromChapter, chapterIntroComicsID);
                UserBehaviorStatisticsModules.Instance.LogEvent(UBSEventKeyName.file_start_cartoon, null, enterSceneFromChapterMapDict);
            }

            private void OnBtnComicsNext(GameObject obj)
            {
                if (this.m_curPage >= this.m_maxPage)
                {
                    GameEvents.UIEvents.UI_StartCartoon_Event.OnResetPage.SafeInvoke();
                    this.m_curPage = 1;
                    this.m_pageLable.Text = string.Format("{0}/{1}", this.m_curPage, this.m_maxPage);
                    this.m_comicNextImgBg.Sprite = "btn_skip_type5.png";
                    return;
                }

                GameEvents.UIEvents.UI_StartCartoon_Event.OnNextClick.SafeInvoke();
            }

            private void OnNextComic()
            {
                this.m_curPage++;
                if (this.m_curPage == this.m_maxPage)
                {
                    this.m_comicNextImgBg.Sprite = "btn_skip_type6.png";
                }
                this.m_pageLable.Text = string.Format("{0}/{1}", this.m_curPage, this.m_maxPage);
            }
        }

        /// <summary>
        /// NPC 详情组件
        /// </summary>
        private class NPCDetailComponent : GameUIComponent
        {
            private GameUIComponent m_npcDetailInfoRoot = null;
            private GameLabel m_lbNpcName = null;
            private GameLabel m_lbNpcAge = null;
            private GameLabel m_lbNpcAstrology = null;
            private GameLabel m_lbNpcHeight = null;
            private GameLabel m_lbFigure = null;
            private GameLabel m_lbNpcOccupation = null;
            private GameLabel m_lbNpcRace = null;
            private GameLabel m_lbIdentity = null;
            private GameLabel m_lbNpcDesc = null;
            //private GameTexture m_lbNpcPicture = null;
            private GameButton m_btnClose = null;

            private ConfNpc m_npcConfig = null;
            private ChapterNpcInfo produce = null;

            //private GameButton m_btnChatWithNpc = null;
            private long m_currentTalkID = 0L;

            protected override void OnInit()
            {
                this.m_npcDetailInfoRoot = Make<GameUIComponent>("Panel_tipsanimate:Image:Panel:NpcContentView:Viewport:Content");

                this.m_lbNpcName = Make<GameLabel>("Panel_tipsanimate:Text");
                this.m_lbNpcAge = this.m_npcDetailInfoRoot.Make<GameLabel>("Image_age:Text_2");
                this.m_lbNpcAstrology = this.m_npcDetailInfoRoot.Make<GameLabel>("Image_astrology:Text_2");
                this.m_lbNpcHeight = this.m_npcDetailInfoRoot.Make<GameLabel>("Image_height:Text_2");
                this.m_lbFigure = this.m_npcDetailInfoRoot.Make<GameLabel>("Image_figure:Text_2");
                this.m_lbNpcOccupation = this.m_npcDetailInfoRoot.Make<GameLabel>("Image_occupation:Text_2");
                this.m_lbNpcRace = this.m_npcDetailInfoRoot.Make<GameLabel>("Image_race:Text_2");
                this.m_lbIdentity = this.m_npcDetailInfoRoot.Make<GameLabel>("Image_identity:Text_2");
                this.m_lbNpcDesc = this.m_npcDetailInfoRoot.Make<GameLabel>("Text_2 (1)");
                this.m_btnClose = Make<GameButton>("Panel_tipsanimate:Button");
                //this.m_lbNpcPicture = Make<GameTexture>("RawImage (1)");
                //this.m_btnChatWithNpc = Make<GameButton>("Button_converse");
            }

            public override void OnShow(object param)
            {
                base.OnShow(param);
                this.m_btnClose.AddClickCallBack(BtnClose);
                //SetCloseBtnID("Panel_tipsanimate:Button");
                //m_btnChatWithNpc.AddClickCallBack(OnBtnChatWithNpcClick);
            }

            public void SetNpcInfo(long npcId, ChapterNpcInfo produce)
            {
                m_npcConfig = ConfNpc.Get(npcId);
                this.produce = produce;
                if (m_npcConfig == null)
                    Debug.LogError($"npc {npcId} not exist");

                Dictionary<UBSParamKeyName, object> viewNpcInfoKeyPoint = new Dictionary<UBSParamKeyName, object>();
                viewNpcInfoKeyPoint.Add(UBSParamKeyName.View_NPCInfo, npcId);
                UserBehaviorStatisticsModules.Instance.LogEvent(UBSEventKeyName.file_NPC, null, viewNpcInfoKeyPoint);

                RefreshNPCInfo();

            }

            /// <summary>
            /// Refresh ui
            /// </summary>
            public void RefreshNPCInfo()
            {
                long dialogID1 = this.m_npcConfig.dialogueId;
                long dialogID2 = this.m_npcConfig.dialogueId2;
                long dialogID3 = this.m_npcConfig.dialogueId3;
                long dialogID4 = this.m_npcConfig.dialogueId4;
                this.m_currentTalkID = this.produce.DialogueId;

                //NPC解锁就显示基本信息  status ==2
                this.m_lbNpcName.Text = LocalizeModule.Instance.GetString(this.m_npcConfig.name);
                this.m_lbNpcAge.Text = this.m_npcConfig.age.ToString();
                this.m_lbNpcAstrology.Text = LocalizeModule.Instance.GetString(this.m_npcConfig.horoscope);
                this.m_lbNpcHeight.Text = $"{this.m_npcConfig.height}";
                this.m_lbFigure.Text = LocalizeModule.Instance.GetString(this.m_npcConfig.otherFeatures);
                this.m_lbNpcOccupation.Text = LocalizeModule.Instance.GetString(this.m_npcConfig.profession);
                this.m_lbIdentity.Text = LocalizeModule.Instance.GetString(this.m_npcConfig.identity);
                //this.m_lbNpcPicture.TextureName = this.m_npcConfig.icon;
                this.m_lbNpcRace.Text = LocalizeModule.Instance.GetString(this.m_npcConfig.race);
                this.m_lbNpcDesc.Text = "";


                //第一段，解锁背景信息
                if (this.produce.DialogueId == dialogID2)
                    this.m_lbNpcDesc.Text = LocalizeModule.Instance.GetString(this.m_npcConfig.background);

                //第二段，加上 案件描述信息
                if (this.produce.DialogueId == dialogID3)
                {
                    StringBuilder sb = new StringBuilder();
                    sb.AppendLine(LocalizeModule.Instance.GetString(this.m_npcConfig.background));
                    sb.AppendLine(LocalizeModule.Instance.GetString(this.m_npcConfig.caseDescs));
                    this.m_lbNpcDesc.Text = sb.ToString();
                }

                //第四段，加入 支线信息
                if (this.produce.DialogueId == dialogID4)
                {
                    StringBuilder sb = new StringBuilder();
                    sb.AppendLine(LocalizeModule.Instance.GetString(this.m_npcConfig.background));
                    sb.AppendLine(LocalizeModule.Instance.GetString(this.m_npcConfig.caseDescs));
                    sb.AppendLine(LocalizeModule.Instance.GetString(this.m_npcConfig.branchDescs));
                    this.m_lbNpcDesc.Text = sb.ToString();
                }
            }

            private void OnBtnChatWithNpcClick(GameObject btn)
            {
                TalkUIHelper.OnStartTalk(this.m_currentTalkID,TalkDialogEnum.AchiveTalk);

                Visible = false;
                //LogicHandler.CloseFrame();

                Dictionary<UBSParamKeyName, object> _param = new Dictionary<UBSParamKeyName, object>();
                _param.Add(UBSParamKeyName.ContentID, this.m_currentTalkID);
                UserBehaviorStatisticsModules.Instance.LogEvent(UBSEventKeyName.file_NPCdialogue, null, _param);
            }

            public override void OnHide()
            {
                base.OnHide();
                this.m_btnClose.RemoveClickCallBack(BtnClose);
            }

            private void BtnClose(GameObject obj)
            {
                Visible = false;
                GameEvents.UIEvents.UI_Archives_Event.OnCloseNpcDetail.SafeInvoke();
                GameEvents.UIEvents.UI_Archives_Event.OnForbidNpcListScroll.SafeInvoke(false);
            }
        }
    }
}