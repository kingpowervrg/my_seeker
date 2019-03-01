using EngineCore;
using Google.Protobuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SeekerGame
{
    [UILogicHandler(UIDefine.UI_BIND)]
    public class BindUILogic : BaseViewComponetLogic
    {
        PromoptView m_promopt_view;
        RewardView m_reward_view;

        public override void OnPackageRequest(IMessage imsg, params object[] msg_params)
        {
            base.OnPackageRequest(imsg, msg_params);
        }

        public override void OnScResponse(object s)
        {
            base.OnScResponse(s);
        }

        public override void OnShow(object param)
        {
            base.OnShow(param);

            if (null != param)
            {
                if (param is BindPromoptData)
                {
                    var data = param as BindPromoptData;
                    m_promopt_view.Refresh(data);
                    m_promopt_view.Visible = true;
                    m_reward_view.Visible = false;
                }
                else if (param is BindRewardData)
                {
                    var data = param as BindRewardData;
                    m_reward_view.Refresh(data);
                    m_reward_view.Visible = true;
                    m_promopt_view.Visible = false;
                }
            }
        }
        public override FrameDisplayMode UIFrameDisplayMode
        {
            get
            {
                return FrameDisplayMode.WINDOWED;
            }
        }
        public override void OnHide()
        {
            base.OnHide();
        }

        protected override void OnInit()
        {
            base.OnInit();

            m_promopt_view = Make<PromoptView>("Panel_1");
            m_reward_view = Make<RewardView>("Panel_2");
        }

        public void OnQuit()
        {
            this.CloseFrame();
        }
    }
}
