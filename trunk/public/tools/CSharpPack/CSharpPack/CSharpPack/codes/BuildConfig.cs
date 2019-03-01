using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;

namespace ClientResourcePacker
{
    class BuildConfig
    {
        private static Dictionary<FileInfo, List<ConfigSheetInfo>> m_configDataList = new Dictionary<FileInfo, List<ConfigSheetInfo>>();

        /// <summary>
        /// 解析所有配置信息
        /// </summary>
        private static void ResolvingConfigData()
        {
            Console.WriteLine("Resolving Excel Data...");

            m_configDataList.Clear();
            m_configDataList = ExcelHelper.GetDirectoryAllExcelConfigData(PathConfig.EXCEL_CONFIG_PATH);
        }

        static int StartProcess(string file, string argument, string workingFolder = null)
        {
            try
            {
                ProcessStartInfo info = new ProcessStartInfo(file);
                info.Arguments = argument;
                info.UseShellExecute = false;
                info.CreateNoWindow = true;
                if (workingFolder != null)
                    info.WorkingDirectory = workingFolder;

                Process p = Process.Start(info);
                p.WaitForExit();
                return p.ExitCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Process start error:" + ex);
                return 1;
            }
        }


        /// <summary>
        /// Build AssetBundle
        /// </summary>
        /// <param name="packtype"></param>
        private static void DoPackConfigAssets(string packtype = "1", string path = "")
        {
            Console.WriteLine("pack assets begin ...");
            //win32
            OperatingSystem system = System.Environment.OSVersion;
            string cmd = "";
            Console.WriteLine("Platform:" + system.Platform);

            if (system.Platform == PlatformID.Win32NT)
                cmd = Path.Combine(ConfigurationManager.AppSettings["windowsunitypath"]);
            else if (system.Platform == PlatformID.MacOSX || system.Platform == PlatformID.Unix)
                cmd = Path.Combine(ConfigurationManager.AppSettings["macunitypath"]);

            if (!string.IsNullOrEmpty(path))
            {
                cmd = path + "/Unity.exe";
            }

            if (!File.Exists(cmd))
                throw new FileNotFoundException("Unity.exe 不存在，请在appconfig里配置unity目录");

            string param = "-executeMethod EngineCore.Editor.PackEntry.";

            switch (int.Parse(packtype))
            {
                case 1:
                    param = param + "PackAllWindows";
                    break;
                case 2:
                    param = param + "PackAllAndroid";
                    break;
                case 3:
                    param = param + "PackAllIOS";
                    break;
                case 4:
                    param = param + "PackAndroidConfig";
                    break;
                case 5:
                    param = param + "PackConfig";
                    break;
                case 8:
                    param = "-executeMethod GenSceneTextureReport." + "GenReport";
                    break;
                case 9:
                    param = "-executeMethod GenProjectEffectReport." + "GenReport";
                    break;
                case 10:
                    param = "-executeMethod GenProjectCharacterReport.GenerateProjectCharacterReportToFile";
                    break;
                case 11:
                    param = "-executeMethod GenProjectEffectReportToExcel.GenReport";
                    break;
                case 21:
                    param += "PackAllAndroidRelease";
                    break;
                default:
                    Console.WriteLine("打包客户端配置成功");
                    return;
            }


            Console.WriteLine(param);

            string logFile = " -logFile " + PathConfig.PROJECT_ROOT + "debug.log";

            if (File.Exists(logFile))
                File.Delete(logFile);


            if (StartProcess(cmd, string.Format("-batchmode  -quit -projectPath {0} {1} {2}", PathConfig.PACK_UNITY_PROJECT_PATH, param, logFile)) != 0)
            {
                ParseUnityLogErrorToConsole(PathConfig.PROJECT_ROOT + "debug.log");
                Console.WriteLine("打包资源失败.");
            }
            else
            {
                Console.WriteLine("打包资源完毕！");
            }
        }

        static void ParseUnityLogErrorToConsole(string path)
        {
            bool startRead = false;
            using (FileStream fs = new FileStream(path, System.IO.FileMode.Open, System.IO.FileAccess.Read, FileShare.ReadWrite))
            {
                using (StreamReader sr = new StreamReader(fs, System.Text.Encoding.UTF8))
                {
                    while (sr.EndOfStream == false)
                    {
                        string line = sr.ReadLine();
                        if (line == "pack error:")
                            startRead = true;

                        if (startRead)
                            Console.WriteLine(line);
                    }
                }
            }
        }

        /// <summary>
        /// 生成客户端配置
        /// </summary>
        static void GenerateConfig()
        {
            Console.WriteLine("Pack Config Begin...");

            //所有配置文件信息
            Dictionary<FileInfo, List<ConfigSheetInfo>> configDict = m_configDataList.Count == 0 ? ExcelHelper.GetDirectoryAllExcelConfigData(PathConfig.EXCEL_CONFIG_PATH) : m_configDataList;

            foreach (KeyValuePair<FileInfo, List<ConfigSheetInfo>> pair in configDict)
            {
                List<ConfigSheetInfo> excelSheetList = pair.Value;
                MergeSheet(excelSheetList);
            }

            if (Directory.Exists(PathConfig.SCRIPT_RPOJECT_CONFIG_PATH))
            {
                //生成CSharp解析文件
                PathUtil.ClearOrCreateEmptyFolder(PathConfig.SCRIPT_RPOJECT_CONFIG_PATH);

                if (!File.Exists(PathConfig.CONF_CSHARP_TMPL))
                {
                    Console.WriteLine("C# 模板不存在");
                    return;
                }
                FileInfo cSharpTemp = new FileInfo(PathConfig.CONF_CSHARP_SQLITE_TEMPL);

                Console.WriteLine("generate client config parser file");

                List<ConfigSheetInfo> allConfigList = new List<ConfigSheetInfo>();
                foreach (KeyValuePair<FileInfo, List<ConfigSheetInfo>> pair in configDict)
                {
                    FileInfo excelInfo = pair.Key;
                    List<ConfigSheetInfo> excelSheetList = pair.Value;
                    for (int i = 0; i < excelSheetList.Count; ++i)
                    {
                        bool isClientConfig = ConfigGenerator.GeneralCSharpClientConfigClass(excelSheetList[i], excelInfo, PathConfig.SCRIPT_RPOJECT_CONFIG_PATH, cSharpTemp);
                        if (isClientConfig)
                            allConfigList.Add(excelSheetList[i]);
                    }
                }

                if (allConfigList.Count > 0)
                {
                    FileInfo factTemplateInfo = new FileInfo(PathConfig.CONF_ALL_PARSER_TMPL);
                    if (factTemplateInfo != null)
                        ConfigGenerator.GeneralCSharpAllConfigParserClass(allConfigList, factTemplateInfo);
                }

                Console.WriteLine("generate client config parser file completed");
            }

            //生成byte文件
            PathUtil.ClearFolderByType(PathConfig.GENERATE_CLIENT_DATA_PATH, "bytes");
            Console.WriteLine("------------begin generate client data config------------");

            List<ConfigGenerator.ConfSqliteModel> sqliteModelList = new List<ConfigGenerator.ConfSqliteModel>();

            foreach (KeyValuePair<FileInfo, List<ConfigSheetInfo>> pair in configDict)
            {
                FileInfo excelInfo = pair.Key;
                List<ConfigSheetInfo> excelSheetList = pair.Value;

                for (int i = 0; i < excelSheetList.Count; ++i)
                {
                    ConfigGenerator.ConfSqliteModel sqliteModel = ConfigGenerator.GetConfigSqliteModel(excelSheetList[i]);
                    if (sqliteModel != null)
                        sqliteModelList.Add(sqliteModel);
                }
            }

            ConfigGenerator.WriteSqliteModelToDatabase(sqliteModelList);

            Console.WriteLine("data generate completed");

            //拷贝db文件到代码工程
            PathUtil.ClearOrCreateEmptyFolder(PathConfig.PROJECT_SQLITE_DB_PATH);
            string targetPath = PathConfig.PROJECT_SQLITE_DB_PATH + "ConfData.bytes";
            File.Copy(PathConfig.SQLITE_DB_PATH, targetPath);
        }

        static void MergeSheet(List<ConfigSheetInfo> excelSheetList)
        {
            for (int i = 0; i < excelSheetList.Count; ++i)
            {
                for (int j = i + 1; j < excelSheetList.Count; ++j)
                {
                    if (excelSheetList[i].ConfigSheetName.Equals(excelSheetList[j].ConfigSheetName))
                    {
                        ConfigData data1 = excelSheetList[i].SheetData;
                        ConfigData data2 = excelSheetList[j].SheetData;

                        List<string> cl_list = new List<string>();
                        List<string> type_list = new List<string>();
                        List<string> name_list = new List<string>();
                        List<string> msg_list = new List<string>();
                        List<bool> arr_list = new List<bool>();

                        //为方便运算------数据  列转行
                        Dictionary<string, string[]> data1_list = new Dictionary<string, string[]>();
                        for (int m = 0; m < data1.name_list.Length; ++m)
                        {
                            if (name_list.Contains(data1.name_list[m]) == false)
                            {
                                cl_list.Add(data1.cl_list[m]);
                                type_list.Add(data1.type_list[m]);
                                name_list.Add(data1.name_list[m]);
                                //msg_list.Add(data1.msg_list[m]);
                                arr_list.Add(data1.arr_list[m]);
                            }

                            string[] rowData = new string[data1.data_list.Count];
                            for (int n = 0; n < data1.data_list.Count; n++)
                            {
                                rowData[n] = data1.data_list[n][m];
                            }
                            data1_list.Add(data1.name_list[m], rowData);
                        }

                        //第二sheet数据  列转行
                        Dictionary<string, string[]> data2_list = new Dictionary<string, string[]>();
                        for (int m = 0; m < data2.name_list.Length; ++m)
                        {
                            if (name_list.Contains(data2.name_list[m]) == false)
                            {
                                cl_list.Add(data2.cl_list[m]);
                                type_list.Add(data2.type_list[m]);
                                name_list.Add(data2.name_list[m]);
                                //msg_list.Add(data2.msg_list[m]);
                                arr_list.Add(data2.arr_list[m]);
                            }

                            string[] rowData = new string[data2.data_list.Count];
                            for (int n = 0; n < data2.data_list.Count; n++)
                            {
                                rowData[n] = data2.data_list[n][m];
                            }
                            data2_list.Add(data2.name_list[m], rowData);
                        }

                        List<string[]> data_list = new List<string[]>();
                        for (int m = 0; m < name_list.Count; ++m)
                        {
                            //复制数据
                            string[] sheet1data = data1_list.ContainsKey(name_list[m]) ? data1_list[name_list[m]] : new string[data1.data_list.Count];
                            string[] sheet2data = data2_list.ContainsKey(name_list[m]) ? data2_list[name_list[m]] : new string[data2.data_list.Count];
                            string[] rowData = new string[sheet1data.Length + sheet2data.Length];
                            sheet1data.CopyTo(rowData, 0);
                            sheet2data.CopyTo(rowData, sheet1data.Length);
                            data_list.Add(rowData);
                        }

                        data1.cl_list = cl_list.ToArray();
                        data1.type_list = type_list.ToArray();
                        data1.name_list = name_list.ToArray();
                        //data1.msg_list = msg_list.ToArray();
                        data1.arr_list = arr_list.ToArray();
                        data1.cl_list = cl_list.ToArray();
                        data1.data_list.Clear();
                        for (int m = 0; m < data_list[0].Length; m++)
                        {
                            string[] rowData = new string[name_list.Count];
                            for (int n = 0; n < name_list.Count; n++)
                            {
                                rowData[n] = data_list[n][m];
                            }
                            data1.data_list.Add(rowData);
                        }

                        excelSheetList.Remove(excelSheetList[j]);
                        j--;
                    }
                }
            }
        }

        static int Main(string[] args)
        {
            //Console.WriteLine(PathConfig.PROJECT_ROOT);
            //return 0;
            //1. 解析所有Excel配置
            ResolvingConfigData();

            //2.生成客户端.cs 及.bytes
            GenerateConfig();

            //3.打包成Unity资源
            if (args.Length == 1)
                DoPackConfigAssets(args[0]);
            else if (args.Length == 2)
                DoPackConfigAssets(args[0], args[1]);
            else
                DoPackConfigAssets();

            Console.ReadKey();
            return 0;
        }
    }
}
