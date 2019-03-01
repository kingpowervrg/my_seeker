using DG.Tweening;
using EngineCore;
using System.Collections.Generic;
using UnityEngine;
namespace SeekerGame
{
    [UILogicHandler(UIDefine.UI_GIFTRESULT)]
    public class GiftResultUILogic : UILogicBase
    {
        private GameSpine m_spine;
        private GameUIComponent m_desc_root;
        private GameButton m_OK;
        private GameUIContainer m_Grid;

        private int m_exp;

        private Transform m_bagTrans = null;

        protected override void OnInit()
        {
            base.OnInit();

            m_spine = Make<GameSpine>("libao_01_SkeletonData");
            m_desc_root = Make<GameUIComponent>("BG:Panel");
            m_desc_root.Visible = false;
            m_OK = m_desc_root.Make<GameButton>("btnSure");
            m_Grid = m_desc_root.Make<GameUIContainer>("ScrollView:Scroll_View");
            this.m_bagTrans = Transform.Find("BG/Bag");
        }

        public override void OnShow(object param)
        {
            base.OnShow(param);

            GameEvents.UIEvents.UI_Bonus_Pop_View_Event.Tell_OnBlock.SafeInvoke(true);

            if (param == null)
            {
                return;
            }
            m_OK.AddClickCallBack(OnClick);
            m_spine.Visible = false;

            if (param is SCDropResp)
            {
                SCDropResp giftData = (SCDropResp)param;
                OnInitData(new List<DropInfo>(giftData.DropInfos));
                ShowProp();
            }
            else if (param is SCAutoOpenGiftDropResp)
            {
                SCAutoOpenGiftDropResp giftData = (SCAutoOpenGiftDropResp)param;
                OnInitData(new List<DropInfo>(giftData.DropInfos));
                ShowGift();
            }
            else if (param is ResultWindowData)
            {
                ResultWindowData windowData = param as ResultWindowData;
                OnInitResultData(windowData);
                ShowProp();
            }
        }

        public override FrameDisplayMode UIFrameDisplayMode
        {
            get
            {
                return FrameDisplayMode.WINDOWED;
            }
        }
        private void ShowGift()
        {
            m_spine.Visible = true;
            m_spine.PlayAnimation();
            TimeModule.Instance.SetTimeout(() => m_desc_root.Visible = true, 1.15f);
            TimeModule.Instance.SetTimeout(() => m_spine.Visible = false, 2.5f);
        }

        private void ShowProp()
        {
            m_desc_root.Visible = true;
        }


        private void OnInitData(List<DropInfo> DropInfos)
        {
            m_exp = 0;
            int count = DropInfos.Count;
            m_Grid.EnsureSize<GiftContent>(count);
            for (int i = 0; i < count; i++)
            {
                GiftContent giftContent = m_Grid.GetChild<GiftContent>(i);
                giftContent.SetData(DropInfos[i]);
                giftContent.SetTargetTransform(this.m_bagTrans);
                giftContent.Visible = true;
                ConfProp prop = ConfProp.Get(DropInfos[i].PropId);
                //TODO: 放在各自系统增减装备。
                //if ((int)PROP_TYPE.E_FUNC == prop.type || (int)PROP_TYPE.E_CHIP == prop.type || (int)PROP_TYPE.E_NROMAL == prop.type || (int)PROP_TYPE.E_ENERGE == prop.type)
                //{
                //    GlobalInfo.MY_PLAYER_INFO.AddSingleBagInfo(DropInfos[i].PropId, DropInfos[i].Count);
                //}
                m_exp += CommonHelper.GetPropExp(DropInfos[i].PropId, DropInfos[i].Count);
            }
        }

   
        //纯展示
        private void OnInitResultData(ResultWindowData data)
        {
            int count = data.m_itemDatas.Count;
            m_Grid.EnsureSize<GiftContent>(count);
            for (int i = 0; i < count; i++)
            {
                GiftContent giftContent = m_Grid.GetChild<GiftContent>(i);
                giftContent.SetData(data.m_itemDatas[i]);
                giftContent.SetTargetTransform(this.m_bagTrans);
                giftContent.Visible = true;
            }
        }

        private bool m_isClick = false;
        private void OnClick(GameObject obj)
        {
            if (this.m_isClick)
            {
                return;
            }
            this.m_isClick = true;
            EngineCoreEvents.AudioEvents.PlayAudio.SafeInvoke(Audio.AudioType.UISound, EngineCommonAudioKey.Button_Click_Common.ToString());
            for (int i = 0; i < m_Grid.ChildCount; i++)
            {
                GiftContent giftcontent = m_Grid.GetChild<GiftContent>(i);
                giftcontent.PlayTweener();
            }
            TimeModule.Instance.SetTimeout(() =>
            {
                EngineCoreEvents.AudioEvents.PlayAudio.SafeInvoke(Audio.AudioType.UISound, GameCustomAudioKey.prop_into_bag.ToString());
                EngineCoreEvents.UIEvent.HideUIEvent.SafeInvoke(UIDefine.UI_GIFTRESULT);
                GameEvents.PlayerEvents.OnExpChanged.SafeInvoke(null, m_exp);
            }, 1f);

        }

        public override void OnHide()
        {
            base.OnHide();
            m_OK.RemoveClickCallBack(OnClick);
            this.m_isClick = false;
            m_desc_root.Visible = false;
            m_spine.StopAnimation(true);
            m_spine.Visible = false;
            GameEvents.UIEvents.UI_Bonus_Pop_View_Event.Tell_OnBlock.SafeInvoke(false);
            GameEvents.UIEvents.UI_Bonus_Pop_View_Event.Tell_OnShow.SafeInvoke();
        }

        public class GiftContent : GameUIContainer
        {
            private GameImage m_Icon;
            private GameLabel m_Number;
            private GameLabel m_Name;
            private TweenScale m_tweener = null;

            private Transform m_target = null;
            protected override void OnInit()
            {
                base.OnInit();
                m_Icon = Make<GameImage>("icon");
                m_Number = Make<GameLabel>("Text");
                m_Name = Make<GameLabel>("name");
                this.m_tweener = this.m_Icon.GetComponent<TweenScale>();
            }

            public override void OnShow(object param)
            {
                base.OnShow(param);
                m_iconAnchorPos = this.m_Icon.Widget.anchoredPosition;
            }

            public void SetData(DropInfo info)
            {
                ConfProp prop = ConfProp.Get(info.PropId);
                if (prop == null)
                {
                    Visible = false;
                    Debug.LogErrorFormat("no exist prop : " + info.PropId);
                }
                m_Icon.Sprite = prop.icon;
                m_Number.Text = string.Format("x{0}", info.Count);
                m_Name.Text = LocalizeModule.Instance.GetString(prop.name);
            }

            public void SetData(ResultItemData itemData)
            {
                ConfProp prop = ConfProp.Get(itemData.m_propID);
                if (prop == null)
                {
                    Visible = false;
                    Debug.LogErrorFormat("no exist prop : " + itemData.m_propID);
                }
                m_Icon.Sprite = prop.icon;
                m_Number.Text = string.Format("x{0}", itemData.m_propCount);
                m_Name.Text = LocalizeModule.Instance.GetString(prop.name);
            }

            public void SetTargetTransform(Transform targetTrans)
            {
                this.m_target = targetTrans;
            }

            private Vector2 m_iconAnchorPos = Vector2.zero;

            public void PlayTweener()
            {
                if (this.m_target == null)
                {
                    return;
                }
                this.m_Icon.Widget.SetParent(LogicHandler.Transform);
                Visible = false;
                this.m_tweener.ResetAndPlay();
                Vector3[] wayPoint = new Vector3[2];
                wayPoint[0] = this.m_Icon.Position;
                wayPoint[1] = this.m_target.position;
                this.m_Icon.Widget.DOPath(wayPoint, 1f, PathType.CatmullRom, PathMode.TopDown2D).SetEase(Ease.InOutQuad).OnComplete(() =>
                {

                    this.m_Icon.Widget.SetParent(gameObject.transform);
                    this.m_Icon.Widget.localScale = Vector3.one;
                    this.m_Icon.Widget.anchoredPosition = m_iconAnchorPos;
                    this.m_Icon.Widget.SetSiblingIndex(1);
                });

            }
        }
    }

    public class ResultWindowData
    {
        public List<ResultItemData> m_itemDatas;

        public ResultWindowData(List<ResultItemData> itemdatas)
        {
            this.m_itemDatas = itemdatas;
        }
    }

    public class ResultItemData
    {
        public long m_propID;
        public int m_propCount;

        public ResultItemData(long m_propID, int m_propCount)
        {
            this.m_propID = m_propID;
            this.m_propCount = m_propCount;
        }
    }

}
