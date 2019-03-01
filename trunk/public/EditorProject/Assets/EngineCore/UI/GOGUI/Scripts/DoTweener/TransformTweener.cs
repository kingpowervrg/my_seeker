using UnityEngine;

namespace EngineCore
{
    [RequireComponent(typeof(RectTransform))]
    [ExecuteInEditMode]
    public abstract class TransformTweener : UITweenerBase
    {
        private RectTransform m_cachedRectTransform = null;

        public bool IsWorldSpace = false;

        protected override void Awake()
        {
            m_cachedRectTransform = GetComponent<RectTransform>();
        }


        /// <summary>
        /// 设置开始(From)值
        /// </summary>
        public abstract void SetStartToCurrentValue();

        /// <summary>
        /// 设置结束(to)值
        /// </summary>
        public abstract void SetEndToCurrentValue();

        /// <summary>
        /// 设置开始(From)值
        /// </summary>
        public abstract void SetCurrentValueToStart();

        /// <summary>
        /// 设置结束(to)值
        /// </summary>
        public abstract void SetCurrentValueToEnd();

        public RectTransform CachedTransfrom
        {
            get
            {
                if (!m_cachedRectTransform)
                    Awake();

                return this.m_cachedRectTransform;
            }

        }
    }
}