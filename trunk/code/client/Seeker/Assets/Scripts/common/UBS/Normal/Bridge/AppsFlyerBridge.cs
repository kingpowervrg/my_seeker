using EngineCore;
using Facebook.Unity;
using GOEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace SeekerGame
{
#if !OUROWNLOG
    [UBSBridgeAttribute(BridgeType.APPSFLYER)]
#endif
    public class AppsFlyerBridge : IUBSBridge
    {
        public bool IsInitialized
        {
            get; set;
        }

        private long CustomUserID
        {
            get; set;
        }
        //public void LogAddedToCartEvent(string contentId, string contentType, string currency, double price)
        //{

        //}

        public void LogEvent(UBSEventKeyName key_)
        {
            SetAppsFlyerDedicatedData();

            Dictionary<UBSParamKeyName, object> raw_params = AppendBaseData();

            AppsFlyer.trackRichEvent(TranslateEventKey(key_), TranslateParamData(raw_params));
        }

        public void LogEvent(UBSEventKeyName key_, float? value4sum_)
        {
            SetAppsFlyerDedicatedData();

            Dictionary<UBSParamKeyName, object> raw_params = AppendBaseData();

            if (!PurchaseEvent(key_, value4sum_, raw_params))
            {
                //if (null != value4sum_)
                //{
                //    Dictionary<string, string> additional_params = TranslateParamData(raw_params);
                //    additional_params[AFInAppEvents.REVENUE] = value4sum_.ToString();
                //    AppsFlyer.trackRichEvent(TranslateEventKey(key_), additional_params);
                //}
                //else
                //{
                //    AppsFlyer.trackRichEvent(TranslateEventKey(key_), TranslateParamData(raw_params));
                //}

                AppsFlyer.trackRichEvent(TranslateEventKey(key_), TranslateParamData(raw_params));
            }
        }

        public void LogEvent(UBSEventKeyName key_, float? value4sum_, Dictionary<UBSParamKeyName, object> params_)
        {
            SetAppsFlyerDedicatedData();

            Dictionary<UBSParamKeyName, object> raw_params = AppendBaseData(params_);

            if (!PurchaseEvent(key_, value4sum_, raw_params))
            {
                //if (null != value4sum_)
                //{
                //    Dictionary<string, string> additional_params = TranslateParamData(raw_params);
                //    additional_params[AFInAppEvents.REVENUE] = value4sum_.ToString();
                //    AppsFlyer.trackRichEvent(TranslateEventKey(key_), additional_params);
                //}
                //else
                //{
                //    AppsFlyer.trackRichEvent(TranslateEventKey(key_), TranslateParamData(raw_params));
                //}

                AppsFlyer.trackRichEvent(TranslateEventKey(key_), TranslateParamData(raw_params));
            }
        }

        public void StartBridge()
        {

            /* Mandatory - set your AppsFlyer’s Developer key. */
            AppsFlyer.setAppsFlyerKey("aenSuFdSXYGEa8HyMfspZA");
            /* For detailed logging */
            //AppsFlyer.setIsDebug(true);

#if UNITY_IOS
   /* Mandatory - set your apple app ID
      NOTE: You should enter the number only and not the "ID" prefix */
   AppsFlyer.setAppID ("1435121875");
            AppsFlyerTracker.instance.StartTracker();
            AppsFlyer.getConversionData ();
   AppsFlyer.trackAppLaunch ();
#elif UNITY_ANDROID
            /* Mandatory - set your Android package name */
            AppsFlyer.setCollectIMEI(true);
            AppsFlyer.setCollectAndroidID(true);
            AndroidJavaClass up = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            AndroidJavaObject currentActivity = up.GetStatic<AndroidJavaObject>("currentActivity");
            AndroidJavaObject contentResolver = currentActivity.Call<AndroidJavaObject>("getContentResolver");
            AndroidJavaClass secure = new AndroidJavaClass("android.provider.Settings$Secure");
            string android_id = secure.CallStatic<string>("getString", contentResolver, "android_id");
            AppsFlyer.setAndroidIdData(android_id);
            AppsFlyer.setAppID(Application.identifier);
            AppsFlyerTracker.instance.StartTracker();
            /* For getting the conversion data in Android, you need to add the "AppsFlyerTrackerCallbacks" listener.*/
            AppsFlyer.init("aenSuFdSXYGEa8HyMfspZA", "AppsFlyerTrackerCallbacks");
            //AppsFlyer.init("ePewqH67ho63AZDuhQYtkP");
#else
            AppsFlyer.setCollectIMEI(true);
            AppsFlyer.setCollectAndroidID(true);
            AppsFlyer.setImeiData(SystemInfo.deviceUniqueIdentifier);
            AppsFlyer.setAppID(Application.identifier);
            AppsFlyerTracker.instance.StartTracker();
            AppsFlyer.init("aenSuFdSXYGEa8HyMfspZA", "AppsFlyerTrackerCallbacks");
#endif

            IsInitialized = true;

        }

        public void DisposeBridge()
        {

            //throw new NotImplementedException();
        }

        private bool PurchaseEvent(UBSEventKeyName event_name_, float? value_, Dictionary<UBSParamKeyName, object> params_ = null)
        {
            if (null != value_ && UBSEventKeyName.Purchased == event_name_)
            {
                Dictionary<string, string> additional_params = TranslateParamData(params_);
                additional_params[AFInAppEvents.REVENUE] = value_.ToString();
                AppsFlyer.trackRichEvent(TranslateEventKey(event_name_), additional_params);
                return true;
            }

            return false;
        }

        private Dictionary<UBSParamKeyName, object> AppendBaseData(Dictionary<UBSParamKeyName, object> params_ = null)
        {
            Dictionary<UBSParamKeyName, object> fb_params;

            if (null == params_)
            {
                fb_params = new Dictionary<UBSParamKeyName, object>();
            }
            else
            {
                fb_params = new Dictionary<UBSParamKeyName, object>(params_);
            }

            fb_params[UBSParamKeyName.player_id] = UBSBaseData.m_player_id;
            fb_params[UBSParamKeyName.operating_sys] = UBSBaseData.m_operating_sys;
            fb_params[UBSParamKeyName.device_model] = UBSBaseData.m_device_model;
            fb_params[UBSParamKeyName.game_version] = GlobalInfo.GAME_VERSION;
            fb_params[UBSParamKeyName.app_version] = UBSBaseData.m_app_version;
            fb_params[UBSParamKeyName.net_state] = Application.internetReachability;
            fb_params[UBSParamKeyName.time_stamp] = DateTime.UtcNow.ToString("MM-dd HH:mm:ss");
            return fb_params;
        }

        private Dictionary<string, string> TranslateParamData(Dictionary<UBSParamKeyName, object> params_)
        {
            Dictionary<string, string> ret = new Dictionary<string, string>();

            List<string> will_delete_keys = new List<string>();

            foreach (KeyValuePair<UBSParamKeyName, object> kvp in params_)
            {
                ret[TranslateParamKey(kvp.Key)] = kvp.Value.ToString();
            }

            return ret;
        }

        private string TranslateParamKey(UBSParamKeyName name_enum_)
        {
            string ret = name_enum_.ToString();

            switch (name_enum_)
            {
                case (UBSParamKeyName.ContentID):
                    ret = AFInAppEvents.CONTENT_ID;
                    break;
                case (UBSParamKeyName.ContentType):
                    ret = AFInAppEvents.CONTENT_TYPE;
                    break;
                case (UBSParamKeyName.NumItems):
                    ret = AFInAppEvents.QUANTITY;
                    break;
                case (UBSParamKeyName.Currency):
                    ret = AFInAppEvents.CURRENCY;
                    break;
                case (UBSParamKeyName.Success):
                    ret = AFInAppEvents.SUCCESS;
                    break;
                case (UBSParamKeyName.Description):
                    ret = AFInAppEvents.DESCRIPTION;
                    break;
            }

            return ret;
        }


        private string TranslateEventKey(UBSEventKeyName name_enum_)
        {
            string ret = name_enum_.ToString();

            switch (name_enum_)
            {
                case (UBSEventKeyName.Purchased):
                    ret = AFInAppEvents.PURCHASE;
                    break;
            }

            return ret;
        }

        public Dictionary<UBSParamKeyName, object> AppendBaseData()
        {
            return AppendBaseData(null);
        }

        private void SetAppsFlyerDedicatedData()
        {
            if (CustomUserID != GlobalInfo.MY_PLAYER_ID && 0 != GlobalInfo.MY_PLAYER_ID)
            {
                CustomUserID = GlobalInfo.MY_PLAYER_ID;
                AppsFlyer.setCustomerUserID(GlobalInfo.MY_PLAYER_ID.ToString());
            }
        }
    }

    public class AppsFlyerTracker : MonoSingleton<AppsFlyerTracker>
    {
        public void StartTracker()
        {
            if (null == this.gameObject.GetComponent<SeekerGame.AppsFlyerTrackerCallbacks>())
            {
                this.gameObject.AddComponent<SeekerGame.AppsFlyerTrackerCallbacks>();
            }
        }

        public override void Init()
        {
            base.Init();

            this.gameObject.name = "AppsFlyerTrackerCallbacks";
        }
    }
}
