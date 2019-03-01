/********************************************************************
	created:  2018-4-8 20:14:9
	filename: GameMainUILogic.cs
	author:	  songguangze@outlook.com
	
	purpose:  主游戏UI
*********************************************************************/
using DG.Tweening;
using EngineCore;
using EngineCore.Utility;

using SeekerGame.NewGuid;
using SeekerGame.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SeekerGame
{
    [UILogicHandler(UIDefine.UI_GAME_MAIN)]
    public class GameMainUILogic : UILogicBase
    {
        private GameLabel m_lbTimeRemain = null;
        private GameButton m_btnPause = null;
        private GameProgressBar m_gameProgress = null;
        private GameUIEffect m_gameProgress_effect = null;
        private bool m_is_show_progress_effect = false;
        private FindingMode m_findingMode = FindingMode.NORMAL;
        private int m_sceneId = 0;
        private long taskConfID = -1;

        private SceneMode m_gameMode = 0;         //普通  迷雾  黑暗

        private GameObject m_panelVedio = null;

        //道具
#if OFFICER_SYS
        private HeroItem[] m_heroImg;
#endif
        private List<FindingItem> m_normalFindingModeItemList = new List<FindingItem>();

        private List<FindingItem> m_uiFindingItemList = new List<FindingItem>();

        private GameUIComponent m_normalFindingItemComponent = null;
        private GameImage m_normal_bg;

        private GameUIComponent m_multipleTimeFindingItemComponent = null;
        private GameImage m_time_bg;

        private GameUIContainer m_flyEffectContainer = null;
        private GameUIContainer m_hintEffectContainer = null;
        private GameUIContainer m_randomDropEffectContainer = null;
        private GameUIContainer m_oneItemMultiplyTimesContainer = null;
        private GameUIContainer m_useItemContainer = null;

        private GameUIEffect m_pickMarkEffect = null;
        private GameUIEffect m_noEnoughTimeEffect = null;
        private GameUIEffect m_errorPunishEffect = null;

        private GameRecycleContainer m_errorTouchEffectContainer = null;

        private int m_sceneAllTaskEntityCount = 0;

        private Dictionary<SceneItemEntity, HintEffectComponent> m_hintingEntityDict = new Dictionary<SceneItemEntity, HintEffectComponent>();
        private int m_gameRemainSeconds = 0;

        private ScanerEffectComponent m_scanerEffectComponent = null;
        private bool m_isAllowControlCamera = true;

        //请求购买道具
        public Action<long> OnRequestBuyPropInfoEvent;
        //局内商店组件
        private GameInternalShopComponent m_gameInternalShopComponent = null;

        //进度刷新
        public Action<int, int> OnRefreshFindingProgress;

        private Audio m_timeover_audio;

        private long m_autoUseItemAfterBuy = -1;

        private const float C_REFRESH_PROGRESS_TIME = 0.025f;
        private bool m_is_refresh_progress = false;

        GameMutilCameraSystem m_mutilCameraSys = null;
        private GameSceneCameraSystem m_sceneCameraSystem = null;

        private GameButton m_backBtn = null;
        private GameObject[] sceneCamera = null;

        private GameUIComponent m_root = null;

        private int m_buy_time_prop_count;
        public int Buy_time_prop_count
        {
            get { return m_buy_time_prop_count; }
            set { m_buy_time_prop_count = value; }
        }
        private GameUIEffect m_sceneTipsEffect = null;
        private GameImage m_Mask = null;

        private GameUIEffect m_exhibitTipsEffect = null; //物件提示

        private GameScenePropUseUILogic m_propUseUILogic = null;
        private GameMissileComponent m_missileComponent = null;
        private GameSkillEffectSystem m_skillEffectSystem = null;
        private GameMainArrowTipData arrowTipData = null;
        private const float C_NORMAL_MODE_BG_HEIGHT = 60.0f;
        private const float C_SHADOW_MODE_BG_HEIGHT = 90.0f;

        private int m_sceneType = 0; // 0表示正常  1表示小节点  2表示多摄像机

        private ScenePropHintComponent m_propHintCom0, m_propHintCom1, m_propHintCom2, m_propHintCom3;
        protected override void OnInit()
        {
            this.m_root = Make<GameUIComponent>("Panel_animation:root");

            this.m_normalFindingItemComponent = this.m_root.Make<GameUIComponent>("Panel_2");
            this.m_normal_bg = this.m_normalFindingItemComponent.Make<GameImage>("Image_back");
            this.m_multipleTimeFindingItemComponent = this.m_root.Make<GameUIComponent>("Panel_3");
            this.m_time_bg = this.m_multipleTimeFindingItemComponent.Make<GameImage>("Image_back");
            m_gameProgress = this.m_normal_bg.Make<GameProgressBar>("" +
                "" +
                "Slider");
            m_gameProgress_effect = m_gameProgress.Make<GameUIEffect>("UI_shouji_zengjia");
            m_gameProgress_effect.EffectPrefabName = "UI_shouji_zengjia.prefab";
            m_btnPause = this.m_root.Make<GameButton>("Button_pause");
            m_lbTimeRemain = this.m_root.Make<GameLabel>("Panel_1:Text_time");
            for (int i = 1; i <= 6; ++i)
            {
                FindingItem findingItem = this.m_root.Make<FindingItem>("Panel_2:Image_" + i);
                findingItem.Visible = false;
                this.m_normalFindingModeItemList.Add(findingItem);
            }

            m_oneItemMultiplyTimesContainer = this.m_root.Make<GameUIContainer>("Panel_3:Container");

            this.m_useItemContainer = Make<GameUIContainer>("Panel_Item:Scroll View:Viewport:Content");
#if OFFICER_SYS
            m_heroImg = new HeroItem[4];
            for (int i = 0; i < 4; i++)
            {
                m_heroImg[i] = this.m_root.Make<HeroItem>("Panel_1:Image_bufficon_" + (i + 1));
            }
#endif

            this.m_flyEffectContainer = this.m_root.Make<GameUIContainer>("PickedEffect");
            this.m_flyEffectContainer.EnsureSize<PickedFlyEffectComponent>(10);

            this.m_randomDropEffectContainer = this.m_root.Make<GameUIContainer>("PickedRandomDropEffect");
            this.m_randomDropEffectContainer.EnsureSize<RandomDropComponent>(10);

            this.m_hintEffectContainer = this.m_root.Make<GameUIContainer>("HintEffect");
            this.m_hintEffectContainer.EnsureSize<HintEffectComponent>(6);

            this.m_pickMarkEffect = this.m_root.Make<GameUIEffect>("PickedMarkEffect");
            this.m_noEnoughTimeEffect = this.m_root.Make<GameUIEffect>("UI_hongping");

            this.m_errorTouchEffectContainer = this.m_root.Make<GameRecycleContainer>("PickError_Effect");

            this.m_errorPunishEffect = this.m_root.Make<GameUIEffect>("UI_pickerrorpunishEffect");
            this.NeedUpdateByFrame = true;

            this.m_scanerEffectComponent = this.m_root.Make<ScanerEffectComponent>("UI_tancebo");
            this.m_gameInternalShopComponent = this.m_root.Make<GameInternalShopComponent>("Panel_Buy");

            this.m_backBtn = this.m_root.Make<GameButton>("btnback");
            this.m_mutilCameraSys = new GameMutilCameraSystem(this.m_backBtn);
            this.m_sceneTipsEffect = Make<GameUIEffect>("sceneTips");
            this.m_Mask = Make<GameImage>("mask");

            this.m_exhibitTipsEffect = Make<GameUIEffect>("UI_xinshouyindao_shou");
            this.m_propUseUILogic = this.m_root.Make<GameScenePropUseUILogic>("Panel_continue");
            this.m_propUseUILogic.Visible = false;
            this.m_missileComponent = this.m_root.Make<GameMissileComponent>("MissileEffect");
            this.m_skillEffectSystem = new GameSkillEffectSystem(this.m_root.Make<GameUIContainer>("SkillEffect"));
            this.m_panelVedio = Transform.Find("Panel_animation/Panel_vedio").gameObject;
            this.m_propHintCom0 = Make<ScenePropHintComponent>("Image_tips_1");
            this.m_propHintCom1 = Make<ScenePropHintComponent>("Image_tips_2");
            this.m_propHintCom2 = Make<ScenePropHintComponent>("Image_tips_3");
            this.m_propHintCom3 = Make<ScenePropHintComponent>("Image_tips_4");
        }

        public override void OnShow(object param)
        {
            if (param != null)
            {
                if (param is MainSceneData)
                {
                    MainSceneData sceneData = param as MainSceneData;
                    this.m_sceneId = (int)sceneData.sceneID;
                    this.taskConfID = sceneData.taskConfID;
                    GameMainHelper.Instance.currentTaskID = this.taskConfID;
                }
                else
                {
                    this.m_sceneId = int.Parse(param.ToString());
                    GameMainHelper.Instance.currentTaskID = -1;
                }
                GameMainHelper.Instance.CheckForcePropTips(this.taskConfID);
                this.m_panelVedio.SetActive(this.m_sceneId / 1000 == 10304);
                LoadingManager.Instance.SCENE_ID = m_sceneId;

                int sceneMode = ConfScene.Get(this.m_sceneId).playMode;
                this.m_gameMode = (SceneMode)((sceneMode / 10000) % 10);
                GameMainHelper.Instance.m_currentScenMode = this.m_gameMode;
                this.m_findingMode = (FindingMode)((sceneMode / 1000) % 10);

                this.m_normal_bg.Widget.sizeDelta = new Vector2(this.m_normal_bg.Widget.sizeDelta.x, C_NORMAL_MODE_BG_HEIGHT);
                this.m_time_bg.Widget.sizeDelta = new Vector2(this.m_time_bg.Widget.sizeDelta.x, C_NORMAL_MODE_BG_HEIGHT);

                if (FindingMode.SHADOW == this.m_findingMode)
                {
                    this.m_normal_bg.Widget.sizeDelta = new Vector2(this.m_normal_bg.Widget.sizeDelta.x, C_SHADOW_MODE_BG_HEIGHT);
                    this.m_time_bg.Widget.sizeDelta = new Vector2(this.m_time_bg.Widget.sizeDelta.x, C_SHADOW_MODE_BG_HEIGHT);
                }
                arrowTipData = GameMainHelper.Instance.isNeedArrowTips(this.taskConfID);
                //反词模式
                if (this.m_findingMode == FindingMode.REVERSE_NAME)
                    GameEvents.MainGameEvents.RevertItemNameToNormal += RevertItemNameToNormalHandler;

                int itemCount = 6;
                if (GameEvents.MainGameEvents.GetSceneTaskItemEntityList != null)
                {
                    List<EntityBase> entityList = GameEvents.MainGameEvents.GetSceneTaskItemEntityList();
                    if (entityList != null)
                    {
                        itemCount = entityList.Count;
                    }
                }
                m_oneItemMultiplyTimesContainer.EnsureSize<FindingItem>(itemCount);

                this.m_multipleTimeFindingItemComponent.Visible = false;
                this.m_normalFindingItemComponent.Visible = false;

                if (CurrentGameScene.ItemFindingMode == SceneItemFindingMode.OneItemMultipleTime)
                {
                    this.m_multipleTimeFindingItemComponent.Visible = true;

                    this.m_uiFindingItemList.Clear();
                    for (int i = 0; i < this.m_oneItemMultiplyTimesContainer.ChildCount; ++i)
                    {
                        FindingItem findingItem = this.m_oneItemMultiplyTimesContainer.GetChild<FindingItem>(i);
                        this.m_uiFindingItemList.Add(findingItem);
                    }
                }
                else
                {
                    this.m_normalFindingItemComponent.Visible = true;
                    this.m_uiFindingItemList.Clear();
                    this.m_uiFindingItemList.AddRange(this.m_normalFindingModeItemList);
                }

                for (int i = 0; i < m_normalFindingModeItemList.Count; ++i)
                {
                    FindingItem findingItem = m_normalFindingModeItemList[i];
                    findingItem.InitUIEffect(this.m_findingMode);
                }

                for (int i = 0; i < m_oneItemMultiplyTimesContainer.ChildCount; ++i)
                {
                    FindingItem item = m_oneItemMultiplyTimesContainer.GetChild<FindingItem>(i);
                    item.InitUIEffect(this.m_findingMode);
                }

            }
            this.m_propUseUILogic.Visible = false;
            this.sceneCamera = GameObject.FindGameObjectsWithTag("MainCamera");
            this.m_mutilCameraSys.OnShow();

            this.m_backBtn.AddClickCallBack(OnClickQuitScene);
            this.m_btnPause.AddClickCallBack(OnBtnGamePauseClick);
            this.m_lbTimeRemain.Text = "";
            this.m_propHintCom0.Visible = false;
            this.m_propHintCom1.Visible = false;
            this.m_propHintCom2.Visible = false;
            this.m_propHintCom3.Visible = false;
            GameEvents.MainGameEvents.OnStartGame += OnStartGame;
            GameEvents.MainGameEvents.OnScenePanelVisible += OnScenePanelVisible;
            GameEvents.MainGameEvents.OnPickedSceneObject += OnPickedSceneItem;
            GameEvents.MainGameEvents.NotifyHintSceneItemEntityList += OnHintingSceneItemEntityList;
            GameEvents.MainGameEvents.NotifyHintSceneItemEntityListNoCameraFollow += OnHintingSceneItemEntityListNoCameraFollow;
            GameEvents.MainGameEvents.OnErrorTouch += OnErrorTouch;
            GameEvents.MainGameEvents.OnGameOver += OnGameOver;
            GameEvents.MainGameEvents.ScanAndNotifyHintItems += OnScanAndHintSceneItems;
            GameEvents.MainGameEvents.OnGameStatusChange += OnGameStatusChange;
            GameEvents.UIEvents.UI_GameMain_Event.LockFindItem += LockFindItem;
            GameEvents.SceneEvents.OnScenePropWinVisible += OnScenePropWinVisible;
            GameEvents.SceneEvents.SetSceneType += SetSceneType;
            GameEvents.SceneEvents.OnQuitToMainCamera += OnQuitToMainCamera;
#if OFFICER_SYS
            GameEvents.UIEvents.UI_GameMain_Event.GetHeroItemById = GetHeroItemById;
#endif
            GameEvents.UIEvents.UI_GameMain_Event.GetPropItemById = GetPropItemById;
            GameEvents.MainGameEvents.OnCheckSceneCameraPoint += OnCheckSceneCameraPoint;
            this.m_noEnoughTimeEffect.Visible = false;
            this.m_noEnoughTimeEffect.EffectPrefabName = "UI_hongping.prefab";
            this.m_noEnoughTimeEffect.FitUIScale = true;

            m_errorTouchEffectContainer.EnsureSize<GameUIEffect>(3);
            for (int i = 0; i < this.m_errorTouchEffectContainer.ChildCount; ++i)
                this.m_errorTouchEffectContainer.GetChild<GameUIEffect>(i).EffectPrefabName = "UI_dianjishibai.prefab";


            this.m_errorPunishEffect.EffectPrefabName = "UI_dianjishibai_02.prefab";
            this.m_errorPunishEffect.Visible = false;

            this.m_scanerEffectComponent.Visible = false;
            OnRequestBuyPropInfoEvent = OnRequestBuyPropInfo;

            RefreshPlayerPropItems();
            this.m_root.Visible = true;
            this.m_btnPause.Visible = true;
            m_timeover_audio = null;
            m_is_refresh_progress = false;

            this.StartUpdateProgerss();

            this.m_gameProgress_effect.Visible = false;
            this.m_gameProgress.Value = 0.0f;

            Buy_time_prop_count = 0;

            this.m_exhibitTipsEffect.EffectPrefabName = "UI_xinshouyindao_shou.prefab";
            this.m_exhibitTipsEffect.Visible = false;
            GameEvents.SceneEvents.OnSceneExhibitTipsGuide += OnSceneExhibitTips;
            if (GameMainHelper.Instance.isNeedFingerTips(this.m_sceneId, this.taskConfID, true))
            {
                GameEvents.SceneEvents.OnSceneExhibitTips += OnSceneExhibitTips;

            }
            EngineCoreEvents.InputEvent.OnTouchScene += OnTouchScene;

        }

        private void OnClickQuitScene(GameObject obj)
        {
            if (GuidNewNodeManager.Instance.GetNodeStatus(GuidNewNodeManager.ForbidBtnBack) == NodeStatus.Complete)
            {
                GameEvents.SceneEvents.OnClickQuitScene.SafeInvoke(m_sceneType);
            }
            //this.m_sceneType = 0;
        }

        private void LockFindItem(long entityID, bool isLock)
        {
            for (int i = 0; i < m_uiFindingItemList.Count; i++)
            {
                if (m_uiFindingItemList[i].ExhibitID == entityID)
                {
                    m_uiFindingItemList[i].LockFindItem(isLock);
                    return;
                }
            }
        }

        private void OnQuitToMainCamera(string cameraName)
        {

            List<long> removeHintEffect = new List<long>();
            foreach (var kv in m_cacheHintEffect)
            {
                if (kv.Value.ClearHintEntityByCamera(cameraName))
                    removeHintEffect.Add(kv.Key);
            }
            for (int i = 0; i < removeHintEffect.Count; i++)
            {
                m_cacheHintEffect.Remove(removeHintEffect[i]);
            }
            removeHintEffect.Clear();
        }

        private void SetSceneType(int sceneType)
        {
            this.m_sceneType = sceneType;
        }

#if OFFICER_SYS
        private Transform GetHeroItemById(long heroId)
        {
            HeroItem heroItem = GetHeroItemByID(heroId);
            if (heroItem != null)
            {
                return heroItem.Widget;
            }
            return null;
        }
#endif

        private Transform GetPropItemById(long propId)
        {
            UseableItem item = GetUseableItemByItemID(propId);
            if (item != null)
            {
                return item.Widget;
            }
            return null;
        }


        private void OnScenePanelVisible(bool visible)
        {
            this.m_root.Visible = visible;
        }

        private void OnGameOver(SceneBase.GameResult result)
        {
            if ((byte)result <= 127) //超时
            {
                EngineCore.EngineCoreEvents.SystemEvents.OnApplicationPause -= OnApplicationPauseHandler;
                this.m_exhibitTipsEffect.Visible = false;
                this.m_propUseUILogic.Visible = true;
                this.m_noEnoughTimeEffect.Visible = false;
                OnBtnGamePause(true);
            }
        }

        private void OnScenePropWinVisible(bool visible)
        {
            if (!visible)
            {
                EngineCore.EngineCoreEvents.SystemEvents.OnApplicationPause += OnApplicationPauseHandler;
            }
        }

        private SceneItemEntity tipsEntity = null;
        private void OnSceneExhibitTips(SceneItemEntity entity, bool visible)
        {
            if (entity != null && visible)
            {
                this.tipsEntity = entity;
                this.m_exhibitTipsEffect.Position = CameraUtility.WorldPointInCanvasRectTransform(entity.EntityPosition, root);// + Vector3.left * 9f + Vector3.up * 15f;
            }
            this.m_exhibitTipsEffect.Visible = visible;
        }
        //private SceneItemEntity m_currentItemEntity = null;

        private void OnTouchScene()
        {
            //this.m_exhibitTipsEffect.Visible = false;
            if (this.m_exhibitTipsEffect.Visible && tipsEntity != null)
            {
                this.m_exhibitTipsEffect.Position = CameraUtility.WorldPointInCanvasRectTransform(tipsEntity.EntityPosition, root);// + Vector3.left * 9f + Vector3.up * 15f;
            }
        }

        private void OnGameStatusChange(SceneBase.GameStatus status)
        {
            if (status == SceneBase.GameStatus.PAUSE)
            {
                for (int i = 0; i < this.m_hintEffectContainer.ChildCount; i++)
                {
                    HintEffectComponent hint = this.m_hintEffectContainer.GetChild<HintEffectComponent>(i);
                    hint.Visible = false;
                }
                //this.m_hintEffectContainer. = false;
            }
        }

        public void OnStartGame()
        {
            CameraManager.Instance.SetCameraParam(this.m_sceneId);
            m_isAllowControlCamera = CameraManager.Instance.EnableGameCameraController;
            m_sceneCameraSystem = new GameSceneCameraSystem(this.m_backBtn);
            this.m_sceneCameraSystem.SetQuitObj(this.m_btnPause);
            this.m_sceneAllTaskEntityCount = GameEvents.MainGameEvents.GetSceneTaskItemEntityList().Count;

            int findNum = 6;
            if (CurrentGameScene.ItemFindingMode == SceneItemFindingMode.OneItemMultipleTime)
            {
                findNum = this.m_sceneAllTaskEntityCount;
            }

            List<SceneItemEntity> initEntityList = GameEvents.MainGameEvents.RequestNextFindingTaskEntities(findNum);
            InitFindingItem(initEntityList);
#if OFFICER_SYS
            IList<long> heroList = GameEvents.MainGameEvents.RequestHeroList();
            InitHeroList(heroList);
#endif

            this.m_gameProgress.Visible = CurrentGameScene.ItemFindingMode == SceneItemFindingMode.Once;
            this.m_gameProgress_effect.Visible = false;
            this.m_gameProgress.Value = 0.0f;
            m_is_show_progress_effect = CurrentGameScene.ItemFindingMode == SceneItemFindingMode.Once;
            if (GlobalInfo.GAME_NETMODE == GameNetworkMode.Network) //NewGuid.GuidNewManager.Instance.GetProgressByIndex(3)
                EngineCore.EngineCoreEvents.SystemEvents.OnApplicationPause += OnApplicationPauseHandler;

            RefreshProgress(true);

            GameEvents.MainGameEvents.OnPunish += OnPunishHandler;
            if (GlobalInfo.GAME_NETMODE == GameNetworkMode.Network && PlayerPrefTool.NeedSceneTips())
            {
                this.m_Mask.Visible = true;
                GameEvents.MainGameEvents.OnGameStatusChange.SafeInvoke(SceneBase.GameStatus.PAUSE);
                this.m_sceneTipsEffect.EffectPrefabName = "UI_xinshouyindao_shou02.prefab";
                this.m_sceneTipsEffect.Visible = true;
                TimeModule.Instance.SetTimeout(() =>
                {
                    this.m_Mask.AddPressDownCallBack(MaskClick);
                }, 1.0f);
            }
            GameEvents.MainGameEvents.OnGameInitComplete.SafeInvoke();


            if (CurrentGameScene.GameOverLimit == GameOverCondition.TIME_LIMIT)
            {
                GameEvents.MainGameEvents.OnGameTimeTick += ShowRemainTime;
            }

        }

        private void MaskClick(GameObject obj)
        {
            GameEvents.MainGameEvents.OnGameStatusChange.SafeInvoke(SceneBase.GameStatus.GAMING);
            this.m_sceneTipsEffect.Visible = false;
            this.m_Mask.Visible = false;
            this.m_Mask.RemovePressDownCallBack(MaskClick);
        }

        public void RefreshPlayerPropItems()
        {
            ConfScene confScene = ConfScene.Get(this.m_sceneId);
            this.m_useItemContainer.EnsureSize<UseableItem>(confScene.props.Length);
            for (int i = 0; i < confScene.props.Length; ++i)
            {
                UseableItem useItem = this.m_useItemContainer.GetChild<UseableItem>(i);
                useItem.gameObject.name = confScene.props[i].ToString();
                useItem.NeedForceTips = GameMainHelper.Instance.isNeedPropTips(confScene.props[i], taskConfID);
                useItem.SetData(confScene.props[i], arrowTipData);
                if (i == 0)
                {
                    useItem.SetScenePropHintCom(this.m_propHintCom0);
                }
                else if (i == 1)
                {
                    useItem.SetScenePropHintCom(this.m_propHintCom1);
                }
                else if (i == 2)
                {
                    useItem.SetScenePropHintCom(this.m_propHintCom2);
                }
                else
                {
                    useItem.SetScenePropHintCom(this.m_propHintCom3);
                }
                useItem.Visible = true;
                useItem.EnableItem = true;
            }

            float scaleFactor = 4.0f / confScene.props.Length;

            this.m_useItemContainer.SetScale(new Vector3(scaleFactor, scaleFactor, 0));

            this.m_useItemContainer.Widget.anchoredPosition = new Vector2(0, -this.m_useItemContainer.Widget.rect.height * (1 - scaleFactor));
            GameEvents.MainGameEvents.OnGetMissileStartPos += OnGetMissileStartPos;
        }

        private void OnGetMissileStartPos(Vector3 entityPos, Action cb)
        {
            #region 三维炸弹
            //UseableItem useItem = GetUseableItemByItemID(3);
            //Vector3 startPos = CameraUtility.UIPostionToWorldPos(useItem.Position,null);
            //Vector3 endPos = entityPos;
            //GameEvents.MainGameEvents.OnMissileFly.SafeInvoke(startPos, endPos, cb);
            #endregion
            #region 二维炸弹
            UseableItem useItem = GetUseableItemByItemID(3);
            Vector3 startPos = useItem.Position;
            Vector3 endPos = CameraUtility.WorldPointInCanvasRectTransform(entityPos, root);
            GameEvents.MainGameEvents.OnMissileFly.SafeInvoke(startPos, endPos, cb);
            #endregion
        }

        /// <summary>
        /// 游戏内商店买完道具返回自动使用
        /// </summary>
        /// <param name="itemID"></param>
        public void UseItemAfterBuy(long itemID)
        {
            this.m_autoUseItemAfterBuy = itemID;
        }

        private void InitFindingItem(List<SceneItemEntity> findingItemList)
        {
            //if (CurrentGameScene.ItemFindingMode == SceneItemFindingMode.Once)
            //{
            for (int i = 0; i < findingItemList.Count; ++i)
            {
                SceneItemEntity findItemEntity = findingItemList[i];
                SetFindingItemUIInfo(i, findItemEntity);
            }

        }
#if OFFICER_SYS
        private void InitHeroList(IList<long> heroIds)
        {
            for (int i = 0; i < m_heroImg.Length; i++)
            {
                if (i < heroIds.Count)
                {
                    OfficerInfo officeInfo = GlobalInfo.MY_PLAYER_INFO.GetOfficerInfoByPlayId(heroIds[i]);
                    if (officeInfo == null)
                    {
                        continue;
                    }
                    m_heroImg[i].SetData(officeInfo);
                    m_heroImg[i].Visible = true;
                }
                else
                {
                    m_heroImg[i].Visible = false;
                }
            }
        }
#endif

        /// <summary>
        /// 设置寻找物体UI信息
        /// </summary>
        /// <param name="index"></param>
        /// <param name="sceneItemInfo"></param>
        private void SetFindingItemUIInfo(int index, SceneItemEntity sceneItemInfo)
        {
            if (index < 0 || index >= this.m_uiFindingItemList.Count)
                Debug.LogError("item index Error:" + index);

            Confexhibit confItemInfo = Confexhibit.Get(sceneItemInfo.EntityData.itemID);
            if (confItemInfo != null)
            {
                this.m_uiFindingItemList[index].FindMode = this.m_findingMode;
                this.m_uiFindingItemList[index].SetFindingItemData(confItemInfo.iconName, confItemInfo.name, sceneItemInfo.EntityData.itemID, confItemInfo.isStory == 1);
                this.m_uiFindingItemList[index].EntityId = sceneItemInfo.EntityId;
                this.m_uiFindingItemList[index].Visible = true;
            }
        }

        private void RevertItemNameToNormalHandler(int num, long carryId)
        {
            if (this.m_uiFindingItemList.Count < num)
                num = this.m_uiFindingItemList.Count;

            int[] randomIndex = CommonUtils.GetRandomList(this.m_uiFindingItemList.Count);
            for (int i = 0; i < num; ++i)
            {

                FindingItem uiFindItem = GetRandIndex(i, randomIndex);
                if (uiFindItem != null)
                {
#if OFFICER_SYS
                    this.m_skillEffectSystem.PlayTuoWeiEffect(carryId, uiFindItem.Widget.position, () =>
                    {
                        uiFindItem.ReverseToNormal();
                    });
#else
                    uiFindItem.ReverseToNormal();
#endif
                }
            }
        }

        private FindingItem GetRandIndex(int i, int[] randomIndex)
        {
            if (i >= randomIndex.Length)
            {
                return null;
            }
            int randomUIIndex = randomIndex[i];
            FindingItem uiFindItem = this.m_uiFindingItemList[randomUIIndex];
            if (uiFindItem.Visible && uiFindItem.IsFanci)
            {
                uiFindItem.IsFanci = false;
                return uiFindItem;
            }
            return GetRandIndex(i + 1, randomIndex);
        }

        private void OnHintingSceneItemEntityList(List<SceneItemEntity> hintingItemList)
        {
            string cameraName = string.Empty;
            if (hintingItemList.Count == 1)
            {
                cameraName = hintingItemList[0].CameraName;
            }
            this.m_sceneCameraSystem.CheckSceneCameraPoint(() =>
            {
                List<HintEffectComponent> hintEffectComponentList = GetAvaliableHintEffectComponentList(hintingItemList);
                if (hintingItemList.Count == 1)
                {
                    this.m_exhibitTipsEffect.Visible = false;
                    GameEvents.MainGameEvents.OnResetFingerTips.SafeInvoke();
                    //新手引导特殊处理
                    if (GuidNewNodeManager.Instance.GetNodeStatus(GuidNewNodeManager.FindItemByID) == NodeStatus.Complete)
                        GameEvents.MainGameEvents.OnCameraMove.SafeInvoke(hintingItemList[0].EntityObject.EntityGameObject);
                }
                for (int i = 0; i < hintEffectComponentList.Count; ++i)
                    hintEffectComponentList[i].Visible = true;
            }, cameraName);

        }

        private void OnHintingSceneItemEntityListNoCameraFollow(List<SceneItemEntity> hintingItemList)
        {
            List<HintEffectComponent> hintEffectComponentList = GetAvaliableHintEffectComponentList(hintingItemList);
            for (int i = 0; i < hintEffectComponentList.Count; ++i)
                hintEffectComponentList[i].Visible = true;
        }

        Dictionary<long, HintEffectComponent> m_cacheHintEffect = new Dictionary<long, HintEffectComponent>();
        //List<HintEffectComponent> m_cacheHintEffect = new List<HintEffectComponent>();
        private List<HintEffectComponent> GetAvaliableHintEffectComponentList(List<SceneItemEntity> hintingItemList)
        {
            this.m_hintingEntityDict.Clear();
            for (int i = 0; i < this.m_hintEffectContainer.ChildCount; ++i)
            {
                this.m_hintEffectContainer.GetChild<HintEffectComponent>(i).EndHint();
            }

            for (int i = 0; i < hintingItemList.Count; ++i)
            {

                SceneItemEntity newHintSceneItemEntity = hintingItemList[i];
                //bool isInCamera = CameraUtility.IsGameObjectInCameraView(newHintSceneItemEntity.EntityObject.EntityGameObject);
                HintEffectComponent hintEffectComponent = this.m_hintEffectContainer.GetChild<HintEffectComponent>(i);
                bool needHint = !this.m_sceneCameraSystem.EntityInCurrentCamera(newHintSceneItemEntity.CameraName);
                hintEffectComponent.SetHintSceneEntity(newHintSceneItemEntity, needHint);
                if (needHint)
                {
                    if (!m_cacheHintEffect.ContainsKey(newHintSceneItemEntity.EntityData.itemID))
                    {
                        m_cacheHintEffect.Add(newHintSceneItemEntity.EntityData.itemID, hintEffectComponent);
                    }
                    //m_cacheHintEffect.Add(hintEffectComponent);
                }
                this.m_hintingEntityDict.Add(newHintSceneItemEntity, hintEffectComponent);
            }

            return this.m_hintingEntityDict.Values.ToList();
        }

        private void OnErrorTouch(float errorTime, Vector2 touchScreenPoint)
        {
            Vector3 pos = CameraUtility.ScreenPointInCanvasRectTransform(touchScreenPoint, root);
            GameUIEffect errorEffect = this.m_errorTouchEffectContainer.GetAvaliableContainerElement<GameUIEffect>();
            errorEffect.Position = pos;
            errorEffect.Visible = true;
            TimeModule.Instance.SetTimeout(() => { this.m_errorTouchEffectContainer.RecycleElement<GameUIEffect>(errorEffect); }, 0.5f);

            if (CurrentGameScene.GameOverLimit == GameOverCondition.STEPS_LIMIT)
            {
                this.m_lbTimeRemain.Text = (CurrentGameScene.GameOverCondition as GameOverBySteps).RemainStep.ToString();
            }
        }

        public void OnPunishHandler(float punish, Vector2 touchScreenPoint, float remainTime)
        {
            Vector3 pos = CameraUtility.ScreenPointInCanvasRectTransform(touchScreenPoint, root);
            this.m_errorPunishEffect.Position = pos;
            this.m_errorPunishEffect.Visible = true;
            TimeModule.Instance.SetTimeout(() => { this.m_errorPunishEffect.Visible = false; }, 2f);
        }
        public override FrameDisplayMode UIFrameDisplayMode
        {
            get
            {
                return FrameDisplayMode.WINDOWED;
            }
        }

        /// <summary>
        /// 游戏暂停按钮
        /// </summary>
        /// <param name="btnGamePause"></param>
        private void OnBtnGamePauseClick(GameObject btnGamePause)
        {
            if (GuidNewNodeManager.Instance.GetNodeStatus(GuidNewNodeManager.ForbidScenePause) == NodeStatus.None)
            {
                return;
            }
            if (this.m_gameInternalShopComponent.CachedVisible)
                this.m_gameInternalShopComponent.Visible = false;
            this.m_root.Visible = false;
            switch (this.m_gameMode)
            {
                case SceneMode.FOGGY:
                case SceneMode.DARKER:
                    ScreenDrawer.instance.PauseAllBrush();          //写的真tm乱套
                    break;
            }

            OnBtnGamePause(false);
        }

        private void OnBtnGamePause(bool hidePause)
        {
            EngineCoreEvents.AudioEvents.PlayAudio.SafeInvoke(Audio.AudioType.UISound, EngineCommonAudioKey.Button_Click_Common.ToString());

            float foundProgress = RefreshProgress();
            if (foundProgress < 1.0f)
            {
                CSSceneSuspendRequest pauseRequest = new CSSceneSuspendRequest();
                pauseRequest.PlayerId = GlobalInfo.MY_PLAYER_ID;

#if !NETWORK_SYNC || UNITY_EDITOR
                GameEvents.NetWorkEvents.SendAsyncMsg.SafeInvoke(pauseRequest);
#else
                GameEvents.NetWorkEvents.SendMsg.SafeInvoke(pauseRequest);
#endif



                EngineCoreEvents.UIEvent.ShowUIEventWithParam.SafeInvoke(
                    new FrameMgr.OpenUIParams(UIDefine.UI_GAME_MAIN_SETTING)
                    {
                        IsBlur = true,
                        Param = new PauseData()
                        {
                            m_mode = ENUM_SEARCH_MODE.E_SEARCH_ROOM,
                            m_id = m_sceneId,
                            m_hidePause = hidePause
                        }
                    });
                GameEvents.UIEvents.UI_Pause_Event.OnHidePauseFrame.SafeInvoke(!hidePause);
                this.m_btnPause.Visible = false;
                MessageHandler.RegisterMessageHandler(MessageDefine.SCSceneResumeResponse, OnResumeGame);
            }
        }

        private void OnResumeGame(object msg)
        {
            MessageHandler.UnRegisterMessageHandler(MessageDefine.SCSceneResumeResponse, OnResumeGame);
            this.m_btnPause.Visible = true;
            if(!this.m_root.CachedVisible)
                this.m_root.Visible = true;
            if (this.m_autoUseItemAfterBuy != -1)
            {
                UseableItem useableItem = GetUseableItemByItemID(this.m_autoUseItemAfterBuy);
                if (useableItem != null)
                    useableItem.BtnPropItem(null);

                this.m_autoUseItemAfterBuy = -1;
            }


            switch (this.m_gameMode)
            {
                case SceneMode.FOGGY:
                case SceneMode.DARKER:
                    ScreenDrawer.instance.ResumeAllBrush();          //写的真tm乱套
                    break;
            }

        }

        private UseableItem GetUseableItemByItemID(long itemID)
        {
            for (int i = 0; i < this.m_useItemContainer.ChildCount; ++i)
            {
                UseableItem useItem = this.m_useItemContainer.GetChild<UseableItem>(i);
                if (useItem.ItemID == itemID)
                    return useItem;
            }

            return null;
        }
#if OFFICER_SYS
        private HeroItem GetHeroItemByID(long heroID)
        {
            for (int i = 0; i < this.m_heroImg.Length; ++i)
            {
                if (m_heroImg[i].HeroId == heroID)
                    return this.m_heroImg[i];
            }
            return null;
        }
#endif
        /// <summary>
        /// 找到物品
        /// </summary>
        /// <param name="sceneItemEntity"></param>
        private void OnPickedSceneItem(SceneItemEntity sceneItemEntity)
        {
            GameEvents.MainGameEvents.OnResetArrowTipsTime.SafeInvoke();
            this.m_exhibitTipsEffect.Visible = false;
            int findingItemIndex = this.m_uiFindingItemList.FindIndex(item => item.EntityId == sceneItemEntity.EntityId);

            if (m_cacheHintEffect.ContainsKey(sceneItemEntity.EntityData.itemID))
            {
                m_cacheHintEffect[sceneItemEntity.EntityData.itemID].ClearHintEntityByObj(sceneItemEntity);
                m_cacheHintEffect.Remove(sceneItemEntity.EntityData.itemID);
            }
            Vector3 startPos = CameraUtility.WorldPointInCanvasRectTransform(sceneItemEntity.EntityPosition, root);
            Vector3 endPos = this.m_uiFindingItemList[findingItemIndex].Position;
            PickedFlyEffectComponent effectComponent = GetAvaliablePickedEffectComponent();
            Confexhibit pickItemConfig = Confexhibit.Get(sceneItemEntity.EntityData.itemID);
            if (effectComponent != null)
            {
                effectComponent.Visible = true;
                sceneItemEntity.EntityObject.EntityTransform.DOScale(sceneItemEntity.EntityScale * 1.5f, 0.4f).SetEase(Ease.InCubic).OnComplete(() =>
                {
                    sceneItemEntity.EntityObject.EntityTransform.DOScale(0f, 0.4f).OnComplete(() =>
                    {
                        startPos = CameraUtility.WorldPointInCanvasRectTransform(sceneItemEntity.EntityPosition, root);
                        effectComponent.SetPickedObject(sceneItemEntity.EntityObject.EntityGameObject, endPos);
                        effectComponent.SetFlyPath(startPos, endPos, Canvas.planeDistance, 1.5f);
                        effectComponent.SetPickedItemIcon(pickItemConfig.iconName);
                        effectComponent.PlayPickEffect();
                        //effectComponent.Visible = true;
                    });
                });

                effectComponent.OnFlyEffectPlayFinished = () =>
                {
                    if (findingItemIndex != -1 && CurrentGameScene.ItemFindingMode != SceneItemFindingMode.OneItemMultipleTime)
                    {
                        this.m_uiFindingItemList[findingItemIndex].PickedEffect.ReplayEffect();
                    }
                    if (findingItemIndex != -1)
                    {
                        TimeModule.Instance.SetTimeout(() =>
                        {
                            if (SceneModule.Instance.CurrentScene is GameSceneBase)
                            {
                                List<SceneItemEntity> newFindingTaskEntityList = GameEvents.MainGameEvents.RequestNextFindingTaskEntities(1);
                                FindingItem findingItemComponent = this.m_uiFindingItemList[findingItemIndex];
                                //if (CurrentGameScene.ItemFindingMode != SceneItemFindingMode.OneItemMultipleTime)
                                //{
                                //    this.m_uiFindingItemList[findingItemIndex].PickedEffect.Visible = true;
                                //}
                                if (CurrentGameScene.ItemFindingMode != SceneItemFindingMode.OneItemMultipleTime)
                                {
                                    findingItemComponent.Visible = false;
                                }

                                //findingItemComponent.Visible = false;
                                if (newFindingTaskEntityList.Count > 0)
                                {
                                    SceneItemEntity newFindingEntity = newFindingTaskEntityList[0];
                                    //SetFindingItemUIInfo(findingItemIndex, newFindingEntity);
                                    TimeModule.Instance.SetTimeout(() => SetFindingItemUIInfo(findingItemIndex, newFindingEntity), 0.2f);
                                }
                                //else
                                //{
                                //    if (CurrentGameScene.ItemFindingMode != SceneItemFindingMode.OneItemMultipleTime)
                                //    {
                                //        TimeModule.Instance.SetTimeout(() => findingItemComponent.Visible = false, 0.2f);
                                //    }
                                //}

                                RefreshProgress(true);
                            }

                        }, 0.8f);
                    }

                    //RefreshProgress(true);
                };
            }

            if (CurrentGameScene.m_randomOutputCalculator != null)
            {
                RandomDropComponent dropComponent = GetAvaliableRandomDropEffectComponent();
                long[] dropItemIds = CurrentGameScene.m_randomOutputCalculator.GetRandomOutputItemIDByPickTime(GameEvents.MainGameEvents.GetFoundTaskItemEntityList().Count - 1);
                if (dropComponent != null && dropItemIds.Length > 0)
                {
                    dropComponent.Visible = true;
                    dropComponent.SetRandomDropItem(dropItemIds, startPos);
                }
            }


            if (this.m_hintingEntityDict.ContainsKey(sceneItemEntity))
            {
                HintEffectComponent hintEffectComponent = this.m_hintingEntityDict[sceneItemEntity];
                hintEffectComponent.EndHint();
                this.m_hintingEntityDict.Remove(sceneItemEntity);
            }
            else
            {
                this.m_pickMarkEffect.Position = startPos;
                this.m_pickMarkEffect.EffectPrefabName = "UI_xuanzhong02.prefab";
                this.m_pickMarkEffect.Visible = true;
                TimeModule.Instance.SetTimeout(() => { this.m_pickMarkEffect.Visible = false; }, 0.5f);
            }


            //<刷新进度
            EnableUpdateProgress();
            //>
        }

        private void EnableUpdateProgress()
        {
            this.m_is_refresh_progress = true;
        }

        private void StartUpdateProgerss()
        {
            TimeModule.Instance.SetTimeInterval(UpdateProgress, C_REFRESH_PROGRESS_TIME);
        }

        private void StopUpdateProgress()
        {
            TimeModule.Instance.RemoveTimeaction(StartUpdateProgerss);
        }

        private void UpdateProgress()
        {
            if (!m_is_refresh_progress)
                return;

            RefreshProgress(C_REFRESH_PROGRESS_TIME);

            RefreshEffect();
        }

        private void RefreshEffect()
        {
            if (m_is_show_progress_effect)
            {
                m_is_show_progress_effect = false;
                this.m_gameProgress_effect.Visible = false;
                this.m_gameProgress_effect.Visible = true;
            }

            CommonHelper.EffectProgressbarValueSync(m_gameProgress, m_gameProgress_effect);
        }

        private void RefreshProgress(float dt_)
        {
            float foundCount = (float)GameEvents.MainGameEvents.GetFoundTaskItemEntityList().Count;
            float foundProgress = foundCount / this.m_sceneAllTaskEntityCount;


            if (this.m_gameProgress.Value < foundProgress)
            {
                this.m_gameProgress.Value += dt_;
            }
            else
            {
                this.m_gameProgress.Value = foundProgress;
                m_gameProgress_effect.Visible = false;
                m_is_show_progress_effect = true;
                this.m_is_refresh_progress = false;
            }

            if (NewGuid.GuidNewManager.Instance.GetProgressByIndex(3))
                this.m_btnPause.Visible = foundProgress != 1.0;

            if (foundProgress == 1.0f)
                EngineCore.EngineCoreEvents.SystemEvents.OnApplicationPause -= OnApplicationPauseHandler;
        }

        private int GetSceneEntityIndex(SceneItemEntity sceneItemEntity)
        {
            return GameEvents.MainGameEvents.GetSceneTaskItemEntityList().IndexOf(sceneItemEntity);
        }

        public PickedFlyEffectComponent GetAvaliablePickedEffectComponent()
        {
            for (int i = 0; i < this.m_flyEffectContainer.ChildCount; ++i)
            {
                PickedFlyEffectComponent flyEffectComponent = this.m_flyEffectContainer.GetChild<PickedFlyEffectComponent>(i);
                if (!flyEffectComponent.Visible)
                    return flyEffectComponent;
            }

            return null;
        }

        public RandomDropComponent GetAvaliableRandomDropEffectComponent()
        {
            for (int i = 0; i < this.m_randomDropEffectContainer.ChildCount; ++i)
            {
                RandomDropComponent dropEffect = this.m_randomDropEffectContainer.GetChild<RandomDropComponent>(i);
                if (!dropEffect.Visible)
                    return dropEffect;
            }

            return null;
        }
        private void StartRefreshUIEffect()
        {
            CommonHelper.EffectProgressbarValueSync(m_gameProgress, m_gameProgress_effect);
        }
        private void StopRefreshUIEffect()
        {
            TimeModule.Instance.RemoveTimeaction(StartRefreshUIEffect);
        }

        private float GetProgress()
        {
            float foundCount = (float)GameEvents.MainGameEvents.GetFoundTaskItemEntityList().Count;
            float foundProgress = foundCount / this.m_sceneAllTaskEntityCount;

            return foundProgress;
        }

        private float RefreshProgress(bool progressChanged = false)
        {
            float foundCount = (float)GameEvents.MainGameEvents.GetFoundTaskItemEntityList().Count;
            float foundProgress = foundCount / this.m_sceneAllTaskEntityCount;
            //this.m_gameProgress.Value = foundProgress;


            if (!MathUtil.FloatEqual(foundProgress, this.m_gameProgress.Value))
            {
                if (m_is_show_progress_effect)
                {
                    Debug.Log("ENABLE EFFECT");
                    m_is_show_progress_effect = false;
                    this.m_gameProgress_effect.Visible = false;
                    this.m_gameProgress_effect.Visible = true;
                }


                DOTween.To(() => this.m_gameProgress.Value, x => this.m_gameProgress.Value = x, foundProgress, C_REFRESH_PROGRESS_TIME);
                //TimeModule.Instance.SetTimeInterval(StartRefreshUIEffect, 0.0001f);
                //TimeModule.Instance.SetTimeout(StopRefreshUIEffect, 0.6f);
                CommonHelper.EffectProgressbarValueSync(m_gameProgress, m_gameProgress_effect);
            }
            else if (!m_is_show_progress_effect)
            {
                Debug.Log("DISABLE EFFECT");
                m_gameProgress_effect.Visible = false;
                m_is_show_progress_effect = true;

                this.m_is_refresh_progress = false;
            }

            if (NewGuid.GuidNewManager.Instance.GetProgressByIndex(3))
                this.m_btnPause.Visible = foundProgress != 1.0;

            if (progressChanged)
                this.OnRefreshFindingProgress?.Invoke((int)foundCount, this.m_sceneAllTaskEntityCount);

            if (foundProgress == 1.0f)
                EngineCore.EngineCoreEvents.SystemEvents.OnApplicationPause -= OnApplicationPauseHandler;



            return foundProgress;
        }

        private void OnScanAndHintSceneItems(float time, int hintNum)
        {
            this.m_sceneCameraSystem.CheckSceneCameraPoint(() =>
            {
                List<SceneItemEntity> hintEntityList = GameEvents.MainGameEvents.GetSceneItemEntityList(hintNum);
                List<HintEffectComponent> hintEffectComponents = GetAvaliableHintEffectComponentList(hintEntityList);

                if (m_isAllowControlCamera)
                {
                    CameraManager.Instance.EnableGameCameraController = false;
                    TimeModule.Instance.SetTimeout(() => { CameraManager.Instance.EnableGameCameraController = true; }, time + 0.1f);
                }
                this.m_scanerEffectComponent.SetHintEntityComponentList(hintEffectComponents);
                this.m_scanerEffectComponent.DoScan(time);
            }, string.Empty);

        }

        private void OnDestoryPropSkill()
        {
            ConfScene confScene = ConfScene.Get(this.m_sceneId);
            if (confScene == null)
            {
                return;
            }
            GameSkillManager.Instance.OnBreakPropSkill(confScene.props);
        }

        private void OnApplicationPauseHandler(bool isPause)
        {

            if (isPause)
            {

                OnBtnGamePauseClick(null);
            }
        }

        public override void OnHide()
        {
            base.OnHide();
            m_cacheHintEffect.Clear();
            LoadingManager.Instance.SCENE_ID = 0;
            GameMainHelper.Instance.currentTaskID = -1;
            this.m_sceneType = 0;
            this.m_Mask.Visible = false;
            GameMainHelper.Instance.m_currentScenMode = SceneMode.NORMALGAME;
            this.m_mutilCameraSys.OnHide();
            this.m_btnPause.RemoveClickCallBack(OnBtnGamePauseClick);
            this.m_backBtn.RemoveClickCallBack(OnClickQuitScene);
            this.m_noEnoughTimeEffect.Visible = false;
            GameEvents.MainGameEvents.OnScenePanelVisible -= OnScenePanelVisible;
            GameEvents.MainGameEvents.OnStartGame -= OnStartGame;
            GameEvents.MainGameEvents.OnPickedSceneObject -= OnPickedSceneItem;
            GameEvents.MainGameEvents.OnGameStatusChange -= OnGameStatusChange;
            GameEvents.MainGameEvents.NotifyHintSceneItemEntityList -= OnHintingSceneItemEntityList;
            GameEvents.MainGameEvents.NotifyHintSceneItemEntityListNoCameraFollow -= OnHintingSceneItemEntityListNoCameraFollow;
            GameEvents.MainGameEvents.OnErrorTouch -= OnErrorTouch;
            GameEvents.MainGameEvents.ScanAndNotifyHintItems -= OnScanAndHintSceneItems;
            GameEvents.MainGameEvents.OnPunish -= OnPunishHandler;
            GameEvents.MainGameEvents.OnGameOver -= OnGameOver;
            GameEvents.SceneEvents.OnScenePropWinVisible -= OnScenePropWinVisible;
            GameEvents.SceneEvents.SetSceneType -= SetSceneType;
            GameEvents.MainGameEvents.OnGetMissileStartPos -= OnGetMissileStartPos;
            GameEvents.SceneEvents.OnSceneExhibitTipsGuide -= OnSceneExhibitTips;
            GameEvents.UIEvents.UI_GameMain_Event.LockFindItem -= LockFindItem;
            GameEvents.MainGameEvents.OnCheckSceneCameraPoint -= OnCheckSceneCameraPoint;
            GameEvents.SceneEvents.OnQuitToMainCamera -= OnQuitToMainCamera;
            if (GameMainHelper.Instance.isNeedFingerTips(this.m_sceneId, this.taskConfID, true))
            {
                GameEvents.SceneEvents.OnSceneExhibitTips -= OnSceneExhibitTips;

            }
            EngineCoreEvents.InputEvent.OnTouchScene -= OnTouchScene;
            this.taskConfID = -1;
            OnDestoryPropSkill();
            if (this.m_findingMode == FindingMode.REVERSE_NAME)
                GameEvents.MainGameEvents.RevertItemNameToNormal -= RevertItemNameToNormalHandler;

            if (null != m_timeover_audio)
                m_timeover_audio.Stop();

            if (this.m_gameInternalShopComponent.CachedVisible)
                m_gameInternalShopComponent.Visible = false;
            EngineCore.EngineCoreEvents.SystemEvents.OnApplicationPause -= OnApplicationPauseHandler;
            //CameraManager.Instance.EnableGameCameraController = false;
            EngineCoreEvents.UIEvent.HideUIEvent.SafeInvoke(UIDefine.UI_GAME_MAIN_SETTING);


            for (int i = 0; i < m_uiFindingItemList.Count; ++i)
                m_uiFindingItemList[i].Visible = false;

            m_gameProgress_effect.Visible = false;

            this.StopUpdateProgress();
            this.m_sceneCameraSystem.OnDestory();
            if (Buy_time_prop_count > 0)
            {
                Dictionary<UBSParamKeyName, object> internalBuyItemKeypoint = new Dictionary<UBSParamKeyName, object>();
                internalBuyItemKeypoint.Add(UBSParamKeyName.Description, UBSDescription.PROPBUY);
                internalBuyItemKeypoint.Add(UBSParamKeyName.PropItem_Num, Buy_time_prop_count);
                UserBehaviorStatisticsModules.Instance.LogEvent(UBSEventKeyName.game_timefinish, Buy_time_prop_count, internalBuyItemKeypoint);
            }
        }

        private void OnCheckSceneCameraPoint(Action cb)
        {
            this.m_sceneCameraSystem.CheckSceneCameraPoint(cb, string.Empty);
        }

        /// <summary>
        /// 显示剩余时间
        /// </summary>
        /// <param name="remainTime"></param>
        private void ShowRemainTime(int remainTime)
        {
            this.m_gameRemainSeconds = remainTime;
            int remainMinute = remainTime / 60;
            int remainSecond = remainTime % 60;
            if (GuidNewManager.Instance.GetProgressByIndex(2))
            {
                this.m_lbTimeRemain.Text = string.Format("{0}:{1}", remainMinute <= 9 ? "0" + remainMinute.ToString() : remainMinute.ToString(), remainSecond <= 9 ? "0" + remainSecond.ToString() : remainSecond.ToString());
            }
            if (remainMinute == 0 && remainSecond <= 9)
            {
                if (!this.m_noEnoughTimeEffect.Visible)
                    EngineCoreEvents.AudioEvents.PlayAndGetAudio.SafeInvoke(Audio.AudioType.Sound, GameCustomAudioKey.timeover.ToString(), audio =>
                    {
                        m_timeover_audio = audio;
                    });
                this.m_noEnoughTimeEffect.Visible = true;
            }
            else
            {
                if (null != m_timeover_audio)
                    m_timeover_audio.Stop();

                this.m_noEnoughTimeEffect.Visible = false;
            }
        }

        public override void Update()
        {
            base.Update();

            if (this.m_useItemContainer != null && this.m_useItemContainer.ChildCount > 0)
            {
                for (int i = 0; i < this.m_useItemContainer.ChildCount; i++)
                {
                    UseableItem item = this.m_useItemContainer.GetChild<UseableItem>(i);
                    if (item != null)
                    {
                        if (this.m_gameRemainSeconds <= 0 || GetProgress() == 1)
                            item.OutClick = false;
                        else
                            item.OutClick = true;
                    }
                }
            }
        }

        public GameSceneBase CurrentGameScene { get { return SceneModule.Instance.CurrentScene as GameSceneBase; } }

#if OFFICER_SYS
        /// <summary>
        /// 英雄
        /// </summary>
        private class HeroItem : GameUIComponent
        {
            private GameImage m_icon;
            private OfficerInfo m_officerInfo;
            private GameLabel m_level = null;
            private GameUIEffect m_buffUIEffect = null;

            protected override void OnInit()
            {
                base.OnInit();
                m_icon = Make<GameImage>(gameObject);
                this.m_level = Make<GameLabel>("Image:Text");
                this.m_buffUIEffect = Make<GameUIEffect>("UI_fancimoshi_beidongjineng");
            }

            public void SetData(OfficerInfo officerInfo)
            {
                m_officerInfo = officerInfo;
                ConfOfficer officer = ConfOfficer.Get(officerInfo.OfficerId);
                if (officer == null)
                {
                    return;
                }
                long skillId = SkillTools.GetSkillIdByLevel(officer, officerInfo.Level);
                ConfSkill confSkill = ConfSkill.Get(skillId);
                if (confSkill == null)
                {
                    return;
                }
                this.m_level.Text = officerInfo.Level.ToString();
                m_icon.Sprite = confSkill.icon;
                if (confSkill.phase != 2)
                {
                    m_icon.SetGray(true);
                }
                else
                {
                    m_icon.SetGray(false);
                }
            }

            private void OnSkillFinish(long carryId)
            {

                if (m_officerInfo.PlayerOfficerId == carryId)
                {
                    this.m_buffUIEffect.Visible = true;
                    this.m_buffUIEffect.SetEffectHideTime(1f);
                    m_icon.SetGray(true);
                }
            }

            public override void OnShow(object param)
            {
                base.OnShow(param);
                GameEvents.Skill_Event.OnSkillFinish += OnSkillFinish;
                this.m_buffUIEffect.EffectPrefabName = "UI_fancimoshi_beidongjineng.prefab";
                this.m_buffUIEffect.Visible = false;
            }

            public override void OnHide()
            {
                base.OnHide();
                GameEvents.Skill_Event.OnSkillFinish -= OnSkillFinish;
            }

            public long HeroId
            {
                get { return m_officerInfo.PlayerOfficerId; }
            }
        }
#endif
        /// <summary>
        /// 局内购买道具-查询道具信息请求
        /// </summary>
        /// <param name="propID"></param>
        private void OnRequestBuyPropInfo(long propID)
        {
            MessageHandler.RegisterMessageHandler(MessageDefine.SCMarketItemResponse, OnSearchBuyPropInfoResponse);
            CSMarketItemRequest req = new CSMarketItemRequest();
            req.PropId = propID;

#if !NETWORK_SYNC || UNITY_EDITOR
            GameEvents.NetWorkEvents.SendHalfSyncMsg.SafeInvoke(req);
#else
            GameEvents.NetWorkEvents.SendMsg.SafeInvoke(req);
#endif

        }

        /// <summary>
        ///局内购买道具-查询道具信息响应
        /// </summary>
        /// <param name="resp"></param>
        private void OnSearchBuyPropInfoResponse(object msg)
        {
            MessageHandler.UnRegisterMessageHandler(MessageDefine.SCMarketItemResponse, OnSearchBuyPropInfoResponse);
            SCMarketItemResponse marketPropInfoMsg = msg as SCMarketItemResponse;
            if (!MsgStatusCodeUtil.OnError(marketPropInfoMsg.ResponseStatus))
            {
                Debug.Log("hahahah=====");
                this.m_gameInternalShopComponent.SetBuyItemInfo(marketPropInfoMsg);
                if (!this.m_gameInternalShopComponent.CachedVisible)
                    this.m_gameInternalShopComponent.Visible = true;
                OnBtnGamePause(true);
                //OnBtnGamePauseClick(null);
            }
        }
    }

    /// <summary>
    /// 寻物模式
    /// </summary>
    public enum FindingMode
    {
        NORMAL = 1,
        REVERSE_NAME,       //反词模式
        SHADOW, //剪影模式
    }
}