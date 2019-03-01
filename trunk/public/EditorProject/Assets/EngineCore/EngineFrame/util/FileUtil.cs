/********************************************************************
	created: 2013-9-2 15:22:40
	filename:EngineFileUtil.cs
	author:	 songguangze@outlook.com
	
	purpose: 引擎文件帮助类，各种文件夹，文件操作
*********************************************************************/
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace EngineCore
{
    public class EngineFileUtil
    {
        //IndexMap分隔字符//
        public const string msIndexMapSpliter = ":";
        //扩展名//
        public const string m_fbxExt = ".fbx";
        public const string m_matExt = ".mat";
        public const string m_maxExt = ".max";
        public const string m_prefabExt = ".prefab";
        public const string m_animExt = ".anim";
        public const string m_animSrcFileContain = "@";
        public const string m_txtExt = ".txt";
        public const string m_md5Ext = ".md5.txt";
        public const string m_packExt = ".pack.txt";
        public const string m_curveAnimExt = ".ca.txt";
        public const string m_sceneDataExt = ".scene.txt";
        public const string m_unityExt = ".unity";
        public const string m_sceneExt = ".unity3d";
        public const string m_bundleExt = ".bundle";
        public const string m_bytesExt = ".bytes";
        public const string m_hmdExt = ".unity.txt";
        public const string m_reportExt = ".report.txt";
        public const string m_oggExt = ".ogg";
        public const string m_assetExt = ".asset";
        public const string m_txtidxExt = ".txt_idx";
        public const string m_idxExtPost = "_idx";

#if RES_PROJECT
        //编辑器工程用于查找模糊大小写资源缓存
        private static Dictionary<string, string> m_fuzzycaseinsensitivePathCache = new Dictionary<string, string>();
#endif

        //搜索文件。由于FirePackBundle处于Unity运行时环境中，其.NET版本不支持SearchOption，所以必须由此编辑期模块中的函数处理。//
        public static string[] GetFilesFunc(string path, string searchPattern, bool includeSubDir)
        {
            //if(!Directory.Exists(path))
            //	return new string[]{};
            return Directory.GetFiles(path, searchPattern,
                includeSubDir ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly);
        }

        //获取文件大小。由于FirePackBundle处于Unity运行时环境中，其.NET版本不支持FileInfo.Length，所以必须由此编辑期模块中的函数处理。//
        public static long GetFileLength(string path)
        {
            FileInfo fi = new FileInfo(path);
            return fi.Length;
        }

        //工具函数//
        public static void SaveText(string str, string textFilePath)
        {
            FileStream fileStream = new FileStream(textFilePath, FileMode.Create);
            StreamWriter writer = new StreamWriter(fileStream);
            writer.Write(str);
            writer.Flush();
            writer.Close();
            fileStream.Close();
        }
        public static string ReadText(string textFilePath)
        {
            if (!File.Exists(textFilePath))
                return string.Empty;
            FileStream fileStream = new FileStream(textFilePath, FileMode.Open);
            StreamReader reader = new StreamReader(fileStream);
            string buf = reader.ReadToEnd();
            reader.Close();
            fileStream.Close();
            return buf;
        }

        /// <summary>
        /// 获取根目录下面的所有子目录
        /// </summary>
        /// <param name="rootDirectory"></param>
        /// <returns></returns>
        public static List<string> SearchDirectoryRecursively(string rootDirectory, bool includeSelf = true)
        {
            List<string> directories = new List<string>();

            if (includeSelf)
                directories.Add(rootDirectory);

            string[] childDirectories = Directory.GetDirectories(rootDirectory);
            foreach (string childDirectory in childDirectories)
            {
                List<string> childrenDirectories = SearchDirectoryRecursively(childDirectory, true);
                directories.AddRange(childrenDirectories);
            }

            return directories;

        }


        public static void CopyDirectory(string sourceDirectory, string destDirectory)
        {
            //判断源目录和目标目录是否存在，如果不存在，则创建一个目录
            if (!Directory.Exists(sourceDirectory))
            {
                Directory.CreateDirectory(sourceDirectory);
            }
            if (!Directory.Exists(destDirectory))
            {
                Directory.CreateDirectory(destDirectory);
            }
            //拷贝文件
            CopyFile(sourceDirectory, destDirectory);

            //拷贝子目录       
            //获取所有子目录名称
            string[] directionName = Directory.GetDirectories(sourceDirectory);

            foreach (string directionPath in directionName)
            {
                //根据每个子目录名称生成对应的目标子目录名称
                string directionPathTemp = destDirectory + "/" + directionPath.Substring(sourceDirectory.Length + 1);

                //递归下去
                CopyDirectory(directionPath, directionPathTemp);
            }
        }
        public static void CopyFile(string sourceDirectory, string destDirectory)
        {
            //获取所有文件名称
            string[] fileName = Directory.GetFiles(sourceDirectory, "*.*", SearchOption.AllDirectories);

            if (Directory.Exists(destDirectory))
                DeleteDirectory(destDirectory);

            Directory.CreateDirectory(destDirectory);

            foreach (string filePath in fileName)
            {
                //根据每个文件名称生成对应的目标文件名称
                string filePathTemp = destDirectory + "/" + filePath.Substring(sourceDirectory.Length + 1);
                string fileDirectory = Path.GetDirectoryName(filePathTemp);

                //若不存在，直接复制文件；若存在，覆盖复制
                if (File.Exists(filePathTemp))
                    File.Copy(filePath, filePathTemp, true);
                else
                    File.Copy(filePath, filePathTemp);
            }
        }

        public static void ClearFolder(string dst)
        {
            string[] oldfiles = Directory.GetFiles(dst);
            //删除streamingAssets中旧文件
            foreach (string oldfile in oldfiles)
            {
                if (oldfile.Contains("config/") || oldfile.Contains("pax/"))
                    continue;
                if (oldfile.Contains(".mp4"))
                    continue;
                if (oldfile.Contains("config."))
                    continue;
                if (oldfile.Contains("skip.png"))
                    continue;
                if (oldfile.Contains(".txt"))
                    continue;

                if (oldfile.Contains(".dll"))
                    continue;

                File.Delete(oldfile);
            }
        }


        /// <summary>
        /// 获取文件相对路径
        /// </summary>
        /// <param name="fileFullPath"></param>
        /// <param name="relativeDirectoryName"></param>
        /// <returns></returns>
        public static string GetFileRelativePath(string fileFullPath, string relativeDirectoryName)
        {
            string[] pathSpliteByRelative = fileFullPath.Split(new string[] { relativeDirectoryName }, StringSplitOptions.RemoveEmptyEntries);

            if (pathSpliteByRelative.Length == 0)
                return string.Empty;

            string fileRelativePath = pathSpliteByRelative[0];

            //如果有后缀则是文件
            if (Path.HasExtension(fileRelativePath))
                return "./";

            return fileRelativePath;
        }

        /// <summary>
        /// 拷贝指定文件到相对路径
        /// </summary>
        /// <param name="srcRootDirectory"></param>
        /// <param name="dstRootDirectory"></param>
        /// <param name="searchPattern"></param>
        /// <param name="appendExtension"></param>
        public static void CopyFileToSameRelativePath(string srcRootDirectory, string dstRootDirectory, string searchPattern = "*.*", string appendExtension = "")
        {
            if (!Directory.Exists(srcRootDirectory))
                return;

            string[] srcDirectoryAllFiles = Directory.GetFiles(srcRootDirectory, searchPattern, SearchOption.AllDirectories);
            int srcPathLength = srcRootDirectory.Length;

            foreach (string filePath in srcDirectoryAllFiles)
            {
                //文件相对路径
                string fileRelativePath = filePath.Substring(srcPathLength);

                string dstFilePath = dstRootDirectory + fileRelativePath;
                string dstFileFullName = dstFilePath + appendExtension;

                FileInfo dstFileInfo = new FileInfo(dstFileFullName);

                if (!dstFileInfo.Directory.Exists)
                    Directory.CreateDirectory(dstFileInfo.Directory.FullName);

                File.Copy(filePath, dstFileInfo.FullName, true);
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="deleteDir"></param>
        public static void DeleteDirectory(string deleteDir)
        {
            string[] files = Directory.GetFiles(deleteDir);
            string[] dirs = Directory.GetDirectories(deleteDir);

            foreach (string file in files)
            {
                File.SetAttributes(file, FileAttributes.Normal);
                File.Delete(file);
            }

            foreach (string dir in dirs)
            {
                DeleteDirectory(dir);
            }

            Directory.Delete(deleteDir, false);

        }

        /// <summary>
        /// 获取文件的真实名称根据大泪
        /// </summary>
        /// <param name="fileFuzzyPath"></param>
        /// <returns></returns>
        public static string GetFileRealNameByFuzzyCaseInsensitivePath(string fileFullPathCaseInsensitive)
        {
#if RES_PROJECT
            string fileNameIgnoreCase = Path.GetFileName(fileFullPathCaseInsensitive.ToLower());

            string fileRealName = string.Empty;
            if (m_fuzzycaseinsensitivePathCache.TryGetValue(fileNameIgnoreCase, out fileRealName))
                return fileRealName;
            else
            {
                DirectoryInfo fileDirInfo = new DirectoryInfo(Path.GetDirectoryName(fileFullPathCaseInsensitive));
                FileInfo[] direcotryAllFiles = fileDirInfo.GetFiles();
                for (int i = 0; i < direcotryAllFiles.Length; ++i)
                {
                    FileInfo fileInfo = direcotryAllFiles[i];
                    if (m_fuzzycaseinsensitivePathCache.ContainsKey(fileInfo.Name.ToLower()))
                        throw new Exception($"file name exist:{fileInfo.Name}");
                    else
                        m_fuzzycaseinsensitivePathCache.Add(fileInfo.Name.ToLower(), fileInfo.Name);
                }

                if (m_fuzzycaseinsensitivePathCache.TryGetValue(fileNameIgnoreCase, out fileRealName))
                    return fileRealName;

                Debug.LogWarning("file :" + fileFullPathCaseInsensitive + " not exist");

                return string.Empty;
            }
#else
             DirectoryInfo fileDirInfo = new DirectoryInfo(Path.GetDirectoryName(fileFullPathCaseInsensitive));
            string fileName = Path.GetFileName(fileFullPathCaseInsensitive);
            return fileDirInfo.GetFiles().FirstOrDefault(directoryFileInfo => directoryFileInfo.Name.Equals(fileName, StringComparison.OrdinalIgnoreCase)).Name;
#endif
        }

        /// <summary>
        /// 获取指定文件夹的文件
        /// </summary>
        /// <param name="directoryPath"></param>
        /// <param name="searchPattern"></param>
        /// <param name="searchOption"></param>
        /// <param name="createDirectoryIfNotExist"></param>
        /// <returns></returns>
        public static FileInfo[] GetFilesInDirectory(string directoryPath, string searchPattern, SearchOption searchOption = SearchOption.AllDirectories, bool createDirectoryIfNotExist = false)
        {
            DirectoryInfo searchDirectoryInfo = new DirectoryInfo(directoryPath);
            FileInfo[] filesInDirectory = null;
            if (!searchDirectoryInfo.Exists)
            {
                if (createDirectoryIfNotExist)
                    searchDirectoryInfo.Create();

                filesInDirectory = new FileInfo[0];
            }
            else
                filesInDirectory = searchDirectoryInfo.GetFiles(searchPattern, searchOption);

            return filesInDirectory;
        }

        /// <summary>
        /// 清空文件夹里指定类型数据
        /// </summary>
        /// <param name="dir_path"></param>
        /// <param name="type"></param>
        public static void ClearFolderByType(string dir_path, string type)
        {
            DirectoryInfo di = new DirectoryInfo(dir_path);
            if (di.Exists)
            {
                FileInfo[] files = di.GetFiles();
                string[] fullFileName;
                foreach (FileInfo file in files)
                {
                    fullFileName = file.Name.Split('.');
                    if (fullFileName.Length > 0)
                    {
                        string fileExt = fullFileName[fullFileName.Length - 1];
                        if (fileExt == type)
                        {
                            string file_path = Path.Combine(dir_path, file.Name);
                            File.Delete(file_path);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 获取文件夹内指定扩展名的文件列表
        /// </summary>
        /// <param name="folderPath"></param>
        /// <param name="fileExtensions"></param>
        /// <returns></returns>
        public static List<FileInfo> GetFolderFileInfoList(string folderPath, params string[] fileExtensions)
        {
            List<FileInfo> fileList = new List<FileInfo>();
            string[] directoryFilesPath = Directory.GetFiles(folderPath);
            foreach (string filePath in directoryFilesPath)
            {
                FileInfo fileInfo = new FileInfo(filePath);
                if (!fileInfo.Exists)
                    continue;

                if (fileInfo.Name.StartsWith("~$"))
                    continue;

                string fileExtension = fileInfo.Extension;
                if (fileExtensions.Contains(fileExtension))
                    fileList.Add(fileInfo);

            }

            return fileList;
        }

        /// <summary>
        /// 清空文件夹
        /// </summary>
        /// <param name="folderPath"></param>
        public static void ClearOrCreateEmptyFolder(string folderPath)
        {
            if (!Directory.Exists(folderPath))
                Directory.CreateDirectory(folderPath);
            else
            {
                string[] folderFiles = Directory.GetFiles(folderPath);
                for (int i = 0; i < folderFiles.Length; ++i)
                    File.Delete(folderFiles[i]);
            }
        }

        /// <summary>
        /// 批量拷贝文件
        /// </summary>
        /// <param name="srcPath"></param>
        /// <param name="dstPath"></param>
        /// <param name="filePattern"></param>
        /// <returns></returns>
        public static int BatchCopyFiles(string srcPath, string dstPath, string filePattern = null)
        {
            if (!Directory.Exists(srcPath))
                return 0;

            string[] copyFiles = Directory.GetFiles(srcPath, filePattern);

            ClearOrCreateEmptyFolder(dstPath);

            int copyFileCount = 0;
            for (int i = 0; i < copyFiles.Length; ++i)
            {
                string fileName = Path.GetFileName(copyFiles[i]);
                string dstFileName = Path.Combine(dstPath, fileName);
                File.Copy(copyFiles[i], dstFileName);
                copyFileCount++;
            }

            return copyFileCount;
        }

        /// <summary>
        /// 文件是否使用中
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static bool IsFileInUsing(string filePath)
        {
            bool inUse = true;
            FileStream fs = null;

            try
            {
                fs = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.None);

                inUse = false;
            }
            catch
            {

            }
            finally
            {
                if (fs != null)
                    fs.Close();
            }

            return inUse;
        }

        /// <summary>
        /// 给定路径是文件或文件夹
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static bool IsFileOrDir(string path)
        {
            FileAttributes fa = File.GetAttributes(path);

            if ((fa & FileAttributes.Directory) == FileAttributes.Directory)
                return false;
            else
                return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="directory"></param>
        /// <param name="searchPattern"></param>
        /// <returns></returns>
        public static string[] GetDirectoryFilesWithMultipleSearchPattern(string directory, string searchPattern)
        {
            return searchPattern.Split('|').SelectMany(filter => Directory.GetFiles(directory, filter, SearchOption.AllDirectories)).ToArray();
        }
    }
}
