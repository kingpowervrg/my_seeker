#if UNITY_EDITOR
using GOEditor;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace EngineCore.Editor
{
    public class GOESelectWindow : EditorWindow
    {
        //可编辑的各种类型的helper//
        protected static Dictionary<JsonObjectHelperTypes, string> mHelperNameMaps = new Dictionary<JsonObjectHelperTypes, string>();
        protected static Dictionary<JsonObjectHelperTypes, JsonObjectUIHelper> mUIHelperMap = new Dictionary<JsonObjectHelperTypes, JsonObjectUIHelper>();
        protected JsonObjectHelperTypes mCurTypeName = JsonObjectHelperTypes.PackV5;                                            //当前编辑的类型名字//

        const string ConfStringInterfaceHelperPrefix = "CONF_SIHELPER_";
        const string ConfStringInterfaceHelperSelectedFilesPrefix = "CONF_SIHELPER_FILES_";
        const string ConfStringInterfaceMultipleChoicePrefix = "CONF_SIHELPER_MULTIPLE_CHOICE_";
        Dictionary<JsonObjectHelperTypes, HashSet<string>> mHelperSelectedFiles = new Dictionary<JsonObjectHelperTypes, HashSet<string>>();
        string m_strNew = "";                                                       //新建文件名//
        string m_strFilter = "";                                                    //文件名过滤器//
        protected List<string> m_filesFiltered = new List<string>();                //过滤后的文件名//
        protected List<string> m_filesFilteredDisp = new List<string>();        //过滤后的文件名，用于显示，不包含路径//
        protected int m_indexSelectedFile = -1;                                         //当前选中的文件索引//
        protected int m_SelectedFileName = -1;                                          //当前选中的文件索引//

        protected object mCurEditingObject = null;                      //当前编辑的SI//

        internal static Dictionary<JsonObjectHelperTypes, JsonObjectUIHelper> HelperMap
        {
            get { return mUIHelperMap; }
        }

        public JsonObjectUIHelper Helper
        {
            get { return mUIHelperMap[mCurTypeName]; }
        }

        public JsonObjectHelperTypes CurTypeName
        {
            get { return mCurTypeName; }
            set { mCurTypeName = value; }
        }

        void SelectType(JsonObjectHelperTypes typeName)
        {
            mCurTypeName = typeName;
            //重新搜索//
            DoSearch(m_strFilter);

            onSelectFile(-1);
        }

        protected virtual void ShowInEditAndPropertyWindow()
        {
            //显示编辑窗口//
            JsonObjectEditWindow we = EditorWindow.GetWindow(typeof(JsonObjectEditWindow), false, "编辑") as JsonObjectEditWindow;
            we.Show();
            we.SetCurStringInterface(mCurEditingObject, Helper);
            //显示属性窗口//
            JsonObjectPropertyWindow wp = EditorWindow.GetWindow(typeof(JsonObjectPropertyWindow), false, "属性") as JsonObjectPropertyWindow;
            wp.Show();
            wp.SetCurStringInterface(mCurEditingObject, Helper);
        }

        // Use this for initialization
        void Start()
        {
        }

        // Update is called once per frame
        void Update()
        {
        }

        ///////////////////////////// 多选功能///////////////
        #region 
        void SaveSelectedFiles()
        {
            foreach (KeyValuePair<JsonObjectHelperTypes, JsonObjectUIHelper> i in mUIHelperMap)
            {
                string key = string.Format("{0}{1}", ConfStringInterfaceHelperSelectedFilesPrefix, i.Key.ToString().ToUpper());
                HashSet<string> set = mHelperSelectedFiles[i.Key];
                System.Text.StringBuilder sb = new System.Text.StringBuilder();
                foreach (string j in set)
                {
                    sb.Append(j);
                    sb.Append("|");
                }
                EditorPrefs.SetString(key, sb.Length > 0 ? sb.ToString(0, sb.Length - 1) : "");
            }
        }


        void LoadSelectedFiles(JsonObjectHelperTypes type, JsonObjectUIHelper helper, string conf)
        {
            HashSet<string> files = new HashSet<string>();
            string[] f = conf.Split('|');
            foreach (string i in f)
            {
                if (string.IsNullOrEmpty(i))
                    continue;
                files.Add(i);
            }
            mHelperSelectedFiles[type] = files;
        }
        #endregion
        ///////////////////////////// 保存本地配置///////////////
        #region 
        protected static void SaveConfiguration()
        {
            foreach (KeyValuePair<JsonObjectHelperTypes, JsonObjectUIHelper> i in mUIHelperMap)
            {
                SaveHelperConfiguration(i.Value);
            }
        }

        public static void SaveHelperConfiguration(JsonObjectUIHelper helper)
        {
            JsonObjectHelperTypes type = ((JsonObjectTypeAttribute)helper.GetType().GetCustomAttributes(typeof(JsonObjectTypeAttribute), false)[0]).Type;
            Dictionary<string, string> dic = new Dictionary<string, string>();
            helper.OnSaveConfig(dic);
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            foreach (KeyValuePair<string, string> i in dic)
            {
                sb.Append(string.Format("{0}|{1}\n", i.Key, i.Value));
            }
            EditorPrefs.SetString(string.Format("{0}{1}", ConfStringInterfaceHelperPrefix, type.ToString().ToUpper()), sb.ToString());
        }

        protected void LoadConfiguration()
        {
            foreach (KeyValuePair<JsonObjectHelperTypes, JsonObjectUIHelper> i in mUIHelperMap)
            {
                //      D.error("ConfStringInterfaceHelperPrefix + "+ i.Key.ToString().ToUpper());
                string conf = EditorPrefs.GetString(string.Format("{0}{1}", ConfStringInterfaceHelperPrefix, i.Key.ToString().ToUpper()));
                if (!string.IsNullOrEmpty(conf))
                {
                    LoadSIConfig(i.Value, conf);
                }

                conf = EditorPrefs.GetString(string.Format("{0}{1}", ConfStringInterfaceHelperSelectedFilesPrefix, i.Key.ToString().ToUpper()));
                if (!string.IsNullOrEmpty(conf))
                {
                    LoadSelectedFiles(i.Key, i.Value, conf);
                }
                else
                    mHelperSelectedFiles[i.Key] = new HashSet<string>();
            }
        }

        public static void LoadUIHelperConfig(JsonObjectUIHelper helper)
        {
            JsonObjectTypeAttribute t = helper.GetType().GetCustomAttributes(typeof(JsonObjectTypeAttribute), false)[0] as JsonObjectTypeAttribute;
            string conf = EditorPrefs.GetString(string.Format("{0}{1}", ConfStringInterfaceHelperPrefix, t.Type.ToString().ToUpper()));
            LoadSIConfig(helper, conf);
        }

        static void LoadSIConfig(JsonObjectUIHelper helper, string conf)
        {
            string[] lines = conf.Split('\n');
            Dictionary<string, string> dic = new Dictionary<string, string>();
            foreach (string line in lines)
            {
                string[] param = line.Split('|');
                if (param.Length == 2)
                {
                    dic[param[0]] = param[1];
                }
            }
            helper.OnLoadConfig(dic);
        }
        #endregion

        string GetCurrentSelectFileName()
        {
            if (null != m_filesFiltered && m_filesFiltered.Count > 0 && m_indexSelectedFile >= 0 && m_indexSelectedFile < m_filesFiltered.Count && null != m_filesFiltered[m_indexSelectedFile] && m_filesFiltered[m_indexSelectedFile].Length > 0)
                return m_filesFiltered[m_indexSelectedFile];
            else
                return null;
        }

        void OnFileChanged()
        {
            AssetDatabase.Refresh();
        }

        protected void Init()
        {
            InitTypes();
        }

        private List<string> strTypeList;
        private List<JsonObjectHelperTypes> strTypes;
        private int selIdx = 0;
        private void InitTypes()
        {
            strTypeList = new List<string>();
            strTypes = new List<JsonObjectHelperTypes>();
            foreach (JsonObjectHelperTypes str in mUIHelperMap.Keys)
            {
                strTypes.Add(str);
                strTypeList.Add(mHelperNameMaps[str]);
            }
            if (null != strTypes && strTypes.Count > 0)
            {
                for (int i = 0; i < strTypes.Count; i++)
                {
                    if (strTypes[i] == mCurTypeName)
                        selIdx = i;
                }
            }
        }

        protected virtual void onNewFile()
        {
            string strFullName = Helper.GetSearchDir() + m_strNew + Helper.GetFileExt();
            strFullName = strFullName.Replace("\\", "/");
            //如文件已存在则提醒//
            FileInfo fi = new FileInfo(strFullName);
            bool overwrite = true;
            if (string.IsNullOrEmpty(m_strNew))
            {
                overwrite = false;
                EditorUtility.DisplayDialog("错误",
                    "请先输入文件名!", "OK");
            }
            if (fi.Exists)
            {
                overwrite = false;
                overwrite = EditorUtility.DisplayDialog("信息",
                    "文件 : " + m_strNew + Helper.GetFileExt() + "已经存在\r\n确定要覆盖吗？",
                    "是", "否");
            }
            if (overwrite)
            {
                //构造对象//
                mCurEditingObject = Helper.OnNew();
                if (null != mCurEditingObject)
                {
                    //写入文件//
                    Helper.OnSave(mCurEditingObject, strFullName);

                    OnFileChanged();

                    //重新搜索//
                    DoSearch(m_strFilter);
                    onSelectFile(strFullName);
                }
            }
        }

        private void onSaveFile()
        {
            string strFullName = GetCurrentSelectFileName();
            if (null != strFullName)
            {
                Helper.OnSave(mCurEditingObject, strFullName);
                OnFileChanged();
            }
        }

        private void onSaveOther()
        {
            //显示保存窗口//
            string strFullName = EditorUtility.SaveFilePanel("Save As...",
                Path.GetFullPath(Helper.GetSearchDir()), Path.GetFileName(GetCurrentSelectFileName()),
                "");
            if (null != strFullName && strFullName.Length > 0)
            {
                Helper.OnSave(mCurEditingObject, strFullName);
                OnFileChanged();

                //重新搜索//
                DoSearch(m_strFilter);
                onSelectFile(strFullName);
            }
        }

        private void OnDeleteFile()
        {
            string strCurFileName = GetCurrentSelectFileName();
            if (null != strCurFileName)
            {
                FileInfo fi = new FileInfo(strCurFileName);
                if (fi.Exists && EditorUtility.DisplayDialog("警告",
                        "文件 : " + strCurFileName + "\r\n确定要删除吗？",
                        "是", "否"))
                {
                    AssetDatabase.DeleteAsset(strCurFileName);
                    Helper.OnDelete(strCurFileName);
                    OnFileChanged();
                    DoSearch(m_strFilter);
                    onSelectFile(-1);

                    EditorUtility.DisplayDialog("信息",
                        "文件已删除 : " + strCurFileName,
                        "确定");
                }
            }
        }

        private Vector2 m_scrollPositionSelect; //选择列表滚动位置//
        void OnGUI()
        {
            if (HelperMap.Count == 0)
                return;

            if (Helper != null && Helper.IsDirty)
            {
                SaveHelperConfiguration(Helper);
            }

            GUILayoutOption[] voptionsNoMaxWidth = {
            GUILayout.MaxWidth (10000),
            GUILayout.ExpandHeight (true),
            GUILayout.ExpandWidth (true)};

            //选择区//
            GUILayout.BeginVertical();

            //类型//
            GUILayout.Label("编辑器类型");

            selIdx = EditorGUILayout.Popup(selIdx, strTypeList.ToArray());
            if (mCurTypeName != strTypes[selIdx])
            {
                SelectType(strTypes[selIdx]);
            }

            /////检测是否正在运行,这个检测仅由SelectWindow进行，UIHelper中不需要检测///
            if (!Application.isPlaying && Helper.DoesNeedStarted())
            {
                GUILayout.Label("请先运行游戏");
                GUILayout.EndVertical();

                mCurEditingObject = null;
                ShowInEditAndPropertyWindow();
                return;
            }

            //新建//
            if (Helper.CanNew())
            {
                GUILayout.Label("新建");
                GUILayout.BeginHorizontal();

                m_strNew = EditorGUILayout.TextField(m_strNew);
                if (GUILayout.Button("新建配置", GUILayout.Width(60)))
                {
                    onNewFile();
                }
                GUILayout.EndHorizontal();
            }

            if (mCurEditingObject != null)
            {

                GUILayout.BeginHorizontal();
                //保存//
                if (Helper.CanSave())
                {
                    if (GUILayout.Button("保存配置"))
                    {
                        onSaveFile();
                    }
                }
                //另存为//
                if (Helper.CanNew())
                {
                    if (GUILayout.Button("另存配置"))
                    {
                        onSaveOther();
                    }
                }
                //删除//
                if (Helper.CanDelete())
                {
                    if (GUILayout.Button("删除"))
                    {
                        OnDeleteFile();
                    }
                }
                GUILayout.EndHorizontal();
            }
            //选择//
            GUILayout.Label("选择");

            GUILayout.BeginHorizontal();

            string strFilter = EditorGUILayout.TextField(m_strFilter);

            if (GUILayout.Button("搜索配置", GUILayout.Width(60)) || strFilter != m_strFilter)
            {
                string name = GetCurrentSelectFileName();
                DoSearch(strFilter);
                if (name != null)
                    onSelectFile(name);
            }
            GUILayout.EndHorizontal();
            GUILayout.Space(10);

            bool multipleChoice;
            if (Helper.CanMultiple)
            {
                string multipleKey = ConfStringInterfaceMultipleChoicePrefix + mCurTypeName.ToString().ToUpper();
                bool old = EditorPrefs.GetBool(multipleKey, false);
                multipleChoice = GUILayout.Toggle(old, "多选");
                if (old != multipleChoice)
                {
                    EditorPrefs.SetBool(multipleKey, multipleChoice);
                }
            }
            else
                multipleChoice = false;
            GUILayout.Space(10);
            //选择列表//
            if (m_filesFiltered.Count > 0)
            {
                HashSet<string> selectedFiles = mHelperSelectedFiles[mCurTypeName];

                m_scrollPositionSelect = GUILayout.BeginScrollView(m_scrollPositionSelect, false, true, voptionsNoMaxWidth);
                if (multipleChoice)
                {
                    for (int i = 0; i < m_filesFiltered.Count; i++)
                    {
                        string name = m_filesFiltered[i];
                        bool exists = selectedFiles.Contains(name);

                        bool c = GUILayout.Toggle(exists, m_filesFilteredDisp[i]);
                        if (c)
                            selectedFiles.Add(name);
                        else
                            selectedFiles.Remove(name);
                        if (c != exists)
                        {
                            SaveSelectedFiles();
                        }
                    }
                }
                else
                {
                    int idx = GUILayout.SelectionGrid(m_indexSelectedFile, m_filesFilteredDisp.ToArray(), 1);
                    if (idx != m_indexSelectedFile)
                    {
                        onSelectFile(idx);
                    }
                }
                GUILayout.EndScrollView();
                if (multipleChoice)
                {
                    Helper.MakeMultipleActionUI();
                    if (GUILayout.Button(Helper.MultipleActionName))
                    {
                        foreach (string i in selectedFiles)
                        {
                            string error;
                            if (!Helper.MultipleAction(i, out error))
                            {
                                EditorUtility.DisplayDialog("错误", error, "确定");
                                break;
                            }
                        }
                        EditorUtility.DisplayDialog("完成", string.Format("所有文件{0}完成", Helper.MultipleActionName), "确定");
                    }

                    GUILayout.Space(10);
                }
            }

            GUILayout.EndVertical();
        }

        private void onSelectFile(string name)
        {
            name = Path.GetFileNameWithoutExtension(name);
            onSelectFile(m_filesFilteredDisp.IndexOf(name));
        }

        private void onSelectFile(int idx)
        {
            //加载并设为当前StringInterface//
            if (null != m_filesFiltered && m_filesFiltered.Count > 0)
            {
                m_indexSelectedFile = idx;
                //选中响应//
                string selectFileName = GetCurrentSelectFileName();
                if (null != selectFileName)
                    mCurEditingObject = Helper.OnSelect(selectFileName);
                else
                    mCurEditingObject = null;
            }
            //显示到编辑窗口和属性窗口//
            //如此行在大括号外面则会一直更新，导致Search框无法输入文字//
            ShowInEditAndPropertyWindow();
        }

        protected virtual void DoSearch(string strFilter)
        {
            m_strFilter = strFilter;
            string[] filenames = Directory.GetFiles(Helper.GetSearchDir(), "*" + Helper.GetFileExt(), SearchOption.AllDirectories);
            m_filesFiltered = new List<string>();
            m_filesFilteredDisp.Clear();
            for (int i = 0; i < filenames.Length; i++)
            {
                string str = filenames[i];
                if (str.Contains(m_strFilter))
                {
                    m_filesFiltered.Add(str.Replace("\\", "/"));
                }
            }
            m_filesFiltered.Sort(Helper.SortFiles);
            foreach (string str in m_filesFiltered)
            {
                m_filesFilteredDisp.Add(Path.GetFileNameWithoutExtension(str));
            }
        }
    }
}
#endif
