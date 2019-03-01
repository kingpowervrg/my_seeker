using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ClientResourcePacker
{
    public class PathUtil
    {
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
    }
}
