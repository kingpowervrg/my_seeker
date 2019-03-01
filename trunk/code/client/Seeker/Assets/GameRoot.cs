/********************************************************************
	created:  2018-3-23 11:27:29
	filename: GameRoot.cs
	author:	  songguangze@outlook.com
	
	purpose:  客户端入口
*********************************************************************/
using EngineCore;
using SqliteDriver;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.EventSystems;
using Utf8Json.Formatters;

namespace SeekerGame
{
    public class GameRoot : MonoSingleton<GameRoot>
    {
        //客户端状态机
        private ClientFSM m_clientFsm = new ClientFSM();
        private bool m_isEngineReady = false;

        //默认音乐及音效的AudioMixer
        public AudioMixerGroup m_BGMAudioMixerGroup = null;
        public AudioMixerGroup m_soundAudioMixerGroup = null;

        private float gui_start_y = 0.0f;

        private void Awake()
        {
            //gui_start_y = Screen.height / 3.0f;

            PregameUILogic.instance.StartInitLoading();

            PreInitEngine();

            m_clientFsm.InitClientFSM();
            ModuleMgr.Instance.Init();
            CameraManager.Instance.Init();

            //限帧
            if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer)
                Application.targetFrameRate = 30;
            else
                Application.targetFrameRate = 60;
        }

        private bool needListenScreenClick = false;
        public bool NeedListenScreenClick
        {
            get { return needListenScreenClick; }
            set
            {
                needListenScreenClick = value;
                screenClickTime = 0f;
            }
        }
        private float screenClickTime = 0f;

        private void Update()
        {
            ModuleMgr.Instance.Update();

            if (m_isEngineReady)
            {
                m_clientFsm.Tick(Time.deltaTime);
            }
            if (needListenScreenClick)
            {
                screenClickTime += Time.deltaTime;

                if (Input.touchCount > 0 || Input.GetMouseButtonDown(0))
                {
                    screenClickTime = 0f;
                }
                if (screenClickTime >= 7f)
                {
                    GameEvents.UIEvents.UI_GameEntry_Event.OnMainCityHint.SafeInvoke();
                    NeedListenScreenClick = false;
                }
            }
#if UNITY_EDITOR && UNITY_DEBUG
            if (Input.GetKeyUp(KeyCode.Q) || Input.touchCount > 1)
            {
                needShowGUI = !needShowGUI;
            }
#endif
        }

        public bool skipGuid = false;

        private void LateUpdate()
        {
            if (m_isEngineReady)
            {
                ModuleMgr.Instance.LateUpdate();
            }
        }

        private void OnDestroy()
        {
            if (SQLiteHelper.Instance() != null)
                SQLiteHelper.Instance().CloseSql();

        }
        //#if UNITY_EDITOR && UNITY_DEBUG || UNITY_ANDROID
        public bool enable = true;

        string scene_txt = "seek scene id";
        string tween_time;
        string drag_td;

        private bool needShowGUI = true;

        string content = "";
        string expPath;
        string mainPath = null;
        string patchPath = null;
        bool need_download = false;
        string progresssTex = string.Empty;
        string buildName = string.Empty;

        private void OnGUI()
        {
            if (!needShowGUI)
            {
                return;
            }
            if (enable)
            {

                GUILayout.BeginArea(new Rect(10, gui_start_y, 300, Screen.height));

                scene_txt = GUILayout.TextField(scene_txt, GUILayout.Width(150), GUILayout.Height(40));

                if (GUILayout.Button("seek", GUILayout.Width(150), GUILayout.Height(40)))
                {
                    if (!string.IsNullOrEmpty(scene_txt))
                    {
                        long scene_id;
                        if (long.TryParse(scene_txt, out scene_id))
                        {
                            //GameIAPModule.Instance.OnTransactionDone.SafeInvoke(ConfCharge.Get(scene_id).desc);
                            CommonHelper.OpenEnterGameSceneUI(scene_id);
                        }
                    }
                }


                progresssTex = GUILayout.TextField(progresssTex, GUILayout.Width(150), GUILayout.Height(40));

                if (GUILayout.Button("progresssTex", GUILayout.Width(150), GUILayout.Height(40)))
                {
                    SeekerGame.NewGuid.GuidNewManager.Instance.TestProgressIndex(int.Parse(progresssTex));
                }
                if (GUILayout.Button("SkipCurrentCapter", GUILayout.Width(150), GUILayout.Height(40)))
                {
                    GameEvents.UIEvents.UI_StartCartoon_Event.OnSkipCurrentCapter.SafeInvoke();
                }
                if (GUILayout.Button("EnterCartoon", GUILayout.Width(150), GUILayout.Height(40)))
                {
                    FrameMgr.OpenUIParams param = new FrameMgr.OpenUIParams(UIDefine.UI_COMICS_1);
                    param.Param = ConfCartoonScene.Get(40008).sceneInfoIds[0];
                    EngineCoreEvents.UIEvent.ShowUIEventWithParam.SafeInvoke(param);
                }
                buildName = GUILayout.TextField(buildName, GUILayout.Width(150), GUILayout.Height(40));
                if (GUILayout.Button("UnLock", GUILayout.Width(150), GUILayout.Height(40)))
                {
                    GameEvents.BigWorld_Event.OnUnLock.SafeInvoke(buildName);
                }
                if (GUILayout.Button("EnterTalk", GUILayout.Width(150), GUILayout.Height(40)))
                {
                    SceneModule.Instance.EnterExhibitionHallScene();
                    //PresuadeUILogic.Show(1000);
                    //ReasoningUILogic.ShowReasonUIById(1);
                    //GameEvents.UIEvents.UI_GameEntry_Event.OnNewPoliceEffect.SafeInvoke(true);
                    //TalkUIHelper.OnStartTalk(int.Parse(buildName));
                    //GameEvents.UIEvents.UI_GameEntry_Event.OnLockMainIcon.SafeInvoke(int.Parse(buildName),true);
                }
                if (GUILayout.Button("ShowReasonUIById", GUILayout.Width(150), GUILayout.Height(40)))
                {
                    ///PresuadeUILogic.Show(1000);
                    ReasoningUILogic.ShowReasonUIById(1);
                    //GameEvents.UIEvents.UI_GameEntry_Event.OnNewPoliceEffect.SafeInvoke(true);
                    //TalkUIHelper.OnStartTalk(int.Parse(buildName));
                    //GameEvents.UIEvents.UI_GameEntry_Event.OnLockMainIcon.SafeInvoke(int.Parse(buildName),true);
                }
                if (GUILayout.Button("SkyeyeUILogic", GUILayout.Width(150), GUILayout.Height(40)))
                {
                    SkyeyeUILogic.Show();
                }
                GUILayout.EndArea();

            }
        }


        //#endif

        /// <summary>
        /// 引擎初始化完成
        /// </summary>
        private void OnEngineReady()
        {
            EngineCoreEvents.SystemEvents.OnEngineReady -= OnEngineReady;

            this.m_isEngineReady = true;

            PlayerPrefTool.SetBool(ENUM_PREF_KEY.E_FIRST_TIME_LAUNCH.ToString(), false);

            m_clientFsm.GotoState((int)ClientFSM.ClientState.INIT);

            //GuidManager.Instance.LoadGuid();
        }

        /// <summary>
        /// 预初始化引擎层
        /// </summary>
        private void PreInitEngine()
        {
            LogReporter.InitLogReporter();
            ReflectionHelper.InitAssembly();
            Utf8Json.Resolvers.CompositeResolver.RegisterAndSetAsDefault(new[] { PrimitiveObjectFormatter.Default }, new[] { Utf8Json.Resolvers.GeneratedResolver.Instance, Utf8Json.Resolvers.BuiltinResolver.Instance });

            EngineCoreEvents.SystemEvents.OnEngineReady += OnEngineReady;
            EngineCoreEvents.BridgeEvent.GetGameRootObject = () => { return gameObject; };
            EngineCoreEvents.BridgeEvent.GetGameRootBehaviour = () => { return this; };
            EngineCoreEvents.BridgeEvent.StartCoroutine = (coroutine) => StartCoroutine(coroutine);
            EngineCore.EngineDelegateCore.GameClientEntrySceneName = "GameClient";
            EngineDelegateCore.AudioRootGameObject = transform.Find("AudioObject").gameObject;

            EngineCore.EngineDelegateCore.DefaultBGMMixerGroup = this.m_BGMAudioMixerGroup;
            EngineCore.EngineDelegateCore.DefaultEffectMixerGroup = this.m_soundAudioMixerGroup;

            //开关，是否在编辑器下模拟对应的平台
            EngineCore.EngineDelegateCore.Editor_Simulate_Player = false;

#if UNITY_ANDROID || UNITY_IOS
            if (!string.IsNullOrEmpty(GameConst.MARKET_FLAG))
                EngineDelegateCore.DynamicResource = true;
#endif
            EngineCore.EngineDelegateCore.IsFirstTimeLaunch = PlayerPrefTool.HasKey(ENUM_PREF_KEY.E_FIRST_TIME_LAUNCH.ToString());
        }

        private void OnApplicationQuit()
        {
            //强制关闭Sql防止进程占用
            if (SQLiteHelper.Instance() != null)
                SQLiteHelper.Instance().CloseSql();

            ModuleMgr.Instance.Destroy();
        }

        private void OnApplicationPause(bool pause)
        {
            if (!pause && GlobalInfo.MY_PLAYER_ID > 0)
            {
                GameEvents.PlayerEvents.RequestLatestPlayerInfo.SafeInvoke();
            }
            EngineCoreEvents.SystemEvents.OnApplicationPause.SafeInvoke(pause);
        }

        public ClientFSM GameFSM
        {
            get { return this.m_clientFsm; }
        }
    }
}