using EngineCore;
using GOGUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
namespace SeekerGame
{
    public class SceneLevelUpView : BaseViewComponet<SceneLevelUpUILogic>
    {
        #region 标题
        GameUIComponent m_title_root;
        GameLabel m_title_txt;
        GameUIEffect m_title_effect;
        #endregion



        #region 等级
        GameUIComponent m_lvl_root;
        GameLabel m_from_lvl_txt;
        GameLabel m_to_lvl_txt;
        GameProgressBar m_lvl_slider;
        GameLabel m_cur_vit_txt;
        GameLabel m_max_vit_txt;
        GameUIEffect m_lvl_bar_effect;

        GameUIEffect m_lvl_effect;
        #endregion



        #region 奖励
        GameUIContainer m_grid;
        #endregion



        #region 按钮
        GameButton m_ok_btn;
        GameLabel m_btn_title;
        #endregion

        SCSceneRewardResponse m_data;
        int m_idx;
        float m_max_vit;
        protected override void OnInit()
        {
            base.OnInit();

            m_title_root = this.Make<GameUIComponent>("Image");
            m_title_txt = m_title_root.Make<GameLabel>("Text");
            m_title_txt.Text = string.Format("{0}!", LocalizeModule.Instance.GetString("recharge_title").ToUpper());
            m_title_effect = m_title_root.Make<GameUIEffect>("UI_dengjishengji_01");
            m_title_effect.EffectPrefabName = "UI_dengjishengji_01.prefab";


            m_lvl_root = this.Make<GameUIComponent>("Lvl_Root");
            m_from_lvl_txt = m_lvl_root.Make<GameLabel>("Panel:Text (1)");
            m_to_lvl_txt = m_lvl_root.Make<GameLabel>("Panel:Text (2)");
            m_lvl_slider = m_lvl_root.Make<GameProgressBar>("Image_energy:Slider");
            m_cur_vit_txt = m_lvl_root.Make<GameLabel>("Image_energy:Text_number");
            m_max_vit_txt = m_lvl_root.Make<GameLabel>("Image_energy:Text_number_max");
            m_lvl_bar_effect = m_lvl_root.Make<GameUIEffect>("Image_energy:UI_tili_zengjia");
            m_lvl_bar_effect.EffectPrefabName = "UI_tili_zengjia.prefab";
            m_lvl_effect = m_lvl_root.Make<GameUIEffect>("Panel:UI_dengjishengji_02");
            m_lvl_effect.EffectPrefabName = "UI_dengjishengji_02.prefab";

            m_grid = this.Make<GameUIContainer>("Scroll View:Viewport");

            m_ok_btn = this.Make<GameButton>("btnSure");
            m_btn_title = m_ok_btn.Make<GameLabel>("Text");


        }

        public override void OnShow(object param)
        {
            base.OnShow(param);
            m_ok_btn.AddClickCallBack(OnClick);

        }

        public override void OnHide()
        {
            base.OnHide();
            m_ok_btn.RemoveClickCallBack(OnClick);

            StopRefreshVitUIEffect();
            m_title_effect.Visible = false;
            m_lvl_effect.Visible = false;
            m_lvl_bar_effect.Visible = false;
        }

        public void Refresh(SCSceneRewardResponse msg_)
        {
            m_data = msg_;

            this.Refresh();
        }

        private void Refresh()
        {
            m_from_lvl_txt.Text = string.Format("Lv.{0}", m_data.Difficulty - 1);
            m_to_lvl_txt.Text = string.Format("Lv.{0}", m_data.Difficulty);
            m_max_vit = CommonData.MAXVIT;
            m_cur_vit_txt.Text = GlobalInfo.MY_PLAYER_INFO.Vit.ToString();
            m_max_vit_txt.Text = string.Format("/{0}", m_max_vit);

            m_grid.EnsureSize<DropItemIcon>(m_data.GiftItems.Count);

            for (int i = 0; i < m_grid.ChildCount; ++i)
            {
                ConfProp prop = ConfProp.Get(m_data.GiftItems[i].ItemId);

                m_grid.GetChild<DropItemIcon>(i).InitSprite(prop.icon, m_data.GiftItems[i].Num, prop.id);
                m_grid.GetChild<DropItemIcon>(i).Visible = true;
            }
            m_idx = 0;

            StepTo(++m_idx);

        }

        private void StepTo(int idx_)
        {

            if (1 == idx_)
            {
                this.m_title_root.Visible = true;
                this.m_lvl_root.Visible = false;
                this.m_grid.Visible = false;
                this.m_ok_btn.Visible = false;

                m_title_effect.Visible = true;


                float pending_time = 1.5f;

                TimeModule.Instance.SetTimeout(() => StepTo(++m_idx), pending_time);
            }
            else if (2 == idx_)
            {
                this.m_title_root.Visible = true;
                this.m_lvl_root.Visible = true;
                this.m_grid.Visible = false;
                this.m_ok_btn.Visible = false;

                m_lvl_effect.Visible = true;
                m_lvl_bar_effect.Visible = true;

                float pending_time = 0.5f;

                m_lvl_bar_effect.Visible = true;
                /*     TweenProgressBarValue.Begin(m_lvl_slider.gameObject, pending_time, 0f, 1.0f);
                     TweenText.Begin(m_cur_vit_txt.Label, pending_time, 0f, 0f, m_max_vit);*/
                TimeModule.Instance.SetTimeInterval(StartRefreshExpUIEffect, 0.0001f);


                TimeModule.Instance.SetTimeout(() => StepTo(++m_idx), pending_time);
            }
            else if (3 == idx_)
            {
                this.m_title_root.Visible = true;
                this.m_lvl_root.Visible = true;
                this.m_grid.Visible = true;
                this.m_ok_btn.Visible = true;
            }
        }

        private void StartRefreshExpUIEffect()
        {
            CommonHelper.EffectProgressbarValueSync(m_lvl_slider, m_lvl_bar_effect);
        }
        private void StopRefreshVitUIEffect()
        {
            TimeModule.Instance.RemoveTimeaction(StartRefreshExpUIEffect);
        }

        protected virtual void OnClick(GameObject obj)
        {
            EngineCoreEvents.AudioEvents.PlayAudio.SafeInvoke(Audio.AudioType.UISound, EngineCommonAudioKey.Button_Click_Common.ToString());


            CurViewLogic().OnClose();

        }
    }
}
