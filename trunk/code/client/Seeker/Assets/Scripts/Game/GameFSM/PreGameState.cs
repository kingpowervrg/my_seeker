/********************************************************************
	created:  2018-7-18 21:46:44
	filename: PreGameState.cs
	author:	  songguangze@outlook.com
	
	purpose:  预游戏状态,用于检查更新
*********************************************************************/
using EngineCore;
using SqliteDriver;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SeekerGame
{
    public class PreGameState : SimpleFSMStateBase
    {
        private Queue<Action> m_preGameQueue = new Queue<Action>();

        //private List<string> m_preloadAssetsList = new List<string>();
        private List<string> m_persistAssetList = new List<string>()
        {
            GameConst.EXHIBIT_ICON_BUNDLE,
            GameConst.ATLAS_ICON_BUNDLE,
            GameConst.LOADING_IMAGE_BUNDLE,
            "UI_jindutiao.prefab",
            UIDefine.UI_Loading,
            GameConst.COMMON_BG
        };


        /// <summary>
        /// 异步预加载资源
        /// </summary>
        private List<string> m_asyncPreloadBundleList = new List<string>()
        {
            GameConst.ATLAS_ICON_BUNDLE,
            GameConst.EXHIBIT_ICON_BUNDLE,
            GameConst.COMMON_BG,
        };

        /// <summary>
        /// 同步预加载资源
        /// </summary>
        private List<string> m_syncPreloadBundleList = new List<string>()
        {
            GameConst.LOADING_IMAGE_BUNDLE,
            "UI_jindutiao.prefab"
        };


        private int m_remainPreloadCount = 0;
        private int m_totalPreloadCount = 0;

        public override void BeginState(int stateFlag)
        {
            EngineCoreEvents.ResourceEvent.IsLoadAssetBundleAllAssets = IsLoadAllAsset;

            //AddPreloadAssets();
            InitPreloadQueue();
            if (EngineDelegateCore.DynamicResource)
            {
                if (!string.IsNullOrEmpty(GameConst.MARKET_FLAG))
                {
                    MessageHandler.RegisterMessageHandler(MessageDefine.SCGetPathConfigResponse, OnGetVersion);

                    CSGetPathConfigRequest reqVersion = new CSGetPathConfigRequest();
                    reqVersion.Plat = GameConst.MARKET_FLAG;

                    GameEvents.NetWorkEvents.SendMsg.SafeInvoke(reqVersion);
                }
                else
                {
                    SysConf.GAME_RES_URL = "http://resourcetest.fotoable-conan.com/download/windows/";
                    InitAsset();
                }
            }
            else
            {
                GlobalInfo.GAME_VERSION = Application.version;

                InitAsset();
            }
        }

        private void InitAsset()
        {
            if (EngineDelegateCore.DynamicResource)
                GameUpdateModule.Instance.CheckGameUpdate(OnCheckedUpdateFinish);
            else
                InitClientConfig();
        }



        private void InitPreloadQueue()
        {
            this.m_preGameQueue.Clear();
            this.m_preGameQueue.Enqueue(ReloadGameModules);
            this.m_preGameQueue.Enqueue(DownloadOptionalAssets);
            this.m_preGameQueue.Enqueue(PreloadAssets);
            this.m_preGameQueue.Enqueue(InitLoadingSystem);
            this.m_preGameQueue.Enqueue(InitSDKBridge);

            this.m_totalPreloadCount = this.m_preGameQueue.Count;
        }


        private void OnCheckedUpdateFinish(int updateAssetsCount)
        {
            if (updateAssetsCount <= 0)
            {
                InitClientConfig();
            }
            else
            {
                PregameUILogic.instance.SetToSegmentStart(1, 0);

                GameUpdateModule.Instance.StartGameUpdate(OnFinishUpdate,
                    assetUpdateInfo =>
                    {
                        PregameUILogic.instance.SetDeltaValue((float)(assetUpdateInfo.TotalAssetCount - assetUpdateInfo.RemainDownloadAssetCount) / assetUpdateInfo.TotalAssetCount);
                    });
            }
        }

        private void OnFinishUpdate(AssetUpdateInfo assetUpdateInfo)
        {
            InitClientConfig();
        }


        /// <summary>
        /// 初始化加载队列
        /// </summary>
        private void InitClientConfig()
        {
#if UNITY_DEBUG
            EngineCoreEvents.SystemEvents.NetWorkError += OnServerError;
#endif

            PregameUILogic.instance.SetToSegmentStart(2);

            ConfFact.Register();
            ClientConfigManager.Instance.Load();
            if (SQLiteHelper.Instance() != null)
                SQLiteHelper.Instance().CloseSql();

            SQLiteLoad.LoadSQLite(() =>
            {
                SQLiteHelper.Instance();

                LoadNext();
            });
        }

        private void OnServerError(int errorCode, string errorContent)
        {
            MsgStatusCodeUtil.OnError(errorCode);
        }

        /// <summary>
        /// 下载可选资源
        /// </summary>
        private void DownloadOptionalAssets()
        {
            if (EngineDelegateCore.DynamicResource)
            {
                List<ConfAssetManifest> optionalAssets;
                ConfAssetManifest.GetConfigByCondition("AssetLevel = '1'", out optionalAssets);
                string[] optionalAssetsList = optionalAssets.Select(assetManifest => assetManifest.AssetBundleName).ToArray();

                ResourceMgr.Instance.RequestDynamicAssets(optionalAssetsList, assetUpdateInfo =>
                {
                    float delta = 1 - ((float)assetUpdateInfo.RemainDownloadAssetCount / assetUpdateInfo.TotalAssetCount);
                    Debug.Log("total count :" + assetUpdateInfo.TotalAssetCount + "   remain count :" + assetUpdateInfo.RemainDownloadAssetCount);
                    PregameUILogic.instance.SetDeltaValue(delta * (1.0f / m_totalPreloadCount));
                }, finish =>
                {
                    LoadNext();
                });
            }
            else
                LoadNext();
        }


        private void OnGetVersion(object message)
        {
            MessageHandler.UnRegisterMessageHandler(MessageDefine.SCGetPathConfigResponse, OnGetVersion);
            SCGetPathConfigResponse msgVersionInfo = message as SCGetPathConfigResponse;

            GetPathInfo versionInfo = msgVersionInfo.Infos;

            GlobalInfo.GAME_VERSION = versionInfo.Version;

            string[] serverVersion = versionInfo.Version.Split('.');
            string[] localVersion = Application.version.Split('.');

            if (serverVersion[0] != localVersion[0] || serverVersion[1] != localVersion[1])
            {
                UpdateTool.OpenAppStore();
            }
            else
            {
                SysConf.GAME_RES_URL = versionInfo.Path;
                InitAsset();
            }
        }


        private void PreloadAssets()
        {
            this.m_remainPreloadCount = this.m_syncPreloadBundleList.Count;
            for (int i = 0; i < this.m_syncPreloadBundleList.Count; ++i)
                ResourceModule.Instance.PreloadBundle(this.m_syncPreloadBundleList[i], OnPreloadAssetsCallback);

            for (int i = 0; i < this.m_asyncPreloadBundleList.Count; ++i)
                ResourceModule.Instance.PreloadBundle(this.m_asyncPreloadBundleList[i], null);
        }

        private void OnPreloadAssetsCallback(string name, AssetBundle unityAssetBundle)
        {
            if (this.m_persistAssetList.Contains(name))
                ResourceModule.Instance.SetAssetBundlePersistent(name);

            this.m_remainPreloadCount--;
            if (this.m_remainPreloadCount <= 0)
                LoadNext();
        }

        private void ReloadGameModules()
        {
            LocalizeModule localizeModule = ModuleMgr.Instance.GetModule<LocalizeModule>((byte)GameModuleTypes.LOCALIZE_MODULE);
            if (localizeModule == null)
                ModuleMgr.Instance.AddModule<LocalizeModule>((byte)GameModuleTypes.LOCALIZE_MODULE, true);
            else
                localizeModule.InitLocalizeLanguage();

            //#if UNITY_ANDROID || UNITY_IOS
            GameIAPModule IAPModule = ModuleMgr.Instance.GetModule<GameIAPModule>((byte)GameModuleTypes.IAP_MODULE);
            if (IAPModule == null)
                ModuleMgr.Instance.AddModule<GameIAPModule>((byte)GameModuleTypes.IAP_MODULE, true);

#if UNITY_DEBUG
            GMModule.InitAllGMCommand();
#endif

            GameEvents.System_Events.PlayMainBGM(true);

            LoadNext();
        }


        private void InitLoadingSystem()
        {
            LoadingManager.Instance.Init();

            LoadNext();
        }

        private void InitSDKBridge()
        {
            //GlobalInfo.Instance.InitAudioBridge();

            LoadNext();
        }

        private void LoadNext()
        {
            float preloadProgress = 1 - ((float)this.m_preGameQueue.Count / this.m_totalPreloadCount);
            PregameUILogic.instance.SetDeltaValue(preloadProgress);

            if (this.m_preGameQueue.Count > 0)
            {
                Action pregameAction = this.m_preGameQueue.Dequeue();
                GameRoot.instance.StartCoroutine(DoLoad(pregameAction));
            }
            else
            {
                GameRoot.instance.GameFSM.GotoState((int)ClientFSM.ClientState.PROLOGUE);
            }
        }


        private bool IsLoadAllAsset(string bundleName)
        {
            return this.m_persistAssetList.Contains(bundleName);
        }

        IEnumerator DoLoad(Action loadAction)
        {
            yield return null;
            loadAction?.Invoke();
        }
    }
}