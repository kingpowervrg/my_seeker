using EngineCore;
using System.Collections.Generic;

namespace SeekerGame
{
    ////签到入口
    //public enum SignInEntry
    //{
    //    Start,
    //    Activity
    //}

    public class SignInSystem
    {
        //private SignInEntry m_EntryType;
        public SCPlayerCheckInInfoResp m_data;
        private List<SignInItemTable> m_itemTable = new List<SignInItemTable>();

        public int GetItemCountByIndex(int i)
        {
            return m_itemTable[i].count;
        }

        //public SignInSystem(SignInEntry entryType)
        //{
        //    m_EntryType = entryType;
        //    RegisterSignInSystem();
        //}

        public SignInSystem()
        {
            //m_EntryType = SignInEntry.Activity;
            RegisterSignInSystem();

        }

        public void InitSignSystem(SCPlayerCheckInInfoResp res)
        {
            if (res == null)
            {
                GetSignInData();
            }
            else
            {
                m_data = res;
                GameEvents.UIEvents.UI_SignIn_Event.OnSignIn.SafeInvoke();
            }
        }

        private void RegisterSignInSystem()
        {
            MessageHandler.RegisterMessageHandler(MessageDefine.SCPlayerCheckInInfoResp, OnResponse);
            MessageHandler.RegisterMessageHandler(MessageDefine.SCPlayerCheckInResp, OnResponse);
        }

        private void UnRegisterSignInSystem()
        {
            MessageHandler.UnRegisterMessageHandler(MessageDefine.SCPlayerCheckInInfoResp, OnResponse);
            MessageHandler.UnRegisterMessageHandler(MessageDefine.SCPlayerCheckInResp, OnResponse);
        }

        public void GetSignInData()
        {
            CSPlayerCheckInInfoReq req = new CSPlayerCheckInInfoReq();

#if !NETWORK_SYNC || UNITY_EDITOR
            GameEvents.NetWorkEvents.SendAsyncMsg.SafeInvoke(req);
#else
            GameEvents.NetWorkEvents.SendMsg.SafeInvoke(req);
#endif


        }

        private void OnResponse(object obj)
        {
            if (obj is SCPlayerCheckInInfoResp)
            {
                SCPlayerCheckInInfoResp res = (SCPlayerCheckInInfoResp)obj;
                if (res == null)
                {
                    EngineCoreEvents.UIEvent.HideUIEvent.SafeInvoke(UIDefine.UI_SIGNIN);
                    return;
                }
                m_data = res;
                GameEvents.UIEvents.UI_SignIn_Event.OnSignIn.SafeInvoke();
            }
            else if (obj is SCPlayerCheckInResp)
            {
                SCPlayerCheckInResp res = (SCPlayerCheckInResp)obj;
                if (res.Result == 1)
                {
                    SignInItemTable curItem = GetCurrentReward();
                    if (curItem != null)
                    {
                        //放在礼物展示页面，统一加入背包
                        GlobalInfo.MY_PLAYER_INFO.AddSingleBagInfo(curItem.id, curItem.count);

                        GameEvents.UIEvents.UI_GameEntry_Event.Listen_OnCombinePropCollected.SafeInvoke();

                        SCDropResp dropRes = new SCDropResp();
                        DropInfo dropInfo = new DropInfo();
                        dropInfo.PropId = curItem.id;
                        dropInfo.Count = curItem.count;
                        dropRes.DropInfos.Add(dropInfo);
                        FrameMgr.OpenUIParams param = new FrameMgr.OpenUIParams(UIDefine.UI_GIFTRESULT);
                        param.Param = dropRes;
                        EngineCoreEvents.UIEvent.ShowUIEventWithParam.SafeInvoke(param);
                    }
                }
                EngineCoreEvents.UIEvent.HideUIEvent.SafeInvoke(UIDefine.UI_SIGNIN);
            }
        }

        public bool AddItemTable()
        {
            ConfCheckIn confCheckIn = ConfCheckIn.Get(m_data.Id);
            if (confCheckIn == null)
            {
                return false;
            }
            m_itemTable.Clear();
            m_itemTable.Add(new SignInItemTable(confCheckIn.reward1, confCheckIn.count1));
            m_itemTable.Add(new SignInItemTable(confCheckIn.reward2, confCheckIn.count2));
            m_itemTable.Add(new SignInItemTable(confCheckIn.reward3, confCheckIn.count3));
            m_itemTable.Add(new SignInItemTable(confCheckIn.reward4, confCheckIn.count4));
            m_itemTable.Add(new SignInItemTable(confCheckIn.reward5, confCheckIn.count5));
            m_itemTable.Add(new SignInItemTable(confCheckIn.reward6, confCheckIn.count6));
            m_itemTable.Add(new SignInItemTable(confCheckIn.reward7, confCheckIn.count7));
            return true;
        }

        public SignInItemTable GetCurrentReward()
        {
            ConfCheckIn checkIn = ConfCheckIn.Get(m_data.Id);
            if (checkIn == null || m_data == null)
            {
                return null;
            }
            return m_itemTable[m_data.Day - 1];
        }

        public string GetPropNameByIndex(int index)
        {
            ConfProp prop = ConfProp.Get(m_itemTable[index].id);
            if (prop != null)
            {
                return prop.icon;
            }
            return string.Empty;
        }

        public long GetPropIdByIndex(int index)
        {
            if (index < m_itemTable.Count)
            {
                return m_itemTable[index].id;
            }
            return -1;
        }

        public void OnDispose()
        {
            UnRegisterSignInSystem();
        }
    }

    public class SignInItemTable
    {
        public long id;
        public int count;

        public SignInItemTable(long id, int count)
        {
            this.id = id;
            this.count = count;
        }
    }
}
