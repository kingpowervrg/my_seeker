//#define TEST
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EngineCore;
using UnityEngine;
using UnityEngine.UI;

#if UNITY_IOS || UNITY_ANDROID || UNITY_EDITOR
//using Facebook.Unity;
#endif

namespace SeekerGame
{
    public class AddFriendNewUI : GameUIComponent
    {
        //GameLabel m_title_text;
        GameInputField m_input;
        GameImage m_add_btn;
        //GameButton m_facebook_invite_btn;
        GameLabel m_add_btn_text;
        //GameLabel m_player_id_text;
        //GameLabel m_invite_text;
        
        

        protected override void OnInit()
        {
            //m_title_text = this.Make<GameLabel>("Panel:Text_title");
            //m_title_text.Text = LocalizeModule.Instance.GetString("friend_account");
            m_input = this.Make<GameInputField>("Panel:Image_back:InputField");
            m_input.input.placeholder.GetComponent<Text>().text = LocalizeModule.Instance.GetString("friend_input_ID");
            m_add_btn = m_input.Make<GameImage>("Button");
            m_add_btn_text = m_add_btn.Make<GameLabel>("Text");
            m_add_btn_text.Text = LocalizeModule.Instance.GetString("friend_invite_btn");
            //m_facebook_invite_btn = this.Make<GameButton>("Panel:Button2");
            //m_player_id_text = this.Make<GameLabel>("Panel:Text_ID");
            //m_invite_text = this.Make<GameLabel>("Panel:Text_detail");
            //m_invite_text.Text = LocalizeModule.Instance.GetString("friend_invite_ID");
            //this.SetCloseBtnID("Panel:Button_close");
           
        }

        public override void OnShow(object param)
        {
            m_add_btn.AddClickCallBack(OnAddClick);
            //m_facebook_invite_btn.AddClickCallBack(OnInviteClick);
            m_input.AddChangeCallBack(OnInputting);
            //m_player_id_text.Text = string.Format(LocalizeModule.Instance.GetString("friend_self_ID") + ":{0}", GlobalInfo.MY_PLAYER_ID.ToString());
            

            if (string.IsNullOrEmpty(m_input.Text))
            {
                m_add_btn.Enable = false;
                m_add_btn.SetGray(true);
            }
            else
            {
                m_add_btn.Enable = true;
                m_add_btn.SetGray(false);
            }

        }

        public override void OnHide()
        {
            m_add_btn.RemoveClickCallBack(OnAddClick);
            //m_facebook_invite_btn.RemoveClickCallBack(OnInviteClick);
            m_input.RemoveChangeCallBack(OnInputting);
            
        }

        private void OnInputting(string content_)
        {
            if (string.IsNullOrEmpty(content_))
            {
                m_add_btn.Enable = false;
                m_add_btn.SetGray(true);
            }
            else
            {
                m_add_btn.Enable = true;
                m_add_btn.SetGray(false);
            }
        }




        private void OnAddClick(GameObject obj)
        {
            if (string.IsNullOrEmpty(m_input.Text))
                return;
            long val;
            if (false == long.TryParse(m_input.Text, out val))
                return;

            if (val == GlobalInfo.MY_PLAYER_ID)
            {
                PopUpData pd = new PopUpData();
                pd.title = string.Empty;
                pd.content = "friend_number_self_ID";
                pd.content_param0 = "";
                pd.isOneBtn = true;
                pd.OneButtonText = "UI.OK";
                pd.twoStr = "UI.OK";
                pd.oneAction = null;
                pd.twoAction = null;

                PopUpManager.OpenPopUp(pd);

                return;

            }

#if !TEST
            //CurViewLogic().RequestAddFriend(val);
            GameEvents.UIEvents.UI_Friend_Event.Listen_AddFriend.SafeInvoke(val);
#endif
        }

        //        private void OnInviteClick(GameObject obj)
        //        {
        //#if UNITY_IOS
        //            CommonHelper.FBShareLink("https://play.google.com/apps/testing/com.fotoable.thechief", /*callback:*/ HandleResult);
        //#elif UNITY_ANDROID
        //            CommonHelper.FBShareLink("https://play.google.com/store/apps/details?id=com.fotoable.thechief", HandleResult);
        //#elif UNITY_EDITOR
        //            CommonHelper.FBShareLink("https://play.google.com/store/apps/details?id=com.fotoable.thechief", HandleResult);
        //#endif
        //        }

        //protected void HandleResult(string error_)
        //{
        //    if (!string.IsNullOrEmpty(error_))
        //    {
        //        PopUpManager.OpenNormalOnePop(error_);
        //    }
        //}
    }
}
