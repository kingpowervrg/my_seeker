using EngineCore;
using System;
using System.Collections.Generic;
using UnityEngine;
namespace SeekerGame
{

    [UILogicHandler(UIDefine.UI_GAMESTART_1)]
    public class GameStartUILogic : UILogicBase
    {
        private GameStart_Name m_NameComponent = null;
        private GameStart_HeadIconPanel m_HeadIconComponent = null;
        private ConfGuidNew m_confNew = null;
        private GameImage m_bgBtn = null;
        protected override void OnInit()
        {
            base.OnInit();
            this.m_NameComponent = Make<GameStart_Name>("Panel_name");
            this.m_HeadIconComponent = Make<GameStart_HeadIconPanel>("Panel_headdraw");
            this.m_bgBtn = Make<GameImage>("RawImage");
        }

        public override void OnShow(object param)
        {
            base.OnShow(param);
            m_confNew = param as ConfGuidNew;
            ReflashGameStart();
            //GameEvents.UI_Guid_Event.OnGuidNewNext += MoveNextIndex;
            GameEvents.UIEvents.UI_StartCartoon_Event.OnNext += OnNext;
            this.m_bgBtn.AddClickCallBack(OnNextBtn);
        }

        public override void OnHide()
        {
            base.OnHide();
            //GameEvents.UI_Guid_Event.OnGuidNewNext -= MoveNextIndex;
            GameEvents.UIEvents.UI_StartCartoon_Event.OnNext -= OnNext;
            this.m_bgBtn.RemoveClickCallBack(OnNextBtn);
        }
        public override FrameDisplayMode UIFrameDisplayMode
        {
            get
            {
                return FrameDisplayMode.FULLSCREEN;
            }
        }
        public void ReflashGameStart()
        {
            this.m_NameComponent.Visible = int.Parse(m_confNew.typeValue) == 0;
            this.m_HeadIconComponent.Visible = int.Parse(m_confNew.typeValue) == 1;
            //this.m_bgBtn.Enable = !this.m_HeadIconComponent.Visible;
        }

        private void OnNextBtn(UnityEngine.GameObject obj)
        {
            if (this.m_HeadIconComponent.Visible)
            {
                return;
            }
            GameEvents.UIEvents.UI_StartCartoon_Event.OnNextGameStart.SafeInvoke(this.m_confNew.id);
            //GameEvents.UI_Guid_Event.OnGuidNewEnd.SafeInvoke(this.m_confNew.id);
            OnNext();
        }

        private void OnNext()
        {
            if (this.m_confNew.nextId > 0)
            {
                this.m_confNew = ConfGuidNew.Get(this.m_confNew.nextId);
                ReflashGameStart();
                return;
            }
        }

        public class GameStart_Name : GameUIComponent
        {
            private GameInputField m_input = null;
            private GameUIEffect m_effect = null;
            protected override void OnInit()
            {
                base.OnInit();
                this.m_input = Make<GameInputField>("Panel_animation:InputField");
                this.m_effect = Make<GameUIEffect>("UI_zhiwen");
            }

            public override void OnShow(object param)
            {
                base.OnShow(param);
                GameEvents.UIEvents.UI_StartCartoon_Event.OnNextGameStart += OnNextGameStart;
                this.m_input.Text = "Police_" + GlobalInfo.MY_PLAYER_ID;
                this.m_input.input.ActivateInputField();
                this.m_effect.EffectPrefabName = "UI_zhiwen.prefab";
                this.m_effect.Visible = true;
                //TimeModule.Instance.SetTimeout(()=> {

                //    //this.m_effect.Visible = true;
                //},2f);
                //m_input.input.onEndEdit.AddListener(OnSubmitText);

                //GameEvents.UIEvents.UI_StartCartoon_Event.OnNext += 
            }

            //private void OnSubmitText(string str)
            //{
            //}

            public override void OnHide()
            {
                base.OnHide();
                GameEvents.UIEvents.UI_StartCartoon_Event.OnNextGameStart -= OnNextGameStart;
            }

            private void OnNextGameStart(long id)
            {
                if (id != 10005)
                {
                    return;
                }
                string inputName = m_input.Text;
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

                GameEvents.UI_Guid_Event.OnGuidNewEnd.SafeInvoke(10005);
                //GameEvents.UIEvents.UI_StartCartoon_Event.OnNext.SafeInvoke();
            }


        }

        public class GameStart_HeadIconPanel : GameUIComponent
        {
            private GameStart_Icon[] m_HeadImg = null;
            protected override void OnInit()
            {
                base.OnInit();
                this.m_HeadImg = new GameStart_Icon[4];
                for (int i = 0; i < 4; i++)
                {
                    this.m_HeadImg[i] = Make<GameStart_Icon>("RawImage_" + i);
                    this.m_HeadImg[i].SetData(i, i == 0, OnClickListener);
                }
            }

            private void OnClickListener()
            {
                for (int i = 0; i < 4; i++)
                {
                    this.m_HeadImg[i].SetStatus(true);
                }
            }

            public class GameStart_Icon : GameUIComponent
            {
                private int m_index = -1;
                private GameTexture m_HeadImg = null;
                private GameUIEffect m_effect = null;
                private TweenRotationEuler m_tweener = null;
                private bool m_isChoose = false;
                private Action m_clickListener = null;

                private bool m_isComplete = false;
                protected override void OnInit()
                {
                    base.OnInit();
                    this.m_HeadImg = Make<GameTexture>(gameObject);
                    this.m_tweener = GetComponent<TweenRotationEuler>();
                    this.m_effect = Make<GameUIEffect>("UI_faliang_02");
                }

                public override void OnShow(object param)
                {
                    base.OnShow(param);
                    this.m_HeadImg.AddClickCallBack(OnChooseHeadIcon);
                    GameEvents.UIEvents.UI_StartCartoon_Event.OnNextGameStart += OnNextGameStart;
                    GameEvents.UIEvents.UI_StartCartoon_Event.OnChooseHead += OnChooseHead;
                    this.m_effect.ClearRotation = true;
                    this.m_effect.EffectPrefabName = "UI_faliang_02.prefab";
                }

                public override void OnHide()
                {
                    base.OnHide();
                    this.m_HeadImg.RemoveClickCallBack(OnChooseHeadIcon);
                    GameEvents.UIEvents.UI_StartCartoon_Event.OnNextGameStart -= OnNextGameStart;
                    GameEvents.UIEvents.UI_StartCartoon_Event.OnChooseHead -= OnChooseHead;
                    this.m_isComplete = false;
                }

                public void SetData(int index, bool isChoose, Action clickListener)
                {
                    this.m_index = index;
                    this.m_isChoose = isChoose;
                    this.m_effect.Visible = isChoose;
                    this.m_clickListener = clickListener;
                }

                public void SetStatus(bool isComplete)
                {
                    m_isComplete = isComplete;
                }

                private void OnChooseHeadIcon(UnityEngine.GameObject obj)
                {
                    if (m_isComplete)
                    {
                        return;
                    }
                    this.m_isChoose = true;
                    this.m_clickListener();
                    OnNextGameStart(10006);
                }

                private void OnNextGameStart(long id)
                {
                    if (id != 10006)
                    {
                        return;
                    }
                    if (this.m_isChoose)
                    {
                        CSPlayerRenewIconReq changeIconReq = new CSPlayerRenewIconReq();
                        changeIconReq.NewIcon = CommonData.DEFAULT_PLAYER_IMAGE_LIST[this.m_index];
                        GameEvents.NetWorkEvents.SendMsg.SafeInvoke(changeIconReq);
                        GameEvents.UIEvents.UI_StartCartoon_Event.OnChooseHead.SafeInvoke(this.m_index);
                    }
                }

                private void OnChooseHead(int index)
                {
                    if (this.m_index == index)
                    {
                        PlayChooseEffect();
                    }
                    else
                    {
                        PlayUnChooseEffect();
                    }
                }

                private void PlayChooseEffect()
                {
                    this.m_effect.Visible = true;
                    TimeModule.Instance.SetTimeout(() =>
                    {
                        this.m_tweener.ResetAndPlay();
                        this.m_effect.Visible = false;
                        TweenScale pos = this.m_HeadImg.gameObject.AddComponent<TweenScale>();
                        pos.From = this.m_HeadImg.Widget.anchoredPosition;
                        pos.To = Vector3.zero;
                        pos.Duration = 0.5f;
                        //pos.delay = 0.5f;
                        pos.PlayForward();

                        TweenScale scale = this.m_HeadImg.gameObject.AddComponent<TweenScale>();
                        scale.To = Vector3.one * 1.2f;
                        scale.Duration = 0.5f;
                        //scale.delay = 0.5f;
                        scale.PlayForward();

                        TimeModule.Instance.SetTimeout(() =>
                        {
                            GameObject.DestroyImmediate(pos);
                            GameObject.DestroyImmediate(scale);
                            GameEvents.UI_Guid_Event.OnGuidNewEnd.SafeInvoke(10006);
                            GameEvents.UIEvents.UI_StartCartoon_Event.OnNext.SafeInvoke();
                        }, 2f);
                    }, 0.5f);


                }

                private void PlayUnChooseEffect()
                {
                    this.m_effect.Visible = false;
                    TweenAlpha alpha = this.m_HeadImg.gameObject.AddComponent<TweenAlpha>();
                    alpha.To = 0f;
                    alpha.Duration = 0.5f;
                    alpha.PlayForward();
                    TimeModule.Instance.SetTimeout(() =>
                    {
                        GameObject.DestroyImmediate(alpha);
                    }, 0.5f);
                }
            }
        }

    }
}
