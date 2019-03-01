#if OFFICER_SYS
using EngineCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SeekerGame
{
    public class EventGamePoliceDispatchManager : Singleton<EventGamePoliceDispatchManager>
    {

        /// <summary>
        /// 栏位： 序号
        /// </summary>
        Dictionary<int, long> m_dict = new Dictionary<int, long>();


        public EventGamePoliceDispatchManager()
        {

        }


        public bool AddDispatch(int index_, long police_id_)
        {
            foreach (var item in m_dict.Values)
            {
                if (item == police_id_)
                    return false;
            }

            if (!this.m_dict.ContainsKey(index_))
            {
                this.m_dict.Add(index_, police_id_);
            }
            else
            {
                if (this.m_dict[index_] == police_id_)
                    return false;

                this.m_dict[index_] = police_id_;
            }

            return true;
        }

        public long GetDispath(int index_)
        {
            long ret;
            if (this.m_dict.TryGetValue(index_, out ret))
            {
                return ret;
            }

            return -1;
        }

        public void RemoveDispath(int index_)
        {

            if (this.m_dict.ContainsKey(index_))
            {
                this.m_dict.Remove(index_);
            }
        }

        public void RemoveDispatch(long officer_id_)
        {
            foreach (var kvp in m_dict)
            {
                if (kvp.Value == officer_id_)
                {
                    m_dict.Remove(kvp.Key);
                    return;
                }
            }
        }

        /// <summary>
        /// 所派警员的id
        /// </summary>
        /// <returns></returns>
        public List<long> GetAllDispathOfficersID()
        {

            return this.m_dict.Values.ToList();
        }

        public Dictionary<int, long> GetAll()
        {
            return m_dict;
        }


        public void Clear()
        {
            m_dict.Clear();
        }

        public ENUM_SEARCH_MODE GetGameType()
        {
            return ENUM_SEARCH_MODE.E_EVENTGAME;
        }

    }
}
#endif