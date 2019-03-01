using System;
using System.Collections.Generic;
using EngineCore;
namespace SeekerGame.NewGuid
{
    public class GuidNewFuncAchievementComplete : GuidNewFunctionBase
    {
        //private long achievementID;
        public override void OnInit(long funcID, string[] param)
        {
            base.OnInit(funcID, param);
            //this.achievementID = int.Parse(param[0]);
        }

        public override void OnExecute()
        {
            base.OnExecute();
            MessageHandler.RegisterMessageHandler(MessageDefine.SCAchievementResponse, OnResponse);
            CSAchievementRequest req = new CSAchievementRequest();
            GameEvents.NetWorkEvents.SendMsg.SafeInvoke(req);

        }

        private void OnResponse(object obj)
        {
            MessageHandler.UnRegisterMessageHandler(MessageDefine.SCAchievementResponse, OnResponse);
            if (obj is SCAchievementResponse)
            {
                SCAchievementResponse res = (SCAchievementResponse)obj;
                for (int i = 0; i < res.Achievements.Count; i++)
                {
                    ConfAchievement confAchieve = ConfAchievement.Get(res.Achievements[i].Id);
                    if (res.Achievements[i].Progress >= confAchieve.progress1) //res.Achievements[i].Id == achievementID && 
                    {
                        OnDestory();
                        return;
                    }
                }
            }
            GameEvents.RedPointEvents.User_OnNewAchievementEvent += User_OnNewAchievementEvent;
        }

        private void User_OnNewAchievementEvent()
        {
            OnDestory();
        }

        public override void OnDestory(FuncState funcState = FuncState.Complete)
        {
            base.OnDestory(funcState);
            GameEvents.RedPointEvents.User_OnNewAchievementEvent -= User_OnNewAchievementEvent;
        }

        public override void ClearFunc()
        {
            base.ClearFunc();
            MessageHandler.UnRegisterMessageHandler(MessageDefine.SCAchievementResponse, OnResponse);
            GameEvents.RedPointEvents.User_OnNewAchievementEvent -= User_OnNewAchievementEvent;
        }
    }
}
