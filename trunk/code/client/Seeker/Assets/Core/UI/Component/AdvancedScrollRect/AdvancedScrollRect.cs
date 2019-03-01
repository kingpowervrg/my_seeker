using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


namespace EngineCore
{
    public abstract class AdvancedScrollRect : GameScrollRect
    {

        public Action<int, GameObject> OnAddChildItem;

        public Action<int, GameObject> OnDeleteChildItem;

        public Func<GameObject> CreateGameObjectHandler;

        public Action<GameObject> DeleteGameObjectHandler;

        public Action<int, PointerEventData> OnMouseStateChanged;

        public Action<int, GameObject> OnDragItem;

        public Action<PointerEventData> OnScrollRectDragging;

        public Action<PointerEventData> OnMouseExitScrollRectAndDragging;

        public Action OnContentAnchoredPositionChanged;


        [HideInInspector]
        private int totalCount = 0;  //negative means INFINITE mode

        [HideInInspector]
        public float threshold = 1;         //阈值,防止在边界时可能出现反复创建删除元素的情况
        [HideInInspector]
        public bool reverseDirection = false;
        [HideInInspector]
        public float rubberScale = 1;

        private int itemTypeStart = 0;
        private int itemTypeEnd = 0;
        protected int directionSign = 0;

        protected Vector2 m_contentSpacing = Vector2.zero;          //格子之间的间距
        private int m_ContentConstraintCount = 0;       //限制
        protected GridLayoutGroup m_gridLayout = null;
        protected HorizontalOrVerticalLayoutGroup m_horizontalOrVerticalLayout = null;
        protected ContentSizeFitter m_contentFitter = null;

        private ScrollRectMouseStatus m_currentMouseState = ScrollRectMouseStatus.None;

        private int m_onDraggingItemIndex = -1;
        private GameObject m_onDraggingGameObject = null;

        protected Vector2 m_realContentSize = Vector2.zero;   //真实内容列表大小

        protected bool m_isMoveToStart = false;
        protected bool m_isMoveToEnd = false;


        protected override void Awake()
        {
            if (content != null)
            {
                m_gridLayout = content.GetComponent<GridLayoutGroup>();
                m_horizontalOrVerticalLayout = content.GetComponent<HorizontalOrVerticalLayoutGroup>();
                m_contentFitter = content.gameObject.GetComponent<ContentSizeFitter>();
                if (m_contentFitter == null)
                    m_contentFitter = content.gameObject.AddComponent<ContentSizeFitter>();

                m_contentFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
                m_contentFitter.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
            }

            //创建及删除节点的Handler
            CreateGameObjectHandler = AddChild;
            DeleteGameObjectHandler = DeleteChild;
        }


        protected virtual Vector2 ContentSpacing
        {
            get
            {
                if (m_contentSpacing != Vector2.zero)
                    return m_contentSpacing;
                else if (m_gridLayout != null)
                    m_contentSpacing = m_gridLayout.spacing;
                else
                    m_contentSpacing = Vector2.zero;

                return m_contentSpacing;
            }
        }


        protected int contentConstraintCount
        {
            get
            {
                if (m_ContentConstraintCount > 0)
                    return m_ContentConstraintCount;

                m_ContentConstraintCount = 1;
                if (m_gridLayout != null)
                {
                    if (m_gridLayout.constraint == GridLayoutGroup.Constraint.Flexible)
                        Debug.Log("不支持Flexible Constraint ");
                    else
                        m_ContentConstraintCount = m_gridLayout.constraintCount;
                }

                return m_ContentConstraintCount;
            }
        }

        /// <summary>
        /// 更新节点
        /// </summary>
        /// <param name="viewBounds"></param>
        /// <param name="contentBounds"></param>
        /// <returns></returns>
        protected abstract bool UpdateItems(Bounds viewBounds, Bounds contentBounds);

        /// <summary>
        /// 获取节点大小
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        protected abstract float GetSize(RectTransform item);

        protected abstract Vector2 GetVector(float value);

        /// <summary>
        /// 清空所有子节点
        /// </summary>
        public void ClearCells()
        {
            if (Application.isPlaying)
            {
                itemTypeStart = 0;
                itemTypeEnd = 0;
                totalCount = 0;
                for (int i = content.childCount - 1; i >= 0; i--)
                {
                    Transform find = content.GetChild(i);
                    ReturnObject(i, find);
                }
            }
        }

        public void RefreshCells()
        {
            if (Application.isPlaying && this.isActiveAndEnabled)
            {
                itemTypeEnd = itemTypeStart = 0;

                if (content.childCount > 0)
                {
                    //处理已存在的元素
                    for (int i = 0; i < content.childCount; i++)
                    {
                        Transform find = content.GetChild(i);

                        if (totalCount > 0 && i == 0)
                        {
                            if (OnAddChildItem != null)
                                OnAddChildItem(i, find.gameObject);

                            continue;
                        }



                        if (itemTypeEnd < totalCount - 1)
                        {
                            if (OnAddChildItem != null)
                            {
                                OnAddChildItem(i, find.gameObject);
                            }


                            itemTypeEnd++;

                        }

                        else
                        {
                            ReturnObject(i, find);
                            i--;
                        }
                    }

                }

            }


        }


        public void RefillCells(int startIdx = 0)
        {
            if (Application.isPlaying)
            {
                content.anchoredPosition = new Vector2(oldContentX, oldContentY);

                StopMovement();
                itemTypeStart = reverseDirection ? totalCount - startIdx : startIdx;
                itemTypeEnd = itemTypeStart;

                for (int i = 0; i < content.childCount; i++)
                {
                    Transform find = content.GetChild(i);

                    if (totalCount >= 0 && itemTypeEnd >= totalCount)
                    {
                        ReturnObject(i, find);
                        i--;
                    }
                    else
                    {
                        if (OnAddChildItem != null)
                            OnAddChildItem(i, find.gameObject);

                        itemTypeEnd++;
                    }
                }

                Vector2 pos = content.anchoredPosition;
                if (directionSign == -1)
                    pos.y = 0;
                else if (directionSign == 1)
                    pos.x = 0;
                content.anchoredPosition = pos;

                UpdateBoundsAndItems();
            }
        }

        protected float NewItemAtStart()
        {
            if (totalCount >= 0 && itemTypeStart - contentConstraintCount < 0)
            {
                return 0;
            }

            if (totalCount == 0)
                return 0;

            float size = 0;
            for (int i = 0; i < contentConstraintCount; i++)
            {
                itemTypeStart--;
                RectTransform newItem = InstantiateNextItem(itemTypeStart);
                newItem.SetAsFirstSibling();
                size = Mathf.Max(GetSize(newItem), size);
            }

            if (!reverseDirection)
            {
                Vector2 offset = GetVector(size);
                content.anchoredPosition += offset;
                m_PrevPosition += offset;
                m_ContentStartPosition += offset;
            }

            return size;
        }


        protected float DeleteItemAtStart()
        {
            //if ((totalCount >= 0 && itemTypeEnd >= totalCount - 1) || content.childCount == 0)
            //{
            //    return 0;
            //}


            float size = 0;
            for (int i = 0; i < contentConstraintCount; i++)
            {
                Transform find = null;
                for (int j = 0; j < content.childCount; j++)
                {
                    find = content.GetChild(j);
                    if (find.name != TEMPLATE && find.name != POOLOBJECT)
                    {
                        break;
                    }
                }
                if (find == null)
                {
                    return 0;
                }
                RectTransform oldItem = find.GetComponent<RectTransform>();
                size = Mathf.Max(GetSize(oldItem), size);
                ReturnObject(itemTypeStart, oldItem);

                itemTypeStart++;

                if (content.childCount == 0)
                {
                    break;
                }
            }

            if (!reverseDirection)
            {
                Vector2 offset = GetVector(size);
                content.anchoredPosition -= offset;
                m_PrevPosition -= offset;
                m_ContentStartPosition -= offset;
            }

            return size;
        }


        protected float NewItemAtEnd()
        {

            /*      if (totalCount > 0 && itemTypeEnd >= totalCount - 1 && itemTypeEnd == ItemStartIndex && content.childCount > 0 || totalCount == 0)
                  {
                      return 0;
                  }*/

            if (totalCount > 0 && itemTypeEnd >= totalCount - 1 && content.childCount > 0)
                return 0;

            if (totalCount == 0)
                return 0;

            if (totalCount == 1 && content.childCount == 1)
                return 0;


            float size = 0;

            for (int i = 0; i < contentConstraintCount; ++i)
            {
                //第一个的问题,由于初始化是itemStart=itemEnd = 0,所以第一个需要单独处理
                if (content.childCount > 0)
                    itemTypeEnd++;

                RectTransform newItem = InstantiateNextItem(itemTypeEnd);

                size = Mathf.Max(GetSize(newItem), size);

                if (totalCount >= 0 && itemTypeEnd >= totalCount - 1)
                {
                    break;
                }
            }


            if (reverseDirection)
            {
                Vector2 offset = GetVector(size);
                content.anchoredPosition -= offset;
                m_PrevPosition -= offset;
                m_ContentStartPosition -= offset;
            }

            return size;
        }

        protected float DeleteItemAtEnd()
        {
            float size = 0;
            for (int i = 0; i < contentConstraintCount; i++)
            {
                Transform find = null;
                for (int j = content.childCount - 1; j >= 0; j--)
                {
                    find = content.GetChild(j);
                    break;
                }
                if (find == null)
                {
                    return 0;
                }

                RectTransform oldItem = find.GetComponent<RectTransform>();
                size = Mathf.Max(GetSize(oldItem), size);
                int returnObjectIndex = itemTypeEnd;

                ReturnObject(returnObjectIndex, oldItem);
                itemTypeEnd--;

                if ((itemTypeEnd + 1) % contentConstraintCount == 0 || content.childCount == 0)
                {
                    break;  //just delete the whole row
                }
            }

            if (reverseDirection)
            {
                Vector2 offset = GetVector(size);
                content.anchoredPosition += offset;
                m_PrevPosition += offset;
                m_ContentStartPosition += offset;
            }

            return size;
        }


        private RectTransform InstantiateNextItem(int itemIdx)
        {
            GameObject child = CreateGameObjectHandler();

            if (OnAddChildItem != null)
                OnAddChildItem(itemIdx, child);

            return child.GetComponent<RectTransform>();
        }

        private void ReturnObject(int itemIndex, Transform go)
        {
            if (OnDeleteChildItem != null)
                OnDeleteChildItem(itemIndex, go.gameObject);

            DeleteGameObjectHandler(go.gameObject);
        }


        #region 对象容器相关
        private Transform m_containerTransform = null;
        private const string TEMPLATE = "Template";
        private const string POOLOBJECT = "pool";


        protected Transform ContainerTransform
        {
            get
            {
                if (m_containerTransform == null)
                    m_containerTransform = transform;
                return m_containerTransform;
            }
        }


        #endregion

        private bool _usePool = true;
        private Transform poolTransform;

        GameObject template;

        float oldContentX, oldContentY;

        #region Only Pool GameObject
        private MemoryPool<GameObject> m_childPool;

        #endregion


        public bool UseChildPool
        {
            set
            {
                _usePool = value;
                if (value)
                {

                    m_childPool = new MemoryPool<GameObject>();

                    GameObject temp = new GameObject();
                    temp.name = POOLOBJECT;
                    temp.SetActive(false);
                    poolTransform = temp.transform;
                    poolTransform.parent = ContainerTransform;

                    Vector3 localScale = template.transform.localScale;
                    Quaternion localRotation = template.transform.localRotation;

                    template.transform.parent = poolTransform;
                    template.transform.localScale = localScale;
                    template.transform.localRotation = localRotation;
                }
                else
                {
                    if (m_childPool == null)
                        return;
                    GameObject obj = m_childPool.Alloc();
                    if (obj != null)
                    {
                        GameObject.Destroy(obj.gameObject);
                    }
                    m_childPool.Dispose();
                }
            }
            get
            {
                return _usePool;
            }
        }

        public void OnInit()
        {
            //找到Template模板
            if (content == null || content.childCount == 0)
            {
                Debug.LogError("没有找到template节点!");
                return;
            }


            for (int i = 0; i < content.childCount; ++i)
            {
                Transform contentChild = content.GetChild(i);
                if (contentChild != null)
                {
                    template = contentChild.gameObject;
                    template.SetActive(false);
                    break;
                }
            }
            if (template == null)
            {
                Debug.LogError("没有找到template节点!");
                return;
            }
            oldContentX = content.anchoredPosition.x;
            oldContentY = content.anchoredPosition.y;

            UseChildPool = _usePool;
        }

        private GameObject AddChild()
        {
            GameObject go = null;
            if (_usePool)
            {
                go = m_childPool.Alloc();

                if (go != null)
                    go.transform.SetParent(content);

            }
            //if (go == null)
            //    go = GOGUITools.AddChild(content.gameObject, template);

            go.SetActive(true);

            return go;
        }


        private void DeleteChild(GameObject childObject)
        {
            if (_usePool)
            {
                m_childPool.Free(childObject);

                childObject.transform.SetParent(poolTransform);
            }
            else
                GameObject.Destroy(childObject);
        }

        public override void OnBeginDrag(PointerEventData eventData)
        {
            base.OnBeginDrag(eventData);
            this.m_currentMouseState = ScrollRectMouseStatus.BeginDrag;

            SendMouseStateChangedNotification(eventData);

            //获取Drag的item
            for (int i = 0; i < content.childCount; ++i)
            {
                Transform childObject = content.GetChild(i);
                RectTransform childRectTransform = childObject as RectTransform;

                if (RectTransformUtility.RectangleContainsScreenPoint(childRectTransform, eventData.position, eventData.enterEventCamera))
                {
                    this.m_onDraggingItemIndex = itemTypeStart + i;
                    this.m_onDraggingGameObject = childObject.gameObject;
                    break;
                }
            }



            if (OnDragItem != null && this.m_onDraggingGameObject != null)
                OnDragItem(this.m_onDraggingItemIndex, this.m_onDraggingGameObject);
        }


        public override void OnDrag(PointerEventData eventData)
        {
            if (eventData.button != PointerEventData.InputButton.Left)
                return;

            if (!IsActive())
                return;

            //使用PointerEventData 同时要使用eventData.neterEventCamera 否则这个接口会有问题
            if (!RectTransformUtility.RectangleContainsScreenPoint(viewRect, eventData.position, eventData.enterEventCamera))
            {
                if (m_currentMouseState == ScrollRectMouseStatus.ExitScrollRect)
                {
                    if (OnMouseExitScrollRectAndDragging != null)
                        OnMouseExitScrollRectAndDragging(eventData);
                }
                else if (m_currentMouseState == ScrollRectMouseStatus.Draging)
                {
                    this.m_currentMouseState = ScrollRectMouseStatus.ExitScrollRect;

                    SendMouseStateChangedNotification(eventData);
                }

                return;
            }
            else
            {
                if (m_currentMouseState == ScrollRectMouseStatus.ExitScrollRect)
                {
                    m_currentMouseState = ScrollRectMouseStatus.ReEnterScrollRect;

                    SendMouseStateChangedNotification(eventData);
                }
            }

            Vector2 localCursor;
            if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(viewRect, eventData.position, eventData.pressEventCamera, out localCursor))
                return;

            this.m_currentMouseState = ScrollRectMouseStatus.Draging;

            UpdateBoundsAndItems();

            var pointerDelta = localCursor - m_PointerStartLocalCursor;
            Vector2 position = m_ContentStartPosition + pointerDelta;

            Vector2 offset = CalculateOffset(position - content.anchoredPosition);
            position += offset;
            if (movementType == MovementType.Elastic)
            {
                if (offset.x != 0)
                    position.x = position.x - RubberDelta(offset.x, m_ViewBounds.size.x) * rubberScale;
                if (offset.y != 0)
                    position.y = position.y - RubberDelta(offset.y, m_ViewBounds.size.y) * rubberScale;
            }

            SetContentAnchoredPosition(position);


            if (OnScrollRectDragging != null)
                OnScrollRectDragging(eventData);
        }

        public override void OnEndDrag(PointerEventData eventData)
        {
            base.OnEndDrag(eventData);

            this.m_currentMouseState = ScrollRectMouseStatus.EndDrag;
            SendMouseStateChangedNotification(eventData);

            this.m_onDraggingItemIndex = -1;
            this.m_onDraggingGameObject = null;
        }

        protected override void SetContentAnchoredPosition(Vector2 position)
        {
            if (!horizontal)
                position.x = oldContentX;
            if (!vertical)
                position.y = oldContentY;

            //这里精度问题
            if (Vector2.Distance(position, content.anchoredPosition) > 0.001f)
            {
                content.anchoredPosition = position;
                if (OnContentAnchoredPositionChanged != null)
                    OnContentAnchoredPositionChanged();

                UpdateBoundsAndItems();
            }
        }



        protected override void LateUpdate()
        {

            if (!content)
                return;

            if (!horizontal)
                content.anchoredPosition = new Vector2(oldContentX, content.anchoredPosition.y);
            if (!vertical)
                content.anchoredPosition = new Vector2(content.anchoredPosition.x, oldContentY);

            UpdateBoundsAndItems();

            base.LateUpdate();
        }



        protected override void UpdateScrollbars(Vector2 offset)
        {
            if (horizontalScrollbar)
            {
                if (m_ContentBounds.size.x > 0 && totalCount > 0)
                {
                    horizontalScrollbar.size = Mathf.Clamp01((m_ViewBounds.size.x - Mathf.Abs(offset.x)) / m_ContentBounds.size.x * (itemTypeEnd - itemTypeStart) / totalCount);
                }
                else
                    horizontalScrollbar.size = 1;

                horizontalScrollbar.value = horizontalNormalizedPosition;
            }

            if (verticalScrollbar)
            {
                if (m_ContentBounds.size.y > 0 && totalCount > 0)
                    verticalScrollbar.size = Mathf.Clamp01((m_ViewBounds.size.y - Mathf.Abs(offset.y)) / m_ContentBounds.size.y * (itemTypeEnd - itemTypeStart) / totalCount);
                else
                    verticalScrollbar.size = 1;

                verticalScrollbar.value = verticalNormalizedPosition;
            }
        }


        private void SetNormalizedPosition(float value, int axis)
        {
            if (totalCount <= 0 || itemTypeEnd <= itemTypeStart)
                return;

            EnsureLayoutHasRebuilt();
            UpdateBoundsAndItems();

            Vector3 localPosition = content.localPosition;
            float newLocalPosition = localPosition[axis];
            if (axis == 0)
            {
                float elementSize = m_ContentBounds.size.x / (itemTypeEnd - itemTypeStart);
                float totalSize = elementSize * totalCount;
                float offset = m_ContentBounds.min.x - elementSize * itemTypeStart;

                newLocalPosition += m_ViewBounds.min.x - value * (totalSize - m_ViewBounds.size[axis]) - offset;
            }
            else if (axis == 1)
            {
                float elementSize = m_ContentBounds.size.y / (itemTypeEnd - itemTypeStart);
                float totalSize = elementSize * totalCount;
                float offset = m_ContentBounds.max.y + elementSize * itemTypeStart;

                newLocalPosition -= offset - value * (totalSize - m_ViewBounds.size.y) - m_ViewBounds.max.y;
            }

            if (Mathf.Abs(localPosition[axis] - newLocalPosition) > 0.01f)
            {
                localPosition[axis] = newLocalPosition;
                content.localPosition = localPosition;
                m_Velocity[axis] = 0;
                UpdateBoundsAndItems();
            }
        }


        private void UpdateBoundsAndItems()
        {
            m_ViewBounds = new Bounds(viewRect.rect.center, viewRect.rect.size);
            m_ContentBounds = GetBounds();

            if (content == null)
                return;

            if (Application.isPlaying && UpdateItems(m_ViewBounds, m_ContentBounds))
            {
                Canvas.ForceUpdateCanvases();
                m_ContentBounds = GetBounds();

            }
            base.UpdateBounds();
        }


        private void SendMouseStateChangedNotification(PointerEventData pointerData)
        {
            if (OnMouseStateChanged != null)
                OnMouseStateChanged((int)this.m_currentMouseState, pointerData);
        }

        /// <summary>
        /// 检查边界
        /// </summary>
        protected virtual bool UpdateEdgeStatus()
        {
            return false;
        }


        #region Properties
        /// <summary>
        /// 水平归一化的位置
        /// </summary>
        public override float horizontalNormalizedPosition
        {
            get
            {
                UpdateBounds();

                if (totalCount > 0 && itemTypeEnd > itemTypeStart)
                {
                    float elementRealSizeX = TemplateElementSize.x + ContentSpacing.x;

                    /*if (totalCount > 0)
                        this.m_realContentSize = totalCount * elementRealSize - ContentSpacing.x;
                    else
                        this.m_realContentSize = elementRealSize;*/


                    float offset = m_ContentBounds.min.x - elementRealSizeX * ItemStartIndex;

                    if (this.m_realContentSize.x <= m_ViewBounds.size.x)
                        return (m_ViewBounds.min.x > offset) ? 1 : 0;
                    else
                    {
                        //保留两位有效值
                        float normalizePosition = (m_ViewBounds.min.x - offset) / (this.m_realContentSize.x - m_ViewBounds.size.x);

                        return (int)(normalizePosition * 100) / 100f;
                    }
                }
                else
                    return 0.5f;
            }
            set
            {
                SetNormalizedPosition(value, 0);
            }
        }

        public override float verticalNormalizedPosition
        {
            get
            {
                UpdateBounds();

                if (totalCount > 0 && itemTypeEnd > itemTypeStart)
                {
                    float elementRealSizeY = TemplateElementSize.y + ContentSpacing.y;

                    float offset = m_ContentBounds.min.y - elementRealSizeY * ItemStartIndex;

                    if (this.m_realContentSize.y <= m_ViewBounds.size.y)
                        return (m_ViewBounds.min.y > offset) ? 1 : 0;
                    else
                    {
                        float normalizePosition = (m_ViewBounds.min.y - offset) / (this.m_realContentSize.y - m_ViewBounds.size.y);

                        return (int)(normalizePosition * 100) / 100f;
                    }
                }
                else
                    return 0.5f;
            }
            set
            {
                SetNormalizedPosition(value, 1);
            }

        }

        /// <summary>
        /// 计算真实内容大小
        /// </summary>
        /// <param name="totalSize"></param>
        /// <remarks>content 必须有grid约束否则按单排或单列处理</remarks>
        protected void UpdateReadContentSize(int totalSize)
        {
            if (totalSize == 0)
                this.m_realContentSize = Vector2.zero;
            else
            {
                if (m_gridLayout != null)
                {
                    if (m_gridLayout.constraint == GridLayoutGroup.Constraint.FixedColumnCount)
                    {
                        if (contentConstraintCount == 1)
                            m_realContentSize.x = TemplateElementSize.x;
                        else
                            m_realContentSize.x = contentConstraintCount * (TemplateElementSize.x + ContentSpacing.x) - ContentSpacing.x;


                        int rowCount = Mathf.CeilToInt((float)totalSize / contentConstraintCount);
                        if (rowCount == 1)
                            m_realContentSize.y = TemplateElementSize.y;
                        else
                            m_realContentSize.y = rowCount * (TemplateElementSize.y + ContentSpacing.y) - ContentSpacing.y;
                    }
                    else if (m_gridLayout.constraint == GridLayoutGroup.Constraint.FixedRowCount)
                    {
                        if (contentConstraintCount == 1)
                            m_realContentSize.y = TemplateElementSize.y;
                        else
                            m_realContentSize.y = contentConstraintCount * (TemplateElementSize.y + ContentSpacing.y) - ContentSpacing.y;

                        int columnCount = Mathf.CeilToInt((float)totalSize / contentConstraintCount);
                        if (columnCount == 1)
                            m_realContentSize.x = TemplateElementSize.x;
                        else
                            m_realContentSize.x = columnCount * (TemplateElementSize.x + ContentSpacing.x) - ContentSpacing.x;
                    }
                }
            }

        }


        /// <summary>
        /// 根据实际的索引(startindex endIndex 获取实际的Recttransform)
        /// </summary>
        /// <param name="realIndex"></param>
        /// <returns></returns>
        public RectTransform GetChildByRealIndex(int realIndex)
        {
            RectTransform targetTransform = null;
            if (realIndex == 0 && ItemStartIndex == 0 && ItemEndIndex == 0)
                targetTransform = content.GetChild(0) as RectTransform;
            else
            {
                int indexInContent = realIndex - ItemStartIndex;
                targetTransform = content.GetChild(indexInContent) as RectTransform;
            }
            return targetTransform;
        }


        public int OnDraggingItemIndex
        {
            get { return m_onDraggingItemIndex; }
        }

        public GameObject OnDraggingGameObject
        {
            get { return m_onDraggingGameObject; }
        }

        public int ItemStartIndex
        {
            get { return itemTypeStart; }
        }

        public int ItemEndIndex
        {
            get { return itemTypeEnd; }
        }

        public bool IsMoveToStart
        {
            get
            {
                if (totalCount < 0)
                    return false;
                else if (totalCount == 0)
                    return true;
                else
                    return horizontalNormalizedPosition <= 0;
            }
        }

        public bool IsMoveToEnd
        {
            get
            {
                if (totalCount < 0)
                    return false;
                else if (totalCount == 0)
                    return true;
                else
                    return horizontalNormalizedPosition >= 1;
            }
        }

        /// <summary>
        /// 当前Scroll Rect是否是合法位置
        /// </summary>
        protected virtual bool IsValidPosition
        {
            get { return true; }
        }


        protected Vector2 TemplateElementSize
        {
            get
            {
                if (m_gridLayout != null)
                {
                    return m_gridLayout.cellSize;
                }

                return Vector2.zero;
            }
        }

        public int TotalCount
        {
            get { return totalCount; }
            set
            {
                totalCount = value;
                UpdateReadContentSize(value);
            }
        }

        public Bounds ViewBounds
        {
            get { return m_ViewBounds; }
        }

        #endregion


        public enum ScrollRectMouseStatus
        {
            None,
            BeginDrag,
            Draging,
            ReEnterScrollRect,
            ExitScrollRect,
            EndDrag
        }
    }
}