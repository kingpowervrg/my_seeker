using EngineCore;
using System;
using System.Collections.Generic;
using UnityEngine;
namespace SeekerGame
{
    public class CombineTipsView : GameUIComponent
    {
        TweenPosition m_tween;
        GameLabel m_combine_name_txt;
        BigStuffItemView[] m_big_stuffs = new BigStuffItemView[6];

        public bool Busy { get; set; } = false;

        protected override void OnInit()
        {
            base.OnInit();

            m_tween = this.GetComponent<TweenPosition>();
            m_combine_name_txt = Make<GameLabel>("Text");
            for (int i = 0; i < 6; ++i)
            {
                m_big_stuffs[i] = Make<BigStuffItemView>($"Image ({i})");
            }
        }

        public override void OnShow(object param)
        {
            base.OnShow(param);
            m_tween.AddTweenCompletedCallback(OnTweenFinished);
        }

        public override void OnHide()
        {
            base.OnHide();
            m_tween.RemoveTweenCompletedCallback(OnTweenFinished);
        }


        public void Refresh()
        {
            if (Busy)
                return;

            long next_tip_id = CombineDataManager.Instance.FetchPropTips();
            if (0 == next_tip_id)
                return;

            Busy = true;

            RefreshProps(next_tip_id);

            this.Visible = true;
            m_tween.ResetAndPlay();

        }

        void RefreshProps(long id_)
        {
            ConfCombineFormula data = ConfCombineFormula.Get(id_);
            m_combine_name_txt.Text = LocalizeModule.Instance.GetString(data.name);
            long[] big_props = new long[] { data.propId1, data.propId2, data.propId3, data.propId4, data.propId5, data.propId6 };

            PlayerPropMsg prop_info;
            bool have;
            int num;
            for (int i = 0; i < big_props.Length; ++i)
            {
                if (0 == big_props[i])
                    continue;

                prop_info = GlobalInfo.MY_PLAYER_INFO.GetBagInfosByID(big_props[i]);
                have = null != prop_info ? true : false;
                num = have ? prop_info.Count : 0;

                m_big_stuffs[i].Refresh(big_props[i], num, ConfProp.Get(big_props[i]).icon, have);
            }

        }

        void OnTweenFinished()
        {
            this.Visible = false;
            Busy = false;

            Refresh();
        }
    }

}

