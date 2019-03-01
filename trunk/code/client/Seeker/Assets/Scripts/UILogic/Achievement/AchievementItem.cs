using EngineCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace SeekerGame
{
    public class AchievementItem : GameLoopItem
    {

        private GameImage m_Icon_img;
        private GameLabel m_Name_lab;
        private GameLabel m_Time_lab;
        private GameImage m_Lock_img;
        private GameImage m_Tips_img;
        //private GameButton m_item_btn;
        private AchievementMsg m_data;

        private int m_Submit = 0;
        private GameUIEffect m_receiveEffect = null;
        private bool m_showLock = false;

        protected override void OnInit()
        {
            base.OnInit();
            m_Icon_img = Make<GameImage>("icon");
            m_Name_lab = Make<GameLabel>("Text_name");
            m_Time_lab = Make<GameLabel>("Text_time");
            m_Lock_img = Make<GameImage>("Image_lock");
            m_Tips_img = Make<GameImage>("Image_tips");
            //m_Tips_img.Visible = false;
            //m_item_btn = Make<GameButton>(gameObject);
            this.m_receiveEffect = Make<GameUIEffect>("Image_tips:UI_zhuxianrenwu_liwu");
        }

        protected override void OnLoopItemBecameVisible()
        {
            m_Icon_img.AddClickCallBack(Click);
            GameEvents.UIEvents.UI_Achievement_Event.OnReceiveData += OnReceiveData;

            //if (m_data == null)
            //{
            //    Visible = false;
            //    return;
            //}

            OnInitUI(m_data);
            if (!m_showLock)
            {
                m_Lock_img.Visible = false;
                m_Icon_img.Color = Color.white;
            }
        }

        protected override void OnLoopItemBecameInvisible()
        {
            this.m_receiveEffect.Visible = false;
            m_Icon_img.RemoveClickCallBack(Click);
            GameEvents.UIEvents.UI_Achievement_Event.OnReceiveData -= OnReceiveData;
        }

        public void SetData(AchievementMsg msg, bool showLock)
        {
            m_data = msg;
            m_showLock = showLock;
        }

        private void OnInitUI(AchievementMsg msg)
        {
            ConfAchievement confItem = ConfAchievement.Get(msg.Id);

            m_Name_lab.Text = LocalizeModule.Instance.GetString(confItem.name);
            m_Icon_img.Sprite = confItem.rewardicon1;
            if (msg.Progress < confItem.progress1)
            {
                m_Submit = 0;
                m_Lock_img.Visible = true;
                m_Icon_img.Color = new Color(1, 1, 1, 0.6f);
                m_Time_lab.Visible = false;
                m_Tips_img.Visible = false;
            }
            else
            {
                long finishTime = msg.FinishTime1;

                if (msg.Progress < confItem.progress2 && msg.Progress >= confItem.progress1)
                {
                    m_Icon_img.Sprite = confItem.rewardicon1;
                    m_Submit = 2;
                    finishTime = msg.FinishTime1;
                }
                else if (msg.Progress < confItem.progress3 && msg.Progress >= confItem.progress2)
                {
                    m_Icon_img.Sprite = confItem.rewardicon2;
                    m_Submit = 6;
                    finishTime = msg.FinishTime2;
                }
                else if (msg.Progress >= confItem.progress3)
                {
                    m_Icon_img.Sprite = confItem.rewardicon3;
                    m_Submit = 14;
                    finishTime = msg.FinishTime;
                }
                m_Tips_img.Visible = (msg.SubmitStatus < m_Submit);
                DateTime dt = CommonTools.TimeStampToDateTime(finishTime * 10000);
                if (dt != null)
                {
                    m_Time_lab.Text = string.Format("{0:D2}.{1:D2}.{2:D2}", dt.Year, dt.Month, dt.Day);
                }
                m_Lock_img.Visible = false;
                m_Icon_img.Color = Color.white;
                LoadTipEffect();
            }
        }

        private void LoadTipEffect()
        {
            if (m_Tips_img.Visible)
            {
                this.m_receiveEffect.EffectPrefabName = "UI_zhuxianrenwu_liwu.prefab";
                this.m_receiveEffect.Visible = true;
            }
            else
            {
                this.m_receiveEffect.Visible = false;
            }
        }

        private void Click(GameObject obj)
        {
            EngineCoreEvents.AudioEvents.PlayAudio.SafeInvoke(Audio.AudioType.UISound, GameCustomAudioKey.achievement_detail.ToString());
            FrameMgr.OpenUIParams param = new FrameMgr.OpenUIParams(UIDefine.UI_ACHIEVEMENT_POP);
            param.Param = m_data;
            EngineCoreEvents.UIEvent.ShowUIEventWithParam.SafeInvoke(param);
        }

        private void OnReceiveData(long id, int factor)
        {
            if (id == m_data.Id)
            {
                m_data.SubmitStatus |= factor;
                m_Tips_img.Visible = (m_data.SubmitStatus != m_Submit);
                LoadTipEffect();
            }

        }
    }
}
