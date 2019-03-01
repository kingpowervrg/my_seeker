//#define TEST
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EngineCore;
using UnityEngine;
using GOEngine;
using Google.Protobuf;
using DG.Tweening;
using DG.Tweening.Plugins.Options;

namespace SeekerGame
{
    public class WinView : BaseViewComponet<ResultUILogic>
    {

        private GameButton m_win_continue_btn;
        private GameUIEffect m_win_effect;

        WinLvlUpDetailView m_lvl_up_detail_view;
        WinLvlUpRewardView m_lvl_up_reward_view;
        CanvasGroup m_lvl_up_reward_cg;
        TweenAlpha m_lvl_up_reward_hide_tween;

        WinNormalRewardView m_normal_reward_view;

        private ENUM_SEARCH_MODE m_mode = ENUM_SEARCH_MODE.E_INVALID;
        WinFailData m_data;
        private int m_cur_step;

        private const float STS_TWEEN_TIME = 1.0f;

        protected override void OnInit()
        {
            base.OnInit();

            m_win_continue_btn = Make<GameButton>("btnSure");
            m_lvl_up_detail_view = Make<WinLvlUpDetailView>("Panel_level");
            m_lvl_up_reward_view = Make<WinLvlUpRewardView>("Panel_levelupreward");
            m_lvl_up_reward_cg = m_lvl_up_reward_view.GetComponent<CanvasGroup>();
            m_lvl_up_reward_hide_tween = m_lvl_up_reward_view.GetComponents<TweenAlpha>().Where((e) => UITweenerBase.TweenTriggerType.OnHide == e.m_triggerType).FirstOrDefault();
            m_lvl_up_reward_hide_tween.AddTweenCompletedCallback(LvlUpRewardHideFinished);
            m_normal_reward_view = Make<WinNormalRewardView>("Panel_reward");
            //m_win_effect = Make<GameUIEffect>("RawImage:UI_changjing_jiesuan");
            //m_win_effect.EffectPrefabName = "UI_changjing_jiesuan.prefab";
            //m_win_effect.Visible = false;
        }


        public override void OnShow(object param)
        {
            base.OnShow(param);

            if (null != param)
            {
                var ret = param as WinFailData;

                this.m_mode = ret.m_mode;
                m_data = ret;
            }
            m_win_continue_btn.AddClickCallBack(ContinueClicked);

            //m_win_effect.Visible = true;

            m_lvl_up_reward_view.gameObject.transform.localPosition = Vector3.zero;
            m_lvl_up_reward_cg.alpha = 1.0f;


            m_cur_step = 0;
            UiStepTo(++m_cur_step);
        }

        public override void OnHide()
        {
            base.OnHide();

            m_win_continue_btn.RemoveClickCallBack(ContinueClicked);

           
        }

        public void Refresh(WinFailData data_)
        {
            m_mode = data_.m_mode;
            m_data = data_;
        }


        private void ContinueClicked(GameObject go)
        {
            EngineCoreEvents.AudioEvents.PlayAudio.SafeInvoke(Audio.AudioType.UISound, EngineCommonAudioKey.Button_Click_Common.ToString());
            UiStepTo(++m_cur_step);
        }

        private void UiStepTo(int step_)
        {
            if (1 == step_)
            {

                if (ENUM_SEARCH_MODE.E_JIGSAW == this.m_mode)
                {
                    ShowNormalRewardFirst();
                }
                else if (ENUM_SEARCH_MODE.E_SEARCH_ROOM == this.m_mode)
                {
                    SCSceneRewardResponse msg = m_data.m_msg as SCSceneRewardResponse;

                    if (msg.UpLevelRewards.Count > 0)
                        ShowLvlUpReward();
                    else
                        ShowNormalRewardFirst();
                }

            }
            else if (2 == step_)
            {
                if (ENUM_SEARCH_MODE.E_JIGSAW == this.m_mode)
                {
                    CurViewLogic().Quit();
                }
                else if (ENUM_SEARCH_MODE.E_SEARCH_ROOM == this.m_mode)
                {

                    SCSceneRewardResponse msg = m_data.m_msg as SCSceneRewardResponse;

                    if (msg.UpLevelRewards.Count > 0)
                        HideLvlUpReward();
                    else
                        CurViewLogic().Quit();
                }
            }
            else if (3 == step_)
            {

                CurViewLogic().Quit();
            }
        }

        void ShowNormalRewardFirst()
        {
            m_lvl_up_detail_view.SetVisible(false);
            m_lvl_up_reward_view.SetVisible(false);
            ShowNormalRewardSencond();
        }

        void ShowNormalRewardSencond()
        {
            m_normal_reward_view.Refresh(m_data);
            m_normal_reward_view.Visible = true;
        }

        void ShowLvlUpReward()
        {
            m_normal_reward_view.SetVisible(false);
            m_lvl_up_detail_view.Refresh(m_data);
            m_lvl_up_detail_view.Visible = true;
            m_lvl_up_reward_view.Refresh(m_data);
            m_lvl_up_reward_view.Visible = true;
        }

        void HideLvlUpReward()
        {
            m_lvl_up_detail_view.Visible = false;
            m_lvl_up_reward_view.Visible = false;
        }



        void LvlUpRewardHideFinished()
        {
            ShowNormalRewardSencond();
        }


    }


    public class OutPutData
    {
        public long ItemID { get; set; }
        public string Icon { get; set; }
        public int Count { get; set; }
    }


}