/********************************************************************
	created:  2018-5-24 11:53:47
	filename: ChapterSystem.cs
	author:	  songguangze@fotoable.com
	
	purpose:  玩家章节系统
*********************************************************************/
using EngineCore;
using Google.Protobuf;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SeekerGame
{
    public class ChapterSystem : IDisposable
    {
        private PlayerInfo m_playerInfo = null;

        private ChapterSet m_playerChapterSet = new ChapterSet();
        private ChapterInfo m_currentChapterInfo = null;        //当前正在进行的章节
        private int m_openChapterCount = 0;

        public ChapterSystem(PlayerInfo playerInfo)
        {
            this.m_playerInfo = playerInfo;
            this.m_openChapterCount = int.Parse(ConfServiceConfig.Get(22).fieldValue);

            MessageHandler.RegisterMessageHandler(MessageDefine.SCChapterListResponse, OnSyncPlayerChapterList);
            MessageHandler.RegisterMessageHandler(MessageDefine.SCChapterDetailResponse, OnSyncChapterDetailInfo);
            MessageHandler.RegisterMessageHandler(MessageDefine.SCChapterStatusChangeNotice, OnChapterStatusChaged);

            GameEvents.TaskEvents.OnAcceptNewTask += (taskID) =>
            {
                SyncCurrentChapterDetail();
            };

            //GM跳章节
#if UNITY_DEBUG
            GMModule.GMCommandWrapList.FirstOrDefault(cmd => cmd.MessageType == typeof(GmFinishTask)).GMCommandInjector = GMjumpTask;
#endif
        }




        /// <summary>
        /// 初始化玩家档案系统
        /// </summary>
        public void InitPlayerChapterSystem()
        {
            CSChapterListRequest msg = new CSChapterListRequest();

            GameEvents.NetWorkEvents.SendAsyncMsg.SafeInvoke(msg);

            Debug.Log("send CSChapterListRequest");
        }

        /// <summary>
        /// 同步玩家解锁的章节列表
        /// </summary>
        /// <param name="msg"></param>
        private void OnSyncPlayerChapterList(object msg)
        {
            SCChapterListResponse messageChapterList = msg as SCChapterListResponse;
            bool allChapterFinished = true;
            for (int i = 0; i < messageChapterList.Chapters.Count; ++i)
            {
                PlayerChapterInfo playerChapterInfo = messageChapterList.Chapters[i];
                long chapterConfID = playerChapterInfo.ChapterId;
                long chapterUUID = playerChapterInfo.PlayerChapterId;

                //if (chapterConfID > this.m_openChapterCount)
                //    continue;

                ChapterInfo chapterInfo = new ChapterInfo(chapterConfID);
                chapterInfo.ChapterStatus = (ChapterStatus)playerChapterInfo.Status;
                chapterInfo.PlayerChapterUUID = chapterUUID;

                if (chapterInfo.ChapterStatus == ChapterStatus.UNLOCK)
                {
                    this.m_currentChapterInfo = chapterInfo;
                    FindObjSceneDataManager.Instance.RefreshAllInfo(this.m_currentChapterInfo.ChapterID);

                    allChapterFinished = false;

                    //同步已解锁的章节
                    SyncCurrentChapterDetail();
                }
                //下一章还没解锁


                this.m_playerChapterSet.AddChapterInfo(chapterInfo);
            }


            //初始化玩家未解锁的章节
            for (int i = 1; i <= this.m_openChapterCount; ++i)
            {
                ConfChapter confchapter = ConfChapter.Get(i);
                if (!this.m_playerChapterSet.IsExistChapterInfo(confchapter.id))
                {
                    ChapterInfo chapterInfo = new ChapterInfo(confchapter.id);
                    this.m_playerChapterSet.AddChapterInfo(chapterInfo);
                    allChapterFinished = false;
                }
            }

            //最后所有章节全部解锁
            if (allChapterFinished)
            {
                
                this.m_currentChapterInfo = this.m_playerChapterSet.GetChapterInfoByID(this.m_openChapterCount);
                FindObjSceneDataManager.Instance.RefreshAllInfo(this.m_currentChapterInfo.ChapterID);
                SyncCurrentChapterDetail();
            }

            if (this.m_currentChapterInfo == null)
            {
                this.m_currentChapterInfo = m_playerChapterSet.GetLastestChapterInfo();
                FindObjSceneDataManager.Instance.RefreshAllInfo(this.m_currentChapterInfo.ChapterID);
                SyncCurrentChapterDetail();
            }
        }

        /// <summary>
        /// 章节变化通知
        /// </summary>
        /// <param name="msg"></param>
        private void OnChapterStatusChaged(object msg)
        {
            SCChapterStatusChangeNotice message = msg as SCChapterStatusChangeNotice;
            ChapterInfo existChapterInfo = this.m_playerChapterSet.GetChapterInfoByID(message.ChapterId);
            existChapterInfo.PlayerChapterUUID = message.PlayerChapterId;

            if (existChapterInfo.ChapterStatus == ChapterStatus.LOCK)
            {
                //解锁新场景
                if ((ChapterStatus)message.NewStatus == ChapterStatus.UNLOCK)
                    UnLockChapter(message.PlayerChapterId, message.ChapterId);
            }
            else if (existChapterInfo.ChapterStatus == ChapterStatus.UNLOCK)
            {
                //场景完成
                if ((ChapterStatus)message.NewStatus == ChapterStatus.DONE)
                {
                    existChapterInfo.ChapterStatus = ChapterStatus.DONE;
                    GameEvents.ChapterEvents.OnDoneChapter.SafeInvoke(existChapterInfo);
                }
            }
            existChapterInfo.ChapterStatus = (ChapterStatus)message.NewStatus;

            GameEvents.ChapterEvents.OnChapterInfoUpdated.SafeInvoke(existChapterInfo);
        }

        /// <summary>
        /// 解锁新的章节
        /// </summary>
        /// <param name="chapterUUID"></param>
        /// <param name="chapterConfID"></param>
        public void UnLockChapter(long chapterUUID, long chapterConfID)
        {
            ChapterInfo unlockedChapterInfo = this.m_playerChapterSet.UnLockNewChapter(chapterUUID, chapterConfID);
            SyncChapterDetailInfo(chapterUUID);

            m_currentChapterInfo = unlockedChapterInfo;
            FindObjSceneDataManager.Instance.RefreshAllInfo(this.m_currentChapterInfo.ChapterID);
            GameEvents.ChapterEvents.OnUnlockChapter.SafeInvoke(unlockedChapterInfo);
        }

        /// <summary>
        /// 同步章节详细信息
        /// </summary>
        /// <param name="msg"></param>
        private void OnSyncChapterDetailInfo(object msg)
        {
            SCChapterDetailResponse msgChapterDetail = msg as SCChapterDetailResponse;
            long chapterUUID = msgChapterDetail.PlayerChapterId;
            long chapterConfigID = msgChapterDetail.ChapterId;

            if (chapterUUID == 0 || chapterConfigID == 0)
            {
                Debug.LogWarning("Sync ChapterID is 0");
                return;
            }

            ChapterInfo existChapterInfo = this.m_playerChapterSet.GetChapterInfoByID(chapterConfigID);
            existChapterInfo.SyncChapterInfo(msgChapterDetail.Scenes, msgChapterDetail.Tasks, msgChapterDetail.Npcs, msgChapterDetail.Clues);

            GameEvents.ChapterEvents.OnChapterInfoUpdated.SafeInvoke(existChapterInfo);
        }

        /// <summary>
        /// 获取章节最新信息
        /// </summary>
        /// <param name="chapterID"></param>
        public ChapterInfo GetChapterLatestInfo(long chapterID)
        {
            ChapterInfo chapterInfo = this.m_playerChapterSet.GetChapterInfoByID(chapterID);
            if (chapterInfo.ChapterStatus == ChapterStatus.DONE)
            {
                if (chapterInfo.ChapterSyncStatus == SyncStatus.LOCAL)
                    SyncChapterDetailInfo(chapterInfo.PlayerChapterUUID);
            }

            return chapterInfo;
        }

        /// <summary>
        /// 同步当前章节最新信息
        /// </summary>
        public void SyncCurrentChapterDetail()
        {
            if (this.m_currentChapterInfo != null)
                SyncChapterDetailInfo(this.m_currentChapterInfo.PlayerChapterUUID);
        }

        /// <summary>
        /// 同步章节详细信息
        /// </summary>
        /// <param name="chapterUUID"></param>
        private void SyncChapterDetailInfo(long chapterUUID)
        {
            CSChapterDetailRequest requestDetailChapterInfo = new CSChapterDetailRequest();
            requestDetailChapterInfo.PlayerChapterId = chapterUUID;

            GameEvents.NetWorkEvents.SendAsyncMsg.SafeInvoke(requestDetailChapterInfo);
        }

#if UNITY_DEBUG
        /// <summary>
        /// GM注入命令-跳任务
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        private List<IMessage> GMjumpTask(Dictionary<string, string> param)
        {
            List<IMessage> messageList = new List<IMessage>();

            if (!param.ContainsKey("taskid"))
                return null;

            long chapterID = 0;
            if (!param.ContainsKey("chapterid"))
                chapterID = long.Parse(param["chapterid"]);

            long finishTaskID = long.Parse(param["taskid"]);
            if (chapterID == 0)
            {
                ChapterInfo chapterInfo = FindChapterByTaskID(finishTaskID);
                if (chapterInfo == null)
                    return null;

                chapterID = chapterInfo.ChapterID;
            }

            GmFinishTask finshTaskCmd = new GmFinishTask();
            finshTaskCmd.PlayerId = GlobalInfo.MY_PLAYER_ID;
            finshTaskCmd.TaskId = finishTaskID;
            finshTaskCmd.ChapterId = chapterID;

            messageList.Add(finshTaskCmd);

            return messageList;
        }
#endif

        public List<ChapterInfo> PlayerChapterSet
        {
            get
            {
                List<ChapterInfo> allChapterList = new List<ChapterInfo>();
                allChapterList.AddRange(this.m_playerChapterSet.UnlockedChapterList);
                allChapterList.AddRange(this.m_playerChapterSet.LockChapterList);

                return allChapterList;
            }
        }

        public ChapterSet ChapterSet
        {
            get { return this.m_playerChapterSet; }
        }

        /// <summary>
        /// 通过任务ID找到章节配置
        /// </summary>
        /// <param name="taskConfigID"></param>
        /// <returns></returns>
        public ChapterInfo FindChapterByTaskID(long taskConfigID)
        {
            ConfChapter chapterConfig = ConfChapter.array.Find(confChapter => confChapter.taskIds.Contains(taskConfigID));
            if (chapterConfig == null)
                return null;

            return this.m_playerChapterSet.GetChapterInfoByID(chapterConfig.id);
        }

        /// <summary>
        /// 通过章节中场景ID获取
        /// </summary>
        /// <param name="chapterSceneID"></param>
        /// <returns></returns>
        public ConfChapter FindChapterBySceneID(long chapterSceneID)
        {
            ConfChapter chapterConfig = ConfChapter.array.Find(confChapter => confChapter.scenceIds.Contains(chapterSceneID));
            if (chapterConfig == null)
                return null;

            return chapterConfig;
        }

        /// <summary>
        /// 获取章节所用到的所有资源
        /// </summary>
        /// <param name="chapterID"></param>
        /// <returns></returns>
        public List<string> GetChapterAssetList(long chapterID)
        {
            List<ConfAssetManifest> chapterAssets;
            ConfAssetManifest.GetConfigByCondition($"chapterID = {chapterID}", out chapterAssets);
            return chapterAssets.Select(chaterAsset => chaterAsset.AssetBundleName).ToList();
        }

        /// <summary>
        /// 获取章节所需动态资源列表
        /// </summary>
        /// <param name="chapterID"></param>
        /// <returns></returns>
        public List<string> GetChapterDynamicAssetList(long chapterID)
        {
            List<string> chapterAllBundleNameList = GetChapterAssetList(chapterID);
            List<string> localNotIncludeBundleList = ResourceMgr.Instance.BundleMap.GetNotIncludeBundleList(chapterAllBundleNameList);

            return localNotIncludeBundleList;
        }

        /// <summary>
        /// 章节所需资源是否存在
        /// </summary>
        /// <param name="chapterID"></param>
        /// <returns></returns>
        public bool IsChapterAssetExist(long chapterID)
        {
            return GetChapterDynamicAssetList(chapterID).Count == 0;
        }



        public void Dispose()
        {
            MessageHandler.UnRegisterMessageHandler(MessageDefine.SCChapterListResponse, OnSyncPlayerChapterList);
            MessageHandler.UnRegisterMessageHandler(MessageDefine.SCChapterDetailResponse, OnSyncChapterDetailInfo);
            MessageHandler.UnRegisterMessageHandler(MessageDefine.SCChapterStatusChangeNotice, OnChapterStatusChaged);

            GameEvents.TaskEvents.OnAcceptNewTask -= (taskID) =>
            {
                SyncCurrentChapterDetail();
            };
        }

        public ChapterInfo CurrentChapterInfo => this.m_currentChapterInfo;

    }
}