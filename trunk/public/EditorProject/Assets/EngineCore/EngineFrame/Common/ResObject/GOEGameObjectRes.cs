using System;
using UnityEngine;
using System.Collections.Generic;

namespace EngineCore
{
#if UNITY_EDITOR
    public
#else
    internal
#endif
        class GameObjectRes : ObjectRes
    {
        protected UnityEngine.GameObject mGameObject = null;
        protected Transform mTransform = null;
        protected bool mAutoDelete = true;

        public Transform Transform { get { return mTransform; } }
        
        protected override void OnLoadRes (string name)
        {
            base.OnLoadRes (name);
            
            if ( mObject != null )
            {			
                mGameObject = mObject as GameObject;
                mTransform = mGameObject.transform;
            }
            
            if ( null == mGameObject )
            {
                mResStatus = ResStatus.ERROR;
            }
            else
            {			
                UnityEngine.Object.DontDestroyOnLoad( mGameObject );
            }
        }
        
        protected override void  DestoryObject()
        {
            mGameObject = null;
            base.DestoryObject();
        }

        private void SetGameObject(GameObject obj)
        {
            mResStatus = ResStatus.OK;

            mObject = obj;

            OnLoadRes(obj.name);
        }
        
        public UnityEngine.GameObject GameObject
        {
            get { return mGameObject; }
            set { this.SetGameObject(value); }
        }

        internal T AddUnityComponent<T>() where T : UnityEngine.Component
        {
            if (this.ResStatus != ResStatus.OK)
            {
                return null;
            }

            T t = mGameObject.AddComponent<T>();

            UnityEngine.Object.DontDestroyOnLoad(t);

            return t;
        }

        internal T GetUnityComponent<T>() where T : UnityEngine.Component
        {
            if (this.ResStatus != ResStatus.OK)
            {
                return null;
            }

            return mGameObject.GetComponent<T>();
        }

        internal T[] GetUnityComponentsInChildren<T>(bool includeInactive) where T : UnityEngine.Component
        {
            if (this.ResStatus != ResStatus.OK)
            {
                return null;
            }

            if (this.GameObject == null)
            {
                return null;
            }

            return this.GameObject.GetComponentsInChildren<T>(includeInactive);
        }
    }
}

