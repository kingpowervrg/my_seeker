using DG.Tweening;
using GOGUI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EngineCore
{
    public class GameUIComponent
    {
        public GameObject gameObject;
        RectTransform widget;
        Transform savedTransform;
        List<GameUIComponent> children = new List<GameUIComponent>();
        public bool FromPool = false;

        //是否强制执行Update(慎用！)
        private bool m_isForceUpdate = false;

        static Material grayMaterial;
        static Material shadowMaterial;

        public RectTransform Widget { get { return widget; } }

        public object Parameter { get; set; }

        public UILogicBase LogicHandler { get; set; }

        public List<GameUIComponent> Children { get { return children; } }

        private Renderer[] cachedChildRenderList = null;

        //UI组件上的Tweener
        private UITweenerBase[] m_uiComponentTweeners = null;

        protected Material GrayMaterial
        {
            get
            {
                if (!grayMaterial)
                {
                    Shader grayShader = ShaderModule.Instance.GetShader("UI/Gray");
                    grayMaterial = new Material(grayShader);
                }
                return grayMaterial;
            }
        }

        protected Material ShadowMaterial
        {
            get
            {
                if (!shadowMaterial)
                {
                    Shader shadowShader = ShaderModule.Instance.GetShader("UI/Shadow");
                    shadowMaterial = new Material(shadowShader);
                }
                return shadowMaterial;
            }
        }

        public float X
        {
            get
            {
                if (widget)
                {
                    return widget.anchoredPosition.x;
                }
                else
                {
                    if (!savedTransform)
                        savedTransform = gameObject.transform;
                    return savedTransform.localPosition.x;
                }
            }
            set
            {
                if (widget)
                {
                    Vector2 old = widget.anchoredPosition;
                    Vector2 vec = new Vector3(value, old.y);
                    widget.anchoredPosition = vec;
                }
                else
                {
                    if (!savedTransform)
                        savedTransform = gameObject.transform;
                    Vector3 old = savedTransform.localPosition;
                    Vector3 vec = new Vector3(value, old.y, old.z);
                    savedTransform.localPosition = vec;
                }
            }
        }

        public float Y
        {
            get
            {
                if (widget)
                {
                    return widget.anchoredPosition.y;
                }
                else
                {
                    if (!savedTransform)
                        savedTransform = gameObject.transform;
                    return savedTransform.localPosition.y;
                }
            }
            set
            {
                if (widget)
                {
                    Vector2 old = widget.anchoredPosition;
                    Vector2 vec = new Vector3(old.x, value);
                    widget.anchoredPosition = vec;
                }
                else
                {
                    if (!savedTransform)
                        savedTransform = gameObject.transform;
                    Vector3 old = savedTransform.localPosition;
                    Vector3 vec = new Vector3(old.x, value, old.z);
                    savedTransform.localPosition = vec;
                }
            }
        }


        public Vector3 Position
        {
            get { return gameObject.transform.position; }

            set { gameObject.transform.position = value; }

        }

        protected virtual void SetCloseBtnID(string str)
        {
            GameButton close = Make<GameButton>(str);
            if (close != null)
                close.AddClickCallBack((GameObject go) => { this.Visible = false; EngineCoreEvents.AudioEvents.PlayAudio.SafeInvoke(Audio.AudioType.UISound, EngineCommonAudioKey.Close_Window.ToString()); });
        }

        protected void SetCloseImageID(string strImage)
        {
            GameImage closeImage = Make<GameImage>(strImage);
            if (closeImage != null)
                closeImage.AddClickCallBack((GameObject go) => { this.Visible = false; EngineCoreEvents.AudioEvents.PlayAudio.SafeInvoke(Audio.AudioType.UISound, EngineCommonAudioKey.Close_Window.ToString()); });
        }

        public virtual void SetGray(bool gray)
        {
            /*UIMySprite sp = gameObject.GetComponent<UIMySprite>();
            if (sp == null)
                return;
            sp.SetGray = gray;*/
        }

        public bool Init(string name, GameObject root)
        {
            gameObject = root.GetUGUIComponentByID(name);
            if (gameObject == null)
            {
                Debug.LogError(name + "控件未找到" + " rootname=" + root.name);
                return false;
            }

            Init(gameObject);
            return true;
        }


        public bool TryInit(string name, GameObject root)
        {
            gameObject = root.GetUGUIComponentByID(name);
            if (gameObject == null)
            {
                return false;
            }

            Init(gameObject);
            return true;
        }

        public void Init(GameObject go)
        {
            gameObject = go;
            widget = GetComponent<RectTransform>();

            cachedVisible = Visible;
            cachedChildRenderList = go.GetComponentsInChildren<Renderer>(true);

            OnInit();
        }


        protected virtual void OnInit()
        {
        }

        public virtual void Dispose()
        {

        }
        public void Dispose(bool keepChildren)
        {
            Dispose();
            for (int i = 0; i < children.Count; i++)
            {
                var child = children[i];
                child.Dispose(keepChildren);
            }
            if (!keepChildren)
            {
                children.Clear();
                if (LogicHandler != null)
                {
                    LogicHandler.RemoveComponent(this);
                }
            }
        }

        public virtual void OnShow(object param)
        {

        }


        public virtual void OnHide()
        {
            if (!isHiding)
                return;
            TimeModule.Instance.RemoveTimeaction(setClose);

            setClose();
        }


        /// <summary>
        /// 启动协程
        /// </summary>
        /// <param name="e"></param>
        public void StartUICoroutine(IEnumerator e)
        {
            LogicHandler.StartUICoroutine(e);
        }


        public virtual void OnUpdate()
        {

        }

        public virtual void ForceUpdate()
        {

        }
        public virtual void OnLateUpdate()
        {

        }

        bool cachedVisible = false;
        public bool CachedVisible
        {
            get
            {
                return cachedVisible;
            }
        }

        public virtual bool Visible
        {
            get
            {
                if (!gameObject)
                    return false;
                return gameObject.activeSelf;
            }
            set
            {
                if (!gameObject)
                    return;
                cachedVisible = value;
                if (value != gameObject.activeSelf)
                {
                    if (!gameObject.activeInHierarchy && !value)
                    {
                        SetVisible(value);
                        return;
                    }
                    if (m_uiComponentTweeners == null)
                        m_uiComponentTweeners = gameObject.GetComponentsInChildren<UITweenerBase>(true);
                    if (m_uiComponentTweeners.Length == 0)
                    {
                        SetVisible(value);
                        return;
                    }
                    if (value)
                        PlayUIOnShowAnimation();
                    else
                        PlayUIOnHideAnimation();
                }
                else if (value)
                    PlayUIOnShowAnimation();
            }
        }
        public bool VisibleInHierarchy
        {
            get
            {
                if (!gameObject)
                    return false;
                return gameObject.activeInHierarchy;
            }
        }

        private bool isHiding = false;


        private void setClose()
        {
            isHiding = false;
            SetVisible(false);
        }

        /// <summary>
        /// 改变Visible并且不触发关闭动画
        /// </summary>
        /// <param name="value"></param>
        public void SetVisible(bool value)
        {
            if (Visible != value)
            {
                gameObject.SetActive(value);
                if (value)
                    OnShow(Parameter);
                else
                    OnHide();

                if (this.cachedChildRenderList != null)
                {
                    for (int i = 0; i < this.cachedChildRenderList.Length; ++i)
                        this.cachedChildRenderList[i].gameObject.layer = value ? LayerDefine.UIShowLayer : LayerDefine.UIHideLayer;
                }

                //if (LogicHandler != null)
                //    Functions.Guide.OnGameUIVisibleChange.SafeInvoke(LogicHandler.GUIFrame.ResName, value);
            }
        }

        public void SetActive(bool value)
        {
            gameObject.SetActive(value);
        }

        public void SetScale(Vector3 p)
        {
            gameObject.transform.localScale = p;
        }

        public virtual void AddClickCallBack(EventTriggerListener.VoidDelegate func)
        {
            EventTriggerListener lis = EventTriggerListener.Get(gameObject);
            lis.parameter = this;
            lis.onClick = func;
        }

        public virtual void AddClickCallBackPosition(EventTriggerListener.VectorDelegate func)
        {
            EventTriggerListener lis = EventTriggerListener.Get(gameObject);
            lis.parameter = this;
            lis.onClickPosition = func;
        }

        public virtual void RemoveClickCallBackPosition(EventTriggerListener.VectorDelegate func)
        {
            EventTriggerListener lis = EventTriggerListener.Get(gameObject);
            lis.onClickPosition = null;
        }

        public void AddLongPressCallBack(GOGUI.EventTriggerListener.VectorDelegate callBack)
        {
            EventTriggerListener listener = EventTriggerListener.Get(gameObject);
            listener.OnLongPress = callBack;
        }

        public void RemoveLongPressCallBack(GOGUI.EventTriggerListener.VectorDelegate callBack)
        {
            EventTriggerListener listener = EventTriggerListener.Get(gameObject);
            listener.OnLongPress = null;
        }

        public void AddLongPressEndCallBack(GOGUI.EventTriggerListener.VoidDelegate callBack)
        {
            EventTriggerListener listener = EventTriggerListener.Get(gameObject);
            listener.OnLongPressEnd = callBack;
        }

        public void RemoveLongPressEndCallBack(GOGUI.EventTriggerListener.VoidDelegate callBack)
        {
            EventTriggerListener listener = EventTriggerListener.Get(gameObject);
            listener.OnLongPressEnd = null;
        }

        public virtual void RemoveClickCallBack(EventTriggerListener.VoidDelegate func)
        {
            EventTriggerListener triggerListener = EventTriggerListener.Get(gameObject);
            triggerListener.onClick = null;
        }
        public virtual void AddOverCallBack(EventTriggerListener.VoidDelegate func)
        {
            EventTriggerListener lis = EventTriggerListener.Get(gameObject);
            lis.onEnter = func;
        }

        public void RemoveOverCallBack(EventTriggerListener.VoidDelegate func)
        {
            EventTriggerListener lis = EventTriggerListener.Get(gameObject);
            lis.onEnter = null;
        }

        public virtual void AddDeselectCallBack(EventTriggerListener.VoidDelegate func)
        {
            EventTriggerListener lis = EventTriggerListener.Get(gameObject);
            lis.parameter = this;
            lis.onDeselect = func;
        }

        public void RemoveDeselectCallBack(EventTriggerListener.VoidDelegate func)
        {
            EventTriggerListener lis = EventTriggerListener.Get(gameObject);
            lis.parameter = this;
            lis.onDeselect = null;

        }

        public virtual void AddSelectCallBack(EventTriggerListener.VoidDelegate func)
        {
            EventTriggerListener lis = EventTriggerListener.Get(gameObject);
            lis.parameter = this;
            lis.onSelect = func;
        }

        public void RemoveSelectCallBack(EventTriggerListener.VoidDelegate func)
        {
            EventTriggerListener lis = EventTriggerListener.Get(gameObject);
            lis.parameter = this;
            lis.onSelect = null;
        }


        public virtual void AddExitCallBack(EventTriggerListener.VoidDelegate func)
        {
            EventTriggerListener lis = EventTriggerListener.Get(gameObject);
            lis.onExit = func;
        }

        public void RemoveExitCallBack(EventTriggerListener.VoidDelegate func)
        {
            EventTriggerListener lis = EventTriggerListener.Get(gameObject);
            lis.onExit = null;
        }

        public void AddPressDownCallBack(EventTriggerListener.VoidDelegate func)
        {
            EventTriggerListener lis = EventTriggerListener.Get(gameObject);
            lis.parameter = this;
            lis.onDown = func;
        }

        public void AddPressUpCallBack(EventTriggerListener.VoidDelegate func)
        {
            EventTriggerListener lis = EventTriggerListener.Get(gameObject);
            lis.parameter = this;
            lis.onUp = func;
        }
        public void AddPressUpCallBack(EventTriggerListener.BoolDelegate func)
        {
            EventTriggerListener lis = EventTriggerListener.Get(gameObject);
            lis.onUpAndState = func;
        }
        public void RemovePressDownCallBack(EventTriggerListener.VoidDelegate func)
        {
            EventTriggerListener lis = EventTriggerListener.Get(gameObject);
            lis.onDown = null;
        }

        public void RemovePressUpCallBack(EventTriggerListener.VoidDelegate func)
        {
            EventTriggerListener lis = EventTriggerListener.Get(gameObject);
            lis.onUp = null;
        }
        public void RemovePressUpCallBack(EventTriggerListener.BoolDelegate func)
        {
            EventTriggerListener lis = EventTriggerListener.Get(gameObject);
            lis.onUpAndState = null;
        }
        public void AddDragCallBack(EventTriggerListener.Vector2Delegate func)
        {
            DragEventTriggerListener lis = DragEventTriggerListener.Get(gameObject);
            lis.onDrag += func;
        }

        public void RemoveDragCallBack(EventTriggerListener.Vector2Delegate func)
        {
            DragEventTriggerListener lis = DragEventTriggerListener.Get(gameObject);
            lis.onDrag = null;
        }

        public void AddDragStartCallBack(EventTriggerListener.VectorDelegate func)
        {
            DragEventTriggerListener lis = DragEventTriggerListener.Get(gameObject);
            lis.onDragStart = func;
        }
        public void RemoveDragStartCallBack(EventTriggerListener.VectorDelegate func)
        {
            DragEventTriggerListener lis = DragEventTriggerListener.Get(gameObject);
            lis.onDragStart = null;
        }

        public void AddDragEndCallBack(EventTriggerListener.VectorDelegate func)
        {
            DragEventTriggerListener lis = DragEventTriggerListener.Get(gameObject);
            lis.onDragEnd = func;
        }

        public void RemoveDragEndCallBack(EventTriggerListener.VectorDelegate func)
        {
            DragEventTriggerListener lis = DragEventTriggerListener.Get(gameObject);
            lis.onDragEnd = null;
        }

        /*新增事件*/
        public void AddLongClickCallBack(EventTriggerListener.FloatDelegate func)
        {
            EventTriggerListener lis = EventTriggerListener.Get(gameObject);
            lis.onLongClick = func;
        }

        public void RemoveLongClickCallBack(EventTriggerListener.FloatDelegate func)
        {
            EventTriggerListener lis = EventTriggerListener.Get(gameObject);
            lis.onLongClick = null;
        }

        public void CancelLongClick()
        {
            EventTriggerListener lis = EventTriggerListener.Get(gameObject);
            lis.CancelLongCllick();
        }


        public static void Make(GameUIComponent com, GameObject root, UILogicBase logic, GameUIComponent parent)
        {
            if (parent != null)
                parent.children.Add(com);
            if (logic != null)
            {
                com.LogicHandler = logic;
                logic.AddComponent(com);
            }
            com.Init(root);
        }

        public static void Make(GameUIComponent com, string id, GameObject root, UILogicBase logic, GameUIComponent parent)
        {
            if (parent != null)
                parent.children.Add(com);
            if (logic != null)
            {
                com.LogicHandler = logic;
                logic.AddComponent(com);
            }
            com.Init(id, root);
        }

        public static T Make<T>(GameObject root, UILogicBase logic, GameUIComponent parent = null) where T : GameUIComponent, new()
        {
            T com = new T();
            if (parent != null)
                parent.children.Add(com);
            if (logic != null)
            {
                com.LogicHandler = logic;
                logic.AddComponent(com);
            }
            com.Init(root);
            if (com.gameObject == null)
                return null;
            return com;
        }



        public static T TryMake<T>(string id, GameObject root, UILogicBase logic, GameUIComponent parent = null) where T : GameUIComponent, new()
        {
            T com = new T();
            com.TryInit(id, root);

            if (com.gameObject == null)
                return null;

            if (parent != null)
                parent.children.Add(com);
            if (logic != null)
            {
                com.LogicHandler = logic;
                logic.AddComponent(com);
            }

            return com;
        }

        public static T Make<T>(string id, GameObject root, UILogicBase logic, GameUIComponent parent = null) where T : GameUIComponent, new()
        {
            T com = new T();
            if (parent != null)
                parent.Children.Add(com);
            if (logic != null)
            {
                com.LogicHandler = logic;
                logic.AddComponent(com);
            }
            com.Init(id, root);
            if (com.gameObject == null)
                return null;
            return com;
        }

        public T Make<T>(string id, GameObject root) where T : GameUIComponent, new()
        {
            return Make<T>(id, root, LogicHandler, this);
        }

        public T TryMake<T>(string id, GameObject root) where T : GameUIComponent, new()
        {
            return TryMake<T>(id, root, LogicHandler, this);
        }

        public T Make<T>(string id) where T : GameUIComponent, new()
        {
            return Make<T>(id, gameObject);
        }

        public T TryMake<T>(string id) where T : GameUIComponent, new()
        {
            return TryMake<T>(id, gameObject);
        }

        public T Make<T>(GameObject root) where T : GameUIComponent, new()
        {
            T com = new T();
            if (LogicHandler != null)
            {
                children.Add(com);
                com.LogicHandler = LogicHandler;
                LogicHandler.AddComponent(com);
            }
            com.Init(root);
            if (com.gameObject == null)
                return null;
            return com;
        }


        public T GetComponent<T>()
            where T : Component
        {
            T res = gameObject.GetComponent<T>();
            if (res == null)
            {
                T[] array = gameObject.GetComponentsInChildren<T>(true);
                if (array.Length > 0)
                    res = array[0];
            }
            if (res == null)
            {
                //D.warn("Cannot find any {0} component in {1}.", typeof(T), gameObject);
            }
            return res;
        }
        public T[] GetComponents<T>() where T : Component
        {
            T[] array = gameObject.GetComponentsInChildren<T>(true);
            //if (array.Length > 0)
            //    res = array[0];
            return array;
        }

        /// <summary>
        /// 播放UI动画
        /// </summary>
        /// <param name="playForward"></param>
        /// <param name="PlayFinishCallback"></param>
        public float PlayUITweener(UITweenerBase.TweenTriggerType triggerType, bool playForward = true, TweenCallback PlayFinishCallback = null)
        {
            UITweenerBase maxDurationTweener = null;

            foreach (UITweenerBase tweener in UIComponentTweeners)
            {
                if (tweener && tweener.isActiveAndEnabled && tweener.m_triggerType == triggerType)
                {
                    if (maxDurationTweener == null)
                        maxDurationTweener = tweener;
                    else
                    {
                        if (maxDurationTweener.TweenTotalTime < tweener.TweenTotalTime)
                            maxDurationTweener = tweener;
                    }

                    tweener.ResetAndPlay(playForward);
                }
            }

            if (maxDurationTweener != null && PlayFinishCallback != null)
                maxDurationTweener.SetTweenCompletedCallback(PlayFinishCallback);

            return maxDurationTweener != null ? maxDurationTweener.TweenTotalTime : 0;
        }


        #region Private Method

        /// <summary>
        /// Play Animation when UI SetVisible OnShow
        /// </summary>
        private void PlayUIOnShowAnimation()
        {
            SetVisible(true);
            TimeModule.Instance.RemoveTimeaction(setClose);
            PlayUITweener(UITweenerBase.TweenTriggerType.OnShow, true);
        }

        /// <summary>
        /// Play Animation When UI OnHide
        /// </summary>
        private void PlayUIOnHideAnimation()
        {
            TimeModule.Instance.RemoveTimeaction(setClose);

            float maxOnHideAnimationLength = PlayUITweener(UITweenerBase.TweenTriggerType.OnHide);

            isHiding = true;
            TimeModule.Instance.SetTimeout(setClose, maxOnHideAnimationLength, false, false);
        }

        #endregion

        /// <summary>
        /// Public Property Access UIComponent Tweeners
        /// </summary>
        public UITweenerBase[] UIComponentTweeners
        {
            get
            {
                if (m_uiComponentTweeners == null)
                    m_uiComponentTweeners = gameObject.GetComponentsInChildren<UITweenerBase>(true);

                return m_uiComponentTweeners;
            }
        }

        /// <summary>
        /// 事件参数
        /// </summary>
        public EventTriggerListener BaseEventTriggerListener => EventTriggerListener.Get(gameObject);

        public static GameUIComponent ToGameUIComponent(GameObject go)
        {
            if (go)
            {
                GOGUI.EventTriggerListener uel = go.GetComponent<GOGUI.EventTriggerListener>();
                if (uel)
                {
                    return uel.parameter as GameUIComponent;
                }
                else
                    return null;
            }
            else
                return null;
        }

        /// <summary>
        /// 强制Update
        /// </summary>
        public bool IsForceUpdate
        {
            get { return m_isForceUpdate; }
            set { m_isForceUpdate = value; }
        }
    }
}
