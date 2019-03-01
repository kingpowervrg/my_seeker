using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EngineCore;
using System;

namespace SeekerGame
{
    public class PopUpManager
    {

        public static void OpenCashBuyError()
        {
            OpenCashBuyErrorPop("shop_no_note_tips");
        }

        public static void OpenCoinBuyError()
        {
            OpenCashBuyErrorPop("shop_no_coin_tips_2");
        }

        private static void OpenCashBuyErrorPop(string content)
        {
            PopUpData pd = new PopUpData();
            pd.title = string.Empty;
            pd.content = content;
            pd.isOneBtn = false;
            pd.OneButtonText = "shop_no";
            pd.twoStr = "shop_go";
            pd.oneAction = null;
            pd.twoAction = delegate ()
            {
                EngineCoreEvents.UIEvent.ShowUIEvent.SafeInvoke(UIDefine.UI_IAPCASH);
            };
            OpenPop(pd);
        }

        public static void OpenNormalOnePop(string content, string param = "", Action act_ = null)
        {
            PopUpData pd = new PopUpData();
            pd.title = string.Empty;
            pd.content = content;
            pd.isOneBtn = true;
            pd.OneButtonText = "UI.OK";
            pd.oneAction = act_;
            pd.content_param0 = param;
            OpenPop(pd);
        }

        public static void OpenPopUp(PopUpData pd_)
        {
            OpenPop(pd_);
        }

        public static void ClosePopUp()
        {
            EngineCoreEvents.UIEvent.HideUIEvent.SafeInvoke(UIDefine.UI_POPUP);
        }


        private static void OpenPop(PopUpData pd)
        {
            FrameMgr.OpenUIParams param = new FrameMgr.OpenUIParams(UIDefine.UI_POPUP);
            param.Param = pd;
            EngineCoreEvents.UIEvent.ShowUIEventWithParam.SafeInvoke(param);
        }

        public static void OpenGoToVitShop()
        {
            PopUpData pd = new PopUpData();
            pd.isClose = true;
            pd.title = "UI_ENTER_GAME_NO_ENERGY";
            pd.content = "UI_ENERGY_buy";
            pd.isOneBtn = true;
            pd.OneButtonText = "shop_go";
            pd.oneAction = delegate ()
            {
                FrameMgr.OpenUIParams ui_param = new FrameMgr.OpenUIParams(UIDefine.UI_SHOPENERGY);
                ui_param.Param = true;

                EngineCoreEvents.UIEvent.ShowUIEventWithParam.SafeInvoke(ui_param);
            };
            OpenPop(pd);
        }

        public static void OpenGoToCashShop()
        {
            PopUpData pd = new PopUpData();
            pd.isClose = true;
            pd.title = "UI_PLAYERINFO_CASH_NOT_ENOUTH";
            pd.content = "shop_no_note_tips";
            pd.isOneBtn = true;
            pd.OneButtonText = "shop_go";
            pd.oneAction = delegate ()
            {
                EngineCoreEvents.UIEvent.ShowUIEvent.SafeInvoke(UIDefine.UI_IAPCASH);
            };
            OpenPop(pd);
        }

        //public static void OpenGoToCoinShop()
        //{
        //    PopUpData pd = new PopUpData();
        //    pd.isClose = true;
        //    pd.title = "shop_no_coin_tips_1";
        //    pd.content = "shop_no_coin_tips_2";
        //    pd.isOneBtn = true;
        //    pd.OneButtonText = "shop_go";
        //    pd.oneAction = delegate ()
        //    {
        //        EngineCoreEvents.UIEvent.ShowUIEvent.SafeInvoke(UIDefine.UI_SHOPCOIN);
        //    };
        //    OpenPop(pd);
        //}

        public static void OpenGoToCoinShop()
        {
            PopUpData pd = new PopUpData();
            pd.isClose = false;
            pd.title = "shop_rmb_no";
            pd.content = "shop_no_coin_tips_1";
            pd.isOneBtn = true;
            pd.OneButtonText = "UI.OK";
            //pd.oneAction = delegate ()
            //{
            //    EngineCoreEvents.UIEvent.ShowUIEvent.SafeInvoke(UIDefine.UI_SHOPCOIN);
            //};
            OpenPop(pd);
        }

        public static void OnCloseSureUI(string content,string continueStr,string exitStr,Action cb)
        {
            PopUpData pd = new PopUpData();
            pd.isClose = true;
            pd.title = string.Empty;
            pd.content = content;
            pd.isOneBtn = false;
            pd.OneButtonText = exitStr;
            pd.twoStr = continueStr;
            pd.oneAction = cb;
            OpenPop(pd);
        }
    }
}

