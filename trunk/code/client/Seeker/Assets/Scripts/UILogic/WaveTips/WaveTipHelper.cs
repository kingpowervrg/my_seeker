using System;
using System.Collections.Generic;
using EngineCore;
using UnityEngine;

namespace SeekerGame
{
    public class WaveTipHelper
    {
        public static void OpenWaveWindow()
        {
            FrameMgr.OpenUIParams uiparam = new FrameMgr.OpenUIParams(UIDefine.UI_WaveTips);
            uiparam.Param = null;
            EngineCoreEvents.UIEvent.ShowUIEventWithParam.SafeInvoke(uiparam);
        }

        public static void OnHideWaveWindow()
        {
            EngineCoreEvents.UIEvent.HideUIEvent.SafeInvoke(UIDefine.UI_WaveTips);
        }

        public static void LoadWaveContent(string content)
        {
            WaveTipData tipData = new WaveTipData(content,Vector3.zero,-1);
            GameEvents.UIEvents.UI_WaveTip_Event.OnShowTips.SafeInvoke(tipData);
            FrameMgr.OpenUIParams uiparam = new FrameMgr.OpenUIParams(UIDefine.UI_WaveTips);
            uiparam.Param = tipData;
            EngineCoreEvents.UIEvent.ShowUIEventWithParam.SafeInvoke(uiparam);
        }

        public static void LoadWaveContent(string content,float value)
        {
            WaveTipData tipData = new WaveTipData(content, Vector3.zero, value);
            GameEvents.UIEvents.UI_WaveTip_Event.OnShowTips.SafeInvoke(tipData);
            FrameMgr.OpenUIParams uiparam = new FrameMgr.OpenUIParams(UIDefine.UI_WaveTips);
            uiparam.Param = tipData;
            EngineCoreEvents.UIEvent.ShowUIEventWithParam.SafeInvoke(uiparam);
        }

        public static void LoadWaveContent(string content,Vector3 postion)
        {
            WaveTipData tipData = new WaveTipData(content, postion,-1);
            GameEvents.UIEvents.UI_WaveTip_Event.OnShowTips.SafeInvoke(tipData);
            FrameMgr.OpenUIParams uiparam = new FrameMgr.OpenUIParams(UIDefine.UI_WaveTips);
            uiparam.Param = tipData;
            EngineCoreEvents.UIEvent.ShowUIEventWithParam.SafeInvoke(uiparam);
        }

    }
}
