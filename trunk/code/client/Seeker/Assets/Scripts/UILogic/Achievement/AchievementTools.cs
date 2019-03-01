using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SeekerGame
{
    public class AchievementTools
    {
        public static readonly Comparison<AchievementMsg> s_Comparer = CompareAchievement;
        private static int CompareAchievementById(AchievementMsg msg0,AchievementMsg msg1)
        {
            long msgID0 = msg0.Id;
            long msgID1 = msg1.Id;
            if (msgID0 < msgID1)
            {
                return -1;
            }
            if (msgID0 == msgID1)
            {
                return 0;
            }
            return 1;
        }

        private static int CompareAchievement(AchievementMsg msg01, AchievementMsg msg02)
        {

            long msgFinishTime1 = getFinishTime(msg01);
            long msgFinishTime2 = getFinishTime(msg02);
            if (msgFinishTime1 == 0 && msgFinishTime2 == 0)
            {
                return CompareAchievementById(msg01,msg02);
            }
            if (msgFinishTime1 < msgFinishTime2)
            {
                return 1;
            }
            if (msgFinishTime1 == msgFinishTime2)
            {
                return 0;
            }
            return -1;
        }

        private static long getFinishTime(AchievementMsg msg)
        {
            if (msg.FinishTime > 0)
            {
                return msg.FinishTime;
            }
            else if (msg.FinishTime2 > 0)
            {
                return msg.FinishTime2;
            }
            else if (msg.FinishTime1 > 0)
            {
                return msg.FinishTime1;
            }
            return 0;
        }
    }

}
