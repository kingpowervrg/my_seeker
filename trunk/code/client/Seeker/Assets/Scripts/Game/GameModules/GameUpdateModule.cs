/********************************************************************
	created:  2018-7-20 14:16:4
	filename: GameUpdateModule.cs
	author:	  songguangze@outlook.com
	
	purpose:  更新Module
*********************************************************************/
using EngineCore;
using System;
using UnityEngine;

namespace SeekerGame
{
    public class GameUpdateModule : AbstractModule
    {
        private static GameUpdateModule m_instance = null;

        private Action<int> OnCheckUpdateResultCallback = null;

        public GameUpdateModule()
        {
            AutoStart = true;
            m_instance = this;
        }

        public override void Start()
        {
            base.Start();
        }

        public void CheckGameUpdate(Action<int> CheckUpdateResultCallback)
        {
            this.OnCheckUpdateResultCallback = CheckUpdateResultCallback;

#if UNITY_ANDROID || UNITY_IOS
            EngineCoreEvents.ResourceEvent.CheckUpdate.SafeInvoke(OnCheckUpdateResultCallback);
#else
            CheckUpdateResultCallback(0);
#endif
        }

        public void StartGameUpdate(Action<AssetUpdateInfo> OnFinishUpdate, Action<AssetUpdateInfo> UpdateProgressCallback)
        {
            EngineCoreEvents.ResourceEvent.BeginUpdateAssets.SafeInvoke(OnFinishUpdate, UpdateProgressCallback);
        }

        /// <summary>
        /// 资源服务器连接出错
        /// </summary>
        private void AssetServerError(string errorMsg)
        {
            PopUpManager.OpenPopUp(new PopUpData()
            {
                content = errorMsg,
                isOneBtn = true,
            });
        }

        public static GameUpdateModule Instance
        {
            get { return m_instance; }
        }
    }
}