/********************************************************************
	created:  2018-3-29 14:14:49
	filename: ModuleMgr.cs
	author:	  songguangze@outlook.com
	
	purpose:  游戏全局模块管理器
*********************************************************************/
using System;
using System.Collections;
using System.Collections.Generic;

namespace EngineCore
{
    /// <summary>
    /// 游戏全局模块管理器
    /// </summary>
    public class ModuleMgr : Singleton<ModuleMgr>
    {
        private Dictionary<byte, AbstractModule> m_gameModuleDict = new Dictionary<byte, AbstractModule>();
        private bool m_isAllModuleReady = false;


        public ModuleMgr()
        {
        }

        public void Init()
        {
            InitGameModules();

            Start();
        }

        /// <summary>
        /// 初始化所有核心Module
        /// </summary>
        private void InitGameModules()
        {
            Type[] allModuleTypes = ReflectionHelper.GetAssemblyCustomAttributeTypeList<EngineCoreModuleAttribute>();
            for (int i = 0; i < allModuleTypes.Length; ++i)
            {
                EngineCoreModuleAttribute EngineCoreModuleAttribute = allModuleTypes[i].GetCustomAttributes(typeof(EngineCoreModuleAttribute), false)[0] as EngineCoreModuleAttribute;

                AbstractModule targetGameModule = Activator.CreateInstance(allModuleTypes[i], true) as AbstractModule;
                AddModule((byte)EngineCoreModuleAttribute.Type, targetGameModule);
            }
        }

        /// <summary>
        /// 添加Module
        /// </summary>
        /// <param name="moduleType"></param>
        /// <param name="gameModule"></param>
        public void AddModule(byte moduleType, AbstractModule gameModule)
        {
            if (!m_gameModuleDict.ContainsKey(moduleType))
                m_gameModuleDict.Add(moduleType, gameModule);
        }

        /// <summary>
        /// 添加自定义Module
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="moduleType"></param>
        /// <param name="param"></param>
        public void AddModule<T>(byte moduleType, bool autoStart = false, object[] param = null) where T : AbstractModule
        {
            T moduleInstance = Activator.CreateInstance(typeof(T), param) as T;
            AddModule(moduleType, moduleInstance);
            if (moduleInstance.AutoStart || autoStart)
                moduleInstance.Start();
        }


        public void Start()
        {
            foreach (KeyValuePair<byte, AbstractModule> pair in m_gameModuleDict)
            {
                if (pair.Value.AutoStart)
                    pair.Value.Start();
            }

            EngineCoreEvents.BridgeEvent.GetGameRootBehaviour().StartCoroutine(CheckEngineModulesReady());
        }

        private IEnumerator CheckEngineModulesReady()
        {
            foreach (KeyValuePair<byte, AbstractModule> pair in m_gameModuleDict)
            {
                if (!pair.Value.IsModuleStarted)
                {
                    yield return null;
                    EngineCoreEvents.BridgeEvent.GetGameRootBehaviour().StartCoroutine(CheckEngineModulesReady());
                    yield break;
                }
            }
            EngineCoreEvents.SystemEvents.OnEngineReady.SafeInvoke();

            yield break;
        }

        public void Update()
        {
            List<byte> moduleKeys = new List<byte>(m_gameModuleDict.Keys);
            foreach (byte moduleKey in moduleKeys)
                m_gameModuleDict[moduleKey].Update();
        }

        public void LateUpdate()
        {
            foreach (KeyValuePair<byte, AbstractModule> pair in m_gameModuleDict)
                pair.Value.LateUpdate();
        }

        /// <summary>
        /// 获取Module
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="type"></param>
        /// <returns></returns>
        public T GetModule<T>(byte type) where T : AbstractModule
        {
            if (m_gameModuleDict.ContainsKey(type))
                return m_gameModuleDict[type] as T;

            return null;
        }

        /// <summary>
        /// 程序切后台
        /// </summary>
        public void OnApplicationPause()
        {
            List<byte> moduleKeys = new List<byte>(m_gameModuleDict.Keys);
            foreach (byte moduleKey in moduleKeys)
                m_gameModuleDict[moduleKey].OnApplicationPause();
        }

        /// <summary>
        /// 程序唤醒
        /// </summary>
        public void OnApplicationWakeup()
        {
            List<byte> moduleKeys = new List<byte>(m_gameModuleDict.Keys);
            foreach (byte moduleKey in moduleKeys)
                m_gameModuleDict[moduleKey].OnApplicationResume();
        }

        public override void Destroy()
        {
            foreach (KeyValuePair<byte, AbstractModule> pair in m_gameModuleDict)
                pair.Value.Dispose();
        }

    }

    /// <summary>
    /// 引擎核心层模块类型枚举
    /// </summary>
    public enum ModuleType
    {
        TIME_MODULE = 1,            //时间Module
        RESOURCE_MODULE,            //资源管理Module
        NETWORK_MODULE,         //网络管理Module
        UI_MODULE,              //UI管理Module
        INPUT_MODULE,           //输入管理Module
        AUDIO_MODULE,           //声音Module
        SHADER_MODULE,          //全局Shader管理器
    }

    /// <summary>
    /// 全局Module 标记Attribute
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
    public sealed class EngineCoreModuleAttribute : Attribute
    {
        private readonly ModuleType m_moduleType;

        public EngineCoreModuleAttribute(ModuleType moduleType)
        {
            this.m_moduleType = moduleType;
        }

        public ModuleType Type
        {
            get { return this.m_moduleType; }
        }

    }
}
