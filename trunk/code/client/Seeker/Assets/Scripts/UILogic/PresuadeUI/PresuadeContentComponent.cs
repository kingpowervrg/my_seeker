using System;
using System.Collections.Generic;
using EngineCore;
using UnityEngine;
using DG.Tweening;
namespace SeekerGame
{
    public enum PersuadeTalkType
    {
        Self,
        NPC
    }

    public enum PersuadeType
    {
        NormalTalk,
        PersuadeTalk,
        FeedBackTalk
    }

    public class PresuadeContentComponent : GameUIComponent
    {
        private GameLabel m_contentLab = null;
        private TextFader m_contentFader = null;
        private GameButton m_leftBtn = null;
        private GameButton m_rightBtn = null;
        private GameButton m_confutBtn = null;

        private GameTexture m_selfTex = null;
        private GameSpine m_selfSpine = null;

        private GameTexture m_npcTex = null;
        private GameSpine m_npcSpine = null;

        private GameUIComponent m_currentNpcIcon = null;
        private GameUIComponent m_currentSelfIcon = null;

        private GameUIComponent m_iconCom = null;
        private GameImage m_iconImg = null;

        private GameUIContainer m_serialIconGrid = null;
        private GameUIComponent m_Image_chat = null;
        private TweenPosition m_npcTween = null;

        private PersuadeData m_persuadeData = null;
        private PersuadeItemData m_currentItemData = null;
        private int m_currentItemIndex = -1;
        private int m_lastItemIndex = -1;

        private PresuadeChooseComponent m_chooseCom = null;
        private Dictionary<int, PresuadeRecordData> m_serialIndexDic = new Dictionary<int, PresuadeRecordData>();
        private int m_startItemIndex = -1;
        protected override void OnInit()
        {
            base.OnInit();

            this.m_selfTex = Make<GameTexture>("self");
            this.m_selfSpine = Make<GameSpine>("self_spine");

            this.m_npcTex = Make<GameTexture>("npc");
            this.m_npcSpine = Make<GameSpine>("npc_spine");

            this.m_contentLab = Make<GameLabel>("Image_chat:ContentLab");
            this.m_contentFader = this.m_contentLab.GetComponent<TextFader>();
            this.m_leftBtn = Make<GameButton>("Image_chat:btnLeft");
            this.m_rightBtn = Make<GameButton>("Image_chat:btnRight");
            this.m_serialIconGrid = Make<GameUIContainer>("Image_chat:serialIconGrid");
            this.m_Image_chat = Make<GameUIComponent>("Image_chat");
            this.m_confutBtn = Make<GameButton>("btnConfut");
            this.m_iconCom = Make<GameUIComponent>("Image_icon");
            this.m_iconImg = this.m_iconCom.Make<GameImage>("icon");
        }

        public override void OnShow(object param)
        {
            base.OnShow(param);
            this.m_leftBtn.AddClickCallBack(OnLeftMove);
            this.m_rightBtn.AddClickCallBack(OnRightMove);
            this.m_confutBtn.AddClickCallBack(OnConfutBtn);
            MessageHandler.RegisterMessageHandler(MessageDefine.SCSkyEyeRewardReq,OnRes);
            this.m_contentFader.OnComplete = OnComplete;
            GameEvents.UIEvents.UI_Presuade_Event.OnChooseId += OnChooseId;
            this.m_leftBtn.Visible = false;
        }

        public override void OnHide()
        {
            base.OnHide();
            this.m_leftBtn.RemoveClickCallBack(OnLeftMove);
            this.m_rightBtn.RemoveClickCallBack(OnRightMove);
            this.m_confutBtn.RemoveClickCallBack(OnConfutBtn);
            MessageHandler.UnRegisterMessageHandler(MessageDefine.SCSkyEyeRewardReq, OnRes);
            GameEvents.UIEvents.UI_Presuade_Event.OnChooseId -= OnChooseId;
            this.m_isInPersuadeTalk = false;
            this.m_serialIconGrid.Visible = false;
            this.m_needShowIcon = false;
            this.m_errorChoose = false;
            this.m_startItemIndex = -1;
            this.m_needShowRefut = false;
            this.m_serialIndexDic.Clear();
            this.m_currentNpcIcon.Visible = false;
            this.m_currentSelfIcon.Visible = false;
            this.m_confutBtn.Visible = false;
            this.m_iconCom.Visible = false;
            this.m_lastItemIndex = -1;
        }

        private long skyeyeId;
        public void SetSkyeyeId(long id)
        {
            skyeyeId = id;
        }

        private void OnRes(object obj)
        {
            if (obj is SCSkyEyeRewardReq)
            {
                SCSkyEyeRewardReq res = (SCSkyEyeRewardReq)obj;
                List<ResultItemData> itemDataArray = new List<ResultItemData>();
                for (int i = 0; i < res.Items.Count; i++)
                {
                    ResultItemData itemData = new ResultItemData(res.Items[i].PropId, res.Items[i].Num);
                    itemDataArray.Add(itemData);
                }
                ResultWindowData resultData = new ResultWindowData(itemDataArray);
                FrameMgr.OpenUIParams param = new FrameMgr.OpenUIParams(UIDefine.UI_GIFTRESULT);
                param.Param = resultData;
                EngineCoreEvents.UIEvent.ShowUIEventWithParam.SafeInvoke(param);
                GlobalInfo.MY_PLAYER_INFO.AddSkyEyeHasRewardById(res.SkyEyeId);
                PresuadeUILogic.Hide();
                GameEvents.UIEvents.UI_SkyEye_Event.OnSkyEyeCompleteById.SafeInvoke(this.skyeyeId);

            }
        }

        private void OnLeftMove(GameObject obj)
        {
            if (this.m_contentFader.enabled)
            {
                this.m_contentFader.enabled = false;
            }
            else
            {
                if (this.m_confutBtn.Visible)
                {
                    this.m_confutBtn.Visible = false;
                }
                if (m_errorChoose)
                {
                    m_errorChoose = false;
                    SetCurrentValue(this.m_currentItemIndex);
                    return;
                }
                int forwardIndex = this.m_persuadeData.m_items[this.m_currentItemData.forwardIndex].forwardIndex;
                if ((PersuadeType)this.m_persuadeData.m_items[forwardIndex].persuadeType == PersuadeType.NormalTalk)
                {
                    this.m_lastItemIndex = -1;
                }
                else
                {
                    this.m_lastItemIndex = forwardIndex;
                }
                SetCurrentValue(this.m_currentItemData.forwardIndex);
            }
        }

        private void OnRightMove(GameObject obj)
        {
            if (this.m_contentFader.enabled)
            {
                this.m_contentFader.enabled = false;
            }
            else
            {
                if (this.m_confutBtn.Visible)
                {
                    this.m_confutBtn.Visible = false;
                }
                if (m_errorChoose)
                {
                    m_errorChoose = false;
                    SetCurrentValue(this.m_currentItemIndex);
                    return;
                }
                if (this.m_iconCom.Visible)
                {
                    this.m_iconCom.Visible = false;
                }
                if ((PersuadeType)m_currentItemData.persuadeType == PersuadeType.PersuadeTalk)
                {
                    this.m_lastItemIndex = this.m_currentItemIndex;
                }
                if (this.m_currentItemData.nextIndex < 0)
                {
                    if ((PersuadeType)m_currentItemData.persuadeType == PersuadeType.FeedBackTalk)
                    {
                        if (NeedLoop(this.m_lastItemIndex))
                        {
                            return;
                        }
                        SetCurrentValue(m_persuadeData.m_items[this.m_lastItemIndex].nextIndex);
                    }
                    else
                    {
                        //gameover
                        CSSkyEyeRewardReq req = new CSSkyEyeRewardReq();
                        req.SkyEyeId = skyeyeId;
                        GameEvents.NetWorkEvents.SendAsyncMsg.SafeInvoke(req);
                    }
                }
                else
                {
                    if (NeedLoop(this.m_currentItemIndex))
                    {
                        return;
                    }
                    SetCurrentValue(this.m_currentItemData.nextIndex);
                }

            }
        }

        private bool NeedLoop(int currentIndex)
        {
            if ((PersuadeType)m_persuadeData.m_items[currentIndex].persuadeType == PersuadeType.PersuadeTalk
                        &&m_persuadeData.m_items[currentIndex].nextIndex >= 0
                    && (PersuadeType)m_persuadeData.m_items[m_persuadeData.m_items[currentIndex].nextIndex].persuadeType == PersuadeType.NormalTalk)
            {
                if (!GameEvents.UIEvents.UI_Presuade_Event.OnRecordComplete())
                {
                    SetCurrentValue(this.m_startItemIndex);
                    return true;
                }
            }
            return false;
        }

        private void OnConfutBtn(GameObject obj)
        {
            m_chooseCom.Visible = true;
        }

        public void SetInitData(PersuadeData persuadeData,int total, PresuadeChooseComponent chooseCom, Dictionary<int, PresuadeRecordData> serialIndexDic)
        {
            this.m_persuadeData = persuadeData;
            this.m_chooseCom = chooseCom;
            this.m_serialIndexDic = serialIndexDic;
            string npcIcon = ConfNpc.Get(persuadeData.npcId).icon;
            string npcIconSpine = CommonData.GetSpineHead(npcIcon);
            npcIcon = string.IsNullOrEmpty(npcIconSpine) ? CommonData.GetBigPortrait(npcIcon) : npcIconSpine;

            string selfIcon = "juzhang_laonianren_01_SkeletonData.asset";//GlobalInfo.MY_PLAYER_INFO.PlayerIcon;
            string selfIconSpine = CommonData.GetSpineHead(npcIcon);
            selfIcon = string.IsNullOrEmpty(selfIconSpine) ? CommonData.GetBigPortrait(selfIcon) : selfIconSpine;

            this.m_currentNpcIcon = GetCurrentIcon(this.m_npcSpine,this.m_npcTex, npcIcon);
            this.m_currentSelfIcon = GetCurrentIcon(this.m_selfSpine, this.m_selfTex, selfIcon);

            this.m_npcTween = this.m_currentNpcIcon.GetComponent<TweenPosition>();
            this.m_serialIconGrid.EnsureSize<SerialToggle>(total);
            for (int i = 0; i < total; i++)
            {
                SerialToggle t = this.m_serialIconGrid.GetChild<SerialToggle>(i);
                t.SetIndex(i);
                t.Visible = true;
            }
        }

        private Color m_specialColor = new Color(1f,0.854f,0.329f);
        private bool m_isInPersuadeTalk = false;
        private int m_persuadeIndex = -1;
        private bool m_needShowRefut = false;
        public void SetCurrentValue(int currentIndex)
        {
            this.m_currentItemIndex = currentIndex;
            PresuadeRecordData recordData = null;
            this.m_currentItemData = this.m_persuadeData.m_items[currentIndex];
            if ((PersuadeType)this.m_currentItemData.persuadeType == PersuadeType.PersuadeTalk)
            {
                if (m_persuadeIndex >= 0)
                {
                    this.m_serialIconGrid.GetChild<SerialToggle>(m_persuadeIndex).CheckMark = false;
                }
                recordData = this.m_serialIndexDic[this.m_currentItemIndex];
                if (m_serialIndexDic.ContainsKey(this.m_currentItemIndex))
                {
                    m_persuadeIndex = recordData.serialIndex;
                }
                else
                {
                    Debug.LogError("error  m_persuadeIndex : " + m_persuadeIndex);
                }
                if (!m_isInPersuadeTalk)
                {
                    m_isInPersuadeTalk = true;
                    this.m_serialIconGrid.Visible = true;
                    m_persuadeIndex = 0;
                    this.m_startItemIndex = this.m_currentItemIndex;
                }
                GameEvents.UIEvents.UI_Presuade_Event.OnNoticProgressFillEffect.SafeInvoke(true);
                this.m_serialIconGrid.GetChild<SerialToggle>(m_persuadeIndex).CheckMark = true;
                this.m_currentSelfIcon.Visible = false;
                m_needShowRefut = !recordData.isRecord;
                if (Widget.sizeDelta.x / 2f != this.m_currentNpcIcon.Widget.anchoredPosition.x)
                {
                    this.m_currentNpcIcon.Widget.DOLocalMoveX(Widget.sizeDelta.x / 2f, 0.4f).SetEase(Ease.InOutQuad).OnComplete(() =>
                    {
                        //this.m_confutBtn.Visible = !recordData.isRecord;
                    });
                }

                if (this.m_currentItemData.forwardIndex >= 0 && (PersuadeType)this.m_persuadeData.m_items[this.m_currentItemData.forwardIndex].persuadeType == PersuadeType.PersuadeTalk)
                {
                    this.m_leftBtn.Visible = true;
                }
                else
                {
                    this.m_leftBtn.Visible = false;
                }
            }
            else
            {
                this.m_leftBtn.Visible = false;
                if (this.m_lastItemIndex >= 0 && (PersuadeType)this.m_persuadeData.m_items[this.m_lastItemIndex].persuadeType == PersuadeType.PersuadeTalk)
                {
                    m_needShowRefut = false;
                    this.m_confutBtn.Visible = false;
                    this.m_currentNpcIcon.Widget.DOAnchorPos3D(this.m_npcTween.To, 0.4f).SetEase(Ease.InOutQuad);
                }
                if ((PersuadeTalkType)this.m_currentItemData.talkType == PersuadeTalkType.NPC)
                {
                    if (!this.m_currentNpcIcon.Visible)
                    {
                        this.m_currentNpcIcon.Visible = true;
                    }
                }
                if ((PersuadeTalkType)this.m_currentItemData.talkType == PersuadeTalkType.Self)
                {
                    if (!this.m_currentSelfIcon.Visible)
                    {
                        this.m_currentSelfIcon.Visible = true;
                    }
                    if (this.m_needShowIcon)
                    {
                        this.m_needShowIcon = false;
                        this.m_iconCom.Visible = true;
                    }
                }
            }
            
            if (!this.m_Image_chat.Visible)
            {
                this.m_Image_chat.Visible = true;
            }
            SetContentLab(m_currentItemData.content,recordData != null && recordData.isRecord);
        }

        private void SetContentLab(string content,bool isGray)
        {
            if (m_errorChoose)
            {
                this.m_contentLab.color = Color.red;
            }
            else if (isGray)
            {
                this.m_contentLab.color = Color.gray;
            }
            else
            {
                this.m_contentLab.color = (PersuadeType)this.m_currentItemData.persuadeType == PersuadeType.PersuadeTalk ? m_specialColor : Color.white;
            }
            this.m_contentFader.enabled = true;
            this.m_contentLab.Text = LocalizeModule.Instance.GetString(content);
        }

        private void OnComplete()
        {
            if (m_needShowRefut)
            {
                this.m_confutBtn.Visible = m_needShowRefut;
            }
            else if (this.m_confutBtn.Visible != m_needShowRefut)
            {
                this.m_confutBtn.Visible = m_needShowRefut;
            }
        }

        private bool m_needShowIcon = false;
        private bool m_errorChoose = false;
        private void OnChooseId(long chooseId)
        {
            m_needShowRefut = false;
            this.m_confutBtn.Visible = false;
            //this.m_iconCom.Visible = true;
            if (chooseId == this.m_persuadeData.m_items[this.m_currentItemIndex].evidenceID)
            {
                //sure
                this.m_serialIconGrid.GetChild<SerialToggle>(m_persuadeIndex).SetState(true);
                this.m_iconImg.Sprite = ConfProp.Get(chooseId).icon;
                this.m_needShowIcon = true;
                this.m_lastItemIndex = this.m_currentItemIndex;
                this.m_serialIndexDic[this.m_currentItemIndex].isRecord = true;
                GameEvents.UIEvents.UI_Presuade_Event.OnRecordByIndex.SafeInvoke(this.m_currentItemIndex);
                SetCurrentValue(this.m_currentItemData.feedbackIndex);
            }
            else
            {
                //error
                GameEvents.UIEvents.UI_Presuade_Event.OnNoticProgressFillEffect.SafeInvoke(false);
                m_errorChoose = true;
                SetContentLab("persuade_tips", false);
            }
        }

        private GameUIComponent GetCurrentIcon(GameSpine spine,GameTexture tex,string headIcon)
        {
            GameUIComponent currentIcon = null;
            if (headIcon.Contains(".png"))
            {
                tex.TextureName = headIcon;
                currentIcon = tex;
            }
            else
            {
                spine.SpineName = headIcon;
                currentIcon = spine;
            }
            return currentIcon;
        }

        public class SerialToggle : GameUIComponent
        {
            private GameImage m_togImg = null;
            private TweenAlpha[] m_alpha = null;
            private bool m_hasConfut = false;
            protected override void OnInit()
            {
                base.OnInit();
                this.m_togImg = Make<GameImage>(gameObject);
                this.m_alpha = GetComponents<TweenAlpha>();
            }

            public override void OnHide()
            {
                base.OnHide();
                this.m_isCheck = false;
                this.m_hasConfut = false;
                ReflashTog();
            }

            public void SetIndex(int index)
            {
                this.m_alpha[0].Delay = index * 0.1f;
                this.m_alpha[1].Delay = index * 0.1f;
            }

            public void SetState(bool hasConfut)
            {
                this.m_hasConfut = hasConfut;
                ReflashTog();
            }

            private void PlayCheckMarkAnimator(bool forward)
            {
                float targetScale = 1.2f;
                if (!forward)
                {
                    targetScale = 1f;
                }
                this.m_togImg.Widget.DOScale(targetScale, 0.4f).SetEase(Ease.InOutQuad);
            }

            private void ReflashTog()
            {
                if (m_isCheck)
                {
                    if (this.m_hasConfut)
                    {
                        this.m_togImg.Sprite = "checkbox_slected_type2_2.png";
                    }
                    else
                    {
                        this.m_togImg.Sprite = "checkbox_slected_type2_4.png";
                    }
                }
                else
                {
                    if (this.m_hasConfut)
                    {
                        this.m_togImg.Sprite = "checkbox_slected_type2_1.png";
                    }
                    else
                    {
                        this.m_togImg.Sprite = "checkbox_slected_type2_3.png";
                    }
                }
            }

            private bool m_isCheck = false;

            public bool CheckMark
            {
                set {
                    this.m_isCheck = value;
                    ReflashTog();
                    PlayCheckMarkAnimator(this.m_isCheck);
                }
                get { return this.m_isCheck; }
            }
        }
    }

    public class PresuadeProgressComponent : GameUIComponent
    {
        private GameProgressBar m_progressBar = null;
        private GameUIContainer m_lineGrid = null;
        private GameLabel m_titleLab = null;
        private GameImage m_fillEffect = null;
        private GameUIEffect m_sucessEffect = null;
        private GameUIEffect m_failEffect = null;
        private TweenAlpha m_tweenAlpha = null;
        private float width = 340f;
        private int m_currentLen = 0;
        private int m_totalLen = 0;
        protected override void OnInit()
        {
            base.OnInit();
            this.m_titleLab = Make<GameLabel>("Text");
            this.m_progressBar = Make<GameProgressBar>("Slider");
            this.m_lineGrid = this.m_progressBar.Make<GameUIContainer>("lineGrid");
            this.m_fillEffect = this.m_progressBar.Make<GameImage>("fillEffect");
            this.m_tweenAlpha = this.m_fillEffect.GetComponent<TweenAlpha>();
            this.m_sucessEffect = this.m_progressBar.Make<GameUIEffect>("UI_fanbo_chenggong");
            this.m_failEffect = this.m_progressBar.Make<GameUIEffect>("UI_fanbo_shibai");
        }

        public override void OnShow(object param)
        {
            base.OnShow(param);
            GameEvents.UIEvents.UI_Presuade_Event.OnRecordByIndex += OnRecordByIndex;
            GameEvents.UIEvents.UI_Presuade_Event.OnRecordComplete = OnRecordComplete;
            GameEvents.UIEvents.UI_Presuade_Event.OnNoticProgressFillEffect += OnNoticProgressFillEffect;
            this.m_sucessEffect.EffectPrefabName = "UI_fanbo_chenggong.prefab";
            this.m_failEffect.EffectPrefabName = "UI_fanbo_shibai.prefab";
            this.m_sucessEffect.Visible = false;
            this.m_failEffect.Visible = false;
            this.m_fillEffect.Visible = false;
        }

        public override void OnHide()
        {
            base.OnHide();
            this.m_currentLen = 0;
            this.m_fillEffect.Visible = false;
            this.m_sucessEffect.Visible = false;
            this.m_failEffect.Visible = false;
            GameEvents.UIEvents.UI_Presuade_Event.OnRecordByIndex -= OnRecordByIndex;
            GameEvents.UIEvents.UI_Presuade_Event.OnNoticProgressFillEffect -= OnNoticProgressFillEffect;
        }

        private float m_fillWidth = 0f;
        public void SetData(int total,string title)
        {
            this.m_totalLen = total;
            this.m_titleLab.Text = LocalizeModule.Instance.GetString(title);
            this.m_progressBar.Value = 0;
            float itemWidth = width / total;
            this.m_lineGrid.EnsureSize<GameUIComponent>(total - 1);
            Vector2 startPos = this.m_lineGrid.ContainerTemplate.GetComponent<RectTransform>().anchoredPosition;
            m_fillWidth = itemWidth - 2f;
            this.m_fillEffect.Widget.sizeDelta = new Vector2(m_fillWidth, this.m_fillEffect.Widget.sizeDelta.y);
            this.m_fillEffect.Widget.anchoredPosition = startPos + Vector2.right * 2f;
            for (int i = 0; i < total - 1; i++)
            {
                GameUIComponent com = this.m_lineGrid.GetChild<GameUIComponent>(i);
                
                com.Widget.anchoredPosition = startPos + itemWidth * (i + 1) * Vector2.right;
                com.Visible = true;
            }
        }

        private void OnRecordByIndex(int index)
        {
            this.m_fillEffect.Visible = false;
            m_currentLen++;
            float startValue = this.m_progressBar.Value;
            float endValue = m_currentLen / (float)m_totalLen;
            this.m_sucessEffect.Widget.anchoredPosition = this.m_progressBar.Value * this.m_progressBar.Widget.sizeDelta.x * Vector2.right;
            this.m_sucessEffect.ReplayEffect();
            DOTween.To(x => this.m_progressBar.Value = x, startValue, endValue, 0.4f).OnUpdate(()=> {
                this.m_sucessEffect.Widget.anchoredPosition = this.m_progressBar.Value * this.m_progressBar.Widget.sizeDelta.x * Vector2.right;
            }).OnComplete(()=> {
                OnNoticProgressFillEffect(true);
            });
        }

        private void OnNoticProgressFillEffect(bool effect)
        {
            if (m_currentLen >= m_totalLen)
                return;
            if (effect)
            {
                if (m_currentLen > 0)
                {
                    GameUIComponent template = this.m_lineGrid.GetChild<GameUIComponent>(m_currentLen - 1);
                    this.m_fillEffect.Widget.anchoredPosition = template.Widget.anchoredPosition;
                }
                this.m_fillEffect.Widget.sizeDelta = m_fillWidth * Vector2.right + this.m_fillEffect.Widget.sizeDelta.y * Vector2.up;
                this.m_fillEffect.Color = Color.white;
                this.m_tweenAlpha.ResetAndPlay();
                this.m_fillEffect.Visible = true;
            }
            else
            {
                float width = this.m_fillEffect.Widget.sizeDelta.x;
                this.m_tweenAlpha.SetTweenCompleted();
                this.m_tweenAlpha.Stop();
                this.m_fillEffect.Color = Color.red;
                float tempWidth = 0f;
                DOTween.To(x => tempWidth = x, width, 0, 0.8f).OnUpdate(()=> {
                    this.m_fillEffect.Widget.sizeDelta = Vector2.up * this.m_fillEffect.Widget.sizeDelta.y + Vector2.right * tempWidth;
                    this.m_failEffect.Widget.anchoredPosition = this.m_fillEffect.Widget.anchoredPosition + Vector2.right * tempWidth;
                    this.m_failEffect.Visible = true;
                }).OnComplete(()=> {
                    this.m_failEffect.Visible = false;
                });
            }
           
        }

        private bool OnRecordComplete()
        {
            return m_currentLen == m_totalLen;
        }
    }
}
