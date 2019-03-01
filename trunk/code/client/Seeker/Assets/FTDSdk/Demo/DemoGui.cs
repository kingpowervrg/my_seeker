using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DemoGui : MonoBehaviour
{
    public static List<Action<string>> actionlist = new List<Action<string>>();

    [SerializeField]
    private string android_appId;
    [SerializeField]
    private string android_appKey;
    [SerializeField]
    private string android_signWay;

    [SerializeField]
    private string ios_appId;
    [SerializeField]
    private string ios_appKey;
    [SerializeField]
    private string ios_signWay;

    private static int onlineTimeParamsValue = 1;
    Vector2 scrollPosition;

    void Awake()
    {
#if UNITY_IOS
        FTDSdk. initIos(android_appId, android_appKey, android_signWay);
#elif UNITY_ANDROID
        FTDSdk.initAndroid(android_appId, android_appKey, android_signWay);
#else
#endif
        //FTDSdk.init(android_appId, android_appKey, android_signWay);

        //以下设置回调接口，若不需要则可以忽略不设置
        FtOnattributeChangedListener myListener = new MyListener();
        FtHttpCallback myCallback = new MyHttpCallback();
        FtPayVerifyCallback payVerifyCallback = new MyPayVerifyCallback();
        FTDSdk.getInstance().setOnAttributeListener(myListener);
        FTDSdk.getInstance().setHttpCallback(myCallback);
        FTDSdk.getInstance().setPayVerifyCallback(payVerifyCallback);
        Dictionary<String, String> onlineTimeParams = new Dictionary<String, String>();
        onlineTimeParams.Add("Awake", "aaa");

        if (FTDSdk.getInstance() == null)
        {
            return;
        }
        FTDSdk.getInstance().setOnlineTimeParams(onlineTimeParams);
    }
    //游戏失去焦点也就是进入后台时 focus为false 切换回前台时 focus为true
    void OnApplicationFocus(bool focus)
    {
        Debug.Log("focus = " + focus);

        if (focus)
        {
            //切换到前台时执行，游戏启动时执行一次

        }
        else
        {
            //切换到后台时执行
        }
    }

    void OnApplicationPause(bool pauseStatus)
    {
        Debug.Log("pauseStatus = " + pauseStatus);

        if (FTDSdk.getInstance() == null)
        {
            return;
        }
        FTDSdk.getInstance().OnFTApplicationPause(pauseStatus);
    }
    // Use this for initialization
    void Start()
    {
        //初始化滚动条位置  
        if (FTDSdk.getInstance() == null)
        {
            return;
        }
        FTDSdk.getInstance().OnFTStart();
    }

    // Update is called once per frame
    void Update()
    {
        //遍历action 在Update中调用action自然是主线程调用  
        for (int i = 0; i < actionlist.Count; i++)
        {
            actionlist[i](Time.time + "");
        }
        actionlist.Clear();
    }

    void OnGUI()
    {
        DrawScrollViewt();
    }
   
    /// <summary>
    /// Displays a scrollable list of logs.
    /// </summary>
    void DrawScrollViewt()
    {
        //scrollPosition = GUILayout.BeginScrollView(scrollPosition);
        scrollPosition = GUI.BeginScrollView(new Rect(10, 10, Screen.width-30, Screen.height/2-20), scrollPosition, new Rect(0, 0, Screen.width-46, 600), false, false);
        DrawButton();
        GUI.EndScrollView();

        // Ensure GUI colour is reset before drawing other components.
        GUI.contentColor = Color.white;
    }
    /// <summary>
    /// Displays button
    /// </summary>
    void DrawButton()
    {

        int space = 20;
        int btnAreWidth = (Screen.width - 40) / 4;
        int btnWidth = btnAreWidth - 3 * space / 2;
        int btnHeight = 80;
        GUILayout.BeginVertical();
        GUILayout.Space(space);
        GUILayout.BeginHorizontal();
        GUILayout.Space(space);
        GUI.skin.button.fontSize = 17;
        if (GUILayout.Button("初始化接口", GUILayout.Width(btnWidth), GUILayout.Height(btnHeight)))
        {
            FTDSdk.initAndroid(android_appId, android_appKey, android_signWay);
        }
        GUILayout.Space(space);

        if (GUILayout.Button("adjust归因数据上报", GUILayout.Width(btnWidth), GUILayout.Height(btnHeight)))
        {
            string attrjson = @"{
                    ""media_source"": ""tapjoy_int"",
		            ""agency"": ""starcomm"",
		            ""site_id"": ""57"",
		            ""af_status"": ""Non -organic"",
		            ""af_siteid"": null,
		            ""af_sub1"": null,
		            ""campaign"": ""July4 -Campaign"",
		            ""channel"": ""adjust"",
		            ""af_sub2"": ""subtext1""
                }";
            if (FTDSdk.getInstance() == null)
            {
                return;
            }
            FTDSdk.getInstance().sendAttributeData("adjust", attrjson);
        }
        GUILayout.Space(space);

        if (GUILayout.Button("appsflyer归因数据上报", GUILayout.Width(btnWidth), GUILayout.Height(btnHeight)))
        {
            string attrjson = @"{
                    ""media_source"": ""tapjoy_int"",
		            ""agency"": ""starcomm"",
		            ""site_id"": ""57"",
		            ""af_status"": ""Non -organic"",
		            ""af_siteid"": null,
		            ""af_sub1"": null,
		            ""campaign"": ""July4 -Campaign"",
		            ""channel"": ""adjust"",
		            ""af_sub2"": ""subtext1""
                }";

            if (FTDSdk.getInstance() == null)
            {
                return;
            }
            FTDSdk.getInstance().sendAttributeData("appsflyer", attrjson);
        }
        GUILayout.Space(space);

        if (GUILayout.Button("登录事件", GUILayout.Width(btnWidth), GUILayout.Height(btnHeight)))
        {
            Dictionary<String, String> testCustomParams = new Dictionary<String, String>();
            testCustomParams.Add("userId", "5210045");
            if (FTDSdk.getInstance() == null)
            {
                return;
            }
            FTDSdk.getInstance().logEventLogin("google", "nameLogin","in",500151451,55652124,testCustomParams);
        }
        GUILayout.Space(space);

        GUILayout.EndHorizontal();
        GUILayout.Space(space);

        GUILayout.BeginHorizontal();
        GUILayout.Space(space);

        if (GUILayout.Button("设置TAG", GUILayout.Width(btnWidth), GUILayout.Height(btnHeight)))
        {

            if (FTDSdk.getInstance() == null)
            {
                return;
            }
            FTDSdk.getInstance().setTags(new string[] { "cc", "ace", "accd" });
        }
        GUILayout.Space(space);

        if (GUILayout.Button("注册事件", GUILayout.Width(btnWidth), GUILayout.Height(btnHeight)))
        {

            Dictionary<String, String> testCustomParams = new Dictionary<String, String>();
            testCustomParams.Add("userId", "6665666");
            if (FTDSdk.getInstance() == null)
            {
                return;
            }
            FTDSdk.getInstance().logEventRegist("facebook","nameRegist", null);
        }
        GUILayout.Space(space);

        if (GUILayout.Button("支付事件", GUILayout.Width(btnWidth), GUILayout.Height(btnHeight)))
        {

            Dictionary<String, String> testCustomParams = new Dictionary<String, String>();
            if (FTDSdk.getInstance() == null)
            {
                return;
            }
            FTDSdk.getInstance().logEventPurchase("google", "cindm.xj.djx.1", "500钻石", 499, 499, "USD", testCustomParams);
        }
        GUILayout.Space(space);

        if (GUILayout.Button("完成新手引导", GUILayout.Width(btnWidth), GUILayout.Height(btnHeight)))
        {

            Dictionary<String, String> testCustomParams = new Dictionary<String, String>();
            testCustomParams.Add("time", "5000");
            if (FTDSdk.getInstance() == null)
            {
                return;
            }
            FTDSdk.getInstance().logEventCompletedTutorial(true, testCustomParams);
        }
        GUILayout.Space(space);
        GUILayout.EndHorizontal();
        GUILayout.Space(space);

        GUILayout.BeginHorizontal();
        GUILayout.Space(space);

        if (GUILayout.Button("自定义事件", GUILayout.Width(btnWidth), GUILayout.Height(btnHeight)))
        {

            Dictionary<String, String> testCustomParams = new Dictionary<String, String>();
            testCustomParams.Add("level", "30");
            testCustomParams.Add("level30", "already");

            if (FTDSdk.getInstance() == null)
            {
                return;
            }
            FTDSdk.getInstance().logCustomEvent("Level升级", "xikdhf", testCustomParams);
        }
        GUILayout.Space(space);
        if (GUILayout.Button("IOS支付验证", GUILayout.Width(btnWidth), GUILayout.Height(btnHeight)))
        {

            if (FTDSdk.getInstance() == null)
            {
                return;
            }
            Dictionary<String, String> testCustomParams = new Dictionary<String, String>();
            testCustomParams.Add("customparamskey", "customparamsvalue");
            FTDSdk.getInstance().validateAndTrackInAppPurchase("1", "299", "USD", "transactionId", "itemid", "itemname", "399", testCustomParams);
        }
        GUILayout.Space(space);

        if (GUILayout.Button("添加in/out附加参数", GUILayout.Width(btnWidth), GUILayout.Height(btnHeight)))
        {

            Dictionary<String, String> onlineTimeParams = new Dictionary<String, String>();
            onlineTimeParamsValue++;
            onlineTimeParams.Add("level", onlineTimeParamsValue.ToString());
            onlineTimeParams.Add("level" + onlineTimeParamsValue, "startlevel");

            if (FTDSdk.getInstance() == null)
            {
                return;
            }
            FTDSdk.getInstance().setOnlineTimeParams(onlineTimeParams);
        }
        GUILayout.Space(space);
        if (GUILayout.Button("广告展示1", GUILayout.Width(btnWidth), GUILayout.Height(btnHeight)))
        {

            if (FTDSdk.getInstance() == null)
            {
                return;
            }
            Dictionary<String, String> testCustomParams = new Dictionary<String, String>();
            testCustomParams.Add("adDisplayEvent", "s16888");
            FTDSdk.getInstance().adDisplayEvent("ad_app1", "ad_media_source1", "ad_campaign1", "ad_channel1", "ad1", testCustomParams);
        }
        GUILayout.Space(space);
        GUILayout.EndHorizontal();
        GUILayout.Space(space);

        GUILayout.BeginHorizontal();
        GUILayout.Space(space);
        if (GUILayout.Button("广告点击1", GUILayout.Width(btnWidth), GUILayout.Height(btnHeight)))
        {

            if (FTDSdk.getInstance() == null)
            {
                return;
            }
            Dictionary<String, String> testCustomParams = new Dictionary<String, String>();
            testCustomParams.Add("adClickEvent", "rul5555");
            FTDSdk.getInstance().adClickEvent("ad_app1", "ad_media_source1", "ad_campaign1", "ad_channel1", "ad1", testCustomParams);
        }
        GUILayout.Space(space);
        if (GUILayout.Button("广告展示2", GUILayout.Width(btnWidth), GUILayout.Height(btnHeight)))
        {

            if (FTDSdk.getInstance() == null)
            {
                return;
            }
            Dictionary<String, String> testCustomParams = new Dictionary<String, String>();
            testCustomParams.Add("adDisplayEvent", "url16777");
            FTDSdk.getInstance().adDisplayEvent("ad_app2", "ad_media_source2", "ad_campaign2", "ad_channel2", "ad2", testCustomParams);
        }
        GUILayout.Space(space);
        if (GUILayout.Button("广告点击2", GUILayout.Width(btnWidth), GUILayout.Height(btnHeight)))
        {

            if (FTDSdk.getInstance() == null)
            {
                return;
            }
            Dictionary<String, String> testCustomParams = new Dictionary<String, String>();
            testCustomParams.Add("adClickEvent", "urld666");
            FTDSdk.getInstance().adClickEvent("ad_app2", "ad_media_source2", "ad_campaign2", "ad_channel2", "ad2", testCustomParams);
        }
        GUILayout.Space(space);
        if (GUILayout.Button("加载完成", GUILayout.Width(btnWidth), GUILayout.Height(btnHeight)))
        {

            if (FTDSdk.getInstance() == null)
            {
                return;
            }
            Dictionary<String, String> testCustomParams = new Dictionary<String, String>();
            testCustomParams.Add("trackEventLoading", "urld666");
            FTDSdk.getInstance().trackEventLoading(true, testCustomParams);
        }
        GUILayout.Space(space);
        GUILayout.EndHorizontal();

        GUILayout.Space(space);
        GUILayout.BeginHorizontal();
        GUILayout.Space(space);
        if (GUILayout.Button("登录关卡", GUILayout.Width(btnWidth), GUILayout.Height(btnHeight)))
        {

            if (FTDSdk.getInstance() == null)
            {
                return;
            }
            Dictionary<String, String> testCustomParams = new Dictionary<String, String>();
            testCustomParams.Add("trackLevelIn", "rul5555");
            testCustomParams.Add("trackLevelIn2", "rul5dfd555");

            FTDSdk.getInstance().trackLevelIn("15","in",4511685,666865, testCustomParams);
        }
        GUILayout.Space(space);
        if (GUILayout.Button("完成关卡", GUILayout.Width(btnWidth), GUILayout.Height(btnHeight)))
        {

            if (FTDSdk.getInstance() == null)
            {
                return;
            }
            Dictionary<String, String> testCustomParams = new Dictionary<String, String>();
            testCustomParams.Add("trackLevelDone", "999");
            testCustomParams.Add("trackLevelDone2", "rul5dfd888555");

            FTDSdk.getInstance().trackLevelDone(true, "15", testCustomParams);
        }
        GUILayout.Space(space);
        if (GUILayout.Button("道具购买", GUILayout.Width(btnWidth), GUILayout.Height(btnHeight)))
        {

            if (FTDSdk.getInstance() == null)
            {
                return;
            }
            Dictionary<String, String> testCustomParams = new Dictionary<String, String>();
            testCustomParams.Add("trackPropPurchase", "5210045");
            testCustomParams.Add("userid", "5214301");
            testCustomParams.Add("ip", "168.1.0.1");
            testCustomParams.Add("pannel", "1");

            FTDSdk.getInstance().trackPropPurchase("ham1", "锤子", 2, "diamond1", "钻石", 10000, testCustomParams);
        }
        GUILayout.Space(space);
        if (GUILayout.Button("道具使用", GUILayout.Width(btnWidth), GUILayout.Height(btnHeight)))
        {

            if (FTDSdk.getInstance() == null)
            {
                return;
            }
            Dictionary<String, String> testCustomParams = new Dictionary<String, String>();
            testCustomParams.Add("trackPropUse", "5210045");
            testCustomParams.Add("userid", "5214301");
            testCustomParams.Add("ip", "168.1.0.1");
            testCustomParams.Add("pannel", "1");

            FTDSdk.getInstance().trackPropUse("ham1", "锤子", 2, testCustomParams);
        }
        GUILayout.Space(space);
        GUILayout.EndHorizontal();
        GUILayout.Space(space);
    }

    class MyListener : FtOnattributeChangedListener
    {
        public void onAttributionChanged(string jsonAttr)
        {
            //这里进行游戏逻辑
            actionlist.Add(curname => {
                Debug.Log(" || attr=" + jsonAttr + " >> curname = " + curname);
            });
        }
    }
    class MyHttpCallback : FtHttpCallback
    {
        public void onReponse(int code, string request, string mResponse)
        {
            //这里进行游戏逻辑
            actionlist.Add(curname => {
                if (code == 200)
                {
                    Debug.Log("HTTP SUCCESS.");
                }
                else
                {
                    Debug.Log("HTTP FAILED.");
                }
            });
        }
    }

    class MyPayVerifyCallback : FtPayVerifyCallback
    {
        public void callback(string callback)
        {
            //这里进行游戏逻辑
            actionlist.Add(curname => {
                Debug.Log(" || callback=" + callback + " >> curname = " + curname);
            });
        }
    }
}
