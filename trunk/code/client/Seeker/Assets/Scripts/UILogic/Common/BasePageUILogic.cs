using EngineCore;

namespace SeekerGame
{
    public class BasePageUILogic : UILogicBase
    {
        protected string m_pageStr = "leftBtn:";
        protected string[] m_pageBtnName = { "btnTotal", "btnNotGet" };
        protected string[] m_toggleName = { "one", "two" };
        protected ToggleWithArrowTween[] m_toggles;
        protected int m_pageCount = 0;

        protected int m_CurrntIndex = -1;

        protected virtual void InitPageBtnStr()
        {
            m_pageBtnName = new string[2] { "btnTotal", "btnNotGet" };
            m_toggleName = new string[2] { "one", "two" };
        }

        protected virtual void InitController()
        {
            m_pageCount = m_pageBtnName.Length;
            m_toggles = new ToggleWithArrowTween[m_pageCount];
            for (int i = 0; i < m_pageCount; i++)
            {
                m_toggles[i] = Make<ToggleWithArrowTween>(m_pageStr + m_pageBtnName[i]);
                m_toggles[i].Refresh(i, m_toggleName[i], 0 == i, OnPageChange);
            }
        }

        protected virtual void InitListener()
        {

        }



        protected virtual void RemoveListener()
        {
            //for (int i = 0; i < m_pageCount; i++)
            //{
            //    m_pageBtn[i].RemoveClickCallBack(OnPageChange);
            //}
        }

        protected override void OnInit()
        {
            base.OnInit();
            InitPageBtnStr();
            InitController();
        }

        public override void OnShow(object param)
        {
            base.OnShow(param);
            InitListener();

            this.m_toggles[0].Checked = false;
            this.m_toggles[0].Checked = true;
        }

        public override void OnHide()
        {
            base.OnHide();
            RemoveListener();
            m_CurrntIndex = -1;
        }

        private void OnPageChange(int i, bool flag)
        {
            if (i != m_CurrntIndex && flag)
            {
                if (-1 != m_CurrntIndex)
                    EngineCoreEvents.AudioEvents.PlayAudio.SafeInvoke(Audio.AudioType.UISound, EngineCommonAudioKey.table_change.ToString());

                m_CurrntIndex = i;
                OnPageChangeClick(i);
            }
            else if (!flag)
            {
                OnPageChangeCanel(i);
            }
        }

        private void OnPageChange(bool flag, int i)
        {
            if (i != m_CurrntIndex && flag)
            {
                if (-1 != m_CurrntIndex)
                    EngineCoreEvents.AudioEvents.PlayAudio.SafeInvoke(Audio.AudioType.UISound, EngineCommonAudioKey.table_change.ToString());

                m_CurrntIndex = i;
                OnPageChangeClick(i);
            }
            else if (!flag)
            {
                OnPageChangeCanel(i);
            }
        }

        protected virtual void OnPageChangeClick(int i)
        {
        }

        protected virtual void OnPageChangeCanel(int i)
        {
        }

        public override FrameDisplayMode UIFrameDisplayMode
        {
            get
            {
                return FrameDisplayMode.WINDOWED;
            }
        }
    }
}

