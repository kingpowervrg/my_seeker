using conf;
using GOEditor;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace EngineCore.Editor
{
    //打包UIHelper//
    [JsonObjectType(JsonObjectHelperTypes.Config)]
    class GOEConfigUIHelper : JsonObjectUIHelper
    {
        //是否需要先启动游戏//
        public override bool DoesNeedStarted() { return false; }
        //搜索目录还是文件//
        public override bool SearchForDir() { return false; }
        //搜索目录//
        public override string GetSearchDir() { return "Assets/GOEditor/Pack/Definitions/"; }
        //扩展名//
        public override string GetFileExt() { return GOEPack.m_packExt; }

        //new//
        public override bool CanNew() { return false; }
        public override object OnNew()
        {
            return null;
        }
        //save//
        public override bool CanSave() { return true; }
        //delete//
        public override bool CanDelete() { return false; }

        public override Dictionary<string, string> EnumOptions(object target, string paramName)
        {
            PropertyInfo pro = target.GetType().GetProperty(paramName);
            string flag = ConfigAttribute.GetFieldFlag(pro);
            if (flag != null)
            {
                string[] flags = flag.Split('.');
                if (flags.Length == 2)
                {
                    string fullName = "conf." + flags[0];
                    string[] pros = flags[1].Split('|');
                    string vpro = pros[0];
                    string dpro = pros[1];
                    Dictionary<string, string> enums = new Dictionary<string, string>();
                    IList list = GetConfigData(fullName) as IList;
                    {
                        for (int i = 0; i < list.Count; i++)
                        {
                            object obj = list[i];
                            string v = obj.GetType().GetProperty(vpro).GetValue(obj, null).ToString();
                            string d = obj.GetType().GetProperty(dpro).GetValue(obj, null).ToString();
                            enums.Add(v, d);
                        }
                    }
                    return enums;
                }
            }
            return null;
        }

        public override void OnSave(object target, string strFullName)
        {
            MethodInfo info = currentConfigType.GetMethod("Save");
            info.Invoke(target, null);
        }

        public override void MakeEditUI(object target)
        {
            if (target == null || target as IList == null)
                return;
            IList list = target as IList;
            GUILayout.BeginVertical();
            if (GUILayout.Button("新建"))
            {
                object newo = Activator.CreateInstance(currentConfigType);
                autoSn(list, newo);
                list.Add(newo);
            }
            for (int i = 0; i < list.Count; i++)
            {
                object obj = list[i];
                GUILayout.BeginHorizontal();
                if (GUILayout.Button(obj.ToString()))
                {
                    JsonObjectPropertyWindow wp = EditorWindow.GetWindow(typeof(JsonObjectPropertyWindow), false, "属性") as JsonObjectPropertyWindow;
                    wp.SetCurStringInterface(obj, this);
                }
                if (GUILayout.Button("删除", GUILayout.Width(80)))
                {
                    list.Remove(obj);
                    i--;
                }
                GUILayout.EndHorizontal();
            }
            GUILayout.EndVertical();
        }

        private void autoSn(IList list, object obj)
        {
            int sn = 0;
            PropertyInfo pi = currentConfigType.GetProperty("sn");
            foreach (object old in list)
            {
                int osn = (int)pi.GetValue(old, null);
                if (osn >= sn)
                    sn = osn + 1;
            }
            pi.SetValue(obj, sn, null);
        }

        private Type currentConfigType;
        public override object OnSelect(string strFullName)
        {
            currentConfigType = Type.GetType(strFullName);
            return GetConfigData(currentConfigType);
        }

        private object GetConfigData(string strFullName)
        {
            Type type = Type.GetType(strFullName);
            return GetConfigData(type);
        }

        private object GetConfigData(Type type)
        {
            PropertyInfo pi = type.GetProperty("Datas");
            return pi.GetValue(null, null);
        }
    }
}
