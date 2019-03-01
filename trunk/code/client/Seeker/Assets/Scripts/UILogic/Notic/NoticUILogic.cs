
#define GUEST_LOGIN

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EngineCore;
using UnityEngine.UI;

namespace SeekerGame
{
    [UILogicHandler(UIDefine.UI_NOTIC)]
    public class NoticUILogic : UILogicBase
    {
        private GameImage m_gridRoot_img;
        private GameUIContainer m_imgRoot_con;
        private GameUIContainer m_textRoot_con;
        private GameButton m_close_btn;

        private NoticImage3DScrollComponent m_imageScroll = null;
        private void InitController()
        {
            //m_gridRoot_img = Make<GameImage>("Panel_scroll:grid");

            //m_imgRoot_con = m_gridRoot_img.Make<GameUIContainer>("imgRoot");
            m_textRoot_con = Make<GameUIContainer>("Panel_tipsanimate:noticTextScroll:grid");
            m_close_btn = Make<GameButton>("Button_close");
            this.m_imageScroll = Make<NoticImage3DScrollComponent>("Panel_tipsanimate:Notic_Img");
        }

        protected override void OnInit()
        {
            base.OnInit();
            InitController();
        }
        public override FrameDisplayMode UIFrameDisplayMode
        {
            get
            {
                return FrameDisplayMode.FULLSCREEN;
            }
        }
        public override void OnShow(object param)
        {
            base.OnShow(param);
            MessageHandler.RegisterMessageHandler(MessageDefine.SCNoticeListResponse, OnResponse);
            m_close_btn.AddClickCallBack(BtnClose);
            CSNoticeListRequest res = new CSNoticeListRequest();

#if !NETWORK_SYNC || UNITY_EDITOR
            GameEvents.NetWorkEvents.SendAsyncMsg.SafeInvoke(res);
#else
            GameEvents.NetWorkEvents.SendMsg.SafeInvoke(res);
#endif
            //Debug.Log("enter show over==");


            UserBehaviorStatisticsModules.Instance.LogEvent(UBSEventKeyName.Notice_Show);
        }


        public override void OnHide()
        {
            base.OnHide();
            MessageHandler.UnRegisterMessageHandler(MessageDefine.SCNoticeListResponse, OnResponse);
            m_close_btn.RemoveClickCallBack(BtnClose);

            UserBehaviorStatisticsModules.Instance.LogEvent(UBSEventKeyName.Notice_Close);
        }

        private void OnResponse(object obj)
        {
            if (obj is SCNoticeListResponse)
            {
                //Debug.Log("enter SCNoticeListResponse==");
                SCNoticeListResponse res = obj as SCNoticeListResponse;
                OnInitPanel(res);
                //Debug.Log("enter SCNoticeListResponse over ==" + res.Notices.Count);

            }
        }

        private void OnInitPanel(SCNoticeListResponse res)
        {
            List<NoticeInfo> imgInfos = new List<NoticeInfo>();
            List<NoticeInfo> textInfos = new List<NoticeInfo>();
            for (int i = 0; i < res.Notices.Count; i++)
            {
                NoticeInfo notice = res.Notices[i];
                System.DateTime dt = CommonTools.TimeStampToDateTime(notice.EndTime * 10000);
                if (notice.LevelLimit != 0 || System.DateTime.Now > dt)
                {
                    continue;
                }
                if (notice.Type == 1)
                {
                    //图片
                    imgInfos.Add(notice);
                    //imgInfos.Add(notice);
                }
                else if (notice.Type == 2)
                {
                    //文字
                    textInfos.Add(notice);
                }
            }
            textInfos.Sort((x, y) => -CompareNoticeTex(x, y));
            int imgCount = Mathf.CeilToInt(imgInfos.Count / 2f);
            m_imageScroll.SetData(imgInfos); 
            //m_imageScroll.SetData(imgInfos);

            //m_imgRoot_con.EnsureSize<NoticImgPanel>(imgCount);
            //for (int i = 0; i < imgCount; i++)
            //{
            //    NoticImgPanel imgPanel = m_imgRoot_con.GetChild<NoticImgPanel>(i);
            //    if (i * 2 + 1 >= imgInfos.Count)
            //    {
            //        imgPanel.setData(imgInfos[i * 2], null, i % 2 == 0);
            //    }
            //    else
            //    {
            //        imgPanel.setData(imgInfos[i * 2], imgInfos[i * 2 + 1], i % 2 == 0);
            //    }
            //}

            m_textRoot_con.EnsureSize<NoticTextComponent>(textInfos.Count);
            for (int i = 0; i < textInfos.Count; i++)
            {
                NoticTextComponent textPanel = m_textRoot_con.GetChild<NoticTextComponent>(i);
                textPanel.SetData(textInfos[i]);
                textPanel.Visible = true;
            }
        }

        private float GetLayoutHeigh(GridLayoutGroup layout, int count)
        {
            if (count <= 0)
            {
                return 0f;
            }
            float imgLayoutHei = layout.cellSize.y * count + layout.padding.bottom + layout.spacing.y * (count - 1);
            return imgLayoutHei;
        }

        private void BtnClose(GameObject obj)
        {
            EngineCoreEvents.AudioEvents.PlayAudio.SafeInvoke(Audio.AudioType.UISound,EngineCommonAudioKey.Close_Window.ToString());

            EngineCoreEvents.UIEvent.HideUIEvent.SafeInvoke(UIDefine.UI_NOTIC);
#if GUEST_LOGIN
            //EngineCoreEvents.UIEvent.ShowUIEvent.SafeInvoke(UIDefine.UI_GUEST_LOGIN);
#else
            //EngineCoreEvents.UIEvent.ShowUIEvent.SafeInvoke(UIDefine.UI_LOGIN);
#endif

        }

        private int CompareNoticeTex(NoticeInfo noticInfo0, NoticeInfo noticInfo1)
        {
            if (noticInfo0.StartTime > noticInfo1.StartTime)
            {
                return 1;
            }
            else if (noticInfo0.StartTime == noticInfo1.StartTime)
            {
                return 0;
            }
            return -1;

        }

        /// <summary>
        /// 公告文字
        /// </summary>
        public class NoticTextComponent : GameUIComponent
        {
            private GameLabel m_titleLab = null;
            private GameLabel m_contentLab = null;
            private GameLabel m_timeLab = null;
            protected override void OnInit()
            {
                base.OnInit();
                this.m_titleLab = Make<GameLabel>("Text_title");
                this.m_contentLab = Make<GameLabel>("Text_content");
                this.m_timeLab = Make<GameLabel>("Text_time");
            }

            public void SetData(NoticeInfo noticInfo)
            {
                this.m_titleLab.Text = LocalizeModule.Instance.GetString(noticInfo.Title);
                this.m_contentLab.Text = LocalizeModule.Instance.GetString(noticInfo.Content);
                //Debug.Log(this.m_contentLab.Label.preferredHeight);
                //this.contentSizeFitter.SetLayoutVertical();
                //LayoutRebuilder.MarkLayoutForRebuild(this.m_contentLab.Widget);
                this.m_timeLab.Text = CommonTools.TimeStampToDateTime(noticInfo.UpdateTime * 10000).ToString("dd/MM/yyyy") ;
                TimeModule.Instance.SetTimeout(()=> {
                    Widget.sizeDelta = new Vector2(Widget.sizeDelta.x, (this.m_contentLab.Widget.sizeDelta.y + Mathf.Abs(this.m_contentLab.Widget.anchoredPosition.y)));
                },0.2f);
                
            }
        }
    }
}

