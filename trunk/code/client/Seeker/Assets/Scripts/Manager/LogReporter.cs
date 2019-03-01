/********************************************************************
	created:  2018-9-21 18:29:28
	filename: LogReporter.cs
	author:	  songguangze@outlook.com
	
	purpose:  客户端日志上报
*********************************************************************/
using System.Collections.Generic;
using UnityEngine;

namespace SeekerGame
{
    public class LogReporter
    {
        public static void InitLogReporter()
        {
#if (UNITY_ANDROID && !UNITY_DEBUG) || UNITY_IOS
            Debug.unityLogger.logEnabled = false;
            Application.logMessageReceivedThreaded -= LogCallback;
            Application.logMessageReceivedThreaded += LogCallback;
#else   
            Debug.unityLogger.logEnabled = true;
#endif
        }

        private static void LogCallback(string condition, string stackTrace, LogType type)
        {
            switch (type)
            {
                case LogType.Exception:
                case LogType.Error:
                    Dictionary<UBSParamKeyName, object> logParam = new Dictionary<UBSParamKeyName, object>();
                    logParam.Add(UBSParamKeyName.error_content, condition);
                    logParam.Add(UBSParamKeyName.error_stack, stackTrace);
                    UserBehaviorStatisticsModules.Instance.LogEvent(UBSEventKeyName.system_log, null, logParam);
                    break;
            }
        }
    }
}