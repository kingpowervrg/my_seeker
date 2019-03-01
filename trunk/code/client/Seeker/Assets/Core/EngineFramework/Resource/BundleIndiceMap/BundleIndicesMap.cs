using SQLite4Unity3d;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;

namespace EngineCore
{
    public class BundleIndicesMap : IDisposable
    {
        private SQLiteConnection m_bundleIndicesmapConnection = null;


        public BundleIndicesMap(string bundleIndicesMapDbPath)
        {
            try
            {
                this.m_bundleIndicesmapConnection = new SQLiteConnection(bundleIndicesMapDbPath);
            }
            catch (Exception ex)
            {
                Debug.LogError(ex.Message);
            }
        }

        public BundleIndicesMap(SQLiteConnection dbConnection)
        {
            this.m_bundleIndicesmapConnection = dbConnection;
        }

        /// <summary>
        /// 添加Bundle信息
        /// </summary>
        /// <param name="bundleName"></param>
        /// <param name="bundleHash"></param>
        /// <param name="dependency"></param>
        /// <param name="bundleAssets"></param>
        public void AddBundleIndices(string bundleName, string bundleHash, string[] dependency, string[] bundleAssets, int bundleSize)
        {
            BundleIndexItemInfo existBundleIndexItemInfo = GetBundleIndexItemInfo(bundleName);

            if (existBundleIndexItemInfo != null)
            {
                if (existBundleIndexItemInfo.BundleHash == bundleHash)
                    return;
            }
            else
                existBundleIndexItemInfo = new BundleIndexItemInfo();

            existBundleIndexItemInfo.BundleName = bundleName;
            existBundleIndexItemInfo.BundleAssetsArray = bundleAssets;
            existBundleIndexItemInfo.BundleDependencyArray = dependency;
            existBundleIndexItemInfo.BundleHash = bundleHash;
            existBundleIndexItemInfo.BundleSize = bundleSize;

            AddOrUpdateBundleIndex(existBundleIndexItemInfo);

            DeleteBundleAssets(bundleName);
            BundleAssetItem[] bundleAssetIndex = new BundleAssetItem[bundleAssets.Length + 1];
            for (int i = 0; i < bundleAssets.Length; ++i)
                bundleAssetIndex[i] = new BundleAssetItem()
                {
                    AssetBelongsBundle = bundleName,
                    AssetName = bundleAssets[i]
                };

            bundleAssetIndex[bundleAssets.Length] = new BundleAssetItem() { AssetName = bundleName, AssetBelongsBundle = bundleName };

            AddBundleAssetsByTransaction(bundleAssetIndex.ToList());
        }

        public void AddBundleIndicesByTransaction(string bundleName, string bundleHash, string[] dependency, string[] bundleAssets, int bundleSize)
        {
            if (!this.m_bundleIndicesmapConnection.IsInTransaction)
                this.m_bundleIndicesmapConnection.BeginTransaction();

            AddBundleIndices(bundleName, bundleHash, dependency, bundleAssets, bundleSize);
        }

        private void AddOrUpdateBundleIndex(BundleIndexItemInfo bundleIndexMap)
        {
            //this.m_bundleIndicesmapConnection.BeginTransaction();
            this.m_bundleIndicesmapConnection.Delete<BundleIndexItemInfo>(bundleIndexMap.BundleName);
            this.m_bundleIndicesmapConnection.Insert(bundleIndexMap);
            //this.m_bundleIndicesmapConnection.Commit();
            //return this.m_bundleIndicesmapConnection.InsertOrReplace(bundleIndexMap);
        }


        public void RemoveBundleIndices(string bundleName)
        {
            this.m_bundleIndicesmapConnection.Delete<BundleIndexItemInfo>(bundleName);
            DeleteBundleAssets(bundleName);
        }

        public void RemoveBundleIndices(BundleIndexItemInfo removeBundleItemInfo)
        {
            this.m_bundleIndicesmapConnection.Delete(removeBundleItemInfo);
            DeleteBundleAssets(removeBundleItemInfo.BundleName);
        }

        public int RemoveAsset(string assetName)
        {
            return this.m_bundleIndicesmapConnection.Delete<BundleAssetItem>(assetName);
        }

        public BundleIndexItemInfo GetBundleIndexItemInfo(string bundleName)
        {
            CreateTableIfNowExist<BundleIndexItemInfo>();

            return this.m_bundleIndicesmapConnection.Find<BundleIndexItemInfo>(bundleName);
        }

        /// <summary>
        /// 通过bundleHash获取Bundle名
        /// </summary>
        /// <param name="bundleHashName"></param>
        /// <returns></returns>
        public BundleIndexItemInfo GetBundleInfoByHashName(string bundleHashName)
        {
            string queryStr = "SELECT * FROM tb_bundlemap WHERE BundleHash = ?";
            string bundleHashKey = Path.GetFileNameWithoutExtension(bundleHashName);
            return this.m_bundleIndicesmapConnection.Query<BundleIndexItemInfo>(queryStr, bundleHashKey).FirstOrDefault();
        }

        public List<BundleIndexItemInfo> GetAllBundleIndexItemList()
        {
            string queryStr = "SELECT * FROM tb_bundlemap";
            List<BundleIndexItemInfo> allBundleItemList = this.m_bundleIndicesmapConnection.Query<BundleIndexItemInfo>(queryStr);
            return allBundleItemList;
        }

        /// <summary>
        /// 获取Bundle包含的资源列表
        /// </summary>
        /// <param name="bundleName"></param>
        /// <returns></returns>
        public List<BundleAssetItem> GetBelongsBundleAssetsList(string bundleName)
        {
            string strQueryBundleAssets = $"SELECT * FROM tb_assetsIndex WHERER AssetBelongsBundle='{bundleName}'";
            return this.m_bundleIndicesmapConnection.Query<BundleAssetItem>(strQueryBundleAssets);
        }

        /// <summary>
        /// Bundlemap比对
        /// </summary>
        /// <param name="dstDbConnection"></param>
        /// <returns></returns>
        public BundleMapDiffResult Diff(SQLiteConnection dstDbConnection, bool closeDstDbConnection = true)
        {
            BundleMapDiffResult diffResult = new BundleMapDiffResult();
            //diffResult.AppendBundleIndexList = new List<BundleIndexItemInfo>();
            diffResult.DirtyBundleIndexList = new List<BundleIndexItemInfo>();

            Dictionary<string, BundleIndexItemInfo> currentAllBundleIndexDict = GetAllBundleIndexItemList().ToDictionary(bundleMapInfo => bundleMapInfo.BundleName);

            BundleIndicesMap dstBundleIndicesMap = new BundleIndicesMap(dstDbConnection);
            List<BundleIndexItemInfo> dstAllBundleIndexList = dstBundleIndicesMap.GetAllBundleIndexItemList();

            for (int i = 0; i < dstAllBundleIndexList.Count; ++i)
            {
                BundleIndexItemInfo dstBundleIndexItem = dstAllBundleIndexList[i];
                string dstBundleHash = dstBundleIndexItem.BundleHash;
                string dstBundleName = dstBundleIndexItem.BundleName;

                BundleIndexItemInfo existBundleIndexItem;
                if (currentAllBundleIndexDict.TryGetValue(dstBundleName, out existBundleIndexItem))
                {
                    if (existBundleIndexItem.BundleHash != dstBundleHash)
                        diffResult.DirtyBundleIndexList.Add(dstBundleIndexItem);

                    currentAllBundleIndexDict.Remove(dstBundleName);
                }
                else
                {
                    diffResult.AddAppendBundleItemInfo(dstBundleIndexItem);
                }
            }

            List<BundleIndexItemInfo> obsoletedBundleList = currentAllBundleIndexDict.Values.ToList();
            diffResult.ObsoleteBundleIndexList = obsoletedBundleList;

            if (closeDstDbConnection)
                dstDbConnection.Dispose();

            return diffResult;
        }

        /// <summary>
        /// 对比两个Bundlemap的不同
        /// </summary>
        /// <param name="otherBundleIndicesMap"></param>
        /// <returns></returns>
        public BundleMapDiffResult Diff(BundleIndicesMap otherBundleIndicesMap)
        {
            return Diff(otherBundleIndicesMap.BundleIndicesMapConn, false);
        }

        /// <summary>
        /// 删除Bundle包含的资源列表
        /// </summary>
        /// <param name="bundleName"></param>
        /// <returns></returns>
        private int DeleteBundleAssets(string bundleName)
        {
            string strDeleteBundleAssets = $"DELETE FROM tb_assetsIndex WHERE AssetBelongsBundle='{bundleName}'";
            return this.m_bundleIndicesmapConnection.Execute(strDeleteBundleAssets);
        }

        /// <summary>
        /// 添加资源索引
        /// </summary>
        /// <param name="bundleAssetsList"></param>
        /// <returns></returns>
        private int AddBundleAssets(List<BundleAssetItem> bundleAssetsList)
        {
            try
            {
                //this.m_bundleIndicesmapConnection.BeginTransaction();
                CreateTableIfNowExist<BundleAssetItem>();

                int affectRows = this.m_bundleIndicesmapConnection.InsertAll(bundleAssetsList);
                //this.m_bundleIndicesmapConnection.Commit();
                return affectRows;
            }
            catch (Exception ex)
            {
                StringBuilder sbError = new StringBuilder("重复资源,");
                for (int i = 0; i < bundleAssetsList.Count; ++i)
                    sbError.AppendFormat("assetName:{0},bundleName:{1}\n", bundleAssetsList[i].AssetName, bundleAssetsList[i].AssetBelongsBundle);

                Debug.LogError(sbError.ToString());
            }
            return 0;
        }

        private void AddBundleAssetsByTransaction(List<BundleAssetItem> bundleAssetsList)
        {
            CreateTableIfNowExist<BundleAssetItem>();
            for (int i = 0; i < bundleAssetsList.Count; ++i)
                this.m_bundleIndicesmapConnection.Insert(bundleAssetsList[i]);
        }

        /// <summary>
        /// 批量删除Bundle列表
        /// </summary>
        /// <param name="deleteBundleIndexItemList"></param>
        public void DeleteBundleList(List<BundleIndexItemInfo> deleteBundleIndexItemList)
        {
            this.BundleIndicesMapConn.BeginTransaction();
            for (int i = 0; i < deleteBundleIndexItemList.Count; ++i)
                RemoveBundleIndices(deleteBundleIndexItemList[i]);

            this.m_bundleIndicesmapConnection.Commit();
        }

        /// <summary>
        /// 获取Bundle数量
        /// </summary>
        /// <returns></returns>
        public int GetBundleCount()
        {
            string queryStr = "SELECT COUNT(*) FROM tb_bundlemap";
            return this.m_bundleIndicesmapConnection.ExecuteScalar<int>(queryStr);
        }

        /// <summary>
        /// 获取指定Bundle有关联的Bundle列表
        /// </summary>
        /// <param name="bundleNameList"></param>
        /// <returns></returns>
        public List<BundleIndexItemInfo> GetAssociateBundleIndexList(List<string> bundleNameList)
        {
            List<BundleIndexItemInfo> assolitedBundleList = new List<BundleIndexItemInfo>();

            for (int i = 0; i < bundleNameList.Count; ++i)
            {
                GetBundleAssociateBundleList(bundleNameList[i], assolitedBundleList);
            }

            return assolitedBundleList;
        }

        private void GetBundleAssociateBundleList(string bundleName, List<BundleIndexItemInfo> associateBundleList)
        {
            BundleIndexItemInfo bundleInfo = GetBundleIndexItemInfo(bundleName);
            if (bundleInfo == null)
                return;

            associateBundleList.Add(bundleInfo);

            for (int i = 0; i < bundleInfo.BundleDependencyArray.Length; ++i)
                GetBundleAssociateBundleList(bundleInfo.BundleDependencyArray[i], associateBundleList);
        }


        /// <summary>
        /// 获取依赖于指定Bundle被依赖Bundle列表
        /// </summary>
        /// <param name="bundleName"></param>
        /// <returns></returns>
        public List<BundleIndexItemInfo> GetDependOnBundleList(string bundleName)
        {
            string queryStr = $"SELECT * FROM tb_bundlemap WHERE BundleDependency LIKE '%{bundleName}%'";
            List<BundleIndexItemInfo> dependOnBundleList = this.m_bundleIndicesmapConnection.Query<BundleIndexItemInfo>(queryStr);

            return dependOnBundleList;
        }

        /// <summary>
        /// 通过Assets名获取Bundle名
        /// </summary>
        /// <param name="assetName"></param>
        /// <returns></returns>
        public BundleIndexItemInfo GetBundleItemInfoByAssetName(string assetName)
        {
            string strQuery = "select * from tb_bundlemap where BundleName = (select AssetBelongsBundle from tb_assetsIndex where AssetName=?)";

            List<BundleIndexItemInfo> bundleIndexItemInfoList = this.m_bundleIndicesmapConnection.Query<BundleIndexItemInfo>(strQuery, assetName);
            return bundleIndexItemInfoList.Count > 0 ? bundleIndexItemInfoList[0] : null;
        }

        /// <summary>
        /// 通过asset名获取所属于的bundle信息列表
        /// </summary>
        /// <param name="assetsList"></param>
        /// <returns></returns>
        public List<BundleIndexItemInfo> GetBundleItemInfoListByAssetsList(List<string> assetsList)
        {
            StringBuilder queryBuilder = new StringBuilder("SELECT * FROM tb_bundlemap WHERE BundleName in (SELECT DISTINCT(AssetBelongsBundle) FROM tb_assetsIndex WHERE AssetName in (");
            for (int i = 0; i < assetsList.Count; ++i)
            {
                queryBuilder.AppendFormat("'{0}'", assetsList[i]);
                if (i != assetsList.Count - 1)
                    queryBuilder.Append(",");
            }
            queryBuilder.Append("))");

            List<BundleIndexItemInfo> bundleItemList = this.m_bundleIndicesmapConnection.Query<BundleIndexItemInfo>(queryBuilder.ToString());

            return bundleItemList;
        }

        /// <summary>
        /// 设置BundleMap信息
        /// </summary>
        /// <param name="infoKey"></param>
        /// <param name="value"></param>
        public void SetBundleMapInfo(BundleMapKey infoKey, string value)
        {
            CreateTableIfNowExist<BundleMapInfo>();

            BundleMapInfo info = new BundleMapInfo();
            info.BundleMapInfoKey = (int)infoKey;
            info.BundleMapInfoValue = value;

            this.m_bundleIndicesmapConnection.InsertOrReplace(info);
        }

        /// <summary>
        /// 获取BundleMap信息
        /// </summary>
        /// <param name="infoKey"></param>
        public string GetBundleMapInfo(BundleMapKey infoKey)
        {
            bool hasBundleInfoTable = CreateTableIfNowExist<BundleMapInfo>();
            if (!hasBundleInfoTable)
                return string.Empty;

            BundleMapInfo bundleMapInfo = this.m_bundleIndicesmapConnection.Find<BundleMapInfo>((int)infoKey);

            return bundleMapInfo?.BundleMapInfoValue;
        }

        /// <summary>
        /// 获取BundleMap中不包括的列表
        /// </summary>
        /// <param name="bundleNameList"></param>
        /// <returns></returns>
        public List<string> GetNotIncludeBundleList(List<string> bundleNameList)
        {
            StringBuilder sqlBuilder = new StringBuilder("SELECT * FROM tb_bundlemap WHERE BundleName in (");
            for (int i = 0; i < bundleNameList.Count; ++i)
            {
                if (i != bundleNameList.Count - 1)
                    sqlBuilder.AppendFormat("'{0}',", bundleNameList[i]);
                else
                    sqlBuilder.AppendFormat("'{0}')", bundleNameList[i]);
            }

            List<BundleIndexItemInfo> includeBundleList = this.m_bundleIndicesmapConnection.Query<BundleIndexItemInfo>(sqlBuilder.ToString());
            List<string> notIncludeList = bundleNameList.Except(includeBundleList.Select(bundleIndexItem => bundleIndexItem.BundleName)).ToList();

            return notIncludeList;
        }

        public void ClearBundleIndicesData()
        {
            this.m_bundleIndicesmapConnection.BeginTransaction();
            this.m_bundleIndicesmapConnection.CreateTable<BundleAssetItem>();
            this.m_bundleIndicesmapConnection.DeleteAll<BundleAssetItem>();
            this.m_bundleIndicesmapConnection.CreateTable<BundleIndexItemInfo>();
            this.m_bundleIndicesmapConnection.DeleteAll<BundleIndexItemInfo>();
            this.m_bundleIndicesmapConnection.CreateTable<BundleMapInfo>();
            this.m_bundleIndicesmapConnection.DeleteAll<BundleMapInfo>();
            this.m_bundleIndicesmapConnection.Commit();
        }

        private bool CreateTableIfNowExist<T>() where T : new()
        {
            TableMapping tableMapping = this.m_bundleIndicesmapConnection.GetMapping<T>();
            if (tableMapping == null)
            {
                this.m_bundleIndicesmapConnection.CreateTable<T>();
                return false;
            }

            return true;
        }

        /// <summary>
        /// 获取所有场景的Bundle索引信息
        /// </summary>
        /// <returns></returns>
        private List<BundleAssetItem> GetSceneBundleList()
        {
            string queryStr = "SELECT * FROM tb_assetsIndex WHERE AssetName LIKE '%.unity'";
            List<BundleAssetItem> sceneBundleIndexList = this.m_bundleIndicesmapConnection.Query<BundleAssetItem>(queryStr);

            return sceneBundleIndexList;
        }

        public void ForceCommit()
        {
            if (this.m_bundleIndicesmapConnection.IsInTransaction)
                this.m_bundleIndicesmapConnection.Commit();
        }


        public void Dispose()
        {
            ForceCommit();

            m_bundleIndicesmapConnection?.Dispose();
            m_bundleIndicesmapConnection = null;
        }


        public SQLiteConnection BundleIndicesMapConn => this.m_bundleIndicesmapConnection;

        /// <summary>
        /// Bundlemap比对结果
        /// </summary>
        public class BundleMapDiffResult
        {
            public List<BundleIndexItemInfo> DirtyBundleIndexList = new List<BundleIndexItemInfo>();
            public List<BundleIndexItemInfo> ObsoleteBundleIndexList = new List<BundleIndexItemInfo>();

            private List<BundleIndexItemInfo> m_appendBundleIndexItemList = new List<BundleIndexItemInfo>();
            private Dictionary<string, BundleIndexItemInfo> m_appendBundleIndexItemDict = new Dictionary<string, BundleIndexItemInfo>();

            /// <summary>
            /// 添加新增资源信息
            /// </summary>
            /// <param name="appendBundleIndexItemInfo"></param>
            public void AddAppendBundleItemInfo(BundleIndexItemInfo appendBundleIndexItemInfo)
            {
                this.m_appendBundleIndexItemList.Add(appendBundleIndexItemInfo);
                this.m_appendBundleIndexItemDict.Add(appendBundleIndexItemInfo.BundleName, appendBundleIndexItemInfo);
            }

            public void RemoveAppendBundleItemInfo(string appendBundleName)
            {
                BundleIndexItemInfo removeBundleInfo = GetAppendBundleItemInfoByBundleName(appendBundleName);
                if (removeBundleInfo != null)
                {
                    m_appendBundleIndexItemList.Remove(removeBundleInfo);
                    this.m_appendBundleIndexItemDict.Remove(appendBundleName);
                }
            }

            /// <summary>
            /// 通过bundleName获取新增Bundle信息
            /// </summary>
            /// <param name="appendBundleName"></param>
            /// <returns></returns>
            public BundleIndexItemInfo GetAppendBundleItemInfoByBundleName(string appendBundleName)
            {
                BundleIndexItemInfo bundleInfo;
                if (this.m_appendBundleIndexItemDict.TryGetValue(appendBundleName, out bundleInfo))
                    return bundleInfo;

                return null;
            }
        }

        /// <summary>
        /// BundleMap信息的Key
        /// </summary>
        public enum BundleMapKey
        {
            BUNDLEMAP_IDENTIFY,     //Bundlemap Hash值
        }
    }
}