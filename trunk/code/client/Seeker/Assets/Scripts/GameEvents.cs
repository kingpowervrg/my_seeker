/********************************************************************
	created:  2018-3-27 11:28:40
	filename: GameEvents.cs
	author:	  songguangze@outlook.com
	
	purpose:  客户端事件
*********************************************************************/

using EngineCore;
using Google.Protobuf;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace SeekerGame
{
    public static class GameEvents
    {
        /// <summary>
        /// UI事件
        /// </summary>
        public static class UIEvents
        {
            public static class UI_Enter_Event
            {
#if OFFICER_SYS
                public static SafeAction<long> EVT_SELECT_POLICE;
                public static SafeAction OnClearDispatchPoclie;
                public static SafeAction<int> OnLimitPoliceNum;


                public static SafeFunc<List<long>> Tell_GetAllDispatchedOfficerID;
#endif

                public static SafeFunc<ENUM_SEARCH_MODE> Tell_GetGameType;
            }

            public static class UI_Drop_Event
            {
                public static Func<EUNM_BASE_REWARD, Transform> GetPlayerInfoGameObject;
            }

            //背包事件
            public static class UI_Bag_Event
            {
                public static SafeAction<long> OnPropCost;
                public static SafeAction<Transform> OnItemClick;
                public static Func<BagTypeEnum> GetCurrentBagType;

                public static SafeAction<long> Tell_OnPropIn;

                public static SafeFunc<IEnumerable<PropData>> Listen_GetAllExhibitions;
            }

            //对话事件
            public static class UI_Talk_Event
            {
                public static SafeAction<long> OnTalkFinish; //对话结束事件
                public static SafeAction<long> OnTalkChoose; //选择ID
                public static SafeAction OnTalkRewardFinish; //对话领取奖励结束
                public static SafeAction OnTalkNextPart;
            }

            //主界面事件
            public static class UI_GameEntry_Event
            {
                public static SafeAction OnReflashPanel;

                public static SafeAction<int, int> OnReflashVit;
                public static SafeAction<int, int> OnReflashCoin;
                public static SafeAction<int, int> OnReflashCash;
                public static SafeAction OnReflashLevel;

                public static SafeAction<float> OnCountDownVit;
                public static SafeAction<string> OnOpenPanel;

                public static SafeAction<bool> OnControlActivity;

                public static SafeAction OnCloseNoticRedDot;

                public static SafeAction<string> OnGameEntryOpen;
                public static Func<string> GetCurrentGameEntryUI;

                public static SafeAction<float> OnInfiniteVit; //无限体力

                public static SafeAction OnMainCityHint; //主场景提示

                public static SafeAction<float> OnBlockTaskTouch;

                public static SafeAction<int, bool> OnLockMainIcon; //主界面是否带锁

                public static SafeAction<int> OnLockMainIconComplete;

                public static SafeAction<bool> OnNewPoliceEffect;

                public static SafeAction OnOpenBottomButton;

                //首次进入到主界面
                public static SafeAction OnFirstTimeEnterGame;

                //自动领取奖励
                public static SafeAction OnReceiveRewardAuto;

                /// <summary>
                /// 合成材料获得
                /// </summary>
                public static SafeAction Listen_OnCombinePropCollected;

                //进入界面背景
                public static SafeAction<bool> OnMaskBGVisible;
            }

            public static class UI_Shop_Event
            {
                public static SafeAction<long, Transform> OnChooseItem;

                public static SafeAction<ShopItemData> Listen_ShowBuyEnergyPopView;
            }

            public static class UI_Select_Police_Event
            {
                public static SafeAction<int> OnIndexChosen;
            }


            public static class UI_SLots_Events
            {
                public static SafeAction OnAgain;
                public static SafeAction OnOK;
            }

            public static class UI_Activity_Event
            {
                public static SafeAction<int> OnTweenFinish;
                public static SafeAction<int> OnClick;
                public static SafeAction<int> OnChooseFinish;

                public static SafeAction<int> OnDragTexure;
            }

            public static class UI_Achievement_Event
            {
                public static SafeAction<long, int> OnReceiveData; //领取奖励
                public static SafeAction OnReflashAchievement;

                public static SafeAction<long, bool> AchievementStateChange;
            }

            public static class UI_PlayerTitle_Event
            {
                public static SafeAction<TitleComponent, long> OnChoose;
                public static SafeAction OnClose; //关闭子界面 
                public static SafeAction OnOpen; //开启子界面
            }
            public static class UI_Friend_Event
            {
                public static SafeAction<FriendReqType> OnRefreshFriendPage; //刷新好友相关界面

                public static SafeAction<long, ENUM_INFO_CONTROL> OnInfoChanged;
                public static SafeAction<long, ENUM_APPLICATION_CONTROL> OnApplicationChanged;
                public static SafeAction<long, ENUM_GIFT_CONTROL> OnGiftChanged;
                public static SafeAction<long> Listen_AddFriend;

                public static SafeAction<long> Tell_add_recommend_firend_ok;
                public static SafeAction<List<long>> Listen_add_recommend_friend;
                public static SafeAction<long> Listen_check_recommend_info;

                public static SafeAction<ENUM_FRIEND_VIEW_TYPE> Listen_ShowView;

            }

            public static class UI_Pause_Event
            {
                public static SafeAction OnQuit;
                public static SafeAction OnClosePauseFrame;
                public static SafeAction<bool> OnHidePauseFrame;
            }

            public static class UI_SignIn_Event
            {
                public static SafeAction OnSignIn;
            }

            public static class UI_StartCartoon_Event
            {
                public static SafeAction OnNext;
                public static SafeAction OnFinish;
                public static SafeAction OnNextCapter;
                public static SafeAction<bool, int> OnNextBtnVisible;

                public static SafeAction OnSkipCurrentCapter;

                public static SafeAction OnNextClick; //点击一下

                public static SafeAction OnNextKeyCapter; //调到关键页签
                public static SafeAction<int> OnPageChange;
                //新手引导漫画  new
                public static SafeAction<long> OnNextGameStart;
                public static SafeAction<int> OnChooseHead;

                public static SafeAction OnResetPage;
            }

            public static class UI_Loading_Event
            {
                //public static SafeAction OnSceneBaseLoadOver;
                //public static SafeAction OnSceneAssetLoadOver;

                public static SafeAction<int, bool> OnLoadingState; //阶段   是否完成
                public static SafeAction<int, float> OnLoadingStageTime; //当前阶段所花费的时间
                public static SafeAction<int, float, float> OnChangeStageTime;
                public static SafeAction OnLoadingOver;

                public static SafeAction OnStartLoading;

                public static SafeAction OnStartLoadingComplete;
            }

            public static class UI_Jigsaw_Event
            {
                public static SafeAction<bool> OnPause;
                public static SafeAction<bool> OnJigsawFinish;
            }

            public static class UI_POPUP_Event
            {
                public static SafeAction OnConfirm;
            }

            public static class UI_PushGift_Event
            {
                public static SafeFunc<ENUM_PUSH_GIFT_BLOCK_TYPE, bool> OnGo;
            }

            public static class UI_WaveTip_Event
            {
                public static SafeAction<WaveTipData> OnShowTips;
            }

            public static class UI_Bonus_Pop_View_Event
            {
                public static SafeAction<EUNM_BONUS_POP_VIEW_TYPE> Listen_OnShow;

                public static SafeAction<EUNM_BONUS_POP_VIEW_TYPE> Tell_OnCache;
                public static SafeAction Tell_OnShow;
                public static SafeAction<bool> Tell_OnBlock;
                public static SafeAction<bool> BonusPopViewVisible;
            }

            public static class UI_FB_Event
            {
                public static SafeAction Listen_FbLoginStatusChanged;
            }

            public static class UI_GameMain_Event
            {
#if OFFICER_SYS
                public static Func<long, Transform> GetHeroItemById;
#endif
                public static Func<long, Transform> GetPropItemById;
                public static SafeAction<long, bool> LockFindItem;


            }

            public static class UI_EventGame_Event
            {
#if OFFICER_SYS
                public static SafeAction<int> Tell_AddAnOfficer;

                public static SafeAction<int> Listen_NeedAnOfficer;
                public static SafeAction<long> Listen_RemoveAnOfficer;
#endif
                public static SafeAction<long, int> EventNormalState;
            }

            public static class UI_Archives_Event
            {
                /// <summary>
                /// 点击人物对话
                /// </summary>
                public static SafeAction<Vector2> OnClickChatPerson;
                public static SafeAction<bool> OnForbidNpcListScroll;
                public static SafeAction OnCloseNpcDetail;
            }

            public static class UI_Reason_Event
            {
                public static SafeAction<bool> OnCheckReasonItem;
                public static Func<Vector3> OnGetReasonItemPosition;
                public static SafeAction<string, bool> OnReasonIconVisible;
            }

            public static class UI_Scan_Event
            {
                public static SafeAction Listen_ResumeGame;
                /// <summary>
                /// long:clue id
                /// 找到了一个线索
                /// </summary>
                public static SafeAction<long> Listen_FindClue;

                /// <summary>
                /// long : anchor view unique id
                /// 删除种植在场景中的线索
                /// </summary>
                public static SafeAction<long> Listen_RemoveClueAnchor;

                /// <summary>
                /// 回收详情
                /// </summary>
                public static SafeAction<ClueDetailView> Listen_RecycleDetailItemView;


                /// <summary>
                /// long:clue id
                /// 展示飞行图标
                /// </summary>
                public static SafeAction<long> Listen_ShowFlyIconItemView;


                /// <summary>
                /// 回收飞行图标
                /// </summary>
                public static SafeAction<FlyIconItemView> Listen_RecycleFlyIconItemView;



                /// <summary>
                /// 回收上漂体力数字
                /// </summary>
                public static SafeAction<FlyNumItemView> Listen_RecycleFlyVitNumItemView;


                /// <summary>
                /// 增加寻找线索的进度
                /// long:clue id
                /// </summary>
                public static SafeAction<long> Listen_AddClueProgress;

                /// <summary>
                /// 增加体力特效飞行结束
                /// 
                /// </summary>
                public static SafeAction<FlyVitEffectItemView> Listen_VitEffectFinishFly;



                /// <summary>
                /// 显示体力奖励页面
                /// </summary>
                public static SafeAction Listen_ShowReward;


                /// <summary>
                /// 增加体力
                /// </summary>
                public static SafeAction<int> Tell_AddVit;


                /// <summary>
                /// 体力震动结束
                /// </summary>
                public static SafeAction Listen_ShakeFinished;
            }

            public static class UI_Presuade_Event
            {
                public static SafeAction<long> OnChooseId;
                public static SafeAction<int> OnRecordByIndex;
                public static SafeAction<bool> OnNoticProgressFillEffect;

                public static Func<bool> OnRecordComplete;

            }

            public static class UI_Scroll3D_Event
            {
                public static SafeAction OnScrollStart;
                public static SafeAction<int> OnScrollEnd;
                public static SafeAction OnGetCurrentOverIndex;
            }

            public static class UI_SkyEye_Event
            {
                public static SafeAction<string, string> OnOpenIconDetail;
                public static SafeAction<long> OnSkyEyeCompleteById;
                //public static SafeAction<>
            }

            public static class UI_Common_Event
            {
                public static SafeAction<bool> OnCommonUIVisible;
            }
        }
        public static class UI_Guid_Event
        {
            public static SafeAction<GUIFrame> OnOpenUI;
            public static SafeAction<GUIFrame> OnCloseUI;
            public static SafeAction OnNextGuid;
            public static SafeAction<string> OnpenComponentByName;

            public static SafeAction<long> OnGuidNewEnd; //外部触发当前引导完成
            public static SafeAction<long> OnGuidNewNext;
            public static SafeAction<long> OnGuidNewComplete; //引导完成回调
            public static SafeAction<string, SeekerGame.NewGuid.NodeStatus> OnGuidNewNodeStatusChange;
            //
            public static SafeAction<List<Vector2[]>, List<MaskEmptyType>, MaskAnimType, string, bool> OnShowMask;
            public static SafeAction<string, int, Vector2, Vector2, int> OnTalkEvent; //内容  朝向  起始位置  结束位置
            public static SafeAction<Vector2, bool> OnMaskClick; //遮罩点击 世界坐标
            public static SafeAction<bool> OnMaskTalkVisible;
            public static SafeAction OnMaskTickComplete;
            public static SafeAction<bool> OnEnableClick;
            public static SafeAction<bool> OnUIEnableClick;
            public static SafeAction OnTalkClick;

            public static SafeAction<Vector2> OnEventClick;

            public static Func<bool> OnGetUIMaskStatus;
            public static SafeAction<bool> OnMaskEnableClick; //遮罩是否能点击
            public static SafeAction<bool> OnMaskLongClick;
            //特效
            public static SafeAction<long, string, Vector2, Vector2, float> OnLoadEffect;
            public static SafeAction<long, bool> OnRemoveEffect;
            public static SafeAction<bool> OnClearMaskEffect; //清除遮罩特效
            public static Func<long, GameUIEffect> OnGetMaskEffect; //
            //局内
            public static SafeAction OnSceneReady;
            public static SafeAction OnSceneStart;
            public static SafeAction OnSceneShowResult; //展示结束界面
            public static SafeAction<bool> OnFindSceneResult; //寻物结果

            //警员滑动
            public static SafeAction<long> OnSelectPolice;

            //主界面解锁操作
            public static SafeAction OnMainIcon;
            public static SafeAction<int> OnMainIconUnLock;
            public static SafeAction<int> OnMainIconUnLockComplete; //解锁完成  ----new use
            public static SafeAction<int> OnMainIconStartMove; //底部位移
            public static SafeAction<int> OnMainIconMoveComplete; //位移完成

            //开场漫画引导
            public static SafeAction OnStartGuidCartoonOver; //开场漫画结束
            public static SafeAction OnStartGuidCartoonNext; //开场漫画下一个存储点
            public static SafeAction<string, string, Vector3> OnStartCartoonSelectHead;

            public static SafeAction<string, string> OnOnpenFrame; //打开界面

            public static SafeAction<bool> OnSeekOpenClose; //选择警员打开关闭

            public static SafeAction OnClearGuid;

            //拼图引导
            public static SafeAction<int> OnJigsawEnd; //0 表示操作结束  1表示拼图结束
            public static SafeAction<bool> OnEnablePause;

            #region 1123新增修改
            public static SafeAction<Vector4, int> OnReflashCircleMask;
            public static Func<int, Vector4> OnGetCurrentMaskInfo;
            public static SafeAction<Vector2> OnMaskClickEvent; //遮罩点击任何地方
            #endregion

            public static SafeAction<bool> OnNewPolice;
            public static Func<long, Transform> GetPoliceItemById;
            //public static SafeAction<string>
        }

        public static class Skill_Event
        {
            public static SafeAction<long> OnSkillStart;
            public static SafeAction<long> OnSkillFinish; //技能释放结束
            public static SafeAction<long> OnSkillReset; //重置技能
            public static SafeAction<long> OnSkillCDOver; //技能冷却结束
            public static SafeAction<long> OnSkillError; //技能释放错误
            public static SafeAction<long, bool> OnHeroSKillResult;

            public static SafeAction OnUnlimitedVitSkillStart; //无限体力技能释放
        }

        public static class BigWorld_Event
        {
            public static SafeAction<string> OnClick;
            public static SafeAction<string> OnUnLock;
            public static SafeAction<long> OnBuildUnLockComplete;

            public static SafeAction<long, string, bool> OnShowBuildTopUI;
            public static SafeAction<BuidAchorData, GameObject, bool> OnBuildTopUIByObj;
            public static SafeAction<long, string> OnHideBuidTopUI;
            public static SafeAction<long, int> OnReflashBuildIconStatus; //刷新图标
            public static SafeAction<long, int> OnReflashBuildStatus;

            public static SafeAction OnReflashScreen;

            public static SafeAction<int, float> OnReflashTime; //刷新时间

            public static SafeAction<bool> OnCameraCollider;

            public static SafeAction<long> OnCameraMove;

            public static SafeAction OnReflashBuidTask;

            public static SafeAction<BuildTopIconComponent, bool> OnBuildTopActive;

            public static SafeAction<long> OpenBuildTopByHead5NumInSceneID;

            public static SafeAction OnClickScreen; //点击屏幕

            public static Func<long, bool> OnCheckBuildStatusByID;

            //<大地图，支线、池任务，图标
            public static SafeFunc<Dictionary<string, GameObject>> Listen_GetAllBranchAnchors;
            public static SafeFunc<Dictionary<string, GameObject>> Listen_GetAllPoolAnchors;

            public static SafeAction<UITaskOnBuildData> Listen_ShowTaskOnBuild;
            //>

            public static SafeAction OnReflashBigWorld;

        }

        /// <summary>
        /// 系统相关事件
        /// </summary>
        public static class System_Events
        {
            public static SafeAction<string> SetLoadingTips;

            /// <summary>
            /// 开启/关闭音效
            /// </summary>
            public static SafeAction<bool> EnableSoundEvent;

            /// <summary>
            /// 开启/关闭音乐
            /// </summary>
            public static SafeAction<bool> EnableMusicEvent;

            /// <summary>
            /// 开启/关闭购买
            /// </summary>
            public static SafeAction<bool> EnablePurchaseEvent;

            //背包数据更新
            //public static SafeAction<Dictionary<long, int>> OnBagAddItems;

            /// <summary>
            /// 播放主背景音乐
            /// </summary>
            public static Action<bool> PlayMainBGM;

        }

        /// <summary>
        /// 网络事件
        /// </summary>
        public static class NetWorkEvents
        {
            /// <summary>
            /// 发送同步消息(有UI阻塞)
            /// </summary>
            public static SafeAction<IMessage> SendMsg;


            /// <summary>
            /// 发送同步消息(有UI阻塞,但没有重发)
            /// </summary>
            public static SafeAction<IMessage> SendHalfSyncMsg;

            /// <summary>
            /// 发送异步消息
            /// </summary>
            public static SafeAction<IMessage> SendAsyncMsg;



        }


        /// <summary>
        /// 场景事件
        /// </summary>
        public static class SceneEvents
        {
            /// <summary>
            /// 进入场景
            /// </summary>
            public static SafeAction OnEnterScene;

            /// <summary>
            /// 离开场景
            /// </summary>
            public static SafeAction OnLeaveScene;

            /// <summary>
            /// 从场景移出Entity
            /// </summary>
            public static SafeAction<EntityBase> RemoveEntity;

            /// <summary>
            /// 释放Entity
            /// </summary>
            public static SafeAction<EntityBase> FreeEntityFromScene;

            /// <summary>
            /// 场景添加特效
            /// </summary>
            public static SafeFunc<string, EffectEntity> AddLazyEffectToScene;

            /// <summary>
            /// 设置场景类型  小节点还是多摄像机
            /// </summary>
            public static SafeAction<int> SetSceneType;

            /// <summary>
            /// 退出当前类型  进入到普通场景
            /// </summary>
            public static SafeAction<int> OnClickQuitScene;

            public static SafeAction<string> OnEnterTalkScene;
            public static SafeAction OnLeaveTalkScene;

            public static SafeAction<SceneItemEntity, bool> OnSceneExhibitTips;
            public static SafeAction<SceneItemEntity, bool> OnSceneExhibitTipsGuide;
            public static SafeAction<bool> OnScenePropWinVisible;

            public static SafeAction OnLeaveSceneComplete; //

            public static Func<string, bool> EntityInCurrentCamera;

            public static SafeAction<string> OnQuitToMainCamera;



            /// <summary>
            /// 进入寻物 场景
            /// </summary>
            public static SafeAction<SCSceneEnterResponse> OnEnterSeekScene;



            /// <summary>
            /// 进入寻物 场景
            /// </summary>
            public static SafeAction Listen_EnterExhibitionHallScene;

            //public static SafeFunc<long, Vector3> Listen_GetExhibitPos;
            //public static SafeFunc<long, Quaternion> Listen_GetExhibitRotate;
        }

        /// <summary>
        /// 游戏玩法内事件
        /// </summary>
        public static class MainGameEvents
        {
            //找到物品
            public static SafeAction<SceneItemEntity> OnPickedSceneObject;

            /// <summary>
            /// 游戏内Tick事件(剩余时间)
            /// </summary>
            public static SafeAction<int> OnGameTimeTick;

            /// <summary>
            /// 错误点击(错误的时间)
            /// </summary>
            public static SafeAction<float, Vector2> OnErrorTouch;

            /// <summary>
            /// 惩罚 (惩罚时间,gesture,游戏剩余时间)
            /// </summary>
            public static SafeAction<float, Vector2, float> OnPunish;

            /// <summary>
            /// 游戏状态变化
            /// </summary>
            public static SafeAction<SceneBase.GameStatus> OnGameStatusChange;

            /// <summary>
            /// 获取所有任务物品列表
            /// </summary>
            public static Func<List<EntityBase>> GetSceneTaskItemEntityList;

            /// <summary>
            /// 获取找到的Entity列表
            /// </summary>
            public static Func<List<SceneItemEntity>> GetFoundTaskItemEntityList;

            /// <summary>
            /// 推送下一个待寻找的物体
            /// </summary>
            public static Action<List<SceneItemEntity>> PushFindingTaskEntities;

            /// <summary>
            /// 请求待寻找物体列表
            /// </summary>
            public static Func<int, List<SceneItemEntity>> RequestNextFindingTaskEntities;

            /// <summary>
            /// 获取当前正在寻找物体列表
            /// </summary>
            public static Func<List<SceneItemEntity>> GetCurrentFindingEntities;

            /// <summary>
            /// 游戏结束事件
            /// </summary>
            public static SafeAction<SceneBase.GameResult> OnGameOver;


            /// <summary>
            /// 强制游戏结束事件,断网恢复后调用
            /// </summary>
            public static SafeAction<SceneBase.GameResult> OnForceGameOver;


            /// <summary>
            /// 开启、关闭场景特殊效果(雾,黑天)
            /// </summary>
            public static SafeAction<bool> EnableSceneSpecialEffect;

            /// <summary>
            /// 获取提示场景中物体列表
            /// </summary>
            public static Func<int, List<SceneItemEntity>> GetSceneItemEntityList;

            /// <summary>
            /// 根据物体ID获取物体
            /// </summary>
            public static Func<long, SceneItemEntity> GetSceneItemEntityByID;

            /// <summary>
            /// 发送提示物件通知
            /// </summary>
            public static SafeAction<List<SceneItemEntity>> NotifyHintSceneItemEntityList;

            /// <summary>
            /// 发送提示物件通知 摄像机不跟随
            /// </summary>
            public static SafeAction<List<SceneItemEntity>> NotifyHintSceneItemEntityListNoCameraFollow;

            /// <summary>
            /// 发送特效提示物件
            /// </summary>
            public static Action<bool, long> OnStreamerHint;

            /// <summary>
            /// 请求提示物件 数量
            /// </summary>
            public static Action<int> RequestHintSceneItemList;

            /// <summary>
            /// 请求提示物件  摄像机不跟随
            /// </summary>
            public static Action<int> RequestHintSceneItemNoCameraFollow;

            /// <summary>
            /// 自动拾取场景物体
            /// </summary>
            public static Action<int> AutoPickSceneItems;

            /// <summary>
            /// 反词模式消除
            /// </summary>
            public static SafeAction<int, long> RevertItemNameToNormal;

            //增加时间<true 固定时间  false 百分比>
            public static SafeAction<bool, float> AddGameTime;

#if OFFICER_SYS
            public static Func<IList<long>> RequestHeroList;
#endif

            /// <summary>
            /// 按时间提示场景中物体 (扫描时间,提示物体数量)
            /// </summary>
            public static SafeAction<float, int> ScanAndNotifyHintItems;

            /// <summary>
            /// 物件是否已经被找到(EntityID,是否被找到)
            /// </summary>
            public static Func<string, bool> IsSceneItemFound;

            /// <summary>
            /// 游戏开始
            /// </summary>
            public static SafeAction OnStartGame;

            /// <summary>
            /// 游戏初始化完成
            /// </summary>
            public static SafeAction OnGameInitComplete;

            public static SafeAction<Vector2> OnSceneClick; //局内点击

            public static SafeAction<long, int, int> OnPropUseTips; //提示使用道具  1为箭头

            public static SafeAction<long, int> OnGuidPropUseTips; //引导提示 1为箭头

            public static SafeAction<long, bool> OnForbidProp; //切换摄像机禁用道具

            public static SafeAction<long, bool> OnAlwaysForbidProp; //长期禁用道具

            //导弹飞行
            public static SafeAction<Vector3, Vector3, Action> OnMissileFly;

            //获取导弹起始位置
            public static SafeAction<Vector3, Action> OnGetMissileStartPos; //

            public static SafeAction<GameObject> OnCameraMove;

            public static SafeAction OnResetFingerTips;

            public static SafeAction<int, HedgehogTeam.EasyTouch.Gesture, float> OnCameraZoomOrRotation;

            public static SafeAction<bool> OnFingerForbidden;

            public static Func<Vector3> GetCameraBound;

            public static SafeAction<bool> OnScenePanelVisible;

            public static SafeAction OnResetArrowTipsTime;

            public static SafeAction<Action> OnCheckSceneCameraPoint;

            //清空摄像机状态
            public static SafeAction OnClearCameraStatus;

        }


        public static class BuyEvent
        {
            public static SafeAction<MarkeBuyResponse> OnShopRes;
            public static SafeAction<long, int, int, ShopType> OnShopReq;
        }

        /// <summary>
        /// 任务系统相关事件
        /// </summary>
        public static class TaskEvents
        {
            /// <summary>
            /// 接收到新的任务
            /// </summary>
            public static SafeAction<long> OnAcceptNewTask;


            /// <summary>
            /// 讲下一个任务提前创建出来，用于显示。
            /// </summary>
            public static SafeAction<long> OnAcceptNewTaskLocal;

            /// <summary>
            /// 完成任务并领取奖励
            /// </summary>
            public static SafeAction<int, TaskBase> OnCompletedTask;

            //任务完成
            public static SafeAction<TaskBase> OnTaskFinish;

            public static SafeAction<int> OnSyncedTaskList;

            public static SafeAction<TaskBase> OnReceiveTask;

            public static SafeAction<bool> OnBlockSyncTask;


            public static SafeAction<NormalTask, Action<long>> OnTryShowCollectionTaskDetail;
            public static SafeAction<NormalTask> OnCollectTaskReward;
            public static SafeAction<NormalTask, Action<long>> OnShowCollectionDetail;
        }

        /// <summary>
        /// 邮件系统相关事件
        /// </summary>
        public static class MailEvents
        {
            /// <summary>
            /// 邮件列表变化
            /// </summary>
            public static SafeAction<MailType, List<Mail>> OnMailListChanged;

            /// <summary>
            /// 阅读邮件
            /// </summary>
            public static SafeAction<long> OnReadMail;

        }

        /// <summary>
        /// 章节系统相关事件
        /// </summary>
        public static class ChapterEvents
        {
            /// <summary>
            /// 章节信息更新
            /// </summary>
            public static SafeAction<ChapterInfo> OnChapterInfoUpdated;

            /// <summary>
            /// 解锁新章节
            /// </summary>
            public static SafeAction<ChapterInfo> OnUnlockChapter;

            /// <summary>
            /// 章节完成
            /// </summary>
            public static SafeAction<ChapterInfo> OnDoneChapter;

            /// <summary>
            /// 打开档案指定页签
            /// </summary>
            public static SafeAction<int> OnpenChapterUIByIndex;

            /// <summary>
            /// 章节下载成功
            /// </summary>
            public static SafeAction<ChapterInfo> OnChapterDownloadFinish;
        }

        /// <summary>
        /// 玩家个人信息相关事件
        /// </summary>
        public static class PlayerEvents
        {
            /// <summary>
            /// 请求玩家最新的个人信息
            /// </summary>
            public static Action RequestLatestPlayerInfo;
            public static Action Listen_SyncTitle;
            public static SafeFunc<SafeAction, int, bool> OnExpChanged;

            /// <summary>
            /// 刷新成就
            /// </summary>
            public static SafeAction RequestRecentAhievement;
        }


        public static class RedPointEvents
        {
            public static SafeAction Sys_OnRefreshByPlayerPrefs;

            //<已读新消息
            public static SafeAction Sys_OnNewEmailReadedEvent;
            public static SafeAction Sys_OnNewFriendReadedEvent;
            public static SafeAction Sys_OnNewApplyReadedEvent;
            public static SafeAction Sys_OnNewGiftReadedEvent;
            public static SafeAction Sys_OnNewActivityReadedEvent;
            public static SafeAction Sys_OnNewAchievementReadedEvent;
            public static SafeAction Sys_OnNewNoticeReadedEvent;

            public static SafeAction<int> Sys_OnNewChapterEvent;
            //>

            //<新消息通知
            public static SafeAction User_OnNewEmailEvent;
            public static SafeAction<bool> User_OnNewFriendEvent;
            public static SafeAction User_OnNewActivityEvent;
            public static SafeAction User_OnNewAchievementEvent;
            public static SafeAction User_OnNewNoticeEvent;
#if OFFICER_SYS
            public static SafeAction<bool> User_OnNewPoliceEvent;
#endif

            public static SafeAction<int> User_OnNewChapterEvent;
            public static SafeAction<bool> User_OnNewChapterBannerEvent;
            //>
        }

        public static class IAPEvents
        {
            public static SafeAction<long> Sys_BuyProductEvent;
            public static SafeAction<string> Sys_BuyProductIOSEvent;
            public static SafeFunc<long, string> Sys_GetPriceEvent;
            public static SafeFunc<string, string> Sys_GetPriceIOSEvent;

            public static SafeFunc<long, float> Sys_GetUSDPriceEvent;


            public static SafeAction<long> OnTransactionDone;
        }

        public static class NetworkWatchEvents
        {
            public static SafeAction NetPass;
            public static SafeAction NetError;
        }
    }
}