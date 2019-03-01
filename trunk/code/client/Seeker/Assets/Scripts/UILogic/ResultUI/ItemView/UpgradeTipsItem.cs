using EngineCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace SeekerGame
{
    public class UpgradeTipsItem : GameUIComponent
    {
        SafeAction<string> m_act;
        GameImage m_icon;
        GameLabel m_text;
        GameUIEffect m_effect;

        private string m_link_ui_name;
        protected override void OnInit()
        {
            base.OnInit();
            m_icon = this.Make<GameImage>("Background");
            m_text = this.Make<GameLabel>("Text");
            m_effect = this.Make<GameUIEffect>("UI_shibai_zengqiang");
            m_effect.EffectPrefabName = "UI_shibai_zengqiang.prefab";
        }

        public override void OnShow(object param)
        {
            base.OnShow(param);
            m_icon.AddClickCallBack(OnClicked);
            m_effect.Visible = true;
        }

        public override void OnHide()
        {
            base.OnHide();
            m_icon.RemoveClickCallBack(OnClicked);
            m_effect.Visible = false;
        }

        public void Refresh(string icon_, string txt_, string ui_name_, Action<string> act_ = null)
        {
            m_icon.Sprite = icon_;
            m_text.Text = LocalizeModule.Instance.GetString(txt_);
            m_link_ui_name = ui_name_;

            if (null != act_)
                m_act = act_;
        }

        private void OnClicked(GameObject obj)
        {

            if (!m_act.IsNull)
                this.m_act.SafeInvoke(m_link_ui_name);

            GameEvents.UIEvents.UI_GameEntry_Event.OnOpenPanel.SafeInvoke(m_link_ui_name);
        }
    }
}