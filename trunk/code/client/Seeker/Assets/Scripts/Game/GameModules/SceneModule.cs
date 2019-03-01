/********************************************************************
	created:  2018-4-8 11:27:50
	filename: SceneModule.cs
	author:	  songguangze@outlook.com
	
	purpose:  游戏场景逻辑Module
*********************************************************************/

#define GUEST_LOGIN
#define WAV

using EngineCore;
using HedgehogTeam.EasyTouch;
using System.Collections.Generic;

namespace SeekerGame
{
    public class SceneModule : AbstractModule
    {
        private SceneBase m_currentScene = null;
        private static SceneModule m_intance;
        private ConfScene m_sceneData = null;
        private long m_taskConfID = -1;
        public SceneModule()
        {
            m_intance = this;
            AutoStart = true;
        }

        public override void Start()
        {
            base.Start();

            GameEvents.SceneEvents.OnEnterScene += OnEnterScene;
            GameEvents.SceneEvents.OnLeaveScene += OnLeaveScene;

            GameEvents.SceneEvents.OnEnterTalkScene += OnEnterTalkScene;
            GameEvents.SceneEvents.OnLeaveTalkScene += OnLeaveTalkScene;

            GameEvents.SceneEvents.OnEnterSeekScene += EnterSeekScene;

            //MessageHandler.RegisterMessageHandler(MessageDefine.SCSceneEnterResponse, OnEnterSceneResponse);
            MessageHandler.RegisterMessageHandler(MessageDefine.SCSceneRewardResponse, OnSCGameCurrencyResponse);
        }


        /// <summary>
        /// 尝试创建游戏
        /// </summary>
        /// <param name="sceneId"></param>
        public bool TryCreateGameScene(long sceneId, long taskConfID = -1)
        {
            this.m_taskConfID = taskConfID;
            m_sceneData = ConfScene.Get(sceneId);
            if (m_sceneData == null)
                return false;

            return true;
        }

        private void EnterSeekScene(SCSceneEnterResponse msgResponse)
        {
            OnEnterSceneResponse(msgResponse);
        }

        /// <summary>
        /// 进入场景结果
        /// </summary>
        /// <param name="msgresponse"></param>
        private void OnEnterSceneResponse(object msgResponse)
        {
            SCSceneEnterResponse msg = msgResponse as SCSceneEnterResponse;
            if (!MsgStatusCodeUtil.OnError(msg.ResponseStatus))
            {
                int gameMode = (this.m_sceneData.playMode / 10000) % 10;
                if (this.m_currentScene != null)
                    this.m_currentScene.DestroyScene();

                LoadingProgressManager.Instance.LoadProgressScene();
                BigWorldManager.Instance.ClearBigWorld();
                //预加载音频
#if MP3
           EngineCoreEvents.ResourceEvent.PreloadAssetEvent.SafeInvoke(ConfSound.Get("Game_01").SoundPath, null);
#elif WAV
                //EngineCoreEvents.ResourceEvent.PreloadAssetEvent.SafeInvoke(ConfSound.Get("Game_01").WavPath, null);
#endif
                GameEvents.System_Events.PlayMainBGM(false);

                this.m_currentScene = SceneFactory.Instance.CreateScene((SceneMode)gameMode, (int)this.m_sceneData.id);
                (this.m_currentScene as GameSceneBase).InitScene(this.m_sceneData, (IList<long>)msg.SceneExhibits, (IList<long>)msg.TaskExhibits, (IList<long>)msg.OfficerIds, msg.Seconds);

                if (msg.IsDropScene)
                {
                    (this.m_currentScene as GameSceneBase).InitRandomOutput(msg.TaskExhibits.Count, msg.DropItems);
                }

            }
        }

        private void OnSCGameCurrencyResponse(object msgResponse)
        {
            Dictionary<UBSParamKeyName, object> _params = new Dictionary<UBSParamKeyName, object>()
                            {
                                { UBSParamKeyName.TotalTime, ((GameSceneBase)CurrentScene).GetElapseTime()},
                                { UBSParamKeyName.Error_Times, ((GameSceneBase)CurrentScene).TotalErrorTimes},
                                { UBSParamKeyName.Punish_Times, ((GameSceneBase)CurrentScene).GetPunishTime()},
                            };
            UserBehaviorStatisticsModules.Instance.LogEvent(UBSEventKeyName.game_time, null, _params);


            SCSceneRewardResponse _msg = msgResponse as SCSceneRewardResponse;

            TimeModule.Instance.SetTimeout(() =>
            {
                if (!MsgStatusCodeUtil.OnError(_msg.ResponseStatus))
                {
                    if (0 == _msg.UpLevelRewards.Count)
                    {
                        WinFailData data = new WinFailData(ENUM_SEARCH_MODE.E_SEARCH_ROOM, _msg);

                        FrameMgr.OpenUIParams param = new FrameMgr.OpenUIParams(UIDefine.UI_WIN);

                        param.Param = data;

                        EngineCoreEvents.UIEvent.ShowUIEventWithParam.SafeInvoke(param);

                        Dictionary<UBSParamKeyName, object> _param = new Dictionary<UBSParamKeyName, object>();
                        _param.Add(UBSParamKeyName.Success, _msg.SceneId > 0 ? 1 : 0);
                        _param.Add(UBSParamKeyName.SceneID, SceneModule.Instance.SceneData.id);
                        UserBehaviorStatisticsModules.Instance.LogEvent(UBSEventKeyName.game_finish, _msg.SceneId > 0 ? 1 : 0, _param);

                        if (_msg.SceneId < 0)
                        {
                            //PushGiftManager.Instance.TurnOn(ENUM_PUSH_GIFT_BLOCK_TYPE.E_SEEK);
                        }
                    }
                    else
                    {
                        SceneLevelUpData data = new SceneLevelUpData()
                        {
                            msg = _msg,
                            m_click_act = null,
                        };

                        FrameMgr.OpenUIParams param = new FrameMgr.OpenUIParams(UIDefine.UI_SCENE_LEVEL_UP);
                        param.Param = data;

                        EngineCoreEvents.UIEvent.ShowUIEventWithParam.SafeInvoke(param);
                    }
                }
                else
                {
                    SCSceneRewardResponse rsp = new SCSceneRewardResponse();
                    rsp.SceneId = -1;
                    WinFailData data = new WinFailData(ENUM_SEARCH_MODE.E_SEARCH_ROOM, rsp);
                    FrameMgr.OpenUIParams param = new FrameMgr.OpenUIParams(UIDefine.UI_WIN);
                    param.Param = data;
                    EngineCoreEvents.UIEvent.ShowUIEventWithParam.SafeInvoke(param);

                    Dictionary<UBSParamKeyName, object> _param = new Dictionary<UBSParamKeyName, object>();
                    _param.Add(UBSParamKeyName.Success, 0);
                    _param.Add(UBSParamKeyName.Description, _msg.ResponseStatus.Code);
                    _param.Add(UBSParamKeyName.SceneID, SceneModule.Instance.SceneData.id);
                    UserBehaviorStatisticsModules.Instance.LogEvent(UBSEventKeyName.game_finish, null, _param);

                    //PushGiftManager.Instance.TurnOn(ENUM_PUSH_GIFT_BLOCK_TYPE.E_SEEK);
                }



            }, 2.5f);
        }

        private void OnEnterTalkScene(string sceneName)
        {
            if (this.m_currentScene != null)
                this.m_currentScene.DestroyScene();

            LoadingProgressManager.Instance.LoadProgressTalkScene();
            BigWorldManager.Instance.ClearBigWorld();
            this.m_currentScene = SceneFactory.Instance.CreateScene(SceneMode.NORMALTALK) as NormalTalkScene;
            (this.m_currentScene as NormalTalkScene).InitScene(sceneName);
        }

        private void OnLeaveTalkScene()
        {
            if (m_currentScene is NormalTalkScene)
            {
                OnLeaveScene();
            }
            EnterMainScene();
        }



        public void EnterExhibitionHallScene()
        {
            if (this.m_currentScene != null)
                this.m_currentScene.DestroyScene();

            LoadingProgressManager.Instance.LoadProgressTalkScene();
            BigWorldManager.Instance.ClearBigWorld();
            this.m_currentScene = SceneFactory.Instance.CreateScene(SceneMode.EXHITITIONHALL) as ExhititionHallScene;
            (this.m_currentScene as ExhititionHallScene).InitScene();
        }

        private void OnLeaveExhibitionHallScene()
        {
            if (m_currentScene is NormalTalkScene)
            {
                OnLeaveScene();
            }
            EnterMainScene();
        }

        /// <summary>
        /// 开始游戏
        /// </summary>
        public void StartGame()
        {
            if (this.m_currentScene is GameSceneBase)
            {
                (this.m_currentScene as GameSceneBase).StartGame();

                GameEvents.MainGameEvents.OnGameOver += OnGameOverHandler;
            }
        }

        public void CreateScene(SceneMode sceneMode, int sceneId = -1)
        {
            this.m_currentScene = SceneFactory.Instance.CreateScene(SceneMode.NORMALSCENE);

        }


        public void EnterMainScene(string open_ui_name_ = null)
        {
            CreateScene(SceneMode.NORMALSCENE);
            if (null != open_ui_name_)
            {
                //FrameMgr.OpenUIParams data_entry = new FrameMgr.OpenUIParams(UIDefine.UI_GAMEENTRY)
                //{
                //    Param = open_ui_name_,
                //};

                //FrameMgr.OpenUIParams data_banner = new FrameMgr.OpenUIParams(UIDefine.UI_BANNER)
                //{
                //    Param = open_ui_name_,
                //};

                //EngineCoreEvents.UIEvent.ShowUIEventWithParam.SafeInvoke(data_entry);
                //EngineCoreEvents.UIEvent.ShowUIEventWithParam.SafeInvoke(data_banner);
                BigWorldManager.Instance.LoadBigWorld(open_ui_name_);
            }
            else
            {
                BigWorldManager.Instance.LoadBigWorld();
                //EngineCoreEvents.UIEvent.ShowUIEvent.SafeInvoke(UIDefine.UI_GAMEENTRY);
                //EngineCoreEvents.UIEvent.ShowUIEvent.SafeInvoke(UIDefine.UI_BANNER);
            }
#if GUEST_LOGIN
            EngineCoreEvents.UIEvent.HideUIEvent.SafeInvoke(UIDefine.UI_GUEST_LOGIN);
#else
            EngineCoreEvents.UIEvent.HideUIEvent.SafeInvoke(UIDefine.UI_LOGIN);
#endif


        }

        public override void Update()
        {
            if (m_isStarted)
            {
                if (m_currentScene != null)
                    m_currentScene.Update();
            }
            VitManager.Instance.Tick();

            //实体管理器以后功能健全可当一个Module走生命周期 guangze song
            EntityManager.Instance.Update();
        }


        public void EnableSceneInput(bool isEnable)
        {
            if (isEnable)
            {
                EngineCoreEvents.InputEvent.OnOneFingerTouchup += OnTapScreen;
            }
            else
            {
                EngineCoreEvents.InputEvent.OnOneFingerTouchup -= OnTapScreen;
            }

        }


        private void OnTapScreen(Gesture gesture)
        {

        }


        public void OnEnterScene()
        {
            if (this.m_currentScene != null)
            {
                if ((int)this.m_currentScene.SceneMode < 10)
                {
                    EngineCoreEvents.UIEvent.HideUIEvent.SafeInvoke(UIDefine.UI_GAMEENTRY);
                    EngineCoreEvents.UIEvent.HideUIEvent.SafeInvoke(UIDefine.UI_BANNER);
                    EngineCoreEvents.UIEvent.HideUIEvent.SafeInvoke(UIDefine.UI_BUILD_TOP);
                    EngineCoreEvents.UIEvent.HideUIEvent.SafeInvoke(UIDefine.UI_TASK_ON_BUILD);
                    if (!(m_currentScene is NormalTalkScene) && !(m_currentScene is ExhititionHallScene))
                    {
                        EngineCoreEvents.UIEvent.ShowUIEventWithParam.SafeInvoke(new FrameMgr.OpenUIParams(UIDefine.UI_GAME_READY)
                        {
                            //Param = this.m_sceneData.id
                            Param = new MainSceneData()
                            {
                                sceneID = this.m_sceneData.id,
                                taskConfID = this.m_taskConfID
                            }
                        });
                        EngineCoreEvents.UIEvent.ShowUIEventWithParam.SafeInvoke(new FrameMgr.OpenUIParams(UIDefine.UI_GAME_MAIN)
                        {
                            //Param = this.m_sceneData.id
                            Param = new MainSceneData()
                            {
                                sceneID = this.m_sceneData.id,
                                taskConfID = this.m_taskConfID
                            }
                        });

                        EnableSceneInput(true);
                    }
                    else if (m_currentScene is ExhititionHallScene)
                    {
                        //展厅ui
                        EngineCoreEvents.UIEvent.ShowUIEvent.SafeInvoke(UIDefine.UI_EXHIBITION_HALL);
                    }
                }
            }
        }

        /// <summary>
        /// 退出场景
        /// </summary>
        private void OnLeaveScene()
        {
            if (this.m_currentScene != null)
            {
                if ((int)this.m_currentScene.SceneMode < 10)
                {
                    this.m_currentScene.DestroyScene();
                    this.m_sceneData = null;
                }
            }
            InputModule.Instance.Enable = false;
            EngineCoreEvents.ResourceEvent.LeaveScene.SafeInvoke();
            //BigWorldManager.Instance.LoadBigWorld();


        }

        /// <summary>
        /// 游戏结束处理
        /// </summary>
        /// <param name="gameResult"></param>
        private void OnGameOverHandler(SceneBase.GameResult gameResult)
        {
            //GameEvents.MainGameEvents.OnGameOver -= OnGameOverHandler;

            this.m_currentScene.StopAndRemoveSceneBGM(false);

            if ((byte)gameResult > 127)
            {
                GameEvents.MainGameEvents.OnGameOver -= OnGameOverHandler;
                TimeModule.Instance.SetTimeout(() => { EngineCoreEvents.UIEvent.HideUIEvent.SafeInvoke(UIDefine.UI_GAME_MAIN); }, (byte)gameResult > 127 ? 3f : 0.1f);
                CSSceneRewardRequest request = new CSSceneRewardRequest();
                request.PlayerId = GlobalInfo.MY_PLAYER_ID;
                request.SceneId = SceneModule.Instance.SceneData.id;
                request.Result = (byte)gameResult > 127 ? 1 : 0;

                GameEvents.NetWorkEvents.SendMsg.SafeInvoke(request);      //避免结算界面还没出来闪loading
            }
            //else
            //{

            //}


        }



        //private void OnForceGameOverHandler(SceneBase.GameResult gameResult)
        //{
        //    EngineCoreEvents.UIEvent.HideUIEvent.SafeInvoke(UIDefine.UI_GAME_MAIN);
        //    this.m_currentScene.StopAndRemoveSceneBGM(false);
        //    //EngineCoreEvents.UIEvent.ShowUIEventWithParam.SafeInvoke(new FrameMgr.OpenUIParams(UIDefine.UI_CLEARING)
        //    //{
        //    //    IsBlur = true,
        //    //    Param = (byte)gameResult > 127
        //    //});

        //    if ((byte)gameResult > 127)
        //    {
        //        CSSceneRewardRequest request = new CSSceneRewardRequest();
        //        request.PlayerId = GlobalInfo.MY_PLAYER_ID;
        //        request.SceneId = SceneModule.Instance.SceneData.id;
        //        request.Result = (byte)gameResult > 127 ? 1 : 0;

        //        GameEvents.NetWorkEvents.SendMsg.SafeInvoke(request);
        //    }
        //    else
        //    {
        //        SCSceneRewardResponse msg = new SCSceneRewardResponse();
        //        msg.SceneId = -1;
        //        WinFailData data = new WinFailData(ENUM_SEARCH_MODE.E_SEARCH_ROOM, msg);

        //        FrameMgr.OpenUIParams param = new FrameMgr.OpenUIParams(UIDefine.UI_WIN);

        //        param.Param = data;

        //        EngineCoreEvents.UIEvent.ShowUIEventWithParam.SafeInvoke(param);
        //    }
        //}


        public override void Dispose()
        {
            GameEvents.SceneEvents.OnEnterScene -= OnEnterScene;
            GameEvents.SceneEvents.OnLeaveScene -= OnLeaveScene;
            GameEvents.SceneEvents.OnEnterTalkScene -= OnEnterTalkScene;
            GameEvents.SceneEvents.OnLeaveTalkScene -= OnLeaveTalkScene;
            GameEvents.SceneEvents.OnEnterSeekScene -= EnterSeekScene;
            GameEvents.MainGameEvents.OnGameOver -= OnGameOverHandler;
            //MessageHandler.UnRegisterMessageHandler(MessageDefine.SCSceneEnterResponse, OnEnterSceneResponse);
            MessageHandler.UnRegisterMessageHandler(MessageDefine.SCSceneRewardResponse, OnSCGameCurrencyResponse);
        }


        private void OnPrepareScene()
        {
            int gameMode = (this.m_sceneData.playMode / 10000) % 10;
            if (this.m_currentScene != null)
                this.m_currentScene.DestroyScene();

            LoadingProgressManager.Instance.LoadProgressScene();
            BigWorldManager.Instance.ClearBigWorld();
            //预加载音频
#if MP3
           EngineCoreEvents.ResourceEvent.PreloadAssetEvent.SafeInvoke(ConfSound.Get("Game_01").SoundPath, null);
#elif WAV
            //EngineCoreEvents.ResourceEvent.PreloadAssetEvent.SafeInvoke(ConfSound.Get("Game_01").WavPath, null);
#endif

            GameEvents.System_Events.PlayMainBGM(false);
            this.m_currentScene = SceneFactory.Instance.CreateScene((SceneMode)gameMode, (int)this.m_sceneData.id);

        }

        #region 1122本地直接进入场景
        public void OnEnterLocalSceneByID(int sceneID, IList<long> sceneExhibits, IList<long> taskExhibits, bool needPolice)
        {
            TryCreateGameScene(sceneID);
            int totalTime = 0;
            IList<long> polices = new List<long>();
#if OFFICER_SYS
            if (needPolice)
            {
                polices = PoliceDispatchManager.Instance.GetAllDispathOfficersID();
                for (int i = 0; i < polices.Count; i++)
                {
                    ConfOfficer confPolice = ConfOfficer.Get(polices[i]);
                    if (confPolice == null)
                    {
                        continue;
                    }
                    totalTime += confPolice.secondGain;
                }
            }
            else
            {
                totalTime = 18000; //30分钟
            }
#endif
            TimeModule.Instance.SetTimeout(() =>
            {
                OnPrepareScene();
                (this.m_currentScene as GameSceneBase).InitScene(this.m_sceneData, sceneExhibits, taskExhibits, polices, totalTime);
            }, 0.5f);

        }
        #endregion

        public ConfScene SceneData
        {
            get { return this.m_sceneData; }
        }

        public SceneBase CurrentScene
        {
            get { return this.m_currentScene; }
        }

        public static SceneModule Instance
        {
            get { return m_intance; }
        }

    }

    public class MainSceneData
    {
        public long sceneID;
        public long taskConfID;
    }
}