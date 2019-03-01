/********************************************************************
	created:  2018-7-19 11:4:48
	filename: AssetUpdateInfo.cs
	author:	  songguangze@outlook.com
	
	purpose:  资源更新信息
*********************************************************************/
using System.Collections.Generic;
using UnityEngine;

namespace EngineCore
{
    public class AssetUpdateInfo
    {
        private int m_totalAssetSize;
        private int m_remainDownloadSize;
        private int m_totalAssetCount;
        private int m_remailDownloadAssetCount;
        private Dictionary<string, BundleIndexItemInfo> m_updatingBundleMapItemInfo = new Dictionary<string, BundleIndexItemInfo>();
        private List<string> m_remainDownloadFileList = new List<string>();
        private List<string> m_errorList = new List<string>();
        private int m_retryTimes = 0;
        private float m_startDownloadTime = 0;


        public void AddUpdateBundleIndexItem(BundleIndexItemInfo needUpdateBundleIndexItemInfo)
        {
            if (this.m_updatingBundleMapItemInfo.ContainsKey(needUpdateBundleIndexItemInfo.BundleHashName))
            {
                Debug.LogWarning($"add update file duplicate {needUpdateBundleIndexItemInfo.BundleName}");
                return;
            }


            this.m_updatingBundleMapItemInfo.Add(needUpdateBundleIndexItemInfo.BundleHashName, needUpdateBundleIndexItemInfo);
            this.m_remainDownloadFileList.Add(needUpdateBundleIndexItemInfo.BundleHashName);

            this.m_totalAssetSize += needUpdateBundleIndexItemInfo.BundleSize;
            this.m_remainDownloadSize += needUpdateBundleIndexItemInfo.BundleSize;
            this.m_totalAssetCount++;
            this.m_remailDownloadAssetCount++;
        }


        /// <summary>
        /// 文件下载完成
        /// </summary>
        /// <param name="downloadedBundleItemInfo"></param>
        public void SetAssetFinishDownload(BundleIndexItemInfo downloadedBundleItemInfo)
        {
            this.m_remainDownloadFileList.Remove(downloadedBundleItemInfo.BundleHashName);
            this.m_remainDownloadSize -= downloadedBundleItemInfo.BundleSize;
            this.m_remailDownloadAssetCount--;

            this.m_errorList.Remove(downloadedBundleItemInfo.BundleHashName);
        }

        public void SetAssetFinishDownload(string downloadedBundleHashName)
        {
            if (this.m_updatingBundleMapItemInfo.ContainsKey(downloadedBundleHashName))
                SetAssetFinishDownload(this.m_updatingBundleMapItemInfo[downloadedBundleHashName]);

        }


        public void RetryErrorAssets(List<string> errorList)
        {
            this.m_errorList = errorList;
            if (this.m_errorList.Count > 0)
                this.m_retryTimes++;
        }

        /// <summary>
        /// 下载文件总大小
        /// </summary>
        public int TotalAssetSize => this.m_totalAssetSize;

        /// <summary>
        /// 剩余需要下载的文件大小
        /// </summary>
        public int RemainDownloadSize => this.m_remainDownloadSize;

        /// <summary>
        /// 下载文件总数量
        /// </summary>
        public int TotalAssetCount => this.m_totalAssetCount;

        /// <summary>
        /// 剩余下载数量 
        /// </summary>
        public int RemainDownloadAssetCount => this.m_remailDownloadAssetCount;

        /// <summary>
        /// 剩余需要下载的文件列表
        /// </summary>
        public List<string> RemainDownloadFileList => this.m_remainDownloadFileList;

        /// <summary>
        /// 出错文件列表
        /// </summary>
        public List<string> ErrorAssetList => this.m_errorList;

        /// <summary>
        /// 出错文件数量
        /// </summary>
        public int ErrorAssetCount => this.ErrorAssetList.Count;

        /// <summary>
        /// 开始下载的时间
        /// </summary>
        public float StartDownloadTime
        {
            get { return this.m_startDownloadTime; }
            set
            {
                if (this.m_startDownloadTime == 0f)
                    this.m_startDownloadTime = value;
            }
        }

        /// <summary>
        /// 平均每个文件下载的时间
        /// </summary>
        public float AveragePerFileTime => (Time.time - StartDownloadTime) / (TotalAssetCount - RemainDownloadAssetCount);

        /// <summary>
        /// 平均下载速度
        /// </summary>
        public float DownloadSpeed => (TotalAssetSize - RemainDownloadSize) / (Time.time - StartDownloadTime);

        /// <summary>
        /// 错误尝试次数
        /// </summary>
        public int RetryErrorTimes => this.m_retryTimes;

        /// <summary>
        /// 下载进度
        /// </summary>
        public float DownloadProgress => 1.0f - ((float)RemainDownloadAssetCount / TotalAssetCount);
    }
}