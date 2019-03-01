/********************************************************************
	created:  2018-4-19 11:24:52
	filename: ScreenDrawer.cs
	author:	  songguangze@outlook.com
	
	purpose:  屏幕画板
*********************************************************************/
using EngineCore;
using HedgehogTeam.EasyTouch;
using System.Collections.Generic;
using UnityEngine;

namespace SeekerGame.Rendering
{
    public class ScreenDrawer : MonoSingleton<ScreenDrawer>
    {
        public float SWIPE_THRESHOLD = 0.1f;            //是否处于画画状态阈值

        public PaintBrushMode paintBrushMode = PaintBrushMode.MULTIPLE;

        //画笔内存池队列
        private Dictionary<PaintBrushMode, MemoryPool<PaintBrush>> m_brushPoolDict = new Dictionary<PaintBrushMode, MemoryPool<PaintBrush>>();

        //当前激活的画笔
        private List<PaintBrush> m_currentActiveBrushList = new List<PaintBrush>();

        private Camera m_brushCamera = null;

        //当前使用的画笔
        private PaintBrush m_currentBrush = null;

        public RenderTexture m_drawingTexture = null;

        private static int SortOrderAllocer = 0;

        private float BrushWorldZ;

        private void Awake()
        {
            InitBrushPool();
        }


        private void OnEnable()
        {
            InitBrushCamera();

            m_drawingTexture = RenderTexture.GetTemporary(this.m_brushCamera.pixelWidth, this.m_brushCamera.pixelHeight, 0);
            this.m_brushCamera.targetTexture = m_drawingTexture;
            this.m_brushCamera.enabled = true;
            Graphics.SetRenderTarget(this.m_drawingTexture);

            EngineCoreEvents.InputEvent.OnSwipe += OnDrawing;
            EngineCoreEvents.ResourceEvent.LeaveScene += OnLeaveScene;
            EngineCoreEvents.InputEvent.OnSwipeEnd += OnDrawingEnd;
            if (this.paintBrushMode == PaintBrushMode.SINGLE)
            {
                this.BrushTime = float.MaxValue;
                this.BrushFadeoutTime = GameConst.DARK_SCENE_BRUSH_FADE_TIME;
                //初始化就出现手电筒
                Gesture gesture = new Gesture();
                gesture.position = Vector2.right * Screen.width / 2f + Vector2.up * Screen.height / 2f;
                gesture.deltaPosition = Vector2.one;
                OnDrawing(gesture);
            }
            else
            {
                this.BrushTime = GameConst.TRAIL_BRUSH_TIME;
                this.BrushFadeoutTime = GameConst.BRUSH_FADEOUT_TIME;

            }

        }

        /// <summary>
        /// 初始化画笔相机
        /// </summary>
        private void InitBrushCamera()
        {
            if (this.m_brushCamera == null)
            {
                this.m_brushCamera = gameObject.GetOrAddComponent<Camera>();
                this.m_brushCamera.cullingMask = 1 << LayerDefine.SceneDrawingBoardLayer;
                this.m_brushCamera.depth = -5;
                this.m_brushCamera.orthographic = true;
                this.m_brushCamera.nearClipPlane = 1;
                this.m_brushCamera.farClipPlane = 10;
                this.m_brushCamera.allowHDR = false;
                this.m_brushCamera.clearFlags = CameraClearFlags.SolidColor;
                this.m_brushCamera.backgroundColor = Color.black;

                BrushWorldZ = gameObject.transform.position.z + 3;
                EasyTouch.AddCamera(this.m_brushCamera);
            }
        }

        public void PauseAllBrush()
        {
            for (int i = this.m_currentActiveBrushList.Count - 1; i >= 0; --i)
            {
                PaintBrush activeBrush = this.m_currentActiveBrushList[i];
                activeBrush.PauseBrush();
            }
        }

        public void ResumeAllBrush()
        {
            for (int i = this.m_currentActiveBrushList.Count - 1; i >= 0; --i)
            {
                PaintBrush activeBrush = this.m_currentActiveBrushList[i];
                activeBrush.ResumeBrush();
            }
        }

        private void Update()
        {
            for (int i = this.m_currentActiveBrushList.Count - 1; i >= 0; --i)
            {
                PaintBrush activeBrush = this.m_currentActiveBrushList[i];
                switch (activeBrush.BrushPaintStatus)
                {
                    case PaintBrush.BrushStatus.FADING:
                    case PaintBrush.BrushStatus.PAINTING:
                        activeBrush.Tick();
                        break;
                    case PaintBrush.BrushStatus.FADEEND:
                        RecycleBrush(activeBrush);
                        this.m_currentActiveBrushList.Remove(activeBrush);
                        break;
                }
            }
        }



        private void OnDrawing(Gesture gesture)
        {
            if (SceneModule.Instance.CurrentScene != null && SceneModule.Instance.CurrentScene is GameSceneBase)
            {
                if (((GameSceneBase)SceneModule.Instance.CurrentScene).CurGameStatus != SceneBase.GameStatus.GAMING)
                {
                    return;
                }
            }
            if (gesture.deltaPosition.sqrMagnitude < SWIPE_THRESHOLD)
                return;

            Vector3 brushWorldPos = this.m_brushCamera.ScreenToWorldPoint(new Vector3(gesture.position.x, gesture.position.y));
            brushWorldPos.z = BrushWorldZ;

            if (this.m_currentBrush == null || Time.time - this.m_currentBrush.BrushStartTime > BrushTime)
                this.m_currentBrush = ChangeBrush(brushWorldPos);

            this.m_currentBrush.BrushTransform.position = brushWorldPos;
            this.m_currentBrush.IsActive = true;
        }

        private void OnDrawingEnd(Gesture gesture)
        {
            if (this.m_currentBrush != null)
            {
                this.m_currentBrush.ForceBeginFade(Time.time);
                this.m_currentBrush = null;
            }
        }


        /// <summary>
        /// 更换画笔
        /// </summary>
        /// <returns></returns>
        private PaintBrush ChangeBrush(Vector3 position)
        {
            PaintBrush brush = this.m_brushPoolDict[paintBrushMode].Alloc();

            if (brush == null)
            {
                if (this.paintBrushMode == PaintBrushMode.MULTIPLE)
                {
                    brush = new TrailPaintBrush();
                    brush.BrushSize = GameConst.TRAIL_BRUSH_SIZE;
                }
                else
                {
                    //todo:有时间重构
                    for (int i = 0; i < this.m_currentActiveBrushList.Count; ++i)
                    {
                        if (this.m_currentActiveBrushList[i].PaintBrushType == PaintBrush.BrushType.ONETIME_BRUSH)
                        {
                            brush = this.m_currentActiveBrushList[i];
                            this.m_currentActiveBrushList.Remove(brush);
                            break;
                        }
                    }

                    if (brush == null)
                    {
                        brush = new OneTimeBrush();
                        brush.BrushSize = GameConst.DARK_SCENE_BRUSH_SIZE;
                    }
                }

                brush.BrushFadeTime = BrushFadeoutTime;
                brush.BrushTime = BrushTime;

                brush.InitBrush(position);
            }


            this.m_currentBrush = brush;
            this.m_currentBrush.BrushSortOrder = ++SortOrderAllocer;
            this.m_currentBrush.BrushStartTime = Time.time;
            this.m_currentBrush.BrushPaintStatus = PaintBrush.BrushStatus.PAINTING;

            this.m_currentActiveBrushList.Add(this.m_currentBrush);

            return brush;
        }

        /// <summary>
        /// 回收画笔
        /// </summary>
        /// <param name="paintBrush"></param>
        private void RecycleBrush(PaintBrush paintBrush)
        {
            this.m_brushPoolDict[paintBrushMode].Free(paintBrush);

            SortOrderAllocer--;

            for (int i = 0; i < this.m_currentActiveBrushList.Count; ++i)
                this.m_currentActiveBrushList[i].BrushSortOrder--;
        }

        private void InitBrushPool()
        {
            this.m_brushPoolDict.Add(PaintBrushMode.MULTIPLE, new MemoryPool<PaintBrush>(20));
            this.m_brushPoolDict.Add(PaintBrushMode.SINGLE, new MemoryPool<PaintBrush>(1));
        }

        private void OnLeaveScene()
        {
            this.enabled = false;
        }

        public RenderTexture DrawingTexture
        {
            get { return this.m_drawingTexture; }
        }


        private void OnDisable()
        {
            this.m_brushCamera.targetTexture = null;
            this.m_brushCamera.enabled = false;
            RenderTexture.ReleaseTemporary(this.m_drawingTexture);

            EngineCoreEvents.ResourceEvent.LeaveScene -= OnLeaveScene;
            EngineCoreEvents.InputEvent.OnSwipe -= OnDrawing;
        }

        /// <summary>
        /// 画笔整个生命时间
        /// </summary>
        public float BrushTime
        {
            get; set;
        }

        /// <summary>
        /// 画笔过渡时间
        /// </summary>
        public float BrushFadeoutTime
        {
            get; set;
        }

        public Camera ScreenDrawingBoardCamera
        {
            get { return this.m_brushCamera; }
        }


        /// <summary>
        /// 绘画模式
        /// </summary>
        public enum PaintBrushMode
        {
            SINGLE,     //单画笔
            MULTIPLE    //多段模式
        }
    }
}