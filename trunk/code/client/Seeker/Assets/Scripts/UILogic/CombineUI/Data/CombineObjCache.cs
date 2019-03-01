using EngineCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace SeekerGame
{



    public class CombinedObjCache
    {



        Dictionary<string, Asset3DObj> m_cache = new Dictionary<string, Asset3DObj>();

        public void AddToCache(string obj_name_, string asset_name_, GameObject obj_)
        {
            if (m_cache.ContainsKey(obj_name_))
            {
                return;
            }


            m_cache.Add(obj_name_, new Asset3DObj(asset_name_, obj_));
        }


        public GameObject GetFromCache(string obj_name_)
        {
            return m_cache.ContainsKey(obj_name_) ? m_cache[obj_name_].m_asset : null;
        }


        public IEnumerable<Asset3DObj> AllCachedObjs()
        {
            return m_cache.Values;
        }

        public void Clear()
        {
            m_cache.Clear();
        }

    }

    public class Asset3DObj
    {
        public Asset3DObj(string asset_name_, GameObject asset_obj_)
        {
            m_asset_name = asset_name_;
            m_asset = asset_obj_;
        }

        public string m_asset_name;
        public GameObject m_asset;
    }
}

