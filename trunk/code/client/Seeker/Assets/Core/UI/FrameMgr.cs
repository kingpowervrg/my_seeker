using EngineCore.Utility;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

namespace EngineCore
{
    /// <summary>
    /// UI窗口管理器
    /// </summary>
    [EngineCoreModule(EngineCore.ModuleType.UI_MODULE)]
    public class FrameMgr : AbstractModule
    {
        private static FrameMgr m_instance = null;

        //UI根节点
        private GameObject m_uiRootObject = null;

        Dictionary<string, GUIFrame> m_frameDict = new Dictionary<string, GUIFrame>();

        HashSet<string> uiNeedsGC = new HashSet<string>();

        //可见的窗口
        private SortedSet<GUIFrame> m_visibleFrameSet = new SortedSet<GUIFrame>(new FrameDepthComparer());
        private List<GUIFrame> m_updatingFrameList = new List<GUIFrame>();

        //全屏UI
        private HashSet<string> m_showingfullScreenUISet = new HashSet<string>();

        public HashSet<string> UINeedsGC { get { return uiNeedsGC; } }

        /// <summary>
        /// 屏宽比
        /// </summary>
        public float ScreenAspectRatio { get; set; }

        private float m_lastFreeResTime = 0f;

        //窗口流转上下文
        private FrameContext m_frameContext = new FrameContext();

        //预加载的UI
        private HashSet<string> m_preloadUISet = new HashSet<string>();

        public FrameMgr()
        {
            AutoStart = true;
            m_instance = this;
        }

        public override void Start()
        {
            base.Start();

            InitFrameModule();

            EngineCoreEvents.UIEvent.ShowUIEvent += SimpleShowUI;
            EngineCoreEvents.UIEvent.ShowUIEventWithParam += ShowFrameWithParams;
            EngineCoreEvents.UIEvent.ShowUIAndGetFrameWithParam += ShowAndGetFrameWithParams;
            EngineCoreEvents.UIEvent.ShowUIByOther += ShowFrameByOther;
            EngineCoreEvents.UIEvent.PreloadFrame += PreloadFrame;
            EngineCoreEvents.UIEvent.GetFrameEvent += GetFrame;
            EngineCoreEvents.UIEvent.HideFrameThenDestroyEvent += HideFrameThenDestroy;
            EngineCoreEvents.UIEvent.HideUIWithParamEvent += HideFrameWithParam;
            EngineCoreEvents.UIEvent.HideUIEvent += SimpleHideUI;

            UnityEngine.SceneManagement.SceneManager.sceneLoaded += OnLoadedScene;

            GOGUI.GOGUITools.GetAssetAction = EngineCoreEvents.ResourceEvent.GetAssetEvent;
            GOGUI.GOGUITools.ReleaseAssetAction = EngineCoreEvents.ResourceEvent.ReleaseAssetEvent;
        }

        /// <summary>
        /// 初始化UI管理器
        /// </summary>
        private void InitFrameModule()
        {
            UILogicMapper.Initialize();

            ScreenAspectRatio = (float)Screen.width / Screen.height;

            ResetUIRootForScreen(UIRoot);
        }


        /// <summary>
        /// 初始化UI节点
        /// </summary>
        /// <param name="root"></param>
        public void ResetUIRootForScreen(GameObject root)
        {
            UICamera = UIRoot.GetComponentInChildren<Camera>();

            GOGUI.GOGUITools.UICamera = UICamera;
            InitUIEventSystem();
        }

        /// <summary>
        /// 初始化UI事件系统
        /// </summary>
        private void InitUIEventSystem()
        {
            EventSystem uiEventSystem = UIRoot.GetComponentInChildren<EventSystem>();
            if (uiEventSystem == null)
            {
                GameObject eventSystemGameObject = new GameObject("EventSystem");
                eventSystemGameObject.AddComponent<EventSystem>();
                eventSystemGameObject.AddComponent<StandaloneInputModule>();

                eventSystemGameObject.transform.SetParent(UIRoot.transform);
            }
        }

        public override void Update()
        {
            for (int i = 0; i < m_updatingFrameList.Count; ++i)
                this.m_updatingFrameList[i].LogicHandler.Update();
        }

        List<IEnumerator> coroutines = new List<IEnumerator>();

        public void StartCoroutine(IEnumerator e)
        {
            coroutines.Add(e);
        }

        public override void LateUpdate()
        {
            //模态的协程
            for (int i = coroutines.Count - 1; i >= 0; --i)
            {
                IEnumerator e = coroutines[i];
                if (!e.MoveNext())//已经结束了的协程
                {
                    coroutines.RemoveAt(i);
                }
            }
        }

        public override void Dispose()
        {
            EngineCoreEvents.UIEvent.ShowUIEvent -= SimpleShowUI;
            EngineCoreEvents.UIEvent.ShowUIEventWithParam -= ShowFrameWithParams;
            EngineCoreEvents.UIEvent.ShowUIAndGetFrameWithParam -= ShowAndGetFrameWithParams;
            EngineCoreEvents.UIEvent.PreloadFrame -= PreloadFrame;
            EngineCoreEvents.UIEvent.HideUIEvent -= SimpleHideUI;
            EngineCoreEvents.UIEvent.HideFrameThenDestroyEvent -= HideFrameThenDestroy;
            EngineCoreEvents.UIEvent.HideUIWithParamEvent -= HideFrameWithParam;
            EngineCoreEvents.UIEvent.GetFrameEvent -= GetFrame;
            UnityEngine.SceneManagement.SceneManager.sceneLoaded -= OnLoadedScene;
        }

        #region Show UI Implements
        /// <summary>
        /// 简单显示UI
        /// </summary>
        /// <param name="uiName"></param>
        private void SimpleShowUI(string uiName)
        {
            ShowFrameWithParams(new OpenUIParams(uiName));
        }

        /// <summary>
        /// 从别的窗口打开指定的窗口
        /// </summary>
        /// <param name="targetUIName"></param>
        /// <param name="fromFrameName"></param>
        private void ShowFrameByOther(string targetUIName, string fromFrameName)
        {
            GUIFrame fromFrame = GetFrame(fromFrameName);
            OpenUIParams openUIParam = new OpenUIParams(targetUIName);
            openUIParam.OpenByFrameName = fromFrameName;

            ShowFrameWithParams(openUIParam);
        }

        /// <summary>
        /// 显示UI
        /// </summary>
        /// <param name="openParams"></param>
        private void ShowFrameWithParams(OpenUIParams openParams)
        {
            //当UI在预加载中取消预加载
            this.m_preloadUISet.Remove(openParams.UIName);

            ShowAndGetFrameWithParams(openParams);
        }

        /// <summary>
        /// 显示UI
        /// </summary>
        /// <param name="openParams"></param>
        /// <returns></returns>
        private GUIFrame ShowAndGetFrameWithParams(OpenUIParams openParams)
        {
            GUIFrame showFrame = ShowFrameInternal(openParams);

            return showFrame;
        }

        #endregion

        /// <summary>
        /// 预加载UI
        /// </summary>
        /// <param name="uiName"></param>
        private void PreloadFrame(string uiName)
        {
            GUIFrame frame = GetFrame(uiName);
            if (frame == null && !m_preloadUISet.Contains(uiName))
            {
                this.m_preloadUISet.Add(uiName);
                ShowFrameInternal(new OpenUIParams(uiName));
            }
        }

        #region Hide UI Implements

        /// <summary>
        /// 关闭UI
        /// </summary>
        /// <param name="uiName"></param>
        private void SimpleHideUI(string uiName)
        {
            GUIFrame hideFrame = GetFrame(uiName);

            //以防GUIFrame没Show就Hide
            if (hideFrame != null)
                HideFrameInternal(hideFrame);
        }

        /// <summary>
        /// 关闭UI
        /// </summary>
        /// <param name="hideParams"></param>
        private void HideFrameWithParam(HideUIParams hideParams)
        {
            GUIFrame hideFrame = GetFrame(hideParams.UIName);

            if (hideFrame != null)
            {
                hideFrame.SetHideFrameParam(hideParams);
                HideFrameInternal(hideFrame);
            }
        }

        /// <summary>
        /// 关闭UI并立即删除
        /// </summary>
        /// <param name="name"></param>
        private void HideFrameThenDestroy(string name)
        {
            HideUIParams hideFrameParam = new HideUIParams(name) { DestroyFrameImmediately = true };
            HideFrameWithParam(hideFrameParam);
        }

        #endregion


        private GUIFrame ShowFrameInternal(OpenUIParams openUIParam)
        {
            GUIFrame frame = GetFrame(openUIParam.UIName);
            if (frame == null)
            {
                frame = new GUIFrame(openUIParam);

                frame.OnFrameLoaded = OnFrameLoaded;
                frame.OnAfterShowFrameEvent = OnFrameShowFinish;
                frame.OnBeforeFrameDestroyEvent = OnHideFrame;
                frame.OnPostHideFrameEvent = OnFrameHideFinish;
                frame.OnAfterFrameDestroyEvent = OnFrameDestroyFinish;

                if (this.m_frameDict.ContainsKey(frame.ResName))
                    Debug.LogError($"frame :{frame.ResName} already loaded");
                else
                    this.m_frameDict.Add(frame.ResName, frame);

                frame.Load();
            }
            else if (frame.IsResOK)
            {
                frame.OpenFrameParam = openUIParam;

                ShowLoadedFrame(frame);

                SeekerGame.GameEvents.UI_Guid_Event.OnOpenUI.SafeInvoke(frame);
            }

            return frame;
        }

        private void HideFrameInternal(GUIFrame hideFrame)
        {
            if (hideFrame.IsResOK && hideFrame.Visible)
            {
                GUIFrame hideFrameOpenByFrame = m_frameContext.PopFrame();
                if (hideFrameOpenByFrame != null)
                {
                    AddVisibleFrame(hideFrameOpenByFrame);
                    hideFrameOpenByFrame.Visible = true;
                    ShowUIFinish(hideFrameOpenByFrame.LogicHandler);
                }
                hideFrame.HideFrame();
                //HideUIFinish(hideFrame.LogicHandler);
            }
            else
                hideFrame.Visible = false;
        }

        /// <summary>
        /// 窗口加载回调,初始化窗口逻辑
        /// </summary>
        private void OnFrameLoaded(GUIFrame loadedFrame)
        {
            loadedFrame.FrameRootTransform.SetParent(UIRoot.transform, Vector3.zero, Quaternion.identity, Vector3.one);

            UILogicMapper.MakeUILogic(loadedFrame);

            if (this.m_preloadUISet.Contains(loadedFrame.ResName))
                this.m_preloadUISet.Remove(loadedFrame.ResName);
            else
                ShowLoadedFrame(loadedFrame);
        }

        private void OnFrameShowFinish(GUIFrame showFrame)
        {
            if (showFrame.Visible)
                this.m_frameContext.AddFrameToContext(showFrame, showFrame.OpenFrameParam.OpenByFrameName);

            if (showFrame.FrameDisplayMode == FrameDisplayMode.FULLSCREEN)
            {
                GUIFrame openByFrame = GetFrame(showFrame.OpenFrameParam.OpenByFrameName);
                if (openByFrame != null && openByFrame.ResStatus == ResStatus.OK/* && !this.m_frameContext.IsExceptFrame(openByFrame)*/)
                {
                    //比较深度，如果OpenBy窗口比当前窗口低，可以直接关掉
                    if (openByFrame.FrameTopsideDepth <= showFrame.FrameTopsideDepth && openByFrame.Visible)
                    {
                        openByFrame.Visible = false;

                        RemoveVisibleFrame(openByFrame);
                    }
                }

                ShowUIFinish(showFrame.LogicHandler);
            }
        }

        /// <summary>
        /// 窗口打开回调
        /// </summary>
        /// <param name="loadedFrame"></param>
        private void ShowLoadedFrame(GUIFrame loadedFrame)
        {
            AddVisibleFrame(loadedFrame);

            loadedFrame.ShowFrame();

            if (uiNeedsGC.Contains(loadedFrame.ResName))
                DelayGC();

            loadedFrame.OpenFrameParam.OnShowFinishCallback?.Invoke(loadedFrame.LogicHandler);

            SeekerGame.GameEvents.UI_Guid_Event.OnOpenUI.SafeInvoke(loadedFrame);
        }


        void DelayGC()
        {
            float now = Time.time;

            if ((now - this.m_lastFreeResTime) > 10)
            {
                m_lastFreeResTime = now;
                TimeModule.Instance.SetTimeout(() =>
                {
                    EngineCoreEvents.ResourceEvent.TryGCCacheEvent.SafeInvoke();
                }, 1f);
            }
        }


        /// <summary>
        /// 隐藏uilogicbase
        /// </summary>
        /// <param name="excludes"></param>
        /// <param name="destroy"></param>
        /// <param name="isBreakModal">是否连不隐藏的链也断掉</param>
        public void HideAllFrames(List<string> excludes, bool destroy = false)
        {
            var keys = m_frameDict.ToArray();
            HashSet<string> ex = new HashSet<string>(excludes);

            foreach (var i in keys)
            {
                if (excludes != null && ex.Contains(i.Key))
                    continue;

                if (destroy)
                    HideFrameThenDestroy(i.Key);
                else
                    SimpleHideUI(i.Key);
            }
        }



        public GUIFrame GetFrame(string name)
        {
            GUIFrame window;
            if (m_frameDict.TryGetValue(name, out window))
                return window;
            else
                return null;
        }



        /// <summary>
        /// 获取最上层的GUIFrame
        /// </summary>
        /// <returns></returns>
        public GUIFrame GetTopsideFrame()
        {
            return m_visibleFrameSet.Count > 0 ? m_visibleFrameSet.ElementAt(0) : null;
        }

        /// <summary>
        /// GUIFrame 深度变化更新
        /// </summary>
        /// <param name="frame"></param>
        public void SetFrameDepthChanged(GUIFrame frame)
        {
            if (this.m_visibleFrameSet.RemoveWhere(item => item.ResName == frame.ResName) > 0)
                this.m_visibleFrameSet.Add(frame);
        }

        /// <summary>
        /// UI显示回调，处理全屏UI
        /// </summary>
        /// <param name="onShowUILogic"></param>
        private void ShowUIFinish(UILogicBase onShowUILogic)
        {
            if (onShowUILogic.UIFrameDisplayMode == FrameDisplayMode.FULLSCREEN)
            {
                EngineCoreEvents.CameraEvents.EnableMainCamera(false);
                this.m_showingfullScreenUISet.Add(onShowUILogic.UIFrame.ResName);
            }
        }

        /// <summary>
        /// UI隐藏
        /// </summary>
        /// <param name="onHideUILogic"></param>
        private void OnHideFrame(GUIFrame frame)
        {
            if (frame.LogicHandler.UIFrameDisplayMode == FrameDisplayMode.FULLSCREEN)
                this.m_showingfullScreenUISet.Remove(frame.ResName);

            if (this.m_showingfullScreenUISet.Count == 0)
                EngineCoreEvents.CameraEvents.EnableMainCamera(true);
        }

        #region Destroy
        /// <summary>
        /// 窗口隐藏
        /// </summary>
        /// <param name="hideFinishFrame"></param>
        private void OnFrameHideFinish(GUIFrame hideFinishFrame)
        {
            RemoveVisibleFrame(hideFinishFrame);
            if (hideFinishFrame.HideFrameParam.DestroyFrameImmediately)
                hideFinishFrame.DestroyFrame();
            else
            {
                if (hideFinishFrame.LogicHandler.AutoDestroy)
                    hideFinishFrame.SetFrameForDestroying();
            }
        }

        private void OnFrameDestroyFinish(GUIFrame destroyedFrame)
        {
            this.m_frameDict.Remove(destroyedFrame.ResName);
        }
        #endregion

        /// <summary>
        /// 添加可见窗口
        /// </summary>
        /// <param name="visibleFrame"></param>
        private void AddVisibleFrame(GUIFrame visibleFrame)
        {
            if (!this.m_visibleFrameSet.Contains(visibleFrame))
                this.m_visibleFrameSet.Add(visibleFrame);

            if (!this.m_updatingFrameList.Contains(visibleFrame) && visibleFrame.LogicHandler.NeedUpdateByFrame)
                this.m_updatingFrameList.Add(visibleFrame);

        }

        /// <summary>
        /// 删除可见窗口
        /// </summary>
        /// <param name="inVisibleFrame"></param>
        private void RemoveVisibleFrame(GUIFrame inVisibleFrame)
        {
            this.m_visibleFrameSet.Remove(inVisibleFrame);
            this.m_updatingFrameList.Remove(inVisibleFrame);
        }



        /// <summary>
        /// 处理额外场景载入时，处理全屏窗口逻辑
        /// </summary>
        /// <param name="loadedScene"></param>
        /// <param name="loadSceneMode"></param>
        private void OnLoadedScene(Scene loadedScene, LoadSceneMode loadSceneMode)
        {
            if (loadSceneMode == LoadSceneMode.Additive && m_showingfullScreenUISet.Count > 0)
                EngineCoreEvents.SystemEvents.SendTaskExecuteNextFrame.SafeInvoke(() =>
                {
                    EngineCoreEvents.CameraEvents.EnableMainCamera(false);
                });

        }

        /// <summary>
        /// 程序切后台
        /// </summary>
        public override void OnApplicationPause()
        {
            //foreach (string fn in _visibleFrames)
            //{
            //    GUIFrame window = GetFrame(fn);
            //    window.LogicHandler.OnHangup();
            //}
        }

        /// <summary>
        /// 程序唤醒
        /// </summary>
        public override void OnApplicationResume()
        {
            //foreach (string fn in _visibleFrames)
            //{
            //    GUIFrame window = GetFrame(fn);
            //    window.LogicHandler.OnWakeUp();
            //}
        }

        /// <summary>
        /// 窗口是否可见
        /// </summary>
        /// <param name="frameName"></param>
        /// <returns></returns>
        public bool IsVisible(string frameName)
        {
            return m_visibleFrameSet.SingleOrDefault(frame => frame.ResName == frameName) != null;
        }

        /// <summary>
        /// 窗口是否可见
        /// </summary>
        /// <param name="frame"></param>
        /// <returns></returns>
        public bool IsVisible(GUIFrame frame)
        {
            return m_visibleFrameSet.Contains(frame);
        }

        /// <summary>
        /// 隐藏显示UI根节点
        /// </summary>
        public bool ShowUI
        {
            set
            {
                UIRoot.SetActive(value);
            }
        }

        /// <summary>
        /// UI摄像机对象
        /// </summary>
        public Camera UICamera { get; private set; }

        /// <summary>
        /// UI根节点
        /// </summary>
        public GameObject UIRoot
        {
            get
            {
                if (m_uiRootObject == null)
                    m_uiRootObject = GameObject.Find("UIRoot");

                return this.m_uiRootObject;
            }
        }

        public static FrameMgr Instance => m_instance;

        /// <summary>
        /// 显示UI的参数
        /// </summary>
        public class OpenUIParams
        {
            public readonly string UIName;                                   //UI名
            public object Param;                                             //ShowUI时传入的参数
            public bool IsBlur = false;                                      //是否虚化背景
            public Action<UILogicBase> OnShowFinishCallback = null;         //UI关闭时回调
            public bool WithBlackBackground = false;                        //是否有黑背景
            public string OpenByFrameName = string.Empty;                   //通过哪个窗口打开
            public bool OpenUIWithAnimation = true;                         //窗口打开是否播放动画

            public OpenUIParams(string uiName)
            {
                if (!uiName.EndsWithFast(".prefab"))
                    uiName = $"{uiName}.prefab";

                UIName = uiName;
            }
        }

        /// <summary>
        /// 关闭UI的参数
        /// </summary>
        public class HideUIParams
        {
            public readonly string UIName;               //UI名
            public bool DestroyFrameImmediately = false;   //是否立即销毁
            public bool ClearOpenBy = false;    //是否清除打开此UI的窗口参数
            public float DestoryFrameDelayTime = 30.0f; //窗口销毁延时
            public bool HideFrameWithAnimation = true;      //窗口关闭是否播放动画

            public HideUIParams(string uiName)
            {
                UIName = uiName;
            }
        }

        /// <summary>
        /// GUIFrame 深度比较器
        /// </summary>
        private class FrameDepthComparer : IComparer<GUIFrame>
        {
            public int Compare(GUIFrame x, GUIFrame y)
            {
                if (x.FrameDepth == 0)
                    Debug.LogError(x.ResName + " not loaded");

                if (y.FrameDepth == 0)
                    Debug.LogError(y.ResName + "not loaded");

                //从高到低排序
                return y.FrameDepth - x.FrameDepth;
            }
        }
    }
}
