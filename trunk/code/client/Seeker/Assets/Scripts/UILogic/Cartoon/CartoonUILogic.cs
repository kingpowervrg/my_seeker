#define TEST
using EngineCore;
using Google.Protobuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GOEngine;
using GOGUI;

namespace SeekerGame
{
    [UILogicHandler(UIDefine.UI_CARTOON)]
    public class CartoonUILogic : BaseViewComponetLogic
    {
        CartoonUI m_view;

        long m_cartoon_id;
        Queue<long> m_levels;

        public override void OnPackageRequest(IMessage imsg, params object[] msg_params)
        {
            base.OnPackageRequest(imsg, msg_params);

            if (imsg is CSCartoonRewardRequest)
            {
                CSCartoonRewardRequest req = imsg as CSCartoonRewardRequest;
            }

        }

        public override void OnScResponse(object s)
        {
            base.OnScResponse(s);

            if (s is SCCartoonRewardReqsponse)
            {
                var rsp = s as SCCartoonRewardReqsponse;

                this.CloseFrame();

                WinFailData data = new WinFailData(ENUM_SEARCH_MODE.E_CARTOON, rsp);
                FrameMgr.OpenUIParams param = new FrameMgr.OpenUIParams(UIDefine.UI_WIN);
                param.Param = data;
                EngineCoreEvents.UIEvent.ShowUIEventWithParam.SafeInvoke(param);


                //卡通不掉落物品

            }
        }

        public override void OnShow(object param)
        {
            base.OnShow(param);

            MessageHandler.RegisterMessageHandler(MessageDefine.SCCartoonRewardReqsponse, OnScResponse);


            m_cartoon_id = (long)param;

            ConfCartoonScene data;

            ConfCartoonScene.GetConfig(m_cartoon_id, out data);

#if !TEST
            string[] lvls = data.sceneInfoIds;

            m_levels = new Queue<long>();
            foreach( string lvl in lvls)
            {
                if( !string.IsNullOrEmpty(lvl))
                {
                    m_levels.Enqueue(long.Parse(lvl));
                }
            }
#else
            m_levels = new Queue<long>();
            m_levels.Enqueue(1001);
            m_levels.Enqueue(1002);
#endif


            this.m_view.Refresh(m_levels.Dequeue());

        }

        public override void OnHide()
        {
            base.OnHide();

            MessageHandler.UnRegisterMessageHandler(MessageDefine.SCCartoonRewardReqsponse, OnScResponse);
        }

        protected override void OnInit()
        {
            base.OnInit();
            m_view = this.Make<CartoonUI>(root.name);

        }

        public void Win()
        {
            if (m_levels.Count > 0)
            {
                this.m_view.Refresh(m_levels.Dequeue());
            }
            else
            {
                CSCartoonRewardRequest req = new CSCartoonRewardRequest();
                OnScRequest(req);
            }
        }

        public void Winnnn()
        {
            CSCartoonRewardRequest req = new CSCartoonRewardRequest();
            OnScRequest(req);
        }

        public override FrameDisplayMode UIFrameDisplayMode
        {
            get
            {
                return FrameDisplayMode.WINDOWED;
            }
        }
    }
}
