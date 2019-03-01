using EngineCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace SeekerGame
{
    public static class EventGameUIAssist
    {

        public static void BeginEventGame(long event_id_)
        {

            EventGameManager.Instance.Event_id = event_id_;
            EventGameManager.Instance.LastScore = 0;
            EventGameManager.Instance.Score = 0;
            EventGameEntryData gamedata = new EventGameEntryData();
            gamedata.M_event_id = event_id_;
            gamedata.M_normal_drops = new List<long>();
            gamedata.M_full_drops = new List<long>();

            FrameMgr.OpenUIParams open_data = new FrameMgr.OpenUIParams(UIDefine.UI_EVENT_INGAME_ENTRY);
            open_data.Param = gamedata;

            EngineCoreEvents.UIEvent.ShowUIEventWithParam.SafeInvoke(open_data);
        }

        public static List<long> GetPhaseIDs(long event_id_)
        {
            ConfEvent event_data = ConfEvent.Get(event_id_);

            return GetPhaseIDs(event_data);
        }

        public static List<long> GetPhaseIDs(ConfEvent event_data_)
        {
            List<long> ret = new List<long>();

            for (int i = 0; i < event_data_.phases.Length; i++)
            {
                ret.Add(event_data_.phases[i]);
            }

            return ret;
        }

        public static Dictionary<int, string> GetPhaseKeyWords(ConfEventPhase phase_data_)
        {

            Dictionary<int, string> ret = new Dictionary<int, string>();

            for (int i = 0; i < phase_data_.keyWords.Length; i++)
            {
                int key_word_id = phase_data_.keyWords[i];//int.Parse(key_str);
                string key_word = LocalizeModule.Instance.GetString(ConfKeyWords.Get(key_word_id).word);
                ret.Add(key_word_id, key_word);
            }

            return ret;
        }

        public static string GetPoliceIconByIndex(int idx_)
        {
            long officer_id;

            if (EventGameManager.Instance.GetOfficerIdByIndex(idx_, out officer_id))
                return ConfOfficer.Get(officer_id).icon;

            return null;
        }

        public static long GetPoliceIDByIndex(int idx_)
        {
            long officer_id;
            EventGameManager.Instance.GetOfficerIdByIndex(idx_, out officer_id);
            return officer_id;
        }

        public static List<long> GetPoliceKeyWordByOfficerID(long officer_id_)
        {
            List<long> ret = new List<long>();

            ConfOfficer confOfficer = ConfOfficer.Get(officer_id_);
            for (int i = 0; i < confOfficer.features.Length; i++)
            {
                ret.Add(confOfficer.features[i]);
            }
            return ret;
        }


        public static List<string> GetPoliceKeyWordStrByOfficerID(long officer_id_)
        {

            List<string> ret = new List<string>();
            ConfOfficer confOfficer = ConfOfficer.Get(officer_id_);
            for (int i = 0; i < confOfficer.features.Length; i++)
            {
                int word_id = confOfficer.features[i];
                string word_str = LocalizeModule.Instance.GetString(ConfKeyWords.Get(word_id).word);
                ret.Add(word_str);
            }

            return ret;
        }

        public static List<string> GetPoliceKeyWordIconsByOfficerID(long officer_id_)
        {

            List<string> ret = new List<string>();
            ConfOfficer confOfficer = ConfOfficer.Get(officer_id_);
            for (int i = 0; i < confOfficer.features.Length; i++)
            {
                int word_id = confOfficer.features[i];
                string word_str = LocalizeModule.Instance.GetString(ConfKeyWords.Get(word_id).icon);
                ret.Add(word_str);
            }
            return ret;
        }


        public static void GetFeedBackAndDialogue(long officer_id_, long phase_id_, int valuation_, out string feedback_, out string dialogue_)
        {
            officer_id_ = 0 == officer_id_ ? 101 : officer_id_;
            string temp_feedback = 0 == valuation_ ? ConfEventPhase.Get(phase_id_).normaFeedback : ConfEventPhase.Get(phase_id_).successFeedback;
            string temp_dialogue = 0 == valuation_ ? ConfEventPhase.Get(phase_id_).normalDialogue : ConfEventPhase.Get(phase_id_).successDialogue;

            string officer_name = LocalizeModule.Instance.GetString(ConfOfficer.Get(officer_id_).name);

            feedback_ = LocalizeModule.Instance.GetString(temp_feedback);

            feedback_ = ExchangeKeyWords(feedback_, officer_name);

            dialogue_ = LocalizeModule.Instance.GetString(temp_dialogue);

            dialogue_ = ExchangeKeyWords(dialogue_, officer_name);
        }






        //public static bool GoToNextPhase(EventGamePlayUIView view = null)
        //{

        //    long phase_id;
        //    if (EventGameManager.Instance.FetchCurPhaseID(out phase_id))
        //    {
        //        List<long> _params = new List<long>
        //                {
        //                    EventGameManager.Instance.Event_id,
        //                    phase_id,
        //                    (long)(EventGameManager.Instance.Score),
        //                };
        //        if (null == view)
        //        {
        //            FrameMgr.OpenUIParams open_data = new FrameMgr.OpenUIParams(UIDefine.UI_EVENT_INGAME_PLAY);
        //            open_data.Param = _params;

        //            EngineCoreEvents.UIEvent.ShowUIEventWithParam.SafeInvoke(open_data);
        //        }
        //        else
        //        {
        //            view.Reset(_params);
        //        }

        //        return true;
        //    }

        //    return false;
        //}


        public static void OnBtnGamePauseClick(GameObject btnGamePause)
        {
            EngineCoreEvents.AudioEvents.PlayAudio.SafeInvoke(Audio.AudioType.UISound, EngineCommonAudioKey.Button_Click_Common.ToString());
            EngineCoreEvents.UIEvent.ShowUIEventWithParam.SafeInvoke(
                new FrameMgr.OpenUIParams(UIDefine.UI_GAME_MAIN_SETTING)
                {
                    Param = new PauseData()
                    {
                        m_mode = ENUM_SEARCH_MODE.E_EVENTGAME,
                        m_id = 0,
                    }
                });
        }

#if OFFICER_SYS
        public static List<long> GetOfficerPlayerIdsByOfficerIDs(List<long> officer_ids_)
        {
            List<long> ret = new List<long>();

            foreach (var item in officer_ids_)
            {
                foreach (var info in GlobalInfo.MY_PLAYER_INFO.Officer_infos)
                {
                    if (item == info.OfficerId)
                    {
                        ret.Add(info.PlayerOfficerId);
                    }
                }
            }

            return ret;
        }
#endif

        private static string ExchangeKeyWords(string old_str_, string officer_name_)
        {
            if (old_str_.Contains("<policename>"))
            {
                old_str_ = old_str_.Replace("<policename>", officer_name_);
            }
            else if (old_str_.Contains("<player>"))
            {
                old_str_ = old_str_.Replace("<player>", GlobalInfo.MY_PLAYER_INFO.PlayerNickName);
            }

            return old_str_;
        }



    }
}
