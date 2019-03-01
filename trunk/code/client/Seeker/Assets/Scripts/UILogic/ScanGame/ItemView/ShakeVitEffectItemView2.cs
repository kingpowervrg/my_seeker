using EngineCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace SeekerGame
{
    public class ShakeVitEffectItemView2 : GameUIComponent
    {

        protected GameUIEffect m_effect;
        float m_delay;
        protected override void OnInit()
        {
            base.OnInit();

        }

        public override void OnShow(object param)
        {
            base.OnShow(param);

            TimeModule.Instance.SetTimeout(RecycleMyself, m_delay);
        }

        public override void OnHide()
        {
            base.OnHide();
            TimeModule.Instance.RemoveTimeaction(RecycleMyself);
        }

        public void InitPrefabNameAndDelay(string name_, float delay_)
        {
            if (null != m_effect)
                return;

            m_effect = this.Make<GameUIEffect>(name_);
            m_effect.EffectPrefabName = $"{name_}.prefab";

            m_delay = delay_;
        }

        public void Shake()
        {
            Debug.Log("shake !!!!!!!!!!!!!!!!!!!!!!");
            if (this.Visible)
                return;

            this.Visible = true;
        }

        void RecycleMyself()
        {
            this.Visible = false;
            GameEvents.UIEvents.UI_Scan_Event.Listen_ShakeFinished.SafeInvoke();
        }


    }
}