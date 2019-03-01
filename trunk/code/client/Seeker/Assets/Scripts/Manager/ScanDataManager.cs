using EngineCore;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace SeekerGame
{
    public class ScanDataManager : Singleton<ScanDataManager>
    {

        Dictionary<long, ScanJsonData> m_dict = new Dictionary<long, ScanJsonData>();

        public void GetScanData(long scan_id_, Action<ScanJsonData> onLoad_)
        {
            if (m_dict.ContainsKey(scan_id_))
            {
                onLoad_(m_dict[scan_id_]);
                return;
            }

            EngineCoreEvents.ResourceEvent.GetAssetEvent.SafeInvoke($"Scan{scan_id_}.json", (assetName, assetObject) =>
            {
                ScanJsonData data = Utf8Json.JsonSerializer.Deserialize<ScanJsonData>(assetObject.ToString());

                m_dict.Add(scan_id_, data);
                onLoad_(m_dict[scan_id_]);

                EngineCoreEvents.ResourceEvent.ReleaseAssetEvent.SafeInvoke(assetName, assetObject);

            }, LoadPriority.HighPrior);
        }


        Dictionary<long, Dictionary<int, HashSet<long>>> m_all_examin_clue_datas = new Dictionary<long, Dictionary<int, HashSet<long>>>();
        public System.Collections.Generic.Dictionary<int, System.Collections.Generic.HashSet<long>> Examin_clue_datas(long scan_id_)
        {
            if (!m_all_examin_clue_datas.ContainsKey(scan_id_))
            {
                LoadExamineClueData(scan_id_);
            }
            return m_all_examin_clue_datas[scan_id_];
        }


        void LoadExamineClueData(long scan_id_)
        {
            ConfFind scan_data = ConfFind.Get(scan_id_);

            Dictionary<int, HashSet<long>> examin_clue_datas = new Dictionary<int, HashSet<long>>();

            if (0 != scan_data.findtype1)
            {
                if (examin_clue_datas.ContainsKey(scan_data.findtype1))
                {
                    Debug.LogError("重复的尸检线索种类");
                    return;
                }
                examin_clue_datas.Add(scan_data.findtype1, new HashSet<long>(scan_data.finds1));
            }

            if (0 != scan_data.findtype2)
            {
                if (examin_clue_datas.ContainsKey(scan_data.findtype2))
                {
                    Debug.LogError("重复的尸检线索种类");
                    return;
                }
                examin_clue_datas.Add(scan_data.findtype2, new HashSet<long>(scan_data.finds2));
            }

            if (0 != scan_data.findtype3)
            {
                if (examin_clue_datas.ContainsKey(scan_data.findtype3))
                {
                    Debug.LogError("重复的尸检线索种类");
                    return;
                }
                examin_clue_datas.Add(scan_data.findtype3, new HashSet<long>(scan_data.finds3));
            }

            m_all_examin_clue_datas.Add(scan_id_, examin_clue_datas);
        }



    }
}
