using UnityEngine.UI;

namespace EngineCore
{
    public abstract class MaskableGraphicTweener : UITweenerBase
    {
        private MaskableGraphic m_cachedMaskableGraphic;

        protected override void Awake()
        {
            m_cachedMaskableGraphic = GetComponent<MaskableGraphic>();
        }

        public MaskableGraphic CachedMaskableGraphic => this.m_cachedMaskableGraphic;
    }
}