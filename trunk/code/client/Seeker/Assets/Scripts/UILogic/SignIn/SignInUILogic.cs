using EngineCore;
using System.Collections.Generic;
using UnityEngine;

namespace SeekerGame
{
    [UILogicHandler(UIDefine.UI_SIGNIN)]
    public class SignInUILogic : UILogicBase
    {
        private GameTexture m_BgImage;
        private SignInItem[] m_Items;
        //private SignImageIcon[] m_signIcon;
        //private GameUIEffect[] m_UIEffectItem;
        private GameUIEffect m_UIEffect;
        private GameButton m_BtnGet;
        private GameLabel m_BtnLabel;
        private GameUIEffect m_mainEffect = null;
        //private SCPlayerCheckInInfoResp m_data;
        private SignInSystem m_SignInSystem;

        private int MaxSize = 7;

        protected override void OnInit()
        {
            base.OnInit();
            m_BgImage = Make<GameTexture>("RawImage");
            m_BtnGet = Make<GameButton>("btn_buy");
            m_BtnLabel = m_BtnGet.Make<GameLabel>("Text");
            m_Items = new SignInItem[MaxSize];
            for (int i = 1; i <= MaxSize; i++)
            {
                m_Items[i - 1] = Make<SignInItem>("Image_" + i);
                m_Items[i - 1].Visible = false;
                SignImageIcon imageIcon = Make<SignImageIcon>("Icon:Image_icon_" + i);
                m_Items[i - 1].SetIcon(imageIcon);
            }
            m_UIEffect = Make<GameUIEffect>("UI_meirilingqu");
            this.m_mainEffect = Make<GameUIEffect>("RawImage:UI_meirilingqu_03");
        }
        public override FrameDisplayMode UIFrameDisplayMode
        {
            get
            {
                return FrameDisplayMode.WINDOWED;
            }
        }
        public override void OnShow(object param)
        {
            base.OnShow(param);
            m_BtnGet.AddClickCallBack(OnReceive);
            GameEvents.UIEvents.UI_SignIn_Event.OnSignIn += OnSignIn;

            this.m_mainEffect.EffectPrefabName = "UI_meirilingqu_03.prefab";
            this.m_mainEffect.Visible = true;
            m_SignInSystem = new SignInSystem();
            if (param is SCPlayerCheckInInfoResp)
            {
                m_SignInSystem.InitSignSystem((SCPlayerCheckInInfoResp)param);
            }
            else
            {
                m_SignInSystem.InitSignSystem(null);
            }
        }

        private void OnSignIn()
        {
            SetData();
        }

        private void OnError()
        {
            for (int i = 0; i < MaxSize; i++)
            {
                m_Items[i].Visible = false;
            }
        }

        private void SetData()
        {
            if (m_SignInSystem.m_data.Status == 0)
            {
                m_UIEffect.gameObject.transform.SetParent(m_Items[m_SignInSystem.m_data.Day - 1].gameObject.transform, false);
                m_UIEffect.gameObject.transform.localPosition = Vector3.zero + Vector3.left * 2f;
                if (m_SignInSystem.m_data.Day < 7)
                {
                    m_UIEffect.EffectPrefabName = "UI_meirilingqu_01.prefab";
                }
                else if (m_SignInSystem.m_data.Day == 7)
                {
                    m_UIEffect.EffectPrefabName = "UI_meirilingqu_02.prefab";
                }
            }
            m_UIEffect.Visible = (m_SignInSystem.m_data.Status == 0);
            if (!m_SignInSystem.AddItemTable())
            {
                OnError();
            }

            if (m_SignInSystem.m_data.Status == 0)
            {
                m_BtnLabel.Text = "Receive";
            }
            else
            {
                m_BtnLabel.Text = "Close";
            }

            for (int i = 0; i < MaxSize; i++)
            {
                //string iconName = m_SignInSystem.GetPropNameByIndex(i);
                int curDay = i + 1;
                bool hasGet = false;
                ////int day = m_SignInSystem.m_data.Day;
                //if (m_SignInSystem.m_data.Day > curDay)
                //{

                //}
                if (m_SignInSystem.m_data.Status == 1 && m_SignInSystem.m_data.Day == curDay) //已领取
                {
                    hasGet = true;
                }
                m_Items[i].SetData(m_SignInSystem.GetPropIdByIndex(i), m_SignInSystem.GetItemCountByIndex(i), m_SignInSystem.m_data.Day > curDay || hasGet);
                m_Items[i].Visible = true;
            }
        }

        public override void OnHide()
        {
            base.OnHide();
            m_BtnGet.RemoveClickCallBack(OnReceive);
            GameEvents.UIEvents.UI_SignIn_Event.OnSignIn -= OnSignIn;
            m_SignInSystem.OnDispose();
            this.m_mainEffect.Visible = false;

            //GameEvents.UIEvents.UI_Bonus_Pop_View_Event.Tell_OnHide.SafeInvoke();
            //GameEvents.UIEvents.UI_Bonus_Pop_View_Event.Tell_OnShow.SafeInvoke();
        }

        private void OnReceive(GameObject obj)
        {

            if (m_SignInSystem.m_data.Status == 0)
            {
                EngineCoreEvents.AudioEvents.PlayAudio.SafeInvoke(Audio.AudioType.UISound, GameCustomAudioKey.qiandao_open.ToString());
                CSPlayerCheckInReq req = new CSPlayerCheckInReq();
#if !NETWORK_SYNC || UNITY_EDITOR
                GameEvents.NetWorkEvents.SendAsyncMsg.SafeInvoke(req);
#else
                GameEvents.NetWorkEvents.SendMsg.SafeInvoke(req);
#endif

            }
            else
            {
                EngineCoreEvents.AudioEvents.PlayAudio.SafeInvoke(Audio.AudioType.UISound, EngineCommonAudioKey.Close_Window.ToString());
                EngineCoreEvents.UIEvent.HideUIEvent.SafeInvoke(UIDefine.UI_SIGNIN);
            }

        }

        public class SignInItem : GameUIComponent
        {
            //private GameImage m_Icon_Img;
            private GameLabel m_Number_Lab;
            //private GameImage m_Choose_Img;

            //private GameImage m_Tag_Img;
            //private GameLabel m_Tag_Lab;
            //private GameUIEffect m_UIEffect;
            //private GameImage m_mask_Img;
            private SignImageIcon m_ImageIcon = null;
            protected override void OnInit()
            {
                base.OnInit();
                //m_Icon_Img = Make<GameImage>("Image_icon");
                m_Number_Lab = Make<GameLabel>("Text_number");
                //m_Choose_Img = Make<GameImage>("Image_received");

                //m_Tag_Img = Make<GameImage>("Image_lable");
                //m_Tag_Lab = m_Tag_Img.Make<GameLabel>("Text");
                //m_UIEffect = Make<GameUIEffect>("");
                //m_mask_Img = Make<GameImage>("Image");
            }

            public override void OnShow(object param)
            {
                base.OnShow(param);
            }

            public void SetData(long itemId, int num, bool flag)
            {
                this.m_ImageIcon.SetData(itemId, num);
                //this.m_ImageIcon.SetIconName(icon);
                m_Number_Lab.Text = string.Format("x{0}", num);
                SetState(flag);
                //m_Choose_Img.Visible = flag;
            }

            public void SetIcon(SignImageIcon m_icon)
            {
                this.m_ImageIcon = m_icon;
            }


            private void SetState(bool b)
            {
                if (b)
                {
                    SetState(0.7f);
                    this.m_ImageIcon.SetChoose(b, 0.7f);
                    //m_mask_Img.Visible = true;
                }
                else
                {
                    SetState(1f);
                    this.m_ImageIcon.SetChoose(b, 1f);
                    // m_mask_Img.Visible = false;
                }
            }

            private void SetState(float alpha)
            {

                m_Number_Lab.color = new Color(m_Number_Lab.color.r, m_Number_Lab.color.g, m_Number_Lab.color.b, alpha);
            }
            public override void OnHide()
            {
                base.OnHide();
            }
        }

        public class SignImageIcon : GameUIComponent
        {
            private GameImage m_IconImg;
            private GameImage m_ChooseImg;
            private GameImage m_maskImg = null;

            //private GameImage m_Tag_Img;
            //private GameLabel m_Tag_Lab;

            private long m_item_id;
            private int Num;
            protected override void OnInit()
            {
                base.OnInit();
                this.m_IconImg = this.Make<GameImage>(gameObject);
                this.m_ChooseImg = this.Make<GameImage>("Image_received");
                this.m_maskImg = this.Make<GameImage>("Image");

                //m_Tag_Img = Make<GameImage>("Image_lable");
                //m_Tag_Lab = m_Tag_Img.Make<GameLabel>("Text");
            }

            public override void OnShow(object param)
            {
                base.OnShow(param);
                this.m_IconImg.AddPressDownCallBack(PressDown);
                this.m_IconImg.AddPressUpCallBack(PressUp);
            }

            public override void OnHide()
            {
                base.OnHide();
                this.m_IconImg.RemovePressDownCallBack(PressDown);
                this.m_IconImg.RemovePressUpCallBack(PressUp);
            }

            //public void SetIconName(string iconName)
            //{
            //    this.m_IconImg.Sprite = iconName;
            //}

            public void SetData(long itemId, int count)
            {
                this.m_item_id = itemId;
                this.Num = count;
                ConfProp confprop = ConfProp.Get(itemId);
                if (confprop == null)
                {
                    Debug.LogError("error prop id:" + itemId);
                    return;
                }
                this.m_IconImg.Sprite = confprop.icon;

            }

            public void SetChoose(bool flag, float alpha)
            {
                this.m_ChooseImg.Visible = flag;
                this.m_maskImg.Visible = flag;
                m_IconImg.Color = new Color(m_IconImg.Color.r, m_IconImg.Color.g, m_IconImg.Color.b, alpha);
                //m_Tag_Img.Color = new Color(m_Tag_Img.Color.r, m_Tag_Img.Color.g, m_Tag_Img.Color.b, alpha);
                //m_Tag_Lab.color = new Color(m_Tag_Lab.color.r, m_Tag_Lab.color.g, m_Tag_Lab.color.b, alpha);
            }

            private void PressDown(GameObject obj)
            {
                Vector2 screenPoint2 = RectTransformUtility.WorldToScreenPoint(CameraManager.Instance.UICamera, this.Widget.position);

                int address_count = ConfProp.Get(m_item_id).address.Length;

                ToolTipsData data = new ToolTipsData()
                {
                    ItemID = m_item_id,
                    CurCount = 0,
                    MaxCount = 0,

                    ScreenPos = screenPoint2 - new Vector2(ToolTipsView.C_WIDTH * 0.5f/* - this.Widget.sizeDelta.x * 0.5f*/, -10.0f * address_count),
                };


                FrameMgr.OpenUIParams ui_data = new FrameMgr.OpenUIParams(UIDefine.UI_TOOL_TIPS)
                {
                    Param = data,
                };

                EngineCoreEvents.UIEvent.ShowUIEventWithParam.SafeInvoke(ui_data);
            }

            private void PressUp(GameObject obj)
            {
                EngineCoreEvents.UIEvent.HideUIEvent.SafeInvoke(UIDefine.UI_TOOL_TIPS);
            }
        }
    }
}
