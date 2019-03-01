using System;
using System.Collections.Generic;
using EngineCore;
using UnityEngine;

namespace SeekerGame.NewGuid
{
    public class GuidNewTalkComponent : GameUIComponent
    {
        private GuidNewTalkItem m_leftRoot;
        private GuidNewTalkItem m_rightRoot;

        private GuidNewTalkItem m_currentRoot = null;
        protected override void OnInit()
        {
            base.OnInit();
            this.m_leftRoot = Make<GuidNewTalkItem>("left");
            this.m_rightRoot = Make<GuidNewTalkItem>("right");
        }

        public override void OnShow(object param)
        {
            base.OnShow(param);
            this.m_leftRoot.Visible = false;
            this.m_rightRoot.Visible = false;
            GameEvents.UI_Guid_Event.OnTalkEvent += OnTalkEvent;
            GameEvents.UI_Guid_Event.OnMaskTalkVisible += OnMaskTalkVisible;
            GameEvents.UI_Guid_Event.OnClearGuid += OnClearGuid;
        }

        public override void OnHide()
        {
            base.OnHide();
            GameEvents.UI_Guid_Event.OnTalkEvent -= OnTalkEvent;
            GameEvents.UI_Guid_Event.OnMaskTalkVisible -= OnMaskTalkVisible;
            GameEvents.UI_Guid_Event.OnClearGuid -= OnClearGuid;
        }

        private void OnClearGuid()
        {
            OnMaskTalkVisible(false);
            Debug.Log("OnClearGuid ====");
        }

        private void OnMaskTalkVisible(bool visible)
        {
            if (!visible && this.m_currentRoot != null)
            {
                this.m_currentRoot.Hide();
            }
        }

        private void OnTalkEvent(string content,int type,Vector2 startPos,Vector2 endPos,int needClick)
        {
            if (this.m_currentRoot != null && this.m_currentRoot.Visible)
            {
                //this.m_currentRoot.PlayBackward(() =>
                //{
                //    OnPlayTalk(content, type, startPos, endPos);
                //});
                this.m_currentRoot.Visible = false;
            }
            //else
            //{
            //    OnPlayTalk(content, type, startPos, endPos);
            //}
            OnPlayTalk(content, type, startPos, endPos);
        }

        private void OnPlayTalk(string content, int type, Vector2 startPos, Vector2 endPos)
        {

            if (type == 0)
            {
                this.m_currentRoot = this.m_leftRoot;
            }
            else if (type == 1)
            {
                this.m_currentRoot = this.m_rightRoot;
            }
            this.m_currentRoot.SetData(content, startPos, endPos);
            this.m_currentRoot.Visible = true;
        }


        public class GuidNewTalkItem : GameUIComponent
        {
            private GameLabel m_LabContent;
            private GameImage m_Img;
            private TweenScale m_tweener;
            private TweenAlpha m_LabTweener,m_ImgTweener;
            private GameTexture m_headIcon = null;
            protected override void OnInit()
            {
                base.OnInit();
                this.m_LabContent = Make<GameLabel>("TextGroup:Text_detail");
                this.m_Img = Make<GameImage>("Image_1");
                this.m_tweener = gameObject.GetComponent<TweenScale>();
                this.m_LabTweener = this.m_LabContent.Widget.parent.GetComponent<TweenAlpha>();
                this.m_ImgTweener = this.m_Img.gameObject.GetComponent<TweenAlpha>();
                this.m_headIcon = Make<GameTexture>(gameObject);
            }

            public override void OnShow(object param)
            {
                base.OnShow(param);
                if (GlobalInfo.GAME_NETMODE == GameNetworkMode.Network)
                {
                    this.m_headIcon.TextureName = "image_secretary_type1_size3.png";
                }
                else
                {
                    this.m_headIcon.TextureName = "image_Mr.X_size4_1.png";
                }
                //this.m_LabContent.Visible = false;
                //this.m_Img.Visible = false;
            }

            public void SetData(string content,Vector2 startPos,Vector2 endPos)
            {
                this.m_LabContent.Text = LocalizeModule.Instance.GetString(content);
                this.m_tweener.From = startPos;
                this.m_tweener.To = endPos;
                this.m_tweener.ResetAndPlay();

                //this.m_LabTweener.ResetAndPlay();
                //this.m_ImgTweener.ResetAndPlay();
                this.m_LabContent.Visible = true;
                this.m_Img.Visible = true;
                this.m_LabTweener.ResetAndPlay();
                this.m_ImgTweener.ResetAndPlay();
                //this.m_tweener.SetOnFinished(() =>
                //{
                    
                //});
                
                
            }



            public void Hide()
            {
                Visible = false;
                //this.m_tweener.PlayBackward();
                //this.m_tweener.SetOnFinished(() =>
                //{
                //    Debug.Log(" -------------- talk hide");
                //    Visible = false;
                //});
            }

            public void PlayBackward(Action cb)
            {
                this.m_tweener.SetTweenCompletedCallback(() =>
                {
                    if (cb != null)
                    {
                        cb();
                    }
                });
                this.m_tweener.PlayBackward();
                
            }

            public override void OnHide()
            {
                base.OnHide();
            }

        }
    }

}
