/********************************************************************
	created:  2018-4-24 11:21:41
	filename: LoadingManager.cs
	author:	  songguangze@outlook.com
	
	purpose:  整合游戏各种Loading需求
*********************************************************************/
using EngineCore;
using System;
using System.Collections.Generic;

namespace SeekerGame
{
    public class LoadingManager : Singleton<LoadingManager>
    {
        public long SCENE_ID = 0L;

        long m_offline_time = -1L;
        float m_reconnect_time;
        public void Init()
        {
            EngineCore.EngineCoreEvents.ResourceEvent.PreloadAssetEvent.SafeInvoke(UIDefine.UI_SYNC_LOADING, (preloadStatus) =>
            {
                if (preloadStatus)
                {
                    EngineCore.EngineCoreEvents.UIEvent.ShowUIEventWithParam.SafeInvoke(new FrameMgr.OpenUIParams(UIDefine.UI_SYNC_LOADING)
                    {
                        Param = false,
                        OnShowFinishCallback = (loadingUILogic) =>
                        {
                            loadingUILogic.CloseFrame();
                        }
                    });

                    EngineCore.EngineCoreEvents.SystemEvents.OnSendingSyncMsg += OnSendingSyncMessage;
                    EngineCore.EngineCoreEvents.SystemEvents.OnRetryingSyncMsg += OnRetryingSyncMessage;
                    EngineCore.EngineCoreEvents.SystemEvents.OnSendingSyncMsgCallback += OnSendingSyncMessageCallback;
                }
            });

            MessageHandler.RegisterMessageHandler(MessageDefine.SCPingResponse, OnSCPingResponse);
            MessageHandler.RegisterMessageHandler(MessageDefine.SCSceneReconnectResponse, OnReconnectResponse);
            MessageHandler.RegisterMessageHandler(MessageDefine.SCJigsawReconnectResponse, OnReconnectResponse);


            GameEvents.NetworkWatchEvents.NetError += NetError;
            GameEvents.NetworkWatchEvents.NetPass += NetPass;
        }

        /// <summary>
        /// 同步网络消息Block
        /// </summary>
        /// <param name="messageId"></param>
        private static void OnSendingSyncMessage(int messageId)
        {
            if (!LoadingProgressManager.Instance.IsLoading)
            {
                //TimeModule.Instance.SetTimeout(DelayShowSync, 1f);
                DelayShowSync();
            }

        }


        private void OnRetryingSyncMessage(int messageId, NetworkModule.NetworkStatus state)
        {
            if (NetworkModule.NetworkStatus.WAITTING_RETRY == state)
            {
                ShowRetry();
            }
            else if (NetworkModule.NetworkStatus.RESET_RETRY_TIME == state)
            {
                ResetRetry();
            }

        }


        private static void DelayShowSync()
        {
            EngineCore.EngineCoreEvents.UIEvent.ShowUIEvent.SafeInvoke(UIDefine.UI_SYNC_LOADING);
            GameEvents.System_Events.SetLoadingTips.SafeInvoke("Syncing...");
        }

        private void ResetRetry()
        {
            GameEvents.UIEvents.UI_POPUP_Event.OnConfirm.SafeInvoke();
        }

        public void ShowRetry()
        {
            FrameMgr.OpenUIParams ui_params = new FrameMgr.OpenUIParams(UIDefine.UI_POPUP_WITH_CONFIRM);

            List<object> pops = new List<object>();

            PopUpTickerData pd = new PopUpTickerData();
            pd.title = "NETWORK_RETRY_TITLE";
            pd.content = "NETWORK_RETRY_CONTENT";
            pd.content_param0 = null;
            pd.OneButtonText = "close_button";
            pd.isOneBtn = true;
            pd.order_in_layer = 13000;
            pd.oneAction = BackToLogin;
            pd.ticker_seconds = (int)HttpRetry.C_MAX_RETRY_SECONDS;

            PopUpConfirmData pd_confirm = new PopUpConfirmData();
            pd_confirm.title = "";
            pd_confirm.content = "NETWORK_RETRY_CONFIRM";
            pd_confirm.content_param0 = null;
            pd_confirm.isOneBtn = false;
            pd_confirm.twoStr = "close_button";
            pd_confirm.oneAction = ContinueRetry;
            pd_confirm.twoAction = BackToLogin;

            pops.Add(pd);
            pops.Add(pd_confirm);

            ui_params.Param = pops;

            EngineCoreEvents.UIEvent.ShowUIEventWithParam.SafeInvoke(ui_params);
        }

        private static void HideRetry()
        {
            EngineCoreEvents.UIEvent.HideUIEvent.SafeInvoke(UIDefine.UI_POPUP_WITH_CONFIRM);
        }

        private void ContinueRetry()
        {
            Dictionary<UBSParamKeyName, object> _param = new Dictionary<UBSParamKeyName, object>();
            _param.Add(UBSParamKeyName.Description, "continue retry");
            UserBehaviorStatisticsModules.Instance.LogEvent(UBSEventKeyName.net_error, null, _param);

            EngineCoreEvents.SystemEvents.RetryPendingMsgs.SafeInvoke(true);
        }

        public void CreateRetry()
        {
            EngineCoreEvents.SystemEvents.RetryPendingMsgs.SafeInvoke(false);
            EngineCoreEvents.SystemEvents.RetryPendingMsgs.SafeInvoke(true);
            EngineCoreEvents.SystemEvents.OnEnableRetry.SafeInvoke(true);
        }

        private void DestroyRetry()
        {
            EngineCoreEvents.SystemEvents.OnEnableRetry.SafeInvoke(false);
            EngineCoreEvents.SystemEvents.RetryPendingMsgs.SafeInvoke(false);
        }

        private static void DelayHideSync()
        {
            EngineCore.EngineCoreEvents.UIEvent.HideUIEvent.SafeInvoke(UIDefine.UI_SYNC_LOADING);
        }

        /// <summary>
        /// 同步网络消息Block
        /// </summary>
        /// <param name="messageId"></param>
        /// <param name="status"></param>
        private void OnSendingSyncMessageCallback(int messageId, EngineCore.NetworkModule.NetworkStatus status)
        {
            DelayHideSync();
            switch (status)
            {
                case EngineCore.NetworkModule.NetworkStatus.NORMAL:
                case EngineCore.NetworkModule.NetworkStatus.SYNC_SUCCEED:
                    EngineCore.EngineCoreEvents.UIEvent.HideUIEvent.SafeInvoke(UIDefine.UI_SYNC_LOADING);
                    break;
                case EngineCore.NetworkModule.NetworkStatus.RETRY_CACHE_CLEAR:
                    HideRetry();
                    break;
                case EngineCore.NetworkModule.NetworkStatus.TIMEOUT:
                    EngineCore.EngineCoreEvents.UIEvent.ShowUIEvent.SafeInvoke(UIDefine.UI_SYNC_LOADING);
                    GameEvents.System_Events.SetLoadingTips.SafeInvoke(LocalizeModule.Instance.GetString("NETWORK_TIMEOUT"));

                    if (m_offline_time < 0)
                        m_offline_time = CommonTools.GetCurrentTimeSecond();


                    TimeModule.Instance.SetTimeout(() =>
                    {
                        OfflineTips();
                    }, 2.5f);
                    break;
                case NetworkModule.NetworkStatus.EXPIRE:
                    break;
                case NetworkModule.NetworkStatus.OFFLINE_WARNNING:
                    {
                        TimeModule.Instance.SetTimeout(() =>
                        {
                            OfflineTipsNoRetry();
                        }, 2.5f);
                    }
                    break;
            }
        }

        public void OfflineTipsNoRetry()
        {
            EngineCore.EngineCoreEvents.UIEvent.HideUIEvent.SafeInvoke(UIDefine.UI_SYNC_LOADING);

            PopUpData pd = new PopUpData();
            pd.title = "NETWORK_TIME";
            pd.content = "systen_unusual";
            pd.content_param0 = null;
            pd.isOneBtn = true;
            pd.order_in_layer = 13000;

            PopUpManager.OpenPopUp(pd);
        }

        public void OfflineTips()
        {
            HttpPingModule.Instance.Enable = false;
            EngineCore.EngineCoreEvents.UIEvent.HideUIEvent.SafeInvoke(UIDefine.UI_SYNC_LOADING);

            PopUpData pd = new PopUpData();
            pd.title = "NETWORK_TIME";
            pd.content = "systen_unusual";
            pd.content_param0 = null;
            pd.isOneBtn = false;
            pd.twoStr = "close_button";
            pd.order_in_layer = 13000;
            if (InOrOutScene.InScene == (InOrOutScene)((SceneModule.Instance.CurrentScene is GameSceneBase) ? 1 : 0))
            {
                if (SceneBase.GameStatus.GAMEOVER != ((GameSceneBase)(SceneModule.Instance.CurrentScene)).CurGameStatus)
                {
                    PauseGame();
                    pd.oneAction = SendNeedMoreTime;
                }
                else
                {
                    pd.oneAction = TestNetworkAgain;
                }
            }
            else if (0 != LoadingManager.Instance.SCENE_ID)
            {
                PauseGame();
                pd.oneAction = SendNeedMoreTime;
            }
            else
            {
                if (GameRoot.instance.GameFSM.CurrentState.StateFlag.Equals((int)ClientFSM.ClientState.LOGIN))
                {
                    pd.oneAction = null;
                    ResetOfflineTime();
                }
                else
                    pd.oneAction = TestNetworkAgain;
            }

            pd.twoAction = BackToLogin;

            PopUpManager.OpenPopUp(pd);


        }

        private void PauseGame()
        {
            if (0 == SCENE_ID)
                return;

            if (2 == SCENE_ID / 10000)
            {
                GameEvents.UIEvents.UI_Jigsaw_Event.OnPause.SafeInvoke(true);
            }
            else if (4 == SCENE_ID / 10000)
            {

            }
            else
            {
                GameEvents.MainGameEvents.OnGameStatusChange.SafeInvoke(SceneBase.GameStatus.PAUSE);
            }
        }

        private void ResetOfflineTime()
        {
            m_offline_time = -1L;
        }

        private void OnSCPingResponse(object msg_)
        {
            if (HttpPingModule.Instance.Enable)
                return;
            GameEvents.UIEvents.UI_GameEntry_Event.OnOpenPanel.SafeInvoke(UIDefine.UI_CITY);
            //if (SeekerGame.NewGuid.GuidNewManager.Instance.GetProgressByIndex(6))
            //{

            //}


            ResetOfflineTime();
            HttpPingModule.Instance.Enable = true;
        }

        private void OnReconnectResponse(object msg_)
        {
            CommonTools.CheckNetError(true);
            ResetOfflineTime();
            HttpPingModule.Instance.Enable = true;
        }

        private void SendPingAgain()
        {
            HttpPingModule.Instance.SendSyncPing();
        }

        private void TestNetworkAgain()
        {
            EngineCore.EngineCoreEvents.UIEvent.ShowUIEvent.SafeInvoke(UIDefine.UI_SYNC_LOADING);
            CommonHelper.TestNetwork();
        }

        private void NetError()
        {
            OfflineTips();
        }

        private void NetPass()
        {
            EngineCore.EngineCoreEvents.UIEvent.HideUIEvent.SafeInvoke(UIDefine.UI_SYNC_LOADING);

            if (HttpPingModule.Instance.Enable)
                return;
            GameEvents.UIEvents.UI_GameEntry_Event.OnOpenPanel.SafeInvoke(UIDefine.UI_CITY);
            //if (SeekerGame.NewGuid.GuidNewManager.Instance.GetProgressByIndex(6))
            //{

            //}


            ResetOfflineTime();
            HttpPingModule.Instance.Enable = true;
        }

        private void SendNeedMoreTime()
        {
            if (2 == SCENE_ID / 10000)
            {
                CSJigsawReconnectRequest req = new CSJigsawReconnectRequest
                {
                    SceneId = SCENE_ID,
                    StartTime = (long)m_offline_time,
                    EndTime = (long)CommonTools.GetCurrentTimeSecond(),
                };

#if !NETWORK_SYNC || UNITY_EDITOR
                GameEvents.NetWorkEvents.SendAsyncMsg.SafeInvoke(req);
#else
            GameEvents.NetWorkEvents.SendMsg.SafeInvoke(req);
#endif

            }
            else if (4 == SCENE_ID / 10000)
            {

            }
            else
            {
                CSSceneReconnectRequest req = new CSSceneReconnectRequest
                {
                    SceneId = SCENE_ID,
                    StartTime = (long)m_offline_time,
                    EndTime = (long)CommonTools.GetCurrentTimeSecond(),
                };

#if !NETWORK_SYNC || UNITY_EDITOR
                GameEvents.NetWorkEvents.SendAsyncMsg.SafeInvoke(req);
#else
            GameEvents.NetWorkEvents.SendMsg.SafeInvoke(req);
#endif

            }
        }

        private void BackToLogin()
        {

            UserBehaviorStatisticsModules.Instance.LogEvent(UBSEventKeyName.back_to_login);

            ResetOfflineTime();
            DestroyRetry();
            EngineCoreEvents.ResourceEvent.LeaveScene.SafeInvoke();
            BigWorldManager.Instance.ClearBigWorld();


            if (GameRoot.instance.GameFSM.GotoState((int)ClientFSM.ClientState.LOGIN))
            {
                EngineCoreEvents.UIEvent.HideFrameThenDestroyEvent.SafeInvoke(UIDefine.UI_COMICS_GUID);
#if UNITY_DEBUG
                FrameMgr.Instance.HideAllFrames(new System.Collections.Generic.List<string>() { UIDefine.UI_GUEST_LOGIN, UIDefine.UI_GM, UIDefine.UI_GUID });

#else
                FrameMgr.Instance.HideAllFrames(new System.Collections.Generic.List<string>() { UIDefine.UI_GUEST_LOGIN ,UIDefine.UI_GUID});
#endif

                GameEvents.UI_Guid_Event.OnClearGuid.SafeInvoke();

            }

            EngineCore.EngineCoreEvents.UIEvent.HideUIEvent.SafeInvoke(UIDefine.UI_SYNC_LOADING);
        }

    }
}