/********************************************************************
	created:  2018-10-9 11:21:41
	filename: PregameUILogic.cs
	author:	  songguangze@outlook.com
	
	purpose:  客户端日志上报
*********************************************************************/
using DG.Tweening;
using EngineCore;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace SeekerGame
{
    public class PregameUILogic : MonoSingleton<PregameUILogic>
    {
        private Slider m_initSlider = null;
        private Text m_sliderProgress = null;

        public readonly float[] SLIDER_SEGMENT = { 0, 0.1f, 0.5f, 1f };
        public readonly float[] SEGMENT_TWEEN_TIME = { 0, 1f, 20f, 10f };

        private int m_currentSegment = 0;
        private Tweener m_sliderTweener;
        public Canvas canvas;

        public void TweenToSegment(int segment)
        {
            if (segment < SLIDER_SEGMENT.Length)
            {
                SetToValue(SLIDER_SEGMENT[segment], SEGMENT_TWEEN_TIME[segment]);
                this.m_currentSegment = segment;
            }
        }

        public void SetToSegmentStart(int segment, float deltaTime = 0.1f)
        {
            SetToValue(SLIDER_SEGMENT[segment], deltaTime);
            this.m_currentSegment = segment;
        }

        public void StartInitLoading()
        {
            TweenToSegment(1);
        }


        public void SetToValue(float value, float deltaTime = 0.1f, Action OnCompletedCallback = null)
        {
            if (!m_initSlider)
            {
                m_initSlider = transform.Find("Slider").GetComponent<Slider>();
                m_sliderProgress = m_initSlider.transform.Find("Text").GetComponent<Text>();
            }

            if (this.m_sliderTweener != null)
                this.m_sliderTweener.Kill();

            this.m_sliderTweener = m_initSlider.DOValue(value, deltaTime).OnComplete(() => OnCompletedCallback?.Invoke());
        }

        public void SetDeltaValue(float deltaValue)
        {
            float segmentDeltaValue = SLIDER_SEGMENT[m_currentSegment + 1] - SLIDER_SEGMENT[m_currentSegment];
            float realDelta = SLIDER_SEGMENT[m_currentSegment] + deltaValue * segmentDeltaValue;

            SetToValue(realDelta);
        }

        private float CurrentValue
        {
            get { return this.m_initSlider != null ? this.m_initSlider.value : 0; }
        }


        void Update()
        {
            if (m_sliderProgress != null)
            {
                m_sliderProgress.text = $"{(int)(CurrentValue * 100)} %";
            }
        }

        public void Destory()
        {
            //TimeModule.Instance.SetTimeout(()=> GameObject.Destroy(gameObject),0.5f);
            gameObject.SetActive(false);

        }

    }
}