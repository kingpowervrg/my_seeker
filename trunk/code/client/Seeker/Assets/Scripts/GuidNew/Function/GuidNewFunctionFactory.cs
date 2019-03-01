using System;
using System.Collections.Generic;
using UnityEngine;

namespace SeekerGame.NewGuid
{
    public class GuidNewFunctionFactory : Singleton<GuidNewFunctionFactory>
    {
        private Dictionary<string, Type> funcTypes = new Dictionary<string, Type>();

        public GuidNewFunctionFactory()
        {
            #region 前面一大串
            funcTypes.Add(GuidNewFunctionDefine.LoadSingleMask, typeof(GuidNewFuncLoadMask));
            funcTypes.Add(GuidNewFunctionDefine.StartCartoon, typeof(GuidNewPreFuncStartCartoon));
            funcTypes.Add(GuidNewFunctionDefine.PreFuncId, typeof(GuidNewPreFuncId));
            funcTypes.Add(GuidNewFunctionDefine.ShowBuildTop, typeof(GuidNewFuncShowBuildTop));
            funcTypes.Add(GuidNewFunctionDefine.MainUIMoveOver, typeof(GuidNewPreFuncMainUIMoveOver));
            funcTypes.Add(GuidNewFunctionDefine.HideNode, typeof(GuidNewFuncHideNode));
            funcTypes.Add(GuidNewFunctionDefine.FrameOpen, typeof(GuidNewPreFuncFrameOpen));
            funcTypes.Add(GuidNewFunctionDefine.FuncFrameOpen, typeof(GuidNewFuncFrameOpen));
            funcTypes.Add(GuidNewFunctionDefine.PreHideNode, typeof(GuidNewPreFuncHideOpen));
            funcTypes.Add(GuidNewFunctionDefine.LoadTalk, typeof(GuidNewFuncLoadTalk));
            funcTypes.Add(GuidNewFunctionDefine.CloseTalk, typeof(GuidNewFuncCloseTalk));
            funcTypes.Add(GuidNewFunctionDefine.LoadEffect, typeof(GuidNewFuncLoadEffect));
            funcTypes.Add(GuidNewFunctionDefine.RemoveEffect, typeof(GuidNewFuncRemoveEffect));
            funcTypes.Add(GuidNewFunctionDefine.SelectPoliceDrag, typeof(GuidNewFuncSelectPoliceDrag));
#if OFFICER_SYS
            funcTypes.Add(GuidNewFunctionDefine.ResetSelectPolice, typeof(GuidNewFuncResetSelectPolice));
#endif
            funcTypes.Add(GuidNewFunctionDefine.LoadMutilMask, typeof(GuidNewFuncLoadMutilMask));
            funcTypes.Add(GuidNewFunctionDefine.PreProgressComplete, typeof(GuidNewPreFuncProgressComplete));
            funcTypes.Add(GuidNewFunctionDefine.MaskEnable, typeof(GuidNewFuncMaskEnable));
            //Scene
            funcTypes.Add(GuidNewFunctionDefine.SceneStart, typeof(GuidNewFuncSceneStart));
            funcTypes.Add(GuidNewFunctionDefine.ScenePause, typeof(GuidNewFuncScenePause));
            funcTypes.Add(GuidNewFunctionDefine.SceneTips, typeof(GuidNewFuncSceneErrorTouch));
            funcTypes.Add(GuidNewFunctionDefine.SceneHintItem, typeof(GuidNewFuncSceneHintItem));
            funcTypes.Add(GuidNewFunctionDefine.SceneShowResult, typeof(GuidNewFuncSceneSucessResult));
            funcTypes.Add(GuidNewFunctionDefine.SceneInit, typeof(GuidNewFuncSceneInit));
            funcTypes.Add(GuidNewFunctionDefine.BuildUnLock, typeof(GuidNewFuncBuildUnLock));
            funcTypes.Add(GuidNewFunctionDefine.ScenePropHint, typeof(GuidNewFuncScenePropHint));

            funcTypes.Add(GuidNewFunctionDefine.SceneFindComplete, typeof(GuidNewFuncSceneFindComplete));
            funcTypes.Add(GuidNewFunctionDefine.LoadEffectByFrame, typeof(GuidNewFuncLoadEffectByFrame));
            funcTypes.Add(GuidNewFunctionDefine.DelayTime, typeof(GuidNewFuncDelayTime));
            funcTypes.Add(GuidNewFunctionDefine.ProgressComplete, typeof(GuidNewFuncProgressComplete));
            funcTypes.Add(GuidNewFunctionDefine.EnableClick, typeof(GuidNewFuncEnableClick));
            funcTypes.Add(GuidNewFunctionDefine.EnableUIClick, typeof(GuidNewFuncUIEnableClick));

            funcTypes.Add(GuidNewFunctionDefine.LoadMainIcon, typeof(GuidNewFuncLoadMainIcon));
            funcTypes.Add(GuidNewFunctionDefine.MainBannerUnLock, typeof(GuidNewFuncMainBannerUnLock));
            funcTypes.Add(GuidNewFunctionDefine.GuidNewComplete, typeof(GuidNewFuncGuidNewComplete));
            funcTypes.Add(GuidNewFunctionDefine.SkillComplete, typeof(GuidNewFuncSkillComplete));
            funcTypes.Add(GuidNewFunctionDefine.SetNodeStatus, typeof(GuidNewFuncSetNodeStatus));
            funcTypes.Add(GuidNewFunctionDefine.CheckNodeStatus, typeof(GuidNewFuncCheckNodeStatus));

            funcTypes.Add(GuidNewFunctionDefine.BannerOpen, typeof(GuidNewFuncBannerOpen));
            funcTypes.Add(GuidNewFunctionDefine.OpenByAlpha, typeof(GuidNewFuncControlOpenByAlpha));
            funcTypes.Add(GuidNewFunctionDefine.FrameCloseListener, typeof(GuidNewFuncFrameCloseListener));
            funcTypes.Add(GuidNewFunctionDefine.TaskComplete, typeof(GuidNewFuncTaskComplete));

            funcTypes.Add(GuidNewFunctionDefine.GameEntryUIOpen, typeof(GuidNewFuncGameEntryUIOpen));
            funcTypes.Add(GuidNewFunctionDefine.AchievementComplete, typeof(GuidNewFuncAchievementComplete));
#if OFFICER_SYS
            funcTypes.Add(GuidNewFunctionDefine.SelectPoliceParam, typeof(GuidNewFuncSelectPoliceParam));
#endif
            funcTypes.Add(GuidNewFunctionDefine.FrameItemOpen, typeof(GuidNewFuncFrameItemOpen));
            #endregion

            #region 拼图
            funcTypes.Add(GuidNewFunctionDefine.JigsawOperateEnd, typeof(GuidNewFuncJigsawOperateEnd));
            #endregion

            #region 局内
            funcTypes.Add(GuidNewFunctionDefine.PreOnEnterScene, typeof(GuidNewPreFuncOnEnterScene));
            funcTypes.Add(GuidNewFunctionDefine.SceneFingerTip, typeof(GuidNewFuncSceneFingerTip));
            funcTypes.Add(GuidNewFunctionDefine.SceneBlackEffect, typeof(GuidNewFuncSceneBlackEffect));
            #endregion

            #region 1122新增修改
            //公共相关
            funcTypes.Add(GuidNewFunctionDefine.CommonParam, typeof(GuidNewFuncSetCommonParam));
            funcTypes.Add(GuidNewFunctionDefine.AddItemToUser, typeof(GuidNewFuncAddItemToUser));
            funcTypes.Add(GuidNewFunctionDefine.LevelUp, typeof(GuidNewFuncLevelUp));
            funcTypes.Add(GuidNewFunctionDefine.ChangeGameStateToLogin, typeof(GuidNewFuncChangeGameStateToLogin));
            funcTypes.Add(GuidNewFunctionDefine.LeaveScene, typeof(GuidNewFuncLeaveScene));
            funcTypes.Add(GuidNewFunctionDefine.SaveProgress, typeof(GuidNewFuncSaveProgress));
            funcTypes.Add(GuidNewFunctionDefine.EffectMaskByFrame, typeof(GuidNewFuncEffectMaskByFrame));
            funcTypes.Add(GuidNewFunctionDefine.SetBasePlayInfo, typeof(GuidNewFuncSetBasePlayInfo));
            //UI相关
            funcTypes.Add(GuidNewFunctionDefine.UI_Open, typeof(GuidNewFuncOpenUILogic));
            funcTypes.Add(GuidNewFunctionDefine.UIIsHide, typeof(GuidNewFuncUIIsHide));
            funcTypes.Add(GuidNewFunctionDefine.OpenComponent, typeof(GuidNewFuncOpenComponent));
            //场景相关
            funcTypes.Add(GuidNewFunctionDefine.EnterSceneLocal, typeof(GuidNewFuncEnterSceneLocal));
            funcTypes.Add(GuidNewFunctionDefine.LoadCircleMaskByPosition, typeof(GuidNewFuncLoadCircleMaskByPosition));
            funcTypes.Add(GuidNewFunctionDefine.LoadCircleMaskByItemID, typeof(GuidNewFuncLoadCircleMaskByItemID));
            funcTypes.Add(GuidNewFunctionDefine.MoveCircleMaskByPostion, typeof(GuidNewFuncMoveCircleMaskByPostion));
            funcTypes.Add(GuidNewFunctionDefine.OnStartGame, typeof(GuidNewFuncOnStartGame));
            funcTypes.Add(GuidNewFunctionDefine.CircleMaskClick, typeof(GuidNewFuncCircleMaskClick));
            funcTypes.Add(GuidNewFunctionDefine.SceneExhibitEffectTips, typeof(GuidNewFuncSceneExhibitEffectTips));
            funcTypes.Add(GuidNewFunctionDefine.SceneComplete, typeof(GuidNewFuncSceneComplete));
            funcTypes.Add(GuidNewFunctionDefine.SceneEntityHintRule, typeof(GuidNewFuncSceneEntityHintRule));
            funcTypes.Add(GuidNewFunctionDefine.ScenePropArrow, typeof(GuidNewFuncScenePropArrow));
            funcTypes.Add(GuidNewFunctionDefine.SceneListenPropFind, typeof(GuidNewFuncSceneListenPropFind));
            funcTypes.Add(GuidNewFunctionDefine.ForbidProp, typeof(GuidNewFuncForbidProp));
            //资源相关
            funcTypes.Add(GuidNewFunctionDefine.ResourceLoadRes, typeof(GuidNewFuncResourceLoadRes));
            //遮罩相关
            funcTypes.Add(GuidNewFunctionDefine.MaskVisible, typeof(GuidNewFuncMaskVisible));
            //任务相关
            funcTypes.Add(GuidNewFunctionDefine.RefreshTaskById, typeof(GuidNewFuncRefreshTaskById));
            #endregion

            #region 1213修改
            funcTypes.Add(GuidNewFunctionDefine.EffectIndicateCircle, typeof(GuidNewFuncEffectIndicateCircle));
            funcTypes.Add(GuidNewFunctionDefine.SceneUnLockItemWord, typeof(GuidNewFuncSceneUnLockItemWord));
            funcTypes.Add(GuidNewFunctionDefine.ScenePropHintRule, typeof(GuidNewFuncScenePropHintRule));
            funcTypes.Add(GuidNewFunctionDefine.UnLockMainIcon, typeof(GuidNewFuncUnLockMainIcon));
            funcTypes.Add(GuidNewFunctionDefine.LoopTaskComplete, typeof(GuidNewFuncLoopTaskComplete));
            funcTypes.Add(GuidNewFunctionDefine.EventNormalState, typeof(GuidNewFuncEventNormalState));
            funcTypes.Add(GuidNewFunctionDefine.MutilEffectMaskByFrame, typeof(GuidNewFuncMutilEffectMaskByFrame));
#if OFFICER_SYS
            funcTypes.Add(GuidNewFunctionDefine.ListenNewPolice, typeof(GuidNewFuncListenNewPolice));
            funcTypes.Add(GuidNewFunctionDefine.NewPoliceMainPanel, typeof(GuidNewFuncNewPoliceMainPanel));
            funcTypes.Add(GuidNewFunctionDefine.HintNewPolice, typeof(GuidNewFuncHintNewPolice));
#endif
            funcTypes.Add(GuidNewFunctionDefine.CompleteGuidByGroup, typeof(GuidNewFuncCompleteGuidByGroup));
            funcTypes.Add(GuidNewFunctionDefine.OpenGameEntryButton, typeof(GuidNewOpenGameEntryButton));
            #endregion
            guidTypes.Add(GuidEnum.Guid_Cartoon, typeof(GuidNewStartCartoon));
            guidTypes.Add(GuidEnum.Guid_Click, typeof(GuidNewNormal));

        }
        //创建前置条件
        public GuidNewPreFunctionBase CreatePreFunctionById(long funcId)
        {
            ConfGuidNewFunction confFunc = ConfGuidNewFunction.Get(funcId);
            if (confFunc == null)
            {
                Debug.LogError("PrefuncId error " + funcId);
                return null;
            }
            if (!funcTypes.ContainsKey(confFunc.funcName))
            {
                Debug.LogError("PrefuncName error " + confFunc.funcName);
                return null;
            }
            Type guidType = funcTypes[confFunc.funcName];
            GuidNewPreFunctionBase baseGuid = Activator.CreateInstance(guidType) as GuidNewPreFunctionBase;
            if (baseGuid == null)
            {
                Debug.LogError("baseGuid error " + confFunc.funcName);
            }
            baseGuid.OnInit(confFunc.funcParams);
            return baseGuid;
        }
        //创建函数
        public GuidNewFunctionBase CreateFunctionById(long funcId)
        {
            ConfGuidNewFunction confFunc = ConfGuidNewFunction.Get(funcId);
            if (confFunc == null)
            {
                Debug.LogError("funcId error " + funcId);
                return null;
            }
            if (!funcTypes.ContainsKey(confFunc.funcName))
            {
                Debug.LogError("funcName error " + confFunc.funcName);
                return null;
            }
            Type guidType = funcTypes[confFunc.funcName];
            GuidNewFunctionBase baseGuid = Activator.CreateInstance(guidType) as GuidNewFunctionBase;
            if (baseGuid == null)
            {
                Debug.LogError("func eror : " + funcId);
            }
            baseGuid.OnInit(funcId, confFunc.funcParams);
            return baseGuid;
        }

        private Dictionary<GuidEnum, Type> guidTypes = new Dictionary<GuidEnum, Type>();

        public GuidNewBase CreateGuidById(long guidID)
        {
            ConfGuidNew confGuid = ConfGuidNew.Get(guidID);
            if (confGuid == null)
            {
                return null;
            }
            Type guidType = guidTypes[(GuidEnum)confGuid.type];
            GuidNewBase baseGuid = Activator.CreateInstance(guidType) as GuidNewBase;
            baseGuid.SetGuidID(guidID);
            return baseGuid;
        }

    }
}
