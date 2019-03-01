/********************************************************************
	created:  2019-1-24 12:26:25
	filename: FrameVisibilityComponent.cs
	author:	  songguangze@outlook.com
	
	purpose:  窗口渲染组件
*********************************************************************/
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace EngineCore
{
    internal class FrameVisibilityComponent
    {
        private Canvas[] m_frameCanvases;
        private Renderer[] m_canvasRenderers;
        private GraphicRaycaster[] m_graphicRaycasters;
        private bool m_frameVisible = false;

        private GUIFrame m_frameInfo = null;

        //GUIFrame下所有Canvas深度信息
        private List<CanvasDepthInfo> m_frameAllCanvasDepthList = new List<CanvasDepthInfo>();
        private bool m_isDirty = false;

        public FrameVisibilityComponent(GUIFrame frame)
        {
            this.m_frameInfo = frame;

            BuildFrameVisibilityComponent();
        }

        private void BuildFrameVisibilityComponent()
        {
            if (m_frameInfo.FrameRoot == null)
                Debug.LogError($"{m_frameInfo.ResName} can't find root gameobject");

            if (m_frameCanvases != null)
                Debug.LogWarning($"{m_frameInfo.ResName} already initialized");

            m_frameCanvases = m_frameInfo.FrameRoot.GetComponentsInChildren<Canvas>(true);
            m_canvasRenderers = m_frameInfo.FrameRoot.GetComponentsInChildren<Renderer>(true);
            m_graphicRaycasters = m_frameInfo.FrameRoot.GetComponentsInChildren<GraphicRaycaster>(true);

            //init GUIFrame All Canvas Depth
            for (int i = 0; i < m_frameCanvases.Length; ++i)
            {
                CanvasDepthInfo canvasRawDepthInfo = new CanvasDepthInfo(m_frameCanvases[i]);
                this.m_frameAllCanvasDepthList.Add(canvasRawDepthInfo);
            }

            m_frameAllCanvasDepthList = m_frameAllCanvasDepthList.OrderByDescending(canvasDepth => canvasDepth.CurrentDepth).ToList();
            FrameDepth = this.m_frameInfo.UIRootCanvas.sortingOrder;

            this.m_isDirty = false;
        }

        /// <summary>
        /// 优化 GraphicRaycaster.Raycast 避免CPU周期浪费
        /// </summary>
        public void OptimizeFrameGraphic()
        {
            for (int i = 0; i < this.m_frameCanvases.Length; ++i)
            {
                Canvas canvasInFrame = this.m_frameCanvases[i];

                IList<Graphic> canvasGraphics = GraphicRegistry.GetGraphicsForCanvas(canvasInFrame);

                for (int j = 0; j < canvasGraphics.Count; ++j)
                {
                    Graphic graphic = canvasGraphics[j];
                    if (!graphic.raycastTarget)
                        GraphicRegistry.UnregisterGraphicForCanvas(canvasInFrame, graphic);
                }
            }
        }

        /// <summary>
        /// GUIFrame Visibility Render Dirty
        /// </summary>
        public void SetDirty()
        {
            this.m_isDirty = true;
        }

        /// <summary>
        /// 将当前窗口置顶
        /// </summary>
        public void SetFrameToTopside()
        {
            GUIFrame currentTopsideFrame = FrameMgr.Instance.GetTopsideFrame();
            if (currentTopsideFrame != this.m_frameInfo)
            {
                int diffDepth = currentTopsideFrame.FrameDepth + 1;

                for (int i = 0; i < m_frameAllCanvasDepthList.Count; ++i)
                {
                    CanvasDepthInfo canvasRawDepthInfo = this.m_frameAllCanvasDepthList[i];
                    canvasRawDepthInfo.SetDiffDepth(diffDepth);
                }

                FrameDepth += diffDepth;

                FrameMgr.Instance.SetFrameDepthChanged(this.m_frameInfo);
            }
        }

        /// <summary>
        /// 重置窗口深度
        /// </summary>
        public void ResetFrameRenderDepth()
        {
            if (!FrameIsRawDepth())
            {
                if (this.m_frameAllCanvasDepthList.Count > 0 && this.m_frameAllCanvasDepthList[0].AdjustDiffDepth != 0)
                {
                    int adjuestDepth = this.m_frameAllCanvasDepthList[0].AdjustDiffDepth;
                    for (int i = 0; i < this.m_frameAllCanvasDepthList.Count; ++i)
                    {
                        CanvasDepthInfo canvasDepthInfo = this.m_frameAllCanvasDepthList[i];
                        canvasDepthInfo.Reset();
                    }

                    FrameDepth -= adjuestDepth;

                    FrameMgr.Instance.SetFrameDepthChanged(this.m_frameInfo);
                }
            }
        }


        public bool Visible
        {
            get
            {
                return m_frameVisible;
            }
            set
            {
                m_frameVisible = value;
                if (m_isDirty)
                    BuildFrameVisibilityComponent();

                //只处理Active = true的
                for (int i = 0; i < Mathf.Max(m_frameCanvases.Length, m_graphicRaycasters.Length, m_canvasRenderers.Length); ++i)
                {
                    if (i < m_frameCanvases.Length)
                        m_frameCanvases[i].gameObject.layer = m_frameVisible ? LayerDefine.UIShowLayer : LayerDefine.UIHideLayer;

                    if (i < m_canvasRenderers.Length)
                        m_canvasRenderers[i].gameObject.layer = m_frameVisible ? LayerDefine.UIShowLayer : LayerDefine.UIHideLayer;

                    if (i < m_graphicRaycasters.Length)
                        m_graphicRaycasters[i].enabled = m_frameVisible;
                }
            }
        }

        /// <summary>
        /// 窗口是否是原始深度
        /// </summary>
        /// <returns></returns>
        public bool FrameIsRawDepth()
        {
            if (this.m_frameAllCanvasDepthList.Count > 0)
                return this.m_frameAllCanvasDepthList[0].AdjustDiffDepth == 0;

            return true;
        }

        /// <summary>
        /// GUIFrame 最高层的Canvas
        /// </summary>
        public int FrameTopsideCanvasDepth => this.m_frameAllCanvasDepthList[0].CurrentDepth;

        /// <summary>
        /// 窗口根节点深度
        /// </summary>
        public int FrameDepth
        {
            get; private set;
        }



        /// <summary>
        /// Canvas 深度信息
        /// </summary>
        private class CanvasDepthInfo
        {
            public Canvas TargetCanvas;
            public int CanvasRawDepth;
            public int AdjustDiffDepth;

            public CanvasDepthInfo(Canvas canvasInfo)
            {
                this.TargetCanvas = canvasInfo;
                this.CanvasRawDepth = canvasInfo.sortingOrder;
            }

            public void SetDiffDepth(int diffDepth)
            {
                this.AdjustDiffDepth = diffDepth;
                this.TargetCanvas.sortingOrder += diffDepth;
            }

            public void Reset()
            {
                this.TargetCanvas.sortingOrder = CanvasRawDepth;
                this.AdjustDiffDepth = 0;
            }

            //当前深度
            public int CurrentDepth => this.TargetCanvas.sortingOrder;
        }
    }
}
