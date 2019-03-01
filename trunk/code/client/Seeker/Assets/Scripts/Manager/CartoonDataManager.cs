using EngineCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SeekerGame
{
    public class CartoonDataManager : Singleton<CartoonDataManager>
    {

        Dictionary<long, CartoonItemJson> m_dict = new Dictionary<long, CartoonItemJson>();

        public void InitCartoonData()
        {
            if (m_dict.Count > 0)
            {
                return;
            }
            EngineCoreEvents.ResourceEvent.GetAssetEvent.SafeInvoke("Cartoon.json", (assetName, assetObject) =>
            {
                CartoonData data = Utf8Json.JsonSerializer.Deserialize<CartoonData>(assetObject.ToString());

                foreach (var cartoon in data.M_cartoons)

                { CartoonDataManager.Instance.AddData(cartoon); };

                EngineCoreEvents.ResourceEvent.ReleaseAssetEvent.SafeInvoke(assetName, assetObject);

            }, LoadPriority.PostLoad);
        }
        public void AddData(CartoonItemJson data_)
        {

            if (!this.m_dict.ContainsKey(data_.Item_id))
            {
                m_dict.Add(data_.Item_id, data_);
            }
            else
            {
                this.m_dict[data_.Item_id] = data_;
            }

        }

        public CartoonItemJson GetData(long id_)
        {
            CartoonItemJson ret;
            if (this.m_dict.TryGetValue(id_, out ret))
            {
                return ret;
            }

            return null;
        }

        public void RemoveData(int index_)
        {

            if (this.m_dict.ContainsKey(index_))
            {
                this.m_dict.Remove(index_);
            }
        }
    }
}
