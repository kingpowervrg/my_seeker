/********************************************************************
	created:  2018-4-26 13:29:11
	filename: GameSceneBase.cs
	author:	  songguangze@outlook.com
	
	purpose:  游戏场景基类
*********************************************************************/
using EngineCore;
using EngineCore.Utility;
using fastJSON;
using HedgehogTeam.EasyTouch;
using SeekerGame.NewGuid;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
namespace SeekerGame
{
    public abstract class GameSceneBase : SceneBase
    {
        public const int MAX_FINDING_ENTITY_NUM = 6;            //每次最多显示可寻找的数量

        //场景ID
        protected int m_sceneId = 0;
        protected ConfScene m_sceneData = null;

        protected SceneItemJson m_sceneInfo = null;
        protected bool m_isLightMapReady = false;
        protected bool m_isSceneReady = false;

        protected GameStatus m_gameStatus = GameStatus.NONE;
        protected Texture m_sceneExtraLightTex = null;

        public GameStatus CurGameStatus { get { return m_gameStatus; } }

        //游戏结束条件
        protected IGameOverCondition m_gameOverCondition = null;

        protected GameSceneTime m_gameTimer = null;
        private int m_gameTotalTime = GameConst.TOTAL_TIME;

        private UnityEngine.Texture m_hintTexture = null;
        private UnityEngine.Texture m_HintTexture
        {
            get
            {
                if (m_hintTexture == null)
                {
                    m_hintTexture = UnityEngine.Resources.Load(GameConst.STREAMER_EFFECT_TEXTURE) as Texture;
                }
                return m_hintTexture;
            }
        }

        public float GetElapseTime()
        {
            return m_gameTimer.ElaspeTime;
        }


        public int GetPunishTime()
        {
            return m_gameTimer.PunishTimes * GameConst.PUNISH_TIME;
        }

        public float GetRemainTime()
        {
            return m_gameTimer.RemainTime;
        }

        public RandomOutputCalculator m_randomOutputCalculator = null;

        private IList<long> m_heroList;

        //已找到的物件列表
        private List<SceneItemEntity> m_foundItemEntityList = new List<SceneItemEntity>();

        //当前正在找的物品列表
        private List<SceneItemEntity> m_currentFindingEntityList = new List<SceneItemEntity>();

        //等待寻找的物品列表
        private Queue<SceneItemEntity> m_waitingEntityQueue = new Queue<SceneItemEntity>();

        public GameSceneBase(SceneMode sceneMode, int sceneId) : base(sceneMode)
        {
            this.m_sceneId = sceneId;

            MessageHandler.RegisterMessageHandler(MessageDefine.SCSceneReconnectResponse, OnReconnectResponse);

            GameEvents.NetworkWatchEvents.NetPass += OnReconnectResponse;
        }

        public virtual void InitScene(object sceneData, IList<long> decoratorList, IList<long> taskObjectList, IList<long> officerIds)
        {
            this.m_sceneData = (ConfScene)sceneData;
            this.m_heroList = officerIds;
            LoadScene(this.m_sceneData.sceneInfo);
            LoadSceneObjects(decoratorList, taskObjectList);
#if OFFICER_SYS
            GameSkillManager.Instance.CreatePoliceSkill(officerIds);

#endif
            RegisterMainGameEvents();
        }

        public virtual void InitScene(object sceneData, IList<long> decoratorList, IList<long> taskObjectList, IList<long> officerIds, int gameTime)
        {
            this.m_gameTotalTime = gameTime;
            InitScene(sceneData, decoratorList, taskObjectList, officerIds);
        }

        public void InitRandomOutput(int totalCount, IList<GiftItem> dropItems)
        {
            this.m_randomOutputCalculator = new RandomOutputCalculator(totalCount, dropItems);
        }


        protected virtual void RegisterMainGameEvents()
        {
            GameEvents.MainGameEvents.GetFoundTaskItemEntityList = () => { return this.m_foundItemEntityList; };
            GameEvents.MainGameEvents.GetSceneTaskItemEntityList = () => { return EntityManager.Instance.GetEntityListByEntityType(EntityType.Scene_Object); };
            GameEvents.MainGameEvents.GetSceneItemEntityList = HintSceneItemEntities;
            GameEvents.MainGameEvents.GetSceneItemEntityByID = GetSceneItemEntityByID;
            GameEvents.MainGameEvents.AutoPickSceneItems = AutoPickSceneItem;
            GameEvents.MainGameEvents.OnStreamerHint = OnStreamerHint;
            GameEvents.MainGameEvents.GetCurrentFindingEntities = () =>
            {
                return this.m_currentFindingEntityList;
            };

            GameEvents.MainGameEvents.RequestHintSceneItemList = OnRequestHintSceneItemEntityList;
            GameEvents.MainGameEvents.RequestHintSceneItemNoCameraFollow = RequestHintSceneItemNoCameraFollow;
            GameEvents.MainGameEvents.RequestNextFindingTaskEntities = PushSceneItemList;
#if OFFICER_SYS
            GameEvents.MainGameEvents.RequestHeroList = RequestHeroList;
#endif
            GameEvents.MainGameEvents.IsSceneItemFound = IsEntityFound;
        }


        /// <summary>
        /// 请求载入场景
        /// </summary>
        /// <param name="sceneName"></param>
        public void LoadScene(string sceneName)
        {
            EngineCore.EngineCoreEvents.ResourceEvent.LoadAdditiveScene.SafeInvoke(sceneName, OnLoadedScene);
        }

        private IList<long> RequestHeroList()
        {
            return m_heroList;
        }

        /// <summary>
        /// 载入场景完成
        /// </summary>
        protected virtual void OnLoadedScene()
        {
            GameEvents.UIEvents.UI_Loading_Event.OnLoadingState.SafeInvoke(1, true);
            CameraManager.Instance.ResetMainCamera();
            GameEvents.SceneEvents.OnEnterScene.SafeInvoke();

            InputModule.Instance.Enable = true;
            EasyTouch.SetEnableAutoSelect(true);
            EasyTouch.instance.touchCameras[0].camera = Camera.main;
            EngineCoreEvents.InputEvent.OnOneFingerTouchup += OnTouchScreen;
            GameEvents.MainGameEvents.OnGameStatusChange += OnGameStatusChange;
            ActiveSceneEntities();
        }

        private void OnGameStatusChange(GameStatus status)
        {
            this.m_gameStatus = status;
        }

        /// <summary>
        /// 载入场景物件
        /// </summary>
        /// <param name="decoratorList"></param>
        /// <param name="taskObjectList"></param>
        protected virtual void LoadSceneObjects(IList<long> decoratorList, IList<long> taskObjectList)
        {
            EngineCoreEvents.ResourceEvent.GetAssetEvent.SafeInvoke(this.m_sceneData.exhibitGroupId, (assetName, assetObject) =>
            {
                m_sceneInfo = JSON.ToObject<SceneItemJson>(assetObject.ToString());

                EngineCoreEvents.ResourceEvent.ReleaseAssetEvent.SafeInvoke(m_sceneData.exhibitGroupId, assetObject);

                Dictionary<long, ItemInfoJson> sceneItemConfigDict = new Dictionary<long, ItemInfoJson>();

                //临时修改加载lightmap
                GOGUI.GOGUITools.GetAssetAction.SafeInvoke(m_sceneInfo.lightMapName, (prefabName, obj) =>
                {
                    this.m_isLightMapReady = true;
                    //Texture lightTex = null;

                    if (StringUtils.IsNullEmptyOrWhiteSpace(m_sceneInfo.lightMapName) || obj == null)
                        Debug.LogWarning($"can't find lightmap in  {assetName}");

                    if (obj != null)
                    {
                        this.m_sceneExtraLightTex = obj as Texture;
                    }

                    for (int i = 0; i < m_sceneInfo.items.Count; ++i)
                    {
                        ItemInfoJson itemInfo = m_sceneInfo.items[i];

                        if (!sceneItemConfigDict.ContainsKey(itemInfo.itemID))
                            sceneItemConfigDict.Add(itemInfo.itemID, itemInfo);
                        else
                            Debug.LogError("item id duplicate ,item id:" + itemInfo.itemID + ", config info :" + assetName);
                    }


                    //todo:之后重构 2018-7-26 20:25:39
                    for (int i = 0; i < decoratorList.Count; ++i)
                    {
                        long itemID = decoratorList[i];
                        if (!sceneItemConfigDict.ContainsKey(itemID))
                            Debug.LogError("no item :" + itemID + " in " + assetName);

                        ItemInfoJson itemInfo = sceneItemConfigDict[itemID];

                        EntityBase sceneItemEntity = EntityManager.Instance.AllocEntity(EntityType.Scene_Decoration);
                        Confexhibit confItem = Confexhibit.Get(itemInfo.itemID);
                        if (confItem == null || string.IsNullOrEmpty(confItem.assetName))
                        {
                            Debug.Log("can't find item:" + itemInfo.itemID);
                            continue;
                        }
                        sceneItemEntity.SetAssetName(confItem.assetName);
                        sceneItemEntity.SetEntityData(itemInfo);
                        sceneItemEntity.SetLightTexture(m_sceneExtraLightTex);
                        sceneItemEntity.PreloadAsset();

                        AddSceneEntity(sceneItemEntity);
                    }

                    for (int i = 0; i < taskObjectList.Count; ++i)
                    {
                        long itemID = taskObjectList[i];
                        if (!sceneItemConfigDict.ContainsKey(itemID))
                            Debug.LogError("no item :" + itemID + " in " + assetName);

                        ItemInfoJson itemInfo = sceneItemConfigDict[itemID];

                        EntityBase sceneItemEntity = EntityManager.Instance.AllocEntity(EntityType.Scene_Object);
                        Confexhibit confItem = Confexhibit.Get(itemInfo.itemID);
                        if (confItem == null || string.IsNullOrEmpty(confItem.assetName))
                        {
                            Debug.Log("can't find item:" + itemInfo.itemID);
                            continue;
                        }
                        sceneItemEntity.SetAssetName(confItem.assetName);
                        sceneItemEntity.SetEntityData(itemInfo);
                        sceneItemEntity.SetLightTexture(m_sceneExtraLightTex);
                        sceneItemEntity.PreloadAsset();

                        AddSceneEntity(sceneItemEntity);
                    }

                    InitFindingEntityItems();

                }, LoadPriority.Default);
                ///////


            }, LoadPriority.Prior);
        }

        /// <summary>
        /// 激活场景实体
        /// </summary>
        protected virtual void ActiveSceneEntities()
        {
            if (this.m_sceneInfo == null || !this.m_isLightMapReady)
            {
                TimeModule.Instance.SetTimeout(ActiveSceneEntities, Time.deltaTime);
                return;
            }

            bool allDone = true;
            foreach (EntityBase entity in AllSceneEntities)
            {
                if (entity.AssetLoadStatus == ResStatus.NONE)
                {
                    entity.Load();
                    allDone = false;
                }
                else if (entity.AssetLoadStatus == ResStatus.WAIT)
                    allDone = false;
            }

            if (!allDone)
            {
                TimeModule.Instance.SetTimeout(ActiveSceneEntities, Time.deltaTime);
                return;
            }
            //List<EntityBase> allEntityInScene = EntityManager.Instance.GetAllEntityList();


            ////等所有实体预加载完成
            //foreach (EntityBase entity in AllSceneEntities)
            //{
            //    if (entity.AssetLoadStatus == ResStatus.WAIT)
            //    {
            //        TimeModule.Instance.SetTimeout(ActiveSceneEntities, Time.deltaTime);
            //        return;
            //    }
            //}




            this.m_isSceneReady = true;
            GameEvents.UIEvents.UI_Loading_Event.OnLoadingState.SafeInvoke(2, true);
        }

        public virtual void StartGame()
        {
            if (!this.m_isSceneReady)
            {
                TimeModule.Instance.SetTimeout(StartGame, Time.deltaTime);
            }
            else
            {
                GameSkillManager.Instance.Start();

                this.m_gameTimer = new GameSceneTime(this.m_gameTotalTime);

                InitGameOverCondition();

                this.m_gameStatus = GameStatus.GAMING;

                GameEvents.MainGameEvents.OnGameStatusChange.SafeInvoke(GameStatus.GAMING);
                GameEvents.MainGameEvents.OnStartGame.SafeInvoke();
                GameEvents.MainGameEvents.OnErrorTouch += OnErrorTouch;

                if (GlobalInfo.GAME_NETMODE == GameNetworkMode.Network)
                {
                    CSSceneStartRequest startNotice = new CSSceneStartRequest();
                    startNotice.PlayerId = GlobalInfo.MY_PLAYER_ID;
                    GameEvents.NetWorkEvents.SendAsyncMsg.SafeInvoke(startNotice);
                }

            }
        }

        private void InitGameOverCondition()
        {
            switch (GameOverLimit)
            {
                case SeekerGame.GameOverCondition.STEPS_LIMIT:
                    this.m_gameOverCondition = new GameOverBySteps(m_sceneData.clickCount);
                    break;
                case SeekerGame.GameOverCondition.TIME_LIMIT:
                    this.m_gameOverCondition = new GameOverByTime(1000);
                    break;
            }
        }


        private void OnErrorTouch(float errorTouchTime, Vector2 errorTouchPos)
        {
            TotalErrorTimes++;
        }


        /// <summary>
        /// 初始化待寻找物体
        /// </summary>
        private void InitFindingEntityItems()
        {
            List<EntityBase> allFindingItems = EntityManager.Instance.GetEntityListByEntityType(EntityType.Scene_Object);
            for (int i = 0; i < allFindingItems.Count; ++i)
                this.m_waitingEntityQueue.Enqueue(allFindingItems[i] as SceneItemEntity);
        }

        /// <summary>
        /// 推送下一个寻找的物件
        /// </summary>
        private void PushNextEntityItem()
        {
            if (this.m_waitingEntityQueue.Count > 0)
            {
                SceneItemEntity findingItemEntity = this.m_waitingEntityQueue.Dequeue();
                this.m_currentFindingEntityList.Add(findingItemEntity);

                GameEvents.MainGameEvents.PushFindingTaskEntities.SafeInvoke(new List<SceneItemEntity>() { findingItemEntity });
            }
        }

        private List<SceneItemEntity> PushSceneItemList(int num)
        {
            int findingItemNum = Mathf.Min(num, this.m_waitingEntityQueue.Count);
            List<SceneItemEntity> entityList = new List<SceneItemEntity>();
            while (findingItemNum > 0)
            {
                SceneItemEntity findingItemEntity = this.m_waitingEntityQueue.Dequeue();
                this.m_currentFindingEntityList.Add(findingItemEntity);
                entityList.Add(findingItemEntity);
                findingItemEntity.EntityObject.IsEnable = true;

                findingItemNum--;
            }

            return entityList;

        }


        /// <summary>
        /// 物体识别判断
        /// </summary>
        /// <param name="gesture"></param>
        protected virtual void OnTouchScreen(Gesture gesture)
        {
            if (this.m_gameStatus == GameStatus.GAMING || NewGuid.GuidNewNodeManager.Instance.GetNodeStatus(NewGuid.GuidNewNodeManager.SceneTips) == NewGuid.NodeStatus.None)
            {
                GameEvents.MainGameEvents.OnSceneClick.SafeInvoke(gesture.position);
                TotalTouchCount++;
                if (gesture.pickedObject != null && gesture.pickedObject.layer == LayerDefine.SceneTargetObjectLayer)
                {
                    if (!gesture.pickedObject.CompareTag("cameraPoint"))
                        EngineCoreEvents.AudioEvents.PlayAudio.SafeInvoke(Audio.AudioType.UISound, GameCustomAudioKey.game_exhibit.ToString());
                    OnPickedSceneObject(gesture.pickedObject);
                }
                else
                {
                    EngineCoreEvents.AudioEvents.PlayAudio.SafeInvoke(Audio.AudioType.UISound, GameCustomAudioKey.game_error.ToString());
                    GameEvents.MainGameEvents.OnErrorTouch.SafeInvoke(TimeModule.GameRealTime.RealTime, gesture.position);
                }
            }
        }

        /// <summary>
        /// 识别到物体
        /// </summary>
        /// <param name="gameObject"></param>
        private void OnPickedSceneObject(GameObject gameObject)
        {
            string entityId = gameObject.transform.root.name;
            SceneItemEntity sceneItemEntity = EntityManager.Instance.GetEntityByEntityId<SceneItemEntity>(entityId);

            OnPickedSceneItemEntity(sceneItemEntity, false);
        }

        /// <summary>
        /// 找到物体
        /// </summary>
        /// <param name="pickedSceneItemEntity"></param>
        private void OnPickedSceneItemEntity(SceneItemEntity pickedSceneItemEntity, bool isMissile)
        {
            if (!this.m_currentFindingEntityList.Contains(pickedSceneItemEntity))
                return;

            if (!string.IsNullOrEmpty(pickedSceneItemEntity.CameraName))
            {
                if (!GameEvents.SceneEvents.EntityInCurrentCamera(pickedSceneItemEntity.CameraName) && !isMissile)
                    return;
            }


            this.m_foundItemEntityList.Add(pickedSceneItemEntity);
            this.m_currentFindingEntityList.Remove(pickedSceneItemEntity);

            //todo:设置物体被找到的效果
            //pickedSceneItemEntity.EntityObject.Visible = false;

            GameEvents.MainGameEvents.OnPickedSceneObject.SafeInvoke(pickedSceneItemEntity);
            GameEvents.MainGameEvents.OnResetFingerTips.SafeInvoke();

            //全部找到几秒后退出
            //if (CheckAllEntityFound())
            //{
            //    this.m_gameStatus = GameStatus.GAMEOVER;
            //    GameEvents.MainGameEvents.OnGameStatusChange.SafeInvoke(GameStatus.GAMEOVER);

            //    //TimeModule.Instance.SetTimeout(() =>
            //    //{
            //    GameEvents.MainGameEvents.OnGameOver.SafeInvoke(GameResult.ALL_ITEM_FOUND);
            //    //}, 3f);
            //}
        }

        private bool CheckGameOver()
        {
            if (m_gameOverCondition.GameResult != GameResult.NONE)
            {
                this.m_gameStatus = GameStatus.GAMEOVER;

                GameEvents.MainGameEvents.OnGameOver.SafeInvoke(m_gameOverCondition.GameResult);

                return true;
            }

            if (CheckAllEntityFound())
            {
                this.m_gameStatus = GameStatus.GAMEOVER;

                GameEvents.MainGameEvents.OnGameOver.SafeInvoke(GameResult.ALL_ITEM_FOUND);

                return true;
            }

            return false;
        }


        /// <summary>
        /// 自动识取物品
        /// </summary>
        /// <param name="num"></param>
        public void AutoPickSceneItem(int num)
        {
            GameEvents.MainGameEvents.OnCheckSceneCameraPoint.SafeInvoke(() =>
            {
                List<SceneItemEntity> notFoundEntityList = HintSceneItemEntities(num);
                for (int i = 0; i < notFoundEntityList.Count; ++i)
                {
                    OnPickedSceneItemEntity(notFoundEntityList, i);
                }
            });
        }

        private void OnPickedSceneItemEntity(List<SceneItemEntity> notFoundEntityList, int i)
        {
            if (i == 0)
            {
                GameEvents.MainGameEvents.OnGetMissileStartPos.SafeInvoke(notFoundEntityList[i].EntityPosition, () =>
                {
                    OnPickedSceneItemEntity(notFoundEntityList[i], true);
                });
            }
            else
            {
                TimeModule.Instance.SetTimeout(() =>
                {
                    GameEvents.MainGameEvents.OnGetMissileStartPos.SafeInvoke(notFoundEntityList[i].EntityPosition, () =>
                    {
                        OnPickedSceneItemEntity(notFoundEntityList[i], true);
                    });
                }, i * 0.5f);
            }

        }

        /// <summary>
        /// 请求提示物件
        /// </summary>
        /// <param name="hintNum"></param>
        private void OnRequestHintSceneItemEntityList(int hintNum)
        {
            if (GuidNewNodeManager.Instance.GetNodeStatus(GuidNewNodeManager.FindItemByID) == NodeStatus.None)
            {
                //新手引导特殊处理
                string findID = GuidNewNodeManager.Instance.GetCommonParams(GuidNewNodeManager.NeedFindID);
                SceneItemEntity entity = GetSceneItemEntityByID(long.Parse(findID));
                GameEvents.MainGameEvents.NotifyHintSceneItemEntityList.SafeInvoke(new List<SceneItemEntity>() { entity });
            }
            else
            {
                List<SceneItemEntity> hintEntityList = HintSceneItemEntities(hintNum);
                GameEvents.MainGameEvents.NotifyHintSceneItemEntityList.SafeInvoke(hintEntityList);
            }

        }

        /// <summary>
        /// 请求提示物件
        /// </summary>
        /// <param name="hintNum"></param>
        private void RequestHintSceneItemNoCameraFollow(int hintNum)
        {
            List<SceneItemEntity> hintEntityList = HintSceneItemEntities(hintNum);
            GameEvents.MainGameEvents.NotifyHintSceneItemEntityListNoCameraFollow.SafeInvoke(hintEntityList);
        }

        /// <summary>
        /// 提示场景物体
        /// </summary>
        /// <param name="hintNum"></param>
        /// <returns></returns>
        public List<SceneItemEntity> HintSceneItemEntities(int hintNum)
        {
            List<SceneItemEntity> notFoundEntityList = GetNotFoundSceneItemEntities();
            if (notFoundEntityList.Count < hintNum)
                hintNum = notFoundEntityList.Count;

            if (hintNum >= GameConst.MAX_HINT_ITEM_COUNT)
                hintNum = GameConst.MAX_HINT_ITEM_COUNT;

            List<SceneItemEntity> hintEntityList = new List<SceneItemEntity>();
            int[] randomIndex = CommonUtils.GetRandomList(hintNum);
            for (int i = 0; i < randomIndex.Length; ++i)
                hintEntityList.Add(notFoundEntityList[randomIndex[i]]);

            return hintEntityList;
        }

        private SceneItemEntity GetSceneItemEntityByID(long itemID)
        {
            List<SceneItemEntity> notFoundEntityList = GetNotFoundSceneItemEntities();
            for (int i = 0; i < notFoundEntityList.Count; i++)
            {
                if (notFoundEntityList[i].EntityData.itemID == itemID)
                {
                    return notFoundEntityList[i];
                }
            }
            return null;
        }


        /// <summary>
        /// 获取未找到的物体
        /// </summary>
        /// <returns></returns>
        private List<SceneItemEntity> GetNotFoundSceneItemEntities()
        {
            List<EntityBase> sceneAllTaskItemEntities = GameEvents.MainGameEvents.GetSceneTaskItemEntityList.SafeInvoke();

            List<SceneItemEntity> notFoundSceneItemList = new List<SceneItemEntity>();
            for (int i = 0; i < sceneAllTaskItemEntities.Count; ++i)
            {
                SceneItemEntity taskSceneItem = sceneAllTaskItemEntities[i] as SceneItemEntity;
                if (!this.m_foundItemEntityList.Contains(taskSceneItem))
                    notFoundSceneItemList.Add(taskSceneItem);
            }

            return notFoundSceneItemList;
        }


        /// <summary>
        /// 检查是否所有物品被找到
        /// </summary>
        /// <returns></returns>
        private bool CheckAllEntityFound()
        {
            int totalItemCount = EntityManager.Instance.GetEntityListByEntityType(EntityType.Scene_Object).Count;
            int foundItemCount = m_foundItemEntityList.Count;

            return foundItemCount == totalItemCount;
        }

        private bool IsEntityFound(string entityID)
        {
            return m_foundItemEntityList.Find(foundEntity => foundEntity.EntityId == entityID) != null;
        }

        public override void Update()
        {
            if (this.m_gameTimer != null)
                this.m_gameTimer.Tick(TimeModule.GameRealTime.DeltaTime);

            if (CurGameStatus == GameStatus.GAMING)
            {
                if (CheckGameOver())
                    GameEvents.MainGameEvents.OnGameStatusChange.SafeInvoke(GameStatus.GAMEOVER);
            }
        }

        public virtual void ExitGameScene()
        {
            StopAndRemoveSceneBGM(false);
            GameEvents.MainGameEvents.OnErrorTouch -= OnErrorTouch;
            this.m_gameTimer.Dispose();

            GameSkillManager.Instance.OnDestory();
            InputModule.Instance.Enable = false;
            EngineCoreEvents.UIEvent.HideUIEvent.SafeInvoke(UIDefine.UI_GAME_MAIN_SETTING);
            EngineCoreEvents.UIEvent.HideUIEvent.SafeInvoke(UIDefine.UI_GAME_MAIN);
        }

        public override void DestroyScene()
        {
            ExitGameScene();
            base.DestroyScene();
            if (this.m_sceneExtraLightTex != null)
                EngineCoreEvents.ResourceEvent.ReleaseAssetEvent.SafeInvoke(this.m_sceneInfo.lightMapName, this.m_sceneExtraLightTex);
            if (this.m_hintTexture != null)
            {
                UnityEngine.Resources.UnloadAsset(this.m_hintTexture);
            }
            m_cacheEntityShader.Clear();
            EngineCoreEvents.InputEvent.OnOneFingerTouchup -= OnTouchScreen;
            GameEvents.MainGameEvents.OnGameStatusChange -= OnGameStatusChange;
            MessageHandler.UnRegisterMessageHandler(MessageDefine.SCSceneReconnectResponse, OnReconnectResponse);
        }

        private void OnReconnectResponse()
        {
            //全部找到几秒后退出
            if (CheckAllEntityFound())
            {
                this.m_gameStatus = GameStatus.GAMEOVER;
                GameEvents.MainGameEvents.OnGameStatusChange.SafeInvoke(GameStatus.GAMEOVER);

                TimeModule.Instance.SetTimeout(() =>
                {
                    EngineCoreEvents.UIEvent.HideUIEvent.SafeInvoke(UIDefine.UI_GAME_MAIN);
                    this.StopAndRemoveSceneBGM(false);

                    SCSceneRewardResponse rsp = new SCSceneRewardResponse();
                    rsp.SceneId = -1;
                    WinFailData data = new WinFailData(ENUM_SEARCH_MODE.E_SEARCH_ROOM, rsp);
                    FrameMgr.OpenUIParams param = new FrameMgr.OpenUIParams(UIDefine.UI_WIN);
                    param.Param = data;
                    EngineCoreEvents.UIEvent.ShowUIEventWithParam.SafeInvoke(param);

                    //PushGiftManager.Instance.TurnOn(ENUM_PUSH_GIFT_BLOCK_TYPE.E_SEEK);
                }, 3f);

                Dictionary<UBSParamKeyName, object> _param = new Dictionary<UBSParamKeyName, object>();
                _param.Add(UBSParamKeyName.Success, 0);
                _param.Add(UBSParamKeyName.SceneID, this.m_sceneId);
                UserBehaviorStatisticsModules.Instance.LogEvent(UBSEventKeyName.game_finish, null, _param);
            }
        }

        private void OnReconnectResponse(object messageResponse)
        {
            SCSceneReconnectResponse msg = messageResponse as SCSceneReconnectResponse;
            if (!MsgStatusCodeUtil.OnError(msg.ResponseStatus))
            {
                OnReconnectResponse();
            }
        }

        private Dictionary<long, Shader> m_cacheEntityShader = new Dictionary<long, Shader>();
        private void OnStreamerHint(bool isHint, long exhibitID)
        {
            SceneItemEntity entity = GameEvents.MainGameEvents.GetSceneItemEntityByID(exhibitID);
            if (entity == null || entity.EntityObject == null)
            {
                return;
            }
            if (isHint)
            {
                if (m_cacheEntityShader.ContainsKey(exhibitID))
                {
                    return;
                }
                Shader entityShader = entity.EntityObject.EntityMat.shader;
                m_cacheEntityShader.Add(exhibitID, entityShader);
                Shader hintShader = ShaderModule.Instance.GetShader(GameConst.STREAMER_SHADER);
                entity.EntityObject.EntityMat.shader = hintShader;
                int m_shaderCircleID = Shader.PropertyToID("_StreamerTex");
                entity.EntityObject.EntityMat.SetTexture(m_shaderCircleID, m_HintTexture);
            }
            else
            {
                if (m_cacheEntityShader.ContainsKey(exhibitID))
                {
                    entity.EntityObject.EntityMat.shader = m_cacheEntityShader[exhibitID];
                    m_cacheEntityShader.Remove(exhibitID);
                }
            }
        }

        /// <summary>
        /// 寻物物品模式
        /// </summary>
        public SceneItemFindingMode ItemFindingMode
        {
            get
            {
                return (SceneItemFindingMode)((m_sceneData.playMode / 100) % 10);
            }
        }

        /// <summary>
        /// 游戏结束条件
        /// </summary>
        public GameOverCondition GameOverLimit
        {
            get { return (GameOverCondition)((m_sceneData.playMode / 100000) % 10); }
        }


        #region Properties

        /// <summary>
        /// 错误次数
        /// </summary>
        public int TotalErrorTimes { get; private set; }

        /// <summary>
        /// 总点击次数
        /// </summary>
        public int TotalTouchCount { get; private set; }

        /// <summary>
        /// 游戏时间
        /// </summary>
        public GameSceneTime GameTimer => this.m_gameTimer;

        /// <summary>
        /// 游戏结束条件
        /// </summary>
        public IGameOverCondition GameOverCondition => this.m_gameOverCondition;
        #endregion

    }

    /// <summary>
    /// 随机产出
    /// </summary>
    public class RandomOutputCalculator
    {
        public int[] m_outputPerTime = null;
        private List<long> m_allDropIDList = null;

        public RandomOutputCalculator(int sceneItemNum, IList<GiftItem> itemList)
        {
            m_allDropIDList = new List<long>();
            int totalItemCount = 0;
            for (int i = 0; i < itemList.Count; ++i)
            {
                totalItemCount += itemList[i].Num;
                for (int j = 0; j < itemList[i].Num; ++j)
                    m_allDropIDList.Add(itemList[i].ItemId);
            }

            m_allDropIDList.Shuffle();

            int minNumPerTime = totalItemCount / sceneItemNum;
            int remainNum = totalItemCount % sceneItemNum;

            int[] randomArr = CommonUtils.GetRandomList(sceneItemNum);
            int[] moreThanAverageArr = new int[remainNum];
            Array.Copy(randomArr, moreThanAverageArr, remainNum);


            m_outputPerTime = new int[sceneItemNum];
            for (int i = 0; i < m_outputPerTime.Length; ++i)
            {
                if (moreThanAverageArr.Contains(i))
                    m_outputPerTime[i] = minNumPerTime + 1;
                else
                    m_outputPerTime[i] = minNumPerTime;
            }
        }


        public long[] GetRandomOutputItemIDByPickTime(int pickTime)
        {
            int dropNum = this.m_outputPerTime[pickTime];
            long[] dropId = new long[dropNum];
            for (int i = 0; i < dropNum; ++i)
            {
                dropId[i] = this.m_allDropIDList[i];
                this.m_allDropIDList.RemoveAt(i);
            }

            return dropId;

        }



    }
}