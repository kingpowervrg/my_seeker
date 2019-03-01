using System;
using UnityEngine;

namespace SeekerGame
{
    public class VitManager : Singleton<VitManager>
    {
        private long m_LastAddVitTime = 0;
        private DateTime m_LastVitDateTime;
        private float m_TotalSecond = 0;
        private bool m_NeedTick = false;

        private float m_InfiniteVitTime = -1; //无限体力剩余时间  时间为秒
        private float m_lastInfiniteVitTime = -1;

        public bool IsInfiniteVit()
        {
            return m_InfiniteVitTime > 0;
        }



        public void ReflashInfiniteVitTime(float time)
        {
            this.m_InfiniteVitTime = time / 1000;
            this.m_lastInfiniteVitTime = this.m_InfiniteVitTime;
            GameEvents.UIEvents.UI_GameEntry_Event.OnInfiniteVit.SafeInvoke(m_InfiniteVitTime);
            Debug.Log("m_InfiniteVitTime : " + m_InfiniteVitTime);
        }

        public void SetLastAddVitTime(long lastAddVitTime)
        {
            this.m_LastAddVitTime = lastAddVitTime % CommonData.MillisRecoverOneVit;
            m_LastVitDateTime = DateTime.Now.AddSeconds(-this.m_LastAddVitTime);
            //m_LastVitDateTime = CommonTools.TimeStampToDateTime(lastAddVitTime * 10000);
            SetLastVitDT(m_LastVitDateTime);
            //SetLastVitDT(DateTime.Now.AddSeconds(-120));
        }

        private void SetLastVitDT(DateTime dt)
        {
            m_LastVitDateTime = dt;
            double second = (DateTime.Now - m_LastVitDateTime).TotalSeconds;
            float vitSecond = CommonData.MillisRecoverOneVit;
            m_TotalSecond = vitSecond - (float)second;
        }
        //public void Set

        public void Tick()
        {
            if (m_InfiniteVitTime > 0)
            {
                m_InfiniteVitTime -= Time.deltaTime;
                if (this.m_InfiniteVitTime < 1f)
                {
                    GameEvents.UIEvents.UI_GameEntry_Event.OnInfiniteVit.SafeInvoke(m_InfiniteVitTime);
                    m_lastInfiniteVitTime = 0f;
                }
                if (m_lastInfiniteVitTime - m_InfiniteVitTime >= 1f)
                {
                    GameEvents.UIEvents.UI_GameEntry_Event.OnInfiniteVit.SafeInvoke(m_InfiniteVitTime);
                    m_lastInfiniteVitTime = m_InfiniteVitTime;
                }
            }

            //|| GlobalInfo.MY_PLAYER_INFO.Vit >= CommonData.MAXVIT
            if (m_TotalSecond <= 0 || GlobalInfo.MY_PLAYER_INFO.Vit >= CommonData.MAXVIT)
            {
                GameEvents.UIEvents.UI_GameEntry_Event.OnCountDownVit.SafeInvoke(0);
                return;
            }
            m_TotalSecond -= Time.deltaTime;
            if (m_TotalSecond <= 0)
            {
                SetLastVitDT(DateTime.Now);
                GlobalInfo.MY_PLAYER_INFO.ChangeVit(1);
            }
            GameEvents.UIEvents.UI_GameEntry_Event.OnCountDownVit.SafeInvoke(m_TotalSecond);
        }
    }
}
