using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EngineCore;
using System;
using GOGUI;

namespace SeekerGame
{
    [UILogicHandler(UIDefine.UI_POPUP)]
    public class PopUpUILogic : UILogicBase
    {
        private GameLabel m_title_lab;
        private GameLabel m_content_lab;

        private GameLabel m_one_root;
        private GameButton m_one_btn;
        private GameLabel m_one_lab;

        private GameLabel m_two_root;
        private GameButton m_twoFirst_btn;
        private GameButton m_twoSecond_btn;
        private GameLabel m_twoFirst_lab;
        private GameLabel m_twoSecond_lab;

        private PopUpData m_data = null;

        private GameButton m_close_btn;
        //private UITweener[] tweener = null;

        private int m_ori_order_in_layer;
        private void InitController()
        {
            m_title_lab = Make<GameLabel>("Panel_use:title");
            m_content_lab = Make<GameLabel>("Panel_use:content:content");

            m_one_root = Make<GameLabel>("Panel_use:one");
            m_one_btn = this.Make<GameButton>("Panel_use:one:btn0");
            m_one_lab = this.Make<GameLabel>("Panel_use:one:btn0:Text");

            m_two_root = Make<GameLabel>("Panel_use:two");
            m_twoFirst_btn = this.Make<GameButton>("Panel_use:two:btn0");
            m_twoSecond_btn = this.Make<GameButton>("Panel_use:two:btn1");
            m_twoFirst_lab = this.Make<GameLabel>("Panel_use:two:btn0:Text");
            m_twoSecond_lab = this.Make<GameLabel>("Panel_use:two:btn1:Text");

            m_ori_order_in_layer = this.Canvas.sortingOrder;

            m_close_btn = this.Make<GameButton>("Button_close");
            //this.tweener = Transform.GetComponentsInChildren<UITweener>(true);
        }

        protected override void OnInit()
        {
            base.OnInit();
            InitController();
        }

        private void InitListener()
        {
            m_one_btn.AddClickCallBack(BtnOne);
            m_twoFirst_btn.AddClickCallBack(BtnTwoFirst);
            m_twoSecond_btn.AddClickCallBack(BtnTwoSecond);
            m_close_btn.AddSelectCallBack(onClickClose);
        }

        private void UnInitListener()
        {
            m_one_btn.RemoveClickCallBack(BtnOne);
            m_twoFirst_btn.RemoveClickCallBack(BtnTwoFirst);
            m_twoSecond_btn.RemoveClickCallBack(BtnTwoSecond);
            m_close_btn.RemoveClickCallBack(onClickClose);
        }

        public override void OnShow(object param)
        {
            base.OnShow(param);
            InitListener();
            if (param != null)
            {
                m_data = (PopUpData)param;
                InitPanel(m_data);
            }
            //for (int i = 0; i < this.tweener.Length; i++)
            //{
            //    this.tweener[i].ResetAndPlayForward();
            //}
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

        private void InitPanel(PopUpData data)
        {
            if (data == null)
            {
                return;
            }

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
                if (!string.IsNullOrEmpty(data.OneButtonText))
                    m_twoFirst_lab.Text = LocalizeModule.Instance.GetString(data.OneButtonText);
                if (!string.IsNullOrEmpty(data.twoStr))
                    m_twoSecond_lab.Text = LocalizeModule.Instance.GetString(data.twoStr);
            }

            if (data.order_in_layer > 0)
                this.Canvas.sortingOrder = data.order_in_layer;
        }

        private void BtnOne(GameObject obj)
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
            EngineCoreEvents.AudioEvents.PlayAudio.SafeInvoke(Audio.AudioType.UISound,EngineCommonAudioKey.Close_Window.ToString());
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
        public override FrameDisplayMode UIFrameDisplayMode
        {
            get
            {
                return FrameDisplayMode.WINDOWED;
            }
        }

    }


    public class PopUpData
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
    }
}


