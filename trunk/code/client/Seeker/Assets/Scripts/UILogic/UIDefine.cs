/********************************************************************
	created:  2018-4-4 9:55:50
	filename: UIDefine.cs
	author:	  songguangze@outlook.com
	
	purpose:  UI Prefab名称定义
*********************************************************************/
using System;
using System.Collections.Generic;
using System.Reflection;

public static class UIDefine
{
    private static Dictionary<int, string> m_uiIDDict = new Dictionary<int, string>();

    //游戏内UI
    public const string UI_GAME_MAIN = "UI_sence_1.prefab";

    //游戏内暂时，设置UI
    public const string UI_GAME_MAIN_SETTING = "UI_sence_pause_animate.prefab";

    //进入游戏前的UI
    //public const string UI_ENGER_GAME_UI = "UI_start_1.prefab";

    //进入寻物游戏前的UI
    public const string UI_FIND_OBJ_ENTER_UI = "UI_start_2.prefab";

    //背包界面
    public const string UI_BAG = "UI_bag_1.prefab"; //UI_bag_1  UI_bag_1_anima

    //背包使用对话框
    public const string UI_BAGUSE = "UI_bag_useprop_animate.prefab";

    //拼图
    public const string UI_JIGSAW = "UI_jigsaws_1.prefab";


    //进入拼图
    public const string UI_ENTER_JIGSAW = "UI_jigsaws_start.prefab";


    //拼图教学
    public const string UI_JIGSAW_STUDY = "UI_jigsaws_2.prefab";

    //登录
    public const string UI_LOGIN = "UI_sence_login.prefab";

    //拼图结算
    public const string UI_WIN = "UI_SceneLevelUp_2.prefab";

    //场景对话
    public const string UI_SCENETALK = "UI_castchat_1.prefab";

    //用户信息UI
    public const string UI_PLAYER_INFO = "UI_playerdetail_1_animate.prefab";

    //场景对话奖励
    public const string UI_TALKREWAR = "UI_castchat_2.prefab";

    //警员信息
#if OFFICER_SYS
    public const string UI_POLICE = "UI_Police.prefab";
#endif

    public const string UI_GAMEENTRY = "UI_mainpanel_2.prefab";

    //用于等待的Loading
    public const string UI_SYNC_LOADING = "UI_Loading_Sync.prefab";

    //游戏上方的Banner
    public const string UI_BANNER = "UI_mainpanel_toparea_2.prefab";

    public const string UI_EVENT_INGAME_ENTRY = "UI_event_start.prefab";
    public const string UI_EVENT_INGAME_PLAY = "UI_event_ingame_new.prefab";
    public const string UI_EVENT_INGAME_SCORE = "UI_event_ingame_2.prefab";

    public const string UI_SHOP = "UI_store_1.prefab";
    public const string UI_SHOPENERGY = "UI_store_energy_animate.prefab";
    public const string UI_SHOPCOIN = "UI_store_coin_animate.prefab";
    public const string UI_IAPCASH = "UI_store_cash_animate.prefab";

    public const string UI_POPUP = "UI_popup_0_animate.prefab";

    public const string UI_POPUP_WITH_CONFIRM = "UI_popup_1_animate.prefab";

    //public const string UI_SELECT_POLICE = "UI_select_police2.prefab";
#if OFFICER_SYS
    public const string UI_SELECT_POLICE_GRID = "UI_select_police3.prefab";
#endif

    //公告
    public const string UI_NOTIC = "UI_activities_1.prefab";

    //场景收集性活动
    public const string UI_SCENE_RANDOM_COLLECTION_ACTIVITY = "UI_activities_common.prefab";

    //活动预热界面 
    public const string UI_PRE_ACTIVITY = "UI_activities_common_2.prefab";

    //抽奖
    public const string UI_SLOTS = "UI_slots_1.prefab";

    //活动界面
    public const string UI_ACTIVITY = "UI_activities_3.prefab";

    //漫画界面
    public const string UI_CARTOON = "UI_Cartoon.prefab";

    public const string UI_ACHIEVEMENT = "UI_achievement_1.prefab";

    public const string UI_ACHIEVEMENT_POP = "UI_achievement_2.prefab";

    //邮件界面
    public const string UI_MAIL = "UI_mail_1.prefab";

    //好友界面
    public const string UI_FRIEND = "UI_friend_1.prefab";

    public const string UI_GUEST_LOGIN = "UI_login_1.prefab";
    //档案(章节界面)
    public const string UI_CHAPTER = "UI_archives_1.prefab";

    //新手引导
    public const string UI_GUID = "UI_Guid.prefab";

    //GM命令窗口
    public const string UI_GM = "UI_GMCmdWindow.prefab";

    public const string UI_GIFTRESULT = "UI_GiftResult.prefab";
    //签到
    public const string UI_SIGNIN = "UI_dailyattendance.prefab";

    //游戏准备
    public const string UI_GAME_READY = "UI_sence_ready.prefab";
    //开场漫画
    public const string UI_COMICS_1 = "UI_comics_1.prefab";

    //新手引导开场漫画
    public const string UI_COMICS_GUID = "UI_comics_newhand_new.prefab";

    //城市界面
    public const string UI_CITY = "UI_city.prefab";

    public const string UI_Loading = "UI_Loading_1.prefab";
    public const string UI_FB_Loading = "UI_FB_Loading_1.prefab";

    public const string UI_UNLOCK = "UI_unlockbuilding_animate.prefab"; //UI_unlockbuilding

    public const string UI_BUILD_TOP = "UI_build_top.prefab";

    //升级奖励
    public const string UI_LEVEL_UP = "UI_levelup.prefab";


    //场景升级奖励
    public const string UI_SCENE_LEVEL_UP = "UI_SceneLevelUp.prefab";

    public const string UI_TOOL_TIPS = "UI_ToolTips.prefab";

    public const string UI_GROUP_TOOL_TIPS = "UI_GroupToolTips.prefab";

    public const string UI_Loading_02 = "UI_Loading_2.prefab";

    public const string UI_GAMESTART_1 = "UI_gamestart_1.prefab"; //选头像和起名字

    public const string UI_GIFTBAG = "UI_giftbag.prefab";

    public const string UI_WaveTips = "UI_waveTips.prefab";

    public const string UI_BIND = "UI_binding.prefab";

    //好友界面
    public const string UI_TASK_ON_BUILD = "UI_task_on_build.prefab";

    public const string UI_ProloguePlayVideo = "UI_Video.prefab";
    //部分顶部条
    public const string UI_MAINPANEL_INGAME = "UI_mainpanel_toparea_ingame.prefab";

    //新手引导登陆
    public const string UI_GUIDLOGIN = "UI_login_newhand.prefab";

    //章节地图
    public const string UI_ChapterMap = "UI_chapterMap.prefab";


    public const string UI_ACHIEVEMENTHINT = "UI_achievement_3.prefab";

    public const string UI_REASONING = "UI_reasoning_1.prefab";
    //尸检游戏
    public const string UI_SCAN_GAME = "UI_postmortem_ingame.prefab";

    //说服玩法
    public const string UI_persuade_Ingame = "UI_persuade_ingame.prefab";


    //任务接受界面
    public const string UI_ACCEPT_TASK = "UI_taskaccept_1.prefab";

    //合成
    public const string UI_COMBINE = "UI_synthesis_1.prefab";

    //天眼计划
    public const string UI_SKYEYE = "UI_skyeye_1.prefab";


    public const string UI_EXHIBITION_HALL = "UI_Exhibition_Hall.prefab";

    public static void InitUIDefinition()
    {

        FieldInfo[] uiDefineFields = typeof(UIDefine).GetFields(System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.FlattenHierarchy);

        foreach (FieldInfo info in uiDefineFields)
        {
            UIIDDefine uiIdDefine;
            if (Enum.TryParse<UIIDDefine>(info.Name, out uiIdDefine))
            {
                string uiName = (string)info.GetValue(null);
                m_uiIDDict.Add((int)uiIdDefine, uiName);
            }
        }

    }


    public static string GetUINameByID(int uiIDDefine)
    {
        return "";
    }

    public static string GetUINameByID(UIIDDefine uiIDDefine)
    {
        return "";
    }
}

public enum UIIDDefine
{
    UI_TASK,
}
