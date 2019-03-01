/********************************************************************
	created:  2018-4-19 11:24:52
	filename: PaintBrush.cs
	author:	  songguangze@outlook.com
	
	purpose:  基于TrailRender 实现的画笔
*********************************************************************/
using EngineCore;
using UnityEngine;

namespace SeekerGame.Rendering
{
    public class TrailPaintBrush : PaintBrush
    {
        private TrailRenderer m_paintBrushRender = null;
        private const float DEFAULT_BRUSH_SIZE = 0.8f;

        public TrailPaintBrush() : base(BrushType.TRAIL_RENDERER_BRUSH)
        {

        }

        public override void InitBrush(Vector3 brushPosition)
        {
            this.m_brushName = "TrailBrush";
            this.m_brushObject = new GameObject(m_brushName);
            this.m_brushObject.transform.position = brushPosition;
            GameObjectUtil.SetLayer(m_brushObject, LayerDefine.SceneDrawingBoardLayer, true);

            InitBrushRenderer();
        }


        /// <summary>
        /// 初始化画笔
        /// </summary>
        protected void InitBrushRenderer()
        {
            if (this.m_paintBrushRender == null)
            {
                this.m_paintBrushRender = m_brushObject.AddComponent<TrailRenderer>();
                this.m_paintBrushRender.startWidth = BrushSize;
                this.m_paintBrushRender.endWidth = BrushSize;
                this.m_paintBrushRender.receiveShadows = false;
                this.m_paintBrushRender.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
                this.m_paintBrushRender.allowOcclusionWhenDynamic = false;
                this.m_paintBrushRender.time = 50;          //时间长一点，保证Mesh不会被干掉
                this.m_paintBrushRender.numCapVertices = 4; //头部更圆滑一些
                this.m_paintBrushRender.minVertexDistance = 0.2f;

                this.m_brushMaterial = new Material(ShaderModule.Instance.GetShader("SeekGame/PainterBrush"));
                this.m_brushMaterial.SetFloat("_FadeoutFactor", 1);
                this.m_paintBrushRender.material = this.m_brushMaterial;
            }
        }


        public override void Tick()
        {
            if (!IsPause)
            {
                if (this.BrushPaintStatus == BrushStatus.PAINTING)
                {
                    if (this.BrushStartTime + this.BrushTime > Time.time)
                    {
                        this.BrushFadeStartTime = Time.time;
                        this.BrushFadeEndTime = this.BrushFadeStartTime + BrushFadeTime;
                        this.BrushPaintStatus = BrushStatus.FADING;
                    }
                }
                else if (this.BrushPaintStatus == BrushStatus.FADING)
                {
                    this.m_fadeFactor = MathUtil.Remap01(Time.time, this.BrushFadeStartTime, this.BrushFadeEndTime);
                    if (this.m_fadeFactor < 0 || this.m_fadeFactor > 1)
                        EndFade();
                    else
                        this.m_brushMaterial.SetFloat("_FadeoutFactor", 1 - this.m_fadeFactor);
                }
            }
        }


        public void EndFade()
        {
            this.BrushPaintStatus = BrushStatus.FADEEND;
            IsActive = false;
            this.m_paintBrushRender.Clear();
            this.m_brushMaterial.SetFloat("_FadeoutFactor", 1);
        }


        /// <summary>
        /// 画笔强制结束
        /// </summary>
        public override void ForceEnd()
        {
            EndFade();
        }


        public override int BrushSortOrder
        {
            get { return base.BrushSortOrder; }
            set
            {
                base.BrushSortOrder = value;
                this.m_paintBrushRender.sortingOrder = value;
            }
        }

        public override float BrushSize
        {
            get
            {
                return base.BrushSize;
            }

            set
            {
                base.BrushSize = value;
                if (this.m_paintBrushRender != null)
                {
                    this.m_paintBrushRender.startWidth = value;
                    this.m_paintBrushRender.endWidth = value;
                }
            }
        }


    }
}