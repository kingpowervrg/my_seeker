using EngineCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace SeekerGame
{
    public class ShakeVitEffectItemView : GameUIComponent
    {

        protected GameUIEffect m_effect;
        GameRecycleContainer m_parent = null;
        protected override void OnInit()
        {
            base.OnInit();
            m_effect = this.Make<GameUIEffect>("UI_tili_jiesuan_doudong");
            m_effect.EffectPrefabName = "UI_tili_jiesuan_doudong.prefab";
        }

        public override void OnShow(object param)
        {
            base.OnShow(param);

            TimeModule.Instance.SetTimeout(RecycleMyself, 1.0f);
        }

        public override void OnHide()
        {
            base.OnHide();

        }



        public void RegisterRecycleParent(GameRecycleContainer parent_)
        {
            m_parent = parent_;
        }

        void RecycleMyself()
        {
            if (null == m_parent)
                return;

            m_parent.RecycleElement<ShakeVitEffectItemView>(this);
        }


    }
}