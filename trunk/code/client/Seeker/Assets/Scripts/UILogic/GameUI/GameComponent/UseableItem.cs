using SeekerGame.NewGuid;
using System.Collections.Generic;
using EngineCore;
using UnityEngine;
namespace SeekerGame
{
    /// <summary>
    /// 道具
    /// </summary>
    public class UseableItem : GameUIComponent
    {
        private GameButton m_btnItem;
        private GameImage m_imgItem;
        private GameLabel m_lbRemain;
        private GameUIEffect m_effect;
        private GameFilledImage m_fillImage;
        private long m_propId;
        private long m_skillId;
        private int m_keepPropNum = 0;
        private ConfSkill m_confSKill = null;
        private GameUIEffect m_propUseEffect = null;
        private GameUIEffect m_tishi_02Effect = null; //强烈提示

        private bool m_isArrowTips = false;
        private GameImage m_arrowImg = null;
        private bool m_isforbid = false; //是否禁用

        private ScenePropHintComponent m_wordHintCom = null;

        private GameImage m_LockImg = null;
        public bool EnableItem
        {
            set
            {
                m_btnItem.Enable = value;
            }
        }

        protected override void OnInit()
        {
            this.m_btnItem = Make<GameButton>("Button_3");
            this.m_imgItem = Make<GameImage>("Button_3");
            this.m_lbRemain = Make<GameLabel>("Button_3:Image:Text");
            this.m_effect = Make<GameUIEffect>("Button_3:UI_xunzhao");
            this.m_fillImage = Make<GameFilledImage>("Button_3:FillImage");
            this.m_propUseEffect = this.m_imgItem.Make<GameUIEffect>("UI_daoju_tishi");
            this.m_arrowImg = Make<GameImage>("Arrow");
            this.m_tishi_02Effect = this.m_imgItem.Make<GameUIEffect>("UI_daoju_tishi02");
            this.m_LockImg = Make<GameImage>("Lock");
        }

        public override void OnShow(object param)
        {
            base.OnShow(param);
            this.m_isCD = false;

            this.m_btnItem.AddClickCallBack(BtnPropItem);
            GameEvents.Skill_Event.OnSkillFinish += OnSkillFinish;
            GameEvents.Skill_Event.OnSkillCDOver += OnSkillReset;
            GameEvents.MainGameEvents.OnGameStatusChange += OnGameStatusChange;
            GameEvents.MainGameEvents.OnPropUseTips += OnPropUseTips;
            GameEvents.MainGameEvents.OnGuidPropUseTips += OnGuidPropUseTips;
            GameEvents.MainGameEvents.OnForbidProp += OnForbidProp;
            GameEvents.MainGameEvents.OnAlwaysForbidProp += OnAlwaysForbidProp;
            this.m_LockImg.Visible = false;
            this.m_effect.EffectPrefabName = "UI_xunzhao.prefab";
            this.m_effect.Visible = false;
            this.m_arrowImg.Visible = false;
            this.m_tishi_02Effect.Visible = false;
            //this.m_effect.vi
            this.m_propUseEffect.EffectPrefabName = "UI_daoju_tishi.prefab";
            this.m_propUseEffect.Visible = false;
        }

        private void OnGameStatusChange(SceneBase.GameStatus status)
        {
            this.m_fillImage.SetGameStatus(status);
        }

        private void OnPropUseTips(long propID, int type, int continueTime)
        {
            if (0 == type)
            {
                if (propID <= 0)
                {
                    return;
                }
                if (propID == m_propId)
                {
                    this.m_propUseEffect.Visible = true;
                    this.m_propUseEffect.ReplayEffect();
                    this.m_propUseEffect.SetEffectHideTime(4f);
                }
            }
            else if (1 == type && m_isArrowTips && !this.m_arrowImg.Visible)
            {
                this.m_arrowImg.Visible = true;
                if (continueTime > 0)
                {
                    TimeModule.Instance.SetTimeout(OnHideArrow, continueTime);
                }
            }
            else if (2 == type && needForceTips)
            {
                PlayerPropMsg propMsg = GlobalInfo.MY_PLAYER_INFO.GetBagInfosByID(this.m_propId);
                if (propMsg != null && propMsg.Count > 0)
                {
                    this.m_tishi_02Effect.EffectPrefabName = "UI_daoju_tishi02.prefab";
                    this.m_tishi_02Effect.Visible = true;
                    this.m_wordHintCom.SetContent(LocalizeModule.Instance.GetString(ConfProp.Get(m_propId).description));
                    this.m_wordHintCom.Visible = true;
                }
            }
        }

        private void OnGuidPropUseTips(long propID, int continueTime)
        {
            if (propID == m_propId)
            {
                this.m_arrowImg.Visible = true;
                if (continueTime > 0)
                {
                    TimeModule.Instance.SetTimeout(OnHideArrow, continueTime);
                }
            }
        }

        private void OnHideArrow()
        {
            GameEvents.MainGameEvents.OnResetArrowTipsTime.SafeInvoke();
            TimeModule.Instance.RemoveTimeaction(OnHideArrow);
            this.m_arrowImg.Visible = false;
        }

        public override void OnHide()
        {
            base.OnHide();
            this.m_btnItem.RemoveClickCallBack(BtnPropItem);
            GameEvents.Skill_Event.OnSkillFinish -= OnSkillFinish;
            GameEvents.Skill_Event.OnSkillCDOver -= OnSkillReset;
            GameEvents.MainGameEvents.OnGameStatusChange -= OnGameStatusChange;
            GameEvents.MainGameEvents.OnPropUseTips -= OnPropUseTips;
            GameEvents.MainGameEvents.OnGuidPropUseTips -= OnGuidPropUseTips;
            GameEvents.MainGameEvents.OnForbidProp -= OnForbidProp;
            GameEvents.MainGameEvents.OnAlwaysForbidProp -= OnAlwaysForbidProp;
            this.m_effect.Visible = false;
            this.m_propUseEffect.Visible = false;
            this.m_isforbid = false;
            this.m_isCD = false;
            TimeModule.Instance.RemoveTimeaction(OnHideArrow);
            OnForbidProp(-1, false);
        }

        public void BtnPropItem(GameObject obj)
        {
            this.m_arrowImg.Visible = false;
            if (m_isArrowTips)
            {
                OnHideArrow();
            }
            if (m_isCD || m_isforbid)
            {
                return;
            }
            //新手引导无限使用
            if (GuidNewNodeManager.Instance.GetCommonParams(GuidNewNodeManager.PropFree).Equals(this.m_propId.ToString()))
            {
                StartReleaseSkill();
                return;
            }

            PlayerPropMsg propMsg = GlobalInfo.MY_PLAYER_INFO.GetBagInfosByID(this.m_propId);
            //直接购买
            if (propMsg == null || propMsg.Count <= 0)
            {
                if (GlobalInfo.GAME_NETMODE == GameNetworkMode.Standalone)
                {
                    return;
                }
                EngineCoreEvents.AudioEvents.PlayAudio.SafeInvoke(Audio.AudioType.UISound, EngineCommonAudioKey.Button_Click_Common.ToString());
                (LogicHandler as GameMainUILogic).OnRequestBuyPropInfoEvent(this.m_propId);
            }
            else
            {
                if (NewGuid.GuidNewNodeManager.Instance.GetNodeStatus(NewGuid.GuidNewNodeManager.ForbidPropUse) == NodeStatus.None)
                {
                    return;
                }
                StartReleaseSkill();
                this.m_tishi_02Effect.Visible = false;
                this.m_wordHintCom.Visible = false;
            }
        }

        private void StartReleaseSkill()
        {
            if (m_skillId < 0)
            {
                EngineCoreEvents.AudioEvents.PlayAudio.SafeInvoke(Audio.AudioType.UISound, EngineCommonAudioKey.Button_Click_Common.ToString());
                return;
            }

            ConfSkill confSkill = ConfSkill.Get(m_skillId);
            if (confSkill == null)
            {
                EngineCoreEvents.AudioEvents.PlayAudio.SafeInvoke(Audio.AudioType.UISound, EngineCommonAudioKey.Button_Click_Common.ToString());
                return;
            }

            GameSkillManager.Instance.OnStartSkill(m_propId);

            //Dictionary<UBSParamKeyName, object> internalBuyItemKeypoint = new Dictionary<UBSParamKeyName, object>();
            //internalBuyItemKeypoint.Add(UBSParamKeyName.PropItem_ID, m_propId);
            //internalBuyItemKeypoint.Add(UBSParamKeyName.PropItem_Num, 1);

            //UserBehaviorStatisticsModules.Instance.LogEvent(UBSEventKeyName.game_use_propitem, 1, internalBuyItemKeypoint);
        }

        public void SetData(long id, GameMainArrowTipData arrowData)
        {
            if (arrowData != null)
            {
                this.m_isArrowTips = (arrowData.propId == id);
            }
            this.m_propId = id;
            ConfProp confProp = ConfProp.Get(id);
            if (confProp == null)
            {
                Visible = false;
            }
            this.m_skillId = confProp.skillId;
            this.m_confSKill = ConfSkill.Get(this.m_skillId);
            this.m_imgItem.Sprite = confProp.icon;
            PlayerPropMsg propMsg = GlobalInfo.MY_PLAYER_INFO.GetBagInfosByID(id);
            this.m_keepPropNum = propMsg == null ? 0 : propMsg.Count;

            if (GuidNewNodeManager.Instance.GetCommonParams(GuidNewNodeManager.PropFree).Equals(this.m_propId.ToString()))
            {
                this.m_lbRemain.Text = "free";
                return;
            }

            if (propMsg == null || propMsg.Count <= 0)
            {
                this.m_lbRemain.Text = "+";
                this.m_imgItem.SetGray(true);
                //this.CanClick = false;
                //this.m_btnItem.Enable = false;
            }
            else
            {
                this.m_lbRemain.Text = propMsg.Count.ToString();
                this.m_propId = id;

                //this.CanClick = true;
                //this.m_btnItem.Enable = true;
                this.m_imgItem.SetGray(false);
            }
            //ReflashBtnEnable();
        }

        private void OnSkillFinish(long carryId)
        {
            ConfProp confprop = ConfProp.Get(carryId);
            if (confprop == null)
            {
                return;
            }
            ConfSkill releaseSkill = ConfSkill.Get(confprop.skillId);
            if (releaseSkill == null)
            {
                return;
            }
            if (carryId == this.m_propId)
            {
                m_fillImage.FillTime = this.m_confSKill.cd;
                if (GuidNewNodeManager.Instance.GetCommonParams(GuidNewNodeManager.PropFree) == this.m_propId.ToString())
                {
                    ResetSkill();
                    return;
                }
                GlobalInfo.MY_PLAYER_INFO.ReducePropForBag(this.m_propId);
                PlayerPropMsg propMsg = GlobalInfo.MY_PLAYER_INFO.GetBagInfosByID(this.m_propId);
                if (propMsg == null)
                {
                    this.m_lbRemain.Text = "0";
                }
                else
                {
                    this.m_lbRemain.Text = propMsg.Count.ToString();
                }
                ResetSkill();


                Dictionary<UBSParamKeyName, object> internalBuyItemKeypoint = new Dictionary<UBSParamKeyName, object>();
                internalBuyItemKeypoint.Add(UBSParamKeyName.Description, UBSDescription.PROPUSE);
                internalBuyItemKeypoint.Add(UBSParamKeyName.PropItem_ID, carryId);
                internalBuyItemKeypoint.Add(UBSParamKeyName.PropItem_Num, 1);
                UserBehaviorStatisticsModules.Instance.LogEvent(UBSEventKeyName.game_propuse, 1, internalBuyItemKeypoint);

                //this.CanClick = false;
                //this.m_btnItem.Enable = false;
            }
            else if (IsFunctionProp(releaseSkill))
            {
                if (IsFunctionProp(this.m_confSKill) || this.m_lbRemain.Text.Equals("+"))
                {
                    m_fillImage.FillTime = releaseSkill.cd;
                    this.m_isCD = true;
                }
            }
        }

        private void ResetSkill()
        {
            this.m_isCD = true;
            this.m_effect.ReplayEffect();
            this.m_effect.SetEffectHideTime(1.5f);
        }

        private void OnSkillReset(long carryId)
        {
            ConfProp confprop = ConfProp.Get(carryId);
            if (confprop == null)
            {
                return;
            }
            ConfSkill releaseSkill = ConfSkill.Get(confprop.skillId);
            if (releaseSkill == null)
            {
                return;
            }
            if (carryId == this.m_propId)
            {
                if (GuidNewNodeManager.Instance.GetCommonParams(GuidNewNodeManager.PropFree) == this.m_propId.ToString())
                {
                    this.m_isCD = false;
                    return;
                }
                PlayerPropMsg propMsg = GlobalInfo.MY_PLAYER_INFO.GetBagInfosByID(this.m_propId);
                if (propMsg != null)
                {
                    //this.m_canClick = true;
                    //this.m_btnItem.Enable = true;
                    this.m_imgItem.SetGray(false);
                }
                else
                {
                    //this.CanClick = false;
                    this.m_lbRemain.Text = "+";
                    this.m_imgItem.SetGray(true);
                    //this.m_btnItem.Enable = false;
                }
                this.m_isCD = false;
            }
            else if (IsFunctionProp(releaseSkill))
            {
                if (IsFunctionProp(this.m_confSKill) || this.m_lbRemain.Text.Equals("+"))
                {
                    this.m_isCD = false;
                }
            }
        }

        private bool IsFunctionProp(ConfSkill skill)
        {
            if (skill.type == 11 || skill.type == 12)
            {
                return true;
            }
            return false;
        }

        public void SetScenePropHintCom(ScenePropHintComponent propHint)
        {
            this.m_wordHintCom = propHint;
        }
        /// <summary>
        /// 禁用道具
        /// </summary>
        /// <param name="propId"></param>
        /// <param name="flag"></param>
        private void OnForbidProp(long propId, bool flag)
        {
            if (propId <= 0 || propId == m_propId)
            {
                m_isforbid = flag;
                return;
            }
        }

        private void OnAlwaysForbidProp(long propId, bool flag)
        {
            if (propId == m_propId)
            {
                this.m_LockImg.Visible = flag;
            }
        }

        private bool m_isCD = false;


        public void ReflashBtnEnable()
        {
            this.m_btnItem.Enable = m_canClick & m_outClick;
            //if (SeekerGame.NewGuid.GuidNewManager.Instance.GetProgressByIndex(3))
            //{

            //}
        }

        private bool m_canClick = true;


        private bool m_outClick = true;
        public bool OutClick
        {
            get { return m_outClick; }
            set
            {
                m_outClick = value;
                ReflashBtnEnable();
            }
        }

        public long ItemID => this.m_propId;

        private bool needForceTips = false;
        public bool NeedForceTips
        {
            get
            {
                return needForceTips;
            }
            set { needForceTips = value; }
        }
    }
}
