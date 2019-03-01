using EngineCore;
using System.Collections.Generic;

namespace SeekerGame
{
    public class JigsawDataManager : Singleton<JigsawDataManager>
    {

        Dictionary<int, JigsawGameData> m_dict = new Dictionary<int, JigsawGameData>();

        public void InitJigsawData()
        {
            if (m_dict.Count > 0)
            {
                return;
            }
            EngineCoreEvents.ResourceEvent.GetAssetEvent.SafeInvoke("Jigsaw.json", (assetName, assetObject) =>
            {
                JigsawData data = Utf8Json.JsonSerializer.Deserialize<JigsawData>(assetObject.ToString());

                foreach (JigsawDataJson json in data.M_jon_datas)
                    AddJigsaw(json.M_template_id, json);

                EngineCoreEvents.ResourceEvent.ReleaseAssetEvent.SafeInvoke("Jigsaw.json", assetObject);

            }, LoadPriority.PostLoad);
        }


        public void AddJigsaw(int index_, JigsawDataJson data_)
        {
            JigsawGameData gameData = new JigsawGameData
            {
                M_game_template_id = data_.M_template_id,
                M_game_dimention = data_.M_dimention,
                M_game_chips = new Dictionary<string, JigsawChipJson>(),
            };

            foreach (var item in data_.M_chips)
            {
                gameData.M_game_chips.Add(item.M_chip_name, item);
            }

            if (!this.m_dict.ContainsKey(index_))
            {
                m_dict.Add(index_, gameData);
            }
            else
            {
                this.m_dict[index_] = gameData;
            }

        }

        public JigsawGameData GetJigsaw(int index_)
        {
            JigsawGameData ret;
            if (this.m_dict.TryGetValue(index_, out ret))
            {
                return ret;
            }

            return null;
        }

        public void RemoveJigsaw(int index_)
        {

            if (this.m_dict.ContainsKey(index_))
            {
                this.m_dict.Remove(index_);
            }
        }
    }
}
