using System;
using UnityEngine;

namespace EngineCore
{
    /// <summary>
    ///游戏真实时间实现
    /// </summary>
    public class Realtime : IRealtime
    {
        private float realTime = 0;
        private float deltaTime = 0;

        public void Start()
        {
            realTime = UnityEngine.Time.realtimeSinceStartup;
        }

        public float DeltaTime
        {
            get { return deltaTime; }
        }

        public float RealTime
        {
            get { return realTime; }
            set { realTime = value; }
        }

        public long RealTimeMill
        {
            get { return Convert.ToInt64(realTime * 1000); }
        }

        public void Update()
        {
            deltaTime = UnityEngine.Time.realtimeSinceStartup - realTime;
            realTime = UnityEngine.Time.realtimeSinceStartup;
        }
    }
}