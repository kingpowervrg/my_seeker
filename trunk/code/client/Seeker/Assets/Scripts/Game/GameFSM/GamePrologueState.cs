using EngineCore;
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using SeekerGame.NewGuid;

namespace SeekerGame
{
    class GamePrologueState : SimpleFSMStateBase
    {
        public GamePrologueState()
        { }

        private Queue<Action> m_gameStateExecuteQueue = new Queue<Action>();

        //进入GameState预加载的资源
        private List<string> m_preloadAssetsList = new List<string>()
        {
            GameConst.SHADER_BUNDLE,
        };

        private int m_remainPreloadCount = 0;

        public override void BeginState(int stateFlag)
        {
            this.m_gameStateExecuteQueue.Enqueue(PreloadAssets);
            this.m_gameStateExecuteQueue.Enqueue(InitGameLogicModules);
            this.m_gameStateExecuteQueue.Enqueue(InitPrologue);
            LoadNext();
        }

        public override void EndState(int nextState)
        {

        }

        private void InitPrologue()
        {
            GuidNewManager.Instance.OnClearGuid();
            GuidNewManager.Instance.LoadCurrentProgressValueForLocal();
            if (GuidNewManager.Instance.CheckNeedSkipLocalGuid())
            {
                GameRoot.instance.GameFSM.GotoState((int)ClientFSM.ClientState.LOGIN);
            }
            else
            {
                GlobalInfo.GAME_NETMODE = GameNetworkMode.Standalone;
                GlobalInfo.Instance.CreatePlayerInfo();
                PregameUILogic.instance.Destory();
                TaskOnBuildManager.Instance.SyncRecord();
                GlobalInfo.MY_PLAYER_INFO.InitPlayerChapterSystem();
                GlobalInfo.MY_PLAYER_INFO.InitPlayerTaskSystem();
                JigsawDataManager.Instance.InitJigsawData();
                GuidNewManager.Instance.OnLoad();
                //if (GuidNewManager.Instance.GetProgressByIndex(1))
                //{
                //    BigWorldManager.Instance.LoadBigWorld();
                //}
            }
            LoadNext();
        }


        /// <summary>
        /// 初始化游戏逻辑Module
        /// </summary>
        private void InitGameLogicModules()
        {
            UserBehaviorStatisticsModules.Instance.LogEvent(UBSEventKeyName.InitGameLogicModules);

            //ModuleMgr.Instance.AddModule<SceneModule>((byte)GameModuleTypes.SCENE_MODULE, true);

            /*******
             * 内购初始化会激发上次未完成的订单，所以需要登陆后，获得playerid后，初始化。
             */
            //#if UNITY_ANDROID || UNITY_IOS
            GameIAPModule IAPModule = ModuleMgr.Instance.GetModule<GameIAPModule>((byte)GameModuleTypes.IAP_MODULE);
            IAPModule.InitIAP();
            //#endif

            LoadNext();
        }

        private void PreloadAssets()
        {
            UserBehaviorStatisticsModules.Instance.LogEvent(UBSEventKeyName.PreloadAssets);

            this.m_remainPreloadCount = this.m_preloadAssetsList.Count;
            for (int i = 0; i < this.m_preloadAssetsList.Count; ++i)
                ResourceModule.Instance.PreloadBundle(this.m_preloadAssetsList[i], OnPreloadAssetBundle);
        }

        private void OnPreloadAssetBundle(string name, AssetBundle preloadAssetBundle)
        {
            this.m_remainPreloadCount--;
            if (name == GameConst.SHADER_BUNDLE)
            {
                ResourceModule.Instance.SetAssetBundlePersistent(name);
                Shader[] shaders = preloadAssetBundle.LoadAllAssets<Shader>();
                ShaderModule.Instance.AddShaderBundle(shaders);
            }

            if (this.m_remainPreloadCount <= 0)
                LoadNext();
        }

        private void LoadNext()
        {
            if (this.m_gameStateExecuteQueue.Count > 0)
            {
                Action pregameAction = this.m_gameStateExecuteQueue.Dequeue();
                GameRoot.instance.StartCoroutine(DoLoad(pregameAction));
            }
        }

        IEnumerator DoLoad(Action loadAction)
        {
            yield return null;
            loadAction?.Invoke();
        }
    }
}
