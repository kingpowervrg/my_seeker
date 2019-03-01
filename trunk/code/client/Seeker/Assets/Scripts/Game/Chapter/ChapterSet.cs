/********************************************************************
	created:  2018-5-24 12:25:53
	filename: ChapterSet.cs
	author:	  songguangze@fotoable.com
	
	purpose:  章节集
*********************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;

namespace SeekerGame
{
    public class ChapterSet
    {
        private Dictionary<long, ChapterInfo> m_chapterInfoDict = new Dictionary<long, ChapterInfo>();

        private List<ChapterInfo> m_lockedChapterList = new List<ChapterInfo>();
        private List<ChapterInfo> m_unlockChapterList = new List<ChapterInfo>();

        /// <summary>
        /// 添加章节信息
        /// </summary>
        /// <param name="chapterInfo"></param>
        public void AddChapterInfo(ChapterInfo chapterInfo)
        {
            if (this.m_chapterInfoDict.ContainsKey(chapterInfo.ChapterID))
                throw new Exception($"chapter uuid { chapterInfo.PlayerChapterUUID} exist");

            this.m_chapterInfoDict.Add(chapterInfo.ChapterID, chapterInfo);
            switch (chapterInfo.ChapterStatus)
            {
                case ChapterStatus.DONE:
                case ChapterStatus.UNLOCK:
                    this.m_unlockChapterList.Add(chapterInfo);
                    break;
                case ChapterStatus.LOCK:
                    this.m_lockedChapterList.Add(chapterInfo);
                    break;
            }
        }

        public void RemoveChapterInfo(long chapterConfId)
        {

        }

        public void RemoveChapterInfo(ChapterInfo removeChapterInfo)
        {

        }

        public ChapterInfo GetLastestChapterInfo()
        {
            this.m_unlockChapterList = this.m_unlockChapterList.OrderBy(chapterInfo => chapterInfo.ChapterID).ToList();
            return this.m_unlockChapterList.Last();
        }

        /// <summary>
        /// 解锁新章节
        /// </summary>
        /// <param name="chapterUUID"></param>
        /// <param name="chapterConfID"></param>
        public ChapterInfo UnLockNewChapter(long chapterUUID, long chapterConfID)
        {
            ChapterInfo newChapterInfo = GetChapterInfoByID(chapterConfID);
            if (newChapterInfo != null)
            {
                this.m_lockedChapterList.Remove(newChapterInfo);

                newChapterInfo.PlayerChapterUUID = chapterUUID;
                newChapterInfo.ChapterStatus = ChapterStatus.UNLOCK;
                this.m_unlockChapterList.Add(newChapterInfo);
            }
            return newChapterInfo;
        }

        /// <summary>
        /// 通过ConfigID获取章节信息
        /// </summary>
        /// <param name="chapterConfId"></param>
        /// <returns></returns>
        public ChapterInfo GetChapterInfoByID(long chapterConfId)
        {
            ChapterInfo chapterInfo;
            if (this.m_chapterInfoDict.TryGetValue(chapterConfId, out chapterInfo))
                return chapterInfo;

            return null;
        }

        /// <summary>
        /// 通过UUID获取章节信息
        /// </summary>
        /// <param name="playerChapterUUID"></param>
        /// <returns></returns>
        public ChapterInfo GetChapterInfoByUUID(long playerChapterUUID)
        {
            return this.m_chapterInfoDict.Values.FirstOrDefault(chapterInfo => chapterInfo.PlayerChapterUUID == playerChapterUUID);
        }

        /// <summary>
        /// 章节信息是否存在 
        /// </summary>
        /// <param name="chapterConfID"></param>
        /// <returns></returns>
        public bool IsExistChapterInfo(long chapterConfID)
        {
            return this.m_chapterInfoDict.ContainsKey(chapterConfID);
        }

        /// <summary>
        /// 未解锁的章节信息
        /// </summary>
        public List<ChapterInfo> LockChapterList => this.m_lockedChapterList;

        /// <summary>
        /// 已解锁的章节信息(完成的，正在进行的)
        /// </summary>
        public List<ChapterInfo> UnlockedChapterList => this.m_unlockChapterList;
    }
}