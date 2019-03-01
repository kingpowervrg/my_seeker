#define SYNC
#define BEST_HTTP

#if !BEST_HTTP
/********************************************************************
	created:  2018-3-29 16:35:25
	filename: HttpClient.cs
	author:	  songguangze@outlook.com
	
	purpose:  Http客户端
*********************************************************************/
using EngineCore.Utility;
using Google.Protobuf;
using SeekerGame;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using UnityEngine;

namespace EngineCore
{
    public class HttpClient
    {
        enum IP_TYPE_ENUM
        {
            E_IP_NONE,
            E_IPV4,
            E_IPV6,
        }

        public Action<string> OnErrorEvent = null;

        private string m_serverIp = string.Empty;
        //消息处理队列
        private ThreadSafeQueue<ResponseMessageWrapper> m_messageQueue = new ThreadSafeQueue<ResponseMessageWrapper>();

        //线程同步上下文
        private SynchronizationContext m_context;


        private HttpRetry m_retry_wrapper;


        private Dictionary<string, long> m_sending_times;
        private int m_atomic_sending_counter;

        public HttpClient(string serverIp)
        {
            this.m_serverIp = serverIp;
            this.m_context = SynchronizationContext.Current;
            this.m_retry_wrapper = new HttpRetry();
            ResumeRetryMessage();
            m_is_retry_ui_show = false;
            m_sending_times = new Dictionary<string, long>();
            m_atomic_sending_counter = 0;
        }

        public void SendMessage(RequestMessageWrapper message)
        {
            ThreadPool.QueueUserWorkItem(T_RawSend, message);
        }

        private bool m_is_retrying = false;
        private bool m_is_retry_ui_show = false;

        public ThreadSafeQueue<RequestMessageWrapper> GetRetryQueue()
        {
            return m_retry_wrapper.GetRetryQueue();
        }


        //public void RetryMessage(RequestMessageWrapper message)
        //{

        //    if (!m_is_retry_ui_show)
        //    {
        //        //弹出倒计时界面
        //        EngineCoreEvents.SystemEvents.OnSendingSyncMsgCallback.SafeInvoke(0, NetworkModule.NetworkStatus.SYNC_SUCCEED);
        //        EngineCoreEvents.SystemEvents.OnRetryingSyncMsg.SafeInvoke(0, NetworkModule.NetworkStatus.WAITTING_RETRY);
        //        m_is_retry_ui_show = true;
        //    }


        //    ThreadPool.QueueUserWorkItem(T_RetryRawSend, message);

        //    //if (!m_retry_wrapper.CheckTime())
        //    //{
        //    //    if (m_retry_wrapper.IsPendingMsg())
        //    //    {
        //    //        //弹出确认提示
        //    //        EngineCoreEvents.SystemEvents.OnRetryingSyncMsg.SafeInvoke(0, NetworkModule.NetworkStatus.RESET_RETRY_TIME);
        //    //    }
        //    //    else
        //    //    {
        //    //        FinishRetryMessage();
        //    //    }
        //    //}

        //}

        public void RetryMessage()
        {
            if (m_is_retrying)
                return;

            SuspendRetryMessage();


            Debug.Log("update重试发送来了");

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
                    ThreadPool.QueueUserWorkItem(T_RetryRawSend, retry_req);
                    //ThreadPool.QueueUserWorkItem(T_RawSend, retry_req);
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

        //private void LogIpType(IP_TYPE_ENUM type_)
        //{
        //    Dictionary<UBSParamKeyName, object> _param = new Dictionary<UBSParamKeyName, object>();
        //    _param.Add(UBSParamKeyName.Description, IP_TYPE_ENUM.E_IPV6 == type_ ? 6 : 4);
        //    UserBehaviorStatisticsModules.Instance.LogEvent(UBSEventKeyName.ip_type, null, _param);
        //}

        private void T_DoSyncSend(RequestMessageWrapper sendMessageWrapper, HttpWebRequest request, bool retry)
        {
            if (!retry)
                this.m_context.Post(WorkQueueCallback, new SendMessageCallback()
                {
                    MessageId = sendMessageWrapper.MessageId,
                    Status = NetworkModule.NetworkStatus.WAITING_SYNC
                });



            byte[] requestData = sendMessageWrapper.MessageBody.ToByteArray();
            long send_time;
            long delta_time;
            try
            {
                send_time = CommonUtils.GetCurTimeMillSenconds();

                using (Stream stream = request.GetRequestStream())
                {
                    request.GetRequestStream().Write(requestData, 0, requestData.Length);
                }

            }
            catch (Exception e)
            {
                Debug.LogError(string.Format("reqing msg id :{0} - {1}", sendMessageWrapper.MessageId, e.ToString()));
                BornRetry(sendMessageWrapper);

                return;
            }

            try
            {


                HttpWebResponse response = (HttpWebResponse)request.GetResponse();

                if (!retry)
                    this.m_context.Post(WorkQueueCallback, new SendMessageCallback()
                    {
                        MessageId = sendMessageWrapper.MessageId,
                        Status = NetworkModule.NetworkStatus.SYNC_SUCCEED
                    });
                else
                    this.m_context.Post(WorkQueueCallback, new SendMessageCallback()
                    {
                        MessageId = sendMessageWrapper.MessageId,
                        Status = NetworkModule.NetworkStatus.RETRY_ONE_FINISHED
                    });
                using (Stream responseStream = response.GetResponseStream())
                {
                    int responseBufferLength = (int)response.ContentLength;

                    if (response.Headers["msgId"] == null)
                    {
                        Debug.Log("error message no message id on response header");
                        return;
                    }
                    else
                    {
                        int messageId = int.Parse(response.Headers["msgId"]);
                        if (messageId == 400)
                        {
                            GOEngine.DebugUtil.Log($"server error == 400  ,server ip:{this.m_serverIp}");
                        }

                        byte[] buffer = new byte[responseBufferLength];
                        responseStream.Read(buffer, 0, responseBufferLength);

                        delta_time = CommonUtils.GetCurTimeMillSenconds() - send_time;

                        if (delta_time > 500)
                            Debug.LogWarning($"C# HTTP SYNC : cur msg {request.RequestUri.ToString()} total cost {delta_time} ms");

                        if (!string.IsNullOrEmpty(response.Headers["errorCode"]))
                        {
                            int errorCode = int.Parse(response.Headers["errorCode"]);
                            string errorContent = Encoding.UTF8.GetString(buffer);

                            EngineCoreEvents.SystemEvents.NetWorkError.SafeInvoke(errorCode, errorContent);
                        }
                        else
                        {

                            IMessage message = EngineCore.MessageParser.Parse(messageId, buffer);

                            this.m_messageQueue.Enqueue(new ResponseMessageWrapper(messageId, message));
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Debug.LogError(string.Format("rsping msg id :{0} - {1}", sendMessageWrapper.MessageId, e.ToString()));

                BornRetry(sendMessageWrapper);
                return;
            }

        }

        private void BornRetry(RequestMessageWrapper sendMessageWrapper)
        {
            Debug.Log("产生重新发送!!!睡眠");
            Debug.Log(HttpRetry.GetCurrentTimeSecond());
            Thread.Sleep(3000);
            Debug.Log("产生重新发送!!!睡眠结束");
            Debug.Log(HttpRetry.GetCurrentTimeSecond());

            Debug.Log("入列!!!");
            this.m_retry_wrapper.Enqueue(sendMessageWrapper);

            this.m_context.Post(WorkQueueCallback, new SendMessageCallback()
            {
                MessageId = sendMessageWrapper.MessageId,
                Status = NetworkModule.NetworkStatus.RETRY_ONE_FINISHED
            });
        }

        private class MyCallbackParam
        {
            public RequestMessageWrapper State;
            public HttpWebRequest Request;
        }

        private void AsyncWriteRequestStream(IAsyncResult ar)
        {
            // 取出回调前的状态参数
            MyCallbackParam cp = (MyCallbackParam)ar.AsyncState;
            RequestMessageWrapper sendMessageWrapper = cp.State;

            byte[] requestData = sendMessageWrapper.MessageBody.ToByteArray();

            try
            {
                // 结束写入数据的操作
                using (Stream bw = cp.Request.EndGetRequestStream(ar))
                {
                    bw.Write(requestData, 0, requestData.Length);
                }

                // 开始异步向服务器发起请求
                cp.Request.BeginGetResponse(GetResponseCallback, cp);
            }
            catch (Exception e)
            {
                //Debug.LogError(string.Format("rsping msg id :{0} - {1}", sendMessageWrapper.MessageId, e.ToString()));

                this.m_context.Post(WorkQueueCallback, new SendMessageCallback()
                {
                    MessageId = sendMessageWrapper.MessageId,
                    Status = NetworkModule.NetworkStatus.LOG_ERROR
                });
            }
        }

        private void GetResponseCallback(IAsyncResult ar)
        {
            // 取出回调前的状态参数
            MyCallbackParam cp = (MyCallbackParam)ar.AsyncState;
            RequestMessageWrapper sendMessageWrapper = cp.State as RequestMessageWrapper;

            long start_time = m_sending_times[sendMessageWrapper.m_unique_identifier];
            m_sending_times.Remove(sendMessageWrapper.m_unique_identifier);

            try
            {
                // 读取服务器的响应
                using (HttpWebResponse response = (HttpWebResponse)cp.Request.EndGetResponse(ar))
                {
                    using (Stream responseStream = response.GetResponseStream())
                    {
                        int responseBufferLength = (int)response.ContentLength;

                        if (response.Headers["msgId"] == null)
                        {
                            Debug.Log("error message no message id on response header");
                            return;
                        }
                        else
                        {
                            int messageId = int.Parse(response.Headers["msgId"]);
                            if (messageId == 400)
                            {
                                GOEngine.DebugUtil.Log($"server error == 400  ,server ip:{this.m_serverIp}");
                            }

                            else if (messageId == 1)
                            {
                                GOEngine.DebugUtil.Log($"server error == 1  ,remote login");
                            }

                            byte[] buffer = new byte[responseBufferLength];
                            responseStream.Read(buffer, 0, responseBufferLength);

                            if (!string.IsNullOrEmpty(response.Headers["errorCode"]))
                            {
                                int errorCode = int.Parse(response.Headers["errorCode"]);
                                string errorContent = Encoding.UTF8.GetString(buffer);

                                EngineCoreEvents.SystemEvents.NetWorkError.SafeInvoke(errorCode, errorContent);
                            }
                            else
                            {

                                long delta_time = CommonUtils.GetCurTimeMillSenconds() - start_time;

                                Debug.LogWarning($"C# HTTP ASYNC : cur msg {sendMessageWrapper.MessageId} total cost {delta_time} ms");

                                IMessage message = EngineCore.MessageParser.Parse(messageId, buffer);

                                this.m_messageQueue.Enqueue(new ResponseMessageWrapper(messageId, message));
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                //Debug.LogError(string.Format("rsping msg id :{0} - {1}", sendMessageWrapper.MessageId, e.ToString()));

                this.m_context.Post(WorkQueueCallback, new SendMessageCallback()
                {
                    MessageId = sendMessageWrapper.MessageId,
                    Status = NetworkModule.NetworkStatus.LOG_ERROR
                });
            }
        }

        private void T_DoASyncSend(RequestMessageWrapper sendMessageWrapper, HttpWebRequest request)
        {


            MyCallbackParam cp = new MyCallbackParam()
            {
                State = sendMessageWrapper,
                Request = request,
            };

            request.BeginGetRequestStream(AsyncWriteRequestStream, cp);
        }

        private void ForceIpv6(HttpWebRequest request)
        {
            request.ServicePoint.BindIPEndPointDelegate = (servicePount, remoteEndPoint, retryCount) =>
            {
                if (remoteEndPoint.AddressFamily == System.Net.Sockets.AddressFamily.InterNetworkV6)
                {
                    return new IPEndPoint(IPAddress.IPv6Any, 0);
                }

                return new IPEndPoint(IPAddress.Any, 0);


            };
        }

        private void T_RetryRawSend(object state)
        {
            T_RawSendInternal(state, true);
        }

        private void T_RawSend(object state)
        {
            T_RawSendInternal(state, false);
        }


        private void T_RawSendInternal(object state, bool retry)
        {

            RequestMessageWrapper sendMessageWrapper = state as RequestMessageWrapper;
            if (!retry && sendMessageWrapper.RequestType == RequestMessageType.SYNC)
            {
                Debug.Log(string.Format("{0} retry count left {1}", sendMessageWrapper.MessageBody.Descriptor.Name, sendMessageWrapper.m_retry_count));
            }
            string serverTarget = MessageTargetDefine.GetServerUrl(sendMessageWrapper.MessageBody);
            if (string.IsNullOrEmpty(serverTarget))
                return;

            Uri serverUri = new Uri(this.m_serverIp + "/" + serverTarget);
            HttpWebRequest request = null;
            if (serverUri.Scheme == "https")
            {
                ServicePointManager.ServerCertificateValidationCallback = (a, b, c, d) => { return true; };
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;//配置协议 4.6 好像默认不设SecurityProtocolType.Ssl3
                request = WebRequest.Create(serverUri) as HttpWebRequest;
                request.ProtocolVersion = HttpVersion.Version10;
            }
            else
                request = WebRequest.Create(serverUri) as HttpWebRequest;

            request.Timeout = 15000;
            //request.ReadWriteTimeout = 10000;
            request.ContentType = "application/x-protobuf";
            request.Accept = "application/x-protobuf";
            request.Method = "POST";
            //ForceIpv6(request);

            string accountToken = EngineCoreEvents.SystemEvents.FetchUserIdentification();
            if (!string.IsNullOrEmpty(accountToken))
                request.Headers["accessToken"] = accountToken;

            //byte[] requestData = sendMessageWrapper.MessageBody.ToByteArray();
            if (sendMessageWrapper.RequestType == RequestMessageType.SYNC)
            {
                T_DoSyncSend(sendMessageWrapper, request, retry);
            }
            else
            {
                sendMessageWrapper.m_unique_identifier = (++m_atomic_sending_counter).ToString();
                m_sending_times.Add(sendMessageWrapper.m_unique_identifier, CommonUtils.GetCurTimeMillSenconds());

                T_DoASyncSend(sendMessageWrapper, request);
            }

        }



        /// <summary>
        /// 接收异步响应
        /// </summary>
        /// <param name="asyncResult"></param>
        private void ReceiveResponseAsync(IAsyncResult asyncResult)
        {
            RequestContext requestContext = asyncResult.AsyncState as RequestContext;
            HttpWebResponse response = requestContext.request.EndGetResponse(asyncResult) as HttpWebResponse;
            if (requestContext.requestType == RequestMessageType.SYNC)
            {
                this.m_context.Post(WorkQueueCallback, new SendMessageCallback()
                {
                    MessageId = requestContext.MessageId,
                    Status = NetworkModule.NetworkStatus.SYNC_SUCCEED
                });
            }


            using (Stream responseStream = response.GetResponseStream())
            {
                int responseBufferLength = (int)response.ContentLength;

                if (response.Headers["msgId"] == null)
                {
                    Debug.Log("error message no message id on response header");
                    return;
                }
                else
                {
                    int messageId = int.Parse(response.Headers["msgId"]);
                    if (messageId == 400)
                    {
                        GOEngine.DebugUtil.Log($"server error == 400  ,server ip:{this.m_serverIp}");
                    }

                    else if (messageId == 1)
                    {
                        GOEngine.DebugUtil.Log($"server error == 1  ,remote login");
                    }

                    byte[] buffer = new byte[responseBufferLength];
                    responseStream.Read(buffer, 0, responseBufferLength);

                    if (!string.IsNullOrEmpty(response.Headers["errorCode"]))
                    {
                        int errorCode = int.Parse(response.Headers["errorCode"]);
                        string errorContent = Encoding.UTF8.GetString(buffer);

                        EngineCoreEvents.SystemEvents.NetWorkError.SafeInvoke(errorCode, errorContent);
                    }
                    else
                    {

                        IMessage message = EngineCore.MessageParser.Parse(messageId, buffer);

                        this.m_messageQueue.Enqueue(new ResponseMessageWrapper(messageId, message));
                    }
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

        }

        private void ErrorQueueCallback(object val)
        {
            if (null != OnErrorEvent)
            {
                string error_str = (string)val;
                OnErrorEvent(error_str);
            }
        }

        private void OnlyErrorNoContent()
        {
            OnErrorEvent?.Invoke("");
        }

        public ThreadSafeQueue<ResponseMessageWrapper> MessageQueue
        {
            get { return this.m_messageQueue; }
        }




        /// <summary>
        /// 请求上下文
        /// </summary>
        private class RequestContext
        {
            public HttpWebRequest request;
            public RequestMessageType requestType;
            public int MessageId;
        }


    }


}
#endif