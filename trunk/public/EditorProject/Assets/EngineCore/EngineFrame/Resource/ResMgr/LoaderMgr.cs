using System.Collections.Generic;

namespace EngineCore
{
#if UNITY_EDITOR
    public
#else
    internal
#endif
        class GOELoaderMgr : ResComponent
    {
        private List<ResourceLoader> mListLoader = new List<ResourceLoader>();
        private List<ResourceLoader> mDownLoader = new List<ResourceLoader>();

        private int mMaxLoader = 2;
        private bool mSortDirty = false;

        internal bool IsFree
        {
            get { return mDownLoader.Count == 0 && mListLoader.Count == 0; }
        }

        internal int GetDownloadLoaderCount()
        {
            return mDownLoader.Count;
        }

        internal int GetWaitForLoaderCount()
        {
            return mListLoader.Count;
        }

        internal ResourceLoader GetDownloadLoaderByIndex(int index)
        {
            if (index < 0 || index >= mDownLoader.Count)
            {
                return null;
            }

            return mDownLoader[index];

        }

        internal ResourceLoader GetWaitForLoaderByIndex(int index)
        {
            if (index < 0 || index >= mListLoader.Count)
            {
                return null;
            }

            return mListLoader[index];
        }

        internal void SetLoaderPriority(ResourceLoader loader, LoadPriority priority)
        {
            if (loader.Priority == priority)
            {
                return;
            }

            loader.Priority = priority;
            mSortDirty = true;
        }

        internal int MaxLoader
        {
            set { mMaxLoader = value; }
        }

        internal bool AddLoader(ResourceLoader loader)
        {
            loader.State = LoaderState.Wait;
            mListLoader.Add(loader);
            mSortDirty = true;

            return true;
        }

        internal override void Update()
        {
            this.UpdateNormalLoader();
        }

        private void UpdateNormalLoader()
        {
            int loadingCnt = 0;
            for (int i = 0; i < mDownLoader.Count;)
            {
                ResourceLoader loader = mDownLoader[i];
                if (loader.IsDestroyed)
                {
                    mDownLoader.RemoveAt(i);
                    continue;
                }
                loader.Update();
                if (loader.IsLoading)
                    loadingCnt++;
                if (loader.IsDone)
                {
                    loader.State = LoaderState.Over;
                    loader.Resource.OnLoadRes();
                    mDownLoader.RemoveAt(i);
                    //break;
                }
                else
                {
                    ++i;
                }
            }

            if (Resource.StopLoad)
            {
                return;
            }

            if (loadingCnt < mMaxLoader && mListLoader.Count > 0)
            {
                this.SortLoader();

                for (int i = 0; i < (mMaxLoader - loadingCnt) && mListLoader.Count > 0; ++i)
                {
                    ResourceLoader resloader = mListLoader[0];
                    mListLoader.RemoveAt(0);

                    if (!resloader.IsDestroyed)
                    {
                        mDownLoader.Add(resloader);
                        resloader.Update();
                        resloader.State = LoaderState.Loading;
                        //break;
                    }
                }
            }
        }

        private void SortLoader()
        {
            if (mSortDirty)
            {
                mSortDirty = false;
                mListLoader.Sort((a, b) => b.Priority - a.Priority);
            }
        }

        private LoadPriority GetDownloadLoaderLowestPrioriy()
        {
            LoadPriority priority = LoadPriority.Default;
            foreach (ResourceLoader loader in mDownLoader)
            {
                if (loader.Priority < priority)
                {
                    priority = loader.Priority;
                }
            }

            return priority;
        }

        private void SortLoader(List<ResourceLoader> listLoader)
        {
            listLoader.Sort(this.SortLoaderByPriority);
        }

        private int SortLoaderByPriority(ResourceLoader l, ResourceLoader r)
        {
            if (r.Priority > l.Priority)
            {
                return 1;
            }
            else if (r.Priority == l.Priority)
            {
                return 0;
            }
            else
            {
                return -1;
            }
        }
    }
}
