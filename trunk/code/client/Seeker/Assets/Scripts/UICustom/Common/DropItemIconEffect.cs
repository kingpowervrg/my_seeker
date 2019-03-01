using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DG.Tweening;
using EngineCore;
using UnityEngine;
namespace SeekerGame
{
    class DropItemIconEffect : DropItemIcon
    {
        private GameUIEffect m_exhabit_effect;

        protected override void OnInit()
        {
            base.OnInit();

            m_exhabit_effect = Make<GameUIEffect>("UI_teshuwupin_tishi");
            m_exhabit_effect.EffectPrefabName = "UI_teshuwupin_tishi.prefab";
            m_exhabit_effect.Visible = false;
        }

        public override void OnShow(object param)
        {
            base.OnShow(param);


        }


        public override void OnHide()
        {
            base.OnHide();


        }

        public void InitSprite(string tex_name_, int num_ = 0, long item_id = 0, bool effect_= false)
        {
            base.InitSprite(tex_name_, num_, item_id);

            m_exhabit_effect.Visible = effect_;
        }
    }

}
