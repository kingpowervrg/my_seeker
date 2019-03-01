using UnityEngine;
using EngineCore;

namespace SeekerGame
{
    public class LoadingSystem
    {
        private float[] m_Time = new float[] { 2f, 3f, 1f };
        private float[] m_Progress = new float[] { 0.4f, 0.9f, 1f };
        private bool[] m_state;
        private int m_step = 0;
        private string m_ui_name;

        public LoadingSystem(LoadingData loadingData, string ui_name_ = null)
        {
            m_Time = loadingData.m_time;
            m_Progress = loadingData.m_progress;
            m_state = loadingData.m_state;
            m_step = 0;
            timesection = 0f;
            m_ui_name = ui_name_;
            GameEvents.UIEvents.UI_Loading_Event.OnLoadingState += OnLoadingOverState;
            GameEvents.UIEvents.UI_Loading_Event.OnLoadingStageTime += OnLoadingStageTime;
            GameEvents.UIEvents.UI_Loading_Event.OnChangeStageTime += OnChangeStageTime;
        }

        private float m_CurrentSpeed
        {
            get
            {
                if (m_step > 0)
                {
                    return (m_Progress[m_step] - m_Progress[m_step - 1]) / m_Time[m_step];
                }
                return m_Progress[m_step] / m_Time[m_step];
            }
        }

        private float timesection = 0f;
        public float TimeSection
        {
            get
            {
                return timesection;
            }
        }

        public void UpdateLoading()
        {
            if (m_step >= m_Time.Length || !m_state[m_step])
            {
                return;
            }
            timesection += Time.deltaTime * m_CurrentSpeed;
            if (timesection >= m_Progress[m_step])
            {
                if (!NextStep())
                {
                    return;
                }
            }
        }

        private bool NextStep()
        {
            m_step++;
            if (m_step >= m_Time.Length)
            {
                if (string.IsNullOrEmpty(m_ui_name))
                    EngineCoreEvents.UIEvent.HideUIEvent.SafeInvoke(UIDefine.UI_Loading);
                else
                    EngineCoreEvents.UIEvent.HideUIEvent.SafeInvoke(m_ui_name);
                return false;
            }
            return true;
        }

        public void OnHide()
        {
            GameEvents.UIEvents.UI_Loading_Event.OnLoadingState -= OnLoadingOverState;
            GameEvents.UIEvents.UI_Loading_Event.OnLoadingStageTime -= OnLoadingStageTime;
            GameEvents.UIEvents.UI_Loading_Event.OnChangeStageTime -= OnChangeStageTime;
        }

        private void OnLoadingOverState(int stage, bool state)
        {
            m_state[stage] = state;
        }

        private void OnLoadingStageTime(int stage, float time)
        {
            m_Time[stage] = time;
        }

        private void OnChangeStageTime(int stage, float progress, float time)
        {
            float totalTime = (1f / progress) * time;
            OnLoadingStageTime(stage, totalTime);
        }


    }
}
