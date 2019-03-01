using conf;
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace EngineCore.Editor
{
    public class GoeConfigSeclectWindow : GOESelectWindow
    {
        protected override void DoSearch(string strFilter)
        {
            if (CurTypeName == JsonObjectHelperTypes.Config)
            {
                m_filesFiltered = new List<string>();
                m_filesFilteredDisp.Clear();
                List<Type> configTypes = new List<Type>();
                Type[] types = Assembly.GetAssembly(typeof(ConfHelper)).GetTypes();
                foreach (Type type in types)
                {
                    if (type.GetCustomAttributes(typeof(ConfigAttribute), false).Length > 0)
                    {
                        configTypes.Add(type);
                        m_filesFiltered.Add(type.FullName);
                        m_filesFilteredDisp.Add(type.Name);
                    }
                }
                Debug.Log(configTypes.Count);
            }
            else
                base.DoSearch(strFilter);
        }

        protected override void ShowInEditAndPropertyWindow()
        {
            base.ShowInEditAndPropertyWindow();
            if (CurTypeName == JsonObjectHelperTypes.Config)
            {
                JsonObjectPropertyWindow wp = EditorWindow.GetWindow(typeof(JsonObjectPropertyWindow), false, "属性") as JsonObjectPropertyWindow;
                wp.SetCurStringInterface(null, Helper);
            }
        }
    }
}
