using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Google.Protobuf.Collections;

namespace SeekerGame
{

    public enum BagTypeEnum
    {
        Total,
        Recently,
        Energy,
        Prop,
        Pieces,
        GiftBox,
        Else,
        None
    }

    public enum PropType
    {
        EnergyProp,
        FunctionProp,
        PiecesProp,
        GiftBoxProp,
        NormalProp,
        //OtherProp
    }

    public class PropDataNetWork
    {
        public long id;
        public int sum;
    }
    /// <summary>
    /// 模拟数据
    /// </summary>
    public class PropNetworkData
    {
        public List<PropDataNetWork> pds;
    }

    /// <summary>
    /// 一格背包的数据
    /// </summary>
    public class PropData
    {
        public ConfProp prop;
        public int num;

        public PropData(ConfProp prop, int num)
        {
            this.prop = prop;
            this.num = num;
        }
    }

    public class BagHelper
    {

        public static PropNetworkData getTestData()
        {
            PropNetworkData propData = new PropNetworkData();
            propData.pds = new List<PropDataNetWork>();
            for (int i = 0; i < 33; i++)
            {
                PropDataNetWork pd = new PropDataNetWork();
                pd.id = 1000 + i;
                pd.sum = 48;
                propData.pds.Add(pd);
            }
            return propData;
        }

        /// <summary>
        /// 获取指定类型的道具
        /// </summary>
        /// <param name="propType"></param>
        /// <param name="propData"></param>
        /// <returns></returns>
        public static List<PropData> getPropForType(PropType propType, List<PropData> allPropData)
        {
            List<PropData> propList = new List<PropData>();
            for (int i = 0; i < allPropData.Count; i++)
            {
                if (allPropData[i].prop.type == (int)propType)
                {
                    propList.Add(allPropData[i]);
                }
            }
            return propList;
        }

        public static List<PropData> getPropData(PropNetworkData propData)
        {
            if (propData == null)
            {
                return new List<PropData>();
            }
            List<PropData> propList = new List<PropData>();
            for (int i = 0; i < propData.pds.Count; i++)
            {
                PropDataNetWork pd = propData.pds[i];
                ConfProp prop = ConfProp.Get(pd.id);

                if (prop.heapSize > 0 && prop.heapSize < pd.sum)
                {
                    int cellNum = pd.sum / prop.heapSize;
                    int finalPropNum = pd.sum % prop.heapSize;
                    for (int j = 0; j < cellNum; j++)
                    {
                        PropData realPropData = new PropData(prop, prop.heapSize);
                        propList.Add(realPropData);
                    }
                    if (finalPropNum != 0)
                    {
                        PropData realPropData = new PropData(prop, finalPropNum);
                        propList.Add(realPropData);
                    }
                }
                else
                {
                    PropData realPropData = new PropData(prop, pd.sum);
                    propList.Add(realPropData);
                }
            }
            return propList;
        }

        public static List<PropData> getPropData(RepeatedField<PlayerPropMsg> propMsg)
        {

            if (propMsg == null)
            {
                return new List<PropData>();
            }
            List<PropData> propList = new List<PropData>();
            for (int i = 0; i < propMsg.Count; i++)
            {
                PlayerPropMsg pd = propMsg[i];
                ConfProp prop = ConfProp.Get(pd.PropId);
                if (prop == null)
                {
                    continue;
                }
                if (prop.heapSize > 0 && prop.heapSize < pd.Count)
                {
                    int cellNum = pd.Count / prop.heapSize;
                    int finalPropNum = pd.Count % prop.heapSize;
                    for (int j = 0; j < cellNum; j++)
                    {
                        PropData realPropData = new PropData(prop, prop.heapSize);
                        propList.Add(realPropData);
                    }
                    if (finalPropNum != 0)
                    {
                        PropData realPropData = new PropData(prop, finalPropNum);
                        propList.Add(realPropData);
                    }
                }
                else
                {
                    PropData realPropData = new PropData(prop, pd.Count);
                    propList.Add(realPropData);
                }
            }
            return propList;
        }

        public static List<PropData> getExhibitData(Dictionary<long, PlayerPropMsg> propMsg)
        {

            if (propMsg == null || 0 == propMsg.Count)
            {
                return new List<PropData>();
            }

            List<PlayerPropMsg> datas = new List<PlayerPropMsg>(propMsg.Values);

            List<PropData> propList = new List<PropData>();
            for (int i = 0; i < datas.Count; i++)
            {
                PlayerPropMsg pd = datas[i];
                ConfProp prop = ConfProp.Get(pd.PropId);
                if (prop == null)
                {
                    continue;
                }

                if ((int)PROP_TYPE.E_EXHABIT != prop.type) 
                    continue;

                if (prop.heapSize > 0 && prop.heapSize < pd.Count)
                {
                    int cellNum = pd.Count / prop.heapSize;
                    int finalPropNum = pd.Count % prop.heapSize;
                    for (int j = 0; j < cellNum; j++)
                    {
                        PropData realPropData = new PropData(prop, prop.heapSize);
                        propList.Add(realPropData);
                    }
                    if (finalPropNum != 0)
                    {
                        PropData realPropData = new PropData(prop, finalPropNum);
                        propList.Add(realPropData);
                    }
                }
                else
                {
                    PropData realPropData = new PropData(prop, pd.Count);
                    propList.Add(realPropData);
                }
            }
            return propList;
        }

        public static List<PropData> getPropData(Dictionary<long, PlayerPropMsg> propMsg)
        {

            if (propMsg == null || 0 == propMsg.Count)
            {
                return new List<PropData>();
            }

            List<PlayerPropMsg> datas = new List<PlayerPropMsg>(propMsg.Values);

            List<PropData> propList = new List<PropData>();
            for (int i = 0; i < datas.Count; i++)
            {
                PlayerPropMsg pd = datas[i];
                ConfProp prop = ConfProp.Get(pd.PropId);
                if (prop == null)
                {
                    continue;
                }

                if ((int)PROP_TYPE.E_EXHABIT == prop.type) //陈列物件除外
                    continue;

                if (prop.heapSize > 0 && prop.heapSize < pd.Count)
                {
                    int cellNum = pd.Count / prop.heapSize;
                    int finalPropNum = pd.Count % prop.heapSize;
                    for (int j = 0; j < cellNum; j++)
                    {
                        PropData realPropData = new PropData(prop, prop.heapSize);
                        propList.Add(realPropData);
                    }
                    if (finalPropNum != 0)
                    {
                        PropData realPropData = new PropData(prop, finalPropNum);
                        propList.Add(realPropData);
                    }
                }
                else
                {
                    PropData realPropData = new PropData(prop, pd.Count);
                    propList.Add(realPropData);
                }
            }
            return propList;
        }

        public static int getBagIndexForID(long id, List<PropData> pds)
        {
            for (int i = 0; i < pds.Count; i++)
            {
                if (pds[i].prop.id == id)
                {
                    return i;
                }
            }
            return -1;
        }
        /// <summary>
        /// 根据类型进行排序
        /// </summary>
        /// <returns></returns>
        public static List<PropData> sortPropForType(List<PropData> propData)
        {
            if (propData == null)
            {
                return new List<PropData>();
            }
            List<PropData> sortProp = new List<PropData>();
            Dictionary<PropType, List<PropData>> allTypeProp = new Dictionary<PropType, List<PropData>>();
            allTypeProp.Add(PropType.GiftBoxProp, new List<PropData>());
            allTypeProp.Add(PropType.EnergyProp, new List<PropData>());
            allTypeProp.Add(PropType.FunctionProp, new List<PropData>());
            allTypeProp.Add(PropType.PiecesProp, new List<PropData>());
            allTypeProp.Add(PropType.NormalProp, new List<PropData>());
            for (int i = 0; i < propData.Count; i++)
            {
                PropData pd = propData[i];
                PropType propType = (PropType)pd.prop.type;
                if (allTypeProp.ContainsKey(propType))
                {
                    allTypeProp[propType].Add(pd);
                }
                else
                {
                    allTypeProp[PropType.NormalProp].Add(pd);
                }
            }
            foreach (var kv in allTypeProp)
            {
                sortProp.AddRange(SortPropData(kv.Value));
            }
            return sortProp;
        }

        public static List<PropData> getPropForPage(BagTypeEnum bagType, List<PropData> propData)
        {
            List<PropData> allProp = new List<PropData>();
            switch (bagType)
            {
                case BagTypeEnum.Total:
                    allProp = sortPropForType(propData);
                    break;
                case BagTypeEnum.Recently:
                    allProp = sortPropForType(propData);
                    break;
                case BagTypeEnum.Energy:
                    allProp = getPropForType(PropType.EnergyProp, propData);
                    break;
                case BagTypeEnum.Prop:
                    allProp = getPropForType(PropType.FunctionProp, propData);
                    break;
                case BagTypeEnum.Pieces:
                    allProp = getPropForType(PropType.PiecesProp, propData);
                    break;
                case BagTypeEnum.GiftBox:
                    allProp = getPropForType(PropType.GiftBoxProp, propData);
                    break;
                case BagTypeEnum.Else:
                    allProp = getPropForType(PropType.NormalProp, propData);
                    break;
                default:
                    break;
            }
            return allProp;
        }

        private static List<PropData> SortPropData(List<PropData> propDataList)
        {
            propDataList.Sort((PropData a, PropData b) => { return a.prop.id > b.prop.id ? 1 : -1; });
            return propDataList;
        }
    }
}


