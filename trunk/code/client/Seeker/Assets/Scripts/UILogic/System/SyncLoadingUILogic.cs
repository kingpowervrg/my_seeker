using EngineCore;
using System.Collections.Generic;
using UnityEngine;

namespace SeekerGame
{
    [UILogicHandler(UIDefine.UI_SYNC_LOADING)]
    public class SyncLoadingUILogic : UILogicBase
    {
        private GameLabel m_lbLoadingTips = null;
        private GameImage m_bg = null;
        private long m_time;
        bool m_is_update;

        private GameObject m_syncEffect = null;

        protected override void OnInit()
        {
            base.OnInit();
            AutoDestroy = false;

            //NeedLateUpdateByFrame = true;
            this.m_lbLoadingTips = Make<GameLabel>("Text");
            this.m_bg = Make<GameImage>("background");
            this.m_syncEffect = Transform.Find("waiting/Effect/UI_juhua").gameObject;
            Renderer[] renderers = this.m_syncEffect.GetComponentsInChildren<Renderer>();
            for (int i = 0; i < renderers.Length; ++i)
                renderers[i].sortingLayerName = "Front";
        }

        public override void OnShow(object param)
        {
            base.OnShow(param);

            GameEvents.System_Events.SetLoadingTips += OnSetLoadingTips;

            HideEffect();
            TimeModule.Instance.RemoveTimeactionInObject(this.root, ShowEffect);
            TimeModule.Instance.SetTimeoutLiveInObject(this.root, ShowEffect, 1.0f);
            m_time = System.DateTime.Now.Ticks;
            m_is_update = true;
            //m_seconds = 0;
            if (null != param)
            {
                m_is_update = (bool)param;
            }

        }

        private void HideEffect()
        {
            m_bg.Color = new Color(m_bg.Color.r, m_bg.Color.g, m_bg.Color.b, 0.01f);
            this.m_syncEffect.SetActive(false);
        }

        private void ShowEffect()
        {
            m_bg.Color = new Color(m_bg.Color.r, m_bg.Color.g, m_bg.Color.b, 0.7f);
            if (this.m_syncEffect == null)
            {
                TimeModule.Instance.RemoveTimeactionInObject(this.root, ShowEffect);
                return;
            }
            this.m_syncEffect.SetActive(true);
        }

        private void OnSetLoadingTips(string loadingTips)
        {
            this.m_lbLoadingTips.Text = loadingTips;
        }

        public override void OnHide()
        {
            base.OnHide();
            m_is_update = false;
            TimeModule.Instance.RemoveTimeactionInObject(this.root, ShowEffect);
            HideEffect();

            GameEvents.System_Events.SetLoadingTips -= OnSetLoadingTips;
        }
        //private long m_seconds = 0;
        public override void LateUpdate()
        {
            base.LateUpdate();

            if (!m_is_update)
                return;

            long elapse_time = System.DateTime.Now.Ticks - m_time;


            if (elapse_time > 50000000)
            {
                m_is_update = false;
                LoadingManager.Instance.OfflineTips();
            }
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