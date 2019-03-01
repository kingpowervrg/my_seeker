using EngineCore;
using System.Text;
namespace SeekerGame
{
    [UILogicHandler(UIDefine.UI_ProloguePlayVideo)]
    public class ProloguePlayVideoUILogic : UILogicBase
    {
        private GameVideoComponent m_video = null;
        private GameLabel m_textLab = null;
        private float durationTime = 0f;
        protected override void OnInit()
        {
            base.OnInit();
            this.m_video = Make<GameVideoComponent>("RawImage");
            this.m_textLab = Make<GameLabel>("RawImage:Text");
        }

        public override void OnShow(object param)
        {
            base.OnShow(param);
            if (param != null)
            {
                GlobalInfo.Enable_Music = false;
                string videoName = (string)param;
                this.m_video.m_playComplete = PlayComplete;
                this.m_video.VideoName = videoName;
                
                //this.m_video.PlayVideo();
                ///TimeModule.Instance.SetTimeout(PlayComplete,3f);
            }
        }

        public override FrameDisplayMode UIFrameDisplayMode
        {
            get
            {
                return FrameDisplayMode.FULLSCREEN;
            }
        }

        public override void OnHide()
        {
            base.OnHide();
            GlobalInfo.Enable_Music = true;
        }

        string descTxt = string.Empty;
        private int len = 0;
        private int index = 0;
        StringBuilder descBuild ;
        private void PlayComplete()
        {
            this.m_textLab.Text = string.Empty;
            this.m_textLab.Visible = true;
            this.descTxt = LocalizeModule.Instance.GetString("guide_0_06");
            len = this.descTxt.Length;
            descBuild = new StringBuilder(len);
            TimeModule.Instance.SetTimeout(PlayText,0.1f,true);
        }

        private void PlayText()
        {
            if (index >= len)
            {
                TimeModule.Instance.RemoveTimeaction(PlayText);
                TimeModule.Instance.SetTimeout(()=> {
                    EngineCoreEvents.UIEvent.HideUIEvent.SafeInvoke(UIDefine.UI_ProloguePlayVideo);
                }, 1.5f);
                return;
            }
            descBuild.Append(this.descTxt[index]);
            index++;
            this.m_textLab.Text = descBuild.ToString();
        }
    }

    public class ProloguePlayVideoManager
    {
        public static void PlayVideo(string videoName)
        {
            RegisterGuest();
            FrameMgr.OpenUIParams param = new FrameMgr.OpenUIParams(UIDefine.UI_ProloguePlayVideo);
            param.Param = videoName;
            EngineCoreEvents.UIEvent.ShowUIEventWithParam.SafeInvoke(param);
        }

        public static void RegisterGuest()
        {
            //偷偷的注册一下
            if (null == PlayerPrefTool.GetUsername(ENUM_LOGIN_TYPE.E_GUEST))
            {
                MessageHandler.RegisterMessageHandler(MessageDefine.SCRegGuestResponse, OnScResponse);
                LoginUtil.RequestRegisterGuest();
            }
        }

        public static void UnRegisterGuest()
        {
            MessageHandler.UnRegisterMessageHandler(MessageDefine.SCRegGuestResponse, OnScResponse);
        }

        private static void OnScResponse(object s)
        {
            if (s is SCRegGuestResponse)
            {
                MessageHandler.UnRegisterMessageHandler(MessageDefine.SCRegGuestResponse, OnScResponse);
                GOEngine.DebugUtil.Log("receive SCRegGuestResponse");
                if (s != null)
                {
                    var rsp = s as SCRegGuestResponse;
                    if (!string.IsNullOrEmpty(rsp.GuestIdentify))
                    {
                        PlayerPrefTool.SetUsername(rsp.GuestIdentify, ENUM_LOGIN_TYPE.E_GUEST);
                        GOEngine.DebugUtil.Log("GuestIdentify:" + rsp.GuestIdentify);
                    }
                }

            }
        }
    }
}
