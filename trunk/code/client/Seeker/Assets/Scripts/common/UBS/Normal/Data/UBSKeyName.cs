using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SeekerGame
{
    public enum UBSParamKeyName
    {
        //基础
        player_id,
        operating_sys,
        device_model,
        game_version,
        app_version,
        net_state,
        time_stamp,

        //用户
        ContentID, //事件主体id
        ContentType, //主体种类
        NumItems, //主体数量
        Currency, //货币
        PhaseID, //事件玩法，阶段id
        OfficerID, //警官id
        Description, //描述
        Success, //0失败， 1成功
        SceneID,

        //章节相关
        Select_ChapterID,
        EnterSceneFromChapter,
        EnterChapterFinishSceneFromChapter,
        View_NPCInfo,

        //主游戏
        PropItem_ID,            //道具ID
        PropItem_Num,           //购买数量
        Error_Times,            //错误点击次数
        Punish_Times,           //惩罚次数
        TotalTime,              //消耗总时间

        error_content,
        error_stack,


        send,
        reveive,
    }

    public enum UBSEventKeyName
    {

        Purchased, //购买


        //用户事件
        Notice_Show, //公告打开
        Notice_Close, //公告关闭
        Login_LOGO,
        Login_ver,
        Login_updata,
        Login_SDK,
        Login_carton,
        Login_loading,
        game_start,
        game_policechoice,
        game_begin,
        game_time,
        game_propuse,
        game_propbuy,
        //game_use_propitem,
        game_timefinish,
        game_finish,
        dialogue_star,
        dialogue_finish,
        event_start,
        event_begin,
        event_Phase,
        event_finish,
        Pintu_begin,
        Pintu_start,
        Pintu_finish,
        carton_star,
        carton_finish,
        bag_in,
        Pror_use,
        shop_in,
        shop_buy,
        blackmarket_in,
        blackmarket_buy,
        blackmarket_refresh,
        police_in,
        police_get,
        police_promote,

        //档案系统相关
        file_in,
        file_choice,
        file_start,
        file_start_cartoon,
        file_clue,
        file_NPC,
        file_NPCdialogue,
        file_task,
        file_story,


        achievement_in,
        achievement_get,
        title_in,
        title_get,
        title_use,
        user_in,
        user_music,
        user_soundeffect,
        user_purchaselimit,
        user_notice,
        user_help,
        user_emil,
        emil_send,
        user_account,
        post_in,
        post_read,
        activity_in,
        lotto_in,
        lotto_play,
        build_plan,

        guide_in,
        system_log,          //处理异常及报错

        net_error, //断网了
        back_to_login, //玩家主动取消断网重连
        ip_type,
        touch_task,//点击任务
        get_task_reward,//领取任务奖励

        PreloadAssets, //点击登录后的加载步骤1
        InitGameLogicModules,//点击登录后的加载步骤2
        InitWorld,//点击登录后的加载步骤3
        EnablePing,//点击登录后的加载步骤4

        download_error,         //下载异常

        send_msg_time, //消息从发送到接受的耗时(包含失败)
    }


    public class UBSCurrency
    {
        public static string RMB = "CNY"; //人民币
        public static string OU_YUAN = "Euro"; //欧元
        public static string YING_BANG = "Pound sterling"; //英镑
    }

    public class UBSDescription
    {
        public static string NO_OFFICER_SELECTED = "未选择警员";
        public static string NOT_ENOUGH_VIT = "体力值不足";
        public static string NOT_ENOUGH_OFFICER = "警员数量不足";
        public static string NROMAL = "普通";
        public static string PERFECT = "完美";
        public static string PROPBUY = "道具购买";
        public static string PROPUSE = "道具使用";
        public static string PROPSELL = "道具卖出";
        public static string COIN100 = "100金币";

    }
}
