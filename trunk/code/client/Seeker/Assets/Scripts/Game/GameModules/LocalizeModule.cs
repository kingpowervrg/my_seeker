/********************************************************************
	created:  2018-3-23
	filename: LocalizeModule.cs
	author:	  songguangze@outlook.com
	
	purpose:	本地化Module
    todo:
        本地化流程的通用数据格式 需要之后抽象，再把LocalizeModule移到Core里
*********************************************************************/

using EngineCore;
using System;
using System.Reflection;
using UnityEngine;

namespace SeekerGame
{
    public class LocalizeModule : AbstractModule
    {
        private static LocalizeModule m_instance = null;
        private string m_currentLanguage = string.Empty;

        public LocalizeModule()
        {
            AutoStart = true;
            m_instance = this;
        }

        /// <summary>
        /// 根据系统语言加载本地化文件
        /// </summary>
        public void InitLocalizeLanguage()
        {
            switch (Application.systemLanguage)
            {
                case SystemLanguage.Chinese:
                case SystemLanguage.ChineseSimplified:
                    CurrentLanguage = "Chinese";
                    break;
                default:
                    CurrentLanguage = Application.systemLanguage.ToString();
                    break;
            }
        }


        /// <summary>
        /// 获取指定语言的值
        /// </summary>
        /// <param name="languageKey"></param>
        /// <param name="values"></param>
        /// <returns></returns>
        public string GetString(string languageKey, params object[] values)
        {
            string languageValue = string.Empty;

            ConfLanguage languageWithKey = ConfLanguage.Get(languageKey);

            if (languageWithKey == null)
                return languageKey;

            Type typeOfLanguage = languageWithKey.GetType();
            FieldInfo targetLanguageFieldInfo = typeOfLanguage.GetField(CurrentLanguage, BindingFlags.GetField | BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);

            if (targetLanguageFieldInfo == null)
                targetLanguageFieldInfo = typeOfLanguage.GetField("English", BindingFlags.GetField | BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);

            string value = (string)targetLanguageFieldInfo.GetValue(languageWithKey);
            if (values != null && values.Length > 0)
            {
                return string.Format(value, values);
            }
            else
            {
                return value;
            }
        }

        /// <summary>
        /// 当前语言
        /// </summary>
        public string CurrentLanguage
        {
            get
            {
                if (string.IsNullOrEmpty(this.m_currentLanguage))
                    InitLocalizeLanguage();

                return this.m_currentLanguage;
            }
            private set
            {
                if (string.IsNullOrEmpty(m_currentLanguage) || m_currentLanguage != value)
                {
                    m_currentLanguage = value;
                    InitLocalizeLanguage();
                }
            }
        }

        public static LocalizeModule Instance
        {
            get { return m_instance; }
        }
    }
}