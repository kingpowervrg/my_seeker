using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DG.Tweening;
using EngineCore;
using UnityEngine;
namespace SeekerGame
{
    class DropItemIconShake : DropItemIcon
    {
        protected TweenScale m_exhabit_effect;

        protected override void OnInit()
        {
            base.OnInit();

            m_exhabit_effect = m_icon.GetComponent<TweenScale>();
        }

        public override void OnShow(object param)
        {
            base.OnShow(param);


        }


        public override void OnHide()
        {
            base.OnHide();


        }

        public override void SetNum(int num_, float duration_)
        {
            base.SetNum(num_, duration_);

            m_exhabit_effect.Duration = Mathf.Equals(m_exhabit_effect.Duration, 0.0f) ? 0.2f : m_exhabit_effect.Duration;
            m_exhabit_effect.LoopTimes = (int)(duration_ / m_exhabit_effect.Duration);
            m_exhabit_effect.ResetAndPlay();
            
        }

    }

}
