using EngineCore;
using UnityEngine;
using GOGUI;
namespace SeekerGame
{
    [UILogicHandler(UIDefine.UI_UNLOCK)]
    public class BuildUnLockUILogic : UILogicBase
    {
        private GameTexture m_buildTex;
        private GameButton m_btnLock;
        private GameLabel m_describe;
        private GameButton m_btnClose;
        private GameLabel m_labName;

        private GameImage m_ImgCash;
        private GameImage m_ImgCoin;
        private GameLabel m_labCashCost;
        private GameLabel m_labCoinCost;

        private GameImage m_ImgCashSingle;
        private GameLabel m_labCashSingle;
        private GameUIEffect m_uiEffect;

        private long buildID;
        private ConfBuilding m_confBuild = null;
        //private UITweenerBase[] tweener = null;

        protected override void OnInit()
        {
            base.OnInit();
            m_buildTex = Make<GameTexture>("Panel_animation:RawImage_building");
            m_btnLock = Make<GameButton>("Panel_animation:btn_unlock");
            m_describe = Make<GameLabel>("Panel_animation:Text_detail");
            m_btnClose = Make<GameButton>("Panel_animation:Button_close");
            m_labName = Make<GameLabel>("Panel_animation:Text_name");
            this.m_ImgCash = m_btnLock.Make<GameImage>("Image:cash");
            this.m_ImgCoin = m_btnLock.Make<GameImage>("Image:coin");
            m_labCashCost = this.m_ImgCash.Make<GameLabel>("Text");
            m_labCoinCost = this.m_ImgCoin.Make<GameLabel>("Text");

            this.m_ImgCashSingle = m_btnLock.Make<GameImage>("Image:cash_single");
            this.m_labCashSingle = m_ImgCashSingle.Make<GameLabel>("Text");
            m_uiEffect = Make<GameUIEffect>("Panel_animation:btn_unlock:UI_jianzhujiesuo");
            //this.tweener = Transform.GetComponentsInChildren<UITweenerBase>(true);
            
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
            MessageHandler.RegisterMessageHandler(MessageDefine.SCBuildingUnlockResp,OnRes);
            this.m_btnClose.AddClickCallBack(BtnClose);
            this.m_btnLock.AddClickCallBack(BtnUnLock);
            if (param != null)
            {
                buildID = (long)param;
            }
            m_confBuild = ConfBuilding.Get(buildID);
            if (m_confBuild == null)
            {
                EngineCoreEvents.UIEvent.HideUIEvent.SafeInvoke(UIDefine.UI_UNLOCK);
                return;
            }
            ShowCashAndCoin(m_confBuild.cash != 0& m_confBuild.coin != 0);
            this.m_ImgCashSingle.Sprite = m_confBuild.cash == 0 ? "icon_mainpanel_coin_2.png" : "icon_mainpanel_cash_2.png";
            this.m_labCashSingle.Text = m_confBuild.cash == 0 ? m_confBuild.coin.ToString():m_confBuild.cash.ToString();
            m_labCashCost.Text = m_confBuild.cash.ToString();
            m_labCoinCost.Text = m_confBuild.coin.ToString();
            m_buildTex.TextureName = m_confBuild.stateName;
            m_describe.Text = LocalizeModule.Instance.GetString(m_confBuild.descs);
            m_labName.Text = LocalizeModule.Instance.GetString(m_confBuild.name);
            m_uiEffect.EffectPrefabName = "UI_jianzhujiesuo.prefab";
            //for (int i = 0; i < this.tweener.Length; i++)
            //{
            //    this.tweener[i].ResetAndPlay();
            //}
           // m_uiEffect.Visible = true;
        }

        private void ShowCashAndCoin(bool flag)
        {
            this.m_ImgCash.Visible = flag;
            this.m_ImgCoin.Visible = flag;
            this.m_ImgCashSingle.Visible = !flag;
        }

        public override void OnHide()
        {
            base.OnHide();
            MessageHandler.UnRegisterMessageHandler(MessageDefine.SCBuildingUnlockResp, OnRes);
            this.m_btnClose.RemoveClickCallBack(BtnClose);
            this.m_btnLock.RemoveClickCallBack(BtnUnLock);
            //this.m_uiEffect.Visible = false;
        }

        private void BtnUnLock(GameObject obj)
        {
            CSBuildingUnlockReq req = new CSBuildingUnlockReq();
            req.BuildingId = buildID;

#if !NETWORK_SYNC || UNITY_EDITOR
            GameEvents.NetWorkEvents.SendAsyncMsg.SafeInvoke(req);
#else
            GameEvents.NetWorkEvents.SendMsg.SafeInvoke(req);
#endif
        }

        private void BtnClose(GameObject obj)
        {
            EngineCoreEvents.UIEvent.HideUIEvent.SafeInvoke(UIDefine.UI_UNLOCK);
        }

        private void OnRes(object obj)
        {
            if (obj is SCBuildingUnlockResp)
            {
                SCBuildingUnlockResp res = obj as SCBuildingUnlockResp;
                if (!MsgStatusCodeUtil.OnError(res.ResponseStatus))
                {
                    GlobalInfo.MY_PLAYER_INFO.ChangeCash(-m_confBuild.cash);
                    GlobalInfo.MY_PLAYER_INFO.ChangeCoin(-m_confBuild.coin);
                    GameEvents.BigWorld_Event.OnUnLock.SafeInvoke(m_confBuild.lockResource);
                    EngineCoreEvents.UIEvent.HideUIEvent.SafeInvoke(UIDefine.UI_UNLOCK);
                }
            }
        }
    }
}
