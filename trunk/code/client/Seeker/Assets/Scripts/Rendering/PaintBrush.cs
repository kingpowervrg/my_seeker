/********************************************************************
	created:  2018-4-23 10:38:35
	filename: PaintBrush.cs
	author:	  songguangze@outlook.com
	
	purpose:  画笔基类
*********************************************************************/

using EngineCore;
using UnityEngine;

namespace SeekerGame.Rendering
{
    public abstract class PaintBrush
    {
        protected BrushStatus m_brushStatus = BrushStatus.WAITING;
        protected GameObject m_brushObject = null;
        protected string m_brushName = string.Empty;
        protected float m_fadeFactor = 1f;

        protected Material m_brushMaterial = null;

        private bool m_isActive = false;
        private float m_brushSize = 0.8f;
        private int m_brushSortOrder = 0;

        protected float m_pauseTime;
        protected float m_resumeTime;
        protected bool IsPause = false;

        public PaintBrush(BrushType brushType)
        {
            this.PaintBrushType = brushType;
        }

        public abstract void InitBrush(Vector3 brushPosition);

        public virtual void PauseBrush()
        {
            this.m_pauseTime = Time.time;
            this.IsPause = true;
        }

        public virtual void ResumeBrush()
        {
            float pauseDuration = Time.time - this.m_pauseTime;

            BrushStartTime += pauseDuration;
            BrushFadeStartTime += pauseDuration;
            BrushFadeEndTime += pauseDuration;

            this.IsPause = false;
        }


        public virtual void Tick()
        {

        }

        /// <summary>
        /// 强制停止
        /// </summary>
        public abstract void ForceEnd();

        /// <summary>
        /// 强制开始
        /// </summary>
        /// <param name="beginFadeTime"></param>
        public virtual void ForceBeginFade(float beginFadeTime)
        {

        }


        public GameObject BrushGameObject => this.m_brushObject;

        public Transform BrushTransform => this.m_brushObject.transform;



        public BrushStatus BrushPaintStatus
        {
            get { return this.m_brushStatus; }
            set { this.m_brushStatus = value; }
        }


        public bool IsActive
        {
            get
            {
                return this.m_isActive;
            }
            set
            {
                this.m_isActive = value;
                this.m_brushObject.SetActive(value);
            }
        }

        /// <summary>
        /// 画笔可以使用的时间
        /// </summary>
        public float BrushTime
        {
            get; set;
        }

        /// <summary>
        /// 画笔开始使用时间
        /// </summary>
        public float BrushStartTime
        {
            get; set;
        }

        /// <summary>
        /// 画笔Fade时间
        /// </summary>
        public float BrushFadeTime
        {
            get; set;
        }

        /// <summary>
        /// 画笔开始过渡时间
        /// </summary>
        public float BrushFadeStartTime
        {
            get; set;
        }


        /// <summary>
        /// 画笔结束过渡时间
        /// </summary>
        public float BrushFadeEndTime
        {
            get; set;
        }

        public virtual float BrushSize
        {
            get
            {
                return this.m_brushSize;
            }
            set
            {
                this.m_brushSize = value;
            }
        }

        public virtual int BrushSortOrder
        {
            get { return this.m_brushSortOrder; }
            set { this.m_brushSortOrder = value; }
        }

        /// <summary>
        /// 画笔类型
        /// </summary>
        public BrushType PaintBrushType
        {
            get;
            private set;
        }


        /// <summary>
        /// 画笔状态
        /// </summary>
        public enum BrushStatus
        {
            WAITING,            //等待使用
            PAINTING,           //绘制中
            FADING,             //Fade
            FADEEND,            //Fade结束
        }

        /// <summary>
        /// 画笔类型
        /// </summary>
        public enum BrushType
        {
            TRAIL_RENDERER_BRUSH,
            ONETIME_BRUSH
        }
    }
}