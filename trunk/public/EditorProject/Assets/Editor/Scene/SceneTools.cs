using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class SceneTools {

    public static string getConfigPath()
    {
        string curPath = Application.dataPath;
        int lastIndex = curPath.LastIndexOf("/");
        curPath = curPath.Substring(0, lastIndex);
        lastIndex = curPath.LastIndexOf("/");
        string configPath = curPath.Substring(0, lastIndex);
        return configPath + "/config/";
    }

    public static string getJsonFile(string path)
    {
        if (!File.Exists(path))
        {
            return string.Empty;
        }
        string content = string.Empty;
        StreamReader reader = new StreamReader(path, System.Text.Encoding.Default);
        content = reader.ReadToEnd();
        reader.Close();
        return content;
    }
}
