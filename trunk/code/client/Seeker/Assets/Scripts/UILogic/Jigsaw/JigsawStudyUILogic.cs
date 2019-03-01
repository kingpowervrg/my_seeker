using EngineCore;
using GOEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SeekerGame
{
    [UILogicHandler(UIDefine.UI_JIGSAW_STUDY)]
    public class JigsawStudyUILogic : UILogicBase
    {
        GameUIEffect m_ui_effect;
        private System.Action<UILogicBase> m_close_act;
        private GameButton m_close_btn;
        protected override void OnInit()
        {
            base.OnInit();

            m_ui_effect = Make<GameUIEffect>("UI_shouzhijiantou");
            m_ui_effect.EffectPrefabName = "UI_shouzhijiantou.prefab";
            m_close_btn = Make<GameButton>("Button_continue");
        }

        public override void OnShow(object param)
        {
            base.OnShow(param);
            m_close_btn.AddClickCallBack(OnCloseClicked);
            m_ui_effect.Visible = true;
            if (null != param)
            {
                m_close_act = param as System.Action<UILogicBase>;

            }
        }

        public override void OnHide()
        {
            base.OnHide();

            m_close_btn.RemoveClickCallBack(OnCloseClicked);

            m_ui_effect.Visible = false;
        }

        private void OnCloseClicked(GameObject obj_)
        {
            m_close_act?.Invoke(this);

            TimeModule.Instance.SetTimeout(() => this.CloseFrame(), 0.7f);
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