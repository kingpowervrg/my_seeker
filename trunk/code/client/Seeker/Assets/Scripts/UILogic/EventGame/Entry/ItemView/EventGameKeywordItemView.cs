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
    public class EventGameKeywordItemView : GameUIComponent
    {
        private GameImage m_icon;

        private GameLabel m_txt;


        private GameUIEffect m_effect;

        private int m_idx;
        public int Idx
        {
            get { return m_idx; }
        }

        private long m_word_id;
        public long Word_id
        {
            get { return m_word_id; }
        }

        protected override void OnInit()
        {
            base.OnHide();

            m_icon = Make<GameImage>("icon");
            m_txt = Make<GameLabel>("Text");
            m_effect = Make<GameUIEffect>("UI_jihuoguanjianci02");
            m_effect.EffectPrefabName = "UI_jihuoguanjianci02.prefab";
            m_effect.Visible = false;
        }

        public override void OnShow(object param)
        {
            base.OnShow(param);
            m_effect.Visible = false;
        }

        public override void OnHide()
        {
            base.OnHide();
            m_effect.Visible = false;
        }



        public void Refresh(int idx_, long word_id_)
        {
            m_idx = idx_;
            m_word_id = word_id_;

            ConfKeyWords kw = ConfKeyWords.Get(word_id_);

            m_icon.Sprite = kw.icon;
            m_txt.Text = LocalizeModule.Instance.GetString(kw.word);
        }

        public void Match(long key_word_id_)
        {
            m_effect.Visible = m_word_id == key_word_id_;
        }

        public void Match(bool v_)
        {
            m_effect.Visible = v_;
        }

    }
}
