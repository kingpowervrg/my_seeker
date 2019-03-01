using System;
using System.Collections.Generic;
using Google.Protobuf;

namespace EngineCore
{
    public class MessageStandalonFactory : Singleton<MessageStandalonFactory>
    {
        public Dictionary<int, Type> m_message = new Dictionary<int, Type>();

        public MessageStandalonFactory()
        {
            m_message.Add(MessageDefine.CSSceneSuspendRequest,typeof(SCSceneSuspendResponse));
            m_message.Add(MessageDefine.CSSceneResumeRequest, typeof(SCSceneResumeResponse));
        }

        public IMessage GetResponseMessage(int sendMessageID)
        {
            if (m_message.ContainsKey(sendMessageID))
            {
                Type type = m_message[sendMessageID];
                IMessage response = Activator.CreateInstance(type) as IMessage;
                return response;
            }
            return null;
        }
    }
}
