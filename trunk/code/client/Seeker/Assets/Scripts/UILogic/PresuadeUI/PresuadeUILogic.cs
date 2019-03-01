using System;
using System.Collections.Generic;
using EngineCore;
using UnityEngine;

namespace SeekerGame
{
    [UILogicHandler(UIDefine.UI_persuade_Ingame)]
    public class PresuadeUILogic : UILogicBase
    {
        private PresuadeContentComponent m_chatCom = null;
        private PresuadeProgressComponent m_progressCom = null;
        private PresuadeChooseComponent m_chooseCom = null;

        private SkyeyeData m_skyeyeData = null;
        private PersuadeData m_persuadeData;
        private int m_totalConfute = 0; //反驳个数
        private int m_totalMainContent = 0; //主线个数
        private GameUIComponent m_downCom = null;
        private GameUIComponent m_topCom = null;
        private GameLabel m_describleLab = null;
        private TextFader m_describleFader = null;
        private GameUIComponent m_bgBtn = null;
        private GameButton m_btnQuit = null;
        private Dictionary<int, PresuadeRecordData> m_serialIndexDic = new Dictionary<int, PresuadeRecordData>();
        protected override void OnInit()
        {
            base.OnInit();
            
            this.m_chatCom = Make<PresuadeContentComponent>("Person");
            this.m_progressCom = Make<PresuadeProgressComponent>("Panel_topleft");
            this.m_chooseCom = Make<PresuadeChooseComponent>("Panel");

            this.m_bgBtn = Make<GameUIComponent>("RawImage_bg");
            this.m_downCom = Make<GameUIComponent>("RawImage_bg:Down");
            this.m_topCom = Make<GameUIComponent>("RawImage_bg:Top");
            this.m_describleLab = Make<GameLabel>("RawImage_bg:Text");
            this.m_btnQuit = Make<GameButton>("Button_pause");
            this.m_describleFader = this.m_describleLab.GetComponent<TextFader>();
        }

        public override void OnShow(object param)
        {
            base.OnShow(param);
            m_skyeyeData = (SkyeyeData)param;
            this.m_chatCom.SetSkyeyeId(m_skyeyeData.skyeyeId);
            m_persuadeData = ClientConfigManager.Instance.GetPersuadeByID((int)m_skyeyeData.persuadeId);
            this.m_describleLab.Text = LocalizeModule.Instance.GetString(m_persuadeData.introduce);
            this.m_describleFader.OnComplete = TextFadeComplete;
            GetTotalProgress();
            this.m_chooseCom.SetData(m_persuadeData.evidenceIds);
            this.m_chatCom.SetInitData(this.m_persuadeData,this.m_totalMainContent, this.m_chooseCom, m_serialIndexDic);
            this.m_progressCom.SetData(this.m_totalConfute,LocalizeModule.Instance.GetString(m_persuadeData.name));
            this.m_chatCom.SetCurrentValue(0);
            this.m_bgBtn.AddClickCallBack(BtnSkipDescrible);
            this.m_btnQuit.AddClickCallBack(Quit);
            SkipDescrible(false);
        }
        public override FrameDisplayMode UIFrameDisplayMode
        {
            get
            {
                return FrameDisplayMode.FULLSCREEN;
            }
        }
        public override void OnHide()
        {
            base.OnHide();
            this.m_bgBtn.RemoveClickCallBack(BtnSkipDescrible);
            this.m_btnQuit.RemoveClickCallBack(Quit);
            this.m_chatCom.Visible = false;
            this.m_progressCom.Visible = false;
            this.m_chooseCom.Visible = false;
            this.m_btnQuit.Visible = false;
            this.m_hasSkip = false;
            this.m_describleFader.enabled = true;
            //SkipDescrible(false);
        }

        private bool m_hasSkip = false;
        private void BtnSkipDescrible(GameObject obj)
        {
            if (m_hasSkip)
            {
                return;
            }
            if (this.m_describleFader.enabled)
            {
                this.m_describleFader.enabled = false;
                //TextFadeComplete();
            }
            else
            {
                TimeModule.Instance.RemoveTimeaction(TextFadeCompleteTimeOut);
                m_hasSkip = true;
                SkipDescrible(true);
            }
        }

        private void SkipDescrible(bool skip)
        {
            this.m_downCom.Visible = !skip;
            this.m_topCom.Visible = !skip;
            this.m_describleLab.Visible = !skip;
            
            if (skip)
            {
                TimeModule.Instance.SetTimeout(()=> {
                    this.m_btnQuit.Visible = true;
                    this.m_chatCom.Visible = true;
                    this.m_progressCom.Visible = true;
                },0.4f);
            }
        }

        private void Quit(GameObject obj)
        {
            if (this.m_chooseCom.Visible)
            {
                this.m_chooseCom.Visible = false;
            }
            else
            {
                PopUpManager.OnCloseSureUI("persuade_exit_tips", "persuade_exit_ta", "persuade_exit_quit",()=> {
                    PresuadeUILogic.Hide();
                });
                //Hide();
            }
        }

        private void TextFadeComplete()
        {
            TimeModule.Instance.SetTimeout(TextFadeCompleteTimeOut, 1f);
        }

        private void TextFadeCompleteTimeOut()
        {
            m_hasSkip = true;
            SkipDescrible(true);
        }

        public static void Show(long skyEyeId, long persuadeId)
        {
            SkyeyeData skydata = new SkyeyeData(skyEyeId,persuadeId);
            FrameMgr.OpenUIParams param = new FrameMgr.OpenUIParams(UIDefine.UI_persuade_Ingame);
            param.Param = skydata;
            EngineCoreEvents.UIEvent.ShowUIEventWithParam.SafeInvoke(param);
        }

        public static void Hide()
        {
            EngineCoreEvents.UIEvent.HideUIEvent.SafeInvoke(UIDefine.UI_persuade_Ingame);
        }

        private void GetTotalProgress()
        {
            m_totalConfute = 0;
            this.m_totalMainContent = 0;
            int index = 0;
            do
            {
                if (m_persuadeData.m_items[index].persuadeType == 1 && m_persuadeData.m_items[index].evidenceID > 0)
                {
                    m_totalConfute++;
                }
                if (m_persuadeData.m_items[index].persuadeType == 1)
                {
                    m_serialIndexDic.Add(index, new PresuadeRecordData(m_totalMainContent));
                    this.m_totalMainContent++;
                }
                index = m_persuadeData.m_items[index].nextIndex;
            }
            while (index >= 0);
        }
    }

    public class PresuadeRecordData
    {
        public int serialIndex;
        public bool isRecord = false;

        public PresuadeRecordData(int index)
        {
            this.serialIndex = index;
            this.isRecord = false;
        }
    }
}

public class SkyeyeData
{
    public long skyeyeId;
    public long persuadeId;

    public SkyeyeData(long skyeyeId, long persuadeId)
    {
        this.skyeyeId = skyeyeId;
        this.persuadeId = persuadeId;
    }
}
