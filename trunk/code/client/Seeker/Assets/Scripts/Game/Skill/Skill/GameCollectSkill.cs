using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EngineCore;

namespace SeekerGame
{
    public class GameCollectSkill : GameSkillBase
    {
        public GameCollectSkill(long skillId) : base(skillId)
        {
        }

        public override void OnStart()
        {
            base.OnStart();
            if (m_confSkill.type == 11)
            {
                if (m_confSkill.gain > 1)
                {
                    if (GameEvents.MainGameEvents.ScanAndNotifyHintItems != null)
                    {
                        GameEvents.MainGameEvents.ScanAndNotifyHintItems.SafeInvoke(m_confSkill.cd, m_confSkill.gain);
                    }
                }
                else if (m_confSkill.gain == 1)
                {
                    if (GameEvents.MainGameEvents.RequestHintSceneItemList != null)
                    {
                        GameEvents.MainGameEvents.RequestHintSceneItemList.SafeInvoke(m_confSkill.gain);
                    }
                }
                
            }
            else if (m_confSkill.type == 12)
            {
                if (GameEvents.MainGameEvents.AutoPickSceneItems != null)
                {
                    GameEvents.MainGameEvents.AutoPickSceneItems(m_confSkill.gain);
                }
            }
            else if (m_confSkill.type == 13)
            {
                GameEvents.MainGameEvents.EnableSceneSpecialEffect.SafeInvoke(false);

                TimeModule.Instance.SetTimeout(RecoverSceneSpecialEffect, m_confSkill.gain);
            }
            else if (m_confSkill.type == 14)
            {
                GameEvents.MainGameEvents.RevertItemNameToNormal.SafeInvoke(m_confSkill.gain,m_carryBase.CarryID);
            }
            else if (m_confSkill.type == 1)
            {
                GameEvents.MainGameEvents.AddGameTime.SafeInvoke(true, m_confSkill.gain);
            }
            else if (m_confSkill.type == 2)
            {
                GameEvents.MainGameEvents.AddGameTime.SafeInvoke(false, m_confSkill.gain);
            }
        }

        private void RecoverSceneSpecialEffect()
        {
            GameEvents.MainGameEvents.EnableSceneSpecialEffect.SafeInvoke(true);
        }


        public override void BreadSkill()
        {
            base.BreadSkill();
            TimeModule.Instance.RemoveTimeaction(RecoverSceneSpecialEffect);
        }

        private float remainTime = 0f;
        public override void OnPause()
        {
            base.OnPause();
            if (m_confSkill.type == 13)
            {
                TimeModule.TimeFunc timeFunc = TimeModule.Instance.GetDuplicate(RecoverSceneSpecialEffect);
                if (timeFunc == null)
                {
                    return;
                }
                this.remainTime = timeFunc.time;
                TimeModule.Instance.RemoveTimeaction(RecoverSceneSpecialEffect);
            }
        }

        public override void OnResume()
        {
            base.OnResume();
            if (m_confSkill.type == 13)
            {
                if (remainTime > 0f)
                {
                    TimeModule.Instance.SetTimeout(RecoverSceneSpecialEffect, remainTime);
                }
            }
            
        }
    }
}
