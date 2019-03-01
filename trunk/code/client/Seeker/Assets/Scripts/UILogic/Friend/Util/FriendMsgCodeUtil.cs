using Google.Protobuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
namespace SeekerGame
{
    public class FriendMsgCodeUtil
    {
        public static void OnSuccess(IMessage s)
        {
            string pop_str = string.Empty;
            string pop_param = string.Empty;
            if (s is SCFriendResponse)
            {
                var rsp = s as SCFriendResponse;

                if (FriendReqType.Agreeing == rsp.Type)
                {
                    //if (PlayerPrefTool.RefreshApplicationNoticeTime())
                    //    pop_str = "friend_ask_seven";
                }

            }
            else if (s is SCFriendAddResponse)
            {
                pop_str = "friend_ask_for";
            }
            else if (s is SCFriendDelResponse)
            {


            }
            else if (s is SCFriendAgreeResponse)
            {
                pop_str = "friend_add_ok";
            }
            else if (s is SCFriendDelApplyResponse)
            {

            }
            else if (s is SCFriendGiftResponse)
            {



            }
            else if (s is SCFriendGiftSendResponse)
            {
                pop_str = "friend_gift_send";
                pop_param = FriendDataManager.Instance.Send_gift_left_num.ToString();
            }
            else if (s is SCFriendGiftDrawResponse)
            {

            }
            else if (s is SCFriendViewResponse)
            {
            }
            else if (s is SCFriendRecommendApplyResponse)
            {
                pop_str = "friend_ask_for";
                WaveTipHelper.LoadWaveContent(pop_str);
                return;
            }

            if (!string.IsNullOrEmpty(pop_str))
                PopupInfo(pop_str, pop_param);
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
