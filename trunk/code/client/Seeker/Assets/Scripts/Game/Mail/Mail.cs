/********************************************************************
	created:  2018-5-16 15:15:29
	filename: Mail.cs
	author:	  songguangze@fotoable.com
	
	purpose:  邮件对象
*********************************************************************/
using System;
using System.Collections.Generic;

namespace SeekerGame
{
    public class Mail
    {
        private long m_mailUUId = 0L;
        private MailType m_mailType = MailType.SYSTEM;
        private List<ItemWrapper> m_mailItemList = new List<ItemWrapper>();
        private long m_mailExpireTime = 0L;
        private long m_mailSendTime = 0L;

        private string m_mailTitle = string.Empty;
        private string m_mailDesc = string.Empty;

        public Mail(long mailUUID, MailType mailType, long mailSendTime, string mailTitle, string mailDesc, long mailExpireTime)
        {
            this.m_mailUUId = mailUUID;
            this.m_mailType = mailType;
            this.m_mailTitle = mailTitle;
            this.m_mailDesc = mailDesc;
            this.m_mailSendTime = mailSendTime * 10000;
            this.m_mailExpireTime = mailExpireTime * 10000;
        }

        public bool IsVisiable
        {
            get
            {
                if ((HasRewardItem && MailStatus == MailStatus.Rewarded) || IsExpire || (MailStatus == MailStatus.Read && !HasRewardItem))
                    return false;

                return true;
            }
        }

        public bool HasRewardItem => this.m_mailItemList.Count > 0;
        public long MailExpireTime => this.m_mailExpireTime;
        public long MailUUID => this.m_mailUUId;
        public bool IsRead
        {
            get
            {
                return !(MailStatus == MailStatus.Unread || (HasRewardItem && MailStatus == MailStatus.Read));
            }
        }

        public List<ItemWrapper> MailRewardItemList => this.m_mailItemList;
        public long MailSendTime => this.m_mailSendTime;
        public MailType MailType => this.m_mailType;
        public MailStatus MailStatus { get; set; } = MailStatus.Unread;
        public string MailTitle => this.m_mailTitle;
        public string MailDesc => this.m_mailDesc;
        public bool IsExpire => (CommonTools.TimeStampToDateTime(MailExpireTime) - DateTime.Now).Seconds < 0;
    }

    /// <summary>
    /// 邮件类型
    /// </summary>
    public enum MailType
    {
        FRIEND = 1,
        SYSTEM,
    }

    /// <summary>
    /// 邮件状态
    /// </summary>
    public enum MailStatus
    {
        Unread,
        Read,
        Rewarded
    }

}