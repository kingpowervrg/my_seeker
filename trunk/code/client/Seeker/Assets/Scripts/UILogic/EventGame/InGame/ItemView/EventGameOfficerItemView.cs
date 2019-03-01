using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;
using EngineCore;
using GOEngine;
using GOGUI;

namespace SeekerGame
{
    public class EventGameOfficerItemView : GameUIComponent
    {
        private GameToggleButton m_toggle;
        private GameTexture m_officer_portrait;
        private GameUIContainer m_grid;
        private GameImage m_mask;
        private GameUIEffect m_effect;


        private long m_officer_id;
        public long Officer_id
        {
            get { return m_officer_id; }
            set { m_officer_id = value; }
        }
        private SafeAction<long, bool> m_clicked_event;

        protected override void OnInit()
        {
            m_toggle = Make<GameToggleButton>("Background");
            m_officer_portrait = m_toggle.Make<GameTexture>("Icon");
            m_grid = m_toggle.Make<GameUIContainer>("grid");
            m_mask = m_toggle.Make<GameImage>("Image");
            m_effect = Make<GameUIEffect>("UI_jingyuanpaiqian_pinzhi");
        }

        public override void OnShow(object param)
        {
            m_toggle.AddChangeCallBack(OnClicked);
            m_effect.Visible = true;
        }

        public override void OnHide()
        {
            m_toggle.RemoveChangeCallBack(OnClicked);
            m_effect.Visible = false;
        }

        public void Refresh(long officer_id_, bool checked_, Action<long, bool> Clicked_)
        {
            Officer_id = officer_id_;
            m_clicked_event = Clicked_;

            m_officer_portrait.TextureName = ConfOfficer.Get(officer_id_).portrait;

            List<string> key_word_icons = EventGameUIAssist.GetPoliceKeyWordIconsByOfficerID(officer_id_);

            m_grid.EnsureSize<GameImage>(key_word_icons.Count);

            for (int i = 0; i < m_grid.ChildCount; ++i)
            {
                var item = m_grid.GetChild<GameImage>(i);
                item.Sprite = key_word_icons[i];
                item.Visible = true;
            }

            m_toggle.Checked = checked_;
            m_mask.Visible = false;

            m_effect.EffectPrefabName = $"UI_jingyuanpaiqian_pinzhi0{ConfOfficer.Get(officer_id_).quality}.prefab";
            m_effect.Visible = true;
        }

        public bool EnableClick
        {
            set { m_toggle.Enabled = value; }
            get { return m_toggle.Enabled; }
        }

        private void OnClicked(bool val)
        {


            m_mask.Visible = val;
            m_clicked_event.SafeInvoke(Officer_id, val);
        }
    }
}
