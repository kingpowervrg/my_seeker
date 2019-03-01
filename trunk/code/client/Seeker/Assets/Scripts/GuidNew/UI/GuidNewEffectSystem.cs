using System;
using System.Collections.Generic;
using UnityEngine;
using EngineCore;

namespace SeekerGame.NewGuid
{
    /// <summary>
    /// 规定大于等于10000为遮罩特效  15000为遮罩圆形特效  20000为手指
    /// </summary>
    public class GuidNewEffectSystem
    {
        private GameUIContainer m_artRoot = null;
        private Dictionary<long, GameUIEffect> m_allEffect = new Dictionary<long, GameUIEffect>();
        public GuidNewEffectSystem(GameUIContainer artRoot)
        {
            this.m_artRoot = artRoot;
            PreLoadEffect(20000, "UI_xinshouyindao_shou.prefab");
            GameEvents.UI_Guid_Event.OnLoadEffect += OnLoadEffect;
            GameEvents.UI_Guid_Event.OnRemoveEffect += OnRemoveEffect;
            GameEvents.UI_Guid_Event.OnClearMaskEffect += OnRemoveMaskEffect;
            GameEvents.UI_Guid_Event.OnClearGuid += OnClearGuid;
            GameEvents.UI_Guid_Event.OnGetMaskEffect = OnGetMaskEffect;
        }

        private void OnLoadEffect(long effectID,string resName,Vector2 pos,Vector2 scale,float rotation)
        {
            if (m_allEffect.ContainsKey(effectID))
            {
                m_allEffect[effectID].Widget.anchoredPosition = pos;
                m_allEffect[effectID].SetRealScale(scale);
                //m_allEffect[effectID].SetRotation(Vector3.forward * rotation); 
                m_allEffect[effectID].Visible = true;
                //m_allEffect[effectID].SetScale(scale);
                //m_allEffect[effectID].gameObject.SetActive(true); 
            }
            else
            {
                GameUIEffect effect = m_artRoot.AddChild<GameUIEffect>();
                effect.gameObject.name = effectID.ToString();
                effect.EffectPrefabName = resName;
                effect.Widget.anchoredPosition = pos;
                effect.SetRealScale(scale);
                //effect.SetRotation(Vector3.forward * rotation);
                effect.Visible = true;
                m_allEffect.Add(effectID,effect);
            }
        }

        private void PreLoadEffect(long effectID, string resName)
        {
            GameUIEffect effect = m_artRoot.AddChild<GameUIEffect>();
            effect.SetRenderQueue(10001);
            effect.gameObject.name = effectID.ToString();
            effect.EffectPrefabName = resName;
            effect.Widget.anchoredPosition = Vector3.zero;
            effect.Visible = false;
            m_allEffect.Add(effectID, effect);
        }

        private int GetEffectIndex(string effectID)
        {
           
            //foreach (var kv in m_allEffect)
            //{
            //    index++;
            //    if (kv.Key == effectID)
            //    {
            //        return index;
            //    }
            //}
            int index = -1;
            int count = m_artRoot.ChildCount;
            for (int i = 0; i < count; i++)
            {
                index++;
                Transform tran = m_artRoot.GetChildByIndex(i);
                if (tran.name.Equals(effectID))
                {
                    return index;
                }
            }
            //Transform tran = m_artRoot.

            return -1;
        }

        private void OnRemoveEffect(long effectID,bool isDestory)
        {
            if (m_allEffect.ContainsKey(effectID))
            {
                int index = GetEffectIndex(effectID.ToString());
                //if (index < 0)
                //{
                //    return;
                //}
                if (isDestory)
                {
                    //Transform effectTran = m_artRoot.Widget.Find(effectID.ToString());
                    //if (effectTran != null)
                    //{
                    //    //m_artRoot.RemoveChildByIndex
                    //}
                    m_allEffect[effectID].Dispose();
                    m_allEffect.Remove(effectID);
                    m_artRoot.RemoveChildByIndex(index);
                    Debug.Log("remove single   === " + effectID);
                }
                else
                {
                    //m_allEffect[effectID].gameObject.SetActive(false);
                     m_allEffect[effectID].Visible = false;
                }
                
            }
        }

        private void OnRemoveMaskEffect(bool isDestory)
        {
            //int index = -1;
            List<long> removeKey = new List<long>();
            foreach (var kv in m_allEffect)
            {
                //index++;
                if (kv.Key >= 10000)
                {
                    if (isDestory)
                    {

                        kv.Value.Dispose();
                        //kv.Value.Visible = false;
                        removeKey.Add(kv.Key);
                        //int index = GetEffectIndex(kv.Key.ToString());
                        //m_artRoot.RemoveChildByIndex(index);
                        Debug.Log("remove mutil   === " + kv.Key);
                    }
                    else
                    {
                        kv.Value.Visible = false;
                        //kv.Value.gameObject.SetActive(false);
                    }
                }
            }
            for (int i = 0; i < removeKey.Count; i++)
            {
                m_allEffect.Remove(removeKey[i]);
            }
        }

        private void OnClearGuid()
        {
            foreach (var kv in m_allEffect)
            {
                kv.Value.Visible = false;
            }
        }

        public void OnDestory()
        {
            GameEvents.UI_Guid_Event.OnLoadEffect -= OnLoadEffect;
            GameEvents.UI_Guid_Event.OnRemoveEffect -= OnRemoveEffect;
            GameEvents.UI_Guid_Event.OnClearMaskEffect -= OnRemoveMaskEffect;
            GameEvents.UI_Guid_Event.OnClearGuid -= OnClearGuid;
        }

        private GameUIEffect OnGetMaskEffect(long effectID)
        {
            if (m_allEffect.ContainsKey(effectID))
            {
                return m_allEffect[effectID];
            }
            return null;
        }
    }
}
