/********************************************************************
	created:  2018-3-28 19:40:26
	filename: PackByDir.cs
	author:	  songguangze@outlook.com
	
	purpose:  打包分包系统
*********************************************************************/
using GOEditor;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace EngineCore.Editor
{
    public class AssetPackSystem
    {
        private static Dictionary<string, Type> m_packHandlerDict = new Dictionary<string, Type>();
        public static bool IsInitPackSystem = false;

        public static void InitAssetPackSystem()
        {
            //if (!IsInitPackSystem)
            //{
            m_packHandlerDict.Clear();

            Type[] packHandlerTypes = ReflectionHelper.GetAssemblyCustomAttributeTypeList<PackTypeAttribute>(typeof(PackHandlerBase));

            for (int i = 0; i < packHandlerTypes.Length; ++i)
            {
                PackTypeAttribute packTypeHandler = packHandlerTypes[i].GetCustomAttributes(typeof(PackTypeAttribute), false)[0] as PackTypeAttribute;

                m_packHandlerDict.Add(packTypeHandler.PackTypeName, packHandlerTypes[i]);
            }

            //IsInitPackSystem = true;
            //}
        }

        /// <summary>
        /// 创建具体分包对象
        /// </summary>
        /// <param name="packType"></param>
        /// <param name="packSetting"></param>
        /// <param name="packBundleSetting"></param>
        /// <returns></returns>
        public static PackHandlerBase CreatePackHalder(string packType, GOEPackV5 packSetting, PackBundleSetting packBundleSetting)
        {
            //if (!IsInitPackSystem)
            InitAssetPackSystem();

            if (!m_packHandlerDict.ContainsKey(packType))
            {
                Debug.LogError("can't implement packtype: " + packType);
                return null;
            }

            Type handlerType = m_packHandlerDict[packType];

            PackHandlerBase handlerInstance = Activator.CreateInstance(handlerType, new object[] { packSetting, packBundleSetting }) as PackHandlerBase;

            return handlerInstance;
        }

    }

}