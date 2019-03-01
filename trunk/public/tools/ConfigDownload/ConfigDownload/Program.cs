using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;


namespace ConfigDownload
{
    class Program
    {
        private const string shieldFile = "shieldFile.txt";

        static void Main(string[] args)
        {
            string curPath = System.Environment.CurrentDirectory;
            bool requestWeb = RequestWeb("http://10.0.107.229:7001/file/tar");
            if (requestWeb)
            {
                Console.WriteLine("请求生成所有excel成功，开始下载文件...");
            }
            else
            {
                Console.WriteLine("请求生成所有excel失败!!!!");
            }

            bool down = DownLoadConfig("http://10.0.107.229:7001/file/download/all.tar", "all.tar");
            if (down)
            {
                Console.WriteLine("下载完成，开始解压文件...");
            }
            else
            {
                Console.WriteLine("下载失败!!!");
            }
            bool zip = UnZip(curPath + "/all.tar", curPath);
            if (zip)
            {
                Console.WriteLine("解压完成，开始复制文件...");
            }
            else
            {
                Console.WriteLine("解压失败!!!");
            }
            bool copy = CopyFile(curPath + "/xls", curPath + "/../config", curPath + "/" + shieldFile);
            if (copy)
            {
                Console.WriteLine("文件复制完成，按任意键退出...");
            }
            else
            {
                Console.WriteLine("文件复制失败");
            }
        }

        private static bool DownLoadConfig(string url, string localUrl)
        {

            FileStream writeStream = new FileStream(localUrl, FileMode.Create);// 文件不保存创建一个文件
            int startPos = 0;
            try
            {
                HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(url);
                request.Method = "POST";
                request.UserAgent = "MSIE";
                Stream readStream = request.GetResponse().GetResponseStream();
                byte[] byteArray = new byte[1024];
                int contentSize = readStream.Read(byteArray, 0, byteArray.Length);
                long currentPos = startPos;
                while (contentSize > 0)
                {
                    currentPos += contentSize;
                    writeStream.Write(byteArray, 0, contentSize);// 写入本地文件
                    contentSize = readStream.Read(byteArray, 0, byteArray.Length);// 继续向远程文件读取

                }
                //关闭流
                writeStream.Close();
                readStream.Close();
                return true;
            }
            catch (Exception)
            {
                writeStream.Close();
                return false;
            }
        }

        // 从文件头得到远程文件的长度
        private static long GetHttpLength(string url)
        {
            long length = 0;
            try
            {
                HttpWebRequest req = (HttpWebRequest)HttpWebRequest.Create(url);// 打开网络连接
                req.Method = "POST";
                req.UserAgent = "MSIE";
                HttpWebResponse rsp = (HttpWebResponse)req.GetResponse();

                if (rsp.StatusCode == HttpStatusCode.OK)
                {
                    length = rsp.ContentLength;// 从文件头得到远程文件的长度
                }

                rsp.Close();
                return length;
            }
            catch (Exception e)
            {
                return length;
            }

        }

        private static bool UnZip(string fileZip, string zipFolder)
        {
            bool res = TarHelper.UnZip(fileZip, zipFolder);
            return res;
        }

        private static bool CopyFile(string srcDir, string destDir, string shieldPath)
        {
            if (!Directory.Exists(srcDir))
            {
                Console.WriteLine("源文件目录不存在!!!");
                return false;
            }
            if (!Directory.Exists(destDir))
            {
                Console.WriteLine("目标文件目录不存在!!!");
                return false;
            }
            if (!File.Exists(shieldPath))
            {
                File.Create(shieldPath);
            }
            List<string> shiledFiles = GetShieldFiles(shieldPath);


            string[] srcFiles = Directory.GetFiles(srcDir);
            for (int i = 0; i < srcFiles.Length; i++)
            {
                FileInfo fileInfo = new FileInfo(srcFiles[i]);
                if (!CompareShieldFile(shiledFiles, fileInfo.Name))
                {
                    File.Copy(srcFiles[i], destDir + "/" + fileInfo.Name, true);
                }
            }
            return true;
        }

        private static List<string> GetShieldFiles(string path)
        {
            List<string> shieldFiles = new List<string>();
            StreamReader reader = new StreamReader(path);
            string fileName = reader.ReadLine();
            while (!string.IsNullOrEmpty(fileName))
            {
                shieldFiles.Add(fileName.Trim());
                fileName = reader.ReadLine();
            }
            reader.Close();
            return shieldFiles;
        }

        private static bool CompareShieldFile(List<string> shiledFiles, string file)
        {
            for (int i = 0; i < shiledFiles.Count; i++)
            {
                if (shiledFiles[i].Equals(file))
                {
                    return true;
                }
            }
            return false;
        }

        private static bool RequestWeb(string url)
        {
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(url);
            request.Method = "POST";
            HttpWebResponse rsp = (HttpWebResponse)request.GetResponse();
            if (rsp.StatusCode == HttpStatusCode.OK)
            {
                rsp.Close();
                return true;
            }
            rsp.Close();
            return false;
        }
    }
}
