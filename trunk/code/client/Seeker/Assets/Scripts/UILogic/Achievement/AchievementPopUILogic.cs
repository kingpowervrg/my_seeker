using EngineCore;
using GOGUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace SeekerGame
{
    [UILogicHandler(UIDefine.UI_ACHIEVEMENT_POP)]
    public class AchievementPopUILogic : UILogicBase
    {
        private GameLabel m_Name_lab;
        private GameLabel m_Desc_lab;
        private GameButton m_Close_btn;
        private GameLabel m_Title_lab; //称号
        private GameImage m_BG_img;
        private AchievementPopItem m_PopItem01;
        private AchievementPopItem m_PopItem02;
        private AchievementPopItem m_PopItem03;
        private GameLabel m_titleContentLab = null;
        //private UITweenerBase[] m_tweener = null;

        private bool m_canVisible = false;
        protected override void OnInit()
        {
            base.OnInit();
            m_BG_img = Make<GameImage>("Image");
            //m_Name_lab = Make<GameLabel>("name");
            //m_Desc_lab = Make<GameLabel>("desc");
            m_Title_lab = Make<GameLabel>("Panel_animation:title");
            m_Close_btn = Make<GameButton>("Panel_animation:Button_close");
            m_PopItem01 = Make<AchievementPopItem>("Panel_animation:Image_1");
            m_PopItem02 = Make<AchievementPopItem>("Panel_animation:Image_2");
            m_PopItem03 = Make<AchievementPopItem>("Panel_animation:Image_3");
            this.m_titleContentLab = Make<GameLabel>("Panel_animation:title:state");
            //this.m_tweener = Transform.GetComponentsInChildren<UITweenerBase>();
        }

        public override void OnShow(object param)
        {
            base.OnShow(param);
            m_Close_btn.AddClickCallBack(OnClose);
            m_BG_img.AddClickCallBack(OnClose);
            this.m_canVisible = true;
            this.OnShowTweenFinished = ShowTweenFinished;
            if (param != null)
            {
                AchievementMsg msg = (AchievementMsg)param;
                SetData(msg);
            }
        }
        public override FrameDisplayMode UIFrameDisplayMode
        {
            get
            {
                return FrameDisplayMode.WINDOWED;
            }
        }
        private void ShowTweenFinished()
        {
            this.m_PopItem01.OnPlayEffect();
            this.m_PopItem02.OnPlayEffect();
            this.m_PopItem03.OnPlayEffect();
            this.m_canVisible = true;
        }

        public override void OnHide()
        {
            base.OnHide();
            m_Close_btn.RemoveClickCallBack(OnClose);
            m_BG_img.RemoveClickCallBack(OnClose);
            TimeModule.Instance.RemoveTimeaction(() => { OnClose(); });
        }

        public void SetData(AchievementMsg msg)
        {
            ConfAchievement confItem = ConfAchievement.Get(msg.Id);
            //m_Name_lab.Text = LocalizeModule.Instance.GetString(confItem.name);
            //m_Desc_lab.Text = LocalizeModule.Instance.GetString(confItem.info);
            long titleID = -1;
            long.TryParse(confItem.nobility, out titleID);

            if (titleID < 0)
            {
                m_Title_lab.Visible = false;
            }
            else
            {
                ConfTitle confTitle = ConfTitle.Get(titleID);
                if (confTitle != null)
                {
                    m_Title_lab.Visible = true;
                    m_Title_lab.Text = LocalizeModule.Instance.GetString("cheng_jiu_3", LocalizeModule.Instance.GetString(confTitle.name));
                    this.m_titleContentLab.Text = LocalizeModule.Instance.GetString(confTitle.info);
                }
                else
                {
                    m_Title_lab.Visible = false;
                }
            }
            m_PopItem01.SetData(msg, confItem.progress1, confItem.rewardicon1, confItem.reward1, confItem.cash1, 0);
            m_PopItem02.SetData(msg, confItem.progress2, confItem.rewardicon2, confItem.reward2, confItem.cash1, 1);
            m_PopItem03.SetData(msg, confItem.progress3, confItem.rewardicon3, confItem.reward3, confItem.cash1, 2);

        }

        private void OnClose(GameObject obj)
        {
            if (!m_canVisible)
            {
                return;
            }
            this.m_PopItem01.OnPlayEffect(false);
            this.m_PopItem02.OnPlayEffect(false);
            this.m_PopItem03.OnPlayEffect(false);
            OnClose();
            //for (int i = 0; i < this.m_tweener.Length; i++)
            //{
            //    if (this.m_tweener[i].style == UITweenerBase.Style.OnHide || this.m_tweener[i].style == UITweenerBase.Style.OnShow)
            //    {
            //        this.m_tweener[i].PlayBackward();
            //    }
            //}
            //TimeModule.Instance.SetTimeout(() => { OnClose(); }, 0.6f);
            //OnClose();
        }

        private void OnClose()
        {
            EngineCoreEvents.AudioEvents.PlayAudio.SafeInvoke(Audio.AudioType.UISound, EngineCommonAudioKey.Close_Window.ToString());
            EngineCoreEvents.UIEvent.HideUIEvent.SafeInvoke(UIDefine.UI_ACHIEVEMENT_POP);
        }
    }

    public class AchievementPopItem : GameUIComponent
    {
        private GameImage m_Icon_img;
        private GameImage m_BigLock_img;
        private GameImage m_SmallLock_img;

        private GameLabel m_Time_lab;
        private GameLabel m_Term_lab; //条件
        private GameLabel m_Percent01_lab; //比例
        private GameLabel m_Percent02_lab;

        private GameLabel m_Achieve_lab;
        private GameLabel m_Cash_lab;
        private GameImage m_RewardBG_img; //奖励边框

        private GameButton m_Receive_btn;
        private GameImage m_Receive_img;

        private GameUIEffect m_lockEffect = null;
        private Color m_NormalRewardBG_color = new Color(0.149f, 0.556f, 1f); //默认边框颜色
        private Color m_GrayColor_color = new Color(0.541f, 0.611f, 0.686f); //边框置灰颜色
        private Color m_PercentColor = new Color(0f, 0.67f, 0.09f); //成就未解锁百分号颜色
        private Color m_SmallLockColor_color = new Color(0.827f, 0.827f, 0.905f); //小锁置灰颜色
        private Color m_CashGray_color = new Color(0.494f, 0.494f, 0.494f); //奖励置灰颜色
        private Color m_NormalReward01_color = new Color(0.615f, 0.29f, 0f);
        private Color m_NormalReward02_color = new Color(0.109f, 0.474f, 0f);
        private Color m_NormalText_color = new Color(0.074f, 0.258f, 0.396f);

        private Color m_NumberGreenColor = new Color(0.078f, 0.474f, 0f);
        private Color m_NumberRedColor = new Color(0.788f, 0.196f, 0.196f);
        private int[] m_ShiftFactor = { 2, 4, 8 }; //位运算因子

        private long m_Id = -1;
        private int m_Stage = -1;
        private bool m_hasReceive = false;

        private int m_submit = -1;

        private int m_Reward;
        private int m_Cash;
        private bool m_needShowEffect = false;
        protected override void OnInit()
        {
            base.OnInit();
            m_Icon_img = Make<GameImage>("RawImage");
            m_BigLock_img = Make<GameImage>("Image_lock");
            m_SmallLock_img = Make<GameImage>("Image:Image");
            m_Time_lab = Make<GameLabel>("Text_time");
            m_Term_lab = Make<GameLabel>("Text_1");
            m_Percent01_lab = Make<GameLabel>("Text_2");
            m_Percent02_lab = Make<GameLabel>("Text_3");

            m_Achieve_lab = Make<GameLabel>("Image:Image_1:Text");
            m_Cash_lab = Make<GameLabel>("Image:Image_2:Text");
            m_Receive_btn = Make<GameButton>("btn_1");
            m_Receive_img = Make<GameImage>("Image_received");
            m_RewardBG_img = Make<GameImage>("Image");
            this.m_lockEffect = Make<GameUIEffect>("UI_chengjiu");
        }

        public override void OnShow(object param)
        {
            base.OnShow(param);
            m_Receive_btn.AddClickCallBack(OnGetAchieve);
            MessageHandler.RegisterMessageHandler(MessageDefine.SCAchievementSubmitResponse, OnResponse);
        }

        public override void OnHide()
        {
            base.OnHide();
            m_Receive_btn.RemoveClickCallBack(OnGetAchieve);
            MessageHandler.UnRegisterMessageHandler(MessageDefine.SCAchievementSubmitResponse, OnResponse);
            m_Id = -1;
            this.m_lockEffect.Visible = false;
            this.m_needShowEffect = false;
        }

        public void SetData(AchievementMsg msg, int progress, string rewardicon, int reward, int cash, int index)
        {
            if (msg == null)
            {
                Visible = false;
                return;
            }
            this.m_needShowEffect = false;
            m_Id = msg.Id;
            m_Stage = index;
            m_Reward = reward;
            m_Cash = cash;
            //m_submit = msg.SubmitStatus;
            ConfAchievement confItem = ConfAchievement.Get(msg.Id);
            if (confItem == null)
            {
                Visible = false;
                return;
            }
            long finishTime = msg.FinishTime1;
            if (index == 1)
            {
                finishTime = msg.FinishTime2;
            }
            else if (index == 2)
            {
                finishTime = msg.FinishTime;
            }
            DateTime dt = CommonTools.TimeStampToDateTime(finishTime * 10000);
            if (dt != null)
            {
                m_Time_lab.Text = string.Format("{0:D2}.{1:D2}.{2:D2}", dt.Year, dt.Month, dt.Day);
            }
            this.m_lockEffect.Visible = false;
            m_Icon_img.Sprite = rewardicon;
            if (msg.Progress < progress)
            {
                //未解锁
                m_Icon_img.Color = new Color(0.494f, 0.494f, 0.494f, 0.549f);
                m_BigLock_img.Visible = true;
                m_SmallLock_img.Sprite = "icon_lock_2.png";
                m_SmallLock_img.Color = m_SmallLockColor_color;
                m_RewardBG_img.Color = m_GrayColor_color;

                m_Time_lab.Visible = false;
                m_Term_lab.Text = LocalizeModule.Instance.GetString("Achievement_" + confItem.type);//"Achievement_" + confItem.type
                m_Percent01_lab.Text = msg.Progress.ToString();
                m_Percent01_lab.color = m_NumberRedColor;//m_PercentColor;
                m_Percent02_lab.Text = string.Format("/{0}", progress);
                m_Percent02_lab.color = m_NumberGreenColor;
                m_Achieve_lab.Text = reward.ToString();
                m_Cash_lab.Text = cash.ToString();
                //m_Achieve_lab.color = m_CashGray_color;
                //m_Cash_lab.color = m_CashGray_color;
                m_Achieve_lab.color = m_NormalReward01_color;
                m_Cash_lab.color = m_NormalReward01_color;
                m_Receive_btn.Visible = false;
                m_Receive_img.Visible = false;

            }
            else
            {
                int submit = msg.SubmitStatus & m_ShiftFactor[index];
                //m_Percent02_lab.color = new Color(0.074f,0.258f,0.396f);
                if (0 == submit)
                {
                    this.m_needShowEffect = true;
                    this.m_lockEffect.EffectPrefabName = "UI_chengjiu0" + (index + 1) + ".prefab";
                    //TimeModule.Instance.SetTimeout(OnPlayEffect,1.8f);
                    //this.m_lockEffect.Visible = true;
                    //未领取
                    m_Icon_img.Color = Color.white;
                    m_BigLock_img.Visible = false;
                    m_SmallLock_img.Sprite = "icon_giftbox_1.png";
                    m_SmallLock_img.Color = Color.white;
                    m_RewardBG_img.Color = m_NormalRewardBG_color;

                    m_Time_lab.Visible = true;
                    m_Term_lab.Text = LocalizeModule.Instance.GetString("Achievement_" + confItem.type); //confItem.name
                    m_Percent01_lab.Text = progress.ToString();
                    m_Percent01_lab.color = m_NumberGreenColor;//m_NormalText_color;
                    m_Percent02_lab.Text = string.Format("/{0}", progress);
                    m_Percent02_lab.color = m_NumberGreenColor;
                    m_Achieve_lab.Text = reward.ToString();
                    m_Cash_lab.Text = cash.ToString();
                    m_Achieve_lab.color = m_NormalReward01_color;
                    m_Cash_lab.color = m_NormalReward02_color;
                    //m_Receive_img.Sprite = "btn_common_3.png";
                    m_Receive_btn.Visible = true;
                    m_Receive_img.Visible = false;
                }
                else
                {
                    //已经领取
                    m_Icon_img.Color = Color.white;
                    m_BigLock_img.Visible = false;
                    m_SmallLock_img.Sprite = "icon_giftbox_1.png";
                    m_SmallLock_img.Color = m_SmallLockColor_color;
                    m_RewardBG_img.Color = m_GrayColor_color;

                    m_Time_lab.Visible = true;
                    m_Term_lab.Text = LocalizeModule.Instance.GetString("Achievement_" + confItem.type);
                    m_Percent01_lab.Text = progress.ToString();
                    m_Percent01_lab.color = m_NormalText_color;
                    m_Percent02_lab.Text = string.Format("/{0}", progress);
                    m_Percent02_lab.color = m_NormalText_color;
                    m_Achieve_lab.Text = reward.ToString();
                    m_Cash_lab.Text = cash.ToString();
                    m_Achieve_lab.color = m_CashGray_color;
                    m_Cash_lab.color = m_CashGray_color;
                    m_Receive_img.Visible = true;
                    m_Receive_btn.Visible = false;
                }
            }
        }

        private void HasGetStyle()
        {
            //已经领取
            m_Icon_img.Color = Color.white;
            m_BigLock_img.Visible = false;
            m_SmallLock_img.Sprite = "icon_lock_2.png";
            m_SmallLock_img.Color = m_SmallLockColor_color;
            m_RewardBG_img.Color = m_GrayColor_color;

            m_Time_lab.Visible = true;
            m_Percent01_lab.color = m_NormalText_color;
            m_Percent02_lab.color = m_NormalText_color;
            m_Achieve_lab.color = m_CashGray_color;
            m_Cash_lab.color = m_CashGray_color;
            m_Receive_img.Visible = true;
            m_Receive_btn.Visible = false;
        }

        private void OnGetAchieve(GameObject obj)
        {
            EngineCoreEvents.AudioEvents.PlayAudio.SafeInvoke(Audio.AudioType.UISound, GameCustomAudioKey.achievement_get.ToString());
            if (m_Id < 0 || m_Stage < 0)
            {
                return;
            }
            m_hasReceive = true;
            CSAchievementSubmitRequest req = new CSAchievementSubmitRequest();
            req.Id = m_Id;
            req.Stage = m_Stage + 1;

#if !NETWORK_SYNC || UNITY_EDITOR
            GameEvents.NetWorkEvents.SendHalfSyncMsg.SafeInvoke(req);
#else
            GameEvents.NetWorkEvents.SendMsg.SafeInvoke(req);
#endif


        }

        private void OnResponse(object obj)
        {
            if (obj is SCAchievementSubmitResponse && m_hasReceive)
            {
                m_hasReceive = false;
                SCAchievementSubmitResponse res = (SCAchievementSubmitResponse)obj;
                if (res.Status == null)
                {
                    this.m_lockEffect.Visible = false;
                    Dictionary<UBSParamKeyName, object> _params = new Dictionary<UBSParamKeyName, object>()
                    {
                        { UBSParamKeyName.ContentID,m_Id },
                        { UBSParamKeyName.ContentType,m_Stage }
                    };
                    UserBehaviorStatisticsModules.Instance.LogEvent(UBSEventKeyName.achievement_get, 1.0f, _params);
                    GameEvents.UIEvents.UI_Achievement_Event.OnReceiveData.SafeInvoke(m_Id, m_ShiftFactor[m_Stage]);
                    HasGetStyle();
                    GlobalInfo.MY_PLAYER_INFO.ChangeCash(m_Cash);
                    GlobalInfo.MY_PLAYER_INFO.AddAchieve(m_Reward);
                    //GameEvents.UIEvents.UI_Achievement_Event.OnReflashAchievement.SafeInvoke();
                }
            }
        }

        public void OnPlayEffect()
        {
            if (this.m_needShowEffect)
            {
                OnPlayEffect(true);
            }
        }

        public void OnPlayEffect(bool isPlay)
        {
            this.m_lockEffect.Visible = isPlay;
        }
    }
}
