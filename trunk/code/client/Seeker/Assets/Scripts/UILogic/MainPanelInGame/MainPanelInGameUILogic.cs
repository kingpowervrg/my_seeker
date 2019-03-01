using EngineCore;
using UnityEngine;
using DG.Tweening;
namespace SeekerGame
{
    [UILogicHandler(UIDefine.UI_MAINPANEL_INGAME)]
    public class MainPanelInGameUILogic : UILogicBase
    {
        private GameLabelExtend m_cashLab = null;
        private GameLabelExtend m_coinLab = null;

        private GameLabel m_totalEnergyLab = null;
        private GameLabelExtend m_currentEnergyLab = null;
        private GameProgressBar m_progressBar = null;

        private GameImage m_VitBG = null;
        private GameUIEffect m_VitEffect = null;


        static int S_Ref_Count = 0;

        protected override void OnInit()
        {
            base.OnInit();
            this.m_cashLab = Make<GameLabelExtend>("Panel_top:Image_cash:Text_number");
            this.m_coinLab = Make<GameLabelExtend>("Panel_top:Image_coin:Text_number");

            this.m_currentEnergyLab = Make<GameLabelExtend>("Panel_top:Image_energy:Text_number");
            this.m_totalEnergyLab = this.m_currentEnergyLab.Make<GameLabel>("totalNumber");
            this.m_progressBar = Make<GameProgressBar>("Panel_top:Image_energy:Slider");

            this.m_VitBG = Make<GameImage>("Panel_top:Image_energy");

            this.m_VitEffect = Make<GameUIEffect>("Panel_top:Image_energy:Image_icon:UI_tili");

            NeedUpdateByFrame = true;
        }

        public override void OnShow(object param)
        {
            base.OnShow(param);
            GameEvents.UIEvents.UI_GameEntry_Event.OnReflashCoin += ReflashCoin;
            GameEvents.UIEvents.UI_GameEntry_Event.OnReflashCash += ReflashCash;
            GameEvents.UIEvents.UI_GameEntry_Event.OnReflashVit += OnReflashVit;
            this.m_VitEffect.EffectPrefabName = "UI_tili.prefab";
            m_VitEffect.Visible = false;
            PlayerInfoManager.OnPlayerInfoUpdatedEvent += OnInitUI;
            OnInitUI(GlobalInfo.MY_PLAYER_INFO);

        }

        public override void OnHide()
        {
            base.OnHide();
            this.m_VitEffect.Visible = false;
            GameEvents.UIEvents.UI_GameEntry_Event.OnReflashCoin -= ReflashCoin;
            GameEvents.UIEvents.UI_GameEntry_Event.OnReflashCash -= ReflashCash;
            GameEvents.UIEvents.UI_GameEntry_Event.OnReflashVit -= OnReflashVit;
            PlayerInfoManager.OnPlayerInfoUpdatedEvent -= OnInitUI;
        }

        public override void Update()
        {
            base.Update();
            this.m_cashLab.OnUpdate();
            this.m_coinLab.OnUpdate();
            this.m_currentEnergyLab.OnUpdate();
        }

        #region reflash data
        private void OnInitUI(PlayerInfo playerInfo)
        {
            this.m_cashLab.Text = playerInfo.Cash.ToString();
            this.m_coinLab.Text = playerInfo.Coin.ToString();
            this.m_currentEnergyLab.Text = playerInfo.Vit.ToString();
            ReflashVitEffect(playerInfo.Vit);
            this.m_totalEnergyLab.Text = "/" + CommonData.MAXVIT.ToString();
        }

        private void ReflashCoin(int startCoin, int endCoin)
        {
            m_coinLab.SetChangeTextRoll(startCoin, endCoin);
        }

        private void OnReflashVit(int startVit, int endVit)
        {
            this.m_currentEnergyLab.SetChangeTextRoll(startVit, endVit);
            ReflashVitEffect(endVit);
        }

        private void ReflashCash(int startCash, int endCash)
        {
            m_cashLab.SetChangeTextRoll(startCash, endCash);
        }

        private void ReflashVitEffect(float endVit)
        {
            float width = this.m_VitBG.Widget.rect.width;
            //float posX = Mathf.Min(endVit, CommonData.MAXVIT) * (width / CommonData.MAXVIT) - width / 2f;
            //this.m_VitEffect.gameObject.transform.localPosition = Vector3.right * posX;
            float aim_val = endVit / CommonData.MAXVIT;
            aim_val = Mathf.Clamp01(aim_val);
            if (!MathUtil.FloatEqual(this.m_progressBar.Value, aim_val))
            {
                DOTween.To(() => this.m_progressBar.Value, x => this.m_progressBar.Value = x, aim_val, 0.5f).OnUpdate(() =>
                {
                    m_VitEffect.Visible = true;
                    Vector3 topRightConner = this.m_progressBar.FillRectangleWorldConners[2];
                    Vector3 bottomRightConner = this.m_progressBar.FillRectangleWorldConners[3];
                    Vector3 centerPos = new Vector3(topRightConner.x, (topRightConner.y + bottomRightConner.y) / 2, topRightConner.z);
                    m_VitEffect.Position = centerPos;
                }).OnComplete(() =>
                {
                    this.m_VitEffect.Visible = false;
                });
                //CommonHelper.UpdateEffectPosByProgressbar(this.m_vit_progress, m_VitEffect, 0.05f, 0.5f);
            }
        }
        #endregion

        public static void Show()
        {
            ++S_Ref_Count;
            EngineCoreEvents.UIEvent.ShowUIEvent.SafeInvoke(UIDefine.UI_MAINPANEL_INGAME);
        }

        public static void Hide()
        {
            --S_Ref_Count;

            if (S_Ref_Count <= 0)
            {
                S_Ref_Count = 0;
                EngineCoreEvents.UIEvent.HideUIEvent.SafeInvoke(UIDefine.UI_MAINPANEL_INGAME);
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
