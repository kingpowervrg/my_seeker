using BestHTTP;
using EngineCore.Utility;
using Google.Protobuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace EngineCore
{
    public class HttpPairCahce
    {
        private static Queue<IMessage> S_PAIR_REQS = new Queue<IMessage>();

        public static void Clear()
        {
            S_PAIR_REQS.Clear();
        }

        public static void EnCache(IMessage req_)
        {
            S_PAIR_REQS.Enqueue(req_);
        }

        public static void DeCache()
        {
            S_PAIR_REQS.Dequeue();
        }

        public static IMessage PeekCache()
        {
            return S_PAIR_REQS.Peek();
        }
       
    }
}
