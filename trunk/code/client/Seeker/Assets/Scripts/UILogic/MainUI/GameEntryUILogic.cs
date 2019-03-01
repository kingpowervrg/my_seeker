//#define TEST
using DG.Tweening;
using EngineCore;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SeekerGame
{
    [UILogicHandler(UIDefine.UI_GAMEENTRY)]
    public partial class GameEntryUILogic : UILogicBase
    {
        public static Vector3 S_UI_CHAPTER_BTN_POS;
        public static int EnterMainEnterTimes = 0;

        private GameButton[] m_entry_Tog; //底部入口按钮
        private TweenPosition m_entry_tog_show_tween_pos;

        private TweenRotationEuler m_menuBtnTweener = null;

        private TweenAlpha m_entry_tog_tween_alpha;

        private GameButton m_hide_btn;
        private GameImage m_btnSwitchRedPointMark = null;

        private Dictionary<string, GameImage> m_red_points; //红点标志
        private Dictionary<string, SafeAction> m_remove_red_points;//红点已读，去掉红点后的相关数据处理

        private GameImage[] m_entryBackground;


        private Color m_defaultColor = new Color(118f / 255f, 149f / 255f, 164f / 255f);
        private Color m_pressColor = Color.white;


        private GameButton m_activity_btn;
        private GameUIEffect m_activity_btn_effect;
        private GameImage m_ActivityRedPoint;
        //Tweener
        private Transform m_panelDown = null;


        //任务面板
        private PlayerTaskComponent m_playerTaskPanelComponent = null;
        private GameUIComponent m_taskPanel = null;

        private GameUIComponent m_activityUI = null;
        private GameUIComponent m_bottomUI = null;

        //推送礼包
        private GameButton m_push_gift_btn;
        private GameUIComponent m_push_gift_count_root;
        private GameLabel m_push_gift_count_txt;
        private GameLabel m_push_gift_left_time_txt;
        private GiftView m_push_gift_view;
        private GameUIEffect m_push_gift_btn_effect;

        private GameUIComponent m_maskBG = null;
        /// <summary>
        /// 合成提示
        /// </summary>
        CombineTipsView m_combine_tips;
#if OFFICER_SYS
        private GameUIEffect m_newPoliceEffect = null;
#endif
        private GameUIComponent m_bottomButtonComponent = null;
        private UnityEngine.UI.GridLayoutGroup m_gridComponent = null;
        //private GuidNewMainIconNewUILogic m_MainIcon = null;
#if OFFICER_SYS
        private string[] m_panelName = new string[] { UIDefine.UI_MAIL, UIDefine.UI_CHAPTER, UIDefine.UI_POLICE, UIDefine.UI_BAG, UIDefine.UI_ACHIEVEMENT, UIDefine.UI_SHOP, UIDefine.UI_FRIEND }; //面板名称
                    private int[] m_panelMsg = new int[] {
                                                  0
                                                ,MessageDefine.SCEventDropInfoResponse
                                                ,0
                                                ,MessageDefine.SCPlayerPropResponse
                                                ,0
                                                ,MessageDefine.MarketResponse
                                    ,MessageDefine.SCFriendResponse
                                                };
             private readonly int MAXPANEL = 7;
#else
        private string[] m_panelName = new string[] { UIDefine.UI_MAIL, UIDefine.UI_CHAPTER, UIDefine.UI_COMBINE, UIDefine.UI_BAG, UIDefine.UI_ACHIEVEMENT, UIDefine.UI_SHOP, UIDefine.UI_FRIEND }; //面板名称
        private int[] m_panelMsg = new int[] {
                                                  0
                                                ,0
                                                ,0
                                                ,MessageDefine.SCPlayerPropResponse
                                                ,0
                                                ,0
                                                ,MessageDefine.SCFriendResponse
                                                };
        private readonly int MAXPANEL = 7;
#endif


        private int m_currentIndex = -1;

        public static long S_CUR_EXCUTE_TASK_ID = 0L;

        protected override void OnInit()
        {
            base.OnInit();
            InitController();
        }

        public override FrameDisplayMode UIFrameDisplayMode
        {
            get
            {
                return FrameDisplayMode.WINDOWED;
            }
        }

        private void InitController()
        {
            m_entry_Tog = new GameButton[MAXPANEL];
            this.m_maskBG = Make<GameUIComponent>("MaskBG");
            //m_entry_tog_show_tween_pos = new List<TweenPosition>();
            //m_entry_tog_hide_tween_pos = new List<TweenPosition>();
            //m_entry_tog_tween_alpha = new List<TweenAlpha>();
            this.m_activityUI = Make<GameUIComponent>("ActivityAnimator");
            this.m_entryBackground = new GameImage[MAXPANEL];
            m_red_points = new Dictionary<string, GameImage>();
            this.m_bottomUI = Make<GameUIComponent>("Panel_down");
            this.m_bottomButtonComponent = Make<GameUIComponent>("Panel_down:mask:grid");
            this.m_gridComponent = this.m_bottomButtonComponent.Widget.GetComponent<UnityEngine.UI.GridLayoutGroup>();
            for (int i = 0; i < MAXPANEL; i++)
            {
                m_entry_Tog[i] = Make<GameButton>(string.Format("Panel_down:mask:grid:Toggle_{0}", i + 1));
#if OFFICER_SYS
                if (i == 2)
                {
                    m_newPoliceEffect = m_entry_Tog[i].Make<GameUIEffect>("newPolice");
                }
#endif
                GameImage img = m_entry_Tog[i].Make<GameImage>("ImgWarn");
                m_red_points[m_panelName[i]] = img;
                this.m_entryBackground[i] = m_entry_Tog[i].Make<GameImage>(m_entry_Tog[i].gameObject);
                btnEntry(i);

            }

            m_entry_tog_show_tween_pos = this.m_bottomButtonComponent.GetComponent<TweenPosition>();
            m_entry_tog_tween_alpha = this.m_bottomButtonComponent.GetComponent<TweenAlpha>();
            m_entry_tog_show_tween_pos.SetTweenCompletedCallback(() => EnableBottomButtons(true));

            m_hide_btn = Make<GameButton>("Panel_down:Button_hide");
            m_menuBtnTweener = m_hide_btn.GetComponent<TweenRotationEuler>();
            m_btnSwitchRedPointMark = m_hide_btn.Make<GameImage>("ImgWarn");

            m_playerTaskPanelComponent = Make<PlayerTaskComponent>("Panel_Task");
            this.m_taskPanel = m_playerTaskPanelComponent.Make<GameUIComponent>("Panel");
            m_activity_btn = Make<GameButton>("ActivityAnimator:Button_activities");
            m_activity_btn_effect = Make<GameUIEffect>("ActivityAnimator:Button_activities:UI_huodong_02");
            m_activity_btn_effect.EffectPrefabName = "UI_huodong_02.prefab";
            m_ActivityRedPoint = m_activity_btn.Make<GameImage>("ImgWarn");

#if OFFICER_SYS
            m_remove_red_points = new Dictionary<string, SafeAction>()
            {
                { m_panelName[0], GameEvents.RedPointEvents.Sys_OnNewEmailReadedEvent },
                { m_panelName[1], null },
                { m_panelName[2], null },
                { m_panelName[3], null },
                { m_panelName[4], GameEvents.RedPointEvents.Sys_OnNewAchievementReadedEvent },
                { m_panelName[5], null },
                { m_panelName[6], null /*GameEvents.RedPointEvents.Sys_OnNewFriendReadedEvent*/ },
            };
#else

            m_remove_red_points = new Dictionary<string, SafeAction>()
            {
                { m_panelName[0], GameEvents.RedPointEvents.Sys_OnNewEmailReadedEvent },
                { m_panelName[1], null },
                { m_panelName[2], null },
                { m_panelName[3],  null},
                { m_panelName[4], GameEvents.RedPointEvents.Sys_OnNewAchievementReadedEvent },
                { m_panelName[5], null},
                { m_panelName[6], null},
            };
#endif

            this.m_panelDown = this.Transform.Find("Panel_down");

            m_push_gift_btn = this.Make<GameButton>("ActivityAnimator:Button_activitiesgift");
            m_push_gift_count_root = m_push_gift_btn.Make<GameLabel>("ImgWarn");
            m_push_gift_count_txt = m_push_gift_btn.Make<GameLabel>("ImgWarn:Text");
            m_push_gift_left_time_txt = m_push_gift_btn.Make<GameLabel>("time");
            m_push_gift_view = this.Make<GiftView>("Panel_activitiesgift");

            m_push_gift_btn_effect = this.Make<GameUIEffect>("ActivityAnimator:Button_activitiesgift:UI_huodong_03");
            m_push_gift_btn_effect.EffectPrefabName = "UI_huodong_03.prefab";


            m_combine_tips = Make<CombineTipsView>("Image");
        }

        public override void OnShow(object param)
        {
            base.OnShow(param);
#if OFFICER_SYS
            LockBtn(5, !NewGuid.GuidNewManager.Instance.GetProgressByIndex(8));
            LockBtn(6, !NewGuid.GuidNewManager.Instance.GetProgressByIndex(10));
            LockBtn(1, !NewGuid.GuidNewManager.Instance.GetProgressByIndex(11));
#else
            LockBtn(4, !NewGuid.GuidNewManager.Instance.GetProgressByIndex(8));
            LockBtn(5, !NewGuid.GuidNewManager.Instance.GetProgressByIndex(10));
            LockBtn(1, !NewGuid.GuidNewManager.Instance.GetProgressByIndex(11));
#endif

#if OFFICER_SYS
            PoliceDispatchManager.Instance.PreloadLastestDispatchOffers();
#endif

            if (GameEntryCommonManager.Instance.m_needMainUITweener)
            {
                GameEntryCommonManager.Instance.m_needMainUITweener = false;
                GameEntryCommonManager.Instance.m_mainUITweenerComplete = true;
                SeekerGame.NewGuid.GuidNewManager.Instance.OnReflashGuidStatus();
            }
            if (GlobalInfo.GAME_NETMODE == GameNetworkMode.Standalone)
            {
                GameEntryCommonManager.Instance.m_mainUITweenerComplete = true;
            }
            this.m_panelDown.gameObject.SetActive(GlobalInfo.GAME_NETMODE == GameNetworkMode.Network);
            //GameEvent

            this.InitListener();
            this.m_currentIndex = -1;
            GameEntryHelper.m_IsFirst = true;

            GameEntryHelper.m_CurrentBtn = m_panelName[0];

            if (null != param)
            {
                string open_ui_name = param as string;
                GameEvents.UIEvents.UI_GameEntry_Event.OnOpenPanel.SafeInvoke(open_ui_name);
            }

            GameEvents.RedPointEvents.Sys_OnRefreshByPlayerPrefs.SafeInvoke();

            if (null != param)
            {
                string open_ui_name = param as string;

                if ("GiftView" == open_ui_name)
                {
                    this.m_cur_call_ui_name = open_ui_name;
                }
                else
                {
                    this.m_cur_call_ui_name = string.Empty;
                }
#if OFFICER_SYS
                if (!open_ui_name.Equals(UIDefine.UI_BAG) && !open_ui_name.Equals(UIDefine.UI_POLICE))
#else
                if (!open_ui_name.Equals(UIDefine.UI_BAG))
#endif
                //同步背包
                {
                    //GlobalInfo.MY_PLAYER_INFO.SyncPlayerBag();
                }
            }
            if (GlobalInfo.GAME_NETMODE == GameNetworkMode.Standalone)
            {
                this.m_activity_btn.Visible = false;
                this.m_push_gift_btn.Visible = false;
            }

            GameEvents.System_Events.PlayMainBGM(true);

            m_push_gift_view.Visible = false;
            EnterMainEnterTimes++;

            if (EnterMainEnterTimes == 1)
                GameEvents.UIEvents.UI_GameEntry_Event.OnFirstTimeEnterGame.SafeInvoke();


            m_push_gift_btn_effect.Visible = false;
            m_activity_btn_effect.Visible = true;

            m_is_bottom_btn_show = false;

            if (HaveNotice())
                m_btnSwitchRedPointMark.Visible = true;

            m_hide_btn.AddClickCallBack(ShowBottomButton);

            sm = new SignInManager();


        }

        private void btnEntry(int i)
        {
            m_entry_Tog[i].AddClickCallBack(delegate (GameObject flag)
            {
                EntryChoose(i, true);
            });
        }

        private void OnBtnDelayTime()
        {
            SetBtnVisible(false);
            TimeModule.Instance.SetTimeout(() =>
            {
                SetBtnVisible(true);
            }, 0.5f);
        }

        private void SetBtnVisible(bool flag)
        {
            for (int i = 0; i < m_entry_Tog.Length; i++)
            {
#if OFFICER_SYS
                if (i == 1)
                {
                    this.m_entry_Tog[i].Enable = flag && NewGuid.GuidNewManager.Instance.GetProgressByIndex(11);
                }
                else if (i == 5)
                {
                    this.m_entry_Tog[i].Enable = flag && NewGuid.GuidNewManager.Instance.GetProgressByIndex(8);
                }
                else if (i == 6)
                {
                    this.m_entry_Tog[i].Enable = flag && NewGuid.GuidNewManager.Instance.GetProgressByIndex(10);
                }
                else
                {
                    this.m_entry_Tog[i].Enable = flag;
                }
#else
                if (i == 1)
                {
                    this.m_entry_Tog[i].Enable = flag && NewGuid.GuidNewManager.Instance.GetProgressByIndex(11);
                }
                else if (i == 4)
                {
                    this.m_entry_Tog[i].Enable = flag && NewGuid.GuidNewManager.Instance.GetProgressByIndex(8);
                }
                else if (i == 5)
                {
                    this.m_entry_Tog[i].Enable = flag && NewGuid.GuidNewManager.Instance.GetProgressByIndex(10);
                }
                else
                {
                    this.m_entry_Tog[i].Enable = flag;
                }
#endif
            }
        }

        private void LockBtn(int index, bool islock)
        {
            m_entry_Tog[index].Visible = !islock;
        }

        private void EntryChoose(int i, bool flag)
        {
            if (m_panelName.Length > i)
            {
                GameEvents.BigWorld_Event.OnClickScreen.SafeInvoke();
                if (i == 0)
                {
                    if (GlobalInfo.GAME_NETMODE == GameNetworkMode.Network)
                    {
                        this.m_activity_btn.Visible = true;
                        this.m_push_gift_btn.Visible = true;

                    }
                }

                if (i == 0)
                {
                    GameRoot.instance.NeedListenScreenClick = false;
                }
                else
                {
                    GameRoot.instance.NeedListenScreenClick = true;
                }

                OnBtnDelayTime();
                GameEntryHelper.S_TWEEN_DIR = m_currentIndex - i < 0 ? ENUM_UI_TWEEN_DIR.E_LEFT : ENUM_UI_TWEEN_DIR.E_RIGHT;

                GameEvents.UIEvents.UI_GameEntry_Event.OnGameEntryOpen.SafeInvoke(m_panelName[i]);
                if (UIDefine.UI_BAG == m_panelName[i])
                {
                    EngineCoreEvents.AudioEvents.PlayAudio.SafeInvoke(Audio.AudioType.UISound, GameCustomAudioKey.bag_open.ToString());
                }
                else if (UIDefine.UI_SHOP == m_panelName[i])
                {
                    EngineCoreEvents.AudioEvents.PlayAudio.SafeInvoke(Audio.AudioType.UISound, GameCustomAudioKey.shop_open.ToString());
                }
                else if (UIDefine.UI_ACHIEVEMENT == m_panelName[i])
                {
                    EngineCoreEvents.AudioEvents.PlayAudio.SafeInvoke(Audio.AudioType.UISound, GameCustomAudioKey.achievement_open.ToString());
                }
#if OFFICER_SYS
                else if (UIDefine.UI_POLICE == m_panelName[i])
                {
                    OnNewPoliceEffect(false);
                    EngineCoreEvents.AudioEvents.PlayAudio.SafeInvoke(Audio.AudioType.UISound, GameCustomAudioKey.policeman_open.ToString());
                }
#endif
                else if (UIDefine.UI_CHAPTER == m_panelName[i])
                {

                    EngineCoreEvents.AudioEvents.PlayAudio.SafeInvoke(Audio.AudioType.UISound, GameCustomAudioKey.file_open.ToString());
                }
                else if (UIDefine.UI_FRIEND == m_panelName[i])
                {
                    EngineCoreEvents.AudioEvents.PlayAudio.SafeInvoke(Audio.AudioType.UISound, GameCustomAudioKey.friend_open.ToString());
                }
                else
                {
                    EngineCoreEvents.AudioEvents.PlayAudio.SafeInvoke(Audio.AudioType.UISound, EngineCommonAudioKey.table_change.ToString());
                }

                if (UIDefine.UI_FRIEND != m_panelName[i])
                {
                    m_red_points[m_panelName[i]].Visible = false;
                }

                SetBgImage(m_currentIndex, i);
                m_currentIndex = i;
                GameEntryHelper.m_CurrentBtn = m_panelName[i];
                GameEntryHelper.btnTransPanel(m_panelName[i]);

                if (null != m_remove_red_points[m_panelName[i]])
                {
                    m_remove_red_points[m_panelName[i]].SafeInvoke();
                }


            }
        }

        private void SetBgImage(int lastIndex, int curIndex)
        {
            if (lastIndex >= 0 && curIndex >= 0)
            {
                Action act = () =>
                {
                    int firstIndex = m_entry_Tog[0].Widget.GetSiblingIndex();
                    int lastAbsIndex = m_entry_Tog[lastIndex].Widget.GetSiblingIndex() - firstIndex;
                    int curAbsIndex = m_entry_Tog[curIndex].Widget.GetSiblingIndex() - firstIndex;
                };

                if (m_entry_Tog[lastIndex].Widget.sizeDelta.x > 0)
                {
                    act();
                }
                else
                    TimeModule.Instance.SetTimeout(act, 0.25f);


            }

        }

        private void InitListener()
        {
            RegisterMessage();
        }

        private string m_cur_call_ui_name = string.Empty;

        
        SignInManager sm;
        bool m_is_bottom_btn_show = true;

        public void RequestPushGift()
        {
            GameEvents.NetWorkEvents.SendAsyncMsg.SafeInvoke(new CSGetPushRequest());
        }



        private void ShowBottomButton(GameObject obj)
        {
            m_is_bottom_btn_show = !m_is_bottom_btn_show;
            EnableBottomButtons(false);

            if (m_is_bottom_btn_show)
            {
                EngineCoreEvents.AudioEvents.PlayAudio.SafeInvoke(Audio.AudioType.UISound, GameCustomAudioKey.main_ui_zhankai.ToString());
                EngineCoreEvents.AudioEvents.PlayAudio.SafeInvoke(Audio.AudioType.UISound, GameCustomAudioKey.main_ui_extend.ToString());

                m_entry_tog_show_tween_pos.ResetAndPlay();

                m_entry_tog_tween_alpha.ResetAndPlay();
                m_menuBtnTweener.ResetAndPlay();
            }
            else
            {
                EngineCoreEvents.AudioEvents.PlayAudio.SafeInvoke(Audio.AudioType.UISound, GameCustomAudioKey.main_ui_shousuo.ToString());


                m_entry_tog_show_tween_pos.PlayBackward();

                m_entry_tog_tween_alpha.PlayBackward();
                m_menuBtnTweener.PlayBackward();
            }
            m_menuBtnTweener.SetTweenCompletedCallback(() =>
            {
                if (HaveNotice())
                    this.m_btnSwitchRedPointMark.Visible = !m_is_bottom_btn_show;
            });

        }

        private void EnableBottomButtons(bool v_)
        {

            for (int i = 0; i < m_entry_Tog.Length; ++i)
            {
#if OFFICER_SYS
                if (i == 1)
                {
                    this.m_entry_Tog[i].Enable = v_ && NewGuid.GuidNewManager.Instance.GetProgressByIndex(11);
                }
                else if (i == 5)
                {
                    this.m_entry_Tog[i].Enable = v_ && NewGuid.GuidNewManager.Instance.GetProgressByIndex(8);
                }
                else if (i == 6)
                {
                    this.m_entry_Tog[i].Enable = v_ && NewGuid.GuidNewManager.Instance.GetProgressByIndex(10);
                }
                else
                {
                    m_entry_Tog[i].Enable = v_;
                }
#else
                if (i == 1)
                {
                    this.m_entry_Tog[i].Enable = v_ && NewGuid.GuidNewManager.Instance.GetProgressByIndex(11);
                }
                else if (i == 4)
                {
                    this.m_entry_Tog[i].Enable = v_ && NewGuid.GuidNewManager.Instance.GetProgressByIndex(8);
                }
                else if (i == 5)
                {
                    this.m_entry_Tog[i].Enable = v_ && NewGuid.GuidNewManager.Instance.GetProgressByIndex(10);
                }
                else
                {
                    m_entry_Tog[i].Enable = v_;
                }
#endif
            }
        }

        private void RefreshPushGiftBtn()
        {
            this.m_push_gift_count_txt.Text = PushGiftManager.Instance.GetLoginTypeCount().ToString();

            if (PushGiftManager.Instance.GetLoginTypeCount() > 0)
            {
                this.m_push_gift_left_time_txt.Text = CommonTools.SecondToStringDay2Minute((double)(PushGiftManager.Instance.GetLoginTypeLeftTime()));
                TimeModule.Instance.SetTimeInterval(UpdatePushGiftTimer, 30.0f);
                m_push_gift_btn.Visible = true;
            }
            else
            {

                TimeModule.Instance.RemoveTimeaction(UpdatePushGiftTimer);
                m_push_gift_btn.Visible = false;
            }

        }

        private void UpdatePushGiftTimer()
        {
            this.m_push_gift_left_time_txt.Text = CommonTools.SecondToStringDay2Minute((double)(PushGiftManager.Instance.GetLoginTypeLeftTime()));
        }

        private void OnPushGiftClicked(GameObject obj)
        {
            m_push_gift_view.Visible = true;
            GameEvents.BigWorld_Event.OnClickScreen.SafeInvoke();
            RequestPushGift();
        }

        private void OnPushGiftRsp(object s)
        {
            if (s is SCGetPushResponse)
            {
                var rsp = s as SCGetPushResponse;

                List<Push_Info> infos = new List<Push_Info>();

                if (null == rsp.Infos || 0 == rsp.Infos.Count)
                {
                    m_push_gift_btn.Visible = false;
                    return;
                }

                m_push_gift_btn.Visible = true;
                infos.AddRange(rsp.Infos);
                PushGiftManager.Instance.RefreshLoginPush(infos);

                RefreshPushGiftBtn();
                RefreshLoginPushGiftView();

            }

        }

        private void OnShowBonusPopView(EUNM_BONUS_POP_VIEW_TYPE t_)
        {
            if (EUNM_BONUS_POP_VIEW_TYPE.E_PUSH_GIFT != t_)
            {
                return;
            }

            if (IAPTools.instance.IsInitialized())
            {
                bool taskStatus5 = SeekerGame.NewGuid.GuidNewManager.Instance.GetProgressByIndex(5);
                bool taskStatus7 = SeekerGame.NewGuid.GuidNewManager.Instance.GetProgressByIndex(7);

                ENUM_PUSH_GIFT_BLOCK_TYPE pg_type = ENUM_PUSH_GIFT_BLOCK_TYPE.E_NONE;



                pg_type = PushGiftManager.Instance.GetTurnOnType();
                if (ENUM_PUSH_GIFT_BLOCK_TYPE.E_NONE != pg_type && taskStatus5)
                {
                    if (ENUM_PUSH_GIFT_BLOCK_TYPE.E_LOGIN == pg_type && !string.IsNullOrEmpty(this.m_cur_call_ui_name))
                    {
                        this.m_cur_call_ui_name = string.Empty;

                        this.ShowPushGiftView(pg_type);
                    }
                    else
                    {
                        this.ShowPushGiftView(pg_type);
                    }
                }
                else
                {
                    m_push_gift_view.Visible = false;
                }
            }
        }
        private void RefreshLoginPushGiftView()
        {
            if (!m_push_gift_view.Visible)
                return;

            List<Push_Info> cur_infos = PushGiftManager.Instance.GetPushInfosByTurnOnType(ENUM_PUSH_GIFT_BLOCK_TYPE.E_LOGIN);

            if (null != cur_infos)
            {
                m_push_gift_view.Refresh(cur_infos, OnGiftViewClosed);
            }
        }

        public bool ShowPushGiftView(ENUM_PUSH_GIFT_BLOCK_TYPE block_type)
        {
            List<Push_Info> cur_infos = PushGiftManager.Instance.GetPushInfosByTurnOnType(block_type);

            if (null != cur_infos && cur_infos.Count > 0)
            {
                m_push_gift_view.Refresh(cur_infos, OnGiftViewClosed);
                m_push_gift_view.Visible = true;
            }

            return true;
        }


        private void OnGiftViewClosed()
        {
            m_push_gift_btn_effect.Visible = true;

            RefreshPushGiftBtn();
        }

        public override void OnHide()
        {
            base.OnHide();


            GameRoot.instance.NeedListenScreenClick = false;
            UnRegisterMessage();
            m_currentIndex = -1;
            GameEntryHelper.m_CurrentBtn = m_panelName[0];

            m_push_gift_btn_effect.Visible = false;
            m_activity_btn_effect.Visible = false;
            TimeModule.Instance.RemoveTimeaction(UpdatePushGiftTimer);

            m_is_bottom_btn_show = false;
            ShowBottomButton(null);

            m_hide_btn.RemoveClickCallBack(ShowBottomButton);
        }

        private void RegisterMessage()
        {
            GameEvents.UIEvents.UI_GameEntry_Event.OnOpenPanel += OnOpenPanel;
            GameEvents.UIEvents.UI_GameEntry_Event.OnControlActivity += OnControlActivity;
#if OFFICER_SYS
            GameEvents.RedPointEvents.User_OnNewPoliceEvent += OnNewPolice;
#endif
            GameEvents.RedPointEvents.User_OnNewFriendEvent += OnNewFriend;
            GameEvents.RedPointEvents.User_OnNewAchievementEvent += OnNewAchievement;
            GameEvents.RedPointEvents.User_OnNewActivityEvent += OnNewActivityEvent;
            GameEvents.UIEvents.UI_GameEntry_Event.GetCurrentGameEntryUI += GetCurrentGameEntryUI;
            //GameEvents.UI_Guid_Event.OnSeekOpenClose += OnSeekOpenClose;
            GameEvents.UI_Guid_Event.OnMainIconUnLockComplete += OnMainIconUnLockComplete;
            GameEvents.UIEvents.UI_PushGift_Event.OnGo += ShowPushGiftView;
            GameEvents.UIEvents.UI_Bonus_Pop_View_Event.Listen_OnShow += OnShowBonusPopView;
            GameEvents.UIEvents.UI_GameEntry_Event.OnMainCityHint += OnMainCityHint;
            GameEvents.RedPointEvents.User_OnNewChapterEvent += User_OnNewChapterEvent;
            GameEvents.RedPointEvents.User_OnNewChapterBannerEvent += User_OnNewChapterBannerEvent;
            GameEvents.IAPEvents.OnTransactionDone += DisableItem;
            GameEvents.UIEvents.UI_GameEntry_Event.OnLockMainIcon += OnLockMainIcon;
#if OFFICER_SYS
            GameEvents.UIEvents.UI_GameEntry_Event.OnNewPoliceEffect += OnNewPoliceEffect;
#endif
            GameEvents.UIEvents.UI_GameEntry_Event.OnOpenBottomButton += OpenBottomButton;
            GameEvents.UIEvents.UI_Bonus_Pop_View_Event.BonusPopViewVisible += BonusPopViewVisible;

            GameEvents.UIEvents.UI_GameEntry_Event.Listen_OnCombinePropCollected += OnCombineTipsShow;
            GameEvents.UIEvents.UI_GameEntry_Event.OnMaskBGVisible += OnMaskBGVisible;
            GameEvents.UIEvents.UI_Common_Event.OnCommonUIVisible += OnCommonUIVisible;
            m_activity_btn.AddClickCallBack(BtnActivity);
            for (int i = 0; i < MAXPANEL; i++)
            {
                if (m_panelMsg[i] != 0)
                {
                    MessageHandler.RegisterMessageHandler(m_panelMsg[i], GameEntryHelper.TransPanel);
                }
            }

            m_push_gift_btn.AddClickCallBack(OnPushGiftClicked);
            MessageHandler.RegisterMessageHandler(MessageDefine.SCGetPushResponse, OnPushGiftRsp);


            //MessageHandler.RegisterMessageHandler(MessageDefine.SCAutoOpenGiftDropResp, CommonHelper.OnOpenAutoGiftCallback);

        }

        private void OnCommonUIVisible(bool visible)
        {
            if (m_activityUI.CachedVisible != visible)
                m_activityUI.Visible = visible;
            if (m_bottomUI.CachedVisible != visible)
                m_bottomUI.Visible = visible;
            if (m_taskPanel.CachedVisible != visible)
                m_taskPanel.Visible = visible;
        }

        //todo 解锁完成
        private void OnMainIconUnLockComplete(int index)
        {
#if OFFICER_SYS
            if (index < 6)
#else
            if (index < 5)
#endif
            {
                TweenAlpha tweenAlpha = this.m_entryBackground[index + 1].gameObject.GetOrAddComponent<TweenAlpha>();
                tweenAlpha.From = 0f;
                tweenAlpha.To = 1f;
                tweenAlpha.Duration = 0.5f;


                this.m_entryBackground[index + 1].Visible = true;

                tweenAlpha.PlayForward();


                TimeModule.Instance.SetTimeout(() =>
                {

                    GameObject.DestroyImmediate(tweenAlpha);
                }, 0.5f);
            }
        }

        //private void OnSeekOpenClose(bool open)
        //{
        //    //this.m_MainIcon.Visible = !open;
        //}

        private void OnMainCityHint()
        {
            //this.m_UI_chengsi_anniu.Visible = true;
        }

        private void UnRegisterMessage()
        {
            GameEvents.UIEvents.UI_GameEntry_Event.OnOpenPanel -= OnOpenPanel;
            GameEvents.UIEvents.UI_GameEntry_Event.OnControlActivity -= OnControlActivity;
#if OFFICER_SYS
            GameEvents.RedPointEvents.User_OnNewPoliceEvent -= OnNewPolice;
#endif
            GameEvents.RedPointEvents.User_OnNewFriendEvent -= OnNewFriend;
            GameEvents.RedPointEvents.User_OnNewAchievementEvent -= OnNewAchievement;
            GameEvents.RedPointEvents.User_OnNewActivityEvent -= OnNewActivityEvent;
            GameEvents.UIEvents.UI_GameEntry_Event.GetCurrentGameEntryUI -= GetCurrentGameEntryUI;
            //GameEvents.UI_Guid_Event.OnSeekOpenClose -= OnSeekOpenClose;
            GameEvents.UI_Guid_Event.OnMainIconUnLockComplete -= OnMainIconUnLockComplete;
            GameEvents.UIEvents.UI_PushGift_Event.OnGo -= ShowPushGiftView;
            GameEvents.UIEvents.UI_Bonus_Pop_View_Event.Listen_OnShow -= OnShowBonusPopView;
            GameEvents.UIEvents.UI_GameEntry_Event.OnMainCityHint -= OnMainCityHint;
            GameEvents.RedPointEvents.User_OnNewChapterEvent -= User_OnNewChapterEvent;
            GameEvents.RedPointEvents.User_OnNewChapterBannerEvent -= User_OnNewChapterBannerEvent;
            GameEvents.IAPEvents.OnTransactionDone -= DisableItem;
            GameEvents.UIEvents.UI_GameEntry_Event.OnLockMainIcon -= OnLockMainIcon;
#if OFFICER_SYS
            GameEvents.UIEvents.UI_GameEntry_Event.OnNewPoliceEffect -= OnNewPoliceEffect;
#endif
            GameEvents.UIEvents.UI_GameEntry_Event.OnOpenBottomButton -= OpenBottomButton;
            GameEvents.UIEvents.UI_Bonus_Pop_View_Event.BonusPopViewVisible -= BonusPopViewVisible;
            GameEvents.UIEvents.UI_GameEntry_Event.Listen_OnCombinePropCollected -= OnCombineTipsShow;
            GameEvents.UIEvents.UI_GameEntry_Event.OnMaskBGVisible -= OnMaskBGVisible;
            GameEvents.UIEvents.UI_Common_Event.OnCommonUIVisible -= OnCommonUIVisible;
            m_activity_btn.RemoveClickCallBack(BtnActivity);
            for (int i = 0; i < MAXPANEL; i++)
            {
                if (m_panelMsg[i] != 0)
                {
                    MessageHandler.UnRegisterMessageHandler(m_panelMsg[i], GameEntryHelper.TransPanel);
                }
            }

            m_push_gift_btn.RemoveClickCallBack(OnPushGiftClicked);
            MessageHandler.UnRegisterMessageHandler(MessageDefine.SCGetPushResponse, OnPushGiftRsp);
            MessageHandler.UnRegisterMessageHandler(MessageDefine.MarketResponse, GameEntryHelper.TransPanel);

        }

        private void OnMaskBGVisible(bool visible)
        {
            if(this.m_maskBG.CachedVisible != visible)
                this.m_maskBG.Visible = visible;
            GameEvents.UIEvents.UI_Common_Event.OnCommonUIVisible.SafeInvoke(!visible);
        }

        private string GetCurrentGameEntryUI()
        {
            return m_panelName[m_currentIndex];
        }


        private void BtnActivity(GameObject obj)
        {

            GameEvents.BigWorld_Event.OnClickScreen.SafeInvoke();
            m_ActivityRedPoint.Visible = false;
            GameEvents.RedPointEvents.Sys_OnNewActivityReadedEvent.SafeInvoke();
            EngineCoreEvents.UIEvent.ShowUIEvent.SafeInvoke(UIDefine.UI_ACTIVITY);
        }

        private void OnControlActivity(bool flag)
        {
            m_activity_btn.Visible = flag;
            m_push_gift_btn.Visible = flag;
        }
#if OFFICER_SYS
        private void OnNewPolice(bool val_)
        {
            m_red_points[UIDefine.UI_POLICE].Visible = val_;
        }
#endif

        private void OnNewFriend(bool val_)
        {
            if (NewGuid.GuidNewManager.Instance.GetProgressByIndex(10))
                m_red_points[UIDefine.UI_FRIEND].Visible = val_;
        }

        private void User_OnNewChapterEvent(int type)
        {
            if (NewGuid.GuidNewManager.Instance.GetProgressByIndex(11))
            {
                m_red_points[UIDefine.UI_CHAPTER].Visible = true;
            }
        }

        private void User_OnNewChapterBannerEvent(bool isRead)
        {
            if (NewGuid.GuidNewManager.Instance.GetProgressByIndex(11))
                m_red_points[UIDefine.UI_CHAPTER].Visible = isRead;
        }
        public void DisableItem(long charge_id_)
        {
            long giftID = CommonHelper.GetGiftID(charge_id_);
            if (giftID > 0)
            {
                var gifts = PushGiftManager.Instance.GetPushInfosByTurnOnType(ENUM_PUSH_GIFT_BLOCK_TYPE.E_LOGIN);

                if (null != gifts && gifts.Count > 0)
                {
                    foreach (var gift in gifts)
                    {
                        if (giftID == gift.PushId)
                        {
                            gift.Buyed = true;

                            RefreshPushGiftBtn();
                            break;
                        }
                    }


                }


            }
        }

        private void OnNewAchievement()
        {
            if (m_currentIndex > 0 && m_panelName[m_currentIndex].Equals(UIDefine.UI_ACHIEVEMENT))
            {
                return;
            }
            m_red_points[UIDefine.UI_ACHIEVEMENT].Visible = true;
        }

        private void OnNewActivityEvent()
        {
            m_ActivityRedPoint.Visible = true;
        }

        private void OnOpenPanel(string panelName)
        {
            for (int i = 0; i < m_panelName.Length; i++)
            {
                if (m_panelName[i].Equals(panelName))
                {

                    EntryChoose(i, true);
                    break;
                }
            }

        }


        private bool HaveNotice()
        {
            foreach (var pair in m_red_points)
            {
                if (pair.Value.Visible)
                    return true;
            }

            return false;
        }


        private int togIndex = 0;
        private bool togIsLock;
        private void OnLockMainIcon(int index, bool isLock)
        {

            this.togIndex = index;
            this.togIsLock = isLock;
            if (!m_is_bottom_btn_show)
            {

                ShowBottomButton(null);
                TimeModule.Instance.SetTimeout(OnStartLockMainIcon, 0.5f);
                return;
            }
            OnStartLockMainIcon();

        }

        private void OnStartLockMainIcon()
        {
            this.m_gridComponent.enabled = false;
            int minIndex = 0;
            Vector2 currentPos = Vector2.zero;
            for (int i = 0; i < m_entry_Tog.Length; i++)
            {
                if (i < this.togIndex && m_entry_Tog[i].Visible)
                {
                    if (minIndex <= i)
                    {
                        currentPos = m_entry_Tog[i].Widget.anchoredPosition;
                        minIndex = i;
                    }
                    float targetX = m_entry_Tog[i].Widget.localPosition.x - this.m_gridComponent.cellSize.x - this.m_gridComponent.spacing.x;
                    m_entry_Tog[i].Widget.DOLocalMoveX(targetX, 1f).SetEase(Ease.Linear);
                }
            }
            TimeModule.Instance.SetTimeout(() =>
            {
                m_entry_Tog[this.togIndex].Widget.anchorMax = Vector2.up;
                m_entry_Tog[this.togIndex].Widget.anchorMin = Vector2.up;
                m_entry_Tog[this.togIndex].Widget.sizeDelta = this.m_gridComponent.cellSize;
                m_entry_Tog[this.togIndex].Widget.anchoredPosition = currentPos;
                m_entry_Tog[this.togIndex].Visible = true;
                m_entry_Tog[this.togIndex].Widget.DOScale(Vector3.one * 1.2f, 0.5f).SetEase(Ease.InSine).OnComplete(() =>
                {


                    m_entry_Tog[this.togIndex].Widget.DOScale(Vector3.one, 0.5f).SetEase(Ease.OutSine).OnComplete(() =>
                    {
                        GameEvents.UIEvents.UI_GameEntry_Event.OnLockMainIconComplete.SafeInvoke(this.togIndex);
                        this.m_gridComponent.enabled = true;
                    });
                });
            }, 1f);

        }
#if OFFICER_SYS
        private void OnNewPoliceEffect(bool isPlay)
        {
            m_newPoliceEffect.Visible = isPlay;
            if (isPlay)
            {
                OpenBottomButton();
                m_newPoliceEffect.EffectPrefabName = "UI_dipai_icon.prefab";
                m_newPoliceEffect.ReplayEffect();
            }
        }
#endif

        private void OpenBottomButton()
        {
            if (!m_is_bottom_btn_show)
            {
                ShowBottomButton(null);
            }
        }

        private void BonusPopViewVisible(bool visible)
        {
            m_push_gift_view.Visible = visible;
        }

        private void OnCombineTipsShow()
        {
            m_combine_tips.Refresh();
        }
    }
}
