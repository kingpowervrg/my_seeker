using UnityEngine;
using UnityEngine.UI;
using Utf8Json;

namespace SeekerGame
{
    public class AppsFlyerTrackerCallbacks : MonoBehaviour
    {
        public Text callbacks;

        public void didReceiveConversionData(string conversionData)
        {
            bool firstLaunch = false;
#if UNITY_ANDROID
            AppsflyerData4Android data = JsonSerializer.Deserialize<AppsflyerData4Android>(conversionData);

            string first_launch_str = data.is_first_launch;
            firstLaunch = false;
            if (!bool.TryParse(first_launch_str, out firstLaunch))
            {
                int fl_int;
                if (int.TryParse(first_launch_str, out fl_int))
                {
                    firstLaunch = 1 == fl_int ? true : false;
                }
            }

            if ("Non-organic".Equals(data.af_status))
            {
                if (firstLaunch)
                {
                    PlayerPrefTool.SetADChannel(data.campaign);
                }
            }
            else
            {
                Debug.LogError("This is an organic install");
            }

#else
            AppsflyerData data = JsonSerializer.Deserialize<AppsflyerData>(conversionData);
            firstLaunch = data.is_first_launch;

            if ("Non-organic".Equals(data.af_status))
            {
                if (firstLaunch)
                {
                    PlayerPrefTool.SetADChannel(data.campaign);
                }
            }
            else
            {
                Debug.LogError("This is an organic install");
            }
#endif
        }

        private void SaveChannel(string id_)
        {

        }


        public void didReceiveConversionDataWithError(string error)
        {
            printCallback("My AppsFlyerTrackerCallbacks:: got conversion data error = " + error);
        }

        public void didFinishValidateReceipt(string validateResult)
        {
            printCallback("My AppsFlyerTrackerCallbacks:: got didFinishValidateReceipt  = " + validateResult);

        }

        public void didFinishValidateReceiptWithError(string error)
        {
            printCallback("My AppsFlyerTrackerCallbacks:: got idFinishValidateReceiptWithError error = " + error);

        }

        public void onAppOpenAttribution(string validateResult)
        {
            printCallback("My AppsFlyerTrackerCallbacks:: got onAppOpenAttribution  = " + validateResult);

        }

        public void onAppOpenAttributionFailure(string error)
        {
            printCallback("My AppsFlyerTrackerCallbacks:: got onAppOpenAttributionFailure error = " + error);

        }

        public void onInAppBillingSuccess()
        {
            printCallback("My AppsFlyerTrackerCallbacks:: got onInAppBillingSuccess succcess");

        }
        public void onInAppBillingFailure(string error)
        {
            printCallback("My AppsFlyerTrackerCallbacks:: got onInAppBillingFailure error = " + error);

        }

        public void onInviteLinkGenerated(string link)
        {
            printCallback("My AppsFlyerTrackerCallbacks:: generated userInviteLink " + link);
        }

        public void onOpenStoreLinkGenerated(string link)
        {
            Application.OpenURL(link);
        }

        void printCallback(string str)
        {
            Debug.Log(str);
        }

    }
}
