using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;


public class CartoonJsonUtil
{
    private const string GAME_JSON_PATH = "Res/Cartoon/GameJson";
    private const string CONFIG_JSON_PATH = "Res/Cartoon/CartoonConfig";


    public static void SaveLevelJsonData(CartoonTemplate temp_)
    {
        CartoonItemJson item_json = new CartoonItemJson();
        item_json.Item_id = temp_.m_template_id;
        item_json.M_cartoons = new List<CartoonVideoNamesJson>();

        temp_.Init();

        foreach (var item in temp_.m_cartoon_items)
        {
            CartoonVideoNamesJson names = new CartoonVideoNamesJson();
            names.M_names = new List<string>();

            foreach (var clip in item.m_videos)
            {
                names.M_names.Add(clip.name);
            }

            item_json.M_cartoons.Add(names);
        }

        string j_str = fastJSON.JSON.ToJSON(item_json);
        string _path = GAME_JSON_PATH;

        if (!_path.StartsWith("/"))
            _path = _path.Insert(0, "/");

        if (!_path.EndsWith("/"))
            _path = _path + "/";

        string temp_path = string.Format("{0}{1}{2}{3}", Application.dataPath, _path, item_json.Item_id, ".json");
        ExportJigsawJson.CreateJson(temp_path, j_str);
    }


    public static void SaveAllJsonData()
    {

        string _path = GAME_JSON_PATH;

        if (!_path.StartsWith("/"))
            _path = _path.Insert(0, "/");

        string temp_path = string.Format("{0}{1}", Application.dataPath, _path);

        string[] fileNames = Directory.GetFiles(temp_path, "*.json");
        CartoonData all_data = new CartoonData();
        all_data.M_cartoons = new List<CartoonItemJson>();

        CartoonItemJson cur_file;
        byte[] jsonByte;
        foreach (var fileName in fileNames)
        {
            using (FileStream fsReader = new FileStream(fileName, FileMode.Open))
            {

                jsonByte = new byte[fsReader.Length];
                fsReader.Read(jsonByte, 0, (int)fsReader.Length);
                fsReader.Close();

                string jsonStr = System.Text.Encoding.UTF8.GetString(jsonByte);

                cur_file = fastJSON.JSON.ToObject<CartoonItemJson>(jsonStr);

                all_data.M_cartoons.Add(cur_file);
            }
        }


        _path = CONFIG_JSON_PATH;

        if (!_path.StartsWith("/"))
            _path = _path.Insert(0, "/");

        if (!_path.EndsWith("/"))
            _path = _path + "/";

        temp_path = string.Format("{0}{1}{2}{3}", Application.dataPath, _path, "Cartoon",".json");

        string j_str = fastJSON.JSON.ToJSON(all_data);

        ExportJigsawJson.CreateJson(temp_path, j_str);
    }

    public static CartoonItemJson LoadLevelJsonData(long item_id_)
    {
        CartoonItemJson ret = null;

        string _path = GAME_JSON_PATH;

        if (!_path.StartsWith("/"))
            _path = _path.Insert(0, "/");

        if (!_path.EndsWith("/"))
            _path = _path + "/";

        string temp_path = string.Format("{0}{1}{2}{3}", Application.dataPath, _path, item_id_, ".json");

        if (!File.Exists(temp_path))
            return null;

        byte[] jsonByte;// = System.Text.Encoding.UTF8.GetBytes(jsonStr);
        using (FileStream fsReader = new FileStream(temp_path, FileMode.Open))
        {

            jsonByte = new byte[fsReader.Length];
            fsReader.Read(jsonByte, 0, (int)fsReader.Length);
            fsReader.Close();

            string jsonStr = System.Text.Encoding.UTF8.GetString(jsonByte);

            ret = fastJSON.JSON.ToObject<CartoonItemJson>(jsonStr);
        }

        return ret;
    }

    public static List<string> GetLevelJsonFileNamesWithoutEx()
    {
        List<string> ret = new List<string>();

        string _path = GAME_JSON_PATH;

        if (!_path.StartsWith("/"))
            _path = _path.Insert(0, "/");

        string temp_path = string.Format("{0}{1}", Application.dataPath, _path);

        string[] fileNames = Directory.GetFiles(temp_path, "*.json");

        foreach (var fileName in fileNames)
        {
            string only_file_name = Path.GetFileNameWithoutExtension(fileName);
            ret.Add(only_file_name);
        }

        return ret;


    }



}

