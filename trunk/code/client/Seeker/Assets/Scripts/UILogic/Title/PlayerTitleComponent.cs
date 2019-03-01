using EngineCore;
using UnityEngine;
using System.Collections.Generic;
using System;

namespace SeekerGame
{


    public class PlayerTitleHelper
    {
        public static List<RankData> GetRandData(string json)
        {
            List<RankData> ranks = Utf8Json.JsonSerializer.Deserialize<List<RankData>>(json);
            return ranks;
            //    json = json.Replace("type:", "");
            //    json = json.Replace("value:", "");
            //    json = json.Replace("},{", "|");
            //    json = json.Replace("{", "");
            //    json = json.Replace("}", "");
            //    string[] arrays = json.Split('|');
            //    List<RankData> rankList = new List<RankData>();
            //    for (int i = 0; i < arrays.Length; i++)
            //    {
            //        string[] valueArrays = arrays[i].Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            //        if (valueArrays.Length == 2)
            //        {
            //            RankData rank = new RankData();
            //            int rankType;
            //            int rankValue;
            //            int.TryParse(valueArrays[0], out rankType);
            //            int.TryParse(valueArrays[1], out rankValue);
            //            rank.type = rankType;
            //            rank.value = rankValue;
            //            rankList.Add(rank);
            //        }
            //    }
            //    return rankList;
            }
        }

    public class PlayerTitleComponent : GameUIComponent
    {
        private GameUIContainer m_grid_con;
        private long m_chooseID = -1;
        private GameButton m_close_btn;
        private TitleComponent m_curTitleMsg = null;

        protected override void OnInit()
        {
            base.OnInit();
            m_grid_con = Make<GameUIContainer>("Scroll View:Viewport");
            m_close_btn = Make<GameButton>("Button_close");
        }

        public override void OnShow(object param)
        {
            base.OnShow(param);
            UserBehaviorStatisticsModules.Instance.LogEvent(UBSEventKeyName.title_in, 1.0f, null);
            MessageHandler.RegisterMessageHandler(MessageDefine.SCTitleResponse, OnRespone);
            GameEvents.UIEvents.UI_PlayerTitle_Event.OnChoose += OnChoose;
            m_close_btn.AddClickCallBack(OnClose);
            //ConfTitle.array
            CSTitleRequest req = new CSTitleRequest();
#if !NETWORK_SYNC || UNITY_EDITOR
            GameEvents.NetWorkEvents.SendAsyncMsg.SafeInvoke(req);
#else
            GameEvents.NetWorkEvents.SendMsg.SafeInvoke(req);
#endif


        }

        public override void OnHide()
        {
            base.OnHide();
            MessageHandler.UnRegisterMessageHandler(MessageDefine.SCTitleResponse, OnRespone);
            GameEvents.UIEvents.UI_PlayerTitle_Event.OnChoose -= OnChoose;
            m_close_btn.RemoveClickCallBack(OnClose);
            m_chooseID = -1;
            m_curTitleMsg = null;
            //m_chooseTitle = null;
        }

        private void OnClose(GameObject obj)
        {
            EngineCoreEvents.AudioEvents.PlayAudio.SafeInvoke(Audio.AudioType.UISound,EngineCommonAudioKey.Close_Window.ToString());
            Visible = false;
            GameEvents.UIEvents.UI_PlayerTitle_Event.OnClose.SafeInvoke();
        }

        private void OnRespone(object obj)
        {
            if (obj == null)
            {
                return;
            }
            if (obj is SCTitleResponse)
            {
                SCTitleResponse res = (SCTitleResponse)obj;
                if (res.Status == null)
                {
                    ////后面优化吧
                    List<ConfTitle> titleArray = CompareTitle(res);
                    //List<ConfTitle> titleArray = ConfTitle.array;
                    //m_grid_con.Clear();
                    m_grid_con.EnsureSize<TitleComponent>(titleArray.Count);
                    for (int i = 0; i < titleArray.Count; i++)
                    {
                        TitleComponent title = m_grid_con.GetChild<TitleComponent>(i);
                        title.Visible = true;
                        bool isLock = true;
                        TitleMsg titleMsg = null;
                        bool isChoose = false;
                        for (int j = 0; j < res.Titles.Count; j++)
                        {
                            if (titleArray[i].id == res.Titles[j].TitleId)
                            {
                                isLock = false;
                                titleMsg = res.Titles[j];
                                if (titleMsg.TitleId == m_chooseID)
                                {
                                    isChoose = true;
                                    m_curTitleMsg = title;
                                }
                                break;
                            }
                        }
                        title.SetData(titleArray[i], titleMsg, isChoose, isLock);
                    }
                }
            }
        }

        private void OnChoose(TitleComponent com, long id)
        {
            if (m_curTitleMsg != null)
            {
                m_curTitleMsg.OnUnToggle();
            }
            m_curTitleMsg = com;

        }

        private List<ConfTitle> CompareTitle(SCTitleResponse res)
        {
            List<TitleMsg> titlemsgs = new List<TitleMsg>(res.Titles);
            titlemsgs.Sort(CompareTitle);
            List<ConfTitle> titles = ConfTitle.array;
            List<ConfTitle> newTitles = new List<ConfTitle>();
            for (int i = 0; i < titlemsgs.Count; i++)
            {
                ConfTitle title = ConfTitle.Get(titlemsgs[i].TitleId);
                if (title == null)
                {
                    Debug.LogError("title id is none :" + titlemsgs[i].TitleId);
                    continue;
                }
                newTitles.Add(title);
            }
            for (int i = 0; i < titles.Count; i++)
            {
                bool hasTitle = false;
                for (int j = 0; j < titlemsgs.Count; j++)
                {
                    if (titlemsgs[j].TitleId == titles[i].id)
                    {
                        hasTitle = true;
                        break;
                    }
                }
                if (!hasTitle)
                {
                    newTitles.Add(titles[i]);
                }
            }
            return newTitles;
        }

        private int CompareTitle(TitleMsg msg01, TitleMsg msg02)
        {
            long msgFinishTime1 = msg01.GetTime;
            long msgFinishTime2 = msg02.GetTime;
            if (msgFinishTime1 < msgFinishTime2)
            {
                return 1;
            }
            if (msgFinishTime1 == msgFinishTime2)
            {
                return 0;
            }
            return -1;
        }

        public void SetChooseTitle(long title)
        {
            m_chooseID = title;
        }
    }

    public class TitleComponent : GameUIComponent
    {
        private GameLabel m_Name_lab;
        private GameLabel[] m_addition_lab;
        //private GameLabel m_additionNum01_lab;
        //private GameLabel m_addition02_lab;
        //private GameLabel m_additionNum02_lab;
        private GameLabel m_desc_lab;
        private GameImage m_lock_img;
        private GameImage m_mask_img;
        private GameImage m_bg_img;
        private GameImage m_icon_img;
        private GameImage m_require_img;
        private GameButton m_choose_btn;
        private GameLabel m_Expired_lab;
        private GameLabel m_TimeLab;

        private bool m_currentState = false;
        private TitleMsg m_curTitle = null;

        private GameLabel m_detailLab = null;

        private Color m_grayColor = new Color(0.192f, 0.192f, 0.192f);
        private Color m_oriColor = new Color(0f, 0.164f, 0.258f);
        protected override void OnInit()
        {
            base.OnInit();
            m_Name_lab = Make<GameLabel>("Text_name");
            m_addition_lab = new GameLabel[2];
            m_addition_lab[0] = Make<GameLabel>("Text_2_1");
            //m_additionNum01_lab = Make<GameLabel>("Text_2_1_1");
            m_addition_lab[1] = Make<GameLabel>("Text_2_2");
            //m_additionNum02_lab = Make<GameLabel>("Text_2_2_1");
            m_desc_lab = Make<GameLabel>("Text_3_acquisition");

            m_lock_img = Make<GameImage>("Image_unlock_2");
            m_mask_img = Make<GameImage>("Image_unlock_1");
            m_require_img = Make<GameImage>("Image_require");
            m_Expired_lab = Make<GameLabel>("Text_1");
            m_bg_img = Make<GameImage>("Button");
            m_icon_img = Make<GameImage>("RawImage");
            m_choose_btn = Make<GameButton>("Button");
            m_TimeLab = Make<GameLabel>("time");
            this.m_detailLab = Make<GameLabel>("detail");
        }

        public override void OnShow(object param)
        {
            base.OnShow(param);
            m_choose_btn.AddClickCallBack(OnToggle);
            MessageHandler.RegisterMessageHandler(MessageDefine.SCTitleActiveResponse, OnResponse);
        }

        public override void OnHide()
        {
            base.OnHide();
            m_choose_btn.RemoveClickCallBack(OnToggle);
            MessageHandler.UnRegisterMessageHandler(MessageDefine.SCTitleActiveResponse, OnResponse);
            m_curTitle = null;
            m_currentState = false;
        }

        public void SetData(ConfTitle confTitle, TitleMsg title, bool isChoose, bool isLock)
        {
            if (confTitle == null)
            {
                Visible = false;
                return;
            }
            this.m_detailLab.Text = LocalizeModule.Instance.GetString(confTitle.info);
            m_curTitle = title;
            if (confTitle != null)
            {

                m_Name_lab.Text = LocalizeModule.Instance.GetString(confTitle.name);
                List<RankData> rankdatas = PlayerTitleHelper.GetRandData(confTitle.benefit);
                m_addition_lab[0].Visible = false;
                m_addition_lab[1].Visible = false;
                for (int i = 0; i < 2; i++)
                {
                    if (i < rankdatas.Count)
                    {
                        m_addition_lab[i].Text = LocalizeModule.Instance.GetString(string.Format("RankType_{0}", rankdatas[i].type), rankdatas[i].value);
                        m_addition_lab[i].Visible = !isLock;
                    }
                }

                m_desc_lab.Text = LocalizeModule.Instance.GetString(confTitle.source);
            }
            m_icon_img.Sprite = confTitle.icon;
            bool active = !isLock;
            m_mask_img.Visible = !active;
            m_lock_img.Visible = !active;
            m_desc_lab.Visible = !active;
            m_currentState = isChoose;
            m_choose_btn.Enable = active;
            m_Expired_lab.Visible = false;
            m_Name_lab.color = m_oriColor;
            m_addition_lab[0].color = m_oriColor;
            m_addition_lab[1].color = m_oriColor;
            m_TimeLab.Visible = false;

            if (active)  //是否已拥有
            {

                long nowTicks = CommonTools.DateTimeToTimeStamp(System.DateTime.Now) / 10000;
                if (title.Deadline > 0)
                {
                    if (title.Deadline < nowTicks)
                    {
                        m_TimeLab.Visible = false;
                        //过期
                        m_Name_lab.color = m_grayColor;
                        m_addition_lab[0].color = m_grayColor;
                        //m_additionNum01_lab.color = m_grayColor;
                        m_addition_lab[1].color = m_grayColor;
                        //m_additionNum02_lab.color = m_grayColor;
                        m_bg_img.SetGray(true);
                        m_icon_img.SetGray(true);
                        m_Expired_lab.Visible = true;
                        m_choose_btn.Enable = false;
                    }
                    else
                    {
                        DateTime deadTime = CommonTools.TimeStampToDateTime(title.Deadline * 10000);
                        if (deadTime != null)
                        {
                            m_TimeLab.Text = CommonTools.SecondToTitleString((deadTime - DateTime.Now).TotalSeconds);
                        }
                        m_TimeLab.Visible = true;

                    }
                }

            }
            m_require_img.Visible = m_currentState;
        }

        private bool m_Click = false;
        private void OnToggle(GameObject obj)
        {
            EngineCoreEvents.AudioEvents.PlayAudio.SafeInvoke(Audio.AudioType.UISound,GameCustomAudioKey.role_title.ToString());
            if (m_currentState || m_curTitle == null)
            {
                //Debug.LogError("title error!!!");
                return;
            }
            m_Click = true;
            CSTitleActiveRequest req = new CSTitleActiveRequest();
            req.TitleId = m_curTitle.TitleId;

#if !NETWORK_SYNC || UNITY_EDITOR
            GameEvents.NetWorkEvents.SendHalfSyncMsg.SafeInvoke(req);
#else
            GameEvents.NetWorkEvents.SendMsg.SafeInvoke(req);
#endif

        }

        public void OnUnToggle()
        {
            m_currentState = false;
            m_require_img.Visible = false;
        }

        private void OnResponse(object obj)
        {
            if (m_Click && obj is SCTitleActiveResponse)
            {
                m_Click = false;
                m_currentState = true;
                SCTitleActiveResponse res = (SCTitleActiveResponse)obj;
                if (res.Status == null)
                {
                    m_require_img.Visible = true;
                    GameEvents.UIEvents.UI_PlayerTitle_Event.OnChoose.SafeInvoke(this, m_curTitle.TitleId);
                    Dictionary<UBSParamKeyName, object> _params = new Dictionary<UBSParamKeyName, object>()
                    {
                        { UBSParamKeyName.ContentID,m_curTitle.TitleId },
                    };
                    UserBehaviorStatisticsModules.Instance.LogEvent(UBSEventKeyName.title_use, null, _params);
                }
            }
        }

    }

}
