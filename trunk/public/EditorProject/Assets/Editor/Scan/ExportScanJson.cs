using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.SceneManagement;
using System.IO;
using conf;
using System;
using UnityEngine.UI;

/// <summary>
/// 场景数据保存命名规则
/// 场景文件夹
///     场景数据_1
///     场景数据_2
/// </summary>
public class ExportScanJson : MonoBehaviour
{

    private const string C_TEMPLATE_NAME = "Scan_Template_";
    private const string C_TEMPLATE_PATH = "/Res/Gui/Scan/Prefab/";
    private const string C_JSON_PATH = "/Res/Gui/Scan/Json/";

    [MenuItem("Tools/导出尸检模板数据")]
    static void ExportScanJsonData()
    {
        List<ScanJsonData> jsons = FindAllTemlates();
        ConvertToJson(jsons);

    }

    private static List<ScanJsonData> FindAllTemlates()
    {
        List<ScanJsonData> ret = new List<ScanJsonData>();

        string temp_path = string.Format("{0}{1}", Application.dataPath, C_TEMPLATE_PATH);

        string[] fileNames = Directory.GetFiles(temp_path, "*.prefab");

        foreach (var fileName in fileNames)
        {
            string only_file_anme = Path.GetFileName(fileName);

            if (only_file_anme.Contains(C_TEMPLATE_NAME))
            {
                string asset_file_path = string.Format("{0}{1}{2}", "Assets", C_TEMPLATE_PATH, only_file_anme);
                UnityEngine.Object ui_prefab = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(asset_file_path);


                GameObject template = PrefabUtility.InstantiatePrefab(ui_prefab) as GameObject;

                //>尸检id
                string[] template_words = template.name.Split('_');
                int template_id = int.Parse(template_words[2]);
                //>

                //<尸检图片名
                RawImage r_img = template.transform.Find("Tex").GetComponent<RawImage>();
                string tex_name = System.IO.Path.GetFileName(AssetDatabase.GetAssetPath(r_img.texture));
                Rect tex_rect = new Rect(r_img.rectTransform.anchoredPosition.x, r_img.rectTransform.anchoredPosition.y, r_img.rectTransform.sizeDelta.x, r_img.rectTransform.sizeDelta.y);
                //>

                //<线索id，以及位置
                GameObject anchors_root = template.transform.Find("Anchors").gameObject;
                List<ScanAnchorJsonData> anchor_datas = new List<ScanAnchorJsonData>();


                foreach (Transform child in anchors_root.transform)
                {
                    if (!child.gameObject.activeSelf)
                        continue;

                    int clue_id;

                    if (!int.TryParse(child.name, out clue_id))
                    {
                        Debug.LogError($"Scan Temp {only_file_anme} child {child.name} is not int");
                        continue;
                    }

                    RectTransform rect = child.GetComponent<RectTransform>();

                    ScanAnchorJsonData anchor_data = new ScanAnchorJsonData
                    {
                        M_clue_id = clue_id,
                        M_x = rect.anchoredPosition.x,
                        M_y = rect.anchoredPosition.y,
                        M_w = rect.sizeDelta.x,
                        M_h = rect.sizeDelta.y,
                    };

                    anchor_datas.Add(anchor_data);

                }
                //>

                GameObject.DestroyImmediate(template);


                ScanJsonData data = new ScanJsonData()
                {
                    M_id = template_id,
                    M_tex_name = tex_name,
                    M_tex_x = tex_rect.x,
                    M_tex_y = tex_rect.y,
                    M_tex_w = tex_rect.width,
                    M_tex_h = tex_rect.height,
                    M_anchors = new List<ScanAnchorJsonData>(anchor_datas),
                };

                ret.Add(data);

                Resources.UnloadAsset(ui_prefab);
            }
        }

        return ret;
    }

    private static void ConvertToJson(List<ScanJsonData> datas_)
    {

        foreach (var data in datas_)
        {
            int scan_id = data.M_id;

            string json_str = fastJSON.JSON.ToJSON(data);

            string temp_path = $"{Application.dataPath}{C_JSON_PATH}Scan{scan_id}.json";
            CreateJson(temp_path, json_str);
        }

        Debug.Log("尸检数据导出完毕");

    }

    public static void CreateJson(string path, string jsonStr)
    {
        byte[] jsonByte = System.Text.Encoding.UTF8.GetBytes(jsonStr);
        using (FileStream fsWrite = new FileStream(path, FileMode.Create))
        {
            fsWrite.Write(jsonByte, 0, jsonByte.Length);
            fsWrite.Close();
        }
    }
}
