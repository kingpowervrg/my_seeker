using DG.Tweening;
using EngineCore;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace SeekerGame
{
    [UILogicHandler(UIDefine.UI_BANNER)]
    public class BannerUILogic : UILogicBase
    {
        private const long IAP_COIN_ID = 1L;

        private PlayerInfoUIComponent m_playerInfoComponent = null;

        private GameLabelExtend m_health_lab;
        private GameLabel m_totalVit_lab = null;
        private GameButton m_addHealth_btn;

        private GameLabelExtend m_gold_lab;
        private GameButton m_btnGold_btn;

        private GameLabelExtend m_dollar_lab;
        private GameButton m_btnDollar_btn;
        private GameUIEffect m_dollar_effect;

        private GameFilledImage m_ProgressVit;
        private GameLabel m_CountDownVit_Lab;
        private GameImage m_CountDownVit_Root;
        private GameUIEffect m_VitEffect;
        private GameImage m_VitBG;
        private GameImage m_vitFreeImg = null;

        private GameUIEffect m_tiliEffect;
        private GameUIEffect m_jinbiEffect;

        private GameUIComponent m_panelTop = null;
        private TweenPosition m_paneTopTweener = null;

        private GameImage m_cashImg;
        private GameImage m_coinImg;
        private GameImage m_vitImg;
        private GameProgressBar m_vit_progress;

        private List<string> m_exclude_ui = new List<string>()
        {
            UIDefine.UI_BANNER,
            UIDefine.UI_GAMEENTRY,
            UIDefine.UI_GM,
            UIDefine.UI_BUILD_TOP,
            UIDefine.UI_TASK_ON_BUILD,
            UIDefine.UI_GUID
        };

        protected override void OnInit()
        {
            base.OnInit();

            this.m_playerInfoComponent = Make<PlayerInfoUIComponent>("Panel_top:person");
            this.m_health_lab = Make<GameLabelExtend>("Panel_top:Image_energy:Text_number");
            this.m_totalVit_lab = this.m_health_lab.Make<GameLabel>("totalNumber");
            this.m_addHealth_btn = Make<GameButton>("Panel_top:Image_energy:Button_add");

            this.m_coinImg = Make<GameImage>("Panel_top:Image_coin");
            this.m_gold_lab = Make<GameLabelExtend>("Panel_top:Image_coin:Text_number");
            this.m_btnGold_btn = Make<GameButton>("Panel_top:Image_coin:Button_add");

            this.m_cashImg = Make<GameImage>("Panel_top:Image_cash");
            this.m_dollar_lab = Make<GameLabelExtend>("Panel_top:Image_cash:Text_number");
            this.m_btnDollar_btn = Make<GameButton>("Panel_top:Image_cash:Button_add");
            this.m_dollar_effect = Make<GameUIEffect>("Panel_top:Image_cash:Image_icon:UI_chaopiao_jiemian");
            this.m_dollar_effect.EffectPrefabName = "UI_chaopiao_jiemian.prefab";
            this.m_dollar_effect.Visible = true;


            this.m_vitImg = Make<GameImage>("Panel_top:Image_energy");
            this.m_CountDownVit_Root = Make<GameImage>("Panel_top:Image_energy:Background");
            this.m_ProgressVit = this.m_CountDownVit_Root.Make<GameFilledImage>("Fill");
            this.m_CountDownVit_Lab = this.m_CountDownVit_Root.Make<GameLabel>("countDown");
            this.m_vitFreeImg = this.m_vitImg.Make<GameImage>("Image_free");
            this.m_VitEffect = Make<GameUIEffect>("Panel_top:Image_energy:Image_icon:UI_tili");
            this.m_VitEffect.EffectPrefabName = "UI_tili.prefab";
            m_VitEffect.Visible = false;

            this.m_VitBG = Make<GameImage>("Panel_top:Image_energy");
            this.m_vit_progress = Make<GameProgressBar>("Panel_top:Image_energy:Slider");

            this.m_tiliEffect = Make<GameUIEffect>("Panel_top:Image_energy:Image_icon:UI_tili");
            this.m_tiliEffect.EffectPrefabName = "UI_tili.prefab";
            this.m_jinbiEffect = Make<GameUIEffect>("Panel_top:Image_coin:Image_icon:UI_jinbi");
            this.m_jinbiEffect.EffectPrefabName = "UI_jinbi.prefab";

            this.m_panelTop = Make<GameUIComponent>("Panel_top");
            this.m_paneTopTweener = m_panelTop.GetComponent<TweenPosition>();


            NeedUpdateByFrame = true;
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
            PlayerInfoManager.OnPlayerInfoUpdatedEvent += RefreshPlayerInfo;
            GameEvents.UIEvents.UI_GameEntry_Event.OnCountDownVit += OnCountDownVit;
            GameEvents.RedPointEvents.User_OnNewEmailEvent += OnNewEmail;
            GameEvents.UIEvents.UI_GameEntry_Event.OnOpenPanel += OnOpenPanel;
            GameEvents.UIEvents.UI_Common_Event.OnCommonUIVisible += OnCommonUIVisible;

            this.m_addHealth_btn.AddClickCallBack(OnBtnAddVitClick);
            this.m_btnDollar_btn.AddClickCallBack(OnBtnAddCashClick);
            this.m_btnGold_btn.AddClickCallBack(OnBtnAddCoinClick);
            GameEvents.UIEvents.UI_GameEntry_Event.OnReflashCoin += ReflashCoin;
            GameEvents.UIEvents.UI_GameEntry_Event.OnReflashCash += ReflashCash;
            GameEvents.UIEvents.UI_GameEntry_Event.OnReflashVit += OnReflashVit;
            GameEvents.UI_Guid_Event.OnMainIconUnLockComplete += OnMainIconUnLockComplete;
            GameEvents.UIEvents.UI_GameEntry_Event.OnInfiniteVit += OnInfiniteVit;
            if (!GameEntryCommonManager.Instance.m_needTopareaUITweener)
            {
                GameEntryCommonManager.Instance.m_toPareaTweenerComplete = true;
                SeekerGame.NewGuid.GuidNewManager.Instance.OnReflashGuidStatus();
            }
            GameEntryCommonManager.Instance.SetNeedTopareaUITweener(() =>
            {

                this.m_paneTopTweener.ResetAndPlay();
                this.m_paneTopTweener.SetTweenCompletedCallback(() =>
                {

                    GameEntryCommonManager.Instance.m_toPareaTweenerComplete = true;
                    SeekerGame.NewGuid.GuidNewManager.Instance.OnReflashGuidStatus();
                });
            });

            RefreshPlayerInfo(GlobalInfo.MY_PLAYER_INFO);
            this.m_totalVit_lab.Text = "/" + CommonData.MAXVIT.ToString();



            if (null != param)
            {
                string open_ui_name = param as string;
                GameEvents.UIEvents.UI_GameEntry_Event.OnOpenPanel.SafeInvoke(open_ui_name);
            }

            GameEvents.PlayerEvents.RequestLatestPlayerInfo();

            m_VitEffect.Visible = false;
        }

        public override void OnGuidShow(int type = 0)
        {
            base.OnGuidShow(type);
        }

        private void OnMainIconUnLockComplete(int index)
        {
            if (index == 6)
            {
                TweenAlpha tweener = this.m_vitImg.gameObject.GetOrAddComponent<TweenAlpha>();
                tweener.ResetAndPlay();
                this.m_vitImg.Visible = true;

                TweenAlpha tweenerCoin = this.m_coinImg.gameObject.GetOrAddComponent<TweenAlpha>();
                tweenerCoin.ResetAndPlay();
                this.m_coinImg.Visible = true;
            }
            else if (index == 7)
            {
                TweenAlpha tweener = this.m_cashImg.gameObject.GetOrAddComponent<TweenAlpha>();
                tweener.ResetAndPlay();
                this.m_cashImg.Visible = true;
            }

        }

        private void OnOpenPanel(string panelName)
        {
            if (panelName == UIDefine.UI_PLAYER_INFO)
            {
                EngineCoreEvents.UIEvent.ShowUIEvent.SafeInvoke(UIDefine.UI_PLAYER_INFO);
            }
        }

        private void OnBtnMainClick(GameObject btnMail)
        {
            if (GlobalInfo.GAME_NETMODE == GameNetworkMode.Standalone)
            {
                return;
            }
            GameEvents.BigWorld_Event.OnClickScreen.SafeInvoke();
            EngineCoreEvents.AudioEvents.PlayAudio.SafeInvoke(Audio.AudioType.UISound, GameCustomAudioKey.mail_open.ToString());

            GameEvents.RedPointEvents.Sys_OnNewEmailReadedEvent.SafeInvoke();

            EngineCoreEvents.UIEvent.ShowUIEvent.SafeInvoke(UIDefine.UI_MAIL);
            GameEvents.UIEvents.UI_GameEntry_Event.OnControlActivity.SafeInvoke(false);

        }

        private void OnBtnAddVitClick(GameObject btnAddVit)
        {
            if (GlobalInfo.GAME_NETMODE == GameNetworkMode.Standalone)
            {
                return;
            }
            GameEvents.BigWorld_Event.OnClickScreen.SafeInvoke();
            EngineCoreEvents.AudioEvents.PlayAudio.SafeInvoke(Audio.AudioType.UISound, EngineCommonAudioKey.Button_Click_Common.ToString());
            EngineCoreEvents.UIEvent.ShowUIEvent.SafeInvoke(UIDefine.UI_SHOPENERGY);
        }

        private void OnBtnAddCashClick(GameObject btnAddCash)
        {
            if (GlobalInfo.GAME_NETMODE == GameNetworkMode.Standalone)
            {
                return;
            }
            GameEvents.BigWorld_Event.OnClickScreen.SafeInvoke();
            EngineCoreEvents.AudioEvents.PlayAudio.SafeInvoke(Audio.AudioType.UISound, EngineCommonAudioKey.Button_Click_Common.ToString());
            EngineCoreEvents.UIEvent.ShowUIEvent.SafeInvoke(UIDefine.UI_IAPCASH);
        }

        public void OnBtnAddCoinClick(GameObject btnAddCoin)
        {
            return;
            if (GlobalInfo.GAME_NETMODE == GameNetworkMode.Standalone)
            {
                return;
            }
            GameEvents.BigWorld_Event.OnClickScreen.SafeInvoke();
            EngineCoreEvents.AudioEvents.PlayAudio.SafeInvoke(Audio.AudioType.UISound, EngineCommonAudioKey.Button_Click_Common.ToString());
            EngineCoreEvents.UIEvent.ShowUIEvent.SafeInvoke(UIDefine.UI_SHOPCOIN);
        }

        private void ReflashCoin(int startCoin, int endCoin)
        {
            m_gold_lab.SetChangeTextRoll(startCoin, endCoin);
        }

        private void OnReflashVit(int startVit, int endVit)
        {
            this.m_health_lab.SetChangeTextRoll(startVit, endVit);
            ReflashVitEffect(endVit);
        }

        private void ReflashVitEffect(float endVit)
        {
            float width = this.m_VitBG.Widget.rect.width;
            //float posX = Mathf.Min(endVit, CommonData.MAXVIT) * (width / CommonData.MAXVIT) - width / 2f;
            //this.m_VitEffect.gameObject.transform.localPosition = Vector3.right * posX;
            float aim_val = endVit / CommonData.MAXVIT;
            aim_val = Mathf.Clamp01(aim_val);
            if (!MathUtil.FloatEqual(this.m_vit_progress.Value, aim_val))
            {
                DOTween.To(() => this.m_vit_progress.Value, x => this.m_vit_progress.Value = x, aim_val, 0.5f).OnUpdate(() =>
                {
                    m_VitEffect.Visible = true;
                    Vector3 topRightConner = this.m_vit_progress.FillRectangleWorldConners[2];
                    Vector3 bottomRightConner = this.m_vit_progress.FillRectangleWorldConners[3];
                    Vector3 centerPos = new Vector3(topRightConner.x, (topRightConner.y + bottomRightConner.y) / 2, topRightConner.z);
                    m_VitEffect.Position = centerPos;
                }).OnComplete(() =>
                {
                    this.m_VitEffect.Visible = false;
                });
                //CommonHelper.UpdateEffectPosByProgressbar(this.m_vit_progress, m_VitEffect, 0.05f, 0.5f);
            }
        }

        private void ReflashCash(int startCash, int endCash)
        {
            m_dollar_lab.SetChangeTextRoll(startCash, endCash);
        }

        /// <summary>
        /// 刷新玩家信息数据
        /// </summary>
        /// <param name="playerInfo"></param>
        private void RefreshPlayerInfo(PlayerInfo playerInfo)
        {
            this.m_health_lab.Text = playerInfo.Vit.ToString();
            this.m_gold_lab.Text = playerInfo.Coin.ToString();
            this.m_dollar_lab.Text = playerInfo.Cash.ToString();
            ReflashVitEffect(playerInfo.Vit);
            this.m_playerInfoComponent.RefreshPlayerInfo(playerInfo);
        }

        private void OnCountDownVit(float second)
        {
            this.m_CountDownVit_Root.Visible = (second != 0);
            if (second > 0)
            {
                this.m_CountDownVit_Lab.Text = CommonTools.SecondToStringDDMMSS(second);
                float vitPercent = 1f - second / CommonData.MillisRecoverOneVit;
                this.m_ProgressVit.FillAmmount = vitPercent;
            }
        }

        private void OnNewEmail()
        {
            //m_mail_btn.SetRedPoint(true);
        }


        public override void OnHide()
        {
            base.OnHide();
            GameEntryCommonManager.Instance.m_toPareaTweenerComplete = false;
            GameEvents.UIEvents.UI_GameEntry_Event.OnCountDownVit -= OnCountDownVit;
            PlayerInfoManager.OnPlayerInfoUpdatedEvent -= RefreshPlayerInfo;
            GameEvents.RedPointEvents.User_OnNewEmailEvent -= OnNewEmail;
            GameEvents.UIEvents.UI_GameEntry_Event.OnOpenPanel -= OnOpenPanel;
            GameEvents.UIEvents.UI_GameEntry_Event.OnInfiniteVit -= OnInfiniteVit;
            GameEvents.UIEvents.UI_Common_Event.OnCommonUIVisible -= OnCommonUIVisible;
            this.m_addHealth_btn.RemoveClickCallBack(OnBtnAddVitClick);
            this.m_btnDollar_btn.RemoveClickCallBack(OnBtnAddCashClick);
            this.m_btnGold_btn.RemoveClickCallBack(OnBtnAddCoinClick);
            GameEvents.UIEvents.UI_GameEntry_Event.OnReflashCoin -= ReflashCoin;
            GameEvents.UIEvents.UI_GameEntry_Event.OnReflashCash -= ReflashCash;
            GameEvents.UIEvents.UI_GameEntry_Event.OnReflashVit -= OnReflashVit;
            GameEvents.UI_Guid_Event.OnMainIconUnLockComplete -= OnMainIconUnLockComplete;
            m_VitEffect.Visible = false;
        }

        public override void Update()
        {
            base.Update();
            m_gold_lab.OnUpdate();
            m_dollar_lab.OnUpdate();
        }

        private void OnInfiniteVit(float time)
        {
            if (time > 0)
            {
                this.m_vitFreeImg.Visible = true;
            }
            else
            {
                this.m_vitFreeImg.Visible = false;
            }
        }

        private void OnCommonUIVisible(bool visible)
        {
            if (this.m_panelTop.CachedVisible != visible)
                this.m_panelTop.Visible = visible;
        }
        /// <summary>
        /// 玩家信息UI
        /// </summary>
        private class PlayerInfoUIComponent : GameUIComponent
        {
            //private GameLabel m_lbPlayerNickName = null;
            private GameImage m_imgPlayerIcon = null;
            private GameNetworkRawImage m_playerNetIcon;
            private GameLabel m_lbPlayerLevel = null;
            //private GameProgressBar m_pbExp = null;
            private GameUIEffect m_levelEffect = null;
            private GameImage m_RedDot = null;
            private GameImage m_playerBtn = null;
            protected override void OnInit()
            {
                this.m_playerBtn = Make<GameImage>("icon_btn");
                this.m_imgPlayerIcon = this.m_playerBtn.Make<GameImage>("icon");
                this.m_playerNetIcon = this.m_playerBtn.Make<GameNetworkRawImage>("RawImage_icon");
                //this.m_lbPlayerNickName = Make<GameLabel>("name");
                this.m_lbPlayerLevel = Make<GameLabel>("Text_number");
                //this.m_pbExp = Make<GameProgressBar>("level");
                //this.m_pbExp.Value = 0;
                this.m_RedDot = Make<GameImage>("redDot");
                this.m_levelEffect = Make<GameUIEffect>("UI_zhanghaodengji");
                m_levelEffect.EffectPrefabName = "UI_zhanghaodengji.prefab";
                this.m_levelEffect.Visible = false;
            }

            public override void OnShow(object param)
            {
                this.m_playerBtn.AddClickCallBack(OnPlayerInfoClick);
                //m_playerNetIcon.AddClickCallBack(OnPlayerInfoClick);
                //this.m_pbExp.OnValueChanged += OnProgressbarValueChanged;
                GameEvents.UIEvents.UI_GameEntry_Event.OnCloseNoticRedDot += OnCloseNoticRedDot;
                RefreshPlayerInfo(GlobalInfo.MY_PLAYER_INFO);
                this.m_RedDot.Visible = LocalDataManager.Instance.m_HasNotic;
                this.m_levelEffect.Visible = false;

            }

            private void OnCloseNoticRedDot()
            {
                this.m_RedDot.Visible = false;
            }

            //private void OnProgressbarValueChanged(float value)
            //{
            //    Vector3 topLeftConner = this.m_pbExp.FillRectangleWorldConners[2];
            //    Vector3 bottomLeftConner = this.m_pbExp.FillRectangleWorldConners[3];

            //    Vector3 centerPos = new Vector3(topLeftConner.x, (topLeftConner.y + bottomLeftConner.y) / 2, topLeftConner.z);
            //    m_levelEffect.Position = centerPos;
            //    m_levelEffect.Visible = true;
            //}


            public void RefreshPlayerInfo(PlayerInfo playerInfo)
            {

                if (!string.IsNullOrEmpty(playerInfo.PlayerIcon))
                {
                    if (CommonTools.IsNeedDownloadIcon(playerInfo.PlayerIcon))
                    {
                        this.m_imgPlayerIcon.Visible = false;
                        this.m_playerNetIcon.Visible = true;
                        this.m_playerNetIcon.TextureName = playerInfo.PlayerIcon;
                    }
                    else
                    {
                        this.m_imgPlayerIcon.Visible = true;
                        this.m_playerNetIcon.Visible = false;
                        this.m_imgPlayerIcon.Sprite = CommonData.GetLitterHEAD(playerInfo.PlayerIcon);
                    }
                }
                else
                {
                    this.m_imgPlayerIcon.Visible = true;
                    this.m_playerNetIcon.Visible = false;
                }

                this.m_lbPlayerLevel.Text = string.Format("LV:{0}", playerInfo.Level);

                int currentMaxLevel = Confetl.array.Count;
                if (playerInfo.Level == currentMaxLevel + 1)
                {
                    //this.RefreshExp(1);
                }
                else
                {
                    float nextLevelExp = Confetl.array.Find(conf => conf.level == playerInfo.Level).exp;
                    float currentDeltaExp = nextLevelExp - playerInfo.UpgradeExp;

                    //this.RefreshExp(currentDeltaExp / nextLevelExp);
                }
            }

            private void RefreshExp(float val_)
            {
                //if( MathUtil.FloatEqual( val_, 0.0f))
                //{
                //    val_ = 0.025f;
                //}
                //if (!MathUtil.FloatEqual(this.m_pbExp.Value, val_) && val_ != 0)
                //{
                //    DOTween.To(() => this.m_pbExp.Value, x => this.m_pbExp.Value = x, val_, 0.5f).OnUpdate(()=> {
                //        m_levelEffect.Visible = true;
                //        Vector3 topRightConner = this.m_pbExp.FillRectangleWorldConners[2];
                //        Vector3 bottomRightConner = this.m_pbExp.FillRectangleWorldConners[3];
                //        Vector3 centerPos = new Vector3(topRightConner.x, (topRightConner.y + bottomRightConner.y) / 2, topRightConner.z);
                //        m_levelEffect.Position = centerPos;

                //    }).OnComplete(()=> {
                //        m_levelEffect.Visible = false;
                //    });
                //    //CommonHelper.UpdateEffectPosByProgressbar(this.m_pbExp, m_levelEffect, 0.05f, 0.5f);
                //}
            }


            public override void OnHide()
            {
                base.OnHide();

                //this.m_pbExp.OnValueChanged -= OnProgressbarValueChanged;
                this.m_playerBtn.RemoveClickCallBack(OnPlayerInfoClick);
                //m_playerNetIcon.RemoveClickCallBack(OnPlayerInfoClick);
                GameEvents.UIEvents.UI_GameEntry_Event.OnCloseNoticRedDot -= OnCloseNoticRedDot;
                this.m_levelEffect.Visible = false;

            }


            private void OnPlayerInfoClick(GameObject imgPlayerIcon)
            {
                if (GlobalInfo.GAME_NETMODE == GameNetworkMode.Standalone)
                {
                    return;
                }
                GameEvents.BigWorld_Event.OnClickScreen.SafeInvoke();
                EngineCoreEvents.AudioEvents.PlayAudio.SafeInvoke(Audio.AudioType.UISound, GameCustomAudioKey.role_open.ToString());
                EngineCoreEvents.UIEvent.ShowUIEvent.SafeInvoke(UIDefine.UI_PLAYER_INFO);
            }

        }
    }
}