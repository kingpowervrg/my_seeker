/********************************************************************
	created:  2018-5-16 17:2:51
	filename: GameInitState.cs
	author:   songguangze@fotoable.com
	
	purpose:  邮件系统
*********************************************************************/
using EngineCore;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SeekerGame
{
    public class MailSystem : IDisposable
    {
        private PlayerInfo m_playerInfo = null;
        private SortedList<MailSortKey, Mail> m_mailList = new SortedList<MailSortKey, Mail>(new MailSortKey());
        public MailSystem(PlayerInfo playerInfo)
        {
            this.m_playerInfo = playerInfo;

            MessageHandler.RegisterMessageHandler(MessageDefine.SCEmailListResponse, OnSyncMailList);
            MessageHandler.RegisterMessageHandler(MessageDefine.SCEmailRewardResponse, OnMailRewardResponse);
            MessageHandler.RegisterMessageHandler(MessageDefine.SCEmailChangeNotice, OnNewEmailNotify);

            GameEvents.MailEvents.OnReadMail += OnReadMail;
        }

        #region 同步玩家邮件列表
        /// <summary>
        /// 同步玩家邮件列表
        /// </summary>
        public void SyncPlayerMails()
        {
            CSEmailListRequest req = new CSEmailListRequest();

            GameEvents.NetWorkEvents.SendAsyncMsg.SafeInvoke(req);

            //Debug.Log("send CSEmailListRequest");
        }

        private void OnSyncMailList(object message)
        {
            SCEmailListResponse msg = message as SCEmailListResponse;
            for (int i = 0; i < msg.Emails.Count; ++i)
            {
                EmailInfo syncMailInfo = msg.Emails[i];

                if (syncMailInfo.Status == (int)MailStatus.Rewarded)
                    continue;

                Mail existMailInfo = FindMailByID(syncMailInfo.Id);
                if (existMailInfo != null)
                    continue;
                else
                {
                    Mail mail = new Mail(syncMailInfo.Id, (MailType)syncMailInfo.Type, syncMailInfo.ReceiveTime, syncMailInfo.Title, syncMailInfo.Content, syncMailInfo.Deadline);
                    mail.MailStatus = (MailStatus)syncMailInfo.Status;
                    for (int j = 0; j < syncMailInfo.Rewards.Count; ++j)
                    {
                        EmailReward rewardInfo = syncMailInfo.Rewards[j];
                        mail.MailRewardItemList.Add(new ItemWrapper() { ItemID = rewardInfo.PropId, ItemNum = rewardInfo.Count });
                    }

                    this.m_mailList.Add(new MailSortKey() { MailUUID = syncMailInfo.Id, MailSendTime = syncMailInfo.ReceiveTime }, mail);
                }
            }
        }
        #endregion

        /// <summary>
        /// 有新邮件通知
        /// </summary>
        /// <param name="msg"></param>
        private void OnNewEmailNotify(object msg)
        {
            SyncPlayerMails();
        }

        /// <summary>
        /// 查找邮件
        /// </summary>
        /// <param name="mailUUID"></param>
        /// <returns></returns>
        public Mail FindMailByID(long mailUUID)
        {
            KeyValuePair<MailSortKey, Mail>? mail = this.m_mailList.FirstOrDefault(pair => pair.Value.MailUUID == mailUUID);
            if (mail.HasValue)
                return mail.Value.Value;

            return null;
        }

        /// <summary>
        /// 通过类型过滤邮件
        /// </summary>
        /// <param name="mailType"></param>
        /// <returns></returns>
        public List<Mail> FilterMailByMailType(MailType mailType)
        {
            return this.m_mailList.Where(pair => pair.Value.MailType == mailType).OrderByDescending(pair => pair.Key.MailSendTime).Select(pair => pair.Value).ToList();
        }

        /// <summary>
        /// 过滤未读邮件
        /// </summary>
        /// <returns></returns>
        public List<Mail> FilterUnreadMail()
        {
            List<Mail> unreadMailList = this.m_mailList.Where(pair => !pair.Value.IsRead && pair.Value.IsVisiable).OrderByDescending(pair => pair.Key.MailSendTime).Select(pair => pair.Value).ToList();

            return unreadMailList;
        }

        /// <summary>
        /// 过滤指定类型未读邮件
        /// </summary>
        /// <param name="mailType"></param>
        /// <returns></returns>
        public List<Mail> FilterUnreadMailByType(MailType mailType)
        {
            List<Mail> unreadMail = FilterUnreadMail().Where(mail => mail.MailType == mailType).ToList();

            return unreadMail;
        }

        /// <summary>
        /// 邮件排序
        /// </summary>
        /// <param name="mailType"></param>
        /// <returns></returns>
        public List<Mail> SortMailList(MailType mailType = MailType.SYSTEM)
        {
            List<Mail> unreadMailList = this.m_mailList.Where(pair => pair.Value.MailStatus == MailStatus.Unread).OrderByDescending(pair => pair.Key.MailSendTime).Select(pair => pair.Value).ToList();
            List<Mail> readMailList = this.m_mailList.Where(pair => pair.Value.MailStatus != MailStatus.Unread && pair.Value.IsVisiable).OrderBy(pair => pair.Value.MailExpireTime).Select(pair => pair.Value).ToList();

            List<Mail> sortedMailList = new List<Mail>();
            sortedMailList.AddRange(unreadMailList);
            sortedMailList.AddRange(readMailList);

            List<Mail> ret = new List<Mail>(sortedMailList.Where((item) => mailType == item.MailType));

            return ret;
        }


        /// <summary>
        /// 领取奖励通知
        /// </summary>
        /// <param name="message"></param>
        private void OnMailRewardResponse(object message)
        {
            SCEmailRewardResponse msg = message as SCEmailRewardResponse;
            Mail rewardMail = FindMailByID(msg.Id);
            rewardMail.MailStatus = MailStatus.Rewarded;

            List<Mail> mailList = FilterMailByMailType(rewardMail.MailType);
            GameEvents.MailEvents.OnMailListChanged.SafeInvoke(rewardMail.MailType, mailList);

            GameEvents.PlayerEvents.RequestLatestPlayerInfo.SafeInvoke();
        }

        /// <summary>
        /// 阅读邮件
        /// </summary>
        /// <param name="mailUUID"></param>
        public void OnReadMail(long mailUUID)
        {
            Mail mail = FindMailByID(mailUUID);
            if (mail != null)
            {
                mail.MailStatus = MailStatus.Read;

                CSEmailReadRequest readMailRequest = new CSEmailReadRequest();
                readMailRequest.Id = mail.MailUUID;
                GameEvents.NetWorkEvents.SendAsyncMsg.SafeInvoke(readMailRequest);

                GameEvents.MailEvents.OnMailListChanged.SafeInvoke(mail.MailType, SortMailList());
            }
        }

        public void Dispose()
        {
            MessageHandler.UnRegisterMessageHandler(MessageDefine.SCEmailListResponse, OnSyncMailList);
            MessageHandler.UnRegisterMessageHandler(MessageDefine.SCEmailRewardResponse, OnMailRewardResponse);
            MessageHandler.UnRegisterMessageHandler(MessageDefine.SCEmailChangeNotice, OnNewEmailNotify);

            GameEvents.MailEvents.OnReadMail -= OnReadMail;
        }

        private class MailSortKey : IComparer<MailSortKey>
        {
            public long MailUUID = 0L;
            public long MailSendTime = 0L;

            public int Compare(MailSortKey x, MailSortKey y)
            {
                if (x.MailSendTime > y.MailSendTime)
                    return -1;
                else if (x.MailSendTime < y.MailSendTime)
                    return 1;
                else
                {
                    return x.MailUUID > y.MailUUID ? 1 : -1;
                }
            }
        }
    }

}