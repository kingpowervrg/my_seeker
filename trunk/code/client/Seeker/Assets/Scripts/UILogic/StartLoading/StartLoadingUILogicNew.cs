using System;
using System.Collections.Generic;
using EngineCore;
using UnityEngine;
using UnityEngine.UI;
namespace SeekerGame
{
    
    class StartLoadingUILogicNew : MonoBehaviour
    {
        private Slider m_Slider;
        private RectTransform sliderRect = null;
        private float width = 600f;
        void Awake()
        {
            Transform tran = transform.Find("Slider");
            this.m_Slider = tran.GetComponent<Slider>();
            this.sliderRect = tran.GetComponent<RectTransform>();
        }

        public void Start()
        {
            float[] time = new float[] { 20f };
            float[] progress = new float[] { 1 };
            bool[] state = new bool[] { true };
            //LoadingData loadData = new LoadingData(time, progress, state);
            //m_LoadingSystem = new LoadingSystem(loadData);
            this.m_Slider.value = 0f;
            this.width = sliderRect.sizeDelta.x;
            GameEvents.UIEvents.UI_Loading_Event.OnStartLoadingComplete += OnStartLoadingComplete;
        }

        private void OnStartLoadingComplete()
        {
            this.m_isLoadingComplete = true;
            GameEvents.UIEvents.UI_Loading_Event.OnStartLoadingComplete -= OnStartLoadingComplete;
            this.m_totalTime = 1f;
        }

        public void OnDestory()
        {
            //GameEvents.UIEvents.UI_Loading_Event.OnLoadingOver.SafeInvoke();
        }

        private float m_timesection = 0f;
        private float m_totalTime = 20f;
        private bool m_isLoadingComplete = false;
        private float m_speed
        {
            get {
                return 1f / m_totalTime;
            }
        }
        public void Update()
        {
            this.m_timesection += Time.deltaTime * this.m_speed;
            this.m_Slider.value = Mathf.Clamp01(this.m_timesection);
            if (this.m_timesection >= 1f && this.m_isLoadingComplete)
            {
                GameObject.DestroyImmediate(gameObject);
            }
        }
    }
}
