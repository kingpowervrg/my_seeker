/********************************************************************
	created:  2018-5-24 10:52:22
	filename: ChapterInfo.cs
	author:	  songguangze@outlook.com
	
	purpose:  章节对象
*********************************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SeekerGame
{
    public class ChapterInfo
    {
        private long m_chapterID;

        //章节内场景列表
        private IList<ChapterSceneInfo> m_chapterSectionList = new List<ChapterSceneInfo>();

        //章节内任务列表
        private IList<ChapterTaskInfo> m_chapterTaskList = new List<ChapterTaskInfo>();

        //章节内NPC列表
        private IList<ChapterNpcInfo> m_chapterNpcList = new List<ChapterNpcInfo>();

        //章节线索列表
        private IList<ChapterClueInfo> m_clueList = new List<ChapterClueInfo>();

        public ChapterInfo(long chapterID)
        {
            this.m_chapterID = chapterID;
        }

        /// <summary>
        /// 同步章节详细信息
        /// </summary>
        /// <param name="sceneInfoList"></param>
        /// <param name="taskInfoList"></param>
        /// <param name="npcInfoList"></param>
        /// <param name="clueInfoLIst"></param>
        public void SyncChapterInfo(IList<ChapterSceneInfo> sceneInfoList, IList<ChapterTaskInfo> taskInfoList, IList<ChapterNpcInfo> npcInfoList, IList<ChapterClueInfo> clueInfoLIst)
        {
            this.ChapterSyncStatus = SyncStatus.SYNCED;

            //todo:进度解锁具体通知看需求
            this.m_chapterNpcList = npcInfoList;
            this.m_chapterSectionList = sceneInfoList;
            this.m_chapterTaskList = taskInfoList;
            this.m_clueList = clueInfoLIst;
            StatisticsAllTask();
        }

        private void StatisticsAllTask()
        {
            this.totalTaskNum = ChapterConfData.taskIds.Length;
            this.hasUnLockTaskNum = 0;

            for (int i = 0; i < this.m_chapterTaskList.Count; i++)
            {
                if (this.m_chapterTaskList[i].Status == 2)
                {
                    this.hasUnLockTaskNum++;
                }
            }
            if (m_SyncComplete != null)
            {
                m_SyncComplete();
            }

        }


        /// <summary>
        /// 配置ID
        /// </summary>
        public long ChapterID => this.m_chapterID;

        /// <summary>
        /// UUID
        /// </summary>
        public long PlayerChapterUUID { get; set; }

        /// <summary>
        /// 章节状态
        /// </summary>
        public ChapterStatus ChapterStatus { get; set; } = ChapterStatus.LOCK;

        /// <summary>
        /// 章节静态配置数据
        /// </summary>
        public ConfChapter ChapterConfData => ConfChapter.Get(this.m_chapterID);

        /// <summary>
        /// 章节信息同步状态
        /// </summary>
        public SyncStatus ChapterSyncStatus { get; private set; } = SyncStatus.LOCAL;

        public System.Action m_SyncComplete = null; //同步完成

        public IList<ChapterNpcInfo> NpcInfoList => this.m_chapterNpcList;
        public IList<ChapterSceneInfo> SceneInfoList => this.m_chapterSectionList;
        public IList<ChapterClueInfo> ClueInfoLIst => this.m_clueList;
        public IList<ChapterTaskInfo> TaskList => this.m_chapterTaskList;

        public int totalTaskNum = 0;
        public int hasUnLockTaskNum = 0;

        public int taskProgressValue
        {
            get
            {
                if (totalTaskNum == 0)
                {
                    return 0;
                }
                return (int)((hasUnLockTaskNum / (float)totalTaskNum) * 100);
            }
        }
    }

    /// <summary>
    /// 章节状态
    /// </summary>
    public enum ChapterStatus
    {
        LOCK,       //未解锁
        UNLOCK,     //已解锁
        DONE,       //已完成
    }

    /// <summary>
    /// 同步状态
    /// </summary>
    public enum SyncStatus
    {
        LOCAL,          //本地状态
        SYNCED,         //已同步
    }
}