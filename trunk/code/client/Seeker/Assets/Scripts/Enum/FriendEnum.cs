using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SeekerGame
{
    public enum ENUM_FRIEND_VIEW_TYPE
    {
        E_NONE,
        //E_ADD_FRIEND, //添加好友
        E_FRIEND_DETAIL, //好友详情
        E_FRIEND_INFO, //好友列表
        //E_GIFT_LIST, //礼物列表
        E_RECOMMEND, //好友推荐

    }

    public enum ENUM_APPLICATION_CONTROL
    {
        E_OK,
        E_DEL,
        E_DEL_ALL,
    }

    public enum ENUM_INFO_CONTROL
    {
        E_DETAIL,
        E_DEL,
        E_GIFT,
    }

    public enum ENUM_GIFT_CONTROL
    {
        E_ONE,
        E_ALL,
    }
}

