using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace SeekerGame
{
    public class UpdateTool
    {
        public static void OpenAppStore()
        {
            string appID ="";

#if UNITY_ANDROID && !UNITY_EDITOR
            appID = Application.identifier;
            Debug.Log("app id = " + appID);
            Application.OpenURL("market://details?id=" + appID);
#elif UNITY_IOS && !UNITY_EDITOR
            Debug.Log("app id = " + appID);
            Application.OpenURL("itms-apps://itunes.apple.com/app/id" + appID);
#else
            Debug.Log("Can not open Appstore in editor");
#endif
        }
    }
}
