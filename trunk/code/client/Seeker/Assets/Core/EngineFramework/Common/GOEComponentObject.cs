using System.Collections.Generic;

namespace EngineCore
{
#if UNITY_EDITOR
    public
#else
    internal
#endif
        class ComponentObject
    {
        protected long mGUID = 0;
        static long msGUID = 0;
        protected List<GOEBaseComponent> mListComponent = new List<GOEBaseComponent>();

        public ComponentObject()
        {
            mGUID = ++msGUID;
        }

        internal long GUID
        {
            get { return mGUID; }
        }

        internal T AddComponent<T>() where T : GOEBaseComponent, new()
        {
            T t = new T();
            t.Owner = this;
            mListComponent.Add(t);

            return t;
        }

        internal T GetComponent<T>() where T : GOEBaseComponent
        {
            for (int i = 0; i < mListComponent.Count; ++i)
            {
                T t = mListComponent[i] as T;
                if (t != null)
                {
                    return t;
                }
            }

            return null;
        }

        internal T[] GetComponents<T>() where T : GOEBaseComponent
        {
            List<T> listT = new List<T>();

            for (int i = 0; i < mListComponent.Count; ++i)
            {
                T t = mListComponent[i] as T;
                if (t != null)
                {
                    listT.Add(t);
                }
            }

            return listT.ToArray();
        }

        internal virtual void Update()
        {
            GOEBaseComponent comp;
            for (int i = 0; i < mListComponent.Count;)
            {
                comp = mListComponent[i];
                if (comp.Enable)
                    comp.Update();
                if (comp.LifeOver)
                {
                    mListComponent.RemoveAt(i);
                    comp.OnDestroy();
                }
                else
                {
                    ++i;
                }
            }
        }

        internal virtual void Start()
        {
            for (int i = 0; i < mListComponent.Count; ++i)
            {
                GOEBaseComponent comp = mListComponent[i];
                comp.Start();
            }
        }

        internal virtual void OnDestroy()
        {
            for (int i = 0; i < mListComponent.Count; ++i)
            {
                GOEBaseComponent comp = mListComponent[i];
                comp.OnDestroy();
            }
            ClearList();
        }

        internal void AddComponent(GOEBaseComponent comp)
        {
            comp.Owner = this;
            mListComponent.Add(comp);
        }

        internal void EnableComponent<T>(bool bEnable) where T : GOEBaseComponent, new()
        {
            EnableComponent<T>(this, bEnable);
        }

        static internal T Add<T>(ComponentObject obj) where T : GOEBaseComponent, new()
        {
            T t = obj.GetComponent<T>();
            if (null == t)
            {
                t = obj.AddComponent<T>();
            }

            return t;
        }

        static internal void EnableComponent<T>(ComponentObject obj, bool bEnable) where T : GOEBaseComponent, new()
        {
            if (bEnable)
            {
                T t = Add<T>(obj);
                t.Enable = bEnable;
            }
            else
            {
                T t = obj.GetComponent<T>();
                if (t != null)
                {
                    t.Enable = bEnable;
                }
            }
        }

        internal void DelComponentByIndex(int i)
        {
            if (i < 0 || i >= mListComponent.Count)
            {
                return;
            }
            mListComponent.RemoveAt(i);
        }

        internal void DelComponent(GOEBaseComponent com)
        {
            if (mListComponent.Contains(com))
                mListComponent.Remove(com);
        }

        internal GOEBaseComponent GetComponentByIndex(int i)
        {
            if (i < 0 || i >= mListComponent.Count)
            {
                return null;
            }
            return mListComponent[i];
        }

        internal int GetComponentCount()
        {
            return mListComponent.Count;
        }

        internal virtual void ClearList()
        {
            mListComponent.Clear();
        }

    }
}
