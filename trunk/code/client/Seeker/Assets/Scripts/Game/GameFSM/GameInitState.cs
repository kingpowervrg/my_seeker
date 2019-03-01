/********************************************************************
	created:  2018-3-26 20:39:15
	filename: GameInitState.cs
	author:	  songguangze@fotoable.com
	
	purpose:  游戏初始化状态
*********************************************************************/
using EngineCore;
using System;
using System.Collections.Generic;

namespace SeekerGame
{
    public class GameInitState : SimpleFSMStateBase
    {
        private Queue<Action> m_gameInitQueue = new Queue<Action>();

        public GameInitState()
        {
            m_gameInitQueue.Enqueue(InitServerIp);
            m_gameInitQueue.Enqueue(InitGlobalInfo);
            m_gameInitQueue.Enqueue(InitGameModules);
        }

        public override void BeginState(int stateFlag)
        {
            LoadNext();
        }



        /// <summary>
        /// 初始化服务器地址
        /// </summary>
        private void InitServerIp()
        {
#if UNITY_DEBUG || UNITY_EDITOR
            GlobalInfo.SERVER_ADDRESS = "http://220.249.55.203:6001";
#else
            GlobalInfo.SERVER_ADDRESS = GameConst.ServerIP;
#endif
        }


        private void InitGlobalInfo()
        {
            GlobalInfo.Instance.Init();
        }


        /// <summary>
        /// 初始化游戏Modules
        /// </summary>
        private void InitGameModules()
        {
            //本地化不属于CoreModule 单独挂载
            ModuleMgr.Instance.AddModule<SceneModule>((byte)GameModuleTypes.SCENE_MODULE, true);
            ModuleMgr.Instance.AddModule<HttpPingModule>((byte)GameModuleTypes.PING_MODULE, true);
            ModuleMgr.Instance.AddModule<UserBehaviorStatisticsModules>((byte)GameModuleTypes.UBS_MODULE, true);
            //#if UNITY_ANDROID || UNITY_IOS
            //ModuleMgr.Instance.AddModule<GameIAPModule>((byte)GameModuleTypes.IAP_MODULE, true);
            //#endif

            if (EngineDelegateCore.DynamicResource)
                ModuleMgr.Instance.AddModule<GameUpdateModule>((byte)GameModuleTypes.GAMEUPDATE_MODULE, true);


            //实体管理器
            EntityManager.Instance.InitEntityManager();

#if UNITY_DEBUG

            //Log 模块
            //Reporter.instance.show = false;
#endif
            MsgReflection.Descriptor.GetType();
        }



        private void LoadNext()
        {
            if (m_gameInitQueue.Count > 0)
            {
                Action initAction = m_gameInitQueue.Dequeue();
                initAction();
                LoadNext();
            }
            else
            {
                GameRoot.instance.GameFSM.GotoState((int)ClientFSM.ClientState.PREGAME);
            }
        }
    }
}