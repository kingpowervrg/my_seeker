using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
public class SceneSyncItemWindow : EditorWindow
{
    private List<SceneItemInfo> m_allAssetPath;
    private long m_exhibitID = -1;
    private int m_exhibitIndex = -1;
    private GameObject m_srcObj = null;
    GUIStyle style;
    GUIStyle chooseStyle;
    GUIStyle emptyStyle;
    public static SceneSyncItemWindow CreateWindow()
    {
        Rect rect = new Rect(0, 0, 800, 700);
        SceneSyncItemWindow window = (SceneSyncItemWindow)EditorWindow.GetWindowWithRect(typeof(SceneSyncItemWindow), rect, true, "数据同步窗口");
        if (window.Init())
        {
            window.Show();
        }
        return window;
    }

    public bool Init()
    {
        style = new GUIStyle();
        //style.hover.textColor = Color.red;
        style.normal.textColor = Color.white;
        Texture tex = AssetDatabase.LoadAssetAtPath<Texture>("Assets/Res/Gui/UISprite/bar_type1.png");
        style.normal.background = (Texture2D)tex;
        style.alignment = TextAnchor.MiddleCenter;

        chooseStyle = new GUIStyle();
        chooseStyle.normal.textColor = Color.white;
        Texture chooseTex = AssetDatabase.LoadAssetAtPath<Texture>("Assets/Res/Gui/UISprite/bar_type2.png");
        chooseStyle.normal.background = (Texture2D)chooseTex;
        chooseStyle.alignment = TextAnchor.MiddleCenter;

        emptyStyle = new GUIStyle();
        return true;
    }

    void OnGUI()
    {
        EditorGUILayout.BeginVertical();
        m_srcObj = (GameObject)EditorGUILayout.ObjectField(m_srcObj, typeof(GameObject), true);

        EditorGUILayout.BeginHorizontal();
        OnShowPage();
        OnShowPageItem();
        EditorGUILayout.EndHorizontal();
        OnSysncItem();

        EditorGUILayout.EndVertical();
    }
    private bool[] pageState;
    private int selectPageIndex = -1; //当前选中的页签
    private List<bool>[] selectItemIndex; //当前选中的物件
    private SceneItemInfo currentItemInfo = null;
    private Vector2 itemInfoBenginScroll = Vector2.zero;
    private int mainPageIndex = 0;
    void OnShowPage()
    {
        //页签
        EditorGUILayout.BeginVertical(GUILayout.Width(250), GUILayout.Height(580));
        itemInfoBenginScroll = EditorGUILayout.BeginScrollView(itemInfoBenginScroll, GUILayout.Width(250), GUILayout.Height(580));
        for (int i = 0; i < m_allAssetPath.Count; i++)
        {
            EditorGUILayout.BeginHorizontal();
            pageState[i] = EditorGUILayout.Toggle("", pageState[i], GUILayout.Width(20), GUILayout.Height(30));
            //string[] assetPathSplit = m_allAssetPath[i].Split('/');
            string assetPathName = m_allAssetPath[i].jsonName;//assetPathSplit[assetPathSplit.Length - 1];
            if (selectPageIndex == i)
            {
                if (GUILayout.Button(assetPathName, chooseStyle, GUILayout.Width(220),GUILayout.Height(30)))
                {
                    selectPageIndex = i;

                    currentItemInfo = m_allAssetPath[selectPageIndex];//AssetDatabase.LoadAssetAtPath<SceneItemInfo>(m_allAssetPath[selectPageIndex]);
                }
            }
            else
            {
                if (GUILayout.Button(assetPathName, style, GUILayout.Width(220), GUILayout.Height(30)))
                {
                    selectPageIndex = i;

                    currentItemInfo = m_allAssetPath[selectPageIndex];//AssetDatabase.LoadAssetAtPath<SceneItemInfo>(m_allAssetPath[selectPageIndex]);
                }
            }
            EditorGUILayout.EndHorizontal();
            GUILayout.Button("", emptyStyle,GUILayout.Width(220), GUILayout.Height(5));
        }
        EditorGUILayout.EndScrollView();
        EditorGUILayout.EndVertical();
    }

    void OnShowPageItem()
    {
        EditorGUILayout.BeginVertical();
        if (currentItemInfo == null)
        {
            return;
        }
        for (int i = 0; i < currentItemInfo.items.Count; i++)
        {
            if (currentItemInfo.items[i].itemID == m_exhibitID)
            {
                List<ItemPosInfo> itemPos = currentItemInfo.items[i].itemPos;
                for (int j = 0; j < itemPos.Count; j++)
                {
                    EditorGUILayout.BeginHorizontal();
                   
                    selectItemIndex[selectPageIndex][j] = EditorGUILayout.Toggle(itemPos[j].pos.ToString(), selectItemIndex[selectPageIndex][j]);
                    EditorGUILayout.EndHorizontal();
                }
            }
        }
        EditorGUILayout.EndVertical();
    }

    void OnSysncItem()
    {
        if (GUILayout.Button("确认同步"))
        {
            List<SceneLogSyncData> logdata = new List<SceneLogSyncData>();
            SceneLogSyncData ld = new SceneLogSyncData(m_allAssetPath[mainPageIndex].jsonName, m_exhibitID, m_exhibitIndex);
            logdata.Add(ld);
            for (int i = 0; i < selectItemIndex.Length; i++)
            {
                if (!pageState[i])
                {
                    continue;
                }
                SceneItemInfo scenePageData = m_allAssetPath[i];//AssetDatabase.LoadAssetAtPath<SceneItemInfo>();
                for (int j = 0; j < scenePageData.items.Count; j++)
                {
                    if (scenePageData.items[j].itemID == m_exhibitID)
                    {
                        for (int k = 0; k < scenePageData.items[j].itemPos.Count; k++)
                        {
                            if (selectItemIndex[i][k])
                            {
                                SceneLogSyncData ld1 = new SceneLogSyncData(m_allAssetPath[i].jsonName, m_exhibitID, k);
                                logdata.Add(ld1);
                                scenePageData.items[j].itemPos[k].pos = m_srcObj.transform.position;
                                scenePageData.items[j].itemPos[k].rotate = m_srcObj.transform.eulerAngles;
                                scenePageData.items[j].itemPos[k].scale = m_srcObj.transform.localScale;
                                scenePageData.items[j].itemPos[k].m_cameraNode = this.cameraPoint;
                            }
                        }
                    }
                }
            }
            UserLog.SyncSingleSceneLog(logdata, m_srcObj.transform.position, m_srcObj.transform.eulerAngles, m_srcObj.transform.localScale);
			this.Close();
        }
    }

    private string cameraPoint = string.Empty;
    public void SetData(List<SceneItemInfo> pagePath,long exhibitID,int exhibitIndex,GameObject srcObj,int mainPageIndex,string cameraPoint)
    {
        this.mainPageIndex = mainPageIndex;
        this.cameraPoint = cameraPoint;
        pageState = new bool[pagePath.Count];
        selectItemIndex = new List<bool>[pagePath.Count];
        m_allAssetPath = pagePath;
        this.m_exhibitID = exhibitID;
        this.m_exhibitIndex = exhibitIndex;
        this.m_srcObj = srcObj;
        for (int i = 0; i < pagePath.Count; i++)
        {
            SceneItemInfo itemInfo = m_allAssetPath[i];//AssetDatabase.LoadAssetAtPath<SceneItemInfo>(m_allAssetPath[i]);
            for (int j = 0; j < itemInfo.items.Count; j++)
            {
                if (itemInfo.items[j].itemID == exhibitID)
                {
                    selectItemIndex[i] = new List<bool>();
                    for (int k = 0; k < itemInfo.items[j].itemPos.Count; k++)
                    {
                        if (k == m_exhibitIndex)
                        {
                            selectItemIndex[i].Add(true);
                        }
                        else
                        {
                            selectItemIndex[i].Add(false);
                        }
                    }
                }
            }
        }
    }

}
