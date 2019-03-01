using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SeekerGame
{
    public enum GameCustomeMusicKey
    {
        Main_UI,
        Game_01,
        Event_01,
    }


    public enum GameCustomAudioKey
    {
        bag_open,//    bag_open.mp3    点击打开背包的音效
        item_use,//    item_use.mp3    物品使用的音效
        item_sold,//   item_sold.mp3   物品卖出的音效
        gift_open,//   gift_open.mp3   打开礼包的音效
        bag_soldadd,// tongyong_dianjianniu.mp3    出售数量的增加减少的音效
        shop_open,//   shop_open.mp3   打开商城的音效
        shop_buycoin,//    shop_buy.mp3    金币购买的音效
        shop_buycash,//    shop_buy.mp3    钞票购买的音效
        shop_add,//    tongyong_dianjianniu.mp3    增加减少购买数量的音效
        shop_refresh,//    shop_refresh.mp3    刷新黑市的音效
        achievement_open,//    achievement_open.mp3    点击成就系统按钮音效
        achievement_detail,//  tongyong_dianjianniu.mp3    点击成就展示详情的音效
        achievement_get,// achievement_get.mp3 领取成就的音效
        policeman_open,//  achievement_open.mp3    点击警员页签的音效
        policeman_hire,//  policeman_hire.mp3  聘用警员的音效
        policeman_promote,//   policeman_hire.mp3  晋升警员的音效
        file_open,//   file_open.mp3   点击档案系统按钮的音效
        file_page,//   file_page.mp3   滑动案件时的音效
        friend_open,// tongyong_tablechange.mp3    点击好友系统按钮音效
        friend_heads,//    tongyong_dianjianniu.mp3    点击好友头像音效
        friend_gift,// tongyong_dianjianniu.mp3    点击送好友礼品按钮音效
        friend_unfriend,// friend_unfriend.mp3 确认删除好友音效
        friend_send,// friend_send.mp3 申请已发出
        friend_consent,//  friend_consent.mp3  同意好友申请音效
        friend_getgift,//  achievement_get.mp3 领取好有奖励音效
        friend_addfriend,//    tongyong_dianjianniu.mp3    搜索添加好友音效
        friend_consentapply,// friend_consentapply.mp3 接受好友申请开关的音效
        mail_open,//   tongyong_dianjianniu.mp3    点击邮件系统按钮音效
        mail_openletter,// shop_refresh.mp3    打开单封邮件时的音效
        mail_giftget,//    achievement_get.mp3 领取邮件奖励时的音效
        mail_change,// file_page.mp3   邮件左右翻页时的音效
        role_open,//   tongyong_dianjianniu.mp3    点击角色详情按钮音效
        role_butten,// friend_consentapply.mp3 功能开关的音效
        role_title,//  tongyong_dianjianniu.mp3    选择称号的音效
        qiandao_open,//    achievement_get.mp3 点击签到按钮获取奖励的音效
        lotto_playing,//   lotto_playing.mp3   点击抽奖按钮后的配合特效的轮盘音效
        lotto_show,//  lotto_show.mp3  抽奖结束显示奖励的音效
        activity_change,// file_page.mp3   活动卡片滑动的音效
        activity_open,//   activity_open.mp3   活动卡片滑入&滑出的音效
        task_button,// tongyong_dianjianniu.mp3    点击主界面任务按钮时的音效
        task_confrim,//    achievement_get.mp3 任务完成界面确认按钮音效
        task_next,//   tongyong_dianjianniu.mp3    任务对话点击下一步的音效
        event_choice,//    tongyong_dianjianniu.mp3    事件的选择警员后的确认音效
        task_policechoice,//   task_policechoice.mp3   选择上阵的警员的音效
        task_policecancel,//   task_policecancel.mp3   取消已上阵警员的音效
        task_policepage,// file_page.mp3   滑动警员时的音效
        game_star,//   game_star.mp3   开局时的音效
        game_exhibit,//    game_exhibit.mp3    点击目标物品音效
        game_error,//  game_error.mp3  点击错误目标的音效
        error,//   error.mp3   多次点击错误扣除时间的音效
        policeman_skill,// policeman_skill.mp3 点击警员主动技能的音效
        timeover,//    timeover.mp3    时间耗完倒计时的音效
        sucess,//  sucess.mp3  局内成功结算时的音效
        fail,//    fail.mp3    局内失败结算时的音效
        item_xunwujing,//  item_xunwujing.mp3  使用寻物镜的音效
        item_tanceyi,//    item_tanceyi.mp3    使用探测仪的音效
        item_boom,//   item_boom.mp3   使用炸弹的音效
        item_jiashiqi,//   item_jiashiqi.mp3   使用加时器的音效
        item_shizhijin,//  item_shizhijin.mp3  使用拭纸巾的音效
        item_yingjideng,// item_yingjideng.mp3 使用应急灯的音效
        jigsaw_done,// jigsaw_done.mp3 拼图完成的音效
        jigsaw_click,//    jigsaw_click.mp3    点击拼图音效
        jigsaw_put,//  jigsaw_click.mp3    放置拼图音效【未衔接拼图】
        jigsaw_putdone,//  jigsaw_putdone.mp3  放置拼图音效【衔接拼图成功】
        knock,//   knock.mp3   开场漫画中的敲门音效  knock
        zoom_in, //放大镜镜头进入
        zoom_out, //放大镜镜头出来
        main_ui_shousuo,// 9.wav   点击一键收缩icon的音效
        main_ui_zhankai,//		10.wav  点击一键展开icon的音效
        main_ui_extend,//		11.wav  ICON底条UI展开的特效
        sencond_pop,//		12.wav  长按出现的二级悬浮半透弹窗的UI展开音效
        slider_change,//		13.wav  获得体力、减少体力时，进度条递增、递减动画时的音效
        prop_into_bag,//		17.wav  获得物品进入背包的声音（包括购买成功，各个奖励中点击获得后）
        map_flower,//		18.wav  点击地图场景进入的气泡坐标后，缩略图逐个儿阴柔展现出来的“嘤嘤嘤”音效
        unlock_success,//		19.wav  区域地块解锁成功后庆祝的音效
        vit_full,//		20.wav  升级界面中，体力条怼满时的音效
        buy_cash_success,//		24.wav  购买钞票成功时的音效
        buy_coin_success,//		23.wav  购买金币成功时的音效
        coin_cost,//		25.wav  消耗金币的音效
        cash_cost,//		26.wav  消耗钞票的音效
        black_market_refresh,//		28.wav  黑市，刷新时，UI界面播放物品清单的刷新音效

    }

    //EngineCoreEvents.AudioEvents.PlayAudio.SafeInvoke(Audio.AudioType.UISound, GameCustomAudioKey.slider_change.ToString());
}
