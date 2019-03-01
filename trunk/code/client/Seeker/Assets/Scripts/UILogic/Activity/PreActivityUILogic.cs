/********************************************************************
	created:  2018-7-25 19:20:21
	filename: PreActivityUILogic.cs
	author:	  songguangze@outlook.com
	
	purpose:  预热活动UI
*********************************************************************/
using EngineCore;

namespace SeekerGame
{
    [UILogicHandler(UIDefine.UI_PRE_ACTIVITY)]
    public class PreActivityUILogic : UILogicBase
    {
        private GameTexture m_imgActivity = null;
        private GameLabel m_lbActivityName = null;
        private GameLabel m_lbActivityTime = null;

        private ActivityBaseInfo m_activityBaseInfo = null;

        protected override void OnInit()
        {
            this.m_imgActivity = Make<GameTexture>("RawImage");
            this.m_lbActivityName = Make<GameLabel>("Text_title");
            this.m_lbActivityTime = Make<GameLabel>("Text_time");
        }

        public override void OnShow(object param)
        {
            if (param != null)
            {
                m_activityBaseInfo = param as ActivityBaseInfo;

                MessageHandler.RegisterMessageHandler(MessageDefine.SCActivityDropResponse, OnSyncDropActivityResponse);
                CSActivityRequest requestDropActivityDetail = new CSActivityRequest();
                requestDropActivityDetail.Id = this.m_activityBaseInfo.Id;
#if !NETWORK_SYNC || UNITY_EDITOR
                GameEvents.NetWorkEvents.SendAsyncMsg.SafeInvoke(requestDropActivityDetail);
#else
                    GameEvents.NetWorkEvents.SendMsg.SafeInvoke(requestDropActivityDetail);
#endif


                this.m_lbActivityTime.Text = $"{CommonTools.TimeStampToDateTime(this.m_activityBaseInfo.StartTime).ToString("yyyy-MM-dd HH:mm:ss")} ~ {CommonTools.TimeStampToDateTime(this.m_activityBaseInfo.EndTime).ToString("yyyy-MM-dd HH:mm:ss")}";
            }

            SetCloseBtnID("Button_close");
        }

        public override FrameDisplayMode UIFrameDisplayMode
        {
            get
            {
                return FrameDisplayMode.WINDOWED;
            }
        }
        private void OnSyncDropActivityResponse(object message)
        {
            MessageHandler.UnRegisterMessageHandler(MessageDefine.SCActivityDropResponse, OnSyncDropActivityResponse);
            SCActivityDropResponse msg = message as SCActivityDropResponse;

            this.m_lbActivityName.Text = LocalizeModule.Instance.GetString(msg.Name);
            this.m_imgActivity.TextureName = msg.BackgroundSource;

        }

    }
}