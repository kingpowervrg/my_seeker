//#define TEST

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EngineCore;
using UnityEngine;

namespace SeekerGame
{
    class EventGameEntryNewUIView : BaseViewComponet<EventGameEntryNewUILogic>
    {
#if OFFICER_SYS
        private GameUIContainer m_officers_grid;
#endif
        private GameLabel m_coin_num_txt;
        private GameLabel m_gift_name_txt;
        private GameImage m_event_icon;
        private GameLabel m_event_name;
        private GameLabel m_event_desc;
        private GameLabel m_case_name;
        private GameTexture m_case_tex;
        private GameButton m_ok_btn;
        private GameUIEffect m_ok_effect;
        private GameLabel m_vit_cost_num_txt;

        private GameUIContainer m_keywords_grid;


        private GameLabel m_normal_title_txt;
        private GameLabel m_perfect_title_txt;
        private GameLabel m_normal_cash_txt;
        private GameLabel m_perfect_cash_txt;

        Dictionary<long, List<long>> m_officer_key_word_id_dict;

        protected override void OnInit()
        {
            base.OnInit();
#if OFFICER_SYS
            m_officers_grid = Make<GameUIContainer>("Panel_officer:ScrollView:Viewport");
#endif
            m_event_icon = Make<GameImage>("Image_event:Image_title:Image_Icon");
            m_event_name = Make<GameLabel>("Image_event:Image_title:Text");
            m_event_desc = Make<GameLabel>("Image_event:Text");
            m_case_name = Make<GameLabel>("Image_event:RawImage:Text");
            m_case_tex = Make<GameTexture>("Image_event:RawImage");
            m_ok_btn = Make<GameButton>("Button_action");
            m_ok_effect = Make<GameUIEffect>("UI_tongyong_anniu");
            m_ok_effect.EffectPrefabName = "UI_tongyong_anniu.prefab";
            m_vit_cost_num_txt = m_ok_btn.Make<GameLabel>("Image:Text");

            m_keywords_grid = Make<GameUIContainer>("Panel_keyword:ScrollView:Viewport");

            m_normal_title_txt = Make<GameLabel>("Image_event:Image_normal:Text");
            m_normal_title_txt.Text = LocalizeModule.Instance.GetString("ui.event.CommonDrop");
            m_perfect_title_txt = Make<GameLabel>("Image_event:Image_perfect:Text");
            m_perfect_title_txt.Text = LocalizeModule.Instance.GetString("ui.event.FullScoreDrop");
            m_normal_cash_txt = Make<GameLabel>("Image_event:Image_normal:Image:Text (1)");
            m_perfect_cash_txt = Make<GameLabel>("Image_event:Image_perfect:Image:Text (1)");


#if OFFICER_SYS
            this.InitOfficersKeywords();
#endif
        }




        public override void OnShow(object param)
        {
            base.OnShow(param);

            m_ok_btn.AddClickCallBack(OnStartClick);



        }

        public override void OnHide()
        {
            base.OnHide();

            m_ok_btn.RemoveClickCallBack(OnStartClick);
        }


        public void Refresh(int total_officer_count_)
        {
#if OFFICER_SYS
            m_officers_grid.EnsureSize<EventGameOfficerDispatchItemView>(total_officer_count_);

            for (int i = 0; i < m_officers_grid.ChildCount; ++i)
            {
                m_officers_grid.GetChild<EventGameOfficerDispatchItemView>(i).Init(i);
                m_officers_grid.GetChild<EventGameOfficerDispatchItemView>(i).Visible = true;
            }
#endif

            ConfEvent event_data = ConfEvent.Get(CurViewLogic().Event_id);

            m_vit_cost_num_txt.Text = event_data.vitConsume.ToString();

            HashSet<long> keywords = new HashSet<long>();

            foreach (var phase_id in event_data.phases)
            {
                var phase = ConfEventPhase.Get(phase_id);

                foreach (var key_id in phase.keyWords)
                {
                    keywords.Add(key_id);
                }
            }

            m_keywords_grid.EnsureSize<EventGameKeywordItemView>(keywords.Count);

            int gi = 0;
            foreach (var item in keywords)
            {
                m_keywords_grid.GetChild<EventGameKeywordItemView>(gi).Refresh(gi, item);
                m_keywords_grid.GetChild<EventGameKeywordItemView>(gi).Visible = true;
                ++gi;
            }

            m_event_icon.Sprite = ConfEventAttribute.Get(event_data.type).icon;
            m_event_name.Text = LocalizeModule.Instance.GetString(event_data.name);
            m_event_desc.Text = LocalizeModule.Instance.GetString(event_data.descs);
            m_case_name.Text = LocalizeModule.Instance.GetString(ConfEventAttribute.Get(event_data.type).name);
            m_case_tex.TextureName = event_data.sceneInfo;


            m_normal_cash_txt.Text = event_data.coinGain.ToString();
            m_perfect_cash_txt.Text = event_data.cashGain.ToString();

        }

        public void RefreshDispatch(int idx_, long officer_id_, bool add_ = true)
        {

            RefreshKeywordEffect();

            if (!add_)
                return;

#if OFFICER_SYS
            for (int i = 0; i < m_officers_grid.ChildCount; ++i)
            {
                EventGameOfficerDispatchItemView item = m_officers_grid.GetChild<EventGameOfficerDispatchItemView>(i);
                if (item.Idx == idx_)
                {
                    item.Refresh(officer_id_);
                    return;
                }
            }
#endif
        }

        private void OnStartClick(GameObject obj_)
        {
            CurViewLogic().RequestEnter();
        }
#if OFFICER_SYS
        private void InitOfficersKeywords()
        {
            this.m_officer_key_word_id_dict = new Dictionary<long, List<long>>();
            List<long> officers = new List<long>(GlobalInfo.MY_PLAYER_INFO.Officer_dict.Keys);
            for (int i = 0; i < officers.Count; ++i)
            {
                m_officer_key_word_id_dict.Add(officers[i], EventGameUIAssist.GetPoliceKeyWordByOfficerID(officers[i]));
            }
        }

#endif

        private void RefreshKeywordEffect()
        {
            HashSet<long> all_dispatch_keywords = new HashSet<long>();
#if OFFICER_SYS
            foreach (var officer_id in EventGamePoliceDispatchManager.Instance.GetAllDispathOfficersID())
            {

                List<long> cur_key_word = m_officer_key_word_id_dict[officer_id];
                all_dispatch_keywords.UnionWith(cur_key_word);
            }
#endif
            for (int i = 0; i < m_keywords_grid.ChildCount; ++i)
            {
                var item = m_keywords_grid.GetChild<EventGameKeywordItemView>(i);
                long k_id = item.Word_id;

                if (all_dispatch_keywords.Contains(k_id))
                {
                    item.Match(true);
                }
                else
                {
                    item.Match(false);
                }
            }
        }
    }
}
