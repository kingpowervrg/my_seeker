using Facebook.Unity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using EngineCore;
using GOEngine;

namespace SeekerGame
{
#if !OUROWNLOG
    [UBSBridgeAttribute(BridgeType.FACEBOOK)]
#endif
    public class FaceBookBridge : IUBSBridge
    {
        public bool IsInitialized
        {
            get; set;
        }

        public void LogAddedToCartEvent(string contentId, string contentType, string currency, double price)
        {
            var parameters = new Dictionary<string, object>();
            parameters[AppEventParameterName.ContentID] = contentId;
            parameters[AppEventParameterName.ContentType] = contentType;
            parameters[AppEventParameterName.Currency] = currency;
            FB.LogAppEvent(
                AppEventName.AddedToCart,
                (float)price,
                parameters
            );
        }

        public void LogEvent(UBSEventKeyName key_)
        {
            Dictionary<UBSParamKeyName, object> raw_params = AppendBaseData();

            FB.LogAppEvent(TranslateEventKey(key_), null, TranslateParamData(raw_params));
        }

        public void LogEvent(UBSEventKeyName key_, float? value4sum_)
        {
            Dictionary<UBSParamKeyName, object> raw_params = AppendBaseData();

            if (!PurchaseEvent(key_, value4sum_, raw_params))
                FB.LogAppEvent(TranslateEventKey(key_), value4sum_, TranslateParamData(raw_params));
        }

        public void LogEvent(UBSEventKeyName key_, float? value4sum_, Dictionary<UBSParamKeyName, object> params_)
        {
            Dictionary<UBSParamKeyName, object> raw_params = AppendBaseData(params_);

            if (!PurchaseEvent(key_, value4sum_, raw_params))
                FB.LogAppEvent(
               TranslateEventKey(key_),
               value4sum_,
               TranslateParamData(raw_params)
           );
        }

        public void StartBridge()
        {
            if (!FB.IsInitialized)
            {
                FB.Init(this.OnInitComplete, OnHideUnity);
                IsInitialized = true;
            }
            else
                IsInitialized = false;
        }

        public void DisposeBridge()
        {

            //throw new NotImplementedException();
        }


        private void OnInitComplete()
        {
            DebugUtil.Log("FACEBOOK BRIDGE INIT COMPLETE");
            FB.ActivateApp();
        }

        private void OnHideUnity(bool isGameShown)
        {

        }

        private bool PurchaseEvent(UBSEventKeyName event_name_, float? value_, Dictionary<UBSParamKeyName, object> params_ = null)
        {
            if (null != value_ && UBSEventKeyName.Purchased == event_name_)
            {
                string currency = null;

                if (params_.ContainsKey(UBSParamKeyName.Currency))
                {
                    currency = params_[UBSParamKeyName.Currency] as string;
                }

                Dictionary<string, object> fb_params = null;

                if (null != params_)
                {
                    fb_params = TranslateParamData(params_);

                }

                float fb_val = value_.Value;
                FB.LogPurchase(fb_val, currency, fb_params);

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

        private Dictionary<string, object> TranslateParamData(Dictionary<UBSParamKeyName, object> params_)
        {
            Dictionary<string, object> ret = new Dictionary<string, object>();

            List<string> will_delete_keys = new List<string>();

            foreach (KeyValuePair<UBSParamKeyName, object> kvp in params_)
            {
                ret[TranslateParamKey(kvp.Key)] = kvp.Value;
            }

            return ret;
        }

        private string TranslateParamKey(UBSParamKeyName name_enum_)
        {
            string ret = name_enum_.ToString();

            switch (name_enum_)
            {
                case (UBSParamKeyName.ContentID):
                    ret = AppEventParameterName.ContentID;
                    break;
                case (UBSParamKeyName.ContentType):
                    ret = AppEventParameterName.ContentType;
                    break;
                case (UBSParamKeyName.NumItems):
                    ret = AppEventParameterName.NumItems;
                    break;
                case (UBSParamKeyName.Currency):
                    ret = AppEventParameterName.Currency;
                    break;
                case (UBSParamKeyName.Success):
                    ret = AppEventParameterName.Success;
                    break;
                case (UBSParamKeyName.Description):
                    ret = AppEventParameterName.Description;
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
                    ret = AppEventName.Purchased;
                    break;
            }

            return ret;
        }

        public Dictionary<UBSParamKeyName, object> AppendBaseData()
        {
            return AppendBaseData(null);
        }
    }
}
