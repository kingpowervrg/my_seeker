using Mono.Data.Sqlite;
using System;
using System.IO;
using UnityEngine;

/// <summary>
/// 客户端配置Sqlite加载类
/// </summary>
public class SQLiteLoad
{
    public static bool resLoaded = false;
    private static string loadName = "ConfData.bytes";
    public static string writeName = "ConfData.db";
    public static string tempPath = string.Empty;

    private static Action OnLoadCachedConfigFinished;

    public SQLiteLoad()
    {

    }
    private static void OnLoadFile(string name, UnityEngine.Object obj)
    {
        SqliteConnection.ClearAllPools();
        if (name != loadName)
        {
            Debug.LogWarning("invalid file: " + name + ", need: " + "ConfData.db");
            return;
        }
        TextAsset ta = obj as TextAsset;
        if (ta == null)
        {
            Debug.LogError("text asset is null");
            return;
        }
        string cStr = string.Empty;//数据库位于工程的根目录
#if UNITY_ANDROID || UNITY_IPHONE || UNITY_IOS
        cStr = Application.persistentDataPath + "/" + writeName;
        
#else
        string _tempPath = System.IO.Path.GetTempFileName();
        int _index = _tempPath.IndexOf(".tmp");
        string _newPath = _tempPath.Substring(0, _index);
        tempPath = _newPath;
        cStr = tempPath + "/" + writeName;
        Directory.CreateDirectory(tempPath);
#endif

        if (File.Exists(cStr))
        {
            File.Delete(cStr);
        }
        var buffer = ta.bytes;
        FileStream fileStream = new FileStream(cStr, FileMode.Create);
        fileStream.Write(buffer, 0, buffer.Length);
        fileStream.Close();

        ConfigUtil.ReleaseAsset(name, obj);
        D.log("sqlite date load  success");

        SqliteDriver.SQLiteHelper.Initialize(cStr, (e) => D.log(e), (e) => D.error(e));
        CacheLoaded();

        if (OnLoadCachedConfigFinished != null)
            OnLoadCachedConfigFinished();

    }

    private static void OnLoadFileFromStreamingAsset()
    {
        SqliteConnection.ClearAllPools();
        string cStr = string.Empty;//数据库位于工程的根目录

        string _tempPath = System.IO.Path.GetTempFileName();
        int _index = _tempPath.IndexOf(".tmp");
        string _newPath = _tempPath.Substring(0, _index);
        tempPath = _newPath;
        cStr = tempPath + "/" + writeName;
        Directory.CreateDirectory(tempPath);

        if (File.Exists(cStr))
        {
            File.Delete(cStr);
        }

        if (File.Exists(Application.streamingAssetsPath + "/" + loadName))
        {
            File.Copy(Application.streamingAssetsPath + "/" + loadName, cStr);
        }
        else
        {
            D.error("not find config DB");
        }

        D.log("sqlite date load  success");
        CacheLoaded();
        SqliteDriver.SQLiteHelper.Initialize(Application.streamingAssetsPath + "/" + loadName, (e) => D.log(e), (e) => D.error(e));
        
        if (OnLoadCachedConfigFinished != null)
            OnLoadCachedConfigFinished();

    }
    /// <summary>
    /// 需要进游戏后加入内存在这里添加
    /// </summary>
    private static void CacheLoaded()
    {
        resLoaded = true;

        //if (OnLoadCachedConfigFinished != null)
        //    OnLoadCachedConfigFinished();
    }
    public static void LoadSQLite(Action OnLoadFinish)
    {
        if (resLoaded)
            return;

        OnLoadCachedConfigFinished = OnLoadFinish;
#if UNITY_STANDALONE
        OnLoadFileFromStreamingAsset();
#else
        ConfigUtil.LoadRes(loadName, OnLoadFile);
#endif
    }
}