/*********************************************************************
    created:  2018-11-19 20:58:55
	filename: BundleIndicesMapAdapter.cs
    author:	  songguangze@outlook.com

    purpose:  Bundle信息适配器，用于适配原始Bundle信息到运行时
*********************************************************************/
using System;
using System.Collections.Generic;
using UnityEngine;

namespace EngineCore
{
    public class BundleIndicesMapAdapter
    {
        //Bundle的后缀
        private const string BUNDLE_POSTFIX = ".bundle";

        private BundleIndicesMap m_bundlemapHolder = null;

        /// <summary>
        /// Bundle信息
        /// </summary>
        private Dictionary<int, RuntimeBundleInfo> m_cachedRuntimeBundleInfoDict = new Dictionary<int, RuntimeBundleInfo>();

        public BundleIndicesMapAdapter(BundleIndicesMap bundlemap)
        {
            this.m_bundlemapHolder = bundlemap;
        }


        /// <summary>
        /// 获取资源所属Bundle信息
        /// </summary>
        /// <param name="assetName">Asset名</param>
        /// <param name="isRegisterAssetIndex">是否注册Asset索引信息</param>
        /// <returns></returns>
        public RuntimeBundleInfo GetBundleIndexItemInfo(string assetName, bool isRegisterAssetIndex = true)
        {
            int assetNameHash = assetName.GetHashCode();

            RuntimeBundleInfo assetBelongBundleItemInfo = null;

            if (!this.m_cachedRuntimeBundleInfoDict.TryGetValue(assetNameHash, out assetBelongBundleItemInfo))
            {
                BundleIndexItemInfo rawAssetBelongBundleItemInfo = this.m_bundlemapHolder.GetBundleItemInfoByAssetName(assetName);
                if (rawAssetBelongBundleItemInfo == null)
                    Debug.LogError($"can't find {assetName} belong bundle");
                else
                {
                    assetBelongBundleItemInfo = RuntimeBundleInfo.AdapterToRuntimeBundleInfo(rawAssetBelongBundleItemInfo);

                    try
                    {
                        //注册Bundle名，防止多次访问
                        if (!assetName.EndsWithFast(BUNDLE_POSTFIX))
                            this.m_cachedRuntimeBundleInfoDict.Add(assetBelongBundleItemInfo.BundleName.GetHashCode(), assetBelongBundleItemInfo);

                        this.m_cachedRuntimeBundleInfoDict.Add(assetNameHash, assetBelongBundleItemInfo);

                        if (isRegisterAssetIndex)
                        {
                            foreach (string assetInBundle in assetBelongBundleItemInfo.BundleAssetsArray)
                            {
                                int assetInBundleHash = assetInBundle.GetHashCode();
                                if (assetInBundleHash != assetNameHash)
                                    this.m_cachedRuntimeBundleInfoDict.Add(assetInBundleHash, assetBelongBundleItemInfo);
                            }
                        }
                    }
                    catch (ArgumentException ex)
                    {
                        Debug.LogWarning($"dulpicate asset {assetName}");
                    }
                }
            }

            return assetBelongBundleItemInfo;
        }
    }
}