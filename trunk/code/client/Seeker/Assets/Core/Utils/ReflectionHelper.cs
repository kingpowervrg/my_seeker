/********************************************************************
	created:  2013-8-2  13:38:32
	filename: ReflectionHelper.cs
	author:	  songguangze@outlook.com
	
	purpose:  反射通用帮助类
*********************************************************************/

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace EngineCore
{
    public static class ReflectionHelper
    {
        public static Dictionary<Type, List<Type>> AssemblyWithCustomAttributeDict = new Dictionary<Type, List<Type>>();

        public static void InitAssembly()
        {
            Assembly assemblyInfo = Assembly.GetExecutingAssembly();
            Type[] assemblyTypes = assemblyInfo.GetTypes();

            for (int i = 0; i < assemblyTypes.Length; ++i)
            {
                Type typeInAssembly = assemblyTypes[i];
                object[] typeAttributes = typeInAssembly.GetCustomAttributes(false);
                if (typeAttributes.Length == 1)
                {
                    List<Type> attributeTypeList;
                    if (!AssemblyWithCustomAttributeDict.TryGetValue(typeAttributes[0].GetType(), out attributeTypeList))
                    {
                        attributeTypeList = new List<Type>();
                        AssemblyWithCustomAttributeDict.Add(typeAttributes[0].GetType(), attributeTypeList);
                    }

                    attributeTypeList.Add(typeInAssembly);
                }
            }
        }

        /// <summary>
        /// 根据对象创建新的对象
        /// </summary>
        /// <param name="valueObject"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public static object CreateInstanceFromValue(object valueObject, object[] args)
        {
            Type objectType = (valueObject is Type) ? (valueObject as Type) : valueObject.GetType();
            object newObject = null;
            if (args == null || args.Length == 0)
                newObject = Activator.CreateInstance(objectType);
            else
                newObject = Activator.CreateInstance(objectType, args);

            return newObject;
        }

        /// <summary>
        /// 根据对象创建新的对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="valueObject"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public static T CreateInstanceFromValue<T>(object valueObject, object[] args) where T : class
        {
            object newObject = CreateInstanceFromValue(valueObject, args);

            return newObject as T;
        }

        /// <summary>
        /// 反射创建泛型Dictionary
        /// </summary>
        /// <param name="keyType">Key类型</param>
        /// <param name="valueType">Value类型</param>
        /// <param name="keyAsObj">key值数组</param>
        /// <param name="valueAsObj">Value值数组</param>
        /// <returns></returns>
        public static IDictionary CreateGenericDictionary(Type keyType, Type valueType, object[] keyAsObj, object[] valueAsObj)
        {
            if (keyAsObj.Length != valueAsObj.Length)
                throw new Exception("Key长度与Value长度不一致");

            Type genericDictionaryType = typeof(Dictionary<,>).MakeGenericType(keyType, valueType);
            IDictionary dictionary = CreateInstanceFromValue(genericDictionaryType, null) as IDictionary;
            for (int i = 0; i < keyAsObj.Length; ++i)
                dictionary.Add(keyAsObj[i], valueAsObj[i]);

            return dictionary;
        }

        /// <summary>
        /// 获取变量名称
        /// </summary>
        /// <param name="variableOwner"></param>
        /// <param name="variable"></param>
        /// <returns></returns>
        public static string GetVariableName(object variableOwner, object variable)
        {
            Type ownerType = variableOwner.GetType();
            FieldInfo[] ownerFields = ownerType.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.FlattenHierarchy | BindingFlags.NonPublic);

            foreach (FieldInfo fieldInfo in ownerFields)
            {
                object fieldValue = fieldInfo.GetValue(variableOwner);
                if (fieldValue == variable)
                    return fieldInfo.Name;
            }

            return string.Empty;
        }

        /// <summary>
        /// 根据变量名获取变量的值
        /// </summary>
        /// <param name="variableOwner"></param>
        /// <param name="variableName"></param>
        /// <param name="variableType"></param>
        /// <returns></returns>
        public static object GetVariableValueByVariableName(object variableOwner, string variableName, Type variableType = null)
        {
            Type ownerType = variableOwner.GetType();

            if (variableType == null)
            {
                FieldInfo fieldInfo = ownerType.GetField(variableName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.FlattenHierarchy | BindingFlags.NonPublic);
                return fieldInfo.GetValue(variableOwner);
            }
            else
            {
                FieldInfo[] fieldInfos = ownerType.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.FlattenHierarchy | BindingFlags.NonPublic);
                for (int i = 0; i < fieldInfos.Length; ++i)
                {
                    if (fieldInfos[i].FieldType == variableType)
                        return fieldInfos[i].GetValue(variableOwner);
                }
            }

            return null;
        }

        /// <summary>
        /// 获取类最顶级父类type
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static Type GetHeightestParentTypeByType(Type type)
        {
            if (type.BaseType == null || type.BaseType == typeof(object))
                return type;

            return GetHeightestParentTypeByType(type.BaseType);
        }

        /// <summary>
        /// 获取程序集里标记自定义Attribute的Type列表
        /// </summary>
        /// <param name="typeInAssembly"></param>
        /// <returns></returns>
        /// <remarks>注意使用方式,会产品大量GCAlloc</remarks>
        public static Type[] GetAssemblyCustomAttributeTypeList<T>(Type typeInAssembly) where T : Attribute
        {
            Assembly assemblyInfo = typeInAssembly == null ? Assembly.GetExecutingAssembly() : Assembly.GetAssembly(typeInAssembly);
            Type[] assemblyTypes = assemblyInfo.GetTypes();

            Type[] typesWithCustomAttribute = assemblyTypes.Where(type =>
             {
                 return type.GetCustomAttributes(typeof(T), false).Length > 0;
             }).ToArray();

            return typesWithCustomAttribute;
        }

        public static Type[] GetAssemblyCustomAttributeTypeList<T>() where T : Attribute
        {
            return AssemblyWithCustomAttributeDict.ContainsKey(typeof(T)) ? AssemblyWithCustomAttributeDict[typeof(T)].ToArray() : null;
        }

    }
}
