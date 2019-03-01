/********************************************************************
	created:  2018-3-28 15:27:53
	filename: PackHandlerBase.cs
	author:	  songguangze@outlook.com
	
	purpose:  分包处理基类
*********************************************************************/
using GOEditor;
using System;
using System.Collections.Generic;
using System.IO;

namespace EngineCore.Editor
{
    public abstract class PackHandlerBase
    {
        //AssetBundle 后缀
        public const string AssetBundle_PostFix = ".bundle";

        protected GOEPackV5 m_packSetting = null;
        protected PackBundleSetting m_packBundleSetting = null;

        public PackHandlerBase(GOEPackV5 packSetting, PackBundleSetting packBundleSetting)
        {
            this.m_packSetting = packSetting;
            this.m_packBundleSetting = packBundleSetting;
        }


        /// <summary>
        /// 由分包方式获取打包文件列表
        /// </summary>
        /// <param name="packSetting"></param>
        /// <returns></returns>
        public abstract Dictionary<string, List<string>> GetAssetsDictByPackSetting();

        /// <summary>
        /// 根据打包选项获取需要搜索的文件夹
        /// </summary>
        /// <returns></returns>
        protected List<string> GetPackDirectories()
        {
            string rootDirectory = m_packSetting.SrcDir;
            string childDirctory = m_packBundleSetting.SubFolder;
            string searchDirectory = string.IsNullOrEmpty(childDirctory) ? rootDirectory : Path.Combine(rootDirectory, childDirctory);

            List<string> directories = new List<string>();
            if (m_packBundleSetting.SearchSubDir)
                directories = EngineFileUtil.SearchDirectoryRecursively(searchDirectory);
            else
                directories.Add(searchDirectory);

            return directories;
        }

        /// <summary>
        /// 获取路径下符合打包规定的文件列表
        /// </summary>
        /// <param name="searchDirectory"></param>
        /// <returns></returns>
        protected List<string> GetDirectoryFileListWithSearchOption(string searchDirectory)
        {
            List<string> fileList = new List<string>();

            List<string> directoryAllFiles = new List<string>();
            string[] searchPatterns = m_packBundleSetting.SearchFilters.Split('|');

            foreach (string searchPattern in searchPatterns)
            {
                string[] fileWithSearchPattern = Directory.GetFiles(searchDirectory, searchPattern, SearchOption.TopDirectoryOnly);
                directoryAllFiles.AddRange(fileWithSearchPattern);
            }


            foreach (string fileInDirectory in directoryAllFiles)
            {
                if (!IsMatchNecessary(fileInDirectory))
                    continue;

                if (!IsMatchSize(fileInDirectory))
                    continue;

                if (!IsMatchExclude(fileInDirectory))
                    continue;

                fileList.Add(fileInDirectory.Replace('\\', '/'));
            }

            return fileList;
        }

        /// <summary>
        /// 是否满足必须串
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        private bool IsMatchNecessary(string str)
        {
            if (string.IsNullOrEmpty(m_packBundleSetting.NecessaryFilters))
                return true;

            string[] nessaryFilters = m_packBundleSetting.NecessaryFilters.Split('|');

            foreach (string filter in nessaryFilters)
            {
                string necessaryFilter = filter;
                if (necessaryFilter.Contains("*."))
                    necessaryFilter = necessaryFilter.Substring(1);

                if (str.EndsWith(necessaryFilter))
                    return true;
            }

            return false;
        }

        /// <summary>
        /// 是否满足排除串 
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        private bool IsMatchExclude(string str)
        {
            if (string.IsNullOrEmpty(m_packBundleSetting.NecessaryFilters))
                return true;

            string[] excludeFilters = m_packBundleSetting.ExcludeFilters.Split('|');
            foreach (string filter in excludeFilters)
            {
                string excludeFilter = filter;
                if (excludeFilter.Contains("*."))
                    excludeFilter = excludeFilter.Substring(1);

                if (str.Contains(excludeFilter))
                    return false;
            }

            return true;
        }

        /// <summary>
        /// 是否满足文件大小
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        private bool IsMatchSize(string str)
        {
            System.IO.FileInfo fi = new FileInfo(str);
            return fi.Length > this.m_packBundleSetting.MinSize && fi.Length < this.m_packBundleSetting.MaxSize;
        }

        /// <summary>
        /// 构建AssetBundle名
        /// </summary>
        /// <param name="customName"></param>
        /// <returns></returns>
        protected string MakeAssetbundleName(string customName)
        {
            string assetBundleName = customName;
            if (!string.IsNullOrEmpty(m_packBundleSetting.BundleName))
                assetBundleName = (m_packBundleSetting.BundleName + "_" + customName);

            assetBundleName = assetBundleName.Replace("\\", "/").Replace("/", "_").ToLower() + AssetBundle_PostFix;

            return assetBundleName;
        }


    }

    /// <summary>
    /// 分包类型
    /// </summary>
    public class PackTypeAttribute : Attribute
    {
        private string m_packTypeName = string.Empty;

        public PackTypeAttribute(string packTypeName)
        {
            this.m_packTypeName = packTypeName;
        }

        public string PackTypeName
        {
            get { return m_packTypeName; }
        }
    }
}
