using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using System.IO;

public class LightingData
{
    public const string constConfigPath = "/Res/SceneConfig/";

    public string sceneName;

    public string sceneConfigPath; //场景数据目录

    public string excelPath;

    public string scenePath;

    public string sceneLightMapPath;

    public string sceneLightDataAssetPath;

    private const float ErrorNumber = 0.1f;

    public LightingData(string sceneName,string sceneConfigPath,string excelPath,string scenePath)
    {
        this.sceneName = sceneName;
        this.sceneConfigPath = sceneConfigPath + sceneName;
        this.excelPath = excelPath;
        this.scenePath = scenePath + sceneName;
        this.sceneLightMapPath = this.scenePath + "/Lightmap-0_comp_light.exr";
        this.sceneLightDataAssetPath = this.scenePath + "/LightingData.asset";
    }

    //物件表
    private List<BaseItem> m_exhibitItem = new List<BaseItem>();
    public List<BaseItem> exhibitItem {
        get {
            if (m_exhibitItem.Count == 0)
            {
                System.Reflection.PropertyInfo[] proInfo;
                conf.ConfHelper.LoadFromExcel<BaseItem>(excelPath + "exhibit.xlsx", 0, out m_exhibitItem, out proInfo);
            }
            return m_exhibitItem;
        }
    }
    public BaseItem GetExhibitByID(long Id)
    {
        for (int i = 0; i < exhibitItem.Count; i++)
        {
            if (exhibitItem[i].id == Id)
            {
                return exhibitItem[i];
            }
        }
        return null;
    }

    //物件配置数据
    public Dictionary<string, SceneItemJson> m_allExhibitData = new Dictionary<string, SceneItemJson>();
    public void AddExhibitData(string groupName, SceneItemJson exhibitData)
    {
        if (!m_allExhibitData.ContainsKey(groupName))
        {
            m_allExhibitData.Add(groupName, exhibitData);
        }
        else
        {
            m_allExhibitData[groupName] = exhibitData;
        }
    }

    //物件对象
    public List<ExhibitObjData> m_allExhibitObj = new List<ExhibitObjData>();
    public void AddExhibitObj(ExhibitObjData obj)
    {
        m_allExhibitObj.Add(obj);
    }
    public ExhibitObjData CheckExhibit(long propId, ItemPosInfoJson posInfo)
    {
        for (int i = 0; i < m_allExhibitObj.Count; i++)
        {
            if (m_allExhibitObj[i].m_propId == propId)
            {
                UnityEngine.GameObject mObj = m_allExhibitObj[i].gameObject;
                if (Mathf.Abs(mObj.transform.position.x - posInfo.pos.x) < LightingData.ErrorNumber && Mathf.Abs(mObj.transform.position.y - posInfo.pos.y) < LightingData.ErrorNumber && Mathf.Abs(mObj.transform.position.z - posInfo.pos.z) < LightingData.ErrorNumber)
                {
                    if (Mathf.Abs(mObj.transform.eulerAngles.x - posInfo.rotate.x) < LightingData.ErrorNumber && Mathf.Abs(mObj.transform.eulerAngles.y - posInfo.rotate.y) < LightingData.ErrorNumber && Mathf.Abs(mObj.transform.eulerAngles.z - posInfo.rotate.z) < LightingData.ErrorNumber)
                    {
                        if (Mathf.Abs(mObj.transform.localScale.x - posInfo.scale.x) < LightingData.ErrorNumber && Mathf.Abs(mObj.transform.localScale.y - posInfo.scale.y) < LightingData.ErrorNumber && Mathf.Abs(mObj.transform.localScale.z - posInfo.scale.z) < LightingData.ErrorNumber)
                        {
                            return m_allExhibitObj[i];
                        }
                    }
                }
            }
        }
        return null;
    }

    public Dictionary<MeshRenderer, SceneLightData> m_sceneLightData = new Dictionary<MeshRenderer, SceneLightData>();
    public void SetSceneLightData(MeshRenderer obj,SceneLightData lightData)
    {
        m_sceneLightData.Add(obj,lightData);
    }

    public void ClearSceneLightData()
    {
        m_sceneLightData.Clear();
    }

    public void SaveLightMap()
    {
        for (int i = 0; i < m_allExhibitObj.Count; i++)
        {
            ExhibitObjData objData = m_allExhibitObj[i];
            string groupFileName = objData.m_groupFileName;

            if (m_allExhibitData.ContainsKey(groupFileName))
            {
                ItemInfoJson infoJson = m_allExhibitData[groupFileName].items[objData.m_itemIndex];
                m_allExhibitData[groupFileName].lightMapName = sceneName + ".exr";
                if (infoJson.itemID == objData.m_propId)
                {
                    GetLightMapForGameObject(infoJson.itemPos[objData.m_posIndex], objData.gameObject);
                }
                else
                {
                    Debug.LogError("lightmap error:" + objData.m_propId);
                }
            }
        }
        SaveLightMapToFile();
    }

    public void SetLightMap()
    {
        for (int i = 0; i < m_allExhibitObj.Count; i++)
        {
            ExhibitObjData objData = m_allExhibitObj[i];
            string groupFileName = objData.m_groupFileName;

            if (m_allExhibitData.ContainsKey(groupFileName))
            {
                ItemInfoJson infoJson = m_allExhibitData[groupFileName].items[objData.m_itemIndex];
                if (infoJson.itemID == objData.m_propId)
                {
                    GetLightMapForGameObject(infoJson.itemPos[objData.m_posIndex], objData.gameObject);
                }
                else
                {
                    Debug.LogError("lightmap error:" + objData.m_propId);
                }
            }
        }
        SaveLightMapToFile();
    }

    private void SaveLightMapToFile()
    {
        foreach (var kv in m_allExhibitData)
        {
            string json = fastJSON.JSON.ToJSON(kv.Value);
            StreamWriter writer = new StreamWriter(sceneConfigPath + "/" + kv.Key, false, Encoding.Default);
            writer.Write(json);
            writer.Close();
        }
    }

    private void GetLightMapForGameObject(ItemPosInfoJson infoJson,GameObject obj)
    {
        MeshRenderer render = obj.GetComponent<MeshRenderer>();
        if (render == null)
        {
            return;
        }
        infoJson.lightIndex = render.lightmapIndex;
        infoJson.tilingX = render.lightmapScaleOffset.x;
        infoJson.tilingY = render.lightmapScaleOffset.y;
        infoJson.offsetX = render.lightmapScaleOffset.z;
        infoJson.offsetY = render.lightmapScaleOffset.w;
    }

    
}

public class ExhibitObjData
{
    public string m_groupFileName;
    public long m_propId;
    public int m_itemIndex;
    public int m_posIndex;
    public UnityEngine.GameObject gameObject;

    public ExhibitObjData(string groupFileName,long propId,int posIndex,int itemIndex, GameObject obj)
    {
        m_groupFileName = groupFileName;
        m_propId = propId;
        m_posIndex = posIndex;
        gameObject = obj;
        m_itemIndex = itemIndex;
    }
}

public class SceneLightData
{
    public int lightIndex;
    public Vector4 lightData;

    public SceneLightData(int index,Vector4 lightData)
    {
        this.lightIndex = index;
        this.lightData = lightData;
    }
}