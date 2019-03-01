using EngineCore;
using Google.Protobuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
namespace SeekerGame
{
    public class MsgStatusCodeUtil
    {
        public const int TOKEN_OFFLINE = -6;//, "已下线，请重新登录"),
        public const int TOKEN_TIMEOUT = -5;//, "token过期，请重新登录"),
        public const int PLAYER_BLACK = -4;//, "黑名单"),
        public const int TOKEN_ERROR = -3;//, "token过期，请重新登录"),
        public const int USER_ERROR = -2;//, "用户错误"),
        public const int NOT_SUPPORT = -1;//, "未实现"),
        public const int SUCCESS = 0;//, "OK"),
        public const int PARAM_ERROR = 1;//, "参数出错"),
        public const int COIN_ERROR = 2;//, "金币不足"),
        public const int CASH_ERROR = 3;//, "钞票不足"),
        public const int PROP_FREW = 4;//, "物品不足"),
        public const int UNlOGINED = 5;//, "未登录"),
        public const int TIME_OUT = 6;//, "时间结束"),
        public const int VIT_OUT = 7;//, "体力不足"),
        public const int EXISTED_USER = 8; //用户已存在不能绑定
        public const int LEVEL_ERROR = 9;//, "等级不足")
        public const int NO_USER = 10;//, "当前Identify无用户"),
        public const int USER_BIND = 11;//, "已绑定"),
        public const int REQUEST_FAST = 12;//, "请求过于频繁"),

        public const int FS_ACCESSTOKEN_ERROR = 21;//, "facebook accesstoken error"),
        public const int FS_ACCESSTOKEN_TIMEOUT = 22;//, "facebook accesstoken timeout"),


        public const int PLAER_NAME_TOO_LONG = 30;//,"用户姓名过长"),
        public const int PLAER_NAME_NULL = 31;//,"用户姓名为空"),

        public const int GOOGLE_CHARGE_TOKEN = 41;//,"签名验证出错"),
        public const int GOOGLE_CHARGE_NONE = 42;//,"支付项目为空"),
        public const int GOOGLE_CHARGE_REPEAT = 43;//,"已购买"),
        public const int GOOGLE_CHARGE_STATE = 44;//,"非正常购买状态"),
        public const int IOS_CHARGE_STATE = 51;//,"测试环境，线上环境混淆"),
        public const int IOS_CHARGE_ERROR = 52;//,"验证出错"),
        public const int IOS_CHARGE_NULL = 53;//,"验证无内容"),

        public const int EXCEPTION = 99;//, "服务异常"),
        public const int MARKET_BLACK_LIMIT = 101;//, "不在当前黑市列表"),
        public const int MARKET_BUY_LIMIT = 102;//, "超出购买限制"),
        public const int MARKET_FRESH_CONFLICT = 103;//, "系统刷新"),
        public const int MARKET_OUT_TIME = 104;//, "售卖过期"),
        public const int MARKET_FRESH = 110;//, "刷新上线"),

        public const int PING_MESSAGE_IS_CLEAR = 111;//, "消息队列已清空"),

        public const int ACHIEVEMENT_NOT_FINISH = 201;//, "未完成"),
        public const int ACHIEVEMENT_SUBMITTED = 202;//, "已提交"),
        public const int TITLE_NONE = 204;//, "没有当前头衔"),
        public const int TASK_NONE = 250; //"没有当前任务"),
        public const int TASK_NO_FINISHED = 251; //"没有完成"),
        public const int TASK_REWARDED = 252; //"任务已经是领取完奖励状态，不要重复提交"),

        public const int FRIEND_SAME = 300;//, "同一个人"),
        public const int FRIENDED = 301;//, "您已添加过该好友"),
        public const int FRIEND_ERROR = 302;//, "账号异常，请重新尝试"),
        public const int FRIEND_LIMIT = 303;//, "当前好友已达上限"),
        public const int FRIEND_ADDED = 304;//, "您已添加过该好友"),
        public const int FRIEND_TARGET_LIMIT = 305;//, "对方好友已达上限"),
        public const int FRIEND_NO_APPLY = 306;//, "无好友请求"),
        public const int FRIEND_NO = 307;//, "没有当前好友"),
        public const int FRIEND_GIFT_SENDED = 308;//, "已发送过礼物"),
        public const int FRIEND_GIFT_LIMIT = 310;//, "发送礼物数超过日限制"),
        public const int FRIEND_GIFT_DRAW_LIMIT = 311;//, "领取礼物数超过日限制"),
        public const int FRIEND_GIFT_NO = 312;//, "无礼物"),
        public const int FRIEND_ADD_CLOSED = 313;//, "对方好友关闭"),

        public const int SCENE_NO = 400;//, "没有当前场景"),
        public const int SCENE_NO_TYPE = 401;//, "没有当前场景类型"),
        public const int SCENE_TIMTOUT = 402; //, "场景超时"),
        public const int SCENE_LOCKED = 403;//, "scene locked"),
        public const int SCENE_EVENT_NO_PASS = 404;//, "事件评分不足"),
        public const int SCENE_EVENT_OFFICE_FULL = 405;//, "当前警察已满"),
        public const int SCENE_NOT_SELECT_OFFICER = 407;//, "没有选择警员"),

        public const int OFFICE_NO = 504;//, "没有当前警察"),
        public const int BUILDING_TASK_UNFINISH = 601;//,"任务未完成"),
        public const int BUILDING_HAS_GOT = 602;//,"建筑已经解锁过了"),



        public static bool OnError(int code_, string customContent = "")
        {
            string error_str = string.Empty;
            string pop_str = string.Empty;

            ConfMsgCode MC = ConfMsgCode.Get(code_);

            if (null == MC)
                return false;

            if (!string.IsNullOrEmpty(MC.errorStr) && !string.IsNullOrWhiteSpace(MC.errorStr))
            {
                error_str = MC.errorStr;
            }

            if (!string.IsNullOrEmpty(MC.popStr) && !string.IsNullOrWhiteSpace(MC.popStr))
                pop_str = MC.popStr;

            if (!string.IsNullOrEmpty(error_str))
            {
                Debug.LogError(error_str);
            }

            StringBuilder errorContent = new StringBuilder();
            errorContent.Append(customContent);

            if (VIT_OUT == code_)
            {
                if (!PushGiftManager.Instance.TurnOn(ENUM_PUSH_GIFT_BLOCK_TYPE.E_VIT))
                {
                    //PopUpManager.OpenGoToVitShop();
                    FrameMgr.OpenUIParams ui_param = new FrameMgr.OpenUIParams(UIDefine.UI_SHOPENERGY);
                    ui_param.Param = true;

                    EngineCoreEvents.UIEvent.ShowUIEventWithParam.SafeInvoke(ui_param);

                }
            }
            else if (!string.IsNullOrEmpty(pop_str))
            {
                errorContent = errorContent.Append(pop_str);
                PopupInfo(errorContent.ToString());
            }

            return !string.IsNullOrEmpty(error_str);
        }


        public static bool OnError(ResponseStatus status)
        {
            if (null == status)
                return false;

            int code_ = status.Code;

            return OnError(code_);
        }


        private static void PopupInfo(string content_, string content_param0_ = "")
        {

            PopUpData pd = new PopUpData();
            pd.title = string.Empty;
            pd.content = content_;
            pd.content_param0 = content_param0_;
            pd.isOneBtn = true;
            pd.twoStr = "UI.OK";
            pd.oneAction = null;
            pd.twoAction = null;

            PopUpManager.OpenPopUp(pd);

        }
    }
}
