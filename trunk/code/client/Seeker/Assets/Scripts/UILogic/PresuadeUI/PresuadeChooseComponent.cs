using System;
using System.Collections.Generic;
using EngineCore;
using UnityEngine;

namespace SeekerGame
{
    public class PresuadeChooseComponent : GameUIComponent
    {
        private PresuadeScroll3DComponent m_scroll3D = null;
        //private GameUIComponent m_chooseCom = null;
        private GameButton m_chooseBtn;
        private long[] m_ids;
        protected override void OnInit()
        {
            base.OnInit();
            this.m_scroll3D = Make<PresuadeScroll3DComponent>("Container");
            this.m_chooseBtn = Make<GameButton>("Button");
        }

        public override void OnShow(object param)
        {
            base.OnShow(param);
            this.m_chooseBtn.AddClickCallBack(OnChoose);
            GameEvents.UIEvents.UI_Scroll3D_Event.OnScrollStart += OnScrollStart;
            GameEvents.UIEvents.UI_Scroll3D_Event.OnScrollEnd += OnScrollEnd;
        }

        public override void OnHide()
        {
            base.OnHide();
            this.m_chooseBtn.RemoveClickCallBack(OnChoose);
            GameEvents.UIEvents.UI_Scroll3D_Event.OnScrollStart -= OnScrollStart;
            GameEvents.UIEvents.UI_Scroll3D_Event.OnScrollEnd -= OnScrollEnd;
        }

        private void OnChoose(GameObject obj)
        {
            GameEvents.UIEvents.UI_Presuade_Event.OnChooseId.SafeInvoke(chooseId);
            Visible = false;
        }

        private long chooseId = 0;
        public void SetData(long[] ids)
        {
            m_ids = ids;
            chooseId = ids[ids.Length / 2];
            m_scroll3D.SetData(ids);
        }

        private void OnScrollStart()
        {
            if (this.m_chooseBtn.CachedVisible)
            {
                this.m_chooseBtn.Visible = false;
            }
        }

        private void OnScrollEnd(int index)
        {
            chooseId = m_ids[index];
            this.m_chooseBtn.Visible = true;
        }
    }
}
