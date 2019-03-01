using System;
using System.Collections.Generic;
using EngineCore;
using EngineCore.Utility;
using UnityEngine;
using HedgehogTeam.EasyTouch;
namespace SeekerGame.NewGuid
{
    /// <summary>
    /// 提示物件
    /// </summary>
    public class GuidNewFuncSceneHintItem : GuidNewFunctionBase
    {
        private bool isComplete = false;
        public override void OnInit(long funcID, string[] param)
        {
            base.OnInit(funcID, param);
        }

        public override void OnExecute()
        {
            base.OnExecute();
            GameEvents.MainGameEvents.OnGameOver += OnGameOver;
            TimeModule.Instance.SetTimeout(()=> {
                List<SceneItemEntity> sceneItem = GameEvents.MainGameEvents.GetSceneItemEntityList.SafeInvoke(1);
                if (sceneItem != null && sceneItem.Count > 0)
                {
                    Vector3 entityPos = sceneItem[0].EntityPosition;
                    GUIFrame frame = GuidNewModule.Instance.GetFrameByResName(UIDefine.UI_GUID);
                    entityPos = CameraUtility.WorldPointInCanvasRectTransform(entityPos, frame.LogicHandler.Canvas.gameObject);
                    Vector2[] corn = new Vector2[4];
                    float w = 10f;
                    float h = 10f;
                    if (sceneItem[0].EntityData.itemID == 11197)
                    {
                        w = 20f;
                        h = 50f;
                        entityPos.y += 40f;
                    }
                    corn[0] = new Vector2(entityPos.x - w, entityPos.y - h);
                    corn[1] = new Vector2(entityPos.x - w, entityPos.y + h);
                    corn[2] = new Vector2(entityPos.x + w, entityPos.y + h);
                    corn[3] = new Vector2(entityPos.x + w, entityPos.y - h);
                    if (!isComplete)
                    {
                        GameEvents.UI_Guid_Event.OnShowMask.SafeInvoke(new List<Vector2[]> { corn }, new List<MaskEmptyType> { MaskEmptyType.Rect }, MaskAnimType.ToInner, "aa", false);
                        GUIFrame guidFrame = GuidNewModule.Instance.GetFrameByResName(UIDefine.UI_GUID);
                        if (guidFrame != null)
                        {
                            Vector2[] emptyPos = GuidNewTools.GetEmptyPos(corn, guidFrame.FrameRootTransform);
                            emptyPos[0] += Vector2.left * 20f;
                            emptyPos[0] += Vector2.up * 30f;
                            //Vector2 center = new Vector2((emptyPos[2].x + cornPos[0].x) / 2f, (cornPos[1].y + cornPos[0].y) / 2f);
                            GameEvents.UI_Guid_Event.OnLoadEffect.SafeInvoke(20000, "UI_xinshouyindao_shou.prefab", emptyPos[0], Vector2.one, 0);
                        }

                        GameEvents.UI_Guid_Event.OnMaskClick += OnMaskClick;
                    }
                    else
                    {
                        OnDestory();
                    }

                    return;
                }
                OnDestory();
            },1f);
            //GameEvents.MainGameEvents.RequestHintSceneItemList.SafeInvoke(5);
        }

        private void OnGameOver(SceneBase.GameResult result)
        {
            this.isComplete = true;
        }
        Gesture gesture = null;
        private void OnMaskClick(Vector2 worldPos, bool inner)
        {
            if (inner)
            {
                gesture = new Gesture();
                GameObject obj = gesture.GetCurrentPickedObject();
                if (obj != null)
                {
                    gesture.pickedObject = obj;
                }
                MessageHandler.RegisterMessageHandler(MessageDefine.SCSceneResumeResponse, OnResponse);
                CSSceneResumeRequest resumeReq = new CSSceneResumeRequest();
                resumeReq.PlayerId = GlobalInfo.MY_PLAYER_ID;
                GameEvents.NetWorkEvents.SendMsg.SafeInvoke(resumeReq);
            }
        }

        private void OnResponse(object res)
        {
            if (res is SCSceneResumeResponse)
            {
                MessageHandler.UnRegisterMessageHandler(MessageDefine.SCSceneResumeResponse, OnResponse);
                SCSceneResumeResponse resumeMsg = res as SCSceneResumeResponse;
                if (!MsgStatusCodeUtil.OnError(resumeMsg.Result))
                {
                    GameEvents.MainGameEvents.OnGameStatusChange.SafeInvoke(SceneBase.GameStatus.GAMING);
                    EngineCoreEvents.InputEvent.OnOneFingerTouchup.SafeInvoke(gesture);

                    OnDestory();
                }
            }
        }

        public override void OnDestory(FuncState funcState = FuncState.Complete)
        {
            base.OnDestory(funcState);
            GameEvents.UI_Guid_Event.OnMaskClick -= OnMaskClick;
            GameEvents.MainGameEvents.OnGameOver -= OnGameOver;
        }

        public override void ClearFunc()
        {
            base.ClearFunc();
            GameEvents.UI_Guid_Event.OnMaskClick -= OnMaskClick;
            GameEvents.MainGameEvents.OnGameOver -= OnGameOver;
        }
    }
}
