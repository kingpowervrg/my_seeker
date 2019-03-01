using EngineCore;
using System.Collections.Generic;
using UnityEngine;
namespace SeekerGame
{
    public class ScrollRectComponent : GameUIComponent
    {
        public float itemSpaceX = 10f;
        private GameUIContainer m_container = null;
        public int itemCount = 50;
        public float itemWidth = 302f;
        private Transform m_chatPivot = null;

        public static Transform m_leftPivot = null;
        public static Transform m_rightPivot = null;
        protected override void OnInit()
        {
            base.OnInit();
            this.m_container = Make<GameUIContainer>(gameObject);
            m_leftPivot = Widget.parent.Find("left");
            m_rightPivot = Widget.parent.Find("right");
        }

        public override void OnShow(object param)
        {
            base.OnShow(param);
            //leftPoint = Widget.localPosition + (Widget.sizeDelta.x + itemWidth) * Vector3.left;
            //rightPoint = Widget.localPosition + (Widget.sizeDelta.x + itemWidth) * Vector3.right;
        }

        public override void OnHide()
        {
            base.OnHide();
        }

        public void InitList(IList<ChapterNpcInfo> unlockChapterNpcList, Transform chatPivot)
        {
            this.m_chatPivot = chatPivot;
            itemCount = unlockChapterNpcList.Count;
            float halfItemWidth = itemWidth / 2f;
            m_container.Widget.sizeDelta = new Vector2((itemWidth + itemSpaceX) * itemCount, m_container.Widget.sizeDelta.y);
            RectTransform templateRect = m_container.ContainerTemplate.GetComponent<RectTransform>();
            this.m_container.EnsureSize<NPCIntroComponent>(itemCount);
            for (int i = 0; i < itemCount; i++)
            {
                NPCIntroComponent rectItem = this.m_container.GetChild<NPCIntroComponent>(i);
                rectItem.Widget.localScale = Vector2.one;
                Vector3 itemPos = templateRect.anchoredPosition;
                itemPos.x = halfItemWidth + (itemWidth + itemSpaceX) * i;
                rectItem.Widget.anchoredPosition = itemPos;
                rectItem.SetNpcID(unlockChapterNpcList[i].NpcId, unlockChapterNpcList[i], this.m_chatPivot);
                rectItem.Visible = true;
            }
        }
    }

    /// <summary>
    /// NPC 简介
    /// </summary>
    public class NPCIntroComponent : GameUIComponent
    {
        private long m_npcID;
        private ConfNpc m_npcData;

        private GameTexture m_npcTexture = null;
        //private GameLabel m_npcName = null;
        //private GameImage m_imgSuspect = null;
        private ChapterNpcInfo npcInfo = null;
        private GameButton m_btnChat = null;

        private TweenPosition m_tweenPosition = null;
        private TweenScale m_TweenScale = null;
        private Transform m_scaleChatTrans = null;
        private Vector3 m_tempVector = Vector3.zero;
        private int m_itemStatus = 0; // 当前状态 0列表 1选中
        protected override void OnInit()
        {
            this.m_npcTexture = Make<GameTexture>("RawImage");
            this.m_btnChat = Make<GameButton>("Button");
            this.m_tweenPosition = GetComponent<TweenPosition>();
            this.m_TweenScale = GetComponent<TweenScale>();
            //this.m_npcName = Make<GameLabel>("Text");
            //this.m_imgSuspect = Make<GameImage>("Image");
            //this.m_imgSuspect.Visible = false;
        }

        public override void OnShow(object param)
        {
            this.m_npcTexture.AddClickCallBack(OnBtnShowNpcDetailInfo);
            this.m_btnChat.AddClickCallBack(OnBtnChatWithNpcClick);
            this.m_tempVector = Widget.anchoredPosition;
            this.m_itemStatus = 0;
            GameEvents.UIEvents.UI_Archives_Event.OnClickChatPerson += OnClickChatPerson;
            GameEvents.UIEvents.UI_Archives_Event.OnCloseNpcDetail += OnCloseNpcDetail;
        }

        private void OnBtnChatWithNpcClick(GameObject btn)
        {
            TalkUIHelper.OnStartTalk(this.npcInfo.DialogueId, TalkDialogEnum.AchiveTalk);

            Dictionary<UBSParamKeyName, object> _param = new Dictionary<UBSParamKeyName, object>();
            _param.Add(UBSParamKeyName.ContentID, this.npcInfo.DialogueId);
            UserBehaviorStatisticsModules.Instance.LogEvent(UBSEventKeyName.file_NPCdialogue, null, _param);
        }

        public void SetNpcID(long npcId, ChapterNpcInfo npcInfo, Transform scaleTran)
        {
            this.m_itemStatus = 0;
            this.m_scaleChatTrans = scaleTran;
            this.m_npcID = npcId;
            this.m_npcData = ConfNpc.Get(npcId);
            this.npcInfo = npcInfo;
            if (this.m_npcData == null)
                Debug.LogError($"{npcId} npc config not found");

            RefreshNpcIntro();
        }

        private void RefreshNpcIntro()
        {
            //this.m_npcName.Text = LocalizeModule.Instance.GetString(this.m_npcData.name);
            this.m_npcTexture.TextureName = this.m_npcData.icon;
        }

        private void OnBtnShowNpcDetailInfo(GameObject btnShowNpcDetail)
        {
            if (this.m_itemStatus != 0)
            {
                return;
            }
            this.m_isChoose = true;
            this.m_itemStatus = 1;
            GameEvents.UIEvents.UI_Archives_Event.OnForbidNpcListScroll.SafeInvoke(true);
            GameEvents.UIEvents.UI_Archives_Event.OnClickChatPerson.SafeInvoke(Widget.anchoredPosition);
            Vector3 finalPosition = Widget.parent.InverseTransformPoint(this.m_scaleChatTrans.position);
            this.m_tempVector = Widget.anchoredPosition;
            this.m_tweenPosition.From = Widget.anchoredPosition;
            this.m_tweenPosition.To = (Widget.anchoredPosition.y + 40f) * Vector2.up + finalPosition.x * Vector2.right;
            this.m_tweenPosition.ResetAndPlay();
            this.m_TweenScale.ResetAndPlay();
            ChapterUILogic.OnShowNPCDetailInfo(this.m_npcID, this.npcInfo);
        }

        public override void OnHide()
        {
            base.OnHide();
            ResetItem();
            this.m_npcTexture.RemoveClickCallBack(OnBtnShowNpcDetailInfo);
            this.m_btnChat.RemoveClickCallBack(OnBtnChatWithNpcClick);
            GameEvents.UIEvents.UI_Archives_Event.OnClickChatPerson -= OnClickChatPerson;
            GameEvents.UIEvents.UI_Archives_Event.OnCloseNpcDetail -= OnCloseNpcDetail;
        }

        private void OnClickChatPerson(Vector2 clickPos)
        {
            if (clickPos.x == Widget.anchoredPosition.x)
            {
                return;
            }
            this.m_tempVector = Widget.anchoredPosition;
            Vector3 targetPos = Widget.anchoredPosition;
            if (clickPos.x > Widget.anchoredPosition.x)
            {
                targetPos.x = Widget.parent.InverseTransformPoint(ScrollRectComponent.m_leftPivot.position).x;
            }
            else
            {
                targetPos.x = Widget.parent.InverseTransformPoint(ScrollRectComponent.m_rightPivot.position).x;
            }
            this.m_tweenPosition.From = Widget.anchoredPosition;
            this.m_tweenPosition.To = targetPos;
            this.m_tweenPosition.ResetAndPlay();
        }

        private bool m_isChoose = false;
        private void ResetItem()
        {
            this.m_itemStatus = 0;
            this.m_tweenPosition.From = Widget.anchoredPosition;
            this.m_tweenPosition.To = m_tempVector;
            this.m_tweenPosition.ResetAndPlay();
            if (this.m_isChoose)
            {
                this.m_TweenScale.PlayBackward();
                this.m_isChoose = false;
            }
        }

        private void OnCloseNpcDetail()
        {
            ResetItem();
        }

    }
}
