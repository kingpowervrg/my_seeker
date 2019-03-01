using EngineCore;
using GOGUI;
using Google.Protobuf;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace SeekerGame
{
    [UILogicHandler(UIDefine.UI_IAPCASH)]
    public class IAPcashUILogic : BaseViewComponetLogic
    {
        IAPcashUIView m_view;
        //private UITweenerBase[] tweener = null;
        public override void OnPackageRequest(IMessage imsg, params object[] msg_params)
        {
            base.OnPackageRequest(imsg, msg_params);
        }

        public override void OnScResponse(object s)
        {
            base.OnScResponse(s);
        }

        public override void OnShow(object param)
        {
            base.OnShow(param);
            MainPanelInGameUILogic.Show();
            //for (int i = 0; i < this.tweener.Length; i++)
            //{
            //    this.tweener[i].ResetAndPlay();
            //}

        }
        public override FrameDisplayMode UIFrameDisplayMode
        {
            get
            {
                return FrameDisplayMode.FULLSCREEN;
            }
        }
        public override void OnHide()
        {
            base.OnHide();
            MainPanelInGameUILogic.Hide();
        }

        protected override void OnInit()
        {
            base.OnInit();
            m_view = Make<IAPcashUIView>(root.name);
            this.SetCloseBtnID("Panel:Button_close");
            //this.tweener = Transform.GetComponentsInChildren<UITweenerBase>(true);
        }
    }
}
