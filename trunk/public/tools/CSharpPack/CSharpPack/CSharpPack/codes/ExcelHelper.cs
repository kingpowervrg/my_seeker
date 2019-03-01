using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;


namespace ClientResourcePacker
{
    /// <summary>
    /// 解析Excel配置工具类
    /// </summary>
    public static class ExcelHelper
    {
        public static string[] EXCEL_EXTENSIONS = new string[] { ".xls", ".xlsx", ".xlsm" };


        /// <summary>
        /// 获取目录中所有的Excel文件
        /// </summary>
        /// <param name="directoryPath"></param>
        /// <returns></returns>
        public static List<FileInfo> GetDirectoryAllExcelFiles(string directoryPath)
        {
            List<FileInfo> excelFilesInDirectory = PathUtil.GetFolderFileInfoList(directoryPath, EXCEL_EXTENSIONS);
            return excelFilesInDirectory;
        }


        /// <summary>
        /// 获取目录中所有Excel数据信息
        /// </summary>
        /// <param name="directoryPath"></param>
        /// <returns></returns>
        public static Dictionary<FileInfo, List<ConfigSheetInfo>> GetDirectoryAllExcelConfigData(string directoryPath)
        {
            List<FileInfo> directoryExcels = GetDirectoryAllExcelFiles(directoryPath);

            Dictionary<FileInfo, List<ConfigSheetInfo>> configDataDict = new Dictionary<FileInfo, List<ConfigSheetInfo>>();

            for (int i = 0; i < directoryExcels.Count; ++i)
            {
                FileInfo excelFileInfo = directoryExcels[i];

                List<ConfigSheetInfo> excelSheetInfo = GetAllSheetsInfo(excelFileInfo);

                configDataDict.Add(excelFileInfo, excelSheetInfo);
            }

            return configDataDict;
        }

        /// <summary>
        /// 获取文件夹内所有Excel配置信息
        /// </summary>
        /// <param name="directoryPath"></param>
        /// <returns></returns>
        public static List<ConfigSheetInfo> GetDirectoryAllExcelConfigDataList(string directoryPath)
        {
            Dictionary<FileInfo, List<ConfigSheetInfo>> configDataDict = GetDirectoryAllExcelConfigData(directoryPath);

            List<ConfigSheetInfo> configSheetInfoList = new List<ConfigSheetInfo>();
            configDataDict.Values.ToList().ForEach(excelInfoList => excelInfoList.ForEach(configSheetInfo => configSheetInfoList.Add(configSheetInfo)));

            return configSheetInfoList;
        }


        /// <summary>
        /// 获取Excel里所有的Sheet信息
        /// </summary>
        /// <param name="excelFileInfo"></param>
        /// <returns></returns>
        public static List<ConfigSheetInfo> GetAllSheetsInfo(FileInfo excelFileInfo)
        {
            string fileExtension = excelFileInfo.Extension;
            if (!IsExcel(fileExtension))
            {
                Console.WriteLine(excelFileInfo.FullName + "不是Excel文件");
                return null;
            }

            try
            {
                using (FileStream fs = new FileStream(excelFileInfo.FullName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                {
                    IWorkbook workBook = GetWorkbook(excelFileInfo, fs);

                    int sheetNum = workBook.NumberOfSheets;
                    if (sheetNum == 0)
                    {
                        Console.WriteLine(excelFileInfo.FullName + "sheet 数量为0");
                        return null;
                    }

                    List<ConfigSheetInfo> sheetList = new List<ConfigSheetInfo>(sheetNum);

                    for (int i = 0; i < sheetNum; ++i)
                    {


                        ISheet sheet = workBook.GetSheetAt(i);
                        //检查是否是合法的配置表格式（有的是程序无用的表）
                        string confSheetName = GetConfigSheetName(sheet);
                        if (string.IsNullOrEmpty(confSheetName))
                            continue;

                        Console.WriteLine("resolving sheet:" + confSheetName);

                        ConfigSheetInfo sheetInfo = new ConfigSheetInfo();
                        sheetInfo.ConfigSheetName = confSheetName;
                        //sheetInfo.SheetInfo = sheet;
                        sheetInfo.SheetData = ParseSheet(sheet);

                        sheetList.Add(sheetInfo);
                    }

                    return sheetList;
                }
            }
            catch (IOException e)
            {
                Console.WriteLine("文件：{0} 已经打开，关闭后重新生成", excelFileInfo.FullName);
                return null;
            }

        }


        /// <summary>
        /// 解析Sheet
        /// </summary>
        /// <param name="sheetInfo"></param>
        /// <returns></returns>
        public static ConfigData ParseSheet(ISheet sheetInfo)
        {

            Stopwatch watch = new Stopwatch();
            watch.Start();

            ConfigData configData = new ConfigData();

            //parse title
            ParseConfigTitle(sheetInfo, ref configData);

            //parse data
            ParseConfigData(sheetInfo, ref configData);

            //parse to sqlite datatype

            watch.Stop();
            Console.WriteLine("reslove sheet" + sheetInfo.SheetName + "elapsed :" + watch.Elapsed);

            return configData;
        }

        /// <summary>
        /// 解析Sheet 头信息
        /// </summary>
        /// <param name="configSheet"></param>
        /// <param name="configData"></param>
        private static void ParseConfigTitle(ISheet configSheet, ref ConfigData configData)
        {
            short columnsCount = configSheet.GetRow(0).LastCellNum;
            int rowCount = configSheet.LastRowNum;


            configData.cl_list = new string[columnsCount];
            configData.type_list = new string[columnsCount];
            configData.name_list = new string[columnsCount];
            configData.msg_list = new string[columnsCount];
            configData.arr_list = new bool[columnsCount];

            //parse cl_list
            IRow clRow = configSheet.GetRow(0);
            for (int i = 0; i < columnsCount; ++i)
            {
                ICell cell = clRow.GetCell(i);
                if (cell != null)
                    configData.cl_list[i] = cell.ToString().ToLower().Trim();
                else
                    configData.cl_list[i] = "";
            }

            //parse data type list
            IRow dataTypeRow = configSheet.GetRow(1);
            for (int i = 0; i < columnsCount; ++i)
            {
                ICell cell = dataTypeRow.GetCell(i);
                if (cell != null)
                    configData.type_list[i] = cell.ToString().ToLower().Trim();
                else
                    configData.type_list[i] = "";
            }

            //parse name_list
            IRow nameRow = configSheet.GetRow(2);
            for (int i = 0; i < columnsCount; ++i)
            {
                ICell cell = nameRow.GetCell(i);
                if (cell != null)
                    configData.name_list[i] = cell.ToString().Trim();
                else
                    configData.name_list[i] = "";
            }

            IRow commentRow = configSheet.GetRow(3);
            for (int i = 0; i < columnsCount; ++i)
            {
                ICell cell = commentRow.GetCell(i);
                if (cell != null)
                    configData.msg_list[i] = cell.ToString();
                else
                    configData.msg_list[i] = "";
            }

            for (int i = 0; i < columnsCount; ++i)
            {
                string tmp = configData.type_list[i];
                if (!string.IsNullOrEmpty(tmp) && tmp.Contains('[') && tmp.Contains(']'))
                    configData.arr_list[i] = true;
                else
                    configData.arr_list[i] = false;
            }

        }

        /// <summary>
        /// 解析Sheet中的数据 
        /// </summary>
        /// <param name="configSheet"></param>
        /// <param name="configData"></param>
        private static void ParseConfigData(ISheet configSheet, ref ConfigData configData)
        {
            int rowNum = configSheet.LastRowNum;
            int columnNum = configSheet.GetRow(0).LastCellNum;

            List<string[]> dataList = new List<string[]>();

            //这里是i <= rowNum
            for (int i = 4; i <= rowNum; ++i)
            {
                IRow dataRow = configSheet.GetRow(i);
                if (dataRow == null)
                    continue;

                string[] rowDate = new string[columnNum];

                if (dataRow.GetCell(0) == null)
                    continue;
                    //throw new Exception(string.Format("{0},第{1}行sn为空", configSheet.SheetName, i + 1));

                string sn = dataRow.GetCell(0).ToString();
                if (string.IsNullOrEmpty(sn))
                    continue;

                if (sn.StartsWith("#"))
                    continue;

                for (int j = 0; j < columnNum; ++j)
                {
                    ICell cell = dataRow.GetCell(j);
                    if (cell == null)
                        continue;

                    string cellValue = string.Empty;

                    //处理公式列
                    if (cell.CellType == CellType.Formula)
                    {
                        try
                        {
                            cellValue = cell.StringCellValue;
                        }
                        catch
                        {
                            cellValue = cell.NumericCellValue.ToString();
                        }

                    }
                    else
                        cellValue = cell.ToString();

                    if (string.IsNullOrEmpty(cellValue))
                        continue;

                    cellValue = cellValue.Replace("\\n", "\n");
                    cellValue = cellValue.Replace("\\t", "\t");
                    cellValue = cellValue.Replace("\\r", "\r");
 /*                   cellValue = cellValue.Replace("[", "");
                    cellValue = cellValue.Replace("]", "");
                    cellValue = cellValue.Replace("\"", "");*/

                    rowDate[j] = cellValue;
                }
                dataList.Add(rowDate);
            }

            configData.data_list = dataList;
        }

        private static void ParseDatatypeToSqliteDatatype(ref ConfigData configData)
        {
            string[] sqliteDatatypeList = new string[configData.type_list.Length];
            for (int i = 0; i < configData.type_list.Length; ++i)
            {
                //sqliteDatatypeList[i] = 

            }

        }


        public static bool IsExcel(string fileExtension)
        {
            return EXCEL_EXTENSIONS.Contains(fileExtension.ToLower());
        }


        /// <summary>
        /// 获取Excel文件中的Sheet信息
        /// </summary>
        /// <param name="excelFileInfo"></param>
        /// <param name="excelStream"></param>
        /// <returns></returns>
        public static IWorkbook GetWorkbook(FileInfo excelFileInfo, FileStream excelStream)
        {
            IWorkbook workBook;
            if (excelFileInfo.Extension == ".xls")
                workBook = new HSSFWorkbook(excelStream);
            else
                workBook = new XSSFWorkbook(excelStream);

            return workBook;
        }

        /// <summary>
        /// 获取Sheet名(配置表名)
        /// </summary>
        /// <param name="sheet"></param>
        /// <returns></returns>
        public static string GetConfigSheetName(ISheet sheet)
        {
            string sheetName = sheet.SheetName;
            if (sheetName.ToLower().Contains("sheet"))
                return string.Empty;

            string[] confFormatter = sheetName.Split('|');
            if (confFormatter.Length != 2)
                return string.Empty;

            return confFormatter[1];
        }

    }
}
