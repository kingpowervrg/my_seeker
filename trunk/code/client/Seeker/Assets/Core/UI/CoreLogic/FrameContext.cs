/********************************************************************
	created:  2019-1-16 18:40:12
	filename: FrameContext.cs
	author:	  songguangze@outlook.com
	
	purpose:  UI窗口上下文
           1. 记录窗口流转信息
           2. 配合FrameMgr 尽量保证只有一个全屏窗口在渲染
*********************************************************************/
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace EngineCore
{
    sealed class FrameContext
    {
        //窗口链
        private List<FrameContextElement> m_frameChain = new List<FrameContextElement>();

        //当前Focus窗口
        private FrameContextElement m_currentFrameContextElement = null;

        //窗口列表
        private Dictionary<string, GUIFrame> m_frameDict = new Dictionary<string, GUIFrame>();

        //没有上下文的窗口
        private HashSet<GUIFrame> m_exceptFrame = new HashSet<GUIFrame>();


        public void AddFrameToContext(GUIFrame targetFrame, GUIFrame openByFrame)
        {
            FrameContextElement frameContextElement = null;
            //无上下文
            if (openByFrame == null)
            {
                m_exceptFrame.Add(targetFrame);
            }
            else
            {
                int alreadyChainIndex = GetFrameIndexOfChain(targetFrame);

                //Destroy already context info
                if (alreadyChainIndex != -1)
                {
                    frameContextElement = this.m_frameChain[alreadyChainIndex];

                    //stack bottom
                    //销毁整条链
                    if (alreadyChainIndex == 0)
                        ClearFrameContext();
                    else
                    {
                        if (this.m_frameChain.Count - 1 < alreadyChainIndex)
                            Debug.LogError($"frame index error already index{alreadyChainIndex} , chain count {this.m_frameChain.Count}");

                        FrameContextElement originNextFrameContextElement = this.m_frameChain[alreadyChainIndex + 1];
                        originNextFrameContextElement.OpenByFrame = frameContextElement.OpenByFrame;

                        this.m_frameChain.RemoveAt(alreadyChainIndex);
                    }
                }


                if (frameContextElement == null)
                {
                    frameContextElement = new FrameContextElement(openByFrame);
                    frameContextElement.NextFrame = targetFrame;
                }

                this.m_frameChain.Add(frameContextElement);
            }
        }

        public void AddFrameToContext(GUIFrame targetFrame, string openByFrameName)
        {
            GUIFrame openByFrame = null;
            if (!string.IsNullOrEmpty(openByFrameName))
                openByFrame = FrameMgr.Instance.GetFrame(openByFrameName);

            AddFrameToContext(targetFrame, openByFrame);
        }

        public GUIFrame PopFrame()
        {
            GUIFrame frame = null;
            FrameContextElement frameContextElement = this.m_frameChain.LastOrDefault();
            if (frameContextElement != null)
            {
                frame = frameContextElement.TargetFrame;
                this.m_frameChain.Remove(frameContextElement);
            }

            return frame;
        }

        public void RetrieveFrame()
        {

        }

        public void ClearFrameContext()
        {
            this.m_frameChain.Clear();
        }

        /// <summary>
        /// 是否在窗口链中
        /// </summary>
        /// <param name="frame"></param>
        /// <returns></returns>
        private int GetFrameIndexOfChain(GUIFrame frame)
        {
            return this.m_frameChain.FindIndex(item => item.FrameName == frame.ResName);
        }

        /// <summary>
        /// 获取指定窗口的上下文
        /// </summary>
        /// <param name="frame"></param>
        /// <returns></returns>
        private FrameContextElement GetFrameContextByFrame(GUIFrame frame)
        {
            int frameIndexInChain = GetFrameIndexOfChain(frame);
            return frameIndexInChain == -1 ? null : this.m_frameChain[frameIndexInChain];
        }



        /// <summary>
        /// 窗口上下文子项
        /// </summary>
        class FrameContextElement
        {
            public FrameContextElement(GUIFrame targetFrame)
            {
                this.FrameName = targetFrame.ResName;
                this.TargetFrame = targetFrame;
                this.FrameOpenParam = targetFrame.OpenFrameParam;
            }

            public string FrameName
            {
                get;
                private set;
            }

            public GUIFrame TargetFrame
            {
                get;
                private set;
            }

            public System.Object FrameOpenParam
            {
                get;
                private set;
            }

            public GUIFrame OpenByFrame
            {
                get;
                set;
            }

            /// <summary>
            /// 本窗口打开的下一个窗口
            /// </summary>
            public GUIFrame NextFrame { get; set; }
        }
    }
}