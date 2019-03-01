using System;
using System.Collections.Generic;
using EngineCore;
using UnityEngine;
namespace SeekerGame
{
    [UILogicHandler(UIDefine.UI_GIFTBAG)]
    public class GiftBagUILogic : UILogicBase
    {
        private GiftItemView[] m_giftItem;
        private GameButton m_btnQuit = null;
        private ENUM_PUSH_GIFT_BLOCK_TYPE curType = ENUM_PUSH_GIFT_BLOCK_TYPE.E_NONE;
        protected override void OnInit()
        {
            base.OnInit();
            m_giftItem = new GiftItemView[2];
            this.m_giftItem[0] = Make<GiftItemView>("RawImage");
            this.m_giftItem[1] = Make<GiftItemView>("RawImage_1");
            this.m_btnQuit = Make<GameButton>("Button_close");
        }

        public override void OnShow(object param)
        {
            base.OnShow(param);
            
            if (param != null)
            {
                curType = (ENUM_PUSH_GIFT_BLOCK_TYPE)param;
            }
            else
            {
                OnQuit(null);
            }
            this.m_btnQuit.AddClickCallBack(OnQuit);
            for (int i = 0; i < this.m_giftItem.Length; i++)
            {
                this.m_giftItem[i].Visible = false;
            }
            List<Push_Info> gifts = PushGiftManager.Instance.GetPushInfosByTurnOnType(curType);
            if (gifts != null)
            {
                for (int i = 0; i < gifts.Count; i++)
                {
                    if (i < this.m_giftItem.Length)
                    {
                        m_giftItem[i].Refresh(gifts[i]);
                        m_giftItem[i].Visible = true;
                    }
                }
            }
        }

        public override FrameDisplayMode UIFrameDisplayMode
        {
            get
            {
                return FrameDisplayMode.WINDOWED;
            }
        }

        public override void OnHide()
        {
            base.OnHide();
        }

        private void OnQuit(GameObject obj)
        {
            EngineCoreEvents.UIEvent.HideUIEvent.SafeInvoke(UIDefine.UI_GIFTBAG);
            if (curType == ENUM_PUSH_GIFT_BLOCK_TYPE.E_VIT)
            {
                //PopUpManager.OpenGoToVitShop();

                FrameMgr.OpenUIParams ui_param = new FrameMgr.OpenUIParams(UIDefine.UI_SHOPENERGY);
                ui_param.Param = true;

                EngineCoreEvents.UIEvent.ShowUIEventWithParam.SafeInvoke(ui_param);
            }
            
        }

        public static void OpenGiftBag(ENUM_PUSH_GIFT_BLOCK_TYPE type)
        {
            FrameMgr.OpenUIParams param = new FrameMgr.OpenUIParams(UIDefine.UI_GIFTBAG);
            param.Param = type;
            EngineCoreEvents.UIEvent.ShowUIEventWithParam.SafeInvoke(param);
        }
    }
}
