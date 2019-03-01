using EngineCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SeekerGame
{
    public class BindHelper
    {
        public static void ShowBindPromoptView(BindPromoptData data_)
        {
            FrameMgr.OpenUIParams param = new FrameMgr.OpenUIParams(UIDefine.UI_BIND)
            {
                Param = data_,
            };
            EngineCoreEvents.UIEvent.ShowUIEventWithParam.SafeInvoke(param);
        }


        public static void ShowBindRewardView(BindRewardData data_)
        {
            FrameMgr.OpenUIParams param = new FrameMgr.OpenUIParams(UIDefine.UI_BIND)
            {
                Param = data_,
            };
            EngineCoreEvents.UIEvent.ShowUIEventWithParam.SafeInvoke(param);
        }
    }
}
