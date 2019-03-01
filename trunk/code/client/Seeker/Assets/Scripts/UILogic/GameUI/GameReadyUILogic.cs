/********************************************************************
	created:  2018-6-13 20:26:8
	filename: GameReadyUILogic.cs
	author:	  songguangze@fotoable.com
	
	purpose:  游戏开始准备UI
*********************************************************************/
using EngineCore;
using System.Collections.Generic;
using UnityEngine;

namespace SeekerGame
{
    [UILogicHandler(UIDefine.UI_GAME_READY)]
    public class GameReadyUILogic : UILogicBase
    {
        private GameUIComponent m_readyEffect = null;
        private GameImage m_bg;
        private GameTexture m_bg01;

        private MainSceneData m_mainSceneData = null;
        protected override void OnInit()
        {
            m_bg = Make<GameImage>("Image_bg");
            m_bg01 = Make<GameTexture>("RawImage");
            m_readyEffect = Make<GameUIComponent>("UI_youxikaishi");
        }

        public override void OnShow(object param)
        {
            if (param != null)
            {
                m_mainSceneData = (MainSceneData)param;
            }

            //m_readyEffect.EffectPrefabName = "UI_youxikaishi.prefab";
            m_readyEffect.Visible = false;

            m_bg.Visible = false;
            m_bg01.Visible = false;
            GameEvents.UIEvents.UI_Loading_Event.OnLoadingOver += OnLoadingOver;
        }

        public override void OnGuidShow(int type = 0)
        {
            base.OnGuidShow(type);
            m_bg.Visible = false;
            m_bg01.Visible = false;
        }

        public override void OnHide()
        {
            base.OnHide();
            GameEvents.UIEvents.UI_Loading_Event.OnLoadingOver -= OnLoadingOver;
        }

        private void OnLoadingOver()
        {
            if (GameEvents.MainGameEvents.GetCameraBound == null || m_mainSceneData == null || !GameMainHelper.Instance.isNeedTips(m_mainSceneData.sceneID, m_mainSceneData.taskConfID, true))
            {
                GameEvents.MainGameEvents.OnFingerForbidden.SafeInvoke(false);
                m_bg.Visible = true;
                m_bg01.Visible = true;
                PlaySceneReady();
            }
            else
            {
                GameEvents.MainGameEvents.OnScenePanelVisible.SafeInvoke(false);
                GameEvents.MainGameEvents.OnFingerForbidden.SafeInvoke(true);
                UnityEngine.Vector3 cameraBounds = GameEvents.MainGameEvents.GetCameraBound();
                //SceneModule.Instance.CurrentScene.PlaySceneBGM(ConfSound.Get("Game_01").SoundPath);
                HedgehogTeam.EasyTouch.Gesture gesture = new HedgehogTeam.EasyTouch.Gesture();
                gesture.deltaPinch = cameraBounds.x;
                CameraManager.Instance.IsAutoPlay = true;
                GameEvents.MainGameEvents.OnCameraZoomOrRotation.SafeInvoke(0, gesture, 1f);

                TimeModule.Instance.SetTimeout(() =>
                {
                    HedgehogTeam.EasyTouch.Gesture gestureMove0 = new HedgehogTeam.EasyTouch.Gesture();
                    gesture.swipeVector = UnityEngine.Vector2.left * cameraBounds.y;
                    Debug.Log("right : " + gesture.swipeVector);
                    GameEvents.MainGameEvents.OnCameraZoomOrRotation.SafeInvoke(2, gesture, 1f);
                }, 1f);

                TimeModule.Instance.SetTimeout(() =>
                {
                    HedgehogTeam.EasyTouch.Gesture gestureMove0 = new HedgehogTeam.EasyTouch.Gesture();
                    gesture.swipeVector = UnityEngine.Vector2.right * (cameraBounds.y + cameraBounds.z);
                    Debug.Log("left : " + gesture.swipeVector);
                    GameEvents.MainGameEvents.OnCameraZoomOrRotation.SafeInvoke(2, gesture, 1f);
                }, 2f);

                TimeModule.Instance.SetTimeout(() =>
                {
                    HedgehogTeam.EasyTouch.Gesture gestureMove0 = new HedgehogTeam.EasyTouch.Gesture();
                    gesture.swipeVector = UnityEngine.Vector2.left * cameraBounds.z;
                    Debug.Log("right : " + gesture.swipeVector);
                    GameEvents.MainGameEvents.OnCameraZoomOrRotation.SafeInvoke(2, gesture, 1f);
                }, 3f);

                TimeModule.Instance.SetTimeout(() =>
                {
                    HedgehogTeam.EasyTouch.Gesture gestureMove0 = new HedgehogTeam.EasyTouch.Gesture();
                    gesture.deltaPinch = cameraBounds.x;
                    GameEvents.MainGameEvents.OnCameraZoomOrRotation.SafeInvoke(1, gesture, 1f);
                }, 4f);

                TimeModule.Instance.SetTimeout(() =>
                {
                    CameraManager.Instance.IsAutoPlay = false;
                    GameEvents.MainGameEvents.OnScenePanelVisible.SafeInvoke(true);
                    GameEvents.MainGameEvents.OnFingerForbidden.SafeInvoke(false);
                    GameEvents.MainGameEvents.OnClearCameraStatus.SafeInvoke();
                    m_bg.Visible = true;
                    m_bg01.Visible = true;
                    PlaySceneReady();
                }, 5.2f);
            }


        }

        private void PlaySceneReady()
        {
            this.m_bg.Visible = true;
            this.m_bg01.Visible = true;

            string gameBGM = $"Game_0{Random.Range(1, 3)}";
            SceneModule.Instance.CurrentScene.PlaySceneBGM(gameBGM);

            m_readyEffect.Visible = true;
            TimeModule.Instance.SetTimeout(() =>
            {
                SceneModule.Instance.StartGame();

                CloseFrame();
            }, 2.5f);
        }
        public override FrameDisplayMode UIFrameDisplayMode
        {
            get
            {
                return FrameDisplayMode.WINDOWED;
            }
        }
    }
}