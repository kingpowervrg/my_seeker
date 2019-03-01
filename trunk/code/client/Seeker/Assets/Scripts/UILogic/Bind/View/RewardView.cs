using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EngineCore;
using UnityEngine;

namespace SeekerGame
{
    public class RewardView : BaseViewComponet<BindUILogic>
    {
        GameLabel m_count_txt;
        GameLabel m_title_txt;
        GameLabel m_detail_txt;
        GameButton m_quit_btn;
        SafeAction m_OnQuit;
        protected override void OnInit()
        {
            base.OnInit();

            m_count_txt = Make<GameLabel>("icon (1):name");
            m_title_txt = Make<GameLabel>("title");
            m_title_txt.Text = LocalizeModule.Instance.GetString("UI_FB.bangdingtixing");
            m_detail_txt = Make<GameLabel>("detail");
            m_detail_txt.Text = LocalizeModule.Instance.GetString("UI_FB.bangding");
            m_quit_btn = Make<GameButton>("btn_quit");
        }

        public override void OnShow(object param)
        {
            base.OnShow(param);

            m_quit_btn.AddClickCallBack(OnQuitClick);
        }

        public override void OnHide()
        {
            base.OnHide();

            m_quit_btn.RemoveClickCallBack(OnQuitClick);
        }

        public void Refresh(BindRewardData data_)
        {
            m_count_txt.Text = string.Format("x{0}", data_.m_count);
            m_OnQuit = data_.m_OnOK;
        }

        private void OnQuitClick(GameObject obj_)
        {
            CurViewLogic().OnQuit();

            m_OnQuit.SafeInvoke();
        }
    }
}
