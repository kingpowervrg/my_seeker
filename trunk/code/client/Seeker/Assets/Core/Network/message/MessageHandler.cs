/********************************************************************
	created:  2018-3-29 16:46:21
	filename: MesssageHandler.cs
	author:	  songguangze@outlook.com
	
	purpose:  消息处理中心
*********************************************************************/
using System;
using System.Collections.Generic;
using UnityEngine;

namespace EngineCore
{
    public static class MessageHandler
    {
        private static Dictionary<int, Action<object>> m_messageHandlerDict = new Dictionary<int, Action<object>>();

        public static void RegisterMessageHandler(int msgid, Action<object> func)
        {
            Action<object> action = null;
            if (!m_messageHandlerDict.TryGetValue(msgid, out action))
                m_messageHandlerDict[msgid] = func;
            else
            {
                action -= func;
                action += func;
                m_messageHandlerDict[msgid] = action;
            }
        }




        public static void UnRegisterMessageHandler(int msgid, Action<object> func)
        {
            Action<object> action = null;
            if (m_messageHandlerDict.TryGetValue(msgid, out action))
            {
                action -= func;
                m_messageHandlerDict[msgid] = action;
            }
        }

        public static void Call(int msgid, object msg)
        {
            Action<object> action = null;
            if (!m_messageHandlerDict.TryGetValue(msgid, out action))
                return;
            if (action == null)
                return;
            action(msg);
        }

    }
}
