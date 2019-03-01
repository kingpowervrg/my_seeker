using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class LightSystem {

    protected LightingData m_data;

    private bool m_isBrake = false;
    public void SetBrake()
    {
        m_isBrake = true;
    }

    public LightSystem(LightingData m_data)
    {
        this.m_data = m_data;
    }

    public void OnInit()
    {
        LoadConfigData();
        int count = m_data.exhibitItem.Count;
    }

    protected void LoadConfigData()
    {
        string sceneConfigPath = m_data.sceneConfigPath;
        if (!Directory.Exists(sceneConfigPath))
        {
            return;
        }
        string[] files = Directory.GetFiles(sceneConfigPath);
        for (int i = 0; i < files.Length; i++)
        {
            if (!files[i].Contains(".json") || files[i].Contains("meta"))
            {
                continue;
            }
            StreamReader reader = new StreamReader(files[i]);
            string jsonStr = reader.ReadToEnd();
            reader.Close();
            SceneItemJson groupData = fastJSON.JSON.ToObject<SceneItemJson>(jsonStr);
            string fileName = Path.GetFileName(files[i]);
            m_data.AddExhibitData(fileName, groupData);
        }
    }

    public void LoadModel()
    {
        foreach (var kv in m_data.m_allExhibitData)
        {
            for (int i = 0; i < kv.Value.items.Count; i++)
            {
                ItemInfoJson itemInfo = kv.Value.items[i];
                for (int j = 0; j < itemInfo.itemPos.Count; j++)
                {
                    LoadModel(itemInfo.itemID, itemInfo.itemPos[j], j,i, kv.Key);
                }
            }
        }
    }

    private void LoadModel(long id, ItemPosInfoJson posInfo,int posIndex,int itemIndex,string groupFileName)
    {
        BaseItem baseItem = m_data.GetExhibitByID(id);
        if (baseItem == null)
        {
            Debug.LogError("exhibit id is not exist:" + id);
            return;
        }

        ExhibitObjData exhibitObjData = m_data.CheckExhibit(id, posInfo);
        if (exhibitObjData != null)
        {
            ExhibitObjData objdataTemp = new ExhibitObjData(groupFileName, id, posIndex, itemIndex, exhibitObjData.gameObject);

            m_data.AddExhibitObj(objdataTemp);
            return;
        }
        GameObject obj = LoadGameObject(baseItem.model,posInfo);
        obj.name = groupFileName +"/" +id + "/" + posIndex;
        ExhibitObjData objdata = new ExhibitObjData(groupFileName, id, posIndex,itemIndex, obj);
        m_data.AddExhibitObj(objdata);
    }

    protected virtual GameObject LoadGameObject(string modelPath, ItemPosInfoJson posInfo)
    {
        GameObject obj = UnityEditor.AssetDatabase.LoadAssetAtPath<GameObject>("Assets/" + modelPath + ".prefab");
        obj = GameObject.Instantiate(obj) as GameObject;
        MeshRenderer render = obj.GetComponent<MeshRenderer>();
        if (render != null)
        {
            obj.isStatic = true;
            render.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            render.receiveShadows = false;
            render.lightProbeUsage = UnityEngine.Rendering.LightProbeUsage.Off;
            render.reflectionProbeUsage = UnityEngine.Rendering.ReflectionProbeUsage.Off;
        }
        obj.transform.position = new Vector3(posInfo.pos.x, posInfo.pos.y, posInfo.pos.z);
        obj.transform.eulerAngles = new Vector3(posInfo.rotate.x, posInfo.rotate.y, posInfo.rotate.z);
        obj.transform.localScale = new Vector3(posInfo.scale.x, posInfo.scale.y, posInfo.scale.z);
        return obj;
    }

    public void SaveLightMap()
    {
        m_data.SaveLightMap();
    }
    //保存原始场景
    public void SaveSceneLightMap()
    {
        MoveFile(m_data.sceneLightMapPath, m_data.sceneConfigPath + "/srcLightMap.exr");
        //MoveFile(m_data.sceneLightDataAssetPath,m_data.sceneConfigPath + "/LightingData.asset");
        m_data.ClearSceneLightData();
        GameObject[] root = GameObject.FindGameObjectsWithTag("LightingMap");
        for (int i = 0; i < root.Length; i++)
        {
            MeshRenderer[] meshRender = root[i].GetComponentsInChildren<MeshRenderer>();
            for (int j = 0; j < meshRender.Length; j++)
            {
                MeshRenderer render = meshRender[j];
                SceneLightData lightData = new SceneLightData(render.lightmapIndex, render.lightmapScaleOffset);
                m_data.SetSceneLightData(render,lightData);
            }
        }
    }

    //恢复原始场景
    public void RecoverSceneLightMap()
    {
        if (!m_isBrake)
        {
            return;
        }

        MoveFile(m_data.sceneLightMapPath, m_data.sceneConfigPath + "/" + m_data.sceneName +".exr");
        //MoveFile(m_data.sceneConfigPath + "/LightingData.asset",m_data.sceneLightDataAssetPath);
        string srcLightPath = m_data.sceneConfigPath + "/srcLightMap.exr";
        if (File.Exists(srcLightPath))
        {
            MoveFile(srcLightPath, m_data.sceneLightMapPath);
            //File.Move(srcLightPath,m_data.sceneLightMapPath);
            Texture2D tex = UnityEditor.AssetDatabase.LoadAssetAtPath(m_data.sceneLightMapPath,typeof(Texture2D)) as Texture2D;
            LightmapData mapData = new LightmapData();
            mapData.lightmapColor = tex;
            LightmapSettings.lightmaps[0] = mapData;
        }
        foreach (var kv in m_data.m_sceneLightData)
        {
            kv.Key.lightmapIndex = kv.Value.lightIndex;
            kv.Key.lightmapScaleOffset = kv.Value.lightData;
        }
        UnityEditor.AssetDatabase.Refresh();
    }

    private void MoveFile(string srcFile,string destFile)
    {
        if (File.Exists(srcFile))
        {
            if (File.Exists(destFile))
            {
                File.Delete(destFile);
            }
            File.Move(srcFile, destFile);
        }
    }

    public void BakeScene()
    {
        m_isBrake = true;
        SaveSceneLightMap();
        UnityEditor.Lightmapping.Bake();
    }

    public void BakeOver()
    {
        SaveLightMap();
    }

    public void ClearGameObject()
    {
        for (int i = 0; i < m_data.m_allExhibitObj.Count; i++)
        {
            if (m_data.m_allExhibitObj[i].gameObject != null)
            {
                GameObject.DestroyImmediate(m_data.m_allExhibitObj[i].gameObject);
            }
        }
        m_data.m_allExhibitObj.Clear();
    }
}
