using EngineCore;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SeekerGame
{
    public class CombineDataManager : Singleton<CombineDataManager>
    {
        public const int MAX_LVL = 10;

        Dictionary<long, int> m_dict = new Dictionary<long, int>();

        public class ByCombinePriority : IComparer<CombineTipsData>
        {

            public int Compare(CombineTipsData a, CombineTipsData b)
            {
                if (a.m_combine_priority < b.m_combine_priority)
                    return -1;
                else if (a.m_combine_priority == b.m_combine_priority)
                    return 0;
                else
                    return 1;
            }
        }

        /// <summary>
        /// 主界面，"获得材料提示"的材料id
        /// </summary>
        SortedSet<CombineTipsData> m_cache_prop_tips = new SortedSet<CombineTipsData>(new ByCombinePriority());


        public CombineDataManager()
        {
            GameEvents.UIEvents.UI_Bag_Event.Tell_OnPropIn += AddCombineProp;
            MessageHandler.RegisterMessageHandler(MessageDefine.SCCombineInfoResponse, OnScResponse);
            MessageHandler.RegisterMessageHandler(MessageDefine.SCCombineResponse, OnScResponse);
        }


        public void RegisterMessageHandler()
        {

        }

        void OnScResponse(object s)
        {

            if (s is SCCombineInfoResponse)
            {
                var rsp = s as SCCombineInfoResponse;

                m_dict.Clear();

                var combine_ids = from id in rsp.CombineId
                                  select id;
                var combine_ids_list = combine_ids.ToList();

                var cur_counts = from count in rsp.Count
                                 select count;
                var cur_counts_list = cur_counts.ToList();

                for (int i = 0; i < combine_ids_list.Count() && i < cur_counts_list.Count(); ++i)
                {
                    m_dict.Add(combine_ids_list[i], cur_counts_list[i]);
                }

            }
            else if (s is SCCombineResponse)
            {
                var rsp = s as SCCombineResponse;

                if (MsgStatusCodeUtil.OnError(rsp.Result))
                    return;

                if (false == rsp.Success)
                {
                    return;
                }

                var raw_req = EngineCoreEvents.SystemEvents.GetRspPairReq.SafeInvoke();
                CSCombineRequest req = raw_req as CSCombineRequest;

                if (0 != ConfCombineFormula.Get(req.CombineId).mixLimit)
                {
                    AddCurCount(req.CombineId);
                }

                var combine = ConfCombineFormula.Get(req.CombineId);
                GlobalInfo.MY_PLAYER_INFO.AddSingleBagInfo(combine.outputId, combine.outputCount);

                GameEvents.UIEvents.UI_GameEntry_Event.Listen_OnCombinePropCollected.SafeInvoke();

                //扣除合成消耗的材料
                GlobalInfo.MY_PLAYER_INFO.ReducePropForBag(combine.propId1);
                GlobalInfo.MY_PLAYER_INFO.ReducePropForBag(combine.propId2);
                GlobalInfo.MY_PLAYER_INFO.ReducePropForBag(combine.propId3);
                GlobalInfo.MY_PLAYER_INFO.ReducePropForBag(combine.propId4);
                GlobalInfo.MY_PLAYER_INFO.ReducePropForBag(combine.propId5);

                GlobalInfo.MY_PLAYER_INFO.ReducePropForBag(combine.specialPropId1, combine.special1Count);
                GlobalInfo.MY_PLAYER_INFO.ReducePropForBag(combine.specialPropId2, combine.special2Count);
                GlobalInfo.MY_PLAYER_INFO.ReducePropForBag(combine.specialPropId3, combine.special3Count);
                GlobalInfo.MY_PLAYER_INFO.ReducePropForBag(combine.specialPropId4, combine.special4Count);


                //if (ENUM_COMBINE_TYPE.POLICE == (ENUM_COMBINE_TYPE)combine.type)
                //{
                //    //出勤卡，进背包
                //}
                //else if (ENUM_COMBINE_TYPE.OTHER == (ENUM_COMBINE_TYPE)combine.type)
                //{
                //    //道具，进背包
                //    GlobalInfo.MY_PLAYER_INFO.AddSingleBagInfo(combine.outputId, combine.outputCount);
                //}
                //else if (ENUM_COMBINE_TYPE.COLLECTION == (ENUM_COMBINE_TYPE)combine.type)
                //{
                //    //物件，进储藏室

                //}

            }

        }

        public int GetCurCount(long combine_id_)
        {
            if (!m_dict.ContainsKey(combine_id_))
                return 0;

            return m_dict[combine_id_];
        }


        void AddCurCount(long combine_id_)
        {

            if (!m_dict.ContainsKey(combine_id_))
            {
                m_dict.Add(combine_id_, 1);
            }
            else
                m_dict[combine_id_] += 1;
        }

        public void Sync()
        {
            CSCombineInfoRequest req = new CSCombineInfoRequest();
            GameEvents.NetWorkEvents.SendAsyncMsg.SafeInvoke(req);
        }



        void AddCombineProp(long prop_id_)
        {
            CachePropTips(prop_id_);
        }

        bool CachePropTips(long prop_id_)
        {
            List<ConfCombineFormula> combines = ConfCombineFormula.array;

            combines.Sort((a, b) =>
            {
                if (a.serialNumber < b.serialNumber)
                    return -1;
                else if (a.serialNumber == b.serialNumber)
                    return 0;
                else
                    return 1;
            });

            foreach (ConfCombineFormula combine in combines)
            {
                if (null != GlobalInfo.MY_PLAYER_INFO.GetBagInfosByID(combine.outputId))
                    continue;

                if (combine.propId1 == prop_id_
                    || combine.propId2 == prop_id_
                    || combine.propId3 == prop_id_
                    || combine.propId4 == prop_id_
                    || combine.propId5 == prop_id_
                    || combine.propId6 == prop_id_
                    )
                {
                    CombineTipsData data = new CombineTipsData()
                    {
                        m_combine_id = combine.id,
                        m_combine_priority = combine.serialNumber,
                    };
                    m_cache_prop_tips.Add(data);

                    return true;
                }
            }

            return false;

        }

        public long FetchPropTips()
        {
            if (0 == m_cache_prop_tips.Count)
                return 0;

            var tip = m_cache_prop_tips.First();

            m_cache_prop_tips.Remove(tip);

            return tip.m_combine_id;
        }

    }

    public struct CombineTipsData : IEquatable<CombineTipsData>
    {
        public long m_combine_id;
        public int m_combine_priority;

        public override int GetHashCode()
        {
            return m_combine_id.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            return obj is CombineTipsData && Equals((CombineTipsData)obj);
        }

        public bool Equals(CombineTipsData p)
        {
            return m_combine_id == p.m_combine_id;
        }
    }

}
