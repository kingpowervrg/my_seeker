using EngineCore;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace SeekerGame
{
    public class UserBehaviorStatisticsModules : AbstractModule
    {
        private List<IUBSBridge> m_bridges;

        private static UserBehaviorStatisticsModules m_intance;

        public static UserBehaviorStatisticsModules Instance
        {
            get { return m_intance; }
        }

        private Dictionary<string, object> m_base_params;


        public UserBehaviorStatisticsModules()
        {
            m_intance = this;
            AutoStart = true;
        }

        public override void Start()
        {
            base.Start();
#if UNITY_EDITOR || !UNITY_STANDALONE_WIN
            UBSBaseData.m_player_id = GlobalInfo.MY_PLAYER_ID.ToString();
            UBSBaseData.m_operating_sys = SystemInfo.operatingSystem;
            UBSBaseData.m_device_model = SystemInfo.deviceModel;
            UBSBaseData.m_game_version = GlobalInfo.GAME_VERSION;
            UBSBaseData.m_app_version = Application.version;

            m_bridges = new List<IUBSBridge>();
            Type[] allModuleTypes = ReflectionHelper.GetAssemblyCustomAttributeTypeList<UBSBridgeAttribute>();
            for (int i = 0; i < allModuleTypes.Length; ++i)
            {
                IUBSBridge targetGameModule = Activator.CreateInstance(allModuleTypes[i], true) as IUBSBridge;

                //解决平台问题，在对应的Bridge里用initialize 控制是否加入Module的BridgeList
                targetGameModule.StartBridge();
                if (targetGameModule.IsInitialized)
                    m_bridges.Add(targetGameModule);
            }

#endif
        }


        public override void Dispose()
        {
            base.Dispose();
#if UNITY_EDITOR || !UNITY_STANDALONE_WIN
            foreach (var bridge in m_bridges)
            {
                bridge.DisposeBridge();
            }
#endif
        }

        protected override void setEnable(bool value)
        {
            base.setEnable(value);
        }


        public override void OnApplicationPause()
        {
            base.OnApplicationPause();
        }

        public override void OnApplicationResume()
        {
            base.OnApplicationResume();
        }



        public void LogEvent(UBSEventKeyName key_)
        {
#if !UNITY_EDITOR && (UNITY_IOS || UNITY_ANDROID)

            foreach (var bridge in m_bridges)
            {
                try
                {
                    bridge.LogEvent(key_);
                }
                catch (System.Exception ex)
                {
                    Debug.LogError("UBS ERROR LogEvent(string key_) : " + ex);
                    continue;
                }
            }
#endif
        }

        public void LogEvent(UBSEventKeyName key_, float? value4sum_)
        {
#if !UNITY_EDITOR &&(  UNITY_IOS || UNITY_ANDROID )
            foreach (var bridge in m_bridges)
            {
                try
                {
                    bridge.LogEvent(key_, value4sum_);
                }
                catch (System.Exception ex)
                {
                    Debug.LogError("UBS ERROR LogEvent(string key_, float value_) : " + ex);
                    continue;
                }
            }
#endif
        }

        public void LogEvent(UBSEventKeyName key_, float? value4sum_, Dictionary<UBSParamKeyName, object> params_)
        {
#if !UNITY_EDITOR &&(  UNITY_IOS || UNITY_ANDROID )
            foreach (var bridge in m_bridges)
            {
                try
                {
                    bridge.LogEvent(key_, value4sum_, params_);
                }
                catch (System.Exception ex)
                {
                    Debug.LogError("UBS ERROR LogEvent(string key_, float value_, Dictionary<string, object> params_) : " + ex);
                    continue;
                }
            }
#endif
        }

    }
}
