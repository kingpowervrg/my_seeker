using EngineCore;
using GOEngine;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace SeekerGame
{
    public class AchievementManager : Singleton<AchievementManager>
    {
        public SCAchievementResponse Data
        {
            get;
            private set;
        } = null;



        public AchievementManager()
        {
            GameEvents.PlayerEvents.RequestRecentAhievement += Refresh;
            MessageHandler.RegisterMessageHandler(MessageDefine.SCAchievementResponse, OnResponse);
        }

        public void Init() { }


        void Refresh()
        {
            CSAchievementRequest req = new CSAchievementRequest();

            GameEvents.NetWorkEvents.SendAsyncMsg.SafeInvoke(req);
        }



        private void OnResponse(object obj)
        {
            if (obj is SCAchievementResponse)
            {
                SCAchievementResponse res = (SCAchievementResponse)obj;

                if (!MsgStatusCodeUtil.OnError(res.Status))
                {
                    Data = res;
                }
                else
                {
                    Data = null;
                }

            }
        }
    }
}
