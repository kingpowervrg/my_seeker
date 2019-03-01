using GOGUI;
using System.Collections.Generic;
using UnityEngine;
namespace EngineCore
{
    /// <summary>
    /// 通用容器控件
    /// </summary>
    public class GameUIContainer : GameUIComponent
    {
        //protected GameObject template;
        //protected Transform m_templateTransform;
        public const string TEMPLATE = "Template";
        public const string POOLOBJECT = "pool";

        protected bool _usePool = false;
        protected MemoryPool<GameUIComponent> childPool;
        protected Transform poolTransform;
        Transform cachedTransform;

        private bool m_templateVisiable = false;

        protected List<RectTransform> childList = new List<RectTransform>();
        protected bool childListDirty = false;

        protected virtual Transform ContainerTransform
        {
            get { return cachedTransform; }
        }

        //public GameObject GetTemplate()
        //{
        //    return template;
        //}

        public bool UseChildPool
        {
            set
            {
                _usePool = value;
                if (value)
                {
                    childPool = new MemoryPool<GameUIComponent>(200);
                    GameObject temp = new GameObject();
                    temp.name = POOLOBJECT + "_" + gameObject.name;
                    temp.SetActive(false);
                    poolTransform = temp.transform;
                    poolTransform.parent = ContainerTransform;
                }
                else
                {
                    if (childPool == null)
                        return;
                    GameUIComponent obj = childPool.Alloc();
                    if (obj != null)
                    {
                        GameObject.Destroy(obj.gameObject);
                        obj.Dispose(false);
                    }
                    childPool.Dispose();
                }
            }
            get
            {
                return _usePool;
            }
        }
        protected override void OnInit()
        {
            base.OnInit();
            cachedTransform = gameObject.transform;
            if (ContainerTransform.childCount > 0)
            {
                Transform t = ContainerTransform.GetChild(0);
                if (t != null)
                {
                    if (t.name == TEMPLATE)
                    {
                        this.ContainerTemplate = t;
                        this.m_templateVisiable = ContainerTemplate.gameObject.activeSelf;

                        this.IsTemplateWithCanvas = ContainerTemplate.GetComponentsInChildren<Canvas>(true).Length > 0;

                        t.gameObject.SetActive(false);
                    }
                }
            }

            UseChildPool = _usePool;
        }

        protected GameUIComponent DisposeChild(GameObject child)
        {
            GameUIComponent ui = child.ToGameUIComponent();
            childListDirty = true;
            if (_usePool)
            {
                if (childPool.Free(ui))
                {
                    if (ui != null)
                        ui.Visible = false;
                    else
                        child.SetActive(false);
                    Vector3 sc = child.transform.localScale;
                    Vector3 ps = child.transform.localPosition;
                    child.transform.SetParent(poolTransform);
                    child.transform.localScale = sc;
                    child.transform.localPosition = ps;
                    return ui;
                }
            }
            if (ui != null)
            {
                if (ui.Visible)
                    ui.OnHide();
                ui.Dispose(false);
            }
            child.transform.SetParent(null);
            GameObject.Destroy(child);
            return ui;
        }

        public GameObject AddChild()
        {
            GameObject go = null;
            if (_usePool)
            {
                GameUIComponent ui = childPool.Alloc();
                if (ui != null)
                {
                    go = ui.gameObject;
                    Vector3 sc = ui.Widget.localScale;
                    Vector3 ps = ui.Widget.anchoredPosition3D;
                    ui.Widget.SetParent(ContainerTransform);
                    ui.Widget.localScale = sc;
                    ui.Widget.anchoredPosition3D = ps;
                    ui.Widget.localRotation = Quaternion.identity;
                    ui.FromPool = true;
                    ui.Visible = true;
                }
            }
            if (go == null)
            {
                go = GOGUITools.AddChild(ContainerTransform, ContainerTemplate).gameObject;

                go.SetActive(this.m_templateVisiable);
            }
            childListDirty = true;
            return go;
        }
        public T Make<T>(int idx)
            where T : GameUIComponent, new()
        {
            Transform t = GetChildByIndex(idx);
            T res = Make<T>(t.gameObject);
            EventTriggerListener lis = EventTriggerListener.Get(res.gameObject);
            lis.parameter = res;
            return res;
        }

        public virtual T AddChild<T>()
           where T : GameUIComponent, new()
        {
            GameObject go = AddChild();
            T res = go.ToGameUIComponent() as T;
            if (res != null)
                return res;
            res = Make<T>(go);
            EventTriggerListener lis = EventTriggerListener.Get(go);
            lis.parameter = res;
            return res;
        }

        public virtual T GetChild<T>(int idx)
           where T : GameUIComponent
        {
            Transform val = GetChildByIndex(idx);
            if (val != null)
            {
                return val.gameObject.ToGameUIComponent<T>();
            }
            else
            {
                return null;
            }
        }

        public virtual Transform GetChildByIndex(int idx)
        {
            if (ChildCount > idx)
                return childList[idx];
            else
                return null;
        }

        public virtual int ChildCount
        {
            get
            {
                if (childList.Count > 0 && !childList[0])
                {
                    childListDirty = true;
                }
                if (childListDirty)
                {
                    childListDirty = false;
                    childList.Clear();
                    for (int i = 0; i < ContainerTransform.childCount; i++)
                    {
                        var t = ContainerTransform.GetChild(i);
                        if (t == poolTransform)
                            continue;
                        if (t == ContainerTemplate)
                            continue;
                        childList.Add(t.GetComponent<RectTransform>());
                    }
                }
                return childList.Count;
            }
        }

        public virtual void Clear()
        {
            while (ChildCount > 0)
            {
                RemoveChild(0);
            }
        }
        public virtual T RemoveChild<T>(int idx)
            where T : GameUIComponent
        {
            return RemoveChild(idx) as T;
        }

        GameUIComponent RemoveChild(int idx)
        {
            Transform val = GetChildByIndex(idx);
            if (!val)
                return null;
            return DisposeChild(val.gameObject);
        }

        public GameUIComponent RemoveChildByIndex(int idx)
        {
            Transform val = GetChildByIndex(idx);
            if (!val)
                return null;
            return DisposeChild(val.gameObject);
        }

        public virtual void EnsureSize<T>(int count)
            where T : GameUIComponent, new()
        {
            bool oldVisible = Visible;
            int cur = ChildCount;
            if (cur != count)
                Visible = false;
            if (cur < count)
            {
                int diff = count - cur;
                for (int i = 0; i < diff; i++)
                {
                    AddChild<T>();
                }
            }
            else
            {
                int diff = cur - count;
                for (int i = 0; i < diff; i++)
                {
                    RemoveChild<T>(0);
                }
            }
            if (oldVisible)
                Visible = true;

            if (IsTemplateWithCanvas)
                LogicHandler.UIFrame.SetFrameDirty();
        }

        public Transform ContainerTemplate
        {
            get; private set;
        }


        /// <summary>
        /// Template 是否
        /// </summary>
        private bool IsTemplateWithCanvas
        {
            get; set;
        }
    }

}
