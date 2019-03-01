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
public class ExportJigsawJson : MonoBehaviour
{

    private const string C_TEMPLATE_NAME = "Jigsaw_Template_";
    private const string C_TEMPLATE_PATH = "/Res/Gui/Jigsaw_Template/";
    private const string C_JSON_PATH = "/Res/Gui/JigsawConfig/Jigsaw.json";

    [MenuItem("Tools/导出拼图模板数据")]
    static void ExportJigsawData()
    {
        List<JigsawDataJson> jsons = FindAllTemlates();
        ConvertToJson(jsons);

    }

    private static List<JigsawDataJson> FindAllTemlates()
    {
        List<JigsawDataJson> ret = new List<JigsawDataJson>();

        string temp_path = string.Format("{0}{1}", Application.dataPath, C_TEMPLATE_PATH);

        string[] fileNames = Directory.GetFiles(temp_path, "*.prefab");

        Comparison<int> comparison = (x, y) => { if (x > y) return -1; else if (x == y) return 0; else return 1; };

        foreach (var fileName in fileNames)
        {
            string only_file_anme = Path.GetFileName(fileName);

            if (only_file_anme.Contains(C_TEMPLATE_NAME))
            {
                string asset_file_path = string.Format("{0}{1}{2}", "Assets", C_TEMPLATE_PATH, only_file_anme);
                UnityEngine.Object ui_prefab = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(asset_file_path);


                GameObject template = PrefabUtility.InstantiatePrefab(ui_prefab) as GameObject;

                string[] template_words = template.name.Split('_');
                int template_id = int.Parse(template_words[2]);

                GameObject chip_root = template.transform.Find("Panel").gameObject;

                List<int> chip_ids = new List<int>();
                List<JigsawChipJson> chips = new List<JigsawChipJson>();

                foreach (Transform child in chip_root.transform)
                {
                    if (!child.gameObject.activeSelf)
                        continue;

                    int chip_id = int.Parse(child.name);
                    chip_ids.Add(chip_id);

                    RectTransform rect = child.Find("RawImage").GetComponent<RectTransform>();
                    RawImage image = child.Find("RawImage").GetComponent<RawImage>();

                    string tex_name = System.IO.Path.GetFileName(AssetDatabase.GetAssetPath(image.texture));
                    RectJson rect_j = new RectJson(rect.anchoredPosition.x, rect.anchoredPosition.y, rect.sizeDelta.x, rect.sizeDelta.y);

                    JigsawChipJson chip_j = new JigsawChipJson
                    {
                        M_chip_name = child.name,
                        M_tex_anme = tex_name,
                        M_tex_size = rect_j
                    };

                    chips.Add(chip_j);
                }

                GameObject.DestroyImmediate(template);
                //Resources.UnloadAsset(ui_prefab);

                chip_ids.Sort(comparison);
                int matrix_index = chip_ids[0];

                int row = matrix_index / 10;
                int col = matrix_index % 10;

                int dimension = row > col ? row : col;
                dimension += 1;

                JigsawDataJson data = new JigsawDataJson()
                {
                    M_chips = chips,
                    M_dimention = dimension,
                    M_template_id = template_id
                };

                ret.Add(data);

                //Resources.UnloadAsset(template);
            }
        }

        return ret;
    }

    private static void ConvertToJson(List<JigsawDataJson> jsons_)
    {

        JigsawData data = new JigsawData()
        {
            M_jon_datas = jsons_,
        };
        string j_str = fastJSON.JSON.ToJSON(data);

        string temp_path = string.Format("{0}{1}", Application.dataPath, C_JSON_PATH);
        CreateJson(temp_path, j_str);


        Debug.Log("拼图数据导出完毕");

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
