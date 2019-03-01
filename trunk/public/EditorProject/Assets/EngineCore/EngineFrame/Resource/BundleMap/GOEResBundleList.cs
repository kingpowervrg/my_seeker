using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EngineCore
{
#if UNITY_EDITOR
    public
#else
    internal
#endif
 class ResBundleList : ResComponent
    {
        private List<BundleInfo> mListBundle = new List<BundleInfo>();
        private Dictionary<int, BundleInfo> mDicBundle = new Dictionary<int, BundleInfo>();

        internal void RegisterBundleIdx(string asset, string bundleName, int size = 0)
        {
            BundleInfo bundle = this.GetBundle(bundleName);
            if (bundle == null)
            {
                bundle = new BundleInfo();
                mListBundle.Add(bundle);
                mDicBundle.Add(bundleName.GetHashCode(), bundle);
                bundle.mName = bundleName;
                bundle.Size = size;
            }

            bundle.AddFile(asset);
        }

        internal void UnRegisterByBundleName(string bundleName)
        {
            int hashCode = bundleName.GetHashCode();
            for (int i = 0; i < mListBundle.Count; ++i)
            {
                if (mListBundle[i].mName == bundleName)
                {
                    mListBundle.RemoveAt(i);
                    break;
                }
            }

            mDicBundle.Remove(hashCode);
        }

        internal BundleInfo GetBundle(string name)
        {
            BundleInfo bundle = null;
            if (mDicBundle.TryGetValue(name.GetHashCode(), out bundle))
            {
                return bundle;
            }

            return null;
        }

        internal List<BundleInfo> GetBundleByBeginName(string beginName)
        {
            List<BundleInfo> list = new List<BundleInfo>();
            foreach (BundleInfo bundle in mListBundle)
            {
                if (bundle.mName.IndexOf(beginName) == 0)
                {
                    list.Add(bundle);
                }
            }

            return list;
        }

        internal int GetCount()
        {
            return mListBundle.Count;
        }

        internal BundleInfo GetBundleByIndex(int index)
        {
            if (index < 0 || index >= mListBundle.Count)
            {
                return null;
            }

            return mListBundle[index];
        }
    }
}
