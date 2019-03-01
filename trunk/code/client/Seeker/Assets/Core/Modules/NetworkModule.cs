#define BEST_HTTP
/********************************************************************
	created:  2018-3-27 14:5:30
	filename: NetworkModule.cs
	author:	  songguangze@outlook.com
	
	purpose:  网络Moudle
*********************************************************************/

using Google.Protobuf;
using SeekerGame;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace EngineCore
{
    [EngineCoreModuleAttribute(EngineCore.ModuleType.NETWORK_MODULE)]
    public class NetworkModule : AbstractModule
    {
#if BEST_HTTP
        private BestHttpClient m_httpClient = null;
#else
        private HttpClient m_httpClient = null;
#endif

        private static NetworkModule m_instance;

        private NetworkStatus m_networkStatus = NetworkStatus.NORMAL;

        private SafeAction OnRetryConnect;

        public NetworkModule()
        {
            AutoStart = true;
            m_instance = this;
        }

        public override void Start()
        {
            base.Start();

            GameEvents.NetWorkEvents.SendMsg += SendSyncMessage;
            GameEvents.NetWorkEvents.SendHalfSyncMsg += SendHalfSyncMessage;
            GameEvents.NetWorkEvents.SendAsyncMsg += SendAsyncMessage;
            EngineCoreEvents.SystemEvents.RetryPendingMsgs += RetryPendingMessages;
            EngineCoreEvents.SystemEvents.OnEnableRetry += EnableRetry;
            EngineCoreEvents.SystemEvents.GetRspPairReq += PairOfReq;
            EngineCoreEvents.SystemEvents.Listen_SendingCostTimeMS += LogSendingCostTime;

            EnableRetry(true);
        }

        /// <summary>
        /// 发送异步消息
        /// </summary>
        /// <param name="msg"></param>
        private void SendAsyncMessage(IMessage msg)
        {
            if (GlobalInfo.GAME_NETMODE == GameNetworkMode.Standalone && msg.GetMessageId() != MessageDefine.CSRegGuestRequest)
            {
                MessageStandalonHandler.Call(msg);
                return;
            }
            if (string.IsNullOrEmpty(EngineCoreEvents.BridgeEvent.GetServerAddress()))
                Debug.LogError("no server ip");
            else
            {
                if (this.m_httpClient == null)
                {
#if BEST_HTTP
                    this.m_httpClient = new BestHttpClient(EngineCoreEvents.BridgeEvent.GetServerAddress());
#else
                    this.m_httpClient = new HttpClient(EngineCoreEvents.BridgeEvent.GetServerAddress());
#endif
                    this.m_httpClient.OnErrorEvent = CommonTools.OnHttpError;
                }

                SendMessageInternal(new RequestMessageWrapper(msg.GetMessageId(), msg, RequestMessageType.ASYNC));
            }
        }

        /// <summary>
        /// 发送同步消息
        /// </summary>
        /// <param name="msg"></param>
        private void SendSyncMessage(IMessage msg)
        {
            int messageId = msg.GetMessageId();
            if (messageId == 0)
                Debug.LogError(msg.GetType().ToString() + " no message Id");
            else
            {
                if (GlobalInfo.GAME_NETMODE == GameNetworkMode.Standalone)
                {
                    MessageStandalonHandler.Call(msg);
                    return;
                }
                if (this.m_httpClient == null)
#if BEST_HTTP
                    this.m_httpClient = new BestHttpClient(EngineCoreEvents.BridgeEvent.GetServerAddress());
#else
                    this.m_httpClient = new HttpClient(EngineCoreEvents.BridgeEvent.GetServerAddress());
#endif

                SendMessageInternal(new RequestMessageWrapper(msg.GetMessageId(), msg, RequestMessageType.SYNC));
            }

        }



        private void SendHalfSyncMessage(IMessage msg)
        {
            int messageId = msg.GetMessageId();
            if (messageId == 0)
                Debug.LogError(msg.GetType().ToString() + " no message Id");
            else
            {
                if (GlobalInfo.GAME_NETMODE == GameNetworkMode.Standalone)
                {
                    MessageStandalonHandler.Call(msg);
                    return;
                }
                if (this.m_httpClient == null)
#if BEST_HTTP
                    this.m_httpClient = new BestHttpClient(EngineCoreEvents.BridgeEvent.GetServerAddress());
#else
                    this.m_httpClient = new HttpClient(EngineCoreEvents.BridgeEvent.GetServerAddress());
#endif

                SendMessageInternal(new RequestMessageWrapper(msg.GetMessageId(), msg, RequestMessageType.HALF_SYNC));
            }

        }

        private void SendMessageInternal(RequestMessageWrapper message)
        {
            if (this.m_networkStatus == NetworkStatus.TIMEOUT)
                this.m_httpClient = null;
            else if (this.m_networkStatus == NetworkStatus.WAITING_SYNC)
            {
                Debug.Log("waiting sync response");
                return;
            }
            else
            {
                if (string.IsNullOrEmpty(EngineCoreEvents.BridgeEvent.GetServerAddress()))
                    Debug.LogError("no server ip");
                else
                {

                    if (this.m_httpClient == null)
#if BEST_HTTP
                        this.m_httpClient = new BestHttpClient(EngineCoreEvents.BridgeEvent.GetServerAddress());
#else
                        this.m_httpClient = new HttpClient(EngineCoreEvents.BridgeEvent.GetServerAddress());
#endif

#if UNITY_DEBUG
                    if (1901 != message.MessageId)
                        Debug.Log($"send message :{message.MessageBody.Descriptor.Name}");
#endif

                    this.m_httpClient.SendMessage(message);
                }
            }
        }

        private void RetryPendingMessages(bool retry)
        {
            if (!retry)
            {
                if (this.m_httpClient == null)
                    return;

                this.m_httpClient.DestroyRetryMessage();
                return;
            }

            if (this.m_httpClient == null)
#if BEST_HTTP
                this.m_httpClient = new BestHttpClient(EngineCoreEvents.BridgeEvent.GetServerAddress());
#else
                this.m_httpClient = new HttpClient(EngineCoreEvents.BridgeEvent.GetServerAddress());
#endif

            this.m_httpClient.RestartRetryMessage();
        }

        public void EnableRetry(bool val_)
        {
            if (val_)
                this.OnRetryConnect += UpdateRetry;
            else
                this.OnRetryConnect -= UpdateRetry;
        }

        private IMessage PairOfReq()
        {
            return HttpPairCahce.PeekCache();
        }

        private void LogSendingCostTime(string sendTime_, string receiveTime_, long delta_time_ms, string URL_, byte state_)
        {
            Dictionary<UBSParamKeyName, object> _param = new Dictionary<UBSParamKeyName, object>();

            if (!string.IsNullOrEmpty(sendTime_))
                _param.Add(UBSParamKeyName.send, sendTime_);
            if (!string.IsNullOrEmpty(receiveTime_))
                _param.Add(UBSParamKeyName.reveive, receiveTime_);
            _param.Add(UBSParamKeyName.Description, URL_);
            _param.Add(UBSParamKeyName.ContentType, state_);
            _param.Add(UBSParamKeyName.NumItems, delta_time_ms);
            UserBehaviorStatisticsModules.Instance.LogEvent(UBSEventKeyName.send_msg_time, null, _param);
        }

        private void UpdateRetry()
        {
            this.m_httpClient.RetryMessage();
        }
        public override void Update()
        {
            if (this.m_httpClient != null)
            {
                ResponseMessageWrapper messageWrapper;
#if BEST_HTTP
                while (this.m_httpClient.MessageQueue.Count > 0)
                {
                    messageWrapper = this.m_httpClient.MessageQueue.Dequeue();

#if UNITY_DEBUG
                    if (1902 != messageWrapper.MessageId)
                        Debug.Log($"recv message : { messageWrapper.MessageBody.Descriptor.Name},detail{messageWrapper.MessageBody.ToString()}");
#endif
                    MessageHandler.Call(messageWrapper.MessageId, messageWrapper.MessageBody);
                    HttpPairCahce.DeCache();
                }

#else
                while (this.m_httpClient.MessageQueue.Dequeue(out messageWrapper))
                {
#if UNITY_DEBUG
                    Debug.Log($"recv message : { messageWrapper.MessageBody.Descriptor.Name},detail{messageWrapper.MessageBody.ToString()}");
#endif
                    MessageHandler.Call(messageWrapper.MessageId, messageWrapper.MessageBody);
                }
#endif

                this.OnRetryConnect.SafeInvoke();

            }
        }

        public override void Dispose()
        {
            GameEvents.NetWorkEvents.SendMsg -= SendSyncMessage;
            GameEvents.NetWorkEvents.SendHalfSyncMsg -= SendHalfSyncMessage;
            GameEvents.NetWorkEvents.SendAsyncMsg -= SendAsyncMessage;
            EngineCoreEvents.SystemEvents.RetryPendingMsgs -= RetryPendingMessages;
            EngineCoreEvents.SystemEvents.OnEnableRetry -= EnableRetry;
            EngineCoreEvents.SystemEvents.GetRspPairReq -= PairOfReq;
            EngineCoreEvents.SystemEvents.Listen_SendingCostTimeMS -= LogSendingCostTime;
        }


        public static NetworkModule Instance
        {
            get { return m_instance; }
        }

        /// <summary>
        /// 网络状态
        /// </summary>
        public enum NetworkStatus
        {
            NORMAL,
            TIMEOUT,
            WAITING_SYNC,
            SYNC_SUCCEED,
            EXPIRE,
            LOG_ERROR,
            RESET_RETRY_TIME, //是否继续重连提示
            WAITTING_RETRY,//弹出重连提示
            RETRY_ONE_FINISHED,//单条重连完成（可能成功或者失败）
            RETRY_CACHE_CLEAR,//所有重连成功
            OFFLINE_WARNNING, //断线提醒
        }
    }
}