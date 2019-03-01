using EngineCore;
using fastJSON;
using GOEngine.Implement;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
namespace conf
{
    enum ValueType
    {
        BOOL,
        STRING,
        SHORT,
        INT,
        FLOAT,
        DOUBLE,


        INVALID
    }
    struct ColumnInfo
    {
        public ValueType type;
        public bool isArray;
        public string name;
    }
    public class ConfHelper
    {
        private const int HEAD_ROW_COUNT = 4;
        private static ValueType getValueType(string strType)
        {
            ValueType type = ValueType.INVALID;
            strType = strType.ToLower();
            if (strType.StartsWith("bool"))
                type = ValueType.BOOL;
            else if (strType.StartsWith("string"))
                type = ValueType.STRING;
            else if (strType.StartsWith("short"))
                type = ValueType.SHORT;
            else if (strType.StartsWith("int"))
                type = ValueType.INT;
            else if (strType.StartsWith("float"))
                type = ValueType.FLOAT;
            else if (strType.StartsWith("double"))
                type = ValueType.DOUBLE;
            return type;
        }
        private static bool isArrayType(string strType)
        {
            return strType.IndexOf("[]") >= 0;
        }
        private static ColumnInfo[] getColumnInfos(ISheet sheet)
        {
            int columnCount = sheet.GetRow(0).LastCellNum;
            ColumnInfo[] infos = new ColumnInfo[columnCount];
            for (int column = 0; column < columnCount; column++)
            {
                ColumnInfo info = new ColumnInfo();
                IRow row = sheet.GetRow(1);
                string strType = row.GetCell(column).StringCellValue;
                info.type = getValueType(strType);
                info.isArray = isArrayType(strType);
                info.name = sheet.GetRow(2).GetCell(column).StringCellValue;
                infos[column] = info;
            }
            return infos;
        }
        private static bool loadFromSheet<T>(ISheet sheet, out List<T> list, out PropertyInfo[] fieldInfos) where T : new()
        {
            list = null;
            fieldInfos = null;
            int rowCount = sheet.LastRowNum + 1 - HEAD_ROW_COUNT;
            if (rowCount <= 0)
                return false;
            ColumnInfo[] infos = getColumnInfos(sheet);
            fieldInfos = getFieldInfos<T>(infos);
            List<T> datas = new List<T>();

            for (int i = 0; i < rowCount; i++)
            {
                IRow row = sheet.GetRow(i + HEAD_ROW_COUNT);
                T t = getRowData<T>(row, fieldInfos);
                datas.Add(t);
            }
            list = datas;
            return true;
        }
        private static PropertyInfo[] getFieldInfos<T>(ColumnInfo[] infos)
        {
            Type type = typeof(T);
            PropertyInfo[] fieldInfos = new PropertyInfo[infos.Length];
            for (int i = 0; i < infos.Length; i++)
            {
                fieldInfos[i] = type.GetProperty(infos[i].name);
            }
            return fieldInfos;
        }
        private static T getRowData<T>(IRow row, PropertyInfo[] infos) where T : new()
        {
            T t = new T();
            for (int i = 0; i < infos.Length; i++)
            {
                PropertyInfo info = infos[i];
                string sValue;
                if (row.GetCell(i) != null && row.GetCell(i).CellType != CellType.Formula)
                    sValue = row.GetCell(i).ToString();
                else
                    sValue = string.Empty;
                Type type = info.PropertyType;
                info.SetValue(t, parseData(ref sValue, type), null);
            }

            return t;
        }
        private static string obj2String(object obj, Type type)
        {
            if (type.IsArray)
            {
                IList objs = obj as IList;
                StringBuilder build = new StringBuilder();
                Type tElem = type.GetElementType();
                foreach (object o in objs)
                {
                    build.Append(obj2String(o, tElem));
                    build.Append(",");
                }
                int len = build.Length;
                if (len > 0)
                    build.Remove(len - 1, 1);
                return build.ToString();
            }
            else
            {
                return Convert.ToString(obj);
            }
        }
        private static object parseData(ref string data, Type type)
        {
            if (type.IsArray)
            {
                string[] ary = data.Split(",".ToCharArray());
                Array objs = Array.CreateInstance(type.GetElementType(), ary.Length);

                for (int i = 0; i < ary.Length; i++)
                {
                    objs.SetValue(parseData(ref ary[i], type.GetElementType()), i);
                }

                return objs;
            }
            else
            {
                return JsonUtil.ConvertData(data, type);
            }
        }
        public static bool LoadFromExcel<T>(string excel, int index, out List<T> list, out PropertyInfo[] fieldInfos) where T : new()
        {
            list = null;
            fieldInfos = null;
            using (FileStream stream = File.Open(excel, FileMode.Open, FileAccess.Read))
            {
                IWorkbook book = createWorkbook(excel, stream);
                if (book.NumberOfSheets < index)
                    return false;
                ISheet s = book.GetSheetAt(index);
                return loadFromSheet<T>(s, out list, out fieldInfos);
            }
        }
        private static IWorkbook createWorkbook(string excel, FileStream stream)
        {
            IWorkbook book = null;
            if (excel.ToLower().EndsWith(".xls"))
                book = new HSSFWorkbook(stream);
            else if (excel.ToLower().EndsWith(".xlsx"))
                book = new XSSFWorkbook(stream);
            return book;
        }
        public static bool SaveToExcel<T>(string excel, int index, List<T> datas, PropertyInfo[] fieldInfos) where T : new()
        {
            IWorkbook book = null;
            using (FileStream stream = File.Open(excel, FileMode.Open, FileAccess.Read))
            {
                book = createWorkbook(excel, stream);
                if (book.NumberOfSheets < index)
                {
                    return false;
                }
                stream.Close();
            }
            //System.IO.File.Delete(excel);
            using (FileStream stream = File.Open(excel + ".json", FileMode.OpenOrCreate, FileAccess.ReadWrite))
            {
                ISheet s = book.GetSheetAt(index);
                List<object[]> list = new List<object[]>();
                saveToSheet<T>(datas, s, fieldInfos, list);
                //book.Write(stream);
                string str = JSON.ToJSON(list);
                byte[] buff = System.Text.UTF8Encoding.Default.GetBytes(str);
                stream.Write(buff, 0, buff.Length);
                stream.Close();
            }
            replaceExcel(excel, index);
            System.IO.File.Delete(excel + ".json");
            return true;
        }
        private static void replaceExcel(string file, int index)
        {
            var p = new System.Diagnostics.Process();

            var path = UnityEngine.Application.streamingAssetsPath;
            p.StartInfo.FileName = path + "/../conf/Editor/base/ExcelReplace.exe";
            //p.StartInfo.FileName = @"D:\192.168.30.4\sanguo\code\client\goe\res\editor\Assets\conf\Editor\base\ExcelReplace.exe";
            p.StartInfo.Arguments = "\"" + file + "\" " + index.ToString();
            p.Start();
            p.WaitForExit();
        }

        private static bool saveToSheet<T>(List<T> datas, ISheet sheet, PropertyInfo[] fieldInfos, List<object[]> list) where T : new()
        {
            list.Clear();
            for (int i = 0; i < datas.Count; i++)
            {
                T t = datas[i];
                IRow row = sheet.GetRow(i + HEAD_ROW_COUNT);
                if (row == null)
                    row = sheet.CreateRow(i + HEAD_ROW_COUNT);
                for (int j = 0; j < fieldInfos.Length; j++)
                {
                    PropertyInfo fi = fieldInfos[j];
                    ICell cell = row.GetCell(j);
                    if (cell == null)
                        cell = row.CreateCell(j);
                    object value = fi.GetValue(t, null);
                    string strValue = obj2String(value, fi.PropertyType);
                    if (cell.CellType != CellType.Formula)
                    {
                        cell.SetCellValue(strValue);
                        var jsons = new object[3];
                        jsons[0] = i + HEAD_ROW_COUNT;
                        jsons[1] = j;
                        jsons[2] = strValue;
                        list.Add(jsons);
                    }
                }
            }
            //删除其余不用的数据
            for (int i = sheet.LastRowNum; i >= datas.Count + HEAD_ROW_COUNT; i--)
            {
                IRow row = sheet.GetRow(i);
                if (row != null)
                    sheet.RemoveRow(row);
            }
            return true;
        }
    }
}
