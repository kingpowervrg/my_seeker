using EngineCore;
using UnityEngine;
namespace SeekerGame
{
    public class StartCartoonCapter : GameUIComponent
    {
        private StartCartoonPage[] m_pages;
        //private GameUIContainer m_Con;
        private int m_CurrentIndex = 0;
        protected override void OnInit()
        {
            base.OnInit();
            int count = Widget.childCount;
            m_pages = new StartCartoonPage[count];
            for (int i = 1; i <= count; i++)
            {
                m_pages[i - 1] = Make<StartCartoonPage>("Panel_" + i);
                m_pages[i - 1].Visible = false;
            }

        }

        public override void OnShow(object param)
        {
            base.OnShow(param);
            GameEvents.UIEvents.UI_StartCartoon_Event.OnNext += OnNext;
            GameEvents.UIEvents.UI_StartCartoon_Event.OnResetPage += OnResetPage;
            m_CurrentIndex = 0;
            for (int i = 0; i < m_pages.Length; i++)
            {
                m_pages[i].Visible = false;
            }

            ShowPage();
        }

        private void OnNext()
        {
            ShowPage();
        }

        private void ShowPage()
        {
            if (m_CurrentIndex >= m_pages.Length)
            {
                return;
            }
            if (m_CurrentIndex > 0)
            {
                m_pages[m_CurrentIndex - 1].Visible = false;
            }
            m_pages[m_CurrentIndex].SetIsEnd(m_CurrentIndex >= m_pages.Length - 1);

            m_pages[m_CurrentIndex].Visible = true;
            m_CurrentIndex++;

        }

        public override void OnHide()
        {
            base.OnHide();
            GameEvents.UIEvents.UI_StartCartoon_Event.OnNext -= OnNext;
            GameEvents.UIEvents.UI_StartCartoon_Event.OnResetPage -= OnResetPage;
            m_CurrentIndex = 0;
        }

        private void OnResetPage()
        {
            m_pages[m_pages.Length - 1].Visible = false;
            m_CurrentIndex = 0;
            ShowPage();
        }

        public class StartCartoonPage : GameUIComponent
        {
            private GameButton m_NextBtn;
            private bool m_IsEnd = false;
            private float m_maxTime;
            private float m_lockTime = 0f; //当前时间
            private StartCartoonDelayTime[] m_childPanels = null;
            private int m_panelIndex = 1;
            protected override void OnInit()
            {
                base.OnInit();
                m_NextBtn = Make<GameButton>("Button_next");
            }

            public override void OnShow(object param)
            {
                base.OnShow(param);
                m_NextBtn.Visible = false;
                //Debug.Log("OnShow ====== " + Widget.name);
                m_NextBtn.AddClickCallBack(OnNextBtn);
                GameEvents.UIEvents.UI_StartCartoon_Event.OnNextClick += OnNextClick;
#if UNITY_EDITOR
                GameEvents.UIEvents.UI_StartCartoon_Event.OnSkipCurrentCapter += OnSkipCurrentCapter;
#endif
                this.m_childPanels = gameObject.GetComponentsInChildren<StartCartoonDelayTime>();
                this.m_maxTime = GetComponent<StartCartoonDelayTime>().m_delayTime;
                this.m_lockTime = Time.time;
            }

            private void OnNextClick()
            {
                float afterTime = Time.time - this.m_lockTime;
                //Debug.Log("m_panelIndex : " + m_panelIndex + "  m_childPanels :" + m_childPanels.Length + "  " + this.m_lockTime + "  maxtime : " + this.m_maxTime);
                if (afterTime >= this.m_maxTime || m_panelIndex >= this.m_childPanels.Length)
                {
                    OnNextBtn(null);
                }
                else
                {
                    if (m_panelIndex + 1 >= this.m_childPanels.Length)
                    {
                        if (this.m_childPanels[0].m_delayTime < afterTime) //播放完成直接跳过
                        {
                            OnNextBtn(null);
                        }
                        else
                        {
                            StopCurrentPanel();
                            m_panelIndex++;
                            GotoNextPanel();
                        }
                    }
                    else
                    {
                        if (this.m_childPanels[m_panelIndex + 1].m_delayTime < afterTime)
                        {
                            m_panelIndex++;
                            GotoNextPanel();
                        }
                        else
                        {
                            StopCurrentPanel();
                            m_panelIndex++;
                            GotoNextPanel();
                        }

                    }

                }
            }

            private void StopCurrentPanel()
            {
                UITweenerBase[] tweeners = this.m_childPanels[m_panelIndex].GetComponentsInChildren<UITweenerBase>();
                for (int i = 0; i < tweeners.Length; i++)
                {
                    if (tweeners[i].name.Contains("skip"))
                    {
                        continue;
                    }
                    tweeners[i].SetTweenCompleted();
                }
            }

            private void GotoNextPanel()
            {
                if (m_panelIndex >= this.m_childPanels.Length)
                {
                    return;
                }
                UITweenerBase[] tweeners = this.m_childPanels[m_panelIndex].GetComponentsInChildren<UITweenerBase>();
                for (int i = 0; i < tweeners.Length; i++)
                {
                    float delay = tweeners[i].Delay;
                    delay -= this.m_childPanels[this.m_panelIndex].m_delayTime;
                    tweeners[i].Delay = delay > 0 ? delay : 0;
                }
            }

            public void SetIsEnd(bool isEnd)
            {
                m_IsEnd = isEnd;
            }

            private void OnNextBtn(GameObject obj)
            {
                if (m_IsEnd)
                {
                    //Visible = false;
                    EngineCoreEvents.AudioEvents.PlayAudio.SafeInvoke(Audio.AudioType.UISound, EngineCommonAudioKey.Close_Window.ToString());

                    //CSCartoonRewardRequest rewardReq = new CSCartoonRewardRequest();
                    //GameEvents.NetWorkEvents.SendMsg.SafeInvoke(rewardReq);

                    // EngineCoreEvents.UIEvent.HideUIEvent.SafeInvoke(UIDefine.UI_COMICS_1);

                    // GlobalInfo.MY_PLAYER_INFO.PlayerTaskSystem.SyncTaskDetailInfo(1);

                    // GameEvents.UIEvents.UI_StartCartoon_Event.OnFinish.SafeInvoke();
                }
                else
                {
                    EngineCoreEvents.AudioEvents.PlayAudio.SafeInvoke(Audio.AudioType.UISound, EngineCommonAudioKey.Button_Click_Common.ToString());

                    GameEvents.UIEvents.UI_StartCartoon_Event.OnNext.SafeInvoke();
                }
            }


            public override void OnHide()
            {
                base.OnHide();
                m_IsEnd = false;
                //m_panelIndex = 1;
                m_NextBtn.RemoveClickCallBack(OnNextBtn);
                GameEvents.UIEvents.UI_StartCartoon_Event.OnNextClick -= OnNextClick;
#if UNITY_EDITOR
                GameEvents.UIEvents.UI_StartCartoon_Event.OnSkipCurrentCapter -= OnSkipCurrentCapter;
#endif
            }

            private void OnSkipCurrentCapter()
            {
                EngineCoreEvents.AudioEvents.PlayAudio.SafeInvoke(Audio.AudioType.UISound, EngineCommonAudioKey.Close_Window.ToString());

                // CSCartoonRewardRequest rewardReq = new CSCartoonRewardRequest();
                // GameEvents.NetWorkEvents.SendMsg.SafeInvoke(rewardReq);

                //  EngineCoreEvents.UIEvent.HideUIEvent.SafeInvoke(UIDefine.UI_COMICS_1);

                GameEvents.UIEvents.UI_StartCartoon_Event.OnFinish.SafeInvoke();
            }
        }
    }
}
