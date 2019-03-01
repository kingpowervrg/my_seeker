using EngineCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace SeekerGame
{
    public class WinOutputItem : GameUIComponent
    {
        GameImage m_icon;
        GameLabel m_num;
        GameUIEffect m_effect;

        protected override void OnInit()
        {
            m_icon = this.Make<GameImage>("Image");
            m_num = this.Make<GameLabel>("Text_1");
            m_effect = this.Make<GameUIEffect>("UI_chanchuneirong");
            m_effect.EffectPrefabName = "UI_chanchuneirong.prefab";
        }

        public override void OnShow(object param)
        {
            //m_effect.Visible = true;
        }

        public override void OnHide()
        {
            m_effect.Visible = false;
        }


        public void Refresh(string icon_, string num_)
        {
            m_icon.Sprite = icon_;
            m_num.Text = num_;
        }

        public void ShowEffectDelay(float time_)
        {
            TimeModule.Instance.SetTimeout(() => m_effect.Visible = true, time_);
        }
    }

}
