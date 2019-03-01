using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;

public class CartoonWindow : EditorWindow
{



    private const string SCENE_NAME = "Cartoon_Editor";
    private const string TEMPLATE_PATH = "Res/Cartoon/Template/";
    private const string TEMPLATE_ASSET_PATH = "Assets/" + TEMPLATE_PATH;
    private const string VIDEO_PATH = "Res/Cartoon/Video/";
    private const string VIDEO_ASSET_PATH = "Assets/" + VIDEO_PATH;


    #region 场景
    private string sceneName;
    private Scene m_scene;
    private GameObject m_cartoon_root;
    private long m_cur_editor_cartoon_template_id;
    #endregion


    #region 模板
    private string m_template_path;
    private Dictionary<int, string> m_template_path_names;
    private string[] m_template_names;
    private int[] m_template_ids;
    private int m_selected_template_id;
    #endregion


    #region 玩法数据
    private Dictionary<long, long> m_level_json_max_name_dict;
    private long m_cur_editor_cartoon_level_id;
    #endregion


    [MenuItem("Tools/漫画编辑器")]
    static void CreateWindow()
    {
        Rect rect = new Rect(0, 0, 300, 300);
        CartoonWindow window = (CartoonWindow)EditorWindow.GetWindowWithRect(typeof(CartoonWindow), rect, true, "漫画编辑器");
        window.Init();
        window.Show();
    }

    public void OnLoadLevel(long level_id_)
    {
        CartoonTemplate temp = this.CreateLevelObj(level_id_);

        CartoonItemJson json = CartoonJsonUtil.LoadLevelJsonData(level_id_);

        CartoonItemWithClips clips = this.ConvertVideoNameToClip(json);

        temp.LoadVideos(clips);

        this.ShowNotification(new GUIContent("请用鼠标选中漫画编辑器"));

    }

    private CartoonTemplate CreateLevelObj(long level_id_)
    {
        this.ClearOldCartoon();

        int template_id = (int)(level_id_ / 1000 * 1000);

        if (null == this.m_template_path_names)
            this.InitTemplate();

        m_cartoon_root = GameObject.Find("UI_Cartoon");

        string path = this.m_template_path_names[template_id];
        GameObject gameObj = AssetDatabase.LoadAssetAtPath<GameObject>(path);
        GameObject obj_in_scene = PrefabUtility.InstantiatePrefab(gameObj) as GameObject;
        obj_in_scene.transform.SetParent(m_cartoon_root.transform);
        RectTransform rect = obj_in_scene.GetComponent<RectTransform>();
        rect.anchoredPosition = Vector2.zero;
        rect.sizeDelta = Vector2.zero;

        m_cur_editor_cartoon_template_id = template_id;
        m_cur_editor_cartoon_level_id = level_id_;

        obj_in_scene.name = m_cur_editor_cartoon_level_id.ToString();
        CartoonTemplate ret = obj_in_scene.GetComponent<CartoonTemplate>();
        ret.m_template_id = m_cur_editor_cartoon_level_id;

        return ret;
    }


    void Init()
    {
        //load base item for table
        m_scene = SceneManager.GetActiveScene();
        if (m_scene != null)
        {
            sceneName = m_scene.name;
        }

        if (string.IsNullOrEmpty(sceneName))
        {
            return;
        }

        m_cartoon_root = GameObject.Find("UI_Cartoon");

        this.InitTemplate();
        this.CalcNewLevelName();

    }


    private void InitTemplate()
    {
        string app_path = Application.dataPath;
        if (!app_path.EndsWith("/"))
        {
            app_path += "/";
        }

        string temp_path = TEMPLATE_PATH;
        if (temp_path.StartsWith("/"))
        {
            temp_path = temp_path.Remove(0, 1);
        }
        m_template_path = app_path + temp_path;

        string[] template_path_names = Directory.GetFiles(m_template_path, "*.prefab", SearchOption.TopDirectoryOnly);

        if (0 == template_path_names.Length)
            return;

        m_template_path_names = new Dictionary<int, string>();


        m_template_names = new string[template_path_names.Length];
        m_template_ids = new int[template_path_names.Length];

        for (int i = 0; i < template_path_names.Length; ++i)
        {
            string full_name = Path.GetFileNameWithoutExtension(template_path_names[i]);

            string[] separate_name = full_name.Split('-');
            string template_id_str = separate_name[1];

            m_template_names[i] = template_id_str;
            m_template_ids[i] = int.Parse(m_template_names[i]);

            string file_name_with_extention = Path.GetFileName(template_path_names[i]);
            m_template_path_names.Add(m_template_ids[i], TEMPLATE_ASSET_PATH + file_name_with_extention);
        }

        m_selected_template_id = 0 == m_selected_template_id ? m_template_ids[0] : m_selected_template_id;
    }


    public void CalcNewLevelName()
    {
        List<string> names = CartoonJsonUtil.GetLevelJsonFileNamesWithoutEx();

        m_level_json_max_name_dict = new Dictionary<long, long>();

        foreach (string name in names)
        {
            long name_id = long.Parse(name);
            long key_id = name_id / 1000;

            if (!m_level_json_max_name_dict.ContainsKey(key_id) || name_id > m_level_json_max_name_dict[key_id])
                m_level_json_max_name_dict[key_id] = name_id;
        }
    }

    private void ClearOldCartoon()
    {
        if (null == m_cartoon_root)
        {
            EditorGUILayout.HelpBox("重新打开漫画编辑器", MessageType.Error);
            this.ShowNotification(new GUIContent("错误： 重新打开漫画编辑器"));
            return;
        }

        CartoonTemplate[] all_old = m_cartoon_root.GetComponentsInChildren<CartoonTemplate>(true);

        for (int i = 0; i < all_old.Length; ++i)
        {
            GameObject.DestroyImmediate(all_old[i].gameObject);
        }
    }

    private void CreateNewLevel()
    {


        this.CalcNewLevelName();

        long temp_level_id = this.m_level_json_max_name_dict.ContainsKey(this.m_selected_template_id / 1000) ? this.m_level_json_max_name_dict[this.m_selected_template_id / 1000] + 1 : this.m_selected_template_id + 1;

        this.CreateLevelObj(temp_level_id);

        //string path = this.m_template_path_names[this.m_selected_template_id];
        //GameObject gameObj = AssetDatabase.LoadAssetAtPath<GameObject>(path);
        //GameObject obj_in_scene = PrefabUtility.InstantiatePrefab(gameObj) as GameObject;
        //obj_in_scene.transform.parent = m_cartoon_root.transform;
        //RectTransform rect = obj_in_scene.GetComponent<RectTransform>();
        //rect.anchoredPosition = Vector2.zero;
        //rect.sizeDelta = Vector2.zero;

        //m_cur_editor_cartoon_template_id = this.m_selected_template_id;
        //m_cur_editor_cartoon_level_id = this.m_level_json_max_name_dict.ContainsKey(this.m_selected_template_id) ? this.m_level_json_max_name_dict[this.m_selected_template_id] + 1 : this.m_selected_template_id + 1;

        //obj_in_scene.name = m_cur_editor_cartoon_level_id.ToString();
        //obj_in_scene.GetComponent<CartoonTemplate>().m_template_id = m_cur_editor_cartoon_level_id;

    }

    private void SaveLevelJson()
    {
        if (null != m_cartoon_root)
        {
            CartoonTemplate[] cartoons = m_cartoon_root.GetComponentsInChildren<CartoonTemplate>(true);

            if (null != cartoons && 1 == cartoons.Length)
            {
                CartoonTemplate editing_item = cartoons[0];

                long key_id = editing_item.m_template_id / 1000;

                if (null == this.m_level_json_max_name_dict)
                    this.CalcNewLevelName();

                if (!this.m_level_json_max_name_dict.ContainsKey(key_id) || this.m_level_json_max_name_dict[key_id] < editing_item.m_template_id)
                {
                    this.m_level_json_max_name_dict[key_id] = editing_item.m_template_id;

                    CartoonJsonUtil.SaveLevelJsonData(editing_item);
                }
                else
                {
                    CartoonJsonUtil.SaveLevelJsonData(editing_item);
                }

                this.ShowNotification(new GUIContent("提示： 保存成功"));

            }
            else
            {
                EditorGUILayout.HelpBox("场景内没有，或者有多余的漫画模板", MessageType.Error);
                this.ShowNotification(new GUIContent("错误： 场景内没有，或者有多余的漫画模板"));
                return;
            }
        }
        else
        {
            EditorGUILayout.HelpBox("重新打开漫画编辑器", MessageType.Error);
            this.ShowNotification(new GUIContent("错误： 重新打开漫画编辑器"));
            return;
        }
    }


    public CartoonItemWithClips ConvertVideoNameToClip(CartoonItemJson video_names_)
    {
        CartoonItemWithClips ret = new CartoonItemWithClips();

        ret.Item_id = video_names_.Item_id;
        ret.M_Items = new List<CartoonClips>();

        foreach (var video_name in video_names_.M_cartoons)
        {
            CartoonClips ret_clip = new CartoonClips();
            ret_clip.M_clips = new List<VideoClip>();
            foreach (var name in video_name.M_names)
            {
                string file_name_with_extention = name + ".mp4";
                string path = VIDEO_ASSET_PATH + file_name_with_extention;
                VideoClip clip = AssetDatabase.LoadAssetAtPath<VideoClip>(path);
                ret_clip.M_clips.Add(clip);
            }
            ret.M_Items.Add(ret_clip);
        }

        return ret;
    }

    void OnGUI()
    {
        if (string.IsNullOrEmpty(sceneName))
        {
            EditorGUILayout.HelpBox("未打开场景" + SCENE_NAME, MessageType.Error);
            return;
        }

        if (sceneName != SCENE_NAME)
        {
            EditorGUILayout.HelpBox("请选择场景" + SCENE_NAME, MessageType.Error);
            return;
        }

        EditorGUILayout.BeginVertical();
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("当前场景名称:" + sceneName);
        EditorGUILayout.EndHorizontal();


        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("当前关卡名称:" + m_cur_editor_cartoon_level_id);
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.Space();

        EditorGUILayout.BeginHorizontal();

        if (GUILayout.Button("创建漫画"))
        {
            this.CreateNewLevel();
            EditorUtility.SetDirty(this);
            this.ShowNotification(new GUIContent("请用鼠标选中漫画编辑器"));
        }

        if (null != m_template_names)
            m_selected_template_id = EditorGUILayout.IntPopup("模板id", m_selected_template_id, m_template_names, m_template_ids);


        //if(EditorGUILayout.IntPopup)

        EditorGUILayout.EndHorizontal();

        EditorGUILayout.Space();

        EditorGUILayout.BeginHorizontal();

        if (GUILayout.Button("保存当前漫画"))
        {
            this.SaveLevelJson();
            EditorUtility.SetDirty(this);
        }

        EditorGUILayout.EndHorizontal();

        EditorGUILayout.Space();

        EditorGUILayout.BeginHorizontal();

        if (GUILayout.Button("加载指定漫画"))
        {
            CartoonLevelWindow level_win = CartoonLevelWindow.CreateWindow();
            level_win.OnLevelSelected = OnLoadLevel;
            EditorUtility.SetDirty(this);
  
        }

        EditorGUILayout.EndHorizontal();

        EditorGUILayout.Space();
        EditorGUILayout.Space();
        EditorGUILayout.Space();
        EditorGUILayout.Space();

        EditorGUILayout.BeginHorizontal();

        if (GUILayout.Button("导出全部数据（程序用）"))
        {
            CartoonJsonUtil.SaveAllJsonData();
            this.ShowNotification(new GUIContent("全部数据导出完毕"));
        }

        EditorGUILayout.EndHorizontal();

        //if (GUILayout.Button("添加分页"))
        //{
        //    AddDataPage();
        //}
        //if (GUILayout.Button("删除分页"))
        //{
        //    RemoveDataPage();
        //}

        //EditorGUILayout.EndHorizontal();

        //EditorGUILayout.BeginHorizontal();
        //if (GUILayout.Button("添加物品"))
        //{
        //    AddItem();
        //}
        //if (GUILayout.Button("添加节点"))
        //{
        //    AddItemNode();
        //}
        //if (GUILayout.Button("移除物品"))
        //{
        //    RemoveItem();
        //}
        //if (GUILayout.Button("输出"))
        //{
        //    SaveItem();
        //}

        //if (GUILayout.Button("提取场景光照"))
        //{
        //    SceneBake();
        //}

        //EditorGUILayout.EndHorizontal();

        //#region 具体物品
        //EditorGUILayout.BeginHorizontal();

        //EditorGUILayout.BeginVertical();
        //itemBeginScroll = EditorGUILayout.BeginScrollView(itemBeginScroll, GUILayout.Width(250), GUILayout.Height(580));
        //InitItem();
        //EditorGUILayout.EndScrollView();
        //EditorGUILayout.EndVertical();

        //GUILayout.BeginVertical();
        //itemInfoBenginScroll = EditorGUILayout.BeginScrollView(itemInfoBenginScroll, GUILayout.Width(550), GUILayout.Height(580));
        //ItemClick(selectItem);
        //EditorGUILayout.EndScrollView();
        //GUILayout.EndVertical();

        //EditorGUILayout.EndHorizontal();
        //#endregion
        EditorGUILayout.EndVertical();
    }
}

