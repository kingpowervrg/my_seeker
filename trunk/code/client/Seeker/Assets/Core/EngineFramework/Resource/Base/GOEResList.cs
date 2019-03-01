/*
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace EngineCore
{
#if UNITY_EDITOR
    public
#else
    internal
#endif
    class ResList : ResComponent
    {
        protected List<string> mListRes = new List<string>();
        protected List<string> mListAsset = new List<string>();

        internal void AddBundle(string name, string asset = null)
        {
            foreach (string res in mListRes)
            {
                if (res == name)
                {
                    return;
                }
            }

            if (name.Length == 0)
            {
                Debug.LogError(" ResList:AddBundle name can't empty");
                return;
            }
            BundleInfo bundle = ResourceMgr.Instance.GetBundle(name);
            if (bundle == null)
                Debug.LogError("Cannot find bundle " + name);
            mListRes.Add(name);

            if (asset == null)
            {
                asset = bundle.Files.ElementAt(bundle.Files.Count - 1);
            }
            mListAsset.Add(asset);
            foreach (var i in bundle.DependsOn)
            {
                BundleInfo dep = ResourceMgr.Instance.GetBundle(i);
                mListRes.Add(i);
                mListAsset.Add(dep.FirstAsset);
            }
        }

        internal void AddWWW(string name)
        {
            foreach (string res in mListRes)
            {
                if (res == name)
                {
                    return;
                }
            }

            if (name.Length == 0)
            {
                return;
            }

            mListRes.Add(name);
            BundleIndexItemInfo bundle = ResourceMgr.Instance.BundleMapAdapter.GetBundleIndexItemInfo(name);
            if (bundle == null)
                Debug.LogError("Cannot find bundle " + name);
            mListAsset.Add(bundle.BundleName);
        }


        internal void AddAsset(string name)
        {
            string bundleName = ResourceMgr.Instance.GetBundleName(name);
            if (bundleName != string.Empty)
            {
                this.AddBundle(bundleName, name);
            }
        }
    }
}
*/