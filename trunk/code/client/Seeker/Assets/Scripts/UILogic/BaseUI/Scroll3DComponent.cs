using System;
using System.Collections.Generic;
using EngineCore;
using UnityEngine;
using DG.Tweening;
namespace SeekerGame
{
    public class Scroll3DComponent : GameUIComponent
    {
        protected Vector3 m_centerPos = new Vector3(20, 80);
        private GameUIComponent m_dragTex = null;

        protected Scroll3DItem[] itemArray = null;
        protected GameUIContainer m_container = null;
        protected Transform m_leftParent = null;

        protected int count = 0;

        public const float DRAG_END_PARAM = 0.1f;
        public const float MAX_DRAG = 30f;
        public const float DRAGFACTOR = 4f;

        protected override void OnInit()
        {
            base.OnInit();
            this.m_container = Make<GameUIContainer>("Right");
            this.m_dragTex = Make<GameUIComponent>(gameObject);
            this.m_leftParent = Widget.Find("Left");
        }

        public override void OnShow(object param)
        {
            base.OnShow(param);
            this.m_dragTex.AddDragCallBack(OnDrag);
            this.m_dragTex.AddDragEndCallBack(OnDragEnd);
            //GameEvents.UIEvents.UI_Scroll3D_Event.OnGetCurrentOverIndex += OnGetCurrentOverIndex;
            m_centerPos = this.m_container.ContainerTemplate.localPosition;
        }

        public override void OnHide()
        {
            base.OnHide();
            this.m_dragTex.RemoveDragCallBack(OnDrag);
            this.m_dragTex.RemoveDragEndCallBack(OnDragEnd);
            //GameEvents.UIEvents.UI_Scroll3D_Event.OnGetCurrentOverIndex -= OnGetCurrentOverIndex;
            this.m_scrollDir = 0;
            this.isDragEnd = true;
            this.isTweenerComplete = true;
        }

        private int m_scrollDir = 0;
        protected Tween m_dragTweener = null;

        protected void OnDrag(GameObject go, Vector2 delta,Vector2 pos,bool needLimit)
        {
            if (count <= 1)
            {
                return;
            }
            GameEvents.UIEvents.UI_Scroll3D_Event.OnScrollStart.SafeInvoke();
            if (m_dragTweener != null)
            {
                m_dragTweener.Kill();
            }
            isDragEnd = false;
            isTweenerComplete = false;
            float deltaX = 0;
            float lasdeltaX = 0;
            StopKickBack();
            if(needLimit)
                delta.x = Mathf.Clamp(delta.x, -Scroll3DComponent.MAX_DRAG, Scroll3DComponent.MAX_DRAG);
            m_dragTweener = DOTween.To(x => deltaX = x, 0, delta.x, 0.2f).OnUpdate(() =>
            {
                for (int i = 0; i < itemArray.Length; i++)
                {
                    itemArray[i].OnDrag(deltaX - lasdeltaX);
                }
                lasdeltaX = deltaX;
            }).OnComplete(() =>
            {
                this.isTweenerComplete = true;
                if (isDragEnd)
                {
                    OnKickBack(0);
                }
            });
        }

        private void OnDrag(GameObject go, Vector2 delta, Vector2 pos)
        {
            OnDrag(go,delta,pos,true);
        }


        private bool isDragEnd = true;
        private bool isTweenerComplete = true;
        private void OnDragEnd(GameObject go, Vector2 delta)
        {
            this.isDragEnd = true;
            if (this.isTweenerComplete)
            {
                OnKickBack(0f);
            }
        }

        private void OnKickBack(float deltaX)
        {
            if (count <= 1)
            {
                return;
            }
            int minIndex = 9999;
            int maxIndex = -9999;
            for (int i = 0; i < itemArray.Length; i++)
            {
                int index = itemArray[i].GetCurrentIndex(deltaX);
                minIndex = Mathf.Min(minIndex, index);
                maxIndex = Mathf.Max(maxIndex, index);

            }
            int overIndex = 0;
            if (minIndex > 0)
            {
                overIndex = minIndex;
            }
            else if (maxIndex < 0)
            {
                overIndex = maxIndex;
            }
            for (int i = 0; i < itemArray.Length; i++)
            {
                itemArray[i].OnDragEnd(deltaX, overIndex);
            }

        }

        private void StopKickBack()
        {
            for (int i = 0; i < itemArray.Length; i++)
            {
                itemArray[i].StopKickBack();
            }
        }

        private void OnGetCurrentOverIndex()
        {
            int minIndex = 9999;
            int maxIndex = -9999;
            for (int i = 0; i < itemArray.Length; i++)
            {
                int index = itemArray[i].GetCurrentIndex(0);
                minIndex = Mathf.Min(minIndex, index);
                maxIndex = Mathf.Max(maxIndex, index);

            }
            int overIndex = 0;
            if (minIndex > 0)
            {
                overIndex = minIndex;
            }
            else if (maxIndex < 0)
            {
                overIndex = maxIndex;
            }
            for (int i = 0; i < itemArray.Length; i++)
            {
                itemArray[i].OnDragEnd(0, overIndex);
            }
        }

        protected void ResetItemCom()
        {
            if (itemArray == null)
                return;
            for (int i = 0; i < itemArray.Length; i++)
            {
                itemArray[i].Widget.SetParent(this.m_container.Widget);
                itemArray[i].Widget.SetAsLastSibling();
                itemArray[i].Visible = false;
            }
        }

        protected int CheckCurrentType()
        {
            if (m_leftParent.childCount == 0)
                return 1;
            if (m_container.ChildCount == 1)
                return -1;
            return 0;
        }
    }

    public class Scroll3DItem : GameUIComponent
    {
        private CanvasGroup m_canvasGroup = null;
        private Transform m_leftParent = null;
        private Transform m_oriParent = null;
        private Vector3 m_centerPos = Vector3.zero;
        private int m_type = 0; //0 中间  1 左边  2 右边
        private int m_index = 0;

        protected float xSpace = 100f;
        protected int maxNum = 5;
        private float maxLen;
        private int m_totalCount = 0;

        protected int m_IndexData = 0;
        protected override void OnInit()
        {
            base.OnInit();
            this.m_canvasGroup = GetComponent<CanvasGroup>();
        }

        public override void OnShow(object param)
        {
            base.OnShow(param);
            this.m_oriParent = Widget.parent;
            this.maxLen = maxNum * xSpace;
            InitUI();
        }

        public override void OnHide()
        {
            base.OnHide();
        }

        public void SetData(int totalCount, int i, Vector3 centerPos, Transform leftParent)
        {
            this.isLeft = false;
            this.m_totalCount = totalCount;
            this.m_leftParent = leftParent;
            this.m_centerPos = centerPos;
            int centerIndex = totalCount / 2;
            this.m_IndexData = i;
            Widget.localPosition = m_centerPos + (centerIndex - i) * xSpace * Vector3.right;
            if (Widget.localPosition.x == m_centerPos.x)
            {
                GameEvents.UIEvents.UI_Scroll3D_Event.OnScrollEnd.SafeInvoke(this.m_IndexData);
            }
        }

        private void InitUI()
        {
            float currentSpace = xSpace;
            if (m_type == 1)
            {
                currentSpace = -xSpace;
            }
            float xFactor = Mathf.Abs(Widget.localPosition.x - m_centerPos.x) / this.maxLen;
            ReflashItem();
        }

        private bool isLeft = false;
        private Tween tweenerMoveing = null;
        public void OnDrag(float delta)
        {
            
            Widget.localPosition += delta * Vector3.right * Scroll3DComponent.DRAGFACTOR;
            ReflashItem();
        }

        private void OnMovingTweener()
        {
            ReflashItem();
        }

        private void ReflashItem()
        {
            float xFactor = Mathf.Abs(Widget.localPosition.x - m_centerPos.x) / this.maxLen;
            Widget.localScale = Mathf.Clamp(1f - xFactor, 0.2f, 1f) * Vector3.one;
            this.m_canvasGroup.alpha = Mathf.Clamp(1f - xFactor, 0.2f, 1f);
            if (!isLeft && Widget.localPosition.x < m_centerPos.x && this.m_canvasGroup.alpha <= 0.9f)
            {
                isLeft = true;
                Widget.SetParent(m_leftParent);
                Widget.SetAsLastSibling();
            }
            else if (this.m_canvasGroup.alpha > 0.9f && isLeft)
            {
                isLeft = false;
                Widget.SetParent(m_oriParent);
                Widget.SetAsLastSibling();
            }
        }

        private int currentIndex = 0;
        public int GetCurrentIndex(float detalX)
        {
            float itemPosX = Widget.localPosition.x;// + detalX * Scroll3DComponent.DRAG_END_PARAM;
            //右边
            int posIndex_min = 0;
            int posIndex_max = 0;
            posIndex_min = Mathf.FloorToInt((itemPosX - m_centerPos.x) / xSpace);
            posIndex_max = Mathf.CeilToInt((itemPosX - m_centerPos.x) / xSpace);
            float minPos = m_centerPos.x + xSpace * posIndex_min;
            float maxPos = m_centerPos.x + xSpace * posIndex_max;
            if (Mathf.Abs(minPos - itemPosX) > Mathf.Abs(maxPos - itemPosX))
            {
                currentIndex = posIndex_max;
            }
            else
            {
                currentIndex = posIndex_min;
            }
            return currentIndex;
        }

        private Tween rotateTweener = null;
        private Tween dragEndTweener = null;
        public void OnDragEnd(float deltaX, int overIndex)
        {
            OnDragKickBack(overIndex);
        }

        public void StopKickBack()
        {
            if (rotateTweener != null)
            {
                rotateTweener.Kill();
            }
        }

        private void OnDragKickBack(int overIndex)
        {
            Vector3 targetPos = Widget.localPosition;
            float maxPos = m_centerPos.x + xSpace * (currentIndex - overIndex);
            targetPos.x = maxPos;

            if (rotateTweener != null)
            {
                rotateTweener.Kill();
            }
            rotateTweener = Widget.DOLocalMove(targetPos, 0.5f).OnUpdate(OnRotateTweenerTweening).OnComplete(()=> {
                if (Widget.localPosition.x == m_centerPos.x)
                {
                    GameEvents.UIEvents.UI_Scroll3D_Event.OnScrollEnd.SafeInvoke(this.m_IndexData);
                }
            });
        }

        private void OnRotateTweenerTweening()
        {
            ReflashItem();
            
        }

        private void OnDragEndTweenering()
        {
            ReflashItem();
        }

    }
}
