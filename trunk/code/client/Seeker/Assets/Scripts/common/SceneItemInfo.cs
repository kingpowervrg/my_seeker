using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ItemType
{
    Normal,
    Task
}


/// <summary>
/// 场景的一组数据
/// </summary>
public class SceneItemInfo : ScriptableObject
{
    public string lightMapName;
    //private List<ItemInfo> m_items;// = new List<ItemInfo>();
    [SerializeField]
    public List<ItemInfo> items = new List<ItemInfo>();
}

/// <summary>
/// 物品单个信息
/// </summary>
[System.Serializable]
public class ItemInfo
{
    public long itemID;
    public string itemName;
    public float itemPercent;
    //private List<ItemPosInfo> m_itemPos = new List<ItemPosInfo>();
    [SerializeField]
    public List<ItemPosInfo> itemPos = new List<ItemPosInfo>();
}

/// <summary>
/// 物品位置信息
/// </summary>
[System.Serializable]
public class ItemPosInfo
{
    public Vector3 pos; //位置
    public ItemType type; //类型
    public Vector3 rotate; //旋转角度
    public Vector3 scale;
    public float percent; //百分比

    

    public int lightIndex;
    public float tilingX;
    public float tilingY;
    public float offsetX;
    public float offsetY;

    public ItemPosInfo()
    {
        pos = Vector3.zero;
        type = ItemType.Normal;
        rotate = Vector3.zero;
        scale = Vector3.one;
        percent = 100;
    }
}


/// <summary>
/// 物件基础表
/// </summary>
public class BaseItem
{
    public int id { get; set; }

    public string icon { get; set; }

    public string model { get; set; }

    public string name { get; set; }

    public int isStory { get; set; }

    public string descs{ get; set; }

    public string icon_name{ get; set; }

}

/// <summary>
/// 该场景所有数据
/// </summary>
/// 
public class SceneDataJson
{
    public List<SceneItemServerJson> sceneDatas { get; set; }
}


public class SceneItemJson
{
    public string lightMapName { get; set; }
    public List<ItemInfoJson> items { get; set; }

    public SceneItemJson() { }

    public SceneItemJson(SceneItemInfo sceneItemInfo)
    {
        CopySceneItemInfo(sceneItemInfo);
    }

    public void CopySceneItemInfo(SceneItemInfo sceneItemInfo)
    {
        if (sceneItemInfo == null)
        {
            return;
        }
        items = new List<ItemInfoJson>();
        for (int i = 0; i < sceneItemInfo.items.Count; i++)
        {
            ItemInfo itemInfo = sceneItemInfo.items[i];
            ItemInfoJson infoJson = new ItemInfoJson();
            infoJson.itemPos = new List<ItemPosInfoJson>();
            infoJson.itemID = itemInfo.itemID;
            infoJson.itemName = itemInfo.itemName;
            for (int j = 0; j < itemInfo.itemPos.Count; j++)
            {
                ItemPosInfoJson posJson = new ItemPosInfoJson(itemInfo.itemPos[j]);
                infoJson.itemPos.Add(posJson);
            }
            items.Add(infoJson);
        }
    }
}

public class ItemInfoJson
{
    public long itemID { get; set; }
    public string itemName { get; set; }
    public List<ItemPosInfoJson> itemPos { get; set; }
}

public class Vector3Json
{
    public float x { get; set; }
    public float y { get; set; }
    public float z { get; set; }
    public Vector3Json()
    { }

    public Vector3Json(float x,float y,float z)
    {
        this.x = x;
        this.y = y;
        this.z = z;
    }

    public Vector3Json(Vector3 v)
    {
        this.x = v.x;
        this.y = v.y;
        this.z = v.z;
    }
}

public class ItemPosInfoJson
{
    public Vector3Json pos { get; set; } //位置
    public int type { get; set; } //类型
    public Vector3Json rotate { get; set; } //旋转角度
    public Vector3Json scale { get; set; } //缩放
    public float percent { get; set; } //百分比

    public string cameraNode { get; set; }
    public int lightIndex { get; set; }
    public float tilingX { get; set; }
    public float tilingY { get; set; }
    public float offsetX { get; set; }
    public float offsetY { get; set; }
    public ItemPosInfoJson()
    { }

    public ItemPosInfoJson(ItemPosInfo posInfo)
    {
        pos = new Vector3Json(posInfo.pos.x, posInfo.pos.y, posInfo.pos.z);
        type = (int)posInfo.type;
        rotate = new Vector3Json(posInfo.rotate.x, posInfo.rotate.y, posInfo.rotate.z);
        scale = new Vector3Json(posInfo.scale);
        percent = posInfo.percent;

        lightIndex = posInfo.lightIndex;
        tilingX = posInfo.tilingX;
        tilingY = posInfo.tilingY;
        offsetX = posInfo.offsetX;
        offsetY = posInfo.offsetY;
    }
}

#region 场景数据
public class SceneItemServerJson
{
    public long groupId { get; set; }
    public List<ItemInfoServerJson> items { get; set; }

    public SceneItemServerJson() { }

    public SceneItemServerJson(SceneItemInfo sceneItemInfo)
    {
        CopySceneItemInfo(sceneItemInfo);
    }

    public void CopySceneItemInfo(SceneItemInfo sceneItemInfo)
    {
        if (sceneItemInfo == null)
        {
            return;
        }
        items = new List<ItemInfoServerJson>();
        for (int i = 0; i < sceneItemInfo.items.Count; i++)
        {
            ItemInfo itemInfo = sceneItemInfo.items[i];
            ItemInfoServerJson infoJson = new ItemInfoServerJson();
            infoJson.itemPos = new List<ItemPosInfoServerJson>();
            infoJson.itemID = itemInfo.itemID;
            infoJson.itemName = itemInfo.itemName;
            for (int j = 0; j < itemInfo.itemPos.Count; j++)
            {
                ItemPosInfoServerJson posJson = new ItemPosInfoServerJson(itemInfo.itemPos[j]);
                infoJson.itemPos.Add(posJson);
            }
            items.Add(infoJson);
        }
    }
}

public class ItemInfoServerJson
{
    public long itemID { get; set; }
    public string itemName { get; set; }
    public List<ItemPosInfoServerJson> itemPos { get; set; }
}


public class ItemPosInfoServerJson
{
    public Vector3Json pos { get; set; } //位置
    public int type { get; set; } //类型
    //public Vector3Json rotate { get; set; } //旋转角度
    public float percent { get; set; } //百分比

    public ItemPosInfoServerJson()
    { }

    public ItemPosInfoServerJson(ItemPosInfo posInfo)
    {
        pos = new Vector3Json(posInfo.pos.x, posInfo.pos.y, posInfo.pos.z);
        type = (int)posInfo.type;
        //rotate = new Vector3Json(posInfo.rotate.x, posInfo.rotate.y, posInfo.rotate.z);
        percent = posInfo.percent;

    }
}
#endregion

/// <summary>
/// 场景基础表
/// </summary>
public class SceneConfig
{
    public int id { get; set; }
    public string thumbnail { get; set; }
    public string name { get; set; }
    public int difficulty { get; set; }
    public int outputMoney { get; set; }
    public int outputExp { get; set; }
    public int outputDiamond { get; set; }
    public int outputVit { get; set; }
    public int dropId { get; set; }
    public string sceneInfo { get; set; }
    public int dataGroupInfo { get; set; }
    public int mode { get; set; }
    public int objSum { get; set; }
}

