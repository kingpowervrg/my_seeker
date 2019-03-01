using System;
using System.Collections.Generic;
using UnityEngine;
using com.ftdsdk.sdk;
using System.Collections;
using System.Runtime.InteropServices;

public class FTDSdk : MonoBehaviour
    {
        private static FTDSdk instance;
        private const string errorMsgEditor = "FTDSdk: SDK can not be used in Editor.";
        private const string errorMsgStart = "FTDSdk: SDK not started. Start it manually using the 'start' method.";
        private const string errorMsgPlatform = "FTDSdk: SDK can only be used in Android, iOS .";
        private const string errorMsgNotInit = "FTDSdk: SDK not init .";
        public static FtPayVerifyCallback payVerifyCallback;
        public static FtOnattributeChangedListener changedListener;

        private static bool isInit = false;
        private static bool isOnFTApplicationPause = false;
        private static bool isOnFTStart = false;

        public static string sdkVer = "2.1.0";
#if UNITY_IOS

        [DllImport("__Internal")]
        private static extern void fInitAppVestAndStartIt(string appid, string appkey, string way);

        [DllImport("__Internal")]
        private static extern void fRegisteredSuccese(string registChannel, string name, string customParams);

        [DllImport("__Internal")]
        private static extern void fLoginedSuccese(string loginChannel, string name, string way, string intime, string outtime, string customParams);

        [DllImport("__Internal")]
        private static extern void fNewbieGuideSuccese(string isSuccess, string customParams);

        [DllImport("__Internal")]
        private static extern void fTrackRevenueSuccese(string itemid, string itemName, string usdprice, string price, string currency, string channel, string customParams);

        [DllImport("__Internal")]
        private static extern void fGetCustomEvent(string eventName, string eventId, string customParams);

        [DllImport("__Internal")]
        private static extern void fGetAttributionReturnFromChannel(string jsonAttr, string channel);

        [DllImport("__Internal")]
        private static extern void fGetUserInAppWithWay(string way);

        [DllImport("__Internal")]
        private static extern void fGetUserWithTags(string tag); 

        [DllImport("__Internal")]
        private static extern void fGetUserWithUserAttributes(string attributes);

        [DllImport("__Internal")]
        private static extern void fValidateAndTrackInAppPurchase(string productIdentifier
                                    ,string price,string currency,string transactionId,string itemid,string itemname,string usdPrice,string customParams);

         [DllImport("__Internal")]
        private static extern void fGetUserAdvertisingDisplay(string ad_app,string ad_media_source,string ad_campaign,string ad_channel,string ad,string customParams);

         [DllImport("__Internal")]
        private static extern void fGetUserADClick(string ad_app,string ad_media_source,string ad_campaign,string ad_channel,string ad,string customParams);

        [DllImport("__Internal")]
        private static extern void fLoginLevels(string level,string way,string intime,string outtime, string customParams);

        [DllImport("__Internal")]
        private static extern void fCompleteLevel(string level,string issuccess, string customParams);

        [DllImport("__Internal")]
        private static extern void fPropsToUse(string propid,string propname,string propnum, string customParams);

        [DllImport("__Internal")]
        private static extern void fPropsToBuy(string propid,string propname,string propnum,string coinid,string coinname ,string costcoin ,string customParams);

        [DllImport("__Internal")]
        private static extern void fFinishedLoading(string issuccess ,string customParams);


#endif
    private FTDSdk()
        {

        }
        /**
         * 获取api
         * */
        public static FTDSdk getInstance()
        {
            if(instance == null)
            {
                    GameObject ftGameObject = GameObject.Find("FTDSdk");
                    if(ftGameObject == null)
                    {
                          ftGameObject = new GameObject("FTDSdk");
                          ftGameObject.AddComponent<FTDSdk>();
                    }
                    instance = ftGameObject.GetComponent<FTDSdk>();
            }
      
        return instance;
        }

        /**
         * SDK初始化
         * */
       public static void  init(string appId, string appKey, string signWay)
        {
        if (IsEditor())
            {
                return;
            }
            if (!checkInitParams(appId, appKey, signWay))
            {
                return;
            }
#if UNITY_IOS
            initIos(appId, appKey, signWay);
#elif UNITY_ANDROID
            initAndroid(appId, appKey, signWay);
#else
                        Debug.Log(errorMsgPlatform);
#endif
        }

        /**
         * 初始化android api
         * */
        public static void initAndroid(string appId, string appKey, string signWay)
        {
            Debug.Log("initAndroid.");

        if (IsEditor())
            {
                return;
            }
#if UNITY_ANDROID
            if (!checkInitParams(appId, appKey, signWay))
            {
                return;
            }
            AndroidJavaObject currentActivity = new AndroidJavaClass("com.unity3d.player.UnityPlayer").GetStatic<AndroidJavaObject>("currentActivity");
            FTDAndroid.ftdSdkInit(currentActivity, appId, appKey, signWay);
            isInit = true;
#endif
        }

        /**
         * 初始化IOS api
         * */
        public static void initIos(string appId, string appKey, string signWay)
        {
            Debug.Log("initIos.");

            if (IsEditor())
            {
                return;
            }
#if UNITY_IOS
            if (!checkInitParams(appId, appKey, signWay))
            {
                Debug.Log("Invalid parameters");
                return;
            }
             fInitAppVestAndStartIt(appId, appKey, signWay);
            isInit = true;
#endif
        }
        public void OnFTApplicationPause(bool pauseStatus)
        {
            Debug.Log("OnApplicationPause  = " + pauseStatus);
            isOnFTApplicationPause = true;
      
            if (!checkAvailable())
            {
                return;
            }

#if UNITY_IOS
                      if (pauseStatus)
                        {
                        fGetUserInAppWithWay("out");
                        }
                        else
                        {
                        fGetUserInAppWithWay("in");
                        }
#elif UNITY_ANDROID
        if (pauseStatus)
                        {
                            FTDAndroid.onPause();
                        }
                        else
                        {
                            FTDAndroid.onResume();
                        }
#else
            Debug.Log(errorMsgPlatform);
#endif
        }
    public void OnFTStart()
    {
        Debug.Log("Unity  OnFTStart ");
        isOnFTStart = true;
        if (!checkAvailable())
        {
            return;
        }

#if UNITY_IOS
 
        fGetUserInAppWithWay("in");
        
#endif
    }

    /**
     * 设置归因回调
     * */
    public void setOnAttributeListener(FtOnattributeChangedListener changedListener)
        {
            Debug.Log("setOnAttributeListener success.");
        if (!checkAvailable())
            {
                return;
            }
        FTDSdk.changedListener = changedListener;

#if UNITY_IOS
        // No action, iOS SDK is subscribed to iOS lifecycle notifications.
#elif UNITY_ANDROID
        FTDAndroid.setOnAttributeListener(new FTDAttributeCallback(changedListener));
#else
                        Debug.Log(errorMsgPlatform);
#endif

    }
    /**
     * 设置ios支付验证回调
     * */

    public void setPayVerifyCallback(FtPayVerifyCallback payVerifyCallback)
    {
        Debug.Log("setPayVerifyCallback success.");
        if (!checkAvailable())
        {
            return;
        }
        FTDSdk.payVerifyCallback = payVerifyCallback;
    }

    /**
     * 设置Http回调
     * */
    public void setHttpCallback(FtHttpCallback ftHttpCallback)
        {
            Debug.Log("setHttpCallback success.");

            if (!checkAvailable())
            {
                return;
            }
#if UNITY_IOS
                        // No action, iOS SDK is subscribed to iOS lifecycle notifications.
#elif UNITY_ANDROID
            FTDAndroid.setHttpCallback(new FTDHttpCallback(ftHttpCallback));
#else
                        Debug.Log(errorMsgPlatform);
#endif

        }
        /**
         * 上报渠道SDK归因
         * @param coversionData 渠道回调返回的归因
         */
        public void sendAttributeData(String channel, String jsonAttr)
        {
            Debug.Log("sendAttributeData . >>>>>" + "channel=" + channel + " || json=" + jsonAttr);
            checkOnlineMethod();

            if (!checkAvailable())
            {
                return;
            }
#if UNITY_IOS
            fGetAttributionReturnFromChannel(jsonAttr,channel);
#elif UNITY_ANDROID
            FTDAndroid.sendAttributeData(channel, jsonAttr);
#else
            Debug.Log(errorMsgPlatform);
#endif
           
        }

    /**
     * 上报登录事件
     *
     * @param loginChannel 登录渠道
     * @param name         登录名称
     * @param way          登入/登出 > in/out
     * @param intime       登入时间(登出时为本次登入时间)>1541561218
     * @param outtime      登出时间>1541561230
     * @param customParams 自定义参数，根据需要传入，不需要传null即可
     */
    public void logEventLogin(String loginChannel, String name, String way, long intime, long outtime, Dictionary<String, String> customParams)
        {
            Debug.Log("logEventLogin . >>>>>" + "loginChannel=" + loginChannel + " || customParams=" + dic2json(customParams));
            checkOnlineMethod();

            if (!checkAvailable())
            {
                return;
            }
#if UNITY_IOS
            fLoginedSuccese(loginChannel, name, way, intime+"", outtime+"", dic2json(customParams));
#elif UNITY_ANDROID
            FTDAndroid.logEventLogin(loginChannel, name, way, intime, outtime,customParams);

#else
            Debug.Log(errorMsgPlatform);
#endif
        }
    /**
     * 上报注册事件
     *
     * @param registChannel 注册渠道
     * @param name          注册用户名
     * @param customParams  自定义参数，根据需要传入，不需要传null即可
     */
    public void logEventRegist(String registChannel,String name, Dictionary<String, String> customParams)
        {
            Debug.Log("logEventRegist . >>>>>" + "registChannel=" + registChannel + " || customParams=" + dic2json(customParams));
            checkOnlineMethod();

            if (!checkAvailable())
            {
                return;
            }
#if UNITY_IOS
           fRegisteredSuccese(registChannel, name, dic2json(customParams));
#elif UNITY_ANDROID
            FTDAndroid.logEventRegist(registChannel, name,customParams);
#else
            Debug.Log(errorMsgPlatform);
#endif
        }

    /**
      * 上报支付事件
      * @param channel 支付渠道（注意这里并不是传广告渠道，而是支付渠道，比如google支付就可以是“google”，自己定义）
      * @param itemid 商品id
      * @param itemName 商品id
      * @param usdPrice 必须是美分价格，否则无法计算数据,必传数据
      * @param price 支付金额(必须以“分”为单位)
      * @param currency 币种
      * @param customParams 自定义参数，根据需要传入
      */
    public void logEventPurchase(String channel, String itemid, String itemName, int usdPrice, int price, String currency, Dictionary<String, String> customParams)
        {
            Debug.Log("logEventPurchase . >>>>>" + "channel=" + channel + " || itemid=" + itemid + " || itemName=" + itemName + " || usdPrice=" + usdPrice + " || price=" + price + " || currency=" + currency + " || customParams=" + dic2json(customParams));
            checkOnlineMethod();

            if (!checkAvailable())
            {
                return;
            }
#if UNITY_IOS
            fTrackRevenueSuccese(itemid,itemName,usdPrice+"",price+"",currency,channel, dic2json(customParams));
#elif UNITY_ANDROID
            FTDAndroid.logEventPurchase(channel, itemid, itemName, usdPrice,price, currency, customParams);
#else
            Debug.Log(errorMsgPlatform);
#endif
        }

        /**
           * 完成新手引导
           * @param isSuccess 是否成功
           * @param customParams 自定义参数，根据需要传入，不需要传null即可
           */
        public void logEventCompletedTutorial(bool isSuccess, Dictionary<String, String> customParams)
        {
            Debug.Log("logEventCompletedTutorial . >>>>>" + "isSuccess=" + isSuccess + " || customParams=" + dic2json(customParams));
            checkOnlineMethod();

            if (!checkAvailable())
            {
                return;
            }
#if UNITY_IOS
            fNewbieGuideSuccese(isSuccess+"", dic2json(customParams));
#elif UNITY_ANDROID
            FTDAndroid.logEventCompletedTutorial(isSuccess, customParams);
#else
            Debug.Log(errorMsgPlatform);
#endif
        }

    //ios支付验证接口
    public void validateAndTrackInAppPurchase(string productIdentifier , string price, string currency, string transactionId, string itemid, string itemname, string usdPrice, Dictionary<String, String> customParams)
    {

        Debug.Log("validateAndTrackInAppPurchase . >>>>>" + "productIdentifier=" + productIdentifier + ",price=" + price + ",currency=" + currency + ",transactionId=" + transactionId + ",itemid=" + itemid + ",itemname=" + itemname + ",usdPrice=" + usdPrice + " || customParams=" + dic2json(customParams));
        checkOnlineMethod();

        if (!checkAvailable())
        {
            return;
        }

#if UNITY_IOS
        fValidateAndTrackInAppPurchase( productIdentifier,  price,  currency,  transactionId, itemid, itemname, usdPrice,  dic2json(customParams));
#endif
    }

    /**
     * 自定义事件
     * @param eventName 自定义事件名称
     * @param eventId 自定义事件id
     * @param params 自定义事件参数
     */
    public void logCustomEvent(String eventName, String eventId, Dictionary<String, String> customParams)
        {
            Debug.Log("logCustomEvent . >>>>>" + "eventName=" + eventName + " || eventId=" + eventId + " || customParams=" + dic2json(customParams));
           checkOnlineMethod();

            if (!checkAvailable())
            {
                return;
            }
#if UNITY_IOS
            fGetCustomEvent(eventName,eventId, dic2json(customParams));
#elif UNITY_ANDROID
            FTDAndroid.logCustomEvent(eventName, eventId, customParams);
#else
            Debug.Log(errorMsgPlatform);
#endif
        }
    /**
     * 广告展示事件上报
     *
     * @param ad_app          内推：运营配置链接里app.appsflyer.com/和？之间的字符
     *                        <p>
     *                        第三方广告平台：0000000000
     * @param ad_media_source 内推：运营配置链接里的参数pid
     *                        <p>
     *                        第三方广告平台：广告平台名称，例如：Facebook/Vungle/Unity
     * @param ad_campaign     内推：运营配置链接里的参数c
     *                        <p>
     *                        第三方广告平台：placementid
     * @param ad_channel      内推：运营配置链接里的参数af_channel
     *                        <p>
     *                        第三方广告平台： 空值
     * @param ad              内推：运营配置链接里的参数af_ad
     *                        <p>
     *                        第三方广告平台：广告类型vedio/interstitial
     * @param customParams
     */
    public void adDisplayEvent(String ad_app, String ad_media_source, String ad_campaign, String ad_channel, String ad, Dictionary<String, String> customParams)
    {
        Debug.Log("adDisplayEvent . >>>>>" + "ad_app=" + ad_app + "ad_media_source=" + ad_media_source + "ad_campaign=" + ad_campaign + "ad_channel=" + ad_channel + "ad=" + ad + " || customParams=" + dic2json(customParams));
        checkOnlineMethod();

        if (!checkAvailable())
        {
            return;
        }
#if UNITY_IOS
            fGetUserAdvertisingDisplay(ad_app, ad_media_source, ad_campaign, ad_channel, ad, dic2json(customParams));
#elif UNITY_ANDROID
        FTDAndroid.adDisplayEvent(ad_app, ad_media_source, ad_campaign, ad_channel, ad, customParams);
#else
            Debug.Log(errorMsgPlatform);
#endif
    }
    /**
     * 广告点击事件上报
     *
     * @param ad_app          内推：运营配置链接里app.appsflyer.com/和？之间的字符
     *                        <p>
     *                        第三方广告平台：0000000000
     * @param ad_media_source 内推：运营配置链接里的参数pid
     *                        <p>
     *                        第三方广告平台：广告平台名称，例如：Facebook/Vungle/Unity
     * @param ad_campaign     内推：运营配置链接里的参数c
     *                        <p>
     *                        第三方广告平台：placementid
     * @param ad_channel      内推：运营配置链接里的参数af_channel
     *                        <p>
     *                        第三方广告平台： 空值
     * @param ad              内推：运营配置链接里的参数af_ad
     *                        <p>
     *                        第三方广告平台：广告类型vedio/interstitial
     * @param customParams
     */
    public void adClickEvent(String ad_app, String ad_media_source, String ad_campaign, String ad_channel, String ad, Dictionary<String, String> customParams)
    {
        Debug.Log("adClickEvent . >>>>>" + "ad_app=" + ad_app + "ad_media_source=" + ad_media_source + "ad_campaign=" + ad_campaign + "ad_channel=" + ad_channel + "ad=" + ad + " || customParams=" + dic2json(customParams));
        checkOnlineMethod();

        if (!checkAvailable())
        {
            return;
        }
#if UNITY_IOS
            fGetUserADClick(ad_app, ad_media_source, ad_campaign, ad_channel, ad,  dic2json(customParams));
#elif UNITY_ANDROID
        FTDAndroid.adClickEvent(ad_app, ad_media_source, ad_campaign, ad_channel, ad, customParams);
#else
            Debug.Log(errorMsgPlatform);
#endif
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
    public void trackLevelIn(String level, String way, long intime, long outtime, Dictionary<String, String> customParams)
    {
        Debug.Log("trackLevelIn . >>>>>" + "level=" + level + " || way=" + way + " || intime=" + intime + " || outtime=" + outtime + " || customParams=" + dic2json(customParams));
        checkOnlineMethod();

        if (!checkAvailable())
        {
            return;
        }
#if UNITY_IOS
        fLoginLevels(level, way, intime+"", outtime+"",  dic2json(customParams));
#elif UNITY_ANDROID
        FTDAndroid.trackLevelIn( level,  way,  intime,  outtime,  customParams);
#else
            Debug.Log(errorMsgPlatform);
#endif
    }

    /**
  * 完成关卡事件
  *
  * @param isSuccess    是否成功
  * @param level        关卡>12
  * @param customParams 自定义参数，根据需要传入，不需要传null即可
  */
    public void trackLevelDone(bool isSuccess, String level, Dictionary<String, String> customParams)
    {
        Debug.Log("trackLevelDone . >>>>>" + "isSuccess=" + isSuccess + " || level=" + level + " || customParams=" + dic2json(customParams));
        checkOnlineMethod();

        if (!checkAvailable())
        {
            return;
        }
#if UNITY_IOS
        fCompleteLevel(isSuccess+"", level, dic2json(customParams));
#elif UNITY_ANDROID
        FTDAndroid.trackLevelDone( isSuccess,  level, customParams);
#else
            Debug.Log(errorMsgPlatform);
#endif
    }

    /**
     * 道具购买事件
     *
     * @param propid       道具ID > ham1
     * @param propname     道具名称 > "锤子"
     * @param propnum      购买数量 > 1
     * @param coinid       消耗虚拟货币ID> "diamond1"（如果没有ID就和名称保持一致）
     * @param coinname     消耗虚拟货币名称>" 钻石"
     * @param costcoin     消耗金币 > 10000
     * @param customParams
     */
    public void trackPropPurchase(String propid, String propname, int propnum, String coinid, String coinname, int costcoin, Dictionary<String, String> customParams)
    {
        Debug.Log("trackPropPurchase . >>>>>" + "propid=" + propid + " || propname=" + propname + " || propnum=" + propnum + " || coinid=" + coinid + " || coinname=" + coinname + " || costcoin=" + costcoin + " || customParams=" + dic2json(customParams));
        checkOnlineMethod();

        if (!checkAvailable())
        {
            return;
        }
#if UNITY_IOS
        fPropsToBuy(propid, propname, propnum+"", coinid, coinname, costcoin + "", dic2json(customParams));
#elif UNITY_ANDROID
        FTDAndroid.trackPropPurchase( propid,  propname,  propnum, coinid, coinname, costcoin, customParams);
#else
            Debug.Log(errorMsgPlatform);
#endif
    }

    /**
    * 道具使用事件
    *
    * @param propid       道具ID > ham1
    * @param propname     道具名称 > "锤子"
    * @param propnum      使用数量 > 1
    * @param customParams
    */
    public void trackPropUse(String propid, String propname, int propnum, Dictionary<String, String> customParams)
    {
        Debug.Log("trackPropUse . >>>>>" + "propid=" + propid + " || propname=" + propname + " || propnum=" + propnum + " || customParams=" + dic2json(customParams));
        checkOnlineMethod();

        if (!checkAvailable())
        {
            return;
        }
#if UNITY_IOS
        fPropsToUse(propid, propname, propnum+"", dic2json(customParams));
#elif UNITY_ANDROID
        FTDAndroid.trackPropUse( propid,  propname,  propnum, customParams);
#else
            Debug.Log(errorMsgPlatform);
#endif
    }

    /**
   * 完成加载事件上报
   *
   * @param isSuccess    是否成功
   * @param customParams 自定义参数，根据需要传入，不需要传null即可
   */
    public void trackEventLoading(bool isSuccess, Dictionary<String, String> customParams)
    {
        Debug.Log("trackEventLoading . >>>>>" + "isSuccess=" + isSuccess  + " || customParams=" + dic2json(customParams));
        checkOnlineMethod();

        if (!checkAvailable())
        {
            return;
        }
#if UNITY_IOS
        fFinishedLoading(isSuccess+"", dic2json(customParams));
#elif UNITY_ANDROID
        FTDAndroid.trackEventLoading(isSuccess,  customParams);
#else
            Debug.Log(errorMsgPlatform);
#endif
    }
    /**
     * 设置tag接口，此TAG是标识在设备层级的tag，设置即会在tag数组中追加tag。新设置tag不会覆盖旧tag，且不可删除。
     * 一旦设置每次数据上报都会通过通传参数发送给SDK服务器，服务器可根据tag对数据进行归类计算等。
     *
     * @param tags
     */
    public void setTags(String[] tags)
    {
        if (tags == null || tags.Length < 1)
        {
            return;
        }
        Debug.Log("setTags . >>>>>" + "tags=" + "[\"" + string.Join("\",\"", tags) + "\"]");
        if (!checkAvailable())
        {
            return;
        }
#if UNITY_IOS

            string tagStr =  "[\"" + string.Join("\",\"", tags) + "\"]";
            fGetUserWithTags(tagStr);
    
#elif UNITY_ANDROID
        FTDAndroid.setTags(tags);
#else
            Debug.Log(errorMsgPlatform);
#endif
    }
  /**
    * 设置在线时长事件in/out 附加参数，有则更新，无则追加，暂不可删除
    *
    * @param onlineTimeParams
    */
    public void setOnlineTimeParams(Dictionary<String, String> onlineTimeParams)
    {
        if (onlineTimeParams == null)
        {
            return;
        }
        Debug.Log("onlineTimeParams . >>>>>" + "onlineTimeParams=" + dic2json(onlineTimeParams));
        if (!checkAvailable())
        {
            return;
        }
#if UNITY_IOS

           fGetUserWithUserAttributes(dic2json(onlineTimeParams));
    
#elif UNITY_ANDROID
        FTDAndroid.setOnlineTimeParams(onlineTimeParams);
#else
            Debug.Log(errorMsgPlatform);
#endif
    }
    //判断当前环境是否是editor
    private static bool IsEditor()
        {
#if UNITY_EDITOR
                        Debug.Log(errorMsgEditor);
                        return true;
#else
                            return false;
#endif
        }

        //检查SDK是否可用
        private static bool checkAvailable()
        {
            if (IsEditor())
            {
                return false;
            }
            if (!isInit)
            {
                Debug.Log(errorMsgNotInit);
                return false;
            }
            return true;
        }

    /**
     * ios支付验证回调
     * */
#if UNITY_IOS
        //IOS 回调不能删
        public void onPayResultCallback(string response)
        {
            if (FTDSdk.payVerifyCallback != null)
            {
                FTDSdk.payVerifyCallback.callback(response);
            }
        }
    public void onAttributeCallback(string attrJson)
    {
        if (FTDSdk.changedListener != null)
        {
            FTDSdk.changedListener.onAttributionChanged(attrJson);
        }
    }

#endif
    //检查初始化参数是否有效
    private static bool checkInitParams(string appid,string appkey,string signWay)
        {
            if(appid == null || appkey == null || signWay == null)
            {
                Debug.Log("Invalid parameters");

                return false;
            }
            if (appid.Trim().Contains(" ") || appkey.Trim().Contains(" ") || signWay.Trim().Contains(" "))
            {
                Debug.Log("Invalid parameters");

                return false;
            }
            return true;
        }

        private static string dic2json(Dictionary<string , string> dictionary)
        {
            if (dictionary == null)
            {
                return "";
            }
            ArrayList list = new ArrayList();
            
            foreach (KeyValuePair<string, string> pair in dictionary)
            {
                list.Add(String.Format("\"{0}\":\"{1}\"", pair.Key, pair.Value));
            }

            return  "{" + string.Join(",", (string[])list.ToArray(typeof(string))) + "}";
        }
        private static bool checkOnlineMethod()
    {
            if (!isOnFTApplicationPause)
            {
#if UNITY_ANDROID
            Debug.Log("FTDSDK : no OnFTApplicationPause !!!!");
#endif

            return false;
            }
            if (!isOnFTStart)
                {
                Debug.Log("FTDSDK : no OnFTStart !!!!");
                return false;
            }
       
            return true;
        }
    }

