using EngineCore;
using Google.Protobuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SeekerGame
{
    [UILogicHandler(UIDefine.UI_LEVEL_UP)]
    public class LevelUpUILogic : BaseViewComponetLogic
    {
        protected LevelUpView m_view;
        protected LevelUpData m_data;
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

            GameEvents.UIEvents.UI_Bonus_Pop_View_Event.Tell_OnBlock.SafeInvoke(true);

            if (null != param)
            {
                m_data = param as LevelUpData;

                SCPlayerUpLevel msg = (SCPlayerUpLevel)m_data.msg;
                m_view.Visible = true;
                m_view.Refresh(msg);

                this.AddProp(msg);
            }


        }


        private void AddProp(SCPlayerUpLevel msg)
        {
            foreach (var reward in msg.Rewards)
            {
                ConfProp prop = ConfProp.Get(reward.PropId);

                if ((int)PROP_TYPE.E_FUNC == prop.type || (int)PROP_TYPE.E_CHIP == prop.type || (int)PROP_TYPE.E_NROMAL == prop.type || (int)PROP_TYPE.E_ENERGE == prop.type)
                {
                    GlobalInfo.MY_PLAYER_INFO.AddSingleBagInfo(reward.PropId, reward.Num);
                }
            }

            GameEvents.UIEvents.UI_GameEntry_Event.Listen_OnCombinePropCollected.SafeInvoke();
        }

        public override void OnHide()
        {
            base.OnHide();

            GameEvents.UIEvents.UI_Bonus_Pop_View_Event.Tell_OnBlock.SafeInvoke(false);
            GameEvents.UIEvents.UI_Bonus_Pop_View_Event.Tell_OnShow.SafeInvoke();

            //GlobalInfo.MY_PLAYER_INFO.SyncPlayerBag();
        }

        protected override void OnInit()
        {
            base.OnInit();
            m_view = this.Make<LevelUpView>(root.name);
        }

        public override FrameDisplayMode UIFrameDisplayMode
        {
            get
            {
                return FrameDisplayMode.WINDOWED;
            }
        }
        public void OnClose()
        {
            this.CloseFrame();

            //PlayerInfoManager.OnPlayerInfoUpdatedEvent(GlobalInfo.MY_PLAYER_INFO);
            if (!m_data.m_click_act.IsNull)
                m_data.m_click_act.SafeInvoke();
        }

        public void OnGoToMap(long bulid_id_)
        {
            this.CloseFrame();

            //PlayerInfoManager.OnPlayerInfoUpdatedEvent(GlobalInfo.MY_PLAYER_INFO);
            BigWorldManager.Instance.LoadBigWorld("", true, bulid_id_);

            //if (!m_data.m_click_act.IsNull)
            //{
            //    m_data.m_click_act.SafeInvoke();
            //}
            //else
            //{
            //    BigWorldManager.Instance.LoadBigWorld("", true, 1);
            //}
        }
    }
}
