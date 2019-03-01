/********************************************************************
	created:  2018-3-29 14:14:49
	filename: ModuleMgr.cs
	author:	  songguangze@outlook.com
	
	purpose:  游戏全局模块管理器
*********************************************************************/
using System;
using System.Collections.Generic;

namespace EngineCore
{
    /// <summary>
    /// 游戏全局模块管理器
    /// </summary>
    public class ModuleMgr : Singleton<ModuleMgr>
    {
        private Dictionary<ModuleType, AbstractModule> m_gameModuleDict = new Dictionary<ModuleType, AbstractModule>();

        public ModuleMgr()
        {
        }

        public void Init()
        {
            InitGameModules();
        }

        /// <summary>
        /// 初始化所有Module
        /// </summary>
        private void InitGameModules()
        {
            Type[] allModuleTypes = ReflectionHelper.GetAssemblyCustomAttributeTypeList<GameModuleAttribute>(typeof(IGameModule));
            for (int i = 0; i < allModuleTypes.Length; ++i)
            {
                GameModuleAttribute gameModuleAttribute = allModuleTypes[i].GetCustomAttributes(typeof(GameModuleAttribute), false)[0] as GameModuleAttribute;

                AbstractModule targetGameModule = Activator.CreateInstance(allModuleTypes[i], true) as AbstractModule;
                AddModule(gameModuleAttribute.Type, targetGameModule);
            }
        }


        public void AddModule(ModuleType moduleType, AbstractModule gameModule)
        {
            if (!m_gameModuleDict.ContainsKey(moduleType))
                m_gameModuleDict.Add(moduleType, gameModule);
        }

        public void Start()
        {
            foreach (KeyValuePair<ModuleType, AbstractModule> pair in m_gameModuleDict)
                pair.Value.Start();
        }

        public void Update()
        {
            foreach (KeyValuePair<ModuleType, AbstractModule> pair in m_gameModuleDict)
                pair.Value.Update();
        }

        public void LateUpdate()
        {
            foreach (KeyValuePair<ModuleType, AbstractModule> pair in m_gameModuleDict)
                pair.Value.LateUpdate();
        }

        /// <summary>
        /// 获取Module
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="type"></param>
        /// <returns></returns>
        public T GetModule<T>(ModuleType type) where T : AbstractModule
        {
            if (m_gameModuleDict.ContainsKey(type))
                return m_gameModuleDict[type] as T;

            return null;
        }

    }

    /// <summary>
    /// 模型类型枚举
    /// </summary>
    public enum ModuleType : byte
    {
        TIME_MODULE,            //时间Module
        RESOURCE_MODULE,        //资源管理Module
        LOCALIZE_MODULE,        //本地化Module
        NETWORK_MODULE,         //网络管理Module
        UI_MODULE,              //UI管理Module
    }

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
    public sealed class GameModuleAttribute : Attribute
    {
        private readonly ModuleType m_moduleType;
        public GameModuleAttribute(ModuleType moduleType)
        {
            this.m_moduleType = moduleType;
        }

        public ModuleType Type
        {
            get { return this.m_moduleType; }
        }

    }
}
