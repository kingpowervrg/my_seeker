/********************************************************************
	created:  2018-12-24 0:26:33
	filename: PathResolver.cs
	author:	  songguangze@outlook.com
	
	purpose:  各种平台下资源路径获取
*********************************************************************/

using System.IO;
using UnityEngine;

namespace EngineCore
{
    public abstract class PathResolver
    {
        /// <summary>
        /// PersistentDataPath
        /// </summary>
        public static string ApplicationPersistentDataPath = Application.persistentDataPath;

        /// <summary>
        /// StreamingAssetsPath
        /// </summary>
        public static string ApplicationStreamingAssetsPath = Application.streamingAssetsPath;

        /// <summary>
        /// DataPath
        /// </summary>
        public static string ApplicationDataPath = Application.dataPath;

        /// <summary>
        /// 获取具体的资源路径
        /// </summary>
        /// <param name="assetName"></param>
        /// <param name="isWWW"></param>
        /// <returns></returns>
        public abstract string GetPath(string assetName, bool isWWW);

        /// <summary>
        /// 获取动态资源的路径
        /// </summary>
        /// <param name="assetName"></param>
        /// <param name="isWWW"></param>
        /// <returns></returns>
        protected string GetDynamicPath(string assetName)
        {
            string assetDynamicPath = ApplicationPersistentDataPath + "/" + assetName;

            return assetDynamicPath;
        }

        /// <summary>
        /// 获取内置资源路径
        /// </summary>
        /// <param name="assetName"></param>
        /// <param name="isWWW"></param>
        /// <returns></returns>
        protected abstract string GetBuildinAssetPath(string assetName);

        protected abstract string SetToWWWPath(string path);

        /// <summary>
        /// 获取资源在Resources中的路径
        /// </summary>
        /// <param name="assetRelativePath"></param>
        /// <returns></returns>
        public string GetPathInResources(string assetRelativePath)
        {
            return Path.GetFileNameWithoutExtension(assetRelativePath);
        }
    }
}