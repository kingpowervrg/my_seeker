#define BEST_HTTP
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace EngineCore
{
    public class HttpRetry
    {
        public const long C_MAX_RETRY_SECONDS = 60L;

        private static long S_CUR_RETRY_SECOND = -1L;
#if BEST_HTTP
        private Queue<RequestMessageWrapper> m_retry_req_queue = new Queue<RequestMessageWrapper>();

        public Queue<RequestMessageWrapper> GetRetryQueue()
        {
            return m_retry_req_queue;
        }
#else
        private ThreadSafeQueue<RequestMessageWrapper> m_retry_req_queue = new ThreadSafeQueue<RequestMessageWrapper>();
      
        public ThreadSafeQueue<RequestMessageWrapper> GetRetryQueue()
        {
            return m_retry_req_queue;
        }
#endif
        public void InitTime()
        {
            S_CUR_RETRY_SECOND = -1;
        }

        private void ResetTime()
        {
            S_CUR_RETRY_SECOND = GetCurrentTimeSecond();
        }


        public bool CheckTime()
        {
            if (S_CUR_RETRY_SECOND < 0)
            {
                ResetTime();
                return true;
            }

            if (GetCurrentTimeSecond() - S_CUR_RETRY_SECOND < C_MAX_RETRY_SECONDS)
            {
                return true;
            }
            else
            {
                return false;
            }
        }


        public void Enqueue(RequestMessageWrapper req_)
        {
            this.m_retry_req_queue.Enqueue(req_);
        }


        public RequestMessageWrapper Dequeue()
        {
            RequestMessageWrapper first_wrapper;
#if BEST_HTTP

            if (0 == this.m_retry_req_queue.Count)
                return null;
            first_wrapper = this.m_retry_req_queue.Dequeue();

#else

            if (false == this.m_retry_req_queue.Dequeue(out first_wrapper))
            {
                return null;
            }
#endif


            //发送，继续倒计时
            return first_wrapper;

        }

        public void ClearQueue()
        {
            this.m_retry_req_queue.Clear();
        }

        public bool IsPendingMsg()
        {
            return this.m_retry_req_queue.Count > 0;
        }





        private static DateTime GetCurrentTime()
        {
            return DateTime.Now;
        }

        public static long GetCurrentTimeSecond()
        {
            DateTime curDT = GetCurrentTime();
            long totalSecond = curDT.Hour * 60 * 60 + curDT.Minute * 60 + curDT.Second;
            return totalSecond;
        }
    }






}
