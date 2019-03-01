using EngineCore;
using GOEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace SeekerGame
{
    public class RedPointManager : Singleton<RedPointManager>
    {
        public void Sync()
        {

        }

        public RedPointManager()
        {
            MessageHandler.RegisterMessageHandler(MessageDefine.SCEmailChangeNotice, OnNewEmailNotify);

            MessageHandler.RegisterMessageHandler(MessageDefine.SCActivityNewResponse, OnNewActivityNotify);
            MessageHandler.RegisterMessageHandler(MessageDefine.SCAchievementUnlockResponse, OnNewAchievementNotify);
#if OFFICER_SYS
            MessageHandler.RegisterMessageHandler(MessageDefine.SCPlayerPropResponse, OnNewPoliceNotify);
#endif
            MessageHandler.RegisterMessageHandler(MessageDefine.SCFriendNoticeResponse, OnNewFriendNotify);
            MessageHandler.RegisterMessageHandler(MessageDefine.SCChapterRedNotice, OnNewChapterNotify);

            GameEvents.RedPointEvents.Sys_OnRefreshByPlayerPrefs += OnRefreshByPlayerPrefs;


            GameEvents.RedPointEvents.Sys_OnNewEmailReadedEvent += OnNewEmailReaded;
            GameEvents.RedPointEvents.Sys_OnNewFriendReadedEvent += OnNewFriendReaded;
            GameEvents.RedPointEvents.Sys_OnNewApplyReadedEvent += OnNewApplyReaded;
            GameEvents.RedPointEvents.Sys_OnNewGiftReadedEvent += OnNewGiftReaded;
            GameEvents.RedPointEvents.Sys_OnNewActivityReadedEvent += OnNewActivityReaded;
            GameEvents.RedPointEvents.Sys_OnNewAchievementReadedEvent += OnNewAchievementReaded;
            GameEvents.RedPointEvents.Sys_OnNewNoticeReadedEvent += OnNewNoticeReaded;

            GameEvents.RedPointEvents.Sys_OnNewChapterEvent += OnNewChapterReaded;
#if OFFICER_SYS
            GameEvents.System_Events.OnBagAddItems += OnNewPoliceNotify;
#endif

        }

        private void OnRefreshByPlayerPrefs()
        {
            if (PlayerPrefTool.GetIsThereNewEmail())
            {
                OnNewEmailNotify(null);
            }

            if (PlayerPrefTool.GetIsThereNewFriend() || PlayerPrefTool.GetIsThereNewApply() || PlayerPrefTool.GetIsThereNewGift())
            {
                OnNewFriendNotify(null);
            }


            if (PlayerPrefTool.GetIsThereNewActivity())
            {
                OnNewActivityNotify(null);
            }

            if (PlayerPrefTool.GetIsThereNewAchievement())
            {

                OnNewAchievementNotify(null);
            }

            if (PlayerPrefTool.GetIsThereNewNotice())
            {
                OnNewNoticeNotify(null);
            }
#if OFFICER_SYS
            if (PlayerPrefTool.GetIsThereNewPolice())
            {
                OnNewPoliceNotify();
            }
#endif

            if (PlayerPrefTool.GetIsThereNewChapterNPC())
            {
                OnNewChapterTypeNotify(0);
            }

            if (PlayerPrefTool.GetIsThereNewChapterClue())
            {
                OnNewChapterTypeNotify(1);
            }

            if (PlayerPrefTool.GetIsThereNewChapterScene())
            {
                OnNewChapterTypeNotify(2);
            }
        }
        //刷新档案红点数据
        public void ReflashArchivesRedPoint()
        {
            int redPointCount = 0;
            if (PlayerPrefTool.GetIsThereNewChapterNPC())
            {
                OnNewChapterTypeNotify(0);
                redPointCount++;
            }

            if (PlayerPrefTool.GetIsThereNewChapterClue())
            {
                OnNewChapterTypeNotify(1);
                redPointCount++;
            }

            if (PlayerPrefTool.GetIsThereNewChapterScene())
            {
                OnNewChapterTypeNotify(2);
                redPointCount++;
            }
            if (redPointCount == 0)
            {
                GameEvents.RedPointEvents.User_OnNewChapterBannerEvent.SafeInvoke(false); //
            }
        }


        private void OnNewEmailNotify(object msg)
        {
            Debug.LogWarning("新邮件通知！！！！！！！！");

            PlayerPrefTool.SetIsThereNewEmail(true);

            if (!GameEvents.RedPointEvents.User_OnNewEmailEvent.IsNull)
                GameEvents.RedPointEvents.User_OnNewEmailEvent.SafeInvoke();
        }

        private void OnNewFriendNotify(object msg)
        {

            if (null == msg)
            {
                GameEvents.RedPointEvents.User_OnNewFriendEvent.SafeInvoke(true);

                FriendDataManager.Instance.RefreshFriendInfo(FriendReqType.Added);
                TimeModule.Instance.SetTimeout(() => FriendDataManager.Instance.RefreshFriendInfo(FriendReqType.Addinfo), 0.5f);
                TimeModule.Instance.SetTimeout(() => FriendDataManager.Instance.RefreshFriendInfo(FriendReqType.Agreeing), 1.0f);
                TimeModule.Instance.SetTimeout(() => FriendDataManager.Instance.RefreshFriendGift(), 1.5f);

                return;
            }

            //1：有新增好友，2：有好友请求 3：有好友礼物
            var rsp = msg as SCFriendNoticeResponse;

            if (1 == rsp.Point)
            {
                PlayerPrefTool.SetIsThereNewFriend(true);
                GameEvents.RedPointEvents.User_OnNewFriendEvent.SafeInvoke(true);

                FriendDataManager.Instance.RefreshFriendInfo(FriendReqType.Added);
                TimeModule.Instance.SetTimeout(() => FriendDataManager.Instance.RefreshFriendInfo(FriendReqType.Addinfo), 0.5f);
            }
            else if (2 == rsp.Point)
            {
                PlayerPrefTool.SetIsThereNewApply(true);
                GameEvents.RedPointEvents.User_OnNewFriendEvent.SafeInvoke(true);

                FriendDataManager.Instance.RefreshFriendInfo(FriendReqType.Agreeing);

            }
            else if (3 == rsp.Point)
            {
                PlayerPrefTool.SetIsThereNewGift(true);
                GameEvents.RedPointEvents.User_OnNewFriendEvent.SafeInvoke(true);
                FriendDataManager.Instance.RefreshFriendGift();
            }


        }


        private void OnNewActivityNotify(object msg)
        {
            PlayerPrefTool.SetIsThereNewActivity(true);
            GameEvents.RedPointEvents.User_OnNewActivityEvent.SafeInvoke();
        }

        private void OnNewChapterNotify(object msg)
        {
            if (msg is SCChapterRedNotice)
            {
                SCChapterRedNotice res = (SCChapterRedNotice)msg;

                if (res.Reds != null)
                {
                    for (int i = 0; i < res.Reds.Count; i++)
                    {
                        Debug.Log("reddot ==== " + res.Reds[i]);
                        OnNewChapterTypeNotify(res.Reds[i]);
                    }

                }
            }
        }

        private void OnNewChapterTypeNotify(int type)
        {
            if (type == 0)
            {
                PlayerPrefTool.SetIsThereNewChapterNPC(true);
            }
            else if (type == 1)
            {
                PlayerPrefTool.SetIsThereNewChapterClue(true);
            }
            else if (type == 2)
            {
                PlayerPrefTool.SetIsThereNewChapterScene(true);
            }
            if (!GameEvents.RedPointEvents.User_OnNewChapterEvent.IsNull)
                GameEvents.RedPointEvents.User_OnNewChapterEvent.SafeInvoke(type);
        }

        private void OnNewAchievementNotify(object msg)
        {
            PlayerPrefTool.SetIsThereNewAchievement(true);
            if (!GameEvents.RedPointEvents.User_OnNewAchievementEvent.IsNull)
            {
                if (msg != null)
                {
                    SCAchievementUnlockResponse achievementMsg = (SCAchievementUnlockResponse)msg;
                    AchievementPopHintUILogic.Show(achievementMsg.Id);

                    GameEvents.PlayerEvents.RequestRecentAhievement.SafeInvoke();
                }
                GameEvents.RedPointEvents.User_OnNewAchievementEvent.SafeInvoke();
            }

        }

        private void OnNewNoticeNotify(object msg)
        {
            PlayerPrefTool.SetIsThereNewNotice(true);

            if (!GameEvents.RedPointEvents.User_OnNewNoticeEvent.IsNull)
                GameEvents.RedPointEvents.User_OnNewNoticeEvent.SafeInvoke();
        }
#if OFFICER_SYS
        private void OnNewPoliceNotify()
        {
            GameEvents.RedPointEvents.User_OnNewPoliceEvent.SafeInvoke(true);
        }



        private void OnNewPoliceNotify(object msg)
        {
            var rsp = msg as SCPlayerPropResponse;

            Dictionary<long, int> all_item_in_bag = new Dictionary<long, int>();

            foreach (var item in rsp.PlayerProps)
            {
                if (!all_item_in_bag.ContainsKey(item.PropId))
                {
                    all_item_in_bag.Add(item.PropId, item.Count);
                }
                else
                {
                    all_item_in_bag[item.PropId] += item.Count;
                }
            }

            if (IsThereCombineItem(all_item_in_bag.Keys.ToList<long>()))
            {
                if (IsThereAnOfficerCouldCombine(all_item_in_bag))
                {
                    if (IsThereAnOfficerCouldBeEmployed())
                    {
                        GameEvents.UI_Guid_Event.OnNewPolice.SafeInvoke(true);
                    }
                    //有升级的可能
                    GameEvents.RedPointEvents.User_OnNewPoliceEvent.SafeInvoke(true);
                    PlayerPrefTool.SetIsThereNewPolice(true);
                    return;
                }
            }

            GameEvents.RedPointEvents.User_OnNewPoliceEvent.SafeInvoke(false);
            PlayerPrefTool.SetIsThereNewPolice(false);
        }

        private void OnNewPoliceNotify(Dictionary<long, int> addtional_items)
        {
            if (PlayerPrefTool.GetIsThereNewPolice())
                return;

            if (IsThereCombineItem(addtional_items.Keys.ToList<long>()))
            {
                Dictionary<long, int> all_items = new Dictionary<long, int>();

                foreach (var item in GlobalInfo.MY_PLAYER_INFO.Bag_infos)
                {
                    all_items.Add(item.Key, item.Value.Count);
                }

                if (IsThereAnOfficerCouldCombine(all_items))
                {
                    //有升级的可能
                    GameEvents.RedPointEvents.User_OnNewPoliceEvent.SafeInvoke(true);
                    PlayerPrefTool.SetIsThereNewPolice(true);
                }
            }
        }

        #region 只检查是否有警员可升级，发现可以立刻返回
        private bool IsThereCombineItem(List<long> ids_)
        {
            foreach (var item in ConfCombineFormula.array)
            {
                if (ids_.Contains(item.propId1) || ids_.Contains(item.propId2) || ids_.Contains(item.propId3)
                    || ids_.Contains(item.propId4) || ids_.Contains(item.propId5) || ids_.Contains(item.specialPropId1)
                    || ids_.Contains(item.specialPropId2) || ids_.Contains(item.specialPropId3) || ids_.Contains(item.specialPropId4))
                {
                    return true;
                }
            }

            return false;
        }

        private bool IsThisOfficerCouldCombine(ConfOfficer officer_, Dictionary<long, OfficerInfo> my_combined_officers, Dictionary<long, int> all_item_in_bag_)
        {
            ConfOfficer item = officer_;

            long id = item.id;
            int lvl = 0;
            if (my_combined_officers.ContainsKey(id))
            {
                lvl = my_combined_officers[id].Level;
            }

            ConfCombineFormula next_level_combine_info = PoliceUILogicAssist.GetCombineInfo(item, lvl + 1);

            if (null == next_level_combine_info)
            {
                return false;
            }

            if (!all_item_in_bag_.ContainsKey(next_level_combine_info.propId1) || !all_item_in_bag_.ContainsKey(next_level_combine_info.propId2)
                   || !all_item_in_bag_.ContainsKey(next_level_combine_info.propId3) || !all_item_in_bag_.ContainsKey(next_level_combine_info.propId4)
                   || !all_item_in_bag_.ContainsKey(next_level_combine_info.propId5) || !all_item_in_bag_.ContainsKey(next_level_combine_info.specialPropId1)
                   || !all_item_in_bag_.ContainsKey(next_level_combine_info.specialPropId2) || !all_item_in_bag_.ContainsKey(next_level_combine_info.specialPropId3)
                   || !all_item_in_bag_.ContainsKey(next_level_combine_info.specialPropId4))
            {
                //if (!bag_items.ContainsKey(next_level_combine_info.propId1) || !bag_items.ContainsKey(next_level_combine_info.propId2)
                // || !bag_items.ContainsKey(next_level_combine_info.propId3) || !bag_items.ContainsKey(next_level_combine_info.propId4)
                // || !bag_items.ContainsKey(next_level_combine_info.propId5) || !bag_items.ContainsKey(next_level_combine_info.specialPropId1)
                // || !bag_items.ContainsKey(next_level_combine_info.specialPropId2) || !bag_items.ContainsKey(next_level_combine_info.specialPropId3)
                // || !bag_items.ContainsKey(next_level_combine_info.specialPropId4))
                //{
                return false;
                //}
            }

            //int s_id1_in_bag_count = bag_items.ContainsKey(next_level_combine_info.specialPropId1) ? bag_items[next_level_combine_info.specialPropId1].Count : 0;
            //int s_id2_in_bag_count = bag_items.ContainsKey(next_level_combine_info.specialPropId2) ? bag_items[next_level_combine_info.specialPropId2].Count : 0;
            //int s_id3_in_bag_count = bag_items.ContainsKey(next_level_combine_info.specialPropId3) ? bag_items[next_level_combine_info.specialPropId3].Count : 0;
            //int s_id4_in_bag_count = bag_items.ContainsKey(next_level_combine_info.specialPropId4) ? bag_items[next_level_combine_info.specialPropId4].Count : 0;

            int s_id1_in_add_count = all_item_in_bag_.ContainsKey(next_level_combine_info.specialPropId1) ? all_item_in_bag_[next_level_combine_info.specialPropId1] : 0;
            int s_id2_in_add_count = all_item_in_bag_.ContainsKey(next_level_combine_info.specialPropId2) ? all_item_in_bag_[next_level_combine_info.specialPropId2] : 0;
            int s_id3_in_add_count = all_item_in_bag_.ContainsKey(next_level_combine_info.specialPropId3) ? all_item_in_bag_[next_level_combine_info.specialPropId3] : 0;
            int s_id4_in_add_count = all_item_in_bag_.ContainsKey(next_level_combine_info.specialPropId4) ? all_item_in_bag_[next_level_combine_info.specialPropId4] : 0;


            //if (s_id1_in_bag_count + s_id1_in_add_count < next_level_combine_info.special1Count
            //    || s_id2_in_bag_count + s_id2_in_add_count < next_level_combine_info.special2Count
            //    || s_id3_in_bag_count + s_id3_in_add_count < next_level_combine_info.special3Count
            //    || s_id4_in_bag_count + s_id4_in_add_count < next_level_combine_info.special4Count)
            if (s_id1_in_add_count < next_level_combine_info.special1Count ||
                s_id2_in_add_count < next_level_combine_info.special2Count ||
                s_id3_in_add_count < next_level_combine_info.special3Count ||
                s_id4_in_add_count < next_level_combine_info.special4Count)

            {
                return false;
            }

            //if (bag_items[next_level_combine_info.specialPropId1].Count < next_level_combine_info.special1Count
            //|| bag_items[next_level_combine_info.specialPropId2].Count < next_level_combine_info.special2Count
            //|| bag_items[next_level_combine_info.specialPropId3].Count < next_level_combine_info.special3Count
            //|| bag_items[next_level_combine_info.specialPropId4].Count < next_level_combine_info.special4Count)
            //{

            //    continue;
            //}

            return true;
        }



        private bool IsThereAnOfficerCouldCombine(Dictionary<long, int> all_item_in_bag)
        {
            Dictionary<long, OfficerInfo> my_combined_officers = GlobalInfo.MY_PLAYER_INFO.Officer_dict;

            foreach (var item in ConfOfficer.array)
            {

                if (!IsThisOfficerCouldCombine(item, my_combined_officers, all_item_in_bag))
                {
                    continue;
                }

                return true;
            }

            return false;

        }
        #endregion






        #region 统一计算当前所有可以升级的警员，没个警员都检验
        List<long> m_could_comined_officers;

        public List<long> Could_comined_officers
        {
            get { return m_could_comined_officers; }
        }

        public void RefreshAllOfficersListCouldCombine()
        {
            m_could_comined_officers = new List<long>();

            Dictionary<long, int> all_items_in_bag = new Dictionary<long, int>();

            foreach (var item in GlobalInfo.MY_PLAYER_INFO.Bag_infos)
            {
                all_items_in_bag.Add(item.Key, item.Value.Count);
            }

            Dictionary<long, OfficerInfo> my_combined_officers = GlobalInfo.MY_PLAYER_INFO.Officer_dict;

            foreach (var item in ConfOfficer.array)
            {

                if (!IsThisOfficerCouldCombine(item, my_combined_officers, all_items_in_bag))
                {
                    continue;
                }

                m_could_comined_officers.Add(item.id);

            }
        }

        public bool IsThereAnOfficerCouldBeEmployed()
        {
            HashSet<long> my_officers = new HashSet<long>(GlobalInfo.MY_PLAYER_INFO.Officer_dict.Keys);
            HashSet<long> all_could_comined_officers = new HashSet<long>(m_could_comined_officers);

            all_could_comined_officers.ExceptWith(my_officers);

            return all_could_comined_officers.Count > 0;
        }

        public bool IsThisOfficerInCombinedList(long id_)
        {
            return null != m_could_comined_officers ? m_could_comined_officers.Contains(id_) : false;
        }
        #endregion
#endif




        private void OnNewEmailReaded()
        {
            PlayerPrefTool.SetIsThereNewEmail(false);
        }

        private void OnNewFriendReaded()
        {
            PlayerPrefTool.SetIsThereNewFriend(false);

            if (false == PlayerPrefTool.GetIsThereNewApply() && false == PlayerPrefTool.GetIsThereNewGift())
            {
                GameEvents.RedPointEvents.User_OnNewFriendEvent.SafeInvoke(false);
            }
        }

        private void OnNewApplyReaded()
        {
            PlayerPrefTool.SetIsThereNewApply(false);

            if (false == PlayerPrefTool.GetIsThereNewFriend() && false == PlayerPrefTool.GetIsThereNewGift())
            {
                GameEvents.RedPointEvents.User_OnNewFriendEvent.SafeInvoke(false);
            }
        }

        private void OnNewGiftReaded()
        {
            PlayerPrefTool.SetIsThereNewGift(false);

            if (false == PlayerPrefTool.GetIsThereNewFriend() && false == PlayerPrefTool.GetIsThereNewApply())
            {
                GameEvents.RedPointEvents.User_OnNewFriendEvent.SafeInvoke(false);
            }
        }

        private void OnNewActivityReaded()
        {
            PlayerPrefTool.SetIsThereNewActivity(false);
        }

        private void OnNewChapterReaded(int type)
        {
            if (type == 0)
            {
                PlayerPrefTool.SetIsThereNewChapterNPC(false);
            }
            else if (type == 1)
            {
                PlayerPrefTool.SetIsThereNewChapterClue(false);
            }
            else if (type == 2)
            {
                PlayerPrefTool.SetIsThereNewChapterScene(false);
            }
            CSClearChapterRedRequest req = new CSClearChapterRedRequest();
            req.ClearType = type;
            GameEvents.NetWorkEvents.SendAsyncMsg.SafeInvoke(req);
        }

        private void OnNewAchievementReaded()
        {
            PlayerPrefTool.SetIsThereNewAchievement(false);
        }

        private void OnNewNoticeReaded()
        {
            PlayerPrefTool.SetIsThereNewNotice(false);
        }


    }
}
