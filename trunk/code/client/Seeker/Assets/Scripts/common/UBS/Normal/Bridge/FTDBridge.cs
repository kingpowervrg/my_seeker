#define TEST
using System.Collections.Generic;
using UnityEngine;

namespace SeekerGame
{
#if !OUROWNLOG
#if !TEST
    [UBSBridgeAttribute(BridgeType.FTD)]
#endif
#endif
    public class FTDBridge : IUBSBridge
    {
        public bool IsInitialized
        {
            get; set;
        }


        public void LogEvent(UBSEventKeyName key_)
        {
            Dictionary<UBSParamKeyName, object> raw_params = AppendBaseData();

            if (!SpecialEvent(key_, null, raw_params))
            {
#if !TEST
                FTDSdk.getInstance().logCustomEvent(TranslateEventKey(key_), TranslateEventKey(key_), TranslateParamData(raw_params));
                Debug.LogWarning("FTD LOG EVENT key");
#endif
            }
        }

        public void LogEvent(UBSEventKeyName key_, float? value4sum_)
        {
            Dictionary<UBSParamKeyName, object> raw_params = AppendBaseData();

            if (!SpecialEvent(key_, value4sum_, raw_params))
            {
#if !TEST
                FTDSdk.getInstance().logCustomEvent(TranslateEventKey(key_), TranslateEventKey(key_), TranslateParamData(raw_params));
                Debug.LogWarning("FTD LOG EVENT key value");
#endif
            }
        }

        public void LogEvent(UBSEventKeyName key_, float? value4sum_, Dictionary<UBSParamKeyName, object> params_)
        {
            Dictionary<UBSParamKeyName, object> raw_params = AppendBaseData(params_);

            if (!SpecialEvent(key_, value4sum_, raw_params))
            {
#if !TEST
                FTDSdk.getInstance().logCustomEvent(
             TranslateEventKey(key_),
             TranslateEventKey(key_),
             TranslateParamData(raw_params)

                );

                Debug.LogWarning("FTD LOG EVENT key value category");
#endif
            }
        }

        public void StartBridge()
        {
#if !TEST
            FTDInstance.instance.Create();
#if ANDROID
            FTDSdk.init("100038", "7b47498a26385ca94a02337c367bb9fe", "avst");
            Debug.LogWarning("FTD INIT COMPLETE");
#elif IOS
            FTDSdk.init("100039", "e4539fe1132370d1701aa2dd6d934b50", "avst");
#endif
            FTDSdk.init("100038", "7b47498a26385ca94a02337c367bb9fe", "avst");

            IsInitialized = true;
#endif


        }

        public void DisposeBridge()
        {
            IsInitialized = false;

            //throw new NotImplementedException();
        }


        private bool SpecialEvent(UBSEventKeyName event_name_, float? value_, Dictionary<UBSParamKeyName, object> params_ = null)
        {
            if (null != value_ && UBSEventKeyName.Purchased == event_name_)
            {
                //购买
                string currency = null;

                if (params_.ContainsKey(UBSParamKeyName.Currency))
                {
                    currency = params_[UBSParamKeyName.Currency] as string;
                }

                Dictionary<string, string> fb_params = null;

                if (null != params_)
                {
                    fb_params = TranslateParamData(params_);

                }

                long itemid = 0L;
                if (params_.ContainsKey(UBSParamKeyName.ContentID))
                {
                    if (!long.TryParse(params_[UBSParamKeyName.ContentID].ToString(), out itemid))
                    {
                        Debug.LogError("UBS Buy error product id = " + params_[UBSParamKeyName.ContentID].ToString());
                    }
                }

                string itemName = null;
                if (params_.ContainsKey(UBSParamKeyName.Description))
                {
                    itemName = params_[UBSParamKeyName.Description].ToString();
                }

                float usd_val = GameEvents.IAPEvents.Sys_GetUSDPriceEvent.SafeInvoke(itemid);

                float local_val = value_.Value;

                string channel = "win";


#if ANDROID
                channel = "google";
#elif IOS
                channel = "apple";
#endif
#if !TEST
                FTDSdk.getInstance().logEventPurchase(channel, itemid.ToString(), itemName, (int)(usd_val * 100), (int)(local_val * 100), currency, fb_params);
#endif

                return true;
            }



            if (UBSEventKeyName.Login_LOGO == event_name_)
            {
                //普通登录
                Dictionary<string, string> fb_params = null;

                if (null != params_)
                {
                    fb_params = TranslateParamData(params_);
                }

#if !TEST
                foreach (var kvp in fb_params)
                {
                    Debug.LogError(string.Format("log event login : key = {0} , val = {1} ", kvp.Key, kvp.Value));
                }


                string channel = "win";


#if ANDROID
                channel = "google";
#elif IOS
                channel = "apple";
#endif

                FTDSdk.getInstance().logEventLogin(channel, "", "", 0, 0, fb_params);
#endif

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

            return ret;
        }


        private string TranslateEventKey(UBSEventKeyName name_enum_)
        {
            string ret = name_enum_.ToString();

            switch (name_enum_)
            {
                case (UBSEventKeyName.Purchased):
                    ret = "userbuy";
                    break;
            }

            return ret;
        }

        public Dictionary<UBSParamKeyName, object> AppendBaseData()
        {
            return AppendBaseData(null);
        }

        private class FTDInstance : MonoSingleton<FTDInstance>
        {
            public void Create()
            {
#if !TEST
                if (null == this.gameObject.GetComponent<FTDSdk>())
                {
                    this.gameObject.AddComponent<FTDSdk>();
                }
#endif
            }

            public override void Init()
            {
                base.Init();

                this.gameObject.name = "FTDSdk";
            }
        }
    }
}
