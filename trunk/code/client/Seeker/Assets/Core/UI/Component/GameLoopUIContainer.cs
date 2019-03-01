/********************************************************************
	created:  2019-1-3 19:53:46
	filename: GameLoopUIContainer.cs
	author:	  songguangze@outlook.com
	
	purpose:  循环利用的通用容器控件
*********************************************************************/
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace EngineCore
{
    public class GameLoopUIContainer<T> : GameUIContainer where T : GameLoopItem, new()
    {
        private ScrollRect m_scrollRect;
        /// <summary>每个Item的边距</summary>
        private Vector2 m_itemSpacing;
        /// <summary>每个格子的大小（Size + Spacing）</summary>
        private Vector2 m_GridSize;
        /// <summary>每个格子的单向大小（垂直是y, 水平是x）</summary>
        float m_GridSizeOne;
        /// <summary>可视区域内Item的列数</summary>
        private int m_viewItemColCount = 1;
        /// <summary>可视区域内Item的行数</summary>
        private int m_viewItemRowCount = 1;
        /// <summary>行数*列数</summary>
        private int m_viewItemCount;
        /// <summary>scrowview的widget</summary>
        private RectTransform m_content;
        /// <summary>布局方式</summary>
        private LayoutGroup m_LayoutGroup;
        /// <summary>边距</summary>
        private RectOffset m_oldPadding;
        /// <summary>数据数组渲染的起始下标(从0开始)</summary>
        private int m_startRowOrCol;
        /// <summary>上一次数据数组渲染的起始下标(从0开始)</summary>
        private int last_startRowOrCol;
        /// <summary>Panel面板的初始位置</summary>
        Vector2 panelPos;
        /// <summary>第一个元素的位置只设置一次就好</summary>
        bool haveSetStartPos = false;
        /// <summary>第一个元素的位置</summary>
        Vector2 itemStartPos = Vector2.zero;
        /// <summary>第一个元素的渲染值只设置一次就好</summary>
        bool haveFindStartSibling = false;
        /// <summary>第一个元素的渲染值</summary>
        int itemStartSibling;
        /// <summary>第一个元素和第二个元素的x差， 第一行元素和第二行元素的y差</summary>
        Vector2 itemAddPos = Vector2.zero;

        /// <summary>从上到下或者从左到右</summary>
        public bool IsUpToDownOrLeftToRight = true;
        //可见区域长度
        private float ViewSpace;
        /// <summary>拉到最底部时候开始的行（列）（从0开始的下标）</summary>
        int showBottomStartRowOrCol;
        /// <summary>当前数据量显示所需要的所有行（列）数</summary>
        int totalRowOrCol;
        /// <summary>可视区域可以显示的行（列）数</summary>
        int viewRowOrCol;
        /// <summary>数据量个数</summary>
        private int m_dataCount;
        /// <summary>在下一帧设置为true</summary>
        bool canScrowRectMove = false;
        /// <summary>比实际增加几行或者几列</summary>
        public int addRowOrCol = 1;
        /// <summary>可以显示的行或者列</summary>
        int canShowRowOrCol = 0;
        public bool isDirectionOpposite = false;

        //对象池
        private ObjectPool<T> m_dataPool = new ObjectPool<T>();
        private List<T> m_currentDataItemList = new List<T>();

        Vector2 GetPosByIndex(int index)
        {
            if (IsVertical)
            {
                return itemStartPos + new Vector2((index % m_viewItemColCount) * itemAddPos.x, (index / m_viewItemColCount) * itemAddPos.y);
            }
            else
            {
                return itemStartPos + new Vector2((index / m_viewItemRowCount) * itemAddPos.x, (index % m_viewItemRowCount) * itemAddPos.y);
            }
        }

        List<RectTransform> lastChildOrderList = new List<RectTransform>();

        public int ChildItemCount
        {
            get
            {
                if (childList.Count > 0 && !childList[0])
                {
                    childListDirty = true;
                }
                if (childListDirty)
                {
                    childListDirty = false;
                    childList.Clear();
                    lastChildOrderList.Clear();
                    for (int i = 0; i < ContainerTransform.childCount; i++)
                    {
                        var t = ContainerTransform.GetChild(i);
                        if (t == poolTransform)
                            continue;
                        if (t == ContainerTemplate)
                        {
                            continue;
                        }
                        t.gameObject.SetActive(true);
                        childList.Add(t.GetComponent<RectTransform>());
                    }
                    if (childList.Count > 0 && !haveFindStartSibling)
                    {
                        haveFindStartSibling = true;
                        itemStartSibling = childList[0].GetSiblingIndex();
                    }
                    int start = itemStartSibling;
                    for (int i = 0; i < childList.Count; ++i)
                    {
                        //设置一下渲染顺序
                        childList[i].SetSiblingIndex(start);
                        lastChildOrderList.Add(childList[i]);
                        ++start;
                    }
                }
                return childList.Count;
            }
        }

        /// <summary>
        /// 设置当前数据量
        /// </summary>
        private int DataCount
        {
            set
            {
                if (value < 0)
                {
                    value = 0;
                }
                m_dataCount = value;
                if (IsVertical)
                {
                    if (value % m_viewItemColCount == 0)
                    {
                        totalRowOrCol = value / m_viewItemColCount;
                    }
                    else
                    {
                        totalRowOrCol = value / m_viewItemColCount + 1;
                    }
                }
                else
                {
                    if (value % m_viewItemRowCount == 0)
                    {
                        totalRowOrCol = value / m_viewItemRowCount;
                    }
                    else
                    {
                        totalRowOrCol = value / m_viewItemRowCount + 1;
                    }
                }
                showBottomStartRowOrCol = Mathf.Max(0, totalRowOrCol - viewRowOrCol);
                canScrowRectMove = true;
                last_startRowOrCol = -100000;
                if (nextFrameIndex >= 0)
                {
                    ScrollTo(nextFrameIndex);
                }
                ScorwRectMove(Vector2.zero);
            }
        }
        /// <summary>设置m_GridSize和m_GridSizeOne</summary>
        Vector2 SetGridSize
        {
            set
            {
                m_GridSize = value;
                m_GridSizeOne = IsVertical ? value.y : value.x;
            }
        }
        /// <summary>设置扩展</summary>
        void ExpandPadding()
        {
            int frontSpace = 0;
            int behindSpace = 0;
            if (IsUpToDownOrLeftToRight)
            {
                if (isDirectionOpposite)
                {
                    behindSpace = Mathf.CeilToInt(m_startRowOrCol * m_GridSizeOne);
                    frontSpace = Mathf.CeilToInt(Mathf.Max(0, m_GridSizeOne * showBottomStartRowOrCol - behindSpace));
                }
                else
                {
                    frontSpace = Mathf.CeilToInt(m_startRowOrCol * m_GridSizeOne);
                    behindSpace = Mathf.CeilToInt(Mathf.Max(0, m_GridSizeOne * showBottomStartRowOrCol - frontSpace));
                }
            }
            else
            {
                if (isDirectionOpposite)
                {
                    frontSpace = Mathf.CeilToInt(m_startRowOrCol * m_GridSizeOne);
                    behindSpace = Mathf.CeilToInt(Mathf.Max(0, m_GridSizeOne * showBottomStartRowOrCol - frontSpace));
                }
                else
                {
                    behindSpace = Mathf.CeilToInt(m_startRowOrCol * m_GridSizeOne);
                    frontSpace = Mathf.CeilToInt(Mathf.Max(0, m_GridSizeOne * showBottomStartRowOrCol - behindSpace));
                }
            }
            if (IsVertical)
            {
                m_LayoutGroup.padding = new RectOffset(m_oldPadding.left, m_oldPadding.right, m_oldPadding.top + frontSpace, m_oldPadding.bottom + behindSpace);
            }
            else
            {
                m_LayoutGroup.padding = new RectOffset(m_oldPadding.left + frontSpace, m_oldPadding.right + behindSpace, m_oldPadding.top, m_oldPadding.bottom);
            }
        }

        void HandleItem(RectTransform rect, int dataIndex)
        {
            rect.anchoredPosition = GetPosByIndex(dataIndex);

            if (dataIndex < m_dataCount)
            {
                T updateItem = GetChild(dataIndex);
                updateItem.SetItemGameObject(rect.gameObject);
                updateItem.OnLoopItemAppear();
                updateItem.Visible = true;      //todo:onshow onhide 逻辑也会走。之后看能不能把接口整合
            }
            else
            {
                rect.gameObject.SetActive(false);
            }
        }

        private void OnLoopItemDisappear(int dataIndex)
        {
            if (dataIndex < m_dataCount)
            {
                T disappareItem = GetChild(dataIndex);
                disappareItem.OnLoopItemDisappear();
            }
        }

        int nextFrameIndex = -1;
        /// <summary>
        /// 在调用此方法的时候确定  VisibleInHierarchy == true
        /// 定位到第几行或者第几列, 在显示区域的层数， 在最底或者最高， 其他在可见的第一层
        /// </summary>
        /// <param name="index">下标(从0开始)</param>
        public void ScrollTo(int index)
        {
            if (canScrowRectMove)
            {
                int rowOrCol = 1;
                if (IsVertical)
                {
                    rowOrCol = (index / m_viewItemColCount + 1);
                }
                else
                {
                    rowOrCol = (index / m_viewItemRowCount + 1);
                }
                rowOrCol = Mathf.Min(rowOrCol, totalRowOrCol);
                if (rowOrCol < canShowRowOrCol)
                {
                    m_content.anchoredPosition = panelPos;
                }
                else
                {
                    int maxMin = totalRowOrCol - canShowRowOrCol;
                    if (maxMin < 0)
                    {
                        m_content.anchoredPosition = panelPos;
                    }
                    else if (rowOrCol > maxMin)
                    {
                        SetPos(totalRowOrCol, ViewSpace);
                    }
                    else
                    {
                        SetPos(rowOrCol - canShowRowOrCol / 2);
                    }
                }
                nextFrameIndex = -1;
            }
            else
            {
                nextFrameIndex = index;
            }
        }

        void SetPos(int index, float height = 0)
        {
            float space = 0;
            if (IsUpToDownOrLeftToRight)
            {
                space = m_GridSizeOne;
            }
            else
            {
                space = -m_GridSizeOne;
                height = -height;
            }
            if (IsVertical)
            {
                m_content.anchoredPosition = new Vector2(panelPos.x, panelPos.y - height + index * space);
            }
            else
            {
                m_content.anchoredPosition = new Vector2(panelPos.x - height + index * space, panelPos.y);
            }
        }

        /// <summary>
        /// 水平滚动
        /// </summary>
        /// <param name="count"></param>
        void HorticalRectMove(int count)
        {
            if (last_startRowOrCol != m_startRowOrCol)
            {
                int moveRowOrCol = m_startRowOrCol - last_startRowOrCol;
                if (Mathf.Abs(moveRowOrCol) >= viewRowOrCol)
                {
                    int dataStartIndex = m_startRowOrCol * m_viewItemRowCount;
                    for (int i = 0; i < count; ++i, ++dataStartIndex)
                    {
                        HandleItem(childList[i], dataStartIndex);
                    }
                }
                else if (moveRowOrCol > 0)
                {
                    ///不用更新的Item
                    int currentChildStart = 0;
                    int lastOrderStrat = moveRowOrCol * m_viewItemRowCount;
                    int totalCount = (viewRowOrCol - moveRowOrCol) * m_viewItemRowCount;
                    for (int i = 0; i < totalCount; ++i)
                    {
                        childList[currentChildStart] = lastChildOrderList[lastOrderStrat];
                        ++currentChildStart;
                        ++lastOrderStrat;
                    }
                    //以下是要更新的Item
                    int dataStartIndex = (last_startRowOrCol + viewRowOrCol) * m_viewItemRowCount;
                    lastOrderStrat = 0;
                    totalCount = moveRowOrCol * m_viewItemRowCount;
                    for (int i = 0; i < totalCount; ++i)
                    {
                        childList[currentChildStart] = lastChildOrderList[lastOrderStrat];
                        OnLoopItemDisappear(dataStartIndex - viewRowOrCol * m_viewItemRowCount);
                        childList[currentChildStart].SetAsLastSibling(); //设置渲染顺序 
                        HandleItem(childList[currentChildStart], dataStartIndex);
                        ++currentChildStart;
                        ++lastOrderStrat;
                        ++dataStartIndex;
                    }
                }
                else
                {
                    //反向滑动

                    moveRowOrCol = -moveRowOrCol;
                    //不用更新的Item
                    int currentChildStart = moveRowOrCol * m_viewItemRowCount;
                    int lastOrderStrat = 0;
                    int totalCount = (viewRowOrCol - moveRowOrCol) * m_viewItemRowCount;
                    for (int i = 0; i < totalCount; ++i)
                    {
                        childList[currentChildStart] = lastChildOrderList[lastOrderStrat];
                        ++currentChildStart;
                        ++lastOrderStrat;
                    }
                    //以下是要更新的Item
                    int dataEndIndex = last_startRowOrCol * m_viewItemRowCount - 1;
                    currentChildStart = moveRowOrCol * m_viewItemRowCount - 1;
                    lastOrderStrat = viewRowOrCol * m_viewItemRowCount - 1;
                    totalCount = moveRowOrCol * m_viewItemRowCount;
                    for (int i = 0; i < totalCount; ++i)
                    {
                        childList[currentChildStart] = lastChildOrderList[lastOrderStrat];
                        OnLoopItemDisappear(dataEndIndex + viewRowOrCol * m_viewItemRowCount);
                        childList[currentChildStart].SetSiblingIndex(itemStartSibling);//设置渲染顺序
                        HandleItem(childList[currentChildStart], dataEndIndex);
                        --currentChildStart;
                        --lastOrderStrat;
                        --dataEndIndex;
                    }
                }
                //更新当前存储的列表
                for (int i = 0; i < count; ++i)
                {
                    lastChildOrderList[i] = childList[i];
                }
                ExpandPadding(); //扩展一下方向
                last_startRowOrCol = m_startRowOrCol;
            }
        }

        /// <summary>
        /// 垂直滚动
        /// </summary>
        /// <param name="count"></param>
        void VerticalRectMove(int count)
        {
            if (last_startRowOrCol != m_startRowOrCol)
            {
                int moveRowOrCol = m_startRowOrCol - last_startRowOrCol;
                if (Mathf.Abs(moveRowOrCol) >= viewRowOrCol)
                {
                    int dataStartIndex = m_startRowOrCol * m_viewItemColCount;
                    for (int i = 0; i < count; ++i, ++dataStartIndex)
                    {
                        HandleItem(childList[i], dataStartIndex);
                    }
                }
                else if (moveRowOrCol > 0)
                {
                    //不用更新的Item
                    int currentChildStart = 0;
                    int lastOrderStrat = moveRowOrCol * m_viewItemColCount;
                    int totalCount = (viewRowOrCol - moveRowOrCol) * m_viewItemColCount;
                    for (int i = 0; i < totalCount; ++i)
                    {
                        childList[currentChildStart] = lastChildOrderList[lastOrderStrat];
                        ++currentChildStart;
                        ++lastOrderStrat;
                    }
                    //以下是要更新的Item
                    int dataStartIndex = (last_startRowOrCol + viewRowOrCol) * m_viewItemColCount;
                    lastOrderStrat = 0;
                    totalCount = moveRowOrCol * m_viewItemColCount;
                    for (int i = 0; i < totalCount; ++i)
                    {
                        childList[currentChildStart] = lastChildOrderList[lastOrderStrat];
                        OnLoopItemDisappear(dataStartIndex - viewRowOrCol * m_viewItemRowCount);
                        childList[currentChildStart].SetAsLastSibling(); //设置渲染顺序 
                        HandleItem(childList[currentChildStart], dataStartIndex);
                        ++currentChildStart;
                        ++lastOrderStrat;
                        ++dataStartIndex;
                    }
                }
                else
                {
                    moveRowOrCol = -moveRowOrCol;
                    //不用更新的Item
                    int currentChildStart = moveRowOrCol * m_viewItemColCount;
                    int lastOrderStrat = 0;
                    int totalCount = (viewRowOrCol - moveRowOrCol) * m_viewItemColCount;
                    for (int i = 0; i < totalCount; ++i)
                    {
                        childList[currentChildStart] = lastChildOrderList[lastOrderStrat];
                        ++currentChildStart;
                        ++lastOrderStrat;
                    }
                    //以下是要更新的Item
                    int dataEndIndex = last_startRowOrCol * m_viewItemColCount - 1;
                    currentChildStart = moveRowOrCol * m_viewItemColCount - 1;
                    lastOrderStrat = viewRowOrCol * m_viewItemColCount - 1;
                    totalCount = moveRowOrCol * m_viewItemColCount;
                    for (int i = 0; i < totalCount; ++i)
                    {
                        childList[currentChildStart] = lastChildOrderList[lastOrderStrat];
                        OnLoopItemDisappear(dataEndIndex + viewRowOrCol * m_viewItemRowCount);
                        childList[currentChildStart].SetSiblingIndex(itemStartSibling);//设置渲染顺序
                        HandleItem(childList[currentChildStart], dataEndIndex);
                        --currentChildStart;
                        --lastOrderStrat;
                        --dataEndIndex;
                    }
                }
                //更新当前存储的列表
                for (int i = 0; i < count; ++i)
                {
                    if (lastChildOrderList.Count > i)
                        lastChildOrderList[i] = childList[i];
                    else
                        lastChildOrderList.Add(childList[i]);
                }
                ExpandPadding(); //扩展一下方向
                last_startRowOrCol = m_startRowOrCol;
            }
        }

        Vector2 currentPos = Vector2.zero;
        /// <summary>
        /// childList 比如原来是   0 1；2 3； 4 5向上移动了两行， 变成  2 3； 4 5； 0 1
        /// </summary>
        /// <param name="data"></param>
        void ScorwRectMove(Vector2 data)
        {
            //if (m_dataCount <= 0)
            //    return;

            int count = ChildItemCount;
            if (count <= 0 || !canScrowRectMove)
                return;

            currentPos = m_content.anchoredPosition;
            float currentOffset = 0;
            if (IsVertical)
            {
                if (IsUpToDownOrLeftToRight)
                {
                    if (currentPos.y >= panelPos.y)
                        currentOffset = currentPos.y - panelPos.y;
                }
                else
                {
                    if (currentPos.y <= panelPos.y)
                        currentOffset = panelPos.y - currentPos.y;
                }
            }
            else
            {
                /*if (IsUpToDownOrLeftToRight)
                {
                    if (currentPos.x >= panelPos.x)
                        currentOffset = currentPos.x - panelPos.x;
                }
                else
                {
                    if (currentPos.x <= panelPos.x)
                        currentOffset = panelPos.x - currentPos.x;
                }*/
                //Scroll View方向为从左到右
                if (IsUpToDownOrLeftToRight)
                {
                    if (currentPos.x <= panelPos.x)
                        currentOffset = panelPos.x - currentPos.x;
                }
                else
                {
                    if (currentPos.x >= panelPos.x)
                        currentOffset = currentPos.x - panelPos.x;
                }
            }

            //垂直算出来的是行，水平算出来的是列
            m_startRowOrCol = Mathf.Min(Mathf.FloorToInt(currentOffset / m_GridSizeOne), showBottomStartRowOrCol);
            if (IsVertical)
            {
                VerticalRectMove(count);
            }
            else
            {
                HorticalRectMove(count);
            }
        }

        protected override void OnInit()
        {
            base.OnInit();
            //记录下m_itemSpacing的大小
            Vector2 m_itemSize = Vector2.zero;
            m_LayoutGroup = gameObject.GetComponent<LayoutGroup>();
            if (m_LayoutGroup is GridLayoutGroup)
            {
                var gridGroup = m_LayoutGroup as GridLayoutGroup;
                m_itemSize = gridGroup.cellSize;
                m_itemSpacing = gridGroup.spacing;
            }
            else
            {
                LayoutElement layOut = ContainerTemplate.GetComponent<LayoutElement>();
                if (m_LayoutGroup is HorizontalLayoutGroup)
                {
                    float width = layOut.minWidth;
                    if (width <= 0)
                    {
                        width = layOut.preferredWidth;
                    }
                    float height = layOut.preferredHeight;
                    if (height <= 0)
                    {
                        height = layOut.minHeight;
                    }
                    m_itemSize = new Vector2(width, height);
                    m_itemSpacing = new Vector2((m_LayoutGroup as HorizontalLayoutGroup).spacing, 0);
                }
                else
                {
                    float width = layOut.preferredWidth;
                    if (width <= 0)
                    {
                        width = layOut.minWidth;
                    }
                    float height = layOut.minHeight;
                    if (height <= 0)
                    {
                        height = layOut.preferredHeight;
                    }
                    m_itemSize = new Vector2(width, height);
                    m_itemSpacing = new Vector2(0, (m_LayoutGroup as VerticalLayoutGroup).spacing);
                }
            }
            m_oldPadding = m_LayoutGroup.padding;

            //if (this.m_scrollRect)
            SetGridSize = m_itemSize + m_itemSpacing;
        }
        public override void OnHide()
        {
            m_dataCount = 0;
            base.OnHide();
        }

        public override void Clear()
        {
            //base.Clear();
            m_dataCount = 0;

            for (int i = 0; i < m_currentDataItemList.Count; ++i)
                this.m_dataPool.Release(m_currentDataItemList[i]);

            this.m_currentDataItemList.Clear();
        }

        /// <summary>
        /// 外部调用增加一条数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public void AddOneData()
        {
            InitM_ScrowRect();
            ++m_dataCount;
            if (ChildItemCount >= m_viewItemCount)
            {
                DataCount = m_dataCount;
            }
            else
            {
                EngineCoreEvents.SystemEvents.SendTaskExecuteNextFrame.SafeInvoke(RefreshChildPosition);
                AddChild();
            }
        }
        /// <summary>
        /// 外部调用的减少一条数据
        /// </summary>
        /// <returns></returns>
        public void RemoveOneData()
        {
            if (m_dataCount <= 0) return;
            InitM_ScrowRect();
            --m_dataCount;
            if (m_dataCount >= m_viewItemCount)
            {
                DataCount = m_dataCount;
            }
            else
            {
                int count = ChildItemCount;
                if (count > 0)
                {
                    RemoveChildByIndex(count - 1);
                    EngineCoreEvents.SystemEvents.SendTaskExecuteNextFrame.SafeInvoke(RefreshChildPosition);
                }
            }
        }

        /// <summary>
        /// 因为ShowUI的时候  activeInHierarchy为false， 所以GetComponentInParent不可用
        /// </summary>
        public int InitM_ScrowRect()
        {
            canScrowRectMove = false;
            nextFrameIndex = -1;
            if (m_scrollRect != null)
                return m_viewItemCount;

            if (Widget.gameObject.activeInHierarchy)
            {
                m_scrollRect = Widget.GetComponentInParent<ScrollRect>();
            }
            else
            {
                m_scrollRect = Widget.parent.GetComponent<ScrollRect>();
            }
            RectTransform m_tranScrollRect = m_scrollRect.GetComponent<RectTransform>();
            m_content = m_scrollRect.content;
            panelPos = m_content.anchoredPosition;

            float height = m_GridSize.y;
            float width = m_GridSize.x;
            if (IsVertical) //垂直排列
            {
                ViewSpace = m_tranScrollRect.rect.height;
                //canShowRowOrCol = Mathf.CeilToInt(ViewSpace / height);
                canShowRowOrCol = Mathf.FloorToInt(ViewSpace / height);
                m_viewItemRowCount = canShowRowOrCol + addRowOrCol;
                if (width <= 0)
                    width = m_tranScrollRect.rect.width;
                m_viewItemColCount = Mathf.Max(1, Mathf.FloorToInt(m_tranScrollRect.rect.width / width));
                viewRowOrCol = m_viewItemRowCount;

                //垂直布局里的Grid 子节点排列必须是Horizontal，否则逻辑也不对，重用的ID也没法计算
                if (m_LayoutGroup is GridLayoutGroup)
                {
                    if (((GridLayoutGroup)m_LayoutGroup).startAxis == GridLayoutGroup.Axis.Vertical)
                        Debug.LogError($"Scroll View is Vertical,GridLayout StartAxis must be Axis.Horizontal");
                }

            }
            else
            {
                ViewSpace = m_tranScrollRect.rect.width;
                if (height <= 0)
                    height = m_tranScrollRect.rect.height;
                m_viewItemRowCount = Mathf.Max(1, Mathf.FloorToInt(m_tranScrollRect.rect.height / height));
                canShowRowOrCol = Mathf.CeilToInt(ViewSpace / width);
                m_viewItemColCount = canShowRowOrCol + addRowOrCol;
                viewRowOrCol = m_viewItemColCount;

                if (m_LayoutGroup is GridLayoutGroup)
                {
                    if (((GridLayoutGroup)m_LayoutGroup).startAxis == GridLayoutGroup.Axis.Horizontal)
                        Debug.LogError($"Scroll View is Horizontal,GridLayout StartAxis must be Axis.Vertical");
                }
            }
            m_viewItemCount = m_viewItemRowCount * m_viewItemColCount;
            m_scrollRect.onValueChanged.AddListener(ScorwRectMove);
            return m_viewItemCount;
        }

        /// <summary>
        /// 因为ContentSizeFitter的RectTransForm下一帧有用
        /// </summary>
        private void RefreshChildPosition()
        {
            int count = ChildItemCount;
            if (count <= 0)
                return;

            if (childList[0].rect.size == Vector2.zero)
            {
                EngineCoreEvents.SystemEvents.SendTaskExecuteNextFrame.SafeInvoke(RefreshChildPosition);
                return;
            }
            SetGridSize = childList[0].rect.size + m_itemSpacing;
            if (!haveSetStartPos)
            {
                haveSetStartPos = true;
                itemStartPos = childList[0].anchoredPosition;
            }
            if (IsVertical)
            {
                if (count > 1)
                {
                    itemAddPos.x = (childList[1].anchoredPosition.x - itemStartPos.x);
                }
                if (count > m_viewItemColCount)
                {
                    itemAddPos.y = (childList[m_viewItemColCount].anchoredPosition.y - itemStartPos.y);
                }
            }
            else
            {
                if (count > 1)
                {
                    itemAddPos.y = (childList[1].anchoredPosition.y - itemStartPos.y);
                }
                if (count > m_viewItemRowCount)
                {
                    itemAddPos.x = (childList[m_viewItemRowCount].anchoredPosition.x - itemStartPos.x);
                }
            }
            DataCount = m_dataCount;
        }

        private bool m_isinsureSize = false;
        public void EnsureSize(int realDataCount)
        {
            m_isinsureSize = true;

            InitM_ScrowRect();

            ScorwRectMove(Vector2.zero);

            int count = Mathf.Min(realDataCount, m_viewItemCount);
            int cur = ChildItemCount;

            if (cur < count)
            {
                int diff = count - cur;
                for (int i = 0; i < diff; i++)
                {
                    AddChild();
                }
            }
            //else
            //{
            //    for (int i = realDataCount; i < cur; i++)
            //        GetChildByIndex(i).gameObject.SetActive(false);
            //}

            /*int dataCountDiff = realDataCount - m_dataCount;
            for (int i = 0; i < Mathf.Abs(dataCountDiff); ++i)
            {
                if (dataCountDiff > 0)
                {
                    T newDataItem = this.m_dataPool.Get() as T;
                    newDataItem.ItemIndex = i;
                    this.m_currentDataItemList.Add(newDataItem);
                }
                else
                {
                    T recycleDataItem = this.m_currentDataItemList[realDataCount] as T;
                    this.m_currentDataItemList.RemoveAt(realDataCount);
                    this.m_dataPool.Release(recycleDataItem);
                }
            }*/
            this.m_currentDataItemList.Clear();
            for (int i = 0; i < realDataCount; ++i)
            {
                if (realDataCount > 0)
                {
                    T newDataItem = this.m_dataPool.Get() as T;
                    newDataItem.ItemIndex = i;
                    this.m_currentDataItemList.Add(newDataItem);
                }
            }

            //下一帧渲染  
            m_dataCount = realDataCount;
            EngineCoreEvents.SystemEvents.SendTaskExecuteNextFrame.SafeInvoke(RefreshChildPosition);
            m_isinsureSize = false;
        }

        public override void EnsureSize<U>(int count)
        {
            EnsureSize(count);
        }

        /// <summary>
        /// override parent method prevernt invoke error
        /// </summary>
        /// <typeparam name="U"></typeparam>
        /// <param name="idx"></param>
        /// <returns></returns>
        public override U GetChild<U>(int idx)
        {
            return GetChild(idx) as U;
        }

        public T GetChild(int idx)
        {
            if (idx >= ChildCount)
                Debug.LogError($"idx error : {idx}");

            return m_currentDataItemList[idx] as T;
        }


        public override int ChildCount => m_dataCount;

        /// <summary>
        /// Scroll View 是否是垂直方向的
        /// </summary>
        private bool IsVertical
        {
            get
            {
                return this.m_scrollRect != null ? this.m_scrollRect.vertical : false;
            }
        }
    }
}