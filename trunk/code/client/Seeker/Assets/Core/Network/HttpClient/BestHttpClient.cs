/********************************************************************
	created:  2018-3-29 16:35:25
	filename: HttpClient.cs
	author:	  songguangze@outlook.com
	
	purpose:  Http客户端
*********************************************************************/
using BestHTTP;
using EngineCore.Utility;
using Google.Protobuf;
using SeekerGame;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using UnityEngine;

namespace EngineCore
{
    public class BestHttpClient
    {




        private static int S_ATOMIC_COUNTER = 0;

        public Action<string> OnErrorEvent = null;

        private string m_serverIp = string.Empty;
        //消息处理队列
        private Queue<ResponseMessageWrapper> m_messageQueue = new Queue<ResponseMessageWrapper>();


        private HttpRetry m_retry_wrapper;


        public BestHttpClient(string serverIp)
        {
            this.m_serverIp = serverIp;
            this.m_retry_wrapper = new HttpRetry();
            ResumeRetryMessage();
            m_is_retry_ui_show = false;
            HttpSendingCahce.Clear();
            HttpPairCahce.Clear();
            S_ATOMIC_COUNTER = 0;
            HTTPManager.MaxConnectionPerServer = 4;
        }

        public void SendMessage(RequestMessageWrapper message)
        {
            RawSend(message);
        }

        private bool m_is_retrying = false;
        private bool m_is_retry_ui_show = false;

        public void RetryMessage()
        {
            if (m_is_retrying)
                return;

            SuspendRetryMessage();


            //Debug.Log("update重试发送来了");

            if (m_retry_wrapper.IsPendingMsg())
            {
                if (!m_is_retry_ui_show)
                {
                    //弹出倒计时界面
                    EngineCoreEvents.SystemEvents.OnSendingSyncMsgCallback.SafeInvoke(0, NetworkModule.NetworkStatus.SYNC_SUCCEED);
                    EngineCoreEvents.SystemEvents.OnRetryingSyncMsg.SafeInvoke(0, NetworkModule.NetworkStatus.WAITTING_RETRY);

                    m_is_retry_ui_show = true;
                }
            }


            if (m_retry_wrapper.CheckTime())
            {

                RequestMessageWrapper retry_req = m_retry_wrapper.Dequeue();


                if (null != retry_req)
                {
                    RetryRawSend(retry_req);
                }
                else
                {
                    FinishRetryMessage();
                }
            }
            else
            {
                if (m_retry_wrapper.IsPendingMsg())
                {
                    //弹出确认提示
                    EngineCoreEvents.SystemEvents.OnRetryingSyncMsg.SafeInvoke(0, NetworkModule.NetworkStatus.RESET_RETRY_TIME);
                }
                else
                {
                    FinishRetryMessage();
                }
            }

        }

        public void DestroyRetryMessage()
        {
            SuspendRetryMessage();
            FinishRetryMessage();
            this.m_retry_wrapper.ClearQueue();
        }


        public void FinishRetryMessage()
        {
            EngineCore.EngineCoreEvents.SystemEvents.OnSendingSyncMsgCallback.SafeInvoke(0, NetworkModule.NetworkStatus.RETRY_CACHE_CLEAR);
            m_is_retry_ui_show = false;
            this.m_retry_wrapper.InitTime();
        }


        public void RestartRetryMessage()
        {
            m_is_retrying = false;
            this.m_retry_wrapper.InitTime();
        }


        public void ResumeRetryMessage()
        {
            m_is_retrying = false;
        }

        public void SuspendRetryMessage()
        {
            m_is_retrying = true;
        }

        /// <summary>
        /// 同步带重发
        /// </summary>
        /// <param name="sendMessageWrapper"></param>
        /// <param name="retry"></param>
        private void DoSyncSend(RequestMessageWrapper sendMessageWrapper, bool retry)
        {

            if (!retry)
                WorkQueueCallback(new SendMessageCallback()
                {
                    MessageId = sendMessageWrapper.MessageId,
                    Status = NetworkModule.NetworkStatus.WAITING_SYNC
                });


            CreateHttpRequest(sendMessageWrapper, OnSyncRequestFinished);
        }

        private void BornErrorLog(RequestMessageWrapper sendMessageWrapper)
        {
            //this.m_context.Post(WorkQueueCallback, new SendMessageCallback()
            //{
            //    MessageId = sendMessageWrapper.MessageId,
            //    Status = NetworkModule.NetworkStatus.LOG_ERROR
            //});

            WorkQueueCallback(new SendMessageCallback()
            {
                MessageId = sendMessageWrapper.MessageId,
                Status = NetworkModule.NetworkStatus.LOG_ERROR
            });

        }

        private void BornRetry(RequestMessageWrapper sendMessageWrapper)
        {
            TimeModule.Instance.SetTimeout(() =>
            {
                //Debug.Log("入列!!!");
                this.m_retry_wrapper.Enqueue(sendMessageWrapper);

                WorkQueueCallback(new SendMessageCallback()
                {
                    MessageId = sendMessageWrapper.MessageId,
                    Status = NetworkModule.NetworkStatus.RETRY_ONE_FINISHED
                });
            }, UnityEngine.Random.Range(3.0f, 5.0f)
            );


        }

        /// <summary>
        /// 异步无提示
        /// </summary>
        /// <param name="sendMessageWrapper"></param>
        private void DoASyncSend(RequestMessageWrapper sendMessageWrapper)
        {
            CreateHttpRequest(sendMessageWrapper, OnASyncRequestFinished);
        }

        /// <summary>
        /// 同步无重发，有断网提示
        /// </summary>
        /// <param name="sendMessageWrapper"></param>
        private void DoHalfSyncSend(RequestMessageWrapper sendMessageWrapper)
        {
            WorkQueueCallback(new SendMessageCallback()
            {
                MessageId = sendMessageWrapper.MessageId,
                Status = NetworkModule.NetworkStatus.WAITING_SYNC
            });

            CreateHttpRequest(sendMessageWrapper, OnHalfSyncRequestFinished);
        }

        private void CreateHttpRequest(RequestMessageWrapper sendMessageWrapper, OnRequestFinishedDelegate OnFinished_)
        {
            string serverTarget = MessageTargetDefine.GetServerUrl(sendMessageWrapper.MessageBody);
            if (string.IsNullOrEmpty(serverTarget))
                return;

            Uri serverUri = new Uri(EngineCoreEvents.BridgeEvent.GetServerAddress.SafeInvoke() + "/" + serverTarget);

            //<best
            BestHTTP.HTTPRequest request = new HTTPRequest(serverUri, HTTPMethods.Post, OnFinished_);
            request.IsKeepAlive = true;
            request.RawData = sendMessageWrapper.MessageBody.ToByteArray(); ;
            request.SetHeader("Content-Type", "application/x-protobuf");
            request.SetHeader("Accept", "application/x-protobuf");
            string accountToken = EngineCoreEvents.SystemEvents.FetchUserIdentification();
            if (!string.IsNullOrEmpty(accountToken))
                request.SetHeader("accessToken", accountToken);

            int send_identifier = ++S_ATOMIC_COUNTER;
            request.SetHeader("sendTime", send_identifier.ToString());

#if UNITY_DEBUG
            DateTime t1 = DateTime.Now;
            TimeSpan t22 = new TimeSpan(t1.Ticks);
            long time2 = Convert.ToInt64(t22.TotalMilliseconds);

            //Debug.LogWarning($"SENDING TIME :{request.Uri}  {t1.ToString("yyyy/MM/dd HH:mm:ss:fffffff")}");
            sendMessageWrapper.Send_time = time2;
            sendMessageWrapper.Send_timestamp = t1.ToString("yyyy/MM/dd HH:mm:ss:fffffff");
#else
            sendMessageWrapper.Send_time = CommonUtils.GetCurTimeMillSenconds();
#endif
            HttpSendingCahce.AddToSendingCache(send_identifier.ToString(), sendMessageWrapper);

            request.Send();
        }


        private void RetryRawSend(object state)
        {
            RawSendInternal(state, true);
        }

        private void RawSend(object state)
        {
            RawSendInternal(state, false);
        }





        private void RawSendInternal(object state, bool retry)
        {

            RequestMessageWrapper sendMessageWrapper = state as RequestMessageWrapper;
            sendMessageWrapper.Is_retry = retry;

            if (sendMessageWrapper.RequestType == RequestMessageType.SYNC)
            {
                DoSyncSend(sendMessageWrapper, retry);
            }
            else if (sendMessageWrapper.RequestType == RequestMessageType.HALF_SYNC)
            {
                DoHalfSyncSend(sendMessageWrapper);
            }
            else
            {
                DoASyncSend(sendMessageWrapper);
            }
            //>
        }





        private void OnSyncRequestFinished(HTTPRequest originalRequest, HTTPResponse response)
        {

            RequestMessageWrapper sendMessageWrapper = HttpSendingCahce.GetAndRemoveFromSendingCache(originalRequest);

            if (null == sendMessageWrapper)
            {
                Debug.LogError($"BEST HTTP : return unknown rsp {response.GetFirstHeaderValue("msgId")}");
                return;
            }

            switch (originalRequest.State)
            {
                case HTTPRequestStates.Finished:
                    //Debug.Log("Request Finished Successfully!\n" + response.DataAsText);
                    OnRsp(sendMessageWrapper, response);

                    if (!sendMessageWrapper.Is_retry)
                        //this.m_context.Post(WorkQueueCallback, new SendMessageCallback()
                        //{
                        //    MessageId = sendMessageWrapper.MessageId,
                        //    Status = NetworkModule.NetworkStatus.SYNC_SUCCEED
                        //});
                        WorkQueueCallback(new SendMessageCallback()
                        {
                            MessageId = sendMessageWrapper.MessageId,
                            Status = NetworkModule.NetworkStatus.SYNC_SUCCEED
                        });
                    else
                        //this.m_context.Post(WorkQueueCallback, new SendMessageCallback()
                        //{
                        //    MessageId = sendMessageWrapper.MessageId,
                        //    Status = NetworkModule.NetworkStatus.RETRY_ONE_FINISHED
                        //});

                        WorkQueueCallback(new SendMessageCallback()
                        {
                            MessageId = sendMessageWrapper.MessageId,
                            Status = NetworkModule.NetworkStatus.RETRY_ONE_FINISHED
                        });

                    break;
                case HTTPRequestStates.Error:
                    {
                        Debug.LogError("BEST HTTP :Request Finished with Error! " + (originalRequest.Exception != null ? (originalRequest.Exception.Message + "\n" + originalRequest.Exception.StackTrace) : "No Exception"));
                        BornRetry(sendMessageWrapper);
                    }
                    break;
                case HTTPRequestStates.Aborted:
                    Debug.LogWarning("BEST HTTP :Request Aborted!");
                    BornRetry(sendMessageWrapper);
                    break;
                case HTTPRequestStates.ConnectionTimedOut:
                    Debug.LogError("BEST HTTP :Connection Timed Out!");
                    BornRetry(sendMessageWrapper);
                    break;
                case HTTPRequestStates.TimedOut:
                    Debug.LogError("BEST HTTP :Processing the request Timed Out!");
                    BornRetry(sendMessageWrapper);
                    break;
                default:
                    break;
            }



        }

        private void OnHalfSyncRequestFinished(HTTPRequest originalRequest, HTTPResponse response)
        {


            RequestMessageWrapper sendMessageWrapper = HttpSendingCahce.GetAndRemoveFromSendingCache(originalRequest);
            if (null == sendMessageWrapper)
            {
                Debug.LogError($"BEST HTTP : return unknown rsp {response.GetFirstHeaderValue("msgId")}");
                return;
            }
            switch (originalRequest.State)
            {
                case HTTPRequestStates.Finished:
                    //Debug.Log("Request Finished Successfully!\n" + response.DataAsText);
                    OnRsp(sendMessageWrapper, response);

                    WorkQueueCallback(new SendMessageCallback()
                    {
                        MessageId = sendMessageWrapper.MessageId,
                        Status = NetworkModule.NetworkStatus.SYNC_SUCCEED
                    });
                    break;
                case HTTPRequestStates.Error:
                    {
                        Debug.LogError("Request Finished with Error! " + (originalRequest.Exception != null ? (originalRequest.Exception.Message + "\n" + originalRequest.Exception.StackTrace) : "No Exception"));
                        WorkQueueCallback(new SendMessageCallback()
                        {
                            MessageId = sendMessageWrapper.MessageId,
                            Status = NetworkModule.NetworkStatus.OFFLINE_WARNNING
                        });
                    }
                    break;
                case HTTPRequestStates.Aborted:
                    Debug.LogWarning("BEST HTTP :Request Aborted!");
                    WorkQueueCallback(new SendMessageCallback()
                    {
                        MessageId = sendMessageWrapper.MessageId,
                        Status = NetworkModule.NetworkStatus.OFFLINE_WARNNING
                    });
                    break;
                case HTTPRequestStates.ConnectionTimedOut:
                    Debug.LogError("BEST HTTP :Connection Timed Out!");
                    WorkQueueCallback(new SendMessageCallback()
                    {
                        MessageId = sendMessageWrapper.MessageId,
                        Status = NetworkModule.NetworkStatus.OFFLINE_WARNNING
                    });
                    break;
                case HTTPRequestStates.TimedOut:
                    Debug.LogError("BEST HTTP :Processing the request Timed Out!");
                    WorkQueueCallback(new SendMessageCallback()
                    {
                        MessageId = sendMessageWrapper.MessageId,
                        Status = NetworkModule.NetworkStatus.OFFLINE_WARNNING
                    });
                    break;
                default:
                    break;
            }



        }

        private void OnASyncRequestFinished(HTTPRequest originalRequest, HTTPResponse response)
        {

            RequestMessageWrapper sendMessageWrapper = HttpSendingCahce.GetAndRemoveFromSendingCache(originalRequest);
            if (null == sendMessageWrapper)
            {
                Debug.LogError($"BEST HTTP : return unknown rsp {response.GetFirstHeaderValue("msgId")}");
                return;
            }
            switch (originalRequest.State)
            {
                case HTTPRequestStates.Finished:
                    //Debug.Log("Request Finished Successfully!\n" + response.DataAsText);
                    OnRsp(sendMessageWrapper, response);
                    break;
                case HTTPRequestStates.Error:
                    Debug.LogError("BEST HTTP :Request Finished with Error! " + (originalRequest.Exception != null ? (originalRequest.Exception.Message + "\n" + originalRequest.Exception.StackTrace) : "No Exception"));
                    BornErrorLog(sendMessageWrapper);
                    break;
                case HTTPRequestStates.Aborted:
                    Debug.LogWarning("BEST HTTP :Request Aborted!");
                    BornErrorLog(sendMessageWrapper);
                    break;
                case HTTPRequestStates.ConnectionTimedOut:
                    Debug.LogError("BEST HTTP :Connection Timed Out!");
                    BornErrorLog(sendMessageWrapper);
                    break;
                case HTTPRequestStates.TimedOut:
                    Debug.LogError("BEST HTTP :Processing the request Timed Out!");
                    BornErrorLog(sendMessageWrapper);
                    break;
            }

        }


        private void OnRsp(RequestMessageWrapper sendMessageWrapper, HTTPResponse response)
        {

            if (response.GetFirstHeaderValue("msgId") == null)
            {
                Debug.Log("BEST HTTP :error message no message id on response header");
                return;
            }
            else
            {
                int messageId = int.Parse(response.GetFirstHeaderValue("msgId"));
                if (messageId == 400)
                {
                    GOEngine.DebugUtil.Log($"BEST HTTP :server error == 400  ,server ip:{this.m_serverIp}");
                }

                byte[] buffer = response.Data;

                if (!string.IsNullOrEmpty(response.GetFirstHeaderValue("errorCode")))
                {
                    int errorCode = int.Parse(response.GetFirstHeaderValue("errorCode"));
                    string errorContent = Encoding.UTF8.GetString(buffer);

                    EngineCoreEvents.SystemEvents.NetWorkError.SafeInvoke(errorCode, errorContent);
                }
                else
                {

                    IMessage message = EngineCore.MessageParser.Parse(messageId, buffer);

                    this.m_messageQueue.Enqueue(new ResponseMessageWrapper(messageId, message));


                    HttpPairCahce.EnCache(sendMessageWrapper.MessageBody);

                }
            }
        }


        /// <summary>
        /// SubThread to mainthread
        /// </summary>
        /// <param name="callbackParams"></param>
        private void WorkQueueCallback(object callbackParams)
        {
            SendMessageCallback sendingStatus = callbackParams as SendMessageCallback;
            if (sendingStatus.Status == NetworkModule.NetworkStatus.TIMEOUT)
            {
                OnlyErrorNoContent();
                EngineCoreEvents.SystemEvents.OnSendingSyncMsgCallback.SafeInvoke(sendingStatus.MessageId, NetworkModule.NetworkStatus.TIMEOUT);
            }
            else if (sendingStatus.Status == NetworkModule.NetworkStatus.WAITING_SYNC)
                EngineCoreEvents.SystemEvents.OnSendingSyncMsg.SafeInvoke(sendingStatus.MessageId);
            else if (sendingStatus.Status == NetworkModule.NetworkStatus.SYNC_SUCCEED)
                EngineCoreEvents.SystemEvents.OnSendingSyncMsgCallback.SafeInvoke(sendingStatus.MessageId, NetworkModule.NetworkStatus.SYNC_SUCCEED);
            else if (sendingStatus.Status == NetworkModule.NetworkStatus.RETRY_ONE_FINISHED)
                ResumeRetryMessage();
            else if (sendingStatus.Status == NetworkModule.NetworkStatus.LOG_ERROR)
            {
                EngineCore.EngineCoreEvents.UIEvent.HideUIEvent.SafeInvoke(UIDefine.UI_SYNC_LOADING);
                OnlyErrorNoContent();
            }
            else if (sendingStatus.Status == NetworkModule.NetworkStatus.OFFLINE_WARNNING)
            {
                EngineCoreEvents.SystemEvents.OnSendingSyncMsgCallback.SafeInvoke(sendingStatus.MessageId, NetworkModule.NetworkStatus.OFFLINE_WARNNING);
                EngineCore.EngineCoreEvents.UIEvent.HideUIEvent.SafeInvoke(UIDefine.UI_SYNC_LOADING);
            }

        }


        private void OnlyErrorNoContent()
        {
            OnErrorEvent?.Invoke("");
        }

        public Queue<ResponseMessageWrapper> MessageQueue
        {
            get { return this.m_messageQueue; }
        }

    }


}
