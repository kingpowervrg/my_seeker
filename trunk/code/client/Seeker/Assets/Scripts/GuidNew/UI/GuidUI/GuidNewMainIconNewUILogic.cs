using DG.Tweening;
using EngineCore;
using System;
using System.Collections.Generic;
using UnityEngine;
namespace SeekerGame
{
    public class GuidNewMainIconNewUILogic : GameUIComponent
    {
        private GameUIContainer m_grid;
        private GameButton[] m_banners;
        private GuidNewMainIconNewCom[] m_mainIcon;
        private string[] m_IconName = new string[] { "XinShouYinDao_02_dangandai_01.png", "XinShouYinDao_02_jingyuan_01.png", "XinShouYinDao_02_gongwenbao_01.png"
        ,"XinShouYinDao_02_jiangbei_01.png","XinShouYinDao_02_jinbi_01.png","XinShouYinDao_02_shouji_01.png"
        ,"XinShouYinDao_02_jinbi_01.png","XinShouYinDao_02_zhibi_01.png","XinShouYinDao_02_xingfeng_01.png"};
        protected override void OnInit()
        {
            base.OnInit();
            this.m_grid = Make<GameUIContainer>(gameObject);

        }

        public override void OnShow(object param)
        {
            base.OnShow(param);
            GameEvents.UI_Guid_Event.OnMainIcon += MainIcon;
        }

        public override void OnHide()
        {
            base.OnHide();
            GameEvents.UI_Guid_Event.OnMainIcon -= MainIcon;
        }

        public void SetBannerWidget(GameButton[] banner)
        {
            this.m_banners = banner;
        }

        private Vector3 GetMainIconDestPos(int i)
        {
            if (i < 6)
            {
                return m_banners[i + 1].Position;
            }
            else
            {
                GUIFrame frame = NewGuid.GuidNewModule.Instance.GetFrameByResName("UI_mainpanel_toparea.prefab");
                Transform iconTran = null;
                if (i == 6)
                    iconTran = frame.FrameRootTransform.Find("Panel_top/Image_coin");
                else if (i == 7)
                    iconTran = frame.FrameRootTransform.Find("Panel_top/Image_cash");
                else if (i == 8)
                    iconTran = frame.FrameRootTransform.Find("Panel_top/Button_back");
                return iconTran.position;
            }
        }

        private void MainIcon()
        {
            TimeModule.Instance.SetTimeout(() =>
            {
                LoadMainIcon();
                OnPlayMainIcon();
            }, 0.2f);

        }

        private void LoadMainIcon()
        {
            this.m_mainIcon = new GuidNewMainIconNewCom[9];
            this.m_grid.EnsureSize<GuidNewMainIconNewCom>(9);
            Transform template = this.m_grid.ContainerTemplate;
            Vector3 upStartPos = template.localPosition + Vector3.up * 70f + Vector3.right * 130f;
            for (int i = 0; i < 9; i++)
            {
                GuidNewMainIconNewCom mainIcon = this.m_grid.GetChild<GuidNewMainIconNewCom>(i);
                this.m_mainIcon[i] = mainIcon;
                if (i < 6)
                {
                    mainIcon.Widget.localPosition = template.transform.localPosition + Vector3.right * 70f * i;
                }
                else
                {
                    mainIcon.Widget.localPosition = upStartPos + Vector3.right * 70f * (i - 6);
                }
                mainIcon.SetData(m_IconName[i], GetMainIconDestPos(i));

                mainIcon.Visible = true;
            }
        }

        private void OnPlayMainIcon()
        {
            for (int i = 0; i < m_mainIcon.Length; i++)
            {
                m_mainIcon[i].Play(0.1f, i);
            }
        }

        public class GuidNewMainIconNewCom : GameUIComponent
        {
            private GameImage m_icon;
            private Vector3 m_destPos;
            private TweenScale m_scale;
            private TweenScale m_position;
            private TweenAlpha m_TweenAlpha = null;
            protected override void OnInit()
            {
                base.OnInit();
                this.m_icon = Make<GameImage>(gameObject);
                this.m_scale = gameObject.GetComponent<TweenScale>();
                this.m_position = gameObject.GetComponent<TweenScale>();
                this.m_TweenAlpha = gameObject.GetComponent<TweenAlpha>();
            }

            public override void OnShow(object param)
            {
                base.OnShow(param);
            }

            public override void OnHide()
            {
                base.OnHide();
            }

            public void SetData(string iconName, Vector2 destPos)
            {
                this.m_icon.Sprite = iconName;
                this.m_destPos = destPos;
                this.m_position.From = Widget.localPosition;
                this.m_position.To = Widget.localPosition - Vector3.up * 10;
            }

            public void Play(float time, int index)
            {
                TimeModule.Instance.SetTimeout(() =>
                {

                    this.m_scale.ResetAndPlay();
                    this.m_position.ResetAndPlay();

                    Vector3[] wayPoint = new Vector3[2];
                    wayPoint[0] = Position;
                    wayPoint[1] = this.m_destPos;
                    TimeModule.Instance.SetTimeout(() =>
                    {

                        this.m_icon.Widget.DOPath(wayPoint, 1f, PathType.CatmullRom, PathMode.TopDown2D).SetEase(Ease.InFlash).OnComplete(() =>
                        {
                            this.m_TweenAlpha.ResetAndPlay();
                            GameEvents.UI_Guid_Event.OnMainIconUnLockComplete.SafeInvoke(index);
                        });

                    }, 0.6f);

                }, time);


            }
        }
    }
}
