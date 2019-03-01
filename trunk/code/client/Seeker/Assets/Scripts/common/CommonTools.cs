using EngineCore;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;

namespace SeekerGame
{
    public class CommonTools
    {
        public static DateTime TimeStampToDateTime(long timeStamp)
        {
            DateTime defaultTime = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1, 0, 0, 0));

            //DateTime defaultTime = new DateTime(1970,1,1,0,0,0);
            long defaultTick = defaultTime.Ticks;
            long tickTime = defaultTick + timeStamp;
            DateTime dt = new DateTime(tickTime);
            return dt;
        }

        public static DateTime GetCurrentTime()
        {
            return DateTime.Now;
        }

        public static long GetCurrentTimeSecond()
        {
            DateTime curDT = CommonTools.GetCurrentTime();
            long totalSecond = curDT.Hour * 60 * 60 + curDT.Minute * 60 + curDT.Second;
            return totalSecond;
        }

        public static float GetCurrentTimePercent()
        {
            long totalSecond = GetCurrentTimeSecond();
            return totalSecond / 86400f;
        }

        public static long DateTimeToTimeStamp(DateTime dt)
        {
            DateTime defaultTime = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1, 0, 0, 0));
            dt = TimeZone.CurrentTimeZone.ToLocalTime(dt);
            //DateTime defaultTime = new DateTime(1970, 1, 1, 0, 0, 0);
            long curTick = dt.Ticks;
            return curTick - defaultTime.Ticks;
        }

        public static string SecondToStringDDMMSS(double second)
        {

            TimeSpan span = TimeSpan.FromSeconds(second);

            return string.Format("{0:D2}:{1:D2}:{2:D2}", (int)span.Hours, span.Minutes, span.Seconds);
        }

        public static string SecondToStringDay2Second(double second)
        {

            TimeSpan span = TimeSpan.FromSeconds(second);
            if (span.TotalDays > 1)
            {
                return string.Format("{0}days", (int)span.TotalDays);
            }
            else
            {
                return string.Format("{0:D2}:{1:D2}:{2:D2}", (int)span.Hours, span.Minutes, span.Seconds);
            }

        }

        public static string SecondToStringDay2Minute(double second)
        {

            TimeSpan span = TimeSpan.FromSeconds(second);
            if (span.TotalDays > 1)
            {
                return string.Format("{0}days", (int)span.TotalDays);
            }
            else
            {
                return string.Format("{0:D2}:{1:D2}:{2:D2}", (int)span.Days, span.Hours, span.Minutes);
            }
        }

        public static string SecondToTitleString(double second)
        {

            TimeSpan span = TimeSpan.FromSeconds(second);
            if (span.TotalDays / 30 > 0)
            {
                return string.Format("{0}month {1}day {2}hour", (int)span.TotalDays / 30, (int)span.TotalDays % 30, span.Hours);
            }
            else
            {
                return string.Format("{0}day {1}hour", (int)span.TotalDays, span.Hours);
            }
            //return string.Format("{0:D2}:{1:D2}:{2:D2}", span.Hours, span.Minutes, span.Seconds);
        }

        public static string SecondToStringMMSS(double second)
        {
            TimeSpan span = TimeSpan.FromSeconds(second);
            return string.Format("{0:D2}:{1:D2}", span.Minutes, span.Seconds);
        }


        public static ENUM_GENDER GetGender(int msg_gender_)
        {
            return (ENUM_GENDER)(msg_gender_);
        }

        public static string GetGenderIcon(int msg_gender_)
        {

            ENUM_GENDER gender = GetGender(msg_gender_);

            return GetGenderIcon(gender);

        }

        public static string GetGenderIcon(ENUM_GENDER gender_)
        {
            return ENUM_GENDER.E_MALE == gender_ ? "label_sex_1.png" : "label_sex_2.png";

        }

        //public DateTime GetTime

        /**//// <summary>
            /// MD5 16位加密 加密后密码为大写
            /// </summary>
            /// <param name="ConvertString"></param>
            /// <returns></returns>
        public static string GetMd5Str1(string ConvertString)
        {
            MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
            string t2 = BitConverter.ToString(md5.ComputeHash(Encoding.UTF8.GetBytes(ConvertString)), 4, 8);
            t2 = t2.Replace("-", "");
            return t2;
        }


        public static bool IsNeedDownloadIcon(string icon_name_)
        {
            return (icon_name_.StartsWithFast("http://") || icon_name_.StartsWithFast("https://"));
        }

        public static void OnHttpError(string content_)
        {
            CheckNetError(false, content_);
        }

        //public static void CheckNetError(bool reset_, string content_ = null)
        //{
        //    if (reset_)
        //    {
        //        if (PlayerPrefTool.GetNetError())
        //        {
        //            PlayerPrefTool.SetNetError(false);

        //            Dictionary<UBSParamKeyName, object> _param = new Dictionary<UBSParamKeyName, object>();
        //            _param.Add(UBSParamKeyName.Description, null != content_ ? content_ : "none");
        //            _param.Add(UBSParamKeyName.Success, 1);
        //            UserBehaviorStatisticsModules.Instance.LogEvent(UBSEventKeyName.net_error, null, _param);
        //        }
        //    }
        //    else
        //    {
        //        PlayerPrefTool.SetNetError(true);

        //        Dictionary<UBSParamKeyName, object> _param = new Dictionary<UBSParamKeyName, object>();
        //        _param.Add(UBSParamKeyName.Description, null != content_ ? content_ : "none");
        //        _param.Add(UBSParamKeyName.Success, 0);
        //        UserBehaviorStatisticsModules.Instance.LogEvent(UBSEventKeyName.net_error, null, _param);
        //    }
        //}

        public static void CheckNetError(bool reset_, string content_ = null)
        {
            Dictionary<UBSParamKeyName, object> _param = new Dictionary<UBSParamKeyName, object>();
            _param.Add(UBSParamKeyName.Description, null != content_ ? string.Format("async error {0}", content_) : "async error");
            UserBehaviorStatisticsModules.Instance.LogEvent(UBSEventKeyName.net_error, null, _param);

        }

        public static long[] StringToLongArray(string str)
        {
            if (string.IsNullOrEmpty(str.Trim()))
            {
                return null;
            }
            string[] strSplit = str.Split(',');
            List<long> LongData = new List<long>();
            //long[] LongData = new long[strSplit.Length];
            for (int i = 0; i < strSplit.Length; i++)
            {
                if (!string.IsNullOrEmpty(strSplit[i]))
                {
                    LongData.Add(long.Parse(strSplit[i]));
                }
                //LongData[i] = long.Parse(strSplit[i]);
            }
            return LongData.ToArray();
        }

        public static string GetAndroidID()
        {
#if UNITY_ANDROID
            AndroidJavaClass up = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            AndroidJavaObject currentActivity = up.GetStatic<AndroidJavaObject>("currentActivity");
            AndroidJavaObject contentResolver = currentActivity.Call<AndroidJavaObject>("getContentResolver");
            AndroidJavaClass secure = new AndroidJavaClass("android.provider.Settings$Secure");
            string android_id = secure.CallStatic<string>("getString", contentResolver, "android_id");

            return android_id;
#endif
            return string.Empty;
        }

        public static string GetIMEI()
        {
#if UNITY_ANDROID
            AndroidJavaObject TM = new AndroidJavaObject("android.telephony.TelephonyManager");

            string IMEI = TM.Call<string>("getDeviceId");

            return IMEI;
#endif

            return string.Empty;
        }

    }
}


