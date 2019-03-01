#if UNITY_DEBUG
using EngineCore;
using Google.Protobuf;
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;
using static SeekerGame.GMModule;

namespace SeekerGame
{
    [UILogicHandler(UIDefine.UI_GM)]
    public class GMUILogic : UILogicBase
    {
        public static GameButton m_btnGmPanel = null;
        private GMMainPanel m_gmPanel = null;

        protected override void OnInit()
        {
            base.OnInit();

            m_btnGmPanel = Make<GameButton>("Button");
            this.m_gmPanel = Make<GMMainPanel>("Panel");
        }

        public override void OnShow(object param)
        {
            base.OnShow(param);
            if (!PlayerPrefs.HasKey("serverip"))
                PlayerPrefs.SetString("serverip", "220.249.55.203:6001");

            m_btnGmPanel.AddClickCallBack(OnBtnShowMainPanelClick);
        }

        private void OnBtnShowMainPanelClick(GameObject btnShowPanel)
        {
            this.m_gmPanel.Visible = true;
            m_btnGmPanel.Visible = false;
        }

        public override void OnHide()
        {
            m_btnGmPanel.RemoveClickCallBack(OnBtnShowMainPanelClick);
        }

        public override FrameDisplayMode UIFrameDisplayMode => FrameDisplayMode.WINDOWED;


        public class GMMainPanel : GameUIComponent
        {
            private GameImage m_closeImage = null;
            private GameUIContainer m_commandListContainer = null;
            private GMCommandDetailPanel m_detailPanel = null;
            public static Action<GMCommandWrap> OnSelectCommand;
            private GMServerSelectPanel m_tempSelectServerComponent = null;

            protected override void OnInit()
            {
                this.m_closeImage = Make<GameImage>("Image");
                this.m_commandListContainer = Make<GameUIContainer>("Scroll View:Viewport:Content");
                this.m_detailPanel = Make<GMCommandDetailPanel>("Panel_Content");
                this.m_tempSelectServerComponent = Make<GMServerSelectPanel>("ServerList");
                OnSelectCommand = ShowCommandDetail;
            }

            public override void OnShow(object param)
            {
                base.OnShow(param);

                this.m_closeImage.AddClickCallBack((btn) =>
                {
                    this.Visible = false;

                    GMUILogic.m_btnGmPanel.Visible = true;
                });

                this.m_commandListContainer.EnsureSize<GMCommandItem>(GMModule.GMCommandWrapList.Count);
                for (int i = 0; i < GMModule.GMCommandWrapList.Count; ++i)
                {
                    GMCommandItem commandItem = this.m_commandListContainer.GetChild<GMCommandItem>(i);
                    commandItem.SetCommand(GMModule.GMCommandWrapList[i]);
                    commandItem.Visible = true;
                }

                if (this.m_commandListContainer.ChildCount > 0)
                    ShowCommandDetail(GMModule.GMCommandWrapList[0]);
            }

            public void ShowCommandDetail(GMCommandWrap selectedCommand)
            {
                this.m_detailPanel.SetCommand(selectedCommand);
            }


            private class GMCommandItem : GameUIComponent
            {
                private GameLabel m_commandName = null;
                private GMCommandWrap commandWrap = null;

                protected override void OnInit()
                {
                    base.OnInit();

                    this.m_commandName = Make<GameLabel>("Text");
                }

                public void SetCommand(GMCommandWrap commandWrap)
                {
                    this.commandWrap = commandWrap;

                    this.m_commandName.Text = commandWrap.conf.messageDesc;
                    this.AddClickCallBack(OnSelectGMCommamd);
                }

                private void OnSelectGMCommamd(GameObject btnCMD)
                {
                    GMMainPanel.OnSelectCommand(this.commandWrap);
                }

                public override void OnHide()
                {
                    RemoveClickCallBack(OnSelectGMCommamd);
                }
            }


            private class GMCommandDetailPanel : GameUIComponent
            {
                private GMCommandWrap m_detailCommandWrap = null;

                private GameLabel m_lbCommandDesc = null;
                private GameButton m_sendCommand = null;

                private GameUIContainer m_gmCommandParamContainer = null;

                protected override void OnInit()
                {
                    this.m_lbCommandDesc = Make<GameLabel>("GMCMDDesc");
                    this.m_gmCommandParamContainer = Make<GameUIContainer>("GMCMDParam:Viewport:Content");
                    this.m_sendCommand = Make<GameButton>("Button");
                }

                public override void OnShow(object param)
                {

                    this.m_sendCommand.AddClickCallBack(OnBtnSendGMCommandClick);
                }


                public void SetCommand(GMCommandWrap commandWrap)
                {
                    this.m_detailCommandWrap = commandWrap;

                    this.m_lbCommandDesc.Text = commandWrap.conf.messageDesc;
                    string[] paramsList = commandWrap.conf.messageFormat.Split(';');

                    this.m_gmCommandParamContainer.EnsureSize<GMCommandParamItem>(paramsList.Length);
                    for (int i = 0; i < paramsList.Length; ++i)
                    {
                        GMCommandParamItem item = this.m_gmCommandParamContainer.GetChild<GMCommandParamItem>(i);
                        item.SetParamName(paramsList[i]);
                        item.Visible = true;
                    }

                }

                private void OnBtnSendGMCommandClick(GameObject btnSendCommand)
                {
                    Dictionary<string, string> injectorParams = new Dictionary<string, string>();

                    IMessage gmMessage = Activator.CreateInstance(this.m_detailCommandWrap.MessageType) as IMessage;
                    PropertyInfo[] fieldInfos = this.m_detailCommandWrap.MessageType.GetProperties(BindingFlags.IgnoreCase | BindingFlags.Instance | BindingFlags.Public);
                    if (fieldInfos.Length != this.m_gmCommandParamContainer.ChildCount)
                        Debug.LogError($"command :{this.m_detailCommandWrap.CommandName} params error");

                    for (int i = 0; i < this.m_gmCommandParamContainer.ChildCount; ++i)
                    {
                        GMCommandParamItem paramItem = this.m_gmCommandParamContainer.GetChild<GMCommandParamItem>(i);
                        string paramValue = string.IsNullOrEmpty(paramItem.ParamValue) ? "0" : paramItem.ParamValue;

                        fieldInfos[i].SetValue(gmMessage, Convert.ChangeType(paramValue, fieldInfos[i].PropertyType));

                        injectorParams.Add(fieldInfos[i].Name.ToLower(), paramItem.ParamValue);
                    }

                    //执行各个模块的注入操作
                    if (this.m_detailCommandWrap.GMCommandInjector != null)
                    {
                        List<IMessage> messageList = this.m_detailCommandWrap.GMCommandInjector(injectorParams);
                        if (messageList == null)
                            return;

                        this.sendMessageList.Clear();
                        this.sendMessageList.AddRange(messageList);
                        BatchSend();
                    }
                    else
                    {
                        GameEvents.NetWorkEvents.SendAsyncMsg.SafeInvoke(gmMessage);
                        TimeModule.Instance.SetTimeout(() =>
                        {
                            GameEvents.NetWorkEvents.SendAsyncMsg.SafeInvoke(new CSPlayerInfoRequest());
                        }, 0.5f);
                    }
                }

                private List<IMessage> sendMessageList = new List<IMessage>();
                private float delay = 0.1f;
                private void BatchSend()
                {
                    if (sendMessageList.Count > 0)
                    {
                        TimeModule.Instance.SetTimeout(() =>
                        {
                            GameEvents.NetWorkEvents.SendAsyncMsg.SafeInvoke(sendMessageList[0]);
                            sendMessageList.RemoveAt(0);

                            BatchSend();
                        }, delay);
                    }

                }


                public override void OnHide()
                {
                    this.m_sendCommand.RemoveClickCallBack(OnBtnSendGMCommandClick);
                }



                private class GMCommandParamItem : GameUIComponent
                {
                    private GameLabel m_lbParamName = null;
                    private GameInputField m_inputParamValue = null;

                    protected override void OnInit()
                    {
                        this.m_lbParamName = Make<GameLabel>("label_paramName");
                        this.m_inputParamValue = Make<GameInputField>("InputField");
                    }

                    public void SetParamName(string paramName)
                    {
                        this.m_lbParamName.Text = paramName;
                        if (String.Equals(paramName, "playerid", StringComparison.CurrentCultureIgnoreCase))
                            this.m_inputParamValue.Text = GlobalInfo.MY_PLAYER_ID.ToString();
                        else
                            this.m_inputParamValue.Text = string.Empty;
                    }

                    public string ParamValue => this.m_inputParamValue.Text;


                }

            }
        }

        public class GMServerSelectPanel : GameUIComponent
        {
            private GameButton m_btnCommonServer = null;
            private GameButton m_btnWangpeng = null;
            private GameButton m_btnXiaolong = null;
            private GameButton m_btnXubo = null;
            private GameButton m_btnLocal = null;
            private GameButton m_btnInternet = null;
            private GameLabel m_lblastTimeServer = null;

            protected override void OnInit()
            {
                base.OnInit();

                this.m_btnCommonServer = Make<GameButton>("ServerListContainer:Button (1)");
                this.m_btnWangpeng = Make<GameButton>("ServerListContainer:Button (2)");
                this.m_btnXiaolong = Make<GameButton>("ServerListContainer:Button (3)");
                this.m_btnXubo = Make<GameButton>("ServerListContainer:Button (4)");
                this.m_btnLocal = Make<GameButton>("ServerListContainer:Button (5)");
                this.m_btnInternet = Make<GameButton>("ServerListContainer:Button (6)");

                this.m_lblastTimeServer = Make<GameLabel>("Text (1)");
            }

            public override void OnShow(object param)
            {
                this.m_btnCommonServer.AddClickCallBack(OnSelectServer);
                this.m_btnWangpeng.AddClickCallBack(OnSelectServer);
                this.m_btnXiaolong.AddClickCallBack(OnSelectServer);
                this.m_btnXubo.AddClickCallBack(OnSelectServer);
                this.m_btnLocal.AddClickCallBack(OnSelectServer);
                this.m_btnInternet.AddClickCallBack(OnSelectServer);

                this.m_lblastTimeServer.Text = $"当前服务器:{GlobalInfo.SERVER_ADDRESS}";
            }

            private void OnSelectServer(GameObject btn)
            {
                string selectServerIP = btn.GetComponentInChildren<Text>().text.Split('-')[1].Replace('\r', ' ').Trim();
                GlobalInfo.SERVER_ADDRESS = "http://" + selectServerIP;
                PlayerPrefs.SetString("serverip", selectServerIP);
                this.m_lblastTimeServer.Text = $"当前服务器:{GlobalInfo.SERVER_ADDRESS}";
            }

            public override void OnHide()
            {
                base.OnHide();
                this.m_btnInternet.RemoveClickCallBack(OnSelectServer);
                this.m_btnCommonServer.RemoveClickCallBack(OnSelectServer);
                this.m_btnWangpeng.RemoveClickCallBack(OnSelectServer);
                this.m_btnXiaolong.RemoveClickCallBack(OnSelectServer);
                this.m_btnXubo.RemoveClickCallBack(OnSelectServer);
                this.m_btnLocal.RemoveClickCallBack(OnSelectServer);
            }

        }
    }
}
#endif