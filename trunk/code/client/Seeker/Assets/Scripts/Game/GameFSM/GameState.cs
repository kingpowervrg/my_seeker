/********************************************************************
	created:  2018-4-8 11:28:58
	filename: GameState.cs
	author:	  songguangze@outlook.com
	
	purpose:  游戏状态
*********************************************************************/

#define GUEST_LOGIN
//#define GUID

using EngineCore;
using SeekerGame.NewGuid;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SeekerGame
{
    public class GameState : SimpleFSMStateBase
    {
        private Queue<Action> m_gameStateExecuteQueue = new Queue<Action>();

        //进入GameState预加载的资源
        private List<string> m_preloadAssetsList = new List<string>()
        {
            GameConst.SHADER_BUNDLE,
        };

        private int m_remainPreloadCount = 0;

        public override void BeginState(int stateFlag)
        {
            //this.m_gameStateExecuteQueue.Enqueue(PreloadAssets);
            //this.m_gameStateExecuteQueue.Enqueue(InitGameLogicModules);
            this.m_gameStateExecuteQueue.Enqueue(InitWorld);
            this.m_gameStateExecuteQueue.Enqueue(EnablePing);
            this.m_gameStateExecuteQueue.Enqueue(SyncOther);


            LoadNext();
            //signInSystem.GetSignInData();
        }


        private void InitWorld()
        {
            UserBehaviorStatisticsModules.Instance.LogEvent(UBSEventKeyName.InitWorld);

            GuidNewManager.Instance.OnLoad();
            //InitGameLogicModules();
            if (GuidNewManager.Instance.GetProgressByIndex(6))
            {
                BigWorldManager.Instance.LoadBigWorld("GiftView");
            }
            else
            {
                GuidNewNodeManager.Instance.SetCanCartoon(true);
                GuidNewModule.Instance.OnTryStart();
            }

            SyncRedPoint();//以来ping，所以要在ping之前启动

            SceneModule.Instance.CreateScene(SceneMode.NORMALSCENE);


#if OFFICER_SYS
            SyncPlayerOfficerInfo();
#endif

            SyncPlayerTaskInfo();
            SyncPlayerChapterInfo();

            SyncPlayerMailInfo();

#if GUEST_LOGIN

            EngineCoreEvents.UIEvent.HideUIEvent.SafeInvoke(UIDefine.UI_GUEST_LOGIN);
            EngineCoreEvents.UIEvent.HideUIEvent.SafeInvoke(UIDefine.UI_GUIDLOGIN);
#else
            EngineCoreEvents.UIEvent.HideUIEvent.SafeInvoke(UIDefine.UI_LOGIN);
#endif
            TaskQueryManager.Instance.RequestTaskComplete();


            TaskOnBuildManager.Instance.SyncRecord();

            TimeModule.Instance.SetTimeout(LoadNext, 0.5f);


            JigsawDataManager.Instance.InitJigsawData();


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

#if OFFICER_SYS
        private void SyncPlayerOfficerInfo()
        {
            CSOfficerListRequest req = new CSOfficerListRequest();
            req.PlayerId = GlobalInfo.MY_PLAYER_ID;
            //GameEvents.NetWorkEvents.SendMsg.SafeInvoke(req);
            GameEvents.NetWorkEvents.SendAsyncMsg.SafeInvoke(req);

            Debug.Log("send CSOfficerListRequest");
        }
#endif

        /// <summary>
        /// 初始化玩家任务系统
        /// </summary>
        private void SyncPlayerTaskInfo()
        {
            GlobalInfo.MY_PLAYER_INFO.InitPlayerTaskSystem();
        }

        /// <summary>
        /// 初始化玩家邮件系统
        /// </summary>
        private void SyncPlayerMailInfo()
        {
            GlobalInfo.MY_PLAYER_INFO.InitPlayerMailSystem();
        }

        /// <summary>
        /// 初始化玩家章节信息系统
        /// </summary>
        private void SyncPlayerChapterInfo()
        {
            GlobalInfo.MY_PLAYER_INFO.InitPlayerChapterSystem();
        }

        private void SyncTitle()
        {
            GameEvents.PlayerEvents.Listen_SyncTitle();
        }

        private void SyncAchievement()
        {
            AchievementManager.Instance.Init();
            GameEvents.PlayerEvents.RequestRecentAhievement.SafeInvoke();
        }

        private void SyncRedPoint()
        {
            RedPointManager.Instance.Sync();
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

        public override void EndState(int nextState)
        {
            GlobalInfo.MY_PLAYER_INFO.Dispose();

            HttpPingModule.Instance.Enable = false;

            EngineCoreEvents.UIEvent.HideUIEvent.SafeInvoke(UIDefine.UI_GAMEENTRY);

            EngineCoreEvents.UIEvent.HideUIEvent.SafeInvoke(UIDefine.UI_BANNER);

            EngineCoreEvents.UIEvent.HideUIEvent.SafeInvoke(UIDefine.UI_BUILD_TOP);

            EngineCoreEvents.UIEvent.HideUIEvent.SafeInvoke(UIDefine.UI_TASK_ON_BUILD);
        }

        private void EnablePing()
        {
            UserBehaviorStatisticsModules.Instance.LogEvent(UBSEventKeyName.EnablePing);

            SyncTitle();
            //同步成就
            SyncAchievement();

            HttpPingModule.Instance.Enable = true;

            LoadingManager.Instance.CreateRetry();

            TimeModule.Instance.SetTimeout(LoadNext, 0.5f);
        }

        private void SyncOther()
        {
            FriendDataManager.Instance.RefreshAllInfo();
            CombineDataManager.Instance.Sync();
        }

    }
}