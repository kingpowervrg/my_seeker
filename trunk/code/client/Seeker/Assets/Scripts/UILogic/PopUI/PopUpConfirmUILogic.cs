using EngineCore;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace SeekerGame
{
    [UILogicHandler(UIDefine.UI_POPUP_WITH_CONFIRM)]
    public class PopUpConfirmUILogic : UILogicBase
    {


        private GameUIComponent m_pop_root;
        private GameLabel m_title_lab;
        private GameLabel m_content_lab;
        private GameLabel m_ticker_lbl;

        private GameLabel m_one_root;
        private GameButton m_one_btn;
        private GameLabel m_one_lab;

        private GameLabel m_two_root;
        private GameButton m_twoFirst_btn;
        private GameButton m_twoSecond_btn;
        private GameLabel m_twoFirst_lab;
        private GameLabel m_twoSecond_lab;

        private PopUpTickerData m_data = null;
        private PopUpConfirmData m_confirm_data = null;

        private GameButton m_close_btn;

        private ConfirmView m_confirm_root;
        private GameLabel m_confirm_title;
        private GameLabel m_confirm_txt;
        private GameButton m_confirm_btn_no;
        private GameButton m_confirm_btn_yes;

        private int m_ori_order_in_layer;

        private float m_cur_seconds;
        private void InitController()
        {
            m_pop_root = Make<GameUIComponent>("Panel_use");
            m_title_lab = m_pop_root.Make<GameLabel>("title");
            m_content_lab = m_pop_root.Make<GameLabel>("content");
            m_ticker_lbl = m_content_lab.Make<GameLabel>("content_ticker");
            m_one_root = m_pop_root.Make<GameLabel>("one");
            m_one_btn = m_one_root.Make<GameButton>("btn0");
            m_one_lab = m_one_btn.Make<GameLabel>("Text");

            m_two_root = m_pop_root.Make<GameLabel>("two");
            m_twoFirst_btn = m_two_root.Make<GameButton>("btn0");
            m_twoSecond_btn = m_two_root.Make<GameButton>("btn1");
            m_twoFirst_lab = m_twoFirst_btn.Make<GameLabel>("Text");
            m_twoSecond_lab = m_twoSecond_btn.Make<GameLabel>("Text");

            m_ori_order_in_layer = this.Canvas.sortingOrder;

            m_close_btn = this.Make<GameButton>("Button_close");

            m_confirm_root = this.Make<ConfirmView>("confirm");


        }
        public override FrameDisplayMode UIFrameDisplayMode
        {
            get
            {
                return FrameDisplayMode.POPUP;
            }
        }
        private void ShowConfirm()
        {
            m_pop_root.Visible = false;
            m_confirm_root.Visible = true;
        }

        protected override void OnInit()
        {
            base.OnInit();
            NeedLateUpdateByFrame = true;
            InitController();
        }

        private void InitListener()
        {
            m_one_btn.AddClickCallBack(BtnOne);
            m_twoFirst_btn.AddClickCallBack(BtnTwoFirst);
            m_twoSecond_btn.AddClickCallBack(BtnTwoSecond);
            m_close_btn.AddSelectCallBack(onClickClose);

            GameEvents.UIEvents.UI_POPUP_Event.OnConfirm += ShowConfirm;
        }

        private void UnInitListener()
        {
            m_one_btn.RemoveClickCallBack(BtnOne);
            m_twoFirst_btn.RemoveClickCallBack(BtnTwoFirst);
            m_twoSecond_btn.RemoveClickCallBack(BtnTwoSecond);
            m_close_btn.RemoveClickCallBack(onClickClose);

            GameEvents.UIEvents.UI_POPUP_Event.OnConfirm -= ShowConfirm;
        }

        public override void OnShow(object param)
        {
            base.OnShow(param);
            InitListener();

            m_confirm_root.Visible = false;

            if (param != null)
            {
                List<object> datas = param as List<object>;


                m_data = (PopUpTickerData)(datas[0]);
                InitPanel(m_data);
                m_confirm_data = (PopUpConfirmData)(datas[1]);

                if (!m_confirm_data.isOneBtn)
                {
                    m_confirm_data.oneActionFromPop = () => { this.m_pop_root.Visible = true; this.ResetTicker(); };
                }

                m_confirm_root.InitConfirmPanel(m_confirm_data);
            }

            Dictionary<UBSParamKeyName, object> _param = new Dictionary<UBSParamKeyName, object>();
            _param.Add(UBSParamKeyName.Description, "sync error");
            UserBehaviorStatisticsModules.Instance.LogEvent(UBSEventKeyName.net_error, null, _param);
        }

        public override void OnHide()
        {
            base.OnHide();
            UnInitListener();
            this.Canvas.sortingOrder = this.m_ori_order_in_layer;
        }

        public override void Dispose()
        {
            base.Dispose();
        }

        private void InitPanel(PopUpTickerData data)
        {
            if (data == null)
            {
                return;
            }


            m_pop_root.Visible = true;
            m_close_btn.Visible = data.isClose;
            m_one_root.Visible = data.isOneBtn;
            m_two_root.Visible = !data.isOneBtn;
            if (data.IsMultipleLanguage)
            {
                if (string.IsNullOrEmpty(data.content_param0))
                    m_content_lab.Text = LocalizeModule.Instance.GetString(data.content);
                else
                    m_content_lab.Text = LocalizeModule.Instance.GetString(data.content, data.content_param0);
            }
            else
            {
                this.m_content_lab.Text = data.content;
            }

            if (string.IsNullOrEmpty(data.title))
                m_title_lab.Text = "";
            else
                m_title_lab.Text = LocalizeModule.Instance.GetString(data.title);

            if (data.isOneBtn)
            {
                if (data.IsMultipleLanguage && !string.IsNullOrEmpty(data.OneButtonText))
                    m_one_lab.Text = LocalizeModule.Instance.GetString(data.OneButtonText);
            }
            else
            {
                m_twoFirst_lab.Text = LocalizeModule.Instance.GetString(data.OneButtonText);
                m_twoSecond_lab.Text = LocalizeModule.Instance.GetString(data.twoStr);
            }

            if (data.order_in_layer > 0)
                this.Canvas.sortingOrder = data.order_in_layer;

            m_cur_seconds = m_data.ticker_seconds;
            m_ticker_lbl.Text = ((int)m_cur_seconds).ToString();
        }

        public void ResetTicker()
        {
            m_cur_seconds = m_data.ticker_seconds;
            m_ticker_lbl.Text = ((int)m_cur_seconds).ToString();
        }

        public void BtnOne(GameObject obj)
        {
            EngineCoreEvents.AudioEvents.PlayAudio.SafeInvoke(Audio.AudioType.UISound, EngineCommonAudioKey.Button_Click_Common.ToString());
            if (m_data == null)
            {
                return;
            }

            if (null != m_data.oneAction)
                m_data.oneAction.SafeInvoke();
            EngineCoreEvents.UIEvent.HideUIEvent.SafeInvoke(UIDefine.UI_POPUP);
        }

        private void BtnTwoFirst(GameObject obj)
        {
            EngineCoreEvents.AudioEvents.PlayAudio.SafeInvoke(Audio.AudioType.UISound, EngineCommonAudioKey.Close_Window.ToString());
            if (m_data == null)
            {
                return;
            }

            if (null != m_data.oneAction)
                m_data.oneAction.SafeInvoke();
            EngineCoreEvents.UIEvent.HideUIEvent.SafeInvoke(UIDefine.UI_POPUP);
        }

        private void BtnTwoSecond(GameObject obj)
        {
            EngineCoreEvents.AudioEvents.PlayAudio.SafeInvoke(Audio.AudioType.UISound, EngineCommonAudioKey.Button_Click_Common.ToString());
            if (m_data == null)
            {
                return;
            }

            if (null != m_data.twoAction)
                m_data.twoAction.SafeInvoke();
            EngineCoreEvents.UIEvent.HideUIEvent.SafeInvoke(UIDefine.UI_POPUP);
        }

        public override void LateUpdate()
        {
            if (m_cur_seconds < 0)
                return;

            m_cur_seconds -= Time.fixedDeltaTime;
            m_ticker_lbl.Text = ((int)m_cur_seconds).ToString();
            if (m_cur_seconds < 0)
            {
                m_ticker_lbl.Text = "0";
                this.m_pop_root.Visible = false;
                this.m_confirm_root.Visible = true;
            }
        }


        private class ConfirmView : GameUIComponent
        {
            private GameLabel m_title_lab;
            private GameLabel m_content_lab;
            private GameLabel m_ticker_lbl;

            private GameLabel m_one_root;
            private GameButton m_one_btn;
            private GameLabel m_one_lab;

            private GameLabel m_two_root;
            private GameButton m_twoFirst_btn;
            private GameButton m_twoSecond_btn;
            private GameLabel m_twoFirst_lab;
            private GameLabel m_twoSecond_lab;

            private PopUpConfirmData m_data = null;

            public void InitConfirmPanel(PopUpConfirmData data)
            {
                if (data == null)
                {
                    return;
                }

                m_data = data;


                m_one_root.Visible = data.isOneBtn;
                m_two_root.Visible = !data.isOneBtn;
                if (data.IsMultipleLanguage)
                {
                    if (string.IsNullOrEmpty(data.content_param0))
                        m_content_lab.Text = LocalizeModule.Instance.GetString(data.content);
                    else
                        m_content_lab.Text = LocalizeModule.Instance.GetString(data.content, data.content_param0);
                }
                else
                {
                    this.m_content_lab.Text = data.content;
                }

                if (string.IsNullOrEmpty(data.title))
                    m_title_lab.Text = "";
                else
                    m_title_lab.Text = LocalizeModule.Instance.GetString(data.title);

                if (data.isOneBtn)
                {
                    if (data.IsMultipleLanguage && !string.IsNullOrEmpty(data.OneButtonText))
                        m_one_lab.Text = LocalizeModule.Instance.GetString(data.OneButtonText);
                }
                else
                {
                    m_twoFirst_lab.Text = LocalizeModule.Instance.GetString(data.OneButtonText);
                    m_twoSecond_lab.Text = LocalizeModule.Instance.GetString(data.twoStr);
                }

            }

            private void InitListener()
            {
                m_one_btn.AddClickCallBack(BtnOne);
                m_twoFirst_btn.AddClickCallBack(BtnTwoFirst);
                m_twoSecond_btn.AddClickCallBack(BtnTwoSecond);
            }

            private void UnInitListener()
            {
                m_one_btn.RemoveClickCallBack(BtnOne);
                m_twoFirst_btn.RemoveClickCallBack(BtnTwoFirst);
                m_twoSecond_btn.RemoveClickCallBack(BtnTwoSecond);
            }

            protected override void OnInit()
            {
                base.OnInit();
                m_title_lab = Make<GameLabel>("title");
                m_content_lab = Make<GameLabel>("txt");
                m_one_root = Make<GameLabel>("one");
                m_one_btn = m_one_root.Make<GameButton>("btn0");
                m_one_lab = m_one_btn.Make<GameLabel>("Text");

                m_two_root = Make<GameLabel>("two");
                m_twoFirst_btn = m_two_root.Make<GameButton>("btn0");
                m_twoSecond_btn = m_two_root.Make<GameButton>("btn1");
                m_twoFirst_lab = m_twoFirst_btn.Make<GameLabel>("Text");
                m_twoSecond_lab = m_twoSecond_btn.Make<GameLabel>("Text");
            }

            public override void OnShow(object param)
            {
                base.OnShow(param);

                this.InitListener();

            }
            public override void OnHide()
            {
                base.OnHide();

                this.UnInitListener();
            }


            public void BtnOne(GameObject obj)
            {
                EngineCoreEvents.AudioEvents.PlayAudio.SafeInvoke(Audio.AudioType.UISound, EngineCommonAudioKey.Button_Click_Common.ToString());
                if (m_data == null)
                {
                    return;
                }

                if (null != m_data.oneAction)
                    m_data.oneAction.SafeInvoke();

                this.Visible = false;
            }

            private void BtnTwoFirst(GameObject obj)
            {
                EngineCoreEvents.AudioEvents.PlayAudio.SafeInvoke(Audio.AudioType.UISound, EngineCommonAudioKey.Close_Window.ToString());
                if (m_data == null)
                {
                    return;
                }

                if (null != m_data.oneAction)
                    m_data.oneAction.SafeInvoke();

                if (null != m_data.oneActionFromPop)
                    m_data.oneActionFromPop.SafeInvoke();

                this.Visible = false;
            }

            private void BtnTwoSecond(GameObject obj)
            {
                EngineCoreEvents.AudioEvents.PlayAudio.SafeInvoke(Audio.AudioType.UISound, EngineCommonAudioKey.Button_Click_Common.ToString());
                if (m_data == null)
                {
                    return;
                }

                if (null != m_data.twoAction)
                    m_data.twoAction.SafeInvoke();

                this.Visible = false;
            }
        }


    }



    public class PopUpConfirmData
    {
        public bool isOneBtn = false;
        public string title = string.Empty;
        public string content;
        public bool IsMultipleLanguage = true;
        public string content_param0;
        public string OneButtonText = "UI.OK";
        public string twoStr;
        public Action oneAction;
        public Action twoAction;
        public Action oneActionFromPop;
    }


    public class PopUpTickerData
    {
        public bool isOneBtn = true;
        public string title = string.Empty;
        public string content;
        public bool IsMultipleLanguage = true;
        public string content_param0;
        public string OneButtonText = "UI.OK";
        public string twoStr;
        public Action oneAction;
        public Action twoAction;
        public int order_in_layer = -1;
        public bool isClose = false;
        public int ticker_seconds = 60;
    }
}




