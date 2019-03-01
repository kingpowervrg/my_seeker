using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace EngineCore
{
#if UNITY_EDITOR
    public
#else
    internal
#endif
        class LoadingProgress : ResList
    {
        private int mTotalSize = 0;
        protected float mProgerss = 0;
        protected bool mDone = false;
        internal Action<float> ProgressHandler;
        internal Action OnEnd;
        Dictionary<string, float> progressMap = new Dictionary<string, float>();
        internal void Clear()
        {
            mListRes.Clear();
            mListAsset.Clear();
            mTotalSize = 0;
            mDone = false;
            ProgressHandler = null;
            progressMap.Clear();
            OnEnd = null;
        }

        internal override void Start()
        {
            foreach (string name in mListRes)
            {
                BundleInfo bundle = ResourceMgr.Instance().GetBundle(name);
                if (bundle == null)
                {
                    //Logger.GetFile( LogFile.Res ).LogError( name + " not find in FireBundle" );
                }
                else
                {
                    mTotalSize += bundle.Size;
                }
                progressMap[name] = 0;
            }
            if (mListRes.Count == 0)
            {
                //Logger.GetFile( LogFile.Res ).LogError(" mListRes.Count == 0" );
            }
        }

        void UpdateProgress(string name, float progress)
        {
            float old = progressMap[name];
            if (progress >= old)
            {
                progressMap[name] = progress;
            }
        }

        internal override void Update()
        {
            base.Update();
            if (mListRes.Count == 0)
                return;
            if (mDone)
            {
                Action endf = OnEnd;
                Clear();
                if (endf != null)
                    endf();
            }
            int downloadsize = 0;
            foreach (string name in mListRes)
            {
                BundleInfo bundle = ResourceMgr.Instance().GetBundle(name);
                if (bundle == null)
                {
                    continue;
                }

                if (this.IsBundleCached(name))
                {
                    downloadsize += bundle.Size;
                    UpdateProgress(name, 1f);
                }
                else
                {
                    Resource res = this.GetRes(name);
                    if (res != null)
                    {
                        float prog = res.Loader.Progress;
                        UpdateProgress(name, prog);
                        downloadsize += (int)((prog) * bundle.Size);
                    }
                    else
                    {
                        float prog;
                        if (progressMap.TryGetValue(name, out prog))
                            downloadsize += (int)((prog) * bundle.Size);
                    }

                }
            }
            if (mTotalSize == 0)
            {
                mProgerss = 0;
                mDone = true;
            }
            else if (downloadsize == mTotalSize)
            {
                mProgerss = 1.0f;
                mDone = true;
            }
            else
            {
                mProgerss = downloadsize * 1.0f / mTotalSize;
            }
            if (ProgressHandler != null)
            {
                ProgressHandler(mProgerss);
            }
        }

        protected Resource GetRes(string name)
        {
            return ResourceMgr.Instance().GetDownloadResource(name);
        }

        private bool IsBundleCached(string name)
        {
            return ResourceMgr.Instance().IsBundleCached(name);
        }


    }
}
