#if OFFICER_SYS
using EngineCore;
using GOEngine;
using GOGUI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SeekerGame
{

    public class PoliceDetailUI : GameUIComponent
    {
        private SafeAction<bool> m_onBack;
        private GameButton m_back_btn;

        private GameImage m_portrait_icon;
        private GameLabel m_name_title;
        private GameLabel m_quality_title;
        private GameLabel m_quality_label;
        private GameLabel m_lvl_title;
        private GameImage m_lvl_icon;

        private GameImage m_skill_icon;
        private GameLabel m_skillLevelLab;
        private GameLabel m_skill_desc;

        private GameUIRadarImage m_ability_icon;

        private GameLabel m_attention_title;
        private GameLabel m_attention_num;

        private GameLabel m_willpower_title;
        private GameLabel m_willpower_num;

        private GameLabel m_observe_title;
        private GameLabel m_observe_num;

        private GameLabel m_memory_title;
        private GameLabel m_memory_num;


        private GameLabel m_special_skill_title;
        private GameLabel m_special_skill_label;
        private GameImage m_specail_skill_icon;

        private List<GameUIComponent> m_key_roots;
        private List<GameImage> m_key_icons;
        private List<GameLabel> m_key_labels;

        public void RegisterBack(System.Action<bool> back_)
        {
            this.m_onBack = back_;
        }

        protected override void OnInit()
        {
            this.m_back_btn = this.Make<GameButton>("btnBack");
            this.m_portrait_icon = this.Make<GameImage>("Image_Head:icon");


            m_name_title = this.Make<GameLabel>("Image_Head:title");
            m_quality_title = this.Make<GameLabel>("Image_Head:rank");
            m_quality_title.Text = LocalizeModule.Instance.GetString("UI_Police.quality");
            m_quality_label = this.Make<GameLabel>("Image_Head:rank_lvl");
            m_lvl_title = this.Make<GameLabel>("Image_Head:capacity");
            m_lvl_title.Text = LocalizeModule.Instance.GetString("UI_Police.lvl");
            m_lvl_icon = this.Make<GameImage>("Image_Head:Image");

            m_skill_icon = this.Make<GameImage>("miaosu:skill_icon");
            this.m_skillLevelLab = m_skill_icon.Make<GameLabel>("Image:Text");
            m_skill_desc = this.Make<GameLabel>("miaosu:Tips");

            m_ability_icon = this.Make<GameUIRadarImage>("Ability:Image_Root:Image");

            m_attention_title = this.Make<GameLabel>("Ability:Text_Attention");
            m_attention_title.Text = LocalizeModule.Instance.GetString("UI_Police.attention");
            m_attention_num = this.Make<GameLabel>("Ability:Text_Attention:Text");

            m_willpower_title = this.Make<GameLabel>("Ability:Text_WillPower");
            m_willpower_title.Text = LocalizeModule.Instance.GetString("UI_Police.willpower");
            m_willpower_num = this.Make<GameLabel>("Ability:Text_WillPower:Text");

            m_observe_title = this.Make<GameLabel>("Ability:Text_Observation");
            m_observe_title.Text = LocalizeModule.Instance.GetString("UI_Police.observe");
            m_observe_num = this.Make<GameLabel>("Ability:Text_Observation:Text");

            m_memory_title = this.Make<GameLabel>("Ability:Text_Memory");
            m_memory_title.Text = LocalizeModule.Instance.GetString("UI_Police.memory");
            m_memory_num = this.Make<GameLabel>("Ability:Text_Memory:Text");

            m_key_roots = new List<GameUIComponent>()
            {
                this.Make<GameUIComponent>("Image_key0"),
                this.Make<GameUIComponent>("Image_key1"),
                this.Make<GameUIComponent>("Image_key2"),
                this.Make<GameUIComponent>("Image_key3"),
                this.Make<GameUIComponent>("Image_key4"),
                this.Make<GameUIComponent>("Image_key5"),
            };

            m_key_icons = new List<GameImage>()
            {
                this.Make<GameImage>("Image_key0:Image"),
                this.Make<GameImage>("Image_key1:Image"),
                this.Make<GameImage>("Image_key2:Image"),
                this.Make<GameImage>("Image_key3:Image"),
                this.Make<GameImage>("Image_key4:Image"),
                this.Make<GameImage>("Image_key5:Image"),
            };

            m_key_labels = new List<GameLabel>()
            {
                this.Make<GameLabel>("Image_key0:Text"),
                this.Make<GameLabel>("Image_key1:Text"),
                this.Make<GameLabel>("Image_key2:Text"),
                this.Make<GameLabel>("Image_key3:Text"),
                this.Make<GameLabel>("Image_key4:Text"),
                this.Make<GameLabel>("Image_key5:Text"),
            };
        }

        public override void OnShow(object param)
        {
            this.m_back_btn.AddClickCallBack(OnBackClicked);
        }

        public override void OnHide()
        {
            this.m_back_btn.RemoveClickCallBack(OnBackClicked);
        }

        public void Refresh(ConfOfficer ori_info_, OfficerInfo server_info_)
        {
            this.m_portrait_icon.Sprite = ori_info_.icon;

            long officer_id = null == server_info_ ? ori_info_.id : server_info_.OfficerId; ;
            int lvl = null == server_info_ ? 0 : server_info_.Level;
            int outsight = null == server_info_ ? ori_info_.outsight : server_info_.Outsight;
            int memory = null == server_info_ ? ori_info_.memory : server_info_.Memory;
            int attention = null == server_info_ ? ori_info_.attention : server_info_.Attention;
            int willpower = null == server_info_ ? ori_info_.willpower : server_info_.WillPower;


            m_name_title.Text = LocalizeModule.Instance.GetString(ori_info_.name);
            m_quality_label.Text = PoliceUILogicAssist.GetQualityString(ori_info_.quality);
            m_lvl_icon.Sprite = PoliceUILogicAssist.GetPoliceRankIcon(lvl);

            string icon_name, desc;

            if (SkillUtil.GetCurLevelSkillIconAndDesc(officer_id, lvl, out icon_name, out desc))
            {
                if (lvl == 0)
                {
                    this.m_skillLevelLab.Text = "1";
                }
                else
                {
                    this.m_skillLevelLab.Text = lvl.ToString();
                }
                m_skill_icon.Sprite = icon_name;
                m_skill_desc.Text = desc;
            }

            m_ability_icon.SetPropList(new List<float> { outsight, memory, attention, willpower });

            m_observe_num.Text = outsight.ToString();
            m_memory_num.Text = memory.ToString();
            m_attention_num.Text = attention.ToString();
            m_willpower_num.Text = willpower.ToString();

            List<string> key_words = PoliceUILogicAssist.GetKeyWords(ori_info_);
            List<string> key_words_icon = PoliceUILogicAssist.GetKeyWordsIcon(ori_info_);
            foreach (var key in m_key_roots)
            {
                key.Visible = false;
            }

            for (int i = 0; i < key_words.Count && i < m_key_labels.Count; ++i)
            {
                m_key_roots[i].Visible = true;
                m_key_labels[i].Text = key_words[i];
                m_key_icons[i].Sprite = key_words_icon[i];
            }

        }



        private void OnBackClicked(GameObject obj_)
        {
            EngineCoreEvents.AudioEvents.PlayAudio.SafeInvoke(Audio.AudioType.UISound, EngineCommonAudioKey.Button_Click_Common.ToString());

            this.Visible = false;

            if (!m_onBack.IsNull)
            {
                m_onBack.SafeInvoke(false);
            }
        }

    }
}

#endif