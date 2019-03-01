using System;
using System.Collections.Generic;
using EngineCore;
namespace SeekerGame
{
    public class ClientConfigManager : Singleton<ClientConfigManager>
    {
        private Dictionary<int, PersuadeData> m_persudeDic = new Dictionary<int, PersuadeData>();
        public void Load()
        {
            if (m_persudeDic.Count == 0)
            {
                EngineCoreEvents.ResourceEvent.GetAssetEvent.SafeInvoke("persuade.json", (assetName, assetObject) =>
                {
                    PersuadeGroupData m_persudeGroup = Utf8Json.JsonSerializer.Deserialize<PersuadeGroupData>(assetObject.ToString());
                    EngineCoreEvents.ResourceEvent.ReleaseAssetEvent.SafeInvoke(assetName, assetObject);
                    for (int i = 0; i < m_persudeGroup.persuadeGroup.Count; i++)
                    {
                        m_persudeDic.Add(m_persudeGroup.persuadeGroup[i].id, m_persudeGroup.persuadeGroup[i]);
                    }
                }, LoadPriority.PostLoad);
            }
        }

        public PersuadeData GetPersuadeByID(int id)
        {
            return m_persudeDic[id];
        }
    }
}
