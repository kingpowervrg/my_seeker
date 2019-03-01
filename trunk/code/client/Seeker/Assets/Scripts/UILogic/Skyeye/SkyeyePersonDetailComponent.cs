using EngineCore;
using UnityEngine;
namespace SeekerGame
{
    public class SkyeyePersonDetailComponent : GameUIComponent
    {
        private GameLabel m_personNameLab = null;
        private GameTexture m_personTex = null;
        private GameLabel m_detailLab = null;
        private GameUIComponent m_maskCom = null;
        private GameButton m_chatBtn = null;
        private GameUIContainer m_resGrid = null;
        private GameButton m_closeBtn = null;

        private long m_presuadeId = 0;
        protected override void OnInit()
        {
            base.OnInit();
            this.m_personNameLab = Make<GameLabel>("Animator:Image_sence:Text_Name");
            this.m_personTex = Make<GameTexture>("Animator:RawImage");
            this.m_detailLab = Make<GameLabel>("Animator:Text_Desc");
            this.m_maskCom = Make<GameUIComponent>("Animator:mask");
            this.m_chatBtn = Make<GameButton>("Animator:RawImage:Button_action");
            this.m_closeBtn = Make<GameButton>("Animator:Button_close");
            this.m_resGrid = Make<GameUIContainer>("Animator:ScrollView:Viewport");
        }

        public override void OnShow(object param)
        {
            base.OnShow(param);
            this.m_closeBtn.AddClickCallBack(OnClose);
            this.m_chatBtn.AddClickCallBack(OnChat);
            this.isCloseing = false;
        }

        public override void OnHide()
        {
            base.OnHide();
            this.isCloseing = false;
            //this.m_closeBtn.Enable = true;
            this.m_closeBtn.RemoveClickCallBack(OnClose);
            this.m_chatBtn.RemoveClickCallBack(OnChat);
            for (int i = 0; i < this.m_resGrid.ChildCount; i++)
            {
                this.m_resGrid.GetChild<SkyEyeProp>(i).Visible = false;
            }
        }

        bool isCloseing = false;
        private void OnClose(GameObject obj)
        {
            if (isCloseing)
                return;
            isCloseing = true;
            //this.m_closeBtn.Enable = false;
            Visible = false;
        }

        private void OnChat(GameObject obj)
        {
            PresuadeUILogic.Show(this.skyEyeId, this.m_presuadeId);
            Visible = false;
        }

        private long skyEyeId;
        public void SetData(ConfSkyEye conSky,bool isUnLock,bool hasReward)
        {
            skyEyeId = conSky.id;
            ConfNpc npc = ConfNpc.Get(conSky.npcId);
            this.m_presuadeId = conSky.refuteId;
            this.m_personNameLab.Text = isUnLock?LocalizeModule.Instance.GetString(npc.name):"?";
            this.m_detailLab.Text = LocalizeModule.Instance.GetString(conSky.descs);
            this.m_personTex.Visible = isUnLock;
            
            this.m_maskCom.Visible = !isUnLock;
            
            if (isUnLock)
            {
                this.m_personTex.TextureName = npc.icon;
            }
            int hasUnLockNumber = 0;
            this.m_resGrid.EnsureSize<SkyEyeProp>(conSky.collectorIds.Length);
            for (int i = 0; i < conSky.collectorIds.Length; i++)
            {
                SkyEyeProp icon = this.m_resGrid.GetChild<SkyEyeProp>(i);
                PlayerPropMsg playerProp = GlobalInfo.MY_PLAYER_INFO.GetBagInfosByID(conSky.collectorIds[i]);
                bool isPropUnLock = false;
                if (playerProp != null && playerProp.Count > 0)
                {
                    isPropUnLock = true;
                    hasUnLockNumber++;
                }
                icon.SetData(conSky.collectorIds[i], isPropUnLock);
                icon.Visible = true;
            }
            this.m_chatBtn.Visible = !hasReward && (hasUnLockNumber == conSky.collectorIds.Length);
        }

        public class SkyEyeProp : GameUIComponent
        {
            private GameImage m_iconImg = null;
            private GameImage m_mask = null;
            private Color m_maskBGColor = new Color(1,1,1,0.6f);
            private Color m_maskIconColor = new Color(0.4f, 0.4f, 0.4f, 1);

            private GameButton m_btn = null;
            
            private ConfProp m_confProp = null;
            protected override void OnInit()
            {
                base.OnInit();
                this.m_iconImg = Make<GameImage>("Image");
                this.m_mask = Make<GameImage>("Background");
                this.m_btn = Make<GameButton>("Image");
            }

            public override void OnShow(object param)
            {
                base.OnShow(param);
                this.m_btn.AddClickCallBack(OnBtn);
            }

            public override void OnHide()
            {
                base.OnHide();
                this.m_btn.RemoveClickCallBack(OnBtn);
            }

            public void SetData(long propId,bool isUnLock)
            {
                m_confProp = ConfProp.Get(propId);
                if (m_confProp == null)
                {
                    Visible = true;
                    return;
                }

                if (isUnLock)
                {
                    m_iconImg.Color = Color.white;
                    m_mask.Color = Color.white;
                }
                else
                {
                    m_iconImg.Color = m_maskBGColor;
                    m_mask.Color = m_maskIconColor;
                }
                this.m_iconImg.Sprite = m_confProp.icon;
            }

            public void OnBtn(GameObject obj)
            {
                GameEvents.UIEvents.UI_SkyEye_Event.OnOpenIconDetail.SafeInvoke(m_confProp.description, m_confProp.icon);
            }
        }

        
    }
}
