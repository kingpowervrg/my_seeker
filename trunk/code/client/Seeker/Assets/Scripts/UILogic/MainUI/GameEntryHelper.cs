#define TEST

using EngineCore;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SeekerGame
{
    public class GameEntryHelper
    {
        public static string m_CurrentBtn = "";
        public static bool m_IsFirst = true;
        public static ENUM_UI_TWEEN_DIR S_TWEEN_DIR = ENUM_UI_TWEEN_DIR.E_LEFT;
        public static void btnTransPanel(string panelName)
        {
            if (panelName.Equals(UIDefine.UI_MAIL))
            {

                EngineCoreEvents.UIEvent.ShowUIEvent.SafeInvoke(UIDefine.UI_MAIL);
            }

            else if (panelName.Equals(UIDefine.UI_BAG))
            {

                //CSPlayerPropRequest msg_prop = new CSPlayerPropRequest();
                //GameEvents.NetWorkEvents.SendHalfSyncMsg.SafeInvoke(msg_prop);

                //EngineCoreEvents.UIEvent.ShowUIEvent.SafeInvoke(UIDefine.UI_BAG);
                EngineCoreEvents.UIEvent.ShowUIByOther.SafeInvoke(UIDefine.UI_BAG, UIDefine.UI_GAMEENTRY);

            }
#if OFFICER_SYS
            else if (panelName.Equals(UIDefine.UI_POLICE))
            {
                EngineCoreEvents.UIEvent.ShowUIEvent.SafeInvoke(UIDefine.UI_POLICE);
            }
#endif
            else if (panelName.Equals(UIDefine.UI_EVENT_INGAME_ENTRY))
            {
                CSEventDropInfoRequest req = new CSEventDropInfoRequest();

#if TEST
                req.EventId = 2;
#endif

#if !NETWORK_SYNC || UNITY_EDITOR
                GameEvents.NetWorkEvents.SendAsyncMsg.SafeInvoke(req);
#else
            GameEvents.NetWorkEvents.SendMsg.SafeInvoke(req);
#endif

            }
            else if (panelName.Equals(UIDefine.UI_SHOP))
            {
                if (ShopHelper.IsShopDataAvaliable(Category.Common))
                {
                    ShopHelper.OpenShop(Category.Common);
                }
                else
                {
                    MessageHandler.RegisterMessageHandler(MessageDefine.MarketResponse, GameEntryHelper.TransPanel);

                    MarketRequest req = new MarketRequest();
                    req.Category = Category.Common;

                    GameEvents.NetWorkEvents.SendAsyncMsg.SafeInvoke(req);

                }
            }
            else if (panelName.Equals(UIDefine.UI_SCENETALK))
            {
                TalkUIHelper.OnStartTalk(3);
            }
            else if (panelName.Equals(UIDefine.UI_SLOTS))
            {
                EngineCoreEvents.UIEvent.ShowUIEvent.SafeInvoke(UIDefine.UI_ACTIVITY);
            }
            else if (panelName.Equals(UIDefine.UI_ACHIEVEMENT))
            {
                EngineCoreEvents.UIEvent.ShowUIEvent.SafeInvoke(UIDefine.UI_ACHIEVEMENT);
            }
            else if (panelName.Equals(UIDefine.UI_FRIEND))
            {
                EngineCoreEvents.UIEvent.ShowUIEvent.SafeInvoke(UIDefine.UI_FRIEND);
            }
            else if (panelName == UIDefine.UI_CHAPTER)
            {
                EngineCoreEvents.UIEvent.ShowUIEvent.SafeInvoke(UIDefine.UI_CHAPTER);
            }
            else if (panelName.Equals(UIDefine.UI_COMBINE))
            {
                EngineCoreEvents.UIEvent.ShowUIEvent.SafeInvoke(UIDefine.UI_COMBINE);
            }

        }

        public static void TransPanel(object s)
        {

            if (s is SCPlayerPropResponse)
            {
                //var data = s as SCPlayerPropResponse;
                //GlobalInfo.MY_PLAYER_INFO.SetBagInfos(data.PlayerProps);
                //GlobalInfo.MY_PLAYER_INFO.SetRecentPropInfos(data.RecentProps);
                //if (m_CurrentBtn == UIDefine.UI_BAG)
                //{
                //    FrameMgr.OpenUIParams param = new FrameMgr.OpenUIParams(UIDefine.UI_BAG);
                //    param.Param = s;
                //    EngineCoreEvents.UIEvent.ShowUIEventWithParam.SafeInvoke(param);
                //}


            }

            else if (s is MarketResponse)
            {
                MessageHandler.UnRegisterMessageHandler(MessageDefine.MarketResponse, GameEntryHelper.TransPanel);

                MarketResponse res = (MarketResponse)s;
                if (res.Category == Category.Common)
                {
                    ShopHelper.openShop(res);
                }
            }
            else if (s is SCFriendResponse)
            {
                var rsp = (SCFriendResponse)s;

                if (FriendReqType.Added == rsp.Type)
                {

                }
            }


        }
    }
}

