/********************************************************************
	created:  2018-9-19 15:47:36
	filename: HttpDownloader.cs
	author:	  songguangze@fotoable.com
	
	purpose:  基于BestHttp的下载系统
*********************************************************************/
using BestHTTP;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

namespace EngineCore
{
    public class HttpDownloader
    {
        //并行下载限制
        public const int MAX_REQUEST_LIMIT = 10;

        private int m_downloadAssetsCount = 0;
        private int m_downloadFinishCount = 0;

        private Action<List<string>> OnDownloadFinishCallback = null;
        private Action<float> OnDownloadProgressCallback = null;
        private Action<string, byte[]> OnDownloadedAssetsCallback = null;
        private Action<string> OnDownloadErrorCallback = (errorAssetName) => { };

        private int m_currentRequestCount = 0;

        private List<string> m_downloadErrorList = new List<string>();

        private Queue<string> m_waitingDownloadQueue = new Queue<string>();

        /// <summary>
        /// 从服务器下载大量文件
        /// </summary>
        /// <param name="assetsUrl"></param>
        /// <param name="OnDownloadedAssets"></param>
        /// <param name="DownloadProgress"></param>
        /// <param name="OnDownloadFinish"></param>
        /// <param name="forceSave"></param>
        public void DownloadMultipleFileFromServer(string[] assetsUrl, Action<string, byte[]> OnDownloadedAssets, Action<float> DownloadProgress = null, Action<List<string>> OnDownloadFinish = null, Action<string> OnDownloadError = null, bool forceSave = false)
        {
            this.OnDownloadFinishCallback = OnDownloadFinish;
            this.OnDownloadProgressCallback = DownloadProgress;
            this.OnDownloadedAssetsCallback = OnDownloadedAssets;
            this.OnDownloadErrorCallback += OnDownloadError;

            for (int i = 0; i < assetsUrl.Length; ++i)
            {
                if (i < MAX_REQUEST_LIMIT)
                    DownloadInternal(assetsUrl[i], forceSave);
                else
                    m_waitingDownloadQueue.Enqueue(assetsUrl[i]);
            }
        }


        private void DownloadInternal(string assetUrl, bool isSaveTopersistentDataPath)
        {
            if (!assetUrl.StartsWithFast("http://"))
                assetUrl = $"{ SysConf.GAME_RES_URL}/{ assetUrl}";

            this.m_currentRequestCount++;

            HTTPRequest downloadRequest = new HTTPRequest(new Uri(assetUrl), (req, resp) =>
            {
                string downloadedAssetName = Path.GetFileName(req.Uri.AbsolutePath);
                byte[] downloadedBuffer = new byte[0];
                switch (req.State)
                {
                    case HTTPRequestStates.Finished:
                        {
                            if (resp.IsSuccess)
                            {
                                downloadedBuffer = resp.Data;

                                if (isSaveTopersistentDataPath)
                                {
                                    string savePath = $"{PathResolver.ApplicationPersistentDataPath}/{downloadedAssetName}";
                                    EngineFileUtil.WriteBytesToFile(savePath, resp.Data);
                                }

                                OnDownloadedAsset(downloadedAssetName, downloadedBuffer);
                            }
                            else
                            {
                                Debug.LogWarning(string.Format("Request finished Successfully, but the server sent an error. Status Code: {0}-{1} Message: {2}",
                                                   resp.StatusCode,
                                                   resp.Message,
                                                   resp.DataAsText));
                            }
                        }
                        break;
                    case HTTPRequestStates.Error:
                        OnDownloadError(downloadedAssetName, "Request Finished with Error! " + (req.Exception != null ? (req.Exception.Message + "\n" + req.Exception.StackTrace) : "No Exception"));
                        break;
                    case HTTPRequestStates.Aborted:
                        OnDownloadError(downloadedAssetName, "Request Aborted!");
                        break;
                    case HTTPRequestStates.ConnectionTimedOut:
                        OnDownloadError(downloadedAssetName, "Connection Timed Out!");
                        break;
                    case HTTPRequestStates.TimedOut:
                        OnDownloadError(downloadedAssetName, "Processing the request Timed Out!");
                        break;
                }

                m_currentRequestCount--;

                if (this.m_waitingDownloadQueue.Count > 0)
                    DownloadInternal(this.m_waitingDownloadQueue.Dequeue(), isSaveTopersistentDataPath);
                else
                {
                    if (this.m_currentRequestCount == 0)
                        OnDownloadFinishCallback?.Invoke(this.m_downloadErrorList);
                }

            });
            downloadRequest.UseStreaming = false;
            downloadRequest.Send();
        }


        private void OnDownloadedAsset(string downloadAssetName, byte[] downloadedBuffer)
        {
            this.m_downloadFinishCount++;

            if (downloadedBuffer == null)
                this.m_downloadErrorList.Add(downloadAssetName);

            if (downloadedBuffer != null || downloadedBuffer.Length > 0)
                EngineCoreEvents.ResourceEvent.OnDownloadedAssetFromWebServer.SafeInvoke(downloadAssetName, downloadedBuffer);

            OnDownloadedAssetsCallback?.Invoke(downloadAssetName, downloadedBuffer);

            NotifyProgress();
        }

        private void OnDownloadError(string errorAssetName, string errorMessage)
        {
            this.m_downloadErrorList.Add(errorAssetName);
            this.OnDownloadErrorCallback?.Invoke(errorAssetName);

            EngineCoreEvents.ResourceEvent.OnDownloadAssetError.SafeInvoke(errorAssetName, errorMessage);
            Debug.LogError($"download file:{errorAssetName} error, errorMessage :{errorMessage}");
        }

        /// <summary>
        /// 从网络下载Texture
        /// </summary>
        /// <param name="textureFullName"></param>
        /// <param name="OnDowloadedTexture"></param>
        /// <param name="serverDomain"></param>
        public void DownloadTextureFromServer(string textureName, Action<Texture2D> OnDowloadedTexture, Action<string> OnDownloadError, string serverDomain = "")
        {
            string textureUrlWithDomain = $"{serverDomain}/{textureName}";
            if (string.IsNullOrEmpty(serverDomain))
                textureUrlWithDomain = $"{SysConf.GAME_RES_URL}/{textureName}";

            DownloadMultipleFileFromServer(new string[] { textureUrlWithDomain }, (texture, textureBuffer) =>
            {
                Texture2D downloadedTexture = Texture2D.blackTexture;
                if (textureBuffer.Length > 0)
                    downloadedTexture.LoadImage(textureBuffer);

                OnDowloadedTexture(downloadedTexture);
            }, null, null, OnDownloadError);
        }

        /// <summary>
        /// 从网络下载文本
        /// </summary>
        /// <param name="textName"></param>
        /// <param name="OnDowloadedTexture"></param>
        /// <param name="serverDomain"></param>
        public void DownloadTextFromServer(string textName, Action<string> OnDowloadedText, Action<string> OnDownloadError, string serverDomain = "")
        {
            if (!textName.StartsWithFast("http://"))
                textName = $"{ SysConf.GAME_RES_URL}{ textName}";

            DownloadMultipleFileFromServer(new string[] { textName }, (text, textBuffer) =>
            {
                string textContent = Encoding.UTF8.GetString(textBuffer);
                OnDowloadedText(textContent);
            }, null, null, OnDownloadError);
        }

        /// <summary>
        /// 从网络下载文件
        /// </summary>
        /// <param name="bufferFileName"></param>
        /// <param name="OnDowloadedBuffer"></param>
        /// <param name="serverDomain"></param>
        public void DownloadBufferFromServer(string bufferFileName, Action<byte[]> OnDowloadedBuffer, Action<string> OnDownloadError, string serverDomain = "")
        {
            string bufferFileUrlWithDomain = $"{serverDomain}/{bufferFileName}";
            if (!bufferFileUrlWithDomain.StartsWithFast("http://"))
                bufferFileUrlWithDomain = $"{SysConf.GAME_RES_URL}/{bufferFileName}";

            DownloadMultipleFileFromServer(new string[] { bufferFileUrlWithDomain }, (text, rawBuffer) =>
            {
                OnDowloadedBuffer(rawBuffer);
            }, null, null, OnDownloadError);
        }


        private void NotifyProgress()
        {
            OnDownloadProgressCallback?.Invoke((float)this.m_downloadFinishCount / this.m_downloadAssetsCount);
        }
    }
}