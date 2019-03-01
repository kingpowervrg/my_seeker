using GOGUI;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace EngineCore
{
    public class GameScrollView : GameUIComponent
    {
        public ScrollRect scrollView;
        protected override void OnInit()
        {
            base.OnInit();
            scrollView = GetComponent<ScrollRect>();
        }

        public float VerticalValue
        {
            get { return scrollView.verticalNormalizedPosition; }
            set { scrollView.verticalNormalizedPosition = value; }
        }

        public float HorizontalValue
        {
            get { return scrollView.horizontalNormalizedPosition; }
            set { scrollView.horizontalNormalizedPosition = value; }
        }

        public void ScrollToTop()
        {
            ScrollTo(0, 1);
        }
        public void ScrollToBottom()
        {
            ScrollTo(1, 0);
        }

        public void ScrollTo(float x, float y)
        {
            scrollView.verticalNormalizedPosition = y;
            scrollView.horizontalNormalizedPosition = x;
        }

        #region 拖拽出边界刷新/分页

        enum Anchor { 
            NONE,

            Hor_LEFT,
            Hor_CENTER,
            Hor_RIGHT,

            Ver_TOP,
            Ver_CENTER,
            Ver_BOTTOM
        };

        enum RefreshDir
        {
            None,
            UP,
            DOWN
        };

        Anchor anchor = Anchor.NONE;
        RefreshDir refreshDir = RefreshDir.None;

        Action<float,int> RefreshAction;
        bool _isDragOutRefresh = false;

        public int TriggerOffset = 100;
        //Data
        RectTransform content;
        RectTransform scrollViewRect;
        float scrollViewLimit = 0.0f;
        float contentLimit = 0.0f;

        int PageIndex = -1;
        int PageCount = 0;
        bool isHorizontal = false;

        bool isTrigger = false;
        public bool IdDragOutRefresh
        {
            set {
                if (_isDragOutRefresh != value)
                {
                    _isDragOutRefresh = value;

                    if (value)
                    {
                        InitDragOutRefresh();
                    }
                    else { 
                    
                    }
                }
            }
            get
            {
                return _isDragOutRefresh;
            }
        }

        public void InitDragOutRefresh()
        {
         
            content = scrollView.content;

            DragEventTriggerListener dragListener = DragEventTriggerListener.Get(gameObject);
            dragListener.onDrag += OnDrag;
            dragListener.onDragStart += OnDragStart;
            dragListener.onDragEnd += OnDragEnd;

            isHorizontal = scrollView.horizontal;
            scrollViewRect = scrollView.gameObject.GetComponent<RectTransform>();
            if (isHorizontal)
            {
                scrollViewLimit = scrollViewRect.sizeDelta.x;

                float value = content.anchorMin.x;
                if (value == 0.0f)
                {
                    anchor = Anchor.Hor_LEFT;
                }
                else if (value == 0.5f)
                {
                    anchor = Anchor.Hor_CENTER;
                }
                else if (value == 1.0f)
                {
                    anchor = Anchor.Hor_RIGHT;
                }
                else
                {
                    Debug.LogError("InitDragOutRefresh Error");
                }
            }
            else
            {
                scrollViewLimit = scrollViewRect.sizeDelta.y;

                float value = content.anchorMin.y;
                if (value == 0.0f)
                {
                    anchor = Anchor.Ver_TOP;
                }
                else if (value == 0.5f)
                {
                    anchor = Anchor.Ver_CENTER;
                }
                else if (value == 1.0f)
                {
                    anchor = Anchor.Ver_BOTTOM;
                }
                else
                {
                    Debug.LogError("InitDragOutRefresh Error");
                }
            }
        }

        public void OnDrag(GameObject go, Vector2 delta, Vector2 pos)
        {
            switch (anchor)
            {
                case Anchor.Ver_TOP:
                    {
                        float contentHieght = content.sizeDelta.y;
                        float y = content.anchoredPosition3D.y;

                        if (y < 0)
                        {
                            if (Math.Abs(y) > 100)
                            {
                                refreshDir = RefreshDir.UP;
                                isTrigger = true;
                                return;
                            }
                            else {
                                refreshDir = RefreshDir.None;
                                isTrigger = false;
                            }
                        }

                        if(contentHieght < scrollViewLimit)
                        {
                            return ;
                        }
                        float offset = y + scrollViewLimit - contentHieght;
                        if (offset > TriggerOffset)
                        {
                            refreshDir = RefreshDir.DOWN;
                            isTrigger = true;
                        }
                        else {
                            refreshDir = RefreshDir.None;
                            isTrigger = false;
                        }
                    }
                    break;
                case Anchor.Ver_CENTER:
                    {
                        float contentHieght = content.sizeDelta.y;
                        float y = content.anchoredPosition3D.y;
                        y -= scrollViewLimit / 2;
                        if (y < 0)
                        {
                            if (Math.Abs(y) > 100)
                            {
                                refreshDir = RefreshDir.UP;
                                isTrigger = true;
                                return;
                            }
                            else
                            {
                                refreshDir = RefreshDir.None;
                                isTrigger = false;
                            }
                        }

                        if (contentHieght < scrollViewLimit)
                        {
                            return;
                        }
                        float offset = y + scrollViewLimit - contentHieght;
                        if (offset > TriggerOffset)
                        {
                            refreshDir = RefreshDir.DOWN;
                            isTrigger = true;
                        }
                        else
                        {
                            refreshDir = RefreshDir.None;
                            isTrigger = false;
                        }
                    }
                    break;
                case Anchor.Ver_BOTTOM:
                    {
                        float contentHieght = content.sizeDelta.y;
                        float y = content.anchoredPosition3D.y;

                        if (y < 0)
                        {
                            if (Math.Abs(y) > 100)
                            {
                                refreshDir = RefreshDir.UP;
                                isTrigger = true;
                                return;
                            }
                            else
                            {
                                refreshDir = RefreshDir.None;
                                isTrigger = false;
                            }
                        }

                        if (contentHieght < scrollViewLimit)
                        {
                            return;
                        }
                        float offset = y + scrollViewLimit - contentHieght;
                        if (offset > TriggerOffset)
                        {
                            refreshDir = RefreshDir.DOWN;
                            isTrigger = true;
                        }
                        else
                        {
                            refreshDir = RefreshDir.None;
                            isTrigger = false;
                        }
                    }
                    break;
                default:
                    Debug.LogError("OnDragStart.error");
                    break;
            }
        }

        public void OnDragStart(GameObject go, Vector2 delta)
        {

        }

        public void OnDragEnd(GameObject go, Vector2 delta)
        {
            if (isTrigger)
            {
                if (refreshDir == RefreshDir.DOWN)
                {
                    PageIndex += 1;

                    RefreshAction.SafeInvoke((float)(PageIndex) * PageCount, (int)refreshDir);
                }
                else
                {
                    if (PageIndex > 0)
                    {
                        PageIndex -= 1;
                        RefreshAction.SafeInvoke((float)(PageIndex) * PageCount,(int)refreshDir);
                    }
                }
            }
            isTrigger = false;
        }

        /// <summary>
        ///一页刷新多少数据 方便逻辑层计数
        /// </summary>
        /// <param name="pageIndex"> 起始页 </param>
        /// /// <param name="pageCount"></param>
        public void SetRefreshInfo(int pageIndex , int pageCount)
        {
            PageIndex = pageIndex;
            PageCount = pageCount;
            IdDragOutRefresh = true;
        }

        public void AddRefreshCallBack(Action<float,int> func)
        {
            if(IdDragOutRefresh == false)
            {
                Debug.LogError("First Call ScrollView.SetRefreshInfo");
            }
            RefreshAction += func;
        }
        #endregion

    }
}