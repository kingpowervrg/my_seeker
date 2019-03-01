#if OFFICER_SYS
using EngineCore;
using GOGUI;
using Google.Protobuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace SeekerGame
{
    public class EventGameOfficerDispatchItemView : GameUIComponent
    {
        private GameImage m_add_img;
        private GameImage m_exchange_img;
        private GameTexture m_head_tex;
        private GameImage m_remove_img;
        private GameLabel m_name_txt;
        private GameUIEffect m_effect;

        private int m_idx;
        public int Idx
        {
            get { return m_idx; }
        }
        private long m_officer_id = 0;
        public long Officer_id
        {
            get { return m_officer_id; }
        }
        protected override void OnInit()
        {
            base.OnHide();

            m_add_img = Make<GameImage>("RawImage_1");
            m_exchange_img = Make<GameImage>("Image_bg");
            m_head_tex = Make<GameTexture>("Image_bg:Panel:RawImage");
            m_remove_img = Make<GameImage>("Image_bg:Image_closebutton");
            m_name_txt = Make<GameLabel>("Image_bg:Text");
            m_effect = Make<GameUIEffect>("UI_jingyuanpaiqian_Send02");
            m_effect.EffectPrefabName = "UI_jingyuanpaiqian_Send02.prefab";
        }

        public override void OnShow(object param)
        {
            base.OnShow(param);
            m_add_img.AddClickCallBack(OnAddClicked);
            m_exchange_img.AddClickCallBack(OnAddClicked);
            m_remove_img.AddClickCallBack(OnRemoveClicked);
        }

        public override void OnHide()
        {
            base.OnHide();
            m_add_img.RemoveClickCallBack(OnAddClicked);
            m_exchange_img.RemoveClickCallBack(OnAddClicked);
            m_remove_img.RemoveClickCallBack(OnRemoveClicked);
            m_effect.Visible = false;
        }

        public void Init(int idx_)
        {
            m_idx = idx_;
            m_officer_id = 0L;

            Occupied(false);
        }

        public void Refresh(long officer_id_)
        {
            m_officer_id = officer_id_;

            ConfOfficer info = ConfOfficer.Get(officer_id_);
            m_head_tex.TextureName = info.portrait;
            m_name_txt.Text = LocalizeModule.Instance.GetString(info.name);
            m_name_txt.color = PoliceUILogicAssist.GetPoliceQualityColor(info.quality);
            Occupied(true);
        }

        private void Occupied(bool v_)
        {
            m_add_img.Visible = !v_;
            m_exchange_img.Visible = v_;
            m_effect.Visible = v_;

        }

        private void OnAddClicked(GameObject obj)
        {
            GameEvents.UIEvents.UI_EventGame_Event.Listen_NeedAnOfficer.SafeInvoke(this.m_idx);
        }

        private void OnRemoveClicked(GameObject obj)
        {
            Occupied(false);
            GameEvents.UIEvents.UI_EventGame_Event.Listen_RemoveAnOfficer.SafeInvoke(m_officer_id);
        }


    }
}
#endif