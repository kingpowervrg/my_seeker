using EngineCore;
using GOEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SeekerGame
{
    public class PlayerInfoManager : Singleton<PlayerInfoManager>
    {
        Dictionary<long, PlayerInfo> m_player_infos_dict = new Dictionary<long, PlayerInfo>();

        public static Action<PlayerInfo> OnPlayerInfoUpdatedEvent;

#if OFFICER_SYS
        public int LimitNum
        {
            get;
            set;
        }
#endif



        public void Sync() { }

        public PlayerInfoManager()
        {
            MessageHandler.RegisterMessageHandler(MessageDefine.SCPlayerUpLevel, OnLevelUp);
            MessageHandler.RegisterMessageHandler(MessageDefine.SCPlayerInfoResponse, OnPlayerInfoUpdated);
#if OFFICER_SYS
            MessageHandler.RegisterMessageHandler(MessageDefine.SCOfficerListResponse, OnInitPlayerOfficerListReponse);
#endif
            MessageHandler.RegisterMessageHandler(MessageDefine.SCreceiveNewTitle, OnGetTitle);
            MessageHandler.RegisterMessageHandler(MessageDefine.SCPlayerPropResponse, OnRefreshBag);
            MessageHandler.RegisterMessageHandler(MessageDefine.SCTitleGetResponse, OnRefreshTitle);
            MessageHandler.RegisterMessageHandler(MessageDefine.SCTitleActiveResponse, OnRefreshTitle);
            MessageHandler.RegisterMessageHandler(MessageDefine.SCTitleResponse, OnRefreshTitle);


#if OFFICER_SYS
            GameEvents.UIEvents.UI_Enter_Event.OnLimitPoliceNum += LimitPoliceNum;
#endif
            GameEvents.PlayerEvents.OnExpChanged += ShowLevelUp;
            GameEvents.UIEvents.UI_Bonus_Pop_View_Event.Listen_OnShow += OnShowBonusPopView;
            GameEvents.UIEvents.UI_Bag_Event.Listen_GetAllExhibitions += GetExhibitInBag;
        }

#if OFFICER_SYS
        private void LimitPoliceNum(int num_)
        {
            LimitNum = num_;
        }
#endif

        public void AddPlayerInfo(long player_id_, PlayerInfo info_)
        {
            if (!this.m_player_infos_dict.ContainsKey(player_id_))
                this.m_player_infos_dict.Add(player_id_, info_);
            else
            {
                this.m_player_infos_dict[player_id_] = info_;
            }
            GameEvents.PlayerEvents.RequestLatestPlayerInfo = SyncLatestPlayerInfo;
            GameEvents.PlayerEvents.Listen_SyncTitle = SyncLastestTitleInfo;

        }

        public PlayerInfo GetPlayerInfo(long player_id_)
        {
            PlayerInfo ret;
            if (this.m_player_infos_dict.TryGetValue(player_id_, out ret))
            {
                return ret;
            }

            return null;
        }

        public void ClearPlayerInfo()
        {
            this.m_player_infos_dict.Clear();
        }

        private void OnPlayerInfoUpdated(object msgResponse)
        {
            SCPlayerInfoResponse msg = msgResponse as SCPlayerInfoResponse;
            PlayerInfo playerInfo = GetPlayerInfo(msg.PlayerId);
            if (playerInfo != null)
            {
                playerInfo.SetCash(msg.Cash).SetCoin(msg.Coin).SetExp(msg.Exp).SetExpMultiple(msg.ExpMultiple)
                 .SetIcon(msg.PlayerIcon).SetLaborUnionn(msg.LaborUnion)
                 .SetLevel(msg.Level).SetUpgradeExp(msg.UpgradeExp).SetVit(msg.Vit);

                playerInfo.PlayerNickName = msg.PlayerName;
                playerInfo.HasRenamed = msg.HasRenamed > 1;

                CommonData.MillisRecoverOneVit = (msg.MillisRecoverOneVit / 1000) + 1;
                VitManager.Instance.SetLastAddVitTime(msg.LastAddVitTime);
                VitManager.Instance.ReflashInfiniteVitTime(msg.InfiniteVitRestTime);
                OnPlayerInfoUpdatedEvent?.Invoke(playerInfo);
            }
            else
                Debug.Log("player :" + msg.PlayerId + " is null");
        }

        /// <summary>
        /// 请求同步玩家最新的个人信息
        /// </summary>
        private void SyncLatestPlayerInfo()
        {
            CSPlayerInfoRequest updatePlayerInfo = new CSPlayerInfoRequest();
            updatePlayerInfo.PlayerId = GlobalInfo.MY_PLAYER_ID;
            GameEvents.NetWorkEvents.SendAsyncMsg.SafeInvoke(updatePlayerInfo);




        }

        private void SyncLastestTitleInfo()
        {
            CSTitleRequest req = new CSTitleRequest();
            GameEvents.NetWorkEvents.SendAsyncMsg.SafeInvoke(req);

            CSTitleGetRequest reqTitle = new CSTitleGetRequest();
            GameEvents.NetWorkEvents.SendAsyncMsg.SafeInvoke(reqTitle);
        }

#if OFFICER_SYS
        private void OnInitPlayerOfficerListReponse(object msg)
        {
            SCOfficerListResponse msgResp = msg as SCOfficerListResponse;
            PlayerInfo p_info = GetPlayerInfo(GlobalInfo.MY_PLAYER_ID);
            p_info.SetOfficerInfos(msgResp.Officers);
            DebugUtil.Log("警员信息下载完成");
        }
#endif
        private void OnGetTitle(object msg)
        {
            SCreceiveNewTitle rsp = msg as SCreceiveNewTitle;
            Dictionary<UBSParamKeyName, object> _params = new Dictionary<UBSParamKeyName, object>()
                    {
                        { UBSParamKeyName.ContentID, rsp.Title},
                    };

            UserBehaviorStatisticsModules.Instance.LogEvent(UBSEventKeyName.title_get, null, _params);
        }


        private void OnRefreshBag(object msg)
        {
            if (msg is SCPlayerPropResponse)
            {
                SCPlayerPropResponse rsp = (SCPlayerPropResponse)msg;

                GlobalInfo.MY_PLAYER_INFO.SetBagInfos(rsp.PlayerProps);

                GlobalInfo.MY_PLAYER_INFO.SetRecentPropInfos(rsp.RecentProps);
            }
        }

        private void OnRefreshTitle(object msg)
        {

            if (msg is SCTitleGetResponse)
            {
                SCTitleGetResponse res = (SCTitleGetResponse)msg;
                if (res.Status == null)
                {
                    if (res.Title != null)
                    {
                        GlobalInfo.MY_PLAYER_INFO.TitleID = res.Title.TitleId;
                    }
                }
            }
            else if (msg is SCTitleActiveResponse)
            {
                SCTitleActiveResponse res = (SCTitleActiveResponse)msg;

                if (res.Status == null)
                {
                    GlobalInfo.MY_PLAYER_INFO.TitleID = res.Title.TitleId;

                }
            }
            else if (msg is SCTitleResponse)
            {
                if (0L == GlobalInfo.MY_PLAYER_INFO.TitleID)
                {
                    SCTitleResponse res = (SCTitleResponse)msg;
                    if (res.Status == null)
                    {
                        foreach (var title in res.Titles)
                        {
                            if (title.Active)
                            {
                                GlobalInfo.MY_PLAYER_INFO.TitleID = title.TitleId;
                                break;
                            }

                        }
                    }
                }
            }


        }

        private Queue<SCPlayerUpLevel> m_level_ups = new Queue<SCPlayerUpLevel>();

        private void OnLevelUp(object msg)
        {
            Debug.Log("奖励页面下发");
            SCPlayerUpLevel rsp = msg as SCPlayerUpLevel;
            m_level_ups.Enqueue(rsp);

            GameEvents.UIEvents.UI_Bonus_Pop_View_Event.Tell_OnCache.SafeInvoke(EUNM_BONUS_POP_VIEW_TYPE.E_LVL_UP);
            GameEvents.UIEvents.UI_Bonus_Pop_View_Event.Tell_OnShow.SafeInvoke();

            if (null != rsp.PushInfo)
            {
                List<Push_Info> infos = new List<Push_Info>();
                infos.Add(rsp.PushInfo);
                PushGiftManager.Instance.RefreshLvlPush(infos);
                //PushGiftManager.Instance.TurnOn(ENUM_PUSH_GIFT_BLOCK_TYPE.E_LVL);
                PushGiftManager.Instance.Cache(ENUM_PUSH_GIFT_BLOCK_TYPE.E_LVL);

                GameEvents.UIEvents.UI_Bonus_Pop_View_Event.Tell_OnCache.SafeInvoke(EUNM_BONUS_POP_VIEW_TYPE.E_PUSH_GIFT);
            }

        }


        private void StartWatchLevelUp()
        {
            Debug.Log("开始升级监测<<<<<<<<<<<<<<<<<<<<");
            TimeModule.Instance.SetTimeInterval(Watching, 0.5f);
            TimeModule.Instance.SetTimeout(StopWatchLevelUp, 3.0f);
        }

        private void StopWatchLevelUp()
        {
            Debug.Log("取消升级监测>>>>>>>>>>>>>>>>>>>>>");
            TimeModule.Instance.RemoveTimeaction(Watching);
        }


        private void Watching()
        {
            if (m_level_ups.Count > 0)
            {
                StopWatchLevelUp();

                //DoShowLevelUp(null);
                GameEvents.UIEvents.UI_Bonus_Pop_View_Event.Tell_OnCache.SafeInvoke(EUNM_BONUS_POP_VIEW_TYPE.E_LVL_UP);

            }
        }

        private void OnShowBonusPopView(EUNM_BONUS_POP_VIEW_TYPE t_)
        {
            if (EUNM_BONUS_POP_VIEW_TYPE.E_LVL_UP != t_)
                return;

            if (0 == m_level_ups.Count)
            {
                //弹出界面在onlevelup里缓存后，有可能被结算界面直接触发弹出。所以缓存里有垃圾数据
                //需要继续执行缓存中后面的数据，一面被垃圾数据打断。
                GameEvents.UIEvents.UI_Bonus_Pop_View_Event.Tell_OnShow.SafeInvoke();
                return;
            }


            DoShowLevelUp(null);
        }


        IEnumerable<PropData> GetExhibitInBag()
        {
            return BagHelper.getExhibitData(GlobalInfo.MY_PLAYER_INFO.Bag_infos);
        }


        private bool ShowLevelUp(SafeAction act_, int delta_exp_)
        {
            Debug.Log("查看升级");

            int nextLevelExp = Confetl.array.Find(conf => conf.level == GlobalInfo.MY_PLAYER_INFO.Level).exp;
            int currentDeltaExp = nextLevelExp - GlobalInfo.MY_PLAYER_INFO.UpgradeExp;

            Debug.Log(string.Format("查看升级 当前经验{0} 增加{1} 最终{2} 升级需要{3}", currentDeltaExp, delta_exp_, currentDeltaExp + delta_exp_, nextLevelExp));

            if (currentDeltaExp + delta_exp_ >= nextLevelExp)
            //if (Old_lvl < this.GetPlayerInfo(GlobalInfo.MY_PLAYER_ID).Level)
            {
                Debug.Log("升级确实提升");
                if (m_level_ups.Count > 0)
                {
                    DoShowLevelUp(act_);

                    return true;
                }
                else
                {
                    StartWatchLevelUp();
                }
            }

            return false;
        }

        private void DoShowLevelUp(SafeAction act_)
        {

            LevelUpData data = new LevelUpData()
            {
                msg = m_level_ups.Dequeue(),
                m_click_act = act_,
            };

            FrameMgr.OpenUIParams param = new FrameMgr.OpenUIParams(UIDefine.UI_LEVEL_UP);
            param.Param = data;

            EngineCoreEvents.UIEvent.ShowUIEventWithParam.SafeInvoke(param);
        }
    }
}
