//#define TEST
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
    public class LoginUtil
    {
#if UNITY_EDITOR || TEST
        private static string test_token = "EAAGlA7zI0PsBALClAUX9ke1CU0iXaWVvZASQnFMM00qZCAUlOCTa0M4KNVTlNXjbUFph6GW96uchZCFm51Wy6cMOmS7gU7XCDFfzx66FuaZASgiBHU2TNfU9KNu1fUpFLkP5I8AwT3Afqc1iKDgZAEhuvNl2TZAZBek4MZAGSPFOiuYSL0Aj0PN5DSFmRZCJbrl6J4xcNDGXfg5fntA5WloGtc9AFi7RcC8xAb6MbVUZAJ5HaBAxOZBdzRf";
        private static string test_id = "102343160677347";
        private static long test_expire_time;
#endif
        public static void OnFaceBookStart()
        {
#if UNITY_EDITOR || TEST
            test_token = "EAAGlA7zI0PsBALClAUX9ke1CU0iXaWVvZASQnFMM00qZCAUlOCTa0M4KNVTlNXjbUFph6GW96uchZCFm51Wy6cMOmS7gU7XCDFfzx66FuaZASgiBHU2TNfU9KNu1fUpFLkP5I8AwT3Afqc1iKDgZAEhuvNl2TZAZBek4MZAGSPFOiuYSL0Aj0PN5DSFmRZCJbrl6J4xcNDGXfg5fntA5WloGtc9AFi7RcC8xAb6MbVUZAJ5HaBAxOZBdzRf";
            test_id = "102343160677347";
            test_expire_time = System.DateTime.Now.Ticks / 10000 + 500000;
            if (FB.IsInitialized)
            {
                OnTestInitComplete();
            }
            else
            {
                FB.Init(OnTestInitComplete, OnHideUnity);
            }

#else

#if UNITY_EDITOR || !UNITY_STANDALONE_WIN
            if (FB.IsInitialized)
            {
                Debug.Log("FACEBOOK : fb button clicked Initialized");
                OnInitComplete();
                return;
            }

            FB.Init(OnInitComplete, OnHideUnity);
#endif

#endif
        }

#if UNITY_EDITOR || TEST
        private static void OnTestInitComplete()
        {
            if (!BindGuestToFacebook())
                NativeFaceBookLogin();
        }
#endif



        public static void OnFaceBookChange()
        {
#if UNITY_EDITOR
            test_token = "EAAGlA7zI0PsBALClAUX9ke1CU0iXaWVvZASQnFMM00qZCAUlOCTa0M4KNVTlNXjbUFph6GW96uchZCFm51Wy6cMOmS7gU7XCDFfzx66FuaZASgiBHU2TNfU9KNu1fUpFLkP5I8AwT3Afqc1iKDgZAEhuvNl2TZAZBek4MZAGSPFOiuYSL0Aj0PN5DSFmRZCJbrl6J4xcNDGXfg5fntA5WloGtc9AFi7RcC8xAb6MbVUZAJ5HaBAxOZBdzRf";
            test_id = "102343160677347";
            test_expire_time = System.DateTime.Now.Ticks / 10000 + 500000;
#endif
            if (GameRoot.instance.GameFSM.GotoStateWithParam((int)ClientFSM.ClientState.LOGIN, ENUM_ACCOUNT_TYPE.E_FACEBOOK))
            {

                EngineCoreEvents.ResourceEvent.LeaveScene.SafeInvoke();
                BigWorldManager.Instance.ClearBigWorld();

                return;
            }

            NativeFaceBookLogin();

        }

        public static void OnAccountChangeToThird(ENUM_ACCOUNT_TYPE type_)
        {


            switch (type_)
            {
                case ENUM_ACCOUNT_TYPE.E_APPLE:

                    break;
                case ENUM_ACCOUNT_TYPE.E_FACEBOOK:

                    CommonHelper.FBLogout();

                    LoadingProgressManager.Instance.LoadFacebook();


#if UNITY_EDITOR
                    //OnFaceBookChange();
                    TimeModule.Instance.SetTimeout(() => { OnFaceBookChange(); }, 1.0f);
#else
                    TimeModule.Instance.SetTimeout(() => { OnFaceBookStart(); }, 1.0f);
                    //OnFaceBookStart();
#endif

                    break;
                case ENUM_ACCOUNT_TYPE.E_TWITTER:
                    break;
                default:
                    break;
            }
        }


        public static void OnAccountNativeLogin(ENUM_ACCOUNT_TYPE type_)
        {

            switch (type_)
            {
                case ENUM_ACCOUNT_TYPE.E_APPLE:

                    break;
                case ENUM_ACCOUNT_TYPE.E_FACEBOOK:

                    NativeFaceBookLogin();

                    break;
                case ENUM_ACCOUNT_TYPE.E_TWITTER:
                    break;

                default:
                    break;
            }
        }

        public static void OnAccountChangeToGuest(bool is_not_reg_ = false)
        {


            PlayerPrefTool.SetUsername("", ENUM_LOGIN_TYPE.E_THIRD);
            PlayerPrefTool.SetUsername("", ENUM_LOGIN_TYPE.E_GUEST);

            if (!is_not_reg_)
                RequestRegisterGuest();
        }






        public static void NativeFaceBookLogin()
        {
            Debug.Log("FACEBOOK : NativeFaceBookLogin");
#if UNITY_EDITOR || TEST
            RequestFaceBookLogin(test_token,
                        test_id,
                        test_expire_time,
                        SystemInfo.deviceUniqueIdentifier,
                        GetOsTypeEnum());
#else
            if (AccessToken.CurrentAccessToken != null)
            {

                RequestFaceBookLogin(AccessToken.CurrentAccessToken.TokenString,
                        AccessToken.CurrentAccessToken.UserId,
                        AccessToken.CurrentAccessToken.ExpirationTime.TotalSeconds(),
                        SystemInfo.deviceUniqueIdentifier,
                        GetOsTypeEnum());
            }
            else
            {
                Debug.Log("FACEBOOK : logged in but no token");
            }
#endif

        }


        private static void RequestFaceBookLogin(string accessToken,
       string fbId, long expiresIn, string deviceId, OsType osType)
        {
            CSFBLoginRequest req = new CSFBLoginRequest();
            req.AccessToken = accessToken;
            req.FbId = fbId;
            req.ExpiresIn = expiresIn;
            req.DeviceId = deviceId;
            req.OsType = osType;
            req.Ad = PlayerPrefTool.GetADChannel();
            //GameEvents.NetWorkEvents.SendMsg.SafeInvoke(req);
            GameEvents.NetWorkEvents.SendAsyncMsg.SafeInvoke(req);

            DebugUtil.Log("send CSFBLoginRequest");
        }

        private static bool BindGuestToFacebook()
        {
            if (null != PlayerPrefTool.GetUsername(ENUM_LOGIN_TYPE.E_GUEST))
            {
                Debug.Log("FACEBOOK : BindGuestToFacebook");

                CSFBBindRequest req = new CSFBBindRequest();
                req.Identify = PlayerPrefTool.GetUsername(ENUM_LOGIN_TYPE.E_GUEST);

#if UNITY_EDITOR  || TEST
                req.AccessToken = test_token;
                req.FbId = test_id;
                req.ExpiresIn = test_expire_time;
#else
                req.AccessToken = AccessToken.CurrentAccessToken.TokenString;
                req.FbId = AccessToken.CurrentAccessToken.UserId;
                req.ExpiresIn = AccessToken.CurrentAccessToken.ExpirationTime.TotalSeconds();
#endif
                //GameEvents.NetWorkEvents.SendMsg.SafeInvoke(req);
                GameEvents.NetWorkEvents.SendAsyncMsg.SafeInvoke(req);
                DebugUtil.Log("send CSFBBindRequest");
                return true;
            }

            return false;
        }

        public static bool IsFbLogin()
        {
            Debug.LogWarning($"FB : third name is {PlayerPrefTool.GetUsername(ENUM_LOGIN_TYPE.E_THIRD)} , logined { FB.IsLoggedIn}");
            return null != PlayerPrefTool.GetUsername(ENUM_LOGIN_TYPE.E_THIRD) && FB.IsLoggedIn;
        }

        public static bool IsThereGuset()
        {
            return null != PlayerPrefTool.GetUsername(ENUM_LOGIN_TYPE.E_GUEST);
        }

        private static void OnInitComplete()
        {
            Debug.Log("FACEBOOK : OnInitComplete");

            if (!FB.IsInitialized)
            {
                return;
            }

            Debug.Log("FACEBOOK : OnInitComplete IsInitialized");
            FB.ActivateApp();

            if (FB.IsLoggedIn)
            {
                if (!BindGuestToFacebook())
                    NativeFaceBookLogin();
            }
            else
            {
                CallFBLogin();
            }



        }

        public static void OnHideUnity(bool isGameShown)
        {

        }

        public static bool BackToLogin(ENUM_ACCOUNT_TYPE at_ = ENUM_ACCOUNT_TYPE.E_INVALID)
        {
            if (!GameRoot.instance.GameFSM.CurrentState.StateFlag.Equals((int)ClientFSM.ClientState.LOGIN))
            {
                Debug.Log("FACEBOOK : cur state is not login");
                HttpPingModule.Instance.Enable = false;
                CommonHelper.ShowLoading(true);
                BigWorldManager.Instance.ClearBigWorld();
                EngineCoreEvents.ResourceEvent.LeaveScene.SafeInvoke();

                if (ENUM_ACCOUNT_TYPE.E_INVALID == at_)
                {
                    TimeModule.Instance.SetTimeout(() => GameRoot.instance.GameFSM.GotoState((int)ClientFSM.ClientState.LOGIN), 1.0f);
                }
                else
                {
                    TimeModule.Instance.SetTimeout(() => GameRoot.instance.GameFSM.GotoStateWithParam((int)ClientFSM.ClientState.LOGIN, at_), 1.0f);
                }

                return true;

            }
            else
            {
                CommonHelper.ShowLoading(false);
            }
            return false;
        }


        protected static void HandleResult(IResult result)
        {
            Debug.Log("FACEBOOK : HandleResult\n");

            if (result == null)
            {
                Debug.Log("FACEBOOK : Null Response\n");
                BackToLogin();
                return;
            }


            // Some platforms return the empty string instead of null.
            if (!string.IsNullOrEmpty(result.Error))
            {
                Debug.Log("FACEBOOK :Error - Check log for details");
                Debug.Log("FACEBOOK :Error Response:\n" + result.Error);
                BackToLogin();
            }
            else if (result.Cancelled)
            {
                Debug.Log("FACEBOOK :Cancelled - Check log for details");
                Debug.Log("FACEBOOK :Cancelled Response:\n" + result.RawResult);
                BackToLogin();
            }
            else if (!string.IsNullOrEmpty(result.RawResult))
            {
                Debug.Log("FACEBOOK :Success - Check log for details");
                Debug.Log("FACEBOOK :Success Response:\n" + result.RawResult);

                if (!BindGuestToFacebook())
                {
                    HttpPingModule.Instance.Enable = false;

                    if (BackToLogin(ENUM_ACCOUNT_TYPE.E_FACEBOOK))
                        return;

                    NativeFaceBookLogin();
                }
            }
            else
            {
                Debug.Log("FACEBOOK :Empty Response\n");
                BackToLogin();
            }

        }

        public static void FBLogout()
        {
            if (FB.IsLoggedIn)
            {
                FB.LogOut();
            }
        }

        private static void CallFBLogin()
        {
            Debug.Log("FACEBOOK : CallFBLogin");
            UserBehaviorStatisticsModules.Instance.LogEvent(UBSEventKeyName.Login_SDK);
            FB.LogInWithReadPermissions(new List<string>() { "public_profile", "email" }, HandleResult);
        }

        private static void CallFBLoginForPublish()
        {
            // It is generally good behavior to split asking for read and publish
            // permissions rather than ask for them all at once.
            //
            // In your own game, consider postponing this call until the moment
            // you actually need it.
            FB.LogInWithPublishPermissions(new List<string>() { "publish_actions" }, HandleResult);
        }


        private static string GetOsType()
        {
            string os_type = string.Empty;
#if UNITY_ANDROID
            Debug.Log("这里是安卓设备^_^");
            os_type = "ANDROID";
#endif

#if UNITY_IPHONE
        Debug.Log("这里是苹果设备>_<");
            os_type = "IPHONE";
#endif

#if UNITY_STANDALONE_WIN
            Debug.Log("我是从Windows的电脑上运行的T_T");
            os_type = "WINDOWS";
#endif

            return os_type;
        }

        private static OsType GetOsTypeEnum()
        {
            OsType os_type = OsType.Web;

#if UNITY_ANDROID
            Debug.Log("这里是安卓设备^_^");
            os_type = OsType.Andorid;
#endif

#if UNITY_IPHONE
        Debug.Log("这里是苹果设备>_<");
            os_type = OsType.Ios;
#endif

#if UNITY_STANDALONE_WIN
            Debug.Log("我是从Windows的电脑上运行的T_T");
            os_type = OsType.Web;
#endif

            return os_type;
        }


        public static void RequestRegisterGuest()
        {
            string uid = SystemInfo.deviceUniqueIdentifier;
            string os_type = GetOsType();


            CSRegGuestRequest req = new CSRegGuestRequest();
#if UNITY_DEBUG
            req.DeviceId = "fotoable";
#else
            req.DeviceId = uid;
#endif
            req.OsType = os_type;
            req.Ad = PlayerPrefTool.GetADChannel();

            //GameEvents.NetWorkEvents.SendMsg.SafeInvoke(req);
            GameEvents.NetWorkEvents.SendAsyncMsg.SafeInvoke(req);
        }

        public static void UseThirdAccount(string idytifier_, bool login_ = true)
        {
            RemoveGuestIdytifier();
            RecordThirdIdytifier(idytifier_);
            if (login_)
                RequestCheckToken(idytifier_);
        }


        public static void RemoveGuestIdytifier()
        {
            PlayerPrefTool.SetUsername(string.Empty, ENUM_LOGIN_TYPE.E_GUEST);
        }

        private static void RecordThirdIdytifier(string guest_name_)
        {
            PlayerPrefTool.SetUsername(guest_name_, ENUM_LOGIN_TYPE.E_THIRD);
        }

        public static void RequestCheckToken(string idytifier_)
        {
            CSIdentifyCheckRequest req = new CSIdentifyCheckRequest();
            req.Identify = idytifier_;
            RecordToken(string.Empty); //清空当前token
            //req.Identify = "5247f4c67faa8bc250b2ebc28a5bcb17";
            GameEvents.NetWorkEvents.SendAsyncMsg.SafeInvoke(req);
        }

        public static void RecordToken(string token_)
        {
            GlobalInfo.SetAccountToken(token_);
            PlayerPrefTool.SetToken(token_);
        }
    }
}
