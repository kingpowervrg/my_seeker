/********************************************************************
	created:  2018-3-23 14:15:4
	filename: TimeModule.cs
	author:	  songguangze@outlook.com
	
	purpose:  客户端时间模块
*********************************************************************/
using System;
using System.Collections.Generic;
using UnityEngine;

namespace EngineCore
{
    [EngineCoreModuleAttribute(EngineCore.ModuleType.TIME_MODULE)]
    public class TimeModule : AbstractModule
    {
        private List<TimeFunc> m_timerFuncs = new List<TimeFunc>();
        private List<Action> m_nextFrameExecuteActionList = new List<Action>();

        private static TimeModule m_instance = null;
        private static IRealtime m_realTime = null;

        private TimeModule()
        {
            AutoStart = true;
            m_instance = this;

            EngineCoreEvents.SystemEvents.SendTaskExecuteNextFrame += ExecuteOnNextFrame;
        }


        /// <summary>
        /// 设置定时任务
        /// </summary>
        /// <param name="fun"></param>
        /// <param name="time"></param>
        /// <param name="loop"></param>
        /// <param name="canPause"></param>
        public void SetTimeout(Action fun, float time, bool loop = false, bool canPause = true)
        {
            SetTimeout(fun, time, -1.0f, loop, canPause);
        }

        public TimeFunc SetTimeout(Action fun, float interval_time, float duration_time, bool loop = false, bool canPause = true)
        {
            if (interval_time > 0)
            {
                if (CheckDuplicate(fun))
                {
                    return GetDuplicate(fun);
                }

                TimeFunc tf = new TimeFunc(fun, interval_time, duration_time);
                tf.loop = loop;
                tf.canPause = canPause;
                m_timerFuncs.Add(tf);

                return tf;
            }
            else
                fun();

            return new TimeFunc();
        }

        /// <summary>
        /// 是否有指定任务
        /// </summary>
        /// <param name="fun"></param>
        /// <returns></returns>
        public bool HasAction(Action fun)
        {
            int len = m_timerFuncs.Count;
            for (int i = 0; i < len; i++)
            {
                TimeFunc funTemp = m_timerFuncs[i];
                if (funTemp.handler == fun)
                    return true;
            }
            return false;
        }

        /// <summary>
        /// 定时循环任务
        /// </summary>
        /// <param name="fun"></param>
        /// <param name="interval"></param>
        /// <param name="canPause"></param>
        public void SetTimeInterval(Action fun, float interval, bool canPause = true)
        {
            if (interval > 0)
                SetTimeout(fun, interval, true, canPause);
        }


        public TimeFunc SetTimeInterval(Action fun, float interval, float duration_, bool canPause = true)
        {
            if (interval > 0)
                return SetTimeout(fun, interval, duration_, true, canPause);
            else
                return new TimeFunc();

        }

        /// <summary>
        /// 清除指定Timer
        /// </summary>
        /// <param name="fun"></param>
        public void RemoveTimeaction(Action fun)
        {
            //清除下一帧队列
            m_nextFrameExecuteActionList.Remove(fun);

            int len = m_timerFuncs.Count;
            for (int i = 0; i < len; i++)
            {
                if (m_timerFuncs[i].handler == fun)
                {
                    m_timerFuncs.RemoveAt(i);
                    return;
                }
            }
        }

        /// <summary>
        /// 清除无效Timer
        /// </summary>
        public void ClearAllCanPaused()
        {
            for (int i = 0; i < m_timerFuncs.Count; i++)
            {
                if (m_timerFuncs[i].canPause)
                {
                    m_timerFuncs.RemoveAt(i);
                    i--;
                }
            }
        }

        /// <summary>
        /// 将方法timeout时间延长
        /// </summary>
        /// <param name="fun"></param>
        /// <param name="time"></param>
        public void TimeFuncAddTime(Action fun, float time)
        {
            TimeFunc tempFunc = GetDuplicate(fun);
            if (tempFunc != null)
            {
                tempFunc.time = time;
            }
        }

        /// <summary>
        /// 是否重复任务
        /// </summary>
        /// <param name="fun"></param>
        /// <returns></returns>
        private bool CheckDuplicate(Action fun)
        {
            int len = m_timerFuncs.Count;
            for (int i = 0; i < len; i++)
            {
                if (m_timerFuncs[i].handler == fun)
                    return true;
            }
            return false;
        }

        /// <summary>
        /// 获取重复任务
        /// </summary>
        /// <param name="fun"></param>
        /// <returns></returns>
        public TimeFunc GetDuplicate(Action fun)
        {
            int len = m_timerFuncs.Count;
            for (int i = 0; i < len; i++)
            {
                if (m_timerFuncs[i].handler == fun)
                    return m_timerFuncs[i];
            }
            return null;
        }

        public override void Start()
        {
            base.Start();

            m_realTime = new Realtime();
            m_realTime.Start();
        }

        public override void Update()
        {
            if (m_isStarted)
            {
                m_realTime.Update();

                TimerTick();
            }
        }

        /// <summary>
        /// 下一帧执行
        /// </summary>
        /// <param name="action"></param>
        private void ExecuteOnNextFrame(Action action)
        {
            this.m_nextFrameExecuteActionList.Add(action);
        }

        /// <summary>
        /// 定时任务的Tick
        /// </summary>
        private void TimerTick()
        {
            for (int i = 0; i < m_nextFrameExecuteActionList.Count; ++i)
                m_nextFrameExecuteActionList[i]();
            m_nextFrameExecuteActionList.Clear();

            float deltaTime = GameRealTime.DeltaTime * Time.timeScale;

            for (int i = m_timerFuncs.Count - 1; i >= 0; --i)
            {
                TimeFunc timerFunc = m_timerFuncs[i];
                if (timerFunc.canPause && Pause)
                    continue;

                timerFunc.time -= deltaTime;
                if (timerFunc.time <= 0)
                {
                    ExecuteOnNextFrame(timerFunc.handler);

                    if (timerFunc.loop)
                    {
                        timerFunc.Reset();

                        if (!timerFunc.undead)
                        {
                            timerFunc.age -= deltaTime;

                            if (timerFunc.age <= 0)
                            {
                                if (null != timerFunc.finish_handler)
                                {
                                    timerFunc.finish_handler();
                                }

                                m_timerFuncs.RemoveAt(i);
                            }
                        }
                    }
                    else
                    {
                        m_timerFuncs.RemoveAt(i);
                    }
                }
            }
        }

        public override void Dispose()
        {
            EngineCoreEvents.SystemEvents.SendTaskExecuteNextFrame -= ExecuteOnNextFrame;
        }

        public bool Pause
        {
            get; set;
        }

        public List<TimeFunc> Functions
        {
            get { return m_timerFuncs; }
        }

        public static TimeModule Instance
        {
            get { return m_instance; }
        }

        /// <summary>
        /// 真实游戏时间
        /// </summary>
        public static IRealtime GameRealTime
        {
            get { return m_realTime; }
        }



        /// <summary>
        /// 定时任务函数封装
        /// </summary>
        public class TimeFunc
        {
            public Action handler;
            public float time;
            public float maxTime;
            public bool loop;
            public bool canPause;
            public bool undead;
            public float age;

            public Action finish_handler = null;

            public TimeFunc()
            {

            }

            public TimeFunc(Action fun, float tm, float age_ = -1)
            {
                handler = fun;
                time = tm;
                maxTime = time;
                loop = false;
                canPause = true;
                undead = age_ < 0.0f ? true : false;
                age = age_;
            }

            public void Reset()
            {
                time = maxTime;
            }

            public void OnComplete(Action handler_)
            {
                finish_handler = handler_;
            }

        }
    }


}
