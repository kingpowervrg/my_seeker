/********************************************************************
	created:  2018-4-4 9:55:50
	filename: GameMainSettingUILogic.cs
	author:	  songguangze@outlook.com
	
	purpose:  游戏内暂停设置界面
*********************************************************************/
using EngineCore;
using GOEngine;
using UnityEngine;
using UnityEngine.UI;

namespace SeekerGame
{

    [UILogicHandler(UIDefine.UI_GAME_MAIN_SETTING)]
    public class GameMainSettingUILogic : UILogicBase
    {
        private GameSliderButton m_sliderBtnBackgroundMusic = null;
        private GameSliderButton m_sliderBtnSound = null;

        private GameButton m_btnContinue = null;
        private GameButton m_btnQuit = null;

        private GameButton m_btnContinueOnly = null;

        private GameImage m_backgroundTexture = null;
        private GameUIComponent m_sceneMask = null;

        private PauseData m_data;

        private GameImage m_bg;
        private GameLabel m_text01, m_text02;
        //private UITweenerBase[] tweener = null;
        protected override void OnInit()
        {
            m_bg = Make<GameImage>("Image_back");
            m_text01 = Make<GameLabel>("Text_1");
            m_text02 = Make<GameLabel>("Text_2");

            m_sliderBtnBackgroundMusic = Make<GameSliderButton>("Toggle_1");
            UITweenerBase[] UITweeners = m_sliderBtnBackgroundMusic.gameObject.GetComponentsInChildren<UITweenerBase>(true);
            foreach (var ut in UITweeners)
            {
                ut.ResetAndPlay();
            }

            m_sliderBtnSound = Make<GameSliderButton>("Toggle_2");

            UITweeners = m_sliderBtnSound.gameObject.GetComponentsInChildren<UITweenerBase>(true);
            foreach (var ut in UITweeners)
            {
                ut.ResetAndPlay();
            }

            m_btnContinue = Make<GameButton>("Button_continue");
            m_btnQuit = Make<GameButton>("Button_quit");
            this.m_btnContinueOnly = Make<GameButton>("Button_continue_Guid");

            m_backgroundTexture = Make<GameImage>("RawImage");
            m_sceneMask = Make<GameUIComponent>("sceneMask");
            m_backgroundTexture.Visible = false;
            m_sceneMask.Visible = false;
            //this.tweener = Transform.GetComponentsInChildren<UITweenerBase>(false);
        }

        public override void OnShow(object param)
        {
            if (null != param)
            {
                m_data = (PauseData)param;
                SwitchBG();
                HidePause(!m_data.m_hidePause);
            }
            GameEvents.UIEvents.UI_Pause_Event.OnClosePauseFrame += OnClosePauseFrame;
            GameEvents.UIEvents.UI_Pause_Event.OnHidePauseFrame += HidePause;


            this.m_sliderBtnBackgroundMusic.AddChangeCallBack(OnSliderBtnMusicChanged);
            this.m_sliderBtnSound.AddChangeCallBack(OnSliderBtnSoundChanged);

            this.m_btnContinue.AddClickCallBack(OnBtnContinueClick);
            this.m_btnQuit.AddClickCallBack(OnBtnQuitClick);
            this.m_btnContinueOnly.AddClickCallBack(OnBtnContinueClick);

            MessageHandler.RegisterMessageHandler(MessageDefine.SCSceneResumeResponse, OnGameResumeResponse);
            MessageHandler.RegisterMessageHandler(MessageDefine.SCSceneQuitResponse, OnGameQuitResponse);
            MessageHandler.RegisterMessageHandler(MessageDefine.SCResumeResponse, OnGameResumeResponse);
            MessageHandler.RegisterMessageHandler(MessageDefine.SCQuitResponse, OnGameQuitResponse);
            //for (int i = 0; i < this.tweener.Length; i++)
            //{
            //    if (this.tweener[i].enabled)
            //    {
            //        this.tweener[i].ResetAndPlay();
            //    }
            //}


            //if (GlobalInfo.Enable_Music)
            //this.m_sliderBtnBackgroundMusic.Checked = false;

            this.m_sliderBtnBackgroundMusic.Checked = GlobalInfo.Enable_Music;

            //if (GlobalInfo.Enable_Sound)
            //this.m_sliderBtnSound.Checked = false;

            this.m_sliderBtnSound.Checked = GlobalInfo.Enable_Sound;
            //OnRefreshButton();

        }

        private void OnClosePauseFrame()
        {
            OnBtnContinueClick(null);
        }

        public override FrameDisplayMode UIFrameDisplayMode
        {
            get
            {
                return FrameDisplayMode.WINDOWED;
            }
        }

        private void HidePause(bool hidePause)
        {
            bool twoContinue = GlobalInfo.GAME_NETMODE == GameNetworkMode.Network; //NewGuid.GuidNewManager.Instance.GetProgressByIndex(5);
            this.m_sliderBtnBackgroundMusic.Visible = hidePause;
            this.m_sliderBtnSound.Visible = hidePause;
            this.m_btnContinue.Visible = hidePause & twoContinue;
            this.m_btnQuit.Visible = hidePause & twoContinue;
            this.m_btnContinueOnly.Visible = hidePause & !twoContinue;
            this.m_bg.Visible = hidePause;
            this.m_text01.Visible = hidePause;
            this.m_text02.Visible = hidePause;
            if (!hidePause)
            {
                m_sceneMask.Visible = hidePause;
                this.m_backgroundTexture.Visible = hidePause;
            }
            else
            {
                SwitchBG();
            }

            //OnRefreshButton();
        }

        private void OnBtnContinueClick(GameObject btnContinue)
        {
            EngineCoreEvents.AudioEvents.PlayAudio.SafeInvoke(Audio.AudioType.UISound, EngineCommonAudioKey.Button_Click_Common.ToString());

            if (null == m_data)
            {
                DebugUtil.LogError("暂停菜单不知道是哪里调用的暂停");
                this.CloseFrame();
                return;
            }

            if (ENUM_SEARCH_MODE.E_SEARCH_ROOM == m_data.m_mode)
            {
                CSSceneResumeRequest resumeReq = new CSSceneResumeRequest();
                resumeReq.PlayerId = GlobalInfo.MY_PLAYER_ID;


#if !NETWORK_SYNC || UNITY_EDITOR
                GameEvents.NetWorkEvents.SendAsyncMsg.SafeInvoke(resumeReq);
#else
            GameEvents.NetWorkEvents.SendMsg.SafeInvoke(resumeReq);
#endif
            }
            else if (ENUM_SEARCH_MODE.E_JIGSAW == m_data.m_mode)
            {

                CSResumeRequest resumeReq = new CSResumeRequest();
                resumeReq.PlayerId = GlobalInfo.MY_PLAYER_ID;


#if !NETWORK_SYNC || UNITY_EDITOR
                GameEvents.NetWorkEvents.SendAsyncMsg.SafeInvoke(resumeReq);
#else
            GameEvents.NetWorkEvents.SendMsg.SafeInvoke(resumeReq);
#endif
            }
            else if (ENUM_SEARCH_MODE.E_EVENTGAME == m_data.m_mode)
            {
                this.CloseFrame();
            }
            else if (ENUM_SEARCH_MODE.E_SCAN == m_data.m_mode)
            {

                this.CloseFrame();
                //调用继续游戏
                GameEvents.UIEvents.UI_Scan_Event.Listen_ResumeGame.SafeInvoke();

            }
        }

        private void OnSliderBtnMusicChanged(bool value)
        {
            EngineCoreEvents.AudioEvents.PlayAudio.SafeInvoke(Audio.AudioType.UISound, EngineCommonAudioKey.table_change.ToString());
            GlobalInfo.Enable_Music = value;
            if (value)
                SceneModule.Instance.CurrentScene.PlaySceneBGM();
        }

        private void OnSliderBtnSoundChanged(bool value)
        {
            EngineCoreEvents.AudioEvents.PlayAudio.SafeInvoke(Audio.AudioType.UISound, EngineCommonAudioKey.table_change.ToString());
            GlobalInfo.Enable_Sound = value;
        }

        private void OnGameResumeResponse(object msg)
        {
            if (null == m_data)
            {
                DebugUtil.LogError("暂停菜单不知道是哪里调用的暂停");
                return;
            }

            if (ENUM_SEARCH_MODE.E_SEARCH_ROOM == m_data.m_mode)
            {
                SCSceneResumeResponse resumeMsg = msg as SCSceneResumeResponse;

                if (!MsgStatusCodeUtil.OnError(resumeMsg.Result))
                {
                    EngineCoreEvents.UIEvent.HideUIEvent.SafeInvoke(UIDefine.UI_GAME_MAIN_SETTING);
                    EngineCoreEvents.UIEvent.BlurUIBackground(false);
                    GameEvents.MainGameEvents.OnGameStatusChange.SafeInvoke(SceneBase.GameStatus.GAMING);
                }
            }
            else if (ENUM_SEARCH_MODE.E_JIGSAW == m_data.m_mode)
            {
                SCResumeResponse resumeMsg = msg as SCResumeResponse;
                if (!MsgStatusCodeUtil.OnError(resumeMsg.Result))
                {
                    EngineCoreEvents.UIEvent.HideUIEvent.SafeInvoke(UIDefine.UI_GAME_MAIN_SETTING);
                }
            }
        }

        private void OnGameQuitResponse(object msg)
        {
            if (null == m_data)
            {
                DebugUtil.LogError("暂停菜单不知道是哪里调用的暂停");
                return;
            }

            if (ENUM_SEARCH_MODE.E_SEARCH_ROOM == m_data.m_mode)
            {
                SCSceneQuitResponse quitGameResp = msg as SCSceneQuitResponse;

                if (!MsgStatusCodeUtil.OnError(quitGameResp.Result))
                {
                    GameEvents.SceneEvents.OnLeaveScene.SafeInvoke();
                    this.CloseFrame();
                    SceneModule.Instance.EnterMainScene();
                }
            }
            else if (ENUM_SEARCH_MODE.E_JIGSAW == m_data.m_mode)
            {
                SCQuitResponse quitGameResp = msg as SCQuitResponse;
                if (!MsgStatusCodeUtil.OnError(quitGameResp.Result))
                {
                    this.CloseFrame();
                }

                GameEvents.UIEvents.UI_GameEntry_Event.OnOpenPanel.SafeInvoke(UIDefine.UI_CITY);
            }

        }

        private void OnBtnQuitClick(GameObject btnQuit)
        {
            EngineCoreEvents.AudioEvents.PlayAudio.SafeInvoke(Audio.AudioType.UISound, EngineCommonAudioKey.Close_Window.ToString());
            if (null == m_data)
            {
                DebugUtil.LogError("暂停菜单不知道是哪里调用的暂停");
                return;
            }
            GameEvents.UIEvents.UI_Pause_Event.OnQuit.SafeInvoke();

            if (ENUM_SEARCH_MODE.E_SEARCH_ROOM == m_data.m_mode)
            {
                CSSceneQuitRequest quitGame = new CSSceneQuitRequest();
                quitGame.PlayerId = GlobalInfo.MY_PLAYER_ID;
                quitGame.Type = 0;

#if !NETWORK_SYNC || UNITY_EDITOR
                GameEvents.NetWorkEvents.SendAsyncMsg.SafeInvoke(quitGame);
#else
            GameEvents.NetWorkEvents.SendMsg.SafeInvoke(quitGame);
#endif
                GameEvents.UI_Guid_Event.OnFindSceneResult.SafeInvoke(false);

            }
            else if (ENUM_SEARCH_MODE.E_JIGSAW == m_data.m_mode)
            {
                CSQuitRequest quitGame = new CSQuitRequest();
                quitGame.PlayerId = GlobalInfo.MY_PLAYER_ID;
                quitGame.SceneId = m_data.m_id;

#if !NETWORK_SYNC || UNITY_EDITOR
                GameEvents.NetWorkEvents.SendAsyncMsg.SafeInvoke(quitGame);
#else
            GameEvents.NetWorkEvents.SendMsg.SafeInvoke(quitGame);
#endif

            }
            else if (ENUM_SEARCH_MODE.E_EVENTGAME == m_data.m_mode)
            {
                this.CloseFrame();
            }
            else if (ENUM_SEARCH_MODE.E_SCAN == m_data.m_mode)
            {
                this.CloseFrame();
            }
        }

        public override void OnHide()
        {
            base.OnHide();
            GameEvents.UIEvents.UI_Pause_Event.OnClosePauseFrame -= OnClosePauseFrame;
            GameEvents.UIEvents.UI_Pause_Event.OnHidePauseFrame -= HidePause;
            MessageHandler.UnRegisterMessageHandler(MessageDefine.SCSceneResumeResponse, OnGameResumeResponse);
            MessageHandler.UnRegisterMessageHandler(MessageDefine.SCSceneQuitResponse, OnGameQuitResponse);
            MessageHandler.UnRegisterMessageHandler(MessageDefine.SCResumeResponse, OnGameResumeResponse);
            MessageHandler.UnRegisterMessageHandler(MessageDefine.SCQuitResponse, OnGameQuitResponse);
            GameEvents.UIEvents.UI_GameEntry_Event.OnMaskBGVisible.SafeInvoke(false);
            this.m_sliderBtnBackgroundMusic.RemoveChangeCallBack(OnSliderBtnMusicChanged);
            this.m_sliderBtnSound.RemoveChangeCallBack(OnSliderBtnSoundChanged);
            this.m_btnContinue.RemoveClickCallBack(OnBtnContinueClick);
            this.m_btnQuit.RemoveClickCallBack(OnBtnQuitClick);
            this.m_btnContinueOnly.RemoveClickCallBack(OnBtnContinueClick);
        }

        private void SwitchBG()
        {
            if (ENUM_SEARCH_MODE.E_JIGSAW == m_data.m_mode || ENUM_SEARCH_MODE.E_EVENTGAME == m_data.m_mode)
            {
                this.m_backgroundTexture.Visible = true;
                m_sceneMask.Visible = false;
            }
            else if (ENUM_SEARCH_MODE.E_SEARCH_ROOM == m_data.m_mode)
            {
                if (GameMainHelper.Instance.m_currentScenMode == SceneMode.DARKER || GameMainHelper.Instance.m_currentScenMode == SceneMode.FOGGY)
                {
                    this.m_backgroundTexture.Visible = true;
                    m_sceneMask.Visible = false;
                }
                else
                {
                    this.m_backgroundTexture.Visible = false;
                    m_sceneMask.Visible = true;
                }
            }
            else
            {
                this.m_backgroundTexture.Visible = false;
                m_sceneMask.Visible = true;
            }

        }

        private void OnRefreshButton()
        {
            bool twoContinue = NewGuid.GuidNewManager.Instance.GetProgressByIndex(5);
            this.m_btnContinue.Visible = twoContinue;
            this.m_btnQuit.Visible = twoContinue;
            this.m_btnContinueOnly.Visible = !twoContinue;
        }
    }
}
