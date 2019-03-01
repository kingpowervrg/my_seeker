using UnityEngine;
using System.Collections.Generic;
using EngineCore;

namespace SeekerGame
{
    public class LocalDataManager : Singleton<LocalDataManager>
    {
        public const string Notic = "_notic";
        public const string LastNoticLevel = "_NoticLevel";

        public bool m_HasNotic = false;

        private long m_curNoticTime = 0;

        private bool m_is_login;
        public bool Is_login
        {
            get { return m_is_login; }
            set { m_is_login = value; }
        }
        public long CurNoticTime
        {
            get { return m_curNoticTime; }
        }


        public LocalDataManager()
        {
            MessageHandler.RegisterMessageHandler(MessageDefine.SCNoticeListResponse, OnScResponse);
        }

        private void OnScResponse(object s)
        {
            if (s is SCNoticeListResponse && Is_login)
            {
                SCNoticeListResponse res = (SCNoticeListResponse)s;
                if (res != null && res.Notices != null)
                {
                    LocalDataManager.Instance.HasNewNotic(res.Notices);
                }


            }
        }

        public string GetPlayerPrefs(string type)
        {
            string key = GlobalInfo.MY_PLAYER_ID + type;
            return PlayerPrefs.GetString(key);
        }

        public void SetPlayerPrefs(string type, string value)
        {
            string key = GlobalInfo.MY_PLAYER_ID + type;
            PlayerPrefs.SetString(key, value);
        }

        #region 新消息通知
        public bool HasNewNotic(IList<NoticeInfo> noticInfo)
        {
            if (noticInfo == null || GlobalInfo.MY_PLAYER_INFO == null)
            {
                return false;
            }
            string lastLevelStr = GetPlayerPrefs(LocalDataManager.LastNoticLevel);
            int LastLevel;
            int.TryParse(lastLevelStr, out LastLevel);


            bool HasLock = false;
            //long times = 0;
            for (int i = 0; i < noticInfo.Count; i++)
            {
                if (m_curNoticTime <= noticInfo[i].UpdateTime && GlobalInfo.MY_PLAYER_INFO.Level >= noticInfo[i].LevelLimit)
                {
                    m_curNoticTime = noticInfo[i].UpdateTime;
                }
                if (GlobalInfo.MY_PLAYER_INFO.Level >= noticInfo[i].LevelLimit && LastLevel < noticInfo[i].LevelLimit)
                {
                    HasLock = true;
                }
            }
            m_HasNotic = HasNewNotic(m_curNoticTime) | HasLock;
            return m_HasNotic;
        }

        public bool HasNewNotic(long time)
        {
            string valueStr = GetPlayerPrefs(LocalDataManager.Notic);
            long valueTime = 0;
            long.TryParse(valueStr, out valueTime);
            if (valueTime != time)
            {
                return true;
            }
            return false;
        }

        public void ReflashNotic()
        {
            m_HasNotic = false;
            if (m_curNoticTime > 0)
            {
                GameEvents.UIEvents.UI_GameEntry_Event.OnCloseNoticRedDot.SafeInvoke();
                SetPlayerPrefs(LocalDataManager.Notic, m_curNoticTime.ToString());
                SetPlayerPrefs(LocalDataManager.LastNoticLevel, GlobalInfo.MY_PLAYER_INFO.Level.ToString());
            }
        }
        #endregion

    }
}
