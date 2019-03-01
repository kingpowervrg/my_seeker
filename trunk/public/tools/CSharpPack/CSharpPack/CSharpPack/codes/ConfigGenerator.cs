using CSharpPack.codes;
using RazorEngine;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace ClientResourcePacker
{
    /// <summary>
    /// 代码及配置生成类
    /// </summary>
    public class ConfigGenerator
    {
        /// <summary>
        /// 生成客户端Lua配置解析
        /// </summary>
        /// <param name="sheetInfo"></param>
        /// <param name="excelFileInfo"></param>
        /// <param name="targetFilePath"></param>
        /// <param name="luaTemplateFileInfo"></param>
        /// <remarks>11.11:guangze song 新项目不使用Lua这里暂时不用，如有需要生成Lua配置可调用</remarks>
        [Obsolete]
        public static void GenerateLuaClientConfigClass(ConfigSheetInfo sheetInfo, FileInfo excelFileInfo, string targetFilePath, FileInfo luaTemplateFileInfo)
        {
            if (luaTemplateFileInfo.Exists)
            {

                string templateContent = File.ReadAllText(luaTemplateFileInfo.FullName);

                SheetMsg luaClassRazorModel = GetRazorModel(sheetInfo, GeneratePlatform.CLIENT, excelFileInfo);

                string generatedCode = Razor.Parse(templateContent, luaClassRazorModel);

                string targetFileName = "Conf" + sheetInfo.ConfigSheetName + ".lua";
                string targetFileFullName = Path.Combine(targetFilePath, targetFileName);

                Console.WriteLine(Path.GetFileName(targetFileFullName));

                File.WriteAllText(targetFileFullName, generatedCode);
            }
        }

        /// <summary>
        /// 生成C#客户端配置表
        /// </summary>
        /// <param name="sheetInfo"></param>
        /// <param name="excelFileInfo"></param>
        /// <param name="targetFilePath"></param>
        /// <param name="CSharpTemplateFileInfo"></param>
        public static bool GeneralCSharpClientConfigClass(ConfigSheetInfo sheetInfo, FileInfo excelFileInfo, string targetFilePath, FileInfo CSharpTemplateFileInfo)
        {
            string templateContent = File.ReadAllText(CSharpTemplateFileInfo.FullName);
            SheetMsg cSharpRazorModel = GetRazorModel(sheetInfo, GeneratePlatform.CLIENT, excelFileInfo);

            if (cSharpRazorModel == null)
                return false;

            string generatedCode = Razor.Parse(templateContent, cSharpRazorModel);

            string targetFileName = "Conf" + sheetInfo.ConfigSheetName + ".cs";
            string targetFileFullName = Path.Combine(targetFilePath, targetFileName);

            Console.WriteLine(Path.GetFileName(targetFileFullName));

            File.WriteAllText(targetFileFullName, generatedCode);

            return true;
        }


        public static void GeneralCSharpAllConfigParserClass(List<ConfigSheetInfo> configList, FileInfo razorTemplateFile)
        {
            string templateContent = File.ReadAllText(razorTemplateFile.FullName);
            ConfigParser parser = GetConfigParserRazorModel(configList);
            if (parser.cls_name_list.Length == 0)
            {
                Console.WriteLine("general all config parser class error");
                return;
            }

            string generatedCode = Razor.Parse(templateContent, parser);
            string targetFile = "ConfFact.cs";
            string targetFileFullName = Path.Combine(PathConfig.SCRIPT_RPOJECT_CONFIG_PATH, targetFile);

            File.WriteAllText(targetFileFullName, generatedCode);

        }

        /// <summary>
        /// 生成客户端二进制配置文件
        /// </summary>
        /// <param name="sheetInfo"></param>
        /// <param name="targetFilePath"></param>
        [Obsolete("已使用Sqlite作为数据载体")]
        public static void GenerateClientBinaryData(ConfigSheetInfo sheetInfo, string targetFilePath)
        {
            List<int> platformColumnIndexList = GetTargetPlatformColunmIndex(sheetInfo, GeneratePlatform.CLIENT);
            if (platformColumnIndexList.Count == 0)
                return;

            string configName = "Conf" + sheetInfo.ConfigSheetName;
            int columnCount = platformColumnIndexList.Count;

            string targetFileFullName = Path.Combine(targetFilePath, sheetInfo.ConfigSheetName + ".bytes");

            try
            {
                Console.WriteLine("generate client data:" + sheetInfo.ConfigSheetName + ".bytes");

                using (BinaryWriter writer = new BinaryWriter(new FileStream(targetFileFullName, FileMode.Create)))
                {

                    //写入表名
                    writer.Write(configName);

                    //写入表列长
                    writer.Write(columnCount);

                    ConfigData configData = sheetInfo.SheetData;

                    //写入表头及类型
                    for (int i = 0; i < columnCount; ++i)
                    {
                        int indexInRaw = platformColumnIndexList[i];
                        string fieldName = configData.name_list[indexInRaw];
                        string fieldType = configData.type_list[indexInRaw];

                        writer.Write(fieldName);
                        writer.Write(fieldType);
                    }

                    //写入数据
                    List<string[]> rowDatas = configData.data_list;
                    writer.Write(configData.data_list.Count);

                    List<string> snList = new List<string>();

                    for (int i = 0; i < rowDatas.Count; ++i)
                    {
                        string[] rowData = rowDatas[i];

                        for (int j = 0; j < platformColumnIndexList.Count; ++j)
                        {
                            int indexInRaw = platformColumnIndexList[j];

                            string dataValue = rowData[indexInRaw];
                            string dataType = configData.type_list[indexInRaw];

                            if (j == 0)
                            {
                                if (snList.Contains(dataValue))
                                {
                                    Console.WriteLine("SN重复" + sheetInfo.ConfigSheetName);
                                    return;
                                }
                                else
                                    snList.Add(dataValue);
                            }

                            //处理数组
                            bool isArray = configData.arr_list[indexInRaw];
                            if (isArray)
                            {
                                dataValue = dataValue.Replace("[", "").Replace("]", "");

                                if (!string.IsNullOrEmpty(dataValue))
                                {

                                    string[] arrValues = dataValue.Split(':');

                                    writer.Write(arrValues.Length);
                                    foreach (string valueInArray in arrValues)
                                        WriteItemWithBytes(writer, dataType, valueInArray);
                                }
                                else
                                    writer.Write(0);

                            }
                            else
                            {
                                WriteItemWithBytes(writer, dataType, dataValue);
                            }

                        }
                    }

                }
            }
            catch (IOException e)
            {
                Console.WriteLine("无法创建二进制数据文件：" + sheetInfo.ConfigSheetName + "," + e.Message);
                return;
            }
        }

        /// <summary>
        /// 生成Excel 的客户端二进制数据
        /// </summary>
        /// <param name="binaryWriter"></param>
        /// <param name="fieldType"></param>
        /// <param name="fieldValue"></param>
        private static void WriteItemWithBytes(BinaryWriter binaryWriter, string fieldType, string fieldValue)
        {
            try
            {
                string strVal = string.IsNullOrEmpty(fieldValue) ? "" : fieldValue.ToLower().Trim();
                if (fieldType.StartsWith("bool"))
                {
                    if (string.IsNullOrEmpty(strVal) || strVal == "false" || strVal == "0.0" || strVal == "0")
                    {
                        //bool nfieldValue = false;
                        //binaryWriter.Write(nfieldValue);
                        binaryWriter.Write("0");
                    }
                    else if (string.IsNullOrEmpty(strVal) || strVal == "true" || strVal == "1.0" || strVal == "1")
                    {
                        //bool nvalue = true;
                        //binaryWriter.Write(nvalue);
                        binaryWriter.Write("1");
                    }
                    else
                    {
                        throw (new Exception("不可识别的bool值: " + strVal));
                    }
                }
                else if (fieldType.StartsWith("byte"))
                {
                    if (string.IsNullOrEmpty(strVal))
                    {
                        Byte nvalue = 0;
                        binaryWriter.Write(nvalue);
                    }
                    else
                    {
                        Byte nvalue = Byte.Parse(strVal);
                        binaryWriter.Write(nvalue);
                    }
                }
                else if (fieldType.StartsWith("short"))
                {
                    if (string.IsNullOrEmpty(strVal))
                    {

                        short nvalue = 0;
                        binaryWriter.Write(nvalue);
                    }
                    else
                    {
                        short nvalue = short.Parse(strVal);
                        binaryWriter.Write(nvalue);
                    }
                }
                else if (fieldType.StartsWith("int"))
                {
                    if (string.IsNullOrEmpty(strVal))
                    {
                        int nvalue = 0;
                        binaryWriter.Write(nvalue);
                    }
                    else
                    {
                        int nvalue = 0;
                        //if (strVal.Contains(".0")) {
                        //    double dvalue = double.Parse(strVal);
                        //    nvalue = (int)dvalue;
                        //}
                        nvalue = Int32.Parse(strVal);
                        binaryWriter.Write(nvalue);
                    }
                }
                else if (fieldType.StartsWith("long"))
                {
                    if (string.IsNullOrEmpty(strVal))
                    {
                        long nvalue = 0;
                        binaryWriter.Write(nvalue);
                    }
                    else
                    {
                        long nvalue = long.Parse(strVal);
                        binaryWriter.Write(nvalue);
                    }
                }
                else if (fieldType.StartsWith("float"))
                {
                    if (string.IsNullOrEmpty(strVal))
                    {
                        float nvalue = 0.0f;
                        binaryWriter.Write(nvalue);
                    }
                    else
                    {
                        float nvalue = float.Parse(strVal);
                        binaryWriter.Write(nvalue);
                    }
                }
                else if (fieldType.StartsWith("double"))
                {
                    if (string.IsNullOrEmpty(strVal))
                    {
                        double nvalue = 0.0;
                        binaryWriter.Write(nvalue);
                    }
                    else
                    {
                        double nvalue = double.Parse(strVal);
                        binaryWriter.Write(nvalue);
                    }
                }
                else if (fieldType.StartsWith("string"))
                {
                    if (string.IsNullOrEmpty(strVal))
                        binaryWriter.Write("");
                    else binaryWriter.Write(fieldValue);
                }
                else
                {
                    throw (new Exception("数据类型不正确 " + fieldType));
                }

            }
            catch (FormatException e)
            {
                FileStream fs = binaryWriter.BaseStream as FileStream;
                if (fs != null)
                    Console.WriteLine(string.Format("生成客户端数据文件：{0}错误，列类型：{1},值：{2}", fs.Name, fieldType, fieldValue));
                else
                    Console.WriteLine(string.Format("生成客户端数据文件：{0}错误，列类型：{1},值：{2}", "数据", fieldType, fieldValue));
                throw;
            }
        }


        /// <summary>
        /// Razor 代码生成
        /// </summary>
        /// <param name="sheetInfo"></param>
        /// <param name="platform"></param>
        /// <param name="excelFileInfo"></param>
        /// <returns></returns>
        public static SheetMsg GetRazorModel(ConfigSheetInfo sheetInfo, GeneratePlatform platform, FileInfo excelFileInfo)
        {
            List<int> platformColumnIndexList = GetTargetPlatformColunmIndex(sheetInfo, platform);
            int targetPlatformColunmCount = platformColumnIndexList.Count;

            SheetMsg razorModel = null;
            if (targetPlatformColunmCount > 0)
            {
                razorModel = new SheetMsg();
                razorModel.AttributeNames = new string[targetPlatformColunmCount];
                razorModel.AttributeTypes = new string[targetPlatformColunmCount];
                razorModel.AttributeFlags = new int[targetPlatformColunmCount];
                razorModel.ExcelName = excelFileInfo.Name;
                razorModel.SheetColumns = targetPlatformColunmCount;
                razorModel.TableName = sheetInfo.ConfigSheetName;
                for (int i = 0; i < platformColumnIndexList.Count; ++i)
                {
                    int colunmIndex = platformColumnIndexList[i];

                    razorModel.AttributeTypes[i] = TypeConvertor(sheetInfo.SheetData.type_list[colunmIndex]);
                    razorModel.AttributeNames[i] = sheetInfo.SheetData.name_list[colunmIndex];
                    razorModel.AttributeFlags[i] = GetAttributeTypeInternalFlag(sheetInfo.SheetData.type_list[colunmIndex]);
                }
            }

            return razorModel;
        }

        public static ConfigParser GetConfigParserRazorModel(List<ConfigSheetInfo> configSheetList)
        {
            List<string> configClassNameList = new List<string>();
            foreach (ConfigSheetInfo item in configSheetList)
            {
                string className = "Conf" + item.ConfigSheetName;
                configClassNameList.Add(className);
            }

            ConfigParser parser = new ConfigParser();
            parser.cls_name_list = configClassNameList.ToArray();

            return parser;
        }

        /// <summary>
        /// 获取要生成平台的数据
        /// </summary>
        /// <param name="sheetInfo"></param>
        /// <param name="platform"></param>
        /// <returns></returns>
        private static List<int> GetTargetPlatformColunmIndex(ConfigSheetInfo sheetInfo, GeneratePlatform platform)
        {

            char platformFlag = 'c';
            switch (platform)
            {
                case GeneratePlatform.CLIENT:
                    platformFlag = 'c';
                    break;
                case GeneratePlatform.SERVER:
                    platformFlag = 's';
                    break;
                case GeneratePlatform.EDITOR:
                    platformFlag = 'e';
                    break;
                default:
                    break;
            }

            List<int> platformColumnIndexList = new List<int>();
            for (int i = 0; i < sheetInfo.SheetData.cl_list.Length; ++i)
            {
                string platformTitle = sheetInfo.SheetData.cl_list[i];
                if (platformTitle.Contains(platformFlag))
                    platformColumnIndexList.Add(i);
            }

            return platformColumnIndexList;
        }

        private static string TypeConvertor(string configType)
        {
            string mapType = configType;
            switch (configType.ToLower())
            {
                case "boolean":
                    mapType = "bool";
                    break;
                case "boolean[]":
                    mapType = "bool[]";
                    break;
            }
            return mapType;
        }


        /// <summary>
        /// 配置数据写入Sqlite
        /// </summary>
        /// <param name="sheetInfo"></param>
        public static ConfSqliteModel GetConfigSqliteModel(ConfigSheetInfo sheetInfo)
        {
            ConfSqliteModel sqliteModel = null;
            try
            {
                List<int> clientDataIndex = GetTargetPlatformColunmIndex(sheetInfo, GeneratePlatform.CLIENT);
                if (clientDataIndex.Count == 0)
                    return null;

                ConfigSqliteData confSqliteData = new ConfigSqliteData(clientDataIndex.Count);
                for (int i = 0; i < clientDataIndex.Count; ++i)
                {
                    int index = clientDataIndex[i];
                    confSqliteData.arr_list[i] = sheetInfo.SheetData.arr_list[index];
                    confSqliteData.cl_list[i] = sheetInfo.SheetData.cl_list[index];
                    //confSqliteData.msg_list[i] = sheetInfo.SheetData.msg_list[index];
                    confSqliteData.name_list[i] = sheetInfo.SheetData.name_list[index];
                    confSqliteData.type_list[i] = sheetInfo.SheetData.type_list[index];
                    confSqliteData.sqliteDatatype_list[i] = TypeConvertorToSqliteDatatype(confSqliteData.type_list[i], confSqliteData.arr_list[i]);
                }

                for (int i = 0; i < sheetInfo.SheetData.data_list.Count; ++i)
                {
                    string[] rowData = sheetInfo.SheetData.data_list[i];
                    string[] clientData = new string[clientDataIndex.Count];

                    for (int j = 0; j < clientDataIndex.Count; ++j)
                    {
                        int index = clientDataIndex[j];

                        clientData[j] = rowData[index];
                    }

                    confSqliteData.data_list.Add(clientData);
                }

                sqliteModel = new ConfSqliteModel();

                //Sqlite创建对应数据表
                string sqlite_tablename = "conf_" + sheetInfo.ConfigSheetName;

                sqliteModel.table_name = sqlite_tablename;
                sqliteModel.table_columnsNames = confSqliteData.name_list;
                sqliteModel.table_colunmsDataTypes = confSqliteData.sqliteDatatype_list;
                sqliteModel.dataList = new List<SqliteDataObject>();

                string strSql = string.Format("INSERT INTO {0} VALUES(", sqlite_tablename);

                for (int i = 0; i < confSqliteData.data_list.Count; ++i)
                {
                    string[] rowData = confSqliteData.data_list[i];
                    bool[] isArrayList = confSqliteData.arr_list;

                    SqliteDataObject sqliteObject = new SqliteDataObject();

                    StringBuilder sql = new StringBuilder(strSql);

                    for (int j = 0; j < rowData.Length; ++j)
                    {
                        bool isArray = isArrayList[j];
                        string rowDataType = confSqliteData.type_list[j];

                        if (isArray)
                        {
                            MemoryStream stream = new MemoryStream();
                            BinaryWriter writer = new BinaryWriter(stream);

                            sql.Append("?");

                            //写入长度
                            if (!string.IsNullOrEmpty(rowData[j]))
                            {
                                string strCellData = rowData[j].Replace("[", "").Replace("]", "");
                                string[] cellData = strCellData.Split(',');
                                writer.Write(cellData.Length);

                                for (int k = 0; k < cellData.Length; ++k)
                                    WriteItemWithBytes(writer, rowDataType, cellData[k]);
                            }
                            else
                                writer.Write(0);

                            sqliteObject.bytesList.Add(stream.ToArray());

                            writer.Close();
                            stream.Close();
                        }
                        else
                        {
                            string dataContent = rowData[j];
                            dataContent = SetSqliteDefaultValue(rowDataType, rowData[j]);

                            dataContent = dataContent.Replace("'", "''");

                            dataContent = string.Format("\'{0}\'", dataContent);

                            //string sqlDataContent = string.Format("\"{0}\"", dataContent);

                            //if (IsJsonString(sqlDataContent))
                            //    sqlDataContent = $"'{sqlDataContent}'";


                            sql.Append(dataContent);
                        }
                        if (j == rowData.Length - 1)
                            sql.Append(")");
                        else
                            sql.Append(",");

                        sqliteObject.strSql = sql.ToString();
                    }
                    sqliteModel.dataList.Add(sqliteObject);

                }
            }
            catch (Exception e)
            {
                Console.WriteLine(sheetInfo.ConfigSheetName + "出错:" + e);
                throw;
            }
            return sqliteModel;
        }

        /// <summary>
        /// 数据写入sqlite
        /// </summary>
        /// <param name="sqliteModelList"></param>
        public static void WriteSqliteModelToDatabase(List<ConfSqliteModel> sqliteModelList)
        {
            Console.WriteLine("begin write config data to sqlite database....");

            //写入数据
            SQLite.SQLiteConnection conn = SQLiteHelper.Instance.SqliteConnection;
            conn.BeginTransaction();

            for (int i = 0; i < sqliteModelList.Count; ++i)
            {
                ConfSqliteModel sqliteModel = sqliteModelList[i];

                Console.WriteLine(sqliteModel.table_name);
                SQLiteHelper.Instance.CreateTable(sqliteModel.table_name, sqliteModel.table_columnsNames, sqliteModel.table_colunmsDataTypes);

                for (int j = 0; j < sqliteModel.dataList.Count; ++j)
                {
                    SqliteDataObject sqlObject = sqliteModel.dataList[j];
                    SQLiteHelper.Instance.InsertValues(sqlObject.strSql, sqlObject.bytesList.ToArray());
                }
            }

            conn.Commit();

        }


        private static string SetSqliteDefaultValue(string dataType, string dataValue)
        {

            if (string.IsNullOrEmpty(dataValue))
            {

                dataType = dataType.ToLower();

                switch (dataType)
                {
                    case "bool":
                    case "boolean":
                        dataValue = "0";
                        break;
                    case "int":
                    case "float":
                    case "double":
                    case "byte":
                    case "short":
                    case "long":
                        dataValue = "0";
                        break;
                    case "string":
                        dataValue = " ";
                        break;
                }
            }
            else
            {
                if (dataType == "bool" || dataType == "boolean")
                {
                    dataValue = dataValue.ToLower();
                    if (dataValue == "true")
                        dataValue = "1";
                    else if (dataValue == "false")
                        dataValue = "0";
                }
            }

            return dataValue;
        }

        private static string TypeConvertorToSqliteDatatype(string configType, bool isArray)
        {
            configType = configType.ToUpper();
            string sqliteDataType = configType;
            if (isArray)
            {
                sqliteDataType = "BLOB";
                return sqliteDataType;
            }

            switch (configType)
            {
                case "STRING":
                    sqliteDataType = "TEXT";
                    break;
                case "INT":
                    sqliteDataType = "INTEGER";
                    break;

            }

            return sqliteDataType;
        }

        /// <summary>
        /// 内部数据类型标记
        /// </summary>
        /// <param name="attributeName"></param>
        /// <returns></returns>
        private static int GetAttributeTypeInternalFlag(string attributeName)
        {
            if (attributeName.StartsWith("int"))
            {
                if (attributeName.Contains("["))
                    return 11;
                else
                    return 1;
            }
            else if (attributeName.StartsWith("string") || attributeName.StartsWith("String"))
            {
                if (attributeName.Contains("["))
                    return 12;
                else
                    return 2;
            }
            else if (attributeName.StartsWith("float"))
            {
                if (attributeName.Contains("["))
                    return 13;
                else
                    return 3;
            }
            else if (attributeName.StartsWith("bool") || attributeName.StartsWith("boolean") || attributeName.StartsWith("Boolean"))
            {
                if (attributeName.Contains("["))
                    return 14;
                else
                    return 4;
            }
            else if (attributeName.StartsWith("short"))
            {
                if (attributeName.Contains("["))
                    return 15;
                else
                    return 5;
            }
            else if (attributeName.StartsWith("byte"))
            {
                if (attributeName.Contains("["))
                    return 16;
                else
                    return 6;
            }
            else if (attributeName.StartsWith("long"))
            {
                if (attributeName.Contains("["))
                    return 17;
                else
                    return 7;

            }
            else if (attributeName.Contains("double"))
            {
                if (attributeName.Contains("["))
                    return 18;
                else
                    return 8;

            }
            return 0;
        }



        public class ConfSqliteModel
        {
            public string table_name = string.Empty;
            public string[] table_columnsNames;
            public string[] table_colunmsDataTypes;
            public List<SqliteDataObject> dataList = new List<SqliteDataObject>();
        }


        public class SqliteDataObject
        {
            public string strSql;
            public List<byte[]> bytesList = new List<byte[]>();
        }

    }
}
