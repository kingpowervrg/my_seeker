#if UNITY_EDITOR
using EngineCore;
using GOEditor;
using GOEngine.Implement;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace EngineCore.Editor
{
    public class JsonObjectPropertyWindow : EditorWindow
    {
        private object m_CurStringInterface = null;                         //当前编辑的根StringInterface//
        private JsonObjectUIHelper m_CurStringInterfaceHelper = null;       //当前SI的辅助对象//
        private Dictionary<object, bool> m_FoldOutMap = new Dictionary<object, bool>();     //折叠状态//
        private const int mcnIndentStep = 1;

        //设置当前StringInterface//
        public void SetCurStringInterface(object si, JsonObjectUIHelper sih)
        {
            if (si != m_CurStringInterface || sih != m_CurStringInterfaceHelper)
            {
                m_CurStringInterface = si;
                m_CurStringInterfaceHelper = sih;
                m_FoldOutMap.Clear();
            }
        }


        private Vector2 m_scrollPositionSI;
        void MakeStringInterfaceUI()
        {
            if (null == m_CurStringInterface || null == m_CurStringInterfaceHelper)
                return;

            GUILayoutOption[] voptionsV = {
            GUILayout.ExpandHeight(true)
        };

            GUILayout.BeginVertical(voptionsV);

            m_scrollPositionSI = GUILayout.BeginScrollView(m_scrollPositionSI, false, true);
            MakeStringInterfaceUIInternal(m_CurStringInterface, 0);
            GUILayout.EndScrollView();

            GUILayout.EndVertical();
        }

        void MakeStringInterfaceUIInternal(object si, int indentLevel)
        {
            if (null == si)
                return;
            //缩进//
            EditorGUI.indentLevel = indentLevel;
            //列出SI类型//
            if (!m_FoldOutMap.ContainsKey(si))
                m_FoldOutMap.Add(si, true);
            //m_SIFoldOutMap[si] = EditorGUILayout.Foldout(m_SIFoldOutMap[si], si.getParamDictionary().getName());
            object[] typeAttr = si.GetType().GetCustomAttributes(typeof(DisplayNameAttribute), false);
            string typeName = typeAttr.Length > 0 ? ((DisplayNameAttribute)typeAttr[0]).DisplayName : si.GetType().FullName;
            m_FoldOutMap[si] = EditorGUILayout.Foldout(m_FoldOutMap[si], typeName);
            if (m_FoldOutMap[si])
            {
                //列出SI属性//
                EditorGUI.indentLevel = indentLevel + 1;
                foreach (PropertyInfo pd in JsonUtil.GetProperties(si))
                {
                    if (JsonFieldAttribute.GetFieldFlag(si, pd.Name) == JsonFieldTypes.HasChildren)
                    {
                        IEnumerable sis = pd.GetValue(si, null) as IEnumerable;
                        foreach (object ssi in sis)
                        {
                            MakeStringInterfaceUIInternal(ssi, indentLevel + mcnIndentStep);
                        }
                        continue;
                    }

                    if (!pd.CanWrite || !pd.CanRead)
                        continue;

                    if (JsonFieldAttribute.GetFieldFlag(si, pd.Name) == JsonFieldTypes.UnEditable)
                        continue;


                    GUILayout.BeginHorizontal();
                    object value = pd.GetValue(si, null);

                    //否则按类型创建控件//
                    string fieldDisName;
                    object[] atts = pd.GetCustomAttributes(typeof(DisplayNameAttribute), false);
                    fieldDisName = atts.Length > 0 ? ((DisplayNameAttribute)atts[0]).DisplayName : pd.Name;
                    if (pd.PropertyType == typeof(bool))
                    {
                        bool v = (bool)value;
                        v = EditorGUILayout.Toggle(fieldDisName, v);
                        value = v;
                    }
                    else if (pd.PropertyType == typeof(float))
                    {
                        float v = (float)value;
                        v = EditorGUILayout.FloatField(fieldDisName, v);
                        value = v;
                    }
                    else if (pd.PropertyType == typeof(int))
                    {
                        int v = (int)value;
                        v = EditorGUILayout.IntField(fieldDisName, v);
                        value = v;
                    }
                    else if (pd.PropertyType == typeof(double))
                    {
                        double v = (double)value;
                        v = (double)EditorGUILayout.FloatField(fieldDisName, (float)v);
                        value = v;
                    }
                    else if (pd.PropertyType == typeof(Color))
                    {
                        Color color = (Color)value;
                        color = EditorGUILayout.ColorField(fieldDisName, color);
                        value = color;
                    }
                    else if (pd.PropertyType == typeof(string[]))
                    {
                        string v = value != null ? JsonUtil.StringArrayToString(value as string[]) : "";
                        v = EditorGUILayout.TextField(fieldDisName, v);
                        value = JsonUtil.StringToStringArray(v);
                    }
                    else if (pd.PropertyType == typeof(int[]))
                    {
                        string v = JsonUtil.ArrayToString(value as int[]);
                        v = EditorGUILayout.TextField(fieldDisName, v);
                        value = JsonUtil.StringToArray<int>(v);
                    }
                    else if (pd.PropertyType == typeof(double[]))
                    {
                        /*  string v = JsonUtil.ArrayToString(value as double[]);
                          v = EditorGUILayout.TextField(fieldDisName, v);
                          if (GUILayout.Button("拾取", GUILayout.Width(100)))
                          {
                              Vector3 pos = Vector3.zero;
                              if ((value as double[]).Length == 3)
                                  pos = new Vector3((float)(value as double[])[0], (float)(value as double[])[1], (float)(value as double[])[2]);
                              PositionPicker picker = PositionPickerTool.PickPosition(pos, si, pd, true);
                          }
                          value = JsonUtil.StringToArray<double>(v);*/
                    }
                    else if (pd.PropertyType == typeof(float[]))
                    {
                        /* string v = JsonUtil.ArrayToString(value as float[]);
                         v = EditorGUILayout.TextField(fieldDisName, v);
                         if (GUILayout.Button("拾取", GUILayout.Width(100)))
                         {
                             Vector3 pos = Vector3.zero;
                             if ((value as float[]).Length == 3)
                                 pos = new Vector3((value as float[])[0], (value as float[])[1], (value as float[])[2]);
                             PositionPicker picker = PositionPickerTool.PickPosition(pos, si, pd, true);
                         }
                         value = JsonUtil.StringToArray<float>(v);*/
                    }
                    else if (pd.PropertyType == typeof(Vector3))
                    {
                        Vector3 v = (Vector3)value;
                        v = EditorGUILayout.Vector3Field(fieldDisName, v);
                        value = v;
                    }
                    else
                    {
                        string v = string.Empty;
                        if (value != null)
                            v = value.ToString();
                        v = EditorGUILayout.TextField(fieldDisName, v);
                        value = v;
                    }
                    Dictionary<string, string> options = m_CurStringInterfaceHelper.EnumOptions(si, pd.Name);
                    if (null != options && options.Count > 0)
                    {
                        //如有枚举选项则创建选项下拉框//
                        int idx = -1;
                        string[] name = new string[options.Count];
                        string[] displayName = new string[options.Count];

                        int j = 0;
                        foreach (KeyValuePair<string, string> i in options)
                        {
                            name[j] = i.Key;
                            displayName[j++] = i.Value;
                        }

                        for (int i = 0; i < name.Length; i++)
                        {
                            if (name[i] == value.ToString())
                                idx = i;
                        }
                        const int popupWidth = 100;

                        idx = EditorGUILayout.Popup(idx, displayName, GUILayout.Width(popupWidth));
                        if (idx != -1)
                            value = JsonUtil.ConvertData(name[idx], pd.PropertyType);
                    }
                    //Debug.Log(value);
                    pd.SetValue(si, value, null);
                    GUILayout.EndHorizontal();
                }
            }
            //缩进//
            EditorGUI.indentLevel = indentLevel;
        }

        void OnGUI()
        {
            MakeStringInterfaceUI();
        }
    }
}
#endif