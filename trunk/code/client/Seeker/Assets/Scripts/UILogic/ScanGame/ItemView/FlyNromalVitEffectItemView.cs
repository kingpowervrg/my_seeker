using EngineCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace SeekerGame
{
    public class FlyNromalVitEffectItemView : FlyVitEffectItemView
    {

        protected override void OnInit()
        {
            base.OnInit();
            m_effect = this.Make<GameUIEffect>("UI_tili_jiesuan_tuowei");
            m_effect.EffectPrefabName = "UI_tili_jiesuan_tuowei.prefab";
            m_tween_pos = this.gameObject.GetComponent<TweenPosition>();
        }

    }
}