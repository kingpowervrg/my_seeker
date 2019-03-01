using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace EngineCore.Editor
{
    public class GOEEditorMainWindow : GoeConfigSeclectWindow
    {
        static GOEEditorMainWindow()
        {
            var fields = from field in typeof(JsonObjectHelperTypes).GetFields()
                         where field.IsStatic
                         select new KeyValuePair<JsonObjectHelperTypes, string>((JsonObjectHelperTypes)field.GetValue(null),
                             field.GetCustomAttributes(typeof(JsonObjectHelperDisplayNameAttribute), false).Length > 0 ?
                                ((JsonObjectHelperDisplayNameAttribute)field.GetCustomAttributes(typeof(JsonObjectHelperDisplayNameAttribute), false)[0]).DisplayName :
                                field.GetValue(null).ToString());
            foreach (KeyValuePair<JsonObjectHelperTypes, string> i in fields)
                mHelperNameMaps[i.Key] = i.Value;
            var helpers = from helper in Assembly.GetAssembly(typeof(GOEEditorMainWindow)).GetTypes()
                          where helper.IsSubclassOf(typeof(JsonObjectUIHelper)) && helper.GetCustomAttributes(typeof(JsonObjectTypeAttribute), false).Length > 0
                          select new KeyValuePair<JsonObjectHelperTypes, JsonObjectUIHelper>(
                              ((JsonObjectTypeAttribute)helper.GetCustomAttributes(typeof(JsonObjectTypeAttribute), false)[0]).Type,
                              (JsonObjectUIHelper)Activator.CreateInstance(helper));
            //注册所有类型//
            foreach (KeyValuePair<JsonObjectHelperTypes, JsonObjectUIHelper> i in helpers)
            {
                mUIHelperMap[i.Key] = i.Value;
            }
        }

        void OnEnable()
        {
            LoadConfiguration();
        }

        public GOEEditorMainWindow()
        {

            Init();
            //mUIHelperMap.Add ("Act", new FireActUIHelper ());
            //选中Pack//
            mCurTypeName = JsonObjectHelperTypes.PackV5;

            JsonObjectUIHelper helper = Helper;
            //搜索//
            //Search(helper.GetSearchDir(), "*" + helper.GetFileExt());
            DoSearch("");
        }

        public static void PerformAll(JsonObjectHelperTypes type, List<string> errors)
        {
            JsonObjectUIHelper helper;
            if (mUIHelperMap.TryGetValue(type, out helper))
            {
                string[] files = System.IO.Directory.GetFiles(helper.GetSearchDir(), "*" + helper.GetFileExt(), System.IO.SearchOption.AllDirectories);
                string error;
                foreach (string i in files)
                {
                    if (!helper.MultipleAction(i, out error))
                        errors.Add(error);
                }
            }
        }
    }
}
