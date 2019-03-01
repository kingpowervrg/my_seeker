#if OFFICER_SYS
using EngineCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SeekerGame
{
    public class PoliceDispatchManager : Singleton<PoliceDispatchManager>
    {

        private double m_total_time_s;

        /// <summary>
        /// 总耗时
        /// </summary>
        public double Total_time_s
        {
            get { return m_total_time_s; }
            set { m_total_time_s = value; }
        }
        private int m_total_vit_cost;

        /// <summary>
        /// 总耗体力
        /// </summary>
        public int Total_vit_cost
        {
            get { return m_total_vit_cost; }
            set { m_total_vit_cost = value; }
        }

        /// <summary>
        /// 栏位： 序号
        /// </summary>
        Dictionary<int, long> m_dict = new Dictionary<int, long>();


        public PoliceDispatchManager()
        {
            GameEvents.UIEvents.UI_Enter_Event.OnClearDispatchPoclie += Clear;
            GameEvents.Skill_Event.OnUnlimitedVitSkillStart += ClearVitCost;
        }

        public void Save()
        {
            PlayerPrefTool.SaveDispatchOfficer(GlobalInfo.MY_PLAYER_ID, m_dict);
        }

        public void Load()
        {

            m_dict = PlayerPrefTool.LoadDispatchOfficer(GlobalInfo.MY_PLAYER_ID);
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

        /// <summary>
        /// 所派警员的id
        /// </summary>
        /// <returns></returns>
        public List<long> GetAllDispathOfficersID()
        {
            //if (this.m_dict.Count == 0)
            //    Load();

            return this.m_dict.Values.ToList();
        }

        public Dictionary<int, long> GetAll()
        {
            return m_dict;
        }

        public void PreloadLastestDispatchOffers()
        {
            List<long> latestDispatchOffers = GetAllDispathOfficersID();
            for (int i = 0; i < latestDispatchOffers.Count; ++i)
            {
                string dispatchOfferImage = ConfOfficer.Get(latestDispatchOffers[i]).portrait;
                if (!string.IsNullOrEmpty(dispatchOfferImage))
                    ResourceModule.Instance.PreloadAsset(dispatchOfferImage, null);
            }
        }

        public void Clear()
        {
            m_dict.Clear();
        }

        private void ClearVitCost()
        {
            Total_vit_cost = 0;
        }

        public ENUM_SEARCH_MODE GetGameType()
        {
            return ENUM_SEARCH_MODE.E_SEARCH_ROOM;
        }
    }
}
#endif