/********************************************************************
	created:  2018-4-2 10:36:46
	filename: UILogicMapper.cs
	author:	  songguangze@outlook.com
	
	purpose:  UI Prefab与逻辑映射类
*********************************************************************/
using System;
using System.Collections.Generic;
using UnityEngine;

namespace EngineCore
{
    public static class UILogicMapper
    {
        private static Dictionary<string, Type> m_uilogicTypeDict = new Dictionary<string, Type>();

        /// <summary>
        /// 初始化所有UI逻辑映射关系
        /// </summary>
        public static void Initialize()
        {
            Type[] typeOfUILogicHandlers = ReflectionHelper.GetAssemblyCustomAttributeTypeList<UILogicHandlerAttribute>();

            foreach (Type uilogicHandlerType in typeOfUILogicHandlers)
            {
                UILogicHandlerAttribute logicHandlerAttribute = uilogicHandlerType.GetCustomAttributes(typeof(UILogicHandlerAttribute), false)[0] as UILogicHandlerAttribute;

                m_uilogicTypeDict.Add(logicHandlerAttribute.PrefabName, uilogicHandlerType);
            }
        }


        /// <summary>
        /// 创建Prefab对应的逻辑对象
        /// </summary>
        /// <param name="uiprefabName"></param>
        /// <returns></returns>
        public static UILogicBase CreateUILogic(string uiprefabName)
        {
            if (m_uilogicTypeDict.ContainsKey(uiprefabName))
            {
                return Activator.CreateInstance(m_uilogicTypeDict[uiprefabName]) as UILogicBase;
            }
            else
            {
                Debug.LogError("can't find target uiloginc:" + uiprefabName);
                return null;
            }
        }


        /// <summary>
        /// 对应的GUIFrame创建逻辑对象
        /// </summary>
        /// <param name="guiFrame"></param>
        public static void MakeUILogic(GUIFrame guiFrame)
        {
            UILogicBase logicHandler = CreateUILogic(guiFrame.ResName);
            if (logicHandler == null)
                throw new Exception("no logic :" + guiFrame.ResName);

            guiFrame.LogicHandler = logicHandler;
            guiFrame.LogicHandler.Init(guiFrame);
        }

    }

    /// <summary>
    /// UI逻辑Attribute
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = true)]
    public class UILogicHandlerAttribute : Attribute
    {
        private readonly string m_uiPrefabName;

        public UILogicHandlerAttribute(string prefabName)
        {
            this.m_uiPrefabName = prefabName;
        }

        public string PrefabName
        {
            get { return this.m_uiPrefabName; }
        }

    }
}