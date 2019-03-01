/********************************************************************
	created:  2018-12-28 14:55:11
	filename: UITweenerBase.cs
	author:	  songguangze@outlook.com
	
	purpose:  基于DoTween的UI动画组件基类    
*********************************************************************/
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;

namespace EngineCore
{
    [ExecuteInEditMode]
    public abstract class UITweenerBase : UIBehaviour, IUIEventSystemInterface
    {
        /// <summary>
        /// 动画曲线
        /// </summary>
        public AnimationCurve animationCurve = new AnimationCurve(new Keyframe(0f, 0f, 0f, 1f), new Keyframe(1f, 1f, 1f, 0f));

        //动画播放类型
        public TweenStyle m_tweenStyle = TweenStyle.Once;

        //动画触发形式
        public TweenTriggerType m_triggerType = TweenTriggerType.Manual;

        /// <summary>
        /// 延迟
        /// </summary>
        public float Delay = 0f;

        /// <summary>
        /// 时间
        /// </summary>
        public float Duration = 0f;

        /// <summary>
        /// 循环次数
        /// </summary>
        public int LoopTimes = 1;

        protected TweenCallback OnTweenStartCallback;
        protected TweenCallback OnTweenUpdateCallback;
        protected TweenCallback OnTweenCompletedCallback;

        protected Tweener m_uiTweener = null;

        private bool m_isPlayForward = true;
        private Ease m_buildinEase = Ease.Unset;

        new protected virtual void Awake()
        {

        }

        new protected virtual void OnEnable()
        {

        }

        new protected virtual void OnDisable()
        {
            Stop();
        }

        public void PlayForward()
        {
            Play(true);
        }

        public void PlayBackward()
        {
            Play(false);
        }

        public void ResetAndPlay(bool isForward = true)
        {
            Stop();

            Play(isForward);
        }


        public virtual void Play(bool playForward)
        {
            this.m_isPlayForward = playForward;
        }

        protected void PlayInternal()
        {
            if (this.m_uiTweener != null)
            {
                this.m_uiTweener.SetAutoKill(false);

                this.m_uiTweener.SetDelay(this.Delay);

                if (!m_isPlayForward)
                {
                    if (!this.m_uiTweener.isBackwards)
                        this.m_uiTweener.Complete(false);

                    this.m_uiTweener.PlayBackwards();
                }

                SetTweenLoopType();
                //set ease
                if (m_buildinEase != Ease.Unset)
                    this.m_uiTweener.SetEase(m_buildinEase);
                else
                    this.m_uiTweener.SetEase(this.animationCurve);

                this.m_uiTweener.OnStart(OnTweenStart);
                this.m_uiTweener.OnUpdate(OnTweenUpdate);
                this.m_uiTweener.OnComplete(OnTweenCompleted);
            }
        }

        private void SetTweenLoopType()
        {
            switch (m_tweenStyle)
            {
                case TweenStyle.Loop:
                    this.m_uiTweener.SetLoops(LoopTimes == 1 ? -1 : LoopTimes);
                    break;
                case TweenStyle.Once:
                    this.m_uiTweener.SetLoops(1);
                    break;
                case TweenStyle.PingPong:
                    this.m_uiTweener.SetLoops(LoopTimes, LoopType.Yoyo);
                    break;
            }
        }


        public void AddTweenCompletedCallback(TweenCallback onTweenCompletedCallback)
        {
            OnTweenCompletedCallback += onTweenCompletedCallback;
        }

        public void RemoveTweenCompletedCallback(TweenCallback onTweenCompletedCallback)
        {
            OnTweenCompletedCallback -= onTweenCompletedCallback;
        }

        public void SetTweenCompletedCallback(TweenCallback onTweenCompletedCallback)
        {
            OnTweenCompletedCallback = onTweenCompletedCallback;
        }

        public void AddTweenStartCallback(TweenCallback onTweenStartCallback)
        {
            OnTweenStartCallback += OnTweenStartCallback;
        }

        public void SetTweenStartCallback(TweenCallback onTweenStartCallback)
        {
            OnTweenStartCallback = onTweenStartCallback;
        }

        public void AddTweenUpdateCallback(TweenCallback onTweenUpdateCallback)
        {
            OnTweenUpdateCallback += onTweenUpdateCallback;
        }

        public void SetTweenUpdateCallback(TweenCallback onTweenUpdateCallback)
        {
            OnTweenUpdateCallback = onTweenUpdateCallback;
        }


        protected virtual void OnTweenStart()
        {
            if (m_isPlayForward)
                this.m_uiTweener.ChangeStartValue(TweenerStartValue);

            OnTweenStartCallback?.Invoke();
        }

        protected virtual void OnTweenUpdate()
        {
            OnTweenUpdateCallback?.Invoke();
        }

        protected virtual void OnTweenCompleted()
        {
            OnTweenCompletedCallback?.Invoke();
        }

        /// <summary>
        /// 动画直接结束
        /// </summary>
        public void SetTweenCompleted()
        {
            m_uiTweener?.Complete(true);
        }


        public void Stop()
        {
            m_uiTweener?.Kill();
        }

        protected override void OnDestroy()
        {
            OnTweenStartCallback = null;
            OnTweenUpdateCallback = null;
            OnTweenCompletedCallback = null;
        }

        public void SetBuildinEase(Ease buildinEaseType)
        {
            this.m_buildinEase = buildinEaseType;
        }

        /// <summary>
        /// 点击
        /// </summary>
        /// <param name="eventData"></param>
        public void OnPointerClick(PointerEventData eventData)
        {
            if (m_triggerType == TweenTriggerType.OnClick && eventData.clickCount == 1)
                PlayForward();

            if (m_triggerType == TweenTriggerType.OnDoubleClick && eventData.clickCount == 2)
                PlayForward();
        }

        /// <summary>
        /// 手指进入
        /// </summary>
        /// <param name="eventData"></param>
        public void OnPointerEnter(PointerEventData eventData)
        {
            if (m_triggerType == TweenTriggerType.OnHoverTrue)
                PlayForward();
        }

        /// <summary>
        /// 手指退出
        /// </summary>
        /// <param name="eventData"></param>
        public void OnPointerExit(PointerEventData eventData)
        {
            if (m_triggerType == TweenTriggerType.OnHoverFalse)
                PlayForward();
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (m_triggerType == TweenTriggerType.OnPressTrue)
                PlayForward();
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (m_triggerType == TweenTriggerType.OnPressFalse)
                PlayForward();
        }

        public void OnSelect(BaseEventData eventData)
        {
            if (m_triggerType == TweenTriggerType.OnSelectTrue)
                PlayForward();
        }

        public void OnDeselect(BaseEventData eventData)
        {
            if (m_triggerType == TweenTriggerType.OnSelectFalse)
                PlayForward();
        }

        /// <summary>
        /// Tweener From Value Getter
        /// </summary>
        protected abstract System.Object TweenerStartValue { get; }

        /// <summary>
        /// Tween 总时间
        /// </summary>
        public float TweenTotalTime => Duration + Delay;

        /// <summary>
        /// 动画触发类型
        /// </summary>
        public enum TweenTriggerType
        {
            Manual,
            OnShow,             //UI显示时
            OnHide,             //UI隐藏时
            OnPressTrue,        //手指按下
            OnPressFalse,       //手指抬起
            OnClick,            //点击
            OnHoverTrue,        //手指进入
            OnHoverFalse,       //手指退出
            OnDoubleClick,      //双击
            OnSelect,
            OnSelectTrue,
            OnSelectFalse,
        }


        /// <summary>
        /// 动画播放模式
        /// </summary>
        public enum TweenStyle
        {
            Once,
            Loop,
            PingPong,
        }
    }
}