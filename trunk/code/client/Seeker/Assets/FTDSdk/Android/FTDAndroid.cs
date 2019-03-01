using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
namespace com.ftdsdk.sdk
{
#if UNITY_ANDROID
    public class FTDAndroid
    {
        private static AndroidJavaClass cFTDSDK = new AndroidJavaClass("com.ftdsdk.www.FTDSDK");
        private static AndroidJavaClass ftdSDKLogical = new AndroidJavaClass("com.ftdsdk.www.logical.FTDSDKLogical");
        private static AndroidJavaObject ftdSdk;
      

        /**
          * SDK初始化
          * @param activity 当前的activtiy
          * @param appId 
          * @param appKey 
          * @param signWay 
          */
        public static void ftdSdkInit(AndroidJavaObject activity, String appId, String appKey, String signWay)
        {
            AndroidJavaObject application = activity.Call<AndroidJavaObject>("getApplication");
            ftdSDKLogical.CallStatic("setUnitySDKVer", FTDSdk.sdkVer);
            ftdSDKLogical.CallStatic("init", application, appId, appKey, signWay);
           
            if (ftdSdk == null)
            {
                ftdSdk = ftdSDKLogical.CallStatic<AndroidJavaObject>("getInstance");
                //ftdSdk = cFTDSDK.CallStatic<AndroidJavaObject>("getApi");

            }
        }
        public static void setOnAttributeListener(FTDAttributeCallback mCallback)
        {
            ftdSdk.Call("setOnAttributeListener", mCallback);
        }
        public static void setHttpCallback(FTDHttpCallback mCallback)
        {
            ftdSdk.Call("setHttpCallback", mCallback);
        }
        public static void onPause()
        {
            ftdSdk.Call("onPause");
        }

        public static void onResume()
        {
            ftdSdk.Call("onResume");
        }

        /**
         * 上报渠道SDK归因
         * @param coversionData 渠道回调返回的归因
         */
        public static void sendAttributeData(String channel, String jsonAttr)
        {
            Debug.Log(">>>>>>>>>>>>>sendAttributeData");
            AndroidJavaObject json = new AndroidJavaObject("org.json.JSONObject", jsonAttr);
            //ftdSdk.Call("sendAttributeData",  channel, json);
            ftdSdk.Call("sendAttributeData", channel, json);
        }

        /**
        * 上报登录事件
        * @param loginChannel 登录渠道
        * @param customParams 自定义参数，根据需要传入，不需要传null即可
        */
        public static void logEventLogin(String loginChannel, String name, String way, long intime, long outtime, Dictionary<String, String> customParams)
        {
            ftdSdk.Call("logEventLogin", loginChannel,name,way,intime,outtime, dic2json(customParams));

            // ftdSdk.Invoke("logEventLogin", new object[] { loginChannel, map });
        }

        /**
         * 上报注册事件
         * @param registChannel 注册渠道
         * @param customParams 自定义参数，根据需要传入，不需要传null即可
         */
        public static void logEventRegist(String registChannel, String name, Dictionary<String, String> customParams)
        {
            ftdSdk.Call("logEventRegist", registChannel,name, dic2json(customParams));

            //ftdSdk.Invoke("logEventRegist", new object[] { registChannel, map });

        }

        /**
        * 上报支付事件
        * @param channel 支付渠道
        * @param itemid 商品id
        * @param itemName 商品id
        * @param usdPrice 必须是美分价格，否则无法计算数据,必传数据
        * @param price 支付金额(必须以“分”为单位)
        * @param currency 币种（使用统一币种，推荐“USD”）
        * @param customParams 自定义参数，根据需要传入
        */
        public static void logEventPurchase(String channel, String itemid, String itemName, int usdPrice, int price, String currency, Dictionary<String, String> customParams)
        {
            ftdSdk.Call("logEventPurchase", channel, itemid, itemName, usdPrice, price, currency, dic2json(customParams));

            //ftdSdk.Invoke("logEventPurchase", new object[] { channel, itemid, itemName, price, currency, map });
        }

        /**
        * 完成新手引导
        * @param isSuccess 是否成功
        * @param customParams 自定义参数，根据需要传入，不需要传null即可
        */
        public static void logEventCompletedTutorial(bool isSuccess, Dictionary<String, String> customParams)
        {

            ftdSdk.Call("logEventCompletedTutorial", isSuccess, dic2json(customParams));

            //ftdSdk.Invoke("logEventCompletedTutorial", new object[] { isSuccess, map });

        }

        /**
         * 自定义事件
         * @param eventName 自定义事件名称
         * @param eventId 自定义事件id
         * @param params 自定义事件参数
         */
        public static void logCustomEvent(String eventName, String eventId, Dictionary<String, String> customParams)
        {
            ftdSdk.Call("logCustomEvent", eventName, eventId, dic2json(customParams));

            //ftdSdk.Invoke("logCustomEvent", new object[] { eventName, eventId, map });
        }
        /**
          * 广告展示事件
          * @param adurl 广告链接
          * @param params 自定义事件参数
          */
        public static void adDisplayEvent(String ad_app, String ad_media_source, String ad_campaign, String ad_channel, String ad, Dictionary<String, String> customParams)
        {
            ftdSdk.Call("adDisplayEvent", ad_app, ad_media_source, ad_campaign, ad_channel, ad, dic2json(customParams));
        }
        /**
        * 广告点击事件
        * @param adurl 广告链接
        * @param params 自定义事件参数
        */
        public static void adClickEvent(String ad_app, String ad_media_source, String ad_campaign, String ad_channel, String ad, Dictionary<String, String> customParams)
        {
            ftdSdk.Call("adClickEvent", ad_app, ad_media_source, ad_campaign, ad_channel, ad, dic2json(customParams));
        }
        /**
         * 设置tag接口，此TAG是标识在设备层级的tag，设置即会在tag数组中追加tag。新设置tag不会覆盖旧tag，且不可删除。
         * 一旦设置每次数据上报都会通过通传参数发送给SDK服务器，服务器可根据tag对数据进行归类计算等。
         *
         * @param tags
         */
        public static void setTags(String[] tags)
        {
             ftdSdk.Call("setTags", javaArrayFromCS(tags));
        }
        /**
         * 设置在线时长事件in/out 附加参数，有则更新，无则追加，暂不可删除
         *
         * @param onlineTimeParams
         */
        public static void setOnlineTimeParams(Dictionary<string, string> onlineTimeParams)
        {
            ftdSdk.Call("setOnlineTimeParams", dic2json(onlineTimeParams));
        }

        /**
             * 登录关卡事件
             *
             * @param level        关卡数>12
             * @param way          登入/登出 > in/out
             * @param intime       登入时间(登出时为本次登入时间)>1541561218
             * @param outtime      登出时间>1541561230
             * @param customParams 自定义参数，根据需要传入，不需要传null即可
             */
        public static void trackLevelIn(String level, String way, long intime, long outtime, Dictionary<String, String> customParams)
        {
            ftdSdk.Call("trackLevelIn", level,way,intime,outtime, dic2json(customParams));
        }
        /**
     * 完成关卡事件
     *
     * @param isSuccess    是否成功
     * @param level        关卡>12
     * @param customParams 自定义参数，根据需要传入，不需要传null即可
     */
        public static void trackLevelDone(bool isSuccess, String level, Dictionary<String, String> customParams)
        {
            ftdSdk.Call("trackLevelDone", isSuccess, level, dic2json(customParams));
        }
        /**
     * 道具购买事件
     *
     * @param propid       道具ID > ham1
     * @param propname     道具名称 > "锤子"
     * @param propnum      购买数量 > 1
     * @param costcoin     消耗金币 > 10000
     * @param customParams
     */
        public static void trackPropPurchase(String propid, String propname, int propnum, String coinid, String coinname,  int costcoin, Dictionary<String, String> customParams)
        {
            ftdSdk.Call("trackPropPurchase", propid, propname, propnum, coinid, coinname, costcoin, dic2json(customParams));
        }
        /**
   * 道具使用事件
   *
   * @param propid       道具ID > ham1
   * @param propname     道具名称 > "锤子"
   * @param propnum      购买数量 > 1
   * @param customParams
   */
        public static void trackPropUse(String propid, String propname, int propnum, Dictionary<String, String> customParams)
        {
            ftdSdk.Call("trackPropUse", propid, propname, propnum, dic2json(customParams));
        }
        /**
  * 完成加载事件上报
  *
  * @param isSuccess    是否成功
  * @param customParams 自定义参数，根据需要传入，不需要传null即可
  */
        public static void trackEventLoading(bool isSuccess, Dictionary<String, String> customParams)
        {
            ftdSdk.Call("trackEventLoading", isSuccess, dic2json(customParams));
        }
        private static string dic2json(Dictionary<string, string> dictionary)
        {
            if (dictionary == null)
            {
                return null;
            }
            ArrayList list = new ArrayList();

            foreach (KeyValuePair<string, string> pair in dictionary)
            {
                list.Add(String.Format("\"{0}\":\"{1}\"", pair.Key, pair.Value));
            }

            return "{" + string.Join(",", (string[])list.ToArray(typeof(string))) + "}";
        }
        private static AndroidJavaObject javaArrayFromCS(string[] values)
        {   
           if(values == null)
            {
                values = new string[] { };
            }
            AndroidJavaClass arrayClass = new AndroidJavaClass("java.lang.reflect.Array");
            AndroidJavaObject arrayObject = arrayClass.CallStatic<AndroidJavaObject>("newInstance", new AndroidJavaClass("java.lang.String"), values.Count());
            for (int i = 0; i < values.Count(); ++i)
            {
                arrayClass.CallStatic("set", arrayObject, i, new AndroidJavaObject("java.lang.String", values[i]));
            }

            return arrayObject;

        } 
}
#endif
    }

