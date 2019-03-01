using System;
using UnityEngine;
namespace EngineCore
{
    public enum ResStatus
    {
        NONE,
        WAIT,
        OK,
        ERROR,
        DESTROYED,
    }
}
namespace EngineCore
{
#if UNITY_EDITOR
    public
#else
    internal
#endif
        class ObjectRes : ComponentObject
    {
        protected string mName = string.Empty;
        protected ResStatus mResStatus = ResStatus.NONE;
        protected UnityEngine.Object mObject = null;
        protected float mDelayDestroyTime = 0.0f;
        private bool mAsyncLoad = true;
        private LoadPriority mLoadPriority = LoadPriority.Default;

        public Action OnResLoaded = null;

        internal ResStatus ResStatus
        {
            get { return mResStatus; }
            set { mResStatus = value; }
        }
        internal UnityEngine.Object Object
        {
            get { return mObject; }
        }

        public string Name
        {
            set
            {
                if (mResStatus == ResStatus.DESTROYED)
                    throw new Exception("Entity already destroyed");
                if (mName == value)
                    return;
                DestoryObject();
                if (mResStatus != ResStatus.NONE)
                    mResStatus = ResStatus.NONE;
                mName = value;
            }
            get { return mName; }
        }

        public LoadPriority Priority
        {
            set { mLoadPriority = value; }
            get { return mLoadPriority; }
        }

        public void Reload()
        {
            if (mResStatus == ResStatus.DESTROYED)
                throw new Exception("Entity already destroyed");
            DestoryObject();
            if (mResStatus != ResStatus.NONE)
                mResStatus = ResStatus.NONE;
            Load();
        }


        public void Load()
        {
            if (mResStatus == ResStatus.DESTROYED)
                throw new Exception("Entity already destroyed");
            if (mName.Length < 1)
            {
                return;
            }
            if (mResStatus != ResStatus.NONE)
            {
                return;
            }

            mResStatus = ResStatus.WAIT;

            LoadRes(mName, OnLoadRes);

        }

        protected virtual void LoadRes(string name, Action<string, UnityEngine.Object> callback)
        {
            ResourceModule.Instance.GetAsset(name, callback, mLoadPriority);
        }

        internal virtual void Destroy()
        {
            DestoryObject();
            base.OnDestroy();
        }

        protected virtual void DestoryObject()
        {
            if (this.ResStatus == ResStatus.OK || this.ResStatus == ResStatus.ERROR)
            {
                ResourceModule.Instance.ReleaseAsset(Name, mObject);
            }
            else if (this.ResStatus == ResStatus.WAIT)
            {
                ResourceModule.Instance.ReleaseAssetCallback(Name, OnLoadRes);
            }
            if (mObject != null)
            {
                mObject = null;
            }

            mResStatus = ResStatus.DESTROYED;
        }

        private void OnLoadRes(string name, UnityEngine.Object obj)
        {
            if (mResStatus == ResStatus.DESTROYED)
                throw new Exception("Entity already destroyed");
            mResStatus = ResStatus.OK;
            if (mObject)
                throw new Exception("黑人问号???!!!!!!!");
            mObject = obj;

            if (null == mObject)
            {
                mResStatus = ResStatus.ERROR;
            }

            this.OnLoadRes(name);

            if (OnResLoaded != null)
            {
                OnResLoaded();
            }
        }

        protected virtual void OnLoadRes(string name)
        {

        }

        internal virtual void OnLeaveScene()
        {
            //切换场景会中断所有加载
            if (mResStatus == ResStatus.WAIT)
                mResStatus = ResStatus.NONE;
        }
    }
}

