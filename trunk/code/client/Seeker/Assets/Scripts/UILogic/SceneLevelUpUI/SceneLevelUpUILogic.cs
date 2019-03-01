using EngineCore;
using Google.Protobuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SeekerGame
{
    [UILogicHandler(UIDefine.UI_SCENE_LEVEL_UP)]
    public class SceneLevelUpUILogic : BaseViewComponetLogic
    {
        protected SceneLevelUpView m_view;
        protected SceneLevelUpData m_data;
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
                m_data = param as SceneLevelUpData;

                SCSceneRewardResponse msg = (SCSceneRewardResponse)m_data.msg;
                m_view.Visible = true;
                m_view.Refresh(msg);

                this.AddProp(msg);
            }


        }

        public override FrameDisplayMode UIFrameDisplayMode
        {
            get
            {
                return FrameDisplayMode.WINDOWED;
            }
        }
        private void AddProp(SCSceneRewardResponse msg)
        {
            foreach (var reward in msg.UpLevelRewards)
            {
                ConfProp prop = ConfProp.Get(reward.ItemId);

                if ((int)PROP_TYPE.E_FUNC == prop.type || (int)PROP_TYPE.E_CHIP == prop.type || (int)PROP_TYPE.E_NROMAL == prop.type || (int)PROP_TYPE.E_ENERGE == prop.type)
                {
                    GlobalInfo.MY_PLAYER_INFO.AddSingleBagInfo(reward.ItemId, reward.Num);
                }
            }

            GameEvents.UIEvents.UI_GameEntry_Event.Listen_OnCombinePropCollected.SafeInvoke();
        }

        public override void OnHide()
        {
            base.OnHide();

            GameEvents.UIEvents.UI_Bonus_Pop_View_Event.Tell_OnBlock.SafeInvoke(false);
            //GameEvents.UIEvents.UI_Bonus_Pop_View_Event.Tell_OnShow.SafeInvoke();
        }

        protected override void OnInit()
        {
            base.OnInit();
            m_view = this.Make<SceneLevelUpView>(root.name);
        }


        public void OnClose()
        {
            this.CloseFrame();

            if (!m_data.m_click_act.IsNull)
                m_data.m_click_act.SafeInvoke();

            SCSceneRewardResponse _msg = m_data.msg;
            WinFailData data = new WinFailData(ENUM_SEARCH_MODE.E_SEARCH_ROOM, _msg);

            FrameMgr.OpenUIParams param = new FrameMgr.OpenUIParams(UIDefine.UI_WIN);

            param.Param = data;

            EngineCoreEvents.UIEvent.ShowUIEventWithParam.SafeInvoke(param);

            Dictionary<UBSParamKeyName, object> _param = new Dictionary<UBSParamKeyName, object>();
            _param.Add(UBSParamKeyName.Success, _msg.SceneId > 0 ? 1 : 0);
            _param.Add(UBSParamKeyName.SceneID, SceneModule.Instance.SceneData.id);
            UserBehaviorStatisticsModules.Instance.LogEvent(UBSEventKeyName.game_finish, _msg.SceneId > 0 ? 1 : 0, _param);

            if (_msg.SceneId < 0)
            {
                //PushGiftManager.Instance.TurnOn(ENUM_PUSH_GIFT_BLOCK_TYPE.E_SEEK);
            }
        }


    }
}
