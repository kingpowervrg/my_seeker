using BestHTTP;
using EngineCore.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace EngineCore
{
    public class HttpSendingCahce
    {
        private static Dictionary<string, RequestMessageWrapper> S_SENDING_REQS = new Dictionary<string, RequestMessageWrapper>();

        public static void Clear()
        {
            S_SENDING_REQS.Clear();
        }

        public static void AddToSendingCache(string send_identifier, RequestMessageWrapper req_)
        {
            S_SENDING_REQS.Add(send_identifier, req_);
        }

        private static void RemoveFromSendingCache(string send_identifier)
        {
            if (!S_SENDING_REQS.ContainsKey(send_identifier))
            {
                Debug.LogError($"BEST HTTP : reqs has no req");
            }

            S_SENDING_REQS.Remove(send_identifier);

        }


        public static RequestMessageWrapper GetAndRemoveFromSendingCache(HTTPRequest req_)
        {
            string send_identifier = req_.GetFirstHeaderValue("sendTime");

            if (!string.IsNullOrEmpty(send_identifier))
            {
                RequestMessageWrapper ret;
                if (S_SENDING_REQS.TryGetValue(send_identifier, out ret))
                {
                    RemoveFromSendingCache(send_identifier);

#if UNITY_DEBUG
                    DateTime t1 = DateTime.Now;
                    string r_timestamp = t1.ToString("yyyy/MM/dd HH:mm:ss:fffffff");
                    //Debug.LogWarning($"SENDING TIME VERIFY : cur msg {req_.Uri.ToString()} {ret.Send_timestamp}");
                    //Debug.LogWarning($"RECEIVING TIME :{req_.Uri}  {r_timestamp}");
                    TimeSpan t22 = new TimeSpan(t1.Ticks);
                    long time2 = Convert.ToInt64(t22.TotalMilliseconds);
                    long cur_time_ms = time2;

                    long delta_ms = cur_time_ms - ret.Send_time;
                    Debug.LogWarning($"BEST HTTP : cur msg {req_.Uri.ToString()} cost {delta_ms} ms");

                    if (delta_ms < 500)
                        return ret;

                    EngineCoreEvents.SystemEvents.Listen_SendingCostTimeMS.SafeInvoke(ret.Send_timestamp, r_timestamp, delta_ms, req_.Uri.ToString(), (byte)(req_.State));
#else
                    long cur_time_ms = CommonUtils.GetCurTimeMillSenconds();
                    long delta_ms = cur_time_ms - ret.Send_time;
                    if (delta_ms < 500)
                        return ret;

                    EngineCoreEvents.SystemEvents.Listen_SendingCostTimeMS.SafeInvoke("", "", delta_ms, req_.Uri.ToString(), (byte)(req_.State));
#endif

                    return ret;
                }
            }
            else
            {
                Debug.LogError($"BEST HTTP : req header has no send time {req_.Uri}");
            }

            return null;
        }
    }
}
