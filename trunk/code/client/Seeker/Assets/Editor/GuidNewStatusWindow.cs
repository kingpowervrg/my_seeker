using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using SeekerGame;
public class GuidNewStatusWindow : EditorWindow
{
    static GuidNewStatusWindow window;
    [MenuItem("PlayerPrefs/GuidNewStatus")]
    static void CreateWindow()
    {
        Rect rect = new Rect(0, 0, 500, 400);
        window = (GuidNewStatusWindow)EditorWindow.GetWindowWithRect(typeof(GuidNewStatusWindow), rect, true, "资源管理器");
        if (window.Init())
        {
            window.Show();
        }
    }

    private bool Init()
    {
        string playerName = PlayerPrefTool.GetUsername(ENUM_LOGIN_TYPE.E_THIRD);
        Debug.Log("playerName === " + playerName);
        return true;
    }
    string playerID;
    string playerProgress;
    string setProgresssValue;
    string progressForID;

    string maxGroup;
    string maxGuidProgress;
    void OnGUI()
    {
        playerID = EditorGUILayout.TextField("玩家ID:", playerID);
        playerProgress = EditorGUILayout.TextField("玩家当前引导进度:", playerProgress);
        setProgresssValue = EditorGUILayout.TextField("引导组ID:", setProgresssValue);
        progressForID = EditorGUILayout.TextField("是否完成:", progressForID);

        maxGroup = EditorGUILayout.TextField("最大引导组:", maxGroup);
        EditorGUILayout.LabelField("引导完成数值：                   " + maxGuidProgress);
        if (GUILayout.Button("查询进度"))
        {
            
            playerProgress = PlayerPrefs.GetString(playerID + "_guidProgress");
        }
        if (GUILayout.Button("设置进度"))
        {
            if (string.IsNullOrEmpty(playerID))
            {
                PlayerPrefs.SetString("guidProgress", playerProgress);
            }
            else
            {
                PlayerPrefs.SetString(playerID + "_guidProgress", playerProgress);
            }
        }
        if (GUILayout.Button("查询单个进度"))
        {
            progressForID = (GetProgressByIndex(int.Parse(setProgresssValue)) ? 1 : 0).ToString();
            //PlayerPrefs.SetInt(playerID + "_guidProgress", int.Parse(playerProgress));
        }
        if (GUILayout.Button("设置单个进度"))
        {
            SetProgressByIndex(int.Parse(setProgresssValue), int.Parse(progressForID) == 1);
            //PlayerPrefs.SetInt(playerID + "_guidProgress", int.Parse(playerProgress));
        }
        if (GUILayout.Button("获取当前引导完成数值"))
        {
            GetMaxProgress();
        }
    }

    public bool GetProgressByIndex(int index)
    {
        int progress = int.Parse(playerProgress);
        return ((progress >> index) & 1) == 1;
    }

    public void SetProgressByIndex(int index, bool complete)
    {
        int progress = int.Parse(playerProgress);
        if (complete)
        {
            progress = (1 << index) | progress;
        }
        else
        {
            progress = (~(1 << index)) & progress;
        }
        playerProgress = progress.ToString();
        PlayerPrefs.SetInt(playerID + "_guidProgress", progress);
    }

    public void GetMaxProgress()
    {
        int groupID = int.Parse(maxGroup);
        int max = 0;
        for (int i = 0; i < groupID; i++)
        {
            max += (int)Mathf.Pow(2,i);
        }
        maxGuidProgress = max.ToString();
    }

}
