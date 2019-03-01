#if OFFICER_SYS
using EngineCore;
using Google.Protobuf;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SeekerGame
{
    [UILogicHandler(UIDefine.UI_SELECT_POLICE_GRID)]
    public class SelectPoliceGridUILogic : BaseViewComponetLogic
    {


        private GameUIContainer m_police_item_pool;
        private GameImage m_closeBtn;

        private List<OfficerInfo> m_officers = new List<OfficerInfo>();
        List<long> m_selected_officer_IDs;

        private long m_officer_timestamp = 0L;

        private List<long> m_dispatched_polices = new List<long>();

        protected override void OnInit()
        {
            base.OnInit();

            IsFullScreen = true;

            this.m_police_item_pool = this.Make<GameUIContainer>("Panel_animation:Viewport");
            if (GlobalInfo.GAME_NETMODE == GameNetworkMode.Network)
            {
                this.SetCloseBtnID("Panel_animation:Button_close");
            }
            this.m_closeBtn = this.Make<GameImage>("Panel_animation:Button_close");
            this.m_closeBtn.Visible = true;

            RefreshOfficerView();



        }

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

            RefreshOfficerView();

            m_selected_officer_IDs = GameEvents.UIEvents.UI_Enter_Event.Tell_GetAllDispatchedOfficerID.SafeInvoke();

        }

        public override void OnGuidShow(int type = 0)
        {
            base.OnGuidShow(type);
            this.m_closeBtn.Visible = false;
        }

        public override void OnHide()
        {
            base.OnHide();
        }




        private void RefreshOfficerView()
        {
            List<OfficerInfo> temp = GlobalInfo.MY_PLAYER_INFO.Officer_infos;

            if (0 != PlayerInfoManager.Instance.LimitNum)
            {
                temp = temp.GetRange(0, PlayerInfoManager.Instance.LimitNum);
            }

            List<long> new_dispatched_ids = GameEvents.UIEvents.UI_Enter_Event.Tell_GetAllDispatchedOfficerID.SafeInvoke();

            bool same = true;

            same = m_dispatched_polices.Count == new_dispatched_ids.Count;

            if (same)
            {
                foreach (var id in new_dispatched_ids)
                {
                    if (!m_dispatched_polices.Contains(id))
                    {
                        same = false;
                        break;
                    }
                }
            }




            if (m_officers.Count != temp.Count || !same)
            {
                m_officers = temp;
                m_officer_timestamp = GlobalInfo.MY_PLAYER_INFO.OfficerTimestamp;

                List<OfficerInfo> temp_officers = new List<OfficerInfo>(m_officers);

                m_dispatched_polices = GameEvents.UIEvents.UI_Enter_Event.Tell_GetAllDispatchedOfficerID.SafeInvoke();

                if (m_dispatched_polices.Count > 0)
                {
                    temp_officers.RemoveAll((item) => m_dispatched_polices.Contains(item.OfficerId));
                }

                int police_num = temp_officers.Count;

                this.m_police_item_pool.EnsureSize<SelectPoliceItem>(police_num);

                for (int i = 0; i < police_num; ++i)
                {
                    OfficerInfo info = temp_officers[i];
                    var item = this.m_police_item_pool.GetChild<SelectPoliceItem>(i);
                    item.Refresh(info, OnClickPolice, GameEvents.UIEvents.UI_Enter_Event.Tell_GetGameType.SafeInvoke());

                    item.gameObject.name = i.ToString();
                    item.Visible = true;
                }

            }
        }


        private void OnClickPolice(long officer_id_)
        {
            EngineCoreEvents.AudioEvents.PlayAudio.SafeInvoke(Audio.AudioType.UISound,GameCustomAudioKey.task_policechoice.ToString());
            GameEvents.UIEvents.UI_Enter_Event.EVT_SELECT_POLICE.SafeInvoke(officer_id_);
            GameEvents.UI_Guid_Event.OnSelectPolice.SafeInvoke(officer_id_);
            EngineCoreEvents.UIEvent.HideUIEvent.SafeInvoke(UIDefine.UI_SELECT_POLICE_GRID);
        }

    }
}
#endif