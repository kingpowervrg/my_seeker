/********************************************************************
	created:  2018-3-27 11:28:40
	filename: MessageWrapper.cs
	author:	  songguangze@outlook.com
	
	purpose:  网络消息包装类
*********************************************************************/
using Google.Protobuf;

namespace EngineCore
{
    /// <summary>
    /// 相应消息包装类
    /// </summary>
    public class ResponseMessageWrapper
    {
        private int m_messageId;
        private IMessage m_messageBody;

        public ResponseMessageWrapper(int messageId, IMessage messageBody)
        {
            m_messageId = messageId;
            m_messageBody = messageBody;
            
        }

        public int MessageId
        {
            get { return this.m_messageId; }
        }

        public IMessage MessageBody
        {
            get { return this.m_messageBody; }
        }
    }

    /// <summary>
    /// 请求消息包装类
    /// </summary>
    public class RequestMessageWrapper
    {
        private int m_messageId;
        private IMessage m_messageBody;
        private RequestMessageType m_requestType;
        public int m_retry_count = 5;
        private bool m_is_retry = false;
        private long m_send_time;

        public string m_unique_identifier;

        /// <summary>
        /// 单位毫秒
        /// </summary>
        public long Send_time
        {
            get { return m_send_time; }
            set { m_send_time = value; }
        }

        private string m_send_timestamp;
        public string Send_timestamp
        {
            get { return m_send_timestamp; }
            set { m_send_timestamp = value; }
        }
        public bool Is_retry
        {
            get { return m_is_retry; }
            set { m_is_retry = value; }
        }
        public RequestMessageWrapper(int messageId, IMessage messageBody, RequestMessageType requestType = RequestMessageType.ASYNC)
        {
            this.m_messageBody = messageBody;
            this.m_messageId = messageId;
            this.m_requestType = requestType;
        }

        public int MessageId => this.m_messageId;

        public IMessage MessageBody => this.m_messageBody;

        public RequestMessageType RequestType => this.m_requestType;

    }

    /// <summary>
    /// 请求消息的类型
    /// </summary>
    public enum RequestMessageType
    {
        ASYNC,      //异步
        SYNC,        //同步
        HALF_SYNC,  //半同步（无重发）
    }
}