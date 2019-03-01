using EngineCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace SeekerGame
{
    public class SkillOutputItem : GameUIComponent
    {
        GameImage m_icon;
        GameLabel m_desc;
        private GameLabel m_skillLevelLab = null;
        List<UITweenerBase> m_tweens;
        protected override void OnInit()
        {
            m_icon = this.Make<GameImage>("Image");
            m_desc = m_icon.Make<GameLabel>("Text");
            m_skillLevelLab = Make<GameLabel>("Image:Image:Text");
            m_tweens = this.gameObject.GetComponents<UITweenerBase>().ToList();
        }

        public override void OnShow(object param)
        {
            m_tweens.ForEach((i) => i.ResetAndPlay());
        }

        public override void OnHide()
        {
            //m_tweens.ForEach((i) => i.ResetAndPlay());
        }


        public void Refresh(string icon_, string desc_, float delay_, int skillLevel)
        {
            m_icon.Sprite = icon_;
            m_desc.Text = desc_;
            m_skillLevelLab.Text = skillLevel.ToString();
            m_tweens.ForEach((i) => { i.ResetAndPlay(); i.Delay = delay_; });
        }

        public void Play()
        {
            m_tweens.ForEach((i) => i.ResetAndPlay());
        }


    }

}
