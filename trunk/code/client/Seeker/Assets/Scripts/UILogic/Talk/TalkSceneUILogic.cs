#define Test
using EngineCore;
using GOEngine;
using System.Collections.Generic;
using UnityEngine;

namespace SeekerGame
{
    [UILogicHandler(UIDefine.UI_SCENETALK)]
    public class TalkSceneUILogic : UILogicBase
    {
        private GameTexture m_bg_tex;
        //private GameImage[] m_article_img;
        //private GameTexture[] m_person_Img;
        //private GameImage[] m_bg_img;
        //private GameLabel[] m_articleName_Lab;
        private TalkSceneTexture[] m_talkTexture = null;

        private GameSpine m_leftIcon, m_rightIcon;
        private GameTexture m_leftPngIcon, m_rightPngIcon;
        private TalkPartUIComponent m_leftPartUI;
        private TalkPartUIComponent m_rightPartUI;

        private TalkData m_talkdata;
        private int m_currentIndex = -1;
        private GameButton m_btnNext = null;
        private List<long> m_chooseID = new List<long>();

        private int m_bundleCount = 0;

        TalkDialogEnum m_talk_type;
        protected override void OnInit()
        {
            base.OnInit();
            InitController();
        }

        public override void OnShow(object param)
        {
            base.OnShow(param);
            GameEvents.UIEvents.UI_Common_Event.OnCommonUIVisible.SafeInvoke(false);
            GameEvents.UIEvents.UI_Talk_Event.OnTalkChoose += EventChoose;
            this.m_btnNext.AddClickCallBack(BtnNextPart);
            this.m_bg_tex.AddClickCallBack(BtnNextPart);
            m_leftPartUI.Visible = false;
            m_rightPartUI.Visible = false;
            m_leftIcon.Visible = false;
            m_rightIcon.Visible = false;
            this.m_leftPngIcon.Visible = false;
            this.m_rightPngIcon.Visible = false;
            this.m_canNextPart = false;
            //MessageHandler.RegisterMessageHandler(MessageDefine.SCChatFinishResponse, OnRes);
            if (param != null)
            {
                TalkUIData ui_param = param as TalkUIData;
                m_talk_type = ui_param.m_talk_type;
                long id = ui_param.talk_id;
                InitData(id);
                //PreLoad();
            }
            TimeModule.Instance.SetTimeout(()=> {
                setPartData();
                m_canNextPart = true;
            },0.4f);
           
            //TODO : 转移到 TalkUIHelper 里的 OnStartTalk
            //CSChatFinishRequest req = new CSChatFinishRequest();
            //req.ChatId = m_talkdata.chatData.id;
            //GameEvents.NetWorkEvents.SendMsg.SafeInvoke(req);
        }
        public override FrameDisplayMode UIFrameDisplayMode
        {
            get
            {
                return FrameDisplayMode.FULLSCREEN;
            }
        }
        private void PreLoad()
        {
            m_bundleCount = 0;
            for (int i = 0; i < m_talkdata.partData.Count; i++)
            {
                string iconName = m_talkdata.partData[i].icon;
                if (!iconName.Contains(".png"))
                {
                    iconName += ".png";
                }
                ResourceModule.Instance.PreloadBundle(iconName, delegate (string name, AssetBundle bundle)
                {
                    m_bundleCount++;
                    if (m_bundleCount >= m_talkdata.partData.Count)
                    {
                        GameEvents.UIEvents.UI_Loading_Event.OnLoadingState.SafeInvoke(2, true);
                    }
                });
            }

        }

        public override void OnHide()
        {
            base.OnHide();
            
            if (GlobalInfo.GAME_NETMODE == GameNetworkMode.Standalone)
            {
                TimeModule.Instance.RemoveTimeaction(TickCanNextPart);
            }
            GameEvents.UIEvents.UI_Talk_Event.OnTalkChoose -= EventChoose;
            this.m_btnNext.RemoveClickCallBack(BtnNextPart);
            this.m_bg_tex.RemoveClickCallBack(BtnNextPart);
            m_chooseID.Clear();
            if (GlobalInfo.GAME_NETMODE == GameNetworkMode.Network)
            {
                if (TalkDialogEnum.SceneTalk == m_talk_type)
                    GlobalInfo.MY_PLAYER_INFO.PlayerTaskSystem.SyncTaskDetailInfo(1);
            }
            if (TalkDialogEnum.TaskEndTalk == m_talk_type)
                GameEvents.UIEvents.UI_Common_Event.OnCommonUIVisible.SafeInvoke(true);
            GameEvents.UIEvents.UI_Talk_Event.OnTalkFinish.SafeInvoke(m_talkdata.chatData.id);
            if (TalkUIHelper.TalkEnum == TalkDialogEnum.SceneTalk)
            {
                GameEvents.UIEvents.UI_GameEntry_Event.OnBlockTaskTouch.SafeInvoke(1.0f);
            }
            m_currentIndex = -1;
            m_talkdata = null;
            this.m_currentPart = null;
        }

        private void InitController()
        {
            m_bg_tex = Make<GameTexture>("bgImg");
            this.m_talkTexture = new TalkSceneTexture[4];

            //m_article_img = new GameImage[4];
            //m_bg_img = new GameImage[4];
            //this.m_person_Img = new GameTexture[4];
            //this.m_articleName_Lab = new GameLabel[4];
            for (int i = 0; i < 4; i++)
            {
                this.m_talkTexture[i] = Make<TalkSceneTexture>(string.Format("Panel_animation:bgImg:ImageBG_{0}", i));
                this.m_talkTexture[i].Visible = false;
                //m_bg_img[i] = Make<GameImage>(string.Format("Panel_animation:bgImg:ImageBG_{0}", i));
                //m_article_img[i] = m_bg_img[i].Make<GameImage>("Image_things");
                //this.m_person_Img[i] = m_bg_img[i].Make<GameTexture>("Panel_role:RawImage");
                //this.m_articleName_Lab[i] = m_bg_img[i].Make<GameLabel>("Text_name");
                //m_bg_img[i].Visible = false;
            }
            this.m_leftIcon = Make<GameSpine>("Panel_animation:icon_leftSpine");
            this.m_rightIcon = Make<GameSpine>("Panel_animation:icon_rightSpine");
            this.m_leftPngIcon = Make<GameTexture>("Panel_animation:icon_leftPng");
            this.m_rightPngIcon = Make<GameTexture>("Panel_animation:icon_rightPng");
            m_leftPartUI = Make<TalkPartUIComponent>("Panel_animation:Panel_left");
            m_leftPartUI.SetIcon(m_leftIcon, this.m_leftPngIcon);
            m_rightPartUI = Make<TalkPartUIComponent>("Panel_animation:Panel_right");
            m_rightPartUI.SetIcon(m_rightIcon, this.m_rightPngIcon);
            this.m_btnNext = Make<GameButton>("Panel_animation:Button_skip");
        }

        private void InitData(long id)
        {
            ConfChat confChat = ConfChat.Get(id);
            if (confChat == null)
            {
                DebugUtil.LogError("chat data is not exist");
            }
            m_talkdata = new TalkData();
            m_talkdata.chatData = confChat;
            string sceneName = confChat.sceanid;
            if (!sceneName.Contains(".png"))
            {
                sceneName += ".png";
            }
            m_bg_tex.TextureName = sceneName;
            ConfChatItem.GetConfig("chatId", confChat.id, out m_talkdata.partData);
        }

        private bool m_canNextPart = false;
        private void BtnNextPart(GameObject obj)
        {
            if (!m_canNextPart)
            {
                return;
            }
            if (this.m_currentPart != null && !this.m_currentPart.ContentFaderComplete)
            {
                return;
            }
            EventChoose(-1);
            if (GlobalInfo.GAME_NETMODE == GameNetworkMode.Standalone)
            {
                m_canNextPart = false;
                TimeModule.Instance.SetTimeout(TickCanNextPart, 1f);
            }

            //GameEvents.UIEvents.UI_Talk_Event.OnTalkNextPart.SafeInvoke();
        }

        private void TickCanNextPart()
        {
            this.m_canNextPart = true;
        }

        private void EventChoose(long id)
        {
            if (id == 0)
            {
                TalkOver();
            }
            else if (id > 0)
            {
                for (int i = 0; i < m_talkdata.partData.Count; i++)
                {
                    ConfChatItem partdata = m_talkdata.partData[i];
                    if (partdata.id == id)
                    {
                        m_currentIndex = i;
                        setPartData(partdata);
                    }
                }
            }
            else if (id < 0)
            {
                setPartData();
            }

        }

        private void setPartData(ConfChatItem partdata)
        {

            if (partdata.jumpids != null)
            {
                for (int i = 0; i < partdata.jumpids.Length; i++)
                {
                    if (!m_chooseID.Contains(partdata.jumpids[i]))
                    {
                        m_chooseID.Add(partdata.jumpids[i]);
                    }
                }
            }

            for (int i = 0; i < this.m_talkTexture.Length; i++)
            {
                string[] apellationName = partdata.apellation.Split(',');
                if (partdata.propids != null && i < partdata.propids.Length)
                {
                    string propName = partdata.propids[i];
                    string articleName = string.Empty;
                    if (i < apellationName.Length)
                    {
                        articleName = LocalizeModule.Instance.GetString(apellationName[i]);
                    }
                    if (propName.Contains("role") || propName.Contains("suspect"))
                    {
                        this.m_talkTexture[i].SetData(propName, articleName, false);
                        //m_person_Img[i].TextureName = propName;
                        //m_article_img[i].Visible = false;
                        //m_person_Img[i].Visible = true;
                    }
                    else
                    {
                        this.m_talkTexture[i].SetData(propName, articleName, true);
                        //m_article_img[i].Sprite = propName;
                        //m_article_img[i].Visible = true;
                        //m_person_Img[i].Visible = false;
                    }
                    m_talkTexture[i].m_isTweener = false;
                    m_talkTexture[i].Visible = true;
                }
                else
                {
                    if (!m_talkTexture[i].m_isTweener)
                    {
                        m_talkTexture[i].m_isTweener = true;
                        m_talkTexture[i].Visible = false;
                    }
                }
            }
            if (0 == partdata.iconPosition)
            {
                m_leftPartUI.Visible = true;
            }
            else
            {
                m_rightPartUI.Visible = true;
            }

            if (partdata.iconPosition == 0)
            {
                m_leftPartUI.setData(partdata);
                this.m_currentPart = m_leftPartUI;
            }
            else// if (partdata.iconPosition == 2)
            {
                m_rightPartUI.setData(partdata);
                this.m_currentPart = m_rightPartUI;
            }
            m_leftPartUI.IsTalk(partdata.iconPosition == 0);
            m_rightPartUI.IsTalk(partdata.iconPosition != 0);
        }

        private TalkPartUIComponent m_currentPart = null;
        private void setPartData()
        {
            GetCurrentIndex();
            //++m_currentIndex;
            if (m_currentIndex < 0 || m_currentIndex >= m_talkdata.partData.Count)
            {
                TalkOver();
                return;
            }
            ConfChatItem partdata = m_talkdata.partData[m_currentIndex];
            setPartData(partdata);
        }

        private void GetCurrentIndex()
        {
            int tempIndex = m_currentIndex + 1;
            for (int i = 0; i < m_chooseID.Count; i++)
            {
                if (tempIndex >= m_talkdata.partData.Count)
                {
                    break;
                }
                if (m_talkdata.partData[tempIndex].id == m_chooseID[i])
                {
                    tempIndex = tempIndex + 1;
                }
            }
            m_currentIndex = tempIndex;
        }

        private void TalkOver()
        {
            if (m_talkdata != null)
            {

                

                Dictionary<UBSParamKeyName, object> _params = new Dictionary<UBSParamKeyName, object>()
                {
                            { UBSParamKeyName.ContentID, m_talkdata.chatData.id},
                };

                UserBehaviorStatisticsModules.Instance.LogEvent(UBSEventKeyName.dialogue_finish, null, _params);
            }
            EngineCoreEvents.UIEvent.HideUIEvent.SafeInvoke(UIDefine.UI_SCENETALK);


        }

        private void OnRes(object obj)
        {
            if (obj is SCChatFinishResponse)
            {
                SCChatFinishResponse res = (SCChatFinishResponse)obj;
                if (res.Status == null)
                {
                    //if (m_talkdata.chatData.rewards != null && m_talkdata.chatData.rewards.Length != 0)
                    //{
                    //    //请求奖励数据
                    //    TalkRewardData rd = new TalkRewardData();
                    //    rd.ID = new long[m_talkdata.chatData.rewards.Length];
                    //    for (int i = 0; i < m_talkdata.chatData.rewards.Length; i++)
                    //    {
                    //        rd.ID[i] = m_talkdata.chatData.rewards[i];
                    //    }
                    //    FrameMgr.OpenUIParams param = new FrameMgr.OpenUIParams(UIDefine.UI_TALKREWAR);
                    //    param.Param = rd;
                    //    EngineCoreEvents.UIEvent.ShowUIEventWithParam.SafeInvoke(param);
                    //}

                }
            }
        }
    }

    public class TalkSceneTexture : GameUIComponent
    {
        private GameImage m_article_img;
        private GameTexture m_person_img;
        private GameLabel m_articleName_Lab;
        public bool m_isTweener = false;
        protected override void OnInit()
        {
            base.OnInit();
            this.m_article_img = Make<GameImage>("Image_things");
            this.m_person_img = Make<GameTexture>("Panel_role:RawImage");
            this.m_articleName_Lab = Make<GameLabel>("Text_name");
        }

        public void SetData(string imgName,string nameLab,bool isArticle)
        {
            this.m_article_img.Visible = isArticle;
            this.m_person_img.Visible = !isArticle;
            this.m_articleName_Lab.Text = LocalizeModule.Instance.GetString(nameLab);
            if (isArticle)
            {
                this.m_article_img.Sprite = imgName;
            }
            else
            {
                this.m_person_img.TextureName = imgName;
            }
        }

        public override void OnHide()
        {
            base.OnHide();
            this.m_isTweener = false;
        }
    }
}


