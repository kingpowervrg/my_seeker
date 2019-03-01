using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EngineCore;
using UnityEngine;

namespace SeekerGame
{
    public class PromoptView : BaseViewComponet<BindUILogic>
    {
        private GameImage m_icon;
        private GameNetworkRawImage m_http_icon;
        private GameLabel m_name_txt;
        private GameLabel m_user_id_txt;
        private GameLabel m_lvl_txt;
        private GameLabel m_title_txt;
        private GameLabel m_detail_txt;
        private GameButton m_ok_btn;
        //private GameButton m_canel_btn;
        //private GameButton m_close_btn;

        private string m_identify;

        private SafeAction m_OnOK;
        protected override void OnInit()
        {
            base.OnInit();

            m_icon = Make<GameImage>("Image:icon");
            m_http_icon = Make<GameNetworkRawImage>("Image:icon (1)");
            m_name_txt = Make<GameLabel>("Image:name");
            m_user_id_txt = Make<GameLabel>("Image:id:id (1)");
            m_lvl_txt = Make<GameLabel>("Image:id (1):id (1)");
            m_title_txt = Make<GameLabel>("title");
            m_title_txt.Text = LocalizeModule.Instance.GetString("UI_FB.qiehuantishi");
            m_detail_txt = Make<GameLabel>("detail");
            m_detail_txt.Text = LocalizeModule.Instance.GetString("UI_FB.bangdingtixing");
            m_ok_btn = Make<GameButton>("btn_buy");
            //m_canel_btn = Make<GameButton>("btn_quit");
            //m_close_btn = Make<GameButton>("Button");
        }

        public override void OnShow(object param)
        {
            base.OnShow(param);

            m_ok_btn.AddClickCallBack(OnOkClicked);
            //m_canel_btn.AddClickCallBack(OnCancelClicked);
            //m_close_btn.AddClickCallBack(OnCancelClicked);
        }

        public override void OnHide()
        {
            base.OnHide();

            m_ok_btn.RemoveClickCallBack(OnOkClicked);
            //m_canel_btn.RemoveClickCallBack(OnCancelClicked);
            //m_close_btn.RemoveClickCallBack(OnCancelClicked);
        }

        public void Refresh(BindPromoptData data_)
        {
            if (data_.m_icon_name.Contains("http") || data_.m_icon_name.Contains("https"))
            {
                m_http_icon.TextureName = data_.m_icon_name;
                m_http_icon.Visible = true;
                m_icon.Visible = false;
            }
            else
            {
                m_icon.Sprite = data_.m_icon_name;
                m_icon.Visible = true;
                m_http_icon.Visible = false;
            }
            m_name_txt.Text = data_.m_name;
            m_user_id_txt.Text = data_.m_user_id.ToString();
            m_lvl_txt.Text = data_.m_lvl.ToString();
            m_OnOK = data_.m_OnOK;
        }

        private void OnOkClicked(GameObject obj_)
        {
            CurViewLogic().OnQuit();

            m_OnOK.SafeInvoke();
        }

        private void OnCancelClicked(GameObject obj_)
        {
            CurViewLogic().OnQuit();
        }
    }
}
