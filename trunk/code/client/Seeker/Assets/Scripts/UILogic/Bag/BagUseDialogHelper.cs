using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EngineCore;

namespace SeekerGame
{

    public class BagUseData
    {
        public PropData prop;
        public PropInfoTypeEnum infoType;

        public BagUseData(PropData p,PropInfoTypeEnum info)
        {
            prop = p;
            infoType = info;
        }
    }

    public class BagUseDialogHelper
    {
        //带参数打开背包使用界面
        public static void EnterBagUseDialog(object obj)
        {
            FrameMgr.OpenUIParams param = new FrameMgr.OpenUIParams(UIDefine.UI_BAGUSE);
            param.Param = obj;
            param.OpenByFrameName = UIDefine.UI_BAG;
            EngineCoreEvents.UIEvent.ShowUIEventWithParam.SafeInvoke(param);
        }

    }
}

