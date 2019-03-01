using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using System.Collections;

namespace EngineCore
{
    public enum ResourceState
    {
        Failed,
        NotCreate,
        Create,
        Wait,
        OK,
    }

#if UNITY_EDITOR
    public
#else
    internal
#endif 
        class Resource
    {
        protected string mName = string.Empty;
        protected ResourceLoader mResLoader = new ResourceLoader();
        private Action<string, WWW> mWWWLoadCalback = null;

        private Action<string, string> mTextLoadCalback = null;
        private Action<string, byte[]> mBytesLoadCalback = null;
        private Action<string, AudioClip> mAudioLoadCalback = null;
        private Action mSceneResourceLoadCallabck = null;
        private Action<string, AssetBundle, bool, LoadPriority> mBundleLoadCallback = null;
        private int mSize = 1000;
        static bool mStopLoad = false;

        protected bool mResOK = false;

        public bool InvalidBundle = false;
        private Action<string> ClearInvalidBundleFun = null;

        public Resource()
        {

            mResLoader.Resource = this;
        }

        public static bool StopLoad
        {
            set { mStopLoad = value; }
            get { return mStopLoad; }
        }

        public bool ResOK
        {
            get { return mResOK; }
        }


        public int Size
        {
            set { mSize = value; }
            get { return mSize; }
        }

        public ResourceLoader Loader
        {
            get { return mResLoader; }
        }

        public string Name
        {
            set { mName = value; }
            get { return mName; }
        }

        public bool DependencyResource { get; set; }

        public void ReleaseWWWCallback(Action<string, WWW> callback)
        {
            if (mWWWLoadCalback == null)
            {
                return;
            }

            mWWWLoadCalback -= callback;
        }
        public void AddGotWWWCallback(Action<string, WWW> callback)
        {
            if (callback != null)
            {
                mWWWLoadCalback -= callback;
                mWWWLoadCalback += callback;
            }
        }

        public void AddGotTextCallback(Action<string, string> callback)
        {
            if (callback != null)
            {
                mResLoader.IsRawResource = true;
                mTextLoadCalback -= callback;
                mTextLoadCalback += callback;
            }
        }

        public void AddGotBytesCallback(Action<string, byte[]> callback)
        {
            if (callback != null)
            {
                mResLoader.IsRawResource = true;
                mBytesLoadCalback -= callback;
                mBytesLoadCalback += callback;
            }
        }


        public void AddGotAudioCallback(Action<string, AudioClip> callback)
        {
            if (callback != null)
            {
                mResLoader.IsRawResource = true;
                mAudioLoadCalback -= callback;
                mAudioLoadCalback += callback;
            }
        }

        public void AddGotSceneCallback(Action callback)
        {
            mSceneResourceLoadCallabck -= callback;
            mSceneResourceLoadCallabck += callback;
        }



        public void AddGotBundleCallback(Action<string, AssetBundle, bool, LoadPriority> callback)
        {
            mBundleLoadCallback -= callback;
            mBundleLoadCallback += callback;
        }

        public void ReleaseBundleCallback(Action<string, AssetBundle, bool, LoadPriority> callback)
        {
            if (mBundleLoadCallback != null)
            {
                mBundleLoadCallback -= callback;
            }
        }

        public void LoadRes()
        {
            if (mResLoader.State == LoaderState.None)
            {
                if (!DependencyResource)
                    mResLoader.IsRawResource = true;
                mResLoader.Name = mName;
                ResourceMgr.Instance().GOELoaderMgr.AddLoader(mResLoader);
            }
        }

        public void OnLoadRes()
        {
            mResOK = true;

            if (InvalidBundle)
            {
                Loader.AssetBundle.Unload(true);
                Destroy();
                InvalidBundle = false;
                if (ClearInvalidBundleFun != null)
                {
                    ClearInvalidBundleFun(mName);
                }

                return;
            }

            if (mTextLoadCalback != null)
            {
                mTextLoadCalback(mName, mResLoader.Text);
                mTextLoadCalback = null;
            }

            if (mBytesLoadCalback != null)
            {
                mBytesLoadCalback(mName, mResLoader.Byte);
                mBytesLoadCalback = null;
            }

            if (mAudioLoadCalback != null)
            {
                mAudioLoadCalback(mName, mResLoader.AudioClip);
                mAudioLoadCalback = null;
            }

            if (mBundleLoadCallback != null)
            {
                mBundleLoadCallback(mName, mResLoader.AssetBundle, DependencyResource, mResLoader.Priority);
                mBundleLoadCallback = null;
            }

            if (mSceneResourceLoadCallabck != null)
            {
                mSceneResourceLoadCallabck();
                mSceneResourceLoadCallabck = null;
            }

            if (mWWWLoadCalback != null)
            {
                mWWWLoadCalback(mName, mResLoader.WWW);
                mWWWLoadCalback = null;
            }
        }

        public void Destroy()
        {
            mResLoader.Destroy();
            if (mTextLoadCalback != null)
            {
                mTextLoadCalback = null;
            }

            if (mBytesLoadCalback != null)
            {
                mBytesLoadCalback = null;
            }

            if (mAudioLoadCalback != null)
            {
                mAudioLoadCalback = null;
            }

            if (mBundleLoadCallback != null)
            {
                mBundleLoadCallback = null;
            }

            if (mSceneResourceLoadCallabck != null)
            {
                mSceneResourceLoadCallabck = null;
            }

            if (mWWWLoadCalback != null)
            {
                mWWWLoadCalback = null;
            }
            mResLoader = null;
        }

        public void SetInvalidState(bool state, Action<string> clearResFun)
        {
            this.InvalidBundle = state;
            ClearInvalidBundleFun -= clearResFun;
            ClearInvalidBundleFun += clearResFun;
        }
    }
}
