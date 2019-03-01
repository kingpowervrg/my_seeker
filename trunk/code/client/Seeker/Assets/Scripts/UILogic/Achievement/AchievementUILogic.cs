using EngineCore;
using System.Collections.Generic;

namespace SeekerGame
{
    [UILogicHandler(UIDefine.UI_ACHIEVEMENT)]
    public class AchievementUILogic : BasePageTweenUILogic
    {
        private GameUIComponent m_panel_down;
        private GameLoopUIContainer<AchievementItem> m_Grid_con;
        private List<AchievementMsg> m_achieveMent_data = new List<AchievementMsg>();
        private GameLabel m_TotalLab = null;
        private bool m_isSend = false;

        protected override void InitPageBtnStr()
        {
            base.InitPageBtnStr();
            m_pageStr = "leftBtn:";
            m_pageBtnName = new string[] { "btnTotal", "btnNotGet" };
            m_toggleName = new string[] { LocalizeModule.Instance.GetString("Achievement_total"), LocalizeModule.Instance.GetString("Achievement_notget") };
        }

        protected override void InitController()
        {
            base.InitController();


            this.m_Grid_con = Make<GameLoopUIContainer<AchievementItem>>("Panel_animation:Panel:Viewport:grid");
            this.m_TotalLab = Make<GameLabel>("totalAchievement:Text");

        }

        protected override void InitListener()
        {
            base.InitListener();
            MessageHandler.RegisterMessageHandler(MessageDefine.SCAchievementResponse, OnResponse);
        }

        public override void OnShow(object param)
        {
            base.OnShow(param);
            SetCloseBtnID("Button_close");
            UserBehaviorStatisticsModules.Instance.LogEvent(UBSEventKeyName.achievement_in, 1.0f, null);
            GameEvents.UIEvents.UI_Achievement_Event.OnReflashAchievement += OnReflashAchievement;
            RefreshDataAndPanel(AchievementManager.Instance.Data);

            EngineCoreEvents.UIEvent.HideUIEvent.SafeInvoke(UIDefine.UI_ENTER_JIGSAW);
            EngineCoreEvents.UIEvent.HideUIEvent.SafeInvoke(UIDefine.UI_EVENT_INGAME_ENTRY);
            //EngineCoreEvents.UIEvent.HideUIEvent.SafeInvoke(UIDefine.UI_ENGER_GAME_UI);
        }

        private void OnReflashAchievement()
        {
            if (m_isSend)
            {
                return;
            }
            this.m_isSend = true;

            GameEvents.PlayerEvents.RequestRecentAhievement.SafeInvoke();

        }




        public override void OnHide()
        {
            base.OnHide();
            this.m_isSend = false;
            m_achieveMent_data.Clear();
            GameEvents.UIEvents.UI_Achievement_Event.OnReflashAchievement -= OnReflashAchievement;
            EngineCoreEvents.UIEvent.HideUIEvent.SafeInvoke(UIDefine.UI_ACHIEVEMENT_POP);
        }

        protected override void RemoveListener()
        {
            base.RemoveListener();
            MessageHandler.UnRegisterMessageHandler(MessageDefine.SCAchievementResponse, OnResponse);
        }

        protected override void OnPageChangeClick(int i)
        {
            base.OnPageChangeClick(i);
            List<AchievementMsg> msgs = OnFilter();
            OnReflashPanel(msgs);
        }

        protected override void OnPageChangeCanel(int i)
        {
            base.OnPageChangeCanel(i);
        }

        private void OnResponse(object obj)
        {
            if (obj is SCAchievementResponse)
            {
                this.m_isSend = false;


                m_achieveMent_data.Clear();
                SCAchievementResponse res = obj as SCAchievementResponse;
                RefreshDataAndPanel(res);
            }
        }

        void RefreshDataAndPanel(SCAchievementResponse res)
        {
            if (null == res)
            {
                return;
            }

            m_achieveMent_data.Clear();

            this.m_TotalLab.Text = res.TotalCount.ToString();
            for (int i = 0; i < res.Achievements.Count; i++)
            {
                m_achieveMent_data.Add(res.Achievements[i]);
            }
            List<AchievementMsg> msgs = OnFilter();
            OnReflashPanel(msgs);
        }

        private void OnReflashPanel(List<AchievementMsg> msgs)
        {
            int count = msgs.Count;
            if (count == 0)
                return;

            count--;

            m_Grid_con.Clear();
            m_Grid_con.EnsureSize(count);
            for (int i = 0; i < count; i++)
            {
                AchievementItem item = m_Grid_con.GetChild(i);
                item.Visible = true;
                if (m_CurrntIndex == 0)
                {
                    item.SetData(msgs[i], true);
                }
                else if (1 == m_CurrntIndex)
                {
                    item.SetData(msgs[i], false);
                }
            }
        }



        private List<AchievementMsg> OnFilter()
        {
            if (m_CurrntIndex == 0)
            {
                m_achieveMent_data.Sort(AchievementTools.s_Comparer);
                return m_achieveMent_data;
            }
            else if (m_CurrntIndex == 1)
            {
                List<AchievementMsg> msgs = new List<AchievementMsg>();
                for (int i = 0; i < m_achieveMent_data.Count; i++)
                {
                    AchievementMsg msg = m_achieveMent_data[i];
                    ConfAchievement confAchieve = ConfAchievement.Get(msg.Id);
                    if (confAchieve != null)
                    {
                        if (msg.Progress < confAchieve.progress1)
                        {
                            msgs.Add(msg);
                        }
                    }

                }
                return msgs;
            }
            else
            {
                return new List<AchievementMsg>();
            }

        }

        public override FrameDisplayMode UIFrameDisplayMode => FrameDisplayMode.FULLSCREEN;
    }
}
