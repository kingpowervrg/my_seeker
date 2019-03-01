//#define Test
using EngineCore;
using GOEngine;
using GOGUI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SeekerGame
{
    [UILogicHandler(UIDefine.UI_BAG)]
    public class BagUILogic : UILogicBase
    {
        string[] m_btnPage_Str = new string[] { "btnTotal", "btnRecently", "btnEnergy", "btnProp", "btnPieces", "btnGiffBox", "btnElse" };

        string[] m_toogle_name_Str = new string[] { LocalizeModule.Instance.GetString( "bag_all_1"),
            LocalizeModule.Instance.GetString( "bag_newly_gain"),
            LocalizeModule.Instance.GetString( "bag_energy_gain"),
            LocalizeModule.Instance.GetString( "bag_prop_gain"),
            LocalizeModule.Instance.GetString( "bag_material"),
            LocalizeModule.Instance.GetString( "bag_gift"),
            LocalizeModule.Instance.GetString( "bag_other") };
        #region 左边按钮
        private GameToggleButton[] m_page_toggle;

        //private TweenScale[] m_page_tween;
        private GameObject[] m_arrow_obj;
        private GameLabel[] m_arrowLab_obj;
        private GameObject[] m_oriLab_obj;

        private GameLabel[] m_pageTog_lab;
        private readonly Color m_destColor = Color.white;
        private readonly Color m_oriColor = new Color(173f / 255f, 226f / 255f, 1f);
        #endregion

        #region 中间信息
        private GameUIContainer m_Prop_grid;
        private GameLabel m_NothingTip_lab;
        private GameUIEffect m_chooseUIEffect;
        private GameImage m_panelDown_Img;
        //private TweenScale m_tweenPos = null;
        #endregion

        private GameButton m_btnClose = null;
        private UIBagPropInfo m_propInfo_panel;

        List<PropData> mCurPropData;
        List<PropData> m_curPropData
        {
            get
            {
                if (mCurPropData == null)
                {
                    mCurPropData = new List<PropData>();
                }
                return mCurPropData;
            }
            set { mCurPropData = value; }
        }

        List<PropData> mRecentPropData;
        List<PropData> m_recentPropData
        {
            get
            {
                if (mRecentPropData == null)
                {
                    mRecentPropData = new List<PropData>();
                }
                return mRecentPropData;
            }
            set { mRecentPropData = value; }
        }
        BagTypeEnum m_LastBagType = BagTypeEnum.None;
        BagTypeEnum m_CurrentBagType = BagTypeEnum.Total;
        private bool m_needClearRecently = false;
        protected override void OnInit()
        {
            base.OnInit();

            this.m_btnClose = Make<GameButton>("Button_close");
            initControl();
            for (int i = 0; i < m_page_toggle.Length; i++)
            {
                btnPageClick(i, m_page_toggle[i]);
            }
        }

        public override FrameDisplayMode UIFrameDisplayMode
        {
            get
            {
                return FrameDisplayMode.FULLSCREEN;
            }
        }

        void initControl()
        {
            #region 控件
            int pageCount = m_btnPage_Str.Length;
            m_page_toggle = new GameToggleButton[pageCount];
            //m_page_tween = new TweenScale[pageCount];
            m_arrow_obj = new GameObject[pageCount];
            this.m_arrowLab_obj = new GameLabel[pageCount];
            this.m_oriLab_obj = new GameObject[pageCount];
            //this.m_arrowLab_obj = new GameObject[pageCount];

            m_pageTog_lab = new GameLabel[pageCount];
            for (int i = 0; i < m_btnPage_Str.Length; i++)
            {
                m_page_toggle[i] = Make<GameToggleButton>(string.Format("Panel_down:leftBtn:{0}", m_btnPage_Str[i]));
                //m_page_tween[i] = m_page_toggle[i].gameObject.GetComponent<TweenScale>();
                m_arrow_obj[i] = m_page_toggle[i].Widget.Find("Background/Checkmark/Arrow").gameObject;
                m_arrowLab_obj[i] = m_page_toggle[i].Make<GameLabel>("Background/Checkmark/Label (1)");
                this.m_oriLab_obj[i] = m_page_toggle[i].Widget.Find("Background/Label").gameObject;

                m_pageTog_lab[i] = m_page_toggle[i].Make<GameLabel>("Label");
                m_pageTog_lab[i].color = m_oriColor;
                m_arrowLab_obj[i].Text = m_pageTog_lab[i].Text = m_toogle_name_Str[i];
                m_arrow_obj[i].SetActive(false);
                m_arrowLab_obj[i].SetActive(false);
                this.m_oriLab_obj[i].SetActive(true);
            }
            m_Prop_grid = Make<GameUIContainer>("Panel_down:Panel:grid");
            m_NothingTip_lab = Make<GameLabel>("Panel_down:nothingTips");
            m_propInfo_panel = Make<UIBagPropInfo>("Panel_down:detail");
            m_chooseUIEffect = Make<GameUIEffect>("Panel_down:UI_xuanzhong");
            m_panelDown_Img = Make<GameImage>("Panel_down");
            //this.m_tweenPos = this.m_panelDown_Img.GetComponent<TweenScale>();
            #endregion
        }

        public override void OnShow(object param)
        {
            base.OnShow(param);
            MainPanelInGameUILogic.Show();
            this.m_btnClose.AddClickCallBack(ClosePanel);
            //SetCloseBtnID("Button_close");
            UserBehaviorStatisticsModules.Instance.LogEvent(UBSEventKeyName.bag_in, 1, null);

            MessageHandler.RegisterMessageHandler(MessageDefine.SCPlayerPropResponse, PlayerPropRequestCallBack);
            MessageHandler.RegisterMessageHandler(MessageDefine.SCPlayerPropRecentResposne, PlayerPropRequestCallBack);
            GameEvents.UIEvents.UI_Bag_Event.OnPropCost += BagCost;
            GameEvents.UIEvents.UI_Bag_Event.OnItemClick += OnItemClick;
            GameEvents.UIEvents.UI_Bag_Event.GetCurrentBagType = GetCurrentBagType;
            m_chooseUIEffect.EffectPrefabName = "UI_xuanzhong.prefab";
            m_chooseUIEffect.Visible = false;
            if (param != null)
            {
                PlayerPropRequestCallBack(param);
            }
            else
            {
                RefreshProps();
            }

            //CommonHelper.UItween(this.m_tweenPos);

        }

        public override void OnHide()
        {
            base.OnHide();
            MainPanelInGameUILogic.Hide();
            this.m_btnClose.RemoveClickCallBack(ClosePanel);
            MessageHandler.UnRegisterMessageHandler(MessageDefine.SCPlayerPropResponse, PlayerPropRequestCallBack);
            MessageHandler.UnRegisterMessageHandler(MessageDefine.SCPlayerPropRecentResposne, PlayerPropRequestCallBack);
            GameEvents.UIEvents.UI_Bag_Event.OnPropCost -= BagCost;
            GameEvents.UIEvents.UI_Bag_Event.OnItemClick -= OnItemClick;
            m_CurrentBagType = BagTypeEnum.Total;
            m_LastBagType = BagTypeEnum.None;
            m_curPropData = null;
            m_recentPropData = null;
            m_needClearRecently = false;
        }

        private void ClosePanel(GameObject obj)
        {
            CloseFrame();
            GameEntryHelper.m_CurrentBtn = string.Empty;
        }

        void BagCost(long id)
        {
            m_LastBagType = BagTypeEnum.None;

            RefreshProps();
        }

        void btnPageClick(int i, GameToggleButton btn)
        {
            btn.AddChangeCallBack(delegate (bool value)
            {
                changePageType(i, value);
            });
        }

        void changePageType(int i, bool value)
        {
            if (value)
            {
                if (BagTypeEnum.None != m_LastBagType)
                    EngineCoreEvents.AudioEvents.PlayAudio.SafeInvoke(Audio.AudioType.UISound, EngineCommonAudioKey.table_change.ToString());
                m_CurrentBagType = (BagTypeEnum)i;
                if (m_LastBagType == m_CurrentBagType)
                {
                    return;
                }
                m_LastBagType = m_CurrentBagType;
            }
            ChangePage(value, i);
        }

        /// <summary>
        /// 刷新背包
        /// </summary>
        /// <param name="confProp"></param>
        void ReflashBag(List<PropData> confProp, int chooseIndex)
        {
            m_chooseUIEffect.gameObject.transform.SetParent(m_panelDown_Img.gameObject.transform, false);
            m_chooseUIEffect.Visible = false;
            //m_Prop_grid.Clear();
            int propCount = confProp.Count;
            m_NothingTip_lab.SetActive(propCount == 0);
            if (propCount == 0)
            {
                m_propInfo_panel.setNoInfoData();
            }
            m_Prop_grid.EnsureSize<BagItem>(propCount);
            for (int i = 0; i < propCount; i++)
            {
                PropData prop = confProp[i];
                BagItem bagItem = m_Prop_grid.GetChild<BagItem>(i);
                bagItem.Visible = true;
                bagItem.setData(prop, m_propInfo_panel, chooseIndex == i);
            }
            m_Prop_grid.Widget.anchoredPosition = Vector2.one;
        }

        void ChangePage(bool value, int i)
        {
            if (value)
            {
                m_arrow_obj[(int)m_CurrentBagType].SetActive(true);
                m_arrowLab_obj[(int)m_CurrentBagType].SetActive(true);
                this.m_oriLab_obj[(int)m_CurrentBagType].SetActive(false);
                //m_page_tween[(int)m_CurrentBagType].PlayForward();
                m_pageTog_lab[(int)m_CurrentBagType].color = m_destColor;
                List<PropData> tempPropData = new List<PropData>();
                m_propInfo_panel.RecentlyInfoEnable = (m_CurrentBagType != BagTypeEnum.Recently);
                if (m_CurrentBagType == BagTypeEnum.Recently)
                {
                    tempPropData = m_recentPropData;
#if Test
                    SCPlayerPropRecentResposne scRes = new SCPlayerPropRecentResposne();
                    PlayerPropRequestCallBack(scRes);
#else
                    CSPlayerPropRecentRequest msg_recentProp = new CSPlayerPropRecentRequest();

#if !NETWORK_SYNC || UNITY_EDITOR
                    GameEvents.NetWorkEvents.SendAsyncMsg.SafeInvoke(msg_recentProp);
#else
            GameEvents.NetWorkEvents.SendMsg.SafeInvoke(msg_recentProp);
#endif

#endif
                }
                else
                {
                    if (m_needClearRecently)
                    {
                        m_needClearRecently = false;
                        m_recentPropData.Clear();
                        GlobalInfo.MY_PLAYER_INFO.ClearRecentPropInfo();
                    }
                    tempPropData = m_curPropData;
                }
                List<PropData> confProp = BagHelper.getPropForPage(m_CurrentBagType, tempPropData);
                ReflashBag(confProp, 0);
            }
            else
            {
                m_arrowLab_obj[i].SetActive(false);
                m_arrow_obj[i].SetActive(false);
                this.m_oriLab_obj[i].SetActive(true);
                //m_page_tween[i].PlayBackward();
                m_pageTog_lab[i].color = m_oriColor;
            }

        }

        void RefreshProps()
        {
            m_curPropData = BagHelper.getPropData(GlobalInfo.MY_PLAYER_INFO.Bag_infos);
            m_recentPropData = BagHelper.getPropData(GlobalInfo.MY_PLAYER_INFO.Recent_Prop_infos);
            if (m_page_toggle[(int)m_CurrentBagType].Checked)
            {
                changePageType((int)m_CurrentBagType, true);
            }
            else
            {
                m_page_toggle[(int)m_CurrentBagType].Checked = true;
            }
        }

        /// <summary>
        /// 请求背包信息回调
        /// </summary>
        /// <param name="obj"></param>
        void PlayerPropRequestCallBack(object obj)
        {
            if (obj is SCPlayerPropResponse)
            {
                SCPlayerPropResponse msg_prop_Res = obj as SCPlayerPropResponse;
                if (msg_prop_Res.ReponseStatus == null || msg_prop_Res.ReponseStatus.Code == 0)
                {
                    m_curPropData = BagHelper.getPropData(msg_prop_Res.PlayerProps);
                    m_recentPropData = BagHelper.getPropData(msg_prop_Res.RecentProps);
                    if (m_page_toggle[(int)m_CurrentBagType].Checked)
                    {
                        changePageType((int)m_CurrentBagType, true);
                    }
                    else
                    {
                        m_page_toggle[(int)m_CurrentBagType].Checked = true;
                    }
                }
            }
            else if (obj is SCPlayerPropRecentResposne)
            {
                SCPlayerPropRecentResposne msg_recentProp_Res = obj as SCPlayerPropRecentResposne;
                if (msg_recentProp_Res.ReponseStatus == null || msg_recentProp_Res.ReponseStatus.Code == 0)
                {
                    m_needClearRecently = true;
                }
            }
        }

        private void OnItemClick(Transform tran)
        {
            if (tran != null)
            {
                m_chooseUIEffect.gameObject.transform.SetParent(tran, false);
                m_chooseUIEffect.Visible = true;
            }
        }

        private BagTypeEnum GetCurrentBagType()
        {
            return this.m_CurrentBagType;
        }

    }
}

