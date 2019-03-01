using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EngineCore.Utility
{
    public class CommonUtils
    {
        public static int[] GetRandomList(int length)
        {
            int[] shuffled = new int[length];
            UnityEngine.Random rand = new UnityEngine.Random();
            for (int i = 0; i < length; i++)
                shuffled[i] = i;
            for (int i = 0; i < length; i++)
            {
                int tmp = UnityEngine.Random.Range(0, length); //record the swaped place

                int tmp2 = shuffled[i];
                shuffled[i] = shuffled[tmp];
                shuffled[tmp] = tmp2;


            }
            return shuffled;
        }

        public static int GetStringCount(string str)
        {
            int count = 0;
            for (int i = 0; i < str.Length; i++)
            {
                if ((int)str[i] > 127)
                {
                    count += 2;
                }
                else
                {
                    count++;
                }
            }
            return count;
        }

        public static long GetCurTimeMillSenconds()
        {
            DateTime t1 = DateTime.Now;
            TimeSpan t22 = new TimeSpan(t1.Ticks);
            long time2 = Convert.ToInt64(t22.TotalMilliseconds);
            return time2;
        }

        public static long ConvertTicksToMillSenconds( long ticks_)
        {
            TimeSpan t22 = new TimeSpan(ticks_);
            long time2 = Convert.ToInt64(t22.TotalMilliseconds);
            return time2;
        }

        public static long GetCurTicks()
        {

            DateTime t1 = DateTime.Now;
            return t1.Ticks;
        }
    }
}