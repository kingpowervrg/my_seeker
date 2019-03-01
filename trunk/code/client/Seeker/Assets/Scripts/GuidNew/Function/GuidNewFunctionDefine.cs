using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SeekerGame.NewGuid
{
    public class GuidNewFunctionDefine
    {
        #region 前面一长串
        public const string LoadSingleMask = "LoadSingleMask";
        public const string StartCartoon = "StartCartoon";
        public const string PreFuncId = "PreFuncId";
        public const string ShowBuildTop = "ShowBuildTop";
        public const string MainUIMoveOver = "MainUIMoveOver";
        public const string FrameOpen = "FrameOpen";
        public const string HideNode = "HideNode";
        public const string FuncFrameOpen = "FuncFrameOpen";
        public const string PreHideNode = "PreHideNode";
        public const string LoadTalk = "LoadTalk"; //加载对话
        public const string CloseTalk = "CloseTalk";
        public const string LoadEffect = "LoadEffect";
        public const string RemoveEffect = "RemoveEffect";
        public const string SelectPoliceDrag = "SelectPoliceDrag";
        public const string ResetSelectPolice = "ResetSelectPolice";
        public const string LoadMutilMask = "LoadMutilMask"; //添加多个遮罩
        public const string PreProgressComplete = "PreProgressComplete";
        public const string MaskEnable = "MaskEnable";
        //场景相关
        public const string SceneStart = "SceneStart";
        public const string ScenePause = "ScenePause";
        public const string SceneTips = "SceneTips"; //场景提示
        public const string SceneHintItem = "SceneHintItem"; //场景提示物件
        public const string SceneFindComplete = "SceneFindComplete";
        public const string SceneShowResult = "SceneShowResult";
        public const string SceneInit = "SceneInit";
        public const string BuildUnLock = "BuildUnLock"; //场景解锁
        public const string ScenePropHint = "ScenePropHint";

        public const string LoadEffectByFrame = "LoadEffectByFrame";
        public const string DelayTime = "DelayTime"; //延时
        public const string ProgressComplete = "ProgressComplete";
        public const string EnableClick = "EnableClick";
        public const string EnableUIClick = "EnableUIClick";

        public const string LoadMainIcon = "LoadMainIcon";//主界面Icon
        public const string MainBannerUnLock = "MainBannerUnLock";
        public const string FrameCloseListener = "FrameCloseListener";
        public const string SkillComplete = "SkillComplete";
        public const string SetNodeStatus = "SetNodeStatus";
        public const string CheckNodeStatus = "CheckNodeStatus";

        public const string BannerOpen = "BannerOpen";
        public const string OpenByAlpha = "OpenByAlpha";

        public const string GuidNewComplete = "GuidNewComplete"; //结束该引导
        public const string TaskComplete = "TaskComplete";

        public const string AchievementComplete = "AchievementComplete";
        public const string GameEntryUIOpen = "GameEntryUIOpen";

        public const string SelectPoliceParam = "SelectPoliceParam";
        public const string FrameItemOpen = "FrameItemOpen";
        #endregion

        #region 拼图
        public const string JigsawOperateEnd = "JigsawOperateEnd";
        #endregion

        #region 场景内
        public const string PreOnEnterScene = "PreOnEnterScene";

        public const string SceneFingerTip = "SceneFingerTip";
        public const string SceneBlackEffect = "SceneBlackEffect";
        #endregion

        #region 1122新增修改
        //公共相关
        public const string CommonParam = "CommonParam";
        public const string AddItemToUser = "AddItemToUser"; //送道具等等
        public const string LevelUp = "LevelUp";
        public const string ChangeGameStateToLogin = "ChangeGameStateToLogin"; //清空单机数据进入到登陆界面
        public const string LeaveScene = "LeaveScene"; //离开场景
        public const string SaveProgress = "SaveProgress"; //保存当前进度
        public const string EffectMaskByFrame = "EffectMaskByFrame"; //特效提示
        public const string SetBasePlayInfo = "SetBasePlayInfo"; //设置个人基本数据
        //UI相关
        public const string UI_Open = "UI_Open";
        public const string UIIsHide = "UIIsHide";
        public const string OpenComponent = "OpenComponent";
        //场景相关
        public const string EnterSceneLocal = "EnterSceneLocal";
        public const string LoadCircleMaskByPosition = "LoadCircleMaskByPosition";
        public const string LoadCircleMaskByItemID = "LoadCircleMaskByItemID";
        public const string MoveCircleMaskByPostion = "MoveCircleMaskByPostion";
        public const string OnStartGame = "OnStartGame";
        public const string CircleMaskClick = "CircleMaskClick";
        public const string SceneExhibitEffectTips = "SceneExhibitEffectTips"; //场景特效提示
        public const string SceneComplete = "SceneComplete"; //场景特效提示
        public const string SceneEntityHintRule = "SceneEntityHintRule";
        public const string ScenePropArrow = "ScenePropArrow"; //箭头提示
        public const string SceneListenPropFind = "SceneListenPropFind"; //监听物品是否被找到
        public const string ForbidProp = "ForbidProp"; //监听物品是否被找到
        //资源相关
        public const string ResourceLoadRes = "ResourceLoadRes";
        //遮罩相关
        public const string MaskVisible = "MaskVisible";
        //任务相关
        public const string RefreshTaskById = "RefreshTaskById";
        #endregion

        #region 1213修改
        public const string EffectIndicateCircle = "EffectIndicateCircle";
        public const string SceneUnLockItemWord = "SceneUnLockItemWord";
        public const string ScenePropHintRule = "ScenePropHintRule";
        public const string UnLockMainIcon = "UnLockMainIcon";
        public const string LoopTaskComplete = "LoopTaskComplete";
        public const string EventNormalState = "EventNormalState";
        public const string MutilEffectMaskByFrame = "MutilEffectMaskByFrame";
        public const string ListenNewPolice = "ListenNewPolice";
        public const string NewPoliceMainPanel = "NewPoliceMainPanel";
        public const string HintNewPolice = "HintNewPolice";
        public const string CompleteGuidByGroup = "CompleteGuidByGroup";
        public const string OpenGameEntryButton = "OpenGameEntryButton";
        #endregion
    }
}
