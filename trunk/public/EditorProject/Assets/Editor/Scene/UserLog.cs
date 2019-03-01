using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class SceneLogSyncData
{
    public string page;
    public long id;
    public int index;

    public SceneLogSyncData(string page, long id, int index)
    {
        this.page = page;
        this.id = id;
        this.index = index;
    }
}

public class UserLog{

    public const string SceneLogPath = "../sceneLog.txt";

    public static string CurrentDateTime
    {
        get {
            return DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "   ";
        }
    }

    private static void CheckSceneLog()
    {
        if (!File.Exists(UserLog.SceneLogPath))
        {
            FileStream filestream = File.Create(UserLog.SceneLogPath);
            filestream.Close();
        }
            
    }

    private static StringBuilder GetSceneLogHeader()
    {
        StringBuilder sceneLogBuilder = new StringBuilder();
        sceneLogBuilder.AppendLine("     ");
        sceneLogBuilder.AppendLine("-----------------------------------------");
        return sceneLogBuilder;
    }

    public static void OpenSceneLog()
    {
        CheckSceneLog();
        StringBuilder sceneLogBuilder = GetSceneLogHeader();
        sceneLogBuilder.AppendLine(CurrentDateTime + "打开场景编辑器");
        ApendSceneLog(sceneLogBuilder.ToString());
    }

    public static void CloseSceneLog()
    {
        StringBuilder sceneLogBuilder = GetSceneLogHeader();
        sceneLogBuilder.AppendLine(CurrentDateTime + "关闭场景编辑器");
        ApendSceneLog(sceneLogBuilder.ToString());
    }

    public static void SaveSceneLog()
    {
        StringBuilder sceneLogBuilder = GetSceneLogHeader();
        sceneLogBuilder.AppendLine(CurrentDateTime + "输出操作");
        ApendSceneLog(sceneLogBuilder.ToString());
    }

    public static void AddItemSceneLog(string page,long itemId)
    {
        StringBuilder sceneLogBuilder = GetSceneLogHeader();
        sceneLogBuilder.AppendLine(CurrentDateTime + "添加物品操作:  " + page + "  物品ID:" + itemId);
        ApendSceneLog(sceneLogBuilder.ToString());
    }

    public static void RemoveItemSceneLog(string page,long itemId)
    {
        StringBuilder sceneLogBuilder = GetSceneLogHeader();
        sceneLogBuilder.AppendLine(CurrentDateTime + "移除物品操作:  " + page + "  物品ID:" + itemId);
        ApendSceneLog(sceneLogBuilder.ToString());
    }

    public static void AddNodeSceneLog(string page, long itemId,int index)
    {
        StringBuilder sceneLogBuilder = GetSceneLogHeader();
        sceneLogBuilder.AppendLine(CurrentDateTime + "添加节点操作:  " + page + "  物品ID:" + itemId + "  节点Index:" + index);
        ApendSceneLog(sceneLogBuilder.ToString());
    }

    public static void RemoveNodeSceneLog(string page, long itemId, int index)
    {
        StringBuilder sceneLogBuilder = GetSceneLogHeader();
        sceneLogBuilder.AppendLine(CurrentDateTime + "移除节点操作:  " + page + "  物品ID:" + itemId + "  节点Index:" + index);
        ApendSceneLog(sceneLogBuilder.ToString());
    }

    public static void AddPageSceneLog(string page)
    {
        StringBuilder sceneLogBuilder = GetSceneLogHeader();
        sceneLogBuilder.AppendLine(CurrentDateTime + "添加页签操作:  " + page);
        ApendSceneLog(sceneLogBuilder.ToString());
    }

    public static void RemovePageSceneLog(string page)
    {
        StringBuilder sceneLogBuilder = GetSceneLogHeader();
        sceneLogBuilder.AppendLine(CurrentDateTime + "移除页签操作:  " + page);
        ApendSceneLog(sceneLogBuilder.ToString());
    }

    public static void RemovePageItemSceneLog(List<SceneLogSyncData> logData) 
    {
        StringBuilder sceneLogBuilder = GetSceneLogHeader();
        sceneLogBuilder.AppendLine(CurrentDateTime + "删除整组");
        for (int i = 0; i < logData.Count; i++)
        {
            sceneLogBuilder.AppendLine(logData[i].page + "  " + logData[i].id + "  " + logData[i].index);
        }
        ApendSceneLog(sceneLogBuilder.ToString());
    }

    public static void SyncAllItemSceneLog()
    {
        StringBuilder sceneLogBuilder = GetSceneLogHeader();
        sceneLogBuilder.AppendLine(CurrentDateTime + "同步所有数据");
        ApendSceneLog(sceneLogBuilder.ToString());
    }

    public static void SyncAllPageSceneLog(string srcPage,string targetPage)
    {
        StringBuilder sceneLogBuilder = GetSceneLogHeader();
        sceneLogBuilder.AppendLine(CurrentDateTime + "同步页签数据:  srcPage: " + srcPage + "  targetPage:" + targetPage);
        ApendSceneLog(sceneLogBuilder.ToString());
    }

    public static void SyncSceneLog(List<SceneLogSyncData> sceneLogData,Vector3 pos,Vector3 rotate,Vector3 scale)
    {
        StringBuilder sceneLogBuilder = GetSceneLogHeader();
        sceneLogBuilder.AppendLine(CurrentDateTime + "同步操作:  位置:" + pos.ToString() + "  旋转:" + rotate.ToString() + "  缩放:" + scale.ToString());
        SyncCurrentItemSceneLog(sceneLogData,sceneLogBuilder);
    }

    public static void SyncSingleSceneLog(List<SceneLogSyncData> sceneLogData, Vector3 pos, Vector3 rotate, Vector3 scale)
    {
        StringBuilder sceneLogBuilder = GetSceneLogHeader();
        sceneLogBuilder.AppendLine(CurrentDateTime + "同步单个操作:  位置:" + pos.ToString() + "  旋转:" + rotate.ToString() + "  缩放:" + scale.ToString());
        SyncCurrentItemSceneLog(sceneLogData, sceneLogBuilder);
    }

    private static void SyncCurrentItemSceneLog(List<SceneLogSyncData> sceneLogData, StringBuilder sceneLogBuilder)
    {
        for (int i = 0; i < sceneLogData.Count; i++)
        {
            if (0 == i)
            {
                sceneLogBuilder.AppendLine("                 当前物件: " + sceneLogData[i].page + "  " + sceneLogData[i].id + "  " + sceneLogData[i].index);
            }
            else
            {
                sceneLogBuilder.AppendLine("                          " + sceneLogData[i].page + "  " + sceneLogData[i].id + "  " + sceneLogData[i].index);
            }
        }
        ApendSceneLog(sceneLogBuilder.ToString());
    }

    public static void ApendSceneLog(string log)
    {
        StreamWriter writer = new StreamWriter(UserLog.SceneLogPath, true,Encoding.Default);
        writer.WriteLine(log);
        writer.Close();
    }
}
