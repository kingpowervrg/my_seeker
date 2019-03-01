using EngineCore;
using Google.Protobuf;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace SeekerGame
{
    public class HttpPingModule : AbstractModule
    {
        private static HttpPingModule m_instance;

        public float PING_INTERVAL = 4f;

        protected override void setEnable(bool value)
        {
            if (Enable == value)
                return;

            base.setEnable(value);

            if (value)
            {
                MessageHandler.RegisterMessageHandler(MessageDefine.SCPingResponse, OnSCPingResponse);
                TimeModule.Instance.SetTimeInterval(SendPing, this.PING_INTERVAL, false);
            }
            else
            {
                MessageHandler.UnRegisterMessageHandler(MessageDefine.SCPingResponse, OnSCPingResponse);
                TimeModule.Instance.RemoveTimeaction(SendPing);
            }

        }

        private void OnSCPingResponse(object responseMsg)
        {
            //CommonTools.CheckNetError(true);

            SCPingResponse msg = responseMsg as SCPingResponse;

            for (int i = 0; i < msg.Content.Count; ++i)
            {
                PingResponseContent pingResponseContent = msg.Content[i];
                byte[] messageContentBytes = Convert.FromBase64String(pingResponseContent.Data);

                IMessage message = EngineCore.MessageParser.Parse(pingResponseContent.MsgId, messageContentBytes);

                Debug.Log($"recv message from ping : { message.Descriptor.Name},detail{message.ToString()}");
                MessageHandler.Call(pingResponseContent.MsgId, message);
            }
        }

        private void OnSCExceptionResponse(object responseMsg)
        {
            if (responseMsg is ExceptionRemoteLoginResponse)
            {
                ExceptionRemoteLoginResponse rsp = responseMsg as ExceptionRemoteLoginResponse;
                ConfMsgCode error_code = ConfMsgCode.Get(rsp.Status.Code);
                string error_str = "loading_FB_X";

                if (null != error_code)
                {
                    if (!string.IsNullOrEmpty(error_code.popStr) && !string.IsNullOrWhiteSpace(error_code.popStr))
                        error_str = error_code.popStr;
                }

                BackToLogin("Timeout", error_str);
            }
            else if (responseMsg is ExceptionResponse)
            {
                ExceptionResponse rsp = responseMsg as ExceptionResponse;
                ConfMsgCode error_code = ConfMsgCode.Get(rsp.Status.Code);
                string error_str = "server_unusual";

                if (null != error_code)
                {
                    if (!string.IsNullOrEmpty(error_code.popStr) && !string.IsNullOrWhiteSpace(error_code.popStr))
                        error_str = error_code.popStr;
                }

                BackToLogin("Timeout", error_str);

            }
        }

        public override void Start()
        {
            m_instance = this;
            AutoStart = true;

            Enable = false;

            MessageHandler.RegisterMessageHandler(MessageDefine.ExceptionRemoteLoginResponse, OnSCExceptionResponse);
            MessageHandler.RegisterMessageHandler(MessageDefine.ExceptionResponse, OnSCExceptionResponse);

        }


        private void BackToLogin(string title_, string content_)
        {
            System.Action act = () =>
            {
                BigWorldManager.Instance.ClearBigWorld();
                EngineCoreEvents.ResourceEvent.LeaveScene.SafeInvoke();

                FrameMgr.Instance.HideAllFrames(new List<string>() { UIDefine.UI_GUEST_LOGIN, UIDefine.UI_GM, UIDefine.UI_GUID });


                TimeModule.Instance.SetTimeout(() => GameRoot.instance.GameFSM.GotoState((int)ClientFSM.ClientState.LOGIN), 1.0f);
            };

            PopUpData pd = new PopUpData();
            pd.title = title_;
            pd.content = content_;
            pd.isOneBtn = true;
            pd.oneAction = act;
            PopUpManager.OpenPopUp(pd);
        }

        public void SendPing()
        {
            CSPingRequest pingRequest = new CSPingRequest();

            GameEvents.NetWorkEvents.SendAsyncMsg.SafeInvoke(pingRequest);
        }

        public void SendSyncPing()
        {
            CSPingRequest pingRequest = new CSPingRequest();

            GameEvents.NetWorkEvents.SendMsg.SafeInvoke(pingRequest);
        }

        public static HttpPingModule Instance
        {
            get { return m_instance; }
        }
    }
}