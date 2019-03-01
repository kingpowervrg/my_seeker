using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SeekerGame
{
    public enum ENUM_SEARCH_MODE
    {
        E_INVALID,
        E_SEARCH_ROOM, //室内寻物
        E_JIGSAW,  //结案拼图
        E_EVENTGAME, //事件玩法
        E_CARTOON, //漫画玩法
        E_SCAN, //尸检玩法
    }


    public enum ENUM_POLICE_TYPE
    {
        E_INVALID = -1,
        E_SPECIAL_POLICE = 0, //特警
        E_PATROL_MEN, //巡警
        E_INSPECTOR, //探长
        E_BAU,
        E_CSI,
        E_FORENSIC, //法医
    }

    public enum ENUM_PAGE_TYPE
    {
        E_INVALID = -1,
        E_ALL = 0,
        E_SPECIAL_POLICE, //特警
        E_PATROL_MEN, //巡警
        E_INSPECTOR, //探长
        E_BAU,
        E_CSI,
        E_FORENSIC, //法医
    }


    public enum ENUM_GENDER
    {
        E_NONE, //无
        E_MALE, //男
        E_FEMALE, //女
    }


    public enum EUNM_BASE_REWARD
    {
        //奖励类型0 = 金币1 = 钞票2经验3体力
        E_COIN = 0,
        E_CASH = 1,
        E_EXP = 2,
        E_VIT = 3,
        E_INVALID,
    }


    public enum ENUM_SETTING_BTN_TYPE
    {
        E_THREE = 0,
        E_FOUR,
        E_CHANGEID,
    }

    public enum ENUM_ACCOUNT_TYPE
    {
        E_GUEST,
        E_APPLE,
        E_FACEBOOK,
        E_TWITTER,
        E_GUID,
        E_INVALID,
    }


    public enum IAP_PLATFROM_TYPE
    {
        E_GOOGLE_PLAY = 0,
        E_APPLE_STORE = 1,
    }

    public enum PROP_TYPE
    {
        E_ENERGE = 0,//0体力
        E_FUNC,//1功能
        E_CHIP,//2碎片
        E_GIFT,//3礼盒
        E_NROMAL,//4普通
        E_COIN,//5硬币
        E_CASH,//6纸币
        E_EXP,//7经验
        E_BIND_ENERGE,//8绑定体力
        E_UNLIMITED_ENERGE,//9无限体力
        E_OFFICER, //警员
        E_EXHABIT, //展示物件

    }

    public enum ENUM_UI_TWEEN_DIR
    {
        E_LEFT,
        E_RIGHT,
    }
    //1、寻物失败
    //2、事件失败
    //3、体力值不足
    //4、金币不足
    public enum ENUM_PUSH_GIFT_BLOCK_TYPE
    {
        E_NONE = -1,
        E_LOGIN = 0,
        E_SEEK = 1,
        E_EVENT = 2,
        E_VIT = 3,
        E_COIN = 4,
        E_LVL = 11,
    }

    public enum EUNM_BONUS_POP_VIEW_TYPE
    {
        E_NONE = 0,
        E_LVL_UP, //升级
        E_TASK_REWARD, //任务奖励
        E_DAILY_SIGN, //签到
        E_PUSH_GIFT, //推送礼包
    }

    public enum ENUM_BILLING_ERROR
    {
        E_NONE = 0,
        E_DISABLE, //未添加支付方式
        E_NET_ERROR, //网络不通
    }

    public enum EUNM_TASK_TYPE
    {
        E_MAIN = 1,
        E_BRANCH = 2,
        E_POOL = 3,
    }

    public enum ENUM_RECOMMEND_STATUS
    {
        E_RECOMMEND = 1,
        E_ADDED = 2,
    }

    public enum FRIEND_UI_TOGGLE_TYPE
    {
        Agreeing = 1,
        Addinfo = 2,
        Added = 3,
        gift = 4,
        scarch = 5,
    }

    public enum TASK_ON_BUILD_STATUS
    {
        POOL_TASK,//池任务
        POOL_REWARD,//池任务奖励
        BRANCH_ROLE,//分支警员任务
        BRANCH_TOOL,//分支物品任务
        BRANCH_REWARD,//分支任务奖励
    }


    public enum SCAN_GAME_ITEM_TYPE
    {
        NORMAL,
        SPECIAL,
    }

    public enum ENUM_DROP_TYPE
    {
        PROP,
        EXHABIT,
    }

    public enum ENUM_COMBINE_TYPE
    {
        POLICE = 0,
        COLLECTION = 1,
        OTHER = 2,
        ALL,
    }
}
