using SeekerGame;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EngineCore
{
    public abstract class UILogicBase
    {
        public SafeAction HideAction = new SafeAction();

        protected GameObject root => UIFrame.FrameRoot;

        HashSet<GameUIComponent> components = new HashSet<GameUIComponent>();
        public bool NeedUpdateByFrame = false;
        public bool NeedLateUpdateByFrame = false;

        public float LastUpdateTime { get; set; }

        GameUIComponent[] copiedComponents;
        bool needSync;

        protected GameUIComponent tipUI;
        private string sourceID;

        public Action OnShowTweenFinished { get; set; }
        public GUIFrame UIFrame { get; set; }

        private GameUIComponent m_rootComponent = null;

        /// <summary>
        /// 关闭时自动销毁
        /// </summary>
        public bool AutoDestroy { get; set; }

        /// <summary>
        /// 多少毫秒后将Panel设置为Static，如果为-1则不设置
        /// </summary>
        protected bool SetToStaticTime { get; set; }

        public void Init(GUIFrame guiFrame)
        {
            AutoDestroy = true;
            this.UIFrame = guiFrame;
            this.m_rootComponent = Make<GameUIComponent>(root);

            InitBuildinUIEffect();

            OnInit();
        }

        public T Make<T>(string name)
            where T : GameUIComponent, new()
        {
            T res = Make<T>(name, root);
            return res;
        }

        protected T Make<T>(string name, GameObject root)
            where T : GameUIComponent, new()
        {
            T ui = new T();
            ui.LogicHandler = this;
            bool bFind = ui.Init(name, root);
            if (bFind)
                AddComponent(ui);

            return bFind ? ui : null;
        }

        public void OnShowUIFinish()
        {

        }

        /// <summary>
        /// 对指定的GameObject Make成GameUIComponent
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        public T Make<T>(GameObject obj) where T : GameUIComponent, new()
        {
            T uiComponent = new T();
            uiComponent.LogicHandler = this;
            uiComponent.Init(obj);
            AddComponent(uiComponent);

            return uiComponent;
        }

        public void Make(string name, GameUIComponent ui)
        {
            ui.LogicHandler = this;
            bool bFind = ui.Init(name, root);
            if (bFind)
                AddComponent(ui);
        }

        public void AddComponent(GameUIComponent comp)
        {
            if (comp.LogicHandler == this)
            {
                components.Add(comp);
                needSync = true;
            }
        }

        public void RemoveComponent(GameUIComponent comp)
        {
            if (comp.LogicHandler == this)
            {
                components.Remove(comp);
                needSync = true;
            }
        }

        public virtual void OnShow(object param)
        {

        }
        public virtual void OnShow(object param, bool isReturn)
        {
            GameUIComponent[] copied = GetCopiedComonents();
            for (int i = 0; i < copied.Length; i++)
            {
                if (copied[i].gameObject && copied[i].Visible)
                    copied[i].OnShow(param);
            }

            OnShow(param);
        }

        public virtual void OnGuidShow(int type = 0)
        {

        }

        public virtual void OnHide()
        {
            GameUIComponent[] array = GetCopiedComonents();
            for (int i = 0; i < array.Length; i++)
            {
                if (array[i].gameObject && array[i].Visible)
                {
                    array[i].OnHide();
                }
            }
            HideAction.SafeInvoke();
        }

        /// <summary>
        /// 应用后台挂起回调
        /// </summary>
        public virtual void OnHangup() { }

        /// <summary>
        /// 应用后台唤醒回调
        /// </summary>
        public virtual void OnWakeUp() { }

        GameUIComponent[] GetCopiedComonents()
        {
            if (copiedComponents == null)
                copiedComponents = new GameUIComponent[0];
            if (needSync)
            {
                copiedComponents = new GameUIComponent[components.Count];
                int idx = 0;
                foreach (GameUIComponent i in components)
                {
                    copiedComponents[idx++] = i;
                }
                needSync = false;
            }
            return copiedComponents;
        }

        protected virtual void OnInit()
        {

        }

        protected virtual void SetCloseBtnID(string str)
        {
            string[] strList = str.Split(';');
            for (int i = 0; i < strList.Length; i++)
            {
                GameButton close = Make<GameButton>(strList[i]);
                if (close != null)
                    close.AddClickCallBack(onClickClose);
            }

        }

        public void SetCloseButtonID(params string[] closeButtonIds)
        {
            for (int i = 0; i < closeButtonIds.Length; ++i)
                SetCloseBtnID(closeButtonIds[i]);
        }

        protected virtual void SetTipUI(GameUIComponent tipUI, string source)
        {
            this.tipUI = tipUI;
            this.sourceID = source;
        }

        protected virtual void onClickClose(GameObject obj)
        {
            EngineCoreEvents.AudioEvents.PlayAudio.SafeInvoke(Audio.AudioType.UISound, EngineCommonAudioKey.Button_Click_Common.ToString());
            CloseFrame();
        }

        protected virtual void onClickBack(GameObject go)
        {
            CloseFrame();
        }

        public virtual void Dispose()
        {
            foreach (GameUIComponent i in GetCopiedComonents())
            {
                i.Dispose(false);
            }
            components.Clear();
        }


        public virtual void Update()
        {
            GameUIComponent[] copied = GetCopiedComonents();
            for (int i = 0; i < copied.Length; i++)
            {
                if (copied[i].IsForceUpdate)
                    copied[i].ForceUpdate();
                if (copied[i].CachedVisible)
                    copied[i].OnUpdate();
            }
        }

        public virtual void LateUpdate()
        {
            GameUIComponent[] copied = GetCopiedComonents();
            for (int i = 0; i < copied.Length; i++)
            {
                if (copied[i].CachedVisible)
                    copied[i].OnLateUpdate();
            }
        }


        /// <summary>
        /// 启动UI协程
        /// </summary>
        /// <param name="e"></param>
        public void StartUICoroutine(IEnumerator e)
        {
            //FrameMgr.Instance.StartCoroutine(e);
        }

        public virtual void CloseFrame(bool destroyImmediately = false)
        {
            EngineCoreEvents.UIEvent.HideUIWithParamEvent.SafeInvoke(new FrameMgr.HideUIParams(UIFrame.ResName)
            {
                DestroyFrameImmediately = destroyImmediately
            });
        }

        /// <summary>
        /// 初始化
        /// </summary>
        private void InitBuildinUIEffect()
        {
            UIEffectCanvas[] uiEffectCanvases = UIFrame.FrameRoot.GetComponentsInChildren<UIEffectCanvas>();
            for (int i = 0; i < uiEffectCanvases.Length; ++i)
            {
                UIEffectCanvas uiEffectCanvas = uiEffectCanvases[i];
                if (uiEffectCanvas.IsBuildinEffect)
                {
                    GameBuildinUIEffect buildinEffectLifyController = uiEffectCanvas.gameObject.AddComponent<GameBuildinUIEffect>();
                    buildinEffectLifyController.InitBuildinUIEffect(uiEffectCanvas.BuildinEffectName, this);
                }
            }
        }

        /// <summary>
        /// UIRoot Canvas
        /// </summary>
        public Canvas Canvas => UIFrame.UIRootCanvas;

        /// <summary>
        /// UI根节点RectTransform
        /// </summary>
        public RectTransform Transform => UIFrame.FrameRootTransform;

        /// <summary>
        /// 窗口显示形式
        /// </summary>
        public abstract FrameDisplayMode UIFrameDisplayMode { get; }
    }
}
