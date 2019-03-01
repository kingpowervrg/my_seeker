using EngineCore;
using System.Collections.Generic;
using UnityEngine;

namespace SeekerGame
{
    public enum GuidStartCartoonType
    {
        NORMAL,
        HEAD,
        NAME
    }

    public class GuidStartCartoonCapter : GameUIComponent
    {
        private GuidStartCartoonPage[] m_pages;
        private int m_CurrentIndex = 0;
        private int m_NextIndex = 0;
        private float m_screenX;
        private long m_guidID = -1;
        protected override void OnInit()
        {
            base.OnInit();
            int count = Widget.childCount;
            m_pages = new GuidStartCartoonPage[count];
            for (int i = 1; i <= count; i++)
            {
                m_pages[i - 1] = Make<GuidStartCartoonPage>("Panel_" + i);
                m_pages[i - 1].Visible = false;
            }
        }

        public override void OnShow(object param)
        {
            base.OnShow(param);
            GameEvents.UIEvents.UI_StartCartoon_Event.OnNext += OnNext;
            GameEvents.UIEvents.UI_StartCartoon_Event.OnNextKeyCapter += OnNextKeyCapter;
            for (int i = 0; i < m_pages.Length; i++)
            {
                m_pages[i].Visible = false;
            }
            ShowPage();
        }
        public override void OnHide()
        {
            base.OnHide();
            m_CurrentIndex = 0;
            m_NextIndex = 0;
            m_guidID = -1;
            GameEvents.UIEvents.UI_StartCartoon_Event.OnNext -= OnNext;
            GameEvents.UIEvents.UI_StartCartoon_Event.OnNextKeyCapter -= OnNextKeyCapter;
        }

        public void SetIndex(int curIndex, int nextIndex, float screenX, long guidID)
        {
            m_CurrentIndex = curIndex;
            m_NextIndex = nextIndex;
            this.m_screenX = screenX;
            this.m_guidID = guidID;
        }

        private void OnNext()
        {
            ShowPage();
        }

        private void ShowPage()
        {
            if (m_CurrentIndex >= m_pages.Length)
            {
                return;
            }
            if (m_CurrentIndex > 0)
            {
                m_pages[m_CurrentIndex - 1].Visible = false;
            }
            m_pages[m_CurrentIndex].SetIsEnd(m_CurrentIndex >= m_pages.Length - 1, m_guidID);
            GameEvents.UIEvents.UI_StartCartoon_Event.OnNextBtnVisible.SafeInvoke(true, 1);
            if (m_CurrentIndex == 2)
            {
                GameEvents.UIEvents.UI_StartCartoon_Event.OnNextBtnVisible.SafeInvoke(false, 1);
                m_pages[m_CurrentIndex].SetType(GuidStartCartoonType.HEAD, m_screenX, m_guidID);
                Debug.Log("start head ==");
            }
            else if (m_CurrentIndex == 5)
            {
                TimeModule.Instance.SetTimeout(() => EngineCoreEvents.AudioEvents.PlayAudio.SafeInvoke(Audio.AudioType.UISound, GameCustomAudioKey.knock.ToString()), 1.0f);
                //EngineCoreEvents.AudioEvents.PlayAudio.SafeInvoke(Audio.AudioType.UISound,GameCustomAudioKey.knock.ToString(), null);
            }
            else if (m_CurrentIndex == 7)
            {
                GameEvents.UIEvents.UI_StartCartoon_Event.OnNextBtnVisible.SafeInvoke(false, 1);
                m_pages[m_CurrentIndex].SetType(GuidStartCartoonType.NAME, m_screenX, m_guidID);
                Debug.Log("start Name ==");
            }
            m_pages[m_CurrentIndex].Visible = true;
            m_CurrentIndex++;

        }

        private void OnNextKeyCapter()
        {
            if (m_CurrentIndex != m_NextIndex)
            {
                m_pages[m_CurrentIndex - 1].Visible = false;
                if (m_NextIndex <= 0)
                {
                    GameEvents.UI_Guid_Event.OnGuidNewEnd.SafeInvoke(m_guidID);
                }
                else
                    m_CurrentIndex = m_NextIndex - 1;
                ShowPage();
                GameEvents.UIEvents.UI_StartCartoon_Event.OnPageChange.SafeInvoke(m_CurrentIndex);
            }

        }



        public class GuidStartCartoonPage : GameUIComponent
        {
            //private GameButton m_NextBtn;
            private bool m_IsEnd = false;
            private GuidStartCartoonType cartoonType = GuidStartCartoonType.NORMAL;

            private GameUIContainer m_HeadCon;
            private TweenScale m_TweenHead;
            private GridValue m_gridValue;
            private string m_HeadName = string.Empty;
            private GameNetworkRawImage m_IconTex;
            private TweenScale m_IconTween;

            private GameInputField m_InputName;
            private GameTexture bgTexture;

            private StartCartoonDelayTime[] m_delayTime;
            private StartCartoonDelayTime m_maxTimeChapter;
            private int m_tweenIndex = 1;
            private long m_GuidID;
            private GameUIEffect m_effect = null;
            private float m_startTime = 0f;
            protected override void OnInit()
            {
                base.OnInit();
                //m_NextBtn = Make<GameButton>("Button_next");
                this.m_maxTimeChapter = GetComponent<StartCartoonDelayTime>();
                this.m_delayTime = GetComponents<StartCartoonDelayTime>();
            }

            public void SetType(GuidStartCartoonType type, float m_screenX, long ID)
            {
                m_GuidID = ID;
                cartoonType = type;

                if (type == GuidStartCartoonType.HEAD)
                {
                    //GameEvents.UIEvents.UI_StartCartoon_Event.OnNextBtnVisible.SafeInvoke(false,0);

                    ENUM_LOGIN_TYPE loginType = ENUM_LOGIN_TYPE.E_GUEST;
                    if (null != PlayerPrefTool.GetUsername(ENUM_LOGIN_TYPE.E_THIRD))
                    {
                        loginType = ENUM_LOGIN_TYPE.E_THIRD;
                    }

                    int headCount = CommonData.CartoonHEAD.Count;
                    Transform headRoot = gameObject.transform.Find("Panel (1)/Panel");
                    this.m_TweenHead = headRoot.GetComponent<TweenScale>();
                    this.m_IconTex = Make<GameNetworkRawImage>("icon");
                    this.m_IconTween = m_IconTex.GetComponent<TweenScale>();

                    Material mat = new Material(ShaderModule.Instance.GetShader("SeekerGame/ImageBlur"));
                    mat.SetFloat("_BlurRadius", 1.5f);
                    bgTexture = Make<GameTexture>("RawImage");
                    bgTexture.RawImage.material = mat;

                    m_HeadCon = Make<GameUIContainer>("Panel:grid");
                    m_gridValue = m_HeadCon.GetComponent<GridValue>();
                    float gridWidth = headCount * 150 + m_gridValue.m_spacing * (headCount - 1);
                    m_HeadCon.Widget.sizeDelta = new Vector2(gridWidth, m_HeadCon.Widget.sizeDelta.y);
                    m_HeadCon.EnsureSize<GuidHeadUILogic>(headCount);
                    for (int i = 0; i < headCount; i++)
                    {
                        GuidHeadUILogic head = m_HeadCon.GetChild<GuidHeadUILogic>(i);
                        head.SetData(m_screenX, i, m_gridValue.m_spacing, loginType);
                        head.Visible = true;
                    }

                }
                else if (type == GuidStartCartoonType.NAME)
                {
                    m_InputName = Make<GameInputField>("Panel (1):RawImage:InputField");
                    m_InputName.Text = "Guest_" + GlobalInfo.MY_PLAYER_ID;
                    m_effect = Make<GameUIEffect>("Panel (1):RawImage:Effect:UI_xinshouyindao_shou");
                    m_effect.EffectPrefabName = "UI_xinshouyindao_shou.prefab";
                    m_effect.Visible = true;
                    GameEvents.UIEvents.UI_StartCartoon_Event.OnNextBtnVisible.SafeInvoke(false, 1);

                    m_InputName.AddClickCallBack(InputClick);
                }
            }

            private void InputClick(GameObject obj)
            {
                m_effect.Visible = false;
                m_InputName.RemoveClickCallBack(InputClick);
            }

            public override void OnShow(object param)
            {
                base.OnShow(param);
                GameEvents.UI_Guid_Event.OnStartCartoonSelectHead += OnStartCartoonSelectHead;
                GameEvents.UIEvents.UI_StartCartoon_Event.OnNextCapter += OnNextCapter;
                MessageHandler.RegisterMessageHandler(MessageDefine.SCPlayerRenewIconResp, OnResponse);
                MessageHandler.RegisterMessageHandler(MessageDefine.SCRenameResponse, OnResponse);
                this.m_startTime = Time.time;
                //TimeModule.Instance.SetTimeout(ChapterTimeout, this.m_maxTimeChapter.m_delayTime);
            }

            public void SetIsEnd(bool isEnd, long guidID)
            {
                this.m_GuidID = guidID;
                m_IsEnd = isEnd;
            }

            private void OnNextBtn(GameObject obj)
            {
                //TimeModule.Instance.RemoveTimeaction(ChapterTimeout);
                EngineCoreEvents.AudioEvents.PlayAudio.SafeInvoke(Audio.AudioType.UISound, EngineCommonAudioKey.Button_Click_Common.ToString());
                if (m_IsEnd)
                {
                    GameEvents.UI_Guid_Event.OnGuidNewEnd.SafeInvoke(m_GuidID);
                    //GameEvents.UI_Guid_Event.OnStartGuidCartoonOver.SafeInvoke();
                    //GameEvents.UIEvents.UI_StartCartoon_Event.OnFinish.SafeInvoke();
                }
                else
                {
                    if (cartoonType == GuidStartCartoonType.HEAD)
                    {
                        if (!string.IsNullOrEmpty(m_HeadName))
                        {
                            CSPlayerRenewIconReq changeIconReq = new CSPlayerRenewIconReq();
                            changeIconReq.NewIcon = m_HeadName;
                            GameEvents.NetWorkEvents.SendMsg.SafeInvoke(changeIconReq);
                            GameEvents.UI_Guid_Event.OnGuidNewEnd.SafeInvoke(m_GuidID);
                            GameEvents.UIEvents.UI_StartCartoon_Event.OnNext.SafeInvoke();
                        }
                        else
                        {
                            PopUpManager.OpenNormalOnePop(LocalizeModule.Instance.GetString("guid_photo_no"));
                            //Debug.Log("Facebook Login,don't replace icon");
                            //GameEvents.UIEvents.UI_StartCartoon_Event.OnNext.SafeInvoke();
                            //GameEvents.UI_Guid_Event.OnGuidNewEnd.SafeInvoke(m_GuidID);
                            //GameEvents.UI_Guid_Event.OnStartGuidCartoonNext.SafeInvoke();
                        }
                    }
                    else if (cartoonType == GuidStartCartoonType.NAME)
                    {
                        string inputName = m_InputName.Text;
                        if (string.IsNullOrEmpty(inputName))
                        {
                            PopUpManager.OpenNormalOnePop(LocalizeModule.Instance.GetString("UI_PLAYERINFO_NICK_NULL"));
                            return;
                        }
                        if (EngineCore.Utility.CommonUtils.GetStringCount(inputName) > 20)
                        {
                            PopUpManager.OpenNormalOnePop(LocalizeModule.Instance.GetString("UI_PLAYERINFO_NICK_OVERFLOW"));
                            return;
                        }
                        CSRenameRequest c2sRename = new CSRenameRequest();
                        c2sRename.NewName = inputName;
                        c2sRename.PlayerId = GlobalInfo.MY_PLAYER_ID;
                        GameEvents.NetWorkEvents.SendMsg.SafeInvoke(c2sRename);

                        GameEvents.UI_Guid_Event.OnGuidNewEnd.SafeInvoke(m_GuidID);
                        GameEvents.UIEvents.UI_StartCartoon_Event.OnNext.SafeInvoke();
                    }
                    else
                    {
                        GameEvents.UIEvents.UI_StartCartoon_Event.OnNext.SafeInvoke();
                    }
                }
            }

            private void OnStartCartoonSelectHead(string headName, string cartoonName, Vector3 worldPos)
            {
                GameLabel headNameLab = Make<GameLabel>("guide_p3_1");
                headNameLab.Text = LocalizeModule.Instance.GetString("guide_p3_2");
                GameEvents.UIEvents.UI_StartCartoon_Event.OnNextBtnVisible.SafeInvoke(true, 0);
                //m_NextBtn.Visible = true;
                m_HeadName = headName;
                bgTexture.RawImage.material.SetFloat("_BlurRadius", 0f);
                if (this.m_TweenHead != null)
                {
                    this.m_TweenHead.PlayForward();
                }
                if (this.m_IconTex != null)
                {
                    this.m_IconTex.TextureName = cartoonName;
                    this.m_IconTween.From = gameObject.transform.InverseTransformPoint(worldPos);
                    this.m_IconTex.Visible = true;
                    this.m_IconTween.PlayForward();
                }
            }

            private void OnResponse(object obj)
            {
                if (obj is SCPlayerRenewIconResp && cartoonType == GuidStartCartoonType.HEAD)
                {
                    SCPlayerRenewIconResp res = (SCPlayerRenewIconResp)obj;
                    if (res.Result == 1)
                    {

                        //GameEvents.UI_Guid_Event.OnStartGuidCartoonNext.SafeInvoke();
                    }
                    else
                    {
                        Debug.LogError("change head icon error");
                    }
                }
                else if (obj is SCRenameResponse)
                {
                    SCRenameResponse res = (SCRenameResponse)obj;
                    if (res.Result == 3 || !MsgStatusCodeUtil.OnError(res.Result))
                    {
                        GlobalInfo.MY_PLAYER_INFO.PlayerNickName = res.NewName;
                        PlayerInfoManager.OnPlayerInfoUpdatedEvent(GlobalInfo.MY_PLAYER_INFO);
                    }
                }
            }

            //private void ChapterTimeout()
            //{
            //    this.m_timeout = true;
            //}

            public override void OnHide()
            {
                base.OnHide();
                if (m_effect != null)
                {
                    m_effect.Visible = false;
                }
                if (m_InputName != null)
                {
                    m_InputName.RemoveClickCallBack(InputClick);
                }
                GameEvents.UI_Guid_Event.OnStartCartoonSelectHead -= OnStartCartoonSelectHead;
                GameEvents.UIEvents.UI_StartCartoon_Event.OnNextCapter -= OnNextCapter;
                MessageHandler.UnRegisterMessageHandler(MessageDefine.SCPlayerRenewIconResp, OnResponse);
                MessageHandler.UnRegisterMessageHandler(MessageDefine.SCRenameResponse, OnResponse);
                headClickNum = 0;
            }

            private int headClickNum = 0;
            private void OnNextCapter()
            {
                if (cartoonType == GuidStartCartoonType.HEAD)
                {
                    headClickNum++;
                    if (headClickNum == 2)
                    {
                        OnNextBtn(null);
                    }
                }
                else if (Time.time - this.m_startTime > m_maxTimeChapter.m_delayTime)
                {
                    OnNextBtn(null);
                }
                else
                {
                    SetTweenFinish();
                }
            }

            private void SetTweenFinish()
            {
                if (m_tweenIndex >= this.m_delayTime.Length)
                {
                    OnNextBtn(null);
                    return;
                }
                GetCurrentIndex();
                //Debug.Log("skip index == " + m_tweenIndex);
                UITweenerBase[] tweener = this.m_delayTime[m_tweenIndex].GetComponentsInChildren<UITweenerBase>();
                for (int i = 0; i < tweener.Length; i++)
                {
                    if (tweener[i].name.Contains("skip"))
                    {
                        continue;
                    }
                    tweener[i].SetTweenCompleted();
                }
                m_tweenIndex++;
                SetTweenDelayTime();
            }

            private void GetCurrentIndex()
            {
                for (int i = m_tweenIndex; i < this.m_delayTime.Length - 1; i++)
                {
                    if (Time.time - this.m_startTime < this.m_delayTime[i + 1].m_delayTime)
                    {
                        this.m_tweenIndex = i;
                        return;
                    }
                }
                this.m_tweenIndex = this.m_delayTime.Length - 1;
            }

            private void SetTweenDelayTime()
            {
                if (m_tweenIndex >= this.m_delayTime.Length)
                {
                    return;
                }
                UITweenerBase[] tweener = this.m_delayTime[m_tweenIndex].GetComponentsInChildren<UITweenerBase>();
                for (int i = 0; i < tweener.Length; i++)
                {
                    float delay = tweener[i].Delay;
                    delay -= this.m_delayTime[this.m_tweenIndex].m_delayTime;
                    tweener[i].Delay = delay > 0 ? delay : 0;
                }
            }
        }

        private class GuidHeadUILogic : GameUIComponent
        {
            private GameNetworkRawImage m_tex;
            private TweenScale m_TweenPos;
            private int m_Index;
            private GameToggleButton m_Tog;
            private GameButton m_btnSure;
            private GameUIEffect m_effect = null;
            private ENUM_LOGIN_TYPE m_loginType;
            protected override void OnInit()
            {
                base.OnInit();
                this.m_tex = Make<GameNetworkRawImage>(gameObject);
                this.m_Tog = Make<GameToggleButton>(gameObject);
                this.m_btnSure = Make<GameButton>("sure");
                this.m_effect = this.m_btnSure.Make<GameUIEffect>("UI_manhua_tishi");
                this.m_btnSure.Visible = false;
                this.m_TweenPos = GetComponent<TweenScale>();
            }

            public override void OnShow(object param)
            {
                base.OnShow(param);
                this.m_Tog.AddChangeCallBack(Toggle);
                this.m_btnSure.AddClickCallBack(OnSure);
                this.m_effect.EffectPrefabName = "UI_manhua_tishi.prefab";
                this.m_effect.Visible = true;
                if (m_Index >= 0)
                {
                    m_tex.TextureName = CommonData.CartoonHEAD[m_Index];
                }
                else
                {
                    m_tex.TextureName = GlobalInfo.MY_PLAYER_INFO.NetWorkIcon;
                }
                m_tex.Color = Color.gray;
                if (m_loginType == ENUM_LOGIN_TYPE.E_GUEST && m_Index == 0 || m_Index == -1)
                {
                    this.m_Tog.Checked = true;
                }
                GameEvents.UIEvents.UI_StartCartoon_Event.OnNextCapter += OnNextKeyCapter;
            }

            public override void OnHide()
            {
                base.OnHide();
                this.m_Tog.RemoveChangeCallBack(Toggle);
                this.m_btnSure.RemoveClickCallBack(OnSure);
                this.m_effect.Visible = false;
                GameEvents.UIEvents.UI_StartCartoon_Event.OnNextCapter -= OnNextKeyCapter;
            }

            public void SetData(float screen, int index, float space, ENUM_LOGIN_TYPE loginType)
            {
                Widget.localPosition = new Vector3(screen + index * (150 + space), 24f, 0); //Widget.localPosition.y
                //m_TweenPos.ResetAndPlay();
                m_TweenPos.From = Widget.localPosition;
                m_TweenPos.To = new Vector3(index * (150 + space), Widget.localPosition.y, 0);
                m_TweenPos.Delay = m_TweenPos.Delay * index;
                m_Index = index;
                if (loginType == ENUM_LOGIN_TYPE.E_THIRD)
                {
                    m_Index--;
                }
                m_loginType = loginType;
            }

            private void OnNextKeyCapter()
            {
                if (this.m_Tog.Checked)
                {
                    OnSure(null);
                }
            }

            private void Toggle(bool flag)
            {
                if (flag)
                {
                    m_tex.Color = Color.white;
                    this.m_btnSure.Visible = true;

                }
                else
                {
                    m_tex.Color = Color.gray;
                    this.m_btnSure.Visible = false;
                }
            }

            private void OnSure(GameObject obj)
            {
                GameEvents.UIEvents.UI_StartCartoon_Event.OnNextCapter -= OnNextKeyCapter;
                this.m_btnSure.Visible = false;
                if (m_Index >= 0)
                {
                    GameEvents.UI_Guid_Event.OnStartCartoonSelectHead.SafeInvoke(CommonData.DEFAULT_PLAYER_IMAGE_LIST[m_Index], CommonData.CartoonHEAD[m_Index], m_tex.Position);
                }
                else
                {
                    GameEvents.UI_Guid_Event.OnStartCartoonSelectHead.SafeInvoke(GlobalInfo.MY_PLAYER_INFO.NetWorkIcon, GlobalInfo.MY_PLAYER_INFO.NetWorkIcon, m_tex.Position);
                }
            }
        }
    }
}
