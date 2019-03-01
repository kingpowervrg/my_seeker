using DG.Tweening;
using EngineCore;
using System;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 废弃代码  
/// </summary>
namespace SeekerGame
{
    public class GuidNewMainIconUILogic : GameUIComponent
    {
        private GameUIContainer m_grid;
        private GameToggleButton[] m_banners;
        private List<GuidNewMainIconComponent> m_mainIcon = new List<GuidNewMainIconComponent>();
        private List<GuidNewMainIconData> m_mainIconDatas = null;
        private UnityEngine.UI.HorizontalLayoutGroup m_gridLayout = null;
        protected override void OnInit()
        {
            base.OnInit();
            this.m_grid = Make<GameUIContainer>(gameObject);

            GameEvents.UI_Guid_Event.OnMainIcon += OnMainIcon;
            GameEvents.UI_Guid_Event.OnMainIconUnLock += OnMainIconUnLock;
        }

        public override void OnShow(object param)
        {
            base.OnShow(param);
        }

        public override void OnHide()
        {
            base.OnHide();
            //m_grid.Clear();


        }
        public void SetGridLayout(bool flag)
        {
            this.m_gridLayout.enabled = flag;
        }
        public override void Dispose()
        {
            base.Dispose();
            GameEvents.UI_Guid_Event.OnMainIcon -= OnMainIcon;
            GameEvents.UI_Guid_Event.OnMainIconUnLock -= OnMainIconUnLock;
        }

        private GuidNewMainIconComponent GetMainIconByIndex(int iconIndex)
        {
            for (int i = 0; i < m_mainIconDatas.Count; i++)
            {
                if (m_mainIconDatas[i].IconIndex == iconIndex)
                {
                    return m_mainIcon[i];
                }
            }
            return null;
        }

        /// <summary>
        /// 根据index获取
        /// </summary>
        /// <param name="iconIndex"></param>
        private void OnMainIconUnLock(int iconIndex)
        {
            Vector3 destPos = Vector3.zero;
            if (iconIndex < 4)
            {
                destPos = GetMainIconByIndex(iconIndex - 1).GetAnchorWorldPos();
            }
            else if (iconIndex == 6) //好友解锁
            {
                destPos = GetMainIconByIndex(3).GetAnchorWorldPos();
            }
            else if (iconIndex == 7)//汉堡飞入背包
            {
                destPos = GetMainIconByIndex(3).Position;
            }
            else if (iconIndex == 5)
            {
                destPos = GetMainIconByIndex(4).GetAnchorWorldPos();
            }
            else if (iconIndex == 4)
            {
                destPos = GetMainIconByIndex(6).GetAnchorWorldPos();
            }
            else if (iconIndex == 8 || iconIndex == 9 || iconIndex == 10)
            {
                GUIFrame frame = NewGuid.GuidNewModule.Instance.GetFrameByResName("UI_mainpanel_toparea.prefab");
                Transform iconTran = null;
                if (iconIndex == 8)
                    iconTran = frame.FrameRootTransform.Find("Panel_top/Image_coin");
                else if (iconIndex == 9)
                    iconTran = frame.FrameRootTransform.Find("Panel_top/Image_cash");
                else if (iconIndex == 10)
                    iconTran = frame.FrameRootTransform.Find("Panel_top/Button_back");
                destPos = iconTran.position;
            }

            GetMainIconByIndex(iconIndex).IconFly(destPos);
        }

        public void SetBannerWidget(GameToggleButton[] banner)
        {
            this.m_banners = banner;
            this.m_gridLayout = banner[0].Widget.parent.GetComponent<UnityEngine.UI.HorizontalLayoutGroup>();
        }

        private void OnMainIcon()
        {
            InitFrame(null);
        }

        private void InitFrame(List<GuidNewMainIconData> mainIconDatas)
        {
            this.m_grid.Visible = true;
            this.m_mainIconDatas = mainIconDatas;
            m_grid.EnsureSize<GuidNewMainIconComponent>(mainIconDatas.Count);
            for (int i = 0; i < mainIconDatas.Count; i++)
            {
                GuidNewMainIconComponent mainIcon = m_grid.GetChild<GuidNewMainIconComponent>(i);
                mainIcon.Visible = true;
                if (mainIconDatas[i].IconType == 0)
                {
                    mainIcon.SetData(mainIconDatas[i], m_banners[mainIconDatas[i].IconIndex], this);
                }
                else
                {
                    mainIcon.SetData(mainIconDatas[i], this);
                }
                this.m_mainIcon.Add(mainIcon);

            }
        }

        private Vector3 GetBannerWorldPosByIndex(int index)
        {
            return m_banners[index].Position;
        }

        private int GetBannerSiblingIndex(int index)
        {
            return m_banners[index].Widget.GetSiblingIndex();
        }

        public class GuidNewMainIconComponent : GameUIComponent
        {
            private GameImage m_icon;
            private GuidNewMainIconData m_iconData;
            private GuidNewMainIconUILogic m_parent;
            private GameToggleButton m_banner;
            private GameImage m_Bg;
            private GameLabel m_label;
            private GameImage m_redDot;
            private GameImage m_mask;
            private Transform m_anchor;
            private TweenRotationEuler m_TweenRotation;

            protected override void OnInit()
            {
                base.OnInit();
                this.m_icon = Make<GameImage>(gameObject);
                this.m_TweenRotation = this.m_icon.GetComponent<TweenRotationEuler>();

            }

            public override void OnShow(object param)
            {
                base.OnShow(param);
                GameEvents.UI_Guid_Event.OnMainIconStartMove += OnMainIconMove;
                GameEvents.UI_Guid_Event.OnMainIconMoveComplete += OnMainIconMoveComplete;
            }

            private void InitBanner(bool isUnLock)
            {
                this.m_Bg = m_banner.Make<GameImage>("Background");
                this.m_label = m_banner.Make<GameLabel>("Label");
                this.m_mask = m_banner.Make<GameImage>("Guid");
                this.m_redDot = m_banner.Make<GameImage>("ImgWarn");
                this.m_anchor = m_banner.Widget.Find("anchor");

                this.m_Bg.Visible = isUnLock;
                this.m_label.Visible = isUnLock;
                this.m_banner.Enabled = isUnLock;
                //this.m_redDot.Visible = isUnLock;
                this.m_mask.Visible = !isUnLock;
            }



            public Vector3 GetAnchorWorldPos()
            {
                return m_anchor.position;
            }

            private void SetMainIconInfo(GuidNewMainIconData iconData, GuidNewMainIconUILogic parent)
            {
                this.m_parent = parent;
                m_icon.Sprite = iconData.IconName;
                this.m_iconData = iconData;
            }

            public void SetData(GuidNewMainIconData iconData, GameToggleButton banner, GuidNewMainIconUILogic parent)
            {
                SetMainIconInfo(iconData, parent);
                this.m_banner = banner;
                //this.m_parent = parent;
                //m_icon.Sprite = iconData.IconName;
                if (iconData.IconType != 0)
                {
                    return;
                }
                InitBanner(iconData.IsUnLock);
                //this.m_iconData = iconData;
                if (iconData.IsUnLock)
                {
                    banner.Widget.SetSiblingIndex(iconData.IconIndex);
                    m_icon.Visible = false;
                    Debug.Log("unlock ==== ");
                }
                else
                {
                    if (iconData.IconIndex <= 3)
                    {
                        banner.Widget.SetSiblingIndex(iconData.IconIndex);
                        //banner.Widget.SetAsFirstSibling();
                    }
                    else
                    {
                        banner.Widget.SetAsLastSibling();
                    }
                }
            }

            public void SetData(GuidNewMainIconData iconData, GuidNewMainIconUILogic parent)
            {
                SetMainIconInfo(iconData, parent);
                if (iconData.IsUnLock)
                {
                    m_icon.Visible = false;
                    Debug.Log("unlock ==== ");
                }
            }

            public override void OnHide()
            {
                base.OnHide();
                GameEvents.UI_Guid_Event.OnMainIconStartMove -= OnMainIconMove;
                GameEvents.UI_Guid_Event.OnMainIconMoveComplete -= OnMainIconMoveComplete;
                this.m_status = false;
            }
            //int m_rockNum = 0;
            private void IconStartFly(Vector3 destPos)
            {
                this.m_TweenRotation.From = new Vector3(0, 0, 0);
                this.m_TweenRotation.To = new Vector3(0, 0, 360f);
                this.m_TweenRotation.Duration = 0.2f;
                this.m_TweenRotation.m_tweenStyle = UITweenerBase.TweenStyle.Loop;
                this.m_TweenRotation.PlayForward();
                Vector3[] wayPoint = new Vector3[2];
                wayPoint[0] = m_icon.Position;
                //wayPoint[1] = new Vector3((m_icon.Position.x + destPos.x) / 2f,m_icon.Position.y + 40f,0f);
                wayPoint[1] = destPos;
                this.m_icon.Widget.DOPath(wayPoint, 1f, PathType.CatmullRom, PathMode.TopDown2D).SetEase(Ease.InFlash).OnComplete(() =>
                {
                    //this.Visible = false;
                    this.m_TweenRotation.Stop();
                    this.m_icon.Visible = false;
                    if (m_iconData.IconIndex < 7)
                    {
                        if (m_iconData.IconIndex == 6)
                        {
                            GameEvents.UI_Guid_Event.OnMainIconMoveComplete.SafeInvoke(m_iconData.IconIndex);
                        }
                        else
                        {
                            GameEvents.UI_Guid_Event.OnMainIconStartMove.SafeInvoke(m_iconData.IconIndex);
                        }
                    }
                    else if (m_iconData.IconIndex >= 7 && m_iconData.IconIndex <= 10)
                    {
                        GameEvents.UI_Guid_Event.OnMainIconUnLockComplete.SafeInvoke(m_iconData.IconIndex);
                    }
                    //OnFlyEffectPlayFinished?.Invoke();
                });

            }
            public void IconFly(Vector3 destPos)
            {
                this.m_TweenRotation.From = new Vector3(0, 0, -30f);
                this.m_TweenRotation.To = new Vector3(0, 0, 30f);
                this.m_TweenRotation.Duration = 0.3f;
                this.m_TweenRotation.m_tweenStyle = UITweenerBase.TweenStyle.Once;
                this.m_TweenRotation.Play(true);
                //m_rockNum = 0;
                TimeModule.Instance.SetTimeout(() =>
                {
                    this.m_icon.Widget.rotation = Quaternion.Euler(Vector3.zero);
                    this.m_TweenRotation.Stop();
                    IconStartFly(destPos);
                }, 1f);
            }

            private void OnMainIconMove(int index)
            {
                Vector3 destPos = Vector3.zero;
                if (index > 0 && index < 4)
                {
                    if (m_iconData.IconIndex < index && m_iconData.IsUnLock)
                    {
                        //todo 临时修改  不移动
                        //if (m_iconData.IconIndex == 0)
                        //{
                        //    destPos = this.m_parent.GetBannerWorldPosByIndex(index);
                        //}
                        //else
                        //{
                        //    destPos = this.m_parent.GetBannerWorldPosByIndex(m_iconData.IconIndex - 1);
                        //}
                        //BannerMove(destPos, index);
                        GameEvents.UI_Guid_Event.OnMainIconMoveComplete.SafeInvoke(index);
                    }
                }
                else if (index == 4 || index == 5) //成就  商城
                {
                    //if (m_iconData.IconIndex == index)
                    //{

                    //    //this.m_banner.Widget.SetSiblingIndex(m_parent.GetBannerSiblingIndex(6));
                    //}
                    if (m_iconData.IconIndex == 6)
                    {
                        destPos = this.m_parent.GetBannerWorldPosByIndex(index);
                    }
                    else if (index == 4 && m_iconData.IconIndex == 4)
                    {
                        destPos = this.m_parent.GetBannerWorldPosByIndex(5);
                    }
                    else if (index == 5 && m_iconData.IconIndex == 5)
                    {
                        this.m_mask.Visible = false;
                        return;
                        //destPos = this.m_parent.GetBannerWorldPosByIndex(5);
                    }
                    else
                    {
                        return;
                    }
                    BannerMove(destPos, index);
                }

            }

            private void BannerMove(Vector3 destPos, int index)
            {
                //destPos.z = 0f;
                Vector3[] wayPoint = new Vector3[2];
                wayPoint[0] = m_banner.Position;
                wayPoint[1] = destPos;
                this.m_banner.Widget.DOPath(wayPoint, 1f, PathType.CatmullRom, PathMode.TopDown2D).SetEase(Ease.InFlash).OnComplete(() =>
                {
                    //GetBannerWorldPosByIndex(0);
                    TimeModule.Instance.SetTimeout(() =>
                    {
                        GameEvents.UI_Guid_Event.OnMainIconMoveComplete.SafeInvoke(index);
                    }, 0.5f);
                });
            }

            private bool m_status = false;
            private void OnMainIconMoveComplete(int index)
            {
                if (index != m_iconData.IconIndex || m_status)
                {
                    return;
                }
                m_status = true;
                this.m_mask.Visible = false;
                if (index == 4 || index == 5)
                {
                    this.m_banner.Widget.SetSiblingIndex(index);
                    if (index == 4)
                    {
                        GameEvents.RedPointEvents.User_OnNewAchievementEvent.SafeInvoke();
                    }
                }
                //else if (index > 0 && index < 4){
                //    this.m_banner.Widget.SetSiblingIndex(m_parent.GetBannerSiblingIndex(m_iconData.IconIndex - 1));
                //}
                TweenAlpha bgAlpha = this.m_Bg.gameObject.AddComponent<TweenAlpha>();
                SetControlAlpha(bgAlpha, () =>
                {
                    //GameObject.DestroyImmediate(bgAlpha);
                    m_iconData.IsUnLock = true;

                    GameEvents.UI_Guid_Event.OnMainIconUnLockComplete.SafeInvoke(m_iconData.IconIndex);
                });
                TweenAlpha labAlpha = this.m_label.gameObject.AddComponent<TweenAlpha>();
                SetControlAlpha(labAlpha, () =>
                {
                    //GameObject.DestroyImmediate(labAlpha);
                });
                this.m_Bg.Visible = true;
                this.m_label.Visible = true;
                this.m_banner.Enabled = true;
                //this.m_redDot.Visible = true;
            }

            private void SetControlAlpha(TweenAlpha tween, GOGUI.EventDelegate.Callback onFinish)
            {
                tween.From = 0f;
                tween.To = 1f;
                tween.Duration = 1f;
                tween.PlayForward();
                tween.SetTweenCompletedCallback(() => { onFinish(); });
            }

        }
    }

    public class GuidNewMainIconData
    {
        public string IconName;
        public int IconIndex;
        public bool IsUnLock;
        public int IconType;
        public GuidNewMainIconData(string iconName, int iconIndex, bool isUnLock, int iconType)
        {
            this.IconName = iconName;
            this.IconIndex = iconIndex;
            this.IsUnLock = isUnLock;
            this.IconType = iconType;
        }
    }
}
