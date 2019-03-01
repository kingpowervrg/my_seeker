using System;
using System.Collections.Generic;
using EngineCore;
using UnityEngine;
using DG.Tweening;

namespace SeekerGame
{
    public class GameSkillEffectSystem
    {
        private Dictionary<string, List<GameUIEffect>> m_unUseEffect = new Dictionary<string, List<GameUIEffect>>();
        private Dictionary<string, List<GameUIEffect>> m_usingEffect = new Dictionary<string, List<GameUIEffect>>();
        private GameUIContainer m_containerEffect = null;
        public GameSkillEffectSystem(GameUIContainer containEffect)
        {
            this.m_containerEffect = containEffect;
        }

        public void OnDestory()
        {

        }

        private void PushToUnUseEffect(string effectName, GameUIEffect effect)
        {
            if (m_unUseEffect.ContainsKey(effectName))
            {
                m_unUseEffect[effectName].Add(effect);
            }
            else
            {
                m_unUseEffect.Add(effectName, new List<GameUIEffect>() { effect });
            }
        }

        private void RemoveToUnUseEffect(string effectName, GameUIEffect effect)
        {
            if (m_unUseEffect.ContainsKey(effectName))
            {
                if (m_unUseEffect[effectName].Count > 0)
                {
                    m_unUseEffect[effectName].Remove(effect);
                }
                if (m_unUseEffect[effectName].Count <= 0)
                {
                    m_unUseEffect.Remove(effectName);
                }
            }
        }

        private void PushToUsingEffect(string effectName, GameUIEffect effect)
        {
            if (m_usingEffect.ContainsKey(effectName))
            {
                m_usingEffect[effectName].Add(effect);
            }
            else
            {
                m_usingEffect.Add(effectName, new List<GameUIEffect>() { effect });
            }
        }

        private void RemoveToUsingEffect(string effectName, GameUIEffect effect)
        {
            if (m_usingEffect.ContainsKey(effectName))
            {
                if (m_usingEffect[effectName].Count > 0)
                {
                    m_usingEffect[effectName].Remove(effect);
                }
                if (m_usingEffect[effectName].Count <= 0)
                {
                    m_usingEffect.Remove(effectName);
                }
            }
        }

        private GameUIEffect CreateEffect(string effectName)
        {
            if (m_unUseEffect.ContainsKey(effectName))
            {
                if (m_unUseEffect[effectName].Count > 0)
                {
                    GameUIEffect effect = m_unUseEffect[effectName][0];
                    RemoveToUnUseEffect(effectName, effect);
                    PushToUsingEffect(effectName, effect);
                    return effect;
                }
                return null;
            }
            else
            {
                GameUIEffect effect = m_containerEffect.AddChild<GameUIEffect>();
                effect.EffectPrefabName = effectName;
                return effect;
            }
        }

        public void MoveEffect(string effectName, Vector3 startPos, Vector3 endPos, Action callback)
        {
            GameUIEffect effect = CreateEffect(effectName);
            effect.Position = startPos;
            effect.Visible = true;
            Vector3[] wayPoint = new Vector3[2];
            wayPoint[0] = startPos;
            wayPoint[1] = endPos;
            effect.Widget.DOPath(wayPoint, 1f, PathType.CatmullRom, PathMode.TopDown2D).SetEase(Ease.InOutQuad).OnComplete(() =>
            {
                RemoveToUsingEffect(effectName, effect);
                PushToUnUseEffect(effectName, effect);
                effect.Visible = false;
                if (callback != null)
                {
                    callback();
                }
            });
        }

#if OFFICER_SYS
        public void PlayTuoWeiEffect(long playId,Vector3 endPos,Action callback)
        {
            Transform trans = GameEvents.UIEvents.UI_GameMain_Event.GetHeroItemById(playId);
            MoveEffect("UI_fancimoshi_tuowei.prefab", trans.position, endPos,callback);
        }
#endif
    }
}
