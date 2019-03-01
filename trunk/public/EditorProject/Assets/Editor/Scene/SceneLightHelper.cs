using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

public class SceneObjLightData
{
    public ObjLightMapDataGroup[] groups { get; set; }
}

public class ObjLightMapDataGroup
{
    public long id { get; set; }
    public ObjLightMapData[] objLightDatas { get; set; }
}

public class MColor
{
    public float r { get; set; }
    public float g { get; set; }
    public float b { get; set; }
    public MColor() { }

    public MColor(Color c)
    {
        this.r = c.r;
        this.g = c.g;
        this.b = c.b;
    }
}

/// <summary>
/// 单个位置对应的光照贴图
/// </summary>
public class ObjLightMapData
{
    public float lightIndex { get; set; }
    public float tilingX { get; set; }
    public float tilingY { get; set; }
    public float offsetX { get; set; }
    public float offsetY { get; set; }
    public MColor[] color { get; set; }
}

public class SceneLightHelper {

    public static bool SceneBake()
    {
        Lightmapping.Clear();
        return Lightmapping.Bake();
    }

    private static ObjLightMapData getObjLightMapData(GameObject obj, Texture2D lightTex)
    {
        MeshRenderer render = obj.GetComponent<MeshRenderer>();
        if (render == null)
        {
            return null;
        }
        ObjLightMapData objdata = new ObjLightMapData();
        objdata.lightIndex = render.lightmapIndex;
        objdata.tilingX = render.lightmapScaleOffset.x;
        objdata.tilingY = render.lightmapScaleOffset.y;
        objdata.offsetX = render.lightmapScaleOffset.z;
        objdata.offsetY = render.lightmapScaleOffset.w;
        List<MColor> sceneColors = CreateObjLighting(objdata,lightTex);
        if (sceneColors == null)
        {
            return objdata;
        }
        objdata.color = sceneColors.ToArray();
        return objdata;
    }

    static List<MColor> CreateObjLighting(ObjLightMapData light,Texture2D lightTex)
    {
        if (light == null)
        {
            return null;
        }
        if (lightTex == null)
        {
            return null; 
        }
        int lightWidth = lightTex.width;
        int lightHeigh = lightTex.height;

        int startX = (int)(Mathf.Abs(light.offsetX) * lightWidth);
        int startY = (int)(Mathf.Abs(light.offsetY) * lightHeigh);
        int scaleX = (int)(light.tilingX * lightWidth);
        int scaleY = (int)(light.tilingY * lightHeigh);

        List<MColor> colors = new List<MColor>();
        int width = scaleX;
        int heigh = scaleY;
        Texture2D tex = new Texture2D(width, heigh);

        for (int i = startX; i < startX + scaleX; i++)
        {
            for (int j = startY; j < startY + scaleY; j++)
            {
                Color c = lightTex.GetPixel(i, j);
                tex.SetPixel(i - startX,j - startY,c);
                //colors.Add(new MColor(c));
            }
        }
        tex.Apply();
        byte[] texByte = tex.EncodeToPNG();
        FileStream file = File.Open(Application.dataPath + "/testLight.png", FileMode.Create);
        BinaryWriter writer = new BinaryWriter(file);
        writer.Write(texByte);
        file.Close();
        writer.Close();
        return colors;
    }

    public static void createObjLightData(SceneItemInfo itemInfos,string dir, List<EditorSceneObj> itemObjs, Texture2D lightTex)
    {
        SceneObjLightData sceneLightData = new SceneObjLightData();
        List<ObjLightMapDataGroup> groups = new List<ObjLightMapDataGroup>();
        for (int i = 0; i < itemInfos.items.Count; i++)
        {
            ItemInfo itemInfo = itemInfos.items[i];
            ObjLightMapDataGroup groupData = createObjLightData(itemObjs[i],itemInfo.itemID,lightTex);
            groups.Add(groupData);
        }
        sceneLightData.groups = groups.ToArray();
        string jsonStr = fastJSON.JSON.ToJSON(sceneLightData);
        CreateJson(dir,jsonStr);
        //createObjLightData();
    }

    /// <summary>
    /// 生成单类物体的光照信息  多个位置
    /// </summary>
    /// <param name="objs"></param>
    /// <param name="id"></param>
    static ObjLightMapDataGroup createObjLightData(EditorSceneObj objs,long id, Texture2D lightTex)
    {
        ObjLightMapDataGroup dataGroup = new ObjLightMapDataGroup();
        dataGroup.id = id;
        List<ObjLightMapData> objPosMapData = new List<ObjLightMapData>();
        for (int i = 0; i < objs.itemObj.Count; i++)
        {
            GameObject posObj = objs.itemObj[i];
            ObjLightMapData mapdata = getObjLightMapData(posObj, lightTex);
            objPosMapData.Add(mapdata);
        }
        dataGroup.objLightDatas = objPosMapData.ToArray();
        return dataGroup;

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

    public static void CreateServerJson(string path, string jsonStr)
    {
        if (File.Exists(path))
        {
            File.Delete(path);
        }
        byte[] jsonByte = System.Text.Encoding.UTF8.GetBytes(jsonStr);
        using (FileStream fsWrite = new FileStream(path, FileMode.Create))
        {
            fsWrite.Write(jsonByte, 0, jsonByte.Length);
            fsWrite.Close();
        }

    }

    /// <summary>
    /// 检测当前数据是否合法
    /// </summary>
    /// <param name="itemInfo"></param>
    /// <param name="sceneObjs"></param>
    /// <returns></returns>
    public static bool CheckDataSafe(SceneItemInfo itemInfo, List<EditorSceneObj> sceneObjs)
    {
        int itemCount = 0;
        int objCount = 0;
        for (int i = 0; i < itemInfo.items.Count; i++)
        {
            itemCount += itemInfo.items[i].itemPos.Count;
        }
        for (int j = 0; j < sceneObjs.Count; j++)
        {
            objCount += sceneObjs[j].itemObj.Count;
        }
        return itemCount == objCount;
    }

    public static void setLightMapData(SceneItemInfo itemInfo,List<EditorSceneObj> sceneObjs)
    {
        if (!CheckDataSafe(itemInfo,sceneObjs))
        {
            return;
        }

        for (int i = 0; i < sceneObjs.Count; i++)
        {
            for (int j = 0; j < sceneObjs[i].itemObj.Count; j++)
            {
                GameObject obj = sceneObjs[i].itemObj[j];
                if (obj == null)
                {
                    continue;
                }
                MeshRenderer render = obj.GetComponent<MeshRenderer>();
                ItemPosInfo posInfo = itemInfo.items[i].itemPos[j];
                SetLightData(render,posInfo);
            }
        }
    }

    public static void SetLightData(MeshRenderer render,ItemPosInfo posInfo)
    {
        posInfo.lightIndex = render.lightmapIndex;
        posInfo.tilingX = render.lightmapScaleOffset.x;
        posInfo.tilingY = render.lightmapScaleOffset.y;
        posInfo.offsetX = render.lightmapScaleOffset.z;
        posInfo.offsetY = render.lightmapScaleOffset.w;
    }

    public static void getLightData(MeshRenderer render,ItemPosInfo posInfo)
    {
        render.lightmapIndex = posInfo.lightIndex;
        render.lightmapScaleOffset = new Vector4(posInfo.tilingX,posInfo.tilingY,posInfo.offsetX,posInfo.offsetY);
    }
}



