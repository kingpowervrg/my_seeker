#if OFFICER_SYS
using EngineCore;
using Google.Protobuf;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SeekerGame
{
    public class SelectPoliceItem : GameUIComponent
    {
        private GameTexture m_tex;
        private GameImage m_lvl_icon;
        private GameLabel m_name_txt;

        private GameUIComponent m_skill_root;
        private GameImage m_skill_icon;
        private GameLabel m_skill_lvl_txt;
        private GameLabel m_skill_desc_txt;

        private GameUIComponent m_keyword_root;
        private GameUIContainer m_keyword_grid;


        private OfficerInfo m_info;
        private SafeAction<long> m_on_selected;

        protected override void OnInit()
        {
            base.OnInit();

            m_skill_root = this.Make<GameUIComponent>("Image:Skill");
            m_skill_icon = m_skill_root.Make<GameImage>("Image");
            m_skill_lvl_txt = m_skill_root.Make<GameLabel>("Image (1):Text");
            m_skill_desc_txt = m_skill_root.Make<GameLabel>("Image:Text");
            m_tex = this.Make<GameTexture>("RawImage");
            m_lvl_icon = m_tex.Make<GameImage>("icon");
            m_name_txt = m_tex.Make<GameLabel>("title");

            m_keyword_root = this.Make<GameUIComponent>("Image:ScrollView");
            m_keyword_grid = m_keyword_root.Make<GameUIContainer>("Viewport");
        }

        public override void OnShow(object param)
        {
            base.OnShow(param);

            this.AddClickCallBack(OnClicked);
        }

        public override void OnHide()
        {
            base.OnHide();
            this.RemoveClickCallBack(OnClicked);
        }

        public void Refresh(OfficerInfo info, Action<long> OnSelected_, ENUM_SEARCH_MODE mode_)
        {
            m_info = info;
            m_on_selected = OnSelected_;

            ConfOfficer officer_data = ConfOfficer.Get(info.OfficerId);
            m_tex.TextureName = officer_data.portrait;
            m_name_txt.Text = LocalizeModule.Instance.GetString(officer_data.name);
            m_name_txt.color = PoliceUILogicAssist.GetPoliceQualityColor(officer_data.quality);


            if (ENUM_SEARCH_MODE.E_SEARCH_ROOM == mode_)
            {
                m_skill_root.Visible = true;
                m_keyword_root.Visible = false;

                m_lvl_icon.Sprite = PoliceUILogicAssist.GetPoliceRankIcon(info.Level);
                string icon, desc;
                if (SkillUtil.GetCurLevelSkillIconAndDesc(info.OfficerId, info.Level, out icon, out desc))
                {
                    m_skill_lvl_txt.Text = info.Level.ToString();
                    m_skill_icon.Sprite = icon;
                    m_skill_desc_txt.Text = desc;
                }
            }
            else if (ENUM_SEARCH_MODE.E_EVENTGAME == mode_)
            {
                m_skill_root.Visible = false;
                m_keyword_root.Visible = true;
                m_lvl_icon.Sprite = PoliceUILogicAssist.GetPoliceRankIcon(info.Level);
                List<long> keywords_id = EventGameUIAssist.GetPoliceKeyWordByOfficerID(info.OfficerId);
                m_keyword_grid.EnsureSize<EventGameKeywordItemView>(keywords_id.Count);

                for (int i = 0; i < m_keyword_grid.ChildCount; ++i)
                {
                    m_keyword_grid.GetChild<EventGameKeywordItemView>(i).Refresh(i, keywords_id[i]);
                    m_keyword_grid.GetChild<EventGameKeywordItemView>(i).Visible = true;
                }
            }

        }

        private void OnClicked(GameObject obj_)
        {
            m_on_selected.SafeInvoke(m_info.OfficerId);
        }
    }
}
#endif