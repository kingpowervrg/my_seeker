using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SeekerGame
{
    public enum GamePropType
    {
        Energy, //体力道具
        Function, //功能道具
        Police, //警员道具
        Gift, //礼盒
        Common, //普通道具
        Coin, //硬币
        Cash, //纸币
        Exp, //经验
        BindingEnergy, //绑定体力
        LimitlessEnergy, //无限体力
    }

    public enum GameNetworkMode
    {
        Standalone,  //单机
        Network //联网
    }
}
