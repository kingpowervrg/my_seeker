/********************************************************************
	created:  2019-1-23 20:38:10
	filename: GUIFrame.cs
	author:	  songguangze@outlook.com
	
	purpose:  UI窗口对象
*********************************************************************/
using System;
using UnityEngine;

namespace EngineCore
{
    public class GUIFrame
    {
        #region Public Frame Events
        public Action<GUIFrame> OnFrameLoaded;                  //窗口加载后事件
        public Action<GUIFrame> OnAfterShowFrameEvent;          //窗口显示后事件

        public Action<GUIFrame> OnBeforeHideFrameEvent;         //窗口隐藏时事件
        public Action<GUIFrame> OnPostHideFrameEvent;           //窗口隐藏后事件

        public Action<GUIFrame> OnBeforeFrameDestroyEvent;      //窗口销毁前事件
        public Action<GUIFrame> OnAfterFrameDestroyEvent;       //窗口销毁后事件  
        #endregion

        public ResStatus ResStatus;

        //UI节点下所有默认的Tweener
        private UITweenerBase[] uiTweens;

        //Show窗口时传入的参数
        private FrameMgr.OpenUIParams m_openFrameParam = null;

        //Hide窗口参数
        private FrameMgr.HideUIParams m_hideFrameParam = null;

        private readonly string m_frameResName;
        private bool m_frameVisible = false;

        //FrameRender Component
        private FrameVisibilityComponent m_frameRenderer = null;

        public GUIFrame(FrameMgr.OpenUIParams openFrameParam)
        {
            m_frameResName = openFrameParam.UIName;
            this.m_openFrameParam = openFrameParam;
            this.m_hideFrameParam = new FrameMgr.HideUIParams(openFrameParam.UIName);
        }

        public void Load()
        {
            this.ResStatus = ResStatus.WAIT;
            EngineCoreEvents.ResourceEvent.GetAssetEvent.SafeInvoke(this.ResName, OnLoad, LoadPriority.HighPrior);
        }

        private void OnLoad(string name, UnityEngine.Object obj)
        {
            if (obj == null)
                Debug.LogError($"can't load UIObject :{name}");

            ResStatus = ResStatus.OK;
            FrameRoot = obj as GameObject;

            FrameRootTransform = FrameRoot.GetComponent<RectTransform>();

            this.OnLoadRes();
        }


        protected void OnLoadRes()
        {
            UIRootCanvas.worldCamera = FrameMgr.Instance.UICamera;

            GameObjectUtil.SetLayer(FrameRoot, LayerDefine.UIHideLayer);

            FrameRoot.name = ResName.Replace(".prefab", "");

            FrameRoot.SetActive(true);

            m_frameRenderer = new FrameVisibilityComponent(this);

            uiTweens = FrameRoot.GetComponentsInChildren<UITweenerBase>(false);

            OnFrameLoaded?.Invoke(this);

            //干掉没有用的Graphic
            m_frameRenderer.OptimizeFrameGraphic();
        }

        public void ShowFrame()
        {
            TimeModule.Instance.RemoveTimeaction(OnPostHideFrame);
            TimeModule.Instance.RemoveTimeaction(DestroyFrame);

            if (!Visible)
            {
                this.Visible = true;
                float duration = 0;
                if (OpenFrameParam.OpenUIWithAnimation)
                {
                    foreach (UITweenerBase ut in uiTweens)
                    {
                        if (ut.gameObject.activeInHierarchy && ut.m_triggerType == UITweenerBase.TweenTriggerType.OnShow)
                        {
                            duration = Mathf.Max(duration, ut.Duration + ut.Delay);
                            ut.ResetAndPlay();
                        }
                    }
                }

                if (LogicHandler != null)
                    LogicHandler.OnShow(OpenFrameParam.Param, false);

                TimeModule.Instance.SetTimeout(OnAfterShowFrameFinish, duration, false, false);

                if (OpenFrameParam.IsBlur)
                    EngineCoreEvents.UIEvent.BlurUIBackground(true);
            }
        }

        public void HideFrame()
        {
            if (LogicHandler != null)
            {
                TimeModule.Instance.RemoveTimeaction(OnAfterShowFrameFinish);
                LogicHandler.OnHide();
            }

            if (Visible)
            {
                OnBeforeFrameDestroyEvent?.Invoke(this);

                float duration = 0;

                if (m_hideFrameParam.HideFrameWithAnimation)
                {
                    foreach (UITweenerBase ut in uiTweens)
                    {
                        if (ut.gameObject.activeInHierarchy && ut.m_triggerType == UITweenerBase.TweenTriggerType.OnHide)
                        {
                            duration = Mathf.Max(duration, ut.Duration + ut.Delay);
                            ut.ResetAndPlay();
                        }
                    }
                }
                TimeModule.Instance.SetTimeout(OnPostHideFrame, duration, false, false);

                //窗口在这里被没真正的隐藏，因为有Tween 没执行，遵循开闭原则，对外暴露已经关闭，对内等到真正Tween之后才进行关闭渲染
                this.m_frameVisible = false;
            }
        }

        internal void SetFrameForDestroying()
        {
            if (LogicHandler.AutoDestroy)
            {
                TimeModule.Instance.RemoveTimeaction(DestroyFrame);
                TimeModule.Instance.SetTimeout(DestroyFrame, m_hideFrameParam.DestoryFrameDelayTime);
            }
        }

        /// <summary>
        /// Set GUIFrame Render Dirty
        /// </summary>
        public void SetFrameDirty()
        {
            this.m_frameRenderer.SetDirty();
        }

        /// <summary>
        /// 窗口彻底显示后
        /// </summary>
        private void OnAfterShowFrameFinish()
        {
            LogicHandler.OnShowUIFinish();

            OnAfterShowFrameEvent?.Invoke(this);
        }

        /// <summary>
        /// 窗口彻底关闭后
        /// </summary>
        private void OnPostHideFrame()
        {
            this.Visible = false;

            OnPostHideFrameEvent?.Invoke(this);
        }

        /// <summary>
        /// 设置窗口隐藏参数
        /// </summary>
        /// <param name="hideFrameParam"></param>
        public void SetHideFrameParam(FrameMgr.HideUIParams hideFrameParam)
        {
            if (hideFrameParam.UIName != ResName)
                Debug.LogError($"hide frame param error,frame real name: {this.ResName}, passed hide frame {hideFrameParam.UIName}");

            this.m_hideFrameParam = hideFrameParam;
        }


        /// <summary>
        /// 打开窗口参数
        /// </summary>
        public FrameMgr.OpenUIParams OpenFrameParam
        {
            get
            {
                return this.m_openFrameParam;
            }
            set
            {
                if (value.UIName != this.ResName)
                    Debug.LogError($"open frame param error,frame real name: {this.ResName}, passed resname {value.UIName}");

                this.m_openFrameParam = value;
            }
        }


        public Canvas UIRootCanvas
        {
            get
            {
                if (FrameRoot == null)
                    return null;
                return FrameRoot.GetComponent<Canvas>();
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
                //记录窗口的逻辑可见性
                m_frameVisible = value;

                if (m_frameRenderer != null)
                    m_frameRenderer.Visible = value;
                else
                {
                    Debug.LogWarning($"{ResName} not loaded");
                }
            }
        }


        internal void DestroyFrame()
        {
            OnBeforeFrameDestroyEvent?.Invoke(this);

            OnDestroy();

            OnAfterFrameDestroyEvent?.Invoke(this);

            LogicHandler = null;
            FrameRoot = null;
        }

        /// <summary>
        /// 窗口销毁
        /// </summary>
        private void OnDestroy()
        {
            if (LogicHandler != null)
            {
                LogicHandler.Dispose();
            }

            if (FrameRoot)
            {
                EngineCoreEvents.ResourceEvent.ReleaseAssetEvent.SafeInvoke(ResName, FrameRoot);
                GameObject.Destroy(FrameRoot);
            }
        }

        /// <summary>
        /// 窗口原始深度
        /// </summary>
        public int FrameDepth => this.m_frameRenderer.FrameDepth;

        /// <summary>
        /// 窗口最高层深度
        /// </summary>
        public int FrameTopsideDepth => this.m_frameRenderer.FrameTopsideCanvasDepth;

        /// <summary>
        /// GUIFrame根节点
        /// </summary>
        public GameObject FrameRoot { get; private set; }

        /// <summary>
        /// GUIFrame根节点Transform
        /// </summary>
        public RectTransform FrameRootTransform { get; private set; }

        /// <summary>
        /// 窗口加载状态
        /// </summary>
        public bool IsResOK => ResStatus == ResStatus.OK;

        /// <summary>
        /// 窗口名称
        /// </summary>
        public string ResName => m_frameResName;

        /// <summary>
        /// 窗口逻辑对象
        /// </summary>
        public UILogicBase LogicHandler { get; set; }

        /// <summary>
        /// 窗口显示形式
        /// </summary>
        public FrameDisplayMode FrameDisplayMode => LogicHandler != null ? LogicHandler.UIFrameDisplayMode : FrameDisplayMode.NONE;

        /// <summary>
        /// 窗口隐藏时的参数
        /// </summary>
        public FrameMgr.HideUIParams HideFrameParam => this.m_hideFrameParam;
    }

    /// <summary>
    /// 窗口显示的形式
    /// </summary>
    public enum FrameDisplayMode
    {
        NONE,       //未设置
        WINDOWED,   //窗口化
        FULLSCREEN, //全屏
        POPUP,      //弹出式
    }
}