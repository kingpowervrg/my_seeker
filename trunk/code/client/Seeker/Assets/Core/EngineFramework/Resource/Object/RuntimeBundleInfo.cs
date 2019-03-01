/*********************************************************************
    created:  2018-11-19 19:2:47
	filename: RuntimeBundleInfo.cs
    author:	  songguangze@outlook.com


    purpose:  运行时Bundle信息
*********************************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EngineCore
{
    public class RuntimeBundleInfo : BundleIndexItemInfo
    {
        public RuntimeBundleInfo(BundleIndexItemInfo baseBundleIndexItemInfo)
        {
            base.BundleAssets = baseBundleIndexItemInfo.BundleAssets;
            base.BundleDependency = baseBundleIndexItemInfo.BundleDependency;
            base.BundleHash = baseBundleIndexItemInfo.BundleHash;
            base.BundleName = baseBundleIndexItemInfo.BundleName;
            base.BundleSize = baseBundleIndexItemInfo.BundleSize;
        }

        /// <summary>
        /// Bundle是否一直持有不释放
        /// </summary>
        public bool BundlePersistent { get; set; } = false;

        /// <summary>
        /// 资源是否一直持有不释放
        /// </summary>
        public bool AssetPersistent { get; set; } = false;

        /// <summary>
        /// 是否被依赖
        /// </summary>
        public bool IsDependency { get; set; } = false;

        /// <summary>
        /// 是否可以强制释放
        /// </summary>
        public bool CanForceRelease { get; set; } = false;


        /// <summary>
        /// Adapter Raw BundleIndexItemInfo to Runtime
        /// </summary>
        /// <param name="rawBundleIndexItemInfo"></param>
        /// <returns></returns>
        public static RuntimeBundleInfo AdapterToRuntimeBundleInfo(BundleIndexItemInfo rawBundleIndexItemInfo)
        {
            RuntimeBundleInfo runtimeBundleInfo = new RuntimeBundleInfo(rawBundleIndexItemInfo);

            return runtimeBundleInfo;
        }
    }
}