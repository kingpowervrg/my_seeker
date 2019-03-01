using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EngineCore;
using GOEngine;
using GOGUI;

namespace SeekerGame
{
    [UILogicHandler(UIDefine.UI_TALKREWAR)]
    public class TalkRewardUILogic : UILogicBase
    {
        private GameImage[] m_reward_img;
        private GameLabel[] m_reward_lab;
        private GameImage[] m_root_img;

        private GameButton m_sure_btn;

        private TalkRewardData m_reward_data;
        protected override void OnInit()
        {
            base.OnInit();
            InitController();
            m_sure_btn.AddClickCallBack(btnSure);
        }

        private void InitController()
        {
            m_root_img = new GameImage[4];
            m_reward_img = new GameImage[4];
            m_reward_lab = new GameLabel[4];
            for (int i = 0; i < 4; i++)
            {
                m_root_img[i] = Make<GameImage>(string.Format("Panel/Image0{0}",i));
                m_reward_img[i] = m_root_img[i].Make<GameImage>("Image");
                m_reward_lab[i] = m_root_img[i].Make<GameLabel>("Text");
            }
            m_sure_btn = Make<GameButton>("Button");
        }

        public override void OnShow(object param)
        {
            base.OnShow(param);
            
            if (param != null)
            {
                m_reward_data = (TalkRewardData)param;
            }
            InitUILogic();
        }
        public override FrameDisplayMode UIFrameDisplayMode
        {
            get
            {
                return FrameDisplayMode.WINDOWED;
            }
        }
        private void InitUILogic()
        {
            if (m_reward_data == null || m_reward_data.ID == null)
            {
                Debug.LogError("no reward");
                return;
            }
            if (m_reward_data.ID.Length > 4)
            {
                Debug.LogError("more reward");
            }
            int rewardCount = m_reward_data.ID.Length;
            if (rewardCount == 1)
            {
                float centerY = (m_root_img[0].Y + m_root_img[2].Y) / 2f;
                layoutItem(new GameImage[] { m_root_img[0]}, m_root_img[0].X, centerY, m_root_img[1].X, centerY);
            }
            else if (rewardCount == 2)
            {
                float centerY = (m_root_img[0].Y + m_root_img[2].Y) / 2f;
                layoutItem(new GameImage[] { m_root_img[0],m_root_img[1] }, m_root_img[0].X,centerY, m_root_img[1].X,centerY);
            }
            else if(rewardCount == 3)
            {
                layoutItem(new GameImage[] { m_root_img[2]}, m_root_img[2].X, m_root_img[2].Y, m_root_img[3].X, m_root_img[3].Y);
            }
            for (int i = 0; i < 4; i++)
            {
                if (i < rewardCount)
                {
                    Confexhibit conf = Confexhibit.Get(m_reward_data.ID[i]);
                    if (conf != null)
                    {
                        m_reward_img[i].Sprite = conf.iconName;
                        m_reward_lab[i].Text = LocalizeModule.Instance.GetString(conf.name);
                        m_root_img[i].SetActive(true);
                    }
                    else
                    {
                        m_root_img[i].SetActive(false);
                    }
                }
                else
                {
                    m_root_img[i].SetActive(false);
                }
            }
        }

        public override void OnHide()
        {
            base.OnHide();
        }

        public override void Dispose()
        {
            base.Dispose();
            m_sure_btn.RemoveClickCallBack(btnSure);
        }

        private void btnSure(GameObject obj)
        {
            for (int i = 0; i < m_reward_data.ID.Length; i++)
            {
                GlobalInfo.MY_PLAYER_INFO.AddSingleBagInfo(m_reward_data.ID[i],1);
            }

            GameEvents.UIEvents.UI_GameEntry_Event.Listen_OnCombinePropCollected.SafeInvoke();

            EngineCoreEvents.UIEvent.HideUIEvent.SafeInvoke(UIDefine.UI_TALKREWAR);
            GameEvents.UIEvents.UI_Talk_Event.OnTalkRewardFinish.SafeInvoke();
        }

        private void layoutItem(GameImage[] img,float sX,float sY,float eX,float eY)
        {
            if (img.Length == 0)
            {
                return;
            }
            if (img.Length == 1)
            {
                img[0].X = (sX + eX) / 2f;
                img[0].Y = (sY + eY) / 2f;
            }
            else if (img.Length == 2)
            {
                img[0].X = sX;
                img[0].Y = sY;
                img[1].X = eX;
                img[1].Y = eY;
            }
        }
    }
}


