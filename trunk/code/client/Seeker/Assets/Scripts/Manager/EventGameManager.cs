
using GOEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SeekerGame
{
    public class EventGameManager : Singleton<EventGameManager>
    {
        List<long> m_keep_officer_id_list;
        List<long> m_phases;

        private long m_event_id;
        public long Event_id
        {
            get { return m_event_id; }
            set { m_event_id = value; }
        }

        private int m_score;
        public int Score
        {
            get { return m_score; }
            set { m_score = value; }
        }

        public int LastScore { get; set; }

        private List<long> m_finished_phase_dispatched_officer_id = new List<long>();
        public List<long> Will_dispatched_officer_id
        {
            get { return m_finished_phase_dispatched_officer_id; }
        }

        public int CurPhaseKeyWordCount
        {
            get;
            set;
        }

        public long Most_valuable_officer_id;

        private int TotalPhaseCount
        {
            get;
            set;
        }


        public void Init()
        {
#if OFFICER_SYS
            m_keep_officer_id_list = new List<long>();
            foreach (var info in GlobalInfo.MY_PLAYER_INFO.Officer_infos)
            {
                m_keep_officer_id_list.Add(info.OfficerId);
            }
#endif

            m_phases = new List<long>();



            List<long> phase_id_list = EventGameUIAssist.GetPhaseIDs(Event_id);

            m_phases.AddRange(phase_id_list);

            TotalPhaseCount = m_phases.Count;

            Score = 0;

            Will_dispatched_officer_id.Clear();
        }

        public int GetTotalPhasesCount()
        {
            return TotalPhaseCount;
        }

        public int GetCurPhaseNum()
        {
            return TotalPhaseCount - m_phases.Count + 1;
        }

        public void RemoveDispatchedPolice()
        {
            if (null == Will_dispatched_officer_id || Will_dispatched_officer_id.Count == 0)
                return;
            m_keep_officer_id_list.RemoveAll((item) => Will_dispatched_officer_id.Contains(item));

            Will_dispatched_officer_id.Clear();
        }

        public List<long> GetRetainOfficers()
        {
            return m_keep_officer_id_list;
        }

        public bool GetOfficerIdByIndex(int i, out long id_)
        {
            id_ = -1;
            if (i >= m_keep_officer_id_list.Count)
                return false;

            id_ = m_keep_officer_id_list[i];
            return true;
        }



        public bool FetchCurPhaseID(out long id_, bool force_del_ = false)
        {
            id_ = -1;

            if (0 == m_phases.Count)
                return false;

            id_ = m_phases[0];

            if (force_del_)
            {
                m_phases.RemoveAt(0);
            }

            return true;
        }

        public bool PhaseFinished(out long phase_id_)
        {
            this.RemoveDispatchedPolice();

            return EventGameManager.Instance.FetchCurPhaseID(out phase_id_, true);
        }


    }
}
